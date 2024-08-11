namespace Cuture.Http.Test;

public sealed class TestHttpMessageInvokerPool : IHttpMessageInvokerPool
{
    private readonly HttpClientOwner _httpClient;
    private readonly SimpleHttpMessageInvokerPool _simpleHttpMessageInvokerPool;

    public TestHttpMessageInvokerPool(HttpMessageHandler handler)
    {
        _httpClient = new HttpClientOwner(handler);
        _simpleHttpMessageInvokerPool = new();
    }

    public void Dispose()
    {
        _simpleHttpMessageInvokerPool.Dispose();
        _httpClient.Dispose();
    }

    public IOwner<HttpMessageInvoker> Rent(IHttpRequest request)
    {
        if (request.DisableProxy || request.Proxy is null)
        {
            return _httpClient;
        }
        return _simpleHttpMessageInvokerPool.Rent(request);
    }

    class HttpClientOwner : IOwner<HttpMessageInvoker>
    {
        public HttpMessageInvoker Value { get; }

        public HttpClientOwner(HttpMessageHandler handler)
        {
            Value = new MyHttpMessageInvoker(handler);
        }

        public void Dispose()
        {
        }

        class MyHttpMessageInvoker : HttpMessageInvoker
        {
            public MyHttpMessageInvoker(HttpMessageHandler handler) : base(handler, false)
            {
            }
        }
    }
}
