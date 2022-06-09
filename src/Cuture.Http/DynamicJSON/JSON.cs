using System;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

#pragma warning disable IDE1006 // 命名样式

namespace Cuture.Http.DynamicJSON;

/// <summary>
/// 一个简单的动态JSON访问工具类
/// </summary>
public static class JSON
{
    #region Private 字段

    private static readonly JsonSerializerOptions s_jsonSerializerOptions = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        PropertyNameCaseInsensitive = false,
    };

    #endregion Private 字段

    #region Public 构造函数

    static JSON()
    {
        s_jsonSerializerOptions.Converters.Add(new DynamicJsonConverter<JsonObjectDynamicAccessor>());
        s_jsonSerializerOptions.Converters.Add(new DynamicJsonConverter<JsonArrayDynamicAccessor>());
    }

    #endregion Public 构造函数

    #region Public 方法

    /// <summary>
    /// 将 <paramref name="obj"/> 转换为可动态访问的 JSON 对象
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static dynamic? create(object? obj) => parse(stringify(obj));

    /// <summary>
    /// 将 <paramref name="json"/> 转换为可动态访问的 JSON 对象
    /// </summary>
    /// <param name="json"></param>
    /// <returns></returns>
    public static dynamic? parse(string? json)
    {
        if (string.IsNullOrEmpty(json)
            || string.Equals("null", json, StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var jsonNode = JsonSerializer.Deserialize<JsonNode>(json, s_jsonSerializerOptions)!;
        return new JsonObjectDynamicAccessor(jsonNode);
    }

    /// <summary>
    /// 将 <paramref name="obj"/> 转换为 JSON 字符串
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static string stringify(object? obj)
    {
        if (obj is null)
        {
            return "null";
        }

        if (obj is JsonDynamicAccessor jsonDynamicAccessor)
        {
            return jsonDynamicAccessor.Node.ToJsonString(s_jsonSerializerOptions);
        }
        else
        {
            return JsonSerializer.Serialize(obj, obj.GetType(), s_jsonSerializerOptions);
        }
    }

    #endregion Public 方法

    #region Internal 类

    internal class DynamicJsonConverter<TTargetType> : JsonConverter<TTargetType>
        where TTargetType : JsonDynamicAccessor
    {
        #region Public 方法

        public override TTargetType? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException("Can not read from reader.");
        }

        public override void Write(Utf8JsonWriter writer, TTargetType value, JsonSerializerOptions options)
        {
            value.Node.WriteTo(writer, options);
        }

        #endregion Public 方法
    }

    #endregion Internal 类
}
