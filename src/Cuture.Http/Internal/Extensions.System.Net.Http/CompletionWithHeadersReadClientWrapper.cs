using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http;

/// <summary>
/// 在响应头读取结束时完成的<see cref="HttpClient"/>包装
/// </summary>
internal class CompletionWithHeadersReadClientWrapper : HttpMessageInvoker
{
    #region Private 字段

    private readonly HttpClient _client;
    private readonly bool _disposeHttpClient;

    #endregion Private 字段

    #region Public 构造函数

    /// <inheritdoc cref="CompletionWithHeadersReadClientWrapper"/>
    public CompletionWithHeadersReadClientWrapper(HttpClient client) : this(client, true)
    {
    }

    /// <inheritdoc cref="CompletionWithHeadersReadClientWrapper"/>
    public CompletionWithHeadersReadClientWrapper(HttpClient client, bool disposeHttpClient) : base(EmptyHttpMessageHandler.Instance, false)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _disposeHttpClient = disposeHttpClient;
    }

    #endregion Public 构造函数

    #region Public 方法

    public override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken) => _client.Send(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

    public override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) => _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

    #endregion Public 方法

    #region Protected 方法

    protected override void Dispose(bool disposing)
    {
        if (_disposeHttpClient)
        {
            _client.Dispose();
        }

        base.Dispose(disposing);
    }

    #endregion Protected 方法
}