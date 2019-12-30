using System;

namespace Cuture.Http
{
    /// <summary>
    /// HttpTurbo构建工厂
    /// </summary>
    public interface IHttpTurboClientFactory : IDisposable
    {
        #region 方法

        /// <summary>
        /// 清空所有的HttpClient缓存
        /// </summary>
        void Clear();

        /// <summary>
        /// 获取HttpTurboClient
        /// </summary>
        /// <param name="request">请求</param>
        /// <returns></returns>
        IHttpTurboClient GetTurboClient(IHttpTurboRequest request);

        #endregion 方法
    }
}