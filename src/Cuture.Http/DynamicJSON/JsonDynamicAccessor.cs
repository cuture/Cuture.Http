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
