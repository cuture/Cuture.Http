#pragma warning disable IDE0130

using System.Buffers;
using System.Collections.Frozen;
using System.Runtime.CompilerServices;
using System.Text;
using Cuture.Http.Internal;

namespace Cuture.Http;

/// <summary>
/// 请求构建工具
/// </summary>
public static partial class RequestBuildTool
{
    #region CurlOptionAction

    private readonly record struct OrderedOptionDescriptor(int Order, RequestSetupDelegate RequestSetupDelegate, bool IsNoArgumentOption = false);

    private delegate void RequestSetupDelegate(IHttpRequest request, in CurlCommandToken token);

    private static readonly FrozenDictionary<ReadOnlyMemory<char>, OrderedOptionDescriptor> s_curlOptionDescriptors = new Dictionary<ReadOnlyMemory<char>, OrderedOptionDescriptor>()
    {
        { "-X".AsMemory(), new (0, static (IHttpRequest request, in CurlCommandToken token) => request.UseVerb(token.Value.Span)) },
        { "-H".AsMemory(), new(0, SetupRequestHeader) },
        { "--data-raw".AsMemory(), new(100, SetupRequestContentData) },
        { "--compressed".AsMemory(), new(0, NoopRequestSetupDelegate, true) },  //已强制请求需要压缩
    }.ToFrozenDictionary(OrdinalReadOnlyMemoryCharComparer.Shared);

    private static void NoopRequestSetupDelegate(IHttpRequest request, in CurlCommandToken token)
    { }

    private static void SetupRequestContentData(IHttpRequest request, in CurlCommandToken token)
    {
        request.UsePost();
        var bufferOwner = MemoryPool<byte>.Shared.Rent(Encoding.UTF8.GetMaxByteCount(token.Value.Length));
        try
        {
            var length = Encoding.UTF8.GetBytes(token.Value.Span, bufferOwner.Memory.Span);
            var contentType = request.Options.FirstOrDefault(static m => string.Equals("Content-Type", m.Key, StringComparison.OrdinalIgnoreCase)).Value as string;
            request.WithContent(bufferOwner.Memory.Slice(0, length), contentType, bufferOwner);
        }
        catch
        {
            bufferOwner.Dispose();
            throw;
        }
    }

    private static void SetupRequestHeader(IHttpRequest request, in CurlCommandToken token)
    {
        var span = token.Value.Span.Trim();
        var index = span.IndexOf(':');
        if (index == -1)
        {
            throw new ArgumentException($"Invalid header {token}");
        }
        var key = span.Slice(0, index).TrimEnd().ToString();
        var value = span.Slice(index + 1).TrimStart().ToString();
        if (!request.Headers.TryAddWithoutValidation(key, value))
        {
            request.Options.Set(new(key), value);
        }
    }

    #endregion CurlOptionAction

    #region Public 方法

    /// <inheritdoc cref="FromCurl(ReadOnlySpan{char})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IHttpRequest FromCurl(string command) => FromCurl(command.AsSpan());

    /// <inheritdoc cref="FromCurl(ReadOnlySpan{char})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IHttpRequest FromCurl(ReadOnlySpan<char> command)
    {
        var buffer = ArrayPool<char>.Shared.Rent(command.Length);
        try
        {
            command.CopyTo(buffer);
            return FromCurl(buffer.AsMemory(0, command.Length));
        }
        finally
        {
            ArrayPool<char>.Shared.Return(buffer);
        }
    }

    /// <summary>
    /// 从curl指令构建请求
    /// </summary>
    /// <param name="command">完整curl指令 (有限支持的POSIX指令)</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IHttpRequest FromCurl(ReadOnlyMemory<char> command)
    {
        var index = 0;

        var commandSpan = command.Span.Trim();
        while (index < commandSpan.Length && char.IsWhiteSpace(commandSpan[index]))
        {
            index++;
        }

        if (!commandSpan.Slice(index).StartsWith("curl ", StringComparison.Ordinal))
        {
            throw new ArgumentException($"Input curl command \"{command}\" is invalid");
        }

        index += 5;

        var tokensBuffer = ArrayPool<CurlCommandToken>.Shared.Rent(command.Length / 2);
        var tokenCount = 0;

        try
        {
            SkipWhiteSpace(in commandSpan, ref index);
            do
            {
                switch (commandSpan[index])
                {
                    case '-':   //选项
                        {
                            tokensBuffer[tokenCount++] = ReadOptionToken(command, commandSpan, ref index);
                            break;
                        }
                    case '\'':  //数据
                        {
                            tokensBuffer[tokenCount++] = ReadQuotLiteralToken(command, commandSpan, ref index);
                            break;
                        }
                    case '\\':  //换行
                        {
                            SkipNewLine(in commandSpan, ref index);
                            SkipWhiteSpace(in commandSpan, ref index);
                            break;
                        }
                    default:    //无引号数据
                        {
                            tokensBuffer[tokenCount++] = ReadLiteralToken(command, commandSpan, ref index);
                            break;
                        }
                }

                SkipWhiteSpace(in commandSpan, ref index);
            }
            while (index < commandSpan.Length);

            if (tokenCount < 1)
            {
                throw new ArgumentException($"Input curl command \"{command}\" is invalid");
            }

            var tokens = tokensBuffer.AsSpan(0, tokenCount);
            var setupOperateCount = 0;
            using var setupOperateBufferOwner = MemoryPool<SetupOperate>.Shared.Rent(tokenCount);
            var setupOperateBuffer = setupOperateBufferOwner.Memory.Span;

            CurlCommandToken? waitArgumentOptionToken = null;
            IHttpRequest? request = null;

            for (int i = 0; i < tokens.Length; i++)
            {
                var token = tokens[i];

                switch (token.Type)
                {
                    case CurlCommandTokenType.Option:
                        if (waitArgumentOptionToken is null)
                        {
                            waitArgumentOptionToken = token;
                        }
                        else if (s_curlOptionDescriptors.TryGetValue(waitArgumentOptionToken.Value.Value, out var orderedOptionDescriptor)
                                 && orderedOptionDescriptor.IsNoArgumentOption)
                        {
                            setupOperateBuffer[setupOperateCount++] = new(orderedOptionDescriptor, default);
                            waitArgumentOptionToken = token;
                        }
                        else
                        {
                            throw new ArgumentException($"Invalid token {token}");
                        }
                        break;

                    case CurlCommandTokenType.Literal:
                        if (waitArgumentOptionToken is null)
                        {
                            if (request is null)
                            {
                                request = token.Value.ToString().CreateHttpRequest();
                            }
                            else
                            {
                                throw new ArgumentException($"Invalid token {token}");
                            }
                        }
                        else if (s_curlOptionDescriptors.TryGetValue(waitArgumentOptionToken.Value.Value, out var orderedOptionDescriptor))
                        {
                            setupOperateBuffer[setupOperateCount++] = new(orderedOptionDescriptor, token);
                            waitArgumentOptionToken = null;
                        }
                        else
                        {
                            throw new ArgumentException($"Invalid token {token}");
                        }
                        break;

                    case CurlCommandTokenType.Unknown:
                    default:
                        throw new ArgumentException($"Invalid token {token}");
                }
            }

            if (request is null)
            {
                throw new ArgumentException($"Input curl command \"{command}\" is invalid");
            }

            var setupOperates = setupOperateBufferOwner.Memory.Span.Slice(0, setupOperateCount);

            //排序以确保设置content时已经有conten-type
            setupOperates.Sort(static (a, b) => a.OptionDescriptor.Order - b.OptionDescriptor.Order);

            foreach (var (optionDescriptor, token) in setupOperates)
            {
                optionDescriptor.RequestSetupDelegate(request, token);
            }

            return request;
        }
        finally
        {
            ArrayPool<CurlCommandToken>.Shared.Return(tokensBuffer);
        }

        static void SkipWhiteSpace(in ReadOnlySpan<char> commandSpan, ref int index)
        {
            while (index < commandSpan.Length && char.IsWhiteSpace(commandSpan[index]))
            {
                index++;
            }
        }

        static void SkipNewLine(in ReadOnlySpan<char> commandSpan, ref int index)
        {
            var remainSpan = commandSpan.Slice(index);
            if (remainSpan.StartsWith(['\\', '\n'], StringComparison.Ordinal))
            {
                index += 2;
            }
            else if (remainSpan.StartsWith(['\\', '\r', '\n'], StringComparison.Ordinal))
            {
                index += 3;
            }
            else
            {
                throw new ArgumentException($"Unsupported token at {index} - \"{remainSpan}\"");
            }
        }

        static CurlCommandToken ReadOptionToken(in ReadOnlyMemory<char> command, in ReadOnlySpan<char> commandSpan, ref int index)
        {
            var initIndex = index;

            while (index < command.Length && !char.IsWhiteSpace(commandSpan[index]))
            {
                index++;
            }

            return new(CurlCommandTokenType.Option, initIndex, command.Slice(initIndex, index - initIndex));
        }

        static CurlCommandToken ReadLiteralToken(in ReadOnlyMemory<char> command, in ReadOnlySpan<char> commandSpan, ref int index)
        {
            var initIndex = index;

            while (index < commandSpan.Length)
            {
                var current = commandSpan[index];
                if (char.IsWhiteSpace(current)
                    || current == '\\')
                {
                    break;
                }
                index++;
            }

            var length = index - initIndex;

            return new(CurlCommandTokenType.Literal, initIndex, command.Slice(initIndex, length));
        }

        static CurlCommandToken ReadQuotLiteralToken(in ReadOnlyMemory<char> command, in ReadOnlySpan<char> commandSpan, ref int index)
        {
            var initIndex = index;

            //跳过第一个 '
            index += 1;

            while (index < commandSpan.Length - 1
                   && commandSpan[index] != '\''
                   && commandSpan[index + 1] != '\'')
            {
                index++;
            }

            var length = index - initIndex;

            //跳过最后一个 '
            index += 2;

            return new(CurlCommandTokenType.Literal, initIndex, command.Slice(initIndex + 1, length));
        }
    }

    private readonly record struct SetupOperate(OrderedOptionDescriptor OptionDescriptor, CurlCommandToken Token);

    private readonly record struct CurlCommandToken(CurlCommandTokenType Type, int Index, ReadOnlyMemory<char> Value);

    private enum CurlCommandTokenType : byte
    {
        Unknown,

        /// <summary>
        /// 选项
        /// </summary>
        Option,

        /// <summary>
        /// 字面值
        /// </summary>
        Literal,
    }

    #endregion Public 方法
}
