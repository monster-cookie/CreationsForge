namespace CreationsForge.UnitTests.Engine.Integration;

/// <summary>Identifies the recognized prior-save journal and physical destination state created for cross-owner integration tests.</summary>
public enum WorkspaceRecoveryPhysicalState
{
    /// <summary>The nonterminal Preparing journal retains the exact complete original destination.</summary>
    PreparingAllBefore,

    /// <summary>The nonterminal Prepared journal is paired with the exact complete prepared destination.</summary>
    PreparedAllPrepared,

    /// <summary>The MutationStarted journal is paired with a destination containing both original and prepared artifacts.</summary>
    MutationStartedMixed,
}
