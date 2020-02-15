using System;

using Microsoft.AspNetCore.Mvc;

namespace Cuture.Http.Test.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RedirectionController : ControllerBase
    {
        #region 方法

        [HttpGet]
        [Route("final")]
        public ActionResult Final()
        {
            CheckAndSetCookie(5);
            return Content(Resource.RedirectEnd);
        }

        [HttpGet]
        public ActionResult Get()
        {
            Response.Cookies.Append("Redirection0", "true");
            return RedirectToAction("Redirection1");
        }

        [HttpGet]
        [Route("r1")]
        public ActionResult Redirection1()
        {
            CheckAndSetCookie(1);
            return RedirectToAction("Redirection2");
        }

        [HttpGet]
        [Route("r2")]
        public ActionResult Redirection2()
        {
            CheckAndSetCookie(2);
            return RedirectToAction("Redirection3");
        }

        [HttpGet]
        [Route("r3")]
        public ActionResult Redirection3()
        {
            CheckAndSetCookie(3);
            return RedirectToAction("Redirection4");
        }

        [HttpGet]
        [Route("r4")]
        public ActionResult Redirection4()
        {
            CheckAndSetCookie(4);
            return RedirectToAction("Final");
        }

        private void CheckAndSetCookie(int checkIndex)
        {
            string setKey = $"Redirection{checkIndex}";

            for (int i = checkIndex - 1; i >= 0; i--)
            {
                string checkKey = $"Redirection{i}";

                if (Request.Cookies.TryGetValue(checkKey, out var value)
                    && string.Equals("true", value, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                throw new ArgumentNullException($"{checkKey} not find");
            }

            Response.Cookies.Append(setKey, "true");
        }

        #endregion 方法
    }
}