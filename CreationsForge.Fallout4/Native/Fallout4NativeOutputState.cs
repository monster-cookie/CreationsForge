using CreationsForge.Core.Engine.Contracts;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Plugins.Records;

namespace CreationsForge.Fallout4.Native;

/// <summary>
/// Owns one complete mutable Fallout 4 output and its independent native state at output selection.
/// </summary>
public sealed class Fallout4NativeOutputState : INativeOutputState
{
    /// <summary>The complete mutable native output, including unrelated record families.</summary>
    private readonly Fallout4Mod _mod;

    /// <summary>The independent complete native snapshot captured when the output was selected.</summary>
    private readonly Fallout4Mod _originalMod;

    /// <summary>The ordered first-baseline metadata for FormLists edited in this staged output.</summary>
    private readonly List<NativeEditProvenance> _editProvenance;

    /// <summary>Tracks whether the output state has been disposed.</summary>
    private int _disposed;

    /// <summary>Initializes an independently owned complete Fallout 4 output state.</summary>
    /// <param name="mod">The complete mutable native output owned by this state.</param>
    /// <param name="originalMod">The independent complete native state captured at selection.</param>
    /// <param name="association">The exact output identity and formatting selection.</param>
    /// <param name="baseline">The complete artifact baseline observed during selection.</param>
    /// <param name="editProvenance">The ordered staged-edit provenance to copy, or <see langword="null"/> for a newly selected output.</param>
    /// <exception cref="ArgumentNullException">Thrown when any argument is <see langword="null"/>.</exception>
    internal Fallout4NativeOutputState(
        Fallout4Mod mod,
        Fallout4Mod originalMod,
        OutputAssociation association,
        OutputArtifactSetBaseline baseline,
        IReadOnlyList<NativeEditProvenance>? editProvenance = null)
    {
        ArgumentNullException.ThrowIfNull(mod);
        ArgumentNullException.ThrowIfNull(originalMod);
        ArgumentNullException.ThrowIfNull(association);
        ArgumentNullException.ThrowIfNull(baseline);
        _mod = mod;
        _originalMod = originalMod;
        _editProvenance = new List<NativeEditProvenance>(editProvenance ?? Array.Empty<NativeEditProvenance>());
        Association = association;
        Baseline = baseline;
    }

    /// <summary>Gets the exact selected output identity and native formatting choices.</summary>
    public OutputAssociation Association { get; }

    /// <summary>Gets the immutable plugin-and-strings baseline observed during selection.</summary>
    public OutputArtifactSetBaseline Baseline { get; }

    /// <summary>Creates a defensive complete native copy of the current staged output.</summary>
    /// <param name="cancellationToken">A token checked immediately before and after the native copy.</param>
    /// <returns>A complete independent mutable copy suitable for native preservation inspection.</returns>
    /// <exception cref="ObjectDisposedException">Thrown after this output state has been disposed.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public Fallout4Mod CreateSnapshot(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        cancellationToken.ThrowIfCancellationRequested();
        var snapshot = CopyMod(_mod);
        cancellationToken.ThrowIfCancellationRequested();
        return snapshot;
    }

    /// <summary>Creates a defensive complete native copy of the output state captured at selection.</summary>
    /// <param name="cancellationToken">A token checked immediately before and after the native copy.</param>
    /// <returns>A complete independent mutable copy of the original selected output.</returns>
    /// <exception cref="ObjectDisposedException">Thrown after this output state has been disposed.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public Fallout4Mod CreateOriginalSnapshot(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        cancellationToken.ThrowIfCancellationRequested();
        var snapshot = CopyMod(_originalMod);
        cancellationToken.ThrowIfCancellationRequested();
        return snapshot;
    }

    /// <summary>Releases this output owner and closes subsequent access to its native state.</summary>
    /// <returns>A completed disposal operation.</returns>
    public ValueTask DisposeAsync()
    {
        Interlocked.Exchange(ref _disposed, 1);
        return ValueTask.CompletedTask;
    }

    /// <summary>Borrows the complete mutable output for same-assembly transactional operations.</summary>
    /// <returns>The live mutable native output owned by this state.</returns>
    /// <exception cref="ObjectDisposedException">Thrown after this output state has been disposed.</exception>
    internal Fallout4Mod BorrowMod()
    {
        ThrowIfDisposed();
        return _mod;
    }

    /// <summary>Borrows the immutable-by-convention original native state for same-assembly comparison.</summary>
    /// <returns>The live original native snapshot owned by this state.</returns>
    /// <exception cref="ObjectDisposedException">Thrown after this output state has been disposed.</exception>
    internal IFallout4ModGetter BorrowOriginalMod()
    {
        ThrowIfDisposed();
        return _originalMod;
    }

    /// <summary>Captures a target's first staged-edit baseline without replacing existing provenance.</summary>
    /// <param name="provenance">The complete immutable metadata for the target's first edit session.</param>
    /// <returns><see langword="true"/> when the target was newly recorded; otherwise <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="provenance"/> is <see langword="null"/>.</exception>
    /// <exception cref="ObjectDisposedException">Thrown after this output state has been disposed.</exception>
    internal bool TryAddEditProvenance(NativeEditProvenance provenance)
    {
        ArgumentNullException.ThrowIfNull(provenance);
        ThrowIfDisposed();
        if (_editProvenance.Any(existing => existing.TargetFormKey == provenance.TargetFormKey))
        {
            return false;
        }

        _editProvenance.Add(provenance);
        return true;
    }

    /// <summary>Creates an immutable ordered snapshot of staged-edit provenance for cloning and preview.</summary>
    /// <returns>The current metadata entries in first-edit order.</returns>
    /// <exception cref="ObjectDisposedException">Thrown after this output state has been disposed.</exception>
    internal IReadOnlyList<NativeEditProvenance> GetEditProvenance()
    {
        ThrowIfDisposed();
        return Array.AsReadOnly(_editProvenance.ToArray());
    }

    /// <summary>Creates a complete mutable native copy without retaining the source object.</summary>
    /// <param name="mod">The complete native getter to copy.</param>
    /// <returns>An independent complete mutable Fallout 4 mod.</returns>
    private static Fallout4Mod CopyMod(IFallout4ModGetter mod)
    {
        return (Fallout4Mod)((IModGetter)mod).DeepCopy();
    }

    /// <summary>Rejects access after the output state has been disposed.</summary>
    /// <exception cref="ObjectDisposedException">Thrown after this output state has been disposed.</exception>
    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) != 0, this);
    }
}
