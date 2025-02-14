#pragma warning disable IDE0130

using System.Runtime.CompilerServices;
using System.Text;

namespace Cuture.Http;

/// <summary>
/// HttpFormContent
/// </summary>
public class FormContent : ByteArrayContent
{
    #region 字段

    /// <summary>
    /// <see cref="FormContent"/> 的默认ContentType
    /// </summary>
    public const string ContentType = "application/x-www-form-urlencoded";

    /// <summary>
    /// 空的Content
    /// </summary>
    private static readonly byte[] s_emptyContent = [];

    #endregion 字段

    #region 构造函数

    /// <inheritdoc cref="FormContent(string, string, Encoding)"/>
    public FormContent(string content) : this(content, ContentType, Encoding.UTF8)
    {
    }

    /// <inheritdoc cref="FormContent(string, string, Encoding)"/>
    public FormContent(string content, Encoding encoding) : this(content, ContentType, encoding)
    {
    }

    /// <inheritdoc cref="FormContent(string, string, Encoding)"/>
    public FormContent(string content, string contentType) : this(content, contentType, Encoding.UTF8)
    {
    }

    /// <summary>
    /// HttpFormContent
    /// </summary>
    /// <param name="content">已编码的from内容</param>
    /// <param name="contentType">指定ContentType</param>
    /// <param name="encoding">指定编码类型</param>
    public FormContent(string content, string contentType, Encoding encoding) : base(GetBytes(content, encoding))
    {
        Headers.TryAddWithoutValidation(HttpHeaderDefinitions.ContentType, contentType);
    }

    /// <inheritdoc cref="FormContent(object, string, Encoding, IFormDataFormatter)"/>
    public FormContent(object content) : this(content, ContentType, Encoding.UTF8)
    {
    }

    /// <inheritdoc cref="FormContent(object, string, Encoding, IFormDataFormatter)"/>
    public FormContent(object content, Encoding encoding) : this(content, ContentType, encoding)
    {
    }

    /// <inheritdoc cref="FormContent(object, string, Encoding, IFormDataFormatter)"/>
    public FormContent(object content, IFormDataFormatter formatter) : this(content, ContentType, Encoding.UTF8, formatter)
    {
    }

    /// <inheritdoc cref="FormContent(object, string, Encoding, IFormDataFormatter)"/>
    public FormContent(object content, string contentType) : this(content, contentType, Encoding.UTF8)
    {
    }

    /// <inheritdoc cref="FormContent(object, string, Encoding, IFormDataFormatter)"/>
    public FormContent(object content, string contentType, Encoding encoding) : this(content, contentType, encoding, HttpRequestGlobalOptions.DefaultFormDataFormatter)
    {
    }

    /// <summary>
    /// HttpFormContent
    /// </summary>
    /// <param name="content">用于转换为form的对象</param>
    /// <param name="contentType">指定ContentType</param>
    /// <param name="encoding">指定编码类型</param>
    /// <param name="formatter">指定格式化器</param>
    public FormContent(object content, string contentType, Encoding encoding, IFormDataFormatter formatter) : base(GetBytes(content, encoding, formatter))
    {
        Headers.TryAddWithoutValidation(HttpHeaderDefinitions.ContentType, contentType);
    }

    #endregion 构造函数

    #region 方法

    private static byte[] GetBytes(object content, Encoding encoding, IFormDataFormatter formatter)
    {
        if (content is null)
        {
            return s_emptyContent;
        }
        else if (content is string str)
        {
            return GetBytes(str, encoding);
        }

        var data = string.Empty;

        if (string.IsNullOrEmpty(data))
        {
            data = formatter.Format(content, new(true));
        }

        return encoding.GetBytes(data);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static byte[] GetBytes(string data, Encoding encoding)
    {
        if (string.IsNullOrEmpty(data))
        {
            return s_emptyContent;
        }
        return encoding.GetBytes(data.Replace("%20", "+", StringComparison.Ordinal));
    }

    #endregion 方法
}
