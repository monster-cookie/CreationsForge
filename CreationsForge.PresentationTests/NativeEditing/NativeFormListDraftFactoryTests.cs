using System.Buffers;
using System.Reflection;
using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeWire;
using CreationsForge.NativeEditing.Drafts;
using CreationsForge.NativeEditing.Schema;
using CreationsForge.Starfield.Native.Edits;
using CreationsForge.Starfield.Native.NativeInspection;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Starfield;
using Mutagen.Bethesda.Starfield.Assets;
using Noggog;
using Shouldly;

namespace CreationsForge.PresentationTests.NativeEditing;

/// <summary>Verifies revision-bound seed extraction, required-unset defaults, and exact command serialization.</summary>
public sealed class NativeFormListDraftFactoryTests
{
    /// <summary>The generated native reader used to verify every available default after presentation serialization.</summary>
    private static readonly ReadNativeTypeDelegate ReadNativeType = CreateReadNativeTypeDelegate();

    /// <summary>Verifies a seeded setter begins with the current value and decodes after an explicit typed edit.</summary>
    [Fact]
    public void Create_SeededEditorId_PreservesThenSerializesTypedValue()
    {
        var context = NativeFormListCatalogTests.CreateContexts()[0];
        var seed = CaptureSeed(context, "\"EditorID\":\"OldId\",\"Items\":[],\"Components\":[],\"ConditionalEntries\":[]");
        var command = context.SchemaCatalog.Nodes.Single(key => key.Kind == NativeWireSchemaNodeKind.Command && key.Name == "form-list.set-editor-id");
        var factory = new NativeFormListDraftFactory();

        var created = factory.Create(context, command, seed, NativeFormListDraftSeedSelection.CurrentValue(), NativeWireReadLimits.Default);

        created.Succeeded.ShouldBeTrue(created.Error?.Message);
        var root = created.Value!.Root.ShouldBeOfType<NativeWireObjectDraftNode>();
        var editorId = root.FindProperty("editorId").ShouldBeOfType<NativeWireStringDraftNode>();
        editorId.Value.ShouldBe("OldId");
        editorId.Value = "NewId";
        created.Value.HasChanges.ShouldBeTrue();
        var serialized = new NativeFormListDraftSerializer(new NativeFormListDraftValidator()).Serialize(created.Value, NativeWireReadLimits.Default);
        serialized.Succeeded.ShouldBeTrue(string.Join(Environment.NewLine, serialized.Issues.Select(issue => issue.Message)));
        serialized.GetArguments().GetProperty("editorId").GetString().ShouldBe("NewId");
        context.Codec.Decode(command.Name, serialized.GetArguments(), NativeWireReadLimits.Default).Succeeded.ShouldBeTrue();
    }

    /// <summary>Verifies complete seeded item replacement preserves order, duplicates, and both null-link identities.</summary>
    [Fact]
    public void Create_ReplaceItems_PreservesCompleteOrderedSeed()
    {
        var context = NativeFormListCatalogTests.CreateContexts()[0];
        var seed = CaptureSeed(
            context,
            "\"EditorID\":null,\"Items\":[{\"isNull\":true,\"formKey\":null},{\"isNull\":true,\"formKey\":\"Null\"},{\"isNull\":false,\"formKey\":\"000123:Master.esm\"},{\"isNull\":false,\"formKey\":\"000123:Master.esm\"}],\"Components\":[],\"ConditionalEntries\":[]");
        var command = context.SchemaCatalog.Nodes.Single(key => key.Kind == NativeWireSchemaNodeKind.Command && key.Name == "form-list.replace-items");

        var draft = new NativeFormListDraftFactory().Create(context, command, seed, NativeFormListDraftSeedSelection.CurrentValue(), NativeWireReadLimits.Default).Value!;
        var serialized = new NativeFormListDraftSerializer(new NativeFormListDraftValidator()).Serialize(draft, NativeWireReadLimits.Default);

        serialized.Succeeded.ShouldBeTrue(string.Join(Environment.NewLine, serialized.Issues.Select(issue => issue.Message)));
        serialized.GetArguments().GetProperty("items").GetRawText().ShouldBe("[{\"isNull\":true,\"formKey\":null},{\"isNull\":true,\"formKey\":\"Null\"},{\"isNull\":false,\"formKey\":\"000123:Master.esm\"},{\"isNull\":false,\"formKey\":\"000123:Master.esm\"}]");
    }

    /// <summary>Verifies each unavailable component or condition default exposes only its exact required-unset siblings.</summary>
    [Fact]
    public void SelectUnavailableGeneratedTypes_ExposesExactRequiredUnsetFields()
    {
        var context = NativeFormListCatalogTests.CreateContexts()[0];
        var componentCommand = context.SchemaCatalog.Nodes.Single(key => key.Kind == NativeWireSchemaNodeKind.Command && key.Name == "starfield.form-list.add-component");
        var componentDraft = new NativeFormListDraftFactory().Create(context, componentCommand, null, NativeFormListDraftSeedSelection.InsertAt(0), NativeWireReadLimits.Default).Value!;
        var componentUnion = ((NativeWireObjectDraftNode)componentDraft.Root).FindProperty("component").ShouldBeOfType<NativeWireUnionDraftNode>();

        var componentSelection = componentUnion.SelectOption("Mutagen.Bethesda.Starfield.VolumesComponent");
        componentSelection.Succeeded.ShouldBeTrue(componentSelection.Error?.Message);
        var selectedComponent = componentSelection.Value!.ShouldBeOfType<NativeWireObjectDraftNode>();
        var items = selectedComponent.FindProperty("Items").ShouldBeOfType<NativeWireNullableDraftNode>();
        var presentItems = items.IsNull ? items.SetPresent().Value!.ShouldBeOfType<NativeWireArrayDraftNode>() : items.Value!.ShouldBeOfType<NativeWireArrayDraftNode>();
        var selectedItem = presentItems.Insert(0).Value!.ShouldBeOfType<NativeWireObjectDraftNode>();
        selectedItem.Properties.Where(property => property.Node.ValueState == NativeWireDraftValueState.RequiredUnset).Select(property => property.Name).ShouldBe(
            ["Matrix1", "Matrix2", "Matrix3", "Matrix4", "Unknown1", "Unknown2", "Unknown3", "Ender"]);
        new NativeFormListDraftValidator().Validate(componentDraft, NativeWireReadLimits.Default).Issues.Count.ShouldBe(8);

        foreach (var conditionType in new[] { "Mutagen.Bethesda.Starfield.ConditionFloat", "Mutagen.Bethesda.Starfield.ConditionGlobal" })
        {
            var conditionDraft = CreateFreshConditionDraft(context, conditionType);
            var condition = FindSelectedCondition(conditionDraft);
            condition.Properties.Where(property => property.Node.ValueState == NativeWireDraftValueState.RequiredUnset).Select(property => property.Name).ShouldBe(
                ["CompareOperator", "Flags", "Unknown1", "Unknown2", "Data", "ComparisonValue"]);
            new NativeFormListDraftValidator().Validate(conditionDraft, NativeWireReadLimits.Default).Issues.Count.ShouldBe(6);
        }
    }

    /// <summary>Verifies all available generated concrete defaults hydrate and serialize through the presentation graph before native decoding.</summary>
    [Fact]
    public void GeneratedConcreteDefaults_All719AvailableHydrateSerializeAndDecode()
    {
        var context = NativeFormListCatalogTests.CreateContexts()[0];
        var failures = new List<string>();
        var availableCount = 0;
        var unavailable = new List<string>();
        var concreteNodes = context.SchemaCatalog.Nodes
            .Where(key => key.Kind == NativeWireSchemaNodeKind.Type)
            .Select(key => context.SchemaCatalog.ReadNode(key).Value!)
            .Where(node => node.Schema.TryGetProperty("x-native-construction", out _))
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
                var descriptorResult = new NativeWireSchemaIndex(context).Resolve(node.Key, NativeWireReadLimits.Default);
                if (!descriptorResult.Succeeded || descriptorResult.Value is null)
                {
                    failures.Add($"{node.Key.Name}: schema: {descriptorResult.Error?.Message}");
                    continue;
                }

                var builder = new NativeFormListDraftFactory.DraftBuildState(NativeWireReadLimits.Default, TestContext.Current.CancellationToken);
                var root = builder.Build(descriptorResult.Value, "$", node.Key.Name, true, false, node.DefaultTemplate.Value, NativeWireDraftValueState.Defaulted, false, 1);
                var draft = new NativeFormListDraft(node.Key, context.Identity, null, root, NativeWireReadLimits.Default);
                var serialized = new NativeFormListDraftSerializer(new NativeFormListDraftValidator()).Serialize(draft, NativeWireReadLimits.Default, TestContext.Current.CancellationToken);
                if (!serialized.Succeeded)
                {
                    failures.Add($"{node.Key.Name}: presentation: {string.Join(" | ", serialized.Issues.Select(issue => $"{issue.Path}: {issue.Message}"))}");
                    continue;
                }

                var decoded = NativeWireReadContext.Decode<object>(
                    serialized.GetArguments(),
                    NativeWireReadLimits.Default,
                    TestContext.Current.CancellationToken,
                    (readContext, value) => ReadNativeType(readContext, value, node.Key.Name));
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

        failures.ShouldBeEmpty("Every advertised available generated default must survive the complete presentation graph and native reader.");
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
        var context = NativeFormListCatalogTests.CreateContexts()[0];
        var command = context.SchemaCatalog.Nodes.Single(key => key.Kind == NativeWireSchemaNodeKind.Command && key.Name == "starfield.form-list.add-component");
        var created = new NativeFormListDraftFactory().Create(context, command, null, NativeFormListDraftSeedSelection.InsertAt(0), NativeWireReadLimits.Default);

        created.Succeeded.ShouldBeTrue(created.Error?.Message);
        var component = ((NativeWireObjectDraftNode)created.Value!.Root).FindProperty("component").ShouldBeOfType<NativeWireUnionDraftNode>();
        var selection = component.SelectOption("Mutagen.Bethesda.Starfield.OrbitedDataComponent");

        selection.Succeeded.ShouldBeTrue(selection.Error?.Message);
        var selected = selection.Value.ShouldBeOfType<NativeWireObjectDraftNode>();
        var unknown1 = selected.FindProperty("Unknown1").ShouldBeOfType<NativeWireUnionDraftNode>();
        unknown1.SelectedValue.ShouldBeOfType<NativeWireIntegerDraftNode>().Text.ShouldBe("0");
        var serialized = new NativeFormListDraftSerializer(new NativeFormListDraftValidator()).Serialize(created.Value, NativeWireReadLimits.Default);
        serialized.Succeeded.ShouldBeTrue(string.Join(Environment.NewLine, serialized.Issues.Select(issue => issue.Message)));
        var decoded = context.Codec.Decode(command.Name, serialized.GetArguments(), NativeWireReadLimits.Default);
        decoded.Succeeded.ShouldBeTrue(decoded.Error?.Message);
        decoded.Value.ShouldBeOfType<StarfieldAddComponentEdit>().Component.ShouldBeOfType<OrbitedDataComponent>().Unknown1.ShouldBe(0ul);
    }

    /// <summary>Verifies token-kind selection remains fail-closed when two alternatives admit the same top-level JSON kind.</summary>
    [Fact]
    public void HydrateUnion_WhenTokenKindMatchesTwoAlternatives_RejectsInference()
    {
        var stringDescriptor = new NativeWireSchemaDescriptor(NativeWireSchemaValueKind.String, "String", description: null);
        var options = new[]
        {
            new NativeWireSchemaUnionOption("first", "First", null, (_, _) => EngineResult<NativeWireSchemaDescriptor>.Success(stringDescriptor)),
            new NativeWireSchemaUnionOption("second", "Second", null, (_, _) => EngineResult<NativeWireSchemaDescriptor>.Success(stringDescriptor)),
        };
        var unionDescriptor = new NativeWireSchemaDescriptor(NativeWireSchemaValueKind.Union, "Ambiguous", description: null, unionOptions: options);
        using var value = JsonDocument.Parse("\"same-kind\"");
        var builder = new NativeFormListDraftFactory.DraftBuildState(NativeWireReadLimits.Default, TestContext.Current.CancellationToken);

        var exception = Should.Throw<Exception>(() => builder.Build(unionDescriptor, "$.value", "Value", true, false, value.RootElement, NativeWireDraftValueState.Seeded, true, 1));

        exception.GetType().Name.ShouldBe("DraftBuildException");
        exception.Message.ShouldBe("$.value: seeded union value has no exact discriminator and its String token matches 2 alternatives; selection would require inference.");
    }

    /// <summary>Verifies real owner-derived FormLink-or-index fields track their exact sibling flags while retaining both projections.</summary>
    [Fact]
    public void Create_SeededAnnotatedCondition_OwnerFlagsRefreshFormLinkOrIndexBranch()
    {
        var context = NativeFormListCatalogTests.CreateContexts()[0];
        var formKey = new FormKey((ModKey)"Source.esm", 0x123);
        var parameterFormKey = new FormKey((ModKey)"Source.esm", 0x456);
        var nativeData = new BiomeHasKeywordConditionData();
        nativeData.FirstParameter.Index = 17;
        nativeData.FirstParameter.Link.SetTo(parameterFormKey);
        var source = new FormList(formKey, StarfieldRelease.Starfield);
        source.ConditionalEntries.Add(new FormListConditionalEntry
        {
            Index = null,
            Conditions = new ExtendedList<Condition>
            {
                new ConditionFloat
                {
                    ComparisonValue = 1.0f,
                    Data = nativeData,
                },
            },
        });
        var seed = CaptureSeed(context, formKey, WriteReadView(source));
        var command = context.SchemaCatalog.Nodes.Single(key => key.Kind == NativeWireSchemaNodeKind.Command && key.Name == "starfield.form-list.set-conditional-entries");
        var created = new NativeFormListDraftFactory().Create(context, command, seed, NativeFormListDraftSeedSelection.CurrentValue(), NativeWireReadLimits.Default);

        created.Succeeded.ShouldBeTrue(created.Error?.Message);
        var draft = created.Value!;
        var condition = FindSelectedCondition(draft);
        var conditionData = condition.FindProperty("Data").ShouldBeOfType<NativeWireUnionDraftNode>().SelectedValue.ShouldBeOfType<NativeWireObjectDraftNode>();
        var usesAliases = conditionData.FindProperty("UseAliases").ShouldBeOfType<NativeWireBooleanDraftNode>();
        var usesPackageData = conditionData.FindProperty("UsePackageData").ShouldBeOfType<NativeWireBooleanDraftNode>();
        var firstParameter = conditionData.FindProperty("FirstParameter").ShouldBeOfType<NativeWireFormLinkOrIndexDraftNode>();

        firstParameter.ActiveBranch.ShouldBe(NativeWireFormLinkOrIndexActiveBranch.Link);
        firstParameter.IsIndexNull.ShouldBeFalse();
        usesAliases.Value = true;
        firstParameter.ActiveBranch.ShouldBe(NativeWireFormLinkOrIndexActiveBranch.AliasIndex);
        usesPackageData.Value = true;
        firstParameter.ActiveBranch.ShouldBe(NativeWireFormLinkOrIndexActiveBranch.AliasIndex);
        usesAliases.Value = false;
        firstParameter.ActiveBranch.ShouldBe(NativeWireFormLinkOrIndexActiveBranch.PackageDataIndex);
        usesPackageData.Value = false;
        firstParameter.ActiveBranch.ShouldBe(NativeWireFormLinkOrIndexActiveBranch.Link);
        firstParameter.IndexText.ShouldBe("17");
        firstParameter.Link.IsNull.ShouldBeFalse();
        firstParameter.Link.FormKey.ShouldBe(parameterFormKey.ToString());
        var serialized = new NativeFormListDraftSerializer(new NativeFormListDraftValidator()).Serialize(draft, NativeWireReadLimits.Default);
        serialized.Succeeded.ShouldBeTrue(string.Join(Environment.NewLine, serialized.Issues.Select(issue => issue.Message)));
        var decoded = context.Codec.Decode(command.Name, serialized.GetArguments(), NativeWireReadLimits.Default);
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
        var context = NativeFormListCatalogTests.CreateContexts()[0];
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
        var command = context.SchemaCatalog.Nodes.Single(key => key.Kind == NativeWireSchemaNodeKind.Command && key.Name == "starfield.form-list.set-conditional-entries");
        var created = new NativeFormListDraftFactory().Create(context, command, seed, NativeFormListDraftSeedSelection.CurrentValue(), NativeWireReadLimits.Default);

        created.Succeeded.ShouldBeTrue(created.Error?.Message);
        var condition = FindSelectedCondition(created.Value!);
        var dataUnion = condition.FindProperty("Data").ShouldBeOfType<NativeWireUnionDraftNode>();
        var selection = dataUnion.SelectOption("Mutagen.Bethesda.Starfield.BiomeHasKeywordConditionData");

        selection.Succeeded.ShouldBeTrue(selection.Error?.Message);
        var conditionData = selection.Value.ShouldBeOfType<NativeWireObjectDraftNode>();
        var usesAliases = conditionData.FindProperty("UseAliases").ShouldBeOfType<NativeWireBooleanDraftNode>();
        var firstParameter = conditionData.FindProperty("FirstParameter").ShouldBeOfType<NativeWireFormLinkOrIndexDraftNode>();
        firstParameter.IsIndexNull.ShouldBeTrue();
        firstParameter.IndexText.ShouldBeEmpty();
        var nullSerialization = new NativeFormListDraftSerializer(new NativeFormListDraftValidator()).Serialize(created.Value!, NativeWireReadLimits.Default);
        nullSerialization.Succeeded.ShouldBeTrue(string.Join(Environment.NewLine, nullSerialization.Issues.Select(issue => issue.Message)));
        nullSerialization.GetArguments().GetProperty("conditionalEntries")[0].GetProperty("Conditions")[0].GetProperty("Data").GetProperty("FirstParameter").GetProperty("index").ValueKind.ShouldBe(JsonValueKind.Null);

        usesAliases.Value = true;
        firstParameter.IsIndexNull = false;
        firstParameter.IndexText = "17";

        firstParameter.ActiveBranch.ShouldBe(NativeWireFormLinkOrIndexActiveBranch.AliasIndex);
        var serialized = new NativeFormListDraftSerializer(new NativeFormListDraftValidator()).Serialize(created.Value!, NativeWireReadLimits.Default);
        serialized.Succeeded.ShouldBeTrue(string.Join(Environment.NewLine, serialized.Issues.Select(issue => issue.Message)));
        var decoded = context.Codec.Decode(command.Name, serialized.GetArguments(), NativeWireReadLimits.Default);
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
        var context = NativeFormListCatalogTests.CreateContexts()[0];
        var seed = CaptureSeed(context, "\"EditorID\":null,\"Items\":[],\"Components\":[],\"ConditionalEntries\":[]");
        var command = context.SchemaCatalog.Nodes.Single(key => key.Kind == NativeWireSchemaNodeKind.Command && key.Name == "starfield.form-list.set-conditional-entries");
        var draft = new NativeFormListDraftFactory().Create(context, command, seed, NativeFormListDraftSeedSelection.CurrentValue(), NativeWireReadLimits.Default).Value!;
        var entries = ((NativeWireObjectDraftNode)draft.Root).FindProperty("conditionalEntries").ShouldBeOfType<NativeWireArrayDraftNode>();
        entries.Insert(0).Succeeded.ShouldBeTrue();
        var retainedEntry = entries.Insert(1).Value!.ShouldBeOfType<NativeWireObjectDraftNode>();
        var index = retainedEntry.FindProperty("Index").ShouldBeOfType<NativeWireNullableDraftNode>();
        var conditions = retainedEntry.FindProperty("Conditions").ShouldBeOfType<NativeWireNullableDraftNode>().SetPresent().Value!.ShouldBeOfType<NativeWireArrayDraftNode>();
        var condition = conditions.Insert(0).Value!.ShouldBeOfType<NativeWireUnionDraftNode>();

        entries.Move(1, 0).ShouldBeTrue();
        entries.Remove(1).ShouldBeTrue();
        condition.Path.ShouldBe("$.conditionalEntries[0].Conditions[0]");
        index.Path.ShouldBe("$.conditionalEntries[0].Index");

        var conditionSelection = condition.SelectOption("Mutagen.Bethesda.Starfield.ConditionFloat");
        conditionSelection.Succeeded.ShouldBeTrue(conditionSelection.Error?.Message);
        conditionSelection.Value!.Path.ShouldBe(condition.Path);
        var presentIndex = index.SetPresent();
        presentIndex.Succeeded.ShouldBeTrue(presentIndex.Error?.Message);
        var indexValue = presentIndex.Value.ShouldBeOfType<NativeWireIntegerDraftNode>();
        indexValue.Path.ShouldBe(index.Path);
        indexValue.Text = "-1";

        var serialized = new NativeFormListDraftSerializer(new NativeFormListDraftValidator()).Serialize(draft, NativeWireReadLimits.Default);
        serialized.Succeeded.ShouldBeFalse();
        serialized.Issues.ShouldContain(issue => issue.Code == NativeWireDraftIssueCode.NumericValueOutOfRange && issue.Path == index.Path && issue.Message.StartsWith(index.Path, StringComparison.Ordinal));
    }

    /// <summary>Verifies editing a seeded asset path omits stale normalized projections and decodes through the real Starfield codec.</summary>
    [Fact]
    public void Create_SeededModelAssetEdit_OmitsStaleDerivedPathsAndDecodes()
    {
        const string replacementPath = "Meshes\\CreationsForge\\Replacement.nif";
        var context = NativeFormListCatalogTests.CreateContexts()[0];
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
        var command = context.SchemaCatalog.Nodes.Single(key => key.Kind == NativeWireSchemaNodeKind.Command && key.Name == "starfield.form-list.replace-component");
        var created = new NativeFormListDraftFactory().Create(context, command, seed, NativeFormListDraftSeedSelection.AtIndex(0), NativeWireReadLimits.Default);

        created.Succeeded.ShouldBeTrue(created.Error?.Message);
        var root = created.Value!.Root.ShouldBeOfType<NativeWireObjectDraftNode>();
        var component = root.FindProperty("component").ShouldBeOfType<NativeWireUnionDraftNode>().SelectedValue.ShouldBeOfType<NativeWireObjectDraftNode>();
        var model = component.FindProperty("Model").ShouldBeOfType<NativeWireNullableDraftNode>().Value.ShouldBeOfType<NativeWireObjectDraftNode>();
        var file = model.FindProperty("File").ShouldBeOfType<NativeWireNullableDraftNode>().Value.ShouldBeOfType<NativeWireAssetDraftNode>();
        var assetValue = file.FindProperty("value").ShouldBeAssignableTo<NativeWireObjectDraftNode>()!;
        var givenPath = assetValue.FindProperty("givenPath").ShouldBeOfType<NativeWireStringDraftNode>();
        var dataRelativePath = assetValue.FindProperty("dataRelativePath").ShouldBeOfType<NativeWireStringDraftNode>();
        var extension = assetValue.FindProperty("extension").ShouldBeOfType<NativeWireStringDraftNode>();
        dataRelativePath.ValueState.ShouldBe(NativeWireDraftValueState.Seeded);
        extension.ValueState.ShouldBe(NativeWireDraftValueState.Seeded);

        givenPath.Value = replacementPath;

        dataRelativePath.ValueState.ShouldBe(NativeWireDraftValueState.RequiredUnset);
        extension.ValueState.ShouldBe(NativeWireDraftValueState.RequiredUnset);
        var serialized = new NativeFormListDraftSerializer(new NativeFormListDraftValidator()).Serialize(created.Value, NativeWireReadLimits.Default);
        serialized.Succeeded.ShouldBeTrue(string.Join(Environment.NewLine, serialized.Issues.Select(issue => issue.Message)));
        var serializedAssetValue = serialized.GetArguments().GetProperty("component").GetProperty("Model").GetProperty("File").GetProperty("value");
        serializedAssetValue.TryGetProperty("dataRelativePath", out _).ShouldBeFalse();
        serializedAssetValue.TryGetProperty("extension", out _).ShouldBeFalse();
        var decoded = context.Codec.Decode(command.Name, serialized.GetArguments(), NativeWireReadLimits.Default);
        decoded.Succeeded.ShouldBeTrue(decoded.Error?.Message);
        var replacement = decoded.Value.ShouldBeOfType<StarfieldReplaceComponentEdit>();
        replacement.Component.ShouldBeOfType<ModelComponent>().Model!.File!.GivenPath.ShouldBe(replacementPath);
    }

    /// <summary>Verifies bounded seed capture rejects over-limit input without truncating it.</summary>
    [Fact]
    public void CaptureSeed_OverNodeLimit_FailsWithoutSeed()
    {
        var context = NativeFormListCatalogTests.CreateContexts()[0];
        var formKey = new FormKey((ModKey)"Source.esm", 0x123);
        using var document = JsonDocument.Parse($"{{\"FormKey\":\"{formKey}\",\"Items\":[1,2,3]}}");
        var limits = new NativeWireReadLimits(64, 2, 10, 100, 100);

        var result = new NativeFormListDraftFactory().CaptureSeed(context, Guid.NewGuid(), new WorkspaceRevision(Guid.NewGuid(), 1), CreateContext(formKey), document.RootElement, limits);

        result.Succeeded.ShouldBeFalse();
        result.Error!.Message.ShouldContain("maximum node count 2");
    }

    /// <summary>Creates a condition replacement draft with one newly inserted explicitly selected condition type.</summary>
    private static NativeFormListDraft CreateFreshConditionDraft(CreationsForge.Services.NativeFormListWireCatalogContext context, string conditionType)
    {
        var seed = CaptureSeed(context, "\"EditorID\":null,\"Items\":[],\"Components\":[],\"ConditionalEntries\":[]");
        var command = context.SchemaCatalog.Nodes.Single(key => key.Kind == NativeWireSchemaNodeKind.Command && key.Name == "starfield.form-list.set-conditional-entries");
        var draft = new NativeFormListDraftFactory().Create(context, command, seed, NativeFormListDraftSeedSelection.CurrentValue(), NativeWireReadLimits.Default).Value!;
        var entries = ((NativeWireObjectDraftNode)draft.Root).FindProperty("conditionalEntries").ShouldBeOfType<NativeWireArrayDraftNode>();
        var entry = entries.Insert(0).Value!.ShouldBeOfType<NativeWireObjectDraftNode>();
        entry.FindProperty("Index").ShouldBeOfType<NativeWireNullableDraftNode>().SetNull();
        var conditionsNullable = entry.FindProperty("Conditions").ShouldBeOfType<NativeWireNullableDraftNode>();
        var conditions = conditionsNullable.SetPresent().Value!.ShouldBeOfType<NativeWireArrayDraftNode>();
        var condition = conditions.Insert(0).Value!.ShouldBeOfType<NativeWireUnionDraftNode>();
        condition.SelectOption(conditionType).Succeeded.ShouldBeTrue();
        return draft;
    }

    /// <summary>Finds the single selected condition object in a fresh test draft.</summary>
    private static NativeWireObjectDraftNode FindSelectedCondition(NativeFormListDraft draft)
    {
        var entries = ((NativeWireObjectDraftNode)draft.Root).FindProperty("conditionalEntries").ShouldBeOfType<NativeWireArrayDraftNode>();
        var entry = entries.Items[0].ShouldBeOfType<NativeWireObjectDraftNode>();
        var conditions = entry.FindProperty("Conditions").ShouldBeOfType<NativeWireNullableDraftNode>().Value.ShouldBeOfType<NativeWireArrayDraftNode>();
        return conditions.Items[0].ShouldBeOfType<NativeWireUnionDraftNode>().SelectedValue.ShouldBeOfType<NativeWireObjectDraftNode>();
    }

    /// <summary>Captures one synthetic detached Starfield record seed with a real matching FormKey.</summary>
    private static NativeFormListDraftSeed CaptureSeed(CreationsForge.Services.NativeFormListWireCatalogContext context, string recordMembers)
    {
        var formKey = new FormKey((ModKey)"Source.esm", 0x123);
        using var document = JsonDocument.Parse($"{{\"MajorRecordFlagsRaw\":0,\"FormKey\":\"{formKey}\",\"VersionControl\":1,\"FormVersion\":131,\"Version2\":0,\"StarfieldMajorRecordFlags\":0,{recordMembers}}}");
        var result = new NativeFormListDraftFactory().CaptureSeed(context, Guid.NewGuid(), new WorkspaceRevision(Guid.NewGuid(), 1), CreateContext(formKey), document.RootElement, NativeWireReadLimits.Default);
        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        return result.Value!;
    }

    /// <summary>Captures one supplied detached record view as an exact revision-bound seed.</summary>
    /// <param name="context">The exact active Starfield catalog and codec.</param>
    /// <param name="formKey">The detached record identity.</param>
    /// <param name="record">The complete production-inspector record view.</param>
    /// <returns>The validated detached seed.</returns>
    private static NativeFormListDraftSeed CaptureSeed(CreationsForge.Services.NativeFormListWireCatalogContext context, FormKey formKey, JsonElement record)
    {
        var result = new NativeFormListDraftFactory().CaptureSeed(context, Guid.NewGuid(), new WorkspaceRevision(Guid.NewGuid(), 1), CreateContext(formKey), record, NativeWireReadLimits.Default);
        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        return result.Value!;
    }

    /// <summary>Writes one complete detached Starfield FormList through the production native inspector.</summary>
    /// <param name="formList">The typed native record to inspect.</param>
    /// <returns>A detached complete JSON record view.</returns>
    private static JsonElement WriteReadView(FormList formList)
    {
        var buffer = new ArrayBufferWriter<byte>();
        using (var writer = new Utf8JsonWriter(buffer))
        {
            new StarfieldFormListNativeInspector().WriteReadView(formList, writer, CancellationToken.None);
        }

        using var document = JsonDocument.Parse(buffer.WrittenMemory);
        return document.RootElement.Clone();
    }

    /// <summary>Creates a direct delegate to the generated native reader used by the Core default-contract tests.</summary>
    /// <returns>The exact generated reader delegate.</returns>
    private static ReadNativeTypeDelegate CreateReadNativeTypeDelegate()
    {
        var codecType = typeof(StarfieldFormListNativeInspector).Assembly.GetType("CreationsForge.Starfield.Native.NativeInspection.StarfieldNestedFieldCodec", throwOnError: true)!;
        var method = codecType.GetMethod(
            "ReadNativeType",
            BindingFlags.NonPublic | BindingFlags.Static,
            binder: null,
            types: [typeof(NativeWireReadContext), typeof(NativeWireValue), typeof(string)],
            modifiers: null)
            ?? throw new InvalidOperationException("Generated native reader method was not found.");
        return method.CreateDelegate<ReadNativeTypeDelegate>();
    }

    /// <summary>Represents the generated concrete native reader signature.</summary>
    /// <param name="context">The bounded native read context.</param>
    /// <param name="value">The context-owned wire value.</param>
    /// <param name="nativeTypeName">The exact concrete native type name.</param>
    /// <returns>The completely constructed native object.</returns>
    private delegate object ReadNativeTypeDelegate(NativeWireReadContext context, NativeWireValue value, string nativeTypeName);

    /// <summary>Creates one exact resolved source context for a synthetic detached record.</summary>
    private static FormListContext CreateContext(FormKey formKey)
    {
        var modKey = formKey.ModKey;
        return new FormListContext(new ReferenceRequest(formKey, RecordScope.Source, modKey), ReferenceResolutionStatus.Resolved, modKey, Path.GetFullPath("Source.esm"), 0, PluginRole.Source);
    }
}
