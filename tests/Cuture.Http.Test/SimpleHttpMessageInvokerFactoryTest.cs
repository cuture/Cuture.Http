using System;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cuture.Http.Test;

[TestClass]
public class SimpleHttpMessageInvokerFactoryTest : HttpMessageInvokerFactoryTest<SimpleHttpMessageInvokerPool>
{
    #region Private 字段

    private const int HoldSeconds = 2;

    #endregion Private 字段

    #region Protected 方法

    protected override SimpleHttpMessageInvokerPool CreateFactory()
    {
        return new SimpleHttpMessageInvokerPool(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2));
    }

    #endregion Protected 方法

    #region Public 方法

    [TestMethod]
    public async Task AutoReleaseTest()
    {
        var request = "http://127.0.0.1/index".CreateHttpRequest();

        var owner = _pool.Rent(request);
        var firstClientHash = owner.Value.GetHashCode();
        owner.Dispose();

        owner = _pool.Rent(request);
        Assert.AreEqual(firstClientHash, owner.Value.GetHashCode());
        owner.Dispose();

        await Task.Delay(TimeSpan.FromSeconds(HoldSeconds + HoldSeconds + 1));

        owner = _pool.Rent(request);
        var lastClientHash = owner.Value.GetHashCode();
        Assert.AreNotEqual(firstClientHash, lastClientHash);
    }

    #endregion Public 方法
}
