using System.Runtime.CompilerServices;

namespace Cuture.Http;

//此文件主要包含 HttpMethod 相关的拓展方法

public static partial class IHttpRequestBuildExtensions
{
    #region HttpMethod

    /// <summary>
    /// 使用Delete动作
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IHttpRequest UseDelete(this IHttpRequest request)
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
    public static IHttpRequest UseGet(this IHttpRequest request)
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
    public static IHttpRequest UsePost(this IHttpRequest request)
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
    public static IHttpRequest UsePut(this IHttpRequest request)
    {
        request.Method = HttpMethod.Put;
        return request;
    }

    /// <inheritdoc cref="UseVerb(IHttpRequest,in ReadOnlySpan{char})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IHttpRequest UseVerb(this IHttpRequest request, string httpMethod) => InternalUseVerb(request, httpMethod, httpMethod);

    /// <summary>
    /// 使用指定的Http动作
    /// <para/>
    /// 建议使用纯大写字符串
    /// </summary>
    /// <param name="request">请求</param>
    /// <param name="httpMethod">Http动作</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IHttpRequest UseVerb(this IHttpRequest request, in ReadOnlySpan<char> httpMethod) => InternalUseVerb(request, httpMethod, null);

    /// <summary>
    /// 使用指定的Http动作
    /// </summary>
    /// <param name="request">请求</param>
    /// <param name="httpMethod">Http动作</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IHttpRequest UseVerb(this IHttpRequest request, HttpMethod httpMethod)
    {
        request.Method = httpMethod;
        return request;
    }

    private static IHttpRequest InternalUseVerb(this IHttpRequest request, in ReadOnlySpan<char> httpMethod, string? httpMethodString)
    {
        if (httpMethod.IsWhiteSpace())
        {
            throw new ArgumentNullException(nameof(httpMethod), "must has a value");
        }

        var method = httpMethod switch
        {
            "GET" => HttpMethod.Get,
            "POST" => HttpMethod.Post,
            "DELETE" => HttpMethod.Delete,
            "PUT" => HttpMethod.Put,
            "HEAD" => HttpMethod.Head,
            "OPTIONS" => HttpMethod.Options,
            _ => new HttpMethod(httpMethodString ?? httpMethod.ToString())
        };

        request.Method = method;
        return request;
    }

    #endregion HttpMethod
}
