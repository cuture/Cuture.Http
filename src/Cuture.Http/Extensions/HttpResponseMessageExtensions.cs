using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

#if NETCOREAPP

using System.Buffers;
using System.Text.Json;

#endif

namespace Cuture.Http
{
    /// <summary>
    /// HttpResponseMessage拓展方法
    /// </summary>
    public static class HttpResponseMessageExtensions
    {
        #region Task<HttpResponseMessage>

        #region bytes

        /// <summary>
        /// 以
        /// <see cref="T:byte[]"/>
        /// 接收返回数据
        /// </summary>
        /// <param name="requestTask"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<byte[]> ReceiveAsBytesAsync(this Task<HttpResponseMessage> requestTask)
        {
            using var response = await requestTask.ConfigureAwait(false);
            return await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// 尝试以
        /// <see cref="T:byte[]"/>
        /// 接收返回数据
        /// </summary>
        /// <param name="requestTask"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<HttpOperationResult<byte[]>> TryReceiveAsBytesAsync(this Task<HttpResponseMessage> requestTask)
        {
            var result = new HttpOperationResult<byte[]>();
            try
            {
                result.ResponseMessage = await requestTask.ConfigureAwait(false);
                result.Data = await result.ResponseMessage.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
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
        /// 以
        /// <see cref="string"/>
        /// 接收返回数据
        /// </summary>
        /// <param name="requestTask"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<string> ReceiveAsStringAsync(this Task<HttpResponseMessage> requestTask)
        {
            using var response = await requestTask.ConfigureAwait(false);
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// 尝试以
        /// <see cref="string"/>
        /// 接收返回数据
        /// </summary>
        /// <param name="requestTask"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<HttpOperationResult<string>> TryReceiveAsStringAsync(this Task<HttpResponseMessage> requestTask)
        {
            var result = new HttpOperationResult<string>();
            try
            {
                result.ResponseMessage = await requestTask.ConfigureAwait(false);
                result.Data = await result.ResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }
            return result;
        }

        #endregion String

#if NETCOREAPP

        #region json as JsonDocument

        /// <summary>
        /// 以 json 接收返回数据，并解析为 <see cref="JsonDocument"/> 对象
        /// </summary>
        /// <param name="requestTask"></param>
        /// <param name="jsonDocumentOptions"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<JsonDocument?> ReceiveAsJsonDocumentAsync(this Task<HttpResponseMessage> requestTask, JsonDocumentOptions jsonDocumentOptions = default)
        {
            using var response = await requestTask.ConfigureAwait(false);
            return await response.ReceiveAsJsonDocumentAsync(jsonDocumentOptions).ConfigureAwait(false);
        }

        /// <summary>
        /// 尝试以 json 接收返回数据，并解析为 <see cref="JsonDocument"/> 对象
        /// </summary>
        /// <param name="requestTask"></param>
        /// <param name="jsonDocumentOptions"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<TextHttpOperationResult<JsonDocument>> TryReceiveAsJsonDocumentAsync(this Task<HttpResponseMessage> requestTask, JsonDocumentOptions jsonDocumentOptions = default)
        {
            var result = new TextHttpOperationResult<JsonDocument>();
            try
            {
                result.ResponseMessage = await requestTask.ConfigureAwait(false);

                var json = await result.ResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                result.Text = json;

                if (!string.IsNullOrEmpty(json))
                {
                    result.Data = JsonDocument.Parse(json, jsonDocumentOptions);
                }
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }
            return result;
        }

        #endregion json as JsonDocument

#endif

        #region json as object

        /// <summary>
        /// 以 json 接收返回数据，并解析为类型
        /// <typeparamref name="T"/>
        /// 的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestTask"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<T?> ReceiveAsObjectAsync<T>(this Task<HttpResponseMessage> requestTask, ISerializer<string>? serializer = null)
        {
            using var response = await requestTask.ConfigureAwait(false);
            return await response.ReceiveAsObjectAsync<T>(serializer ?? HttpRequestGlobalOptions.DefaultJsonSerializer).ConfigureAwait(false);
        }

        /// <summary>
        /// 尝试以 json 接收返回数据，并解析为类型
        /// <typeparamref name="T"/>
        /// 的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestTask"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<TextHttpOperationResult<T>> TryReceiveAsObjectAsync<T>(this Task<HttpResponseMessage> requestTask, ISerializer<string>? serializer = null)
        {
            var result = new TextHttpOperationResult<T>();
            try
            {
                result.ResponseMessage = await requestTask.ConfigureAwait(false);
                var json = await result.ResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                result.Text = json;

                if (!string.IsNullOrEmpty(json))
                {
                    result.Data = (serializer ?? HttpRequestGlobalOptions.DefaultJsonSerializer).Deserialize<T>(json);
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
        public static async Task DownloadToStreamAsync(this Task<HttpResponseMessage> requestTask,
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

            using var response = await requestTask.ConfigureAwait(false);

            var contentLength = response.Content.Headers.ContentLength;
#if NETCOREAPP
            using var stream =
#if NET
        await response.Content.ReadAsStreamAsync(token).ConfigureAwait(false);
#else
        await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
#endif
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
#else
            using var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            var buffer = new byte[bufferSize];
            var count = 0L;

            while (!token.IsCancellationRequested
                   && (contentLength == null || count < contentLength.Value))
            {
                var size = await stream.ReadAsync(buffer, 0, bufferSize, token).ConfigureAwait(false);
                if (size == 0)
                {
                    break;
                }
                count += size;
                await targetStream.WriteAsync(buffer, 0, size, token).ConfigureAwait(false);
            }
#endif
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
        public static async Task DownloadToStreamWithProgressAsync(this Task<HttpResponseMessage> requestTask,
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

            using var response = await requestTask.ConfigureAwait(false);

            var contentLength = response.Content.Headers.ContentLength;
#if NETCOREAPP
            using var stream =
#if NET
        await response.Content.ReadAsStreamAsync(token).ConfigureAwait(false);
#else
        await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
#endif
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
#else
            using var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

            var buffer = new byte[bufferSize];
            var count = 0L;

            await progressCallback(contentLength, count).ConfigureAwait(false);

            while (!token.IsCancellationRequested
                   && (contentLength == null || count < contentLength.Value))
            {
                var size = await stream.ReadAsync(buffer, 0, bufferSize, token).ConfigureAwait(false);
                if (size == 0)
                {
                    break;
                }
                count += size;
                await targetStream.WriteAsync(buffer, 0, size, token).ConfigureAwait(false);

                await progressCallback(contentLength, count).ConfigureAwait(false);
            }
#endif
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
        public static async Task<byte[]> DownloadWithProgressAsync(this Task<HttpResponseMessage> requestTask,
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
        public static async Task DownloadToStreamWithProgressAsync(this Task<HttpResponseMessage> requestTask,
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

            using var response = await requestTask.ConfigureAwait(false);

            token.ThrowIfCancellationRequested();

            var contentLength = response.Content.Headers.ContentLength;
#if NETCOREAPP
            using var stream =
#if NET
        await response.Content.ReadAsStreamAsync(token).ConfigureAwait(false);
#else
        await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
#endif
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
#else
            using var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            var buffer = new byte[bufferSize];
            var count = 0L;

            progressCallback(contentLength, count);

            while (!token.IsCancellationRequested
                   && (contentLength == null || count < contentLength.Value))
            {
                var size = await stream.ReadAsync(buffer, 0, bufferSize, token).ConfigureAwait(false);
                if (size == 0)
                {
                    break;
                }
                count += size;
                await targetStream.WriteAsync(buffer, 0, size, token).ConfigureAwait(false);

                progressCallback(contentLength, count);
            }
#endif
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
        public static async Task<byte[]> DownloadWithProgressAsync(this Task<HttpResponseMessage> requestTask,
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
        public static string GetCookie(this HttpResponseMessage responseMessage) => responseMessage.Headers.TryGetValues(HttpHeaderDefinitions.SetCookie, out var cookies) ? string.Join("; ", cookies) : string.Empty;

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

            if (string.Equals(requestUri.Scheme, "https", StringComparison.OrdinalIgnoreCase)
                && string.Equals(location.Scheme, "https", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

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
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<byte[]> ReceiveAsBytesAsync(this HttpResponseMessage responseMessage)
        {
            using (responseMessage)
            {
                return await responseMessage.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// 获取请求返回的json对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="responseMessage"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<T?> ReceiveAsObjectAsync<T>(this HttpResponseMessage responseMessage, ISerializer<string>? serializer = null)
        {
            using (responseMessage)
            {
                var json = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (!string.IsNullOrEmpty(json))
                {
                    return (serializer ?? HttpRequestGlobalOptions.DefaultJsonSerializer).Deserialize<T>(json);
                }
                return default;
            }
        }

#if NETCOREAPP

        /// <summary>
        /// 获取请求返回值并转换为<see cref="JsonDocument"/>对象
        /// </summary>
        /// <param name="responseMessage"></param>
        /// <param name="jsonDocumentOptions"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<JsonDocument?> ReceiveAsJsonDocumentAsync(this HttpResponseMessage responseMessage, JsonDocumentOptions jsonDocumentOptions = default)
        {
            using (responseMessage)
            {
                var stream = await responseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
                return await JsonDocument.ParseAsync(stream, jsonDocumentOptions).ConfigureAwait(false);
            }
        }

#endif

        /// <summary>
        /// 获取请求返回的字符串
        /// </summary>
        /// <param name="responseMessage"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<string> ReceiveAsStringAsync(this HttpResponseMessage responseMessage)
        {
            using (responseMessage)
            {
                return await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
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
}