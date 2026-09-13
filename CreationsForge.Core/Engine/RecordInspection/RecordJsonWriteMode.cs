namespace CreationsForge.Core.Engine.RecordInspection;

/// <summary>Identifies whether record JSON is intended for a readable view or a versioned canonical fingerprint.</summary>
public enum RecordJsonWriteMode
{
    /// <summary>Preserve the existing detached read-view JSON representation.</summary>
    ReadView,

    /// <summary>Write the first version of the lossless canonical fingerprint representation.</summary>
    CanonicalFingerprintV1
}
