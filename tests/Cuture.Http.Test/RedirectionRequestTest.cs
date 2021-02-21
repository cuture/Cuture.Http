using System.Threading.Tasks;

using Cuture.Http.Test.Server;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cuture.Http.Test
{
    [TestClass]
    public class RedirectionRequestTest : WebServerHostTestBase
    {
        #region 方法

        [TestMethod]
        public async Task AutoRedirectionRequestTestAsync()
        {
            await ParallelRequestAsync(10_000,
                                       () => GetRequest().AutoRedirection(true).TryGetAsStringAsync(),
                                       result => Assert.AreEqual(Resource.RedirectEnd, result.Data));
        }

        [TestMethod]
        public async Task LimitedAutoRedirectionRequestTestAsync()
        {
            await ParallelRequestAsync(10_000,
                                       () => GetRequest().AutoRedirection(true).MaxAutoRedirections(3).TryGetAsStringAsync(),
                                       result =>
                                       {
                                           var location = result.ResponseMessage.Headers.Location.OriginalString.ToLowerInvariant();
                                           Assert.AreEqual("/api/redirection/r4", location);
                                       });
        }

        [TestMethod]
        public async Task NoRedirectionRequestTestAsync()
        {
            await ParallelRequestAsync(10_000,
                                       () => GetRequest().TryGetAsStringAsync(),
                                       result =>
                                       {
                                           var location = result.ResponseMessage.Headers.Location.OriginalString.ToLowerInvariant();
                                           Assert.AreEqual("/api/redirection/r1", location);
                                       });
        }

        private IHttpRequest GetRequest() => $"{TestWebHost.TestHost}/api/redirection".CreateHttpRequest();

        #endregion 方法
    }
}