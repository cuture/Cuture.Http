using System;
using System.Threading.Tasks;

using Cuture.Http.Test.Server;
using Cuture.Http.Test.Server.Entity;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cuture.Http.Test
{
    [TestClass]
    public class RequestReuseTest : WebServerHostTestBase
    {
        #region Public 方法

        [TestMethod]
        public async Task DefaultHttpRequestTest()
        {
            var user = new UserInfo()
            {
                Age = 10,
                Name = "TestUser",
            };
            var request = $"{TestWebHost.TestHost}/api/user/update".CreateHttpRequest()
                                                                   .UsePost()
                                                                   .WithJsonContent(user);

            var firstRequestResult = await request.TryGetAsStringAsync();

            Assert.IsTrue(firstRequestResult.IsSuccessStatusCode);

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => request.TryGetAsStringAsync());
        }

        [TestMethod]
        public async Task ReuseableHttpRequestTest()
        {
            var user = new UserInfo()
            {
                Age = 10,
                Name = "TestUser",
            };
            var request = $"{TestWebHost.TestHost}/api/user/update".CreateHttpRequest(true)
                                                                   .UsePost()
                                                                   .WithJsonContent(user);

            for (int i = 0; i < 10; i++)
            {
                var requestResult = await request.TryGetAsObjectAsync<UserInfo>();

                Assert.IsTrue(requestResult.IsSuccessStatusCode);

                Assert.IsTrue(user.Equals(requestResult.Data));
            }
        }

        #endregion Public 方法
    }
}