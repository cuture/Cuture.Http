#if NET

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cuture.Http.Test.Util
{
    [TestClass]
    public class RequestBuildToolTest
    {
        const string PostRawBase64String = "UE9TVCBodHRwczovL211c2ljLm1pZ3UuY24vdjMvYXBpL2NvbW1vbi91c2VyQWN0aW9uUmVwb3J0IEhUVFAvMS4xDQpIb3N0OiBtdXNpYy5taWd1LmNuDQpVc2VyLUFnZW50OiBNb3ppbGxhLzUuMCAoV2luZG93cyBOVCAxMC4wOyBXaW42NDsgeDY0OyBydjo4NC4wKSBHZWNrby8yMDEwMDEwMSBGaXJlZm94Lzg0LjANCkFjY2VwdDogYXBwbGljYXRpb24vanNvbiwgdGV4dC9wbGFpbiwgKi8qDQpBY2NlcHQtTGFuZ3VhZ2U6IHpoLUNOLHpoO3E9MC44LHpoLVRXO3E9MC43LHpoLUhLO3E9MC41LGVuLVVTO3E9MC4zLGVuO3E9MC4yDQpBY2NlcHQtRW5jb2Rpbmc6IGd6aXAsIGRlZmxhdGUsIGJyDQpDb250ZW50LVR5cGU6IGFwcGxpY2F0aW9uL2pzb247Y2hhcnNldD11dGYtOA0KQ29udGVudC1MZW5ndGg6IDc1DQpPcmlnaW46IGh0dHBzOi8vbXVzaWMubWlndS5jbg0KRE5UOiAxDQpDb25uZWN0aW9uOiBrZWVwLWFsaXZlDQpSZWZlcmVyOiBodHRwczovL211c2ljLm1pZ3UuY24vdjMvbXVzaWMvcGxheWVyL2F1ZGlvP2Zyb209bWlndQ0KQ29va2llOiBtZ191ZW1fdXNlcl9pZF85ZmJlNjU5OTQwMGU0M2E0YTU4NzAwYTgyMmZkNTdmOD03MjNiNmRhMC0zNTBmLTQ0MjQtODY4Yy00ODdkNmY1MGJmNjk7IGF1ZGlvcGxheWVyX29wZW49MTsgbWlndV9jb29raWVfaWQ9YWYyMDdhZDctMDY5OS00MjFiLTgxNjctNjgxN2M1M2JkZGE4LW40MTYwODI4NzI3NDg5OTsgYXVkaW9wbGF5ZXJfZXhpc3Q9MTsgV1RfRlBDPWlkPTI5NTk0N2EyZTdhYjNjYjBlNmIxNjA4Mjg3Mjc1NzgwOmx2PTE2MDgyODczNzA1MTA6c3M9MTYwODI4NzI3NTc4MDsgaWRtcGF1dGg9dHJ1ZUBwYXNzcG9ydC5taWd1LmNuOyBwbGF5ZXJfc3RvcF9vcGVuPTA7IHBsYXlsaXN0X2FkZGluZz0xOyBhZGRwbGF5bGlzdF9oYXM9MTsgcGxheWxpc3RfY2hhbmdlPTA7IGFkZF9wbGF5X25vdz0wDQpTZWMtR1BDOiAxDQoNCnsiYWN0aW9uQ29uZklkIjo0LCJhY3Rpb25PYmplY3RJZCI6IjM4NTc2NTIiLCJjb3B5cmlnaHRJZDExIjoiNjM3OTM3MDEzMzUifQ==";

        static ReadOnlySpan<byte> PostRawData => Convert.FromBase64String(PostRawBase64String).AsSpan();

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
            Assert.AreEqual("https://music.migu.cn/v3/api/common/userActionReport", url);
            Assert.AreEqual("HTTP/1.1", version);
        }

        [TestMethod]
        public async Task BuildHasContentRequestAsync()
        {
            var request = RequestBuildTool.FromRaw(PostRawBase64String);
            var httpRst = await request.TryGetAsStringAsync();
            Assert.IsTrue(httpRst.IsSuccessStatusCode);
        }

        [TestMethod]
        public async Task BuildGetRequestAsync()
        {
            var rawBase64Str = "R0VUIGh0dHA6Ly9kZXRlY3Rwb3J0YWwuZmlyZWZveC5jb20vc3VjY2Vzcy50eHQgSFRUUC8xLjENCkhvc3Q6IGRldGVjdHBvcnRhbC5maXJlZm94LmNvbQ0KVXNlci1BZ2VudDogTW96aWxsYS81LjAgKFdpbmRvd3MgTlQgMTAuMDsgV2luNjQ7IHg2NDsgcnY6ODQuMCkgR2Vja28vMjAxMDAxMDEgRmlyZWZveC84NC4wDQpBY2NlcHQ6ICovKg0KQWNjZXB0LUxhbmd1YWdlOiB6aC1DTix6aDtxPTAuOCx6aC1UVztxPTAuNyx6aC1ISztxPTAuNSxlbi1VUztxPTAuMyxlbjtxPTAuMg0KQWNjZXB0LUVuY29kaW5nOiBnemlwLCBkZWZsYXRlDQpDYWNoZS1Db250cm9sOiBuby1jYWNoZQ0KUHJhZ21hOiBuby1jYWNoZQ0KRE5UOiAxDQpDb25uZWN0aW9uOiBrZWVwLWFsaXZlDQoNCg==";
            var request = RequestBuildTool.FromRaw(rawBase64Str);
            var httpRst = await request.TryGetAsStringAsync();
            Assert.IsTrue(httpRst.IsSuccessStatusCode);
        }
    }
}

#endif