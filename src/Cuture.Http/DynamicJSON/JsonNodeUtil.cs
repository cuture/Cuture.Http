using System.Text.Json.Nodes;

namespace Cuture.Http.DynamicJSON;

internal static class JsonNodeUtil
{
    #region Internal 字段

    /// <summary>
    /// <see langword="false"/>
    /// </summary>
    internal static readonly object ObjectFalse = false;

    /// <summary>
    /// <see langword="true"/>
    /// </summary>
    internal static readonly object ObjectTrue = true;

    #endregion Internal 字段

    #region Public 方法

    public static object? GetJsonValueValue(JsonValue jsonValue)
    {
        var jsonValueElement = jsonValue.GetValue<JsonElement>();

        return jsonValueElement.ValueKind switch
        {
            JsonValueKind.Object or JsonValueKind.Array => throw new InvalidOperationException(),
            JsonValueKind.String => jsonValueElement.GetString(),
            JsonValueKind.Number => GetNodeNumberValue(jsonValueElement),
            JsonValueKind.True => ObjectTrue,
            JsonValueKind.False => ObjectFalse,
            _ => null,
        };
    }

    public static object? GetNodeAccessValue(JsonNode? jsonNode)
    {
        if (jsonNode is JsonValue jsonValue)
        {
            return GetJsonValueValue(jsonValue);
        }
        else if (jsonNode is JsonArray jsonArray)
        {
            return new JsonArrayDynamicAccessor(jsonArray);
        }

        return jsonNode is null ? null : new JsonObjectDynamicAccessor(jsonNode);
    }

    public static object GetNodeNumberValue(JsonElement jsonValueElement)
    {
        var rawText = jsonValueElement.GetRawText().Trim('\"');

        var valueSpan = rawText.AsSpan();

        if (valueSpan.Contains('.')
            && double.TryParse(valueSpan, out var vDouble))
        {
            return vDouble;
        }
        else if (int.TryParse(valueSpan, out var vInt32))
        {
            return vInt32;
        }
        else if (long.TryParse(valueSpan, out var vInt64))
        {
            return vInt64;
        }
        else if (decimal.TryParse(valueSpan, out var vDecimal))
        {
            return vDecimal;
        }

        return rawText;
    }

    #endregion Public 方法
}
