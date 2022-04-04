namespace Cuture.Http;

/// <summary>
/// 格式化为FormData时的选项
/// </summary>
public struct FormDataFormatOptions
{
    #region Public 属性

    /// <summary>
    /// 移除值为空的Key
    /// </summary>
    public bool RemoveEmptyKey { get; set; }

    /// <summary>
    /// 对值进行Urlencode
    /// </summary>
    public bool UrlEncode { get; set; }

    #endregion Public 属性

    #region Public 构造函数

    /// <inheritdoc cref="FormDataFormatOptions"/>
    public FormDataFormatOptions(bool urlEncode)
    {
        UrlEncode = urlEncode;
        RemoveEmptyKey = false;
    }

    #endregion Public 构造函数
}
