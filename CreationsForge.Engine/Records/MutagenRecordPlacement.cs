using System.Collections;
using System.Reflection;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;

namespace CreationsForge.Engine.Records;

/// <summary>Publishes major records that live inside a cell or quest instead of a top-level group.</summary>
internal static partial class MutagenRecordPlacement
{
    /// <summary>Builds storage delegates when the record has a known parent shape.</summary>
    /// <param name="modType">The game mod interface.</param>
    /// <param name="recordType">The mutable record type.</param>
    /// <param name="release">The game release constructor argument.</param>
    /// <param name="find">The output lookup.</param>
    /// <param name="publish">The output publication.</param>
    /// <param name="remove">The output removal.</param>
    /// <param name="placeOverride">Places an override through the source context.</param>
    /// <returns>Whether the record has a supported parent.</returns>
    public static bool TryBind(
        Type modType,
        Type recordType,
        object release,
        out Func<IMod, FormKey, IMajorRecordGetter?> find,
        out Action<IMod, IMajorRecord> publish,
        out Func<IMod, FormKey, bool> remove,
        out Func<IMod, IModContext, IMajorRecord>? placeOverride)
    {
        find = (_, _) => null;
        publish = (_, _) => { };
        remove = (_, _) => false;
        placeOverride = null;
        if (recordType.Name == "Cell")
        {
            find = (mod, key) => FindCell(mod, key);
            publish = (mod, record) =>
            {
                if (!ReplaceCell(mod, record))
                {
                    AddInteriorCell(mod, record);
                }
            };
            remove = (mod, key) => RemoveCell(mod, key);
            placeOverride = PlaceByContext;
            return true;
        }

        if (recordType.GetInterfaces().Any(type => type.Name == "IPlaced"))
        {
            find = (mod, key) => FindChild(mod, key, "Temporary", "Persistent");
            publish = (mod, record) =>
            {
                if (!ReplaceChild(mod, record, "Temporary", "Persistent"))
                {
                    PublishInNewCell(mod, recordType, release, cell => AddToList(cell, "Temporary", record));
                }
            };
            remove = (mod, key) => RemoveChild(mod, key, "Temporary", "Persistent");
            placeOverride = PlaceByContext;
            return true;
        }

        if (recordType.Name == "NavigationMesh")
        {
            find = (mod, key) => FindChild(mod, key, "NavigationMeshes");
            publish = (mod, record) =>
            {
                if (!ReplaceChild(mod, record, "NavigationMeshes"))
                {
                    PublishInNewCell(mod, recordType, release, cell => AddToList(cell, "NavigationMeshes", record));
                }
            };
            remove = (mod, key) => RemoveChild(mod, key, "NavigationMeshes");
            placeOverride = PlaceByContext;
            return true;
        }

        if (recordType.Name == "Landscape" && recordType.Assembly.GetType(recordType.Namespace + ".Cell")?.GetProperty("Landscape") is not null)
        {
            find = (mod, key) => EnumerateCells(mod).Select(cell => cell.GetType().GetProperty("Landscape")?.GetValue(cell)).OfType<IMajorRecordGetter>().FirstOrDefault(record => record.FormKey == key);
            publish = (mod, record) =>
            {
                if (!ReplaceLandscape(mod, record))
                {
                    PublishInNewCell(mod, recordType, release, cell => cell.GetType().GetProperty("Landscape")!.SetValue(cell, record));
                }
            };
            remove = RemoveLandscape;
            placeOverride = PlaceByContext;
            return true;
        }

        if (recordType.Name is "DialogBranch" or "DialogTopic" or "Scene")
        {
            var listName = recordType.Name switch
            {
                "DialogBranch" => "DialogBranches",
                "DialogTopic" => "DialogTopics",
                _ => "Scenes",
            };
            find = (mod, key) => FindOnQuests(mod, key, listName);
            publish = (mod, record) =>
            {
                if (!ReplaceOnQuests(mod, record, listName))
                {
                    PublishOnNewQuest(mod, release, quest => AddToList(quest, listName, record));
                }
            };
            remove = (mod, key) => RemoveFromQuests(mod, key, listName);
            placeOverride = (mod, context) => PlaceDialog(mod, context, recordType, release);
            return true;
        }

        if (recordType.Name == "DialogResponses")
        {
            find = FindResponses;
            publish = (mod, record) =>
            {
                if (!ReplaceResponses(mod, record))
                {
                    PublishResponses(mod, record, release);
                }
            };
            remove = RemoveResponses;
            placeOverride = (mod, context) => PlaceDialog(mod, context, recordType, release);
            return true;
        }

        return false;
    }

    private static void PublishInNewCell(IMod mod, Type recordType, object release, Action<object> attach)
    {
        var cellType = recordType.Assembly.GetType(recordType.Namespace + ".Cell")!;
        var cell = Activator.CreateInstance(cellType, mod.GetNextFormKey(), release)!;
        MarkInterior(cell);
        attach(cell);
        AddInteriorCell(mod, cell);
    }

    private static void AddInteriorCell(IMod mod, object cell)
    {
        var cells = mod.GetType().GetProperty("Cells")!.GetValue(mod)!;
        var block = Items(cells).FirstOrDefault(candidate => IsInteriorBlock(candidate, "InteriorCellBlock"));
        if (block is null)
        {
            var assembly = cell.GetType().Assembly;
            var name = cell.GetType().Namespace + ".";
            var blockType = assembly.GetType(name + "CellBlock")!;
            var groupType = assembly.GetType(name + "GroupTypeEnum")!;
            block = Activator.CreateInstance(blockType)!;
            blockType.GetProperty("BlockNumber")!.SetValue(block, 0);
            blockType.GetProperty("GroupType")!.SetValue(block, Enum.Parse(groupType, "InteriorCellBlock"));
            Add(cells, block);
        }

        var subs = block.GetType().GetProperty("SubBlocks")!.GetValue(block);
        var sub = Items(subs).FirstOrDefault(candidate => IsInteriorBlock(candidate, "InteriorCellSubBlock"));
        if (sub is null)
        {
            var assembly = cell.GetType().Assembly;
            var name = cell.GetType().Namespace + ".";
            var subType = assembly.GetType(name + "CellSubBlock")!;
            var groupType = assembly.GetType(name + "GroupTypeEnum")!;
            sub = Activator.CreateInstance(subType)!;
            subType.GetProperty("BlockNumber")!.SetValue(sub, 0);
            subType.GetProperty("GroupType")!.SetValue(sub, Enum.Parse(groupType, "InteriorCellSubBlock"));
            if (subs is null)
            {
                AssignList(block, "SubBlocks", sub);
            }
            else
            {
                Add(subs, sub);
            }
        }

        AddToList(sub, "Cells", cell);
    }

    private static bool IsInteriorBlock(object block, string groupName)
    {
        var number = block.GetType().GetProperty("BlockNumber")?.GetValue(block);
        var group = block.GetType().GetProperty("GroupType")?.GetValue(block);
        return number is not null && Convert.ToInt32(number) == 0 && string.Equals(group?.ToString(), groupName, StringComparison.Ordinal);
    }

    private static void MarkInterior(object cell)
    {
        var flags = cell.GetType().GetProperty("Flags");
        if (flags is null)
        {
            return;
        }

        var enumType = Nullable.GetUnderlyingType(flags.PropertyType) ?? flags.PropertyType;
        if (!enumType.IsEnum)
        {
            return;
        }

        var name = Enum.GetNames(enumType).FirstOrDefault(candidate => candidate.Contains("Interior", StringComparison.Ordinal));
        if (name is null)
        {
            return;
        }

        var interior = Enum.Parse(enumType, name);
        var current = flags.GetValue(cell);
        var combined = current is null
            ? interior
            : Enum.ToObject(enumType, Convert.ToInt64(current) | Convert.ToInt64(interior));
        flags.SetValue(cell, combined);
    }

    private static IMajorRecordGetter? FindCell(IMod mod, FormKey formKey)
    {
        return EnumerateCells(mod).OfType<IMajorRecordGetter>().FirstOrDefault(cell => cell.FormKey == formKey);
    }

    private static bool RemoveCell(IMod mod, FormKey formKey)
    {
        foreach (var block in Items(mod.GetType().GetProperty("Cells")!.GetValue(mod)))
        {
            var subs = block.GetType().GetProperty("SubBlocks")?.GetValue(block);
            foreach (var sub in Items(subs).ToArray())
            {
                var cells = sub.GetType().GetProperty("Cells")?.GetValue(sub);
                if (RemoveMatch(cells, formKey))
                {
                    if (!Items(cells).Any())
                    {
                        RemoveItem(subs, sub);
                    }

                    if (!Items(subs).Any())
                    {
                        RemoveItem(mod.GetType().GetProperty("Cells")!.GetValue(mod), block);
                    }

                    return true;
                }
            }
        }

        foreach (var world in GroupRecords(mod, "Worldspaces").ToArray())
        {
            var top = world.GetType().GetProperty("TopCell");
            if (top?.GetValue(world) is IMajorRecordGetter topCell && topCell.FormKey == formKey)
            {
                top.SetValue(world, null);
                return true;
            }

            if (RemoveInNode(world.GetType().GetProperty("SubCells")?.GetValue(world), formKey))
            {
                return true;
            }
        }

        return false;
    }

    private static bool RemoveInNode(object? node, FormKey formKey)
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

            if (RemoveMatch(list, formKey))
            {
                return true;
            }

            foreach (var child in Items(list).ToArray())
            {
                if (RemoveInNode(child, formKey))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static IMajorRecordGetter? FindChild(IMod mod, FormKey formKey, params string[] listNames)
    {
        foreach (var cell in EnumerateCells(mod))
        {
            foreach (var listName in listNames)
            {
                var match = Items(cell.GetType().GetProperty(listName)?.GetValue(cell)).OfType<IMajorRecordGetter>().FirstOrDefault(record => record.FormKey == formKey);
                if (match is not null)
                {
                    return match;
                }
            }
        }

        return null;
    }

    private static bool RemoveChild(IMod mod, FormKey formKey, params string[] listNames)
    {
        foreach (var cell in EnumerateCells(mod).ToArray())
        {
            foreach (var listName in listNames)
            {
                var list = cell.GetType().GetProperty(listName)?.GetValue(cell);
                if (!RemoveMatch(list, formKey))
                {
                    continue;
                }

                return true;
            }
        }

        return false;
    }

    private static bool RemoveLandscape(IMod mod, FormKey formKey)
    {
        foreach (var cell in EnumerateCells(mod).ToArray())
        {
            var property = cell.GetType().GetProperty("Landscape");
            if (property?.GetValue(cell) is not IMajorRecordGetter landscape || landscape.FormKey != formKey)
            {
                continue;
            }

            property.SetValue(cell, null);
            return true;
        }

        return false;
    }

    private static bool CellIsEmpty(object cell)
    {
        return cell.GetType().GetProperty("Landscape")?.GetValue(cell) is null
            && !Items(cell.GetType().GetProperty("Temporary")?.GetValue(cell)).Any()
            && !Items(cell.GetType().GetProperty("Persistent")?.GetValue(cell)).Any()
            && !Items(cell.GetType().GetProperty("NavigationMeshes")?.GetValue(cell)).Any();
    }

    private static void PublishOnNewQuest(IMod mod, object release, Action<object> attach)
    {
        var questType = mod.GetType().Assembly.GetType(mod.GetType().Namespace + ".Quest")!;
        var quest = Activator.CreateInstance(questType, mod.GetNextFormKey(), release)!;
        attach(quest);
        SetOnGroup(mod, "Quests", quest);
    }

    private static IMajorRecordGetter? FindOnQuests(IMod mod, FormKey formKey, string listName)
    {
        foreach (var quest in GroupRecords(mod, "Quests"))
        {
            var match = Items(quest.GetType().GetProperty(listName)?.GetValue(quest)).OfType<IMajorRecordGetter>().FirstOrDefault(record => record.FormKey == formKey);
            if (match is not null)
            {
                return match;
            }
        }

        return null;
    }

    private static bool RemoveFromQuests(IMod mod, FormKey formKey, string listName)
    {
        foreach (var quest in GroupRecords(mod, "Quests").ToArray())
        {
            var list = quest.GetType().GetProperty(listName)?.GetValue(quest);
            if (!RemoveMatch(list, formKey))
            {
                continue;
            }

            return true;
        }

        return false;
    }

    private static bool QuestIsEmpty(object quest)
    {
        return !Items(quest.GetType().GetProperty("DialogBranches")?.GetValue(quest)).Any()
            && !Items(quest.GetType().GetProperty("DialogTopics")?.GetValue(quest)).Any()
            && !Items(quest.GetType().GetProperty("Scenes")?.GetValue(quest)).Any();
    }

    private static IMajorRecordGetter? FindResponses(IMod mod, FormKey formKey)
    {
        foreach (var topic in AllTopics(mod))
        {
            var match = Items(topic.GetType().GetProperty("Responses")?.GetValue(topic)).OfType<IMajorRecordGetter>().FirstOrDefault(record => record.FormKey == formKey);
            if (match is not null)
            {
                return match;
            }
        }

        return null;
    }

    private static void PublishResponses(IMod mod, IMajorRecord record, object release)
    {
        var topicType = record.GetType().Assembly.GetType(record.GetType().Namespace + ".DialogTopic")!;
        var topic = Activator.CreateInstance(topicType, mod.GetNextFormKey(), release)!;
        AddToList(topic, "Responses", record);
        if (mod.GetType().GetProperty("DialogTopics") is not null)
        {
            SetOnGroup(mod, "DialogTopics", topic);
            return;
        }

        PublishOnNewQuest(mod, release, quest => AddToList(quest, "DialogTopics", topic));
    }

    private static bool RemoveResponses(IMod mod, FormKey formKey)
    {
        foreach (var topic in AllTopics(mod).ToArray())
        {
            if (!RemoveMatch(topic.GetType().GetProperty("Responses")?.GetValue(topic), formKey))
            {
                continue;
            }

            return true;
        }

        return false;
    }

    private static IEnumerable<object> AllTopics(object mod)
    {
        if (mod.GetType().GetProperty("DialogTopics") is not null)
        {
            return GroupRecords(mod, "DialogTopics");
        }

        return GroupRecords(mod, "Quests").SelectMany(quest => Items(quest.GetType().GetProperty("DialogTopics")?.GetValue(quest)));
    }

    /// <summary>Yields each stored major record once, including worldspace cells and nested dialog.</summary>
    /// <param name="mod">The mutable output or a reopened plugin.</param>
    /// <returns>The records whose FormKeys must stay unique.</returns>
    internal static IEnumerable<IMajorRecordGetter> ContainedRecords(IModGetter mod)
    {
        var seen = new HashSet<object>(ReferenceEqualityComparer.Instance);
        foreach (var record in mod.EnumerateMajorRecords())
        {
            if (seen.Add(record))
            {
                yield return record;
            }
        }

        foreach (var cell in EnumerateCells(mod))
        {
            if (cell is IMajorRecordGetter cellRecord && seen.Add(cellRecord))
            {
                yield return cellRecord;
            }

            foreach (var listName in new[] { "Persistent", "Temporary", "NavigationMeshes" })
            {
                foreach (var child in Items(cell.GetType().GetProperty(listName)?.GetValue(cell)).OfType<IMajorRecordGetter>())
                {
                    if (seen.Add(child))
                    {
                        yield return child;
                    }
                }
            }

            if (cell.GetType().GetProperty("Landscape")?.GetValue(cell) is IMajorRecordGetter landscape && seen.Add(landscape))
            {
                yield return landscape;
            }
        }

        foreach (var parent in AllTopics(mod).Concat(GroupRecords(mod, "Quests")))
        {
            if (parent is IMajorRecordGetter parentRecord && seen.Add(parentRecord))
            {
                yield return parentRecord;
            }

            foreach (var listName in new[] { "Responses", "DialogTopics", "DialogBranches", "Scenes" })
            {
                foreach (var child in Items(parent.GetType().GetProperty(listName)?.GetValue(parent)).OfType<IMajorRecordGetter>())
                {
                    if (seen.Add(child))
                    {
                        yield return child;
                    }
                }
            }
        }
    }

    private static bool IsRecordNamed(object record, string name)
    {
        var actual = record.GetType().Name;
        const string suffix = "BinaryOverlay";
        if (actual.EndsWith(suffix, StringComparison.Ordinal))
        {
            actual = actual[..^suffix.Length];
        }

        return actual == name;
    }

    private static IEnumerable<object> EnumerateCells(object mod)
    {
        foreach (var block in Items(mod.GetType().GetProperty("Cells")?.GetValue(mod)))
        {
            foreach (var sub in Items(block.GetType().GetProperty("SubBlocks")?.GetValue(block)))
            {
                foreach (var cell in Items(sub.GetType().GetProperty("Cells")?.GetValue(sub)))
                {
                    yield return cell;
                }
            }
        }

        foreach (var world in GroupRecords(mod, "Worldspaces"))
        {
            foreach (var cell in NestedCells(world))
            {
                yield return cell;
            }
        }
    }

    private static IEnumerable<object> NestedCells(object parent)
    {
        var top = parent.GetType().GetProperty("TopCell")?.GetValue(parent);
        if (top is not null)
        {
            yield return top;
        }

        foreach (var cell in DeepCells(parent.GetType().GetProperty("SubCells")?.GetValue(parent)))
        {
            yield return cell;
        }
    }

    private static IEnumerable<object> DeepCells(object? node)
    {
        if (node is null || node is string)
        {
            yield break;
        }

        if (IsRecordNamed(node, "Cell"))
        {
            yield return node;
            yield break;
        }

        var walked = false;
        foreach (var propertyName in new[] { "Items", "SubBlocks", "Cells" })
        {
            var value = node.GetType().GetProperty(propertyName)?.GetValue(node);
            if (value is null)
            {
                continue;
            }

            walked = true;
            foreach (var cell in DeepCells(value))
            {
                yield return cell;
            }
        }

        if (walked || node is not IEnumerable list)
        {
            yield break;
        }

        foreach (var item in list)
        {
            if (item is null)
            {
                continue;
            }

            foreach (var cell in DeepCells(item))
            {
                yield return cell;
            }
        }
    }

    private static IEnumerable<object> GroupRecords(object mod, string propertyName)
    {
        var group = mod.GetType().GetProperty(propertyName)?.GetValue(mod);
        var records = group?.GetType().GetProperty("Records")?.GetValue(group) ?? group;
        return Items(records);
    }

    private static void SetOnGroup(IMod mod, string propertyName, object record)
    {
        var group = mod.GetType().GetProperty(propertyName)!.GetValue(mod)!;
        var set = group.GetType().GetMethods().First(method => method.Name == "Set" && method.GetParameters().Length == 1);
        set.Invoke(group, [record]);
    }

    private static void RemoveFromGroup(IMod mod, string propertyName, FormKey formKey)
    {
        var group = mod.GetType().GetProperty(propertyName)!.GetValue(mod)!;
        var remove = group.GetType().GetMethods().First(method => method.Name == "Remove" && method.GetParameters() is [{ ParameterType: var parameter }] && parameter == typeof(FormKey));
        remove.Invoke(group, [formKey]);
    }

    private static void AddToList(object parent, string propertyName, object item)
    {
        var property = parent.GetType().GetProperty(propertyName)!;
        var list = property.GetValue(parent);
        if (list is null)
        {
            AssignList(parent, propertyName, item);
            return;
        }

        Add(list, item);
    }

    private static void AssignList(object parent, string propertyName, object item)
    {
        var property = parent.GetType().GetProperty(propertyName)!;
        var elementType = property.PropertyType.IsGenericType ? property.PropertyType.GenericTypeArguments[0] : item.GetType();
        var listType = property.PropertyType.GetConstructor(Type.EmptyTypes) is not null
            ? property.PropertyType
            : ExtendedList().MakeGenericType(elementType);
        var list = Activator.CreateInstance(listType)!;
        Add(list, item);
        property.SetValue(parent, list);
    }

    private static void Add(object list, object item)
    {
        list.GetType().GetMethod("Add")!.Invoke(list, [item]);
    }

    private static bool RemoveMatch(object? list, FormKey formKey)
    {
        var match = Items(list).OfType<IMajorRecordGetter>().FirstOrDefault(record => record.FormKey == formKey);
        return match is not null && RemoveItem(list, match);
    }

    private static bool RemoveItem(object? list, object item)
    {
        var remove = list?.GetType().GetMethod("Remove");
        if (remove is null)
        {
            return false;
        }

        return remove.Invoke(list, [item]) is not false;
    }

    private static IEnumerable<object> Items(object? value)
    {
        if (value is not IEnumerable list)
        {
            yield break;
        }

        foreach (var item in list)
        {
            if (item is not null)
            {
                yield return item;
            }
        }
    }

    private static Type ExtendedList()
    {
        return AppDomain.CurrentDomain.GetAssemblies().Select(assembly => assembly.GetType("Noggog.ExtendedList`1")).First(type => type is not null)!;
    }
}
