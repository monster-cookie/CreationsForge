namespace CreationsForge.Bethesda.Assets.Archives.Ba2;

internal readonly struct Ba2ArchiveTextureChunk
{
    public Ba2ArchiveTextureChunk(ulong dataOffset, uint packedSize, uint unpackedSize, ushort startMip, ushort endMip)
    {
        DataOffset = dataOffset;
        PackedSize = packedSize;
        UnpackedSize = unpackedSize;
        StartMip = startMip;
        EndMip = endMip;
    }

    public ulong DataOffset { get; }

    public uint PackedSize { get; }

    public uint UnpackedSize { get; }

    public ushort StartMip { get; }

    public ushort EndMip { get; }
}
