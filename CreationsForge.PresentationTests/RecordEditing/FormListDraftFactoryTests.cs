using System.Buffers;
using System.Reflection;
using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.RecordWire;
using CreationsForge.RecordEditing.Drafts;
using CreationsForge.RecordEditing.Schema;
using CreationsForge.Starfield.PluginAdapter.Edits;
using CreationsForge.Starfield.PluginAdapter.RecordInspection;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Starfield;
using Mutagen.Bethesda.Starfield.Assets;
using Noggog;
using Shouldly;

namespace CreationsForge.PresentationTests.RecordEditing;

/// <summary>Verifies revision-bound seed extraction, required-unset defaults, and exact command serialization.</summary>
public sealed class FormListDraftFactoryTests
{
    /// <summary>Verifies both localized FormList name commands expose the table's typed fields and accept its synchronized wire shape.</summary>
    [Fact]
    public void Create_TranslatedName_StarfieldAndFallout4SerializeAndDecode()
    {
        foreach (var context in FormListCatalogTests.CreateContexts().Take(2))
        {
            var seed = CaptureSeed(context,
                "\"Name\":{\"targetLanguage\":\"English\",\"value\":\"Original\",\"translations\":[{\"language\":\"English\",\"value\":\"Original\"}]}");
            var command = context.SchemaCatalog.Nodes.Single(key =>
                key.Kind == RecordWireSchemaNodeKind.Command && key.Name.EndsWith(".form-list.set-name", StringComparison.Ordinal));
            var created = new FormListDraftFactory().Create(
                context, command, seed, FormListDraftSeedSelection.CurrentValue(), RecordWireReadLimits.Default);
            created.Succeeded.ShouldBeTrue(created.Error?.Message);
            var name = created.Value!.Root.ShouldBeOfType<RecordWireObjectDraftNode>()
                .FindProperty("name").ShouldBeOfType<RecordWireTranslatedStringDraftNode>();
            name.FindProperty("targetLanguage").ShouldBeOfType<RecordWireStringDraftNode>().Value.ShouldBe("English");
            var value = name.FindProperty("value").ShouldBeOfType<RecordWireNullableDraftNode>();
            value.Value.ShouldBeOfType<RecordWireStringDraftNode>().Value = "Renamed";
            var translation = name.FindProperty("translations").ShouldBeOfType<RecordWireArrayDraftNode>()
                .Items.Single().ShouldBeOfType<RecordWireObjectDraftNode>();
            translation.FindProperty("value").ShouldBeOfType<RecordWireStringDraftNode>().Value = "Renamed";

            var serialized = new FormListDraftSerializer(new FormListDraftValidator()).Serialize(created.Value, RecordWireReadLimits.Default);
            serialized.Succeeded.ShouldBeTrue(string.Join(Environment.NewLine, serialized.Issues.Select(issue => issue.Message)));
            serialized.GetArguments().GetProperty("name").GetProperty("value").GetString().ShouldBe("Renamed");
            context.Codec.Decode(command.Name, serialized.GetArguments(), RecordWireReadLimits.Default)
                .Succeeded.ShouldBeTrue();
        }
    }

    /// <summary>The generated plugin reader used to verify every available default after presentation serialization.</summary>
    private static readonly ReadRecordTypeDelegate ReadRecordType = CreateReadRecordTypeDelegate();

    /// <summary>Verifies a seeded setter begins with the current value and decodes after an explicit typed edit.</summary>
    [Fact]
    public void Create_SeededEditorId_PreservesThenSerializesTypedValue()
    {
        var context = FormListCatalogTests.CreateContexts()[0];
        var seed = CaptureSeed(context, "\"EditorID\":\"OldId\",\"Items\":[],\"Components\":[],\"ConditionalEntries\":[]");
        var command = context.SchemaCatalog.Nodes.Single(key => key.Kind == RecordWireSchemaNodeKind.Command && key.Name == "form-list.set-editor-id");
        var factory = new FormListDraftFactory();

        var created = factory.Create(context, command, seed, FormListDraftSeedSelection.CurrentValue(), RecordWireReadLimits.Default);

        created.Succeeded.ShouldBeTrue(created.Error?.Message);
        var root = created.Value!.Root.ShouldBeOfType<RecordWireObjectDraftNode>();
        var editorId = root.FindProperty("editorId").ShouldBeOfType<RecordWireStringDraftNode>();
        editorId.Value.ShouldBe("OldId");
        editorId.Value = "NewId";
        created.Value.HasChanges.ShouldBeTrue();
        var serialized = new FormListDraftSerializer(new FormListDraftValidator()).Serialize(created.Value, RecordWireReadLimits.Default);
        serialized.Succeeded.ShouldBeTrue(string.Join(Environment.NewLine, serialized.Issues.Select(issue => issue.Message)));
        serialized.GetArguments().GetProperty("editorId").GetString().ShouldBe("NewId");
        context.Codec.Decode(command.Name, serialized.GetArguments(), RecordWireReadLimits.Default).Succeeded.ShouldBeTrue();
    }

    /// <summary>Verifies complete seeded item replacement preserves order, duplicates, and both null-link identities.</summary>
    [Fact]
    public void Create_ReplaceItems_PreservesCompleteOrderedSeed()
    {
        var context = FormListCatalogTests.CreateContexts()[0];
        var seed = CaptureSeed(
            context,
            "\"EditorID\":null,\"Items\":[{\"isNull\":true,\"formKey\":null},{\"isNull\":true,\"formKey\":\"Null\"},{\"isNull\":false,\"formKey\":\"000123:Master.esm\"},{\"isNull\":false,\"formKey\":\"000123:Master.esm\"}],\"Components\":[],\"ConditionalEntries\":[]");
        var command = context.SchemaCatalog.Nodes.Single(key => key.Kind == RecordWireSchemaNodeKind.Command && key.Name == "form-list.replace-items");

        var draft = new FormListDraftFactory().Create(context, command, seed, FormListDraftSeedSelection.CurrentValue(), RecordWireReadLimits.Default).Value!;
        var serialized = new FormListDraftSerializer(new FormListDraftValidator()).Serialize(draft, RecordWireReadLimits.Default);

        serialized.Succeeded.ShouldBeTrue(string.Join(Environment.NewLine, serialized.Issues.Select(issue => issue.Message)));
        serialized.GetArguments().GetProperty("items").GetRawText().ShouldBe("[{\"isNull\":true,\"formKey\":null},{\"isNull\":true,\"formKey\":\"Null\"},{\"isNull\":false,\"formKey\":\"000123:Master.esm\"},{\"isNull\":false,\"formKey\":\"000123:Master.esm\"}]");
    }

    /// <summary>Verifies each unavailable component or condition default exposes only its exact required-unset siblings.</summary>
    [Fact]
    public void SelectUnavailableGeneratedTypes_ExposesExactRequiredUnsetFields()
    {
        var context = FormListCatalogTests.CreateContexts()[0];
        var componentCommand = context.SchemaCatalog.Nodes.Single(key => key.Kind == RecordWireSchemaNodeKind.Command && key.Name == "starfield.form-list.add-component");
        var componentDraft = new FormListDraftFactory().Create(context, componentCommand, null, FormListDraftSeedSelection.InsertAt(0), RecordWireReadLimits.Default).Value!;
        var componentUnion = ((RecordWireObjectDraftNode)componentDraft.Root).FindProperty("component").ShouldBeOfType<RecordWireUnionDraftNode>();

        var componentSelection = componentUnion.SelectOption("Mutagen.Bethesda.Starfield.VolumesComponent");
        componentSelection.Succeeded.ShouldBeTrue(componentSelection.Error?.Message);
        var selectedComponent = componentSelection.Value!.ShouldBeOfType<RecordWireObjectDraftNode>();
        var items = selectedComponent.FindProperty("Items").ShouldBeOfType<RecordWireNullableDraftNode>();
        var presentItems = items.IsNull ? items.SetPresent().Value!.ShouldBeOfType<RecordWireArrayDraftNode>() : items.Value!.ShouldBeOfType<RecordWireArrayDraftNode>();
        var selectedItem = presentItems.Insert(0).Value!.ShouldBeOfType<RecordWireObjectDraftNode>();
        selectedItem.Properties.Where(property => property.Node.ValueState == RecordWireDraftValueState.RequiredUnset).Select(property => property.Name).ShouldBe(
            ["Matrix1", "Matrix2", "Matrix3", "Matrix4", "Unknown1", "Unknown2", "Unknown3", "Ender"]);
        new FormListDraftValidator().Validate(componentDraft, RecordWireReadLimits.Default).Issues.Count.ShouldBe(8);

        foreach (var conditionType in new[] { "Mutagen.Bethesda.Starfield.ConditionFloat", "Mutagen.Bethesda.Starfield.ConditionGlobal" })
        {
            var conditionDraft = CreateFreshConditionDraft(context, conditionType);
            var condition = FindSelectedCondition(conditionDraft);
            condition.Properties.Where(property => property.Node.ValueState == RecordWireDraftValueState.RequiredUnset).Select(property => property.Name).ShouldBe(
                ["CompareOperator", "Flags", "Unknown1", "Unknown2", "Data", "ComparisonValue"]);
            new FormListDraftValidator().Validate(conditionDraft, RecordWireReadLimits.Default).Issues.Count.ShouldBe(6);
        }
    }

    /// <summary>Verifies all available generated concrete defaults hydrate and serialize through the presentation graph before plugin decoding.</summary>
    [Fact]
    public void GeneratedConcreteDefaults_All719AvailableHydrateSerializeAndDecode()
    {
        var context = FormListCatalogTests.CreateContexts()[0];
        var failures = new List<string>();
        var availableCount = 0;
        var unavailable = new List<string>();
        var concreteNodes = context.SchemaCatalog.Nodes
            .Where(key => key.Kind == RecordWireSchemaNodeKind.Type)
            .Select(key => context.SchemaCatalog.ReadNode(key).Value!)
            .Where(node => node.Schema.TryGetProperty("x-record-construction", out _))
            .ToArray();

        foreach (var node in concreteNodes)
        {
            if (!node.DefaultTemplate.HasValue)
            {
                unavailable.Add(node.Key.Name);
                continue;
            }

            try
            {
                var descriptorResult = new RecordWireSchemaIndex(context).Resolve(node.Key, RecordWireReadLimits.Default);
                if (!descriptorResult.Succeeded || descriptorResult.Value is null)
                {
                    failures.Add($"{node.Key.Name}: schema: {descriptorResult.Error?.Message}");
                    continue;
                }

                var builder = new FormListDraftFactory.DraftBuildState(RecordWireReadLimits.Default, TestContext.Current.CancellationToken);
                var root = builder.Build(descriptorResult.Value, "$", node.Key.Name, true, false, node.DefaultTemplate.Value, RecordWireDraftValueState.Defaulted, false, 1);
                var draft = new FormListDraft(node.Key, context.Identity, null, root, RecordWireReadLimits.Default);
                var serialized = new FormListDraftSerializer(new FormListDraftValidator()).Serialize(draft, RecordWireReadLimits.Default, TestContext.Current.CancellationToken);
                if (!serialized.Succeeded)
                {
                    failures.Add($"{node.Key.Name}: presentation: {string.Join(" | ", serialized.Issues.Select(issue => $"{issue.Path}: {issue.Message}"))}");
                    continue;
                }

                var decoded = RecordWireReadContext.Decode<object>(
                    serialized.GetArguments(),
                    RecordWireReadLimits.Default,
                    TestContext.Current.CancellationToken,
                    (readContext, value) => ReadRecordType(readContext, value, node.Key.Name));
                if (!decoded.Succeeded)
                {
                    failures.Add($"{node.Key.Name}: decode: {decoded.Error?.Message}");
                    continue;
                }

                availableCount++;
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                failures.Add($"{node.Key.Name}: {exception.GetType().Name}: {exception.Message}");
            }
        }

        failures.ShouldBeEmpty("Every advertised available generated default must survive the complete presentation graph and plugin reader.");
        concreteNodes.Length.ShouldBe(722);
        availableCount.ShouldBe(719);
        unavailable.ShouldBe(
        [
            "Mutagen.Bethesda.Starfield.ConditionFloat",
            "Mutagen.Bethesda.Starfield.ConditionGlobal",
            "Mutagen.Bethesda.Starfield.VolumesComponentItem",
        ],
        ignoreOrder: true);
    }

    /// <summary>Verifies the real numeric UInt64 union selects its sole token-compatible branch and decodes as a component command.</summary>
    [Fact]
    public void SelectOrbitedDataComponent_NumericUInt64Union_SerializesThroughCodec()
    {
        var context = FormListCatalogTests.CreateContexts()[0];
        var command = context.SchemaCatalog.Nodes.Single(key => key.Kind == RecordWireSchemaNodeKind.Command && key.Name == "starfield.form-list.add-component");
        var created = new FormListDraftFactory().Create(context, command, null, FormListDraftSeedSelection.InsertAt(0), RecordWireReadLimits.Default);

        created.Succeeded.ShouldBeTrue(created.Error?.Message);
        var component = ((RecordWireObjectDraftNode)created.Value!.Root).FindProperty("component").ShouldBeOfType<RecordWireUnionDraftNode>();
        var selection = component.SelectOption("Mutagen.Bethesda.Starfield.OrbitedDataComponent");

        selection.Succeeded.ShouldBeTrue(selection.Error?.Message);
        var selected = selection.Value.ShouldBeOfType<RecordWireObjectDraftNode>();
        var unknown1 = selected.FindProperty("Unknown1").ShouldBeOfType<RecordWireUnionDraftNode>();
        unknown1.SelectedValue.ShouldBeOfType<RecordWireIntegerDraftNode>().Text.ShouldBe("0");
        var serialized = new FormListDraftSerializer(new FormListDraftValidator()).Serialize(created.Value, RecordWireReadLimits.Default);
        serialized.Succeeded.ShouldBeTrue(string.Join(Environment.NewLine, serialized.Issues.Select(issue => issue.Message)));
        var decoded = context.Codec.Decode(command.Name, serialized.GetArguments(), RecordWireReadLimits.Default);
        decoded.Succeeded.ShouldBeTrue(decoded.Error?.Message);
        decoded.Value.ShouldBeOfType<StarfieldAddComponentEdit>().Component.ShouldBeOfType<OrbitedDataComponent>().Unknown1.ShouldBe(0ul);
    }

    /// <summary>Verifies token-kind selection remains fail-closed when two alternatives admit the same top-level JSON kind.</summary>
    [Fact]
    public void HydrateUnion_WhenTokenKindMatchesTwoAlternatives_RejectsInference()
    {
        var stringDescriptor = new RecordWireSchemaDescriptor(RecordWireSchemaValueKind.String, "String", description: null);
        var options = new[]
        {
            new RecordWireSchemaUnionOption("first", "First", null, (_, _) => EngineResult<RecordWireSchemaDescriptor>.Success(stringDescriptor)),
            new RecordWireSchemaUnionOption("second", "Second", null, (_, _) => EngineResult<RecordWireSchemaDescriptor>.Success(stringDescriptor)),
        };
        var unionDescriptor = new RecordWireSchemaDescriptor(RecordWireSchemaValueKind.Union, "Ambiguous", description: null, unionOptions: options);
        using var value = JsonDocument.Parse("\"same-kind\"");
        var builder = new FormListDraftFactory.DraftBuildState(RecordWireReadLimits.Default, TestContext.Current.CancellationToken);

        var exception = Should.Throw<Exception>(() => builder.Build(unionDescriptor, "$.value", "Value", true, false, value.RootElement, RecordWireDraftValueState.Seeded, true, 1));

        exception.GetType().Name.ShouldBe("DraftBuildException");
        exception.Message.ShouldBe("$.value: seeded union value has no exact discriminator and its String token matches 2 alternatives; selection would require inference.");
    }

    /// <summary>Verifies real owner-derived FormLink-or-index fields track their exact sibling flags while retaining both projections.</summary>
    [Fact]
    public void Create_SeededAnnotatedCondition_OwnerFlagsRefreshFormLinkOrIndexBranch()
    {
        var context = FormListCatalogTests.CreateContexts()[0];
        var formKey = new FormKey((ModKey)"Source.esm", 0x123);
        var parameterFormKey = new FormKey((ModKey)"Source.esm", 0x456);
        var recordData = new BiomeHasKeywordConditionData();
        recordData.FirstParameter.Index = 17;
        recordData.FirstParameter.Link.SetTo(parameterFormKey);
        var source = new FormList(formKey, StarfieldRelease.Starfield);
        source.ConditionalEntries.Add(new FormListConditionalEntry
        {
            Index = null,
            Conditions = new ExtendedList<Condition>
            {
                new ConditionFloat
                {
                    ComparisonValue = 1.0f,
                    Data = recordData,
                },
            },
        });
        var seed = CaptureSeed(context, formKey, WriteReadView(source));
        var command = context.SchemaCatalog.Nodes.Single(key => key.Kind == RecordWireSchemaNodeKind.Command && key.Name == "starfield.form-list.set-conditional-entries");
        var created = new FormListDraftFactory().Create(context, command, seed, FormListDraftSeedSelection.CurrentValue(), RecordWireReadLimits.Default);

        created.Succeeded.ShouldBeTrue(created.Error?.Message);
        var draft = created.Value!;
        var condition = FindSelectedCondition(draft);
        var conditionData = condition.FindProperty("Data").ShouldBeOfType<RecordWireUnionDraftNode>().SelectedValue.ShouldBeOfType<RecordWireObjectDraftNode>();
        var usesAliases = conditionData.FindProperty("UseAliases").ShouldBeOfType<RecordWireBooleanDraftNode>();
        var usesPackageData = conditionData.FindProperty("UsePackageData").ShouldBeOfType<RecordWireBooleanDraftNode>();
        var firstParameter = conditionData.FindProperty("FirstParameter").ShouldBeOfType<RecordWireFormLinkOrIndexDraftNode>();

        firstParameter.ActiveBranch.ShouldBe(RecordWireFormLinkOrIndexActiveBranch.Link);
        firstParameter.IsIndexNull.ShouldBeFalse();
        usesAliases.Value = true;
        firstParameter.ActiveBranch.ShouldBe(RecordWireFormLinkOrIndexActiveBranch.AliasIndex);
        usesPackageData.Value = true;
        firstParameter.ActiveBranch.ShouldBe(RecordWireFormLinkOrIndexActiveBranch.AliasIndex);
        usesAliases.Value = false;
        firstParameter.ActiveBranch.ShouldBe(RecordWireFormLinkOrIndexActiveBranch.PackageDataIndex);
        usesPackageData.Value = false;
        firstParameter.ActiveBranch.ShouldBe(RecordWireFormLinkOrIndexActiveBranch.Link);
        firstParameter.IndexText.ShouldBe("17");
        firstParameter.Link.IsNull.ShouldBeFalse();
        firstParameter.Link.FormKey.ShouldBe(parameterFormKey.ToString());
        var serialized = new FormListDraftSerializer(new FormListDraftValidator()).Serialize(draft, RecordWireReadLimits.Default);
        serialized.Succeeded.ShouldBeTrue(string.Join(Environment.NewLine, serialized.Issues.Select(issue => issue.Message)));
        var decoded = context.Codec.Decode(command.Name, serialized.GetArguments(), RecordWireReadLimits.Default);
        decoded.Succeeded.ShouldBeTrue(decoded.Error?.Message);
        var replacement = decoded.Value.ShouldBeOfType<StarfieldSetConditionalEntriesEdit>();
        var decodedCondition = replacement.Entries.Single().Conditions!.Single().ShouldBeOfType<ConditionFloat>();
        var decodedData = decodedCondition.Data.ShouldBeOfType<BiomeHasKeywordConditionData>();
        decodedData.UseAliases.ShouldBeFalse();
        decodedData.UsePackageData.ShouldBeFalse();
        decodedData.FirstParameter.UsesLink().ShouldBeTrue();
        decodedData.FirstParameter.Index.ShouldBe(17u);
        decodedData.FirstParameter.Link.FormKey.ShouldBe(parameterFormKey);
    }

    /// <summary>Verifies selecting a real annotated condition default preserves its null index before an explicit owner-mode index edit.</summary>
    [Fact]
    public void SelectAnnotatedConditionData_NullIndexThenExplicitIndex_SerializesThroughCodec()
    {
        var context = FormListCatalogTests.CreateContexts()[0];
        var formKey = new FormKey((ModKey)"Source.esm", 0x123);
        var source = new FormList(formKey, StarfieldRelease.Starfield);
        source.ConditionalEntries.Add(new FormListConditionalEntry
        {
            Index = null,
            Conditions = new ExtendedList<Condition>
            {
                new ConditionFloat
                {
                    ComparisonValue = 1.0f,
                    Data = new BiomeHasKeywordConditionData(),
                },
            },
        });
        var seed = CaptureSeed(context, formKey, WriteReadView(source));
        var command = context.SchemaCatalog.Nodes.Single(key => key.Kind == RecordWireSchemaNodeKind.Command && key.Name == "starfield.form-list.set-conditional-entries");
        var created = new FormListDraftFactory().Create(context, command, seed, FormListDraftSeedSelection.CurrentValue(), RecordWireReadLimits.Default);

        created.Succeeded.ShouldBeTrue(created.Error?.Message);
        var condition = FindSelectedCondition(created.Value!);
        var dataUnion = condition.FindProperty("Data").ShouldBeOfType<RecordWireUnionDraftNode>();
        var selection = dataUnion.SelectOption("Mutagen.Bethesda.Starfield.BiomeHasKeywordConditionData");

        selection.Succeeded.ShouldBeTrue(selection.Error?.Message);
        var conditionData = selection.Value.ShouldBeOfType<RecordWireObjectDraftNode>();
        var usesAliases = conditionData.FindProperty("UseAliases").ShouldBeOfType<RecordWireBooleanDraftNode>();
        var firstParameter = conditionData.FindProperty("FirstParameter").ShouldBeOfType<RecordWireFormLinkOrIndexDraftNode>();
        firstParameter.IsIndexNull.ShouldBeTrue();
        firstParameter.IndexText.ShouldBeEmpty();
        var nullSerialization = new FormListDraftSerializer(new FormListDraftValidator()).Serialize(created.Value!, RecordWireReadLimits.Default);
        nullSerialization.Succeeded.ShouldBeTrue(string.Join(Environment.NewLine, nullSerialization.Issues.Select(issue => issue.Message)));
        nullSerialization.GetArguments().GetProperty("conditionalEntries")[0].GetProperty("Conditions")[0].GetProperty("Data").GetProperty("FirstParameter").GetProperty("index").ValueKind.ShouldBe(JsonValueKind.Null);

        usesAliases.Value = true;
        firstParameter.IsIndexNull = false;
        firstParameter.IndexText = "17";

        firstParameter.ActiveBranch.ShouldBe(RecordWireFormLinkOrIndexActiveBranch.AliasIndex);
        var serialized = new FormListDraftSerializer(new FormListDraftValidator()).Serialize(created.Value!, RecordWireReadLimits.Default);
        serialized.Succeeded.ShouldBeTrue(string.Join(Environment.NewLine, serialized.Issues.Select(issue => issue.Message)));
        var decoded = context.Codec.Decode(command.Name, serialized.GetArguments(), RecordWireReadLimits.Default);
        decoded.Succeeded.ShouldBeTrue(decoded.Error?.Message);
        var replacement = decoded.Value.ShouldBeOfType<StarfieldSetConditionalEntriesEdit>();
        var decodedData = replacement.Entries.Single().Conditions!.Single().ShouldBeOfType<ConditionFloat>().Data.ShouldBeOfType<BiomeHasKeywordConditionData>();
        decodedData.UseAliases.ShouldBeTrue();
        decodedData.FirstParameter.UsesAlias().ShouldBeTrue();
        decodedData.FirstParameter.Index.ShouldBe(17u);
    }

    /// <summary>Verifies lazy union and nullable values use their current paths after ordered parents move and remove siblings.</summary>
    [Fact]
    public void LazyValues_AfterParentMoveAndRemove_UseCurrentPaths()
    {
        var context = FormListCatalogTests.CreateContexts()[0];
        var seed = CaptureSeed(context, "\"EditorID\":null,\"Items\":[],\"Components\":[],\"ConditionalEntries\":[]");
        var command = context.SchemaCatalog.Nodes.Single(key => key.Kind == RecordWireSchemaNodeKind.Command && key.Name == "starfield.form-list.set-conditional-entries");
        var draft = new FormListDraftFactory().Create(context, command, seed, FormListDraftSeedSelection.CurrentValue(), RecordWireReadLimits.Default).Value!;
        var entries = ((RecordWireObjectDraftNode)draft.Root).FindProperty("conditionalEntries").ShouldBeOfType<RecordWireArrayDraftNode>();
        entries.Insert(0).Succeeded.ShouldBeTrue();
        var retainedEntry = entries.Insert(1).Value!.ShouldBeOfType<RecordWireObjectDraftNode>();
        var index = retainedEntry.FindProperty("Index").ShouldBeOfType<RecordWireNullableDraftNode>();
        var conditions = retainedEntry.FindProperty("Conditions").ShouldBeOfType<RecordWireNullableDraftNode>().SetPresent().Value!.ShouldBeOfType<RecordWireArrayDraftNode>();
        var condition = conditions.Insert(0).Value!.ShouldBeOfType<RecordWireUnionDraftNode>();

        entries.Move(1, 0).ShouldBeTrue();
        entries.Remove(1).ShouldBeTrue();
        condition.Path.ShouldBe("$.conditionalEntries[0].Conditions[0]");
        index.Path.ShouldBe("$.conditionalEntries[0].Index");

        var conditionSelection = condition.SelectOption("Mutagen.Bethesda.Starfield.ConditionFloat");
        conditionSelection.Succeeded.ShouldBeTrue(conditionSelection.Error?.Message);
        conditionSelection.Value!.Path.ShouldBe(condition.Path);
        var presentIndex = index.SetPresent();
        presentIndex.Succeeded.ShouldBeTrue(presentIndex.Error?.Message);
        var indexValue = presentIndex.Value.ShouldBeOfType<RecordWireIntegerDraftNode>();
        indexValue.Path.ShouldBe(index.Path);
        indexValue.Text = "-1";

        var serialized = new FormListDraftSerializer(new FormListDraftValidator()).Serialize(draft, RecordWireReadLimits.Default);
        serialized.Succeeded.ShouldBeFalse();
        serialized.Issues.ShouldContain(issue => issue.Code == RecordWireDraftIssueCode.NumericValueOutOfRange && issue.Path == index.Path && issue.Message.StartsWith(index.Path, StringComparison.Ordinal));
    }

    /// <summary>Verifies editing a seeded asset path omits stale normalized projections and decodes through the real Starfield codec.</summary>
    [Fact]
    public void Create_SeededModelAssetEdit_OmitsStaleDerivedPathsAndDecodes()
    {
        const string replacementPath = "Meshes\\CreationsForge\\Replacement.nif";
        var context = FormListCatalogTests.CreateContexts()[0];
        var formKey = new FormKey((ModKey)"Source.esm", 0x123);
        var source = new FormList(formKey, StarfieldRelease.Starfield);
        source.Components.Add(new ModelComponent
        {
            Model = new Model
            {
                File = new AssetLink<StarfieldModelAssetType>("Meshes\\CreationsForge\\Original.nif"),
            },
        });
        var seed = CaptureSeed(context, formKey, WriteReadView(source));
        var command = context.SchemaCatalog.Nodes.Single(key => key.Kind == RecordWireSchemaNodeKind.Command && key.Name == "starfield.form-list.replace-component");
        var created = new FormListDraftFactory().Create(context, command, seed, FormListDraftSeedSelection.AtIndex(0), RecordWireReadLimits.Default);

        created.Succeeded.ShouldBeTrue(created.Error?.Message);
        var root = created.Value!.Root.ShouldBeOfType<RecordWireObjectDraftNode>();
        var component = root.FindProperty("component").ShouldBeOfType<RecordWireUnionDraftNode>().SelectedValue.ShouldBeOfType<RecordWireObjectDraftNode>();
        var model = component.FindProperty("Model").ShouldBeOfType<RecordWireNullableDraftNode>().Value.ShouldBeOfType<RecordWireObjectDraftNode>();
        var file = model.FindProperty("File").ShouldBeOfType<RecordWireNullableDraftNode>().Value.ShouldBeOfType<RecordWireAssetDraftNode>();
        var assetValue = file.FindProperty("value").ShouldBeAssignableTo<RecordWireObjectDraftNode>()!;
        var givenPath = assetValue.FindProperty("givenPath").ShouldBeOfType<RecordWireStringDraftNode>();
        var dataRelativePath = assetValue.FindProperty("dataRelativePath").ShouldBeOfType<RecordWireStringDraftNode>();
        var extension = assetValue.FindProperty("extension").ShouldBeOfType<RecordWireStringDraftNode>();
        dataRelativePath.ValueState.ShouldBe(RecordWireDraftValueState.Seeded);
        extension.ValueState.ShouldBe(RecordWireDraftValueState.Seeded);

        givenPath.Value = replacementPath;

        dataRelativePath.ValueState.ShouldBe(RecordWireDraftValueState.RequiredUnset);
        extension.ValueState.ShouldBe(RecordWireDraftValueState.RequiredUnset);
        var serialized = new FormListDraftSerializer(new FormListDraftValidator()).Serialize(created.Value, RecordWireReadLimits.Default);
        serialized.Succeeded.ShouldBeTrue(string.Join(Environment.NewLine, serialized.Issues.Select(issue => issue.Message)));
        var serializedAssetValue = serialized.GetArguments().GetProperty("component").GetProperty("Model").GetProperty("File").GetProperty("value");
        serializedAssetValue.TryGetProperty("dataRelativePath", out _).ShouldBeFalse();
        serializedAssetValue.TryGetProperty("extension", out _).ShouldBeFalse();
        var decoded = context.Codec.Decode(command.Name, serialized.GetArguments(), RecordWireReadLimits.Default);
        decoded.Succeeded.ShouldBeTrue(decoded.Error?.Message);
        var replacement = decoded.Value.ShouldBeOfType<StarfieldReplaceComponentEdit>();
        replacement.Component.ShouldBeOfType<ModelComponent>().Model!.File!.GivenPath.ShouldBe(replacementPath);
    }

    /// <summary>Verifies bounded seed capture rejects over-limit input without truncating it.</summary>
    [Fact]
    public void CaptureSeed_OverNodeLimit_FailsWithoutSeed()
    {
        var context = FormListCatalogTests.CreateContexts()[0];
        var formKey = new FormKey((ModKey)"Source.esm", 0x123);
        using var document = JsonDocument.Parse($"{{\"FormKey\":\"{formKey}\",\"Items\":[1,2,3]}}");
        var limits = new RecordWireReadLimits(64, 2, 10, 100, 100);

        var result = new FormListDraftFactory().CaptureSeed(context, Guid.NewGuid(), new WorkspaceRevision(Guid.NewGuid(), 1), CreateContext(formKey), document.RootElement, limits);

        result.Succeeded.ShouldBeFalse();
        result.Error!.Message.ShouldContain("maximum node count 2");
    }

    /// <summary>Creates a condition replacement draft with one newly inserted explicitly selected condition type.</summary>
    private static FormListDraft CreateFreshConditionDraft(CreationsForge.Services.FormListWireCatalogContext context, string conditionType)
    {
        var seed = CaptureSeed(context, "\"EditorID\":null,\"Items\":[],\"Components\":[],\"ConditionalEntries\":[]");
        var command = context.SchemaCatalog.Nodes.Single(key => key.Kind == RecordWireSchemaNodeKind.Command && key.Name == "starfield.form-list.set-conditional-entries");
        var draft = new FormListDraftFactory().Create(context, command, seed, FormListDraftSeedSelection.CurrentValue(), RecordWireReadLimits.Default).Value!;
        var entries = ((RecordWireObjectDraftNode)draft.Root).FindProperty("conditionalEntries").ShouldBeOfType<RecordWireArrayDraftNode>();
        var entry = entries.Insert(0).Value!.ShouldBeOfType<RecordWireObjectDraftNode>();
        entry.FindProperty("Index").ShouldBeOfType<RecordWireNullableDraftNode>().SetNull();
        var conditionsNullable = entry.FindProperty("Conditions").ShouldBeOfType<RecordWireNullableDraftNode>();
        var conditions = conditionsNullable.SetPresent().Value!.ShouldBeOfType<RecordWireArrayDraftNode>();
        var condition = conditions.Insert(0).Value!.ShouldBeOfType<RecordWireUnionDraftNode>();
        condition.SelectOption(conditionType).Succeeded.ShouldBeTrue();
        return draft;
    }

    /// <summary>Finds the single selected condition object in a fresh test draft.</summary>
    private static RecordWireObjectDraftNode FindSelectedCondition(FormListDraft draft)
    {
        var entries = ((RecordWireObjectDraftNode)draft.Root).FindProperty("conditionalEntries").ShouldBeOfType<RecordWireArrayDraftNode>();
        var entry = entries.Items[0].ShouldBeOfType<RecordWireObjectDraftNode>();
        var conditions = entry.FindProperty("Conditions").ShouldBeOfType<RecordWireNullableDraftNode>().Value.ShouldBeOfType<RecordWireArrayDraftNode>();
        return conditions.Items[0].ShouldBeOfType<RecordWireUnionDraftNode>().SelectedValue.ShouldBeOfType<RecordWireObjectDraftNode>();
    }

    /// <summary>Captures one synthetic detached Starfield record seed with a real matching FormKey.</summary>
    private static FormListDraftSeed CaptureSeed(CreationsForge.Services.FormListWireCatalogContext context, string recordMembers)
    {
        var formKey = new FormKey((ModKey)"Source.esm", 0x123);
        using var document = JsonDocument.Parse($"{{\"MajorRecordFlagsRaw\":0,\"FormKey\":\"{formKey}\",\"VersionControl\":1,\"FormVersion\":131,\"Version2\":0,\"StarfieldMajorRecordFlags\":0,{recordMembers}}}");
        var result = new FormListDraftFactory().CaptureSeed(context, Guid.NewGuid(), new WorkspaceRevision(Guid.NewGuid(), 1), CreateContext(formKey), document.RootElement, RecordWireReadLimits.Default);
        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        return result.Value!;
    }

    /// <summary>Captures one supplied detached record view as an exact revision-bound seed.</summary>
    /// <param name="context">The exact active Starfield catalog and codec.</param>
    /// <param name="formKey">The detached record identity.</param>
    /// <param name="record">The complete production-inspector record view.</param>
    /// <returns>The validated detached seed.</returns>
    private static FormListDraftSeed CaptureSeed(CreationsForge.Services.FormListWireCatalogContext context, FormKey formKey, JsonElement record)
    {
        var result = new FormListDraftFactory().CaptureSeed(context, Guid.NewGuid(), new WorkspaceRevision(Guid.NewGuid(), 1), CreateContext(formKey), record, RecordWireReadLimits.Default);
        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        return result.Value!;
    }

    /// <summary>Writes one complete detached Starfield FormList through the production plugin inspector.</summary>
    /// <param name="formList">The typed record to inspect.</param>
    /// <returns>A detached complete JSON record view.</returns>
    private static JsonElement WriteReadView(FormList formList)
    {
        var buffer = new ArrayBufferWriter<byte>();
        using (var writer = new Utf8JsonWriter(buffer))
        {
            new StarfieldFormListInspector().WriteReadView(formList, writer, CancellationToken.None);
        }

        using var document = JsonDocument.Parse(buffer.WrittenMemory);
        return document.RootElement.Clone();
    }

    /// <summary>Creates a direct delegate to the generated plugin reader used by the Core default-contract tests.</summary>
    /// <returns>The exact generated reader delegate.</returns>
    private static ReadRecordTypeDelegate CreateReadRecordTypeDelegate()
    {
        var codecType = typeof(StarfieldFormListInspector).Assembly.GetType("CreationsForge.Starfield.PluginAdapter.RecordInspection.StarfieldNestedFieldCodec", throwOnError: true)!;
        var method = codecType.GetMethod(
            "ReadRecordType",
            BindingFlags.NonPublic | BindingFlags.Static,
            binder: null,
            types: [typeof(RecordWireReadContext), typeof(RecordWireValue), typeof(string)],
            modifiers: null)
            ?? throw new InvalidOperationException("Generated plugin reader method was not found.");
        return method.CreateDelegate<ReadRecordTypeDelegate>();
    }

    /// <summary>Represents the generated concrete plugin reader signature.</summary>
    /// <param name="context">The bounded plugin read context.</param>
    /// <param name="value">The context-owned wire value.</param>
    /// <param name="recordTypeName">The exact concrete plugin type name.</param>
    /// <returns>The completely constructed plugin object.</returns>
    private delegate object ReadRecordTypeDelegate(RecordWireReadContext context, RecordWireValue value, string recordTypeName);

    /// <summary>Creates one exact resolved source context for a synthetic detached record.</summary>
    private static FormListContext CreateContext(FormKey formKey)
    {
        var modKey = formKey.ModKey;
        return new FormListContext(new ReferenceRequest(formKey, RecordScope.Source, modKey), ReferenceResolutionStatus.Resolved, modKey, Path.GetFullPath("Source.esm"), 0, PluginRole.Source);
    }
}
