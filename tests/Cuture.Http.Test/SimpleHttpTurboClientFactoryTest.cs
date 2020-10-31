using System;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cuture.Http.Test
{
    [TestClass]
    public class SimpleHttpTurboClientFactoryTest : HttpTurboClientFactoryTest<SimpleHttpTurboClientFactory>
    {
        #region Private 字段

        private const int _holdSeconds = 5;

        #endregion Private 字段

        #region Protected 方法

        protected override SimpleHttpTurboClientFactory CreateFactory()
        {
            return new SimpleHttpTurboClientFactory(true, TimeSpan.FromSeconds(_holdSeconds));
        }

        #endregion Protected 方法

        #region Public 方法

        [TestMethod]
        public async Task AutoReleaseTest()
        {
            var request = "http://127.0.0.1/index".ToHttpRequest();

            var firstClientHash = _factory.GetTurboClient(request).GetHashCode();

            Assert.AreEqual(firstClientHash, _factory.GetTurboClient(request).GetHashCode());

            await Task.Delay(TimeSpan.FromSeconds(_holdSeconds + 1));

            GC.Collect(2, GCCollectionMode.Forced);
            GC.WaitForPendingFinalizers();

            var lastClientHash = _factory.GetTurboClient(request).GetHashCode();
            Assert.AreNotEqual(firstClientHash, lastClientHash);
        }

        #endregion Public 方法
    }
}