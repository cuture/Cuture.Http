using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Cuture.Http.Test.Server;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cuture.Http.Test;

[TestClass]
public class ProxyRequestTest : WebServerHostWithProxyTestBase
{
    #region 方法

    [TestMethod]
    public async Task DisableProxyRequestTestAsync()
    {
        var count = 50;

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

        HttpRequestGlobalOptions.DisableUseDefaultProxyByDefault = true;

        await ParallelRequestAsync(count,
                                   () => GetRequest().TryGetAsStringAsync(),
                                   AssertAction);

        HttpRequestGlobalOptions.DisableUseDefaultProxyByDefault = false;

        Assert.AreEqual(0, ProxyServer.SystemProxyInfo.RequestTime);

        ProxyServer.DisableSystemProxy();
    }

    [TestInitialize]
    public override async Task InitAsync()
    {
        await base.InitAsync();
        HttpRequestGlobalOptions.DefaultConnectionLimit = 2;
    }

    [TestMethod]
    public async Task ProxyRequestTestAsync()
    {
        var proxyCount = 5;
        var everyRequestCount = 10;

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

    private IHttpRequest GetRequest() => TestWebHost.TestHost.CreateHttpRequest();

    #endregion Base

    #endregion 方法
}
