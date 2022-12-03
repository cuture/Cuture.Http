using System;

using Cuture.Http.DynamicJSON;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cuture.Http.Test.DynamicJSON;

[TestClass]
public class UndefinedTest
{
    [TestMethod]
    public void ShouldReturnUndefinedForErrorField()
    {
        DynamicJSONTestClass.GetTestValue(out _, out var json);

        Assert.IsTrue(json.notexistfield == null);
        Assert.IsTrue(json.notexistfield == JSON.Undefined);
        Assert.IsTrue(JSON.isUndefined(json.notexistfield));
    }

    [TestMethod]
    public void ShouldThrowForOperation()
    {
        DynamicJSONTestClass.GetTestValue(out _, out var json);

        Assert.ThrowsException<InvalidOperationException>(() => json.notexistfield[1]);
        Assert.ThrowsException<InvalidOperationException>(() => json.notexistfield.length);
        Assert.ThrowsException<InvalidOperationException>(() => json.notexistfield + 1);
        Assert.ThrowsException<InvalidOperationException>(() => json.notexistfield - 1);
        Assert.ThrowsException<InvalidOperationException>(() => json.notexistfield * 1);
        Assert.ThrowsException<InvalidOperationException>(() => json.notexistfield / 1);
        Assert.ThrowsException<InvalidOperationException>(() => json.notexistfield % 1);
        Assert.ThrowsException<InvalidOperationException>(() => json.notexistfield ^ 1);
        Assert.ThrowsException<InvalidOperationException>(() => json.notexistfield | 1);
        Assert.ThrowsException<InvalidOperationException>(() => json.notexistfield & 1);
        Assert.ThrowsException<InvalidOperationException>(() => json.notexistfield << 1);
        Assert.ThrowsException<InvalidOperationException>(() => json.notexistfield >> 1);
        Assert.ThrowsException<InvalidOperationException>(() => ~json.notexistfield);
        Assert.ThrowsException<InvalidOperationException>(() => !json.notexistfield);
    }

    [TestMethod]
    public void NullIsSuperficialUndefined()
    {
        DynamicJSONTestClass.GetTestValue(out _, out var json);

        Assert.IsTrue(json.NullableProperty == null);
        Assert.IsTrue(json.NullProperty == null);
        Assert.IsTrue(json.NullableProperty == JSON.Undefined);
        Assert.IsTrue(json.NullProperty == JSON.Undefined);

        Assert.IsFalse(json.NullableProperty != null);
        Assert.IsFalse(json.NullProperty != null);
        Assert.IsFalse(json.NullableProperty != JSON.Undefined);
        Assert.IsFalse(json.NullProperty != JSON.Undefined);

        Assert.IsFalse(JSON.isUndefined(json.NullableProperty));
        Assert.IsFalse(JSON.isUndefined(json.NullProperty));
        Assert.IsFalse(JSON.isUndefined(null));
    }
}
