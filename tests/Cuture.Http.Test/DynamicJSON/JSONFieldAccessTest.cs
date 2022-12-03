using System.Runtime.CompilerServices;

using Cuture.Http.DynamicJSON;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cuture.Http.Test.DynamicJSON;

[TestClass]
public class JSONFieldAccessTest
{
    #region Public 方法

    [TestMethod]
    public void ShouldReturnUndefinedForErrorField()
    {
        DynamicJSONTestClass.GetTestValue(out _, out var json);

        Assert.IsTrue(json.notexistfield == null);
        Assert.IsTrue(json.notexistfield == JSON.Undefined);
        Assert.IsTrue(JSON.isUndefined(json.notexistfield));
    }

    #endregion Public 方法

    #region Private 方法

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void Access(object? obj)
    { }

    #endregion Private 方法
}
