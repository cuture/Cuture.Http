using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Cuture.Http.Test.Server;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cuture.Http.Test
{
    [TestClass]
    public class ProxyRequestTest : WebServerHostWithProxyTestBase
    {
        #region 字段

        private readonly Uri _proxyUri = new Uri($"http://{IPAddress.Loopback}:{ProxyTestServer.ProxyPort}");

        #endregion 字段

        #region 方法

        [TestMethod]
        public async Task ParallelProxyRequestTestAsync()
        {
            HttpDefaultSetting.DefaultConnectionLimit = 200;

            var proxyCount = 20;
            var everyRequestCount = 100;

            var proxys = new List<WebProxy>();

            for (int i = 0; i < proxyCount; i++)
            {
                var user = Guid.NewGuid().ToString("N");
                var password = Guid.NewGuid().ToString("N");
                ProxyServer.Authenticates.Add(user, new ProxyAuthenticateInfo()
                {
                    UserName = user,
                    Password = password,
                });
                proxys.Add(new WebProxy($"http://127.0.0.1:{ProxyTestServer.ProxyPort}")
                {
                    Credentials = new NetworkCredential(user, password)
                });
            };

            var count = proxys.Count * everyRequestCount;

            var all = Enumerable.Range(0, count);

            var tasks = all.Select(m => GetRequest().UseProxy(proxys[m % proxys.Count]).TryGetAsStringAsync()).ToList();

            await Task.WhenAll(tasks);

            tasks.ForEach(m =>
            {
                Assert.AreEqual(Resource.Index, m.Result.Data);
            });

            ProxyServer.Authenticates.Values.ToList().ForEach(m =>
            {
                Assert.AreEqual(everyRequestCount, m.RequestTime);
            });
        }

        private IHttpTurboRequest GetRequest() => TestServer.TestHost.ToHttpRequest();

        #endregion 方法
    }
}