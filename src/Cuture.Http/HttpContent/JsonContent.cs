using System.Net.Http;
using System.Text;

using Newtonsoft.Json;

namespace Cuture.Http
{
    /// <summary>
    /// 提供基于字符串的 HTTP 内容。
    /// MIME 为 "application/json"
    /// </summary>
    public class JsonContent : StringContent
    {
        #region 字段

        /// <summary>
        /// json httpcontent的MIME
        /// </summary>
        public static readonly string MIME = "application/json";

        #endregion 字段

        #region 构造函数

        /// <summary>
        /// 提供基于字符串的 HTTP 内容。
        /// </summary>
        /// <param name="content">用于转换json的实体对象</param>
        public JsonContent(object content) : base(JsonConvert.SerializeObject(content), Encoding.UTF8, MIME)
        {
        }

        /// <summary>
        /// 提供基于字符串的 HTTP 内容。
        /// </summary>
        /// <param name="json">json字符串</param>
        public JsonContent(string json) : base(json, Encoding.UTF8, MIME)
        {
        }

        #endregion 构造函数
    }
}