using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

#if NET5_0

using System.Buffers;

#endif

namespace Cuture.Http
{
    /// <summary>
    /// HttpResponseMessage拓展方法
    /// </summary>
    public static class HttpResponseMessageExtension
    {
#pragma warning disable CA1031 // 不捕获常规异常类型

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

        #region json as JsonObject

        /// <summary>
        /// 以 json 接收返回数据，并解析为 <see cref="JObject"/> 对象
        /// </summary>
        /// <param name="requestTask"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<JObject> ReceiveAsJsonAsync(this Task<HttpResponseMessage> requestTask)
        {
            using var response = await requestTask.ConfigureAwait(false);
            return await response.ReceiveAsJsonAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// 尝试以 json 接收返回数据，并解析为 <see cref="JObject"/> 对象
        /// </summary>
        /// <param name="requestTask"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<TextHttpOperationResult<JObject>> TryReceiveAsJsonAsync(this Task<HttpResponseMessage> requestTask)
        {
            var result = new TextHttpOperationResult<JObject>();
            try
            {
                result.ResponseMessage = await requestTask.ConfigureAwait(false);

                var json = await result.ResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                result.Text = json;

                if (!string.IsNullOrEmpty(json))
                {
                    result.Data = JObject.Parse(json);
                }
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }
            return result;
        }

        #endregion json as JsonObject

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
        public static async Task<T> ReceiveAsObjectAsync<T>(this Task<HttpResponseMessage> requestTask, ISerializer<string> serializer = null)
        {
            using var response = await requestTask.ConfigureAwait(false);
            return await response.ReceiveAsObjectAsync<T>(serializer ?? HttpRequestOptions.DefaultJsonSerializer).ConfigureAwait(false);
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
        public static async Task<TextHttpOperationResult<T>> TryReceiveAsObjectAsync<T>(this Task<HttpResponseMessage> requestTask, ISerializer<string> serializer = null)
        {
            var result = new TextHttpOperationResult<T>();
            try
            {
                result.ResponseMessage = await requestTask.ConfigureAwait(false);
                var json = await result.ResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                result.Text = json;

                if (!string.IsNullOrEmpty(json))
                {
                    result.Data = (serializer ?? HttpRequestOptions.DefaultJsonSerializer).Deserialize<T>(json);
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
                                                       int bufferSize = HttpRequestOptions.DefaultDownloadBufferSize)
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
#if NET5_0
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
                                                                   int bufferSize = HttpRequestOptions.DefaultDownloadBufferSize)
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
#if NET5_0
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
                                                                   int bufferSize = HttpRequestOptions.DefaultDownloadBufferSize)
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
                                                                   int bufferSize = HttpRequestOptions.DefaultDownloadBufferSize)
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
#if NET5_0
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
                                                                   int bufferSize = HttpRequestOptions.DefaultDownloadBufferSize)
        {
            using var mStream = new MemoryStream(512_000);

            await requestTask.DownloadToStreamWithProgressAsync(mStream, progressCallback, token, bufferSize).ConfigureAwait(false);

            return mStream.ToArray();
        }

        #endregion SyncCallback

        #endregion WithProgress

        #endregion Download

        #endregion Task<HttpResponseMessage>

#pragma warning restore CA1031 // 不捕获常规异常类型

        #region HttpResponseMessage

        /// <summary>
        /// 获取请求返回头的 Set-Cookie 字符串内容
        /// </summary>
        /// <param name="responseMessage"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetCookie(this HttpResponseMessage responseMessage) => responseMessage.Headers.TryGetValues(HttpHeaders.SetCookie, out var cookies) ? string.Join("; ", cookies) : string.Empty;

        /// <summary>
        /// 获取获取响应的重定向Uri
        /// </summary>
        /// <param name="response"></param>
        /// <param name="requestUri">请求的Uri</param>
        /// <returns></returns>
        public static Uri GetUriForRedirect(this HttpResponseMessage response, Uri requestUri)
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

            Uri location = response.Headers.Location;
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
        /// 获取请求返回的JObject对象
        /// </summary>
        /// <param name="responseMessage"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<JObject> ReceiveAsJsonAsync(this HttpResponseMessage responseMessage)
        {
            using (responseMessage)
            {
                var json = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (!string.IsNullOrEmpty(json))
                {
                    return JObject.Parse(json);
                }
                return null;
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
        public static async Task<T> ReceiveAsObjectAsync<T>(this HttpResponseMessage responseMessage, ISerializer<string> serializer = null)
        {
            using (responseMessage)
            {
                var json = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (!string.IsNullOrEmpty(json))
                {
                    return (serializer ?? HttpRequestOptions.DefaultJsonSerializer).Deserialize<T>(json);
                }
                return default;
            }
        }

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

        /// <summary>
        /// 获取请求返回头的 Set-Cookie 字符串内容
        /// </summary>
        /// <param name="responseMessage"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetCookie(this HttpResponseMessage responseMessage, out string cookie)
        {
            if (responseMessage.Headers.TryGetValues(HttpHeaders.SetCookie, out var cookies))
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