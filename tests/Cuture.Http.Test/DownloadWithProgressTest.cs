using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Cuture.Http.Test.Server;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cuture.Http.Test
{
    [TestClass]
    public class DownloadWithProgressTest : WebServerHostTestBase
    {
        #region 字段

        private readonly string _hash;
        private readonly string _url = $"{TestWebHost.TestHost}/data.dat";

        #endregion 字段

        #region 构造函数

        public DownloadWithProgressTest()
        {
            using var sha = SHA256.Create();
            _hash = BitConverter.ToString(sha.ComputeHash(Resource.Data));

            Debug.WriteLine($"Sha256:{_hash}");
        }

        #endregion 构造函数

        #region 方法

        [TestMethod]
        public async Task DownloadTimeOutTestAsync()
        {
            await Assert.ThrowsExceptionAsync<OperationCanceledException>(() => GetRequest().TimeOut(750).DownloadWithProgressAsync((count, downloaded) => { }, 40960));
        }

        [TestMethod]
        public async Task DownloadWithUnexpectedContentTestAsync()
        {
            var request = TestWebHost.TestHost.CreateHttpRequest();
            using var stream = new MemoryStream();
            await request.DownloadToStreamAsync(stream);

            Assert.AreEqual(Resource.Index, Encoding.UTF8.GetString(stream.ToArray()));
        }

        /// <summary>
        /// 获取请求
        /// </summary>
        /// <returns></returns>
        public IHttpRequest GetRequest() => _url.CreateHttpRequest().UseGet().AddHeader("Cache-Control", "no-cache");

        [TestMethod]
        public async Task ParallelRequestTestAsync()
        {
            var count = 500;
            int progressCount = 0;

            var tasks = Array(count).Select(m => GetRequest().DownloadWithProgressAsync((count, downloaded) =>
            {
                Interlocked.Increment(ref progressCount);
            }, 40960)).ToList();

            await Task.WhenAll(tasks);
            Assert.IsTrue(progressCount > count * 10);

            tasks.ForEach(m =>
            {
                var result = m.Result;
                Assert.IsNotNull(result);

                using var sha = SHA256.Create();
                var hash = BitConverter.ToString(sha.ComputeHash(result));
                Assert.AreEqual(_hash, hash);
            });
        }

        [TestMethod]
        public async Task SingleDownloadTestAsync()
        {
            int progressCount = 0;
            var result = await GetRequest().DownloadWithProgressAsync((count, downloaded) =>
            {
                Interlocked.Increment(ref progressCount);
                Debug.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} count:{count},downloaded:{downloaded}");
            }, 40960);

            Debug.WriteLine($"progressCount:{progressCount}");

            Assert.IsTrue(progressCount > 10);
            Assert.IsNotNull(result);

            using var sha = SHA256.Create();
            var hash = BitConverter.ToString(sha.ComputeHash(result));
            Assert.AreEqual(_hash, hash);
        }

        #endregion 方法
    }
}