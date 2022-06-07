using System;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Cuture.Http;

//此文件主要包含 请求选项 相关的拓展方法

public static partial class IHttpRequestBuildExtensions
{
    #region Options

    #region Proxy

    /// <summary>
    /// 禁用系统代理
    /// <para/>设置 <see cref="IHttpRequest.DisableProxy"/> 为 true
    /// <para/>默认实现下, 将不使用任何代理进行请求
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IHttpRequest DisableProxy(this IHttpRequest request)
    {
        request.DisableProxy = true;
        request.Proxy = null;
        return request;
    }

    /// <summary>
    /// 使用默认Web代理（理论上默认情况下就是这种状态）
    /// <para/>设置 <see cref="IHttpRequest.Proxy"/> 为 <see cref="WebRequest.DefaultWebProxy"/>
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IHttpRequest UseDefaultWebProxy(this IHttpRequest request)
    {
        request.DisableProxy = false;
        request.Proxy = WebRequest.DefaultWebProxy;
        return request;
    }

    /// <summary>
    /// 使用指定的代理
    /// </summary>
    /// <param name="request"></param>
    /// <param name="webProxy"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IHttpRequest UseProxy(this IHttpRequest request, IWebProxy webProxy)
    {
        request.Proxy = webProxy;
        return request;
    }

    /// <summary>
    /// 使用指定的代理
    /// </summary>
    /// <param name="request"></param>
    /// <param name="proxyAddress">代理地址</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IHttpRequest UseProxy(this IHttpRequest request, string proxyAddress)
    {
        request.Proxy = string.IsNullOrEmpty(proxyAddress) ? null : new WebProxy(proxyAddress);
        return request;
    }

    /// <summary>
    /// 使用指定的代理
    /// </summary>
    /// <param name="request"></param>
    /// <param name="proxyUri">代理地址</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IHttpRequest UseProxy(this IHttpRequest request, Uri proxyUri)
    {
        request.Proxy = proxyUri is null ? null : new WebProxy(proxyUri);
        return request;
    }

    /// <summary>
    /// 使用系统代理
    /// <para/>设置 <see cref="IHttpRequest.Proxy"/> 为 <see cref="WebRequest.GetSystemWebProxy()"/>
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IHttpRequest UseSystemProxy(this IHttpRequest request)
    {
        request.DisableProxy = false;
        request.Proxy = WebRequest.GetSystemWebProxy();
        return request;
    }

    #endregion Proxy

    /// <summary>
    /// 设置是否允许自动重定向
    /// </summary>
    /// <param name="request"></param>
    /// <param name="allowRedirection"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IHttpRequest AutoRedirection(this IHttpRequest request, bool allowRedirection = true)
    {
        request.AllowRedirection = allowRedirection;
        return request;
    }

    /// <summary>
    /// 配置请求
    /// </summary>
    /// <param name="request"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static IHttpRequest Configure(this IHttpRequest request, Action<IHttpRequest> action)
    {
        action.Invoke(request);
        return request;
    }

    /// <summary>
    /// 设置最大重定向次数
    /// </summary>
    /// <param name="request"></param>
    /// <param name="maxAutomaticRedirections"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IHttpRequest MaxAutoRedirections(this IHttpRequest request, int maxAutomaticRedirections)
    {
        request.MaxAutomaticRedirections = maxAutomaticRedirections;
        return request;
    }

    /// <summary>
    /// 超时时间
    /// </summary>
    /// <param name="request"></param>
    /// <param name="milliseconds"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IHttpRequest TimeOut(this IHttpRequest request, int milliseconds)
    {
        request.Timeout = milliseconds;
        return request;
    }

    /// <summary>
    /// 超时时间
    /// </summary>
    /// <param name="request"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IHttpRequest TimeOut(this IHttpRequest request, TimeSpan timeout)
    {
        long num = (long)timeout.TotalMilliseconds;
        if (num < -1 || num > int.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(timeout));
        }
        request.Timeout = (int)num;
        return request;
    }

    #region RequestOptions

    /// <summary>
    /// 使用指定的 <see cref="HttpMessageInvoker"/>
    /// </summary>
    /// <param name="request"></param>
    /// <param name="httpMessageInvoker"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IHttpRequest UseClient(this IHttpRequest request, HttpMessageInvoker httpMessageInvoker)
    {
        request.ExecutionOptions.MessageInvoker = httpMessageInvoker;
        return request;
    }

    /// <summary>
    /// 使用指定的<inheritdoc cref="IHttpMessageInvokerPool"/>
    /// </summary>
    /// <param name="request"></param>
    /// <param name="messageInvokerFactory"></param>
    /// <returns></returns>
    [Obsolete("Use UseInvokerPool instead.", true)]
    public static IHttpRequest UseInvokerFactory(this IHttpRequest request, IHttpMessageInvokerPool messageInvokerFactory)
    {
        request.ExecutionOptions.MessageInvokerPool = messageInvokerFactory;
        return request;
    }

    /// <summary>
    /// 使用指定的<inheritdoc cref="IHttpMessageInvokerPool"/>
    /// </summary>
    /// <param name="request"></param>
    /// <param name="messageInvokerPool"></param>
    /// <returns></returns>
    public static IHttpRequest UseInvokerPool(this IHttpRequest request, IHttpMessageInvokerPool messageInvokerPool)
    {
        request.ExecutionOptions.MessageInvokerPool = messageInvokerPool;
        return request;
    }

    /// <summary>
    /// 使用指定的Json序列化器
    /// </summary>
    /// <param name="request"></param>
    /// <param name="jsonSerializer"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IHttpRequest UseJsonSerializer(this IHttpRequest request, IJsonSerializer jsonSerializer)
    {
        request.ExecutionOptions.JsonSerializer = jsonSerializer;
        return request;
    }

    /// <summary>
    /// 使用指定的请求选项（将会覆盖之前的相关选项设置，应在构建请求的早期进行设置）
    /// </summary>
    /// <param name="request"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IHttpRequest WithOption(this IHttpRequest request, HttpRequestExecutionOptions options)
    {
        request.ExecutionOptions = options;
        return request;
    }

    #endregion RequestOptions

    /// <summary>
    /// 使用取消标记
    /// </summary>
    /// <param name="request"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IHttpRequest WithCancellation(this IHttpRequest request, CancellationToken token)
    {
        request.Token = token;
        return request;
    }

    #endregion Options
}
