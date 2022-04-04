using System;
using System.Net.Http;

namespace Cuture.Http;

internal class TypedByteArrayContent : ByteArrayContent
{
    #region Public 构造函数

    public TypedByteArrayContent(byte[] data, string contentType) : base(data)
    {
        if (string.IsNullOrWhiteSpace(contentType))
        {
            throw new ArgumentException($"“{nameof(contentType)}”不能为 Null 或空白", nameof(contentType));
        }

        Headers.TryAddWithoutValidation(HttpHeaderDefinitions.ContentType, contentType);
    }

    #endregion Public 构造函数
}
