using System.Collections.Generic;
using System.Runtime.CompilerServices;

using Cuture.Http.Util;

namespace Cuture.Http
{
    /// <summary>
    /// <see cref="IHttpRequest"/> 请求拓展类
    /// </summary>
    public static partial class IHttpTurboRequestExtension
    {
        #region Header

        /// <summary>
        /// 添加Header
        /// </summary>
        /// <param name="request"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpRequest AddHeader(this IHttpRequest request, string key, string value)
        {
            request.AddHeader(key, value);
            return request;
        }

        /// <summary>
        /// 添加Header集合
        /// </summary>
        /// <param name="request"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpRequest AddHeaders(this IHttpRequest request, IEnumerable<KeyValuePair<string, string>> headers)
        {
            foreach (var header in headers)
            {
                request.AddHeader(header.Key, header.Value);
            }
            return request;
        }

        /// <summary>
        /// 添加Header且不验证其内容
        /// </summary>
        /// <param name="request"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpRequest AddHeadersWithoutValidation(this IHttpRequest request, IEnumerable<KeyValuePair<string, string>> headers)
        {
            foreach (var header in headers)
            {
                request.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
            return request;
        }

        /// <summary>
        /// 添加Header且不验证其内容
        /// </summary>
        /// <param name="request"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpRequest AddHeaderWithoutValidation(this IHttpRequest request, string key, string value)
        {
            request.Headers.TryAddWithoutValidation(key, value);
            return request;
        }

        /// <summary>
        /// 如果存在Header则将其移除,并添加Header
        /// </summary>
        /// <param name="request"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpRequest AddNewHeader(this IHttpRequest request, string key, string value)
        {
            request.RemoveHeader(key);
            request.AddHeader(key, value);
            return request;
        }

        /// <summary>
        /// 使用Cookie
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpRequest UseCookie(this IHttpRequest request, string cookie)
        {
            request.AddHeader(HttpHeaderDefinitions.Cookie, cookie);
            return request;
        }

        /// <summary>
        /// 使用Referer
        /// </summary>
        /// <param name="request"></param>
        /// <param name="referer"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpRequest UseReferer(this IHttpRequest request, string referer)
        {
            request.AddHeader(HttpHeaderDefinitions.Referer, referer);
            return request;
        }

        /// <summary>
        /// 使用UserAgent
        /// </summary>
        /// <param name="request"></param>
        /// <param name="userAgent"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpRequest UseUserAgent(this IHttpRequest request, string userAgent)
        {
            request.AddHeader(HttpHeaderDefinitions.UserAgent, userAgent);
            return request;
        }

        /// <summary>
        /// 使用基础认证
        /// </summary>
        /// <param name="request"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpRequest WithBasicAuth(this IHttpRequest request, string userName, string password)
        {
            request.AddHeader(BasicAuthUtil.HttpHeader, BasicAuthUtil.EncodeToHeader(userName, password));
            return request;
        }

        #endregion Header
    }
}