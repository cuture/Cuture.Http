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
        #region 构造函数

        public RandomUserAgentTest()
        {
            HttpDefaultSetting.DefaultConnectionLimit = 200;
        }

        #endregion 构造函数

        #region 方法

        public IHttpTurboRequest GetRequest() => $"{TestServer.TestHost}/api/customrequest/get".ToHttpRequest();

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
        private async Task ParallelRequestAsync(Func<IHttpTurboRequest> getRequest, Func<string> getUserAgent)
        {
            var count = GetRequestCount();
            var target = GetTargetResult();
            var all = Enumerable.Range(0, count);

            var tasksWithUA = all.Select(m =>
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

                Assert.AreEqual(userAgent, result.Header[HttpHeaders.UserAgent]);
            });
        }

        #endregion 方法
    }
}