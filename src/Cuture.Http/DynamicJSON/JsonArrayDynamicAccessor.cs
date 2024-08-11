using System.Collections;
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

    #region Public 属性

    public int Length => _jsonArray.Count;

    #endregion Public 属性

    #region Public 构造函数

    public JsonArrayDynamicAccessor(JsonArray jsonArray) : base(jsonArray)
    {
        _jsonArray = jsonArray ?? throw new ArgumentNullException(nameof(jsonArray));
    }

    #endregion Public 构造函数

    #region Public 方法

    public override IEnumerable<string> GetDynamicMemberNames()
    {
        yield return "Length";
    }

    public IEnumerator GetEnumerator()
    {
        return new JsonArrayEnumerator(_jsonArray);
    }

    public override bool TryConvert(ConvertBinder binder, out object? result)
    {
        if (binder.ReturnType == typeof(JsonArray))
        {
            result = _jsonArray;
            return true;
        }
        return base.TryConvert(binder, out result);
    }

    public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object? result)
    {
        //TODO 多维数组
        if (indexes.Length > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(indexes));
        }

        var index = indexes[0];

        if (index is int intIndex)
        {
            result = JsonNodeUtil.GetNodeAccessValue(_jsonArray[intIndex]);
            return true;
        }

#if NET6_0_OR_GREATER

        //低版本可以靠手动定义 System.Index 和 System.Range 类型进行兼容，但会污染命名空间，容易出现冲突
        else if (index is Index systemIndex)
        {
            result = JsonNodeUtil.GetNodeAccessValue(_jsonArray[systemIndex.GetOffset(_jsonArray.Count)]);
            return true;
        }
        else if (index is Range systemRange)
        {
            var (offset, length) = systemRange.GetOffsetAndLength(_jsonArray.Count);

            //创建一个新的 Json 对象进行操作
            //TODO 封装一个针对 JsonNode[] 的包装类，以不用创建新的 Json 对象
            using var memoryStream = new MemoryStream();
            using var writer = new Utf8JsonWriter(memoryStream, JSON.s_defaultJsonWriterOptions);

            writer.WriteStartArray();

            foreach (var item in _jsonArray.AsEnumerable().Skip(offset).Take(length))
            {
                if (item is null)
                {
                    writer.WriteNullValue();
                }
                else
                {
                    item.WriteTo(writer, JSON.s_defaultJsonSerializerOptions);
                }
            }

            writer.WriteEndArray();

            writer.Flush();

            memoryStream.Seek(0, SeekOrigin.Begin);

            result = JsonNodeUtil.GetNodeAccessValue(JsonNode.Parse(memoryStream, JSON.s_defaultJsonNodeOptions, JSON.s_defaultJsonDocumentOptions));

            return true;
        }

#endif
        throw new ArgumentException($"not support for index {index}.");
    }

    public override bool TryGetMember(GetMemberBinder binder, out object? result)
    {
        if (string.Equals(binder.Name, "Length", StringComparison.OrdinalIgnoreCase))
        {
            result = _jsonArray.Count;
            return true;
        }
        return base.TryGetMember(binder, out result);
    }

    public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object? value)
    {
        if (indexes.Length > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(indexes));
        }

        var index = indexes[0];

        if (index is int intIndex)
        {
            while (intIndex >= _jsonArray.Count)
            {
                _jsonArray.Add(null);
            }
            _jsonArray[intIndex] = JsonNode.Parse(JSON.stringify(value));
            return true;
        }
#if NET6_0_OR_GREATER

        //低版本可以靠手动定义 System.Index 和 System.Range 类型进行兼容，但会污染命名空间，容易出现冲突
        else if (index is Index systemIndex)
        {
            _jsonArray[systemIndex.GetOffset(_jsonArray.Count)] = JsonNode.Parse(JSON.stringify(value));
            return true;
        }
#endif

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
