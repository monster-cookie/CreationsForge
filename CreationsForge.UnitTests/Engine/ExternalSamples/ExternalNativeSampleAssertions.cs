using System.Text.Json;
using Mutagen.Bethesda.Plugins;

namespace CreationsForge.UnitTests.Engine.ExternalSamples;

/// <summary>Checks independently derived external override intentions against preview and native values.</summary>
internal static class ExternalNativeSampleAssertions
{
    /// <summary>Rejects an internally inconsistent intended edit before any mutation is applied.</summary>
    /// <param name="intended">The independently derived edit definition.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="intended"/> is null.</exception>
    /// <exception cref="InvalidDataException">Thrown when EditorID is unchanged or ReplaceItems does not describe a material ordered change.</exception>
    internal static void RequireMaterialIntention(ExternalIntendedRecordEdit intended)
    {
        ArgumentNullException.ThrowIfNull(intended);
        using var sourceDocument = JsonDocument.Parse(intended.Source.Json);
        var sourceEditorId = sourceDocument.RootElement.GetProperty("EditorID").GetString();
        if (string.Equals(sourceEditorId, intended.EditorId, StringComparison.Ordinal))
        {
            throw new InvalidDataException($"External override '{intended.Source.FormKey}' did not define a material EditorID change.");
        }

        var itemsChanged = !intended.Items.SequenceEqual(intended.Source.Items);
        if (intended.ReplaceItems && !itemsChanged)
        {
            throw new InvalidDataException($"External override '{intended.Source.FormKey}' defined a no-op ReplaceItems edit.");
        }

        if (!intended.ReplaceItems && itemsChanged)
        {
            throw new InvalidDataException($"External override '{intended.Source.FormKey}' changed intended Items without selecting ReplaceItems.");
        }
    }

    /// <summary>Checks complete inspector JSON against independently derived EditorID and ordered Items values.</summary>
    /// <param name="json">The complete inspector JSON to check.</param>
    /// <param name="intended">The independently derived edit definition.</param>
    /// <param name="phase">The verification phase used in diagnostics.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="json"/> or <paramref name="phase"/> is empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="intended"/> is null.</exception>
    /// <exception cref="InvalidDataException">Thrown when the exact intended values are absent.</exception>
    internal static void RequireInspectorValues(
        string json,
        ExternalIntendedRecordEdit intended,
        string phase)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(json);
        ArgumentNullException.ThrowIfNull(intended);
        ArgumentException.ThrowIfNullOrWhiteSpace(phase);
        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;
        var editorId = root.GetProperty("EditorID").GetString();
        var items = root.GetProperty("Items")
            .EnumerateArray()
            .Select(item => FormKey.Factory(item.GetProperty("formKey").GetString()!));
        RequireNativeValues(editorId, items, intended, phase);
    }

    /// <summary>Checks direct native values against independently derived EditorID and ordered Items values.</summary>
    /// <param name="editorId">The directly observed EditorID.</param>
    /// <param name="items">The directly observed ordered Items sequence.</param>
    /// <param name="intended">The independently derived edit definition.</param>
    /// <param name="phase">The verification phase used in diagnostics.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="phase"/> is empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="items"/> or <paramref name="intended"/> is null.</exception>
    /// <exception cref="InvalidDataException">Thrown when the exact intended values are absent.</exception>
    internal static void RequireNativeValues(
        string? editorId,
        IEnumerable<FormKey> items,
        ExternalIntendedRecordEdit intended,
        string phase)
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(intended);
        ArgumentException.ThrowIfNullOrWhiteSpace(phase);
        if (!string.Equals(editorId, intended.EditorId, StringComparison.Ordinal))
        {
            throw new InvalidDataException($"{phase} EditorID differed from the independent intention for '{intended.Source.FormKey}'.");
        }

        if (!items.SequenceEqual(intended.Items))
        {
            throw new InvalidDataException($"{phase} ordered Items differed from the independent intention for '{intended.Source.FormKey}'.");
        }
    }
}
