using System.Linq;

using Cuture.Http.Test.Server.Entity;

using Microsoft.AspNetCore.Mvc;

namespace Cuture.Http.Test.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        #region 方法

        [Route("update")]
        [HttpPost]
        public JsonResult Update(UserInfo userInfo)
        {
            Response.Headers.Add("R-Content-Type", Request.ContentType);
            return new JsonResult(userInfo);
        }

        [Route("update/form")]
        [HttpPost]
        public ContentResult Update()
        {
            Response.Headers.Add("R-Content-Type", Request.ContentType);
            var form = string.Join("&", Request.Form.Select(m => $"{m.Key}={m.Value}"));
            return Content(form);
        }

        #endregion 方法
    }
}