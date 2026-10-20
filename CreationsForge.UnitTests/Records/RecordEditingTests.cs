using CreationsForge.Engine.Persistence;
using CreationsForge.Engine.Records;
using CreationsForge.Engine.Workspaces;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Strings;

namespace CreationsForge.UnitTests.Workspaces;

/// <summary>Verifies declared record editing through the production workspace boundary.</summary>
public sealed partial class PluginWorkspaceTests
{
    /// <summary>Creates and reads every declared family with one atomic revision increment.</summary>
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
            if (descriptor.Fields.Any(field => field.Path == "Teaches" && field.Kind == RecordValueKind.Object))
            {
                changes.Add(Set("Teaches", NoneTeachTarget()));
            }

            return RecordMutation.Create(descriptor.FamilyId, changes);
        });

        var result = workspace.Records.Apply(new RecordChangeSet(0, mutations));
        var expected = workspace.Records.Families.Select(family => family.FamilyId).ToArray();

        Assert.Equal(expected, expected.Order(StringComparer.Ordinal));
        Assert.Contains("Cell", expected);
        Assert.Contains("Worldspace", expected);
        Assert.DoesNotContain("CellBlock", expected);
        Assert.All(UnwritableFamilies(release), family => Assert.DoesNotContain(family, expected));
        Assert.True(expected.Length >= MinimumFamilyCount(release));
        Assert.Equal(1UL, result.Revision);
        Assert.Equal(expected.Length, result.Records.Count);
        AssertContainedFamilies(workspace.Output, result.Records);
        foreach (var record in result.Records)
        {
            var read = workspace.Records.Read(new RecordLocator(record.FamilyId, record.FormKey, outputModKey));
            var editorId = Assert.IsType<RecordValue.StringRecordValue>(read.Values["EditorID"]);
            Assert.Equal(CanonicalEditorId(record.FamilyId, $"Created{record.FamilyId}"), editorId.Value);
        }
    }

    /// <summary>Overrides, inspects, and compares every declared family while preserving exact source records.</summary>
    [Theory]
    [InlineData(GameRelease.Starfield)]
    [InlineData(GameRelease.Fallout4)]
    [InlineData(GameRelease.SkyrimSE)]
    public async Task OverridesAndComparesEveryDeclaredFirstTrancheFamily(GameRelease release)
    {
        using var directory = new TemporaryDirectory();
        var fixture = await CreateRecordEditingFixture(directory.Path, release);
        var outputModKey = ModKey.FromNameAndExtension("Output.esp");
        using var workspace = CreateFactory().Open(CreateNewRequest(directory.Path, release, [fixture.ModKey], outputModKey));
        var mutations = fixture.FormKeys.Select(pair => RecordMutation.Override(
            pair.Key,
            pair.Value,
            fixture.ModKey,
            [Set("EditorID", RecordValue.FromString($"Overridden{pair.Key}"))]));

        var result = workspace.Records.Apply(new RecordChangeSet(0, mutations));

        Assert.Equal(1UL, result.Revision);
        Assert.Equal(workspace.Records.Families.Count, result.Records.Count);
        foreach (var pair in fixture.FormKeys)
        {
            var comparison = workspace.Records.Compare(
                new RecordLocator(pair.Key, pair.Value, fixture.ModKey),
                new RecordLocator(pair.Key, pair.Value, outputModKey));
            var difference = Assert.Single(comparison.Differences, item => item.Path == "EditorID");
            Assert.Equal(CanonicalEditorId(pair.Key, $"Source{pair.Key}"), Assert.IsType<RecordValue.StringRecordValue>(difference.Left).Value);
            Assert.Equal(CanonicalEditorId(pair.Key, $"Overridden{pair.Key}"), Assert.IsType<RecordValue.StringRecordValue>(difference.Right).Value);
        }
    }

    /// <summary>Keeps unedited Ammunition value and weight when a later change set updates only the editor ID.</summary>
    [Theory]
    [InlineData(GameRelease.Starfield)]
    [InlineData(GameRelease.Fallout4)]
    [InlineData(GameRelease.SkyrimSE)]
    public void LaterEditorIdChangePreservesAmmunitionValueAndWeight(GameRelease release)
    {
        using var directory = new TemporaryDirectory();
        PrepareRequiredStarfieldMaster(directory.Path, release);
        var outputModKey = ModKey.FromNameAndExtension("Output.esp");
        using var workspace = CreateFactory().Open(CreateNewRequest(directory.Path, release, [], outputModKey));
        var created = workspace.Records.Apply(new RecordChangeSet(0,
        [
            RecordMutation.Create("Ammunition",
            [
                Set("EditorID", RecordValue.FromString("Round")),
                Set("Value", RecordValue.FromUnsignedInteger(9)),
                Set("Weight", RecordValue.FromFloatingPoint(2.5)),
            ]),
        ])).Records.Single();

        var updated = workspace.Records.Apply(new RecordChangeSet(1,
        [
            RecordMutation.Override("Ammunition", created.FormKey, outputModKey,
            [
                Set("EditorID", RecordValue.FromString("RenamedRound")),
            ]),
        ])).Records.Single();

        Assert.Equal("RenamedRound", Assert.IsType<RecordValue.StringRecordValue>(updated.Values["EditorID"]).Value);
        Assert.Equal(9UL, Assert.IsType<RecordValue.UnsignedIntegerRecordValue>(updated.Values["Value"]).Value);
        Assert.Equal(2.5, Assert.IsType<RecordValue.FloatingPointRecordValue>(updated.Values["Weight"]).Value);
    }

    /// <summary>Saves one new record for every declared family and reads each editor ID from a fresh workspace.</summary>
    [Theory]
    [InlineData(GameRelease.Starfield)]
    [InlineData(GameRelease.Fallout4)]
    [InlineData(GameRelease.SkyrimSE)]
    public async Task SaveReopensEveryDeclaredFamilyEditorId(GameRelease release)
    {
        using var directory = new TemporaryDirectory();
        PrepareRequiredStarfieldMaster(directory.Path, release);
        var outputModKey = ModKey.FromNameAndExtension("Output.esp");
        IReadOnlyList<RecordSnapshot> created;
        using (var workspace = CreateFactory().Open(CreateNewRequest(directory.Path, release, [], outputModKey)))
        {
            created = workspace.Records.Apply(new RecordChangeSet(0, workspace.Records.Families.Select(descriptor =>
            {
                var changes = new List<RecordFieldChange>
                {
                    Set("EditorID", RecordValue.FromString($"Saved{descriptor.FamilyId}")),
                };
                if (descriptor.Fields.Any(field => field.Path == "Teaches" && field.Kind == RecordValueKind.Object))
                {
                    changes.Add(Set("Teaches", NoneTeachTarget()));
                }

                if (descriptor.FamilyId == "Global")
                {
                    changes.Add(Set("Data", RecordValue.FromFloatingPoint(1.5)));
                }

                return RecordMutation.Create(descriptor.FamilyId, changes);
            }))).Records;
            var result = await workspace.SaveAsync(cancellationToken: TestContext.Current.CancellationToken);
            Assert.True(result.Status == PluginSaveStatus.Succeeded, result.Diagnostic);
        }

        using var reopened = CreateFactory().Open(CreateExistingRequest(directory.Path, release, outputModKey));
        AssertContainedFamilies(reopened.Output, created);
        foreach (var record in created)
        {
            var snapshot = reopened.Records.Read(new RecordLocator(record.FamilyId, record.FormKey, outputModKey));
            Assert.Equal(CanonicalEditorId(record.FamilyId, $"Saved{record.FamilyId}"), Assert.IsType<RecordValue.StringRecordValue>(snapshot.Values["EditorID"]).Value);
            if (record.FamilyId == "Global")
            {
                Assert.Equal(1.5, Assert.IsType<RecordValue.FloatingPointRecordValue>(snapshot.Values["Data"]).Value);
            }
        }
    }

    /// <summary>Saves Ammunition with nondefault value and weight and reads those fields from a fresh workspace.</summary>
    [Theory]
    [InlineData(GameRelease.Starfield)]
    [InlineData(GameRelease.Fallout4)]
    [InlineData(GameRelease.SkyrimSE)]
    public async Task SaveReopensAmmunitionValueAndWeight(GameRelease release)
    {
        using var directory = new TemporaryDirectory();
        PrepareRequiredStarfieldMaster(directory.Path, release);
        var outputModKey = ModKey.FromNameAndExtension("Output.esp");
        FormKey createdFormKey;
        using (var workspace = CreateFactory().Open(CreateNewRequest(directory.Path, release, [], outputModKey)))
        {
            createdFormKey = workspace.Records.Apply(new RecordChangeSet(0,
            [
                RecordMutation.Create("Ammunition",
                [
                    Set("EditorID", RecordValue.FromString("SavedRound")),
                    Set("Value", RecordValue.FromUnsignedInteger(9)),
                    Set("Weight", RecordValue.FromFloatingPoint(2.5)),
                ]),
            ])).Records.Single().FormKey;
            var result = await workspace.SaveAsync(cancellationToken: TestContext.Current.CancellationToken);
            Assert.True(result.Status == PluginSaveStatus.Succeeded, result.Diagnostic);
        }

        using var reopened = CreateFactory().Open(CreateExistingRequest(directory.Path, release, outputModKey));
        var snapshot = reopened.Records.Read(new RecordLocator("Ammunition", createdFormKey, outputModKey));
        Assert.Equal("SavedRound", Assert.IsType<RecordValue.StringRecordValue>(snapshot.Values["EditorID"]).Value);
        Assert.Equal(9UL, Assert.IsType<RecordValue.UnsignedIntegerRecordValue>(snapshot.Values["Value"]).Value);
        Assert.Equal(2.5, Assert.IsType<RecordValue.FloatingPointRecordValue>(snapshot.Values["Weight"]).Value);
    }

    /// <summary>Preserves nulls, ordered duplicate links, translations, nested values, and polymorphic choices in record candidates.</summary>
    [Fact]
    public async Task AppliesReusableFieldShapesWithoutFlatteningRecordValues()
    {
        using var directory = new TemporaryDirectory();
        var fixture = await CreateRecordEditingFixture(directory.Path, GameRelease.Starfield);
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
    public async Task AppliesDeclaredOrderedListOperations()
    {
        using var directory = new TemporaryDirectory();
        var fixture = await CreateRecordEditingFixture(directory.Path, GameRelease.Starfield);
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

    private static void AssertContainedFamilies(Mutagen.Bethesda.Plugins.Records.IModGetter output, IEnumerable<RecordSnapshot> records)
    {
        var actual = output.EnumerateMajorRecords().Select(record => record.FormKey).ToHashSet();
        Assert.All(records, record => Assert.Contains(record.FormKey, actual));
    }

    private static string CanonicalEditorId(string familyId, string requested)
    {
        var prefix = familyId switch
        {
            "GameSetting" or "GameSettingFloat" => "f",
            "GameSettingBool" => "b",
            "GameSettingInt" => "i",
            "GameSettingString" => "s",
            "GameSettingUInt" => "u",
            _ => null,
        };
        return prefix is null || requested.StartsWith(prefix, StringComparison.Ordinal) ? requested : prefix + requested;
    }

    private static string[] UnwritableFamilies(GameRelease release)
    {
        return release switch
        {
            GameRelease.Starfield => ["ContainerModification", "UnknownObjectModification"],
            GameRelease.Fallout4 => ["DamageTypeIndexed"],
            GameRelease.SkyrimSE => [],
            _ => throw new ArgumentOutOfRangeException(nameof(release), release, null),
        };
    }

    private static int MinimumFamilyCount(GameRelease release)
    {
        return release switch
        {
            GameRelease.Starfield => 190,
            GameRelease.Fallout4 => 140,
            GameRelease.SkyrimSE => 120,
            _ => throw new ArgumentOutOfRangeException(nameof(release), release, null),
        };
    }

    private static async Task<RecordEditingFixture> CreateRecordEditingFixture(string directory, GameRelease release)
    {
        PrepareRequiredStarfieldMaster(directory, release);
        var modKey = ModKey.FromNameAndExtension("Source.esm");
        using var workspace = CreateFactory().Open(CreateNewRequest(directory, release, [], modKey));
        var created = workspace.Records.Apply(new RecordChangeSet(0, workspace.Records.Families.Select(descriptor =>
        {
            var changes = new List<RecordFieldChange>
            {
                Set("EditorID", RecordValue.FromString($"Source{descriptor.FamilyId}")),
            };
            if (descriptor.Fields.Any(field => field.Path == "Teaches" && field.Kind == RecordValueKind.Object))
            {
                changes.Add(Set("Teaches", NoneTeachTarget()));
            }

            if (descriptor.FamilyId == "Global")
            {
                changes.Add(Set("Data", RecordValue.FromFloatingPoint(1.5)));
            }

            return RecordMutation.Create(descriptor.FamilyId, changes);
        }))).Records;
        var result = await workspace.SaveAsync(cancellationToken: TestContext.Current.CancellationToken);
        Assert.True(result.Status == PluginSaveStatus.Succeeded, result.Diagnostic);
        return new RecordEditingFixture(modKey, created.ToDictionary(record => record.FamilyId, record => record.FormKey));
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
