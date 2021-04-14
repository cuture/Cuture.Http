using System.Net.Http;

namespace Cuture.Http
{
    /// <summary>
    /// 重写析构函数的<see cref="HttpClient"/>
    /// <para/>
    /// 在析构时将会调用 <see cref="HttpClient.Dispose(bool)"/> ，参数值为 true
    /// <para/>
    /// 请求超时为永不超时 <see cref="System.Threading.Timeout.InfiniteTimeSpan"/>
    /// </summary>
    internal sealed class FinalizeableHttpClient : HttpClient
    {
        #region Public 构造函数

        /// <inheritdoc/>
        public FinalizeableHttpClient()
        {
            Timeout = System.Threading.Timeout.InfiniteTimeSpan;
        }

        /// <inheritdoc/>
        public FinalizeableHttpClient(HttpMessageHandler handler) : base(handler)
        {
            Timeout = System.Threading.Timeout.InfiniteTimeSpan;
        }

        /// <inheritdoc/>
        public FinalizeableHttpClient(HttpMessageHandler handler, bool disposeHandler) : base(handler, disposeHandler)
        {
            Timeout = System.Threading.Timeout.InfiniteTimeSpan;
        }

        #endregion Public 构造函数

        #region Private 析构函数

        ~FinalizeableHttpClient()
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("~FinalizeableHttpClient");
#endif
            Dispose(true);
        }

        #endregion Private 析构函数
    }
}