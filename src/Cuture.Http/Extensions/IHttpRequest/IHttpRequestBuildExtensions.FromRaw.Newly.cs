using System;
using System.Runtime.CompilerServices;
using System.Text;

using static Cuture.Http.Internal.HttpRawDataReadTool;

namespace Cuture.Http
{
    //此文件主要包含 从原始请求内容构建请求 相关的拓展方法

    public static partial class IHttpRequestBuildExtensions
    {
        #region 方法

        #region Headers

        /// <inheritdoc cref="LoadHeadersFromRaw(IHttpRequest, in ReadOnlySpan{byte})"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpRequest LoadHeadersFromRaw(this IHttpRequest request, string rawBase64String) => request.LoadHeadersFromRaw(Convert.FromBase64String(rawBase64String));

        /// <inheritdoc cref="LoadHeadersFromRaw(IHttpRequest, in ReadOnlySpan{byte})"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpRequest LoadHeadersFromRaw(this IHttpRequest request, byte[] rawData) => request.LoadHeadersFromRaw(rawData.AsSpan());

        /// <summary>
        /// 从原始的请求数据中加载Header到请求
        /// <para/>
        /// Note:
        /// <para/>
        /// * <see cref="HttpHeaderDefinitions.Host"/>、<see cref="HttpHeaderDefinitions.ContentType"/>、<see cref="HttpHeaderDefinitions.ContentLength"/>将会被忽略
        /// <para/>
        /// * 注意重复添加header的问题
        /// </summary>
        /// <param name="request"></param>
        /// <param name="rawData"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpRequest LoadHeadersFromRaw(this IHttpRequest request, in ReadOnlySpan<byte> rawData)
        {
            var data = rawData.TruncationStart(NewLineSeparator.AsSpan());
            request.LoadHeaders(ref data, Encoding.UTF8);
            return request;
        }

        #endregion Headers

        #region Content

        /// <inheritdoc cref="LoadContentFromRaw(IHttpRequest, in ReadOnlySpan{byte})"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpRequest LoadContentFromRaw(this IHttpRequest request, string rawBase64String) => request.LoadContentFromRaw(Convert.FromBase64String(rawBase64String));

        /// <inheritdoc cref="LoadContentFromRaw(IHttpRequest, in ReadOnlySpan{byte})"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpRequest LoadContentFromRaw(this IHttpRequest request, byte[] rawData) => request.LoadContentFromRaw(rawData.AsSpan());

        /// <summary>
        /// 从原始的请求数据中加载Content到请求
        /// </summary>
        /// <param name="request"></param>
        /// <param name="rawData"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpRequest LoadContentFromRaw(this IHttpRequest request, in ReadOnlySpan<byte> rawData)
        {
            var data = rawData.TruncationStart(NewLineSeparator.AsSpan());
            var (contentLength, contentType) = RequestBuildTool.LoadHeaders(ref data, null, Encoding.UTF8);

            request.WithContent(data, contentType, contentLength);

            return request;
        }

        #endregion Content

        #region Headers and Content

        /// <inheritdoc cref="LoadHeadersAndContentFromRaw(IHttpRequest, in ReadOnlySpan{byte})"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpRequest LoadHeadersAndContentFromRaw(this IHttpRequest request, string rawBase64String) => request.LoadHeadersAndContentFromRaw(Convert.FromBase64String(rawBase64String));

        /// <inheritdoc cref="LoadHeadersAndContentFromRaw(IHttpRequest, in ReadOnlySpan{byte})"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpRequest LoadHeadersAndContentFromRaw(this IHttpRequest request, byte[] rawData) => request.LoadHeadersAndContentFromRaw(rawData.AsSpan());

        /// <summary>
        /// 从原始的请求数据中加载Header和Content到请求
        /// <para/>
        /// Note:
        /// <para/>
        /// * <see cref="HttpHeaderDefinitions.Host"/>将会被忽略
        /// <para/>
        /// * 注意重复添加header的问题
        /// </summary>
        /// <param name="request"></param>
        /// <param name="rawData"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpRequest LoadHeadersAndContentFromRaw(this IHttpRequest request, in ReadOnlySpan<byte> rawData)
        {
            var data = rawData.TruncationStart(NewLineSeparator.AsSpan());
            var (contentLength, contentType) = request.LoadHeaders(ref data, Encoding.UTF8);

            request.WithContent(data, contentType, contentLength);

            return request;
        }

        #endregion Headers and Content

        #region Internal

        /// <summary>
        /// 加载Header到请求，并返回contentLength和contentType
        /// </summary>
        /// <param name="request"></param>
        /// <param name="data"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static (int contentLength, string contentType) LoadHeaders(this IHttpRequest request, ref ReadOnlySpan<byte> data, Encoding? encoding = null) => RequestBuildTool.LoadHeaders(ref data, request.Headers, encoding);

        #endregion Internal

        #endregion 方法
    }
}