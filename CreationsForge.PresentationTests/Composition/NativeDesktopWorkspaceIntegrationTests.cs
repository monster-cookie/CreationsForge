using Autofac;
using Avalonia.Headless.XUnit;
using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Enums;
using CreationsForge.PresentationTests.Headless;
using CreationsForge.Services;
using CreationsForge.Services.Interfaces;
using CreationsForge.ViewModels;
using Shouldly;

namespace CreationsForge.PresentationTests.Composition;

/// <summary>
/// Verifies each real game adapter produces a usable workspace through the production desktop container and coordinator.
/// </summary>
[Collection(AvaloniaControlTestCollection.Name)]
public sealed class NativeDesktopWorkspaceIntegrationTests
{
    /// <summary>Verifies Starfield source, new-output, and existing-output acquisition through the production desktop graph.</summary>
    /// <returns>A task that completes after coordinator and container disposal.</returns>
    [AvaloniaFact]
    public Task ProductionCoordinator_Starfield_OpensUsableSourceAndOutputs()
    {
        return VerifyGameAsync(SupportedGame.Starfield);
    }

    /// <summary>Verifies Fallout 4 source, new-output, and existing-output acquisition through the production desktop graph.</summary>
    /// <returns>A task that completes after coordinator and container disposal.</returns>
    [AvaloniaFact]
    public Task ProductionCoordinator_Fallout4_OpensUsableSourceAndOutputs()
    {
        return VerifyGameAsync(SupportedGame.Fallout4);
    }

    /// <summary>Verifies Skyrim Special Edition source, new-output, and existing-output acquisition through the production desktop graph.</summary>
    /// <returns>A task that completes after coordinator and container disposal.</returns>
    [AvaloniaFact]
    public Task ProductionCoordinator_Skyrim_OpensUsableSourceAndOutputs()
    {
        return VerifyGameAsync(SupportedGame.Skyrim);
    }

    /// <summary>Verifies two production desktop containers own isolated Starfield coordinators and workspaces over one read-only source.</summary>
    /// <returns>A task that completes after both containers release independent native state.</returns>
    [AvaloniaFact]
    public async Task ProductionCoordinator_TwoContainers_OwnIndependentStarfieldWorkspaces()
    {
        using var fixture = NativeDesktopWorkspaceFixture.Create(SupportedGame.Starfield);
        var sourceBytes = fixture.SnapshotSourceBytes();
        await using var firstContainer = fixture.CreateContainer("IsolationFirst");
        await using var secondContainer = fixture.CreateContainer("IsolationSecond");
        var firstCoordinator = firstContainer.Resolve<INativeWorkspaceCoordinator>();
        var secondCoordinator = secondContainer.Resolve<INativeWorkspaceCoordinator>();

        var firstOpen = await firstCoordinator.OpenAsync(new NativeWorkspaceOpenRequest(
            fixture.CreateSourceRequest(),
            OutputSelectionMode.CreateNew,
            fixture.CreateNewOutput("IsolationFirst")), TestContext.Current.CancellationToken);
        var secondOpen = await secondCoordinator.OpenAsync(new NativeWorkspaceOpenRequest(
            fixture.CreateSourceRequest(),
            OutputSelectionMode.CreateNew,
            fixture.CreateNewOutput("IsolationSecond")), TestContext.Current.CancellationToken);

        firstOpen.Succeeded.ShouldBeTrue(firstOpen.Error?.Message);
        secondOpen.Succeeded.ShouldBeTrue(secondOpen.Error?.Message);
        firstOpen.Value!.WorkspaceId.ShouldNotBe(secondOpen.Value!.WorkspaceId);
        firstCoordinator.CurrentWorkspace!.WorkspaceId.ShouldBe(firstOpen.Value.WorkspaceId);
        secondCoordinator.CurrentWorkspace!.WorkspaceId.ShouldBe(secondOpen.Value.WorkspaceId);

        await firstCoordinator.CloseAsync();
        firstCoordinator.CurrentWorkspace.ShouldBeNull();
        secondCoordinator.CurrentWorkspace.ShouldNotBeNull();
        var secondLists = await ListFormListsAsync(secondCoordinator, RecordScope.Source);
        secondLists.Succeeded.ShouldBeTrue(secondLists.Error?.Message);
        secondLists.Value!.ShouldContain(summary => summary.FormKey == fixture.SourceFormKey);

        await secondCoordinator.DisposeAsync();
        secondCoordinator.CurrentWorkspace.ShouldBeNull();
        fixture.SnapshotSourceBytes().ShouldBe(sourceBytes);
    }

    /// <summary>Exercises one game's complete source, new-output, existing-output, close, and disposal path.</summary>
    /// <param name="game">The game whose real adapter is selected by the production factory.</param>
    /// <returns>A task that completes after the fixture and production graph are released.</returns>
    private static async Task VerifyGameAsync(SupportedGame game)
    {
        using var fixture = NativeDesktopWorkspaceFixture.Create(game);
        var sourceBytes = fixture.SnapshotSourceBytes();
        var existingOutputBytes = File.ReadAllBytes(fixture.ExistingOutput.PluginPath);
        await using var container = fixture.CreateContainer(game.ToString());
        var coordinator = container.Resolve<INativeWorkspaceCoordinator>();
        var createdOutput = fixture.CreateNewOutput(game.ToString());

        var createResult = await coordinator.OpenAsync(new NativeWorkspaceOpenRequest(
            fixture.CreateSourceRequest(),
            OutputSelectionMode.CreateNew,
            createdOutput), TestContext.Current.CancellationToken);

        createResult.Succeeded.ShouldBeTrue(createResult.Error?.Message);
        createResult.Value!.Game.ShouldBe(game);
        createResult.Value.Output.PluginPath.ShouldBe(Path.GetFullPath(createdOutput.PluginPath));
        File.Exists(createdOutput.PluginPath).ShouldBeFalse();
        await AssertReadableFormListAsync(coordinator, fixture.SourceFormKey, RecordScope.Source);
        using (var browserScope = container.BeginLifetimeScope())
        {
            var browser = browserScope.Resolve<NativeFormListBrowserViewModel>();
            await browser.StartAsync();
            var root = browser.Records.Single(record => record.FormKey == fixture.SourceFormKey);
            await browser.SelectRecordAsync(root);

            var directComparison = await coordinator.ExecuteAsync(
                (workspace, cancellationToken) => workspace.CompareFormListAsync(
                    new CompareFormListRequest(
                        browser.SelectedBeforeContext!.Selection,
                        browser.SelectedAfterContext!.Selection),
                    cancellationToken),
                TestContext.Current.CancellationToken);
            directComparison.Succeeded.ShouldBeTrue(directComparison.Error?.Message);
            var comparison = directComparison.Value.ShouldNotBeNull();
            AssertProjection(comparison.Before!.Value, browser.BeforeFields.ShouldHaveSingleItem(), "$");
            AssertProjection(comparison.After!.Value, browser.AfterFields.ShouldHaveSingleItem(), "$");
            var items = browser.BeforeFields.ShouldHaveSingleItem().Children.Single(node => node.Name == "Items");
            items.Children.Count.ShouldBe(3);
        }

        await coordinator.CloseAsync();
        coordinator.CurrentWorkspace.ShouldBeNull();
        var closedRead = await ListFormListsAsync(coordinator, RecordScope.Source);
        closedRead.Succeeded.ShouldBeFalse();
        closedRead.Error!.Code.ShouldBe(EngineErrorCode.InvalidRequest);

        var existingResult = await coordinator.OpenAsync(new NativeWorkspaceOpenRequest(
            fixture.CreateSourceRequest(),
            OutputSelectionMode.OpenExisting,
            fixture.ExistingOutput), TestContext.Current.CancellationToken);

        existingResult.Succeeded.ShouldBeTrue(existingResult.Error?.Message);
        existingResult.Value!.Game.ShouldBe(game);
        existingResult.Value.Output.PluginPath.ShouldBe(Path.GetFullPath(fixture.ExistingOutput.PluginPath));
        var plugins = await coordinator.ExecuteAsync(
            static (workspace, cancellationToken) => workspace.ListPluginsAsync(cancellationToken),
            TestContext.Current.CancellationToken);
        plugins.Succeeded.ShouldBeTrue(plugins.Error?.Message);
        plugins.Value!.ShouldContain(plugin =>
            plugin.Role == PluginRole.Output && plugin.ModKey == fixture.ExistingOutput.ModKey);
        var outputLists = await ListFormListsAsync(coordinator, RecordScope.StagedOutput);
        outputLists.Succeeded.ShouldBeTrue(outputLists.Error?.Message);
        outputLists.Value!.ShouldContain(summary =>
            summary.FormKey == fixture.ExistingOutputFormKey && summary.EditorId == "PresentationOutputList");
        await AssertReadableFormListAsync(coordinator, fixture.ExistingOutputFormKey, RecordScope.StagedOutput);

        await coordinator.DisposeAsync();
        coordinator.CurrentWorkspace.ShouldBeNull();
        fixture.SnapshotSourceBytes().ShouldBe(sourceBytes);
        File.ReadAllBytes(fixture.ExistingOutput.PluginPath).ShouldBe(existingOutputBytes);
        File.Exists(createdOutput.PluginPath).ShouldBeFalse();
    }

    /// <summary>Lists FormLists through the coordinator's serialized workspace borrowing boundary.</summary>
    /// <param name="coordinator">The production coordinator that owns the workspace.</param>
    /// <param name="scope">The native record scope to enumerate.</param>
    /// <returns>The engine list result.</returns>
    private static ValueTask<EngineResult<IReadOnlyList<FormListSummary>>> ListFormListsAsync(
        INativeWorkspaceCoordinator coordinator,
        RecordScope scope)
    {
        return coordinator.ExecuteAsync(
            (workspace, cancellationToken) => workspace.ListFormListsAsync(scope, cancellationToken),
            TestContext.Current.CancellationToken);
    }

    /// <summary>Asserts one exact FormList can be listed and read as a detached native record through the coordinator.</summary>
    /// <param name="coordinator">The production coordinator that owns the workspace.</param>
    /// <param name="formKey">The native FormList identity to read.</param>
    /// <param name="scope">The source or staged-output view containing the record.</param>
    /// <returns>A task that completes after the record is listed and read.</returns>
    private static async Task AssertReadableFormListAsync(
        INativeWorkspaceCoordinator coordinator,
        Mutagen.Bethesda.Plugins.FormKey formKey,
        RecordScope scope)
    {
        var lists = await ListFormListsAsync(coordinator, scope);
        lists.Succeeded.ShouldBeTrue(lists.Error?.Message);
        lists.Value!.ShouldContain(summary => summary.FormKey == formKey);

        var read = await coordinator.ExecuteAsync(
            (workspace, cancellationToken) => workspace.ReadFormListAsync(formKey, scope, cancellationToken),
            TestContext.Current.CancellationToken);
        var readFailure = read.Error is null
            ? "The native read operation failed without a typed engine error."
            : $"{read.Error.Code}: {read.Error.Message}";
        read.Succeeded.ShouldBeTrue(readFailure);
        var record = read.Value.ShouldNotBeNull();
        record.FormKey.ShouldBe(formKey);
    }

    /// <summary>Recursively verifies browser nodes retain the exact detached engine JSON hierarchy and order.</summary>
    /// <param name="element">The authoritative detached engine JSON value.</param>
    /// <param name="node">The corresponding browser projection node.</param>
    /// <param name="expectedName">The expected property, index, or root label.</param>
    private static void AssertProjection(
        JsonElement element,
        NativeJsonFieldNodeViewModel node,
        string expectedName)
    {
        node.Name.ShouldBe(expectedName);
        node.ValueKind.ShouldBe(element.ValueKind);
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                var properties = element.EnumerateObject().ToArray();
                node.Children.Count.ShouldBe(properties.Length);
                node.ValueText.ShouldBe(properties.Length == 0 ? "{}" : "{...}");
                for (var index = 0; index < properties.Length; index++)
                {
                    AssertProjection(properties[index].Value, node.Children[index], properties[index].Name);
                }

                break;
            case JsonValueKind.Array:
                var items = element.EnumerateArray().ToArray();
                node.Children.Count.ShouldBe(items.Length);
                node.ValueText.ShouldBe(items.Length == 0 ? "[]" : "[...]");
                for (var index = 0; index < items.Length; index++)
                {
                    AssertProjection(items[index], node.Children[index], $"[{index}]");
                }

                break;
            default:
                node.Children.ShouldBeEmpty();
                node.ValueText.ShouldBe(element.GetRawText());
                break;
        }
    }
}
