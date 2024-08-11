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
    private static readonly char[][] s_ignoreCookieKey = new char[][]
    {
        "Domain".ToCharArray(),
        "Expires".ToCharArray(),
        "HttpOnly".ToCharArray(),
        "Path".ToCharArray(),
        "Max-Age".ToCharArray()
    };

    #endregion 字段

    #region 方法

    /// <summary>
    /// 分析指定cookie并返回键值对集合
    /// </summary>
    /// <param name="cookieText"></param>
    /// <param name="mergeSameKey">合并同名 Key，而非使用后出现的值覆盖之前的值</param>
    /// <returns></returns>
    public static Dictionary<string, string> AnalysisCookieString(string? cookieText, bool mergeSameKey = true)
    {
        var cookies = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        if (string.IsNullOrWhiteSpace(cookieText))
        {
            return cookies;
        }

        AnalysisCookieString(cookies, cookieText, mergeSameKey);

        return cookies;
    }

    /// <summary>
    /// 清理Cookie，使其成为KV格式
    /// </summary>
    /// <param name="cookieText"></param>
    /// <param name="mergeSameKey">合并同名 Key，而非使用后出现的值覆盖之前的值</param>
    /// <returns></returns>
    public static string Clean(string? cookieText, bool mergeSameKey = true)
    {
        var cookies = AnalysisCookieString(cookieText, mergeSameKey);

        if (cookies.Count > 0)
        {
            var cookieBuilder = new StringBuilder(cookieText!.Length);
            foreach (var item in cookies)
            {
                cookieBuilder.Append($"{item.Key}={item.Value}; ");
            }
            cookieBuilder.Remove(cookieBuilder.Length - 1, 1);
            return cookieBuilder.ToString();
        }

        return string.Empty;
    }

    /// <summary>
    /// 合并Cookie
    /// </summary>
    /// <param name="srcCookie">源cookie</param>
    /// <param name="addonCookie">附加cookie</param>
    /// <param name="mergeSameKey">合并同名 Key，而非使用后出现的值覆盖之前的值</param>
    /// <returns></returns>
    public static string Merge(string? srcCookie, string? addonCookie, bool mergeSameKey = true)
    {
        srcCookie ??= string.Empty;
        if (string.IsNullOrWhiteSpace(addonCookie))
        {
            return srcCookie;
        }

        var cookies = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        AnalysisCookieString(cookies, srcCookie, mergeSameKey);
        AnalysisCookieString(cookies, addonCookie, mergeSameKey);

        if (cookies.Count > 0)
        {
            var cookieBuilder = new StringBuilder(srcCookie.Length + addonCookie.Length);
            foreach (var item in cookies)
            {
                cookieBuilder.Append($"{item.Key}={item.Value}; ");
            }
            cookieBuilder.Remove(cookieBuilder.Length - 1, 1);
            return cookieBuilder.ToString();
        }

        return string.Empty;
    }

    /// <summary>
    /// 分析指定cookie并返回键值对集合
    /// </summary>
    /// <param name="cookies"></param>
    /// <param name="cookieText"></param>
    /// <param name="mergeSameKey">合并同名 Key，而非使用后出现的值覆盖之前的值</param>
    /// <returns></returns>
    private static void AnalysisCookieString(Dictionary<string, string> cookies, string cookieText, bool mergeSameKey = true)
    {
        var span = cookieText.AsSpan();

        while (!span.IsEmpty)
        {
            var semicolonIndex = span.IndexOf(';');

            ReadOnlySpan<char> currentSpan;

            if (semicolonIndex > 0)
            {
                currentSpan = span[..semicolonIndex];
                span = span[(semicolonIndex + 1)..];
            }
            else
            {
                currentSpan = span;
                span = ReadOnlySpan<char>.Empty;
            }

            var equalIndex = currentSpan.IndexOf('=');

            if (equalIndex < 1) //没有等号，跳过当前处理
            {
                continue;
            }

            var keySpan = currentSpan[..equalIndex];

            //移除头部空白
            keySpan = keySpan[(keySpan.IndexOf(' ') + 1)..];

            if (IsIgnoreKey(keySpan))   //不是正确的Cookie，跳过处理
            {
                continue;
            }

            var valueSpan = currentSpan[(equalIndex + 1)..];
            if (valueSpan.IsWhiteSpace())
            {
                continue;
            }

            var key = keySpan.ToString();

            var value = valueSpan.ToString();

            if (cookies.TryGetValue(key, out var existedValue))
            {
                if (mergeSameKey)
                {
                    cookies[key] = $"{existedValue}; {value}";
                }
                else
                {
                    cookies[key] = value;
                }
            }
            else
            {
                cookies.Add(key, value);
            }
        }
    }

    /// <summary>
    /// 是否是忽略的cookie内容
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    private static bool IsIgnoreKey(ReadOnlySpan<char> key)
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
