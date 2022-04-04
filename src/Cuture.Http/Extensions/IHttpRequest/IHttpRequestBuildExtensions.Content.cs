using System;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Buffers;

namespace Cuture.Http
{
    //此文件主要包含 Content 相关的拓展方法

    public static partial class IHttpRequestBuildExtensions
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
        [Obsolete("由于代码自动组装Content的不透明和不准确，建议自行组装，或充分测试")]
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
        [Obsolete("由于代码自动组装Content的不透明和不准确，建议自行组装，或充分测试")]
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

            return request.WithContent(new TypedByteArrayContent(contentLength > 0 ? data.Slice(0, contentLength).ToArray() : data.ToArray(),
                                                                 contentType));
        }

        /// <summary>
        /// 使用指定数据作为Http请求的Content
        /// </summary>
        /// <param name="request"></param>
        /// <param name="data">content的数据</param>
        /// <param name="contentType">Content-Type</param>
        /// <param name="memoryOwner"></param>
        /// <returns></returns>
        internal static IHttpRequest WithContent(this IHttpRequest request, in ReadOnlyMemory<byte> data, string contentType, IMemoryOwner<byte>? memoryOwner = null)
        {
            if (memoryOwner is null)
            {
                return request.WithContent(data.Span, contentType, -1);
            }

            if (string.IsNullOrWhiteSpace(contentType))
            {
                throw new ArgumentException($"“{nameof(contentType)}”不能为 Null 或空白", nameof(contentType));
            }

            return request.WithContent(new TypedMemoryOwnedContent(memoryOwner, data, contentType));
        }

        #region Form

        /// <summary>
        /// 使用FormContent
        /// <para/>
        /// 将 <paramref name="content"/> 使用 <see cref="HttpRequestGlobalOptions.DefaultFormDataFormatter"/> 转化为kv字符串,并UrlEncoded
        /// </summary>
        /// <param name="request"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpRequest WithFormContent(this IHttpRequest request, object content)
                => request.WithFormContent(content, HttpRequestGlobalOptions.DefaultFormDataFormatter);

        /// <summary>
        /// 使用FormContent
        /// </summary>
        /// <param name="request"></param>
        /// <param name="content"></param>
        /// <param name="formatter"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpRequest WithFormContent(this IHttpRequest request, object content, IFormDataFormatter formatter)
                => request.WithContent(new FormContent(content, formatter));

        /// <summary>
        /// 使用FormContent
        /// <paramref name="content"/>为已经urlencode的字符串
        /// </summary>
        /// <param name="request"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpRequest WithFormContent(this IHttpRequest request, string content)
                => request.WithContent(new FormContent(content));

        #endregion Form

        #region Json

        /// <summary>
        /// 使用JsonHttpContent
        /// <para/>
        /// 将 <paramref name="content"/> 使用 请求设置的 JsonSerializer 或 <see cref="HttpRequestGlobalOptions.DefaultJsonSerializer"/> 序列化为json字符串
        /// </summary>
        /// <param name="request"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpRequest WithJsonContent(this IHttpRequest request, object content)
        {
            return request.WithJsonContent(content, request.GetJsonSerializerOrDefault());
        }

        /// <summary>
        /// 使用JsonHttpContent
        /// </summary>
        /// <param name="request"></param>
        /// <param name="content"></param>
        /// <param name="jsonSerializer"></param>
        /// <returns></returns>
        public static IHttpRequest WithJsonContent(this IHttpRequest request, object content, IJsonSerializer jsonSerializer)
                => request.WithContent(new JsonContent(content, jsonSerializer));

        /// <summary>
        /// 使用JsonHttpContent
        /// <paramref name="content"/>为json字符串
        /// </summary>
        /// <param name="request"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpRequest WithJsonContent(this IHttpRequest request, string content)
                => request.WithContent(new JsonContent(content));

        #endregion Json

        #endregion Content
    }
}