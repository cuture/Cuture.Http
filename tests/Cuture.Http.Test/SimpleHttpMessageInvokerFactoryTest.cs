using System;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cuture.Http.Test;

[TestClass]
public class SimpleHttpMessageInvokerFactoryTest : HttpMessageInvokerFactoryTest<SimpleHttpMessageInvokerPool>
{
    #region Protected 方法

    protected override SimpleHttpMessageInvokerPool CreateFactory()
    {
        return new SimpleHttpMessageInvokerPool();
    }

    #endregion Protected 方法

    #region Public 方法

    [TestMethod]
    public async Task AutoReleaseTest()
    {
        const double HoldSeconds = 1.5;

        var pool = new SimpleHttpMessageInvokerPool(TimeSpan.FromSeconds(HoldSeconds), TimeSpan.FromSeconds(HoldSeconds));
        var request = "http://127.0.0.1/index".CreateHttpRequest();

        var owner = pool.Rent(request);
        var firstClientHash = owner.Value.GetHashCode();
        owner.Dispose();

        owner = pool.Rent(request);
        Assert.AreEqual(firstClientHash, owner.Value.GetHashCode());

        owner.Dispose();
        await Task.Delay(TimeSpan.FromSeconds((HoldSeconds + HoldSeconds) * 2));

        owner = pool.Rent(request);
        var lastClientHash = owner.Value.GetHashCode();
        Assert.AreNotEqual(firstClientHash, lastClientHash);
    }

    #endregion Public 方法
}
