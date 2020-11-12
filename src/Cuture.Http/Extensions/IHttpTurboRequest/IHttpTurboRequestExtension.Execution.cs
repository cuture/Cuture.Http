using System;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

namespace Cuture.Http
{
    /// <summary>
    /// <see cref="IHttpTurboRequest"/> 请求拓展类
    /// </summary>
    public static partial class IHttpTurboRequestExtension
    {
        #region 执行请求

        #region Execute

        /// <summary>
        /// 执行请求
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<HttpResponseMessage> ExecuteAsync(this IHttpTurboRequest request)
        {
            if (request.IsSetOptions)
            {
                if (request.RequestOptions.Client != null)
                {
                    return request.RequestOptions.Client.SendAsync(request.AsRequest(), request.Token);
                }
                return InternalGetHttpTurboClient(request, request.RequestOptions).ExecuteAsync(request);
            }

            return InternalGetHttpTurboClient(request, HttpRequestOptions.Default).ExecuteAsync(request);
        }

        /// <summary>
        /// 执行请求
        /// </summary>
        /// <param name="request"></param>
        /// <param name="completionOption"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<HttpResponseMessage> ExecuteAsync(this IHttpTurboRequest request, HttpCompletionOption completionOption)
        {
            if (request.IsSetOptions)
            {
                if (request.RequestOptions.Client != null)
                {
                    return request.RequestOptions.Client.SendAsync(request.AsRequest(), completionOption, request.Token);
                }
                return InternalGetHttpTurboClient(request, request.RequestOptions).ExecuteAsync(request, completionOption);
            }

            return InternalGetHttpTurboClient(request, HttpRequestOptions.Default).ExecuteAsync(request, completionOption);
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
        public static Task<HttpResponseMessage> PostJsonAsync(this IHttpTurboRequest request, object content)
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
        public static Task<HttpResponseMessage> PostJsonAsync(this IHttpTurboRequest request, string json)
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
        public static Task<HttpResponseMessage> PostFormAsync(this IHttpTurboRequest request, object content)
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
        public static Task<HttpResponseMessage> PostFormAsync(this IHttpTurboRequest request, string content)
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
        public static Task<byte[]> GetAsBytesAsync(this IHttpTurboRequest request) => request.ExecuteAsync().ReceiveAsBytesAsync();

        /// <summary>
        /// 执行请求并尝试以
        /// <see cref="T:byte[]"/>
        /// 接收返回数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<HttpOperationResult<byte[]>> TryGetAsBytesAsync(this IHttpTurboRequest request) => request.ExecuteAsync().TryReceiveAsBytesAsync();

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
        public static Task<string> GetAsStringAsync(this IHttpTurboRequest request) => request.ExecuteAsync().ReceiveAsStringAsync();

        /// <summary>
        /// 执行请求并尝试以
        /// <see cref="string"/>
        /// 接收返回数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<HttpOperationResult<string>> TryGetAsStringAsync(this IHttpTurboRequest request) => request.ExecuteAsync().TryReceiveAsStringAsync();

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
        public static Task<JObject> GetAsJsonAsync(this IHttpTurboRequest request) => request.ExecuteAsync().ReceiveAsJsonAsync();

        /// <summary>
        /// 执行请求并尝试以 json 接收返回数据，并解析为
        /// <see cref="JObject"/>
        /// 对象
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<TextHttpOperationResult<JObject>> TryGetAsJsonAsync(this IHttpTurboRequest request) => request.ExecuteAsync().TryReceiveAsJsonAsync();

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
        public static Task<T> GetAsObjectAsync<T>(this IHttpTurboRequest request)
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
        public static Task<TextHttpOperationResult<T>> TryGetAsObjectAsync<T>(this IHttpTurboRequest request)
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
        public static Task DownloadToStreamAsync(this IHttpTurboRequest request, Stream targetStream, int bufferSize = HttpRequestOptions.DefaultDownloadBufferSize)
            => request.ExecuteAsync(HttpCompletionOption.ResponseHeadersRead).DownloadToStreamAsync(targetStream, request.Token, bufferSize);

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
        public static Task DownloadToStreamWithProgressAsync(this IHttpTurboRequest request, Func<long?, long, Task> progressCallback, Stream targetStream, int bufferSize = HttpRequestOptions.DefaultDownloadBufferSize)
            => request.ExecuteAsync(HttpCompletionOption.ResponseHeadersRead).DownloadToStreamWithProgressAsync(targetStream, progressCallback, request.Token, bufferSize);

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
        public static Task DownloadToStreamWithProgressAsync(this IHttpTurboRequest request, Action<long?, long> progressCallback, Stream targetStream, int bufferSize = HttpRequestOptions.DefaultDownloadBufferSize)
            => request.ExecuteAsync(HttpCompletionOption.ResponseHeadersRead).DownloadToStreamWithProgressAsync(targetStream, progressCallback, request.Token, bufferSize);

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
        public static Task<byte[]> DownloadWithProgressAsync(this IHttpTurboRequest request, Func<long?, long, Task> progressCallback, int bufferSize = HttpRequestOptions.DefaultDownloadBufferSize)
            => request.ExecuteAsync(HttpCompletionOption.ResponseHeadersRead).DownloadWithProgressAsync(progressCallback, request.Token, bufferSize);

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
        public static Task<byte[]> DownloadWithProgressAsync(this IHttpTurboRequest request, Action<long?, long> progressCallback, int bufferSize = HttpRequestOptions.DefaultDownloadBufferSize)
            => request.ExecuteAsync(HttpCompletionOption.ResponseHeadersRead).DownloadWithProgressAsync(progressCallback, request.Token, bufferSize);

        #endregion Download

        #endregion 执行请求
    }
}