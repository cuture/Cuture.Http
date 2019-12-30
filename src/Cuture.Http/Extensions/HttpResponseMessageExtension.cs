﻿using System;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
            var response = await requestTask.ConfigureAwait(false);
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
            var response = await requestTask.ConfigureAwait(false);
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
        /// 以 json 接收返回数据，并解析为
        /// <see cref="JObject"/>
        /// 对象
        /// </summary>
        /// <param name="requestTask"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<JObject> ReceiveAsObjectAsync(this Task<HttpResponseMessage> requestTask)
        {
            var response = await requestTask.ConfigureAwait(false);

            return await response.ReceiveAsObjectAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// 尝试以 json 接收返回数据，并解析为
        /// <see cref="JObject"/>
        /// 对象
        /// </summary>
        /// <param name="requestTask"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<TextHttpOperationResult<JObject>> TryReceiveAsObjectAsync(this Task<HttpResponseMessage> requestTask)
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
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<T> ReceiveAsObjectAsync<T>(this Task<HttpResponseMessage> requestTask)
        {
            var response = await requestTask.ConfigureAwait(false);
            return await response.ReceiveAsObjectAsync<T>().ConfigureAwait(false);
        }

        /// <summary>
        /// 尝试以 json 接收返回数据，并解析为类型
        /// <typeparamref name="T"/>
        /// 的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestTask"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<TextHttpOperationResult<T>> TryReceiveAsObjectAsync<T>(this Task<HttpResponseMessage> requestTask)
        {
            var result = new TextHttpOperationResult<T>();
            try
            {
                result.ResponseMessage = await requestTask.ConfigureAwait(false);
                var json = await result.ResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                result.Text = json;

                if (!string.IsNullOrEmpty(json))
                {
                    result.Data = JsonConvert.DeserializeObject<T>(json);
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
                                                       int bufferSize = HttpDefaultSetting.DefaultDownloadBufferSize)
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

            var response = await requestTask.ConfigureAwait(false);

            var contentLength = response.Content.Headers.ContentLength;

            using var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

            var buffer = new byte[bufferSize];
            var count = 0L;

            while (!token.IsCancellationRequested
                   && (contentLength == null || count < contentLength.Value))
            {
                var size = stream.Read(buffer, 0, bufferSize);
                count += size;
                await targetStream.WriteAsync(buffer, 0, size, token).ConfigureAwait(false);
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
        public static async Task DownloadToStreamWithProgressAsync(this Task<HttpResponseMessage> requestTask,
                                                                   Stream targetStream,
                                                                   Func<long?, long, Task> progressCallback,
                                                                   CancellationToken token,
                                                                   int bufferSize = HttpDefaultSetting.DefaultDownloadBufferSize)
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

            var response = await requestTask.ConfigureAwait(false);

            var contentLength = response.Content.Headers.ContentLength;

            using var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

            var buffer = new byte[bufferSize];
            var count = 0L;

            await progressCallback(contentLength, count).ConfigureAwait(false);

            while (!token.IsCancellationRequested
                   && (contentLength == null || count < contentLength.Value))
            {
                var size = stream.Read(buffer, 0, bufferSize);
                count += size;
                await targetStream.WriteAsync(buffer, 0, size, token).ConfigureAwait(false);

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
        public static async Task<byte[]> DownloadWithProgressAsync(this Task<HttpResponseMessage> requestTask,
                                                                   Func<long?, long, Task> progressCallback,
                                                                   CancellationToken token,
                                                                   int bufferSize = HttpDefaultSetting.DefaultDownloadBufferSize)
        {
            var result = new HttpOperationResult<byte[]>();
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
                                                                   int bufferSize = HttpDefaultSetting.DefaultDownloadBufferSize)
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

            var response = await requestTask.ConfigureAwait(false);

            var contentLength = response.Content.Headers.ContentLength;

            using var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

            var buffer = new byte[bufferSize];
            var count = 0L;

            progressCallback(contentLength, count);

            while (!token.IsCancellationRequested
                   && (contentLength == null || count < contentLength.Value))
            {
                var size = stream.Read(buffer, 0, bufferSize);
                count += size;
                await targetStream.WriteAsync(buffer, 0, size, token).ConfigureAwait(false);

                progressCallback(contentLength, count);
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
        public static async Task<byte[]> DownloadWithProgressAsync(this Task<HttpResponseMessage> requestTask,
                                                                   Action<long?, long> progressCallback,
                                                                   CancellationToken token,
                                                                   int bufferSize = HttpDefaultSetting.DefaultDownloadBufferSize)
        {
            var result = new HttpOperationResult<byte[]>();

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
        public static string GetCookie(this HttpResponseMessage responseMessage) => responseMessage.Headers.TryGetValues(HttpHeaders.SetCookie, out var cookies) ? string.Join(" ", cookies) : string.Empty;

        /// <summary>
        /// 获取请求返回的字符串
        /// </summary>
        /// <param name="responseMessage"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<byte[]> ReceiveAsBytesAsync(this HttpResponseMessage responseMessage) => await responseMessage.Content.ReadAsByteArrayAsync().ConfigureAwait(false);

        /// <summary>
        /// 获取请求返回的json对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="responseMessage"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<T> ReceiveAsObjectAsync<T>(this HttpResponseMessage responseMessage)
        {
            var json = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (!string.IsNullOrEmpty(json))
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            return default;
        }

        /// <summary>
        /// 获取请求返回的json对象
        /// </summary>
        /// <param name="responseMessage"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<JObject> ReceiveAsObjectAsync(this HttpResponseMessage responseMessage)
        {
            var json = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (!string.IsNullOrEmpty(json))
            {
                return JObject.Parse(json);
            }
            return null;
        }

        /// <summary>
        /// 获取请求返回的字符串
        /// </summary>
        /// <param name="responseMessage"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<string> ReceiveAsStringAsync(this HttpResponseMessage responseMessage) => await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

        #endregion HttpResponseMessage
    }
}