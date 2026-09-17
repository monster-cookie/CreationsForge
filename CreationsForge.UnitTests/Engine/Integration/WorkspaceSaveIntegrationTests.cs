using System.Runtime.InteropServices;
using System.Text.Json;
using CreationsForge.Bootstrap.Composition;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Enums;
using CreationsForge.Fallout4.PluginAdapter.Edits;
using CreationsForge.Starfield.PluginAdapter.Edits;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Strings;
using Shouldly;

namespace CreationsForge.UnitTests.Engine.Integration;

/// <summary>Verifies the production three-game workspace composition through guarded plugin save and reopen boundaries.</summary>
public sealed class WorkspaceSaveIntegrationTests
{
    /// <summary>Verifies published embedded edits commit twice, adopt each new baseline, invalidate old edit identities, and reopen through a fresh workspace.</summary>
    /// <param name="game">The plugin game whose complete production composition is exercised.</param>
    /// <returns>A task that completes after both independently owned workspaces and the production service graph are released.</returns>
    [Theory]
    [InlineData(SupportedGame.Starfield)]
    [InlineData(SupportedGame.Fallout4)]
    [InlineData(SupportedGame.Skyrim)]
    public async Task EmbeddedOutput_PublishedEditsCommitTwiceAndReopenFromDisk(SupportedGame game)
    {
        using var fixture = WorkspaceIntegrationFixture.Create(game);
        var sourceArtifacts = fixture.SnapshotArtifacts();
        var association = CreateAssociation(fixture, $"{game}RoundTrip.esm", LocalizedOutputMode.Embedded);
        await using var services = EngineComposition.Create();
        FormKey createdFormKey;

        var open = await services.WorkspaceFactory.OpenAsync(
            fixture.CreateOpenRequest(),
            TestContext.Current.CancellationToken);
        open.Succeeded.ShouldBeTrue(open.Error?.Message);
        await using (var workspace = open.Value!)
        {
            var selection = await SelectOutputAsync(workspace, association, OutputSelectionMode.CreateNew);
            var edit = await BeginNewEditAsync(workspace);
            createdFormKey = edit.FormKey;
            await ApplyEditAsync(workspace, edit.EditId, new SetEditorIdEdit("FirstCommittedList"));
            await ApplyEditAsync(workspace, edit.EditId, new ReplaceItemsEdit([fixture.BookFormKey]));
            await AssertPreviewAsync(workspace, createdFormKey, "FirstCommittedList", fixture.BookFormKey);

            var firstSave = await workspace.SaveAsync(
                new SaveRequest(Guid.NewGuid(), workspace.Revision, selection.Baseline),
                TestContext.Current.CancellationToken);

            AssertCommittedSave(workspace, firstSave);
            var firstCommittedBaseline = firstSave.CommittedBaseline.ShouldNotBeNull();
            await AssertRecordAsync(workspace, association, createdFormKey, "FirstCommittedList", fixture.BookFormKey);
            var staleEdit = await workspace.ApplyFormListEditAsync(
                new FormListEditRequest(
                    Guid.NewGuid(),
                    workspace.Revision,
                    edit.EditId,
                    new SetEditorIdEdit("RejectedStaleEdit")),
                TestContext.Current.CancellationToken);
            staleEdit.Succeeded.ShouldBeFalse();
            staleEdit.Error!.Code.ShouldBe(EngineErrorCode.EditNotFound);

            var existingEdit = await workspace.BeginEditAsync(
                new BeginEditRequest(
                    Guid.NewGuid(),
                    workspace.Revision,
                    FormListEditRole.ExistingOutput,
                    targetFormKey: createdFormKey),
                TestContext.Current.CancellationToken);
            existingEdit.Succeeded.ShouldBeTrue(existingEdit.Error?.Message);
            await ApplyEditAsync(workspace, existingEdit.Value!.EditId, new SetEditorIdEdit("SecondCommittedList"));
            await AssertPreviewAsync(workspace, createdFormKey, "SecondCommittedList", fixture.BookFormKey);

            var secondSave = await workspace.SaveAsync(
                new SaveRequest(Guid.NewGuid(), workspace.Revision, firstCommittedBaseline),
                TestContext.Current.CancellationToken);

            AssertCommittedSave(workspace, secondSave);
            secondSave.CommittedBaseline!.BaselineId.ShouldNotBe(firstCommittedBaseline.BaselineId);
            await AssertRecordAsync(workspace, association, createdFormKey, "SecondCommittedList", fixture.BookFormKey);
        }

        var reopened = await services.WorkspaceFactory.OpenAsync(
            fixture.CreateOpenRequest(),
            TestContext.Current.CancellationToken);
        reopened.Succeeded.ShouldBeTrue(reopened.Error?.Message);
        await using (var workspace = reopened.Value!)
        {
            await SelectOutputAsync(workspace, association, OutputSelectionMode.OpenExisting);
            await AssertRecordAsync(workspace, association, createdFormKey, "SecondCommittedList", fixture.BookFormKey);
        }

        AssertArtifactsUnchanged(sourceArtifacts, fixture.SnapshotArtifacts());
    }

    /// <summary>Verifies a newly created localized output commits and freshly reopens with all supported translations or Skyrim's explicit empty table.</summary>
    /// <param name="game">The plugin game whose localized output is exercised.</param>
    /// <returns>A task that completes after both production workspaces and the service graph are released.</returns>
    [Theory]
    [InlineData(SupportedGame.Starfield)]
    [InlineData(SupportedGame.Fallout4)]
    [InlineData(SupportedGame.Skyrim)]
    public async Task LocalizedOutput_TranslationsOrEmptyTableCommitAndFreshlyReopen(SupportedGame game)
    {
        using var fixture = WorkspaceIntegrationFixture.Create(game);
        var sourceArtifacts = fixture.SnapshotArtifacts();
        var association = CreateAssociation(fixture, $"{game}Localized.esm", LocalizedOutputMode.SeparateStringFiles);
        await using var services = EngineComposition.Create();
        FormKey formKey;
        SaveResult save;
        var open = await services.WorkspaceFactory.OpenAsync(
            fixture.CreateOpenRequest(),
            TestContext.Current.CancellationToken);
        open.Succeeded.ShouldBeTrue(open.Error?.Message);
        await using (var workspace = open.Value!)
        {
            var selection = await SelectOutputAsync(workspace, association, OutputSelectionMode.CreateNew);
            var edit = await BeginNewEditAsync(workspace);
            formKey = edit.FormKey;
            await ApplyEditAsync(workspace, edit.EditId, new SetEditorIdEdit("LocalizedMatrixList"));
            var nameEdit = CreateLocalizedNameEdit(game);
            if (nameEdit is not null)
            {
                await ApplyEditAsync(workspace, edit.EditId, nameEdit);
            }

            await AssertPreviewAsync(workspace, edit.FormKey, "LocalizedMatrixList");
            save = await workspace.SaveAsync(
                new SaveRequest(Guid.NewGuid(), workspace.Revision, selection.Baseline),
                TestContext.Current.CancellationToken);

            AssertCommittedSave(workspace, save);
            var savedRecord = await ReadRecordAsync(workspace, association, edit.FormKey);
            AssertJsonRecord(savedRecord, "LocalizedMatrixList", null);
            AssertLocalizedName(savedRecord, game);
        }

        var presentSidecars = save.CommittedBaseline!.Artifacts
            .Where(artifact => artifact.Role != PluginArtifactRole.Plugin && artifact.Fingerprint.Exists)
            .ToArray();
        if (game == SupportedGame.Skyrim)
        {
            var presentSidecar = presentSidecars.ShouldHaveSingleItem();
            presentSidecar.Role.ShouldBe(PluginArtifactRole.Strings);
            presentSidecar.Language.ShouldBe(Language.English.ToString());
            presentSidecar.Fingerprint.Length.ShouldBe(8L);
            File.ReadAllBytes(presentSidecar.Path).ShouldBe(new byte[8]);
        }
        else
        {
            presentSidecars.Select(artifact => artifact.Language)
                .ShouldContain(Language.English.ToString());
            presentSidecars.Select(artifact => artifact.Language)
                .ShouldContain(Language.French.ToString());
        }

        var reopened = await services.WorkspaceFactory.OpenAsync(
            fixture.CreateOpenRequest(),
            TestContext.Current.CancellationToken);
        reopened.Succeeded.ShouldBeTrue(reopened.Error?.Message);
        await using (var workspace = reopened.Value!)
        {
            await SelectOutputAsync(workspace, association, OutputSelectionMode.OpenExisting);
            var reopenedRecord = await ReadRecordAsync(workspace, association, formKey);
            AssertJsonRecord(reopenedRecord, "LocalizedMatrixList", null);
            AssertLocalizedName(reopenedRecord, game);
        }

        AssertArtifactsUnchanged(sourceArtifacts, fixture.SnapshotArtifacts());
    }

    /// <summary>Verifies destination drift before the first write is rejected without overwriting the file or discarding published dirty state.</summary>
    /// <returns>A task that completes after the Starfield workspace and production service graph are released.</returns>
    [Fact]
    public async Task SaveAsync_WhenDestinationAppearsAfterSelection_RetainsDirtyStateWithoutOverwrite()
    {
        using var fixture = WorkspaceIntegrationFixture.Create(SupportedGame.Starfield);
        var sourceArtifacts = fixture.SnapshotArtifacts();
        var association = CreateAssociation(fixture, "StarfieldStaleDestination.esm", LocalizedOutputMode.Embedded);
        await using var services = EngineComposition.Create();
        var open = await services.WorkspaceFactory.OpenAsync(
            fixture.CreateOpenRequest(),
            TestContext.Current.CancellationToken);
        open.Succeeded.ShouldBeTrue(open.Error?.Message);
        await using var workspace = open.Value!;
        var selection = await SelectOutputAsync(workspace, association, OutputSelectionMode.CreateNew);
        var edit = await BeginNewEditAsync(workspace);
        await ApplyEditAsync(workspace, edit.EditId, new SetEditorIdEdit("UnsavedDirtyList"));
        await AssertPreviewAsync(workspace, edit.FormKey, "UnsavedDirtyList");
        var externalBytes = new byte[] { 0x43, 0x46, 0x2D, 0x45, 0x58, 0x54 };
        await File.WriteAllBytesAsync(association.PluginPath, externalBytes, TestContext.Current.CancellationToken);

        var save = await workspace.SaveAsync(
            new SaveRequest(Guid.NewGuid(), workspace.Revision, selection.Baseline),
            TestContext.Current.CancellationToken);

        save.Status.ShouldBe(SaveCommitStatus.NotCommitted);
        save.Error!.Code.ShouldBe(EngineErrorCode.ExternalChangeDetected);
        save.CommittedBaseline.ShouldBeNull();
        workspace.OutputSynchronization.Status.ShouldBe(OutputSynchronizationStatus.Ready);
        File.ReadAllBytes(association.PluginPath).ShouldBe(externalBytes);
        await AssertPreviewAsync(workspace, edit.FormKey, "UnsavedDirtyList");

        await ApplyEditAsync(workspace, edit.EditId, new SetEditorIdEdit("DirtyStateStillEditable"));
        await AssertPreviewAsync(workspace, edit.FormKey, "DirtyStateStillEditable");
        File.ReadAllBytes(association.PluginPath).ShouldBe(externalBytes);
        AssertArtifactsUnchanged(sourceArtifacts, fixture.SnapshotArtifacts());
    }

    /// <summary>Verifies a hard-link-deployed existing output opens and guarded publication replaces only the selected directory entry.</summary>
    /// <returns>A task that completes after the replacement output and retained deployment artifact are compared.</returns>
    [Fact]
    public async Task HardLinkedExistingOutput_SaveBreaksLinkWithoutChangingDeploymentArtifactOnWindows()
    {
        if (!OperatingSystem.IsWindows())
        {
            return;
        }

        using var fixture = WorkspaceIntegrationFixture.Create(SupportedGame.Starfield);
        var association = CreateAssociation(fixture, "HardLinkedOutput.esm", LocalizedOutputMode.Embedded);
        await using var services = EngineComposition.Create();
        FormKey formKey;
        var initialOpen = await services.WorkspaceFactory.OpenAsync(
            fixture.CreateOpenRequest(),
            TestContext.Current.CancellationToken);
        initialOpen.Succeeded.ShouldBeTrue(initialOpen.Error?.Message);
        await using (var workspace = initialOpen.Value!)
        {
            var selection = await SelectOutputAsync(workspace, association, OutputSelectionMode.CreateNew);
            var edit = await BeginNewEditAsync(workspace);
            formKey = edit.FormKey;
            await ApplyEditAsync(workspace, edit.EditId, new SetEditorIdEdit("DeployedOriginal"));
            var save = await workspace.SaveAsync(
                new SaveRequest(Guid.NewGuid(), workspace.Revision, selection.Baseline),
                TestContext.Current.CancellationToken);
            AssertCommittedSave(workspace, save);
        }

        var originalBytes = await File.ReadAllBytesAsync(association.PluginPath, TestContext.Current.CancellationToken);
        var deploymentArtifactPath = Path.Combine(Path.GetDirectoryName(association.PluginPath)!, "HardLinkedOutput.deployed");
        if (!CreateHardLink(deploymentArtifactPath, association.PluginPath, IntPtr.Zero))
        {
            throw new InvalidOperationException($"Could not create the deployed-output hard-link fixture: {Marshal.GetLastPInvokeError()}.");
        }

        var reopen = await services.WorkspaceFactory.OpenAsync(
            fixture.CreateOpenRequest(),
            TestContext.Current.CancellationToken);
        reopen.Succeeded.ShouldBeTrue(reopen.Error?.Message);
        await using (var workspace = reopen.Value!)
        {
            var selection = await SelectOutputAsync(workspace, association, OutputSelectionMode.OpenExisting);
            var edit = await workspace.BeginEditAsync(
                new BeginEditRequest(
                    Guid.NewGuid(),
                    workspace.Revision,
                    FormListEditRole.ExistingOutput,
                    targetFormKey: formKey),
                TestContext.Current.CancellationToken);
            edit.Succeeded.ShouldBeTrue(edit.Error?.Message);
            await ApplyEditAsync(workspace, edit.Value!.EditId, new SetEditorIdEdit("PublishedReplacement"));
            var save = await workspace.SaveAsync(
                new SaveRequest(Guid.NewGuid(), workspace.Revision, selection.Baseline),
                TestContext.Current.CancellationToken);
            AssertCommittedSave(workspace, save);
        }

        File.ReadAllBytes(deploymentArtifactPath).ShouldBe(originalBytes);
        File.ReadAllBytes(association.PluginPath).ShouldNotBe(originalBytes);
    }

    /// <summary>Creates a canonical full-master output association under the fixture-owned temporary root.</summary>
    /// <param name="fixture">The generated plugin fixture that owns the output directory.</param>
    /// <param name="fileName">The output plugin file name.</param>
    /// <param name="localizedOutputMode">How the plugin writer must represent localized strings.</param>
    /// <returns>A full-master association whose plugin does not yet exist.</returns>
    private static OutputAssociation CreateAssociation(
        WorkspaceIntegrationFixture fixture,
        string fileName,
        LocalizedOutputMode localizedOutputMode)
    {
        var outputDirectory = fixture.RootDirectory.CreateSubdirectory("IntegrationOutput");
        return new OutputAssociation(
            Path.Combine(outputDirectory.FullName, fileName),
            ModKey.FromNameAndExtension(fileName),
            localizedOutputMode,
            OutputMasterStyle.Full);
    }

    /// <summary>Selects a new or existing output through the public workspace boundary.</summary>
    /// <param name="workspace">The independently owned workspace.</param>
    /// <param name="association">The complete destination identity and formatting choices.</param>
    /// <param name="mode">Whether the destination must be absent or present.</param>
    /// <returns>The successful selected-output receipt.</returns>
    private static async Task<OutputSelectionReceipt> SelectOutputAsync(
        IPluginWorkspace workspace,
        OutputAssociation association,
        OutputSelectionMode mode)
    {
        var result = await workspace.SelectOutputAsync(
            new SelectOutputRequest(Guid.NewGuid(), workspace.Revision, mode, association),
            TestContext.Current.CancellationToken);
        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        return result.Value!;
    }

    /// <summary>Allocates and publishes a new FormList through the public workspace boundary.</summary>
    /// <param name="workspace">The selected workspace.</param>
    /// <returns>The successful edit receipt and stable edit identity.</returns>
    private static async Task<EditReceipt> BeginNewEditAsync(IPluginWorkspace workspace)
    {
        var result = await workspace.BeginEditAsync(
            new BeginEditRequest(Guid.NewGuid(), workspace.Revision, FormListEditRole.New),
            TestContext.Current.CancellationToken);
        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        return result.Value!;
    }

    /// <summary>Applies and publishes one typed FormList edit through the public workspace boundary.</summary>
    /// <param name="workspace">The selected workspace.</param>
    /// <param name="editId">The staged edit session to mutate.</param>
    /// <param name="edit">The prepared-by-adapter typed mutation.</param>
    /// <returns>A task that completes after the published workspace revision advances.</returns>
    private static async Task ApplyEditAsync(IPluginWorkspace workspace, Guid editId, FormListEdit edit)
    {
        var result = await workspace.ApplyFormListEditAsync(
            new FormListEditRequest(Guid.NewGuid(), workspace.Revision, editId, edit),
            TestContext.Current.CancellationToken);
        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        result.ResultRevision.ShouldBe(workspace.Revision);
    }

    /// <summary>Asserts the public preview contains the published FormList values that will be sent to the save coordinator.</summary>
    /// <param name="workspace">The workspace whose published candidate is previewed.</param>
    /// <param name="formKey">The staged FormList identity.</param>
    /// <param name="expectedEditorId">The expected published EditorID.</param>
    /// <param name="expectedItem">The expected sole item, or <see langword="null"/> when the list must be empty.</param>
    /// <returns>A task that completes after detached preview evidence is inspected.</returns>
    private static async Task AssertPreviewAsync(
        IPluginWorkspace workspace,
        FormKey formKey,
        string expectedEditorId,
        FormKey? expectedItem = null)
    {
        var preview = await workspace.PreviewAsync(TestContext.Current.CancellationToken);
        preview.Succeeded.ShouldBeTrue(preview.Error?.Message);
        var comparison = preview.Value!.Comparisons.Single(candidate => candidate.FormKey == formKey);
        comparison.Changes.ShouldNotBeEmpty();
        AssertJsonRecord(comparison.After!.Value, expectedEditorId, expectedItem);
    }

    /// <summary>Reads one staged output FormList and asserts its detached typed JSON values.</summary>
    /// <param name="workspace">The selected workspace.</param>
    /// <param name="association">The selected output association.</param>
    /// <param name="formKey">The exact output FormList identity.</param>
    /// <param name="expectedEditorId">The expected EditorID.</param>
    /// <param name="expectedItem">The expected sole item, or <see langword="null"/> when the list must be empty.</param>
    /// <returns>A task that completes after the public read view is inspected.</returns>
    private static async Task AssertRecordAsync(
        IPluginWorkspace workspace,
        OutputAssociation association,
        FormKey formKey,
        string expectedEditorId,
        FormKey? expectedItem = null)
    {
        var record = await ReadRecordAsync(workspace, association, formKey);
        AssertJsonRecord(record, expectedEditorId, expectedItem);
    }

    /// <summary>Reads one resolved staged-output FormList as detached typed JSON.</summary>
    /// <param name="workspace">The selected workspace.</param>
    /// <param name="association">The selected output association.</param>
    /// <param name="formKey">The exact output FormList identity.</param>
    /// <returns>The detached complete FormList view.</returns>
    private static async Task<JsonElement> ReadRecordAsync(
        IPluginWorkspace workspace,
        OutputAssociation association,
        FormKey formKey)
    {
        var result = await workspace.ReadFormListViewAsync(
            new ReferenceRequest(formKey, RecordScope.StagedOutput, association.ModKey),
            TestContext.Current.CancellationToken);
        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        return result.Value!.Record!.Value;
    }

    /// <summary>Asserts the common typed JSON fields emitted by every supported FormList inspector.</summary>
    /// <param name="record">The detached complete FormList JSON record.</param>
    /// <param name="expectedEditorId">The expected EditorID.</param>
    /// <param name="expectedItem">The expected sole item, or <see langword="null"/> when the list must be empty.</param>
    private static void AssertJsonRecord(JsonElement record, string expectedEditorId, FormKey? expectedItem)
    {
        record.GetProperty("EditorID").GetString().ShouldBe(expectedEditorId);
        var items = record.GetProperty("Items").EnumerateArray().ToArray();
        items.Length.ShouldBe(expectedItem.HasValue ? 1 : 0);
        if (expectedItem is { } item)
        {
            items[0].GetProperty("formKey").GetString().ShouldBe(item.ToString());
        }
    }

    /// <summary>Creates the supported two-language plugin name edit for a localized output.</summary>
    /// <param name="game">The game whose typed edit implementation owns the plugin name.</param>
    /// <returns>A Starfield or Fallout 4 name edit, or <see langword="null"/> because Skyrim FormLists have no Name field.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="game"/> is undefined.</exception>
    private static FormListEdit? CreateLocalizedNameEdit(SupportedGame game)
    {
        var name = new TranslatedString(Language.English, "Localized English");
        name.Set(Language.French, "Nom localise");
        return game switch
        {
            SupportedGame.Starfield => new StarfieldSetNameEdit(name),
            SupportedGame.Fallout4 => new Fallout4SetNameEdit(name),
            SupportedGame.Skyrim => null,
            _ => throw new ArgumentOutOfRangeException(nameof(game), game, "The localized integration test requires a supported plugin game."),
        };
    }

    /// <summary>Asserts the plugin read view retains all supported localized Name values without inventing Skyrim data.</summary>
    /// <param name="record">The complete detached FormList JSON view.</param>
    /// <param name="game">The game whose exact Name contract is asserted.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="game"/> is undefined.</exception>
    private static void AssertLocalizedName(JsonElement record, SupportedGame game)
    {
        if (game == SupportedGame.Skyrim)
        {
            record.TryGetProperty("Name", out _).ShouldBeFalse();
            return;
        }

        if (game is not SupportedGame.Starfield and not SupportedGame.Fallout4)
        {
            throw new ArgumentOutOfRangeException(nameof(game), game, "The localized integration test requires a supported plugin game.");
        }

        var name = record.GetProperty("Name");
        name.GetProperty("targetLanguage").GetString().ShouldBe(Language.English.ToString());
        name.GetProperty("value").GetString().ShouldBe("Localized English");
        name.GetProperty("translations").EnumerateArray()
            .Select(translation => $"{translation.GetProperty("language").GetString()}={translation.GetProperty("value").GetString()}")
            .ShouldBe(["English=Localized English", "French=Nom localise"]);
    }

    /// <summary>Asserts a save fully committed, reopened, and published its new exact output baseline.</summary>
    /// <param name="workspace">The workspace that adopted the committed output.</param>
    /// <param name="save">The guarded save result.</param>
    private static void AssertCommittedSave(IPluginWorkspace workspace, SaveResult save)
    {
        save.Status.ShouldBe(SaveCommitStatus.Committed);
        save.Error.ShouldBeNull();
        save.CommittedBaseline.ShouldNotBeNull();
        save.ResultRevision.ShouldBe(workspace.Revision);
        workspace.OutputSynchronization.Status.ShouldBe(OutputSynchronizationStatus.Ready);
    }

    /// <summary>Asserts generated source plugins and string files retain their exact paths and bytes.</summary>
    /// <param name="expected">The source artifact snapshot taken before workspace activity.</param>
    /// <param name="actual">The source artifact snapshot taken after workspace activity.</param>
    private static void AssertArtifactsUnchanged(
        IReadOnlyDictionary<string, byte[]> expected,
        IReadOnlyDictionary<string, byte[]> actual)
    {
        actual.Keys.ShouldBe(expected.Keys, ignoreOrder: false);
        foreach (var artifact in expected)
        {
            actual[artifact.Key].ShouldBe(artifact.Value);
        }
    }

    /// <summary>Creates a Windows hard link for deployed-output replacement validation.</summary>
    /// <param name="fileName">The new deployment-artifact path.</param>
    /// <param name="existingFileName">The selected output path whose current bytes are shared.</param>
    /// <param name="securityAttributes">Reserved security attributes, always zero.</param>
    /// <returns><see langword="true"/> when Windows creates the link.</returns>
    [DllImport("kernel32.dll", EntryPoint = "CreateHardLinkW", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool CreateHardLink(
        string fileName,
        string existingFileName,
        IntPtr securityAttributes);
}
