#if NEWLYTFM
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Cuture.Http
{
    //<see cref="https://source.dot.net/#System.Net.Http/System/Net/Http/ReadOnlyMemoryContent.cs"/>

    /// <summary>
    /// 需要释放资源的基于 <see cref="ReadOnlyMemory{T}"/> 的 <see cref="HttpContent"/>
    /// </summary>
    public class DisposeRequiredReadOnlyMemoryContent : HttpContent
    {
        private readonly ReadOnlyMemory<byte> _content;
        private readonly IDisposable _disposable;

        /// <inheritdoc cref="DisposeRequiredReadOnlyMemoryContent"/>
        public DisposeRequiredReadOnlyMemoryContent(ReadOnlyMemory<byte> content, IDisposable disposable)
        {
            _content = content;
            _disposable = disposable;
        }

        /// <inheritdoc/>
        protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context)
            => stream.WriteAsync(_content).AsTask();

#if NET5_0_OR_GREATER

        /// <inheritdoc/>
        protected override void SerializeToStream(Stream stream, TransportContext? context, CancellationToken cancellationToken)
            => stream.Write(_content.Span);

        /// <inheritdoc/>
        protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context, CancellationToken cancellationToken)
            => stream.WriteAsync(_content, cancellationToken).AsTask();

#endif

        /// <inheritdoc/>
        protected override bool TryComputeLength(out long length)
        {
            length = _content.Length;
            return true;
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _disposable.Dispose();
        }
    }
}
#endif