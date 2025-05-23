﻿using System.Net;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;

namespace Cuture.Http;

/// <summary>
/// 可重用的 <see cref="IHttpRequest"/> 实现
/// <para/>
/// * 可以重复使用进行请求
/// </summary>
/// <remarks>
/// <inheritdoc cref="ReuseableHttpRequest"/>
/// </remarks>
/// <param name="requestUri">请求的Uri</param>
public class ReuseableHttpRequest(Uri requestUri) : IHttpRequest
{
    #region Private 字段

    private byte[]? _contentDump;

    private bool _disposedValue;

    private HttpRequestOptions? _httpRequestOptions;

    private HttpRequestExecutionOptions? _options;

    #endregion Private 字段

    #region 属性

    /// <inheritdoc/>
    public bool AllowRedirection { get; set; }

    /// <inheritdoc/>
    public HttpContent? Content { get; set; }

    /// <summary>
    /// 禁用Proxy
    /// <para/>初始值为 <see cref="HttpRequestGlobalOptions.DisableUseDefaultProxyByDefault"/>
    /// </summary>
    public bool DisableProxy { get; set; } = HttpRequestGlobalOptions.DisableUseDefaultProxyByDefault;

    /// <inheritdoc/>
    public HttpRequestExecutionOptions ExecutionOptions
    {
        get => _options ??= HttpRequestExecutionOptions.Default.Clone();
        set => _options = value;
    }

    /// <inheritdoc/>
    public HttpHeaders Headers => new SimpleHttpRequestHeaders();

    /// <inheritdoc/>
    public bool IsSetOptions => _options != null;

    /// <inheritdoc/>
    public int MaxAutomaticRedirections { get; set; } = HttpRequestGlobalOptions.MaxAutomaticRedirections;

    /// <inheritdoc/>
    public HttpMethod Method { get; set; } = HttpMethod.Get;

    /// <inheritdoc/>
    public HttpRequestOptions Options => _httpRequestOptions ??= new();

    /// <inheritdoc/>
    public IWebProxy? Proxy { get; set; }

    /// <inheritdoc/>
    public Uri RequestUri { get; } = requestUri ?? throw new ArgumentNullException(nameof(requestUri));

    /// <inheritdoc/>
    public int? Timeout { get; set; }

    /// <inheritdoc/>
    public CancellationToken Token { get; set; }

    /// <inheritdoc/>
    public Version? Version { get; set; }

    #endregion 属性

    #region 方法

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask<HttpRequestMessage> GetHttpRequestMessageAsync(CancellationToken cancellationToken = default)
    {
        var message = new HttpRequestMessage(Method, RequestUri);

        if (Version != null)
        {
            message.Version = Version;
        }

        CopyHeaders(Headers, message.Headers);

        if (_httpRequestOptions is { } httpRequestOptions)
        {
            foreach (var (key, value) in httpRequestOptions)
            {
                message.Options.Set(new(key), value);
            }
        }

        return Content is null
               ? new(message)
               : DumpContentToHttpRequestMessageAsync(message, cancellationToken);
    }

    private static void CopyHeaders(HttpHeaders source, HttpHeaders target)
    {
        foreach (var item in source)
        {
            target.TryAddWithoutValidation(item.Key, item.Value);
        }
    }

    private async ValueTask<HttpRequestMessage> DumpContentToHttpRequestMessageAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken)
    {
        _contentDump ??= await Content!.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false);

        var localContent = new ByteArrayContent(_contentDump);

        CopyHeaders(Content!.Headers, localContent.Headers);

        httpRequestMessage.Content = localContent;

        return httpRequestMessage;
    }

    #endregion 方法

    #region Dispose

    /// <summary>
    ///
    /// </summary>
    ~ReuseableHttpRequest()
    {
        Dispose(disposing: false);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="disposing"></param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            _disposedValue = true;

            Content?.Dispose();
        }
    }

    #endregion Dispose
}
