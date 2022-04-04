using System;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;

namespace Cuture.Http;

internal static class HttpClientUtil
{
    #region Public 方法

    /// <summary>
    /// 创建默认的<see cref="HttpClient"/>
    /// <para/><see cref="HttpClientHandler.UseProxy"/> = true
    /// <para/><see cref="HttpClientHandler.UseCookies"/> = false
    /// <para/><see cref="HttpClientHandler.AllowAutoRedirect"/> = false
    /// <para/><see cref="HttpClientHandler.AutomaticDecompression"/> = <see cref="DecompressionMethods.GZip"/> | <see cref="DecompressionMethods.Deflate"/>
    /// </summary>
    /// <returns></returns>
    public static HttpClient CreateDefaultClient()
    {
        return new FinalizeableHttpClient(CreateDefaultClientHandler());
    }

    /// <summary>
    /// 创建默认的 <see cref="HttpClientHandler"/>
    /// <para/><see cref="HttpClientHandler.UseProxy"/> = true
    /// <para/><see cref="HttpClientHandler.UseCookies"/> = false
    /// <para/><see cref="HttpClientHandler.AllowAutoRedirect"/> = false
    /// <para/><see cref="HttpClientHandler.AutomaticDecompression"/> = <see cref="DecompressionMethods.GZip"/> | <see cref="DecompressionMethods.Deflate"/>
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HttpClientHandler CreateDefaultClientHandler()
    {
        return new HttpClientHandler()
        {
            UseProxy = true,
            UseCookies = false,
            AllowAutoRedirect = false,
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
        };
    }

    /// <summary>
    /// 创建禁用代理的默认<see cref="HttpClient"/>
    /// <para/>
    /// <para/><see cref="HttpClientHandler.UseProxy"/> = false
    /// <para/><see cref="HttpClientHandler.Proxy"/> = null
    /// <para/><see cref="HttpClientHandler.UseCookies"/> = false
    /// <para/><see cref="HttpClientHandler.AllowAutoRedirect"/> = false
    /// <para/><see cref="HttpClientHandler.AutomaticDecompression"/> = <see cref="DecompressionMethods.GZip"/> | <see cref="DecompressionMethods.Deflate"/>
    /// </summary>
    /// <returns></returns>
    public static HttpClient CreateProxyDisabledDefaultClient()
    {
        return new FinalizeableHttpClient(CreateDefaultClientHandler().DisableProxy());
    }

    /// <summary>
    /// 创建使用代理的<see cref="HttpClient"/>
    /// <para/><see cref="HttpClientHandler.UseProxy"/> = true
    /// <para/><see cref="HttpClientHandler.UseCookies"/> = false
    /// <para/><see cref="HttpClientHandler.AllowAutoRedirect"/> = false
    /// <para/><see cref="HttpClientHandler.AutomaticDecompression"/> = <see cref="DecompressionMethods.GZip"/> | <see cref="DecompressionMethods.Deflate"/>
    /// </summary>
    /// <returns></returns>
    public static HttpClient CreateProxyedClient(IWebProxy webProxy)
    {
        if (webProxy is null)
        {
            throw new ArgumentNullException(nameof(webProxy));
        }

        var httpClientHandler = CreateDefaultClientHandler();
        httpClientHandler.UseProxy = true;
        httpClientHandler.Proxy = webProxy;
        return new FinalizeableHttpClient(httpClientHandler);
    }

    #endregion Public 方法
}
