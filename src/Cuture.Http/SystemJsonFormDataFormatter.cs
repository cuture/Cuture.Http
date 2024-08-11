using System.Text.Encodings.Web;

namespace Cuture.Http;

/// <summary>
/// 基于 System.Text.Json 实现的 <see cref="IFormDataFormatter"/>
/// </summary>
public class SystemJsonFormDataFormatter : IFormDataFormatter
{
    #region Private 字段

    private readonly JsonDocumentOptions _jsonDocumentOptions;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    #endregion Private 字段

    #region Public 构造函数

    /// <inheritdoc cref="SystemJsonFormDataFormatter"/>
    public SystemJsonFormDataFormatter(JsonDocumentOptions jsonDocumentOptions = default, JsonSerializerOptions? jsonSerializerOptions = null)
    {
        _jsonDocumentOptions = jsonDocumentOptions;
        _jsonSerializerOptions = jsonSerializerOptions ?? new JsonSerializerOptions()
        {
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            PropertyNameCaseInsensitive = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
    }

    #endregion Public 构造函数

    #region Public 方法

    /// <inheritdoc/>
    public string Format(object obj, FormDataFormatOptions options = default)
    {
        if (obj is null)
        {
            return string.Empty;
        }

        var jsonData = JsonSerializer.SerializeToUtf8Bytes(obj, _jsonSerializerOptions);
        using var jsonDocument = JsonDocument.Parse(jsonData, _jsonDocumentOptions);

        IEnumerable<JsonProperty> kvs = jsonDocument.RootElement.EnumerateObject();

        if (options.RemoveEmptyKey)
        {
            kvs = kvs.Where(m => !string.IsNullOrWhiteSpace(m.Value.ToString()));
        }

        var items = options.UrlEncode
                        ? kvs.Select(m => $"{FormContentUtil.Encode(m.Name)}={FormContentUtil.Encode(m.Value.ToString())}")
                        : kvs.Select(m => $"{m.Name}={m.Value}");

        return string.Join("&", items);
    }

    #endregion Public 方法
}
