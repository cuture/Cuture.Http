#if NET

using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cuture.Http.Test
{
    [TestClass]
    public class LoadFromRawTest
    {
        const string RawBase64String = "UE9TVCBodHRwczovL211c2ljLm1pZ3UuY24vdjMvYXBpL2NvbW1vbi91c2VyQWN0aW9uUmVwb3J0IEhUVFAvMS4xDQpIb3N0OiBtdXNpYy5taWd1LmNuDQpVc2VyLUFnZW50OiBNb3ppbGxhLzUuMCAoV2luZG93cyBOVCAxMC4wOyBXaW42NDsgeDY0OyBydjo4NC4wKSBHZWNrby8yMDEwMDEwMSBGaXJlZm94Lzg0LjANCkFjY2VwdDogYXBwbGljYXRpb24vanNvbiwgdGV4dC9wbGFpbiwgKi8qDQpBY2NlcHQtTGFuZ3VhZ2U6IHpoLUNOLHpoO3E9MC44LHpoLVRXO3E9MC43LHpoLUhLO3E9MC41LGVuLVVTO3E9MC4zLGVuO3E9MC4yDQpBY2NlcHQtRW5jb2Rpbmc6IGd6aXAsIGRlZmxhdGUsIGJyDQpDb250ZW50LVR5cGU6IGFwcGxpY2F0aW9uL2pzb247Y2hhcnNldD11dGYtOA0KQ29udGVudC1MZW5ndGg6IDc1DQpPcmlnaW46IGh0dHBzOi8vbXVzaWMubWlndS5jbg0KRE5UOiAxDQpDb25uZWN0aW9uOiBrZWVwLWFsaXZlDQpSZWZlcmVyOiBodHRwczovL211c2ljLm1pZ3UuY24vdjMvbXVzaWMvcGxheWVyL2F1ZGlvP2Zyb209bWlndQ0KQ29va2llOiBtZ191ZW1fdXNlcl9pZF85ZmJlNjU5OTQwMGU0M2E0YTU4NzAwYTgyMmZkNTdmOD03MjNiNmRhMC0zNTBmLTQ0MjQtODY4Yy00ODdkNmY1MGJmNjk7IGF1ZGlvcGxheWVyX29wZW49MTsgbWlndV9jb29raWVfaWQ9YWYyMDdhZDctMDY5OS00MjFiLTgxNjctNjgxN2M1M2JkZGE4LW40MTYwODI4NzI3NDg5OTsgYXVkaW9wbGF5ZXJfZXhpc3Q9MTsgV1RfRlBDPWlkPTI5NTk0N2EyZTdhYjNjYjBlNmIxNjA4Mjg3Mjc1NzgwOmx2PTE2MDgyODczNzA1MTA6c3M9MTYwODI4NzI3NTc4MDsgaWRtcGF1dGg9dHJ1ZUBwYXNzcG9ydC5taWd1LmNuOyBwbGF5ZXJfc3RvcF9vcGVuPTA7IHBsYXlsaXN0X2FkZGluZz0xOyBhZGRwbGF5bGlzdF9oYXM9MTsgcGxheWxpc3RfY2hhbmdlPTA7IGFkZF9wbGF5X25vdz0wDQpTZWMtR1BDOiAxDQoNCnsiYWN0aW9uQ29uZklkIjo0LCJhY3Rpb25PYmplY3RJZCI6IjM4NTc2NTIiLCJjb3B5cmlnaHRJZDExIjoiNjM3OTM3MDEzMzUifQ==";

        const string RawUrl = "https://music.migu.cn/v3/api/common/userActionReport";

        const string RawContentString = "{\"actionConfId\":4,\"actionObjectId\":\"3857652\",\"copyrightId11\":\"63793701335\"}";

        [TestMethod]
        public async Task LoadHeadersFromRaw()
        {
            var httpRst = await RawUrl.ToHttpRequest()
                                      .UsePost()
                                      .TryGetAsStringAsync();
            Assert.IsFalse(httpRst.IsSuccessStatusCode);

            httpRst = await RawUrl.ToHttpRequest()
                                  .UsePost()
                                  .LoadHeadersFromRaw(RawBase64String)
                                  .WithJsonContent(RawContentString)
                                  .TryGetAsStringAsync();

            Assert.IsTrue(httpRst.IsSuccessStatusCode);
        }

        [TestMethod]
        public async Task LoadContentFromRaw()
        {
            var httpRst = await RawUrl.ToHttpRequest()
                                      .UsePost()
                                      .LoadHeadersFromRaw(RawBase64String)
                                      .LoadContentFromRaw(RawBase64String)
                                      .TryGetAsStringAsync();

            Assert.IsTrue(httpRst.IsSuccessStatusCode);
        }

        [TestMethod]
        public async Task LoadHeadersAndContentFromRaw()
        {
            var httpRst = await RawUrl.ToHttpRequest()
                                      .UsePost()
                                      .LoadHeadersAndContentFromRaw(RawBase64String)
                                      .TryGetAsStringAsync();

            Assert.IsTrue(httpRst.IsSuccessStatusCode);
        }
    }
}

#endif