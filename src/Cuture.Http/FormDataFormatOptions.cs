namespace Cuture.Http;

/// <summary>
/// 格式化为FormData时的选项
/// </summary>
/// <inheritdoc cref="FormDataFormatOptions"/>
public struct FormDataFormatOptions(bool urlEncode)
{
    #region Public 属性

    /// <summary>
    /// 移除值为空的Key
    /// </summary>
    public bool RemoveEmptyKey { get; set; } = false;

    /// <summary>
    /// 对值进行Urlencode
    /// </summary>
    public bool UrlEncode { get; set; } = urlEncode;

    #endregion Public 属性
}
