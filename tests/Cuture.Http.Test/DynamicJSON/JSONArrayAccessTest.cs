using System.Runtime.CompilerServices;

namespace Cuture.Http.Test.DynamicJSON;

[TestClass]
public class JSONArrayAccessTest
{
    #region Public 方法

    #region IndexRangeAccess

    [TestMethod]
    public void ShouldAccessSuccessfulByIndex()
    {
        DynamicJSONTestClass.GetTestValue(out var origin, out var json);

        for (var i = 1; i < origin.MyProperty6.Length; i++)
        {
            Assert.AreEqual(origin.MyProperty6[^i], json.MyProperty6[^i]);
        }
    }

    [TestMethod]
    public void ShouldSetSuccessfulByIndex()
    {
        DynamicJSONTestClass.GetTestValue(out var origin, out var json);

        for (var i = 1; i < origin.MyProperty6.Length; i++)
        {
            Assert.AreEqual(origin.MyProperty6[^i], json.MyProperty6[^i]);

            var value = Guid.NewGuid().ToString();
            origin.MyProperty6[^i] = value;
            json.MyProperty6[^i] = value;

            Assert.AreEqual(origin.MyProperty6[^i], json.MyProperty6[^i]);
        }
    }

    [TestMethod]
    public void ShouldAccessSuccessfulByRange()
    {
        DynamicJSONTestClass.GetTestValue(out var origin, out var json);

        for (var i = 0; i < origin.MyProperty6.Length; i++)
        {
            var originCollection = origin.MyProperty6[..i];
            var jsonCollection = json.MyProperty6[..i];
            Assert.AreEqual(originCollection.Length, jsonCollection.Length);
        }

        for (var i = 0; i < origin.MyProperty6.Length; i++)
        {
            var originCollection = origin.MyProperty6[i..];
            var jsonCollection = json.MyProperty6[i..];
            Assert.AreEqual(originCollection.Length, jsonCollection.Length);
        }

        for (var i = 0; i < origin.MyProperty6.Length; i++)
        {
            for (var j = i; j < origin.MyProperty6.Length; j++)
            {
                var originCollection = origin.MyProperty6[i..j];
                var jsonCollection = json.MyProperty6[i..j];
                Assert.AreEqual(originCollection.Length, jsonCollection.Length);
            }
        }
    }

    #endregion IndexRangeAccess

    [TestMethod]
    public void ShouldAccessSuccessfulByForeach()
    {
        DynamicJSONTestClass.GetTestValue(out var origin, out var json);

        var index = 0;
        foreach (var item in json.MyProperty6)
        {
            Assert.AreEqual(origin.MyProperty6[index++], item);
        }
    }

    [TestMethod]
    public void ShouldSetSuccessful()
    {
        DynamicJSONTestClass.GetTestValue(out var origin, out var json);

        for (var i = 1; i < origin.MyProperty6.Length; i++)
        {
            Assert.AreEqual(origin.MyProperty6[i], json.MyProperty6[i]);

            var value = Guid.NewGuid().ToString();
            origin.MyProperty6[i] = value;
            json.MyProperty6[i] = value;

            Assert.AreEqual(origin.MyProperty6[i], json.MyProperty6[i]);
        }
    }

    [TestMethod]
    public void ShouldThrowOutOfRangeException()
    {
        DynamicJSONTestClass.GetTestValue(out var origin, out var json);

        Assert.ThrowsException<ArgumentOutOfRangeException>(() => Access(json.MyProperty6[json.MyProperty6.length + 1]));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => Access(json.MyProperty6[-1]));
    }

    #endregion Public 方法

    #region Private 方法

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void Access(object? obj)
    { }

    #endregion Private 方法
}
