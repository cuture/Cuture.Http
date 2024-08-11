using System.Runtime.CompilerServices;

namespace Cuture.Http;

/// <inheritdoc/>
public sealed class DefaultHttpRequestCreator : IHttpRequestCreator
{
    #region 方法

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IHttpRequest CreateRequest(Uri requestUri) => SetDefaultHeaders(new DefaultHttpRequest(requestUri));

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IHttpRequest CreateReuseableRequest(Uri requestUri) => SetDefaultHeaders(new ReuseableHttpRequest(requestUri));

    /// <inheritdoc/>
    public void Dispose() { }

    #endregion 方法

    #region Private 方法

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static IHttpRequest SetDefaultHeaders(IHttpRequest request)
    {
        if (HttpRequestGlobalOptions.DefaultHttpHeaders.Count == 0)
        {
            return request;
        }

        foreach (var item in HttpRequestGlobalOptions.DefaultHttpHeaders)
        {
            request.Headers.TryAddWithoutValidation(item.Key, item.Value);
        }
        return request;
    }

    #endregion Private 方法
}
