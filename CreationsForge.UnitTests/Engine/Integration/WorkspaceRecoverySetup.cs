using CreationsForge.Core.Engine.Contracts;
using Mutagen.Bethesda.Plugins;

namespace CreationsForge.UnitTests.Engine.Integration;

/// <summary>Describes the committed original output immediately before a prior transaction is created.</summary>
internal sealed class WorkspaceRecoverySetup
{
    /// <summary>Initializes immutable pre-transaction state for a caller that must open another owner first.</summary>
    /// <param name="output">The exact complete output association.</param>
    /// <param name="formKey">The output FormList identity modified by the transaction.</param>
    /// <param name="beforeBaseline">The complete original output baseline.</param>
    /// <param name="beforeEditorId">The EditorID stored in the original output.</param>
    /// <param name="preparedEditorId">The EditorID intended by the prior transaction, or <see langword="null"/> when no prepared set is needed.</param>
    internal WorkspaceRecoverySetup(
        OutputAssociation output,
        FormKey formKey,
        OutputArtifactSetBaseline beforeBaseline,
        string beforeEditorId,
        string? preparedEditorId)
    {
        Output = output;
        FormKey = formKey;
        BeforeBaseline = beforeBaseline;
        BeforeEditorId = beforeEditorId;
        PreparedEditorId = preparedEditorId;
    }

    /// <summary>Gets the exact complete output association.</summary>
    internal OutputAssociation Output { get; }

    /// <summary>Gets the output FormList identity modified by the transaction.</summary>
    internal FormKey FormKey { get; }

    /// <summary>Gets the complete original output baseline.</summary>
    internal OutputArtifactSetBaseline BeforeBaseline { get; }

    /// <summary>Gets the EditorID stored in the original output.</summary>
    internal string BeforeEditorId { get; }

    /// <summary>Gets the EditorID intended by the prior transaction, or <see langword="null"/> when no prepared set is needed.</summary>
    internal string? PreparedEditorId { get; }
}
