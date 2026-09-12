using System.Text.Json;
using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Media;

namespace CreationsForge.ViewModels;

/// <summary>Supplies detached JSON pairing and comparison-state presentation for the native FormList browser.</summary>
public sealed partial class NativeFormListBrowserViewModel
{
    /// <summary>Pairs detached JSON projections and assigns presentation-only comparison states.</summary>
    /// <param name="beforeFields">The prior JSON projection.</param>
    /// <param name="afterFields">The resulting JSON projection.</param>
    /// <param name="resultIsWinningOverride">Whether the resulting selection explicitly requests winning overrides.</param>
    /// <param name="cancellationToken">A token observed throughout projection pairing.</param>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    private static void ApplyComparisonStates(
        IReadOnlyList<NativeJsonFieldNodeViewModel> beforeFields,
        IReadOnlyList<NativeJsonFieldNodeViewModel> afterFields,
        bool resultIsWinningOverride,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var pairCount = Math.Min(beforeFields.Count, afterFields.Count);
        for (var index = 0; index < pairCount; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ApplyComparisonState(
                beforeFields[index],
                afterFields[index],
                resultIsWinningOverride,
                cancellationToken);
        }

        for (var index = pairCount; index < beforeFields.Count; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ApplyUnpairedState(
                beforeFields[index],
                NativeComparisonFieldState.Conflict,
                cancellationToken);
        }

        var afterState = resultIsWinningOverride
            ? NativeComparisonFieldState.WinningOverride
            : NativeComparisonFieldState.Conflict;
        for (var index = pairCount; index < afterFields.Count; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ApplyUnpairedState(afterFields[index], afterState, cancellationToken);
        }
    }

    /// <summary>Assigns state to one paired field and recursively pairs its children according to JSON structure.</summary>
    /// <param name="before">The prior field.</param>
    /// <param name="after">The resulting field.</param>
    /// <param name="resultIsWinningOverride">Whether differing resulting fields may be identified as winning overrides.</param>
    /// <param name="cancellationToken">A token observed throughout recursive pairing.</param>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    private static void ApplyComparisonState(
        NativeJsonFieldNodeViewModel before,
        NativeJsonFieldNodeViewModel after,
        bool resultIsWinningOverride,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var identical = AreEquivalent(before, after, cancellationToken);
        before.SetComparisonState(identical
            ? NativeComparisonFieldState.Identical
            : NativeComparisonFieldState.Conflict);
        after.SetComparisonState(identical
            ? NativeComparisonFieldState.Identical
            : resultIsWinningOverride
                ? NativeComparisonFieldState.WinningOverride
                : NativeComparisonFieldState.Conflict);

        if (before.ValueKind == JsonValueKind.Object && after.ValueKind == JsonValueKind.Object)
        {
            ApplyObjectChildStates(
                before.Children,
                after.Children,
                resultIsWinningOverride,
                cancellationToken);
            return;
        }

        if (before.ValueKind == JsonValueKind.Array && after.ValueKind == JsonValueKind.Array)
        {
            ApplyComparisonStates(
                before.Children,
                after.Children,
                resultIsWinningOverride,
                cancellationToken);
            return;
        }

        if (before.Children.Count > 0 || after.Children.Count > 0)
        {
            ApplyUnpairedChildren(
                before.Children,
                NativeComparisonFieldState.Conflict,
                cancellationToken);
            ApplyUnpairedChildren(
                after.Children,
                resultIsWinningOverride
                    ? NativeComparisonFieldState.WinningOverride
                    : NativeComparisonFieldState.Conflict,
                cancellationToken);
        }
    }

    /// <summary>Pairs object properties by exact name and occurrence without treating display paths as semantic identifiers.</summary>
    /// <param name="beforeChildren">The prior object properties in source order.</param>
    /// <param name="afterChildren">The resulting object properties in source order.</param>
    /// <param name="resultIsWinningOverride">Whether differing resulting fields may be identified as winning overrides.</param>
    /// <param name="cancellationToken">A token observed throughout property pairing.</param>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    private static void ApplyObjectChildStates(
        IReadOnlyList<NativeJsonFieldNodeViewModel> beforeChildren,
        IReadOnlyList<NativeJsonFieldNodeViewModel> afterChildren,
        bool resultIsWinningOverride,
        CancellationToken cancellationToken)
    {
        var afterByName = new Dictionary<string, Queue<NativeJsonFieldNodeViewModel>>(StringComparer.Ordinal);
        foreach (var afterChild in afterChildren)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!afterByName.TryGetValue(afterChild.Name, out var occurrences))
            {
                occurrences = new Queue<NativeJsonFieldNodeViewModel>();
                afterByName.Add(afterChild.Name, occurrences);
            }

            occurrences.Enqueue(afterChild);
        }

        foreach (var beforeChild in beforeChildren)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (afterByName.TryGetValue(beforeChild.Name, out var occurrences) && occurrences.Count > 0)
            {
                ApplyComparisonState(
                    beforeChild,
                    occurrences.Dequeue(),
                    resultIsWinningOverride,
                    cancellationToken);
            }
            else
            {
                ApplyUnpairedState(
                    beforeChild,
                    NativeComparisonFieldState.Conflict,
                    cancellationToken);
            }
        }

        var afterState = resultIsWinningOverride
            ? NativeComparisonFieldState.WinningOverride
            : NativeComparisonFieldState.Conflict;
        foreach (var occurrences in afterByName.Values)
        {
            while (occurrences.Count > 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
                ApplyUnpairedState(occurrences.Dequeue(), afterState, cancellationToken);
            }
        }
    }

    /// <summary>Determines whether two projected nodes contain the same complete detached JSON value.</summary>
    /// <param name="before">The prior projected node.</param>
    /// <param name="after">The resulting projected node.</param>
    /// <param name="cancellationToken">A token observed throughout structural comparison.</param>
    /// <returns><see langword="true"/> when the complete projected values are equivalent; otherwise <see langword="false"/>.</returns>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    private static bool AreEquivalent(
        NativeJsonFieldNodeViewModel before,
        NativeJsonFieldNodeViewModel after,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!string.Equals(before.Name, after.Name, StringComparison.Ordinal) ||
            before.ValueKind != after.ValueKind ||
            !string.Equals(before.ValueText, after.ValueText, StringComparison.Ordinal) ||
            before.Children.Count != after.Children.Count)
        {
            return false;
        }

        if (before.ValueKind == JsonValueKind.Object)
        {
            var afterByName = new Dictionary<string, Queue<NativeJsonFieldNodeViewModel>>(StringComparer.Ordinal);
            foreach (var afterChild in after.Children)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (!afterByName.TryGetValue(afterChild.Name, out var occurrences))
                {
                    occurrences = new Queue<NativeJsonFieldNodeViewModel>();
                    afterByName.Add(afterChild.Name, occurrences);
                }

                occurrences.Enqueue(afterChild);
            }

            foreach (var beforeChild in before.Children)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (!afterByName.TryGetValue(beforeChild.Name, out var occurrences) ||
                    occurrences.Count == 0 ||
                    !AreEquivalent(beforeChild, occurrences.Dequeue(), cancellationToken))
                {
                    return false;
                }
            }

            return afterByName.Values.All(occurrences => occurrences.Count == 0);
        }

        return before.Children
            .Zip(after.Children)
            .All(pair => AreEquivalent(pair.First, pair.Second, cancellationToken));
    }

    /// <summary>Assigns one state to an unpaired field and its complete subtree.</summary>
    /// <param name="field">The unpaired projected field.</param>
    /// <param name="state">The side-appropriate differing state.</param>
    /// <param name="cancellationToken">A token observed throughout subtree traversal.</param>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    private static void ApplyUnpairedState(
        NativeJsonFieldNodeViewModel field,
        NativeComparisonFieldState state,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        field.SetComparisonState(state);
        ApplyUnpairedChildren(field.Children, state, cancellationToken);
    }

    /// <summary>Assigns one state to every field in an unpaired subtree collection.</summary>
    /// <param name="fields">The unpaired projected fields.</param>
    /// <param name="state">The side-appropriate differing state.</param>
    /// <param name="cancellationToken">A token observed throughout subtree traversal.</param>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    private static void ApplyUnpairedChildren(
        IReadOnlyList<NativeJsonFieldNodeViewModel> fields,
        NativeComparisonFieldState state,
        CancellationToken cancellationToken)
    {
        foreach (var field in fields)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ApplyUnpairedState(field, state, cancellationToken);
        }
    }

    /// <summary>Creates a read-only value cell whose background communicates the field comparison state.</summary>
    /// <param name="field">The projected JSON field.</param>
    /// <returns>The configured value cell.</returns>
    private static Control CreateFieldValueCell(NativeJsonFieldNodeViewModel? field)
    {
        if (field is null)
        {
            return new TextBlock();
        }

        var value = new TextBlock
        {
            Text = field.ValueText,
            TextTrimming = TextTrimming.CharacterEllipsis,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
        };
        App.ApplyApplicationTextForeground(value);
        var cell = new Border
        {
            Background = GetComparisonFieldBrush(field.ComparisonState),
            Padding = new Thickness(6, 3),
            Child = value
        };
        var stateText = field.ComparisonState switch
        {
            NativeComparisonFieldState.WinningOverride => "Winning Override",
            _ => field.ComparisonState.ToString()
        };
        ToolTip.SetTip(cell, $"{stateText}: {field.ValueText}");
        AutomationProperties.SetName(cell, $"{stateText}: {field.ValueText}");
        return cell;
    }

    /// <summary>Maps a field state to the retained comparison color palette.</summary>
    /// <param name="state">The presentation comparison state.</param>
    /// <returns>The matching translucent field background.</returns>
    internal static IBrush GetComparisonFieldBrush(NativeComparisonFieldState state)
    {
        return state switch
        {
            NativeComparisonFieldState.Identical => new SolidColorBrush(Color.FromArgb(80, 0, 128, 0)),
            NativeComparisonFieldState.Conflict => new SolidColorBrush(Color.FromArgb(80, 192, 0, 0)),
            NativeComparisonFieldState.WinningOverride => new SolidColorBrush(Color.FromArgb(80, 192, 160, 0)),
            _ => new SolidColorBrush(Colors.Transparent)
        };
    }
}
