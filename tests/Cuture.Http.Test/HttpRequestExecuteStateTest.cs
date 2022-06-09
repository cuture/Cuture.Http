using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cuture.Http.Test;

[TestClass]
public class HttpRequestExecuteStateTest
{
    #region Public 方法

    [TestMethod]
    public void ShoudCallDisposeOnlyOnce()
    {
        var owner = new OnceDisposeOwner();
        using var state = new HttpRequestExecuteState(new HttpResponseMessage(), owner);
        state.Dispose();
        Assert.AreEqual(1, owner.DisposeCount);
        state.Dispose();
        Assert.AreEqual(1, owner.DisposeCount);
    }

    [TestMethod]
    public void ShoudCallDisposeOnlyOnceWithParallel()
    {
        var owner = new OnceDisposeOwner();
        using var state = new HttpRequestExecuteState(new HttpResponseMessage(), owner);

        Parallel.For(0, Environment.ProcessorCount * 1000, _ =>
        {
            state.Dispose();
        });

        Assert.AreEqual(1, owner.DisposeCount);
        state.Dispose();
        Assert.AreEqual(1, owner.DisposeCount);
    }

    #endregion Public 方法

    #region Private 类

    private class OnceDisposeOwner : IOwner<HttpMessageInvoker>
    {
        #region Private 字段

        private int _disposeCount = 0;

        #endregion Private 字段

        #region Public 属性

        public int DisposeCount => _disposeCount;

        public HttpMessageInvoker Value { get; }

        #endregion Public 属性

        #region Public 方法

        public void Dispose()
        {
            Assert.AreEqual(1, Interlocked.Increment(ref _disposeCount), "Dispose call error.");
        }

        #endregion Public 方法
    }

    #endregion Private 类
}
