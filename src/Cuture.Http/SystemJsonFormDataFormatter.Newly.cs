using System.Text;
using System.Text.Json;

namespace Cuture.Http
{
    /// <summary>
    /// 基于 System.Text.Json 实现的 <see cref="IFormDataFormatter"/>
    /// </summary>
    public class SystemJsonFormDataFormatter : IFormDataFormatter
    {
        #region Private 字段

        private readonly JsonDocumentOptions _jsonDocumentOptions;
        private readonly JsonSerializerOptions? _jsonSerializerOptions;

        #endregion Private 字段

        #region Public 构造函数

        /// <inheritdoc cref="SystemJsonFormDataFormatter"/>
        public SystemJsonFormDataFormatter(JsonDocumentOptions jsonDocumentOptions = default, JsonSerializerOptions? jsonSerializerOptions = null)
        {
            _jsonDocumentOptions = jsonDocumentOptions;
            _jsonSerializerOptions = jsonSerializerOptions ?? new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            };
        }

        #endregion Public 构造函数

        #region Public 方法

        /// <inheritdoc/>
        public string Format(object obj)
        {
            if (obj is null)
            {
                return string.Empty;
            }
            var jsonData = JsonSerializer.SerializeToUtf8Bytes(obj, _jsonSerializerOptions);
            using var jsonDocument = JsonDocument.Parse(jsonData, _jsonDocumentOptions);

            var builder = new StringBuilder(512);

            foreach (var item in jsonDocument.RootElement.EnumerateObject())
            {
                if (item.Value.GetRawText() is string value
                   && !string.IsNullOrEmpty(value))
                {
                    builder.AppendFormat("{0}={1}&", item.Name, value);
                }
            }

            if (builder.Length > 0)
            {
                builder.Length -= 1;
            }

            return builder.ToString();
        }

        /// <inheritdoc/>
        public string FormatToEncoded(object obj)
        {
            if (obj is null)
            {
                return string.Empty;
            }
            var jsonData = JsonSerializer.SerializeToUtf8Bytes(obj, _jsonSerializerOptions);
            using var jsonDocument = JsonDocument.Parse(jsonData, _jsonDocumentOptions);

            var builder = new StringBuilder(512);

            foreach (var item in jsonDocument.RootElement.EnumerateObject())
            {
                if (item.Value.GetRawText() is string value
                   && !string.IsNullOrEmpty(value))
                {
                    builder.AppendFormat("{0}={1}&", FormContentUtil.Encode(item.Name), FormContentUtil.Encode(value));
                }
            }

            if (builder.Length > 0)
            {
                builder.Length -= 1;
            }

            return builder.ToString();
        }

        #endregion Public 方法
    }
}