using CreationsForge.Engine.Records;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Fallout4;

namespace CreationsForge.Fallout4;

internal static partial class Fallout4RecordFamilies
{
    /// <summary>Gets concrete subclasses and parent-contained major records.</summary>
    private static IReadOnlyList<RecordFamily> ConcreteTypes { get; } =
    [
        SubclassFamily<ArmorModification, IArmorModificationGetter, AObjectModification>("ArmorModification", key => new ArmorModification(key, Fallout4Release.Fallout4), getter => getter.DeepCopy(), mod => mod.ObjectModifications),
        CellFamily(),
        SubclassFamily<DamageType, IDamageTypeGetter, ADamageType>("DamageType", key => new DamageType(key, Fallout4Release.Fallout4), getter => getter.DeepCopy(), mod => mod.DamageTypes),
        DialogChildFamily<DialogBranch, IDialogBranchGetter>("DialogBranch", key => new DialogBranch(key, Fallout4Release.Fallout4), quest => quest.DialogBranches),
        DialogResponsesFamily(),
        DialogChildFamily<DialogTopic, IDialogTopicGetter>("DialogTopic", key => new DialogTopic(key, Fallout4Release.Fallout4), quest => quest.DialogTopics),
        SubclassFamily<GameSettingBool, IGameSettingBoolGetter, GameSetting>("GameSettingBool", key => new GameSettingBool(key, Fallout4Release.Fallout4), getter => getter.DeepCopy(), mod => mod.GameSettings),
        SubclassFamily<GameSettingFloat, IGameSettingFloatGetter, GameSetting>("GameSettingFloat", key => new GameSettingFloat(key, Fallout4Release.Fallout4), getter => getter.DeepCopy(), mod => mod.GameSettings),
        SubclassFamily<GameSettingInt, IGameSettingIntGetter, GameSetting>("GameSettingInt", key => new GameSettingInt(key, Fallout4Release.Fallout4), getter => getter.DeepCopy(), mod => mod.GameSettings),
        SubclassFamily<GameSettingString, IGameSettingStringGetter, GameSetting>("GameSettingString", key => new GameSettingString(key, Fallout4Release.Fallout4), getter => getter.DeepCopy(), mod => mod.GameSettings),
        SubclassFamily<GameSettingUInt, IGameSettingUIntGetter, GameSetting>("GameSettingUInt", key => new GameSettingUInt(key, Fallout4Release.Fallout4), getter => getter.DeepCopy(), mod => mod.GameSettings),
        SubclassFamily<GlobalBool, IGlobalBoolGetter, Global>("GlobalBool", key => new GlobalBool(key, Fallout4Release.Fallout4) { Data = true }, getter => getter.DeepCopy(), mod => mod.Globals),
        SubclassFamily<GlobalFloat, IGlobalFloatGetter, Global>("GlobalFloat", key => new GlobalFloat(key, Fallout4Release.Fallout4) { Data = 1.5f }, getter => getter.DeepCopy(), mod => mod.Globals),
        SubclassFamily<GlobalInt, IGlobalIntGetter, Global>("GlobalInt", key => new GlobalInt(key, Fallout4Release.Fallout4) { Data = 1 }, getter => getter.DeepCopy(), mod => mod.Globals),
        SubclassFamily<GlobalShort, IGlobalShortGetter, Global>("GlobalShort", key => new GlobalShort(key, Fallout4Release.Fallout4) { Data = 1 }, getter => getter.DeepCopy(), mod => mod.Globals),
        Family<Holotape, IHolotapeGetter>("Holotape", key => new Holotape(key, Fallout4Release.Fallout4) { Data = new HolotapeVoice() }, getter => getter.DeepCopy(), mod => mod.Holotapes),
        LandscapeFamily(),
        NavigationMeshFamily(),
        SubclassFamily<NpcModification, INpcModificationGetter, AObjectModification>("NpcModification", key => new NpcModification(key, Fallout4Release.Fallout4), getter => getter.DeepCopy(), mod => mod.ObjectModifications),
        SubclassFamily<ObjectModification, IObjectModificationGetter, AObjectModification>("ObjectModification", key => new ObjectModification(key, Fallout4Release.Fallout4), getter => getter.DeepCopy(), mod => mod.ObjectModifications),
        PlacedFamily<PlacedArrow, IPlacedArrowGetter>("PlacedArrow", key => new PlacedArrow(key, Fallout4Release.Fallout4)),
        PlacedFamily<PlacedBarrier, IPlacedBarrierGetter>("PlacedBarrier", key => new PlacedBarrier(key, Fallout4Release.Fallout4)),
        PlacedFamily<PlacedBeam, IPlacedBeamGetter>("PlacedBeam", key => new PlacedBeam(key, Fallout4Release.Fallout4)),
        PlacedFamily<PlacedCone, IPlacedConeGetter>("PlacedCone", key => new PlacedCone(key, Fallout4Release.Fallout4)),
        PlacedFamily<PlacedFlame, IPlacedFlameGetter>("PlacedFlame", key => new PlacedFlame(key, Fallout4Release.Fallout4)),
        PlacedFamily<PlacedHazard, IPlacedHazardGetter>("PlacedHazard", key => new PlacedHazard(key, Fallout4Release.Fallout4)),
        PlacedFamily<PlacedMissile, IPlacedMissileGetter>("PlacedMissile", key => new PlacedMissile(key, Fallout4Release.Fallout4)),
        PlacedFamily<PlacedNpc, IPlacedNpcGetter>("PlacedNpc", key => new PlacedNpc(key, Fallout4Release.Fallout4)),
        PlacedFamily<PlacedObject, IPlacedObjectGetter>("PlacedObject", key => new PlacedObject(key, Fallout4Release.Fallout4)),
        PlacedFamily<PlacedTrap, IPlacedTrapGetter>("PlacedTrap", key => new PlacedTrap(key, Fallout4Release.Fallout4)),
        DialogChildFamily<Scene, ISceneGetter>("Scene", key => new Scene(key, Fallout4Release.Fallout4), quest => quest.Scenes),
        Family<SoundDescriptor, ISoundDescriptorGetter>("SoundDescriptor", key => new SoundDescriptor(key, Fallout4Release.Fallout4) { Data = new SoundDescriptorStandardData() }, getter => getter.DeepCopy(), mod => mod.SoundDescriptors),
        SubclassFamily<UnknownObjectModification, IUnknownObjectModificationGetter, AObjectModification>("UnknownObjectModification", key => new UnknownObjectModification(key, Fallout4Release.Fallout4), getter => getter.DeepCopy(), mod => mod.ObjectModifications),
        SubclassFamily<WeaponModification, IWeaponModificationGetter, AObjectModification>("WeaponModification", key => new WeaponModification(key, Fallout4Release.Fallout4), getter => getter.DeepCopy(), mod => mod.ObjectModifications),
    ];

    private static RecordFamily SubclassFamily<TRecord, TGetter, TStored>(
        string familyId,
        Func<FormKey, TRecord> create,
        Func<TGetter, TRecord> copy,
        Func<IFallout4Mod, IGroup<TStored>> group,
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
        Func<IFallout4Mod, FormKey, IMajorRecordGetter?> find,
        Action<IFallout4Mod, TRecord> publish,
        Func<IFallout4Mod, FormKey, bool> remove,
        params RecordField<TRecord, TGetter>[] fields)
        where TRecord : class, IMajorRecord
        where TGetter : class, IMajorRecordGetter
    {
        return new RecordFamily<IFallout4Mod, TRecord, TGetter>(familyId, (_, key) => create(key), copy, find, publish, remove, fields);
    }

    private static RecordFamily CellFamily()
    {
        return StoredFamily<Cell, ICellGetter>("Cell", key => new Cell(key, Fallout4Release.Fallout4), getter => getter.DeepCopy(), FindCell, PublishCell, RemoveCell);
    }

    private static RecordFamily LandscapeFamily()
    {
        return StoredFamily<Landscape, ILandscapeGetter>(
            "Landscape",
            key => new Landscape(key, Fallout4Release.Fallout4),
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
            key => new NavigationMesh(key, Fallout4Release.Fallout4),
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

    private static RecordFamily DialogChildFamily<TRecord, TGetter>(string familyId, Func<FormKey, TRecord> create, Func<Quest, IList<TRecord>> list)
        where TRecord : class, IMajorRecord
        where TGetter : class, IMajorRecordGetter
    {
        return StoredFamily<TRecord, TGetter>(
            familyId,
            create,
            getter => (TRecord)(object)getter.DeepCopy(),
            (mod, formKey) => mod.Quests.Select(quest => list((Quest)quest)).SelectMany(children => children).FirstOrDefault(record => record.FormKey == formKey),
            (mod, record) => PublishInNewQuest(mod, parent => list(parent).Add(record), quest => TryReplace(list(quest), record)),
            (mod, formKey) => RemoveFromQuests(mod, quest => RemoveMatch(list(quest), formKey)));
    }

    private static RecordFamily DialogResponsesFamily()
    {
        return StoredFamily<DialogResponses, IDialogResponsesGetter>(
            "DialogResponses",
            key => new DialogResponses(key, Fallout4Release.Fallout4),
            getter => getter.DeepCopy(),
            FindDialogResponses,
            PublishDialogResponses,
            RemoveDialogResponses);
    }

    private static IMajorRecordGetter? FindCell(IFallout4Mod mod, FormKey formKey)
    {
        return EnumerateCells(mod).FirstOrDefault(cell => cell.FormKey == formKey);
    }

    private static void PublishCell(IFallout4Mod mod, Cell cell)
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

    private static bool RemoveCell(IFallout4Mod mod, FormKey formKey)
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

    private static void PublishInNewCell(IFallout4Mod mod, Action<Cell> addToParent, Func<Cell, bool> replaceExisting)
    {
        foreach (var cell in EnumerateCells(mod))
        {
            if (replaceExisting(cell))
            {
                return;
            }
        }

        var parent = new Cell(mod.GetNextFormKey(), Fallout4Release.Fallout4);
        addToParent(parent);
        AddInteriorCell(mod, parent);
    }

    private static bool RemoveFromCells(IFallout4Mod mod, Func<Cell, bool> remove)
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

    private static void AddInteriorCell(IFallout4Mod mod, Cell cell)
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

    private static IEnumerable<Cell> EnumerateCells(IFallout4Mod mod)
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

    private static void PublishInNewQuest(IFallout4Mod mod, Action<Quest> addToParent, Func<Quest, bool> replaceExisting)
    {
        foreach (var quest in mod.Quests.OfType<Quest>())
        {
            if (replaceExisting(quest))
            {
                return;
            }
        }

        var parent = new Quest(mod.GetNextFormKey(), Fallout4Release.Fallout4);
        addToParent(parent);
        mod.Quests.Set(parent);
    }

    private static bool RemoveFromQuests(IFallout4Mod mod, Func<Quest, bool> remove)
    {
        foreach (var quest in mod.Quests.OfType<Quest>().ToArray())
        {
            if (!remove(quest))
            {
                continue;
            }

            if (QuestIsEmpty(quest))
            {
                mod.Quests.Remove(quest.FormKey);
            }

            return true;
        }

        return false;
    }

    private static IMajorRecordGetter? FindDialogResponses(IFallout4Mod mod, FormKey formKey)
    {
        foreach (var quest in mod.Quests)
        {
            foreach (var topic in quest.DialogTopics)
            {
                foreach (var responses in topic.Responses)
                {
                    if (responses.FormKey == formKey)
                    {
                        return responses;
                    }
                }
            }
        }

        return null;
    }

    private static void PublishDialogResponses(IFallout4Mod mod, DialogResponses responses)
    {
        foreach (var quest in mod.Quests.OfType<Quest>())
        {
            foreach (var topic in quest.DialogTopics)
            {
                if (TryReplace(topic.Responses, responses))
                {
                    return;
                }
            }
        }

        var parentTopic = new DialogTopic(mod.GetNextFormKey(), Fallout4Release.Fallout4);
        parentTopic.Responses.Add(responses);
        PublishInNewQuest(mod, quest => quest.DialogTopics.Add(parentTopic), quest => TryReplace(quest.DialogTopics, parentTopic));
    }

    private static bool RemoveDialogResponses(IFallout4Mod mod, FormKey formKey)
    {
        foreach (var quest in mod.Quests.OfType<Quest>().ToArray())
        {
            foreach (var topic in quest.DialogTopics.ToArray())
            {
                if (!RemoveMatch(topic.Responses, formKey))
                {
                    continue;
                }

                if (topic.Responses.Count == 0)
                {
                    RemoveMatch(quest.DialogTopics, topic.FormKey);
                }

                if (QuestIsEmpty(quest))
                {
                    mod.Quests.Remove(quest.FormKey);
                }

                return true;
            }
        }

        return false;
    }

    private static bool QuestIsEmpty(IQuestGetter quest)
    {
        return quest.DialogBranches.Count == 0 && quest.DialogTopics.Count == 0 && quest.Scenes.Count == 0;
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
