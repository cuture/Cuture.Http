using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Cuture.Http.Test.Server;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cuture.Http.Test
{
    /// <summary>
    /// 自定义Json序列化器测试
    /// </summary>
    [TestClass]
    public class CustomJsonSerializerTest : WebServerHostTestBase
    {
        #region 方法

        public IHttpRequest GetRequest() => $"{TestWebHost.TestHost}/api/user/update".CreateHttpRequest().UsePost();

        [TestMethod]
        public async Task ParallelRequestTestAsync()
        {
            var jsonSerializer = new CustomJsonSerializer();

            var user = new UserInfo()
            {
                Age = 10,
                Name = "TestUser中文😂😂😂",
                SpecialName = "~!@#$%^&*(TestUser中文😂😂😂"
            };

            await ParallelRequestAsync(10_000,
                                       () => GetRequest().UseJsonSerializer(jsonSerializer).WithJsonContent(user).TryGetAsObjectAsync<UserInfo>(),
                                       result => Assert.IsTrue(user.Equals(result.Data)));
        }

        /// <summary>
        /// 特殊操作序列化器测试
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task SpecialActionTestAsync()
        {
            var jsonSerializer = new SpecialActionJsonSerializer();

            var user = new UserInfo()
            {
                Age = 10,
                Name = "TestUser中文😂😂😂",
                SpecialName = "~!@#$%^&*(TestUser中文😂😂😂"
            };

            await ParallelRequestAsync(10_000,
                                       () => GetCallbackRequest().UseJsonSerializer(jsonSerializer).WithJsonContent(user).TryGetAsObjectAsync<UserInfo>(),
                                       result => Assert.IsTrue(user.Equals(result.Data)));

            static IHttpRequest GetCallbackRequest()
            {
                return $"{TestWebHost.TestHost}/api/user/update/callback".CreateHttpRequest().UsePost();
            }
        }

        #endregion 方法

        #region Public 类

        public class UserInfo : IEquatable<UserInfo>
        {
            #region 属性

            [JsonPropertyName("age")]
            public int Age { get; set; }

            [JsonPropertyName("field1")]
            public string Field1 { get; set; } = NewGuid();

            [JsonPropertyName("field2")]
            public string Field2 { get; set; } = NewGuid();

            [JsonPropertyName("field3")]
            public string Field3 { get; set; } = NewGuid();

            [JsonPropertyName("field4")]
            public string Field4 { get; set; } = NewGuid();

            [JsonPropertyName("field5")]
            public string Field5 { get; set; } = NewGuid();

            [JsonPropertyName("field6")]
            public string Field6 { get; set; } = NewGuid();

            [JsonPropertyName("field7")]
            public string Field7 { get; set; } = NewGuid();

            [JsonPropertyName("field8")]
            public string Field8 { get; set; } = NewGuid();

            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("Sp_NM")]
            public string SpecialName { get; set; }

            #endregion 属性

            #region 方法

            public bool Equals([AllowNull] UserInfo other)
            {
                return other != null &&
                    Age == other.Age &&
                    Field1 == other.Field1 &&
                    Field2 == other.Field2 &&
                    Field3 == other.Field3 &&
                    Field4 == other.Field4 &&
                    Field5 == other.Field5 &&
                    Field6 == other.Field6 &&
                    Field7 == other.Field7 &&
                    Field8 == other.Field8 &&
                    SpecialName == other.SpecialName &&
                    Name == other.Name;
            }

            private static string NewGuid() => Guid.NewGuid().ToString("N");

            #endregion 方法
        }

        #endregion Public 类

        #region Internal 类

        internal class CustomJsonSerializer : IJsonSerializer
        {
            #region Public 方法

            public T Deserialize<T>(string data)
            {
                return JsonSerializer.Deserialize<T>(data);
            }

            public string Serialize(object value)
            {
                return JsonSerializer.Serialize(value);
            }

            #endregion Public 方法
        }

        internal class SpecialActionJsonSerializer : IJsonSerializer
        {
            #region Public 方法

            public T Deserialize<T>(string data)
            {
                var json = Regex.Match(data, "{.+}").Value;
                return JsonSerializer.Deserialize<T>(json);
            }

            public string Serialize(object value)
            {
                return JsonSerializer.Serialize(value);
            }

            #endregion Public 方法
        }

        #endregion Internal 类
    }
}