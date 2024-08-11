using System.Security.Cryptography;

using Cuture.Http.Test.Server;

namespace Cuture.Http.Test;

[TestClass]
public class RawDataContentRequestTest : WebServerHostTestBase
{
    #region 方法

    [TestMethod]
    public async Task Should_Success_With_Out_ContentType()
    {
        var request = $"{TestWebHost.TestHost}/api/customrequest/hashcontent".CreateHttpRequest();
        var data = new byte[512 * 1024];
        Random.Shared.NextBytes(data);
        request.WithContent(data.AsMemory(), null).UsePost();
        var hash = await request.GetAsStringAsync();
        Assert.AreEqual(Convert.ToHexString(MD5.HashData(data)), hash);
    }

    #endregion 方法
}
