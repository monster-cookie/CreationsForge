using System.Collections;
using System.Reflection;
using CreationsForge.Engine.Persistence;
using CreationsForge.Engine.Records;
using CreationsForge.Engine.Workspaces;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;

namespace CreationsForge.UnitTests.Workspaces;

/// <summary>Verifies override placement keeps source parents and rejects duplicate identities.</summary>
public sealed partial class PluginWorkspaceTests
{
    /// <summary>Keeps persistent, temporary, navmesh, and landscape records on their source cell.</summary>
    [Theory]
    [InlineData(GameRelease.Starfield)]
    [InlineData(GameRelease.Fallout4)]
    [InlineData(GameRelease.SkyrimSE)]
    public async Task NestedOverridesKeepSourceParentAndList(GameRelease release)
    {
        using var directory = new TemporaryDirectory();
        var source = WritePlacementSource(directory.Path, release);
        var outputModKey = ModKey.FromNameAndExtension("Output.esp");
        using (var workspace = OpenPlacement(directory.Path, release, source.ModKey, outputModKey))
        {
            var mutations = new List<RecordMutation>
            {
                Override("PlacedObject", source.Persistent, source.ModKey, "PersistentEdited"),
                Override("PlacedObject", source.Temporary, source.ModKey, "TemporaryEdited"),
                Override("NavigationMesh", source.NavigationMesh, source.ModKey, "NavEdited"),
            };
            if (source.Landscape is { } landscape)
            {
                mutations.Add(Override("Landscape", landscape, source.ModKey, "LandEdited"));
            }

            workspace.Records.Apply(new RecordChangeSet(0, mutations));
            await SaveWorkspace(workspace);
        }

        using var reopened = CreateFactory().Open(CreateExistingRequest(directory.Path, release, outputModKey));
        var cell = RequireRecord(reopened.Output, source.InteriorCell);
        Assert.Equal(source.InteriorCell, CellContaining(reopened.Output, source.Persistent));
        Assert.Equal(source.InteriorCell, CellContaining(reopened.Output, source.Temporary));
        Assert.Equal(source.InteriorCell, CellContaining(reopened.Output, source.NavigationMesh));
        Assert.True(ListContains(cell, "Persistent", source.Persistent));
        Assert.True(ListContains(cell, "Temporary", source.Temporary));
        Assert.True(ListContains(cell, "NavigationMeshes", source.NavigationMesh));
        Assert.Equal(1, CountList(cell, "Persistent"));
        Assert.Equal(1, CountList(cell, "Temporary"));
        Assert.Equal(1, CountList(cell, "NavigationMeshes"));
        Assert.Equal(1, IdentityCount(reopened.Output, source.Persistent));
        Assert.Equal(1, IdentityCount(reopened.Output, source.Temporary));
        if (source.Landscape is { } landscapeKey)
        {
            Assert.Equal(source.InteriorCell, CellContaining(reopened.Output, landscapeKey));
            var stored = cell.GetType().GetProperty("Landscape")?.GetValue(cell) as IMajorRecordGetter;
            Assert.Equal(landscapeKey, stored?.FormKey);
        }

        Assert.Equal("PersistentEdited", EditorId(reopened, "PlacedObject", source.Persistent, outputModKey));
        Assert.Equal("TemporaryEdited", EditorId(reopened, "PlacedObject", source.Temporary, outputModKey));
    }

    /// <summary>Keeps one exterior reference under its worldspace after the worldspace itself is overridden.</summary>
    [Theory]
    [InlineData(GameRelease.Starfield)]
    [InlineData(GameRelease.Fallout4)]
    [InlineData(GameRelease.SkyrimSE)]
    public async Task ExteriorReferenceAppearsOnceAfterWorldspaceOverride(GameRelease release)
    {
        using var directory = new TemporaryDirectory();
        var source = WritePlacementSource(directory.Path, release);
        var outputModKey = ModKey.FromNameAndExtension("Output.esp");
        using (var workspace = OpenPlacement(directory.Path, release, source.ModKey, outputModKey))
        {
            workspace.Records.Apply(new RecordChangeSet(0,
            [
                Override("Worldspace", source.Worldspace, source.ModKey, "WorldEdited"),
                Override("PlacedObject", source.ExteriorPlaced, source.ModKey, "PlacedEdited"),
            ]));
            await SaveWorkspace(workspace);
        }

        using var reopened = CreateFactory().Open(CreateExistingRequest(directory.Path, release, outputModKey));
        Assert.Equal("WorldEdited", EditorId(reopened, "Worldspace", source.Worldspace, outputModKey));
        Assert.Equal("PlacedEdited", EditorId(reopened, "PlacedObject", source.ExteriorPlaced, outputModKey));
        Assert.Equal(1, IdentityCount(reopened.Output, source.ExteriorPlaced));
        Assert.Equal(source.ExteriorCell, CellContaining(reopened.Output, source.ExteriorPlaced));
        Assert.Equal(source.Worldspace, WorldContaining(reopened.Output, source.ExteriorCell));
        Assert.Equal(1, CountList(RequireRecord(reopened.Output, source.ExteriorCell), "Temporary"));
    }

    /// <summary>Overrides a cell or worldspace without copying children, and keeps an exterior cell under its world.</summary>
    [Theory]
    [InlineData(GameRelease.Starfield)]
    [InlineData(GameRelease.Fallout4)]
    [InlineData(GameRelease.SkyrimSE)]
    public async Task ContainerOverridesOmitUnrelatedChildren(GameRelease release)
    {
        using var directory = new TemporaryDirectory();
        var source = WritePlacementSource(directory.Path, release);

        var cellOutput = ModKey.FromNameAndExtension("Cell.esp");
        using (var workspace = OpenPlacement(directory.Path, release, source.ModKey, cellOutput))
        {
            workspace.Records.Apply(new RecordChangeSet(0, [Override("Cell", source.InteriorCell, source.ModKey, "CellOnly")]));
            await SaveWorkspace(workspace);
        }

        using (var reopened = CreateFactory().Open(CreateExistingRequest(directory.Path, release, cellOutput)))
        {
            var cell = RequireRecord(reopened.Output, source.InteriorCell);
            Assert.Equal("CellOnly", EditorId(reopened, "Cell", source.InteriorCell, cellOutput));
            Assert.Equal(0, CountList(cell, "Persistent"));
            Assert.Equal(0, CountList(cell, "Temporary"));
            Assert.Equal(0, CountList(cell, "NavigationMeshes"));
            Assert.Null(cell.GetType().GetProperty("Landscape")?.GetValue(cell));
        }

        var worldOutput = ModKey.FromNameAndExtension("World.esp");
        using (var workspace = OpenPlacement(directory.Path, release, source.ModKey, worldOutput))
        {
            workspace.Records.Apply(new RecordChangeSet(0, [Override("Worldspace", source.Worldspace, source.ModKey, "WorldOnly")]));
            await SaveWorkspace(workspace);
        }

        using (var reopened = CreateFactory().Open(CreateExistingRequest(directory.Path, release, worldOutput)))
        {
            Assert.Equal("WorldOnly", EditorId(reopened, "Worldspace", source.Worldspace, worldOutput));
            Assert.Empty(NestedCellKeys(RequireRecord(reopened.Output, source.Worldspace)));
            Assert.DoesNotContain(reopened.Output.EnumerateMajorRecords(), record => record.FormKey == source.ExteriorCell);
        }

        var exteriorOutput = ModKey.FromNameAndExtension("Exterior.esp");
        using (var workspace = OpenPlacement(directory.Path, release, source.ModKey, exteriorOutput))
        {
            workspace.Records.Apply(new RecordChangeSet(0, [Override("Cell", source.ExteriorCell, source.ModKey, "ExteriorOnly")]));
            await SaveWorkspace(workspace);
        }

        using (var reopened = CreateFactory().Open(CreateExistingRequest(directory.Path, release, exteriorOutput)))
        {
            Assert.Equal("ExteriorOnly", EditorId(reopened, "Cell", source.ExteriorCell, exteriorOutput));
            Assert.Equal(source.Worldspace, WorldContaining(reopened.Output, source.ExteriorCell));
            Assert.Equal(0, CountList(RequireRecord(reopened.Output, source.ExteriorCell), "Temporary"));
            Assert.False(InteriorContains(reopened.Output, source.ExteriorCell));
        }
    }

    /// <summary>Keeps one dialog child on its source parent and leaves the sibling behind.</summary>
    [Theory]
    [InlineData(GameRelease.Starfield)]
    [InlineData(GameRelease.Fallout4)]
    [InlineData(GameRelease.SkyrimSE)]
    public async Task DialogChildKeepsSourceParentAndOmitsSibling(GameRelease release)
    {
        using var directory = new TemporaryDirectory();
        var source = WritePlacementSource(directory.Path, release);
        var outputModKey = ModKey.FromNameAndExtension("Output.esp");
        using (var workspace = OpenPlacement(directory.Path, release, source.ModKey, outputModKey))
        {
            workspace.Records.Apply(new RecordChangeSet(0, [Override(source.DialogChildFamily, source.DialogChild, source.ModKey, "ChildEdited")]));
            await SaveWorkspace(workspace);
        }

        using var reopened = CreateFactory().Open(CreateExistingRequest(directory.Path, release, outputModKey));
        var parent = RequireRecord(reopened.Output, source.DialogParent);
        var listName = source.DialogChildFamily == "DialogResponses" ? "Responses" : "DialogTopics";
        Assert.Equal("ChildEdited", EditorId(reopened, source.DialogChildFamily, source.DialogChild, outputModKey));
        Assert.Equal(source.DialogParentFamily, RecordName(parent));
        Assert.True(ListContains(parent, listName, source.DialogChild));
        Assert.Equal(1, CountList(parent, listName));
        Assert.DoesNotContain(reopened.Output.EnumerateMajorRecords(), record => record.FormKey == source.DialogSibling);
    }

    /// <summary>Keeps both a parent edit and a child edit from one change set, in either order.</summary>
    [Theory]
    [InlineData(GameRelease.Starfield, true)]
    [InlineData(GameRelease.Starfield, false)]
    [InlineData(GameRelease.Fallout4, true)]
    [InlineData(GameRelease.Fallout4, false)]
    [InlineData(GameRelease.SkyrimSE, true)]
    [InlineData(GameRelease.SkyrimSE, false)]
    public async Task ParentAndChildEditsSurviveTogether(GameRelease release, bool childFirst)
    {
        using var directory = new TemporaryDirectory();
        var source = WritePlacementSource(directory.Path, release);
        var outputModKey = ModKey.FromNameAndExtension("Output.esp");
        var parent = Override("Cell", source.InteriorCell, source.ModKey, "ParentEdited");
        var child = Override("PlacedObject", source.Persistent, source.ModKey, "ChildEdited");
        using (var workspace = OpenPlacement(directory.Path, release, source.ModKey, outputModKey))
        {
            workspace.Records.Apply(new RecordChangeSet(0, childFirst ? [child, parent] : [parent, child]));
            await SaveWorkspace(workspace);
        }

        using var reopened = CreateFactory().Open(CreateExistingRequest(directory.Path, release, outputModKey));
        var cell = RequireRecord(reopened.Output, source.InteriorCell);
        Assert.Equal("ParentEdited", EditorId(reopened, "Cell", source.InteriorCell, outputModKey));
        Assert.Equal("ChildEdited", EditorId(reopened, "PlacedObject", source.Persistent, outputModKey));
        Assert.True(ListContains(cell, "Persistent", source.Persistent));
        Assert.Equal(1, CountList(cell, "Persistent"));
        Assert.Equal(0, CountList(cell, "Temporary"));
    }

    /// <summary>Puts created cells in one interior block and marks each cell interior.</summary>
    [Theory]
    [InlineData(GameRelease.Starfield)]
    [InlineData(GameRelease.Fallout4)]
    [InlineData(GameRelease.SkyrimSE)]
    public void CreatedCellsShareOneInteriorBlock(GameRelease release)
    {
        using var directory = new TemporaryDirectory();
        PrepareRequiredStarfieldMaster(directory.Path, release);
        var outputModKey = ModKey.FromNameAndExtension("Output.esp");
        using var workspace = CreateFactory().Open(CreateNewRequest(directory.Path, release, [], outputModKey));
        workspace.Records.Apply(new RecordChangeSet(0,
        [
            RecordMutation.Create("PlacedObject", [Set("EditorID", RecordValue.FromString("First"))]),
            RecordMutation.Create("PlacedObject", [Set("EditorID", RecordValue.FromString("Second"))]),
        ]));

        var cells = workspace.MutableOutput.GetType().GetProperty("Cells")!.GetValue(workspace.MutableOutput)!;
        var blocks = ItemsOf(cells).ToArray();
        var block = Assert.Single(blocks);
        Assert.Equal(0, block.GetType().GetProperty("BlockNumber")!.GetValue(block));
        Assert.Contains("InteriorCellBlock", block.GetType().GetProperty("GroupType")!.GetValue(block)!.ToString(), StringComparison.Ordinal);
        var sub = Assert.Single(ItemsOf(block.GetType().GetProperty("SubBlocks")!.GetValue(block)));
        Assert.Equal(0, sub.GetType().GetProperty("BlockNumber")!.GetValue(sub));
        Assert.Contains("InteriorCellSubBlock", sub.GetType().GetProperty("GroupType")!.GetValue(sub)!.ToString(), StringComparison.Ordinal);
        var created = ItemsOf(sub.GetType().GetProperty("Cells")!.GetValue(sub)).ToArray();
        Assert.Equal(2, created.Length);
        Assert.All(created, cell => Assert.Contains("Interior", cell.GetType().GetProperty("Flags")!.GetValue(cell)!.ToString(), StringComparison.Ordinal));
    }

    /// <summary>Rejects a save when one FormKey is stored more than once.</summary>
    [Theory]
    [InlineData(GameRelease.Starfield)]
    [InlineData(GameRelease.Fallout4)]
    [InlineData(GameRelease.SkyrimSE)]
    public async Task DuplicateFormKeyFailsSave(GameRelease release)
    {
        using var directory = new TemporaryDirectory();
        PrepareRequiredStarfieldMaster(directory.Path, release);
        var outputModKey = ModKey.FromNameAndExtension("Output.esp");
        using var workspace = CreateFactory().Open(CreateNewRequest(directory.Path, release, [], outputModKey));
        workspace.Records.Apply(new RecordChangeSet(0, [RecordMutation.Create("Keyword", [Set("EditorID", RecordValue.FromString("Marker"))])]));
        var duplicate = new FormKey(outputModKey, 0x900);
        var left = CreateRecord(workspace.MutableOutput, "Cell");
        var right = CreateRecord(workspace.MutableOutput, "Cell");
        AddItem(left, "Temporary", CreateRecord(workspace.MutableOutput, "PlacedObject", duplicate));
        AddItem(right, "Temporary", CreateRecord(workspace.MutableOutput, "PlacedObject", duplicate));
        AddInteriorCell(workspace.MutableOutput, left);
        AddInteriorCell(workspace.MutableOutput, right);

        var result = await workspace.SaveAsync(cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(PluginSaveStatus.Failed, result.Status);
        Assert.Contains("duplicate FormKeys", result.Diagnostic, StringComparison.Ordinal);
        Assert.Contains(duplicate.ToString(), result.Diagnostic, StringComparison.Ordinal);
        Assert.False(File.Exists(Path.Combine(directory.Path, outputModKey.ToString())));
    }

    /// <summary>Leaves a different subtype in place when an override would reuse its FormKey.</summary>
    [Theory]
    [InlineData(GameRelease.Fallout4)]
    [InlineData(GameRelease.SkyrimSE)]
    public void DifferentSubtypeAtTheSameFormKeyFailsAndLeavesTheOriginal(GameRelease release)
    {
        using var directory = new TemporaryDirectory();
        var sourceKey = ModKey.FromNameAndExtension("Source.esm");
        var source = CreateMod(release, sourceKey);
        var global = CreateRecord(source, "GlobalInt");
        SetEditor(global, "SourceGlobal");
        SetData(global, 1);
        SetGroup(source, "Globals", global);
        WritePlugin(source, Path.Combine(directory.Path, sourceKey.ToString()));

        var outputModKey = ModKey.FromNameAndExtension("Output.esp");
        using var workspace = OpenPlacement(directory.Path, release, sourceKey, outputModKey);
        var occupant = CreateRecord(workspace.MutableOutput, "GlobalFloat", ((IMajorRecord)global).FormKey);
        SetEditor(occupant, "fOccupant");
        SetData(occupant, 1.5f);
        SetGroup(workspace.MutableOutput, "Globals", occupant);

        var exception = Assert.Throws<RecordEditingException>(() => workspace.Records.Apply(new RecordChangeSet(0,
        [
            Override("GlobalInt", ((IMajorRecord)global).FormKey, sourceKey, "Overridden"),
        ])));

        Assert.Contains("GlobalFloat", exception.Message, StringComparison.Ordinal);
        Assert.Contains(((IMajorRecord)global).FormKey.ToString(), exception.Message, StringComparison.Ordinal);
        var stored = Assert.Single(workspace.Output.EnumerateMajorRecords(), record => record.FormKey == ((IMajorRecord)global).FormKey);
        Assert.Equal("GlobalFloat", RecordName(stored));
        Assert.Equal("fOccupant", stored.EditorID);
    }

    private static async Task SaveWorkspace(PluginWorkspace workspace)
    {
        var result = await workspace.SaveAsync(cancellationToken: TestContext.Current.CancellationToken);
        Assert.True(result.Status == PluginSaveStatus.Succeeded, result.Diagnostic);
    }

    private static PluginWorkspace OpenPlacement(string directory, GameRelease release, ModKey source, ModKey output)
    {
        return CreateFactory().Open(CreateNewRequest(directory, release, [source], output));
    }

    private static RecordMutation Override(string familyId, FormKey formKey, ModKey containingModKey, string editorId)
    {
        return RecordMutation.Override(familyId, formKey, containingModKey, [Set("EditorID", RecordValue.FromString(editorId))]);
    }

    private static string EditorId(PluginWorkspace workspace, string familyId, FormKey formKey, ModKey containingModKey)
    {
        var read = workspace.Records.Read(new RecordLocator(familyId, formKey, containingModKey));
        return Assert.IsType<RecordValue.StringRecordValue>(read.Values["EditorID"]).Value;
    }

    private static PlacementSource WritePlacementSource(string directory, GameRelease release)
    {
        PrepareRequiredStarfieldMaster(directory, release);
        var modKey = ModKey.FromNameAndExtension("Source.esm");
        var mod = CreateMod(release, modKey);
        var cell = CreateRecord(mod, "Cell");
        SetEditor(cell, "Interior");
        MarkSourceInterior(cell);
        var persistent = CreateRecord(mod, "PlacedObject");
        var temporary = CreateRecord(mod, "PlacedObject");
        var navmesh = CreateRecord(mod, "NavigationMesh");
        SetEditor(persistent, "Persistent");
        SetEditor(temporary, "Temporary");
        SetEditor(navmesh, "Nav");
        AddItem(cell, "Persistent", persistent);
        AddItem(cell, "Temporary", temporary);
        AddItem(cell, "NavigationMeshes", navmesh);
        FormKey? landscape = null;
        if (cell.GetType().GetProperty("Landscape") is not null)
        {
            var land = CreateRecord(mod, "Landscape");
            SetEditor(land, "Land");
            cell.GetType().GetProperty("Landscape")!.SetValue(cell, land);
            landscape = ((IMajorRecord)land).FormKey;
        }

        AddInteriorCell(mod, cell);
        var world = CreateRecord(mod, "Worldspace");
        SetEditor(world, "World");
        var exterior = CreateRecord(mod, "Cell");
        var exteriorPlaced = CreateRecord(mod, "PlacedObject");
        SetEditor(exterior, "Exterior");
        SetEditor(exteriorPlaced, "ExteriorPlaced");
        AddItem(exterior, "Temporary", exteriorPlaced);
        AddExteriorCell(world, exterior);
        SetGroup(mod, "Worldspaces", world);
        var dialog = AddDialog(mod);
        if (release == GameRelease.Starfield)
        {
            mod.MasterReferences.Add(new MasterReference { Master = ModKey.FromNameAndExtension("Starfield.esm") });
            var master = new Mutagen.Bethesda.Starfield.StarfieldMod(ModKey.FromNameAndExtension("Starfield.esm"), Mutagen.Bethesda.Starfield.StarfieldRelease.Starfield);
            WritePlugin(mod, Path.Combine(directory, modKey.ToString()), master);
        }
        else
        {
            WritePlugin(mod, Path.Combine(directory, modKey.ToString()));
        }

        return new PlacementSource(
            modKey,
            ((IMajorRecord)cell).FormKey,
            ((IMajorRecord)persistent).FormKey,
            ((IMajorRecord)temporary).FormKey,
            ((IMajorRecord)navmesh).FormKey,
            landscape,
            ((IMajorRecord)world).FormKey,
            ((IMajorRecord)exterior).FormKey,
            ((IMajorRecord)exteriorPlaced).FormKey,
            dialog.Parent,
            dialog.ParentFamily,
            dialog.Child,
            dialog.ChildFamily,
            dialog.Sibling);
    }

    private static (FormKey Parent, string ParentFamily, FormKey Child, string ChildFamily, FormKey Sibling) AddDialog(IMod mod)
    {
        if (mod.GetType().GetProperty("DialogTopics") is not null)
        {
            var topic = CreateRecord(mod, "DialogTopic");
            var child = CreateRecord(mod, "DialogResponses");
            var sibling = CreateRecord(mod, "DialogResponses");
            SetEditor(topic, "Topic");
            SetEditor(child, "Response");
            SetEditor(sibling, "Sibling");
            AddItem(topic, "Responses", child);
            AddItem(topic, "Responses", sibling);
            SetGroup(mod, "DialogTopics", topic);
            return (((IMajorRecord)topic).FormKey, "DialogTopic", ((IMajorRecord)child).FormKey, "DialogResponses", ((IMajorRecord)sibling).FormKey);
        }

        var quest = CreateRecord(mod, "Quest");
        var topicChild = CreateRecord(mod, "DialogTopic");
        var topicSibling = CreateRecord(mod, "DialogTopic");
        SetEditor(quest, "Quest");
        SetEditor(topicChild, "Topic");
        SetEditor(topicSibling, "Sibling");
        AddItem(quest, "DialogTopics", topicChild);
        AddItem(quest, "DialogTopics", topicSibling);
        SetGroup(mod, "Quests", quest);
        return (((IMajorRecord)quest).FormKey, "Quest", ((IMajorRecord)topicChild).FormKey, "DialogTopic", ((IMajorRecord)topicSibling).FormKey);
    }

    private static IMod CreateMod(GameRelease release, ModKey modKey)
    {
        return release switch
        {
            GameRelease.Starfield => new Mutagen.Bethesda.Starfield.StarfieldMod(modKey, Mutagen.Bethesda.Starfield.StarfieldRelease.Starfield),
            GameRelease.Fallout4 => new Mutagen.Bethesda.Fallout4.Fallout4Mod(modKey, Mutagen.Bethesda.Fallout4.Fallout4Release.Fallout4),
            GameRelease.SkyrimSE => new Mutagen.Bethesda.Skyrim.SkyrimMod(modKey, Mutagen.Bethesda.Skyrim.SkyrimRelease.SkyrimSE),
            _ => throw new ArgumentOutOfRangeException(nameof(release), release, null),
        };
    }

    private static object CreateRecord(IMod mod, string typeName)
    {
        return CreateRecord(mod, typeName, mod.GetNextFormKey());
    }

    private static object CreateRecord(IMod mod, string typeName, FormKey formKey)
    {
        var type = mod.GetType().Assembly.GetType($"{mod.GetType().Namespace}.{typeName}")
            ?? throw new InvalidOperationException($"Mutagen type '{typeName}' is not in '{mod.GetType().Namespace}'.");
        var constructor = type.GetConstructors().First(candidate =>
            candidate.GetParameters() is [{ ParameterType: var key }, { ParameterType: var release }]
            && key == typeof(FormKey)
            && release.IsEnum);
        var releaseType = constructor.GetParameters()[1].ParameterType;
        var releaseName = releaseType.Name switch
        {
            "SkyrimRelease" => "SkyrimSE",
            "Fallout4Release" => "Fallout4",
            "StarfieldRelease" => "Starfield",
            _ => throw new InvalidOperationException($"Unsupported release type '{releaseType.Name}'."),
        };
        return constructor.Invoke([formKey, Enum.Parse(releaseType, releaseName)]);
    }

    private static void SetEditor(object record, string editorId)
    {
        record.GetType().GetProperty("EditorID")!.SetValue(record, editorId);
    }

    private static void SetData(object record, object value)
    {
        var data = record.GetType().GetProperty("Data")!;
        var target = Nullable.GetUnderlyingType(data.PropertyType) ?? data.PropertyType;
        if (target == typeof(short) && value is int number)
        {
            data.SetValue(record, (short)number);
            return;
        }

        data.SetValue(record, value);
    }

    private static void SetGroup(object mod, string propertyName, object record)
    {
        var group = mod.GetType().GetProperty(propertyName)!.GetValue(mod)!;
        var set = group.GetType().GetMethods().First(method => method.Name == "Set" && method.GetParameters().Length == 1);
        set.Invoke(group, [record]);
    }

    private static void AddItem(object parent, string propertyName, object item)
    {
        var list = parent.GetType().GetProperty(propertyName)!.GetValue(parent)
            ?? throw new InvalidOperationException($"'{parent.GetType().Name}.{propertyName}' is null.");
        list.GetType().GetMethod("Add")!.Invoke(list, [item]);
    }

    private static void AddInteriorCell(IMod mod, object cell)
    {
        var assembly = cell.GetType().Assembly;
        var name = cell.GetType().Namespace + ".";
        var groupType = assembly.GetType(name + "GroupTypeEnum")!;
        var sub = Activator.CreateInstance(assembly.GetType(name + "CellSubBlock")!)!;
        sub.GetType().GetProperty("BlockNumber")!.SetValue(sub, 0);
        sub.GetType().GetProperty("GroupType")!.SetValue(sub, Enum.Parse(groupType, "InteriorCellSubBlock"));
        AddItem(sub, "Cells", cell);
        var block = Activator.CreateInstance(assembly.GetType(name + "CellBlock")!)!;
        block.GetType().GetProperty("BlockNumber")!.SetValue(block, 0);
        block.GetType().GetProperty("GroupType")!.SetValue(block, Enum.Parse(groupType, "InteriorCellBlock"));
        AddItem(block, "SubBlocks", sub);
        var cells = mod.GetType().GetProperty("Cells")!.GetValue(mod)!;
        cells.GetType().GetMethod("Add")!.Invoke(cells, [block]);
    }

    private static void AddExteriorCell(object world, object cell)
    {
        var assembly = world.GetType().Assembly;
        var name = world.GetType().Namespace + ".";
        var sub = Activator.CreateInstance(assembly.GetType(name + "WorldspaceSubBlock")!)!;
        AddItem(sub, "Items", cell);
        var block = Activator.CreateInstance(assembly.GetType(name + "WorldspaceBlock")!)!;
        AddItem(block, "Items", sub);
        AddItem(world, "SubCells", block);
    }

    private static void MarkSourceInterior(object cell)
    {
        var flags = cell.GetType().GetProperty("Flags");
        if (flags is null)
        {
            return;
        }

        var enumType = Nullable.GetUnderlyingType(flags.PropertyType) ?? flags.PropertyType;
        var interior = Enum.GetNames(enumType).FirstOrDefault(candidate => candidate.Contains("Interior", StringComparison.Ordinal));
        if (interior is null)
        {
            return;
        }

        flags.SetValue(cell, Enum.Parse(enumType, interior));
    }

    private static IMajorRecordGetter RequireRecord(IModGetter mod, FormKey formKey)
    {
        return Assert.Single(mod.EnumerateMajorRecords(), record => record.FormKey == formKey);
    }

    private static int IdentityCount(IModGetter mod, FormKey formKey)
    {
        return mod.EnumerateMajorRecords().Count(record => record.FormKey == formKey);
    }

    private static FormKey CellContaining(IModGetter mod, FormKey child)
    {
        var owners = mod.EnumerateMajorRecords()
            .Where(record => RecordName(record) == "Cell")
            .Where(cell => ListContains(cell, "Persistent", child)
                || ListContains(cell, "Temporary", child)
                || ListContains(cell, "NavigationMeshes", child)
                || cell.GetType().GetProperty("Landscape")?.GetValue(cell) is IMajorRecordGetter landscape && landscape.FormKey == child)
            .ToArray();
        return Assert.Single(owners).FormKey;
    }

    private static FormKey WorldContaining(IModGetter mod, FormKey cell)
    {
        var owners = mod.EnumerateMajorRecords()
            .Where(record => RecordName(record) == "Worldspace")
            .Where(world => NestedCellKeys(world).Contains(cell))
            .ToArray();
        return Assert.Single(owners).FormKey;
    }

    private static bool InteriorContains(IModGetter mod, FormKey formKey)
    {
        var cells = mod.GetType().GetProperty("Cells")!.GetValue(mod);
        return DeepRecords(cells).Any(record => record.FormKey == formKey);
    }

    private static IEnumerable<FormKey> NestedCellKeys(object world)
    {
        var top = world.GetType().GetProperty("TopCell")?.GetValue(world) as IMajorRecordGetter;
        if (top is not null)
        {
            yield return top.FormKey;
        }

        foreach (var cell in DeepRecords(world.GetType().GetProperty("SubCells")?.GetValue(world)))
        {
            if (RecordName(cell) == "Cell")
            {
                yield return cell.FormKey;
            }
        }
    }

    private static IEnumerable<IMajorRecordGetter> DeepRecords(object? node)
    {
        if (node is null or string)
        {
            yield break;
        }

        if (node is IMajorRecordGetter record && RecordName(record) == "Cell")
        {
            yield return record;
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
            foreach (var child in DeepRecords(value))
            {
                yield return child;
            }
        }

        if (walked || node is not IEnumerable list)
        {
            yield break;
        }

        foreach (var item in list)
        {
            foreach (var child in DeepRecords(item))
            {
                yield return child;
            }
        }
    }

    private static bool ListContains(object record, string propertyName, FormKey formKey)
    {
        return ItemsOf(record.GetType().GetProperty(propertyName)?.GetValue(record)).OfType<IMajorRecordGetter>().Any(item => item.FormKey == formKey);
    }

    private static int CountList(object record, string propertyName)
    {
        return ItemsOf(record.GetType().GetProperty(propertyName)?.GetValue(record)).Count();
    }

    private static IEnumerable<object> ItemsOf(object? value)
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

    private static string RecordName(object record)
    {
        var name = record.GetType().Name;
        const string suffix = "BinaryOverlay";
        return name.EndsWith(suffix, StringComparison.Ordinal) ? name[..^suffix.Length] : name;
    }

    private sealed class PlacementSource
    {
        public PlacementSource(
            ModKey modKey,
            FormKey interiorCell,
            FormKey persistent,
            FormKey temporary,
            FormKey navigationMesh,
            FormKey? landscape,
            FormKey worldspace,
            FormKey exteriorCell,
            FormKey exteriorPlaced,
            FormKey dialogParent,
            string dialogParentFamily,
            FormKey dialogChild,
            string dialogChildFamily,
            FormKey dialogSibling)
        {
            ModKey = modKey;
            InteriorCell = interiorCell;
            Persistent = persistent;
            Temporary = temporary;
            NavigationMesh = navigationMesh;
            Landscape = landscape;
            Worldspace = worldspace;
            ExteriorCell = exteriorCell;
            ExteriorPlaced = exteriorPlaced;
            DialogParent = dialogParent;
            DialogParentFamily = dialogParentFamily;
            DialogChild = dialogChild;
            DialogChildFamily = dialogChildFamily;
            DialogSibling = dialogSibling;
        }

        public ModKey ModKey { get; }

        public FormKey InteriorCell { get; }

        public FormKey Persistent { get; }

        public FormKey Temporary { get; }

        public FormKey NavigationMesh { get; }

        public FormKey? Landscape { get; }

        public FormKey Worldspace { get; }

        public FormKey ExteriorCell { get; }

        public FormKey ExteriorPlaced { get; }

        public FormKey DialogParent { get; }

        public string DialogParentFamily { get; }

        public FormKey DialogChild { get; }

        public string DialogChildFamily { get; }

        public FormKey DialogSibling { get; }
    }
}
