using System.Buffers;
using System.Runtime.CompilerServices;

namespace Cuture.Http;

//此文件主要包含 Content 相关的拓展方法

public static partial class IHttpRequestBuildExtensions
{
    #region Content

    /// <summary>
    /// 使用指定的HttpContent
    /// </summary>
    /// <param name="request"></param>
    /// <param name="httpContent"></param>
    /// <param name="disposeExisted">释放之前的Content</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IHttpRequest WithContent(this IHttpRequest request, HttpContent httpContent, bool disposeExisted = true)
    {
        if (disposeExisted)
        {
            request.Content?.Dispose();
        }
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
    public static IHttpRequest WithContent(this IHttpRequest request, in ReadOnlySpan<byte> data, string? contentType, int contentLength = -1)
    {
        //TODO use memory pool
        var contentData = contentLength > 0 ? data[..contentLength].ToArray() : data.ToArray();
        return request.WithContent(new TypedByteArrayContent(contentData, contentType));
    }

    /// <summary>
    /// 使用指定数据作为Http请求的Content
    /// </summary>
    /// <param name="request"></param>
    /// <param name="data">content的数据</param>
    /// <param name="contentType">Content-Type</param>
    /// <param name="memoryOwner"></param>
    /// <returns></returns>
    internal static IHttpRequest WithContent(this IHttpRequest request, in ReadOnlyMemory<byte> data, string? contentType, IMemoryOwner<byte>? memoryOwner = null)
    {
        if (memoryOwner is null)
        {
            return request.WithContent(data.Span, contentType, -1);
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
