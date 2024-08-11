using System.Text;

using Cuture.Http.Util;

namespace Cuture.Http.Test.Util;

[TestClass]
public class BasicAuthUtilTest
{
    #region Public 方法

    [TestMethod]
    public void AsciiEncode()
    {
        var userName = "userName123456";
        var password = "password123456";

        var headerValue = BasicAuthUtil.EncodeToHeader(userName, password);
        DecodeTest(headerValue, userName, password);

        headerValue = BasicAuthUtil.EncodeToHeader(userName, password, Encoding.UTF8);
        DecodeTest(headerValue, userName, password);
    }

    [TestMethod]
    public void Utf8Encode()
    {
        var userName = "name🐱‍💻🐱‍🐉🐱‍👓🐱‍🚀✔👀😃✨";
        var password = "pwd😊😂🤣❤😍😒👌😘";

        var headerValue = BasicAuthUtil.EncodeToHeader(userName, password);
        DecodeTest(headerValue, userName, password);

        headerValue = BasicAuthUtil.EncodeToHeader(userName, password, Encoding.UTF8);
        DecodeTest(headerValue, userName, password);
    }

    #endregion Public 方法

    #region Private 方法

    private void DecodeTest(string value, string userName, string password)
    {
        if (BasicAuthUtil.TryDecode(value, out var decodeUserName, out var decodePassword))
        {
            Assert.AreEqual(userName, decodeUserName);
            Assert.AreEqual(password, decodePassword);
            return;
        }
        Assert.Fail();
    }

    #endregion Private 方法
}
