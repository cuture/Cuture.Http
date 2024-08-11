namespace Cuture.Http;

/// <summary>
/// 基于 System.Text.Json 实现的 <see cref="IJsonSerializer"/>
/// </summary>
public class SystemJsonJsonSerializer : IJsonSerializer
{
    #region Private 字段

    private readonly JsonSerializerOptions _options;

    #endregion Private 字段

    #region Public 构造函数

    /// <inheritdoc cref="SystemJsonJsonSerializer"/>
    public SystemJsonJsonSerializer(JsonSerializerOptions? options = null)
    {
        _options = options ?? new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };
    }

    #endregion Public 构造函数

    #region Public 方法

    /// <inheritdoc/>
    public T? Deserialize<T>(string data) => JsonSerializer.Deserialize<T>(data, _options);

    /// <inheritdoc/>
    public ValueTask<T?> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken = default)
    {
        return JsonSerializer.DeserializeAsync<T?>(stream, _options, cancellationToken);
    }

    /// <inheritdoc/>
    public string Serialize(object value) => JsonSerializer.Serialize(value, _options);

    #endregion Public 方法
}
