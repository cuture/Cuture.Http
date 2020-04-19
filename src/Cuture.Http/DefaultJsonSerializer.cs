using Newtonsoft.Json;

namespace Cuture.Http
{
    /// <summary>
    /// 默认Json序列化器
    /// </summary>
    internal class DefaultJsonSerializer : IJsonSerializer
    {
        #region Public 方法

        public T Deserialize<T>(string data) => JsonConvert.DeserializeObject<T>(data);

        public string Serialize(object value) => JsonConvert.SerializeObject(value);

        #endregion Public 方法
    }
}