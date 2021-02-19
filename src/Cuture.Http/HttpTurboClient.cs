using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using Cuture.Http.Util;

namespace Cuture.Http
{
    /// <summary>
    /// http快速访问客户端<para/>
    /// 封装 <see cref="HttpClient"/> 用于http请求
    /// </summary>
    public sealed class HttpTurboClient : IHttpTurboClient
    {
        #region 字段

        /// <summary>
        /// 是否在dispose时释放HttpClient
        /// </summary>
        private readonly bool _disposeClient;

        /// <summary>
        /// HttpClient
        /// </summary>
        private readonly HttpClient _httpClient;

        #endregion 字段

        #region 构造函数

        /// <summary>
        /// http快速访问客户端<para/>
        /// 封装 <see cref="HttpClient"/> 用于http请求
        /// </summary>
        /// <param name="httpClient">内部执行请求时使用的 <see cref="HttpClient"/></param>
        /// <param name="disposeClient">是否在Dispose时释放 <paramref name="httpClient"/></param>
        public HttpTurboClient(HttpClient httpClient, bool disposeClient = false)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _disposeClient = disposeClient;
        }

        /// <summary>
        /// http快速访问客户端<para/>
        /// 封装 <see cref="HttpClient"/> 用于http请求
        /// </summary>
        /// <param name="httpMessageHandler">内部 <see cref="HttpClient"/> 使用的 <see cref="HttpMessageHandler"/></param>
        /// <param name="disposeHandler">是否在Dispose时释放 <paramref name="httpMessageHandler"/></param>
        public HttpTurboClient(HttpMessageHandler httpMessageHandler, bool disposeHandler = true)
        {
            _httpClient = new HttpClient(httpMessageHandler ?? throw new ArgumentNullException(nameof(httpMessageHandler)), disposeHandler);
            _disposeClient = true;
        }

        /// <summary>
        /// http快速访问客户端<para/>
        /// 封装 <see cref="HttpClient"/> 用于http请求
        /// </summary>
        public HttpTurboClient()
        {
            _disposeClient = true;

#pragma warning disable CA2000 // 丢失范围之前释放对象
            _httpClient = new HttpClient(CreateDefaultClientHandler());
#pragma warning restore CA2000 // 丢失范围之前释放对象

            var defaultHeaders = _httpClient.DefaultRequestHeaders;
            foreach (var item in HttpRequestOptions.DefaultHttpHeaders)
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
            Debug.WriteLine($"终结器:{nameof(HttpTurboClient)}_{GetHashCode()}");

            if (_disposeClient)
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
            if (_disposeClient)
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
        public async Task<HttpResponseMessage> ExecuteAsync(IHttpRequest request) => await ExecuteAsync(request, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false);

        /// <summary>
        /// 执行请求
        /// </summary>
        /// <param name="request"></param>
        /// <param name="completionOption"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async Task<HttpResponseMessage> ExecuteAsync(IHttpRequest request, HttpCompletionOption completionOption)
        {
            if (request.Timeout.HasValue)
            {
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(request.Token);
                cts.CancelAfter(request.Timeout.Value);
                return await InternalExecuteAsync(request, completionOption, cts.Token).ConfigureAwait(false);
            }
            else
            {
                return await InternalExecuteAsync(request, completionOption, request.Token).ConfigureAwait(false);
            }
        }

        #region Internal

        private async Task<HttpResponseMessage> InternalExecuteAsync(IHttpRequest request, HttpCompletionOption completionOption, CancellationToken token)
        {
            if (request.AllowRedirection)
            {
                Uri? redirectUri;
                HttpResponseMessage tmpResponse = null!;
                var innerRequest = request.AsRequest();

                for (int i = 0; i <= request.MaxAutomaticRedirections; i++)
                {
                    tmpResponse = await _httpClient.SendAsync(innerRequest, completionOption, token).ConfigureAwait(false);

                    if ((redirectUri = tmpResponse.GetUriForRedirect(request.RequestUri)) != null)
                    {
                        var redirectRequest = new HttpRequestMessage(HttpMethod.Get, redirectUri);

                        foreach (var item in innerRequest.Headers)
                        {
                            redirectRequest.Headers.Add(item.Key, item.Value);
                        }

                        if (tmpResponse.TryGetCookie(out var setCookie))
                        {
                            if (innerRequest.TryGetCookie(out var cookie)
                                && !string.IsNullOrWhiteSpace(cookie))
                            {
                                cookie = CookieUtility.Merge(cookie, setCookie);
                            }
                            else
                            {
                                cookie = CookieUtility.Clean(setCookie);
                            }

                            redirectRequest.Headers.Remove(HttpHeaderDefinitions.Cookie);
                            redirectRequest.Headers.TryAddWithoutValidation(HttpHeaderDefinitions.Cookie, cookie);
                        }

                        innerRequest.Dispose();
                        tmpResponse.Dispose();

                        innerRequest = redirectRequest;
                        continue;
                    }
                    innerRequest.Dispose();
                    break;
                }
                return tmpResponse;
            }
            else
            {
                return await _httpClient.SendAsync(request.AsRequest(), completionOption, token).ConfigureAwait(false);
            }
        }

        #endregion Internal

        #region 静态方法

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