using System.Buffers.Binary;

namespace CreationsForge.Bethesda.Assets.Nif;

public partial class NifPreviewModelReader
{
    private static NifPreviewVector3 ReadVector3(byte[] data, ref int position)
    {
        var vector = new NifPreviewVector3
        {
            X = ReadSingle(data, ref position),
            Y = ReadSingle(data, ref position),
            Z = ReadSingle(data, ref position)
        };
        return vector;
    }

    private static float ReadSingle(byte[] data, ref int position)
    {
        var value = BinaryPrimitives.ReadSingleLittleEndian(data.AsSpan(position, sizeof(float)));
        position += sizeof(float);
        return value;
    }

    private static NifPreviewVector3 ReadHalfVector3(byte[] data, ref int position)
    {
        var vector = new NifPreviewVector3
        {
            X = (float)BitConverter.UInt16BitsToHalf(BinaryPrimitives.ReadUInt16LittleEndian(data.AsSpan(position, sizeof(ushort)))),
            Y = (float)BitConverter.UInt16BitsToHalf(BinaryPrimitives.ReadUInt16LittleEndian(data.AsSpan(position + sizeof(ushort), sizeof(ushort)))),
            Z = (float)BitConverter.UInt16BitsToHalf(BinaryPrimitives.ReadUInt16LittleEndian(data.AsSpan(position + (sizeof(ushort) * 2), sizeof(ushort))))
        };
        position += sizeof(ushort) * 3;
        return vector;
    }

    private static NifPreviewUV ReadHalfTexCoord(byte[] data, ref int position)
    {
        var uv = new NifPreviewUV
        {
            U = (float)BitConverter.UInt16BitsToHalf(BinaryPrimitives.ReadUInt16LittleEndian(data.AsSpan(position, sizeof(ushort)))),
            V = (float)BitConverter.UInt16BitsToHalf(BinaryPrimitives.ReadUInt16LittleEndian(data.AsSpan(position + sizeof(ushort), sizeof(ushort))))
        };
        position += sizeof(ushort) * 2;
        return uv;
    }

    private static NifPreviewVector3 ReadByteVector3(byte[] data, ref int position)
    {
        var vector = new NifPreviewVector3
        {
            X = ReadNormalizedByte(data[position]),
            Y = ReadNormalizedByte(data[position + 1]),
            Z = ReadNormalizedByte(data[position + 2])
        };
        position += 4;
        return vector;
    }

    private static float ReadNormalizedByte(byte value)
    {
        return (value / 127.5f) - 1f;
    }

    private static bool IsReasonableCoordinate(float value)
    {
        return !float.IsNaN(value) && !float.IsInfinity(value) && MathF.Abs(value) <= MaxReasonableCoordinate;
    }

    private static bool IsReasonableUvCoordinate(float value)
    {
        return !float.IsNaN(value) && !float.IsInfinity(value) && MathF.Abs(value) <= 10000f;
    }

    private static bool IsReasonableScale(float value)
    {
        return !float.IsNaN(value) && !float.IsInfinity(value) && value > 0f && value <= 10000f;
    }

    private static bool IsReasonableUvTransformValue(float value)
    {
        return !float.IsNaN(value) && !float.IsInfinity(value) && MathF.Abs(value) <= 10000f;
    }

    private static bool TryReadUInt16(byte[] data, ref int position, out ushort value)
    {
        value = 0;
        if (data.Length - position < sizeof(ushort))
        {
            return false;
        }

        value = BinaryPrimitives.ReadUInt16LittleEndian(data.AsSpan(position, sizeof(ushort)));
        position += sizeof(ushort);
        return true;
    }

    private static bool TryReadUInt32(byte[] data, ref int position, out uint value)
    {
        value = 0;
        if (data.Length - position < sizeof(uint))
        {
            return false;
        }

        value = BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(position, sizeof(uint)));
        position += sizeof(uint);
        return true;
    }

    private static bool TryReadInt32(byte[] data, ref int position, out int value)
    {
        value = 0;
        if (data.Length - position < sizeof(int))
        {
            return false;
        }

        value = BinaryPrimitives.ReadInt32LittleEndian(data.AsSpan(position, sizeof(int)));
        position += sizeof(int);
        return true;
    }
}
