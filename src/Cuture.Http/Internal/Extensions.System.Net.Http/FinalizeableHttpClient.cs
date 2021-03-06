using System.Net.Http;

namespace Cuture.Http
{
    /// <summary>
    /// 重写析构函数的<see cref="HttpClient"/>
    /// <para/>
    /// 在析构时将会调用 <see cref="HttpClient.Dispose(bool)"/> ，参数值为 true
    /// </summary>
    internal sealed class FinalizeableHttpClient : HttpClient
    {
        #region Public 构造函数

        /// <inheritdoc/>
        public FinalizeableHttpClient()
        {
        }

        /// <inheritdoc/>
        public FinalizeableHttpClient(HttpMessageHandler handler) : base(handler)
        {
        }

        /// <inheritdoc/>
        public FinalizeableHttpClient(HttpMessageHandler handler, bool disposeHandler) : base(handler, disposeHandler)
        {
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