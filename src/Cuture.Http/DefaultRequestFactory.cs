using System;

namespace Cuture.Http
{
    /// <summary>
    /// http请求工厂
    /// </summary>
    public sealed class DefaultRequestFactory : IHttpTurboRequestFactory
    {
        #region 方法

        /// <summary>
        /// 创建请求
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public IHttpRequest CreateRequest(Uri uri) => new DefaultHttpRequest(uri);

        /// <summary>
        /// 创建请求
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public IHttpRequest CreateRequest(string url) => new DefaultHttpRequest(url);

        /// <summary>
        ///
        /// </summary>
        public void Dispose() { }

        #endregion 方法
    }
}