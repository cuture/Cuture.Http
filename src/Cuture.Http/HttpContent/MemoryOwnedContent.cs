using System;
using System.Buffers;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Cuture.Http;

//<see cref="https://source.dot.net/#System.Net.Http/System/Net/Http/ReadOnlyMemoryContent.cs"/>

/// <summary>
/// 内容来自于 <see cref="IMemoryOwner{T}"/> 的 <see cref="HttpContent"/>
/// </summary>
public class MemoryOwnedContent : DisposeRequiredContent
{
    #region Private 字段

    private readonly ReadOnlyMemory<byte> _content;

    #endregion Private 字段

    #region Public 构造函数

    /// <inheritdoc cref="MemoryOwnedContent(IMemoryOwner{byte},int,int)"/>
    public MemoryOwnedContent(IMemoryOwner<byte> memoryOwner, int start)
        : this(memoryOwner, memoryOwner.Memory[start..])
    {
    }

    /// <summary>
    /// <inheritdoc cref="MemoryOwnedContent(IMemoryOwner{byte},ReadOnlyMemory{byte})"/>
    /// </summary>
    /// <param name="memoryOwner"><see cref="IMemoryOwner{T}"/></param>
    /// <param name="start"><paramref name="memoryOwner"/> 中数据的切片起点</param>
    /// <param name="length"><paramref name="memoryOwner"/> 中数据的切片长度</param>
    public MemoryOwnedContent(IMemoryOwner<byte> memoryOwner, int start, int length)
        : this(memoryOwner, memoryOwner.Memory.Slice(start, length))
    {
    }

    /// <summary>
    /// <inheritdoc cref="MemoryOwnedContent"/>
    /// </summary>
    /// <param name="content">内容数据</param>
    /// <param name="memoryOwner"><see cref="IMemoryOwner{T}"/></param>
    public MemoryOwnedContent(IMemoryOwner<byte> memoryOwner, ReadOnlyMemory<byte> content)
        : base(memoryOwner)
    {
        _content = content;
    }

    #endregion Public 构造函数

    #region Protected 方法

    /// <inheritdoc/>
    protected override void SerializeToStream(Stream stream, TransportContext? context, CancellationToken cancellationToken)
        => stream.Write(_content.Span);

    /// <inheritdoc/>
    protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context)
        => stream.WriteAsync(_content).AsTask();

    /// <inheritdoc/>
    protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context, CancellationToken cancellationToken)
        => stream.WriteAsync(_content, cancellationToken).AsTask();

    /// <inheritdoc/>
    protected override bool TryComputeLength(out long length)
    {
        length = _content.Length;
        return true;
    }

    #endregion Protected 方法
}