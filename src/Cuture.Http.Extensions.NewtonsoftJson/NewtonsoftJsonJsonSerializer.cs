using Newtonsoft.Json;

namespace Cuture.Http
{
    /// <summary>
    /// 使用 Newtonsoft.Json 实现的 <inheritdoc cref="IJsonSerializer"/>
    /// </summary>
    public class NewtonsoftJsonJsonSerializer : IJsonSerializer
    {
        #region Private 字段

        private readonly JsonSerializerSettings? _jsonSerializerSettings;

        #endregion Private 字段

        #region Public 构造函数

        /// <inheritdoc cref="NewtonsoftJsonJsonSerializer"/>
        public NewtonsoftJsonJsonSerializer(JsonSerializerSettings? jsonSerializerSettings = null)
        {
            _jsonSerializerSettings = jsonSerializerSettings;
        }

        #endregion Public 构造函数

        #region Public 方法

        /// <inheritdoc/>
        public T? Deserialize<T>(string data) => JsonConvert.DeserializeObject<T>(data, _jsonSerializerSettings);

        /// <inheritdoc/>
        public string Serialize(object value) => JsonConvert.SerializeObject(value, _jsonSerializerSettings!);

        #endregion Public 方法
    }
}