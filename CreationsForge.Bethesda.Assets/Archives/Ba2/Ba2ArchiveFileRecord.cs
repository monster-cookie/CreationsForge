namespace CreationsForge.Bethesda.Assets.Archives.Ba2;

internal readonly struct Ba2ArchiveFileRecord
{
    public Ba2ArchiveFileRecord(ulong dataOffset, uint packedSize, uint unpackedSize)
    {
        DataOffset = dataOffset;
        PackedSize = packedSize;
        UnpackedSize = unpackedSize;
    }

    public ulong DataOffset { get; }

    public uint PackedSize { get; }

    public uint UnpackedSize { get; }
}
