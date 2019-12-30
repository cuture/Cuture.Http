using System;
using System.Linq;

using Newtonsoft.Json.Linq;

namespace Cuture.Http
{
    /// <summary>
    /// 一般的工具拓展
    /// </summary>
    public static class FormContentUtil
    {
        #region 方法

        /// <summary>
        /// UrlEncode
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Encode(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return string.Empty;
            }
            return Uri.EscapeDataString(data).Replace("%20", "+");
        }

        /// <summary>
        /// 获取对象的已编码form表单
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string ToEncodedForm(this object content)
        {
            //TODO 优化它
            var jobject = JObject.FromObject(content);
            var kvs = jobject.Children().Select(m =>
            {
                if (m is JProperty property)
                {
                    return $"{Encode(property.Name)}={Encode(property.Value.ToString())}";
                }
                return null;
            });

            return string.Join("&", kvs);
        }

        /// <summary>
        /// 获取对象的form表单
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string ToForm(this object content)
        {
            //TODO 优化它
            var jobject = JObject.FromObject(content);
            var kvs = jobject.Children().Select(m =>
            {
                if (m is JProperty property)
                {
                    return $"{property.Name}={property.Value.ToString()}";
                }
                return null;
            });

            return string.Join("&", kvs);
        }

        #endregion 方法
    }
}