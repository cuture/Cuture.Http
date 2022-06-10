using System.Linq;
using System.Runtime.CompilerServices;

using Cuture.Http.DynamicJSON;

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
        var origin = new DynamicJSONTestClass();
        var json = JSON.create(origin);
        origin.Check(json);

        Assert.ThrowsException<RuntimeBinderException>(() => Access(json.notexistfield));
    }

    #endregion Public 方法

    [MethodImpl(MethodImplOptions.NoInlining)]
    static void Access(object? obj) { }
}
