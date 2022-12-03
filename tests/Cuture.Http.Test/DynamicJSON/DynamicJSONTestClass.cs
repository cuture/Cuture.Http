using Cuture.Http.DynamicJSON;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cuture.Http.Test.DynamicJSON;

internal class DynamicJSONTestClass
{
    #region Public 属性

    public int MyProperty1 { get; set; } = 1;

    public double MyProperty2 { get; set; } = 2;

    public string MyProperty3 { get; set; } = nameof(MyProperty3);

    public object MyProperty4 { get; set; } = new
    {
        a = 1,
        b = 2,
        c = new
        {
            a = "3",
            b = "4"
        }
    };

    public string[] MyProperty6 { get; set; } = new[] { "1", "2", "3", "4", "5" };

    public int? NullableProperty { get; set; } = null;

    public object? NullProperty { get; set; } = null;

    #endregion Public 属性

    #region Public 方法

    public static void GetTestValue(out DynamicJSONTestClass origin, out dynamic json)
    {
        origin = new DynamicJSONTestClass();
        json = JSON.create(origin);
        origin.Check(json);
    }

    public void Check(dynamic json)
    {
        Assert.AreEqual(MyProperty1, json.MyProperty1);
        Assert.AreEqual(MyProperty2, json.MyProperty2);
        Assert.AreEqual(MyProperty3, json.MyProperty3);
        Assert.AreEqual(1, json.MyProperty4.a);
        Assert.AreEqual(2, json.MyProperty4.b);
        Assert.AreEqual("3", json.MyProperty4.c.a);
        Assert.AreEqual("4", json.MyProperty4.c.b);

        if (MyProperty6?.Length > 0)
        {
            for (int i = 0; i < MyProperty6.Length; i++)
            {
                Assert.AreEqual(MyProperty6[i], json.MyProperty6[i]);
            }
        }
    }

    #endregion Public 方法
}
