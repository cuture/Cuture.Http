using System.Runtime.CompilerServices;

using Microsoft.CSharp.RuntimeBinder;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cuture.Http.Test.DynamicJSON;

[TestClass]
public class JSONFieldAccessTest
{
    #region Public 方法

    [TestMethod]
    public void ShouldThrowRuntimeBinderExceptionForErrorField()
    {
        DynamicJSONTestClass.GetTestValue(out var origin, out var json);

        Assert.ThrowsException<RuntimeBinderException>(() => Access(json.notexistfield));
    }

    #endregion Public 方法

    #region Private 方法

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void Access(object? obj)
    { }

    #endregion Private 方法
}
