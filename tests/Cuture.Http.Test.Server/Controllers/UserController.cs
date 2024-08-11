using System;
using System.Linq;

using Cuture.Http.Test.Server.Entity;

using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

namespace Cuture.Http.Test.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        #region Private 字段

        private static readonly Random s_random = new Random();

        #endregion Private 字段

        #region 方法

        [Route("update")]
        [HttpPost]
        public JsonResult Update(UserInfo userInfo)
        {
            Response.Headers.Append("R-Content-Type", Request.ContentType);
            return new JsonResult(userInfo);
        }

        [Route("update/form")]
        [HttpPost]
        public ContentResult Update()
        {
            Response.Headers.Append("R-Content-Type", Request.ContentType);
            var form = string.Join("&", Request.Form.Select(m => $"{m.Key}={m.Value}"));
            return Content(form);
        }

        [Route("update/callback")]
        [HttpPost]
        public string UpdateCallback(UserInfo userInfo)
        {
            Response.Headers.Append("R-Content-Type", Request.ContentType);
            var json = JsonConvert.SerializeObject(userInfo);
            return $"callback_{s_random.Next(100, 20000)}({json})";
        }

        #endregion 方法
    }
}
