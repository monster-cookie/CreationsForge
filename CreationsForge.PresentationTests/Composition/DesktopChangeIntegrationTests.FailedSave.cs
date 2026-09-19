using Autofac;
using Avalonia.Headless.XUnit;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Enums;
using CreationsForge.Services;
using CreationsForge.Services.Interfaces;
using CreationsForge.ViewModels;
using CreationsForge.Starfield.PluginAdapter.Edits;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Strings;
using Shouldly;

namespace CreationsForge.PresentationTests.Composition;

/// <content>Verifies a failed real Starfield save leaves the new output discardable through the desktop leave flow.</content>
public sealed partial class DesktopChangeIntegrationTests
{
    /// <summary>Verifies the localized new-plugin choice commits a multilingual FormList and its string files through the desktop save flow.</summary>
    /// <returns>A task that completes after destination artifacts and the clean leave receipt are checked.</returns>
    [AvaloniaFact]
    public async Task ProductionChanges_StarfieldLocalizedNewOutput_SaveAndProceed()
    {
        using var fixture = DesktopWorkspaceFixture.Create(SupportedGame.Starfield);
        var sourceBytes = fixture.SnapshotSourceBytes();
        var outputDirectory = fixture.OutputDirectory.CreateSubdirectory("LocalizedSave");
        var outputPath = Path.Combine(outputDirectory.FullName, "LocalizedSave.esm");
        var association = new OutputAssociation(
            outputPath,
            ModKey.FromNameAndExtension("LocalizedSave.esm"),
            LocalizedOutputMode.SeparateStringFiles,
            OutputMasterStyle.Small);
        await using var container = fixture.CreateContainer("LocalizedSave");
        var dialog = new DesktopChangeDialog();
        await using var viewScope = container.BeginLifetimeScope(builder =>
            builder.RegisterInstance(dialog).As<IWorkspaceChangesDialogService>());
        var coordinator = container.Resolve<IWorkspaceCoordinator>();
        var opened = await coordinator.OpenAsync(new WorkspaceLaunchRequest(
            fixture.CreateSourceRequest(), OutputSelectionMode.CreateNew, association),
            TestContext.Current.CancellationToken);
        opened.Succeeded.ShouldBeTrue(opened.Error?.Message);
        var browser = viewScope.Resolve<FormListBrowserViewModel>();
        var changes = viewScope.Resolve<WorkspaceChangesViewModel>();
        await browser.StartAsync();

        var begun = await coordinator.ExecuteAsync(
            (workspace, token) => workspace.BeginEditAsync(
                new BeginEditRequest(Guid.NewGuid(), workspace.Revision, FormListEditRole.New), token),
            TestContext.Current.CancellationToken);
        begun.Succeeded.ShouldBeTrue(begun.Error?.Message);
        var name = new TranslatedString(Language.English, "English name");
        name.Set(Language.French, "Nom francais");
        var applied = await coordinator.ExecuteAsync(
            (workspace, token) => workspace.ApplyFormListEditAsync(
                new FormListEditRequest(
                    Guid.NewGuid(), workspace.Revision, begun.Value!.EditId, new StarfieldSetNameEdit(name)), token),
            TestContext.Current.CancellationToken);
        applied.Succeeded.ShouldBeTrue(applied.Error?.Message);

        dialog.Enqueue(WorkspaceChangesDialogPurpose.Leave, async (model, cancellationToken) =>
        {
            (await model.ApplyLeaveChoiceAsync(WorkspaceChangesDialogChoice.SaveAndProceed, cancellationToken))
                .ShouldBe(WorkspaceLeaveChoiceOutcome.Proceed);
            model.ErrorMessage.ShouldBeNull();
            return WorkspaceChangesDialogResult.Proceed;
        });
        using var reservation = await changes.ReserveLeaveAsync(
            WorkspaceLeaveReason.CloseWorkspace,
            TestContext.Current.CancellationToken);

        dialog.AssertDrained();
        reservation.ShouldNotBeNull().Disposition.ShouldBe(WorkspaceLeaveDisposition.ReadyAndClean);
        File.Exists(outputPath).ShouldBeTrue();
        Directory.GetFiles(Path.Combine(outputDirectory.FullName, "Strings"), "*.STRINGS")
            .ShouldNotBeEmpty();
        fixture.SnapshotSourceBytes().ShouldBe(sourceBytes);
        await changes.ShutdownAndDrainAsync();
        await coordinator.CloseAsync();
    }

    /// <summary>Verifies the desktop's toolbar discard can proceed after strict staged validation rejects embedded nondefault translations.</summary>
    /// <returns>A task that completes after the discarded preview and absent destination are proved.</returns>
    [AvaloniaFact]
    public async Task ProductionChanges_StarfieldFailedNewOutputSave_ToolbarDiscardAndProceed()
    {
        using var fixture = DesktopWorkspaceFixture.Create(SupportedGame.Starfield);
        var sourceBytes = fixture.SnapshotSourceBytes();
        var outputDirectory = fixture.OutputDirectory.CreateSubdirectory("FailedSave");
        var outputPath = Path.Combine(outputDirectory.FullName, "FailedSave.esm");
        var association = new OutputAssociation(
            outputPath,
            ModKey.FromNameAndExtension("FailedSave.esm"),
            LocalizedOutputMode.Embedded,
            OutputMasterStyle.Small);
        await using var container = fixture.CreateContainer("FailedSave");
        var dialog = new DesktopChangeDialog();
        await using var viewScope = container.BeginLifetimeScope(builder =>
            builder.RegisterInstance(dialog).As<IWorkspaceChangesDialogService>());
        var coordinator = container.Resolve<IWorkspaceCoordinator>();
        var opened = await coordinator.OpenAsync(new WorkspaceLaunchRequest(
            fixture.CreateSourceRequest(), OutputSelectionMode.CreateNew, association),
            TestContext.Current.CancellationToken);
        opened.Succeeded.ShouldBeTrue(opened.Error?.Message);
        var browser = viewScope.Resolve<FormListBrowserViewModel>();
        var changes = viewScope.Resolve<WorkspaceChangesViewModel>();
        await browser.StartAsync();

        var begun = await coordinator.ExecuteAsync(
            (workspace, token) => workspace.BeginEditAsync(
                new BeginEditRequest(Guid.NewGuid(), workspace.Revision, FormListEditRole.New), token),
            TestContext.Current.CancellationToken);
        begun.Succeeded.ShouldBeTrue(begun.Error?.Message);
        var name = new TranslatedString(Language.English, "English name");
        name.Set(Language.French, "Nom francais");
        var applied = await coordinator.ExecuteAsync(
            (workspace, token) => workspace.ApplyFormListEditAsync(
                new FormListEditRequest(
                    Guid.NewGuid(), workspace.Revision, begun.Value!.EditId, new StarfieldSetNameEdit(name)), token),
            TestContext.Current.CancellationToken);
        applied.Succeeded.ShouldBeTrue(applied.Error?.Message);

        dialog.Enqueue(WorkspaceChangesDialogPurpose.Save, async (model, cancellationToken) =>
        {
            (await model.ApplyLeaveChoiceAsync(WorkspaceChangesDialogChoice.SaveAndProceed, cancellationToken))
                .ShouldBe(WorkspaceLeaveChoiceOutcome.ContinueDialog);
            model.ErrorMessage.ShouldNotBeNull().ShouldContain("changed field 'Name'");
            return WorkspaceChangesDialogResult.KeepEditing;
        });
        await changes.ShowSaveChangesDialogAsync(TestContext.Current.CancellationToken);
        dialog.Enqueue(WorkspaceChangesDialogPurpose.Discard, async (model, cancellationToken) =>
        {
            model.CanDiscardAndProceed.ShouldBeTrue();
            (await model.ApplyLeaveChoiceAsync(WorkspaceChangesDialogChoice.DiscardAndProceed, cancellationToken))
                .ShouldBe(WorkspaceLeaveChoiceOutcome.Proceed);
            return WorkspaceChangesDialogResult.Proceed;
        });
        await changes.ShowDiscardChangesDialogAsync(TestContext.Current.CancellationToken);

        dialog.AssertDrained();
        (await ReadPreviewAsync(coordinator)).Comparisons.ShouldBeEmpty();
        File.Exists(outputPath).ShouldBeFalse();
        fixture.SnapshotSourceBytes().ShouldBe(sourceBytes);
        await changes.ShutdownAndDrainAsync();
        await coordinator.CloseAsync();
    }
}
