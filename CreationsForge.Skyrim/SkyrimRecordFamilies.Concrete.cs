using CreationsForge.Engine.Records;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace CreationsForge.Skyrim;

internal static partial class SkyrimRecordFamilies
{
    /// <summary>Gets concrete subclasses and parent-contained major records.</summary>
    private static IReadOnlyList<RecordFamily> ConcreteTypes { get; } =
    [
        CellFamily(),
        DialogResponsesFamily(),
        SubclassFamily<GameSettingBool, IGameSettingBoolGetter, GameSetting>("GameSettingBool", key => new GameSettingBool(key, SkyrimRelease.SkyrimSE), getter => getter.DeepCopy(), mod => mod.GameSettings),
        SubclassFamily<GameSettingFloat, IGameSettingFloatGetter, GameSetting>("GameSettingFloat", key => new GameSettingFloat(key, SkyrimRelease.SkyrimSE), getter => getter.DeepCopy(), mod => mod.GameSettings),
        SubclassFamily<GameSettingInt, IGameSettingIntGetter, GameSetting>("GameSettingInt", key => new GameSettingInt(key, SkyrimRelease.SkyrimSE), getter => getter.DeepCopy(), mod => mod.GameSettings),
        SubclassFamily<GameSettingString, IGameSettingStringGetter, GameSetting>("GameSettingString", key => new GameSettingString(key, SkyrimRelease.SkyrimSE), getter => getter.DeepCopy(), mod => mod.GameSettings),
        SubclassFamily<GlobalFloat, IGlobalFloatGetter, Global>("GlobalFloat", key => new GlobalFloat(key, SkyrimRelease.SkyrimSE) { Data = 1.5f }, getter => getter.DeepCopy(), mod => mod.Globals),
        SubclassFamily<GlobalInt, IGlobalIntGetter, Global>("GlobalInt", key => new GlobalInt(key, SkyrimRelease.SkyrimSE) { Data = 1 }, getter => getter.DeepCopy(), mod => mod.Globals),
        SubclassFamily<GlobalShort, IGlobalShortGetter, Global>("GlobalShort", key => new GlobalShort(key, SkyrimRelease.SkyrimSE) { Data = 1 }, getter => getter.DeepCopy(), mod => mod.Globals),
        SubclassFamily<GlobalUnknown, IGlobalUnknownGetter, Global>("GlobalUnknown", key => new GlobalUnknown(key, SkyrimRelease.SkyrimSE) { Data = 1.5f }, getter => getter.DeepCopy(), mod => mod.Globals),
        LandscapeFamily(),
        NavigationMeshFamily(),
        PlacedFamily<PlacedArrow, IPlacedArrowGetter>("PlacedArrow", key => new PlacedArrow(key, SkyrimRelease.SkyrimSE)),
        PlacedFamily<PlacedBarrier, IPlacedBarrierGetter>("PlacedBarrier", key => new PlacedBarrier(key, SkyrimRelease.SkyrimSE)),
        PlacedFamily<PlacedBeam, IPlacedBeamGetter>("PlacedBeam", key => new PlacedBeam(key, SkyrimRelease.SkyrimSE)),
        PlacedFamily<PlacedCone, IPlacedConeGetter>("PlacedCone", key => new PlacedCone(key, SkyrimRelease.SkyrimSE)),
        PlacedFamily<PlacedFlame, IPlacedFlameGetter>("PlacedFlame", key => new PlacedFlame(key, SkyrimRelease.SkyrimSE)),
        PlacedFamily<PlacedHazard, IPlacedHazardGetter>("PlacedHazard", key => new PlacedHazard(key, SkyrimRelease.SkyrimSE)),
        PlacedFamily<PlacedMissile, IPlacedMissileGetter>("PlacedMissile", key => new PlacedMissile(key, SkyrimRelease.SkyrimSE)),
        PlacedFamily<PlacedNpc, IPlacedNpcGetter>("PlacedNpc", key => new PlacedNpc(key, SkyrimRelease.SkyrimSE)),
        PlacedFamily<PlacedObject, IPlacedObjectGetter>("PlacedObject", key => new PlacedObject(key, SkyrimRelease.SkyrimSE)),
        PlacedFamily<PlacedTrap, IPlacedTrapGetter>("PlacedTrap", key => new PlacedTrap(key, SkyrimRelease.SkyrimSE)),
    ];

    private static RecordFamily SubclassFamily<TRecord, TGetter, TStored>(
        string familyId,
        Func<FormKey, TRecord> create,
        Func<TGetter, TRecord> copy,
        Func<ISkyrimMod, IGroup<TStored>> group,
        params RecordField<TRecord, TGetter>[] fields)
        where TRecord : class, TStored
        where TStored : class, IMajorRecord
        where TGetter : class, IMajorRecordGetter
    {
        return StoredFamily(
            familyId,
            create,
            copy,
            (mod, formKey) => group(mod).Records.FirstOrDefault(record => record.FormKey == formKey && record is TGetter),
            (mod, record) => group(mod).Set(record),
            (mod, formKey) => group(mod).Remove(formKey),
            fields);
    }

    private static RecordFamily StoredFamily<TRecord, TGetter>(
        string familyId,
        Func<FormKey, TRecord> create,
        Func<TGetter, TRecord> copy,
        Func<ISkyrimMod, FormKey, IMajorRecordGetter?> find,
        Action<ISkyrimMod, TRecord> publish,
        Func<ISkyrimMod, FormKey, bool> remove,
        params RecordField<TRecord, TGetter>[] fields)
        where TRecord : class, IMajorRecord
        where TGetter : class, IMajorRecordGetter
    {
        return new RecordFamily<ISkyrimMod, TRecord, TGetter>(familyId, (_, key) => create(key), copy, find, publish, remove, fields);
    }

    private static RecordFamily CellFamily()
    {
        return StoredFamily<Cell, ICellGetter>("Cell", key => new Cell(key, SkyrimRelease.SkyrimSE), getter => getter.DeepCopy(), FindCell, PublishCell, RemoveCell);
    }

    private static RecordFamily LandscapeFamily()
    {
        return StoredFamily<Landscape, ILandscapeGetter>(
            "Landscape",
            key => new Landscape(key, SkyrimRelease.SkyrimSE),
            getter => getter.DeepCopy(),
            (mod, formKey) => EnumerateCells(mod).Select(cell => cell.Landscape).FirstOrDefault(landscape => landscape?.FormKey == formKey),
            (mod, landscape) => PublishInNewCell(
                mod,
                parent => parent.Landscape = landscape,
                cell =>
                {
                    if (cell.Landscape?.FormKey != landscape.FormKey)
                    {
                        return false;
                    }

                    cell.Landscape = landscape;
                    return true;
                }),
            (mod, formKey) => RemoveFromCells(
                mod,
                cell =>
                {
                    if (cell.Landscape?.FormKey != formKey)
                    {
                        return false;
                    }

                    cell.Landscape = null;
                    return true;
                }));
    }

    private static RecordFamily NavigationMeshFamily()
    {
        return StoredFamily<NavigationMesh, INavigationMeshGetter>(
            "NavigationMesh",
            key => new NavigationMesh(key, SkyrimRelease.SkyrimSE),
            getter => getter.DeepCopy(),
            (mod, formKey) => EnumerateCells(mod).SelectMany(cell => cell.NavigationMeshes).FirstOrDefault(mesh => mesh.FormKey == formKey),
            (mod, mesh) => PublishInNewCell(mod, parent => parent.NavigationMeshes.Add(mesh), cell => TryReplace(cell.NavigationMeshes, mesh)),
            (mod, formKey) => RemoveFromCells(mod, cell => RemoveMatch(cell.NavigationMeshes, formKey)));
    }

    private static RecordFamily PlacedFamily<TRecord, TGetter>(string familyId, Func<FormKey, TRecord> create)
        where TRecord : class, IPlaced, IMajorRecord
        where TGetter : class, IMajorRecordGetter
    {
        return StoredFamily<TRecord, TGetter>(
            familyId,
            create,
            getter => (TRecord)(object)getter.DeepCopy(),
            (mod, formKey) => EnumerateCells(mod).SelectMany(EnumeratePlaced).OfType<TGetter>().FirstOrDefault(placed => placed.FormKey == formKey),
            (mod, placed) => PublishInNewCell(mod, parent => parent.Temporary.Add(placed), cell => TryReplace(cell.Temporary, placed) || TryReplace(cell.Persistent, placed)),
            (mod, formKey) => RemoveFromCells(mod, cell => RemoveMatch(cell.Temporary, formKey) || RemoveMatch(cell.Persistent, formKey)));
    }

    private static RecordFamily DialogResponsesFamily()
    {
        return StoredFamily<DialogResponses, IDialogResponsesGetter>(
            "DialogResponses",
            key => new DialogResponses(key, SkyrimRelease.SkyrimSE),
            getter => getter.DeepCopy(),
            FindDialogResponses,
            PublishDialogResponses,
            RemoveDialogResponses);
    }

    private static IMajorRecordGetter? FindCell(ISkyrimMod mod, FormKey formKey)
    {
        return EnumerateCells(mod).FirstOrDefault(cell => cell.FormKey == formKey);
    }

    private static void PublishCell(ISkyrimMod mod, Cell cell)
    {
        foreach (var block in mod.Cells)
        {
            foreach (var subBlock in block.SubBlocks)
            {
                if (TryReplace(subBlock.Cells, cell))
                {
                    return;
                }
            }
        }

        AddInteriorCell(mod, cell);
    }

    private static bool RemoveCell(ISkyrimMod mod, FormKey formKey)
    {
        foreach (var block in mod.Cells.ToArray())
        {
            foreach (var subBlock in block.SubBlocks.ToArray())
            {
                if (!RemoveMatch(subBlock.Cells, formKey))
                {
                    continue;
                }

                if (subBlock.Cells.Count == 0)
                {
                    block.SubBlocks.Remove(subBlock);
                }

                if (block.SubBlocks.Count == 0)
                {
                    mod.Cells.Remove(block);
                }

                return true;
            }
        }

        return false;
    }

    private static void PublishInNewCell(ISkyrimMod mod, Action<Cell> addToParent, Func<Cell, bool> replaceExisting)
    {
        foreach (var cell in EnumerateCells(mod))
        {
            if (replaceExisting(cell))
            {
                return;
            }
        }

        var parent = new Cell(mod.GetNextFormKey(), SkyrimRelease.SkyrimSE);
        addToParent(parent);
        AddInteriorCell(mod, parent);
    }

    private static bool RemoveFromCells(ISkyrimMod mod, Func<Cell, bool> remove)
    {
        foreach (var cell in EnumerateCells(mod).ToArray())
        {
            if (!remove(cell))
            {
                continue;
            }

            if (CellIsEmpty(cell))
            {
                RemoveCell(mod, cell.FormKey);
            }

            return true;
        }

        return false;
    }

    private static void AddInteriorCell(ISkyrimMod mod, Cell cell)
    {
        mod.Cells.Add(new CellBlock
        {
            BlockNumber = 0,
            GroupType = GroupTypeEnum.InteriorCellBlock,
            SubBlocks =
            [
                new CellSubBlock
                {
                    BlockNumber = 0,
                    GroupType = GroupTypeEnum.InteriorCellSubBlock,
                    Cells = [cell],
                },
            ],
        });
    }

    private static IEnumerable<Cell> EnumerateCells(ISkyrimMod mod)
    {
        foreach (var block in mod.Cells)
        {
            foreach (var subBlock in block.SubBlocks)
            {
                foreach (var cell in subBlock.Cells)
                {
                    yield return cell;
                }
            }
        }
    }

    private static IEnumerable<IPlaced> EnumeratePlaced(Cell cell)
    {
        foreach (var placed in cell.Persistent)
        {
            yield return placed;
        }

        foreach (var placed in cell.Temporary)
        {
            yield return placed;
        }
    }

    private static bool CellIsEmpty(Cell cell)
    {
        return cell.Persistent.Count == 0 && cell.Temporary.Count == 0 && cell.NavigationMeshes.Count == 0 && cell.Landscape is null;
    }

    private static IMajorRecordGetter? FindDialogResponses(ISkyrimMod mod, FormKey formKey)
    {
        foreach (var topic in mod.DialogTopics)
        {
            foreach (var responses in topic.Responses)
            {
                if (responses.FormKey == formKey)
                {
                    return responses;
                }
            }
        }

        return null;
    }

    private static void PublishDialogResponses(ISkyrimMod mod, DialogResponses responses)
    {
        foreach (var topic in mod.DialogTopics)
        {
            if (TryReplace(topic.Responses, responses))
            {
                return;
            }
        }

        var parentTopic = new DialogTopic(mod.GetNextFormKey(), SkyrimRelease.SkyrimSE);
        parentTopic.Responses.Add(responses);
        mod.DialogTopics.Set(parentTopic);
    }

    private static bool RemoveDialogResponses(ISkyrimMod mod, FormKey formKey)
    {
        foreach (var topic in mod.DialogTopics.ToArray())
        {
            if (!RemoveMatch(topic.Responses, formKey))
            {
                continue;
            }

            if (topic.Responses.Count == 0)
            {
                mod.DialogTopics.Remove(topic.FormKey);
            }

            return true;
        }

        return false;
    }

    private static bool TryReplace<TRecord>(IList<TRecord>? records, TRecord record)
        where TRecord : IMajorRecordGetter
    {
        if (records is null)
        {
            return false;
        }

        for (var index = 0; index < records.Count; index++)
        {
            if (records[index].FormKey != record.FormKey)
            {
                continue;
            }

            records[index] = record;
            return true;
        }

        return false;
    }

    private static bool RemoveMatch<TRecord>(IList<TRecord>? records, FormKey formKey)
        where TRecord : IMajorRecordGetter
    {
        if (records is null)
        {
            return false;
        }

        for (var index = records.Count - 1; index >= 0; index--)
        {
            if (records[index].FormKey != formKey)
            {
                continue;
            }

            records.RemoveAt(index);
            return true;
        }

        return false;
    }
}
