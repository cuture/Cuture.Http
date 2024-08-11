using System.Runtime.CompilerServices;

namespace Cuture.Http;

/// <summary>
/// 一般的工具拓展
/// </summary>
public static class FormContentUtil
{
    #region 方法

    /// <summary>
    /// UrlEncode
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Encode(string? data)
    {
        if (string.IsNullOrEmpty(data))
        {
            return string.Empty;
        }
        return Uri.EscapeDataString(data).Replace("%20", "+", StringComparison.Ordinal);
    }

    /// <summary>
    /// 获取对象的已编码form表单
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public static string ToEncodedForm(object content) => HttpRequestGlobalOptions.DefaultFormDataFormatter.Format(content, new(true));

    /// <summary>
    /// 获取对象的form表单
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public static string ToForm(object content) => HttpRequestGlobalOptions.DefaultFormDataFormatter.Format(content);

    #endregion 方法
}
