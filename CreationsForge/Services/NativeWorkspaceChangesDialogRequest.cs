namespace CreationsForge.Services;

/// <summary>Describes one valid closed purpose for the shared workspace-changes dialog.</summary>
public sealed class NativeWorkspaceChangesDialogRequest
{
    /// <summary>Initializes one validated dialog request.</summary>
    /// <param name="purpose">The closed dialog purpose.</param>
    /// <param name="leaveReason">The leave reason supplied only for a leave request.</param>
    private NativeWorkspaceChangesDialogRequest(
        NativeWorkspaceChangesDialogPurpose purpose,
        NativeWorkspaceLeaveReason? leaveReason)
    {
        Purpose = purpose;
        LeaveReason = leaveReason;
    }

    /// <summary>Gets the closed workflow hosted by the dialog.</summary>
    public NativeWorkspaceChangesDialogPurpose Purpose { get; }

    /// <summary>Gets the guarded application transition, or <see langword="null"/> outside a leave workflow.</summary>
    public NativeWorkspaceLeaveReason? LeaveReason { get; }

    /// <summary>Creates a request that reviews current request-local and workspace-staged changes.</summary>
    /// <returns>A review dialog request.</returns>
    public static NativeWorkspaceChangesDialogRequest ForReview()
    {
        return new NativeWorkspaceChangesDialogRequest(NativeWorkspaceChangesDialogPurpose.Review, leaveReason: null);
    }

    /// <summary>Creates a request that reviews and saves workspace-staged changes.</summary>
    /// <returns>A save dialog request.</returns>
    public static NativeWorkspaceChangesDialogRequest ForSave()
    {
        return new NativeWorkspaceChangesDialogRequest(NativeWorkspaceChangesDialogPurpose.Save, leaveReason: null);
    }

    /// <summary>Creates a request that reviews and discards request-local or workspace-staged changes.</summary>
    /// <returns>A discard dialog request.</returns>
    public static NativeWorkspaceChangesDialogRequest ForDiscard()
    {
        return new NativeWorkspaceChangesDialogRequest(NativeWorkspaceChangesDialogPurpose.Discard, leaveReason: null);
    }

    /// <summary>Creates a request that resolves pending work before one named application transition.</summary>
    /// <param name="reason">The exact transition that remains blocked while the dialog is open.</param>
    /// <returns>A guarded leave dialog request.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="reason"/> is not a defined leave reason.</exception>
    public static NativeWorkspaceChangesDialogRequest ForLeave(NativeWorkspaceLeaveReason reason)
    {
        if (!Enum.IsDefined(reason))
        {
            throw new ArgumentOutOfRangeException(nameof(reason));
        }

        return new NativeWorkspaceChangesDialogRequest(NativeWorkspaceChangesDialogPurpose.Leave, reason);
    }
}
