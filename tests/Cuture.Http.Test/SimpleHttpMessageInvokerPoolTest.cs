using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cuture.Http.Test;

[TestClass]
public class SimpleHttpMessageInvokerPoolTest : HttpMessageInvokerPoolTest<SimpleHttpMessageInvokerPool>
{
    #region Protected 方法

    protected override SimpleHttpMessageInvokerPool CreatePool()
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

    [TestMethod]
    public void ShouldThrowArgumentExceptionWithInvalidTime()
    {
        var datas = new TimeSpan[][] {
           new [] { TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10) },
           new [] { TimeSpan.Zero, TimeSpan.Zero },
           new [] { Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan },
           new [] { TimeSpan.FromMilliseconds(-1), TimeSpan.FromMilliseconds(-1) },
        };
        for (int i = 0; i < datas.Length; i++)
        {
            for (int j = 0; j < datas.Length; j++)
            {
                if (i == 0
                    && j == 0)
                {
                    continue;
                }
                Assert.ThrowsException<ArgumentOutOfRangeException>(() => new SimpleHttpMessageInvokerPool(datas[i][0], datas[j][1]));
            }
        }
    }

    #endregion Public 方法
}
