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
        public string Format(object obj, FormDataFormatOptions options = default)
        {
            var jobject = JObject.FromObject(obj);
            var kvs = jobject.Children().Select(m =>
            {
                if (m is JProperty property)
                {
                    return property;
                }
                return null;
            }).Where(m => m != null);

            if (options.RemoveEmptyKey)
            {
                kvs = kvs.Where(m => !string.IsNullOrWhiteSpace(m!.Value.ToString()));
            }

            var items = options.UrlEncode
                            ? kvs.Select(m => $"{FormContentUtil.Encode(m!.Name)}={FormContentUtil.Encode(m!.Value.ToString())}")
                            : kvs.Select(m => $"{m!.Name}={m!.Value}");

            return string.Join("&", items);
        }

        #endregion Public 方法
    }
}