namespace CreationsForge.Core.Engine.PluginInputs;

/// <summary>
/// Adds cancellation checks to synchronous stream reads and seeks performed by plugin parsers and owns the wrapped stream.
/// </summary>
internal sealed class CancellationCheckingStream : Stream
{
    /// <summary>The source stream owned by this wrapper.</summary>
    private readonly Stream InnerStream;

    /// <summary>The operation-scoped cancellation token checked at plugin parsing boundaries.</summary>
    private readonly CancellationToken CancellationToken;

    /// <summary>Initializes a cancellation-aware owning wrapper.</summary>
    /// <param name="innerStream">The readable and seekable stream owned by this wrapper.</param>
    /// <param name="cancellationToken">The operation-scoped token to check during reads and seeks.</param>
    /// <exception cref="ArgumentException">Thrown when the stream cannot be read or sought.</exception>
    internal CancellationCheckingStream(Stream innerStream, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(innerStream);
        if (!innerStream.CanRead || !innerStream.CanSeek)
        {
            throw new ArgumentException("Plugin parsing requires a readable and seekable stream.", nameof(innerStream));
        }

        InnerStream = innerStream;
        CancellationToken = cancellationToken;
    }

    /// <inheritdoc />
    public override bool CanRead => InnerStream.CanRead;

    /// <inheritdoc />
    public override bool CanSeek => InnerStream.CanSeek;

    /// <inheritdoc />
    public override bool CanWrite => false;

    /// <inheritdoc />
    public override long Length
    {
        get
        {
            CancellationToken.ThrowIfCancellationRequested();
            return InnerStream.Length;
        }
    }

    /// <inheritdoc />
    public override long Position
    {
        get
        {
            CancellationToken.ThrowIfCancellationRequested();
            return InnerStream.Position;
        }

        set
        {
            CancellationToken.ThrowIfCancellationRequested();
            InnerStream.Position = value;
            CancellationToken.ThrowIfCancellationRequested();
        }
    }

    /// <inheritdoc />
    public override void Flush()
    {
        CancellationToken.ThrowIfCancellationRequested();
    }

    /// <inheritdoc />
    public override int Read(byte[] buffer, int offset, int count)
    {
        CancellationToken.ThrowIfCancellationRequested();
        var read = InnerStream.Read(buffer, offset, count);
        CancellationToken.ThrowIfCancellationRequested();
        return read;
    }

    /// <inheritdoc />
    public override int Read(Span<byte> buffer)
    {
        CancellationToken.ThrowIfCancellationRequested();
        var read = InnerStream.Read(buffer);
        CancellationToken.ThrowIfCancellationRequested();
        return read;
    }

    /// <inheritdoc />
    public override int ReadByte()
    {
        CancellationToken.ThrowIfCancellationRequested();
        var value = InnerStream.ReadByte();
        CancellationToken.ThrowIfCancellationRequested();
        return value;
    }

    /// <inheritdoc />
    public override long Seek(long offset, SeekOrigin origin)
    {
        CancellationToken.ThrowIfCancellationRequested();
        var position = InnerStream.Seek(offset, origin);
        CancellationToken.ThrowIfCancellationRequested();
        return position;
    }

    /// <inheritdoc />
    public override void SetLength(long value)
    {
        throw new NotSupportedException("The plugin source stream is read-only.");
    }

    /// <inheritdoc />
    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotSupportedException("The plugin source stream is read-only.");
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            InnerStream.Dispose();
        }

        base.Dispose(disposing);
    }

    /// <inheritdoc />
    public override async ValueTask DisposeAsync()
    {
        await InnerStream.DisposeAsync().ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }
}
