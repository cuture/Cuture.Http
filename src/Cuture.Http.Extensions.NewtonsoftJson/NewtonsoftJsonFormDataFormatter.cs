using System.Linq;

using Newtonsoft.Json.Linq;

namespace Cuture.Http
{
    /// <inheritdoc/>
    public class NewtonsoftJsonFormDataFormatter : IFormDataFormatter
    {
        //TODO 优化它

        #region Public 方法

        /// <inheritdoc/>
        public string Format(object obj)
        {
            var jobject = JObject.FromObject(obj);
            var kvs = jobject.Children().Select(m =>
            {
                if (m is JProperty property)
                {
                    return $"{property.Name}={property.Value}";
                }
                return null;
            });

            return string.Join("&", kvs);
        }

        /// <inheritdoc/>
        public string FormatToEncoded(object obj)
        {
            var jobject = JObject.FromObject(obj);
            var kvs = jobject.Children().Select(m =>
            {
                if (m is JProperty property)
                {
                    return $"{FormContentUtil.Encode(property.Name)}={FormContentUtil.Encode(property.Value.ToString())}";
                }
                return null;
            });

            return string.Join("&", kvs);
        }

        #endregion Public 方法
    }
}