using System;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cuture.Http.Test
{
    [TestClass]
    public class SimpleHttpMessageInvokerFactoryTest : HttpMessageInvokerFactoryTest<SimpleHttpMessageInvokerFactory>
    {
        #region Private 字段

        private const int HoldSeconds = 5;

        #endregion Private 字段

        #region Protected 方法

        protected override SimpleHttpMessageInvokerFactory CreateFactory()
        {
            return new SimpleHttpMessageInvokerFactory(true, TimeSpan.FromSeconds(HoldSeconds));
        }

        #endregion Protected 方法

        #region Public 方法

        [TestMethod]
        public async Task AutoReleaseTest()
        {
            var request = "http://127.0.0.1/index".ToHttpRequest();

            var firstClientHash = _factory.GetInvoker(request).GetHashCode();

            Assert.AreEqual(firstClientHash, _factory.GetInvoker(request).GetHashCode());

            await Task.Delay(TimeSpan.FromSeconds(HoldSeconds + 1));

            GC.Collect(2, GCCollectionMode.Forced);
            GC.WaitForPendingFinalizers();

            var lastClientHash = _factory.GetInvoker(request).GetHashCode();
            Assert.AreNotEqual(firstClientHash, lastClientHash);
        }

        #endregion Public 方法
    }
}