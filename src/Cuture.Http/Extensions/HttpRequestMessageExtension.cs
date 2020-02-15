using System.Net.Http;
using System.Runtime.CompilerServices;

namespace Cuture.Http
{
    /// <summary>
    /// HttpRequestMessage拓展方法
    /// </summary>
    public static class HttpRequestMessageExtension
    {
        #region HttpRequestMessage

        /// <summary>
        /// 获取请求头的 Cookie 字符串内容
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetCookie(this HttpRequestMessage requestMessage) => requestMessage.Headers.TryGetValues(HttpHeaders.Cookie, out var cookies) ? string.Join("; ", cookies) : string.Empty;

        /// <summary>
        /// 获取请求头的 Cookie 字符串内容
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetCookie(this HttpRequestMessage requestMessage, out string cookie)
        {
            if (requestMessage.Headers.TryGetValues(HttpHeaders.Cookie, out var cookies))
            {
                cookie = string.Join("; ", cookies);
                return true;
            }
            else
            {
                cookie = string.Empty;
                return false;
            }
        }

        #endregion HttpRequestMessage
    }
}