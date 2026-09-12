using CreationsForge.Core.Engine.Contracts;
using Mutagen.Bethesda.Starfield;

namespace CreationsForge.Starfield.Native;

/// <summary>
/// Owns one complete mutable Starfield output and its independent native baseline without retaining parsing resources.
/// </summary>
public sealed class StarfieldNativeOutputState : INativeOutputState
{
    /// <summary>The complete mutable staged output, or <see langword="null"/> after disposal.</summary>
    private StarfieldMod? MutableMod;

    /// <summary>The complete independently copied output as it appeared when selected, or <see langword="null"/> after disposal.</summary>
    private StarfieldMod? OriginalMod;

    /// <summary>The ordered first-baseline metadata for every FormList touched in the staged output.</summary>
    private List<NativeEditProvenance>? Provenance;

    /// <summary>Tracks whether this independently owned state has been disposed.</summary>
    private int IsDisposed;

    /// <summary>
    /// Initializes a complete Starfield output state after native admission and artifact verification succeed.
    /// </summary>
    /// <param name="mod">The independently owned mutable output.</param>
    /// <param name="originalMod">The independently owned complete original output baseline.</param>
    /// <param name="association">The exact selected output identity and formatting choices.</param>
    /// <param name="baseline">The verified complete output artifact baseline.</param>
    /// <param name="provenance">Optional ordered provenance copied from an earlier unpublished candidate.</param>
    /// <exception cref="ArgumentNullException">Thrown when a required value is <see langword="null"/>.</exception>
    internal StarfieldNativeOutputState(
        StarfieldMod mod,
        StarfieldMod originalMod,
        OutputAssociation association,
        OutputArtifactSetBaseline baseline,
        IReadOnlyList<NativeEditProvenance>? provenance = null)
    {
        ArgumentNullException.ThrowIfNull(mod);
        ArgumentNullException.ThrowIfNull(originalMod);
        ArgumentNullException.ThrowIfNull(association);
        ArgumentNullException.ThrowIfNull(baseline);
        if (mod.ModKey != association.ModKey || originalMod.ModKey != association.ModKey)
        {
            throw new ArgumentException("The native output state must retain the selected output ModKey.", nameof(mod));
        }

        MutableMod = mod;
        OriginalMod = originalMod;
        Provenance = provenance?.ToList() ?? new List<NativeEditProvenance>();
        Association = association;
        Baseline = baseline;
    }

    /// <summary>Gets the exact output identity and native formatting choices.</summary>
    public OutputAssociation Association { get; }

    /// <summary>Gets the immutable complete artifact observation captured when the output was selected.</summary>
    public OutputArtifactSetBaseline Baseline { get; }

    /// <summary>
    /// Creates a complete detached native snapshot of the current staged output.
    /// </summary>
    /// <param name="cancellationToken">A token checked immediately before and after the uninterruptible native copy.</param>
    /// <returns>An independent read-only-surface Starfield snapshot containing every record and header field.</returns>
    /// <exception cref="ObjectDisposedException">Thrown after this state is disposed.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public IStarfieldModGetter CreateSnapshot(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var copy = BorrowMod().DeepCopy();
        cancellationToken.ThrowIfCancellationRequested();
        return copy;
    }

    /// <summary>Creates a complete detached native snapshot of the output as it appeared when selected.</summary>
    /// <param name="cancellationToken">A token checked immediately before and after the uninterruptible native copy.</param>
    /// <returns>An independent mutable Starfield snapshot containing every original record and header field.</returns>
    /// <exception cref="ObjectDisposedException">Thrown after this state is disposed.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    internal StarfieldMod CreateOriginalSnapshot(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var copy = (StarfieldMod)BorrowOriginalMod().DeepCopy();
        cancellationToken.ThrowIfCancellationRequested();
        return copy;
    }

    /// <summary>Gets the complete mutable native output for same-assembly adapter operations.</summary>
    /// <returns>The live mutable output owned by this state.</returns>
    /// <exception cref="ObjectDisposedException">Thrown after this state is disposed.</exception>
    internal StarfieldMod BorrowMod()
    {
        var mod = Volatile.Read(ref MutableMod);
        ObjectDisposedException.ThrowIf(Volatile.Read(ref IsDisposed) != 0 || mod is null, this);
        return mod;
    }

    /// <summary>Gets the independently owned complete native output baseline for same-assembly comparisons.</summary>
    /// <returns>The immutable-by-convention original native output.</returns>
    /// <exception cref="ObjectDisposedException">Thrown after this state is disposed.</exception>
    internal IStarfieldModGetter BorrowOriginalMod()
    {
        var mod = Volatile.Read(ref OriginalMod);
        ObjectDisposedException.ThrowIf(Volatile.Read(ref IsDisposed) != 0 || mod is null, this);
        return mod;
    }

    /// <summary>Gets the ordered immutable-by-convention first-baseline metadata owned by this candidate.</summary>
    /// <returns>A read-only view over the provenance retained by this state.</returns>
    /// <exception cref="ObjectDisposedException">Thrown after this state is disposed.</exception>
    internal IReadOnlyList<NativeEditProvenance> BorrowProvenance()
    {
        var provenance = Volatile.Read(ref Provenance);
        ObjectDisposedException.ThrowIf(Volatile.Read(ref IsDisposed) != 0 || provenance is null, this);
        return provenance.AsReadOnly();
    }

    /// <summary>Creates an immutable ordered snapshot of every retained first-edit baseline.</summary>
    /// <returns>The current provenance entries in first-edit order.</returns>
    /// <exception cref="ObjectDisposedException">Thrown after this state is disposed.</exception>
    internal IReadOnlyList<NativeEditProvenance> GetEditProvenance()
    {
        return Array.AsReadOnly(BorrowProvenance().ToArray());
    }

    /// <summary>Records one target's first edit baseline while preserving any baseline already captured for that target.</summary>
    /// <param name="provenance">The complete immutable baseline metadata to add when the target has not been edited before.</param>
    /// <returns><see langword="true"/> when the baseline was added; <see langword="false"/> when the target already had a baseline.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="provenance"/> is <see langword="null"/>.</exception>
    /// <exception cref="ObjectDisposedException">Thrown after this state is disposed.</exception>
    internal bool AddProvenanceIfAbsent(NativeEditProvenance provenance)
    {
        ArgumentNullException.ThrowIfNull(provenance);
        var entries = Volatile.Read(ref Provenance);
        ObjectDisposedException.ThrowIf(Volatile.Read(ref IsDisposed) != 0 || entries is null, this);
        if (entries.Any(entry => entry.TargetFormKey == provenance.TargetFormKey))
        {
            return false;
        }

        entries.Add(provenance);
        return true;
    }

    /// <summary>Releases the complete native records retained by this independently owned output state.</summary>
    /// <returns>A completed task because this state owns no open streams or unmanaged resources.</returns>
    public ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref IsDisposed, 1) != 0)
        {
            return ValueTask.CompletedTask;
        }

        MutableMod = null;
        OriginalMod = null;
        Provenance?.Clear();
        Provenance = null;
        return ValueTask.CompletedTask;
    }
}
