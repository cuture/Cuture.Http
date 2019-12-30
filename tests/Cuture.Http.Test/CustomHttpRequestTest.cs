using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Cuture.Http.Test.Server;
using Cuture.Http.Test.Server.Entity;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Newtonsoft.Json.Linq;

namespace Cuture.Http.Test
{
    [TestClass]
    public class CustomHttpRequestTest : WebServerHostTestBase
    {
        #region 字段

        private readonly string _url = $"{TestServer.TestHost}/api/customrequest/post";
        private readonly UserInfo _user1;
        private readonly UserInfo _user2;

        #endregion 字段

        #region 构造函数

        public CustomHttpRequestTest()
        {
            _user1 = new UserInfo() { Name = "Test1", Age = 11 };
            _user2 = new UserInfo() { Name = "Test2", Age = 22 };
        }

        #endregion 构造函数

        #region 方法

        /// <summary>
        /// 获取请求
        /// </summary>
        /// <returns></returns>
        public IHttpTurboRequest GetRequest()
        {
            var content = new MultipartContent
            {
                new JsonContent(_user1),
                new JsonContent(_user2)
            };

            return _url.ToHttpRequest()
                        .UseVerb("CUSTOM")
                        .AddHeader("Header1", "Header1Value")
                        .AddHeaders(
                            new Dictionary<string, string>()
                            {
                                { "Header1", "Header1ValueNew" },
                                { "Header2", "Header2Value" },
                                { "Header3", "Header3Value" },
                                { "Header4", "Header4Value" },
                                { "Header5", "Header5Value" },
                            }
                        )
                        .AddHeader("Header4", string.Empty)
                        .AddNewHeader("Header5", "Header5ValueNew")
                        .WithContent(content);
        }

        [TestMethod]
        public async Task ParallelRequestTestAsync()
        {
            HttpDefaultSetting.DefaultConnectionLimit = 500;

            var count = 10_000;
            var all = Enumerable.Range(0, count);

            var tasks = all.Select(m => GetRequest().TryGetAsObjectAsync<HttpRequestInfo>()).ToArray();

            await Task.WhenAll(tasks);

            var fails = tasks.Where(m =>
            {
                var result = m.Result.Data;
                if (result != null)
                {
                    var content = JArray.Parse(Encoding.UTF8.GetString(result.Content));
                    var headers = result.Header;
                    Assert.AreEqual(true, _user1.Equals(content[0].ToObject<UserInfo>()));
                    Assert.AreEqual(true, _user2.Equals(content[1].ToObject<UserInfo>()));
                    Assert.AreEqual("POST", result.Method);
                    Assert.AreEqual(_url.ToLowerInvariant(), result.Url.ToLowerInvariant());

                    Assert.AreEqual("Header1Value, Header1ValueNew", headers["Header1"]);
                    Assert.AreEqual("Header2Value", headers["Header2"]);
                    Assert.AreEqual("Header3Value", headers["Header3"]);
                    Assert.AreEqual(false, headers.TryGetValue("Header4", out _));
                    Assert.AreEqual("Header5ValueNew", headers["Header5"]);

                    return false;
                }
                return true;
            }).ToArray();

            Assert.AreEqual(0, fails.Length);
        }

        #endregion 方法
    }
}