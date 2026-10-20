using CreationsForge.Engine.Records;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Starfield;
using Noggog;
using Activator = Mutagen.Bethesda.Starfield.Activator;

namespace CreationsForge.Starfield;

/// <summary>Declares the Starfield record families the engine can create, override, and edit.</summary>
internal static partial class StarfieldRecordFamilies
{
    /// <summary>Gets all concrete record family registrations.</summary>
    public static IReadOnlyList<RecordFamily> All { get; private set; } = [];

    static StarfieldRecordFamilies()
    {
        All = [..Core, ..Additional, ..ConcreteTypes];
    }

    private static IReadOnlyList<RecordFamily> Core { get; } =
    [
        Family<FormList, IFormListGetter>("FormList", key => new FormList(key, StarfieldRelease.Starfield), getter => getter.DeepCopy(), mod => mod.FormLists,
            RecordFields.TranslatedString<FormList, IFormListGetter>("Name", false, getter => getter.Name, (record, value) => record.Name = value!),
            RecordFields.FormLinkList<FormList, IFormListGetter>("Items", [typeof(IStarfieldMajorRecordGetter)], getter => getter.Items.Select(item => item.FormKey).ToArray(), ReplaceFormListItems)),
        Family<Global, IGlobalGetter>("Global", key => new Global(key, StarfieldRelease.Starfield), getter => getter.DeepCopy(), mod => mod.Globals,
            RecordFields.NullableSingle<Global, IGlobalGetter>("Data", getter => getter.Data, (record, value) => record.Data = value),
            RecordFields.Enum<Global, IGlobalGetter, Global.MajorFlag>("MajorFlags", false, getter => getter.MajorFlags, (record, value) => record.MajorFlags = value!.Value)),
        Family<Keyword, IKeywordGetter>("Keyword", key => new Keyword(key, StarfieldRelease.Starfield), getter => getter.DeepCopy(), mod => mod.Keywords,
            RecordFields.TranslatedString<Keyword, IKeywordGetter>("Name", false, getter => getter.Name, (record, value) => record.Name = value!),
            RecordFields.String<Keyword, IKeywordGetter>("Notes", true, getter => getter.Notes, (record, value) => record.Notes = value),
            RecordFields.Enum<Keyword, IKeywordGetter, Keyword.TypeEnum>("Type", true, getter => getter.Type, (record, value) => record.Type = value),
            RecordFields.FormLink<Keyword, IKeywordGetter>("AttractionRule", [typeof(IAttractionRuleGetter)], getter => getter.AttractionRule.FormKey, (record, value) => record.AttractionRule.SetTo(value))),
        Family<Message, IMessageGetter>("Message", key => new Message(key, StarfieldRelease.Starfield), getter => getter.DeepCopy(), mod => mod.Messages,
            RecordFields.TranslatedString<Message, IMessageGetter>("Description", false, getter => getter.Description, (record, value) => record.Description = value!),
            RecordFields.TranslatedString<Message, IMessageGetter>("Name", true, getter => getter.Name, (record, value) => record.Name = value),
            RecordFields.NullableUInt32<Message, IMessageGetter>("DisplayTime", getter => getter.DisplayTime, (record, value) => record.DisplayTime = value),
            RecordFields.Enum<Message, IMessageGetter, Message.Flag>("Flags", false, getter => getter.Flags, (record, value) => record.Flags = value!.Value),
            RecordFields.FormLink<Message, IMessageGetter>("OwnerQuest", [typeof(IQuestGetter)], getter => getter.OwnerQuest.FormKey, (record, value) => record.OwnerQuest.SetTo(value))),
        Family<Quest, IQuestGetter>("Quest", key => new Quest(key, StarfieldRelease.Starfield), getter => getter.DeepCopy(), mod => mod.Quests,
            RecordFields.TranslatedString<Quest, IQuestGetter>("Name", true, getter => getter.Name, (record, value) => record.Name = value),
            RecordFields.Enum<Quest, IQuestGetter, Quest.MajorFlag>("MajorFlags", false, getter => getter.MajorFlags, (record, value) => record.MajorFlags = value!.Value),
            RecordFields.FormLinkList<Quest, IQuestGetter>("Keywords", [typeof(IKeywordGetter)], getter => getter.Keywords?.Select(item => item.FormKey).ToArray() ?? [], ReplaceQuestKeywords),
            RecordFields.FormLink<Quest, IQuestGetter>("SourceQuest", [typeof(IQuestGetter)], getter => getter.SourceQuest.FormKey, (record, value) => record.SourceQuest.SetTo(value))),
        Family<Faction, IFactionGetter>("Faction", key => new Faction(key, StarfieldRelease.Starfield), getter => getter.DeepCopy(), mod => mod.Factions,
            RecordFields.TranslatedString<Faction, IFactionGetter>("Name", true, getter => getter.Name, (record, value) => record.Name = value),
            RecordFields.Enum<Faction, IFactionGetter, Faction.FactionFlag>("Flags", false, getter => getter.Flags, (record, value) => record.Flags = value!.Value),
            RecordFields.FormLink<Faction, IFactionGetter>("SharedCrimeFactionList", [typeof(IFormListGetter)], getter => getter.SharedCrimeFactionList.FormKey, (record, value) => record.SharedCrimeFactionList.SetTo(value))),
        Family<Race, IRaceGetter>("Race", key => new Race(key, StarfieldRelease.Starfield), getter => getter.DeepCopy(), mod => mod.Races,
            RecordFields.TranslatedString<Race, IRaceGetter>("Name", false, getter => getter.Name, (record, value) => record.Name = value!),
            RecordFields.TranslatedString<Race, IRaceGetter>("Description", false, getter => getter.Description, (record, value) => record.Description = value!),
            RecordFields.Enum<Race, IRaceGetter, Race.Flag>("Flags", false, getter => getter.Flags, (record, value) => record.Flags = value!.Value)),
        Family<TextureSet, ITextureSetGetter>("TextureSet", key => new TextureSet(key, StarfieldRelease.Starfield), getter => getter.DeepCopy(), mod => mod.TextureSets,
            RecordFields.Enum<TextureSet, ITextureSetGetter, TextureSet.Flag>("Flags", false, getter => getter.Flags, (record, value) => record.Flags = value!.Value)),
        Family<Armor, IArmorGetter>("Armor", key => new Armor(key, StarfieldRelease.Starfield), getter => getter.DeepCopy(), mod => mod.Armors,
            RecordFields.TranslatedString<Armor, IArmorGetter>("Name", true, getter => getter.Name, (record, value) => record.Name = value),
            RecordFields.TranslatedString<Armor, IArmorGetter>("Description", true, getter => getter.Description, (record, value) => record.Description = value),
            RecordFields.Int32<Armor, IArmorGetter>("Value", getter => getter.Value, (record, value) => record.Value = value),
            RecordFields.Single<Armor, IArmorGetter>("Weight", getter => getter.Weight, (record, value) => record.Weight = value),
            RecordFields.Object<Armor, IArmorGetter>("ObjectBounds", false, ["ObjectBounds"], ReadObjectBounds, ValidateObjectBounds, WriteObjectBounds),
            RecordFields.FormLinkList<Armor, IArmorGetter>("Keywords", [typeof(IKeywordGetter)], getter => getter.Keywords?.Select(item => item.FormKey).ToArray() ?? [], ReplaceArmorKeywords)),
        Family<Book, IBookGetter>("Book", key => new Book(key, StarfieldRelease.Starfield), getter => getter.DeepCopy(), mod => mod.Books,
            RecordFields.TranslatedString<Book, IBookGetter>("Name", true, getter => getter.Name, (record, value) => record.Name = value),
            RecordFields.TranslatedString<Book, IBookGetter>("Description", true, getter => getter.Description, (record, value) => record.Description = value),
            RecordFields.UInt32<Book, IBookGetter>("Value", getter => getter.Value, (record, value) => record.Value = value),
            RecordFields.Single<Book, IBookGetter>("Weight", getter => getter.Weight, (record, value) => record.Weight = value),
            RecordFields.Object<Book, IBookGetter>("Teaches", false, ["ActorValue", "Perk", "Spell", "None"], ReadTeachTarget, ValidateTeachTarget, WriteTeachTarget, new Dictionary<string, Type>
            {
                ["ActorValue"] = typeof(IActorValueInformationGetter),
                ["Perk"] = typeof(IPerkGetter),
                ["Spell"] = typeof(ISpellGetter),
            }),
            RecordFields.FormLinkList<Book, IBookGetter>("Keywords", [typeof(IKeywordGetter)], getter => getter.Keywords?.Select(item => item.FormKey).ToArray() ?? [], ReplaceBookKeywords)),
        Family<Activator, IActivatorGetter>("Activator", key => new Activator(key, StarfieldRelease.Starfield), getter => getter.DeepCopy(), mod => mod.Activators,
            RecordFields.TranslatedString<Activator, IActivatorGetter>("Name", true, getter => getter.Name, (record, value) => record.Name = value),
            FloatObjectBounds<Activator, IActivatorGetter>(getter => getter.ObjectBounds, (record, value) => record.ObjectBounds = value),
            KeywordList<Activator, IActivatorGetter>(getter => getter.Keywords, (record, values) => AssignKeywords(record.Keywords ??= [], values)),
            RecordFields.TranslatedString<Activator, IActivatorGetter>("ActivateTextOverride", true, getter => getter.ActivateTextOverride, (record, value) => record.ActivateTextOverride = value),
            RecordFields.Enum<Activator, IActivatorGetter, Activator.Flag>("Flags", true, getter => getter.Flags, (record, value) => record.Flags = value)),
        Family<Ammunition, IAmmunitionGetter>("Ammunition", key => new Ammunition(key, StarfieldRelease.Starfield), getter => getter.DeepCopy(), mod => mod.Ammunitions,
            RecordFields.TranslatedString<Ammunition, IAmmunitionGetter>("Name", true, getter => getter.Name, (record, value) => record.Name = value),
            RecordFields.TranslatedString<Ammunition, IAmmunitionGetter>("Description", false, getter => getter.Description, (record, value) => record.Description = value!),
            RecordFields.UInt32<Ammunition, IAmmunitionGetter>("Value", getter => getter.Value, (record, value) => record.Value = value),
            RecordFields.Single<Ammunition, IAmmunitionGetter>("Weight", getter => getter.Weight, (record, value) => record.Weight = value),
            KeywordList<Ammunition, IAmmunitionGetter>(getter => getter.Keywords, (record, values) => AssignKeywords(record.Keywords ??= [], values)),
            RecordFields.Enum<Ammunition, IAmmunitionGetter, Ammunition.Flag>("Flags", false, getter => getter.Flags, (record, value) => record.Flags = value!.Value)),
        Family<Container, IContainerGetter>("Container", key => new Container(key, StarfieldRelease.Starfield), getter => getter.DeepCopy(), mod => mod.Containers,
            RecordFields.TranslatedString<Container, IContainerGetter>("Name", true, getter => getter.Name, (record, value) => record.Name = value),
            FloatObjectBounds<Container, IContainerGetter>(getter => getter.ObjectBounds, (record, value) => record.ObjectBounds = value),
            KeywordList<Container, IContainerGetter>(getter => getter.Keywords, (record, values) => AssignKeywords(record.Keywords ??= [], values)),
            RecordFields.Enum<Container, IContainerGetter, Container.Flag>("Flags", false, getter => getter.Flags, (record, value) => record.Flags = value!.Value)),
        Family<Door, IDoorGetter>("Door", key => new Door(key, StarfieldRelease.Starfield), getter => getter.DeepCopy(), mod => mod.Doors,
            RecordFields.TranslatedString<Door, IDoorGetter>("Name", true, getter => getter.Name, (record, value) => record.Name = value),
            FloatObjectBounds<Door, IDoorGetter>(getter => getter.ObjectBounds, (record, value) => record.ObjectBounds = value),
            KeywordList<Door, IDoorGetter>(getter => getter.Keywords, (record, values) => AssignKeywords(record.Keywords ??= [], values)),
            RecordFields.Enum<Door, IDoorGetter, Door.Flag>("Flags", false, getter => getter.Flags, (record, value) => record.Flags = value!.Value),
            RecordFields.TranslatedString<Door, IDoorGetter>("AlternateTextOpen", true, getter => getter.AlternateTextOpen, (record, value) => record.AlternateTextOpen = value),
            RecordFields.TranslatedString<Door, IDoorGetter>("AlternateTextClose", true, getter => getter.AlternateTextClose, (record, value) => record.AlternateTextClose = value)),
        Family<Flora, IFloraGetter>("Flora", key => new Flora(key, StarfieldRelease.Starfield), getter => getter.DeepCopy(), mod => mod.Florae,
            RecordFields.TranslatedString<Flora, IFloraGetter>("Name", true, getter => getter.Name, (record, value) => record.Name = value),
            FloatObjectBounds<Flora, IFloraGetter>(getter => getter.ObjectBounds, (record, value) => record.ObjectBounds = value),
            KeywordList<Flora, IFloraGetter>(getter => getter.Keywords, (record, values) => AssignKeywords(record.Keywords ??= [], values)),
            RecordFields.TranslatedString<Flora, IFloraGetter>("ActivateTextOverride", true, getter => getter.ActivateTextOverride, (record, value) => record.ActivateTextOverride = value)),
        Family<Furniture, IFurnitureGetter>("Furniture", key => new Furniture(key, StarfieldRelease.Starfield), getter => getter.DeepCopy(), mod => mod.Furniture,
            RecordFields.TranslatedString<Furniture, IFurnitureGetter>("Name", true, getter => getter.Name, (record, value) => record.Name = value),
            FloatObjectBounds<Furniture, IFurnitureGetter>(getter => getter.ObjectBounds, (record, value) => record.ObjectBounds = value),
            KeywordList<Furniture, IFurnitureGetter>(getter => getter.Keywords, (record, values) => AssignKeywords(record.Keywords ??= [], values)),
            RecordFields.TranslatedString<Furniture, IFurnitureGetter>("ActivateTextOverride", true, getter => getter.ActivateTextOverride, (record, value) => record.ActivateTextOverride = value),
            RecordFields.Enum<Furniture, IFurnitureGetter, Furniture.Flag>("Flags", true, getter => getter.Flags, (record, value) => record.Flags = value)),
        Family<Ingestible, IIngestibleGetter>("Ingestible", key => new Ingestible(key, StarfieldRelease.Starfield), getter => getter.DeepCopy(), mod => mod.Ingestibles,
            RecordFields.TranslatedString<Ingestible, IIngestibleGetter>("Name", true, getter => getter.Name, (record, value) => record.Name = value),
            RecordFields.TranslatedString<Ingestible, IIngestibleGetter>("Description", false, getter => getter.Description, (record, value) => record.Description = value!),
            RecordFields.UInt32<Ingestible, IIngestibleGetter>("Value", getter => getter.Value, (record, value) => record.Value = value),
            RecordFields.Single<Ingestible, IIngestibleGetter>("Weight", getter => getter.Weight, (record, value) => record.Weight = value),
            KeywordList<Ingestible, IIngestibleGetter>(getter => getter.Keywords, (record, values) => AssignKeywords(record.Keywords ??= [], values)),
            RecordFields.Enum<Ingestible, IIngestibleGetter, Ingestible.Flag>("Flags", false, getter => getter.Flags, (record, value) => record.Flags = value!.Value)),
        Family<Key, IKeyGetter>("Key", key => new Key(key, StarfieldRelease.Starfield), getter => getter.DeepCopy(), mod => mod.Keys,
            RecordFields.TranslatedString<Key, IKeyGetter>("Name", true, getter => getter.Name, (record, value) => record.Name = value),
            FloatObjectBounds<Key, IKeyGetter>(getter => getter.ObjectBounds, (record, value) => record.ObjectBounds = value),
            KeywordList<Key, IKeyGetter>(getter => getter.Keywords, (record, values) => AssignKeywords(record.Keywords ??= [], values)),
            RecordFields.UInt32<Key, IKeyGetter>("Value", getter => getter.Value, (record, value) => record.Value = value),
            RecordFields.Single<Key, IKeyGetter>("Weight", getter => getter.Weight, (record, value) => record.Weight = value)),
        Family<LeveledItem, ILeveledItemGetter>("LeveledItem", key => new LeveledItem(key, StarfieldRelease.Starfield), getter => getter.DeepCopy(), mod => mod.LeveledItems,
            FloatObjectBounds<LeveledItem, ILeveledItemGetter>(getter => getter.ObjectBounds, (record, value) => record.ObjectBounds = value),
            RecordFields.Enum<LeveledItem, ILeveledItemGetter, LeveledItem.Flag>("Flags", false, getter => getter.Flags, (record, value) => record.Flags = value!.Value),
            RecordFields.TranslatedString<LeveledItem, ILeveledItemGetter>("OverrideName", true, getter => getter.OverrideName, (record, value) => record.OverrideName = value)),
        Family<MiscItem, IMiscItemGetter>("MiscItem", key => new MiscItem(key, StarfieldRelease.Starfield), getter => getter.DeepCopy(), mod => mod.MiscItems,
            RecordFields.TranslatedString<MiscItem, IMiscItemGetter>("Name", true, getter => getter.Name, (record, value) => record.Name = value),
            FloatObjectBounds<MiscItem, IMiscItemGetter>(getter => getter.ObjectBounds, (record, value) => record.ObjectBounds = value),
            KeywordList<MiscItem, IMiscItemGetter>(getter => getter.Keywords, (record, values) => AssignKeywords(record.Keywords ??= [], values)),
            RecordFields.Int32<MiscItem, IMiscItemGetter>("Value", getter => getter.Value, (record, value) => record.Value = value),
            RecordFields.Single<MiscItem, IMiscItemGetter>("Weight", getter => getter.Weight, (record, value) => record.Weight = value)),
        Family<Static, IStaticGetter>("Static", key => new Static(key, StarfieldRelease.Starfield), getter => getter.DeepCopy(), mod => mod.Statics,
            RecordFields.TranslatedString<Static, IStaticGetter>("Name", true, getter => getter.Name, (record, value) => record.Name = value),
            FloatObjectBounds<Static, IStaticGetter>(getter => getter.ObjectBounds, (record, value) => record.ObjectBounds = value)),
        Family<Weapon, IWeaponGetter>("Weapon", key => new Weapon(key, StarfieldRelease.Starfield), getter => getter.DeepCopy(), mod => mod.Weapons,
            RecordFields.TranslatedString<Weapon, IWeaponGetter>("Name", true, getter => getter.Name, (record, value) => record.Name = value),
            RecordFields.TranslatedString<Weapon, IWeaponGetter>("Description", true, getter => getter.Description, (record, value) => record.Description = value),
            FloatObjectBounds<Weapon, IWeaponGetter>(getter => getter.ObjectBounds, (record, value) => record.ObjectBounds = value),
            KeywordList<Weapon, IWeaponGetter>(getter => getter.Keywords, (record, values) => AssignKeywords(record.Keywords ??= [], values))),
    ];

    private static RecordFamily Family<TRecord, TGetter>(
        string familyId,
        Func<Mutagen.Bethesda.Plugins.FormKey, TRecord> create,
        Func<TGetter, TRecord> copy,
        Func<IStarfieldMod, IGroup<TRecord>> group,
        params RecordField<TRecord, TGetter>[] fields)
        where TRecord : class, IMajorRecord
        where TGetter : class, IMajorRecordGetter
    {
        return new RecordFamily<IStarfieldMod, TRecord, TGetter>(
            familyId,
            (_, key) => create(key),
            copy,
            group,
            fields);
    }

    private static void ReplaceFormListItems(FormList record, IReadOnlyList<FormKey> values)
    {
        record.Items.Clear();
        record.Items.AddRange(values.Select(value => new FormLink<IStarfieldMajorRecordGetter>(value)));
    }

    private static void ReplaceQuestKeywords(Quest record, IReadOnlyList<FormKey> values)
    {
        record.Keywords ??= [];
        ReplaceKeywordLinks(record.Keywords, values);
    }

    private static void ReplaceArmorKeywords(Armor record, IReadOnlyList<FormKey> values)
    {
        record.Keywords ??= [];
        ReplaceKeywordLinks(record.Keywords, values);
    }

    private static void ReplaceBookKeywords(Book record, IReadOnlyList<FormKey> values)
    {
        record.Keywords ??= [];
        ReplaceKeywordLinks(record.Keywords, values);
    }

    private static void ReplaceKeywordLinks(ICollection<IFormLinkGetter<IKeywordGetter>> links, IReadOnlyList<FormKey> values)
    {
        links.Clear();
        foreach (var value in values)
        {
            links.Add(new FormLink<IKeywordGetter>(value));
        }
    }

    private static void AssignKeywords(ICollection<IFormLinkGetter<IKeywordGetter>> links, IReadOnlyList<FormKey> values)
    {
        ReplaceKeywordLinks(links, values);
    }

    private static void AssignLinks<TTarget>(ICollection<IFormLinkGetter<TTarget>>? links, IReadOnlyList<FormKey> values)
        where TTarget : class, IMajorRecordGetter
    {
        if (links is null)
        {
            throw new RecordEditingException("The link list is not initialized on this record.");
        }

        links.Clear();
        foreach (var value in values)
        {
            links.Add(new FormLink<TTarget>(value));
        }
    }

    private static RecordField<TRecord, TGetter> KeywordList<TRecord, TGetter>(
        Func<TGetter, IEnumerable<IFormLinkGetter<IKeywordGetter>>?> read,
        Action<TRecord, IReadOnlyList<FormKey>> replace)
        where TRecord : class, IMajorRecord
        where TGetter : class, IMajorRecordGetter
    {
        return RecordFields.FormLinkList<TRecord, TGetter>(
            "Keywords",
            [typeof(IKeywordGetter)],
            getter => read(getter)?.Select(item => item.FormKey).ToArray() ?? [],
            replace);
    }

    private static RecordField<TRecord, TGetter> FloatObjectBounds<TRecord, TGetter>(
        Func<TGetter, IObjectBoundsGetter?> read,
        Action<TRecord, ObjectBounds> write)
        where TRecord : class, IMajorRecord
        where TGetter : class, IMajorRecordGetter
    {
        return RecordFields.Object<TRecord, TGetter>(
            "ObjectBounds",
            false,
            ["ObjectBounds"],
            getter => ReadFloatBounds(read(getter)),
            ValidateFloatBounds,
            (record, value) => write(record, WriteFloatBounds(value)));
    }

    private static RecordValue ReadFloatBounds(IObjectBoundsGetter? bounds)
    {
        if (bounds is not { } value)
        {
            return RecordValue.Null;
        }

        return RecordValue.FromObject("ObjectBounds", new Dictionary<string, RecordValue>
        {
            ["First.X"] = RecordValue.FromFloatingPoint(value.First.X),
            ["First.Y"] = RecordValue.FromFloatingPoint(value.First.Y),
            ["First.Z"] = RecordValue.FromFloatingPoint(value.First.Z),
            ["Second.X"] = RecordValue.FromFloatingPoint(value.Second.X),
            ["Second.Y"] = RecordValue.FromFloatingPoint(value.Second.Y),
            ["Second.Z"] = RecordValue.FromFloatingPoint(value.Second.Z),
        });
    }

    private static void ValidateFloatBounds(RecordValue value)
    {
        var fields = ((RecordValue.ObjectRecordValue)value).Fields;
        var expected = new[] { "First.X", "First.Y", "First.Z", "Second.X", "Second.Y", "Second.Z" };
        if (fields.Count != expected.Length
            || expected.Any(path => !fields.TryGetValue(path, out var field) || field is not RecordValue.FloatingPointRecordValue floating || floating.Value is < -float.MaxValue or > float.MaxValue))
        {
            throw new RecordEditingException("ObjectBounds requires exactly six finite Single child fields.");
        }
    }

    private static ObjectBounds WriteFloatBounds(RecordValue value)
    {
        var fields = ((RecordValue.ObjectRecordValue)value).Fields;
        return new ObjectBounds
        {
            First = new P3Float(Float(fields, "First.X"), Float(fields, "First.Y"), Float(fields, "First.Z")),
            Second = new P3Float(Float(fields, "Second.X"), Float(fields, "Second.Y"), Float(fields, "Second.Z")),
        };
    }

    private static RecordValue ReadObjectBounds(IArmorGetter record)
    {
        if (record.ObjectBounds is not { } bounds)
        {
            return RecordValue.Null;
        }

        return RecordValue.FromObject("ObjectBounds", new Dictionary<string, RecordValue>
        {
            ["First.X"] = RecordValue.FromFloatingPoint(bounds.First.X),
            ["First.Y"] = RecordValue.FromFloatingPoint(bounds.First.Y),
            ["First.Z"] = RecordValue.FromFloatingPoint(bounds.First.Z),
            ["Second.X"] = RecordValue.FromFloatingPoint(bounds.Second.X),
            ["Second.Y"] = RecordValue.FromFloatingPoint(bounds.Second.Y),
            ["Second.Z"] = RecordValue.FromFloatingPoint(bounds.Second.Z),
        });
    }

    private static void ValidateObjectBounds(RecordValue value)
    {
        var fields = ((RecordValue.ObjectRecordValue)value).Fields;
        var expected = new[] { "First.X", "First.Y", "First.Z", "Second.X", "Second.Y", "Second.Z" };
        if (fields.Count != expected.Length
            || expected.Any(path => !fields.TryGetValue(path, out var field) || field is not RecordValue.FloatingPointRecordValue floating || floating.Value is < -float.MaxValue or > float.MaxValue))
        {
            throw new RecordEditingException("Starfield Armor.ObjectBounds requires exactly six finite Single child fields.");
        }
    }

    private static void WriteObjectBounds(Armor record, RecordValue value)
    {
        var fields = ((RecordValue.ObjectRecordValue)value).Fields;
        record.ObjectBounds = new ObjectBounds
        {
            First = new P3Float(Float(fields, "First.X"), Float(fields, "First.Y"), Float(fields, "First.Z")),
            Second = new P3Float(Float(fields, "Second.X"), Float(fields, "Second.Y"), Float(fields, "Second.Z")),
        };
    }

    private static float Float(IReadOnlyDictionary<string, RecordValue> fields, string path)
    {
        return (float)((RecordValue.FloatingPointRecordValue)fields[path]).Value;
    }

    private static RecordValue ReadTeachTarget(IBookGetter record)
    {
        return record.Teaches switch
        {
            IBookActorValueGetter target => LinkAlternative("ActorValue", target.ActorValue.FormKey),
            IBookPerkGetter target => LinkAlternative("Perk", target.Perk.FormKey),
            IBookSpellGetter target => LinkAlternative("Spell", target.Spell.FormKey),
            IBookTeachesNothingGetter target => RecordValue.FromObject("None", new Dictionary<string, RecordValue>
            {
                ["RawContent"] = RecordValue.FromUnsignedInteger(target.RawContent),
            }),
            _ => throw new RecordEditingException($"Unsupported Starfield Book.Teaches subtype '{record.Teaches?.GetType().FullName}'."),
        };
    }

    private static void ValidateTeachTarget(RecordValue value)
    {
        var target = (RecordValue.ObjectRecordValue)value;
        if (target.Alternative == "None")
        {
            if (target.Fields.Count != 1
                || !target.Fields.TryGetValue("RawContent", out var raw)
                || raw is not RecordValue.UnsignedIntegerRecordValue rawValue
                || rawValue.Value > uint.MaxValue)
            {
                throw new RecordEditingException("Starfield Book.Teaches None requires one UInt32 RawContent field.");
            }

            return;
        }

        if (target.Fields.Count != 1 || !target.Fields.TryGetValue("FormKey", out var link) || link is not RecordValue.FormLinkRecordValue)
        {
            throw new RecordEditingException($"Starfield Book.Teaches {target.Alternative} requires one FormKey field.");
        }
    }

    private static void WriteTeachTarget(Book record, RecordValue value)
    {
        var target = (RecordValue.ObjectRecordValue)value;
        record.Teaches = target.Alternative switch
        {
            "ActorValue" => new BookActorValue { ActorValue = new FormLink<IActorValueInformationGetter>(Link(target)) },
            "Perk" => new BookPerk { Perk = new FormLink<IPerkGetter>(Link(target)) },
            "Spell" => new BookSpell { Spell = new FormLink<ISpellGetter>(Link(target)) },
            "None" => new BookTeachesNothing { RawContent = (uint)((RecordValue.UnsignedIntegerRecordValue)target.Fields["RawContent"]).Value },
            _ => throw new RecordEditingException($"Unsupported Starfield Book.Teaches alternative '{target.Alternative}'."),
        };
    }

    private static RecordValue LinkAlternative(string alternative, FormKey formKey)
    {
        return RecordValue.FromObject(alternative, new Dictionary<string, RecordValue>
        {
            ["FormKey"] = RecordValue.FromFormLink(formKey),
        });
    }

    private static FormKey Link(RecordValue.ObjectRecordValue value)
    {
        return ((RecordValue.FormLinkRecordValue)value.Fields["FormKey"]).FormKey;
    }
}
