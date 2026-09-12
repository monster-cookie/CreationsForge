using CreationsForge.Core.Engine.Contracts;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Strings;

namespace CreationsForge.Core.Engine.NativeInspection;

/// <summary>Provides exact typed leaf equality and deterministic semantic change collection helpers.</summary>
public static class NativeSemanticComparer
{
    /// <summary>Compares root record presence and emits the single canonical insertion or removal descriptor.</summary>
    /// <param name="before">The prior detached native record, or <see langword="null"/>.</param>
    /// <param name="after">The resulting detached native record, or <see langword="null"/>.</param>
    /// <param name="changes">The caller-owned ordered descriptor collection.</param>
    /// <param name="cancellationToken">A token checked before collecting a descriptor.</param>
    /// <returns><see langword="true"/> when both records are present and their typed fields must be compared; otherwise <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="changes"/> is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public static bool CompareRecordPresence(
        IMajorRecordGetter? before,
        IMajorRecordGetter? after,
        ICollection<SemanticChangeDescriptor> changes,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(changes);
        cancellationToken.ThrowIfCancellationRequested();
        if (before is null && after is null)
        {
            return false;
        }

        if (before is null)
        {
            changes.Add(new SemanticChangeDescriptor("$record", SemanticChangeKind.ItemInserted));
            return false;
        }

        if (after is null)
        {
            changes.Add(new SemanticChangeDescriptor("$record", SemanticChangeKind.ItemRemoved));
            return false;
        }

        return true;
    }

    /// <summary>Compares single-precision values by their exact IEEE-754 bit patterns.</summary>
    /// <param name="before">The prior native value.</param>
    /// <param name="after">The resulting native value.</param>
    /// <returns><see langword="true"/> when every bit is equal, including signed zero and NaN payload bits.</returns>
    public static bool BitwiseEquals(float before, float after)
    {
        return BitConverter.SingleToUInt32Bits(before) == BitConverter.SingleToUInt32Bits(after);
    }

    /// <summary>Compares double-precision values by their exact IEEE-754 bit patterns.</summary>
    /// <param name="before">The prior native value.</param>
    /// <param name="after">The resulting native value.</param>
    /// <returns><see langword="true"/> when every bit is equal, including signed zero and NaN payload bits.</returns>
    public static bool BitwiseEquals(double before, double after)
    {
        return BitConverter.DoubleToUInt64Bits(before) == BitConverter.DoubleToUInt64Bits(after);
    }

    /// <summary>Compares nullable native strings ordinally while preserving null versus empty.</summary>
    /// <param name="before">The prior native string, or <see langword="null"/>.</param>
    /// <param name="after">The resulting native string, or <see langword="null"/>.</param>
    /// <returns><see langword="true"/> when both strings have the same nullable state and ordinal contents.</returns>
    public static bool StringEquals(string? before, string? after)
    {
        return string.Equals(before, after, StringComparison.Ordinal);
    }

    /// <summary>Compares nullable byte sequences exactly while preserving null versus empty.</summary>
    /// <param name="before">The prior byte sequence, or <see langword="null"/>.</param>
    /// <param name="after">The resulting byte sequence, or <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token observed while comparing sequence positions.</param>
    /// <returns><see langword="true"/> when both sequences have the same nullable state, length, and byte values.</returns>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public static bool BytesEqual(
        IReadOnlyList<byte>? before,
        IReadOnlyList<byte>? after,
        CancellationToken cancellationToken)
    {
        if (before is null || after is null)
        {
            return before is null && after is null;
        }

        if (before.Count != after.Count)
        {
            return false;
        }

        for (var index = 0; index < before.Count; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (before[index] != after[index])
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>Compares nullable native FormKeys without string conversion.</summary>
    /// <param name="before">The prior native identity, or <see langword="null"/>.</param>
    /// <param name="after">The resulting native identity, or <see langword="null"/>.</param>
    /// <returns><see langword="true"/> when both nullable identities are equal.</returns>
    public static bool FormKeyEquals(FormKey? before, FormKey? after)
    {
        return before == after;
    }

    /// <summary>Compares nullable native form links by wrapper presence, null-link state, and FormKey.</summary>
    /// <param name="before">The prior native link, or <see langword="null"/>.</param>
    /// <param name="after">The resulting native link, or <see langword="null"/>.</param>
    /// <returns><see langword="true"/> when wrapper presence, null-link state, and nullable FormKey are equal.</returns>
    public static bool FormLinkEquals(IFormLinkGetter? before, IFormLinkGetter? after)
    {
        if (before is null || after is null)
        {
            return before is null && after is null;
        }

        return before.IsNull == after.IsNull && before.FormKeyNullable == after.FormKeyNullable;
    }

    /// <summary>Compares nullable native form-link-or-index unions by wrapper presence, exact owner-derived mode, and the active physical value.</summary>
    /// <typeparam name="TMajorGetter">The native major-record getter type accepted by the union link.</typeparam>
    /// <param name="before">The prior native union, or <see langword="null"/>.</param>
    /// <param name="after">The resulting native union, or <see langword="null"/>.</param>
    /// <returns><see langword="true"/> when wrapper presence and every mode projection match and the active link or index is equal. In link mode, an absent nullable FormKey and <see cref="FormKey.Null"/> both represent the same native null value.</returns>
    public static bool FormLinkOrIndexEquals<TMajorGetter>(
        IFormLinkOrIndexGetter<TMajorGetter>? before,
        IFormLinkOrIndexGetter<TMajorGetter>? after)
        where TMajorGetter : class, IMajorRecordGetter
    {
        if (before is null || after is null)
        {
            return before is null && after is null;
        }

        if (before.UsesLink() != after.UsesLink()
            || before.UsesAlias() != after.UsesAlias()
            || before.UsesPackageData() != after.UsesPackageData())
        {
            return false;
        }

        if (before.UsesLink())
        {
            return before.Link.FormKey == after.Link.FormKey;
        }

        return before.Index == after.Index;
    }

    /// <summary>Compares nullable native asset links by null state and preserved path values.</summary>
    /// <param name="before">The prior native asset link, or <see langword="null"/>.</param>
    /// <param name="after">The resulting native asset link, or <see langword="null"/>.</param>
    /// <returns><see langword="true"/> when null state, supplied path, normalized data-relative path, and extension are equal.</returns>
    public static bool AssetLinkEquals(IAssetLinkGetter? before, IAssetLinkGetter? after)
    {
        if (before is null || after is null)
        {
            return before is null && after is null;
        }

        return before.IsNull == after.IsNull
            && string.Equals(before.GivenPath, after.GivenPath, StringComparison.Ordinal)
            && string.Equals(before.DataRelativePath.ToString(), after.DataRelativePath.ToString(), StringComparison.Ordinal)
            && string.Equals(before.Extension, after.Extension, StringComparison.Ordinal);
    }

    /// <summary>Compares nullable translated strings by target language, selected value, and exact language-keyed entries.</summary>
    /// <param name="before">The prior native translated string, or <see langword="null"/>.</param>
    /// <param name="after">The resulting native translated string, or <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token observed while enumerating and comparing language entries.</param>
    /// <returns><see langword="true"/> when nullable state, target language, selected value, and every keyed translation are equal.</returns>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public static bool TranslatedStringEquals(
        ITranslatedStringGetter? before,
        ITranslatedStringGetter? after,
        CancellationToken cancellationToken)
    {
        if (before is null || after is null)
        {
            return before is null && after is null;
        }

        if (before.TargetLanguage != after.TargetLanguage
            || !StringEquals(before.String, after.String)
            || before.NumLanguages != after.NumLanguages)
        {
            return false;
        }

        var beforeTranslations = GetOrderedTranslations(before, cancellationToken);
        var afterTranslations = GetOrderedTranslations(after, cancellationToken);
        if (beforeTranslations.Count != afterTranslations.Count)
        {
            return false;
        }

        for (var index = 0; index < beforeTranslations.Count; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (beforeTranslations[index].Key != afterTranslations[index].Key
                || !StringEquals(beforeTranslations[index].Value, afterTranslations[index].Value))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>Adds a scalar change descriptor when a typed equality check fails.</summary>
    /// <param name="fieldIdentifier">The stable typed field path.</param>
    /// <param name="valuesEqual">Whether the typed native values are exactly equal.</param>
    /// <param name="changes">The caller-owned ordered descriptor collection.</param>
    /// <param name="cancellationToken">A token checked before collecting the descriptor.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="fieldIdentifier"/> is empty or whitespace.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="changes"/> is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public static void CompareValue(
        string fieldIdentifier,
        bool valuesEqual,
        ICollection<SemanticChangeDescriptor> changes,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fieldIdentifier);
        ArgumentNullException.ThrowIfNull(changes);
        cancellationToken.ThrowIfCancellationRequested();
        if (!valuesEqual)
        {
            changes.Add(new SemanticChangeDescriptor(fieldIdentifier, SemanticChangeKind.ValueChanged));
        }
    }

    /// <summary>Copies keyed translations into deterministic numeric language order for one comparison.</summary>
    /// <param name="value">The native translated string to enumerate.</param>
    /// <param name="cancellationToken">A token observed while copying and sorting entries.</param>
    /// <returns>A temporary ordered snapshot that does not retain native state.</returns>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    private static IReadOnlyList<KeyValuePair<Language, string>> GetOrderedTranslations(
        ITranslatedStringGetter value,
        CancellationToken cancellationToken)
    {
        var translations = new List<KeyValuePair<Language, string>>(value.NumLanguages);
        foreach (var translation in value)
        {
            cancellationToken.ThrowIfCancellationRequested();
            translations.Add(translation);
        }

        cancellationToken.ThrowIfCancellationRequested();
        translations.Sort(static (left, right) => ((int)left.Key).CompareTo((int)right.Key));
        return translations;
    }

    /// <summary>Compares nullable ordered collections without collapsing null, order, or duplicate entries.</summary>
    /// <typeparam name="T">The native element type.</typeparam>
    /// <param name="fieldIdentifier">The stable typed collection field path.</param>
    /// <param name="before">The prior ordered collection, or <see langword="null"/>.</param>
    /// <param name="after">The resulting ordered collection, or <see langword="null"/>.</param>
    /// <param name="equals">The complete typed element equality operation.</param>
    /// <param name="changes">The caller-owned ordered descriptor collection.</param>
    /// <param name="cancellationToken">A token observed for every collection position.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="fieldIdentifier"/> is empty or whitespace.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="equals"/> or <paramref name="changes"/> is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public static void CompareOrdered<T>(
        string fieldIdentifier,
        IReadOnlyList<T>? before,
        IReadOnlyList<T>? after,
        Func<T, T, bool> equals,
        ICollection<SemanticChangeDescriptor> changes,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fieldIdentifier);
        ArgumentNullException.ThrowIfNull(equals);
        ArgumentNullException.ThrowIfNull(changes);
        cancellationToken.ThrowIfCancellationRequested();
        if (before is null || after is null)
        {
            if (before is null != (after is null))
            {
                changes.Add(new SemanticChangeDescriptor(fieldIdentifier, SemanticChangeKind.ValueChanged));
            }

            return;
        }

        var commonCount = Math.Min(before.Count, after.Count);
        for (var index = 0; index < commonCount; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!equals(before[index], after[index]))
            {
                changes.Add(new SemanticChangeDescriptor(fieldIdentifier, SemanticChangeKind.ItemChanged, index, index));
            }
        }

        for (var index = commonCount; index < before.Count; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            changes.Add(new SemanticChangeDescriptor(fieldIdentifier, SemanticChangeKind.ItemRemoved, index, null));
        }

        for (var index = commonCount; index < after.Count; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            changes.Add(new SemanticChangeDescriptor(fieldIdentifier, SemanticChangeKind.ItemInserted, null, index));
        }
    }
}
