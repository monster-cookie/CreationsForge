using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeWire;

namespace CreationsForge.NativeEditing.Schema;

/// <summary>Identifies the closed presentation shape of one resolved native wire schema value.</summary>
public enum NativeWireSchemaValueKind
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
    /// <summary>A canonical native FormLink representation.</summary>
    FormLink,
    /// <summary>An owner-mode-sensitive FormLink-or-index representation.</summary>
    FormLinkOrIndex,
    /// <summary>An exact floating-point bit representation.</summary>
    FloatBits,
    /// <summary>A Base64-encoded native byte sequence.</summary>
    ByteArray,
    /// <summary>A translated native string representation.</summary>
    TranslatedString,
    /// <summary>A native asset-path representation.</summary>
    Asset,
    /// <summary>A native color representation.</summary>
    Color,
    /// <summary>A shape-preserving native two-dimensional array.</summary>
    Array2D,
}
