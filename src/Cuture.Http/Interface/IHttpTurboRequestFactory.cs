using System;

namespace Cuture.Http
{
    /// <summary>
    /// http请求工厂
    /// </summary>
    public interface IHttpTurboRequestFactory : IDisposable
    {
        #region 方法

        /// <summary>
        /// 创建请求
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        IHttpTurboRequest CreateRequest(Uri uri);

        /// <summary>
        /// 创建请求
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        IHttpTurboRequest CreateRequest(string url);

        #endregion 方法
    }
}