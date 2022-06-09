using Cuture.Http.DynamicJSON;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cuture.Http.Test.DynamicJSON;

[TestClass]
public class JSONParseTest
{
    #region Public 方法

    [TestMethod]
    public void ShouldSuccessForNull()
    {
        Assert.AreEqual(null, JSON.parse(null));
        Assert.AreEqual(null, JSON.parse("null"));
    }

    [TestMethod]
    public void ShouldSuccessForObjectJson()
    {
        var rawJson = @"{
""a"":1,
""b"":2.0,
""c"":{
    ""a"":""1"",
    ""b"":""2"",
    },
}
";
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

    #endregion Public 方法
}
