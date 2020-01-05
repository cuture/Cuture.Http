using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Cuture.Http.Test.Server.Entity;

using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

using Newtonsoft.Json;

namespace Cuture.Http.Test.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomRequestController : ControllerBase
    {
        #region 方法

        [Route("get")]
        [HttpGet]
        public JsonResult GetAsync()
        {
            return new JsonResult(new HttpRequestInfo()
            {
                Header = Request.Headers.ToDictionary(m => m.Key, m => m.Value.ToString()),
                Method = Request.Method,
                Url = Request.GetEncodedUrl(),
            });
        }

        [Route("post")]
        [HttpPost]
        public async Task<JsonResult> UpdateAsync()
        {
            var userJsons = new List<string>();

            var mpReader = new MultipartReader(Request.GetMultipartBoundary(), Request.Body);

            while (await mpReader.ReadNextSectionAsync() is MultipartSection section)
            {
                userJsons.Add(await section.ReadAsStringAsync());
            }

            var users = userJsons.Select(m => JsonConvert.DeserializeObject<UserInfo>(m)).ToArray();

            return new JsonResult(new HttpRequestInfo()
            {
                Header = Request.Headers.ToDictionary(m => m.Key, m => m.Value.ToString()),
                Method = Request.Method,
                Url = Request.GetEncodedUrl(),
                Content = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(users)),
            });
        }

        [Route("post2")]
        [HttpPost]
        public async Task<JsonResult> UpdateFormedAsync()
        {
            var userJsons = new Dictionary<string, string>();

            var mpReader = new MultipartReader(Request.GetMultipartBoundary(), Request.Body);

            while (await mpReader.ReadNextSectionAsync() is MultipartSection section)
            {
                userJsons.Add(section.GetContentDispositionHeader().Name.Value, await section.ReadAsStringAsync());
            }

            var users = userJsons.Select(m => new KeyValuePair<string, UserInfo>(m.Key, JsonConvert.DeserializeObject<UserInfo>(m.Value))).ToDictionary(m => m.Key, m => m.Value);

            return new JsonResult(new HttpRequestInfo()
            {
                Header = Request.Headers.ToDictionary(m => m.Key, m => m.Value.ToString()),
                Method = Request.Method,
                Url = Request.GetEncodedUrl(),
                Content = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(users)),
            });
        }

        #endregion 方法
    }
}