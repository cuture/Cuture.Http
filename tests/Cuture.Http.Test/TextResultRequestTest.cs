using System.Linq;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cuture.Http.Test
{
    public abstract class TextResultRequestTest : WebServerHostTestBase
    {
        #region 方法

        /// <summary>
        /// 获取请求
        /// </summary>
        /// <returns></returns>
        public abstract IHttpTurboRequest GetRequest();

        /// <summary>
        /// 获取请求总次数
        /// </summary>
        /// <returns></returns>
        public abstract int GetRequestCount();

        /// <summary>
        /// 获取目标请求结果
        /// </summary>
        /// <returns></returns>
        public abstract string GetTargetResult();

        [TestMethod]
        public async Task ParallelRequestTestAsync()
        {
            HttpDefaultSetting.DefaultConnectionLimit = 200;

            var count = GetRequestCount();
            var target = GetTargetResult();
            var all = Enumerable.Range(0, count);

            var tasks = all.Select(m => GetRequest().TryGetAsStringAsync()).ToArray();

            await Task.WhenAll(tasks);

            var fails = tasks.Where(m => !target.Equals(m.Result.Data)).ToArray();
            Assert.AreEqual(0, fails.Length);
        }

        #endregion 方法
    }
}