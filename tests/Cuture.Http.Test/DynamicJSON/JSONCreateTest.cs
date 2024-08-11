using Cuture.Http.DynamicJSON;

namespace Cuture.Http.Test.DynamicJSON;

[TestClass]
public class JSONCreateTest
{
    #region Public 方法

    [TestMethod]
    public void ShouldSuccessForArray()
    {
        var random = new Random();
        for (int size = 0; size < 100; size++)
        {
            var array = Enumerable.Range(0, size).Select(_ => random.Next()).ToArray();

            var jsonArray = JSON.create(array);

            Assert.AreEqual(array.Length, jsonArray.Length);
            Assert.AreEqual(array.Length, jsonArray.length);

            for (int i = 0; i < array.Length; i++)
            {
                Assert.AreEqual(array[i], jsonArray[i]);
            }
        }
    }

    [TestMethod]
    public void ShouldSuccessForClass()
    {
        var obj = new DynamicJSONTestClass();
        var json = JSON.create(obj);
        obj.Check(json);
    }

    [TestMethod]
    public void ShouldSuccessForNull()
    {
        Assert.AreEqual<object?>(null, JSON.create(null));
        Assert.AreEqual<object?>(null, JSON.create("null"));
    }

    [TestMethod]
    public void ShouldSuccessForStructure()
    {
        var obj = new DynamicJSONTestStructure();
        var json = JSON.create(obj);
        obj.Check(json);
    }

    [TestMethod]
    public void ShouldSuccessForValue()
    {
        Assert.AreEqual(true, JSON.create(true));
        Assert.AreEqual(false, JSON.create(false));
        Assert.AreEqual(1, JSON.create(1));
        Assert.AreEqual(1, JSON.create((float)1.0));
        Assert.AreEqual(1, JSON.create(1.0));
        Assert.AreEqual("1", JSON.create('1'));
        Assert.AreEqual("1", JSON.create("1"));
    }

    #endregion Public 方法
}
