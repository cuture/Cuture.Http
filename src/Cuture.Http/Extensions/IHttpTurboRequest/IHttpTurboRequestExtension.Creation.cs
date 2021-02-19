using System;
using System.Net.Http;
using System.Runtime.CompilerServices;

namespace Cuture.Http
{
    //此文件主要包含请求创建的拓展方法

    public static partial class IHttpTurboRequestExtension
    {
        #region Creation

        #region Obslate

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Obsolete("使用 CreateHttpRequest 方法替代")]
        public static IHttpRequest ToHttpRequest(this string requestUri) => ToHttpRequest(requestUri, HttpRequestOptions.DefaultTurboRequestFactory);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Obsolete("使用 CreateHttpRequest 方法替代")]
        public static IHttpRequest ToHttpRequest(this string requestUri, IHttpTurboRequestFactory factory) => factory.CreateRequest(requestUri);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Obsolete("使用 CreateHttpRequest 方法替代")]
        public static IHttpRequest ToHttpRequest(this Uri requestUri) => ToHttpRequest(requestUri, HttpRequestOptions.DefaultTurboRequestFactory);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Obsolete("使用 CreateHttpRequest 方法替代")]
        public static IHttpRequest ToHttpRequest(this Uri requestUri, IHttpTurboRequestFactory factory) => factory.CreateRequest(requestUri);

        #endregion Obslate

        #region for string

        /// <inheritdoc cref="CreateHttpRequest(string, IHttpTurboRequestFactory)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpRequest CreateHttpRequest(this string requestUri) => CreateHttpRequest(requestUri, HttpRequestOptions.DefaultTurboRequestFactory);

        /// <inheritdoc cref="CreateHttpRequest(Uri, IHttpTurboRequestFactory)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpRequest CreateHttpRequest(this string requestUri, IHttpTurboRequestFactory factory) => factory.CreateRequest(requestUri);

        #endregion for string

        #region for Uri

        /// <inheritdoc cref="CreateHttpRequest(Uri, IHttpTurboRequestFactory)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpRequest CreateHttpRequest(this Uri requestUri) => CreateHttpRequest(requestUri, HttpRequestOptions.DefaultTurboRequestFactory);

        /// <summary>
        /// 创建Http请求
        /// </summary>
        /// <param name="requestUri">请求的URI</param>
        /// <param name="factory">请求创建器</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpRequest CreateHttpRequest(this Uri requestUri, IHttpTurboRequestFactory factory) => factory.CreateRequest(requestUri);

        #endregion for Uri

        #region for httpMessageInvoker

        /// <inheritdoc cref="CreateRequest(HttpMessageInvoker, Uri, IHttpTurboRequestFactory)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpRequest CreateRequest(this HttpMessageInvoker httpMessageInvoker, string requestUri) => CreateRequest(httpMessageInvoker, requestUri, HttpRequestOptions.DefaultTurboRequestFactory);

        /// <inheritdoc cref="CreateRequest(HttpMessageInvoker, Uri, IHttpTurboRequestFactory)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpRequest CreateRequest(this HttpMessageInvoker httpMessageInvoker, string requestUri, IHttpTurboRequestFactory factory) => httpMessageInvoker.CreateRequest(new Uri(requestUri, UriKind.RelativeOrAbsolute), factory);

        /// <inheritdoc cref="CreateRequest(HttpMessageInvoker, Uri, IHttpTurboRequestFactory)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpRequest CreateRequest(this HttpMessageInvoker httpMessageInvoker, Uri requestUri) => CreateRequest(httpMessageInvoker, requestUri, HttpRequestOptions.DefaultTurboRequestFactory);

        /// <summary>
        /// 创建Http请求
        /// </summary>
        /// <param name="httpMessageInvoker">用以发起请求的<see cref="HttpMessageInvoker"/></param>
        /// <param name="requestUri">请求的URI</param>
        /// <param name="factory">请求创建器</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpRequest CreateRequest(this HttpMessageInvoker httpMessageInvoker, Uri requestUri, IHttpTurboRequestFactory factory) => factory.CreateRequest(requestUri).UseClient(httpMessageInvoker);

        #endregion for httpMessageInvoker

        #endregion Creation
    }
}