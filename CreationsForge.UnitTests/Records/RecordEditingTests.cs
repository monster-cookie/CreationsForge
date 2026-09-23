using CreationsForge.Engine.Records;
using CreationsForge.Engine.Workspaces;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Strings;

namespace CreationsForge.UnitTests.Workspaces;

/// <summary>Verifies the first record editing tranche through the production workspace boundary.</summary>
public sealed partial class PluginWorkspaceTests
{
    private static readonly string[] FirstTrancheFamilies =
    [
        "Armor",
        "Book",
        "Faction",
        "FormList",
        "Global",
        "Keyword",
        "Message",
        "Quest",
        "Race",
        "TextureSet",
    ];

    /// <summary>Creates and reads every declared first-tranche family with one atomic revision increment.</summary>
    [Theory]
    [InlineData(GameRelease.Starfield)]
    [InlineData(GameRelease.Fallout4)]
    [InlineData(GameRelease.SkyrimSE)]
    public void CreatesAndReadsEveryDeclaredFirstTrancheFamily(GameRelease release)
    {
        using var directory = new TemporaryDirectory();
        PrepareRequiredStarfieldMaster(directory.Path, release);
        var outputModKey = ModKey.FromNameAndExtension("Output.esp");
        using var workspace = CreateFactory().Open(CreateNewRequest(directory.Path, release, [], outputModKey));
        var mutations = workspace.Records.Families.Select(descriptor =>
        {
            var changes = new List<RecordFieldChange>
            {
                Set("EditorID", RecordValue.FromString($"Created{descriptor.FamilyId}")),
            };
            if (descriptor.FamilyId == "Book")
            {
                changes.Add(Set("Teaches", NoneTeachTarget()));
            }

            return RecordMutation.Create(descriptor.FamilyId, changes);
        });

        var result = workspace.Records.Apply(new RecordChangeSet(0, mutations));

        Assert.Equal(FirstTrancheFamilies, workspace.Records.Families.Select(family => family.FamilyId));
        Assert.Equal(1UL, result.Revision);
        Assert.Equal(10, result.Records.Count);
        Assert.Equal(10, workspace.Output.EnumerateMajorRecords().Count());
        foreach (var record in result.Records)
        {
            var read = workspace.Records.Read(new RecordLocator(record.FamilyId, record.FormKey, outputModKey));
            var editorId = Assert.IsType<RecordValue.StringRecordValue>(read.Values["EditorID"]);
            Assert.Equal($"Created{record.FamilyId}", editorId.Value);
        }
    }

    /// <summary>Overrides, inspects, and compares every declared family while preserving exact source records.</summary>
    [Theory]
    [InlineData(GameRelease.Starfield)]
    [InlineData(GameRelease.Fallout4)]
    [InlineData(GameRelease.SkyrimSE)]
    public void OverridesAndComparesEveryDeclaredFirstTrancheFamily(GameRelease release)
    {
        using var directory = new TemporaryDirectory();
        var fixture = CreateRecordEditingFixture(directory.Path, release);
        var outputModKey = ModKey.FromNameAndExtension("Output.esp");
        using var workspace = CreateFactory().Open(CreateNewRequest(directory.Path, release, [fixture.ModKey], outputModKey));
        var mutations = fixture.FormKeys.Select(pair => RecordMutation.Override(
            pair.Key,
            pair.Value,
            fixture.ModKey,
            [Set("EditorID", RecordValue.FromString($"Overridden{pair.Key}"))]));

        var result = workspace.Records.Apply(new RecordChangeSet(0, mutations));

        Assert.Equal(1UL, result.Revision);
        Assert.Equal(10, result.Records.Count);
        foreach (var pair in fixture.FormKeys)
        {
            var comparison = workspace.Records.Compare(
                new RecordLocator(pair.Key, pair.Value, fixture.ModKey),
                new RecordLocator(pair.Key, pair.Value, outputModKey));
            var difference = Assert.Single(comparison.Differences, item => item.Path == "EditorID");
            Assert.Equal($"Source{pair.Key}", Assert.IsType<RecordValue.StringRecordValue>(difference.Left).Value);
            Assert.Equal($"Overridden{pair.Key}", Assert.IsType<RecordValue.StringRecordValue>(difference.Right).Value);
        }
    }

    /// <summary>Preserves nulls, ordered duplicate links, translations, nested values, and polymorphic choices in record candidates.</summary>
    [Fact]
    public void AppliesReusableFieldShapesWithoutFlatteningRecordValues()
    {
        using var directory = new TemporaryDirectory();
        var fixture = CreateRecordEditingFixture(directory.Path, GameRelease.Starfield);
        var outputModKey = ModKey.FromNameAndExtension("Output.esp");
        using var workspace = CreateFactory().Open(CreateNewRequest(directory.Path, GameRelease.Starfield, [fixture.ModKey], outputModKey));
        var keywordKey = fixture.FormKeys["Keyword"];
        var formListKey = fixture.FormKeys["FormList"];
        var armorKey = fixture.FormKeys["Armor"];
        var bookKey = fixture.FormKeys["Book"];
        var translated = RecordValue.FromTranslatedString(
            Language.French,
            new Dictionary<Language, string>
            {
                [Language.English] = "English name",
                [Language.French] = "Nom français",
            });
        var bounds = RecordValue.FromObject("ObjectBounds", new Dictionary<string, RecordValue>
        {
            ["First.X"] = RecordValue.FromFloatingPoint(-1),
            ["First.Y"] = RecordValue.FromFloatingPoint(-2),
            ["First.Z"] = RecordValue.FromFloatingPoint(-3),
            ["Second.X"] = RecordValue.FromFloatingPoint(4),
            ["Second.Y"] = RecordValue.FromFloatingPoint(5),
            ["Second.Z"] = RecordValue.FromFloatingPoint(6),
        });
        var changes = new RecordChangeSet(0,
        [
            RecordMutation.Override("FormList", formListKey, fixture.ModKey,
            [
                Set("Name", translated),
                Set("Items", RecordValue.FromList([RecordValue.FromFormLink(keywordKey), RecordValue.FromFormLink(keywordKey)])),
            ]),
            RecordMutation.Override("Armor", armorKey, fixture.ModKey,
            [
                Set("ObjectBounds", bounds),
                Set("Keywords", RecordValue.FromList([RecordValue.FromFormLink(keywordKey), RecordValue.FromFormLink(keywordKey)])),
            ]),
            RecordMutation.Override("Book", bookKey, fixture.ModKey,
            [
                Set("Teaches", NoneTeachTarget()),
            ]),
            RecordMutation.Override("Keyword", keywordKey, fixture.ModKey,
            [
                Set("AttractionRule", RecordValue.Null),
            ]),
        ]);

        var result = workspace.Records.Apply(changes);

        Assert.Equal(1UL, result.Revision);
        var formList = result.Records.Single(record => record.FamilyId == "FormList");
        var items = Assert.IsType<RecordValue.ListRecordValue>(formList.Values["Items"]);
        Assert.Equal([keywordKey, keywordKey], items.Values.Cast<RecordValue.FormLinkRecordValue>().Select(value => value.FormKey));
        var name = Assert.IsType<RecordValue.TranslatedStringRecordValue>(formList.Values["Name"]);
        Assert.Equal(Language.French, name.TargetLanguage);
        Assert.Equal("Nom français", name.Values[Language.French]);
        var armor = result.Records.Single(record => record.FamilyId == "Armor");
        Assert.IsType<RecordValue.ObjectRecordValue>(armor.Values["ObjectBounds"]);
        Assert.Equal(2, Assert.IsType<RecordValue.ListRecordValue>(armor.Values["Keywords"]).Values.Count);
        Assert.IsType<RecordValue.NullRecordValue>(result.Records.Single(record => record.FamilyId == "Keyword").Values["AttractionRule"]);
        Assert.Equal("None", Assert.IsType<RecordValue.ObjectRecordValue>(result.Records.Single(record => record.FamilyId == "Book").Values["Teaches"]).Alternative);
    }

    /// <summary>Rejects whole change sets without retaining allocation, publication, or revision changes when validation fails.</summary>
    [Fact]
    public void ValidationFailurePreservesPriorOutputAndRevision()
    {
        using var directory = new TemporaryDirectory();
        PrepareRequiredStarfieldMaster(directory.Path, GameRelease.Starfield);
        var outputModKey = ModKey.FromNameAndExtension("Output.esp");
        using var workspace = CreateFactory().Open(CreateNewRequest(directory.Path, GameRelease.Starfield, [], outputModKey));
        var existing = workspace.Records.Apply(new RecordChangeSet(0,
        [
            RecordMutation.Create("Keyword", [Set("EditorID", RecordValue.FromString("Existing"))]),
        ])).Records.Single();
        var invalidReference = new FormKey(ModKey.FromNameAndExtension("Missing.esm"), 0x123);
        var changeSet = new RecordChangeSet(1,
        [
            RecordMutation.Create("Faction", [Set("EditorID", RecordValue.FromString("WouldHaveBeenCreated"))]),
            RecordMutation.Create("FormList",
            [
                Set("Items", RecordValue.FromList([RecordValue.FromFormLink(invalidReference)])),
            ]),
        ]);

        var exception = Assert.Throws<RecordEditingException>(() => workspace.Records.Apply(changeSet));

        Assert.Contains(invalidReference.ToString(), exception.Message, StringComparison.Ordinal);
        Assert.Equal(1UL, workspace.State.Revision);
        Assert.Single(workspace.Output.EnumerateMajorRecords());
        Assert.Equal("Existing", Assert.IsType<RecordValue.StringRecordValue>(
            workspace.Records.Read(new RecordLocator("Keyword", existing.FormKey, outputModKey)).Values["EditorID"]).Value);

        var initialNextFormId = workspace.Output.NextFormID;
        var wrongReferenceType = new RecordChangeSet(1,
        [
            RecordMutation.Create("Faction",
            [
                Set("SharedCrimeFactionList", RecordValue.FromFormLink(existing.FormKey)),
            ]),
        ]);

        var wrongTypeException = Assert.Throws<RecordEditingException>(() => workspace.Records.Apply(wrongReferenceType));
        Assert.Contains(nameof(Mutagen.Bethesda.Starfield.IFormListGetter), wrongTypeException.Message, StringComparison.Ordinal);
        Assert.Equal(initialNextFormId, workspace.Output.NextFormID);

        var invalidListOperation = new RecordChangeSet(1,
        [
            RecordMutation.Create("FormList",
            [
                new RecordFieldChange("Items", RecordCollectionOperation.Insert, RecordValue.FromFormLink(existing.FormKey), index: 1),
            ]),
        ]);

        Assert.Throws<RecordEditingException>(() => workspace.Records.Apply(invalidListOperation));
        Assert.Equal(initialNextFormId, workspace.Output.NextFormID);
        Assert.Equal(1UL, workspace.State.Revision);
        Assert.Single(workspace.Output.EnumerateMajorRecords());
    }

    /// <summary>Rejects a candidate whose registered projection fails without publishing it or consuming its FormID.</summary>
    [Fact]
    public void ProjectionFailurePreservesOutputAllocationAndRevision()
    {
        using var directory = new TemporaryDirectory();
        PrepareRequiredStarfieldMaster(directory.Path, GameRelease.Starfield);
        var outputModKey = ModKey.FromNameAndExtension("Output.esp");
        using var workspace = CreateFactory().Open(CreateNewRequest(directory.Path, GameRelease.Starfield, [], outputModKey));
        var initialNextFormId = workspace.Output.NextFormID;
        var changeSet = new RecordChangeSet(0,
        [
            RecordMutation.Create("Book", []),
        ]);

        var exception = Assert.Throws<RecordEditingException>(() => workspace.Records.Apply(changeSet));

        Assert.Contains("Book.Teaches", exception.Message, StringComparison.Ordinal);
        Assert.Equal(initialNextFormId, workspace.Output.NextFormID);
        Assert.Equal(0UL, workspace.State.Revision);
        Assert.Empty(workspace.Output.EnumerateMajorRecords());
    }

    /// <summary>Rejects explicit null object bounds for Armor in every supported game.</summary>
    [Theory]
    [InlineData(GameRelease.Starfield)]
    [InlineData(GameRelease.Fallout4)]
    [InlineData(GameRelease.SkyrimSE)]
    public void RejectsNullArmorObjectBounds(GameRelease release)
    {
        using var directory = new TemporaryDirectory();
        PrepareRequiredStarfieldMaster(directory.Path, release);
        var outputModKey = ModKey.FromNameAndExtension("Output.esp");
        using var workspace = CreateFactory().Open(CreateNewRequest(directory.Path, release, [], outputModKey));
        var objectBounds = workspace.Records.Families
            .Single(family => family.FamilyId == "Armor")
            .Fields
            .Single(field => field.Path == "ObjectBounds");
        var initialNextFormId = workspace.Output.NextFormID;
        var changeSet = new RecordChangeSet(0,
        [
            RecordMutation.Create("Armor", [Set("ObjectBounds", RecordValue.Null)]),
        ]);

        Assert.False(objectBounds.IsNullable);
        var exception = Assert.Throws<RecordEditingException>(() => workspace.Records.Apply(changeSet));
        Assert.Contains("does not accept a null", exception.Message, StringComparison.Ordinal);
        Assert.Equal(initialNextFormId, workspace.Output.NextFormID);
        Assert.Equal(0UL, workspace.State.Revision);
        Assert.Empty(workspace.Output.EnumerateMajorRecords());
    }

    /// <summary>Applies bounded list operations in order without collapsing duplicate FormLinks.</summary>
    [Fact]
    public void AppliesDeclaredOrderedListOperations()
    {
        using var directory = new TemporaryDirectory();
        var fixture = CreateRecordEditingFixture(directory.Path, GameRelease.Starfield);
        var outputModKey = ModKey.FromNameAndExtension("Output.esp");
        using var workspace = CreateFactory().Open(CreateNewRequest(directory.Path, GameRelease.Starfield, [fixture.ModKey], outputModKey));
        var keywordKey = fixture.FormKeys["Keyword"];
        var result = workspace.Records.Apply(new RecordChangeSet(0,
        [
            RecordMutation.Override("FormList", fixture.FormKeys["FormList"], fixture.ModKey,
            [
                new RecordFieldChange("Items", RecordCollectionOperation.Append, RecordValue.FromFormLink(keywordKey)),
                new RecordFieldChange("Items", RecordCollectionOperation.Insert, RecordValue.FromFormLink(keywordKey), index: 0),
                new RecordFieldChange("Items", RecordCollectionOperation.RemoveAt, index: 1),
                new RecordFieldChange("Items", RecordCollectionOperation.Clear),
                new RecordFieldChange("Items", RecordCollectionOperation.Append, RecordValue.FromFormLink(keywordKey)),
                new RecordFieldChange("Items", RecordCollectionOperation.Append, RecordValue.FromFormLink(keywordKey)),
            ]),
        ]));

        var items = Assert.IsType<RecordValue.ListRecordValue>(Assert.Single(result.Records).Values["Items"]);
        Assert.Equal([keywordKey, keywordKey], items.Values.Cast<RecordValue.FormLinkRecordValue>().Select(value => value.FormKey));
    }

    private static RecordFieldChange Set(string path, RecordValue value)
    {
        return new RecordFieldChange(path, RecordCollectionOperation.Set, value);
    }

    private static RecordValue NoneTeachTarget()
    {
        return RecordValue.FromObject("None", new Dictionary<string, RecordValue>
        {
            ["RawContent"] = RecordValue.FromUnsignedInteger(0),
        });
    }

    private static void PrepareRequiredStarfieldMaster(string directory, GameRelease release)
    {
        if (release == GameRelease.Starfield)
        {
            var modKey = ModKey.FromNameAndExtension("Starfield.esm");
            WriteEmptyPlugin(Path.Combine(directory, modKey.ToString()), modKey, release);
        }
    }

    private static RecordEditingFixture CreateRecordEditingFixture(string directory, GameRelease release)
    {
        return release switch
        {
            GameRelease.Starfield => CreateStarfieldEditingFixture(directory),
            GameRelease.Fallout4 => CreateFallout4EditingFixture(directory),
            GameRelease.SkyrimSE => CreateSkyrimEditingFixture(directory),
            _ => throw new ArgumentOutOfRangeException(nameof(release), release, null),
        };
    }

    private static RecordEditingFixture CreateStarfieldEditingFixture(string directory)
    {
        var modKey = ModKey.FromNameAndExtension("Starfield.esm");
        var mod = new Mutagen.Bethesda.Starfield.StarfieldMod(modKey, Mutagen.Bethesda.Starfield.StarfieldRelease.Starfield);
        var records = new Dictionary<string, IMajorRecord>
        {
            ["Armor"] = Add(mod.Armors, new Mutagen.Bethesda.Starfield.Armor(mod.GetNextFormKey(), Mutagen.Bethesda.Starfield.StarfieldRelease.Starfield)),
            ["Book"] = Add(mod.Books, new Mutagen.Bethesda.Starfield.Book(mod.GetNextFormKey(), Mutagen.Bethesda.Starfield.StarfieldRelease.Starfield)
            {
                Teaches = new Mutagen.Bethesda.Starfield.BookTeachesNothing(),
            }),
            ["Faction"] = Add(mod.Factions, new Mutagen.Bethesda.Starfield.Faction(mod.GetNextFormKey(), Mutagen.Bethesda.Starfield.StarfieldRelease.Starfield)),
            ["FormList"] = Add(mod.FormLists, new Mutagen.Bethesda.Starfield.FormList(mod.GetNextFormKey(), Mutagen.Bethesda.Starfield.StarfieldRelease.Starfield)),
            ["Global"] = Add(mod.Globals, new Mutagen.Bethesda.Starfield.Global(mod.GetNextFormKey(), Mutagen.Bethesda.Starfield.StarfieldRelease.Starfield)),
            ["Keyword"] = Add(mod.Keywords, new Mutagen.Bethesda.Starfield.Keyword(mod.GetNextFormKey(), Mutagen.Bethesda.Starfield.StarfieldRelease.Starfield)),
            ["Message"] = Add(mod.Messages, new Mutagen.Bethesda.Starfield.Message(mod.GetNextFormKey(), Mutagen.Bethesda.Starfield.StarfieldRelease.Starfield)),
            ["Quest"] = Add(mod.Quests, new Mutagen.Bethesda.Starfield.Quest(mod.GetNextFormKey(), Mutagen.Bethesda.Starfield.StarfieldRelease.Starfield)),
            ["Race"] = Add(mod.Races, new Mutagen.Bethesda.Starfield.Race(mod.GetNextFormKey(), Mutagen.Bethesda.Starfield.StarfieldRelease.Starfield)),
            ["TextureSet"] = Add(mod.TextureSets, new Mutagen.Bethesda.Starfield.TextureSet(mod.GetNextFormKey(), Mutagen.Bethesda.Starfield.StarfieldRelease.Starfield)),
        };
        return WriteEditingFixture(directory, mod, records);
    }

    private static RecordEditingFixture CreateFallout4EditingFixture(string directory)
    {
        var modKey = ModKey.FromNameAndExtension("Base.esm");
        var mod = new Mutagen.Bethesda.Fallout4.Fallout4Mod(modKey, Mutagen.Bethesda.Fallout4.Fallout4Release.Fallout4);
        var records = new Dictionary<string, IMajorRecord>
        {
            ["Armor"] = Add(mod.Armors, new Mutagen.Bethesda.Fallout4.Armor(mod.GetNextFormKey(), Mutagen.Bethesda.Fallout4.Fallout4Release.Fallout4)),
            ["Book"] = Add(mod.Books, new Mutagen.Bethesda.Fallout4.Book(mod.GetNextFormKey(), Mutagen.Bethesda.Fallout4.Fallout4Release.Fallout4)
            {
                Teaches = new Mutagen.Bethesda.Fallout4.BookTeachesNothing(),
            }),
            ["Faction"] = Add(mod.Factions, new Mutagen.Bethesda.Fallout4.Faction(mod.GetNextFormKey(), Mutagen.Bethesda.Fallout4.Fallout4Release.Fallout4)),
            ["FormList"] = Add(mod.FormLists, new Mutagen.Bethesda.Fallout4.FormList(mod.GetNextFormKey(), Mutagen.Bethesda.Fallout4.Fallout4Release.Fallout4)),
            ["Global"] = Add(mod.Globals, new Mutagen.Bethesda.Fallout4.GlobalFloat(mod.GetNextFormKey(), Mutagen.Bethesda.Fallout4.Fallout4Release.Fallout4)),
            ["Keyword"] = Add(mod.Keywords, new Mutagen.Bethesda.Fallout4.Keyword(mod.GetNextFormKey(), Mutagen.Bethesda.Fallout4.Fallout4Release.Fallout4)),
            ["Message"] = Add(mod.Messages, new Mutagen.Bethesda.Fallout4.Message(mod.GetNextFormKey(), Mutagen.Bethesda.Fallout4.Fallout4Release.Fallout4)),
            ["Quest"] = Add(mod.Quests, new Mutagen.Bethesda.Fallout4.Quest(mod.GetNextFormKey(), Mutagen.Bethesda.Fallout4.Fallout4Release.Fallout4)),
            ["Race"] = Add(mod.Races, new Mutagen.Bethesda.Fallout4.Race(mod.GetNextFormKey(), Mutagen.Bethesda.Fallout4.Fallout4Release.Fallout4)),
            ["TextureSet"] = Add(mod.TextureSets, new Mutagen.Bethesda.Fallout4.TextureSet(mod.GetNextFormKey(), Mutagen.Bethesda.Fallout4.Fallout4Release.Fallout4)),
        };
        return WriteEditingFixture(directory, mod, records);
    }

    private static RecordEditingFixture CreateSkyrimEditingFixture(string directory)
    {
        var modKey = ModKey.FromNameAndExtension("Base.esm");
        var mod = new Mutagen.Bethesda.Skyrim.SkyrimMod(modKey, Mutagen.Bethesda.Skyrim.SkyrimRelease.SkyrimSE);
        var records = new Dictionary<string, IMajorRecord>
        {
            ["Armor"] = Add(mod.Armors, new Mutagen.Bethesda.Skyrim.Armor(mod.GetNextFormKey(), Mutagen.Bethesda.Skyrim.SkyrimRelease.SkyrimSE)),
            ["Book"] = Add(mod.Books, new Mutagen.Bethesda.Skyrim.Book(mod.GetNextFormKey(), Mutagen.Bethesda.Skyrim.SkyrimRelease.SkyrimSE)
            {
                Teaches = new Mutagen.Bethesda.Skyrim.BookTeachesNothing(),
            }),
            ["Faction"] = Add(mod.Factions, new Mutagen.Bethesda.Skyrim.Faction(mod.GetNextFormKey(), Mutagen.Bethesda.Skyrim.SkyrimRelease.SkyrimSE)),
            ["FormList"] = Add(mod.FormLists, new Mutagen.Bethesda.Skyrim.FormList(mod.GetNextFormKey(), Mutagen.Bethesda.Skyrim.SkyrimRelease.SkyrimSE)),
            ["Global"] = Add(mod.Globals, new Mutagen.Bethesda.Skyrim.GlobalFloat(mod.GetNextFormKey(), Mutagen.Bethesda.Skyrim.SkyrimRelease.SkyrimSE)),
            ["Keyword"] = Add(mod.Keywords, new Mutagen.Bethesda.Skyrim.Keyword(mod.GetNextFormKey(), Mutagen.Bethesda.Skyrim.SkyrimRelease.SkyrimSE)),
            ["Message"] = Add(mod.Messages, new Mutagen.Bethesda.Skyrim.Message(mod.GetNextFormKey(), Mutagen.Bethesda.Skyrim.SkyrimRelease.SkyrimSE)),
            ["Quest"] = Add(mod.Quests, new Mutagen.Bethesda.Skyrim.Quest(mod.GetNextFormKey(), Mutagen.Bethesda.Skyrim.SkyrimRelease.SkyrimSE)),
            ["Race"] = Add(mod.Races, new Mutagen.Bethesda.Skyrim.Race(mod.GetNextFormKey(), Mutagen.Bethesda.Skyrim.SkyrimRelease.SkyrimSE)),
            ["TextureSet"] = Add(mod.TextureSets, new Mutagen.Bethesda.Skyrim.TextureSet(mod.GetNextFormKey(), Mutagen.Bethesda.Skyrim.SkyrimRelease.SkyrimSE)),
        };
        return WriteEditingFixture(directory, mod, records);
    }

    private static TRecord Add<TRecord>(IGroup<TRecord> group, TRecord record)
        where TRecord : class, IMajorRecord
    {
        group.Add(record);
        return record;
    }

    private static RecordEditingFixture WriteEditingFixture(
        string directory,
        IMod mod,
        IReadOnlyDictionary<string, IMajorRecord> records)
    {
        foreach (var pair in records)
        {
            pair.Value.EditorID = $"Source{pair.Key}";
        }

        WritePlugin(mod, Path.Combine(directory, mod.ModKey.ToString()));
        return new RecordEditingFixture(mod.ModKey, records.ToDictionary(pair => pair.Key, pair => pair.Value.FormKey));
    }

    private sealed class RecordEditingFixture
    {
        public RecordEditingFixture(ModKey modKey, IReadOnlyDictionary<string, FormKey> formKeys)
        {
            ModKey = modKey;
            FormKeys = formKeys;
        }

        public ModKey ModKey { get; }

        public IReadOnlyDictionary<string, FormKey> FormKeys { get; }
    }
}
