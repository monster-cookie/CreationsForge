using System.Buffers.Binary;
using System.Globalization;
using System.Text;

namespace CreationsForge.Bethesda.Assets.Nif;

public partial class NifPreviewModelReader
{
    private readonly struct NifHeader
    {
        public NifHeader(uint userVersion, uint bethesdaVersion, IReadOnlyList<string> strings, IReadOnlyList<NifBlock> blocks)
        {
            UserVersion = userVersion;
            BethesdaVersion = bethesdaVersion;
            Strings = strings;
            Blocks = blocks;
        }

        public uint UserVersion { get; }

        public uint BethesdaVersion { get; }

        public IReadOnlyList<string> Strings { get; }

        public IReadOnlyList<NifBlock> Blocks { get; }
    }

    private readonly struct NifMaterialInfo
    {
        public NifMaterialInfo(string materialName, string? texturePath, string? overlayTexturePath, string? decalOpacityTexturePath, StarfieldPreviewColor materialTint, float decalTintRed, float decalTintGreen, float decalTintBlue, float decalOpacity, StarfieldPreviewUvTransform decalUvTransform, bool isDecal, bool isInvisible, bool useAdditiveBlend)
        {
            MaterialName = materialName;
            TexturePath = texturePath;
            OverlayTexturePath = overlayTexturePath;
            DecalOpacityTexturePath = decalOpacityTexturePath;
            MaterialTint = materialTint;
            DecalTintRed = decalTintRed;
            DecalTintGreen = decalTintGreen;
            DecalTintBlue = decalTintBlue;
            DecalOpacity = decalOpacity;
            DecalUvTransform = decalUvTransform;
            IsDecal = isDecal;
            IsInvisible = isInvisible;
            UseAdditiveBlend = useAdditiveBlend;
        }

        public string MaterialName { get; }

        public string? TexturePath { get; }

        public string? OverlayTexturePath { get; }

        public string? DecalOpacityTexturePath { get; }

        public StarfieldPreviewColor MaterialTint { get; }

        public float DecalTintRed { get; }

        public float DecalTintGreen { get; }

        public float DecalTintBlue { get; }

        public float DecalOpacity { get; }

        public StarfieldPreviewUvTransform DecalUvTransform { get; }

        public bool IsDecal { get; }

        public bool IsInvisible { get; }

        public bool UseAdditiveBlend { get; }
    }

    private readonly struct NifShapeMaterial
    {
        public NifShapeMaterial(string meshName, NifMaterialInfo material)
        {
            MeshName = meshName;
            MaterialName = material.MaterialName;
            TexturePath = material.TexturePath;
            OverlayTexturePath = material.OverlayTexturePath;
            DecalOpacityTexturePath = material.DecalOpacityTexturePath;
            MaterialTint = material.MaterialTint;
            DecalTintRed = material.DecalTintRed;
            DecalTintGreen = material.DecalTintGreen;
            DecalTintBlue = material.DecalTintBlue;
            DecalOpacity = material.DecalOpacity;
            DecalUvTransform = material.DecalUvTransform;
            IsDecal = material.IsDecal;
            IsInvisible = material.IsInvisible;
            UseAdditiveBlend = material.UseAdditiveBlend;
        }

        public string MeshName { get; }

        public string MaterialName { get; }

        public string? TexturePath { get; }

        public string? OverlayTexturePath { get; }

        public string? DecalOpacityTexturePath { get; }

        public StarfieldPreviewColor MaterialTint { get; }

        public float DecalTintRed { get; }

        public float DecalTintGreen { get; }

        public float DecalTintBlue { get; }

        public float DecalOpacity { get; }

        public StarfieldPreviewUvTransform DecalUvTransform { get; }

        public bool IsDecal { get; }

        public bool IsInvisible { get; }

        public bool UseAdditiveBlend { get; }
    }

    private readonly struct StarfieldPreviewUvTransform
    {
        public static StarfieldPreviewUvTransform Identity { get; } = new(1f, 1f, 0f, 0f);

        public StarfieldPreviewUvTransform(float scaleU, float scaleV, float offsetU, float offsetV)
        {
            ScaleU = scaleU;
            ScaleV = scaleV;
            OffsetU = offsetU;
            OffsetV = offsetV;
        }

        public float ScaleU { get; }

        public float ScaleV { get; }

        public float OffsetU { get; }

        public float OffsetV { get; }

        public bool IsIdentity =>
            MathF.Abs(ScaleU - 1f) <= 0.0001f &&
            MathF.Abs(ScaleV - 1f) <= 0.0001f &&
            MathF.Abs(OffsetU) <= 0.0001f &&
            MathF.Abs(OffsetV) <= 0.0001f;

        public string Description =>
            $"scale ({ScaleU.ToString("0.###", CultureInfo.InvariantCulture)},{ScaleV.ToString("0.###", CultureInfo.InvariantCulture)}), offset ({OffsetU.ToString("0.###", CultureInfo.InvariantCulture)},{OffsetV.ToString("0.###", CultureInfo.InvariantCulture)})";
    }

    private readonly struct StarfieldPreviewColor
    {
        public static StarfieldPreviewColor White { get; } = new(1f, 1f, 1f, 1f);

        public StarfieldPreviewColor(float red, float green, float blue, float alpha)
        {
            Red = red;
            Green = green;
            Blue = blue;
            Alpha = alpha;
        }

        public float Red { get; }

        public float Green { get; }

        public float Blue { get; }

        public float Alpha { get; }

        public bool IsWhite =>
            MathF.Abs(Red - 1f) <= 0.0001f &&
            MathF.Abs(Green - 1f) <= 0.0001f &&
            MathF.Abs(Blue - 1f) <= 0.0001f &&
            MathF.Abs(Alpha - 1f) <= 0.0001f;

        public string Description =>
            $"({Red.ToString("0.###", CultureInfo.InvariantCulture)},{Green.ToString("0.###", CultureInfo.InvariantCulture)},{Blue.ToString("0.###", CultureInfo.InvariantCulture)},{Alpha.ToString("0.###", CultureInfo.InvariantCulture)})";
    }

    private readonly struct NifBlock
    {
        public NifBlock(string typeName, byte[] data)
        {
            TypeName = typeName;
            Data = data;
        }

        public string TypeName { get; }

        public byte[] Data { get; }
    }

    private struct NifMeshBounds
    {
        private float MinX;
        private float MinY;
        private float MinZ;
        private float MaxX;
        private float MaxY;
        private float MaxZ;
        private bool HasPosition;

        public float LongestAxis => MathF.Max(MaxX - MinX, MathF.Max(MaxY - MinY, MaxZ - MinZ));

        public float ShapeScore
        {
            get
            {
                var x = MaxX - MinX;
                var y = MaxY - MinY;
                var z = MaxZ - MinZ;
                var longest = MathF.Max(x, MathF.Max(y, z));
                var shortest = MathF.Min(x, MathF.Min(y, z));
                return longest <= 0f ? 0f : shortest / longest;
            }
        }

        public string Description => HasPosition
            ? $"X {MinX:N3}..{MaxX:N3}, Y {MinY:N3}..{MaxY:N3}, Z {MinZ:N3}..{MaxZ:N3}"
            : "empty";

        public bool TryInclude(NifPreviewVector3 position, out string rejectionReason)
        {
            rejectionReason = string.Empty;
            if (!IsReasonableCoordinate(position.X) ||
                !IsReasonableCoordinate(position.Y) ||
                !IsReasonableCoordinate(position.Z))
            {
                rejectionReason = $"vertex position was outside the supported preview range ({position.X}, {position.Y}, {position.Z})";
                return false;
            }

            if (!HasPosition)
            {
                MinX = position.X;
                MaxX = position.X;
                MinY = position.Y;
                MaxY = position.Y;
                MinZ = position.Z;
                MaxZ = position.Z;
                HasPosition = true;
                return true;
            }

            MinX = MathF.Min(MinX, position.X);
            MinY = MathF.Min(MinY, position.Y);
            MinZ = MathF.Min(MinZ, position.Z);
            MaxX = MathF.Max(MaxX, position.X);
            MaxY = MathF.Max(MaxY, position.Y);
            MaxZ = MathF.Max(MaxZ, position.Z);
            return true;
        }

        public bool IsUsefulPreviewBounds(NifMeshBounds fallbackBounds)
        {
            return HasPosition &&
                LongestAxis > 0.0001f &&
                LongestAxis < fallbackBounds.LongestAxis * 1000f;
        }
    }

    private readonly struct NifTriangleQuality
    {
        public NifTriangleQuality(int triangleCount, float averageEdgeRatio, float maxEdgeRatio, int degenerateCount)
        {
            TriangleCount = triangleCount;
            AverageEdgeRatio = averageEdgeRatio;
            MaxEdgeRatio = maxEdgeRatio;
            DegenerateCount = degenerateCount;
        }

        public int TriangleCount { get; }

        public float AverageEdgeRatio { get; }

        public float MaxEdgeRatio { get; }

        public int DegenerateCount { get; }

        public string Description =>
            $"edge ratios avg {AverageEdgeRatio:N3}, max {MaxEdgeRatio:N3}, degenerate {DegenerateCount}/{TriangleCount}";
    }

    private readonly struct NifNormalQuality
    {
        public NifNormalQuality(int count, int validCount, float minLength, float maxLength)
        {
            Count = count;
            ValidCount = validCount;
            MinLength = minLength;
            MaxLength = maxLength;
        }

        public int Count { get; }

        public int ValidCount { get; }

        public float MinLength { get; }

        public float MaxLength { get; }

        public string Description =>
            $"{ValidCount}/{Count} valid, length {MinLength:N3}..{MaxLength:N3}";
    }

    private readonly struct BSVertexDescriptor
    {
        public BSVertexDescriptor(ulong value)
        {
            StrideWords = (int)(value & 0xF);
            UVOffset = (int)((value >> 8) & 0xF) * sizeof(uint);
            NormalOffset = (int)((value >> 16) & 0xF) * sizeof(uint);
            Attributes = (ushort)(value >> 44);
        }

        public int StrideWords { get; }

        public int VertexStride => StrideWords * sizeof(uint);

        public int UVOffset { get; }

        public int NormalOffset { get; }

        public ushort Attributes { get; }

        public bool HasUV => (Attributes & 0x2) != 0 && UVOffset > 0 && UVOffset + sizeof(uint) <= VertexStride;

        public bool HasNormals => (Attributes & 0x8) != 0 && NormalOffset > 0 && NormalOffset + sizeof(uint) <= VertexStride;

        public bool HasFullPrecisionPositions => (Attributes & 0x400) == 0x400;

        public string Description =>
            $"stride words {StrideWords}, stride bytes {VertexStride}, attributes 0x{Attributes:X}, uv offset {UVOffset}, normal offset {NormalOffset}, full precision {HasFullPrecisionPositions}";
    }

    private readonly struct BSGeometryCountProbe
    {
        public BSGeometryCountProbe(uint triangleCount, ushort vertexCount, int vertexDataOffset, int countProbeOffset)
        {
            TriangleCount = triangleCount;
            VertexCount = vertexCount;
            VertexDataOffset = vertexDataOffset;
            CountProbeOffset = countProbeOffset;
        }

        public uint TriangleCount { get; }

        public ushort VertexCount { get; }

        public int VertexDataOffset { get; }

        public int CountProbeOffset { get; }
    }

    private readonly struct StarfieldGeometryBounds
    {
        private readonly NifPreviewVector3 Center;
        private readonly NifPreviewVector3 Extents;

        public StarfieldGeometryBounds(NifPreviewVector3 center, NifPreviewVector3 extents)
        {
            Center = center;
            Extents = extents;
        }

        public bool IsReasonable =>
            IsReasonableCoordinate(Center.X) &&
            IsReasonableCoordinate(Center.Y) &&
            IsReasonableCoordinate(Center.Z) &&
            IsReasonableExtent(Extents.X) &&
            IsReasonableExtent(Extents.Y) &&
            IsReasonableExtent(Extents.Z);

        public string Description => $"center {FormatVector(Center)}, extents {FormatVector(Extents)}";

        public NifPreviewVector3 ExpandNormalizedPosition(NifPreviewVector3 position)
        {
            return new NifPreviewVector3
            {
                X = Center.X + (position.X * Extents.X),
                Y = Center.Y + (position.Y * Extents.Y),
                Z = Center.Z + (position.Z * Extents.Z)
            };
        }

        private static bool IsReasonableExtent(float value)
        {
            return !float.IsNaN(value) && !float.IsInfinity(value) && value > 0f && value <= MaxReasonableCoordinate;
        }
    }

    private readonly struct NifObjectTransform
    {
        public static readonly NifObjectTransform Identity = new NifObjectTransform(
            new NifPreviewVector3(),
            NifRotation3x3.Identity,
            1f);

        private readonly NifPreviewVector3 Translation;
        private readonly NifRotation3x3 Rotation;
        private readonly float Scale;

        public NifObjectTransform(NifPreviewVector3 translation, NifRotation3x3 rotation, float scale)
        {
            Translation = translation;
            Rotation = rotation;
            Scale = scale;
        }

        public bool IsIdentity =>
            Translation.X == 0f &&
            Translation.Y == 0f &&
            Translation.Z == 0f &&
            Scale == 1f &&
            Rotation.Equals(NifRotation3x3.Identity);

        public string Description =>
            $"translation {FormatVector(Translation)}, rotation {Rotation.Description}, scale {Scale.ToString("0.###", CultureInfo.InvariantCulture)}";

        public NifPreviewVector3 Apply(NifPreviewVector3 position)
        {
            var scaled = new NifPreviewVector3
            {
                X = position.X * Scale,
                Y = position.Y * Scale,
                Z = position.Z * Scale
            };

            return new NifPreviewVector3
            {
                X = (scaled.X * Rotation.M11) + (scaled.Y * Rotation.M21) + (scaled.Z * Rotation.M31) + Translation.X,
                Y = (scaled.X * Rotation.M12) + (scaled.Y * Rotation.M22) + (scaled.Z * Rotation.M32) + Translation.Y,
                Z = (scaled.X * Rotation.M13) + (scaled.Y * Rotation.M23) + (scaled.Z * Rotation.M33) + Translation.Z
            };
        }

        public NifPreviewVector3 ApplyRotation(NifPreviewVector3 vector)
        {
            return new NifPreviewVector3
            {
                X = (vector.X * Rotation.M11) + (vector.Y * Rotation.M21) + (vector.Z * Rotation.M31),
                Y = (vector.X * Rotation.M12) + (vector.Y * Rotation.M22) + (vector.Z * Rotation.M32),
                Z = (vector.X * Rotation.M13) + (vector.Y * Rotation.M23) + (vector.Z * Rotation.M33)
            };
        }
    }

    private readonly struct NifRotation3x3
    {
        public static readonly NifRotation3x3 Identity = new NifRotation3x3
        {
            M11 = 1f,
            M22 = 1f,
            M33 = 1f
        };

        public float M11 { get; init; }

        public float M12 { get; init; }

        public float M13 { get; init; }

        public float M21 { get; init; }

        public float M22 { get; init; }

        public float M23 { get; init; }

        public float M31 { get; init; }

        public float M32 { get; init; }

        public float M33 { get; init; }

        public bool IsReasonable =>
            IsReasonableRotationValue(M11) &&
            IsReasonableRotationValue(M12) &&
            IsReasonableRotationValue(M13) &&
            IsReasonableRotationValue(M21) &&
            IsReasonableRotationValue(M22) &&
            IsReasonableRotationValue(M23) &&
            IsReasonableRotationValue(M31) &&
            IsReasonableRotationValue(M32) &&
            IsReasonableRotationValue(M33);

        public string Description =>
            $"[{M11:0.###},{M12:0.###},{M13:0.###}; {M21:0.###},{M22:0.###},{M23:0.###}; {M31:0.###},{M32:0.###},{M33:0.###}]";

        private static bool IsReasonableRotationValue(float value)
        {
            return !float.IsNaN(value) && !float.IsInfinity(value) && MathF.Abs(value) <= 2f;
        }
    }

    private enum BSTriShapeCountLayout
    {
        Fallout4,
        SkyrimSpecialEdition,
        StarfieldGeometry
    }

    private enum BSVertexPositionFormat
    {
        DescriptorDefault,
        Float3,
        Half3
    }

    private struct NifBinaryReader
    {
        private readonly byte[] Data;

        public NifBinaryReader(byte[] data)
        {
            Data = data;
            Position = 0;
        }

        public int Position { get; private set; }

        public int Remaining => Data.Length - Position;

        public void Seek(int position)
        {
            if (position < 0 || position > Data.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(position));
            }

            Position = position;
        }

        public byte ReadByte()
        {
            Require(sizeof(byte));
            return Data[Position++];
        }

        public ushort ReadUInt16()
        {
            Require(sizeof(ushort));
            var value = BinaryPrimitives.ReadUInt16LittleEndian(Data.AsSpan(Position, sizeof(ushort)));
            Position += sizeof(ushort);
            return value;
        }

        public uint ReadUInt32()
        {
            Require(sizeof(uint));
            var value = BinaryPrimitives.ReadUInt32LittleEndian(Data.AsSpan(Position, sizeof(uint)));
            Position += sizeof(uint);
            return value;
        }

        public string ReadLineString()
        {
            var start = Position;
            while (Position < Data.Length && Data[Position] != 0x0A)
            {
                Position++;
            }

            if (Position >= Data.Length)
            {
                throw new EndOfStreamException("NIF header string was not terminated.");
            }

            var value = Encoding.ASCII.GetString(Data, start, Position - start);
            Position++;
            return value;
        }

        public string ReadExportString()
        {
            return ReadSizedString();
        }

        public string ReadSizedString()
        {
            if (!TryReadSizedString(out var value))
            {
                throw new InvalidDataException("NIF string length is outside the supported range.");
            }

            return value;
        }

        public bool TryReadSizedString(out string value)
        {
            value = string.Empty;
            if (Remaining < sizeof(uint))
            {
                return false;
            }

            var length = ReadUInt32();
            if (length > MaxReasonableStringLength || Remaining < length)
            {
                return false;
            }

            var bytes = ReadBytes((int)length);
            value = Encoding.UTF8.GetString(bytes);
            return true;
        }

        public byte[] ReadBytes(int count)
        {
            Require(count);
            var bytes = Data.AsSpan(Position, count).ToArray();
            Position += count;
            return bytes;
        }

        private void Require(int count)
        {
            if (count < 0 || Remaining < count)
            {
                throw new EndOfStreamException("NIF stream ended before the expected data could be read.");
            }
        }
    }
}
