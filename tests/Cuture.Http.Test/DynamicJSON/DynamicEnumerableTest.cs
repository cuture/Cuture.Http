using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

using Cuture.Http.DynamicJSON;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cuture.Http.Test.DynamicJSON;

[TestClass]
public class DynamicEnumerableTest
{
    #region Public 方法

    [TestMethod]
    public void ShouldCastFail()
    {
        DynamicJSONTestClass.GetTestValue(out var origin, out var json);

        Assert.ThrowsException<InvalidCastException>(() => (IEnumerable<dynamic>)json.MyProperty6);
        Assert.ThrowsException<InvalidCastException>(() => (IEnumerable<KeyValuePair<string, dynamic?>>)json.MyProperty6);
        Assert.ThrowsException<InvalidCastException>(() => (IDynamicKeyValueEnumerable)json.MyProperty6);

        Assert.ThrowsException<InvalidCastException>(() => (IEnumerable<dynamic>)json);
        Assert.ThrowsException<InvalidCastException>(() => (IEnumerable<KeyValuePair<string, dynamic?>>)json);
        Assert.ThrowsException<InvalidCastException>(() => (IDynamicEnumerable)json);
    }

    [TestMethod]
    public void ShouldCastSuccess()
    {
        DynamicJSONTestClass.GetTestValue(out var origin, out var json);

        #region IDynamicEnumerable

        {
            IDynamicEnumerable dynamicEnumerable = json.MyProperty6;
            Assert.IsNotNull(dynamicEnumerable);

            dynamicEnumerable = (IDynamicEnumerable)json.MyProperty6;
            Assert.IsNotNull(dynamicEnumerable);

            dynamicEnumerable = json.MyProperty6;
            Assert.IsNotNull(dynamicEnumerable);
        }

        #endregion IDynamicEnumerable

        #region IDynamicKeyValueEnumerable

        {
            IDynamicKeyValueEnumerable dynamicKeyValueEnumerable = json;
            Assert.IsNotNull(dynamicKeyValueEnumerable);

            dynamicKeyValueEnumerable = (IDynamicKeyValueEnumerable)json;
            Assert.IsNotNull(dynamicKeyValueEnumerable);

            dynamicKeyValueEnumerable = json;
            Assert.IsNotNull(dynamicKeyValueEnumerable);
        }

        #endregion IDynamicKeyValueEnumerable

        #region IEnumerable<dynamic>

        {
            IEnumerable<dynamic> enumerable = json.MyProperty6;
            Assert.IsNotNull(enumerable);

            enumerable = json;
            Assert.IsNotNull(enumerable);
        }

        #endregion IEnumerable<dynamic>

        #region IEnumerable<dynamic>

        {
            IEnumerable<KeyValuePair<string, dynamic?>> enumerable = json;
            Assert.IsNotNull(enumerable);
        }

        #endregion IEnumerable<dynamic>
    }

    [TestMethod]
    public void ShouldEqualLinqOnOrigin()
    {
        DynamicJSONTestClass.GetTestValue(out var origin, out var json);

        var enumerable = ((IDynamicEnumerable)json.MyProperty6).AsEnumerable();

        Assert.AreEqual(origin.MyProperty6.Length, enumerable.Count());
        Assert.AreEqual(origin.MyProperty6.First(), enumerable.First());
        Assert.AreEqual(origin.MyProperty6.Skip(1).First(), enumerable.Skip(1).First());

        var filtered = origin.MyProperty6.Where(m => m[0] > '1').ToArray();
        var dynamicFiltered = enumerable.Where(m => m[0] > '1').ToArray();

        Assert.AreEqual(filtered.Length, dynamicFiltered.Length);

        for (int i = 0; i < filtered.Length; i++)
        {
            Assert.AreEqual(filtered[i], dynamicFiltered[i]);
        }
    }

    [TestMethod]
    public void ShouldEqualOrigin()
    {
        DynamicJSONTestClass.GetTestValue(out var origin, out var json);

        var enumerable = ((IDynamicEnumerable)json.MyProperty6).AsEnumerable();

        var dynamicToArray = enumerable.ToArray();

        Assert.AreEqual(origin.MyProperty6.Length, dynamicToArray.Length);

        for (int i = 0; i < origin.MyProperty6.Length; i++)
        {
            Assert.AreEqual(origin.MyProperty6[i], dynamicToArray[i]);
        }
    }

    [TestMethod]
    public void ShouldEqualOriginProperty()
    {
        DynamicJSONTestClass.GetTestValue(out var origin, out var json);

        //IDynamicKeyValueEnumerable
        {
            var enumerable = ((IDynamicKeyValueEnumerable)json).AsEnumerable();

            foreach (var (key, value) in enumerable)
            {
                Check(origin, key, value);
            }
        }

        //IEnumerable<dynamic>
        {
            IEnumerable<dynamic> dynamicEnumerable = json;

            foreach (var item in dynamicEnumerable)
            {
                string key = item.Key;
                Assert.IsNotNull(key);
                dynamic value = item.Value;

                Check(origin, key, value);
            }
        }

        static void Check(object origin, string key, dynamic value)
        {
            var type = origin.GetType();
            var originProperty = type.GetProperty(key);
            Assert.IsNotNull(originProperty);

            var originValue = originProperty.GetValue(origin);

            //直接比较有点麻烦。。直接转json字符串比较
            Assert.AreEqual(JsonSerializer.Serialize(originValue), JSON.stringify(value));
        }
    }

    #endregion Public 方法
}
