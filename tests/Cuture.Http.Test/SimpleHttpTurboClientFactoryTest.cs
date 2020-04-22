using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cuture.Http.Test
{
    [TestClass]
    public class SimpleHttpTurboClientFactoryTest : HttpTurboClientFactoryTest<SimpleHttpTurboClientFactory>
    {
        #region Protected 方法

        protected override SimpleHttpTurboClientFactory CreateFactory()
        {
            return new SimpleHttpTurboClientFactory();
        }

        #endregion Protected 方法
    }
}