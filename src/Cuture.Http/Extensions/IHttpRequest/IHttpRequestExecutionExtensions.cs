using System;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Cuture.Http.Util;

using Newtonsoft.Json.Linq;

namespace Cuture.Http
{
    /// <summary>
    /// <see cref="IHttpRequest"/> 执行请求相关拓展方法
    /// </summary>
    public static class IHttpRequestExecutionExtensions
    {
        #region 构造函数

#if NETSTANDARD || NETCOREAPP3_1

        static IHttpRequestExecutionExtensions()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

#endif

        #endregion 构造函数

        #region 方法

        /// <summary>
        /// 根据请求获取一个用以执行请求的 <see cref="HttpMessageInvoker"/>
        /// </summary>
        /// <param name="request">请求</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static HttpMessageInvoker InternalGetHttpMessageInvoker(IHttpRequest request)
        {
            var options = request.IsSetOptions ? request.RequestOptions : HttpRequestOptions.Default;
            return options.MessageInvoker
                        ?? options.MessageInvokerFactory?.GetInvoker(request)
                        ?? throw new ArgumentException($"HttpRequestOptions's {nameof(HttpRequestOptions.MessageInvoker)}、{nameof(HttpRequestOptions.MessageInvokerFactory)} cannot both be null.");
        }

        #endregion 方法

        #region 执行请求

        #region Execute

        /// <summary>
        /// 执行请求
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<HttpResponseMessage> ExecuteAsync(this IHttpRequest request) => InternalGetHttpMessageInvoker(request).ExecuteAsync(request);

        /// <summary>
        /// 执行请求
        /// </summary>
        /// <param name="request"></param>
        /// <param name="completionOption"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<HttpResponseMessage> ExecuteAsync(this IHttpRequest request, HttpCompletionOption completionOption)
        {
            if (completionOption == HttpCompletionOption.ResponseHeadersRead)
            {
                if (InternalGetHttpMessageInvoker(request) is HttpClient httpClient)
                {
                    //TODO pool wrapper
                    return new CompletionWithHeadersReadClientWrapper(httpClient, false).ExecuteAsync(request);
                }
                throw new NotSupportedException($"Only {nameof(HttpClient)} support {nameof(HttpCompletionOption)}.{nameof(HttpCompletionOption.ResponseHeadersRead)}.");
            }
            else
            {
                return request.ExecuteAsync();
            }
        }

        internal static Task<HttpResponseMessage> ExecuteAsync(this HttpMessageInvoker messageInvoker, IHttpRequest request)
        {
            var token = request.Token;
            CancellationTokenSource? cts = null;

            if (request.Timeout.HasValue)
            {
                cts = CancellationTokenSource.CreateLinkedTokenSource();
                cts.CancelAfter(request.Timeout.Value);
                token = cts.Token;
            }

            var task = request.AllowRedirection ? messageInvoker.InternalExecuteWithAutoRedirectCoreAsync(request, token) : messageInvoker.SendAsync(request.AsRequest(), token);

            if (cts != null)
            {
                task.ContinueWith((task, state) => (state as CancellationTokenSource)!.Dispose(), cts, TaskContinuationOptions.None);
            }

            return task;
        }

        internal static async Task<HttpResponseMessage> InternalExecuteWithAutoRedirectCoreAsync(this HttpMessageInvoker messageInvoker, IHttpRequest request, CancellationToken token)
        {
            Uri? redirectUri;
            HttpResponseMessage tmpResponse = null!;
            var innerRequest = request.AsRequest();

            for (int i = 0; i <= request.MaxAutomaticRedirections; i++)
            {
                tmpResponse = await messageInvoker.SendAsync(innerRequest, token).ConfigureAwait(false);

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

        #endregion Execute

        #region Json

        /// <summary>
        /// 将
        /// <paramref name="content"/>
        /// 使用
        /// <see cref="Newtonsoft.Json.JsonConvert.SerializeObject(object)"/>
        /// 转换为json字符串后Post
        /// </summary>
        /// <param name="request"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<HttpResponseMessage> PostJsonAsync(this IHttpRequest request, object content)
        {
            if (request.IsSetOptions
                && request.RequestOptions.JsonSerializer != null)
            {
                request.Content = new JsonContent(content, JsonContent.ContentType, Encoding.UTF8, request.RequestOptions.JsonSerializer);
            }
            else
            {
                request.Content = new JsonContent(content);
            }
            request.Method = HttpMethod.Post;
            return ExecuteAsync(request);
        }

        /// <summary>
        /// 将字符串
        /// <paramref name="json"/>
        /// 以
        /// <see cref="JsonContent"/>
        /// 进行Post
        /// </summary>
        /// <param name="request"></param>
        /// <param name="json"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<HttpResponseMessage> PostJsonAsync(this IHttpRequest request, string json)
        {
            request.Content = new JsonContent(json);
            request.Method = HttpMethod.Post;
            return ExecuteAsync(request);
        }

        #endregion Json

        #region Form

        /// <summary>
        /// 将
        /// <paramref name="content"/>
        /// 转化为kv字符串,进行UrlEncoded后Post
        /// </summary>
        /// <param name="request"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<HttpResponseMessage> PostFormAsync(this IHttpRequest request, object content)
        {
            request.Content = new FormContent(content);
            request.Method = HttpMethod.Post;
            return ExecuteAsync(request);
        }

        /// <summary>
        /// 将
        /// <paramref name="content"/>
        /// 进行UrlEncoded后Post
        /// </summary>
        /// <param name="request"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<HttpResponseMessage> PostFormAsync(this IHttpRequest request, string content)
        {
            request.Content = new FormContent(content);
            request.Method = HttpMethod.Post;
            return ExecuteAsync(request);
        }

        #endregion Form

        #region Result

        #region bytes

        /// <summary>
        /// 执行请求并以
        /// <see cref="T:byte[]"/>
        /// 接收返回数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<byte[]> GetAsBytesAsync(this IHttpRequest request) => request.ExecuteAsync().ReceiveAsBytesAsync();

        /// <summary>
        /// 执行请求并尝试以
        /// <see cref="T:byte[]"/>
        /// 接收返回数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<HttpOperationResult<byte[]>> TryGetAsBytesAsync(this IHttpRequest request) => request.ExecuteAsync().TryReceiveAsBytesAsync();

        #endregion bytes

        #region String

        /// <summary>
        /// 执行请求并以
        /// <see cref="string"/>
        /// 接收返回数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<string> GetAsStringAsync(this IHttpRequest request) => request.ExecuteAsync().ReceiveAsStringAsync();

        /// <summary>
        /// 执行请求并尝试以
        /// <see cref="string"/>
        /// 接收返回数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<HttpOperationResult<string>> TryGetAsStringAsync(this IHttpRequest request) => request.ExecuteAsync().TryReceiveAsStringAsync();

        #endregion String

        #region json an JsonObject

        /// <summary>
        /// 执行请求并以 json 接收返回数据，并解析为
        /// <see cref="JObject"/>
        /// 对象
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<JObject?> GetAsJsonAsync(this IHttpRequest request) => request.ExecuteAsync().ReceiveAsJsonAsync();

        /// <summary>
        /// 执行请求并尝试以 json 接收返回数据，并解析为
        /// <see cref="JObject"/>
        /// 对象
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<TextHttpOperationResult<JObject>> TryGetAsJsonAsync(this IHttpRequest request) => request.ExecuteAsync().TryReceiveAsJsonAsync();

        #endregion json an JsonObject

        #region json as object

        /// <summary>
        /// 执行请求并以 json 接收返回数据，并解析为类型
        /// <typeparamref name="T"/>
        /// 的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<T?> GetAsObjectAsync<T>(this IHttpRequest request)
        {
            if (request.IsSetOptions
                && request.RequestOptions.JsonSerializer != null)
            {
                return request.ExecuteAsync().ReceiveAsObjectAsync<T>(request.RequestOptions.JsonSerializer);
            }
            else
            {
                return request.ExecuteAsync().ReceiveAsObjectAsync<T>(HttpRequestOptions.DefaultJsonSerializer);
            }
        }

        /// <summary>
        /// 执行请求并尝试以 json 接收返回数据，并解析为类型
        /// <typeparamref name="T"/>
        /// 的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<TextHttpOperationResult<T>> TryGetAsObjectAsync<T>(this IHttpRequest request)
        {
            if (request.IsSetOptions
                && request.RequestOptions.JsonSerializer != null)
            {
                return request.ExecuteAsync().TryReceiveAsObjectAsync<T>(request.RequestOptions.JsonSerializer);
            }
            else
            {
                return request.ExecuteAsync().TryReceiveAsObjectAsync<T>(HttpRequestOptions.DefaultJsonSerializer);
            }
        }

        #endregion json as object

        #endregion Result

        #region Download

        //TODO 检查Download方法中直接使用request.Token，是否不受超时限制

        /// <summary>
        /// 执行请求,将请求结果
        /// <see cref="HttpResponseMessage.Content"/>
        /// 内容直接写入目标流
        /// <paramref name="targetStream"/>
        /// </summary>
        /// <param name="request"></param>
        /// <param name="targetStream"></param>
        /// <param name="bufferSize"></param>
        /// <returns></returns>
        public static Task DownloadToStreamAsync(this IHttpRequest request, Stream targetStream, int bufferSize = HttpRequestOptions.DefaultDownloadBufferSize)
                => request.ExecuteAsync(HttpCompletionOption.ResponseHeadersRead)
                          .DownloadToStreamAsync(targetStream, request.Token, bufferSize);

        /// <summary>
        /// 执行请求,将请求结果
        /// <see cref="HttpResponseMessage.Content"/>
        /// 内容直接写入目标流
        /// <paramref name="targetStream"/>
        /// 并附带进度回调
        /// <paramref name="progressCallback"/>
        /// </summary>
        /// <param name="request"></param>
        /// <param name="progressCallback"></param>
        /// <param name="targetStream"></param>
        /// <param name="bufferSize"></param>
        /// <returns></returns>
        public static Task DownloadToStreamWithProgressAsync(this IHttpRequest request, Func<long?, long, Task> progressCallback, Stream targetStream, int bufferSize = HttpRequestOptions.DefaultDownloadBufferSize)
                => request.ExecuteAsync(HttpCompletionOption.ResponseHeadersRead)
                          .DownloadToStreamWithProgressAsync(targetStream, progressCallback, request.Token, bufferSize);

        /// <summary>
        /// 执行请求,将请求结果
        /// <see cref="HttpResponseMessage.Content"/>
        /// 内容直接写入目标流
        /// <paramref name="targetStream"/>
        /// 并附带进度回调
        /// <paramref name="progressCallback"/>
        /// </summary>
        /// <param name="request"></param>
        /// <param name="progressCallback"></param>
        /// <param name="targetStream"></param>
        /// <param name="bufferSize"></param>
        /// <returns></returns>
        public static Task DownloadToStreamWithProgressAsync(this IHttpRequest request, Action<long?, long> progressCallback, Stream targetStream, int bufferSize = HttpRequestOptions.DefaultDownloadBufferSize)
                => request.ExecuteAsync(HttpCompletionOption.ResponseHeadersRead)
                          .DownloadToStreamWithProgressAsync(targetStream, progressCallback, request.Token, bufferSize);

        /// <summary>
        /// 执行请求,将请求结果
        /// <see cref="HttpResponseMessage.Content"/>
        /// 下载为byte[],并附带进度回调
        /// <paramref name="progressCallback"/>
        /// </summary>
        /// <param name="request"></param>
        /// <param name="progressCallback"></param>
        /// <param name="bufferSize"></param>
        /// <returns></returns>
        public static Task<byte[]> DownloadWithProgressAsync(this IHttpRequest request, Func<long?, long, Task> progressCallback, int bufferSize = HttpRequestOptions.DefaultDownloadBufferSize)
                => request.ExecuteAsync(HttpCompletionOption.ResponseHeadersRead)
                          .DownloadWithProgressAsync(progressCallback, request.Token, bufferSize);

        /// <summary>
        /// 执行请求,将请求结果
        /// <see cref="HttpResponseMessage.Content"/>
        /// 下载为byte[],并附带进度回调
        /// <paramref name="progressCallback"/>
        /// </summary>
        /// <param name="request"></param>
        /// <param name="progressCallback"></param>
        /// <param name="bufferSize"></param>
        /// <returns></returns>
        public static Task<byte[]> DownloadWithProgressAsync(this IHttpRequest request, Action<long?, long> progressCallback, int bufferSize = HttpRequestOptions.DefaultDownloadBufferSize)
                => request.ExecuteAsync(HttpCompletionOption.ResponseHeadersRead)
                          .DownloadWithProgressAsync(progressCallback, request.Token, bufferSize);

        #endregion Download

        #endregion 执行请求
    }
}