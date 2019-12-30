using System.Threading.Tasks;

using Cuture.Http.Test.Server;

using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cuture.Http.Test
{
    /// <summary>
    /// host webapi服务的测试基类
    /// </summary>
    public abstract class WebServerHostTestBase
    {
        #region 字段

        protected IHost ServerHost = null;

        #endregion 字段

        #region 方法

        [TestCleanup]
        public virtual async Task CleanupAsync()
        {
            if (TestServer.HostByTestHost)
            {
                await ServerHost?.StopAsync();
            }
        }

        [TestInitialize]
        public virtual async Task InitAsync()
        {
            if (TestServer.HostByTestHost)
            {
                ServerHost = await TestServer.CreateHostBuilder(new string[0]).StartAsync();
            }
        }

        #endregion 方法
    }
}