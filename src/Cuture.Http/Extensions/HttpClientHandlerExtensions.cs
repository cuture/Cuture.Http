using System.Runtime.CompilerServices;

namespace Cuture.Http;

/// <summary>
/// HttpClientHandler拓展
/// </summary>
public static class HttpClientHandlerExtensions
{
    #region 方法

    /// <summary>
    /// 禁用 <paramref name="handler"/> 的Proxy
    /// </summary>
    /// <param name="handler"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HttpClientHandler DisableProxy(this HttpClientHandler handler)
    {
        handler.UseProxy = false;
        handler.Proxy = null;
        return handler;
    }

    #endregion 方法
}
