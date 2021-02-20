using System.Text;
using System.Threading.Tasks;

using Cuture.Http.Test.Server;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cuture.Http.Test
{
    [TestClass]
    public class TextRequestTest : WebServerHostTestBase
    {
        #region 方法

        public IHttpRequest GetRequest() => TestServer.TestHost.CreateHttpRequest();

        [TestMethod]
        public async Task ParallelRequestTestAsync()
        {
            await ParallelRequestAsync(10_000,
                                       () => GetRequest().TryGetAsStringAsync(),
                                       result => Assert.AreEqual(Resource.Index, result.Data));
            //顺便检查一下编码注册
            Encoding.GetEncoding("GBK");
        }

        #endregion 方法
    }
}