using CreationsForge.Core.Engine.Contracts;

namespace CreationsForge.UnitTests.Engine.Foundation;

/// <summary>
/// Provides an immutable synthetic prepared edit for workspace orchestration tests.
/// </summary>
internal sealed class TestPreparedFormListEdit : PreparedFormListEdit
{
    /// <summary>Initializes a valid or rejected synthetic prepared edit.</summary>
    /// <param name="canonicalPayload">The deterministic synthetic canonical payload.</param>
    /// <param name="error">An optional deterministic preparation failure.</param>
    internal TestPreparedFormListEdit(string canonicalPayload, EngineError? error = null)
        : base(OperationFingerprint.Create(System.Text.Encoding.UTF8.GetBytes(canonicalPayload)), error)
    { }

    /// <summary>Initializes a synthetic prepared edit from an already computed complete fingerprint.</summary>
    /// <param name="fingerprint">The complete canonical payload identity.</param>
    /// <param name="error">An optional deterministic preparation failure.</param>
    internal TestPreparedFormListEdit(OperationFingerprint fingerprint, EngineError? error = null)
        : base(fingerprint, error)
    { }
}
