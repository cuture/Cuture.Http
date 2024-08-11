using Cuture.Http.Util;

namespace Cuture.Http.Test.Util;

[TestClass]
public class CookieUtilityTest
{
    #region Public 方法

    [TestMethod]
    public void AnalysisCookieString()
    {
        Assert.AreEqual(0, CookieUtility.AnalysisCookieString(null).Count);
        Assert.AreEqual(0, CookieUtility.AnalysisCookieString(string.Empty).Count);

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

    [TestMethod]
    public void Clean()
    {
        Assert.AreEqual(string.Empty, CookieUtility.Clean(null));
        Assert.AreEqual(string.Empty, CookieUtility.Clean(string.Empty));

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

        var newCookieText = CookieUtility.Clean(cookieText);

        CheckCookies(kvs, newCookieText);
    }

    [TestMethod]
    public void Merge()
    {
        Assert.AreEqual(string.Empty, CookieUtility.Merge(string.Empty, string.Empty));
        Assert.AreEqual(string.Empty, CookieUtility.Merge(string.Empty, null));
        Assert.AreEqual(string.Empty, CookieUtility.Merge(null, string.Empty));
        Assert.AreEqual(string.Empty, CookieUtility.Merge(null, null));

        var cookieText1 = "BD_NOT_HTTPS=1; path=/; Max-Age=300; BIDUPSID=A6CA0EC8A85503305FD031A7F061057F; expires=Thu, 31-Dec-37 23:55:55 GMT; max-age=2147483647; path=/; domain=.baidu.com; PSTM=1649067919; expires=Thu, 31-Dec-37 23:55:55 GMT; max-age=2147483647; path=/; domain=.baidu.com; BAIDUID=A6CA0EC8A85503306B384002260FDAE3:FG=1; max-age=31536000; expires=Tue, 04-Apr-23 10:25:19 GMT; domain=.baidu.com; path=/; version=1; comment=bd";

        var cookieText2 = "cookie2=21dc2a0f2eaf56b8221fb3646c237f67;Path=/;Domain=.taobao.com;Max-Age=-1;HttpOnly; _m_h5_tk=b8ef287e7323a9c3421ee260dd6a1423_1649138215682;Path=/;Domain=taobao.com;Max-Age=604800; _m_h5_tk_enc=45f95bffc92b4f3bc65e793b9a046f58;Path=/;Domain=taobao.com;Max-Age=604800";

        var kvs = new Dictionary<string, string>()
        {
            { "BD_NOT_HTTPS", "1" },
            { "BIDUPSID", "A6CA0EC8A85503305FD031A7F061057F" },
            { "PSTM", "1649067919" },
            { "BAIDUID", "A6CA0EC8A85503306B384002260FDAE3:FG=1" },
            { "version", "1" },
            { "comment", "bd" },
            { "cookie2", "21dc2a0f2eaf56b8221fb3646c237f67" },
            { "_m_h5_tk", "b8ef287e7323a9c3421ee260dd6a1423_1649138215682" },
            { "_m_h5_tk_enc", "45f95bffc92b4f3bc65e793b9a046f58" },
        };

        var newCookieText = CookieUtility.Merge(cookieText1, cookieText2);

        CheckCookies(kvs, newCookieText);
    }

    #endregion Public 方法

    private static void CheckCookies(Dictionary<string, string> kvs, string cookieText)
    {
        var cookieKVs = cookieText.Split(';', StringSplitOptions.RemoveEmptyEntries)
                                  .Select(m =>
                                  {
                                      var index = m.IndexOf('=');
                                      Assert.IsTrue(index > 0);
                                      return new[] {
                                        m[0..index],
                                        m[(index+1)..],
                                    };
                                  });

        foreach (var kv in cookieKVs)
        {
            Assert.AreEqual(2, kv.Length);

            var key = kv[0].Trim();
            var value = kv[1].Trim();

            Assert.IsTrue(kvs.TryGetValue(key, out var actualValue), $"{key} not found.");
            Assert.AreEqual(actualValue, value);

            kvs.Remove(key);
        }
        Assert.AreEqual(0, kvs.Count);
    }
}
