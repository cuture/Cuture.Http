using System;
using System.Text;

namespace Cuture.Http.Util
{
    /// <summary>
    /// 基础认证工具
    /// </summary>
    public static class BasicAuthUtil
    {
        #region Public 字段

        /// <summary>
        /// <see cref="HttpHeaders.Authorization"/>
        /// </summary>
        public const string HttpHeader = HttpHeaders.Authorization;

        #endregion Public 字段

        #region Public 方法

        /// <summary>
        /// base64编码用户名密码
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string Encode(string userName, string password, Encoding encoding = null) => $"{userName}:{password}".EncodeBase64(encoding ?? Encoding.UTF8);

        /// <summary>
        /// 编码为HttpHeader的值（附加Basic到字符串头部）
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string EncodeToHeader(string userName, string password, Encoding encoding = null) => $"Basic {Encode(userName, password, encoding)}";

        /// <summary>
        /// 尝试解码BasicAuth的值
        /// </summary>
        /// <param name="value"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static bool TryDecode(string value, out string userName, out string password, Encoding encoding = null)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                userName = null;
                password = null;
                return false;
            }

            var splitIndex =
#if NET5_0
                value.IndexOf(' ', StringComparison.Ordinal);
#else
                value.IndexOf(' ');
#endif
            if (splitIndex > 0) //移除头部的 Basic
            {
                value = value.Substring(splitIndex + 1, value.Length - splitIndex - 1);
            }

            try
            {
                var bytes = Convert.FromBase64String(value);
                splitIndex = -1;
                for (int i = 0; i < bytes.Length; i++)
                {
                    if (bytes[i] == ':')
                    {
                        splitIndex = i;
                        break;
                    }
                }
                if (splitIndex > 0)
                {
                    encoding ??= Encoding.UTF8;
                    userName = encoding.GetString(bytes, 0, splitIndex);
                    password = encoding.GetString(bytes, splitIndex + 1, bytes.Length - splitIndex - 1);
                    return true;
                }
            }
#pragma warning disable CA1031 // 不捕获常规异常类型
            catch { }
#pragma warning restore CA1031 // 不捕获常规异常类型
            userName = null;
            password = null;
            return false;
        }

        #endregion Public 方法
    }
}