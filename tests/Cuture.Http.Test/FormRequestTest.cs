using System.Diagnostics;

using Cuture.Http.Test.Server;
using Cuture.Http.Test.Server.Entity;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cuture.Http.Test
{
    [TestClass]
    public class FormRequestTest : TextResultRequestTest
    {
        #region 字段

        private readonly string _form = null;
        private readonly UserInfo _user = null;

        #endregion 字段

        #region 构造函数

        public FormRequestTest()
        {
            _user = new UserInfo()
            {
                Age = 10,
                Name = "TestUser中文😂😂😂"
            };

            _form = _user.ToForm();

            Debug.WriteLine(_form);
        }

        #endregion 构造函数

        #region 方法

        public override IHttpTurboRequest GetRequest() => $"{TestServer.TestHost}/api/user/update/form".ToHttpRequest().WithFormConent(_user).UsePost();

        public override int GetRequestCount() => 10_000;

        public override string GetTargetResult() => _form;

        #endregion 方法
    }
}