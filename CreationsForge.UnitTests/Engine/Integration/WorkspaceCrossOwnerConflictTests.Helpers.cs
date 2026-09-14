using System.Collections.ObjectModel;
using CreationsForge.Bootstrap.Composition;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Enums;
using Mutagen.Bethesda.Plugins;
using Shouldly;

namespace CreationsForge.UnitTests.Engine.Integration;

/// <content>Provides deterministic plugin setup, detached-preview, and physical-artifact helpers.</content>
public sealed partial class WorkspaceCrossOwnerConflictTests
{
    /// <summary>Creates one valid embedded output through a separately owned production service lifetime.</summary>
    /// <param name="fixture">The generated plugin source fixture.</param>
    /// <param name="output">The absent output association to create.</param>
    /// <param name="editorId">The deterministic initial EditorID.</param>
    /// <returns>The created FormList identity and committed complete output baseline.</returns>
    private static async Task<(FormKey FormKey, OutputArtifactSetBaseline Baseline)> CreateExistingOutputAsync(
        WorkspaceIntegrationFixture fixture,
        OutputAssociation output,
        string editorId)
    {
        await using var services = EngineComposition.Create();
        var workspaceId = Guid.NewGuid();
        var opened = await services.WorkspaceFactory.OpenAsync(
            fixture.CreateOpenRequest(workspaceId),
            TestContext.Current.CancellationToken);
        opened.Succeeded.ShouldBeTrue(DescribeError(opened.Error));
        await using var workspace = opened.Value.ShouldNotBeNull();
        workspace.WorkspaceId.ShouldBe(workspaceId);
        var selected = await SelectOutputAsync(workspace, output, OutputSelectionMode.CreateNew);
        var beginOperationId = Guid.NewGuid();
        var edit = await workspace.BeginEditAsync(
            new BeginEditRequest(beginOperationId, workspace.Revision, FormListEditRole.New),
            TestContext.Current.CancellationToken);
        edit.Succeeded.ShouldBeTrue(DescribeError(edit.Error));
        var editReceipt = edit.Value.ShouldNotBeNull();
        edit.OperationId.ShouldBe(beginOperationId);
        var applyOperationId = Guid.NewGuid();
        var applied = await workspace.ApplyFormListEditAsync(
            new FormListEditRequest(
                applyOperationId,
                workspace.Revision,
                editReceipt.EditId,
                new SetEditorIdEdit(editorId)),
            TestContext.Current.CancellationToken);
        applied.Succeeded.ShouldBeTrue(DescribeError(applied.Error));
        applied.Value.ShouldNotBeNull().OperationId.ShouldBe(applyOperationId);
        var saveOperationId = Guid.NewGuid();
        var saved = await workspace.SaveAsync(
            new SaveRequest(saveOperationId, workspace.Revision, selected.Baseline),
            TestContext.Current.CancellationToken);
        saved.Status.ShouldBe(SaveCommitStatus.Committed, DescribeError(saved.Error));
        saved.OperationId.ShouldBe(saveOperationId);
        return (editReceipt.FormKey, saved.CommittedBaseline.ShouldNotBeNull());
    }

    /// <summary>Opens, selects, and stages one existing-output EditorID under an independent production lifetime.</summary>
    /// <param name="services">The independently owned production service lifetime.</param>
    /// <param name="fixture">The generated plugin source fixture.</param>
    /// <param name="output">The existing shared output association.</param>
    /// <param name="formKey">The existing output FormList identity.</param>
    /// <param name="workspaceId">The caller-selected owner identity.</param>
    /// <param name="editorId">The owner-specific staged EditorID.</param>
    /// <returns>The live independently disposable owner session.</returns>
    private static async Task<OutputOwnerSession> OpenStagedOwnerAsync(
        EngineServices services,
        WorkspaceIntegrationFixture fixture,
        OutputAssociation output,
        FormKey formKey,
        Guid workspaceId,
        string editorId)
    {
        var opened = await services.WorkspaceFactory.OpenAsync(
            fixture.CreateOpenRequest(workspaceId),
            TestContext.Current.CancellationToken);
        opened.Succeeded.ShouldBeTrue(DescribeError(opened.Error));
        var workspace = opened.Value.ShouldNotBeNull();
        try
        {
            workspace.WorkspaceId.ShouldBe(workspaceId);
            var selectOperationId = Guid.NewGuid();
            var selected = await workspace.SelectOutputAsync(
                new SelectOutputRequest(
                    selectOperationId,
                    workspace.Revision,
                    OutputSelectionMode.OpenExisting,
                    output),
                TestContext.Current.CancellationToken);
            selected.Succeeded.ShouldBeTrue(DescribeError(selected.Error));
            var selection = selected.Value.ShouldNotBeNull();
            selected.OperationId.ShouldBe(selectOperationId);
            var beginOperationId = Guid.NewGuid();
            var begun = await workspace.BeginEditAsync(
                new BeginEditRequest(
                    beginOperationId,
                    workspace.Revision,
                    FormListEditRole.ExistingOutput,
                    targetFormKey: formKey),
                TestContext.Current.CancellationToken);
            begun.Succeeded.ShouldBeTrue(DescribeError(begun.Error));
            var edit = begun.Value.ShouldNotBeNull();
            begun.OperationId.ShouldBe(beginOperationId);
            var applyOperationId = Guid.NewGuid();
            var applied = await workspace.ApplyFormListEditAsync(
                new FormListEditRequest(
                    applyOperationId,
                    workspace.Revision,
                    edit.EditId,
                    new SetEditorIdEdit(editorId)),
                TestContext.Current.CancellationToken);
            applied.Succeeded.ShouldBeTrue(DescribeError(applied.Error));
            applied.Value.ShouldNotBeNull().OperationId.ShouldBe(applyOperationId);
            var state = await ReadStateAsync(workspace);
            var preview = await ReadPreviewAsync(workspace);
            AssertPreviewEditorId(preview, formKey, editorId);
            return new OutputOwnerSession(
                workspace,
                selection,
                edit.EditId,
                editorId,
                selectOperationId,
                beginOperationId,
                applyOperationId,
                Guid.NewGuid(),
                state,
                preview);
        }
        catch
        {
            await workspace.DisposeAsync();
            throw;
        }
    }

    /// <summary>Begins and applies one new staged EditorID in an already selected workspace.</summary>
    /// <param name="workspace">The selected live workspace.</param>
    /// <param name="formKey">The existing output FormList identity.</param>
    /// <param name="editorId">The new staged EditorID.</param>
    /// <returns>The edit identity and final revision.</returns>
    private static async Task<(Guid EditId, WorkspaceRevision Revision)> StageExistingEditAsync(
        IFormListWorkspace workspace,
        FormKey formKey,
        string editorId)
    {
        var begun = await workspace.BeginEditAsync(
            new BeginEditRequest(
                Guid.NewGuid(),
                workspace.Revision,
                FormListEditRole.ExistingOutput,
                targetFormKey: formKey),
            TestContext.Current.CancellationToken);
        begun.Succeeded.ShouldBeTrue(DescribeError(begun.Error));
        var edit = begun.Value.ShouldNotBeNull();
        var applied = await workspace.ApplyFormListEditAsync(
            new FormListEditRequest(
                Guid.NewGuid(),
                workspace.Revision,
                edit.EditId,
                new SetEditorIdEdit(editorId)),
            TestContext.Current.CancellationToken);
        applied.Succeeded.ShouldBeTrue(DescribeError(applied.Error));
        return (edit.EditId, workspace.Revision);
    }

    /// <summary>Selects one output through the public workspace boundary.</summary>
    /// <param name="workspace">The workspace receiving output state.</param>
    /// <param name="output">The complete output association.</param>
    /// <param name="mode">Whether the output must be absent or present.</param>
    /// <returns>The successful selection receipt.</returns>
    private static async Task<OutputSelectionReceipt> SelectOutputAsync(
        IFormListWorkspace workspace,
        OutputAssociation output,
        OutputSelectionMode mode)
    {
        var operationId = Guid.NewGuid();
        var selected = await workspace.SelectOutputAsync(
            new SelectOutputRequest(operationId, workspace.Revision, mode, output),
            TestContext.Current.CancellationToken);
        selected.Succeeded.ShouldBeTrue(DescribeError(selected.Error));
        selected.OperationId.ShouldBe(operationId);
        return selected.Value.ShouldNotBeNull();
    }

    /// <summary>Reads one complete atomic workspace state.</summary>
    /// <param name="workspace">The live workspace.</param>
    /// <returns>The successful immutable state snapshot.</returns>
    private static async Task<WorkspaceState> ReadStateAsync(IFormListWorkspace workspace)
    {
        var result = await workspace.ReadStateAsync(TestContext.Current.CancellationToken);
        result.Succeeded.ShouldBeTrue(DescribeError(result.Error));
        return result.Value.ShouldNotBeNull();
    }

    /// <summary>Reads one complete detached staged preview.</summary>
    /// <param name="workspace">The live workspace.</param>
    /// <returns>The successful immutable preview snapshot.</returns>
    private static async Task<WorkspacePreview> ReadPreviewAsync(IFormListWorkspace workspace)
    {
        var result = await workspace.PreviewAsync(TestContext.Current.CancellationToken);
        result.Succeeded.ShouldBeTrue(DescribeError(result.Error));
        return result.Value.ShouldNotBeNull();
    }

    /// <summary>Reads the exact staged-output EditorID for one FormList.</summary>
    /// <param name="workspace">The selected live workspace.</param>
    /// <param name="output">The selected output association.</param>
    /// <param name="formKey">The output FormList identity.</param>
    /// <returns>The detached EditorID value.</returns>
    private static async Task<string?> ReadEditorIdAsync(
        IFormListWorkspace workspace,
        OutputAssociation output,
        FormKey formKey)
    {
        var result = await workspace.ReadFormListViewAsync(
            new ReferenceRequest(formKey, RecordScope.StagedOutput, output.ModKey),
            TestContext.Current.CancellationToken);
        result.Succeeded.ShouldBeTrue(DescribeError(result.Error));
        return result.Value.ShouldNotBeNull().Record.ShouldNotBeNull()
            .GetProperty("EditorID").GetString();
    }

    /// <summary>Asserts one detached preview contains the expected staged EditorID.</summary>
    /// <param name="preview">The detached preview.</param>
    /// <param name="formKey">The staged FormList identity.</param>
    /// <param name="expectedEditorId">The expected EditorID.</param>
    private static void AssertPreviewEditorId(
        WorkspacePreview preview,
        FormKey formKey,
        string expectedEditorId)
    {
        var comparison = preview.Comparisons.Single(candidate => candidate.FormKey == formKey);
        comparison.After.ShouldNotBeNull().GetProperty("EditorID").GetString()
            .ShouldBe(expectedEditorId);
    }

    /// <summary>Captures all expected output artifact paths with exact bytes or explicit absence.</summary>
    /// <param name="baseline">The complete output baseline defining every expected path.</param>
    /// <returns>A stable path-ordered physical snapshot.</returns>
    private static IReadOnlyDictionary<string, byte[]?> SnapshotOutputArtifacts(
        OutputArtifactSetBaseline baseline)
    {
        var comparer = OperatingSystem.IsWindows() ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;
        var snapshot = baseline.Artifacts
            .OrderBy(artifact => artifact.Path, comparer)
            .ToDictionary(
                artifact => artifact.Path,
                artifact => File.Exists(artifact.Path) ? File.ReadAllBytes(artifact.Path) : null,
                comparer);
        return new ReadOnlyDictionary<string, byte[]?>(snapshot);
    }

    /// <summary>Asserts two physical byte snapshots have identical paths, absence, and content.</summary>
    /// <param name="expected">The expected physical snapshot.</param>
    /// <param name="actual">The actual physical snapshot.</param>
    private static void AssertPhysicalArtifactsUnchanged(
        IReadOnlyDictionary<string, byte[]?> expected,
        IReadOnlyDictionary<string, byte[]?> actual)
    {
        actual.Keys.ShouldBe(expected.Keys, ignoreOrder: false);
        foreach (var artifact in expected)
        {
            if (artifact.Value is null)
            {
                actual[artifact.Key].ShouldBeNull();
            }
            else
            {
                actual[artifact.Key].ShouldNotBeNull().ShouldBe(artifact.Value);
            }
        }
    }

    /// <summary>Asserts two independently captured baselines describe the same complete artifact identities and fingerprints.</summary>
    /// <param name="expected">The first independently captured complete baseline.</param>
    /// <param name="actual">The second independently captured complete baseline.</param>
    private static void AssertBaselineArtifactsEqual(
        OutputArtifactSetBaseline expected,
        OutputArtifactSetBaseline actual)
    {
        actual.BaselineId.ShouldBe(expected.BaselineId);
        actual.Artifacts.Count.ShouldBe(expected.Artifacts.Count);
        for (var index = 0; index < expected.Artifacts.Count; index++)
        {
            var expectedArtifact = expected.Artifacts[index];
            var actualArtifact = actual.Artifacts[index];
            actualArtifact.Path.ShouldBe(expectedArtifact.Path);
            actualArtifact.Role.ShouldBe(expectedArtifact.Role);
            actualArtifact.Language.ShouldBe(expectedArtifact.Language);
            actualArtifact.Fingerprint.ShouldBe(expectedArtifact.Fingerprint);
            actualArtifact.FileIdentity.ShouldBe(expectedArtifact.FileIdentity);
        }
    }

    /// <summary>Asserts generated source plugin and string artifacts retain exact paths and bytes.</summary>
    /// <param name="expected">The source snapshot captured before workspace activity.</param>
    /// <param name="actual">The source snapshot captured after workspace activity.</param>
    private static void AssertSourceArtifactsUnchanged(
        IReadOnlyDictionary<string, byte[]> expected,
        IReadOnlyDictionary<string, byte[]> actual)
    {
        actual.Keys.ShouldBe(expected.Keys, ignoreOrder: false);
        foreach (var artifact in expected)
        {
            actual[artifact.Key].ShouldBe(artifact.Value);
        }
    }

    /// <summary>Creates the common full-master embedded output association used by ordinary conflict cases.</summary>
    /// <param name="fixture">The generated plugin source fixture.</param>
    /// <returns>The absent shared output association.</returns>
    private static OutputAssociation CreateEmbeddedOutputAssociation(WorkspaceIntegrationFixture fixture)
    {
        var outputDirectory = fixture.RootDirectory.CreateSubdirectory("CrossOwnerOutput");
        const string fileName = "PresentationExistingOutput.esm";
        return new OutputAssociation(
            Path.Combine(outputDirectory.FullName, fileName),
            ModKey.FromNameAndExtension(fileName),
            LocalizedOutputMode.Embedded,
            OutputMasterStyle.Full);
    }

    /// <summary>Formats an optional engine failure for assertion diagnostics.</summary>
    /// <param name="error">The engine failure, or <see langword="null"/>.</param>
    /// <returns>The complete stable failure detail.</returns>
    private static string DescribeError(EngineError? error)
    {
        return error is null ? "Engine error: <none>." : $"Engine error: {error.Code}: {error.Message}";
    }

    /// <summary>Owns one live staged output workspace and all exact operation identities used to create it.</summary>
    private sealed class OutputOwnerSession : IAsyncDisposable
    {
        /// <summary>The live workspace, cleared when disposal begins.</summary>
        private IFormListWorkspace? _workspace;

        /// <summary>Initializes a staged owner session.</summary>
        /// <param name="workspace">The live independently owned workspace.</param>
        /// <param name="selection">The exact existing-output selection receipt.</param>
        /// <param name="editId">The stable staged edit identity.</param>
        /// <param name="editorId">The owner-specific staged EditorID.</param>
        /// <param name="selectOperationId">The output-selection operation identity.</param>
        /// <param name="beginOperationId">The edit-begin operation identity.</param>
        /// <param name="applyOperationId">The edit-apply operation identity.</param>
        /// <param name="saveOperationId">The reserved first save operation identity.</param>
        /// <param name="state">The complete state captured after staging.</param>
        /// <param name="preview">The detached preview captured after staging.</param>
        internal OutputOwnerSession(
            IFormListWorkspace workspace,
            OutputSelectionReceipt selection,
            Guid editId,
            string editorId,
            Guid selectOperationId,
            Guid beginOperationId,
            Guid applyOperationId,
            Guid saveOperationId,
            WorkspaceState state,
            WorkspacePreview preview)
        {
            _workspace = workspace;
            WorkspaceId = workspace.WorkspaceId;
            Selection = selection;
            EditId = editId;
            EditorId = editorId;
            SelectOperationId = selectOperationId;
            BeginOperationId = beginOperationId;
            ApplyOperationId = applyOperationId;
            SaveOperationId = saveOperationId;
            State = state;
            Preview = preview;
        }

        /// <summary>Gets the live workspace.</summary>
        internal IFormListWorkspace Workspace => _workspace ?? throw new ObjectDisposedException(nameof(OutputOwnerSession));

        /// <summary>Gets the stable workspace identity even after the live owner is disposed.</summary>
        internal Guid WorkspaceId { get; }

        /// <summary>Gets the exact existing-output selection receipt.</summary>
        internal OutputSelectionReceipt Selection { get; }

        /// <summary>Gets the stable staged edit identity.</summary>
        internal Guid EditId { get; }

        /// <summary>Gets the owner-specific staged EditorID.</summary>
        internal string EditorId { get; }

        /// <summary>Gets the output-selection operation identity.</summary>
        internal Guid SelectOperationId { get; }

        /// <summary>Gets the edit-begin operation identity.</summary>
        internal Guid BeginOperationId { get; }

        /// <summary>Gets the edit-apply operation identity.</summary>
        internal Guid ApplyOperationId { get; }

        /// <summary>Gets the reserved first save operation identity.</summary>
        internal Guid SaveOperationId { get; }

        /// <summary>Gets the complete state captured after staging.</summary>
        internal WorkspaceState State { get; }

        /// <summary>Gets the detached preview captured after staging.</summary>
        internal WorkspacePreview Preview { get; }

        /// <summary>Releases the independently owned workspace exactly once.</summary>
        /// <returns>The asynchronous workspace disposal.</returns>
        public ValueTask DisposeAsync()
        {
            return Interlocked.Exchange(ref _workspace, null)?.DisposeAsync() ?? ValueTask.CompletedTask;
        }
    }
}
