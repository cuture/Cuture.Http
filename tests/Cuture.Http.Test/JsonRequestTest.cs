using System.Diagnostics;

using Cuture.Http.Test.Server;
using Cuture.Http.Test.Server.Entity;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cuture.Http.Test
{
    [TestClass]
    public class JsonRequestTest : TextResultRequestTest
    {
        #region 字段

        private readonly string _json = null;
        private readonly UserInfo _user = null;

        #endregion 字段

        #region 构造函数

        public JsonRequestTest()
        {
            _user = new UserInfo()
            {
                Age = 10,
                Name = "TestUser"
            };

            _json = Newtonsoft.Json.JsonConvert.SerializeObject(_user);
            Debug.WriteLine(_json);
        }

        #endregion 构造函数

        #region 方法

        public override IHttpTurboRequest GetRequest() => $"{TestServer.TestHost}/api/user/update".ToHttpRequest().WithJsonContent(_user).UsePost();

        public override int GetRequestCount() => 10_000;

        public override string GetTargetResult() => _json;

        #endregion 方法
    }
}