using System;
using System.Dynamic;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Cuture.Http.DynamicJSON;

internal abstract class JsonDynamicAccessor : DynamicObject
{
    #region Public 属性

    public JsonNode Node { get; }

    #endregion Public 属性

    #region Public 构造函数

    public JsonDynamicAccessor(JsonNode node)
    {
        Node = node ?? throw new ArgumentNullException(nameof(node));
    }

    #endregion Public 构造函数

    #region Public 方法

    public override string ToString()
    {
        return JSON.stringify(Node);
    }

    #endregion Public 方法

    #region Protected 方法

    private static readonly object s_false = false;
    private static readonly object s_true = true;

    protected static object? CreateNodeAccessValue(JsonNode? jsonNode)
    {
        if (jsonNode is JsonValue jsonValue)
        {
            var jsonValueElement = jsonValue.GetValue<JsonElement>();

            return jsonValueElement.ValueKind switch
            {
                JsonValueKind.Object or JsonValueKind.Array => throw new InvalidOperationException(),
                JsonValueKind.String => jsonValueElement.GetString(),
                JsonValueKind.Number => GetNumberValue(jsonValueElement),
                JsonValueKind.True => s_true,
                JsonValueKind.False => s_false,
                _ => null,
            };
        }
        else if (jsonNode is JsonArray jsonArray)
        {
            return new JsonArrayDynamicAccessor(jsonArray);
        }

        return jsonNode is null ? null : new JsonObjectDynamicAccessor(jsonNode);

        static object GetNumberValue(JsonElement jsonValueElement)
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
    }

    protected static object GetIndex(object[] indexes)
    {
        if (indexes.Length > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(indexes));
        }

        return indexes[0];
    }

    #endregion Protected 方法
}
