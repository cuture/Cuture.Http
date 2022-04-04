using System;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Cuture.Http.Util;

namespace Cuture.Http
{
    /// <summary>
    /// <see cref="IHttpRequest"/> 执行请求相关拓展方法
    /// </summary>
    public static class IHttpRequestExecutionExtensions
    {
        #region 方法

        /// <summary>
        /// 根据请求获取一个用以执行请求的 <see cref="HttpMessageInvoker"/>
        /// </summary>
        /// <param name="request">请求</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static HttpMessageInvoker InternalGetHttpMessageInvoker(IHttpRequest request)
        {
            var options = request.IsSetOptions ? request.ExecutionOptions : HttpRequestExecutionOptions.Default;
            return options.MessageInvoker
                        ?? options.MessageInvokerFactory?.GetInvoker(request)
                        ?? throw new ArgumentException($"HttpRequestOptions's {nameof(HttpRequestExecutionOptions.MessageInvoker)}、{nameof(HttpRequestExecutionOptions.MessageInvokerFactory)} cannot both be null.");
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

            return request.ExecuteAsync();
        }

        internal static Task<HttpResponseMessage> ExecuteAsync(this HttpMessageInvoker messageInvoker, IHttpRequest request)
        {
            if (request.Timeout.HasValue)
            {
                var cts = CancellationTokenSource.CreateLinkedTokenSource(request.Token);
                cts.CancelAfter(request.Timeout.Value);

                var task = request.AllowRedirection
                            ? messageInvoker.InternalExecuteWithAutoRedirectCoreAsync(request, cts.Token)
                            : messageInvoker.SendAsync(request.GetHttpRequestMessage(), cts.Token);

                task.ContinueWith(DisposeCancellationTokenSourceInTaskState, cts, TaskContinuationOptions.None);

                return task;
            }
            else
            {
                return request.AllowRedirection
                        ? messageInvoker.InternalExecuteWithAutoRedirectCoreAsync(request, request.Token)
                        : messageInvoker.SendAsync(request.GetHttpRequestMessage(), request.Token);
            }
        }

        internal static async Task<HttpResponseMessage> InternalExecuteWithAutoRedirectCoreAsync(this HttpMessageInvoker messageInvoker, IHttpRequest request, CancellationToken token)
        {
            Uri? redirectUri;
            HttpResponseMessage tmpResponse = null!;
            var innerRequest = request.GetHttpRequestMessage();

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
        /// 将 <paramref name="content"/> 使用 请求设置的 JsonSerializer 或 <see cref="HttpRequestGlobalOptions.DefaultJsonSerializer"/> 序列化为json字符串后Post
        /// </summary>
        /// <param name="request"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<HttpResponseMessage> PostJsonAsync(this IHttpRequest request, object content)
                => request.UsePost().WithJsonContent(content).ExecuteAsync();

        /// <summary>
        /// 将 <paramref name="content"/> 使用 <paramref name="jsonSerializer"/> 序列化为json字符串后Post
        /// </summary>
        /// <param name="request"></param>
        /// <param name="content"></param>
        /// <param name="jsonSerializer"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<HttpResponseMessage> PostJsonAsync(this IHttpRequest request, object content, IJsonSerializer jsonSerializer)
                => request.UsePost().WithJsonContent(content, jsonSerializer).ExecuteAsync();

        /// <summary>
        /// 将字符串 <paramref name="json"/> 以 <see cref="JsonContent"/> 进行Post
        /// </summary>
        /// <param name="request"></param>
        /// <param name="json"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<HttpResponseMessage> PostJsonAsync(this IHttpRequest request, string json)
                => request.UsePost().WithJsonContent(json).ExecuteAsync();

        #endregion Json

        #region Form

        /// <summary>
        /// 将 <paramref name="content"/> 使用 <see cref="HttpRequestGlobalOptions.DefaultFormDataFormatter"/> 转化为kv字符串,进行UrlEncoded后Post
        /// </summary>
        /// <param name="request"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<HttpResponseMessage> PostFormAsync(this IHttpRequest request, object content)
                => request.UsePost().WithFormContent(content).ExecuteAsync();

        /// <summary>
        /// 将 <paramref name="content"/> 使用 <paramref name="formatter"/> 转化为kv字符串,进行UrlEncoded后Post
        /// </summary>
        /// <param name="request"></param>
        /// <param name="content"></param>
        /// <param name="formatter"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<HttpResponseMessage> PostFormAsync(this IHttpRequest request, object content, IFormDataFormatter formatter)
                => request.UsePost().WithFormContent(content, formatter).ExecuteAsync();

        /// <summary>
        /// 将 <paramref name="content"/> 进行UrlEncoded后Post
        /// </summary>
        /// <param name="request"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<HttpResponseMessage> PostFormAsync(this IHttpRequest request, string content)
                => request.UsePost().WithFormContent(content).ExecuteAsync();

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

        #region json as jsonDocument

        /// <summary>
        /// 执行请求并以 json 接收返回数据，并解析为 <see cref="JsonDocument"/> 对象
        /// </summary>
        /// <param name="request"></param>
        /// <param name="jsonDocumentOptions"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<JsonDocument?> GetAsJsonDocumentAsync(this IHttpRequest request, JsonDocumentOptions jsonDocumentOptions = default) => request.ExecuteAsync().ReceiveAsJsonDocumentAsync(jsonDocumentOptions);

        /// <summary>
        /// 执行请求并尝试以 json 接收返回数据，并解析为 <see cref="JsonDocument"/> 对象
        /// </summary>
        /// <param name="request"></param>
        /// <param name="jsonDocumentOptions"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<TextHttpOperationResult<JsonDocument>> TryGetAsJsonDocumentAsync(this IHttpRequest request, JsonDocumentOptions jsonDocumentOptions = default) => request.ExecuteAsync().TryReceiveAsJsonDocumentAsync(jsonDocumentOptions);

        #endregion json as jsonDocument

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
                => request.ExecuteAsync().ReceiveAsObjectAsync<T>(request.GetJsonSerializerOrDefault());

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
                => request.ExecuteAsync().TryReceiveAsObjectAsync<T>(request.GetJsonSerializerOrDefault());

        #endregion json as object

        #endregion Result

        #region Download

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
        public static Task DownloadToStreamAsync(this IHttpRequest request, Stream targetStream, int bufferSize = HttpRequestGlobalOptions.DefaultDownloadBufferSize)
                => request.ExecuteAsync(HttpCompletionOption.ResponseHeadersRead)
                          .SetRequestContinueTaskWithTimeout(request, (requestTask, token) => requestTask.DownloadToStreamAsync(targetStream, token, bufferSize));

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
        public static Task DownloadToStreamWithProgressAsync(this IHttpRequest request, Func<long?, long, Task> progressCallback, Stream targetStream, int bufferSize = HttpRequestGlobalOptions.DefaultDownloadBufferSize)
                => request.ExecuteAsync(HttpCompletionOption.ResponseHeadersRead)
                          .SetRequestContinueTaskWithTimeout(request, (requestTask, token) => requestTask.DownloadToStreamWithProgressAsync(targetStream, progressCallback, token, bufferSize));

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
        public static Task DownloadToStreamWithProgressAsync(this IHttpRequest request, Action<long?, long> progressCallback, Stream targetStream, int bufferSize = HttpRequestGlobalOptions.DefaultDownloadBufferSize)
                => request.ExecuteAsync(HttpCompletionOption.ResponseHeadersRead)
                          .SetRequestContinueTaskWithTimeout(request, (requestTask, token) => requestTask.DownloadToStreamWithProgressAsync(targetStream, progressCallback, token, bufferSize));

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
        public static Task<byte[]> DownloadWithProgressAsync(this IHttpRequest request, Func<long?, long, Task> progressCallback, int bufferSize = HttpRequestGlobalOptions.DefaultDownloadBufferSize)
                => request.ExecuteAsync(HttpCompletionOption.ResponseHeadersRead)
                          .SetRequestContinueTaskWithTimeout(request, (requestTask, token) => requestTask.DownloadWithProgressAsync(progressCallback, token, bufferSize));

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
        public static Task<byte[]> DownloadWithProgressAsync(this IHttpRequest request, Action<long?, long> progressCallback, int bufferSize = HttpRequestGlobalOptions.DefaultDownloadBufferSize)
                => request.ExecuteAsync(HttpCompletionOption.ResponseHeadersRead)
                          .SetRequestContinueTaskWithTimeout(request, (requestTask, token) => requestTask.DownloadWithProgressAsync(progressCallback, token, bufferSize));

        #endregion Download

        #endregion 执行请求

        #region Private 方法

        private static void DisposeCancellationTokenSourceInTaskState(Task _, object? state) => (state as CancellationTokenSource)!.Dispose();

        private static TTaskResult SetRequestContinueTaskWithTimeout<TTaskResult>(this Task<HttpResponseMessage> requestTask, IHttpRequest request, Func<Task<HttpResponseMessage>, CancellationToken, TTaskResult> createContinueTaskFunc)
                    where TTaskResult : Task
        {
            if (request.Timeout.HasValue)
            {
                var cts = CancellationTokenSource.CreateLinkedTokenSource(request.Token);
                cts.CancelAfter(request.Timeout.Value);

                var task = createContinueTaskFunc(requestTask, cts.Token);

                task.ContinueWith(DisposeCancellationTokenSourceInTaskState, cts, TaskContinuationOptions.None);

                return task;
            }

            return createContinueTaskFunc(requestTask, request.Token);
        }

        #endregion Private 方法
    }
}