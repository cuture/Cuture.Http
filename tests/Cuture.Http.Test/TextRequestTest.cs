using Cuture.Http.Test.Server;

namespace Cuture.Http.Test;

[TestClass]
public class TextRequestTest : WebServerHostTestBase
{
    #region Public 方法

    public static IHttpRequest GetRequest() => TestWebHost.TestHost.CreateHttpRequest();

    [TestMethod]
    public async Task ParallelRequestTestAsync()
    {
        await ParallelRequestAsync(10_000,
                                   () => GetRequest().TryGetAsStringAsync(),
                                   result => Assert.AreEqual(Resource.Index, result.Data));
    }

    #endregion Public 方法
}
