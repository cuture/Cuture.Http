using System;
using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json;

namespace Cuture.Http.Test.Server.Entity
{
    public class UserInfo : IEquatable<UserInfo>
    {
        #region 属性

        [JsonProperty("age")]
        public int Age { get; set; }

        [JsonProperty("field1")]
        public string Field1 { get; set; } = NewGuid();

        [JsonProperty("field2")]
        public string Field2 { get; set; } = NewGuid();

        [JsonProperty("field3")]
        public string Field3 { get; set; } = NewGuid();

        [JsonProperty("field4")]
        public string Field4 { get; set; } = NewGuid();

        [JsonProperty("field5")]
        public string Field5 { get; set; } = NewGuid();

        [JsonProperty("field6")]
        public string Field6 { get; set; } = NewGuid();

        [JsonProperty("field7")]
        public string Field7 { get; set; } = NewGuid();

        [JsonProperty("field8")]
        public string Field8 { get; set; } = NewGuid();

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("Sp_NM")]
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
}
