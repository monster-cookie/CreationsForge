using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.RecordWire;

namespace CreationsForge.RecordEditing.Schema;

/// <summary>Identifies the closed presentation shape of one resolved record wire schema value.</summary>
public enum RecordWireSchemaValueKind
{
    /// <summary>A closed JSON object.</summary>
    Object,
    /// <summary>An ordered JSON array.</summary>
    Array,
    /// <summary>A lazily selected schema union.</summary>
    Union,
    /// <summary>A value whose whole identity may be JSON null.</summary>
    Nullable,
    /// <summary>A JSON Boolean.</summary>
    Boolean,
    /// <summary>A bounded JSON integer.</summary>
    Integer,
    /// <summary>A bounded JSON string.</summary>
    String,
    /// <summary>An integer with known symbolic values and preserved unknown values.</summary>
    Enum,
    /// <summary>A canonical record FormLink representation.</summary>
    FormLink,
    /// <summary>An owner-mode-sensitive FormLink-or-index representation.</summary>
    FormLinkOrIndex,
    /// <summary>An exact floating-point bit representation.</summary>
    FloatBits,
    /// <summary>A Base64-encoded record byte sequence.</summary>
    ByteArray,
    /// <summary>A translated record string representation.</summary>
    TranslatedString,
    /// <summary>A record asset-path representation.</summary>
    Asset,
    /// <summary>A record color representation.</summary>
    Color,
    /// <summary>A shape-preserving record two-dimensional array.</summary>
    Array2D,
}
