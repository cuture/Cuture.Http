using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Cuture.Http;

/// <summary>
/// 伪造Header的拓展类
/// </summary>
public static class HeadersExtension
{
    #region 方法

    /// <summary>
    /// 获取一个随机的IP地址
    /// </summary>
    /// <returns></returns>
    public static string GetRandomIpAddress()
    {
        var sb = new StringBuilder();

        var random = Random.Shared;
        sb.Append(random.Next(11, 240));
        sb.Append('.');
        sb.Append(random.Next(1, 250));
        sb.Append('.');
        sb.Append(random.Next(1, 240));
        sb.Append('.');
        sb.Append(random.Next(1, 240));
        return sb.ToString();
    }

    /// <summary>
    /// 使用随机的X-Forwarded-For标头
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IHttpRequest RandomXFowardFor(this IHttpRequest request)
    {
        request.AddHeader("X-Forwarded-For", GetRandomIpAddress());
        return request;
    }

    #endregion 方法
}
