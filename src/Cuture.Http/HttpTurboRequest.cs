using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Cuture.Http
{
    /// <summary>
    /// http请求
    /// </summary>
    public class HttpTurboRequest : HttpRequestMessage, IHttpTurboRequest
    {
        #region Private 字段

        private HttpRequestOptions _options;

        #endregion Private 字段

        #region 属性

        /// <summary>
        /// 是否允许重定向
        /// </summary>
        public bool AllowRedirection { get; set; }

        /// <summary>
        /// 禁用Proxy
        /// <para/>初始值为 <see cref="HttpRequestOptions.DisableUseDefaultProxyByDefault"/>
        /// </summary>
        public bool DisableProxy { get; set; } = HttpRequestOptions.DisableUseDefaultProxyByDefault;

        public bool IsSetOptions => _options != null;

        /// <summary>
        /// 自动循环处理的最大重定向次数
        /// </summary>
        public int MaxAutomaticRedirections { get; set; } = HttpRequestOptions.MaxAutomaticRedirections;

        /// <summary>
        /// 请求选项
        /// </summary>
        public HttpRequestOptions Options
        {
            get
            {
                if (_options is null)
                {
                    _options = HttpRequestOptions.Default.Copy();
                }

                return _options;
            }
            set => _options = value;
        }

        /// <summary>
        /// Web代理
        /// </summary>
        public IWebProxy Proxy { get; set; }

        /// <summary>
        /// 超时时间
        /// </summary>
        public int? Timeout { get; set; }

        /// <summary>
        /// 取消标记
        /// </summary>
        public CancellationToken Token { get; set; }

        #endregion 属性

        #region 构造函数

        /// <summary>
        /// http请求
        /// </summary>
        public HttpTurboRequest()
        {
        }

        /// <summary>
        /// http请求
        /// </summary>
        /// <param name="url">请求的url</param>
        public HttpTurboRequest(string url) : this(new Uri(url, UriKind.RelativeOrAbsolute))
        {
        }

        /// <summary>
        /// http请求
        /// </summary>
        /// <param name="uri">请求的uri</param>
        public HttpTurboRequest(Uri uri)
        {
            RequestUri = uri ?? throw new ArgumentNullException(nameof(uri));
        }

        #endregion 构造函数

        #region 方法

        /// <summary>
        /// 添加Header
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IHttpTurboRequest AddHeader(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("can not be null or empty", nameof(key));
            }

            if (string.IsNullOrEmpty(value))
            {
                Headers.Remove(key);
            }
            else
            {
                Headers.Add(key, value);
            }
            return this;
        }

        /// <summary>
        /// 添加Header
        /// </summary>
        /// <param name="key"></param>
        /// <param name="values"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IHttpTurboRequest AddHeader(string key, IEnumerable<string> values)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("can not be null or empty", nameof(key));
            }

            if (values is null || !values.Any())
            {
                Headers.Remove(key);
            }
            else
            {
                Headers.Add(key, values);
            }
            return this;
        }

        /// <summary>
        /// 获取 <see cref="HttpRequestMessage"/>
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public HttpRequestMessage AsRequest() => this;

        /// <summary>
        /// 移除Header
        /// </summary>
        /// <param name="key"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveHeader(string key) => Headers.Remove(key);

        #endregion 方法
    }
}