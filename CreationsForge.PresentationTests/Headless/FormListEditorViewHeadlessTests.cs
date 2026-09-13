using System.ComponentModel;
using System.Text.Json;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Enums;
using CreationsForge.RecordEditing;
using CreationsForge.RecordEditing.Drafts;
using CreationsForge.PresentationTests.Support;
using CreationsForge.Services;
using CreationsForge.Services.Interfaces;
using CreationsForge.Skyrim.PluginAdapter.Wire;
using CreationsForge.ViewModels;
using CreationsForge.Views;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Shouldly;

namespace CreationsForge.PresentationTests.Headless;

/// <summary>Verifies record editor controls resynchronize after inactive tab detachment.</summary>
[Collection(AvaloniaControlTestCollection.Name)]
public sealed class FormListEditorViewHeadlessTests
{
    /// <summary>Verifies editor changes published while detached appear when the retained Edit view reattaches.</summary>
    /// <returns>A task that completes after deterministic Begin and reattachment.</returns>
    [AvaloniaFact]
    public async Task FormListEditorView_WhenDetachedStateChanges_ReattachesToCurrentSnapshot()
    {
        var workspaceId = Guid.NewGuid();
        var revision = new WorkspaceRevision(Guid.NewGuid(), 3);
        var sourceModKey = ModKey.FromNameAndExtension("DetachedSource.esm");
        var outputModKey = ModKey.FromNameAndExtension("DetachedOutput.esp");
        var sourcePath = AbsolutePath(sourceModKey.FileName);
        var outputPath = AbsolutePath(outputModKey.FileName);
        var output = new OutputAssociation(
            outputPath,
            outputModKey,
            LocalizedOutputMode.Embedded,
            OutputMasterStyle.Full);
        var workspace = new ReattachEditorWorkspace(
            workspaceId,
            revision,
            output,
            new FormKey(sourceModKey, 0x0123));
        var coordinator = new RecordingWorkspaceCoordinator();
        coordinator.Publish(
            new WorkspaceDescriptor(
                workspaceId,
                SupportedGame.Skyrim,
                GameRelease.SkyrimSE,
                sourcePath,
                [sourcePath],
                output,
                revision),
            workspace);
        var host = new ReattachEditorHost();
        var validator = new FormListDraftValidator();
        using var editor = new FormListEditorViewModel(
            coordinator,
            host,
            new WorkspacePresentationOperationArbiter(),
            new FormListWireCatalogResolver(
                [new SkyrimFormListEditWireCodec()],
                [new SkyrimFormListEditWireSchemaCatalog()]),
            new FormListDraftFactory(),
            validator,
            new FormListDraftSerializer(validator),
            new RecordingReferencePickerService(),
            new InlineUiDispatcher());
        var view = new FormListEditorView(editor);
        var window = new Window
        {
            Width = 1100,
            Height = 850,
            Content = view
        };

        try
        {
            window.Show();
            await editor.BeginNewAsync();
            Dispatcher.UIThread.RunJobs();
            editor.IsSessionActive.ShouldBeTrue();
            var setEditorId = editor.AvailableCommands.Single(command =>
                string.Equals(command.CommandName, "form-list.set-editor-id", StringComparison.Ordinal));
            editor.CreateDraft(setEditorId, FormListDraftSeedSelection.CurrentValue()).Succeeded.ShouldBeTrue();
            Dispatcher.UIThread.RunJobs();
            ControlFinder.FindByAutomationId<ComboBox>(view, "FormListCommandGroupSelector")!
                .SelectedItem.ShouldBe("Identity");

            window.Content = new Border();
            Dispatcher.UIThread.RunJobs();
            var clearEditorId = editor.AvailableCommands.Single(command =>
                string.Equals(command.CommandName, "form-list.clear-editor-id", StringComparison.Ordinal));
            editor.CreateDraft(clearEditorId, FormListDraftSeedSelection.CurrentValue()).Succeeded.ShouldBeTrue();

            window.Content = view;
            Dispatcher.UIThread.RunJobs();
            ControlFinder.FindByAutomationId<ComboBox>(view, "FormListCommandGroupSelector")!
                .SelectedItem.ShouldBe("Identity");
            ControlFinder.FindByAutomationId<ComboBox>(view, "FormListCommandSelector")!
                .SelectedItem.ShouldBeSameAs(clearEditorId);

            window.Content = new Border();
            Dispatcher.UIThread.RunJobs();
            var setFormVersion = editor.AvailableCommands.Single(command =>
                string.Equals(command.CommandName, "form-list.set-form-version", StringComparison.Ordinal));
            var draftResult = editor.CreateDraft(setFormVersion, FormListDraftSeedSelection.CurrentValue());
            draftResult.Succeeded.ShouldBeTrue();
            var root = draftResult.Value!.Root.ShouldBeOfType<RecordWireObjectDraftNode>();
            var formVersion = root.FindProperty("formVersion").ShouldBeOfType<RecordWireIntegerDraftNode>();
            formVersion.Text = "-1";
            editor.ValidationIssues.ShouldNotBeEmpty();

            window.Content = view;
            Dispatcher.UIThread.RunJobs();

            var groupSelector = ControlFinder.FindByAutomationId<ComboBox>(
                view,
                "FormListCommandGroupSelector").ShouldNotBeNull();
            groupSelector.SelectedItem.ShouldBe("Record");
            var commandSelector = ControlFinder.FindByAutomationId<ComboBox>(
                view,
                "FormListCommandSelector").ShouldNotBeNull();
            commandSelector.SelectedItem.ShouldBeSameAs(setFormVersion);
            ControlFinder.FindByAutomationId<TextBox>(view, "RecordWireInteger:$.formVersion")!
                .Text.ShouldBe("-1");
            ControlFinder.FindByAutomationId<ItemsControl>(
                view,
                "FormListEditorValidationIssues").ShouldNotBeNull().ItemCount.ShouldBeGreaterThan(0);
            ControlFinder.FindByAutomationId<Button>(view, "FormListEditorApplyButton")!
                .IsEnabled.ShouldBeFalse();
            ControlFinder.FindByAutomationId<Button>(view, "FormListDiscardFormChangesButton")!
                .IsEnabled.ShouldBeTrue();

            if (HeadlessTestApp.UsesRenderedArtifactRenderer)
            {
                using var bitmap = window.CaptureRenderedFrame().ShouldNotBeNull();
                var screenshotPath = GetValidationScreenshotPath();
                Directory.CreateDirectory(Path.GetDirectoryName(screenshotPath)!);
                bitmap.Save(screenshotPath, PngBitmapEncoderOptions.Default);
                new FileInfo(screenshotPath).Length.ShouldBeGreaterThan(0L);
            }
        }
        finally
        {
            window.Close();
        }
    }

    /// <summary>Creates a fully qualified task-owned plugin path.</summary>
    /// <param name="fileName">The plugin file name.</param>
    /// <returns>The absolute test path.</returns>
    private static string AbsolutePath(string fileName)
    {
        return Path.GetFullPath(Path.Combine(Path.GetTempPath(), "CreationsForge-Editor-Reattach", fileName));
    }

    /// <summary>Gets the repository-relative path for the real editor validation review image.</summary>
    /// <returns>The fully qualified PNG destination under the task-owned work directory.</returns>
    private static string GetValidationScreenshotPath()
    {
        return Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "..",
            "..",
            "..",
            "..",
            ".work",
            "PluginAuthoring",
            "screenshots",
            "plugin-form-list-editor-validation.png"));
    }

    /// <summary>Provides the editor's selection and deterministic refresh boundary.</summary>
    private sealed class ReattachEditorHost : IFormListEditorHost
    {
        /// <inheritdoc />
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <inheritdoc />
        public FormListEditorSelection? Selection => null;

        /// <inheritdoc />
        public Task RefreshAsync(FormKey? reselect = null, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.CompletedTask;
        }

        /// <summary>Raises the declared event for completeness in future host-state scenarios.</summary>
        public void PublishSelectionChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Selection)));
        }
    }

    /// <summary>Implements the exact successful Begin reads needed by the reattachment regression.</summary>
    private sealed class ReattachEditorWorkspace : IFormListWorkspace
    {
        /// <summary>The selected output identity.</summary>
        private readonly OutputAssociation Output;

        /// <summary>The staged FormList identity.</summary>
        private readonly FormKey FormKey;

        /// <summary>The complete selected-output baseline.</summary>
        private readonly OutputArtifactSetBaseline OutputBaseline;

        /// <summary>Initializes the deterministic editor workspace.</summary>
        /// <param name="workspaceId">The workspace identity.</param>
        /// <param name="revision">The initial revision.</param>
        /// <param name="output">The selected output association.</param>
        /// <param name="formKey">The staged FormList identity.</param>
        public ReattachEditorWorkspace(
            Guid workspaceId,
            WorkspaceRevision revision,
            OutputAssociation output,
            FormKey formKey)
        {
            WorkspaceId = workspaceId;
            Revision = revision;
            Output = output;
            FormKey = formKey;
            OutputBaseline = new OutputArtifactSetBaseline(
                revision.BaselineId,
                [new PluginArtifactAssociation(
                    output.PluginPath,
                    PluginArtifactRole.Plugin,
                    language: null,
                    new PluginArtifactFingerprint(false, 0, null))]);
        }

        /// <inheritdoc />
        public Guid WorkspaceId { get; }

        /// <inheritdoc />
        public WorkspaceRevision Revision { get; private set; }

        /// <inheritdoc />
        public OutputSynchronizationState OutputSynchronization { get; } = new(OutputSynchronizationStatus.Ready, null);

        /// <inheritdoc />
        public ValueTask<EngineResult<WorkspaceState>> ReadStateAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return ValueTask.FromResult(EngineResult<WorkspaceState>.Success(
                new WorkspaceState(
                    SupportedGame.Skyrim,
                    GameRelease.SkyrimSE,
                    Output,
                    OutputBaseline,
                    OutputSynchronization,
                    Revision),
                WorkspaceId,
                resultRevision: Revision));
        }

        /// <inheritdoc />
        public ValueTask<EngineResult<EditReceipt>> BeginEditAsync(
            BeginEditRequest request,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Revision = Revision.Next();
            return ValueTask.FromResult(EngineResult<EditReceipt>.Success(
                new EditReceipt(Guid.NewGuid(), FormKey, request.OriginFormKey, request.Role, Revision),
                WorkspaceId,
                request.OperationId,
                request.ExpectedRevision,
                Revision));
        }

        /// <inheritdoc />
        public ValueTask<EngineResult<FormListReadView>> ReadFormListViewAsync(
            ReferenceRequest request,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var context = new FormListContext(
                request,
                ReferenceResolutionStatus.Resolved,
                Output.ModKey,
                Output.PluginPath,
                1,
                PluginRole.Output);
            using var document = JsonDocument.Parse($"{{\"FormKey\":\"{FormKey}\",\"EditorID\":\"BeforeDetach\",\"FormVersion\":44}}");
            return ValueTask.FromResult(EngineResult<FormListReadView>.Success(
                new FormListReadView(context, document.RootElement.Clone()),
                WorkspaceId,
                resultRevision: Revision));
        }

        /// <inheritdoc />
        public ValueTask<EngineResult<WorkspacePreview>> PreviewAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return ValueTask.FromResult(EngineResult<WorkspacePreview>.Success(
                new WorkspacePreview([], 0, []),
                WorkspaceId,
                resultRevision: Revision));
        }

        /// <inheritdoc />
        public ValueTask<EngineResult<OutputSelectionReceipt>> SelectOutputAsync(SelectOutputRequest request, CancellationToken cancellationToken = default) => throw Unsupported();
        /// <inheritdoc />
        public ValueTask<EngineResult<IReadOnlyList<PluginSummary>>> ListPluginsAsync(CancellationToken cancellationToken = default) => throw Unsupported();
        /// <inheritdoc />
        public ValueTask<EngineResult<IReadOnlyList<FormListSummary>>> ListFormListsAsync(RecordScope scope, CancellationToken cancellationToken = default) => throw Unsupported();
        /// <inheritdoc />
        public ValueTask<EngineResult<IMajorRecordGetter>> ReadFormListAsync(FormKey formKey, RecordScope scope, CancellationToken cancellationToken = default) => throw Unsupported();
        /// <inheritdoc />
        public ValueTask<EngineResult<ReferenceSearchPage>> SearchReferencesAsync(ReferenceSearchRequest request, CancellationToken cancellationToken = default) => throw Unsupported();
        /// <inheritdoc />
        public ValueTask<EngineResult<ReferenceResolution>> ResolveReferenceAsync(ReferenceRequest request, CancellationToken cancellationToken = default) => throw Unsupported();
        /// <inheritdoc />
        public ValueTask<EngineResult<OperationReceipt>> ApplyFormListEditAsync(FormListEditRequest request, CancellationToken cancellationToken = default) => throw Unsupported();
        /// <inheritdoc />
        public ValueTask<EngineResult<FormListComparison>> CompareFormListAsync(CompareFormListRequest request, CancellationToken cancellationToken = default) => throw Unsupported();
        /// <inheritdoc />
        public ValueTask<SaveResult> SaveAsync(SaveRequest request, CancellationToken cancellationToken = default) => throw Unsupported();
        /// <inheritdoc />
        public ValueTask<EngineResult<OutputSelectionReceipt>> ResolveOutputRecoveryAsync(ResolveOutputRecoveryRequest request, CancellationToken cancellationToken = default) => throw Unsupported();
        /// <inheritdoc />
        public ValueTask<EngineResult<OutputSelectionReceipt>> ReopenOutputAsync(ReopenOutputRequest request, CancellationToken cancellationToken = default) => throw Unsupported();
        /// <inheritdoc />
        public ValueTask<EngineResult<OperationReceipt>> DiscardChangesAsync(DiscardChangesRequest request, CancellationToken cancellationToken = default) => throw Unsupported();

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }

        /// <summary>Creates the stable unsupported-operation exception for unused workspace APIs.</summary>
        /// <returns>The unsupported-operation exception.</returns>
        private static NotSupportedException Unsupported()
        {
            return new NotSupportedException("This reattachment fixture supports only the editor Begin read path.");
        }
    }
}
