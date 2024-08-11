namespace Cuture.Http.Test.DynamicJSON;

[TestClass]
public class JSONModifyTest
{
    #region Public 方法

    [TestMethod]
    public void ShouldModifyArraySuccess()
    {
        DynamicJSONTestClass.GetTestValue(out var origin, out var json);

        for (int i = 0; i < json.MyProperty6.length; i++)
        {
            Assert.IsNotNull(json.MyProperty6[i]);
            json.MyProperty6[i] = null;
            Assert.IsNull(json.MyProperty6[i]);
        }

        json.MyProperty6 = null;

        Assert.AreEqual<object?>(null, json.MyProperty6);

        json.MyProperty6 = origin.MyProperty6.ToArray();

        origin.Check(json);
    }

    [TestMethod]
    public void ShouldModifyFieldSuccess()
    {
        DynamicJSONTestClass.GetTestValue(out var origin, out var json);

        json.MyProperty1 = origin.MyProperty1 = 2;
        json.MyProperty2 = origin.MyProperty2 = 4;
        json.MyProperty3 = origin.MyProperty3 = "6";
        json.MyProperty6 = origin.MyProperty6 = new[] { "new", "new", "new", "new" };

        origin.Check(json);

        json.MyProperty111 = new { value = "6" };

        Assert.AreEqual("6", json.MyProperty111.value);
    }

    #endregion Public 方法
}
