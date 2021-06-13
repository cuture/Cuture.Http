using System;
using System.Buffers;
using System.Buffers.Text;
using System.Runtime.CompilerServices;
using System.Text;

using static Cuture.Http.Internal.HttpRawDataReadTool;

namespace Cuture.Http
{
    /// <summary>
    /// 请求构建工具
    /// </summary>
    public static class RequestBuildTool
    {
        #region Public 方法

        /// <inheritdoc cref="FromRaw(ReadOnlyMemory{byte}, IMemoryOwner{byte}?)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpRequest FromRaw(string rawBase64String)
        {
            var length = Base64.GetMaxDecodedFromUtf8Length(rawBase64String.Length);
            var memoryOwner = MemoryPool<byte>.Shared.Rent(length);

            if (Convert.TryFromBase64String(rawBase64String, memoryOwner.Memory.Span, out var bytesWritten))
            {
                try
                {
                    return FromRaw(memoryOwner.Memory.Slice(0, bytesWritten), memoryOwner);
                }
                catch
                {
                    memoryOwner.Dispose();
                    throw;
                }
            }

            memoryOwner.Dispose();

            throw new ArgumentException($"无法正确解析base64数据", nameof(rawBase64String));
        }

        /// <inheritdoc cref="FromRaw(ReadOnlyMemory{byte}, IMemoryOwner{byte}?)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpRequest FromRaw(byte[] data) => FromRaw(data.AsMemory(), null);

        /// <summary>
        /// 从请求的原始数据构建请求
        /// </summary>
        /// <param name="data">请求的原始数据</param>
        /// <returns></returns>
        public static IHttpRequest FromRaw(ReadOnlySpan<byte> data) => FromRaw(data.ToArray().AsMemory(), null);

        /// <summary>
        /// 从请求的原始数据构建请求
        /// </summary>
        /// <param name="memory">请求的原始数据</param>
        /// <param name="memoryOwner">数据所有者（如果数据是 <see cref="IMemoryOwner{T}"/> 的切片的话），在请求使用结束后归还内存</param>
        /// <returns></returns>
        public static IHttpRequest FromRaw(ReadOnlyMemory<byte> memory, IMemoryOwner<byte>? memoryOwner = null)
        {
            var data = memory.Span;

            var totalLength = data.Length;

            //HACK 考虑支持 Version
            ReadHttpRequestLine(ref data, out var methodSpan, out var urlSpan, out _);

            var encoding = Encoding.UTF8;

            var url = encoding.GetString(urlSpan);
            var request = url.CreateHttpRequest()
                             .UseVerb(encoding.GetString(methodSpan));

            var (_, contentType) = request.LoadHeaders(ref data, encoding);

            if (data.Length > 0)
            {
                return request.WithContent(memory[(totalLength - data.Length)..], contentType, memoryOwner);
            }

            memoryOwner?.Dispose();

            return request;
        }

        #endregion Public 方法

        #region Internal 方法

        /// <summary>
        /// 加载header到指定<see cref="System.Net.Http.Headers.HttpHeaders"/>，并返回contentLength和contentType
        /// <para/>
        /// 当headers为空时不进行加载，但仍然会读取内容
        /// </summary>
        /// <param name="data"></param>
        /// <param name="headers"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        internal static (int contentLength, string contentType) LoadHeaders(ref ReadOnlySpan<byte> data, System.Net.Http.Headers.HttpHeaders? headers, Encoding? encoding)
        {
            var newLineSeparator = NewLineSeparatorSpan;
            var contentLength = -1;
            var contentType = string.Empty;
            encoding ??= Encoding.UTF8;

            while (data.Length > 0)
            {
                var headerSpan = ReadSegmentData(ref data, newLineSeparator);

                //连续两次换行 - Header结束
                if (headerSpan.Length == 0)
                {
                    break;
                }

                var keySpan = ReadSegmentData(ref headerSpan, ColonSeparator);

                var key = encoding.GetString(keySpan);

                switch (key)
                {
                    case HttpHeaderDefinitions.Host:  //忽略Host
                        continue;
                    case HttpHeaderDefinitions.ContentType:
                        contentType = GetHeaderValue(encoding, headerSpan);
                        continue;
                    case HttpHeaderDefinitions.ContentLength:
                        contentLength = int.Parse(GetHeaderValue(encoding, headerSpan), System.Globalization.CultureInfo.InvariantCulture);
                        continue;
                }
                headers?.TryAddWithoutValidation(key, GetHeaderValue(encoding, headerSpan));
            }

            return (contentLength, contentType);
        }

        /// <summary>
        /// 读取请求行
        /// </summary>
        /// <param name="data"></param>
        /// <param name="method"></param>
        /// <param name="url"></param>
        /// <param name="version"></param>
        internal static void ReadHttpRequestLine(ref ReadOnlySpan<byte> data, out ReadOnlySpan<byte> method, out ReadOnlySpan<byte> url, out ReadOnlySpan<byte> version)
        {
            method = ReadSegmentData(ref data, SpaceSeparator);
            if (method.IsEmpty)
            {
                throw new ArgumentException("not found “method” in data. Please check the raw data.");
            }

            url = ReadSegmentData(ref data, SpaceSeparator);
            if (url.IsEmpty)
            {
                throw new ArgumentException("not found “url” in data. Please check the raw data.");
            }

            version = ReadSegmentData(ref data, NewLineSeparatorSpan);
            if (version.IsEmpty)
            {
                throw new ArgumentException("not found “version” in data. Please check the raw data.");
            }
        }

        #endregion Internal 方法
    }
}