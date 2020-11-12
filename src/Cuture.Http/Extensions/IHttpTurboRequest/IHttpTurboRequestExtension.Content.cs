using System;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;

namespace Cuture.Http
{
    /// <summary>
    /// <see cref="IHttpTurboRequest"/> 请求拓展类
    /// </summary>
    public static partial class IHttpTurboRequestExtension
    {
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
        public static IHttpTurboRequest WithJsonContent(this IHttpTurboRequest request, string content)
        {
            request.Content = new JsonContent(content);
            return request;
        }

        #endregion Content
    }
}