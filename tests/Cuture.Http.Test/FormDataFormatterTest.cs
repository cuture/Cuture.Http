using System;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cuture.Http.Test
{
    [TestClass]
    public abstract class FormDataFormatterTest
    {
        #region Private 字段

        private IFormDataFormatter _formatter;

        #endregion Private 字段

        #region Public 方法

        [TestInitialize]
        public void Init()
        {
            _formatter = InitFormatter();
        }

        [TestMethod]
        [DataRow(null, true)]
        [DataRow("", true)]
        [DataRow(null, false)]
        [DataRow("", false)]
        public void ShouldHasEmptyValue(string name, bool encode)
        {
            var obj = new TestClass() { Name = name };

            var form = encode
                        ? _formatter.Format(obj, new(true))
                        : _formatter.Format(obj);

            Console.WriteLine(form);

            var dic = form.Split('&')
                          .Select(m => m.Split('='))
                          .ToDictionary(m => m[0], m => m[1]);

            Assert.AreEqual(string.Empty, dic[nameof(TestClass.Name)]);
            Assert.AreEqual(string.Empty, dic[nameof(TestClass.Age)]);
        }

        protected abstract IFormDataFormatter InitFormatter();

        #endregion Public 方法

        #region Private 类

        private class TestClass
        {
            #region Public 属性

            public int? Age { get; set; }
            public string Name { get; set; }

            #endregion Public 属性
        }

        #endregion Private 类
    }

    [TestClass]
    public class NewtonsoftJsonFormDataFormatterTest : FormDataFormatterTest
    {
        #region Protected 方法

        protected override IFormDataFormatter InitFormatter()
        {
            return new NewtonsoftJsonFormDataFormatter();
        }

        #endregion Protected 方法
    }

#if NETCOREAPP3_1_OR_GREATER

    [TestClass]
    public class SystemJsonFormDataFormatterTest : FormDataFormatterTest
    {
        #region Protected 方法

        protected override IFormDataFormatter InitFormatter()
        {
            return new SystemJsonFormDataFormatter();
        }

        #endregion Protected 方法
    }

#endif
}