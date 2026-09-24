using CreationsForge.Engine.Persistence;
using CreationsForge.Engine.Records;
using CreationsForge.Engine.Workspaces;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Strings;

namespace CreationsForge.UnitTests.Workspaces;

/// <summary>Verifies native output persistence and in-memory baseline restoration.</summary>
public sealed partial class PluginWorkspaceTests
{
    /// <summary>Creates a physical zero-record output for every supported game and reopens it natively.</summary>
    [Theory]
    [InlineData(GameRelease.Starfield)]
    [InlineData(GameRelease.Fallout4)]
    [InlineData(GameRelease.SkyrimSE)]
    public async Task SaveCreatesPhysicalEmptyOutput(GameRelease release)
    {
        using var directory = new TemporaryDirectory();
        PrepareRequiredStarfieldMaster(directory.Path, release);
        var outputModKey = ModKey.FromNameAndExtension("Empty.esp");
        var outputPath = Path.Combine(directory.Path, outputModKey.ToString());
        using (var workspace = CreateFactory().Open(CreateNewRequest(directory.Path, release, [], outputModKey)))
        {
            var result = await workspace.SaveAsync(cancellationToken: TestContext.Current.CancellationToken);

            Assert.Equal(PluginSaveStatus.Succeeded, result.Status);
            Assert.True(File.Exists(outputPath));
            Assert.False(workspace.State.IsDirty);
            Assert.Empty(workspace.Output.EnumerateMajorRecords());
        }

        using var reopened = CreateFactory().Open(CreateExistingRequest(directory.Path, release, outputModKey));
        Assert.Empty(reopened.Output.EnumerateMajorRecords());
        reopened.Dispose();
        await AssertFreshProcessReopenAsync(
            directory.Path,
            release,
            outputModKey,
            PluginTextStorageMode.Embedded,
            expectedRecordCount: 0);
    }

    /// <summary>Publishes a native output for every supported game and reopens the saved record through a new workspace.</summary>
    [Theory]
    [InlineData(GameRelease.Starfield)]
    [InlineData(GameRelease.Fallout4)]
    [InlineData(GameRelease.SkyrimSE)]
    public async Task SavePublishesAndReopensNativeOutput(GameRelease release)
    {
        using var directory = new TemporaryDirectory();
        PrepareRequiredStarfieldMaster(directory.Path, release);
        var outputModKey = ModKey.FromNameAndExtension("Output.esp");
        var outputPath = Path.Combine(directory.Path, outputModKey.ToString());
        var request = CreateNewRequest(directory.Path, release, [], outputModKey);
        FormKey createdFormKey;

        using (var workspace = CreateFactory().Open(request))
        {
            var created = workspace.Records.Apply(new RecordChangeSet(0,
            [
                RecordMutation.Create("Keyword", [Set("EditorID", RecordValue.FromString("SavedKeyword"))]),
            ])).Records.Single();
            createdFormKey = created.FormKey;
            var progress = new ProgressCollector();

            var result = await workspace.SaveAsync(progress, TestContext.Current.CancellationToken);

            Assert.Equal(PluginSaveStatus.Succeeded, result.Status);
            Assert.Equal(PluginPublicationState.Published, result.PublicationState);
            Assert.Equal(1UL, result.Revision);
            Assert.False(result.RequiresWorkspaceReopen);
            Assert.False(workspace.State.IsDirty);
            Assert.False(workspace.State.RequiresReopen);
            Assert.True(File.Exists(outputPath));
            Assert.Contains(outputPath, result.PublishedPaths, PathComparer);
            Assert.Equal(PluginSavePhase.Preflight, progress.Events.First().Phase);
            Assert.Equal(PluginSavePhase.Complete, progress.Events.Last().Phase);
            Assert.Contains(progress.Events, item => item.Phase == PluginSavePhase.Staging);
            Assert.Contains(progress.Events, item => item.Phase == PluginSavePhase.StagedReopen);
            Assert.Contains(progress.Events, item => item.Phase == PluginSavePhase.Publication);
            Assert.Contains(progress.Events, item => item.Phase == PluginSavePhase.PublishedReopen);
            Assert.All(progress.Events, item =>
            {
                Assert.Equal(outputPath, item.DestinationPath, PathComparer);
                Assert.Equal(1UL, item.Revision);
                Assert.True(item.Elapsed >= TimeSpan.Zero);
                Assert.False(string.IsNullOrWhiteSpace(item.Diagnostic));
            });
            Assert.True(result.Duration >= TimeSpan.Zero);

            var noChanges = await workspace.SaveAsync(cancellationToken: TestContext.Current.CancellationToken);
            Assert.Equal(PluginSaveStatus.NoChanges, noChanges.Status);
            Assert.Equal(PluginPublicationState.Unchanged, noChanges.PublicationState);
        }

        using var reopened = CreateFactory().Open(CreateExistingRequest(directory.Path, release, outputModKey));
        var snapshot = reopened.Records.Read(new RecordLocator("Keyword", createdFormKey, outputModKey));
        Assert.Equal(
            "SavedKeyword",
            Assert.IsType<RecordValue.StringRecordValue>(snapshot.Values["EditorID"]).Value);
        Assert.False(reopened.State.IsDirty);
    }

    /// <summary>Restores the last native saved baseline without touching the published destination.</summary>
    [Theory]
    [InlineData(GameRelease.Starfield)]
    [InlineData(GameRelease.Fallout4)]
    [InlineData(GameRelease.SkyrimSE)]
    public async Task DiscardRestoresLastSavedNativeBaseline(GameRelease release)
    {
        using var directory = new TemporaryDirectory();
        PrepareRequiredStarfieldMaster(directory.Path, release);
        var outputModKey = ModKey.FromNameAndExtension("Output.esp");
        var outputPath = Path.Combine(directory.Path, outputModKey.ToString());
        using var workspace = CreateFactory().Open(CreateNewRequest(directory.Path, release, [], outputModKey));
        var savedRecord = workspace.Records.Apply(new RecordChangeSet(0,
        [
            RecordMutation.Create("Keyword", [Set("EditorID", RecordValue.FromString("SavedKeyword"))]),
        ])).Records.Single();
        Assert.Equal(
            PluginSaveStatus.Succeeded,
            (await workspace.SaveAsync(cancellationToken: TestContext.Current.CancellationToken)).Status);
        var publishedBytes = File.ReadAllBytes(outputPath);
        var discardedRecord = workspace.Records.Apply(new RecordChangeSet(1,
        [
            RecordMutation.Create("Keyword", [Set("EditorID", RecordValue.FromString("DiscardedKeyword"))]),
        ])).Records.Single();

        var result = workspace.Discard();

        Assert.True(result.Changed);
        Assert.False(result.IsDirty);
        Assert.False(workspace.State.IsDirty);
        Assert.Equal(3UL, workspace.State.Revision);
        Assert.Equal(publishedBytes, File.ReadAllBytes(outputPath));
        Assert.Equal(
            "SavedKeyword",
            Assert.IsType<RecordValue.StringRecordValue>(
                workspace.Records.Read(new RecordLocator("Keyword", savedRecord.FormKey, outputModKey)).Values["EditorID"]).Value);
        Assert.ThrowsAny<Exception>(() =>
            workspace.Records.Read(new RecordLocator("Keyword", discardedRecord.FormKey, outputModKey)));
    }

    /// <summary>Discards to the native creation baseline without requiring an earlier physical save.</summary>
    [Fact]
    public void DiscardBeforeFirstSaveRestoresUnsavedEmptyBaseline()
    {
        using var directory = new TemporaryDirectory();
        var outputModKey = ModKey.FromNameAndExtension("Output.esp");
        var outputPath = Path.Combine(directory.Path, outputModKey.ToString());
        using var workspace = CreateFactory().Open(
            CreateNewRequest(directory.Path, GameRelease.Fallout4, [], outputModKey));
        workspace.Records.Apply(new RecordChangeSet(0,
        [
            RecordMutation.Create("Keyword", [Set("EditorID", RecordValue.FromString("DiscardedKeyword"))]),
        ]));

        var result = workspace.Discard();

        Assert.True(result.Changed);
        Assert.True(result.IsDirty);
        Assert.True(workspace.State.IsDirty);
        Assert.Empty(workspace.Output.EnumerateMajorRecords());
        Assert.False(File.Exists(outputPath));
    }

    /// <summary>Saves an existing plugin edit without losing an unrelated native record.</summary>
    [Fact]
    public async Task ExistingOutputSavePreservesUnrelatedNativeRecords()
    {
        using var directory = new TemporaryDirectory();
        var outputModKey = ModKey.FromNameAndExtension("Existing.esp");
        var outputPath = Path.Combine(directory.Path, outputModKey.ToString());
        var plugin = new Mutagen.Bethesda.Fallout4.Fallout4Mod(
            outputModKey,
            Mutagen.Bethesda.Fallout4.Fallout4Release.Fallout4);
        var unrelated = plugin.Weapons.AddNew();
        unrelated.EditorID = "UnrelatedNativeWeapon";
        unrelated.Speed = 1.25f;
        unrelated.Capacity = 7;
        unrelated.Weight = 3.5f;
        var unrelatedFormKey = unrelated.FormKey;
        var editable = plugin.Keywords.AddNew();
        editable.EditorID = "OriginalKeyword";
        var editableFormKey = editable.FormKey;
        WritePlugin(plugin, outputPath);

        using (var workspace = CreateFactory().Open(
                   CreateExistingRequest(directory.Path, GameRelease.Fallout4, outputModKey)))
        {
            workspace.Records.Apply(new RecordChangeSet(0,
            [
                RecordMutation.Override("Keyword", editableFormKey, outputModKey,
                [
                    Set("EditorID", RecordValue.FromString("UpdatedKeyword")),
                ]),
            ]));
            var result = await workspace.SaveAsync(cancellationToken: TestContext.Current.CancellationToken);
            Assert.Equal(PluginSaveStatus.Succeeded, result.Status);
        }

        using var reopened = CreateFactory().Open(
            CreateExistingRequest(directory.Path, GameRelease.Fallout4, outputModKey));
        var native = Assert.IsAssignableFrom<Mutagen.Bethesda.Fallout4.IFallout4ModGetter>(reopened.Output);
        var reopenedUnrelated = Assert.Single(native.Weapons);
        Assert.Equal(unrelatedFormKey, reopenedUnrelated.FormKey);
        Assert.Equal("UnrelatedNativeWeapon", reopenedUnrelated.EditorID);
        Assert.Equal(1.25f, reopenedUnrelated.Speed);
        Assert.Equal((ushort)7, reopenedUnrelated.Capacity);
        Assert.Equal(3.5f, reopenedUnrelated.Weight);
        Assert.Equal(
            "UpdatedKeyword",
            Assert.IsType<RecordValue.StringRecordValue>(
                reopened.Records.Read(new RecordLocator("Keyword", editableFormKey, outputModKey)).Values["EditorID"]).Value);
        reopened.Dispose();
        await AssertFreshProcessReopenAsync(
            directory.Path,
            GameRelease.Fallout4,
            outputModKey,
            PluginTextStorageMode.Embedded,
            expectedRecordCount: 2);
    }

    /// <summary>Saves and reopens a source override with its exact origin identity and master order intact.</summary>
    [Fact]
    public async Task SavePreservesSourceOverrideIdentityAndMasters()
    {
        using var directory = new TemporaryDirectory();
        var fixture = CreateFixture(directory.Path, GameRelease.Fallout4);
        var outputModKey = ModKey.FromNameAndExtension("Output.esp");
        using (var workspace = CreateFactory().Open(
                   CreateNewRequest(directory.Path, GameRelease.Fallout4, [fixture.SelectedModKey], outputModKey)))
        {
            workspace.Records.Apply(new RecordChangeSet(0,
            [
                RecordMutation.Override("FormList", fixture.FormKey, fixture.SelectedModKey,
                [
                    Set("EditorID", RecordValue.FromString("SavedOverride")),
                ]),
            ]));
            var result = await workspace.SaveAsync(cancellationToken: TestContext.Current.CancellationToken);
            Assert.Equal(PluginSaveStatus.Succeeded, result.Status);
        }

        using var reopened = CreateFactory().Open(
            CreateExistingRequest(directory.Path, GameRelease.Fallout4, outputModKey));
        Assert.Equal(
            [fixture.MasterModKey],
            reopened.Sources.Select(source => source.ModKey));
        Assert.Equal(
            [fixture.MasterModKey],
            reopened.Output.MasterReferences.Select(reference => reference.Master));
        Assert.Equal(
            "SavedOverride",
            Assert.IsType<RecordValue.StringRecordValue>(
                reopened.Records.Read(new RecordLocator("FormList", fixture.FormKey, outputModKey)).Values["EditorID"]).Value);
        reopened.Dispose();
        await AssertFreshProcessReopenAsync(
            directory.Path,
            GameRelease.Fallout4,
            outputModKey,
            PluginTextStorageMode.Embedded,
            expectedRecordCount: 1,
            familyId: "FormList",
            expectedFormKey: fixture.FormKey,
            expectedFieldPath: "EditorID",
            expectedStringValue: "SavedOverride");
    }

    /// <summary>Refuses to overwrite a destination that changed after the workspace opened.</summary>
    [Fact]
    public async Task SaveRejectsExternalDestinationReplacement()
    {
        using var directory = new TemporaryDirectory();
        var outputModKey = ModKey.FromNameAndExtension("Output.esp");
        var outputPath = Path.Combine(directory.Path, outputModKey.ToString());
        WriteEmptyPlugin(outputPath, outputModKey, GameRelease.Fallout4);
        using var workspace = CreateFactory().Open(CreateExistingRequest(directory.Path, GameRelease.Fallout4, outputModKey));
        workspace.Records.Apply(new RecordChangeSet(0,
        [
            RecordMutation.Create("Keyword", [Set("EditorID", RecordValue.FromString("PendingKeyword"))]),
        ]));
        var externallyChanged = File.ReadAllBytes(outputPath).Append((byte)0xFF).ToArray();
        var replacementPath = Path.Combine(directory.Path, "replacement.tmp");
        File.WriteAllBytes(replacementPath, externallyChanged);
        File.Move(replacementPath, outputPath, overwrite: true);

        var result = await workspace.SaveAsync(cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(PluginSaveStatus.Failed, result.Status);
        Assert.Equal(PluginPublicationState.Unchanged, result.PublicationState);
        Assert.True(workspace.State.IsDirty);
        Assert.False(workspace.State.RequiresReopen);
        Assert.Equal(externallyChanged, File.ReadAllBytes(outputPath));
        Assert.Contains("changed outside this workspace", result.Diagnostic, StringComparison.Ordinal);
    }

    /// <summary>Reopens and reads a saved native record from a separate process.</summary>
    [Fact]
    public async Task SavedOutputReopensInFreshProcess()
    {
        using var directory = new TemporaryDirectory();
        var outputModKey = ModKey.FromNameAndExtension("Output.esp");
        FormKey createdFormKey;
        using (var workspace = CreateFactory().Open(
                   CreateNewRequest(directory.Path, GameRelease.Fallout4, [], outputModKey)))
        {
            createdFormKey = workspace.Records.Apply(new RecordChangeSet(0,
            [
                RecordMutation.Create("Keyword", [Set("EditorID", RecordValue.FromString("SavedKeyword"))]),
            ])).Records.Single().FormKey;
            var save = await workspace.SaveAsync(cancellationToken: TestContext.Current.CancellationToken);
            Assert.Equal(PluginSaveStatus.Succeeded, save.Status);
        }

        await AssertFreshProcessReopenAsync(
            directory.Path,
            GameRelease.Fallout4,
            outputModKey,
            PluginTextStorageMode.Embedded,
            expectedRecordCount: 1,
            familyId: "Keyword",
            expectedFormKey: createdFormKey,
            expectedFieldPath: "EditorID",
            expectedStringValue: "SavedKeyword");
    }

    /// <summary>Publishes localized string sidecars as part of the same staged file set.</summary>
    [Fact]
    public async Task SavePublishesLocalizedStringSet()
    {
        using var directory = new TemporaryDirectory();
        var outputModKey = ModKey.FromNameAndExtension("Localized.esp");
        var output = new PluginOutputDefinition(
            Path.Combine(directory.Path, outputModKey.ToString()),
            outputModKey,
            MasterStyle.Full,
            PluginTextStorageMode.Localized,
            createNew: true);
        var request = new PluginWorkspaceOpenRequest(GameRelease.Fallout4, directory.Path, [], output);
        var translated = RecordValue.FromTranslatedString(
            Language.English,
            new Dictionary<Language, string> { [Language.English] = "Localized value" });
        FormKey localizedFormKey;

        using (var workspace = CreateFactory().Open(request))
        {
            localizedFormKey = workspace.Records.Apply(new RecordChangeSet(0,
            [
                RecordMutation.Create("Message",
                [
                    Set("EditorID", RecordValue.FromString("LocalizedMessage")),
                    Set("Name", translated),
                    Set("Description", translated),
                ]),
            ])).Records.Single().FormKey;

            var result = await workspace.SaveAsync(cancellationToken: TestContext.Current.CancellationToken);

            Assert.Equal(PluginSaveStatus.Succeeded, result.Status);
            Assert.True(result.PublishedPaths.Count > 1);
            Assert.All(result.PublishedPaths, path => Assert.True(File.Exists(path), path));
            Assert.Contains(result.PublishedPaths, path =>
                path.Contains($"{Path.DirectorySeparatorChar}Strings{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase));
            Assert.Empty(Directory.EnumerateDirectories(directory.Path, ".CreationsForge-save-*"));

            var stringSidecar = result.PublishedPaths.First(path =>
                path.Contains($"{Path.DirectorySeparatorChar}Strings{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase));
            File.SetLastWriteTimeUtc(stringSidecar, File.GetLastWriteTimeUtc(stringSidecar).AddSeconds(5));
            workspace.Records.Apply(new RecordChangeSet(1,
            [
                RecordMutation.Create("Keyword", [Set("EditorID", RecordValue.FromString("PendingKeyword"))]),
            ]));
            var rejected = await workspace.SaveAsync(cancellationToken: TestContext.Current.CancellationToken);
            Assert.Equal(PluginSaveStatus.Failed, rejected.Status);
            Assert.Equal(PluginPublicationState.Unchanged, rejected.PublicationState);
            Assert.Contains("changed outside this workspace", rejected.Diagnostic, StringComparison.Ordinal);
        }

        var existingOutput = new PluginOutputDefinition(
            Path.Combine(directory.Path, outputModKey.ToString()),
            outputModKey,
            MasterStyle.Full,
            PluginTextStorageMode.Localized,
            createNew: false);
        using var reopened = CreateFactory().Open(
            new PluginWorkspaceOpenRequest(GameRelease.Fallout4, directory.Path, [], existingOutput));
        Assert.Single(reopened.Output.EnumerateMajorRecords());
        reopened.Dispose();
        await AssertFreshProcessReopenAsync(
            directory.Path,
            GameRelease.Fallout4,
            outputModKey,
            PluginTextStorageMode.Localized,
            expectedRecordCount: 1,
            familyId: "Message",
            expectedFormKey: localizedFormKey,
            expectedFieldPath: "Name",
            expectedStringValue: "Localized value");
    }

    /// <summary>Removes localized sidecars that are no longer emitted by the staged native output.</summary>
    [Fact]
    public async Task SaveRemovesObsoleteLocalizedSidecars()
    {
        using var directory = new TemporaryDirectory();
        var outputModKey = ModKey.FromNameAndExtension("Localized.esp");
        var output = new PluginOutputDefinition(
            Path.Combine(directory.Path, outputModKey.ToString()),
            outputModKey,
            MasterStyle.Full,
            PluginTextStorageMode.Localized,
            createNew: true);
        using var workspace = CreateFactory().Open(
            new PluginWorkspaceOpenRequest(GameRelease.Fallout4, directory.Path, [], output));
        var bilingual = RecordValue.FromTranslatedString(
            Language.English,
            new Dictionary<Language, string>
            {
                [Language.English] = "English value",
                [Language.French] = "Valeur française",
            });
        var created = workspace.Records.Apply(new RecordChangeSet(0,
        [
            RecordMutation.Create("Message",
            [
                Set("EditorID", RecordValue.FromString("LocalizedMessage")),
                Set("Name", bilingual),
                Set("Description", bilingual),
            ]),
        ])).Records.Single();
        var firstSave = await workspace.SaveAsync(cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(PluginSaveStatus.Succeeded, firstSave.Status);
        var englishOnly = RecordValue.FromTranslatedString(
            Language.English,
            new Dictionary<Language, string> { [Language.English] = "Updated English value" });
        workspace.Records.Apply(new RecordChangeSet(1,
        [
            RecordMutation.Override("Message", created.FormKey, outputModKey,
            [
                Set("Name", englishOnly),
                Set("Description", englishOnly),
            ]),
        ]));

        var secondSave = await workspace.SaveAsync(cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(PluginSaveStatus.Succeeded, secondSave.Status);
        var obsoletePaths = firstSave.PublishedPaths.Except(secondSave.PublishedPaths, PathComparer).ToArray();
        Assert.NotEmpty(obsoletePaths);
        Assert.All(obsoletePaths, path => Assert.False(File.Exists(path), path));
    }

    private static PluginWorkspaceOpenRequest CreateExistingRequest(
        string dataDirectory,
        GameRelease release,
        ModKey outputModKey)
    {
        var output = new PluginOutputDefinition(
            Path.Combine(dataDirectory, outputModKey.ToString()),
            outputModKey,
            MasterStyle.Full,
            PluginTextStorageMode.Embedded,
            createNew: false);
        return new PluginWorkspaceOpenRequest(release, dataDirectory, [], output);
    }

    private static StringComparer PathComparer => OperatingSystem.IsWindows()
        ? StringComparer.OrdinalIgnoreCase
        : StringComparer.Ordinal;

    private sealed class ProgressCollector : IProgress<PluginSaveProgress>
    {
        public List<PluginSaveProgress> Events { get; } = [];

        public void Report(PluginSaveProgress value) => Events.Add(value);
    }
}
