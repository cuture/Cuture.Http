using System;
using System.Net;
using System.Runtime.CompilerServices;

namespace Cuture.Http
{
    /// <summary>
    /// <see cref="IHttpTurboRequest"/> 请求拓展类
    /// </summary>
    public static partial class IHttpTurboRequestExtension
    {
        #region Creation

        /// <summary>
        /// 字符串转换为Http请求
        /// <br/>
        /// 框架有默认并发限制,可以通过设置
        /// <see cref="ServicePointManager.DefaultConnectionLimit"/>
        /// 来放宽并发请求限制
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpTurboRequest ToHttpRequest(this string url) => ToHttpRequest(url, HttpRequestOptions.DefaultTurboRequestFactory);

        /// <summary>
        /// 字符串转换为Http请求
        /// <br/>
        /// 框架有默认并发限制,可以通过设置
        /// <see cref="ServicePointManager.DefaultConnectionLimit"/>
        /// 来放宽并发请求限制
        /// </summary>
        /// <param name="url"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpTurboRequest ToHttpRequest(this string url, IHttpTurboRequestFactory factory) => factory.CreateRequest(url);

        /// <summary>
        /// Uri转换为Http请求
        /// <br/>
        /// 框架有默认并发限制,可以通过设置
        /// <see cref="ServicePointManager.DefaultConnectionLimit"/>
        /// 来放宽并发请求限制
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpTurboRequest ToHttpRequest(this Uri uri) => ToHttpRequest(uri, HttpRequestOptions.DefaultTurboRequestFactory);

        /// <summary>
        /// Uri转换为Http请求
        /// <br/>
        /// 框架有默认并发限制,可以通过设置
        /// <see cref="ServicePointManager.DefaultConnectionLimit"/>
        /// 来放宽并发请求限制
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpTurboRequest ToHttpRequest(this Uri uri, IHttpTurboRequestFactory factory) => factory.CreateRequest(uri);

        #endregion Creation
    }
}