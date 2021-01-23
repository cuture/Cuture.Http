using System;
using System.Runtime.CompilerServices;

#if NETSTANDARD

using System.Text;

#endif

namespace Cuture.Http
{
    /// <summary>
    /// <see cref="IHttpTurboRequest"/> 请求拓展类
    /// </summary>
    public static partial class IHttpTurboRequestExtension
    {
        #region 构造函数

#if NETSTANDARD

        static IHttpTurboRequestExtension()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

#endif

        #endregion 构造函数

        #region 方法

        /// <summary>
        /// 获取一个HttpTurbo
        /// </summary>
        /// <param name="request">本次请求</param>
        /// <param name="options"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IHttpTurboClient InternalGetHttpTurboClient(IHttpTurboRequest request, HttpRequestOptions options)
        {
            return options.TurboClient
                        ?? options.TurboClientFactory?.GetTurboClient(request)
                        ?? throw new ArgumentException($"HttpRequestOptions's {nameof(HttpRequestOptions.Client)}、{nameof(HttpRequestOptions.TurboClient)}、{nameof(HttpRequestOptions.TurboClientFactory)}、cannot both be null.");
        }

        #endregion 方法
    }
}