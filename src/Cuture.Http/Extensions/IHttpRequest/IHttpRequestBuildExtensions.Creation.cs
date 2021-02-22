using System;
using System.Net.Http;
using System.Runtime.CompilerServices;

namespace Cuture.Http
{
    //此文件主要包含 请求创建 相关的拓展方法

    /// <summary>
    /// <see cref="IHttpRequest"/> 构建拓展类
    /// </summary>
    public static partial class IHttpRequestBuildExtensions
    {
        #region Creation

        #region Obslate

        /// <inheritdoc cref="CreateHttpRequest(Uri, IHttpRequestCreator)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Obsolete("使用 CreateHttpRequest 方法替代")]
        public static IHttpRequest ToHttpRequest(this string requestUri) => requestUri.CreateHttpRequest(HttpRequestOptions.DefaultHttpRequestCreator);

        /// <inheritdoc cref="CreateHttpRequest(Uri, IHttpRequestCreator)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Obsolete("使用 CreateHttpRequest 方法替代")]
        public static IHttpRequest ToHttpRequest(this string requestUri, IHttpRequestCreator requestCreator) => requestUri.CreateHttpRequest(requestCreator);

        /// <inheritdoc cref="CreateHttpRequest(Uri, IHttpRequestCreator)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Obsolete("使用 CreateHttpRequest 方法替代")]
        public static IHttpRequest ToHttpRequest(this Uri requestUri) => requestUri.CreateHttpRequest(HttpRequestOptions.DefaultHttpRequestCreator);

        /// <inheritdoc cref="CreateHttpRequest(Uri, IHttpRequestCreator)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Obsolete("使用 CreateHttpRequest 方法替代")]
        public static IHttpRequest ToHttpRequest(this Uri requestUri, IHttpRequestCreator requestCreator) => requestUri.CreateHttpRequest(requestCreator);

        #endregion Obslate

        #region for string

        /// <inheritdoc cref="CreateHttpRequest(string, IHttpRequestCreator)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpRequest CreateHttpRequest(this string requestUri) => CreateHttpRequest(requestUri, HttpRequestOptions.DefaultHttpRequestCreator);

        /// <inheritdoc cref="CreateHttpRequest(string, IHttpRequestCreator)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpRequest CreateHttpRequest(this string requestUri, bool reuseable) => CreateHttpRequest(requestUri, HttpRequestOptions.DefaultHttpRequestCreator, reuseable);

        /// <inheritdoc cref="CreateHttpRequest(Uri, IHttpRequestCreator)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpRequest CreateHttpRequest(this string requestUri, IHttpRequestCreator requestCreator, bool reuseable = false) => new Uri(requestUri, UriKind.RelativeOrAbsolute).CreateHttpRequest(requestCreator, reuseable);

        #endregion for string

        #region for Uri

        /// <inheritdoc cref="CreateHttpRequest(Uri, IHttpRequestCreator)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpRequest CreateHttpRequest(this Uri requestUri) => CreateHttpRequest(requestUri, HttpRequestOptions.DefaultHttpRequestCreator);

        /// <inheritdoc cref="CreateHttpRequest(string, IHttpRequestCreator)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpRequest CreateHttpRequest(this Uri requestUri, bool reuseable) => CreateHttpRequest(requestUri, HttpRequestOptions.DefaultHttpRequestCreator, reuseable);

        /// <summary>
        /// 创建Http请求
        /// </summary>
        /// <param name="requestUri">请求的URI</param>
        /// <param name="requestCreator">请求创建器</param>
        /// <param name="reuseable">是否可重用请求</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpRequest CreateHttpRequest(this Uri requestUri, IHttpRequestCreator requestCreator, bool reuseable = false)
                => !reuseable ? requestCreator.CreateRequest(requestUri)
                              : requestCreator.CreateReuseableRequest(requestUri);

        #endregion for Uri

        #region for httpMessageInvoker

        /// <inheritdoc cref="CreateRequest(HttpMessageInvoker, Uri, IHttpRequestCreator, bool)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpRequest CreateRequest(this HttpMessageInvoker httpMessageInvoker, string requestUri)
                => CreateRequest(httpMessageInvoker, requestUri, HttpRequestOptions.DefaultHttpRequestCreator);

        /// <inheritdoc cref="CreateRequest(HttpMessageInvoker, Uri, IHttpRequestCreator, bool)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpRequest CreateRequest(this HttpMessageInvoker httpMessageInvoker, string requestUri, IHttpRequestCreator requestCreator)
                => CreateRequest(httpMessageInvoker, new Uri(requestUri, UriKind.RelativeOrAbsolute), requestCreator);

        /// <inheritdoc cref="CreateRequest(HttpMessageInvoker, Uri, IHttpRequestCreator, bool)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpRequest CreateRequest(this HttpMessageInvoker httpMessageInvoker, string requestUri, IHttpRequestCreator requestCreator, bool reuseable)
                => CreateRequest(httpMessageInvoker, new Uri(requestUri, UriKind.RelativeOrAbsolute), requestCreator, reuseable);

        /// <inheritdoc cref="CreateRequest(HttpMessageInvoker, Uri, IHttpRequestCreator, bool)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpRequest CreateRequest(this HttpMessageInvoker httpMessageInvoker, Uri requestUri)
                => CreateRequest(httpMessageInvoker, requestUri, HttpRequestOptions.DefaultHttpRequestCreator);

        /// <inheritdoc cref="CreateRequest(HttpMessageInvoker, Uri, IHttpRequestCreator, bool)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpRequest CreateRequest(this HttpMessageInvoker httpMessageInvoker, Uri requestUri, bool reuseable)
                => CreateRequest(httpMessageInvoker, requestUri, HttpRequestOptions.DefaultHttpRequestCreator, reuseable);

        /// <summary>
        /// 创建Http请求
        /// </summary>
        /// <param name="httpMessageInvoker">用以发起请求的<see cref="HttpMessageInvoker"/></param>
        /// <param name="requestUri">请求的URI</param>
        /// <param name="requestCreator">请求创建器</param>
        /// <param name="reuseable">是否可重用请求</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpRequest CreateRequest(this HttpMessageInvoker httpMessageInvoker, Uri requestUri, IHttpRequestCreator requestCreator, bool reuseable = false)
                => !reuseable ? requestCreator.CreateRequest(requestUri).UseClient(httpMessageInvoker)
                              : requestCreator.CreateReuseableRequest(requestUri).UseClient(httpMessageInvoker);

        #endregion for httpMessageInvoker

        #endregion Creation
    }
}