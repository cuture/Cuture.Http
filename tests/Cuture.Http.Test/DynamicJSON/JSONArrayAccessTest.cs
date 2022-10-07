using System;
using System.Runtime.CompilerServices;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cuture.Http.Test.DynamicJSON;

[TestClass]
public class JSONArrayAccessTest
{
    #region Public 方法

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
