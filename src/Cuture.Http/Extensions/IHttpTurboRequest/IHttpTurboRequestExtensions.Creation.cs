using System;
using System.Net.Http;
using System.Runtime.CompilerServices;

namespace Cuture.Http
{
    //此文件主要包含请求创建的拓展方法

    public static partial class IHttpTurboRequestExtensions
    {
        #region Creation

        #region Obslate

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Obsolete("使用 CreateHttpRequest 方法替代")]
        public static IHttpRequest ToHttpRequest(this string requestUri) => ToHttpRequest(requestUri, HttpRequestOptions.DefaultHttpRequestCreator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Obsolete("使用 CreateHttpRequest 方法替代")]
        public static IHttpRequest ToHttpRequest(this string requestUri, IHttpRequestCreator requestCreator) => new Uri(requestUri, UriKind.RelativeOrAbsolute).ToHttpRequest(requestCreator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Obsolete("使用 CreateHttpRequest 方法替代")]
        public static IHttpRequest ToHttpRequest(this Uri requestUri) => ToHttpRequest(requestUri, HttpRequestOptions.DefaultHttpRequestCreator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Obsolete("使用 CreateHttpRequest 方法替代")]
        public static IHttpRequest ToHttpRequest(this Uri requestUri, IHttpRequestCreator requestCreator) => requestCreator.CreateRequest(requestUri);

        #endregion Obslate

        #region for string

        /// <inheritdoc cref="CreateHttpRequest(string, IHttpRequestCreator)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpRequest CreateHttpRequest(this string requestUri) => CreateHttpRequest(requestUri, HttpRequestOptions.DefaultHttpRequestCreator);

        /// <inheritdoc cref="CreateHttpRequest(Uri, IHttpRequestCreator)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpRequest CreateHttpRequest(this string requestUri, IHttpRequestCreator requestCreator) => new Uri(requestUri, UriKind.RelativeOrAbsolute).CreateHttpRequest(requestCreator);

        #endregion for string

        #region for Uri

        /// <inheritdoc cref="CreateHttpRequest(Uri, IHttpRequestCreator)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpRequest CreateHttpRequest(this Uri requestUri) => CreateHttpRequest(requestUri, HttpRequestOptions.DefaultHttpRequestCreator);

        /// <summary>
        /// 创建Http请求
        /// </summary>
        /// <param name="requestUri">请求的URI</param>
        /// <param name="requestCreator">请求创建器</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpRequest CreateHttpRequest(this Uri requestUri, IHttpRequestCreator requestCreator) => requestCreator.CreateRequest(requestUri);

        #endregion for Uri

        #region for httpMessageInvoker

        /// <inheritdoc cref="CreateRequest(HttpMessageInvoker, Uri, IHttpRequestCreator)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpRequest CreateRequest(this HttpMessageInvoker httpMessageInvoker, string requestUri) => CreateRequest(httpMessageInvoker, requestUri, HttpRequestOptions.DefaultHttpRequestCreator);

        /// <inheritdoc cref="CreateRequest(HttpMessageInvoker, Uri, IHttpRequestCreator)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpRequest CreateRequest(this HttpMessageInvoker httpMessageInvoker, string requestUri, IHttpRequestCreator requestCreator) => httpMessageInvoker.CreateRequest(new Uri(requestUri, UriKind.RelativeOrAbsolute), requestCreator);

        /// <inheritdoc cref="CreateRequest(HttpMessageInvoker, Uri, IHttpRequestCreator)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpRequest CreateRequest(this HttpMessageInvoker httpMessageInvoker, Uri requestUri) => CreateRequest(httpMessageInvoker, requestUri, HttpRequestOptions.DefaultHttpRequestCreator);

        /// <summary>
        /// 创建Http请求
        /// </summary>
        /// <param name="httpMessageInvoker">用以发起请求的<see cref="HttpMessageInvoker"/></param>
        /// <param name="requestUri">请求的URI</param>
        /// <param name="requestCreator">请求创建器</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpRequest CreateRequest(this HttpMessageInvoker httpMessageInvoker, Uri requestUri, IHttpRequestCreator requestCreator) => requestCreator.CreateRequest(requestUri).UseClient(httpMessageInvoker);

        #endregion for httpMessageInvoker

        #endregion Creation
    }
}