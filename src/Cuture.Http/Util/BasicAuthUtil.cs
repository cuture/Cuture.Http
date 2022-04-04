using System;
using System.Buffers;
using System.Buffers.Text;
using System.Text;

namespace Cuture.Http.Util;

/// <summary>
/// 基础认证工具
/// </summary>
public static class BasicAuthUtil
{
    #region Public 字段

    /// <summary>
    /// <see cref="HttpHeaderDefinitions.Authorization"/>
    /// </summary>
    public const string HttpHeader = HttpHeaderDefinitions.Authorization;

    #endregion Public 字段

    #region Public 方法

    /// <summary>
    /// base64编码用户名密码
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="password"></param>
    /// <param name="encoding"></param>
    /// <returns></returns>
    public static string Encode(string userName, string password, Encoding? encoding = null)
        => $"{userName}:{password}".EncodeBase64(encoding ?? Encoding.UTF8);

    /// <summary>
    /// 编码为HttpHeader的值（附加Basic到字符串头部）
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="password"></param>
    /// <param name="encoding"></param>
    /// <returns></returns>
    public static string EncodeToHeader(string userName, string password, Encoding? encoding = null)
        => $"Basic {Encode(userName, password, encoding)}";

    /// <summary>
    /// 尝试解码BasicAuth的值
    /// </summary>
    /// <param name="value"></param>
    /// <param name="userName"></param>
    /// <param name="password"></param>
    /// <param name="encoding"></param>
    /// <returns></returns>
    public static bool TryDecode(string value, out string? userName, out string? password, Encoding? encoding = null)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            userName = null;
            password = null;
            return false;
        }

        var span = value.AsSpan();

        var splitIndex = span.IndexOf(' ');

        if (splitIndex > 0) //移除头部的 Basic
        {
            span = span.Slice(splitIndex + 1, value.Length - splitIndex - 1);
        }

        var buffer = ArrayPool<byte>.Shared.Rent(Base64.GetMaxDecodedFromUtf8Length(span.Length));
        Span<byte> bufferSpan = buffer;

        try
        {
            if (!Convert.TryFromBase64Chars(span, bufferSpan, out var bytesWritten))
            {
                userName = null;
                password = null;
                return false;
            }

            splitIndex = bufferSpan.IndexOf((byte)':');

            if (splitIndex > 0)
            {
                encoding ??= Encoding.UTF8;
                userName = encoding.GetString(bufferSpan[..splitIndex]);
                password = encoding.GetString(bufferSpan.Slice(splitIndex + 1, bytesWritten - splitIndex - 1));
                return true;
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }

        userName = null;
        password = null;

        return false;
    }

    #endregion Public 方法
}