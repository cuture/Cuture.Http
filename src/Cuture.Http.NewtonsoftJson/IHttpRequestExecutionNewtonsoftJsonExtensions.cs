using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

namespace Cuture.Http
{
    /// <summary>
    /// <see cref="IHttpRequest"/> 执行请求相关拓展方法
    /// </summary>
    public static class IHttpRequestExecutionNewtonsoftJsonExtensions
    {
        #region 执行请求

        #region Result

        #region json as JsonObject

        /// <summary>
        /// 执行请求并以 json 接收返回数据，并解析为 <see cref="JObject"/> 对象
        /// </summary>
        /// <param name="request"></param>
        /// <param name="jsonLoadSetting"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<JObject?> GetAsJsonAsync(this IHttpRequest request, JsonLoadSettings? jsonLoadSetting = null) => request.ExecuteAsync().ReceiveAsJsonAsync(jsonLoadSetting);

        /// <summary>
        /// 执行请求并尝试以 json 接收返回数据，并解析为 <see cref="JObject"/> 对象
        /// </summary>
        /// <param name="request"></param>
        /// <param name="jsonLoadSetting"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<TextHttpOperationResult<JObject>> TryGetAsJsonAsync(this IHttpRequest request, JsonLoadSettings? jsonLoadSetting = null) => request.ExecuteAsync().TryReceiveAsJsonAsync(jsonLoadSetting);

        #endregion json as JsonObject

        #endregion Result

        #endregion 执行请求
    }
}