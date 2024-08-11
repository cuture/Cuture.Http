using System.Runtime.CompilerServices;

using Cuture.Http.Util;

namespace Cuture.Http;

//此文件主要包含 Header 相关的拓展方法

public static partial class IHttpRequestBuildExtensions
{
    #region HeaderSetting

    #region Accept

    /// <summary>
    /// 设置<see cref="HttpHeaderDefinitions.Accept"/>头
    /// </summary>
    /// <param name="request"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IHttpRequest Accept(this IHttpRequest request, string type)
    {
        if (string.IsNullOrWhiteSpace(type))
        {
            throw new ArgumentException($"“{nameof(type)}”不能为 null 或空白。", nameof(type));
        }

        return request.AddNewHeader(HttpHeaderDefinitions.Accept, type);
    }

    /// <summary>
    /// 设置<see cref="HttpHeaderDefinitions.Accept"/>头的值为<see cref="JsonContent.ContentType"/>
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IHttpRequest AcceptJson(this IHttpRequest request)
    {
        return request.AddNewHeader(HttpHeaderDefinitions.Accept, JsonContent.ContentType);
    }

    #endregion Accept

    /// <summary>
    /// 添加Header
    /// </summary>
    /// <param name="request"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IHttpRequest AddHeader(this IHttpRequest request, string key, string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return request.RemoveHeader(key);
        }
        request.Headers.Add(key, value);
        return request;
    }

    /// <summary>
    /// 添加Header
    /// </summary>
    /// <param name="request"></param>
    /// <param name="key"></param>
    /// <param name="values"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IHttpRequest AddHeader(this IHttpRequest request, string key, IEnumerable<string> values)
    {
        if (values is null || !values.Any())
        {
            request.Headers.Remove(key);
        }
        else
        {
            request.Headers.Add(key, values);
        }
        return request;
    }

    /// <summary>
    /// 添加Header集合
    /// </summary>
    /// <param name="request"></param>
    /// <param name="headers"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IHttpRequest AddHeaders(this IHttpRequest request, IEnumerable<KeyValuePair<string, string>> headers)
    {
        foreach (var header in headers)
        {
            request.AddHeader(header.Key, header.Value);
        }
        return request;
    }

    /// <summary>
    /// 添加Header且不验证其内容
    /// </summary>
    /// <param name="request"></param>
    /// <param name="headers"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IHttpRequest AddHeadersWithoutValidation(this IHttpRequest request, IEnumerable<KeyValuePair<string, string>> headers)
    {
        foreach (var header in headers)
        {
            request.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }
        return request;
    }

    /// <summary>
    /// 添加Header且不验证其内容
    /// </summary>
    /// <param name="request"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IHttpRequest AddHeaderWithoutValidation(this IHttpRequest request, string key, string value)
    {
        request.Headers.TryAddWithoutValidation(key, value);
        return request;
    }

    /// <summary>
    /// 如果存在Header则将其移除,并添加Header
    /// </summary>
    /// <param name="request"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IHttpRequest AddNewHeader(this IHttpRequest request, string key, string value)
    {
        request.RemoveHeader(key);
        request.AddHeader(key, value);
        return request;
    }

    /// <summary>
    /// 移除Header
    /// </summary>
    /// <param name="request"></param>
    /// <param name="key"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IHttpRequest RemoveHeader(this IHttpRequest request, string key)
    {
        request.Headers.Remove(key);
        return request;
    }

    /// <summary>
    /// 使用BearerToken<para/>
    /// 添加 Authorization: Bearer token 到header中
    /// </summary>
    /// <param name="request"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IHttpRequest UseBearerToken(this IHttpRequest request, string token)
    {
        request.AddNewHeader(HttpHeaderDefinitions.Authorization, $"Bearer {token}");
        return request;
    }

    /// <summary>
    /// 使用Cookie
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cookie"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IHttpRequest UseCookie(this IHttpRequest request, string cookie)
    {
        request.AddHeader(HttpHeaderDefinitions.Cookie, cookie);
        return request;
    }

    /// <summary>
    /// 使用Referer
    /// </summary>
    /// <param name="request"></param>
    /// <param name="referer"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IHttpRequest UseReferer(this IHttpRequest request, string referer)
    {
        request.AddHeader(HttpHeaderDefinitions.Referer, referer);
        return request;
    }

    /// <summary>
    /// 使用UserAgent
    /// </summary>
    /// <param name="request"></param>
    /// <param name="userAgent"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IHttpRequest UseUserAgent(this IHttpRequest request, string userAgent)
    {
        request.AddHeader(HttpHeaderDefinitions.UserAgent, userAgent);
        return request;
    }

    /// <summary>
    /// 使用基础认证
    /// </summary>
    /// <param name="request"></param>
    /// <param name="userName"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IHttpRequest WithBasicAuth(this IHttpRequest request, string userName, string password)
    {
        request.AddHeader(BasicAuthUtil.HttpHeader, BasicAuthUtil.EncodeToHeader(userName, password));
        return request;
    }

    #endregion HeaderSetting
}
