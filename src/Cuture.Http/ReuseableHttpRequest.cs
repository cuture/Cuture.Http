using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Cuture.Http
{
    /// <summary>
    /// 可重用的 <see cref="IHttpRequest"/> 实现
    /// <para/>
    /// * 可以重复使用进行请求
    /// </summary>
    public class ReuseableHttpRequest : IHttpRequest
    {
        #region Private 字段

        private HttpRequestExecutionOptions? _options;

        #endregion Private 字段

        #region 属性

        /// <inheritdoc/>
        public bool AllowRedirection { get; set; }

        /// <inheritdoc/>
        public HttpContent? Content { get; set; }

        /// <summary>
        /// 禁用Proxy
        /// <para/>初始值为 <see cref="HttpRequestGlobalOptions.DisableUseDefaultProxyByDefault"/>
        /// </summary>
        public bool DisableProxy { get; set; } = HttpRequestGlobalOptions.DisableUseDefaultProxyByDefault;

        /// <inheritdoc/>
        public HttpHeaders Headers => new SimpleHttpRequestHeaders();

        /// <inheritdoc/>
        public bool IsSetOptions => _options != null;

        /// <inheritdoc/>
        public int MaxAutomaticRedirections { get; set; } = HttpRequestGlobalOptions.MaxAutomaticRedirections;

        /// <inheritdoc/>
        public HttpMethod Method { get; set; } = HttpMethod.Get;

        /// <inheritdoc/>
        public IWebProxy? Proxy { get; set; }

        /// <inheritdoc/>
        public HttpRequestExecutionOptions ExecutionOptions
        {
            get
            {
                if (_options is null)
                {
                    _options = HttpRequestExecutionOptions.Default.Clone();
                }

                return _options;
            }
            set => _options = value;
        }

        /// <inheritdoc/>
        public Uri RequestUri { get; }

        /// <inheritdoc/>
        public int? Timeout { get; set; }

        /// <inheritdoc/>
        public CancellationToken Token { get; set; }

        /// <inheritdoc/>
        public Version? Version { get; set; }

        #endregion 属性

        #region 构造函数

        /// <summary>
        /// <inheritdoc cref="ReuseableHttpRequest"/>
        /// </summary>
        /// <param name="requestUri">请求的Uri</param>
        public ReuseableHttpRequest(Uri requestUri)
        {
            RequestUri = requestUri ?? throw new ArgumentNullException(nameof(requestUri));
        }

        #endregion 构造函数

        #region 方法

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public HttpRequestMessage GetHttpRequestMessage()
        {
            var message = new HttpRequestMessage(Method, RequestUri);

            if (Version != null)
            {
                message.Version = Version;
            }

            CopyHeaders(message.Headers, Headers);

            if (Content != null)
            {
                message.Content = Content;
            }

            return message;
        }

        private static void CopyHeaders(HttpHeaders target, HttpHeaders source)
        {
            foreach (var item in source)
            {
                target.TryAddWithoutValidation(item.Key, item.Value);
            }
        }

        #endregion 方法
    }
}