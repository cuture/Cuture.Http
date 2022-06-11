using System;
using System.Buffers;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Cuture.Http;

/// <summary>
/// HttpResponseMessage拓展方法
/// </summary>
public static partial class HttpResponseMessageExtensions
{
    #region Task<HttpResponseMessage>

    /// <summary>
    /// 将 <see cref="HttpResponseMessage"/> 任务包装为 <see cref="HttpRequestExecuteState"/> 任务，以支持相关的后续处理拓展方法
    /// </summary>
    /// <param name="requestTask"></param>
    /// <returns></returns>
    public static async Task<HttpRequestExecuteState> WrapAsExecuteState(this Task<HttpResponseMessage> requestTask)
    {
        var responseMessage = await requestTask.ConfigureAwait(false);
        return new(responseMessage);
    }

    #region bytes

    /// <summary>
    /// 以
    /// <see cref="T:byte[]"/>
    /// 接收返回数据
    /// </summary>
    /// <param name="requestTask"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<byte[]> ReceiveAsBytesAsync(this Task<HttpRequestExecuteState> requestTask, CancellationToken cancellationToken = default)
    {
        using var executeState = await requestTask.ConfigureAwait(false);
        return await executeState.HttpResponseMessage.EnsureSuccessStatusCode()
                                                     .Content
                                                     .ReadAsByteArrayAsync(cancellationToken)
                                                     .ConfigureAwait(false);
    }

    /// <summary>
    /// 尝试以
    /// <see cref="T:byte[]"/>
    /// 接收返回数据
    /// </summary>
    /// <param name="requestTask"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<HttpOperationResult<byte[]>> TryReceiveAsBytesAsync(this Task<HttpRequestExecuteState> requestTask, CancellationToken cancellationToken = default)
    {
        var result = new HttpOperationResult<byte[]>();
        try
        {
            using var executeState = await requestTask.ConfigureAwait(false);

            var responseMessage = executeState.HttpResponseMessage;
            result.ResponseMessage = responseMessage;
            responseMessage.EnsureSuccessStatusCode();

            result.Data = await responseMessage.Content.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            result.Exception = ex;
        }
        return result;
    }

    #endregion bytes

    #region String

    /// <summary>
    /// 以 <see cref="string"/> 接收返回数据
    /// </summary>
    /// <param name="requestTask"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<string> ReceiveAsStringAsync(this Task<HttpRequestExecuteState> requestTask, CancellationToken cancellationToken = default)
    {
        using var executeState = await requestTask.ConfigureAwait(false);
        return await executeState.HttpResponseMessage.EnsureSuccessStatusCode()
                                                     .Content
                                                     .ReadAsStringAsync(cancellationToken)
                                                     .ConfigureAwait(false);
    }

    /// <summary>
    /// 尝试以 <see cref="string"/> 接收返回数据
    /// </summary>
    /// <param name="requestTask"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<HttpOperationResult<string>> TryReceiveAsStringAsync(this Task<HttpRequestExecuteState> requestTask, CancellationToken cancellationToken = default)
    {
        var result = new HttpOperationResult<string>();
        try
        {
            using var executeState = await requestTask.ConfigureAwait(false);

            var responseMessage = executeState.HttpResponseMessage;
            result.ResponseMessage = responseMessage;
            responseMessage.EnsureSuccessStatusCode();

            result.Data = await responseMessage.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            result.Exception = ex;
        }
        return result;
    }

    #endregion String

    #region json as JsonDocument

    /// <summary>
    /// 以 json 接收返回数据，并解析为 <see cref="JsonDocument"/> 对象
    /// </summary>
    /// <param name="requestTask"></param>
    /// <param name="jsonDocumentOptions"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<JsonDocument?> ReceiveAsJsonDocumentAsync(this Task<HttpRequestExecuteState> requestTask, JsonDocumentOptions jsonDocumentOptions = default, CancellationToken cancellationToken = default)
    {
        using var executeState = await requestTask.ConfigureAwait(false);
        return await executeState.HttpResponseMessage.EnsureSuccessStatusCode()
                                                     .ReceiveAsJsonDocumentAsync(jsonDocumentOptions, cancellationToken)
                                                     .ConfigureAwait(false);
    }

    /// <summary>
    /// 尝试以 json 接收返回数据，并解析为 <see cref="JsonDocument"/> 对象
    /// </summary>
    /// <param name="requestTask"></param>
    /// <param name="jsonDocumentOptions"></param>
    /// <param name="textRequired">需要原始文本</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<TextHttpOperationResult<JsonDocument>> TryReceiveAsJsonDocumentAsync(this Task<HttpRequestExecuteState> requestTask, JsonDocumentOptions jsonDocumentOptions = default, bool textRequired = false, CancellationToken cancellationToken = default)
    {
        var result = new TextHttpOperationResult<JsonDocument>();
        try
        {
            using var executeState = await requestTask.ConfigureAwait(false);

            var responseMessage = executeState.HttpResponseMessage;
            result.ResponseMessage = responseMessage;
            responseMessage.EnsureSuccessStatusCode();

            if (!textRequired)
            {
                result.Data = await responseMessage.ReceiveAsJsonDocumentAsync(jsonDocumentOptions, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                var json = await responseMessage.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                result.Text = json;

                if (!string.IsNullOrEmpty(json))
                {
                    result.Data = JsonDocument.Parse(json, jsonDocumentOptions);
                }
            }
        }
        catch (Exception ex)
        {
            result.Exception = ex;
        }
        return result;
    }

    #endregion json as JsonDocument

    #region json as object

    /// <summary>
    /// 以 json 接收返回数据，并解析为类型
    /// <typeparamref name="T"/>
    /// 的对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="requestTask"></param>
    /// <param name="serializer"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<T?> ReceiveAsObjectAsync<T>(this Task<HttpRequestExecuteState> requestTask, ISerializer<string>? serializer = null, CancellationToken cancellationToken = default)
    {
        using var executeState = await requestTask.ConfigureAwait(false);
        return await executeState.HttpResponseMessage.EnsureSuccessStatusCode()
                                                     .ReceiveAsObjectAsync<T>(serializer ?? HttpRequestGlobalOptions.DefaultJsonSerializer, cancellationToken)
                                                     .ConfigureAwait(false);
    }

    /// <summary>
    /// 尝试以 json 接收返回数据，并解析为类型
    /// <typeparamref name="T"/>
    /// 的对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="requestTask"></param>
    /// <param name="serializer"></param>
    /// <param name="textRequired">需要原始文本</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<TextHttpOperationResult<T>> TryReceiveAsObjectAsync<T>(this Task<HttpRequestExecuteState> requestTask, ISerializer<string>? serializer = null, bool textRequired = false, CancellationToken cancellationToken = default)
    {
        var result = new TextHttpOperationResult<T>();
        try
        {
            using var executeState = await requestTask.ConfigureAwait(false);

            var responseMessage = executeState.HttpResponseMessage;
            result.ResponseMessage = responseMessage;
            responseMessage.EnsureSuccessStatusCode();
            
            if (!textRequired)
            {
                result.Data = await responseMessage.ReceiveAsObjectAsync<T>(serializer, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                var json = await responseMessage.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                result.Text = json;

                if (!string.IsNullOrEmpty(json))
                {
                    result.Data = (serializer ?? HttpRequestGlobalOptions.DefaultJsonSerializer).Deserialize<T>(json);
                }
            }
        }
        catch (Exception ex)
        {
            result.Exception = ex;
        }
        return result;
    }

    #endregion json as object

    #region Download

    /// <summary>
    /// 执行请求
    /// <paramref name="requestTask"/>
    /// 将请求结果
    /// <see cref="HttpResponseMessage.Content"/>
    /// 内容直接写入目标流
    /// <paramref name="targetStream"/>
    /// </summary>
    /// <param name="requestTask"></param>
    /// <param name="targetStream"></param>
    /// <param name="token"></param>
    /// <param name="bufferSize"></param>
    /// <returns></returns>
    public static async Task DownloadToStreamAsync(this Task<HttpRequestExecuteState> requestTask,
                                                   Stream targetStream,
                                                   CancellationToken token,
                                                   int bufferSize = HttpRequestGlobalOptions.DefaultDownloadBufferSize)
    {
        if (requestTask is null)
        {
            throw new ArgumentNullException(nameof(requestTask));
        }

        if (bufferSize < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(bufferSize));
        }

        if (!targetStream.CanWrite)
        {
            throw new ArgumentException($"{nameof(targetStream)} must can write");
        }

        using var executeState = await requestTask.ConfigureAwait(false);

        using var response = executeState.HttpResponseMessage.EnsureSuccessStatusCode();

        var contentLength = response.Content.Headers.ContentLength;

        using var stream = await response.Content.ReadAsStreamAsync(token).ConfigureAwait(false);

        using var buffer = MemoryPool<byte>.Shared.Rent(bufferSize);

        var count = 0L;

        while (!token.IsCancellationRequested
               && (contentLength == null || count < contentLength.Value))
        {
            var size = await stream.ReadAsync(buffer.Memory, token).ConfigureAwait(false);
            if (size == 0)
            {
                break;
            }
            count += size;
            await targetStream.WriteAsync(buffer.Memory.Slice(0, size), token).ConfigureAwait(false);
        }
    }

    #region WithProgress

    #region AsyncCallback

    /// <summary>
    /// 执行请求
    /// <paramref name="requestTask"/>
    /// 将请求结果
    /// <see cref="HttpResponseMessage.Content"/>
    /// 内容直接写入目标流
    /// <paramref name="targetStream"/>
    /// 并附带进度回调
    /// <paramref name="progressCallback"/>
    /// </summary>
    /// <param name="requestTask"></param>
    /// <param name="targetStream"></param>
    /// <param name="progressCallback"></param>
    /// <param name="token"></param>
    /// <param name="bufferSize"></param>
    /// <returns></returns>
    public static async Task DownloadToStreamWithProgressAsync(this Task<HttpRequestExecuteState> requestTask,
                                                               Stream targetStream,
                                                               Func<long?, long, Task> progressCallback,
                                                               CancellationToken token,
                                                               int bufferSize = HttpRequestGlobalOptions.DefaultDownloadBufferSize)
    {
        if (requestTask is null)
        {
            throw new ArgumentNullException(nameof(requestTask));
        }

        if (progressCallback is null)
        {
            throw new ArgumentNullException(nameof(progressCallback));
        }

        if (bufferSize < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(bufferSize));
        }

        if (!targetStream.CanWrite)
        {
            throw new ArgumentException($"{nameof(targetStream)} must can write");
        }

        using var executeState = await requestTask.ConfigureAwait(false);

        using var response = executeState.HttpResponseMessage.EnsureSuccessStatusCode();

        var contentLength = response.Content.Headers.ContentLength;

        using var stream = await response.Content.ReadAsStreamAsync(token).ConfigureAwait(false);

        using var buffer = MemoryPool<byte>.Shared.Rent(bufferSize);

        var count = 0L;

        await progressCallback(contentLength, count).ConfigureAwait(false);

        while (!token.IsCancellationRequested
               && (contentLength == null || count < contentLength.Value))
        {
            var size = await stream.ReadAsync(buffer.Memory, token).ConfigureAwait(false);
            if (size == 0)
            {
                break;
            }
            count += size;
            await targetStream.WriteAsync(buffer.Memory.Slice(0, size), token).ConfigureAwait(false);

            await progressCallback(contentLength, count).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// 执行请求
    /// <paramref name="requestTask"/>
    /// 将请求结果
    /// <see cref="HttpResponseMessage.Content"/>
    /// 下载为byte[],并附带进度回调
    /// <paramref name="progressCallback"/>
    /// </summary>
    /// <param name="requestTask"></param>
    /// <param name="progressCallback"></param>
    /// <param name="token"></param>
    /// <param name="bufferSize"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<byte[]> DownloadWithProgressAsync(this Task<HttpRequestExecuteState> requestTask,
                                                               Func<long?, long, Task> progressCallback,
                                                               CancellationToken token,
                                                               int bufferSize = HttpRequestGlobalOptions.DefaultDownloadBufferSize)
    {
        using var mStream = new MemoryStream(512_000);

        await requestTask.DownloadToStreamWithProgressAsync(mStream, progressCallback, token, bufferSize).ConfigureAwait(false);

        return mStream.ToArray();
    }

    #endregion AsyncCallback

    #region SyncCallback

    /// <summary>
    /// 执行请求
    /// <paramref name="requestTask"/>
    /// 将请求结果
    /// <see cref="HttpResponseMessage.Content"/>
    /// 内容直接写入目标流
    /// <paramref name="targetStream"/>
    /// 并附带进度回调
    /// <paramref name="progressCallback"/>
    /// </summary>
    /// <param name="requestTask"></param>
    /// <param name="targetStream"></param>
    /// <param name="progressCallback"></param>
    /// <param name="token"></param>
    /// <param name="bufferSize"></param>
    /// <returns></returns>
    public static async Task DownloadToStreamWithProgressAsync(this Task<HttpRequestExecuteState> requestTask,
                                                               Stream targetStream,
                                                               Action<long?, long> progressCallback,
                                                               CancellationToken token,
                                                               int bufferSize = HttpRequestGlobalOptions.DefaultDownloadBufferSize)
    {
        if (requestTask is null)
        {
            throw new ArgumentNullException(nameof(requestTask));
        }

        if (progressCallback is null)
        {
            throw new ArgumentNullException(nameof(progressCallback));
        }

        if (bufferSize < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(bufferSize));
        }

        if (!targetStream.CanWrite)
        {
            throw new ArgumentException($"{nameof(targetStream)} must can write");
        }

        using var executeState = await requestTask.ConfigureAwait(false);

        using var response = executeState.HttpResponseMessage.EnsureSuccessStatusCode();

        var contentLength = response.Content.Headers.ContentLength;

        using var stream = await response.Content.ReadAsStreamAsync(token).ConfigureAwait(false);

        using var buffer = MemoryPool<byte>.Shared.Rent(bufferSize);

        var count = 0L;

        progressCallback(contentLength, count);

        while (!token.IsCancellationRequested
               && (contentLength == null || count < contentLength.Value))
        {
            var size = await stream.ReadAsync(buffer.Memory, token).ConfigureAwait(false);
            if (size == 0)
            {
                break;
            }
            count += size;
            await targetStream.WriteAsync(buffer.Memory.Slice(0, size), token).ConfigureAwait(false);

            progressCallback(contentLength, count);
        }

        token.ThrowIfCancellationRequested();
    }

    /// <summary>
    /// 执行请求
    /// <paramref name="requestTask"/>
    /// 将请求结果
    /// <see cref="HttpResponseMessage.Content"/>
    /// 下载为byte[],并附带进度回调
    /// <paramref name="progressCallback"/>
    /// </summary>
    /// <param name="requestTask"></param>
    /// <param name="progressCallback"></param>
    /// <param name="token"></param>
    /// <param name="bufferSize"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<byte[]> DownloadWithProgressAsync(this Task<HttpRequestExecuteState> requestTask,
                                                               Action<long?, long> progressCallback,
                                                               CancellationToken token,
                                                               int bufferSize = HttpRequestGlobalOptions.DefaultDownloadBufferSize)
    {
        using var mStream = new MemoryStream(512_000);

        await requestTask.DownloadToStreamWithProgressAsync(mStream, progressCallback, token, bufferSize).ConfigureAwait(false);

        return mStream.ToArray();
    }

    #endregion SyncCallback

    #endregion WithProgress

    #endregion Download

    #endregion Task<HttpResponseMessage>

    #region HttpResponseMessage

    /// <summary>
    /// 获取请求返回头的 Set-Cookie 字符串内容
    /// </summary>
    /// <param name="responseMessage"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetCookie(this HttpResponseMessage responseMessage)
        => responseMessage.Headers.TryGetValues(HttpHeaderDefinitions.SetCookie, out var cookies)
               ? string.Join("; ", cookies)
               : string.Empty;

    /// <summary>
    /// 获取获取响应的重定向Uri
    /// </summary>
    /// <param name="response"></param>
    /// <param name="requestUri">请求的Uri</param>
    /// <returns></returns>
    public static Uri? GetUriForRedirect(this HttpResponseMessage response, Uri requestUri)
    {
        switch (response.StatusCode)
        {
            case HttpStatusCode.Moved:
            case HttpStatusCode.Found:
            case HttpStatusCode.SeeOther:
            case HttpStatusCode.TemporaryRedirect:
            case HttpStatusCode.MultipleChoices:
                break;

            default:
                return null;
        }

        Uri? location = response.Headers.Location;
        if (location == null)
        {
            return null;
        }

        //记不清为什么有这个逻辑了。。。
        //if (string.Equals(requestUri.Scheme, "https", StringComparison.OrdinalIgnoreCase)
        //    && string.Equals(location.Scheme, "https", StringComparison.OrdinalIgnoreCase))
        //{
        //    return null;
        //}

        if (!location.IsAbsoluteUri)
        {
            location = new Uri(requestUri, location);
        }

        string requestFragment = requestUri.Fragment;
        if (!string.IsNullOrEmpty(requestFragment))
        {
            string redirectFragment = location.Fragment;
            if (string.IsNullOrEmpty(redirectFragment))
            {
                location = new UriBuilder(location) { Fragment = requestFragment }.Uri;
            }
        }

        return location;
    }

    #region ReceiveData

    /// <summary>
    /// 获取请求返回的字符串
    /// </summary>
    /// <param name="responseMessage"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<byte[]> ReceiveAsBytesAsync(this HttpResponseMessage responseMessage, CancellationToken cancellationToken = default)
    {
        using (responseMessage.EnsureSuccessStatusCode())
        {
            return await responseMessage.Content.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// 获取请求返回值并转换为<see cref="JsonDocument"/>对象
    /// </summary>
    /// <param name="responseMessage"></param>
    /// <param name="jsonDocumentOptions"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<JsonDocument?> ReceiveAsJsonDocumentAsync(this HttpResponseMessage responseMessage,
                                                                       JsonDocumentOptions jsonDocumentOptions = default,
                                                                       CancellationToken cancellationToken = default)
    {
        using (responseMessage.EnsureSuccessStatusCode())
        {
            using var stream = await responseMessage.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            return await JsonDocument.ParseAsync(stream, jsonDocumentOptions, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// 获取请求返回的json对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="responseMessage"></param>
    /// <param name="serializer"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<T?> ReceiveAsObjectAsync<T>(this HttpResponseMessage responseMessage,
                                                         ISerializer<string>? serializer = null,
                                                         CancellationToken cancellationToken = default)
    {
        using (responseMessage.EnsureSuccessStatusCode())
        {
            using var stream = await responseMessage.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            return await (serializer ?? HttpRequestGlobalOptions.DefaultJsonSerializer).DeserializeAsync<T>(stream, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// 获取请求返回的字符串
    /// </summary>
    /// <param name="responseMessage"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<string> ReceiveAsStringAsync(this HttpResponseMessage responseMessage, CancellationToken cancellationToken = default)
    {
        using (responseMessage.EnsureSuccessStatusCode())
        {
            return await responseMessage.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    #endregion ReceiveData

    /// <summary>
    /// 获取请求返回头的 Set-Cookie 字符串内容
    /// </summary>
    /// <param name="responseMessage"></param>
    /// <param name="cookie"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGetCookie(this HttpResponseMessage responseMessage, out string cookie)
    {
        if (responseMessage.Headers.TryGetValues(HttpHeaderDefinitions.SetCookie, out var cookies))
        {
            cookie = string.Join("; ", cookies);
            return true;
        }
        else
        {
            cookie = string.Empty;
            return false;
        }
    }

    #endregion HttpResponseMessage
}
