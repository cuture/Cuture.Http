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
    /// 指定 ContentType 的 <inheritdoc cref="MemoryOwnedContent"/>
    /// </summary>
    public class TypedMemoryOwnedContent : MemoryOwnedContent
    {
        /// <inheritdoc cref="TypedMemoryOwnedContent(IMemoryOwner{byte},int,int,string)"/>
        public TypedMemoryOwnedContent(IMemoryOwner<byte> memoryOwner, int start, string contentType)
            : this(memoryOwner.Memory[start..], memoryOwner, contentType)
        {
        }

        /// <summary>
        /// <inheritdoc cref="TypedMemoryOwnedContent(ReadOnlyMemory{byte},IMemoryOwner{byte},string)"/>
        /// </summary>
        /// <param name="memoryOwner"><see cref="IMemoryOwner{T}"/></param>
        /// <param name="start"><paramref name="memoryOwner"/> 中数据的切片起点</param>
        /// <param name="length"><paramref name="memoryOwner"/> 中数据的切片长度</param>
        /// <param name="contentType">ContentType</param>
        public TypedMemoryOwnedContent(IMemoryOwner<byte> memoryOwner, int start, int length, string contentType)
            : this(memoryOwner.Memory.Slice(start, length), memoryOwner, contentType)
        {
        }

        /// <summary>
        /// <inheritdoc cref="TypedMemoryOwnedContent"/>
        /// </summary>
        /// <param name="content">内容数据</param>
        /// <param name="memoryOwner"><see cref="IMemoryOwner{T}"/></param>
        /// <param name="contentType">ContentType</param>
        public TypedMemoryOwnedContent(ReadOnlyMemory<byte> content, IMemoryOwner<byte> memoryOwner, string contentType)
            : base(content, memoryOwner)
        {
            if (string.IsNullOrWhiteSpace(contentType))
            {
                throw new ArgumentException($"“{nameof(contentType)}”不能为 Null 或空白", nameof(contentType));
            }

            Headers.TryAddWithoutValidation(HttpHeaderDefinitions.ContentType, contentType);
        }
    }
}
#endif