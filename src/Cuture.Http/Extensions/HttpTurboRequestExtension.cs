using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

namespace Cuture.Http
{
    /// <summary>
    /// <see cref="IHttpTurboRequest"/> 请求拓展类
    /// </summary>
    public static class HttpTurboRequestExtension
    {
        #region 构造函数

        static HttpTurboRequestExtension()
        {
#if NETSTANDARD2_0
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif
        }

        #endregion 构造函数

        #region Header

        /// <summary>
        /// 添加Header
        /// </summary>
        /// <param name="request"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpTurboRequest AddHeader(this IHttpTurboRequest request, string key, string value)
        {
            request.AddHeader(key, value);
            return request;
        }

        /// <summary>
        /// 添加Header集合
        /// </summary>
        /// <param name="request"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpTurboRequest AddHeaders(this IHttpTurboRequest request, IEnumerable<KeyValuePair<string, string>> headers)
        {
            foreach (var header in headers)
            {
                request.AddHeader(header.Key, header.Value);
            }
            return request;
        }

        /// <summary>
        /// 如果存在Header则将其移除,并添加Header
        /// </summary>
        /// <param name="request"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpTurboRequest AddNewHeader(this IHttpTurboRequest request, string key, string value)
        {
            request.RemoveHeader(key);
            request.AddHeader(key, value);
            return request;
        }

        /// <summary>
        /// 使用Cookie
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpTurboRequest UseCookie(this IHttpTurboRequest request, string cookie)
        {
            request.AddHeader(HttpHeaders.Cookie, cookie);
            return request;
        }

        /// <summary>
        /// 使用Referer
        /// </summary>
        /// <param name="request"></param>
        /// <param name="referer"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpTurboRequest UseReferer(this IHttpTurboRequest request, string referer)
        {
            request.AddHeader(HttpHeaders.Referer, referer);
            return request;
        }

        /// <summary>
        /// 使用UserAgent
        /// </summary>
        /// <param name="request"></param>
        /// <param name="userAgent"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpTurboRequest UseUserAgent(this IHttpTurboRequest request, string userAgent)
        {
            request.AddHeader(HttpHeaders.UserAgent, userAgent);
            return request;
        }

        #endregion Header

        #region HttpMethod

        /// <summary>
        /// 使用Delete动作
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpTurboRequest UseDelete(this IHttpTurboRequest request)
        {
            request.Method = HttpMethod.Delete;
            return request;
        }

        /// <summary>
        /// 使用Get动作
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpTurboRequest UseGet(this IHttpTurboRequest request)
        {
            request.Method = HttpMethod.Get;
            return request;
        }

        /// <summary>
        /// 使用Post动作
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpTurboRequest UsePost(this IHttpTurboRequest request)
        {
            request.Method = HttpMethod.Post;
            return request;
        }

        /// <summary>
        /// 使用Put动作
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpTurboRequest UsePut(this IHttpTurboRequest request)
        {
            request.Method = HttpMethod.Put;
            return request;
        }

        /// <summary>
        /// 使用指定的Http动作
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="httpMethod">Http动作</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpTurboRequest UseVerb(this IHttpTurboRequest request, string httpMethod)
        {
            request.Method = new HttpMethod(httpMethod);
            return request;
        }

        /// <summary>
        /// 使用指定的Http动作
        /// </summary>
        /// <param name="request">请求</param>
        /// <param name="httpMethod">Http动作</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpTurboRequest WithVerb(this IHttpTurboRequest request, HttpMethod httpMethod)
        {
            request.Method = httpMethod;
            return request;
        }

        #endregion HttpMethod

        #region Content

        /// <summary>
        /// 为请求添加HttpContent;
        /// <para/>
        /// 若请求的 Content 为空，则直接设置Content为 <paramref name="httpContent"/>;
        /// <para/>
        /// 若请求的 Content 为 <see cref="MultipartContent"/> 则直接添加 <paramref name="httpContent"/>;
        /// <para/>
        /// 若请求的 Content 为其它类型Content, 则会新建 <see cref="MultipartContent"/> 并添加原有Content和 <paramref name="httpContent"/>;
        /// </summary>
        /// <param name="request"></param>
        /// <param name="httpContent"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpTurboRequest AddContent(this IHttpTurboRequest request, HttpContent httpContent)
        {
            //HACK 处理httpContent为MultipartFormDataContent和MultipartContent时的情况
            switch (request.Content)
            {
                case null:
                    request.Content = httpContent;
                    break;

                case MultipartContent multipartContent:
                    multipartContent.Add(httpContent);
                    break;

                default:
                    request.Content = new MultipartContent()
                    {
                        request.Content,
                        httpContent
                    };
                    break;
            }

            return request;
        }

        /// <summary>
        /// 为请求添加HttpContent;
        /// <para/>
        /// 若请求的 Content 为空，则会新建 <see cref="MultipartFormDataContent"/> 并添加 <paramref name="httpContent"/>;
        /// <para/>
        /// 若请求的 Content 为 <see cref="MultipartFormDataContent"/> 则直接添加 <paramref name="httpContent"/>;
        /// <para/>
        /// 若请求的 Content 为其它类型Content,则会抛出异常
        /// </summary>
        /// <param name="request"></param>
        /// <param name="httpContent"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpTurboRequest AddContent(this IHttpTurboRequest request, HttpContent httpContent, string name)
        {
            //HACK 处理httpContent为MultipartFormDataContent和MultipartContent时的情况
            switch (request.Content)
            {
                case null:
                    request.Content = new MultipartFormDataContent
                    {
                        { httpContent, name }
                    };
                    break;

                case MultipartFormDataContent multipartContent:
                    multipartContent.Add(httpContent, name);
                    break;

                default:
                    throw new InvalidOperationException($"请求已包含非“{nameof(MultipartFormDataContent)}”的Content;");
            }

            return request;
        }

        /// <summary>
        /// 使用指定的HttpContent
        /// </summary>
        /// <param name="request"></param>
        /// <param name="httpContent"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpTurboRequest WithContent(this IHttpTurboRequest request, HttpContent httpContent)
        {
            request.Content = httpContent;
            return request;
        }

        /// <summary>
        /// 使用FormContent
        /// <paramref name="content"/>为已经urlencode的字符串
        /// </summary>
        /// <param name="request"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpTurboRequest WithFormContent(this IHttpTurboRequest request, string content)
        {
            request.Content = new FormContent(content);
            return request;
        }

        /// <summary>
        /// 使用FormContent
        /// <paramref name="content"/>将会自动进行Form化
        /// </summary>
        /// <param name="request"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpTurboRequest WithFormContent(this IHttpTurboRequest request, object content)
        {
            request.Content = new FormContent(content);
            return request;
        }

        /// <summary>
        /// 使用JsonHttpContent
        /// <paramref name="content"/>会自动进行json序列化
        /// </summary>
        /// <param name="request"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpTurboRequest WithJsonContent(this IHttpTurboRequest request, object content)
        {
            request.Content = new JsonContent(content);
            return request;
        }

        /// <summary>
        /// 使用JsonHttpContent
        /// <paramref name="content"/>为json字符串
        /// </summary>
        /// <param name="request"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpTurboRequest WithJsonContent(this IHttpTurboRequest request, string content)
        {
            request.Content = new JsonContent(content);
            return request;
        }

        #endregion Content

        #region Setting

        #region Proxy

        /// <summary>
        /// 禁用系统代理
        /// <para/>设置 <see cref="IHttpTurboRequest.DisableProxy"/> 为 true
        /// <para/>默认实现下, 将不使用任何代理进行请求
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpTurboRequest DisableProxy(this IHttpTurboRequest request)
        {
            request.DisableProxy = true;
            request.Proxy = null;
            return request;
        }

        /// <summary>
        /// 使用默认Web代理（理论上默认情况下就是这种状态）
        /// <para/>设置 <see cref="IHttpTurboRequest.Proxy"/> 为 <see cref="WebRequest.DefaultWebProxy"/>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpTurboRequest UseDefaultWebProxy(this IHttpTurboRequest request)
        {
            request.DisableProxy = false;
            request.Proxy = WebRequest.DefaultWebProxy;
            return request;
        }

        /// <summary>
        /// 使用指定的代理
        /// </summary>
        /// <param name="request"></param>
        /// <param name="webProxy"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpTurboRequest UseProxy(this IHttpTurboRequest request, IWebProxy webProxy)
        {
            request.Proxy = webProxy;
            return request;
        }

        /// <summary>
        /// 使用指定的代理
        /// </summary>
        /// <param name="request"></param>
        /// <param name="proxyAddress">代理地址</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpTurboRequest UseProxy(this IHttpTurboRequest request, string proxyAddress)
        {
            request.Proxy = string.IsNullOrEmpty(proxyAddress) ? null : new WebProxy(proxyAddress);
            return request;
        }

        /// <summary>
        /// 使用指定的代理
        /// </summary>
        /// <param name="request"></param>
        /// <param name="proxyUri">代理地址</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpTurboRequest UseProxy(this IHttpTurboRequest request, Uri proxyUri)
        {
            request.Proxy = proxyUri is null ? null : new WebProxy(proxyUri);
            return request;
        }

        /// <summary>
        /// 使用系统代理
        /// <para/>设置 <see cref="IHttpTurboRequest.Proxy"/> 为 <see cref="WebRequest.GetSystemWebProxy()"/>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpTurboRequest UseSystemProxy(this IHttpTurboRequest request)
        {
            request.DisableProxy = false;
            request.Proxy = WebRequest.GetSystemWebProxy();
            return request;
        }

        #endregion Proxy

        /// <summary>
        /// 允许自动重定向
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpTurboRequest AllowRedirection(this IHttpTurboRequest request)
        {
            request.AllowRedirection = true;
            return request;
        }

        /// <summary>
        /// 设置是否允许自动重定向
        /// </summary>
        /// <param name="request"></param>
        /// <param name="allowRedirection"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpTurboRequest AutoRedirection(this IHttpTurboRequest request, bool allowRedirection)
        {
            request.AllowRedirection = allowRedirection;
            return request;
        }

        /// <summary>
        /// 配置请求
        /// </summary>
        /// <param name="request"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IHttpTurboRequest Configure(this IHttpTurboRequest request, Action<IHttpTurboRequest> action)
        {
            action.Invoke(request);
            return request;
        }

        /// <summary>
        /// 超时时间
        /// </summary>
        /// <param name="request"></param>
        /// <param name="milliseconds"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpTurboRequest TimeOut(this IHttpTurboRequest request, int milliseconds)
        {
            request.Timeout = milliseconds;
            return request;
        }

        /// <summary>
        /// 超时时间
        /// </summary>
        /// <param name="request"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpTurboRequest TimeOut(this IHttpTurboRequest request, TimeSpan timeout)
        {
            long num = (long)timeout.TotalMilliseconds;
            if (num < -1 || num > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(timeout));
            }
            request.Timeout = (int)num;
            return request;
        }

        /// <summary>
        /// 使用指定的HttpClient
        /// </summary>
        /// <param name="request"></param>
        /// <param name="httpClient"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Obsolete("建议使用 UseTurboClient 或 UseTurboClientFactory 替代此方法调用")]
        public static IHttpTurboRequest UseHttpClient(this IHttpTurboRequest request, HttpClient httpClient)
        {
            request.TurboClient = new HttpTurboClient(httpClient);
            return request;
        }

        /// <summary>
        /// 使用指定的HttpTurboClient
        /// </summary>
        /// <param name="request"></param>
        /// <param name="turboClient"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpTurboRequest UseTurboClient(this IHttpTurboRequest request, IHttpTurboClient turboClient)
        {
            request.TurboClient = turboClient;
            return request;
        }

        /// <summary>
        /// 使用指定的TurboClientFactory
        /// </summary>
        /// <param name="request"></param>
        /// <param name="turboClientFactory"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpTurboRequest UseTurboClientFactory(this IHttpTurboRequest request, IHttpTurboClientFactory turboClientFactory)
        {
            request.TurboClientFactory = turboClientFactory;
            return request;
        }

        /// <summary>
        /// 使用取消标记
        /// </summary>
        /// <param name="request"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static IHttpTurboRequest WithCancellationToken(this IHttpTurboRequest request, CancellationToken token)
        {
            request.Token = token;
            return request;
        }

        #endregion Setting

        #region 请求

        #region Execute

        /// <summary>
        /// 执行请求
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<HttpResponseMessage> ExecuteAsync(this IHttpTurboRequest request) => InternalGetHttpTurboClient(request).ExecuteAsync(request);

        /// <summary>
        /// 执行请求
        /// </summary>
        /// <param name="request"></param>
        /// <param name="completionOption"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<HttpResponseMessage> ExecuteAsync(this IHttpTurboRequest request, HttpCompletionOption completionOption) => InternalGetHttpTurboClient(request).ExecuteAsync(request, completionOption);

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
            request.Content = new JsonContent(content);
            request.Method = HttpMethod.Post;
            return InternalGetHttpTurboClient(request).ExecuteAsync(request);
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
            return InternalGetHttpTurboClient(request).ExecuteAsync(request);
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
            return InternalGetHttpTurboClient(request).ExecuteAsync(request);
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
            return InternalGetHttpTurboClient(request).ExecuteAsync(request);
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
        public static Task<T> GetAsObjectAsync<T>(this IHttpTurboRequest request) => request.ExecuteAsync().ReceiveAsObjectAsync<T>();

        /// <summary>
        /// 执行请求并尝试以 json 接收返回数据，并解析为类型
        /// <typeparamref name="T"/>
        /// 的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<TextHttpOperationResult<T>> TryGetAsObjectAsync<T>(this IHttpTurboRequest request) => request.ExecuteAsync().TryReceiveAsObjectAsync<T>();

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
        public static Task DownloadToStreamAsync(this IHttpTurboRequest request, Stream targetStream, int bufferSize = HttpDefaultSetting.DefaultDownloadBufferSize)
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
        public static Task DownloadToStreamWithProgressAsync(this IHttpTurboRequest request, Func<long?, long, Task> progressCallback, Stream targetStream, int bufferSize = HttpDefaultSetting.DefaultDownloadBufferSize)
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
        public static Task DownloadToStreamWithProgressAsync(this IHttpTurboRequest request, Action<long?, long> progressCallback, Stream targetStream, int bufferSize = HttpDefaultSetting.DefaultDownloadBufferSize)
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
        public static Task<byte[]> DownloadWithProgressAsync(this IHttpTurboRequest request, Func<long?, long, Task> progressCallback, int bufferSize = HttpDefaultSetting.DefaultDownloadBufferSize)
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
        public static Task<byte[]> DownloadWithProgressAsync(this IHttpTurboRequest request, Action<long?, long> progressCallback, int bufferSize = HttpDefaultSetting.DefaultDownloadBufferSize)
            => request.ExecuteAsync(HttpCompletionOption.ResponseHeadersRead).DownloadWithProgressAsync(progressCallback, request.Token, bufferSize);

        #endregion Download

        #endregion 请求

        #region ToHttpRequest

        /// <summary>
        /// 字符串转换为Http请求
        /// <br/>
        /// 框架有默认并发限制,可以通过设置
        /// <see cref="HttpDefaultSetting.DefaultConnectionLimit"/>
        /// 或者
        /// <see cref="ServicePointManager.DefaultConnectionLimit"/>
        /// 来放宽并发请求限制
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpTurboRequest ToHttpRequest(this string url) => ToHttpRequest(url, HttpDefaultSetting.DefaultTurboRequestFactory);

        /// <summary>
        /// 字符串转换为Http请求
        /// <br/>
        /// 框架有默认并发限制,可以通过设置
        /// <see cref="HttpDefaultSetting.DefaultConnectionLimit"/>
        /// 或者
        /// <see cref="ServicePointManager.DefaultConnectionLimit"/>
        /// 来放宽并发请求限制
        /// </summary>
        /// <param name="url"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpTurboRequest ToHttpRequest(this string url, IHttpTurboRequestFactory factory)
        {
            if (url.Length == 0)
            {
                throw new ArgumentException("it is null or empty", nameof(url));
            }
            return factory.CreateRequest(url);
        }

        /// <summary>
        /// Uri转换为Http请求
        /// <br/>
        /// 框架有默认并发限制,可以通过设置
        /// <see cref="HttpDefaultSetting.DefaultConnectionLimit"/>
        /// 或者
        /// <see cref="ServicePointManager.DefaultConnectionLimit"/>
        /// 来放宽并发请求限制
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpTurboRequest ToHttpRequest(this Uri uri) => ToHttpRequest(uri, HttpDefaultSetting.DefaultTurboRequestFactory);

        /// <summary>
        /// Uri转换为Http请求
        /// <br/>
        /// 框架有默认并发限制,可以通过设置
        /// <see cref="HttpDefaultSetting.DefaultConnectionLimit"/>
        /// 或者
        /// <see cref="ServicePointManager.DefaultConnectionLimit"/>
        /// 来放宽并发请求限制
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpTurboRequest ToHttpRequest(this Uri uri, IHttpTurboRequestFactory factory) => factory.CreateRequest(uri);

        #endregion ToHttpRequest

        #region 方法

        /// <summary>
        /// 获取一个HttpTurbo
        /// </summary>
        /// <param name="request">本次请求</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IHttpTurboClient InternalGetHttpTurboClient(IHttpTurboRequest request)
        {
            //HACK 支持TurboClientFactory会多那么几次判断
            return request.TurboClient ?? request.TurboClientFactory?.GetTurboClient(request) ?? HttpDefaultSetting.DefaultTurboClientFactory.GetTurboClient(request);
        }

        #endregion 方法
    }
}