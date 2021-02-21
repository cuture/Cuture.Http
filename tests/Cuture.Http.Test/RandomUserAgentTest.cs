using System;
using System.Linq;
using System.Threading.Tasks;

using Cuture.Http.Test.Server;
using Cuture.Http.Test.Server.Entity;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cuture.Http.Test
{
    [TestClass]
    public class RandomUserAgentTest : WebServerHostTestBase
    {
        #region 方法

        public IHttpRequest GetRequest() => $"{TestWebHost.TestHost}/api/customrequest/get".CreateHttpRequest();

        public int GetRequestCount() => 10_000;

        public string GetTargetResult() => Resource.Index;

        [TestMethod]
        public async Task RandomChromeUserAgentTestAsync()
        {
            await ParallelRequestAsync(() => GetRequest(), () => UserAgents.RandomChromeUserAgent());
        }

        [TestMethod]
        public async Task RandomFirefoxUserAgentTestAsync()
        {
            await ParallelRequestAsync(() => GetRequest(), () => UserAgents.RandomFirefoxUserAgent());
        }

        [TestMethod]
        public async Task RandomMobileUserAgentTestAsync()
        {
            await ParallelRequestAsync(() => GetRequest(), () => UserAgents.RandomMobileUserAgent());
        }

        [TestMethod]
        public async Task RandomModifiedUserAgentTestAsync()
        {
            await ParallelRequestAsync(() => GetRequest(), () => UserAgents.RandomModifiedUserAgent());
        }

        [TestMethod]
        public async Task RandomUserAgentTestAsync()
        {
            await ParallelRequestAsync(() => GetRequest(), () => UserAgents.RandomUserAgent());
        }

        [TestMethod]
        private async Task ParallelRequestAsync(Func<IHttpRequest> getRequest, Func<string> getUserAgent)
        {
            var target = GetTargetResult();

            var tasksWithUA = Array(GetRequestCount()).Select(m =>
            {
                var userAgent = getUserAgent();
                return new Tuple<Task<TextHttpOperationResult<HttpRequestInfo>>, string>(
                                getRequest().UseUserAgent(userAgent).TryGetAsObjectAsync<HttpRequestInfo>(),
                                userAgent);
            }).ToList();

            var tasks = tasksWithUA.Select(m => m.Item1).ToArray();

            await Task.WhenAll(tasks);

            tasksWithUA.ForEach(m =>
            {
                var result = m.Item1.Result?.Data;
                var userAgent = m.Item2;

                Assert.IsNotNull(result);
                Assert.IsFalse(string.IsNullOrWhiteSpace(userAgent));

                Assert.IsTrue(UserAgentEquals(userAgent, result.Header[HttpHeaderDefinitions.UserAgent]));
            });
        }

        #endregion 方法
    }
}