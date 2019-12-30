using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Cuture.Http
{
    /// <summary>
    /// http快速访问客户端
    /// </summary>
    public sealed class HttpTurboClient : IHttpTurboClient
    {
        #region 字段

        /// <summary>
        /// HttpClient
        /// </summary>
        private readonly HttpClient _httpClient = null;

        #endregion 字段

        #region 构造函数

        /// <summary>
        /// 封装System.Net.Http.HttpClient
        /// 用于http请求
        /// </summary>
        /// <param name="httpClient">内部使用的HttpClient</param>
        public HttpTurboClient(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        /// <summary>
        /// http快速访问客户端
        /// 封装System.Net.Http.HttpClient
        /// 用于http请求
        /// </summary>
        /// <param name="httpClientHandler">使用的HttpClientHandler</param>
        public HttpTurboClient(HttpClientHandler httpClientHandler)
        {
            _httpClient = new HttpClient(httpClientHandler);
        }

        /// <summary>
        /// 封装System.Net.Http.HttpClient
        /// 用于http请求
        /// </summary>
        public HttpTurboClient() : this(false)
        { }

        /// <summary>
        /// http快速访问客户端
        /// 封装System.Net.Http.HttpClient
        /// 用于http请求
        /// </summary>
        /// <param name="allowRedirection">是否允许自动重定向</param>
        public HttpTurboClient(bool allowRedirection)
        {
#pragma warning disable CA2000 // 丢失范围之前释放对象
            if (allowRedirection)
            {
                _httpClient = new HttpClient(CreateDefaultAllowRedirectionClientHandler());
            }
            else
            {
                _httpClient = new HttpClient(CreateDefaultClientHandler());
            }
#pragma warning restore CA2000 // 丢失范围之前释放对象

            var defaultSettings = HttpDefaultSetting.DefaultHttpHeaders;
            var defaultHeaders = _httpClient.DefaultRequestHeaders;
            foreach (var item in defaultSettings)
            {
                SetHeader(defaultHeaders, item.Key, item.Value);
            }
        }

        /// <summary>
        ///
        /// </summary>
        ~HttpTurboClient()
        {
            Debug.WriteLine($"终结器:{nameof(HttpTurboClient)}_{this.GetHashCode()}");
            _httpClient.Dispose();
        }

        #endregion 构造函数

        #region 方法

        /// <summary>
        /// 销毁相关资源
        /// </summary>
        public void Dispose()
        {
            _httpClient.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 执行请求
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async Task<HttpResponseMessage> ExecuteAsync(IHttpTurboRequest request) => await ExecuteAsync(request, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false);

        /// <summary>
        /// 执行请求
        /// </summary>
        /// <param name="request"></param>
        /// <param name="completionOption"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async Task<HttpResponseMessage> ExecuteAsync(IHttpTurboRequest request, HttpCompletionOption completionOption)
        {
            if (request.Timeout.HasValue)
            {
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(request.Token);
                cts.CancelAfter(request.Timeout.Value);
                return await _httpClient.SendAsync(request.AsRequest(), completionOption, cts.Token).ConfigureAwait(false);
            }
            else
            {
                return await _httpClient.SendAsync(request.AsRequest(), completionOption, request.Token).ConfigureAwait(false);
            }
        }

        #region 静态方法

        /// <summary>
        /// 创建默认的HttpClientHandler
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static HttpClientHandler CreateDefaultAllowRedirectionClientHandler()
        {
            var allowRedirectionClientHandler = CreateDefaultClientHandler();

            allowRedirectionClientHandler.AllowAutoRedirect = true;
            allowRedirectionClientHandler.MaxAutomaticRedirections = 10;

            return allowRedirectionClientHandler;
        }

        /// <summary>
        /// 创建默认的HttpClientHandler
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static HttpClientHandler CreateDefaultClientHandler()
        {
            return new HttpClientHandler()
            {
                UseCookies = false,
                AllowAutoRedirect = false,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            };
        }

        /// <summary>
        /// 设置HttpRequestHeaders的Header
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SetHeader(HttpRequestHeaders headers, string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                headers.Remove(key);
                headers.TryAddWithoutValidation(key, value);
            }
        }

        #endregion 静态方法

        #endregion 方法
    }
}