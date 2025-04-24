namespace Cuture.Http.Test;

[TestClass]
public class LoadFromRawTest
{
    public const string RawBase64String = "UE9TVCBodHRwczovL2VkZ2UubWljcm9zb2Z0LmNvbS9jb21wb25lbnR1cGRhdGVyL2FwaS92MS91cGRhdGUgSFRUUC8xLjENCkhvc3Q6IGVkZ2UubWljcm9zb2Z0LmNvbQ0KQ29ubmVjdGlvbjoga2VlcC1hbGl2ZQ0KQ29udGVudC1MZW5ndGg6IDE0NTYNClgtTWljcm9zb2Z0LVVwZGF0ZS1TZXJ2aWNlLUNvaG9ydDogNjI4NQ0KQ29udGVudC1UeXBlOiBhcHBsaWNhdGlvbi9qc29uDQpTZWMtTWVzaC1DbGllbnQtRWRnZS1WZXJzaW9uOiAxMzUuMC4zMTc5Ljg1DQpTZWMtTWVzaC1DbGllbnQtRWRnZS1DaGFubmVsOiBzdGFibGUNClNlYy1NZXNoLUNsaWVudC1PUzogV2luZG93cw0KU2VjLU1lc2gtQ2xpZW50LU9TLVZlcnNpb246IDEwLjAuMjYxMDANClNlYy1NZXNoLUNsaWVudC1BcmNoOiB4ODZfNjQNClNlYy1NZXNoLUNsaWVudC1XZWJWaWV3OiAwDQpYLUNsaWVudC1EYXRhOiBDSnZ4eWdFPQ0KU2VjLUZldGNoLVNpdGU6IG5vbmUNClNlYy1GZXRjaC1Nb2RlOiBuby1jb3JzDQpTZWMtRmV0Y2gtRGVzdDogZW1wdHkNClVzZXItQWdlbnQ6IE1vemlsbGEvNS4wIChXaW5kb3dzIE5UIDEwLjA7IFdpbjY0OyB4NjQpIEFwcGxlV2ViS2l0LzUzNy4zNiAoS0hUTUwsIGxpa2UgR2Vja28pIENocm9tZS8xMzUuMC4wLjAgU2FmYXJpLzUzNy4zNiBFZGcvMTM1LjAuMC4wDQpBY2NlcHQtRW5jb2Rpbmc6IGd6aXAsIGRlZmxhdGUsIGJyLCB6c3RkDQoNCnsicmVxdWVzdCI6eyJAb3MiOiJ3aW4iLCJAdXBkYXRlciI6Im1zZWRnZSIsImFjY2VwdGZvcm1hdCI6ImNyeDMiLCJhcHAiOlt7ImFwcGlkIjoiamNtY2VncGNlaGRjaGxqZWxkZ21tZmJnY3BubWdlZG8iLCJicmFuZCI6IklOQlgiLCJjb2hvcnQiOiJycmZAMC4wMSIsImVuYWJsZWQiOnRydWUsImV2ZW50IjpbeyJkb3dubG9hZF90aW1lX21zIjo0MDQwLCJkb3dubG9hZGVkIjoxNzYxODAsImRvd25sb2FkZXIiOiJiaXRzIiwiZXZlbnRyZXN1bHQiOjEsImV2ZW50dHlwZSI6MTQsIm5leHR2ZXJzaW9uIjoiMjAyNS40LjI0LjEiLCJwcmV2aW91c3ZlcnNpb24iOiIyMDI1LjQuNS4xIiwidG90YWwiOjE3NjE4MCwidXJsIjoiaHR0cDovL21zZWRnZS5iLnRsdS5kbC5kZWxpdmVyeS5tcC5taWNyb3NvZnQuY29tL2ZpbGVzdHJlYW1pbmdzZXJ2aWNlL2ZpbGVzL2ExMmZmNTFiLWM2MGUtNDI1ZC04NjFjLWUyZjVhZGQ3Y2M3Mj9QMT0xNzQ2MDg4ODI1JlAyPTQwNCZQMz0yJlA0PUtSVHF6UUxWbzhITkxUSEtjSndQRWJtJTJmVUhBYXprJTJidjFiWmJNaU1rYXF5R05aOGRsdm5RWUNxVkttY1lCcE55d05LJTJiMnVFaWJZTDVnU0JUOTN6SUhnJTNkJTNkIn0seyJldmVudHJlc3VsdCI6MSwiZXZlbnR0eXBlIjozLCJpbnN0YWxsX3RpbWVfbXMiOjM0LCJuZXh0ZnAiOiIxLjE3MTI5QzJBNkQ4N0ExQUQ0QTk3MUFDMzg5NENBNkI0NDIxRDA3MUEyQzcwQTY1NzUzRjVDQ0VBNjIxNEIzRjQiLCJuZXh0dmVyc2lvbiI6IjIwMjUuNC4yNC4xIiwicHJldmlvdXNmcCI6IjEuMjc1MzY4MjEwN0E3MkZENDUxQzI0NDQyRkIwNDhENUExMjRBRUREQzAzOUMzMjVFMjFFMTEzQzlFQTRBMjNEMiIsInByZXZpb3VzdmVyc2lvbiI6IjIwMjUuNC41LjEifV0sImluc3RhbGxkYXRlIjotMSwibGFuZyI6InpoLUNOIiwicGFja2FnZXMiOnsicGFja2FnZSI6W3siZnAiOiIxLjE3MTI5QzJBNkQ4N0ExQUQ0QTk3MUFDMzg5NENBNkI0NDIxRDA3MUEyQzcwQTY1NzUzRjVDQ0VBNjIxNEIzRjQifV19LCJ2ZXJzaW9uIjoiMjAyNS40LjI0LjEifV0sImFyY2giOiJ4NjQiLCJkZWR1cCI6ImNyIiwiZG9tYWluam9pbmVkIjpmYWxzZSwiaHciOnsiYXZ4IjoxLCJwaHlzbWVtb3J5Ijo0Nywic3NlIjoxLCJzc2UyIjoxLCJzc2UzIjoxLCJzc2U0MSI6MSwic3NlNDIiOjEsInNzc2UzIjoxfSwiaXNtYWNoaW5lIjoxLCJuYWNsX2FyY2giOiJ4ODYtNjQiLCJvcyI6eyJhcmNoIjoieDg2XzY0IiwicGxhdGZvcm0iOiJXaW5kb3dzIiwidmVyc2lvbiI6IjEwLjAuMjYxMDAuMzc3NSJ9LCJwcm9kdmVyc2lvbiI6IjEzNS4wLjMxNzkuODUiLCJwcm90b2NvbCI6IjMuMSIsInJlcXVlc3RpZCI6InszYmM4ZDA2Ni04NTQyLTQxYWUtOWI4NS1mNjUxODNlM2ZiZjZ9Iiwic2Vzc2lvbmlkIjoie2QzYjY0ZGJhLWY0Y2QtNGEyOC1iZjcxLThmMzZiNGUzNzFlZX0iLCJ1cGRhdGVydmVyc2lvbiI6IjEzNS4wLjMxNzkuODUifX0=";

    public const string RawUrl = "https://edge.microsoft.com/componentupdater/api/v1/update";

    public const string RawContentString = "{\"request\":{\"@os\":\"win\",\"@updater\":\"msedge\",\"acceptformat\":\"crx3\",\"app\":[{\"appid\":\"jcmcegpcehdchljeldgmmfbgcpnmgedo\",\"brand\":\"INBX\",\"cohort\":\"rrf@0.01\",\"enabled\":true,\"event\":[{\"download_time_ms\":4040,\"downloaded\":176180,\"downloader\":\"bits\",\"eventresult\":1,\"eventtype\":14,\"nextversion\":\"2025.4.24.1\",\"previousversion\":\"2025.4.5.1\",\"total\":176180,\"url\":\"http://msedge.b.tlu.dl.delivery.mp.microsoft.com/filestreamingservice/files/a12ff51b-c60e-425d-861c-e2f5add7cc72?P1=1746088825&P2=404&P3=2&P4=KRTqzQLVo8HNLTHKcJwPEbm%2fUHAazk%2bv1bZbMiMkaqyGNZ8dlvnQYCqVKmcYBpNywNK%2b2uEibYL5gSBT93zIHg%3d%3d\"},{\"eventresult\":1,\"eventtype\":3,\"install_time_ms\":34,\"nextfp\":\"1.17129C2A6D87A1AD4A971AC3894CA6B4421D071A2C70A65753F5CCEA6214B3F4\",\"nextversion\":\"2025.4.24.1\",\"previousfp\":\"1.2753682107A72FD451C24442FB048D5A124AEDDC039C325E21E113C9EA4A23D2\",\"previousversion\":\"2025.4.5.1\"}],\"installdate\":-1,\"lang\":\"zh-CN\",\"packages\":{\"package\":[{\"fp\":\"1.17129C2A6D87A1AD4A971AC3894CA6B4421D071A2C70A65753F5CCEA6214B3F4\"}]},\"version\":\"2025.4.24.1\"}],\"arch\":\"x64\",\"dedup\":\"cr\",\"domainjoined\":false,\"hw\":{\"avx\":1,\"physmemory\":47,\"sse\":1,\"sse2\":1,\"sse3\":1,\"sse41\":1,\"sse42\":1,\"ssse3\":1},\"ismachine\":1,\"nacl_arch\":\"x86-64\",\"os\":{\"arch\":\"x86_64\",\"platform\":\"Windows\",\"version\":\"10.0.26100.3775\"},\"prodversion\":\"135.0.3179.85\",\"protocol\":\"3.1\",\"requestid\":\"{3bc8d066-8542-41ae-9b85-f65183e3fbf6}\",\"sessionid\":\"{d3b64dba-f4cd-4a28-bf71-8f36b4e371ee}\",\"updaterversion\":\"135.0.3179.85\"}}";

    [TestInitialize]
    public void Init()
    {
        HttpRequestGlobalOptions.DefaultHttpMessageInvokerPool = new SimpleHttpMessageInvokerPool();
    }

    [TestMethod]
    public async Task LoadHeadersFromRaw()
    {
        var httpRst = await RawUrl.CreateHttpRequest()
                                  .UsePost()
                                  .TryGetAsStringAsync();
        Assert.IsFalse(httpRst.IsSuccessStatusCode);

        httpRst = await RawUrl.CreateHttpRequest()
                              .UsePost()
                              .LoadHeadersFromRawBase64(RawBase64String)
                              .WithJsonContent(RawContentString)
                              .TryGetAsStringAsync();

        Assert.IsTrue(httpRst.IsSuccessStatusCode);
    }

    [TestMethod]
    public async Task LoadContentFromRaw()
    {
        var httpRst = await RawUrl.CreateHttpRequest()
                                  .UsePost()
                                  .LoadHeadersFromRawBase64(RawBase64String)
                                  .LoadContentFromRawBase64(RawBase64String)
                                  .TryGetAsStringAsync();

        Assert.IsTrue(httpRst.IsSuccessStatusCode);
    }

    [TestMethod]
    public async Task LoadHeadersAndContentFromRaw()
    {
        var httpRst = await RawUrl.CreateHttpRequest()
                                  .UsePost()
                                  .LoadHeadersAndContentFromRawBase64(RawBase64String)
                                  .TryGetAsStringAsync();

        Assert.IsTrue(httpRst.IsSuccessStatusCode);
    }
}
