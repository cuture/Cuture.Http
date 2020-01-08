using System.Net.Http;
using System.Text;

using Newtonsoft.Json;

namespace Cuture.Http
{
    /// <summary>
    /// 提供基于json字符串的 HTTP 内容。
    /// </summary>
    public class JsonContent : ByteArrayContent
    {
        #region 字段

        /// <summary>
        /// <see cref="JsonContent"/> 的默认ContentType
        /// </summary>
        public const string ContentType = "application/json";

        #endregion 字段

        #region 构造函数

        /// <summary>
        /// 提供基于json字符串的 HTTP 内容。
        /// </summary>
        /// <param name="content">用于转换json的实体对象</param>
        public JsonContent(object content) : this(content, ContentType, Encoding.UTF8)
        {
        }

        /// <summary>
        /// 提供基于json字符串的 HTTP 内容。
        /// </summary>
        /// <param name="content">用于转换json的实体对象</param>
        /// <param name="encoding">指定编码类型</param>
        public JsonContent(object content, Encoding encoding) : this(content, ContentType, encoding)
        {
        }

        /// <summary>
        /// 提供基于json字符串的 HTTP 内容。
        /// </summary>
        /// <param name="content">用于转换json的实体对象</param>
        /// <param name="contentType">指定ContentType</param>
        public JsonContent(object content, string contentType) : this(content, contentType, Encoding.UTF8)
        {
        }

        /// <summary>
        /// 提供基于json字符串的 HTTP 内容。
        /// </summary>
        /// <param name="content">用于转换json的实体对象</param>
        /// <param name="contentType">指定ContentType</param>
        /// <param name="encoding">指定编码类型</param>
        public JsonContent(object content, string contentType, Encoding encoding) : base(encoding.GetBytes(JsonConvert.SerializeObject(content)))
        {
            Headers.TryAddWithoutValidation(HttpHeaders.ContentType, contentType);
        }

        /// <summary>
        /// 提供基于json字符串的 HTTP 内容。
        /// </summary>
        /// <param name="json">json字符串</param>
        public JsonContent(string json) : this(json, ContentType, Encoding.UTF8)
        {
        }

        /// <summary>
        /// 提供基于json字符串的 HTTP 内容。
        /// </summary>
        /// <param name="json">json字符串</param>
        /// <param name="encoding">指定编码类型</param>
        public JsonContent(string json, Encoding encoding) : this(json, ContentType, encoding)
        {
        }

        /// <summary>
        /// 提供基于json字符串的 HTTP 内容。
        /// </summary>
        /// <param name="json">json字符串</param>
        /// <param name="contentType">指定ContentType</param>
        public JsonContent(string json, string contentType) : this(json, contentType, Encoding.UTF8)
        {
        }

        /// <summary>
        /// 提供基于json字符串的 HTTP 内容。
        /// </summary>
        /// <param name="json">json字符串</param>
        /// <param name="contentType">指定ContentType</param>
        /// <param name="encoding">指定编码类型</param>
        public JsonContent(string json, string contentType, Encoding encoding) : base(encoding.GetBytes(json))
        {
            Headers.TryAddWithoutValidation(HttpHeaders.ContentType, contentType);
        }

        #endregion 构造函数
    }
}