namespace CreationsForge.Bethesda.Assets.Archives.Ba2;

internal readonly struct Ba2ArchiveTextureRecord
{
    public Ba2ArchiveTextureRecord(ushort height, ushort width, byte mipCount, byte dxgiFormat, ushort flags, IReadOnlyList<Ba2ArchiveTextureChunk> chunks)
    {
        Height = height;
        Width = width;
        MipCount = mipCount;
        DxgiFormat = dxgiFormat;
        Flags = flags;
        Chunks = chunks;
    }

    public ushort Height { get; }

    public ushort Width { get; }

    public byte MipCount { get; }

    public byte DxgiFormat { get; }

    public ushort Flags { get; }

    public IReadOnlyList<Ba2ArchiveTextureChunk> Chunks { get; }

    public uint PackedSize => (uint)Chunks.Sum(chunk => chunk.PackedSize);

    public uint UnpackedSize => (uint)Chunks.Sum(chunk => chunk.UnpackedSize);
}
