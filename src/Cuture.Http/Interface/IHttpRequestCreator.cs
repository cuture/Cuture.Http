using System;

namespace Cuture.Http
{
    /// <summary>
    /// <see cref="IHttpRequest"/> 创建器
    /// </summary>
    public interface IHttpRequestCreator : IDisposable
    {
        #region 方法

        /// <summary>
        /// 使用 <paramref name="requestUri"/> 创建 <see cref="IHttpRequest"/>
        /// </summary>
        /// <param name="requestUri"></param>
        /// <returns></returns>
        IHttpRequest CreateRequest(Uri requestUri);

        #endregion 方法
    }
}