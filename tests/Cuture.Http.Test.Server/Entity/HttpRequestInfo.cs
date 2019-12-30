using System.Collections.Generic;

using Newtonsoft.Json;

namespace Cuture.Http.Test.Server.Entity
{
    public class HttpRequestInfo
    {
        #region 属性

        [JsonProperty("content")]
        public byte[] Content { get; set; }

        [JsonProperty("header")]
        public Dictionary<string, string> Header { get; set; }

        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        #endregion 属性
    }
}