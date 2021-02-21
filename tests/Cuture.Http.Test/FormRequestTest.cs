using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Cuture.Http.Test.Server;
using Cuture.Http.Test.Server.Entity;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cuture.Http.Test
{
    [TestClass]
    public class FormRequestTest : WebServerHostTestBase
    {
        #region 方法

        [TestMethod]
        public async Task ContentTypeChangeTestAsync()
        {
            var (user, form) = NewUserWithForm();

            var contentType = "application/x-www-form-urlencoded; charset=utf-8; test=true";
            await ParallelRequestAsync(10_000,
                                       () => GetRequest().WithContent(new FormContent(form, contentType)).TryGetAsStringAsync(),
                                       result =>
                                       {
                                           Assert.AreEqual(contentType, result.ResponseMessage.Headers.GetValues("R-Content-Type").First().ToString());
                                           Assert.AreEqual(form, result.Data);
                                       });
        }

        [TestMethod]
        public async Task FromFormTestAsync()
        {
            var (user, form) = NewUserWithForm();

            await ParallelRequestAsync(10_000,
                                       () => GetRequest().WithFormContent(form).TryGetAsStringAsync(),
                                       result => Assert.AreEqual(form, result.Data));
        }

        [TestMethod]
        public async Task FromObjectTestAsync()
        {
            var (user, form) = NewUserWithForm();

            await ParallelRequestAsync(10_000,
                                       () => GetRequest().WithFormContent(user).TryGetAsStringAsync(),
                                       result => Assert.AreEqual(form, result.Data));
        }

        public static IHttpRequest GetRequest() => $"{TestWebHost.TestHost}/api/user/update/form".CreateHttpRequest().UsePost();

        private static (UserInfo user, string form) NewUserWithForm()
        {
            var user = new UserInfo()
            {
                Age = 10,
                Name = "TestUser中文😂😂😂"
            };

            var form = user.ToForm();
            var encodedForm = user.ToEncodedForm();

            Debug.WriteLine($"New UserInfo: {form}\nEncodedForm: {encodedForm}");

            return (user, form);
        }

        #endregion 方法
    }
}