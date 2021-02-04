#if NETCOREAPP

using System;
using System.Runtime.CompilerServices;
using System.Text;

using static Cuture.Http.Internal.HttpRawDataReadTool;

namespace Cuture.Http
{
    /// <summary>
    /// <see cref="IHttpTurboRequest"/> 请求拓展类
    /// </summary>
    public static partial class IHttpTurboRequestExtension
    {
        #region 方法

        #region Headers

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
        /// <param name="rawBase64String"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpTurboRequest LoadHeadersFromRaw(this IHttpTurboRequest request, string rawBase64String) => request.LoadHeadersFromRaw(Convert.FromBase64String(rawBase64String));

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
        public static IHttpTurboRequest LoadHeadersFromRaw(this IHttpTurboRequest request, byte[] rawData) => request.LoadHeadersFromRaw(rawData.AsSpan());

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
        public static IHttpTurboRequest LoadHeadersFromRaw(this IHttpTurboRequest request, in ReadOnlySpan<byte> rawData)
        {
            var data = rawData.TruncationStart(NewLineSeparator.AsSpan());
            request.LoadHeaders(ref data, Encoding.UTF8);
            return request;
        }

        #endregion Headers

        #region Content

        /// <summary>
        /// 从原始的请求数据中加载Content到请求
        /// </summary>
        /// <param name="request"></param>
        /// <param name="rawBase64String"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpTurboRequest LoadContentFromRaw(this IHttpTurboRequest request, string rawBase64String) => request.LoadContentFromRaw(Convert.FromBase64String(rawBase64String));

        /// <summary>
        /// 从原始的请求数据中加载Content到请求
        /// </summary>
        /// <param name="request"></param>
        /// <param name="rawData"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpTurboRequest LoadContentFromRaw(this IHttpTurboRequest request, byte[] rawData) => request.LoadContentFromRaw(rawData.AsSpan());

        /// <summary>
        /// 从原始的请求数据中加载Content到请求
        /// </summary>
        /// <param name="request"></param>
        /// <param name="rawData"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpTurboRequest LoadContentFromRaw(this IHttpTurboRequest request, in ReadOnlySpan<byte> rawData)
        {
            var data = rawData.TruncationStart(NewLineSeparator.AsSpan());
            var (contentLength, contentType) = RequestBuildTool.LoadHeaders(ref data, null, Encoding.UTF8);

            request.WithContent(data, contentType, contentLength);

            return request;
        }

        #endregion Content

        #region Headers and Content

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
        /// <param name="rawBase64String"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpTurboRequest LoadHeadersAndContentFromRaw(this IHttpTurboRequest request, string rawBase64String) => request.LoadHeadersAndContentFromRaw(Convert.FromBase64String(rawBase64String));

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
        public static IHttpTurboRequest LoadHeadersAndContentFromRaw(this IHttpTurboRequest request, byte[] rawData) => request.LoadHeadersAndContentFromRaw(rawData.AsSpan());

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
        public static IHttpTurboRequest LoadHeadersAndContentFromRaw(this IHttpTurboRequest request, in ReadOnlySpan<byte> rawData)
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
        internal static (int contentLength, string contentType) LoadHeaders(this IHttpTurboRequest request, ref ReadOnlySpan<byte> data, Encoding? encoding = null) => RequestBuildTool.LoadHeaders(ref data, request.Headers, encoding);

        #endregion Internal

        #endregion 方法
    }
}

#endif