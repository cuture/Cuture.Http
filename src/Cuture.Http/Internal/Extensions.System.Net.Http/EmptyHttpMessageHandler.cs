using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http
{
    /// <summary>
    /// 空的<see cref="HttpMessageHandler"/>实现
    /// </summary>
    internal class EmptyHttpMessageHandler : HttpMessageHandler
    {
        public static readonly EmptyHttpMessageHandler Instance = new EmptyHttpMessageHandler();

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) => throw new NotImplementedException();
    }
}
