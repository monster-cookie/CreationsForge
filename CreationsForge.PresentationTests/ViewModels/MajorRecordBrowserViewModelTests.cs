using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Enums;
using CreationsForge.PresentationTests.Support;
using CreationsForge.Services;
using CreationsForge.ViewModels;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Shouldly;

namespace CreationsForge.PresentationTests.ViewModels;

/// <summary>Verifies complete major-record grouping, exact contexts, and native field comparison presentation.</summary>
public sealed class MajorRecordBrowserViewModelTests
{
    /// <summary>Verifies one-pass summary loading includes admitted records and selection compares synchronized origin and winner fields.</summary>
    /// <returns>A task that completes after complete loading and comparison publication.</returns>
    [Fact]
    public async Task StartAndSelectRecord_VisitAllWinningSummariesContextsAndSynchronizedFields()
    {
        var workspaceId = Guid.NewGuid();
        var revision = new WorkspaceRevision(Guid.NewGuid(), 9);
        var sourceMod = ModKey.FromNameAndExtension("Source.esm");
        var patchMod = ModKey.FromNameAndExtension("Patch.esm");
        var bookKey = new FormKey(sourceMod, 0x100);
        var secondBookKey = new FormKey(sourceMod, 0x102);
        var masterOnlyKey = new FormKey(sourceMod, 0x101);
        var keywordKey = new FormKey(patchMod, 0x200);
        var sourcePath = AbsolutePath(sourceMod.FileName);
        var patchPath = AbsolutePath(patchMod.FileName);
        var visitCount = 0;
        var searchRequests = new List<ReferenceSearchRequest>();
        var comparisonRequests = new List<CompareMajorRecordRequest>();
        var workspace = new RecordingFormListBrowserWorkspace(
            workspaceId,
            revision,
            _ => ValueTask.FromResult(EngineResult<WorkspaceState>.Success(
                new WorkspaceState(
                    SupportedGame.Starfield,
                    GameRelease.Starfield,
                    null,
                    null,
                    new OutputSynchronizationState(OutputSynchronizationStatus.Ready, null),
                    revision),
                workspaceId,
                resultRevision: revision)),
            _ => ValueTask.FromResult(EngineResult<IReadOnlyList<PluginSummary>>.Success([], workspaceId, resultRevision: revision)),
            (_, _) => ValueTask.FromResult(EngineResult<IReadOnlyList<FormListSummary>>.Success([], workspaceId, resultRevision: revision)),
            (_, _) => throw new NotSupportedException(),
            null,
            (request, _) =>
            {
                searchRequests.Add(request);
                return ValueTask.FromResult(EngineResult<ReferenceSearchPage>.Success(
                    new ReferenceSearchPage(
                        [
                            Match(bookKey, "Book", "OriginalBook", sourceMod, sourcePath, 0, PluginRole.Source),
                            Match(bookKey, "Book", "ExampleBook", patchMod, patchPath, 1, PluginRole.LoadOrder)
                        ],
                        null),
                    workspaceId,
                    resultRevision: revision));
            },
            (request, _) =>
            {
                comparisonRequests.Add(request);
                var beforeContext = new FormListContext(
                    request.Before,
                    ReferenceResolutionStatus.Resolved,
                    sourceMod,
                    sourcePath,
                    0,
                    PluginRole.Source);
                var afterContext = new FormListContext(
                    request.After,
                    ReferenceResolutionStatus.Resolved,
                    patchMod,
                    patchPath,
                    1,
                    PluginRole.LoadOrder);
                var before = JsonSerializer.SerializeToElement(new { Type = "Book", Name = new { Value = "Before" }, Value = 10 });
                var after = JsonSerializer.SerializeToElement(new { Type = "Book", Name = new { Value = "After" }, Value = 10 });
                return ValueTask.FromResult(EngineResult<MajorRecordComparison>.Success(
                    new MajorRecordComparison(
                        beforeContext,
                        afterContext,
                        "Book",
                        before,
                        after,
                        [new SemanticChangeDescriptor("$record.Name", SemanticChangeKind.ValueChanged)],
                        []),
                    workspaceId,
                    resultRevision: revision));
            },
            (onRecord, onProgress, _) =>
            {
                visitCount++;
                onRecord(Match(bookKey, "Book", "ExampleBook", patchMod, patchPath, 1, PluginRole.LoadOrder));
                onRecord(Match(masterOnlyKey, "Weapon", "BaseWeapon", sourceMod, sourcePath, 0, PluginRole.Source));
                onRecord(Match(secondBookKey, "Book", "AardvarkBook", sourceMod, sourcePath, 0, PluginRole.Source));
                onProgress?.Invoke(sourceMod, 3);
                onRecord(Match(keywordKey, "Keyword", "ExampleKeyword", patchMod, patchPath, 1, PluginRole.LoadOrder));
                onProgress?.Invoke(patchMod, 4);
                return ValueTask.FromResult(EngineResult<int>.Success(4, workspaceId, resultRevision: revision));
            });
        var coordinator = new RecordingWorkspaceCoordinator();
        coordinator.Publish(CreateDescriptor(workspaceId, revision, sourcePath), workspace);
        using var viewModel = new MajorRecordBrowserViewModel(
            coordinator,
            new RecordJsonTreeProjectionService(),
            new InlineUiDispatcher());
        var loadingStatuses = new List<string>();
        viewModel.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(MajorRecordBrowserViewModel.StatusText))
            {
                loadingStatuses.Add(viewModel.StatusText);
            }
        };

        await viewModel.StartAsync();

        visitCount.ShouldBe(1);
        viewModel.Records.Select(record => record.RecordType).ShouldBe(["Book", "Weapon", "Book", "Keyword"]);
        viewModel.RecordGroups.Select(group => group.Label).ShouldBe(["Book (2)", "Keyword (1)", "Weapon (1)"]);
        viewModel.LoadedRecordCountText.ShouldBe("Loaded records: 4");
        viewModel.IsBusy.ShouldBeFalse();
        loadingStatuses.ShouldContain(status => status.Contains("Source.esm") && status.Contains("3 records"));
        viewModel.StatusText.ShouldBe("Loaded all 4 major record(s).");

        viewModel.SelectedPluginOnly = true;
        await viewModel.CurrentFilterTask;
        viewModel.RecordGroups.Select(group => group.Label).ShouldBe(["Book (1)", "Keyword (1)"]);
        viewModel.RecordGroups.SelectMany(group => group.Children).Cast<MajorRecordViewModel>()
            .All(record => record.SourcePath == patchPath).ShouldBeTrue();
        viewModel.LoadedRecordCountText.ShouldBe("Showing 2 of 4 records");
        viewModel.SelectedPluginOnly = false;
        await viewModel.CurrentFilterTask;

        viewModel.EditorIdFilter = "book";
        await viewModel.CurrentFilterTask;
        viewModel.RecordGroups.ShouldHaveSingleItem().Label.ShouldBe("Book (2)");
        viewModel.LoadedRecordCountText.ShouldBe("Showing 2 of 4 records");
        viewModel.FormIdFilter = "00000102";
        await viewModel.CurrentFilterTask;
        viewModel.RecordGroups.ShouldHaveSingleItem().Children.Cast<MajorRecordViewModel>()
            .ShouldHaveSingleItem().FormKey.ShouldBe(secondBookKey);
        viewModel.FormIdFilter = string.Empty;
        viewModel.EditorIdFilter = string.Empty;
        viewModel.RecordSortMode = MajorRecordSortMode.EditorId;
        await viewModel.CurrentFilterTask;
        viewModel.RecordGroups[0].Children.Cast<MajorRecordViewModel>()
            .Select(record => record.FormKey).ShouldBe([secondBookKey, bookKey]);
        viewModel.RecordSortMode = MajorRecordSortMode.FormId;
        await viewModel.CurrentFilterTask;
        viewModel.RecordGroups[0].Children.Cast<MajorRecordViewModel>()
            .Select(record => record.FormKey).ShouldBe([bookKey, secondBookKey]);

        await viewModel.SelectRecordAsync(viewModel.Records[0]);

        var search = searchRequests.ShouldHaveSingleItem();
        search.Query.ShouldBe(bookKey.ToString());
        search.Scope.ShouldBe(RecordScope.AllContexts);
        viewModel.ContextOptions.Count.ShouldBe(3);
        viewModel.ContextOptions[0].Label.ShouldContain(patchMod.FileName.String);
        viewModel.ContextOptions[0].SourcePath.ShouldBe(patchPath);
        viewModel.ContextOptions[0].LoadOrderIndex.ShouldBe(1);
        viewModel.ContextOptions[0].Role.ShouldBe(PluginRole.LoadOrder);
        viewModel.SelectedBeforeContext!.ContainingModKey.ShouldBe(sourceMod);
        viewModel.SelectedAfterContext!.IsWinningOverride.ShouldBeTrue();
        var comparison = comparisonRequests.ShouldHaveSingleItem();
        comparison.Before.ContainingModKey.ShouldBe(sourceMod);
        comparison.After.Scope.ShouldBe(RecordScope.WinningOverrides);
        viewModel.BeforeContext!.ContainingModKey.ShouldBe(sourceMod);
        viewModel.AfterContext!.ContainingModKey.ShouldBe(patchMod);
        viewModel.BeforeFields.ShouldHaveSingleItem().Children.Single(field => field.Name == "Value")
            .ComparisonState.ShouldBe(ComparisonFieldState.Identical);
        viewModel.AfterFields.ShouldHaveSingleItem().Children.Single(field => field.Name == "Name")
            .ComparisonState.ShouldBe(ComparisonFieldState.WinningOverride);
        var beforeName = viewModel.BeforeFields.Single().Children.Single(field => field.Name == "Name");
        var afterName = viewModel.AfterFields.Single().Children.Single(field => field.Name == "Name");
        beforeName.IsExpanded = true;
        afterName.IsExpanded.ShouldBeTrue();
        afterName.IsExpanded = false;
        beforeName.IsExpanded.ShouldBeFalse();
        viewModel.SemanticChanges.ShouldHaveSingleItem().FieldIdentifier.ShouldBe("$record.Name");
        viewModel.HasError.ShouldBeFalse();
    }

    /// <summary>Creates one lightweight record context match.</summary>
    /// <param name="formKey">The record identity.</param>
    /// <param name="recordType">The stable record family.</param>
    /// <param name="editorId">The optional EditorID.</param>
    /// <param name="containingModKey">The containing plugin.</param>
    /// <param name="sourcePath">The containing plugin path.</param>
    /// <param name="loadOrderIndex">The load-order index.</param>
    /// <param name="role">The workspace role.</param>
    /// <returns>The immutable match.</returns>
    private static ReferenceSearchMatch Match(
        FormKey formKey,
        string recordType,
        string? editorId,
        ModKey containingModKey,
        string sourcePath,
        int loadOrderIndex,
        PluginRole role)
    {
        return new ReferenceSearchMatch(
            formKey,
            recordType,
            editorId,
            containingModKey,
            sourcePath,
            loadOrderIndex,
            role);
    }

    /// <summary>Creates a navigation-safe workspace descriptor.</summary>
    /// <param name="workspaceId">The workspace identity.</param>
    /// <param name="revision">The exact workspace revision.</param>
    /// <param name="sourcePath">The selected source path.</param>
    /// <returns>The immutable descriptor.</returns>
    private static WorkspaceDescriptor CreateDescriptor(
        Guid workspaceId,
        WorkspaceRevision revision,
        string sourcePath)
    {
        var outputModKey = ModKey.FromNameAndExtension("Patch.esm");
        return new WorkspaceDescriptor(
            workspaceId,
            SupportedGame.Starfield,
            GameRelease.Starfield,
            sourcePath,
            [sourcePath],
            new OutputAssociation(
                AbsolutePath(outputModKey.FileName),
                outputModKey,
                LocalizedOutputMode.Embedded,
                OutputMasterStyle.Full),
            revision);
    }

    /// <summary>Creates a fully qualified test-only plugin path.</summary>
    /// <param name="fileName">The plugin file name.</param>
    /// <returns>The fully qualified path.</returns>
    private static string AbsolutePath(string fileName)
    {
        return Path.GetFullPath(Path.Combine(Path.GetTempPath(), "CreationsForge-MajorRecordBrowserTests", fileName));
    }
}
