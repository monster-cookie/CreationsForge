using System.ComponentModel;
using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeWire;
using CreationsForge.Services;
using Mutagen.Bethesda.Plugins;

namespace CreationsForge.NativeEditing.Drafts;

/// <summary>Identifies stable local draft validation and mapping failures.</summary>
public enum NativeWireDraftIssueCode
{
    /// <summary>A required value has no current explicit value.</summary>
    RequiredValueUnset,
    /// <summary>A value has the wrong typed shape.</summary>
    InvalidValue,
    /// <summary>A numeric value is not canonical or is outside its allowed range.</summary>
    NumericValueOutOfRange,
    /// <summary>A string violates length, pattern, or format constraints.</summary>
    StringConstraintViolated,
    /// <summary>A collection violates an element, uniqueness, or shape constraint.</summary>
    CollectionConstraintViolated,
    /// <summary>An exact schema reference cannot be resolved safely.</summary>
    SchemaResolutionFailed,
    /// <summary>An operation exceeds its bounded traversal or allocation policy.</summary>
    ResourceLimitExceeded,
    /// <summary>A codec or engine error was mapped back to an exact command path.</summary>
    CodecRejected,
}
