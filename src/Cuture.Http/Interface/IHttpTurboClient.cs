using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Cuture.Http
{
    /// <summary>
    /// http快速访问客户端
    /// </summary>
    public interface IHttpTurboClient : IDisposable
    {
        #region 方法

        /// <summary>
        /// 执行请求
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<HttpResponseMessage> ExecuteAsync(IHttpTurboRequest request);

        /// <summary>
        /// 执行请求
        /// </summary>
        /// <param name="request"></param>
        /// <param name="completionOption"></param>
        /// <returns></returns>
        Task<HttpResponseMessage> ExecuteAsync(IHttpTurboRequest request, HttpCompletionOption completionOption);

        #endregion 方法
    }
}