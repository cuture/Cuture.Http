using Cuture.Http.DynamicJSON;

namespace Cuture.Http.Test.DynamicJSON;

[TestClass]
public class UndefinedTest
{
    [TestMethod]
    public void DeepUndefinedCheck()
    {
        DynamicJSONTestClass.GetTestValue(out _, out var json);

        Assert.IsTrue(JSON.isUndefined(() => json.a.b.c.e.f.g.h.i.j.k));
        Assert.IsFalse(JSON.isUndefined(() => json.MyProperty4.c.a));
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
        Assert.IsFalse(JSON.isUndefined(value: null));
    }

    [TestMethod]
    public void ShouldReturnUndefinedForErrorField()
    {
        DynamicJSONTestClass.GetTestValue(out _, out var json);

        Assert.IsTrue(json.notexistfield == null);
        Assert.IsTrue(json.notexistfield == JSON.Undefined);
        Assert.IsTrue(JSON.isUndefined(json.notexistfield));
    }

    [TestMethod]
    public void ShouldSuccessForCheckOperation()
    {
        DynamicJSONTestClass.GetTestValue(out _, out var json);

        if (json.notexistfield)
        {
            Assert.Fail();
        }

        if (!json.notexistfield)
        {
        }
        else
        {
            Assert.Fail();
        }
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
    }
}
