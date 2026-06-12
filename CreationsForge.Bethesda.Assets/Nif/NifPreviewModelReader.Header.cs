namespace CreationsForge.Bethesda.Assets.Nif;

public partial class NifPreviewModelReader
{
    private static NifHeader ReadHeader(NifBinaryReader reader)
    {
        var headerString = reader.ReadLineString();
        if (!headerString.StartsWith("Gamebryo File Format", StringComparison.Ordinal) &&
            !headerString.StartsWith("NetImmerse File Format", StringComparison.Ordinal))
        {
            throw new InvalidDataException("The file is not a recognized NIF stream.");
        }

        var version = reader.ReadUInt32();
        if (version != Fallout4Version)
        {
            throw new InvalidDataException($"NIF version 0x{version:X8} is not supported by the first preview reader.");
        }

        var endianType = reader.ReadByte();
        if (endianType != 1)
        {
            throw new InvalidDataException("Only little-endian NIF streams are supported.");
        }

        var userVersion = reader.ReadUInt32();
        var blockCount = reader.ReadUInt32();
        if (blockCount > MaxReasonableBlockCount)
        {
            throw new InvalidDataException("NIF block count is outside the supported range.");
        }

        var bethesdaVersion = reader.ReadUInt32();
        if (!IsSupportedBethesdaStreamVersion(userVersion, bethesdaVersion))
        {
            throw new InvalidDataException($"Bethesda stream version {bethesdaVersion} is not supported by the first preview reader.");
        }

        if (TryFindHeaderTables(reader, userVersion, bethesdaVersion, blockCount, out var header, out var failureReason))
        {
            return header;
        }

        throw new InvalidDataException($"NIF header tables could not be located with a supported Bethesda header layout. {failureReason}");
    }

    private static bool IsSupportedBethesdaStreamVersion(uint userVersion, uint bethesdaVersion)
    {
        return bethesdaVersion >= Fallout4BethesdaVersion ||
            userVersion == SkyrimSpecialEditionUserVersion && bethesdaVersion == SkyrimSpecialEditionBethesdaVersion;
    }

    private static bool TryFindHeaderTables(
        NifBinaryReader reader,
        uint userVersion,
        uint bethesdaVersion,
        uint blockCount,
        out NifHeader header,
        out string failureReason)
    {
        header = new NifHeader(userVersion, bethesdaVersion, [], []);
        failureReason = "No plausible block type table was found.";
        var searchStart = reader.Position;
        var searchEnd = searchStart + Math.Min(reader.Remaining, MaxHeaderTableSearchBytes);

        for (var position = searchStart; position <= searchEnd - sizeof(ushort); position++)
        {
            var candidateReader = reader;
            candidateReader.Seek(position);
            if (TryReadHeaderTables(candidateReader, userVersion, bethesdaVersion, blockCount, out header, out var candidateFailureReason))
            {
                return true;
            }

            if (!string.IsNullOrWhiteSpace(candidateFailureReason))
            {
                failureReason = candidateFailureReason;
            }
        }

        return false;
    }

    private static bool TryReadHeaderTables(
        NifBinaryReader reader,
        uint userVersion,
        uint bethesdaVersion,
        uint blockCount,
        out NifHeader header,
        out string failureReason)
    {
        header = new NifHeader(userVersion, bethesdaVersion, [], []);
        failureReason = string.Empty;

        try
        {
            var blockTypeCount = reader.ReadUInt16();
            if (blockTypeCount == 0 || blockTypeCount > MaxReasonableBlockTypeCount)
            {
                return false;
            }

            var blockTypes = new List<string>();
            for (var index = 0; index < blockTypeCount; index++)
            {
                var blockType = reader.ReadSizedString();
                if (!IsPlausibleBlockTypeName(blockType))
                {
                    return false;
                }

                blockTypes.Add(blockType);
            }

            if (!blockTypes.Any(IsLikelyNifBlockTypeName))
            {
                return false;
            }

            var blockTypeIndexes = new List<ushort>();
            for (var index = 0; index < blockCount; index++)
            {
                var blockTypeIndex = reader.ReadUInt16();
                if (blockTypeIndex >= blockTypes.Count)
                {
                    throw new InvalidDataException("NIF block type index is outside the block type table.");
                }

                blockTypeIndexes.Add(blockTypeIndex);
            }

            var blockSizes = new List<uint>();
            for (var index = 0; index < blockCount; index++)
            {
                blockSizes.Add(reader.ReadUInt32());
            }

            if (!TryReadStringTable(ref reader, out var strings) || !TrySkipGroupTable(ref reader))
            {
                return false;
            }

            var blocks = new List<NifBlock>();
            for (var index = 0; index < blockCount; index++)
            {
                var blockSize = blockSizes[index];
                if (blockSize > int.MaxValue || reader.Remaining < blockSize)
                {
                    throw new InvalidDataException("NIF block payload size is outside the stream.");
                }

                blocks.Add(new NifBlock(blockTypes[blockTypeIndexes[index]], reader.ReadBytes((int)blockSize)));
            }

            header = new NifHeader(userVersion, bethesdaVersion, strings, blocks);
            return true;
        }
        catch (Exception exception) when (exception is InvalidDataException or EndOfStreamException or ArgumentOutOfRangeException)
        {
            failureReason = exception.Message;
            return false;
        }
    }

    private static bool IsPlausibleBlockTypeName(string value)
    {
        return value.Length > 0 &&
            value.Length <= 128 &&
            value.All(character => character is >= 'A' and <= 'Z' or >= 'a' and <= 'z' or >= '0' and <= '9' or ' ' or '_' or ':');
    }

    private static bool IsLikelyNifBlockTypeName(string value)
    {
        return value.StartsWith("Ni", StringComparison.Ordinal) ||
            value.StartsWith("BS", StringComparison.Ordinal) ||
            value.StartsWith("bhk", StringComparison.Ordinal);
    }

    private static bool TryReadStringTable(ref NifBinaryReader reader, out IReadOnlyList<string> strings)
    {
        strings = [];
        var stringCount = reader.ReadUInt32();
        var maxStringLength = reader.ReadUInt32();
        if (maxStringLength > MaxReasonableStringLength)
        {
            return false;
        }

        var parsedStrings = new List<string>();
        for (var index = 0; index < stringCount; index++)
        {
            if (!reader.TryReadSizedString(out var value))
            {
                return false;
            }

            parsedStrings.Add(value);
        }

        strings = parsedStrings;
        return true;
    }

    private static bool TrySkipGroupTable(ref NifBinaryReader reader)
    {
        var groupCount = reader.ReadUInt32();
        for (var index = 0; index < groupCount; index++)
        {
            reader.ReadUInt32();
        }

        return true;
    }
}
