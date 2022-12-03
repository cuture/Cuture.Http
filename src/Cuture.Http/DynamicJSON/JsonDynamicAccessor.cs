using System;
using System.Collections.Generic;
using System.Dynamic;
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

    public override bool TryConvert(ConvertBinder binder, out object? result)
    {
        if (!base.TryConvert(binder, out result))
        {
            if (binder.ReturnType == typeof(IEnumerable<dynamic>))
            {
                if (this is IDynamicEnumerable dynamicEnumerable)
                {
                    result = dynamicEnumerable.AsEnumerable();
                    return true;
                }
                else if (this is IDynamicKeyValueEnumerable dynamicKeyValueEnumerable)
                {
                    result = EnumerateKeyValueToObject(dynamicKeyValueEnumerable.AsEnumerable());
                    return true;
                }
            }
            else if (binder.ReturnType == typeof(IEnumerable<KeyValuePair<string, dynamic?>>)
                     && this is IDynamicKeyValueEnumerable dynamicKeyValueEnumerable)
            {
                result = dynamicKeyValueEnumerable.AsEnumerable();
                return true;
            }
        }
        return false;

        static IEnumerable<dynamic> EnumerateKeyValueToObject(IEnumerable<KeyValuePair<string, dynamic?>> keyValueEnumerable)
        {
            foreach (var item in keyValueEnumerable)
            {
                yield return item;
            }
        }
    }

    public override bool TryGetMember(GetMemberBinder binder, out object? result)
    {
        if (!base.TryGetMember(binder, out result))
        {
            result = new Undefined(binder.Name);
        }
        return true;
    }

    #endregion Public 方法

    #region Protected 方法

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
