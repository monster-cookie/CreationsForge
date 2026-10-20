using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;

namespace CreationsForge.Engine.Records;

/// <content>Places overrides on the source parent chain and removes parents created by a failed apply.</content>
internal static partial class MutagenRecordPlacement
{
    /// <summary>Inserts the context record with Mutagen's parent chain and returns the output copy.</summary>
    /// <param name="mod">The mutable output plugin.</param>
    /// <param name="context">The exact source context.</param>
    /// <returns>The mutable record now stored in the output.</returns>
    internal static IMajorRecord PlaceByContext(IMod mod, IModContext context)
    {
        var method = context.GetType().GetMethod("GetOrAddAsOverride")
            ?? throw new RecordEditingException($"Mutagen context '{context.GetType().Name}' cannot place an override.");
        var formKey = (context.Record as IMajorRecordGetter)?.FormKey;
        var cellAlreadyPresent = formKey is { } key && EnumerateCells(mod).OfType<IMajorRecordGetter>().Any(cell => cell.FormKey == key);
        try
        {
            var placed = (IMajorRecord)method.Invoke(context, [mod])!;
            if (!cellAlreadyPresent && IsRecordNamed(placed, "Cell"))
            {
                ClearCellChildren(placed);
            }

            return placed;
        }
        catch (System.Reflection.TargetInvocationException exception)
        {
            throw new RecordEditingException(exception.InnerException?.Message ?? exception.Message, exception.InnerException ?? exception);
        }
    }

    private static void ClearCellChildren(object cell)
    {
        foreach (var listName in new[] { "Persistent", "Temporary", "NavigationMeshes" })
        {
            var list = cell.GetType().GetProperty(listName)?.GetValue(cell);
            list?.GetType().GetMethod("Clear")?.Invoke(list, null);
        }

        var landscape = cell.GetType().GetProperty("Landscape");
        if (landscape?.CanWrite == true)
        {
            landscape.SetValue(cell, null);
        }
    }

    /// <summary>Removes empty parents created during the current apply.</summary>
    /// <param name="mod">The mutable output plugin.</param>
    /// <param name="preserved">FormKeys that were already in the output before the apply.</param>
    internal static void PruneCreatedParents(IMod mod, IReadOnlySet<FormKey> preserved)
    {
        foreach (var cell in EnumerateCells(mod).OfType<IMajorRecordGetter>().ToArray())
        {
            if (preserved.Contains(cell.FormKey) || !CellIsEmpty(cell))
            {
                continue;
            }

            RemoveCell(mod, cell.FormKey);
        }

        if (mod.GetType().GetProperty("DialogTopics") is not null)
        {
            foreach (var topic in GroupRecords(mod, "DialogTopics").OfType<IMajorRecordGetter>().ToArray())
            {
                if (preserved.Contains(topic.FormKey) || Items(topic.GetType().GetProperty("Responses")?.GetValue(topic)).Any())
                {
                    continue;
                }

                RemoveFromGroup(mod, "DialogTopics", topic.FormKey);
            }
        }

        foreach (var quest in GroupRecords(mod, "Quests").OfType<IMajorRecordGetter>().ToArray())
        {
            if (preserved.Contains(quest.FormKey) || !QuestIsEmpty(quest))
            {
                continue;
            }

            RemoveFromGroup(mod, "Quests", quest.FormKey);
        }

        foreach (var world in GroupRecords(mod, "Worldspaces").OfType<IMajorRecordGetter>().ToArray())
        {
            if (preserved.Contains(world.FormKey) || NestedCells(world).Any())
            {
                continue;
            }

            RemoveFromGroup(mod, "Worldspaces", world.FormKey);
        }
    }

    private static IMajorRecord PlaceDialog(IMod mod, IModContext context, Type recordType, object release)
    {
        var source = context.Record as IMajorRecordGetter
            ?? throw new RecordEditingException($"Mutagen context for '{recordType.Name}' did not contain a major record.");
        var listName = recordType.Name switch
        {
            "DialogBranch" => "DialogBranches",
            "DialogTopic" => "DialogTopics",
            "Scene" => "Scenes",
            "DialogResponses" => "Responses",
            _ => throw new RecordEditingException($"Record family '{recordType.Name}' has no dialog parent list."),
        };
        if (context.Parent?.Record is not IMajorRecordGetter)
        {
            var grouped = MutagenRecordCatalog.CopyWithoutChildren(source, recordType.Name);
            SetOnGroup(mod, listName == "DialogTopics" ? "DialogTopics" : "Quests", grouped);
            return grouped;
        }

        var parent = EnsureDialogParent(mod, context.Parent, release);
        if (FindInList(parent, listName, source.FormKey) is IMajorRecord existing)
        {
            return existing;
        }

        var copy = recordType.Name == "DialogTopic"
            ? MutagenRecordCatalog.CopyWithoutChildren(source, recordType.Name)
            : MutagenRecordCatalog.CopyRecord(source);
        AddToList(parent, listName, copy);
        return copy;
    }

    private static object EnsureDialogParent(IMod mod, IModContext parentContext, object release)
    {
        if (parentContext.Record is not IMajorRecordGetter source)
        {
            throw new RecordEditingException("Mutagen parent context did not contain a major record.");
        }

        var name = SourceName(source);
        var existing = FindNamed(mod, source.FormKey, name);
        if (existing is not null)
        {
            return existing;
        }

        var shell = MutagenRecordCatalog.CopyWithoutChildren(source, name);
        if (name == "Quest")
        {
            SetOnGroup(mod, "Quests", shell);
            return shell;
        }

        if (name == "DialogTopic" && mod.GetType().GetProperty("DialogTopics") is not null)
        {
            SetOnGroup(mod, "DialogTopics", shell);
            return shell;
        }

        if (name == "DialogTopic")
        {
            var questContext = parentContext.Parent
                ?? throw new RecordEditingException($"Dialog topic '{source.FormKey}' has no parent quest.");
            var quest = EnsureDialogParent(mod, questContext, release);
            AddToList(quest, "DialogTopics", shell);
            return shell;
        }

        _ = release;
        throw new RecordEditingException($"Record parent '{name}' cannot hold a dialog override.");
    }

    private static object? FindNamed(IMod mod, FormKey formKey, string recordName)
    {
        if (recordName == "Quest")
        {
            return GroupRecords(mod, "Quests").OfType<IMajorRecordGetter>().FirstOrDefault(record => record.FormKey == formKey && record.GetType().Name == recordName);
        }

        return AllTopics(mod).OfType<IMajorRecordGetter>().FirstOrDefault(record => record.FormKey == formKey && record.GetType().Name == recordName);
    }

    private static IMajorRecord? FindInList(object parent, string listName, FormKey formKey)
    {
        return Items(parent.GetType().GetProperty(listName)?.GetValue(parent)).OfType<IMajorRecord>().FirstOrDefault(record => record.FormKey == formKey);
    }

    private static string SourceName(IMajorRecordGetter record)
    {
        var name = record.GetType().Name;
        return name.EndsWith("BinaryOverlay", StringComparison.Ordinal) ? name[..^"BinaryOverlay".Length] : name;
    }

    private static bool ReplaceChild(IMod mod, IMajorRecord record, params string[] listNames)
    {
        foreach (var cell in EnumerateCells(mod))
        {
            foreach (var listName in listNames)
            {
                if (ReplaceInList(cell.GetType().GetProperty(listName)?.GetValue(cell), record))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static bool ReplaceLandscape(IMod mod, IMajorRecord record)
    {
        foreach (var cell in EnumerateCells(mod))
        {
            var property = cell.GetType().GetProperty("Landscape");
            if (property?.GetValue(cell) is IMajorRecordGetter landscape && landscape.FormKey == record.FormKey)
            {
                property.SetValue(cell, record);
                return true;
            }
        }

        return false;
    }

    private static bool ReplaceCell(IMod mod, IMajorRecord record)
    {
        if (ReplaceInteriorCell(mod, record))
        {
            return true;
        }

        foreach (var world in GroupRecords(mod, "Worldspaces"))
        {
            var top = world.GetType().GetProperty("TopCell");
            if (top?.GetValue(world) is IMajorRecordGetter topCell && topCell.FormKey == record.FormKey)
            {
                top.SetValue(world, record);
                return true;
            }

            if (ReplaceInNode(world.GetType().GetProperty("SubCells")?.GetValue(world), record))
            {
                return true;
            }
        }

        return false;
    }

    private static bool ReplaceInteriorCell(IMod mod, IMajorRecord record)
    {
        foreach (var block in Items(mod.GetType().GetProperty("Cells")!.GetValue(mod)))
        {
            foreach (var sub in Items(block.GetType().GetProperty("SubBlocks")?.GetValue(block)))
            {
                if (ReplaceInList(sub.GetType().GetProperty("Cells")?.GetValue(sub), record))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static bool ReplaceOnQuests(IMod mod, IMajorRecord record, string listName)
    {
        foreach (var quest in GroupRecords(mod, "Quests"))
        {
            if (ReplaceInList(quest.GetType().GetProperty(listName)?.GetValue(quest), record))
            {
                return true;
            }
        }

        return false;
    }

    private static bool ReplaceResponses(IMod mod, IMajorRecord record)
    {
        foreach (var topic in AllTopics(mod))
        {
            if (ReplaceInList(topic.GetType().GetProperty("Responses")?.GetValue(topic), record))
            {
                return true;
            }
        }

        return false;
    }

    private static bool ReplaceInNode(object? node, IMajorRecord record)
    {
        if (node is null || node is string || node.GetType().Name == "Cell")
        {
            return false;
        }

        foreach (var propertyName in new[] { "Items", "SubBlocks", "Cells" })
        {
            var list = node.GetType().GetProperty(propertyName)?.GetValue(node);
            if (list is null)
            {
                continue;
            }

            if (ReplaceInList(list, record))
            {
                return true;
            }

            foreach (var child in Items(list).ToArray())
            {
                if (ReplaceInNode(child, record))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static bool ReplaceInList(object? list, IMajorRecord record)
    {
        if (!RemoveMatch(list, record.FormKey))
        {
            return false;
        }

        Add(list!, record);
        return true;
    }
}
