using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Cuture.Http
{
    /// <summary>
    /// 伪造Header的拓展类
    /// </summary>
    public static class HeadersExtension
    {
        #region 字段

        private static readonly Random s_random = new Random();

        #endregion 字段

        #region 方法

        /// <summary>
        /// 获取一个随机的IP地址
        /// </summary>
        /// <returns></returns>
        public static string GetRandomIpAddress()
        {
            var sb = new StringBuilder();
            sb.Append(s_random.Next(11, 240));
            sb.Append('.');
            sb.Append(s_random.Next(1, 250));
            sb.Append('.');
            sb.Append(s_random.Next(1, 240));
            sb.Append('.');
            sb.Append(s_random.Next(1, 240));
            return sb.ToString();
        }

        /// <summary>
        /// 使用随机的X-Forwarded-For标头
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IHttpTurboRequest RandomXFowardFor(this IHttpTurboRequest request)
        {
            request.AddHeader("X-Forwarded-For", GetRandomIpAddress());
            return request;
        }

        #endregion 方法
    }
}