using CreationsForge.Core.Engine.Contracts;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;

namespace CreationsForge.Core.Engine.RecordInspection;

/// <summary>Verifies complete native field equality for every major record in two plugin outputs.</summary>
public static class MajorRecordSetComparer
{
    /// <summary>Compares unique record identities and all installed typed fields without depending on group enumeration order.</summary>
    /// <param name="left">The expected complete major-record set.</param>
    /// <param name="right">The actual complete major-record set.</param>
    /// <param name="inspector">The exact game's native field inspector.</param>
    /// <param name="cancellationToken">A token observed during enumeration and field traversal.</param>
    /// <returns><see langword="true"/> only when both sets have identical unique families, identities, and native values.</returns>
    /// <exception cref="ArgumentNullException">Thrown when a record set or inspector is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown when comparison is canceled.</exception>
    /// <exception cref="NotSupportedException">Thrown when an installed native field cannot be compared losslessly.</exception>
    public static bool AreEqual(
        IEnumerable<IMajorRecordGetter> left,
        IEnumerable<IMajorRecordGetter> right,
        IMajorRecordInspector inspector,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);
        ArgumentNullException.ThrowIfNull(inspector);

        var actualByKey = new Dictionary<FormKey, IMajorRecordGetter>();
        foreach (var record in right)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!actualByKey.TryAdd(record.FormKey, record))
            {
                return false;
            }
        }

        var expectedKeys = new HashSet<FormKey>();
        foreach (var record in left)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!expectedKeys.Add(record.FormKey)
                || !actualByKey.TryGetValue(record.FormKey, out var actual)
                || record.GetType() != actual.GetType()
                || inspector.Compare(record, actual, cancellationToken).Count != 0)
            {
                return false;
            }
        }

        return expectedKeys.Count == actualByKey.Count;
    }
}
