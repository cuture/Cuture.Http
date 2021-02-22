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
    /// 默认<see cref="IHttpRequest"/> 实现
    /// <para/>
    /// 派生自 <see cref="HttpRequestMessage"/> 的 <see cref="IHttpRequest"/> 实现
    /// <para/>
    /// * 不可重复使用进行请求
    /// </summary>
#pragma warning disable CS8766 // 返回类型中引用类型的为 Null 性与隐式实现的成员不匹配(可能是由于为 Null 性特性)。

    public class DefaultHttpRequest : HttpRequestMessage, IHttpRequest
#pragma warning restore CS8766 // 返回类型中引用类型的为 Null 性与隐式实现的成员不匹配(可能是由于为 Null 性特性)。
    {
        #region Private 字段

        private HttpRequestOptions? _options;

        #endregion Private 字段

        #region 属性

        /// <inheritdoc/>
        public bool AllowRedirection { get; set; }

        /// <summary>
        /// 禁用Proxy
        /// <para/>初始值为 <see cref="HttpRequestOptions.DisableUseDefaultProxyByDefault"/>
        /// </summary>
        public bool DisableProxy { get; set; } = HttpRequestOptions.DisableUseDefaultProxyByDefault;

        /// <inheritdoc/>
        public bool IsSetOptions => _options != null;

        /// <inheritdoc/>
        public int MaxAutomaticRedirections { get; set; } = HttpRequestOptions.MaxAutomaticRedirections;

        /// <inheritdoc/>
        public IWebProxy? Proxy { get; set; }

        /// <inheritdoc/>
        public HttpRequestOptions RequestOptions
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

        /// <inheritdoc/>
        public int? Timeout { get; set; }

        /// <inheritdoc/>
        public CancellationToken Token { get; set; }

        #endregion 属性

        #region 构造函数

        /// <summary>
        /// http请求
        /// </summary>
        /// <param name="requestUri">请求的Uri</param>
        public DefaultHttpRequest(Uri requestUri)
        {
            RequestUri = requestUri ?? throw new ArgumentNullException(nameof(requestUri));
        }

        #endregion 构造函数

        #region 方法

        /// <summary>
        /// 添加Header
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IHttpRequest AddHeader(string key, string value)
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
        public IHttpRequest AddHeader(string key, IEnumerable<string> values)
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

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public HttpRequestMessage GetHttpRequestMessage() => this;

        /// <summary>
        /// 移除Header
        /// </summary>
        /// <param name="key"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveHeader(string key) => Headers.Remove(key);

        #endregion 方法
    }
}