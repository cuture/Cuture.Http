using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

using Cuture.Http.Test.Server;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cuture.Http.Test
{
    [TestClass]
    public class GetFileTest : WebServerHostTestBase
    {
        #region 字段

        private readonly string _hash;
        private readonly string _url = $"{TestServer.TestHost}/icon.png";

        #endregion 字段

        #region 构造函数

        public GetFileTest()
        {
            using var sha = SHA256.Create();
            _hash = BitConverter.ToString(sha.ComputeHash(Resource.icon));

            Debug.WriteLine($"Sha256:{_hash}");
        }

        #endregion 构造函数

        #region 方法

        /// <summary>
        /// 获取请求
        /// </summary>
        /// <returns></returns>
        public IHttpTurboRequest GetRequest() => _url.ToHttpRequest().UseGet().AddHeader("Cache-Control", "no-cache");

        [TestMethod]
        public async Task ParallelRequestTestAsync()
        {
            HttpDefaultSetting.DefaultConnectionLimit = 500;

            var tasks = Array(10_000).Select(m => GetRequest().TryGetAsBytesAsync()).ToList();

            await Task.WhenAll(tasks);

            tasks.ForEach(m =>
            {
                var result = m.Result.Data;
                Assert.IsNotNull(result, m.Result?.ResponseMessage?.ToString());

                using var sha = SHA256.Create();
                var hash = BitConverter.ToString(sha.ComputeHash(result));
                Assert.AreEqual(_hash, hash);
            });
        }

        #endregion 方法
    }
}