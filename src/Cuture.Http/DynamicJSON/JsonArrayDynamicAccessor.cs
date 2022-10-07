using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Text.Json.Nodes;

namespace Cuture.Http.DynamicJSON;

internal class JsonArrayDynamicAccessor
    : JsonDynamicAccessor
    , IEnumerable
    , IDynamicEnumerable
{
    #region Private 字段

    private readonly JsonArray _jsonArray;

    #endregion Private 字段

    #region Public 构造函数

    public JsonArrayDynamicAccessor(JsonArray jsonArray) : base(jsonArray)
    {
        _jsonArray = jsonArray ?? throw new ArgumentNullException(nameof(jsonArray));
    }

    #endregion Public 构造函数

    #region Public 方法

    public IEnumerator GetEnumerator()
    {
        return new JsonArrayEnumerator(_jsonArray);
    }

    public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object? result)
    {
        var index = GetIndex(indexes);
        if (index is int intIndex)
        {
            result = JsonNodeUtil.GetNodeAccessValue(_jsonArray[intIndex]);
            return true;
        }

        throw new ArgumentException($"not support for index {index}.");
    }

    public override bool TryGetMember(GetMemberBinder binder, out object? result)
    {
        if (string.Equals(binder.Name, "Length", StringComparison.OrdinalIgnoreCase))
        {
            result = _jsonArray.Count;
            return true;
        }
        result = null;
        return false;
    }

    public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object? value)
    {
        var index = GetIndex(indexes);

        if (index is int intIndex)
        {
            while (intIndex >= _jsonArray.Count)
            {
                _jsonArray.Add(null);
            }
            _jsonArray[intIndex] = JsonNode.Parse(JSON.stringify(value));
            return true;
        }

        throw new ArgumentException($"not support for index {index}.");
    }

    #region IDynamicEnumerable

    public IEnumerable<dynamic?> AsEnumerable()
    {
        var enumerator = GetEnumerator();
        while (enumerator.MoveNext())
        {
            yield return enumerator.Current;
        }
    }

    #endregion IDynamicEnumerable

    #endregion Public 方法

    #region Private 类

    private class JsonArrayEnumerator : IEnumerator
    {
        #region Private 字段

        private readonly JsonArray _jsonArray;

        private int _index = -1;

        #endregion Private 字段

        #region Public 属性

        public object Current => JsonNodeUtil.GetNodeAccessValue(_jsonArray[_index])!;

        #endregion Public 属性

        #region Public 构造函数

        public JsonArrayEnumerator(JsonArray jsonArray)
        {
            _jsonArray = jsonArray;
        }

        #endregion Public 构造函数

        #region Public 方法

        public bool MoveNext()
        {
            if (_index < _jsonArray.Count - 1)
            {
                _index++;
                return true;
            }
            return false;
        }

        public void Reset()
        {
            _index = -1;
        }

        #endregion Public 方法
    }

    #endregion Private 类
}
