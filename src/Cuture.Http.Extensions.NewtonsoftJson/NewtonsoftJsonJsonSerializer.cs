using Newtonsoft.Json;

namespace Cuture.Http
{
    /// <summary>
    /// 使用 Newtonsoft.Json 实现的 <inheritdoc cref="IJsonSerializer"/>
    /// </summary>
    public class NewtonsoftJsonJsonSerializer : IJsonSerializer
    {
        #region Public 方法

        /// <inheritdoc/>
        public T Deserialize<T>(string data) => JsonConvert.DeserializeObject<T>(data);

        /// <inheritdoc/>
        public string Serialize(object value) => JsonConvert.SerializeObject(value);

        #endregion Public 方法
    }
}