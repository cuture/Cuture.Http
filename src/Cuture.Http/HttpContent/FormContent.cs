using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Cuture.Http
{
    /// <summary>
    /// HttpFormContent
    /// </summary>
    public class FormContent : ByteArrayContent
    {
        #region 字段

        /// <summary>
        /// json httpcontent的MIME
        /// </summary>
        public static readonly string MIME = "application/x-www-form-urlencoded";

        /// <summary>
        /// 空的Content
        /// </summary>
        private static readonly byte[] EmptyContent = Array.Empty<byte>();

        #endregion 字段

        #region 构造函数

        /// <summary>
        /// HttpFormContent
        /// </summary>
        /// <param name="content">已编码的from内容</param>
        public FormContent(string content) : base(GetBytes(content))
        {
            Headers.ContentType = new MediaTypeHeaderValue(MIME);
        }

        /// <summary>
        /// HttpFormContent
        /// </summary>
        /// <param name="content">用于转换为form的对象</param>
        public FormContent(object content) : base(GetBytes(content))
        {
            Headers.ContentType = new MediaTypeHeaderValue(MIME);
        }

        #endregion 构造函数

        #region 方法

        private static byte[] GetBytes(object content)
        {
            if (content is null)
            {
                return EmptyContent;
            }
            else if (content is string str)
            {
                return GetBytes(str);
            }

            var data = string.Empty;

            if (string.IsNullOrEmpty(data))
            {
                data = content.ToEncodedForm();
            }

            return Encoding.UTF8.GetBytes(data);
        }

        private static byte[] GetBytes(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return EmptyContent;
            }

            return Encoding.UTF8.GetBytes(data.Replace("%20", "+"));
        }

        #endregion 方法
    }
}