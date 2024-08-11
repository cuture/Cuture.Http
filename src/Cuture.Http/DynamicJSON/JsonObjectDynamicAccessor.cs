using System.Dynamic;
using System.Text.Json.Nodes;

namespace Cuture.Http.DynamicJSON;

internal class JsonObjectDynamicAccessor
    : JsonDynamicAccessor
    , IDynamicKeyValueEnumerable
{
    #region Private 字段

    private readonly JsonObject _jsonObject;

    #endregion Private 字段

    #region Public 构造函数

    public JsonObjectDynamicAccessor(JsonNode jsonNode) : base(jsonNode)
    {
        _jsonObject = jsonNode?.AsObject() ?? throw new ArgumentNullException(nameof(jsonNode));
    }

    #endregion Public 构造函数

    #region Public 方法

    public override IEnumerable<string> GetDynamicMemberNames()
    {
        foreach (var item in _jsonObject)
        {
            yield return item.Key;
        }
    }

    public override bool TryConvert(ConvertBinder binder, out object? result)
    {
        if (binder.ReturnType == typeof(JsonObject))
        {
            result = _jsonObject;
            return true;
        }
        return base.TryConvert(binder, out result);
    }

    public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object? result)
    {
        var propertyName = GetSingleStringIndex(indexes);

        return TryGetMember(propertyName, out result);
    }

    public override bool TryGetMember(GetMemberBinder binder, out object? result)
    {
        if (!TryGetMember(binder.Name, out result))
        {
            return base.TryGetMember(binder, out result);
        }
        return true;
    }

    public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object? value)
    {
        var propertyName = GetSingleStringIndex(indexes);

        SetProperty(propertyName, value);
        return true;
    }

    public override bool TrySetMember(SetMemberBinder binder, object? value)
    {
        SetProperty(binder.Name, value);
        return true;
    }

    #region IDynamicKeyValueEnumerable

    public IEnumerable<KeyValuePair<string, dynamic?>> AsEnumerable()
    {
        foreach (var item in _jsonObject)
        {
            yield return new(item.Key, JSON.create(item.Value));
        }
    }

    #endregion IDynamicKeyValueEnumerable

    #endregion Public 方法

    #region Private 方法

    private void SetProperty(string propertyName, object? value)
    {
        _jsonObject[propertyName] = JsonNode.Parse(JSON.stringify(value));
    }

    private bool TryGetMember(string propertyName, out object? result)
    {
        if (_jsonObject.TryGetPropertyValue(propertyName, out var jsonNode))
        {
            result = JsonNodeUtil.GetNodeAccessValue(jsonNode);
            return true;
        }
        result = null;
        return false;
    }

    #endregion Private 方法
}
