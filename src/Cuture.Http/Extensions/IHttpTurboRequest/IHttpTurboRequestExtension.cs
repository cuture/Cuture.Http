using System.Runtime.CompilerServices;
using System.Text;

namespace Cuture.Http
{
    /// <summary>
    /// <see cref="IHttpTurboRequest"/> 请求拓展类
    /// </summary>
    public static partial class IHttpTurboRequestExtension
    {
        #region 构造函数

        static IHttpTurboRequestExtension()
        {
#if NETSTANDARD2_0
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif
        }

        #endregion 构造函数

        #region 方法

        /// <summary>
        /// 获取一个HttpTurbo
        /// </summary>
        /// <param name="request">本次请求</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IHttpTurboClient InternalGetHttpTurboClient(IHttpTurboRequest request, HttpRequestOptions options)
        {
            return options.TurboClient ?? options.TurboClientFactory?.GetTurboClient(request);
        }

        #endregion 方法
    }
}