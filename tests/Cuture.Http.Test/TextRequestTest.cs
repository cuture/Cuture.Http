using System.Threading.Tasks;

using Cuture.Http.Test.Server;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cuture.Http.Test;

[TestClass]
public class TextRequestTest : WebServerHostTestBase
{
    #region ����

    public static IHttpRequest GetRequest() => TestWebHost.TestHost.CreateHttpRequest();

    [TestMethod]
    public async Task ParallelRequestTestAsync()
    {
        await ParallelRequestAsync(10_000,
                                   () => GetRequest().TryGetAsStringAsync(),
                                   result => Assert.AreEqual(Resource.Index, result.Data));
    }

    #endregion ����
}
