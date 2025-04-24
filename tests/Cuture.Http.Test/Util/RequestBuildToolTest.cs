using System.Text;

namespace Cuture.Http.Test.Util;

[TestClass]
public class RequestBuildToolTest
{
    #region Private 属性

    private static ReadOnlySpan<byte> PostRawData => Convert.FromBase64String(LoadFromRawTest.RawBase64String).AsSpan();

    #endregion Private 属性

    #region Public 方法

    [TestMethod]
    public async Task BuildGetRequestAsync()
    {
        var rawBase64Str = "R0VUIGh0dHBzOi8vZGV0ZWN0cG9ydGFsLmZpcmVmb3guY29tL3N1Y2Nlc3MudHh0IEhUVFAvMS4xDQpIb3N0OiBkZXRlY3Rwb3J0YWwuZmlyZWZveC5jb20NClVzZXItQWdlbnQ6IE1vemlsbGEvNS4wIChXaW5kb3dzIE5UIDEwLjA7IFdpbjY0OyB4NjQ7IHJ2OjEwOS4wKSBHZWNrby8yMDEwMDEwMSBGaXJlZm94LzExMy4wDQpBY2NlcHQ6IHRleHQvaHRtbCxhcHBsaWNhdGlvbi94aHRtbCt4bWwsYXBwbGljYXRpb24veG1sO3E9MC45LGltYWdlL2F2aWYsaW1hZ2Uvd2VicCwqLyo7cT0wLjgNCkFjY2VwdC1MYW5ndWFnZTogemgtQ04semg7cT0wLjgsemgtVFc7cT0wLjcsemgtSEs7cT0wLjUsZW4tVVM7cT0wLjMsZW47cT0wLjINCkFjY2VwdC1FbmNvZGluZzogZ3ppcCwgZGVmbGF0ZSwgYnINCkROVDogMQ0KQ29ubmVjdGlvbjoga2VlcC1hbGl2ZQ0KVXBncmFkZS1JbnNlY3VyZS1SZXF1ZXN0czogMQ0KU2VjLUZldGNoLURlc3Q6IGRvY3VtZW50DQpTZWMtRmV0Y2gtTW9kZTogbmF2aWdhdGUNClNlYy1GZXRjaC1TaXRlOiBub25lDQpTZWMtRmV0Y2gtVXNlcjogPzENClNlYy1HUEM6IDENClByYWdtYTogbm8tY2FjaGUNCkNhY2hlLUNvbnRyb2w6IG5vLWNhY2hlDQoNCg==";
        var request = RequestBuildTool.FromRawBase64(rawBase64Str);
        var httpRst = await request.TryGetAsStringAsync();
        Console.WriteLine(httpRst.Data);
        Assert.IsTrue(httpRst.IsSuccessStatusCode, httpRst.Exception?.ToString());
    }

    [TestMethod]
    public void BuildHasContentRequestAsync()
    {
        var isSuccessStatusCode = RunAsync().GetAwaiter().GetResult();

        Assert.IsTrue(isSuccessStatusCode);

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.WaitForFullGCComplete();

        static async Task<bool> RunAsync()
        {
            var request = RequestBuildTool.FromRawBase64(LoadFromRawTest.RawBase64String);
            var httpRst = await request.TryGetAsStringAsync();

            return httpRst.IsSuccessStatusCode;
        }
    }

    [TestInitialize]
    public void Init()
    {
        HttpRequestGlobalOptions.DefaultHttpMessageInvokerPool = new SimpleHttpMessageInvokerPool();
    }

    [TestMethod]
    public void ReadHttpRequestLine()
    {
        var data = PostRawData;
        var encoding = Encoding.UTF8;
        RequestBuildTool.ReadHttpRequestLine(ref data, out var methodSpan, out var urlSpan, out var versionSpan);
        var method = encoding.GetString(methodSpan);
        var url = encoding.GetString(urlSpan);
        var version = encoding.GetString(versionSpan);
        Console.WriteLine($"method:{method} , url:{url} , version:{version}");

        Assert.AreEqual("POST", method);
        Assert.AreEqual("https://edge.microsoft.com/componentupdater/api/v1/update", url);
        Assert.AreEqual("HTTP/1.1", version);
    }

    #endregion Public 方法
}
