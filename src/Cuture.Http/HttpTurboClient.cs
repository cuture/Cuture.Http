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

        /// <summary>
        /// 外部HttpClient
        /// </summary>
        private readonly bool _outerClient = false;

        #endregion 字段

        #region 构造函数

        /// <summary>
        /// http快速访问客户端
        /// 封装System.Net.Http.HttpClient
        /// 用于http请求
        /// </summary>
        /// <param name="httpClient">内部使用的HttpClient</param>
        public HttpTurboClient(HttpClient httpClient)
        {
            _outerClient = true;
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
        /// http快速访问客户端
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
        /// <param name="maxAutomaticRedirections">允许自动重定向时的最大重定向次数</param>
        public HttpTurboClient(bool allowRedirection, int maxAutomaticRedirections = 10)
        {
#pragma warning disable CA2000 // 丢失范围之前释放对象
            _httpClient = allowRedirection
                ? new HttpClient(CreateDefaultAllowRedirectionClientHandler(maxAutomaticRedirections))
                : new HttpClient(CreateDefaultClientHandler());
#pragma warning restore CA2000 // 丢失范围之前释放对象

            var defaultHeaders = _httpClient.DefaultRequestHeaders;
            foreach (var item in HttpDefaultSetting.DefaultHttpHeaders)
            {
                SetHeader(defaultHeaders, item.Key, item.Value);
            }
        }

        #endregion 构造函数

        #region 析构函数

        /// <summary>
        ///
        /// </summary>
        ~HttpTurboClient()
        {
            Debug.WriteLine($"终结器:{nameof(HttpTurboClient)}_{this.GetHashCode()}");

            if (!_outerClient)
            {
                _httpClient.Dispose();
            }
        }

        #endregion 析构函数

        #region 方法

        /// <summary>
        /// 销毁相关资源
        /// </summary>
        public void Dispose()
        {
            if (!_outerClient)
            {
                _httpClient.Dispose();
            }
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
        /// 创建默认的允许重定向HttpClientHandler
        /// <para/><see cref="HttpClientHandler.UseProxy"/> = true
        /// <para/><see cref="HttpClientHandler.UseCookies"/> = false
        /// <para/><see cref="HttpClientHandler.AllowAutoRedirect"/> = false
        /// <para/><see cref="HttpClientHandler.AutomaticDecompression"/> = <see cref="DecompressionMethods.GZip"/> | <see cref="DecompressionMethods.Deflate"/>
        /// <para/><see cref="HttpClientHandler.AllowAutoRedirect"/> = true
        /// <para/><see cref="HttpClientHandler.MaxAutomaticRedirections"/> = 10
        /// </summary>
        /// <param name="maxAutomaticRedirections">最大重定向次数</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static HttpClientHandler CreateDefaultAllowRedirectionClientHandler(int maxAutomaticRedirections = 10)
        {
            var httpClientHandler = CreateDefaultClientHandler();

            httpClientHandler.AllowAutoRedirect = true;
            httpClientHandler.MaxAutomaticRedirections = maxAutomaticRedirections;

            return httpClientHandler;
        }

        /// <summary>
        /// 创建默认的HttpClientHandler
        /// <para/><see cref="HttpClientHandler.UseProxy"/> = true
        /// <para/><see cref="HttpClientHandler.UseCookies"/> = false
        /// <para/><see cref="HttpClientHandler.AllowAutoRedirect"/> = false
        /// <para/><see cref="HttpClientHandler.AutomaticDecompression"/> = <see cref="DecompressionMethods.GZip"/> | <see cref="DecompressionMethods.Deflate"/>
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static HttpClientHandler CreateDefaultClientHandler()
        {
            return new HttpClientHandler()
            {
                UseProxy = true,
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