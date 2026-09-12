using CreationsForge.Core.Engine.Contracts;
using Mutagen.Bethesda.Skyrim;

namespace CreationsForge.Skyrim.Native;

/// <summary>
/// Owns one complete mutable Skyrim output and an independent native baseline captured when the output was selected.
/// </summary>
public sealed class SkyrimNativeOutputState : INativeOutputState
{
    /// <summary>The complete mutable Skyrim output, or <see langword="null"/> after disposal.</summary>
    private SkyrimMod? _mod;

    /// <summary>The independent complete native baseline, or <see langword="null"/> after disposal.</summary>
    private SkyrimMod? _original;

    /// <summary>The ordered first-edit provenance for every FormList staged in this output.</summary>
    private readonly List<NativeEditProvenance> _editProvenance;

    /// <summary>Tracks whether this independently owned native state has been released.</summary>
    private int _disposed;

    /// <summary>
    /// Initializes one independently owned complete Skyrim output state.
    /// </summary>
    /// <param name="mod">The complete mutable output.</param>
    /// <param name="original">An independent complete native baseline for preview and preservation checks.</param>
    /// <param name="association">The canonical selected output association.</param>
    /// <param name="baseline">The complete physical output artifact baseline.</param>
    /// <param name="editProvenance">Optional ordered staged-edit provenance copied from a prior candidate.</param>
    /// <exception cref="ArgumentException">Thrown when a native mod identity disagrees with the selected output or the native instances are aliased.</exception>
    /// <exception cref="ArgumentNullException">Thrown when any argument is <see langword="null"/>.</exception>
    internal SkyrimNativeOutputState(
        SkyrimMod mod,
        SkyrimMod original,
        OutputAssociation association,
        OutputArtifactSetBaseline baseline,
        IReadOnlyList<NativeEditProvenance>? editProvenance = null)
    {
        ArgumentNullException.ThrowIfNull(mod);
        ArgumentNullException.ThrowIfNull(original);
        ArgumentNullException.ThrowIfNull(association);
        ArgumentNullException.ThrowIfNull(baseline);
        if (ReferenceEquals(mod, original))
        {
            throw new ArgumentException("A Skyrim output requires an independent original native baseline.", nameof(original));
        }

        if (mod.ModKey != association.ModKey || original.ModKey != association.ModKey)
        {
            throw new ArgumentException("The complete Skyrim output and original baseline must match the selected output ModKey.", nameof(mod));
        }

        _mod = mod;
        _original = original;
        Association = association;
        Baseline = baseline;
        _editProvenance = editProvenance?.ToList() ?? new List<NativeEditProvenance>();
    }

    /// <summary>Gets the canonical selected output association.</summary>
    public OutputAssociation Association { get; }

    /// <summary>Gets the immutable complete physical output artifact baseline.</summary>
    public OutputArtifactSetBaseline Baseline { get; }

    /// <summary>
    /// Creates a defensive complete native copy of the current staged output for inspection.
    /// </summary>
    /// <param name="cancellationToken">A token checked immediately before and after the uninterruptible native copy.</param>
    /// <returns>An independently mutable Skyrim mod whose changes cannot affect this output state.</returns>
    /// <exception cref="ObjectDisposedException">Thrown after this output state has been disposed.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public SkyrimMod CreateSnapshot(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return CopyMod(GetMod(), cancellationToken);
    }

    /// <summary>
    /// Creates a defensive complete native copy of the output state captured when selection completed.
    /// </summary>
    /// <param name="cancellationToken">A token checked immediately before and after the uninterruptible native copy.</param>
    /// <returns>An independently mutable Skyrim mod representing the selected output before staged edits.</returns>
    /// <exception cref="ObjectDisposedException">Thrown after this output state has been disposed.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public SkyrimMod CreateOriginalSnapshot(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return CopyMod(GetOriginal(), cancellationToken);
    }

    /// <summary>
    /// Creates an independently owned complete copy for transactional candidate mutation.
    /// </summary>
    /// <param name="cancellationToken">A token checked around each uninterruptible native deep copy.</param>
    /// <returns>A candidate with independent current and original native state.</returns>
    /// <exception cref="ObjectDisposedException">Thrown after this output state has been disposed.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    internal SkyrimNativeOutputState Clone(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var mod = CopyMod(GetMod(), cancellationToken);
        var original = CopyMod(GetOriginal(), cancellationToken);
        return new SkyrimNativeOutputState(
            mod,
            original,
            Association,
            Baseline,
            _editProvenance);
    }

    /// <summary>Records the immutable first-edit baseline for one target without replacing existing provenance.</summary>
    /// <param name="provenance">The exact absent, original-output, or source-context baseline metadata.</param>
    /// <returns><see langword="true"/> when a new target entry was recorded; otherwise <see langword="false"/> when the target already had provenance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="provenance"/> is <see langword="null"/>.</exception>
    /// <exception cref="ObjectDisposedException">Thrown after this output state has been disposed.</exception>
    internal bool RegisterEditProvenance(NativeEditProvenance provenance)
    {
        ArgumentNullException.ThrowIfNull(provenance);
        GetMod();
        if (_editProvenance.Any(existing => existing.TargetFormKey == provenance.TargetFormKey))
        {
            return false;
        }

        _editProvenance.Add(provenance);
        return true;
    }

    /// <summary>Creates an immutable snapshot of ordered first-edit provenance for preview.</summary>
    /// <returns>The ordered provenance entries currently owned by this output.</returns>
    /// <exception cref="ObjectDisposedException">Thrown after this output state has been disposed.</exception>
    internal IReadOnlyList<NativeEditProvenance> GetEditProvenance()
    {
        GetMod();
        return Array.AsReadOnly(_editProvenance.ToArray());
    }

    /// <summary>
    /// Borrows the complete mutable mod for synchronous same-assembly adapter operations.
    /// </summary>
    /// <returns>The live mutable native output owned by this state.</returns>
    /// <exception cref="ObjectDisposedException">Thrown after this output state has been disposed.</exception>
    internal SkyrimMod GetMutableMod()
    {
        return GetMod();
    }

    /// <summary>
    /// Borrows the complete original native baseline for synchronous same-assembly adapter operations.
    /// </summary>
    /// <returns>The live original native baseline owned by this state.</returns>
    /// <exception cref="ObjectDisposedException">Thrown after this output state has been disposed.</exception>
    internal ISkyrimModGetter GetOriginalMod()
    {
        return GetOriginal();
    }

    /// <summary>
    /// Releases references to this independently owned materialized native state.
    /// </summary>
    /// <returns>A completed value task because materialized Skyrim mods retain no open parsing handles.</returns>
    public ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _disposed, 1) == 0)
        {
            _mod = null;
            _original = null;
            _editProvenance.Clear();
        }

        return ValueTask.CompletedTask;
    }

    /// <summary>Gets the current native mod or throws after disposal.</summary>
    /// <returns>The complete mutable output.</returns>
    /// <exception cref="ObjectDisposedException">Thrown after this output state has been disposed.</exception>
    private SkyrimMod GetMod()
    {
        var mod = Volatile.Read(ref _mod);
        ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) != 0 || mod is null, this);
        return mod;
    }

    /// <summary>Gets the original native mod or throws after disposal.</summary>
    /// <returns>The complete original output baseline.</returns>
    /// <exception cref="ObjectDisposedException">Thrown after this output state has been disposed.</exception>
    private SkyrimMod GetOriginal()
    {
        var original = Volatile.Read(ref _original);
        ObjectDisposedException.ThrowIf(Volatile.Read(ref _disposed) != 0 || original is null, this);
        return original;
    }

    /// <summary>Copies every reachable native field into an independent mutable Skyrim mod.</summary>
    /// <param name="source">The complete native getter to copy.</param>
    /// <param name="cancellationToken">A token checked before and after the synchronous copy.</param>
    /// <returns>The independent complete mutable copy.</returns>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    private static SkyrimMod CopyMod(ISkyrimModGetter source, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var copy = new SkyrimMod(source.ModKey, SkyrimRelease.SkyrimSE);
        SkyrimModMixIn.DeepCopyIn(copy, source);
        cancellationToken.ThrowIfCancellationRequested();
        return copy;
    }
}
