using System;
using System.Linq;
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
        public async Task ParallelAutoRedirectionRequestTestAsync()
        {
            await ParallelRequestTestAsync(() => GetRequest().AutoRedirection(true), result =>
            {
                Assert.AreEqual(Resource.RedirectEnd, result.Data);
            });
        }

        [TestMethod]
        public async Task ParallelNoRedirectionRequestTestAsync()
        {
            await ParallelRequestTestAsync(GetRequest, result =>
            {
                var location = result.ResponseMessage.Headers.Location.OriginalString.ToLowerInvariant();
                Assert.AreEqual("/api/redirection/r1", location);
            });
        }

        public async Task ParallelRequestTestAsync(Func<IHttpTurboRequest> action, Action<HttpOperationResult<string>> assertCallback)
        {
            HttpDefaultSetting.DefaultConnectionLimit = 500;

            var count = 10_000;
            var all = Enumerable.Range(0, count);

            var tasks = all.Select(m => action().TryGetAsStringAsync()).ToList();

            await Task.WhenAll(tasks);

            tasks.ForEach(m =>
            {
                assertCallback(m.Result);
            });
        }

        private IHttpTurboRequest GetRequest() => $"{TestServer.TestHost}/api/redirection".ToHttpRequest();

        #endregion 方法
    }
}