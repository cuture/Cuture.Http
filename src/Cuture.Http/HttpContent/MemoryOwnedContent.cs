#if NEWLYTFM
using System;
using System.Buffers;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Cuture.Http
{
    /// <summary>
    /// 内容来自于 <see cref="IMemoryOwner{T}"/> 的 <see cref="HttpContent"/>
    /// </summary>
    public class MemoryOwnedContent : DisposeRequiredReadOnlyMemoryContent
    {
        /// <inheritdoc cref="MemoryOwnedContent(IMemoryOwner{byte},int,int)"/>
        public MemoryOwnedContent(IMemoryOwner<byte> memoryOwner, int start)
            : this(memoryOwner.Memory[start..], memoryOwner)
        {
        }

        /// <summary>
        /// <inheritdoc cref="MemoryOwnedContent(ReadOnlyMemory{byte}, IMemoryOwner{byte})"/>
        /// </summary>
        /// <param name="memoryOwner"><see cref="IMemoryOwner{T}"/></param>
        /// <param name="start"><paramref name="memoryOwner"/> 中数据的切片起点</param>
        /// <param name="length"><paramref name="memoryOwner"/> 中数据的切片长度</param>
        public MemoryOwnedContent(IMemoryOwner<byte> memoryOwner, int start, int length)
            : this(memoryOwner.Memory.Slice(start, length), memoryOwner)
        {
        }

        /// <summary>
        /// <inheritdoc cref="MemoryOwnedContent"/>
        /// </summary>
        /// <param name="content">内容数据</param>
        /// <param name="memoryOwner"><see cref="IMemoryOwner{T}"/></param>
        public MemoryOwnedContent(ReadOnlyMemory<byte> content, IMemoryOwner<byte> memoryOwner)
            : base(content, memoryOwner)
        {
        }
    }
}
#endif