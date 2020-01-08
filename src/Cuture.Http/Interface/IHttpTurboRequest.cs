using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Cuture.Http
{
    /// <summary>
    /// http请求
    /// </summary>
    public interface IHttpTurboRequest
    {
        #region 属性

        /// <summary>
        /// 允许重定向
        /// </summary>
        bool AllowRedirection { get; set; }

        /// <summary>
        /// HttpContent
        /// </summary>
        HttpContent Content { get; set; }

        /// <summary>
        /// 禁用Proxy
        /// </summary>
        bool DisableProxy { get; set; }

        /// <summary>
        /// Headers
        /// </summary>
        HttpRequestHeaders Headers { get; }

        /// <summary>
        /// http方法
        /// </summary>
        HttpMethod Method { get; set; }

        /// <summary>
        /// Web代理
        /// </summary>
        IWebProxy Proxy { get; set; }

        /// <summary>
        /// 请求的Uri
        /// </summary>
        Uri RequestUri { get; }

        /// <summary>
        /// 超时时间
        /// </summary>
        int? Timeout { get; set; }

        /// <summary>
        /// 取消标记
        /// </summary>
        CancellationToken Token { get; set; }

        /// <summary>
        /// 用于请求的HttpTurbo
        /// </summary>
        IHttpTurboClient TurboClient { get; set; }

        /// <summary>
        /// 用于请求的HttpTurboClientFactory
        /// </summary>
        IHttpTurboClientFactory TurboClientFactory { get; set; }

        #endregion 属性

        #region 方法

        /// <summary>
        /// 添加Header
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IHttpTurboRequest AddHeader(string key, string value);

        /// <summary>
        /// 添加Header
        /// </summary>
        /// <param name="key"></param>
        /// <param name="values"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IHttpTurboRequest AddHeader(string key, IEnumerable<string> values);

        /// <summary>
        /// 获取HttpRequestMessage
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        HttpRequestMessage AsRequest();

        /// <summary>
        /// 移除Header
        /// </summary>
        /// <param name="key"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void RemoveHeader(string key);

        #endregion 方法
    }
}