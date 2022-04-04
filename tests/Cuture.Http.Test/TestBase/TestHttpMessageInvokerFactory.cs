using System.Net.Http;

namespace Cuture.Http.Test;

public class TestHttpMessageInvokerFactory : IHttpMessageInvokerFactory
{
    private readonly HttpClient _httpClient;
    private readonly SimpleHttpMessageInvokerFactory _simpleHttpMessageInvokerFactory;

    public TestHttpMessageInvokerFactory(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _simpleHttpMessageInvokerFactory = new SimpleHttpMessageInvokerFactory();
    }

    public void Dispose()
    {
        _simpleHttpMessageInvokerFactory.Dispose();
        _httpClient.Dispose();
    }

    public HttpMessageInvoker GetInvoker(IHttpRequest request)
    {
        if (request.DisableProxy || request.Proxy is null)
        {
            return _httpClient;
        }
        return _simpleHttpMessageInvokerFactory.GetInvoker(request);
    }
}
