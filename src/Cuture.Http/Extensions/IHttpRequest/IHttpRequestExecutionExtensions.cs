using System;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Cuture.Http.Util;

namespace Cuture.Http;

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
    /// <param name="invoker"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static IOwner<HttpMessageInvoker>? InternalGetHttpMessageInvoker(IHttpRequest request, out HttpMessageInvoker invoker)
    {
        var options = request.IsSetOptions ? request.ExecutionOptions : HttpRequestExecutionOptions.Default;
        if (options.MessageInvoker is not null)
        {
            invoker = options.MessageInvoker;
            return null;
        }
        var owner = options.MessageInvokerPool?.Rent(request);
        if (owner is not null)
        {
            invoker = owner.Value;
            return owner;
        }
        throw new ArgumentException($"HttpRequestOptions's {nameof(HttpRequestExecutionOptions.MessageInvoker)}、{nameof(HttpRequestExecutionOptions.MessageInvokerPool)} cannot both be null.");
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
    public static async Task<HttpRequestExecuteState> ExecuteAsync(this IHttpRequest request)
    {
        var invokerOwner = InternalGetHttpMessageInvoker(request, out var messageInvoker);

        CancellationTokenSource? localTokenSource = null;

        try
        {
            var cancellationToken = request.Token;
            using var httpRequestMessage = await request.GetHttpRequestMessageAsync(cancellationToken).ConfigureAwait(false);
            if (request.Timeout > 0)
            {
                localTokenSource = CancellationTokenSource.CreateLinkedTokenSource(request.Token);
                localTokenSource.CancelAfter(request.Timeout.Value);
                cancellationToken = localTokenSource.Token;
            }

            var task = request.AllowRedirection
                       ? messageInvoker.InternalExecuteWithAutoRedirectCoreAsync(httpRequestMessage, request.MaxAutomaticRedirections, cancellationToken)
                       : messageInvoker.SendAsync(httpRequestMessage, cancellationToken);

            var responseMessage = await task.ConfigureAwait(false);

            return new(responseMessage, invokerOwner);
        }
        catch
        {
            invokerOwner?.Dispose();
            throw;
        }
        finally
        {
            localTokenSource?.Dispose();
        }
    }

    internal static async Task<HttpResponseMessage> InternalExecuteWithAutoRedirectCoreAsync(this HttpMessageInvoker messageInvoker, HttpRequestMessage httpRequestMessage, int maxAutomaticRedirections, CancellationToken token)
    {
        Uri? redirectUri;
        HttpResponseMessage tmpResponse = null!;
        var innerRequest = httpRequestMessage;

        for (int i = 0; i <= maxAutomaticRedirections; i++)
        {
            tmpResponse = await messageInvoker.SendAsync(innerRequest, token).ConfigureAwait(false);

            if ((redirectUri = tmpResponse.GetUriForRedirect(httpRequestMessage.RequestUri!)) != null)
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
    public static Task<HttpRequestExecuteState> PostJsonAsync(this IHttpRequest request, object content)
            => request.UsePost().WithJsonContent(content).ExecuteAsync();

    /// <summary>
    /// 将 <paramref name="content"/> 使用 <paramref name="jsonSerializer"/> 序列化为json字符串后Post
    /// </summary>
    /// <param name="request"></param>
    /// <param name="content"></param>
    /// <param name="jsonSerializer"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<HttpRequestExecuteState> PostJsonAsync(this IHttpRequest request, object content, IJsonSerializer jsonSerializer)
            => request.UsePost().WithJsonContent(content, jsonSerializer).ExecuteAsync();

    /// <summary>
    /// 将字符串 <paramref name="json"/> 以 <see cref="JsonContent"/> 进行Post
    /// </summary>
    /// <param name="request"></param>
    /// <param name="json"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<HttpRequestExecuteState> PostJsonAsync(this IHttpRequest request, string json)
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
    public static Task<HttpRequestExecuteState> PostFormAsync(this IHttpRequest request, object content)
            => request.UsePost().WithFormContent(content).ExecuteAsync();

    /// <summary>
    /// 将 <paramref name="content"/> 使用 <paramref name="formatter"/> 转化为kv字符串,进行UrlEncoded后Post
    /// </summary>
    /// <param name="request"></param>
    /// <param name="content"></param>
    /// <param name="formatter"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<HttpRequestExecuteState> PostFormAsync(this IHttpRequest request, object content, IFormDataFormatter formatter)
            => request.UsePost().WithFormContent(content, formatter).ExecuteAsync();

    /// <summary>
    /// 将 <paramref name="content"/> 进行UrlEncoded后Post
    /// </summary>
    /// <param name="request"></param>
    /// <param name="content"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<HttpRequestExecuteState> PostFormAsync(this IHttpRequest request, string content)
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
    public static Task<byte[]> GetAsBytesAsync(this IHttpRequest request) => request.ExecuteAsync().ReceiveAsBytesAsync(request.Token);

    /// <summary>
    /// 执行请求并尝试以
    /// <see cref="T:byte[]"/>
    /// 接收返回数据
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<HttpOperationResult<byte[]>> TryGetAsBytesAsync(this IHttpRequest request) => request.ExecuteAsync().TryReceiveAsBytesAsync(request.Token);

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
    public static Task<string> GetAsStringAsync(this IHttpRequest request) => request.ExecuteAsync().ReceiveAsStringAsync(request.Token);

    /// <summary>
    /// 执行请求并尝试以
    /// <see cref="string"/>
    /// 接收返回数据
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<HttpOperationResult<string>> TryGetAsStringAsync(this IHttpRequest request) => request.ExecuteAsync().TryReceiveAsStringAsync(request.Token);

    #endregion String

    #region json as jsonDocument

    /// <summary>
    /// 执行请求并以 json 接收返回数据，并解析为 <see cref="JsonDocument"/> 对象
    /// </summary>
    /// <param name="request"></param>
    /// <param name="jsonDocumentOptions"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<JsonDocument?> GetAsJsonDocumentAsync(this IHttpRequest request, JsonDocumentOptions jsonDocumentOptions = default) => request.ExecuteAsync().ReceiveAsJsonDocumentAsync(jsonDocumentOptions, request.Token);

    /// <summary>
    /// 执行请求并尝试以 json 接收返回数据，并解析为 <see cref="JsonDocument"/> 对象
    /// </summary>
    /// <param name="request"></param>
    /// <param name="jsonDocumentOptions"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<TextHttpOperationResult<JsonDocument>> TryGetAsJsonDocumentAsync(this IHttpRequest request, JsonDocumentOptions jsonDocumentOptions = default) => request.ExecuteAsync().TryReceiveAsJsonDocumentAsync(jsonDocumentOptions, false, request.Token);

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
            => request.ExecuteAsync().ReceiveAsObjectAsync<T>(request.GetJsonSerializerOrDefault(), request.Token);

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
            => request.ExecuteAsync().TryReceiveAsObjectAsync<T>(request.GetJsonSerializerOrDefault(), false, request.Token);

    #endregion json as object

    #region json as DynamicJson

    /// <summary>
    /// 执行请求并以 json 接收返回数据，并解析为可动态访问的 dynamic 对象
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<dynamic?> GetAsDynamicJsonAsync(this IHttpRequest request) => request.ExecuteAsync().ReceiveAsDynamicJsonAsync(request.Token);

    /// <summary>
    /// 执行请求并尝试以 json 接收返回数据，并解析为可动态访问的 dynamic 对象
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<TextHttpOperationResult<dynamic>> TryGetAsDynamicJsonAsync(this IHttpRequest request) => request.ExecuteAsync().TryReceiveAsDynamicJsonAsync(false, request.Token);

    #endregion json as DynamicJson

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
            => request.ExecuteAsync()
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
            => request.ExecuteAsync()
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
            => request.ExecuteAsync()
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
            => request.ExecuteAsync()
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
            => request.ExecuteAsync()
                      .SetRequestContinueTaskWithTimeout(request, (requestTask, token) => requestTask.DownloadWithProgressAsync(progressCallback, token, bufferSize));

    #endregion Download

    #endregion 执行请求

    #region Private 方法

    private static TTaskResult SetRequestContinueTaskWithTimeout<TTaskResult>(this Task<HttpRequestExecuteState> requestTask, IHttpRequest request, Func<Task<HttpRequestExecuteState>, CancellationToken, TTaskResult> createContinueTaskFunc)
        where TTaskResult : Task
    {
        if (request.Timeout > 0)
        {
            var cts = CancellationTokenSource.CreateLinkedTokenSource(request.Token);
            cts.CancelAfter(request.Timeout.Value);

            var task = createContinueTaskFunc(requestTask, cts.Token);

            task.DisposeAfterTask(cts);

            return task;
        }

        return createContinueTaskFunc(requestTask, request.Token);
    }

    #endregion Private 方法
}
