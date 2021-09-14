using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http
{
    /// <summary>
    /// 空的<see cref="HttpMessageHandler"/>实现
    /// </summary>
    internal class EmptyHttpMessageHandler : HttpMessageHandler
    {
        #region Public 字段

        public static readonly EmptyHttpMessageHandler Instance = new();

        #endregion Public 字段

        #region Protected 方法

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) => throw new NotImplementedException();

        #endregion Protected 方法
    }
}