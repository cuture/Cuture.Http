using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cuture.Http.Test
{
    /// <summary>
    /// host webapi服务以及代理服务的测试基类
    /// </summary>
    public abstract class WebServerHostWithProxyTestBase : WebServerHostTestBase
    {
        #region 字段

        protected ProxyTestServer ProxyServer = null;

        #endregion 字段

        #region 方法

        [TestCleanup]
        public override async Task CleanupAsync()
        {
            await base.CleanupAsync();
            ProxyServer.StopProxyServer();
        }

        [TestInitialize]
        public override async Task InitAsync()
        {
            await base.InitAsync();
            ProxyServer = new ProxyTestServer();
            ProxyServer.StartProxyServer();
        }

        #endregion 方法
    }
}