using System;
using System.Net.Http;
using System.Runtime.CompilerServices;

#if NETSTANDARD || NETCOREAPP3_1

using System.Text;

#endif

namespace Cuture.Http
{
    /// <summary>
    /// <see cref="IHttpRequest"/> 请求拓展类
    /// </summary>
    public static partial class IHttpTurboRequestExtension
    {
        #region 构造函数

#if NETSTANDARD || NETCOREAPP3_1

        static IHttpTurboRequestExtension()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

#endif

        #endregion 构造函数

        #region 方法

        /// <summary>
        /// 根据请求获取一个用以执行请求的 <see cref="HttpMessageInvoker"/>
        /// </summary>
        /// <param name="request">请求</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static HttpMessageInvoker InternalGetHttpMessageInvoker(IHttpRequest request)
        {
            var options = request.IsSetOptions ? request.RequestOptions : HttpRequestOptions.Default;
            return options.MessageInvoker
                        ?? options.MessageInvokerFactory?.GetInvoker(request)
                        ?? throw new ArgumentException($"HttpRequestOptions's {nameof(HttpRequestOptions.MessageInvoker)}、{nameof(HttpRequestOptions.MessageInvokerFactory)} cannot both be null.");
        }

        #endregion 方法
    }
}