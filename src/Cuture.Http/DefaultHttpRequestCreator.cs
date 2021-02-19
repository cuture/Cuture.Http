using System;

namespace Cuture.Http
{
    /// <inheritdoc/>
    public sealed class DefaultHttpRequestCreator : IHttpRequestCreator
    {
        #region 方法

        /// <inheritdoc/>
        public IHttpRequest CreateRequest(Uri requestUri) => new DefaultHttpRequest(requestUri);

        /// <inheritdoc/>
        public void Dispose() { }

        #endregion 方法
    }
}