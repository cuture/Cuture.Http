using System.Net;
using System.Runtime.CompilerServices;

namespace Cuture.Http;

internal static class HttpClientUtil
{
    #region Public 方法

    /// <summary>
    /// 创建默认的 <see cref="HttpClientHandler"/>
    /// <para/><see cref="HttpClientHandler.UseProxy"/> = true
    /// <para/><see cref="HttpClientHandler.UseCookies"/> = false
    /// <para/><see cref="HttpClientHandler.AllowAutoRedirect"/> = false
    /// <para/><see cref="HttpClientHandler.AutomaticDecompression"/> = <see cref="DecompressionMethods.All"/>
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HttpClientHandler CreateDefaultClientHandler()
    {
        return new()
        {
            UseProxy = true,
            UseCookies = false,
            AllowAutoRedirect = false,
            AutomaticDecompression = DecompressionMethods.All,
        };
    }

    /// <summary>
    /// 创建默认的不使用代理 <see cref="HttpClientHandler"/>
    /// <para/><see cref="HttpClientHandler.UseProxy"/> = false
    /// <para/><see cref="HttpClientHandler.UseCookies"/> = false
    /// <para/><see cref="HttpClientHandler.AllowAutoRedirect"/> = false
    /// <para/><see cref="HttpClientHandler.AutomaticDecompression"/> = <see cref="DecompressionMethods.All"/>
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HttpClientHandler CreateNoProxyClientHandler()
    {
        return new()
        {
            UseProxy = false,
            UseCookies = false,
            AllowAutoRedirect = false,
            AutomaticDecompression = DecompressionMethods.All,
        };
    }

    /// <summary>
    /// 创建使用代理的 <see cref="HttpClientHandler"/>
    /// <para/><see cref="HttpClientHandler.UseProxy"/> = true
    /// <para/><see cref="HttpClientHandler.UseCookies"/> = false
    /// <para/><see cref="HttpClientHandler.AllowAutoRedirect"/> = false
    /// <para/><see cref="HttpClientHandler.AutomaticDecompression"/> = <see cref="DecompressionMethods.All"/>
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HttpClientHandler CreateProxyedClientHandler(IWebProxy webProxy)
    {
        return new()
        {
            UseProxy = true,
            Proxy = webProxy ?? throw new ArgumentNullException(nameof(webProxy)),
            UseCookies = false,
            AllowAutoRedirect = false,
            AutomaticDecompression = DecompressionMethods.All,
        };
    }

    #endregion Public 方法
}
