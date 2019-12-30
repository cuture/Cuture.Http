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
            return Content(Resource.RedirectEnd);
        }

        [HttpGet]
        public ActionResult Get()
        {
            return RedirectToAction("Redirection1");
        }

        [HttpGet]
        [Route("r1")]
        public ActionResult Redirection1()
        {
            return RedirectToAction("Redirection2");
        }

        [HttpGet]
        [Route("r2")]
        public ActionResult Redirection2()
        {
            return RedirectToAction("Redirection3");
        }

        [HttpGet]
        [Route("r3")]
        public ActionResult Redirection3()
        {
            return RedirectToAction("Redirection4");
        }

        [HttpGet]
        [Route("r4")]
        public ActionResult Redirection4()
        {
            return RedirectToAction("Final");
        }

        #endregion 方法
    }
}