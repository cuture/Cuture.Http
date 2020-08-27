using System;
using System.Net.Http;
using System.Runtime.CompilerServices;

namespace Cuture.Http
{
    /// <summary>
    /// <see cref="IHttpTurboRequest"/> 请求拓展类
    /// </summary>
    public static partial class IHttpTurboRequestExtension
    {
        #region HttpMethod

        /// <summary>
        /// 使用Delete动作
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpTurboRequest UseDelete(this IHttpTurboRequest request)
        {
            request.Method = HttpMethod.Delete;
            return request;
        }

        /// <summary>
        /// 使用Get动作
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpTurboRequest UseGet(this IHttpTurboRequest request)
        {
            request.Method = HttpMethod.Get;
            return request;
        }

        /// <summary>
        /// 使用Post动作
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpTurboRequest UsePost(this IHttpTurboRequest request)
        {
            request.Method = HttpMethod.Post;
            return request;
        }

        /// <summary>
        /// 使用Put动作
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpTurboRequest UsePut(this IHttpTurboRequest request)
        {
            request.Method = HttpMethod.Put;
            return request;
        }

        /// <summary>
        /// 使用指定的Http动作
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="httpMethod">Http动作</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpTurboRequest UseVerb(this IHttpTurboRequest request, string httpMethod)
        {
            request.Method = new HttpMethod(httpMethod);
            return request;
        }

        /// <summary>
        /// 使用指定的Http动作
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="httpMethod">Http动作</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpTurboRequest UseVerb(this IHttpTurboRequest request, HttpMethod httpMethod)
        {
            request.Method = httpMethod;
            return request;
        }

        /// <summary>
        /// 使用指定的Http动作
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="httpMethod">Http动作</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Obsolete("使用 UseVerb 替代此方法调用")]
        public static IHttpTurboRequest WithVerb(this IHttpTurboRequest request, HttpMethod httpMethod)
        {
            request.Method = httpMethod;
            return request;
        }

        #endregion HttpMethod
    }
}