﻿using System;
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
        #region 方法

        [TestMethod]
        public async Task DisableProxyRequestTestAsync()
        {
            var count = 2_000;

            ProxyServer.SetAsSystemProxy();

            static void AssertAction(HttpOperationResult<string> result)
            {
                Assert.IsFalse(result.ResponseMessage.Headers.Contains(ProxyTestServer.ThroughProxy));
                Assert.AreEqual(Resource.Index, result.Data);
            }

            await ParallelRequestAsync(count,
                                       () => GetRequest().UseSystemProxy().DisableProxy().TryGetAsStringAsync(),
                                       AssertAction);

            Assert.AreEqual(0, ProxyServer.SystemProxyInfo.RequestTime);

            HttpRequestOptions.DisableUseDefaultProxyByDefault = true;

            await ParallelRequestAsync(count,
                                       () => GetRequest().TryGetAsStringAsync(),
                                       AssertAction);

            HttpRequestOptions.DisableUseDefaultProxyByDefault = false;

            Assert.AreEqual(0, ProxyServer.SystemProxyInfo.RequestTime);

            ProxyServer.DisableSystemProxy();
        }

        [TestMethod]
        public async Task ProxyRequestTestAsync()
        {
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
                Assert.IsTrue(m.Result.ResponseMessage.Headers.Contains(ProxyTestServer.ThroughProxy));
                Assert.AreEqual(Resource.Index, m.Result.Data);
            });

            ProxyServer.Authenticates.Values.ToList().ForEach(m =>
            {
                Assert.AreEqual(everyRequestCount, m.RequestTime);
            });
        }

        #region Base

        private IHttpRequest GetRequest() => TestServer.TestHost.CreateHttpRequest();

        #endregion Base

        #endregion 方法
    }
}