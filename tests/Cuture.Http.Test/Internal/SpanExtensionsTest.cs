using System;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cuture.Http.Test.Internal
{
    [TestClass]
    public class SpanExtensionsTest
    {
        #region Public 方法

        [TestMethod]
        [DataRow("0123456789", "456", "0123", "789")]
        [DataRow("0123456789", "012", "", "3456789")]
        [DataRow("0123456789", "789", "0123456", "")]
        [DataRow("0123456789", "987", "0123456789", "0123456789")]
        [DataRow("01234567890123456789", "456", "01234567890123", "789")]
        public void TruncationElementsEndTest(string source, string indentifier, string targetFront, string targetBack)
        {
            var sourceSpan = source.AsSpan();
            var @default = new string(sourceSpan.TruncationEnd(indentifier));
            Assert.AreEqual(targetBack, @default);
            var back = new string(sourceSpan.TruncationEnd(indentifier, true));
            Assert.AreEqual(targetBack, back);
            var front = new string(sourceSpan.TruncationEnd(indentifier, false));
            Assert.AreEqual(targetFront, front);
        }

        [TestMethod]
        [DataRow("0123456789", "456", "0123", "789")]
        [DataRow("0123456789", "012", "", "3456789")]
        [DataRow("0123456789", "789", "0123456", "")]
        [DataRow("0123456789", "987", "0123456789", "0123456789")]
        [DataRow("01234567890123456789", "456", "0123", "7890123456789")]
        public void TruncationElementsStartTest(string source, string indentifier, string targetFront, string targetBack)
        {
            var sourceSpan = source.AsSpan();
            var @default = new string(sourceSpan.TruncationStart(indentifier));
            Assert.AreEqual(targetBack, @default);
            var back = new string(sourceSpan.TruncationStart(indentifier, true));
            Assert.AreEqual(targetBack, back);
            var front = new string(sourceSpan.TruncationStart(indentifier, false));
            Assert.AreEqual(targetFront, front);
        }

        [TestMethod]
        [DataRow("0123456789", '4', "0123", "56789")]
        [DataRow("0123456789", '0', "", "123456789")]
        [DataRow("0123456789", '9', "012345678", "")]
        [DataRow("0123456789", '-', "0123456789", "0123456789")]
        [DataRow("01234567890123456789", '4', "01234567890123", "56789")]
        public void TruncationOneElementEndTest(string source, char indentifier, string targetFront, string targetBack)
        {
            var sourceSpan = source.AsSpan();
            var @default = new string(sourceSpan.TruncationEnd(indentifier));
            Assert.AreEqual(targetBack, @default);
            var back = new string(sourceSpan.TruncationEnd(indentifier, true));
            Assert.AreEqual(targetBack, back);
            var front = new string(sourceSpan.TruncationEnd(indentifier, false));
            Assert.AreEqual(targetFront, front);
        }

        [TestMethod]
        [DataRow("0123456789", '4', "0123", "56789")]
        [DataRow("0123456789", '0', "", "123456789")]
        [DataRow("0123456789", '9', "012345678", "")]
        [DataRow("0123456789", '-', "0123456789", "0123456789")]
        [DataRow("01234567890123456789", '4', "0123", "567890123456789")]
        public void TruncationOneElementStartTest(string source, char indentifier, string targetFront, string targetBack)
        {
            var sourceSpan = source.AsSpan();
            var @default = new string(sourceSpan.TruncationStart(indentifier));
            Assert.AreEqual(targetBack, @default);
            var back = new string(sourceSpan.TruncationStart(indentifier, true));
            Assert.AreEqual(targetBack, back);
            var front = new string(sourceSpan.TruncationStart(indentifier, false));
            Assert.AreEqual(targetFront, front);
        }

        #endregion Public 方法
    }
}