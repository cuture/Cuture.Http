using Cuture.Http.DynamicJSON;

namespace Cuture.Http.Test.DynamicJSON;

[TestClass]
public class JSONParseTest
{
    #region Public 方法

    [TestMethod]
    public void ShouldDeSerializePartialSuccess()
    {
        var obj = new DeSerializePartialClass1()
        {
            P = new()
            {
                P = new()
                {
                    Age = 10,
                    Name = "HelloWorld",
                }
            }
        };

        var json = JSON.create(obj);

        DeSerializePartialClass3 dp = json.P.P;

        Assert.IsNotNull(dp);

        Assert.AreEqual(10, dp.Age);
        Assert.AreEqual("HelloWorld", dp.Name);
    }

    [TestMethod]
    public void ShouldSuccessForNull()
    {
        Assert.AreEqual<object?>(null, JSON.parse(null));
        Assert.AreEqual<object?>(null, JSON.parse("null"));
    }

    [TestMethod]
    public void ShouldSuccessForObjectJson()
    {
        var rawJson =
            """
            {
            "a":1,
            "b":2.0,
            "c":{
                "a":"1",
                "b":"2",
                },
            }
            """;
        var json = JSON.parse(rawJson);

        Assert.AreEqual(1, json.a);
        Assert.AreEqual(2.0, json.b);

        Assert.AreEqual("1", json.c.a);
        Assert.AreEqual("2", json.c.b);
    }

    [TestMethod]
    public void ShouldSuccessForValueString()
    {
        Assert.AreEqual(true, JSON.parse("true"));
        Assert.AreEqual(false, JSON.parse("false"));
        Assert.AreEqual(1, JSON.parse("1"));
        Assert.AreEqual(1.0, JSON.parse("1.0"));
        Assert.AreEqual("1", JSON.parse("\"1\""));
    }

    private class DeSerializePartialClass1
    {
        #region Public 属性

        public DeSerializePartialClass2 P { get; set; }

        #endregion Public 属性
    }

    private class DeSerializePartialClass2
    {
        #region Public 属性

        public DeSerializePartialClass3 P { get; set; }

        #endregion Public 属性
    }

    private class DeSerializePartialClass3
    {
        #region Public 属性

        public int Age { get; set; }

        public string Name { get; set; }

        #endregion Public 属性
    }

    #endregion Public 方法
}
