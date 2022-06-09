using System;
using System.Runtime.CompilerServices;

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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetRandomIpAddress()
    {
        var random = Random.Shared;
        return $"{random.Next(11, 240)}.{random.Next(1, 250)}.{random.Next(1, 240)}.{random.Next(1, 240)}";
    }

    /// <summary>
    /// 使用随机的X-Forwarded-For标头
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IHttpRequest RandomXFowardFor(this IHttpRequest request)
    {
        request.Headers.TryAddWithoutValidation("X-Forwarded-For", GetRandomIpAddress());
        return request;
    }

    #endregion 方法
}
