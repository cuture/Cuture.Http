using System;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;

namespace Cuture.Http
{
    /// <summary>
    /// <see cref="IHttpRequest"/> 请求拓展类
    /// </summary>
    public static partial class IHttpTurboRequestExtensions
    {
        #region Content

#if NETCOREAPP
        /// <summary>
        /// 使用指定数据作为Http请求的Content
        /// </summary>
        /// <param name="request"></param>
        /// <param name="data">content的数据</param>
        /// <param name="contentType">Content-Type</param>
        /// <param name="contentLength">数据长度（如果小于0，则会使用data的全部数据作为Content）</param>
        /// <returns></returns>
        public static IHttpRequest WithContent(this IHttpRequest request, in ReadOnlySpan<byte> data, string contentType, int contentLength = -1)
        {
            if (string.IsNullOrWhiteSpace(contentType))
            {
                throw new ArgumentException($"“{nameof(contentType)}”不能为 Null 或空白", nameof(contentType));
            }

            request.Content?.Dispose();
            request.Content = new TypedByteArrayContent(contentLength > 0 ? data.Slice(0, contentLength).ToArray() : data.ToArray(),
                                                        contentType);

            return request;
        }

#endif

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
        public static IHttpRequest AddContent(this IHttpRequest request, HttpContent httpContent)
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
        public static IHttpRequest AddContent(this IHttpRequest request, HttpContent httpContent, string name)
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
        public static IHttpRequest WithContent(this IHttpRequest request, HttpContent httpContent)
        {
            request.Content?.Dispose();
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
        public static IHttpRequest WithFormContent(this IHttpRequest request, string content)
        {
            request.Content?.Dispose();
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
        public static IHttpRequest WithFormContent(this IHttpRequest request, object content)
        {
            request.Content?.Dispose();
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
        public static IHttpRequest WithJsonContent(this IHttpRequest request, object content)
        {
            request.Content?.Dispose();
            if (request.IsSetOptions
                && request.RequestOptions.JsonSerializer != null)
            {
                request.Content = new JsonContent(content, JsonContent.ContentType, Encoding.UTF8, request.RequestOptions.JsonSerializer);
            }
            else
            {
                request.Content = new JsonContent(content);
            }
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
        public static IHttpRequest WithJsonContent(this IHttpRequest request, string content)
        {
            request.Content?.Dispose();
            request.Content = new JsonContent(content);
            return request;
        }

        #endregion Content
    }
}