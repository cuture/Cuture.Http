using System.Collections.Generic;

using Cuture.Http.Util;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cuture.Http.Test.Util;

[TestClass]
public class CookieUtilityTest
{
    #region Public 方法

    [TestMethod]
    public void AnalysisCookieString()
    {
        var cookieText = "BD_NOT_HTTPS=1; path=/; Max-Age=300; BIDUPSID=A6CA0EC8A85503305FD031A7F061057F; expires=Thu, 31-Dec-37 23:55:55 GMT; max-age=2147483647; path=/; domain=.baidu.com; PSTM=1649067919; expires=Thu, 31-Dec-37 23:55:55 GMT; max-age=2147483647; path=/; domain=.baidu.com; BAIDUID=A6CA0EC8A85503306B384002260FDAE3:FG=1; max-age=31536000; expires=Tue, 04-Apr-23 10:25:19 GMT; domain=.baidu.com; path=/; version=1; comment=bd";

        var kvs = new Dictionary<string, string>()
        {
            { "BD_NOT_HTTPS", "1" },
            { "BIDUPSID", "A6CA0EC8A85503305FD031A7F061057F" },
            { "PSTM", "1649067919" },
            { "BAIDUID", "A6CA0EC8A85503306B384002260FDAE3:FG=1" },
            { "version", "1" },
            { "comment", "bd" },
        };

        var cookies = CookieUtility.AnalysisCookieString(cookieText);

        Assert.AreEqual(kvs.Count, cookies.Count);

        foreach (var kv in kvs)
        {
            Assert.IsTrue(cookies.TryGetValue(kv.Key, out var value));
            Assert.AreEqual(kv.Value, value);
        }
    }

    #endregion Public 方法
}