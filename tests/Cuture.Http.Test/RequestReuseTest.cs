using Cuture.Http.Test.Server;
using Cuture.Http.Test.Server.Entity;

namespace Cuture.Http.Test;

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

        using var firstRequestResult = await request.TryGetAsStringAsync();

        Assert.IsTrue(firstRequestResult.IsSuccessStatusCode);

        await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => request.GetAsStringAsync());
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
            using var requestResult = await request.TryGetAsObjectAsync<UserInfo>();

            Assert.IsTrue(requestResult.IsSuccessStatusCode);

            Assert.IsTrue(user.Equals(requestResult.Data));
        }
    }

    #endregion Public 方法
}
