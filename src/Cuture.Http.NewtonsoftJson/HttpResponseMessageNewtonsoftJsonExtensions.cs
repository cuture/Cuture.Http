using System;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

namespace Cuture.Http
{
    /// <summary>
    /// HttpResponseMessage拓展方法
    /// </summary>
    public static class HttpResponseMessageNewtonsoftJsonExtensions
    {
        #region Task<HttpResponseMessage>

        #region json as JsonObject

        /// <summary>
        /// 以 json 接收返回数据，并解析为 <see cref="JObject"/> 对象
        /// </summary>
        /// <param name="requestTask"></param>
        /// <param name="jsonLoadSetting"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<JObject?> ReceiveAsJsonAsync(this Task<HttpResponseMessage> requestTask, JsonLoadSettings? jsonLoadSetting = null)
        {
            using var response = await requestTask.ConfigureAwait(false);
            return await response.ReceiveAsJsonAsync(jsonLoadSetting).ConfigureAwait(false);
        }

        /// <summary>
        /// 尝试以 json 接收返回数据，并解析为 <see cref="JObject"/> 对象
        /// </summary>
        /// <param name="requestTask"></param>
        /// <param name="jsonLoadSetting"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<TextHttpOperationResult<JObject>> TryReceiveAsJsonAsync(this Task<HttpResponseMessage> requestTask, JsonLoadSettings? jsonLoadSetting = null)
        {
            var result = new TextHttpOperationResult<JObject>();
            try
            {
                result.ResponseMessage = await requestTask.ConfigureAwait(false);

                var json = await result.ResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                result.Text = json;

                if (!string.IsNullOrEmpty(json))
                {
                    result.Data = JObject.Parse(json, jsonLoadSetting);
                }
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }
            return result;
        }

        #endregion json as JsonObject

        #endregion Task<HttpResponseMessage>

        #region HttpResponseMessage

        /// <summary>
        /// 获取请求返回的JObject对象
        /// </summary>
        /// <param name="responseMessage"></param>
        /// <param name="jsonLoadSetting"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<JObject?> ReceiveAsJsonAsync(this HttpResponseMessage responseMessage, JsonLoadSettings? jsonLoadSetting = null)
        {
            using (responseMessage)
            {
                var json = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (!string.IsNullOrEmpty(json))
                {
                    return JObject.Parse(json, jsonLoadSetting);
                }
                return null;
            }
        }

        #endregion HttpResponseMessage
    }
}