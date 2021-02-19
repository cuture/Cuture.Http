using System;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Cuture.Http
{
    /// <summary>
    /// <see cref="IHttpTurboRequest"/> 请求拓展类
    /// </summary>
    public static partial class IHttpTurboRequestExtension
    {
        #region Options

        #region Proxy

        /// <summary>
        /// 禁用系统代理
        /// <para/>设置 <see cref="IHttpTurboRequest.DisableProxy"/> 为 true
        /// <para/>默认实现下, 将不使用任何代理进行请求
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpTurboRequest DisableProxy(this IHttpTurboRequest request)
        {
            request.DisableProxy = true;
            request.Proxy = null;
            return request;
        }

        /// <summary>
        /// 使用默认Web代理（理论上默认情况下就是这种状态）
        /// <para/>设置 <see cref="IHttpTurboRequest.Proxy"/> 为 <see cref="WebRequest.DefaultWebProxy"/>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpTurboRequest UseDefaultWebProxy(this IHttpTurboRequest request)
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
        public static IHttpTurboRequest UseProxy(this IHttpTurboRequest request, IWebProxy webProxy)
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
        public static IHttpTurboRequest UseProxy(this IHttpTurboRequest request, string proxyAddress)
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
        public static IHttpTurboRequest UseProxy(this IHttpTurboRequest request, Uri proxyUri)
        {
            request.Proxy = proxyUri is null ? null : new WebProxy(proxyUri);
            return request;
        }

        /// <summary>
        /// 使用系统代理
        /// <para/>设置 <see cref="IHttpTurboRequest.Proxy"/> 为 <see cref="WebRequest.GetSystemWebProxy()"/>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpTurboRequest UseSystemProxy(this IHttpTurboRequest request)
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
        public static IHttpTurboRequest AutoRedirection(this IHttpTurboRequest request, bool allowRedirection = true)
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
        public static IHttpTurboRequest Configure(this IHttpTurboRequest request, Action<IHttpTurboRequest> action)
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
        public static IHttpTurboRequest MaxAutoRedirections(this IHttpTurboRequest request, int maxAutomaticRedirections)
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
        public static IHttpTurboRequest TimeOut(this IHttpTurboRequest request, int milliseconds)
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
        public static IHttpTurboRequest TimeOut(this IHttpTurboRequest request, TimeSpan timeout)
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
        public static IHttpTurboRequest UseClient(this IHttpTurboRequest request, HttpMessageInvoker httpMessageInvoker)
        {
            request.RequestOptions.MessageInvoker = httpMessageInvoker;
            return request;
        }

        /// <summary>
        /// 使用指定的HttpTurboClient
        /// </summary>
        /// <param name="request"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpTurboRequest UseClient(this IHttpTurboRequest request, IHttpTurboClient client)
        {
            request.RequestOptions.TurboClient = client;
            return request;
        }

        /// <summary>
        /// 使用指定的Json序列化器
        /// </summary>
        /// <param name="request"></param>
        /// <param name="jsonSerializer"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpTurboRequest UseJsonSerializer(this IHttpTurboRequest request, IJsonSerializer jsonSerializer)
        {
            request.RequestOptions.JsonSerializer = jsonSerializer;
            return request;
        }

        /// <summary>
        /// 使用指定的TurboClientFactory
        /// </summary>
        /// <param name="request"></param>
        /// <param name="turboClientFactory"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpTurboRequest UseTurboClientFactory(this IHttpTurboRequest request, IHttpTurboClientFactory turboClientFactory)
        {
            request.RequestOptions.TurboClientFactory = turboClientFactory;
            return request;
        }

        /// <summary>
        /// 使用指定的请求选项（将会覆盖之前的相关选项设置，应在构建请求的早期进行设置）
        /// </summary>
        /// <param name="request"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpTurboRequest WithOption(this IHttpTurboRequest request, HttpRequestOptions options)
        {
            request.RequestOptions = options;
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
        public static IHttpTurboRequest WithCancellation(this IHttpTurboRequest request, CancellationToken token)
        {
            request.Token = token;
            return request;
        }

        #endregion Options
    }
}