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

        [Route("post")]
        [HttpPost]
        public async Task<JsonResult> UpdateAsync()
        {
            var mpReader = new MultipartReader(Request.GetMultipartBoundary(), Request.Body);
            var section1 = await mpReader.ReadNextSectionAsync();
            var json1 = await section1.ReadAsStringAsync();
            var section2 = await mpReader.ReadNextSectionAsync();
            var json2 = await section2.ReadAsStringAsync();

            var user1 = JsonConvert.DeserializeObject<UserInfo>(json1);
            var user2 = JsonConvert.DeserializeObject<UserInfo>(json2);

            return new JsonResult(new HttpRequestInfo()
            {
                Header = Request.Headers.ToDictionary(m => m.Key, m => m.Value.ToString()),
                Method = Request.Method,
                Url = Request.GetEncodedUrl(),
                Content = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new UserInfo[] { user1, user2 })),
            });
        }

        #endregion 方法
    }
}