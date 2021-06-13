using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;

namespace Cuture.Http
{
    /// <summary>
    /// http请求
    /// </summary>
    public interface IHttpRequest : IDisposable
    {
        #region 属性

        /// <summary>
        /// 允许重定向
        /// </summary>
        bool AllowRedirection { get; set; }

        /// <summary>
        /// HttpContent
        /// </summary>
        HttpContent? Content { get; set; }

        /// <summary>
        /// 禁用Proxy
        /// </summary>
        bool DisableProxy { get; set; }

        /// <summary>
        /// 请求选项（应该在构建请求早期进行设置）
        /// </summary>
        HttpRequestExecutionOptions ExecutionOptions { get; set; }

        /// <summary>
        /// Headers
        /// </summary>
        HttpHeaders Headers { get; }

        /// <summary>
        /// 是否设置了请求选项
        /// </summary>
        bool IsSetOptions { get; }

        /// <summary>
        /// 自动循环处理的最大重定向次数
        /// </summary>
        int MaxAutomaticRedirections { get; set; }

        /// <summary>
        /// http方法
        /// </summary>
        HttpMethod Method { get; set; }

        /// <summary>
        /// Web代理
        /// </summary>
        IWebProxy? Proxy { get; set; }

        /// <summary>
        /// 请求的Uri
        /// </summary>
        Uri RequestUri { get; }

        /// <summary>
        /// 超时时间（毫秒）
        /// </summary>
        int? Timeout { get; set; }

        /// <summary>
        /// 取消标记
        /// </summary>
        CancellationToken Token { get; set; }

        /// <inheritdoc cref="HttpVersion"/>
        Version? Version { get; set; }

        #endregion 属性

        #region 方法

        /// <summary>
        /// 获取 <see cref="HttpRequestMessage"/>
        /// </summary>
        /// <returns></returns>
        HttpRequestMessage GetHttpRequestMessage();

        #endregion 方法
    }
}