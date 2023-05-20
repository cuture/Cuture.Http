using System;
using System.Buffers;

namespace Cuture.Http;

/// <summary>
/// 指定 ContentType 的 <inheritdoc cref="MemoryOwnedContent"/>
/// </summary>
public class TypedMemoryOwnedContent : MemoryOwnedContent
{
    #region Public 构造函数

    /// <inheritdoc cref="TypedMemoryOwnedContent(IMemoryOwner{byte},int,int,string)"/>
    public TypedMemoryOwnedContent(IMemoryOwner<byte> memoryOwner, int start, string? contentType)
        : this(memoryOwner, memoryOwner.Memory[start..], contentType)
    {
    }

    /// <summary>
    /// <inheritdoc cref="TypedMemoryOwnedContent(IMemoryOwner{byte},ReadOnlyMemory{byte},string)"/>
    /// </summary>
    /// <param name="memoryOwner"><see cref="IMemoryOwner{T}"/></param>
    /// <param name="start"><paramref name="memoryOwner"/> 中数据的切片起点</param>
    /// <param name="length"><paramref name="memoryOwner"/> 中数据的切片长度</param>
    /// <param name="contentType">ContentType</param>
    public TypedMemoryOwnedContent(IMemoryOwner<byte> memoryOwner, int start, int length, string? contentType)
        : this(memoryOwner, memoryOwner.Memory.Slice(start, length), contentType)
    {
    }

    /// <summary>
    /// <inheritdoc cref="TypedMemoryOwnedContent"/>
    /// </summary>
    /// <param name="memoryOwner"><see cref="IMemoryOwner{T}"/></param>
    /// <param name="content">内容数据</param>
    /// <param name="contentType">ContentType</param>
    public TypedMemoryOwnedContent(IMemoryOwner<byte> memoryOwner, ReadOnlyMemory<byte> content, string? contentType)
        : base(memoryOwner, content)
    {
        Headers.TryAddWithoutValidation(HttpHeaderDefinitions.ContentType, contentType);
    }

    #endregion Public 构造函数
}
