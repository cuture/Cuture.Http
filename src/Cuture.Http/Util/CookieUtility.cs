using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Cuture.Http.Util;

/// <summary>
/// Cookie工具类
/// </summary>
public static class CookieUtility
{
    #region 字段

    /// <summary>
    /// 需要忽略cookie的关键字集合
    /// </summary>
    private static readonly string[] s_ignoreCookieKey = new string[] { "Domain", "Expires", "HttpOnly", "Path", "Max-Age" };

    #endregion 字段

    #region 方法

    /// <summary>
    /// 分析指定cookie并返回键值对集合
    /// </summary>
    /// <param name="cookie"></param>
    /// <returns></returns>
    public static Dictionary<string, string> AnalysisCookieString(string cookie)
    {
        var cookieDic = new Dictionary<string, string>();

        if (!string.IsNullOrEmpty(cookie))
        {
            string[] array = cookie.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in array)
            {
                var str = item.Trim();
                var cookieKV = GetCookieKVFromString(str);
                if (!string.IsNullOrEmpty(cookieKV.Key) && !string.IsNullOrEmpty(cookieKV.Value))
                {
                    if (cookieDic.ContainsKey(cookieKV.Key))
                    {
                        cookieDic[cookieKV.Key] = cookieKV.Value;
                    }
                    else
                    {
                        cookieDic.Add(cookieKV.Key, cookieKV.Value);
                    }
                }
            }
        }

        return cookieDic;
    }

    /// <summary>
    /// 清理Cookie，使其成为KV格式
    /// </summary>
    /// <param name="cookie"></param>
    /// <returns></returns>
    public static string Clean(string cookie)
    {
        if (string.IsNullOrEmpty(cookie))
        {
            return string.Empty;
        }

        var cookieDic = AnalysisCookieString(cookie);

        var cookieBuilder = new StringBuilder(cookie.Length);

        if (cookieDic.Count > 0)
        {
            foreach (var item in cookieDic)
            {
                cookieBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0}={1}; ", item.Key, item.Value);
            }
            cookieBuilder.Remove(cookieBuilder.Length - 1, 1);
        }

        return cookieBuilder.ToString();
    }

    /// <summary>
    /// 合并Cookie
    /// </summary>
    /// <param name="srcCookie">源cookie</param>
    /// <param name="addonCookie">附加cookie</param>
    /// <returns></returns>
    public static string Merge(string srcCookie, string addonCookie)
    {
        if (srcCookie == null)
        {
            srcCookie = string.Empty;
        }
        if (string.IsNullOrEmpty(addonCookie))
        {
            return srcCookie;
        }

        var srcCookieDic = AnalysisCookieString(srcCookie);
        var addonCookieDic = AnalysisCookieString(addonCookie);

        foreach (var item in addonCookieDic)
        {
            if (srcCookieDic.ContainsKey(item.Key))
            {
                srcCookieDic[item.Key] = item.Value;
            }
            else
            {
                srcCookieDic.Add(item.Key, item.Value);
            }
        }

        var result = string.Empty;
        if (srcCookieDic.Count > 0)
        {
            var cookieBuilder = new StringBuilder(srcCookie.Length + addonCookie.Length);
            foreach (var item in srcCookieDic)
            {
                cookieBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0}={1}; ", item.Key, item.Value);
            }
            cookieBuilder.Remove(cookieBuilder.Length - 1, 1);
            result = cookieBuilder.ToString();
        }

        return result;
    }

    /// <summary>
    /// 从一个cookie中获取键值对
    /// </summary>
    /// <param name="cookie"></param>
    /// <returns></returns>
    private static KeyValuePair<string, string> GetCookieKVFromString(string cookie)
    {
        if (!string.IsNullOrEmpty(cookie))
        {
            try
            {
                string[] splitedCookie = cookie.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                var dic = new Dictionary<string, string>();

                foreach (var item in splitedCookie)
                {
                    int index = item.IndexOf("=", StringComparison.OrdinalIgnoreCase);
                    if (index > 0)
                    {
                        dic.Add(item.Substring(0, index), item.Substring(index + 1));
                    }
                }

                foreach (var item in dic)
                {
                    if (!IsIgnoreKey(item.Key))
                    {
                        return new KeyValuePair<string, string>(item.Key, item.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"cookie string:{cookie} analysis fail", ex);
            }
        }
        return new KeyValuePair<string, string>();
    }

    /// <summary>
    /// 是否是忽略的cookie内容
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    private static bool IsIgnoreKey(string key)
    {
        foreach (var ignoreKey in s_ignoreCookieKey)
        {
            if (key.StartsWith(ignoreKey, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }

    #endregion 方法
}
