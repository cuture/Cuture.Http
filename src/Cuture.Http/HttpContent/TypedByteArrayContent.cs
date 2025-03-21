﻿#pragma warning disable IDE0130

namespace Cuture.Http;

internal class TypedByteArrayContent : ByteArrayContent
{
    #region Public 构造函数

    public TypedByteArrayContent(byte[] data, string? contentType) : base(data)
    {
        Headers.TryAddWithoutValidation(HttpHeaderDefinitions.ContentType, contentType);
    }

    #endregion Public 构造函数
}
