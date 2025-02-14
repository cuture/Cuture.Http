#pragma warning disable IDE0130

using System.Runtime.CompilerServices;
using System.Text;
using System.Web;

namespace Cuture.Http;

/// <summary>
/// 编码解码拓展
/// </summary>
public static class CodecExtension
{
    #region Url

    /// <summary>
    /// 使用<see cref="Encoding.UTF8"/>进行UrlDecode
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string UrlDecode(this string content)
    {
        if (string.IsNullOrEmpty(content))
        {
            return content;
        }
        return HttpUtility.UrlDecode(content, Encoding.UTF8);
    }

    /// <summary>
    /// 指定编码
    /// <paramref name="encoding"/>
    /// 进行UrlDecode
    /// </summary>
    /// <param name="content"></param>
    /// <param name="encoding"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string UrlDecode(this string content, Encoding encoding)
    {
        if (string.IsNullOrEmpty(content))
        {
            return content;
        }
        return HttpUtility.UrlDecode(content, encoding);
    }

    /// <summary>
    /// 使用<see cref="Encoding.UTF8"/>进行UrlEncode
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string UrlEncode(this string content)
    {
        if (string.IsNullOrEmpty(content))
        {
            return content;
        }
        return HttpUtility.UrlEncode(content, Encoding.UTF8);
    }

    /// <summary>
    /// 指定编码
    /// <paramref name="encoding"/>
    /// 进行UrlEncode
    /// </summary>
    /// <param name="content"></param>
    /// <param name="encoding"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string UrlEncode(this string content, Encoding encoding)
    {
        if (string.IsNullOrEmpty(content))
        {
            return content;
        }
        return HttpUtility.UrlEncode(content, encoding);
    }

    #endregion Url

    #region Base64

    /// <summary>
    /// 使用
    /// <see cref="Encoding.UTF8"/>
    /// 进行解码Base64字符串
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string DecodeBase64(this string content) => DecodeBase64(content, Encoding.UTF8);

    /// <summary>
    /// 使用指定编码
    /// <paramref name="encoding"/>
    /// 进行解码Base64字符串
    /// </summary>
    /// <param name="content"></param>
    /// <param name="encoding"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string DecodeBase64(this string content, Encoding encoding)
    {
        if (string.IsNullOrEmpty(content))
        {
            return content;
        }
        var bytes = Convert.FromBase64String(content);
        return encoding.GetString(bytes);
    }

    /// <summary>
    /// 使用
    /// <see cref="Encoding.UTF8"/>
    /// 编码为Base64字符串
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string EncodeBase64(this string content) => EncodeBase64(content, Encoding.UTF8);

    /// <summary>
    /// 使用指定编码
    /// <paramref name="encoding"/>
    /// 编码为Base64字符串
    /// </summary>
    /// <param name="content"></param>
    /// <param name="encoding"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string EncodeBase64(this string content, Encoding encoding)
    {
        if (string.IsNullOrEmpty(content))
        {
            return content;
        }

        var bytes = encoding.GetBytes(content);
        return Convert.ToBase64String(bytes);
    }

    #endregion Base64
}
