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

/// <summary>Verifies paged major-record grouping, exact contexts, and native field comparison presentation.</summary>
public sealed class MajorRecordBrowserViewModelTests
{
    /// <summary>Verifies source-plugin pages append by family and a selected record compares synchronized origin and winner fields.</summary>
    /// <returns>A task that completes after paging and comparison publication.</returns>
    [Fact]
    public async Task StartLoadMoreAndSelectRecord_PreserveSourcePagesContextsAndSynchronizedFields()
    {
        var workspaceId = Guid.NewGuid();
        var revision = new WorkspaceRevision(Guid.NewGuid(), 9);
        var sourceMod = ModKey.FromNameAndExtension("Source.esm");
        var patchMod = ModKey.FromNameAndExtension("Patch.esm");
        var bookKey = new FormKey(sourceMod, 0x100);
        var keywordKey = new FormKey(sourceMod, 0x200);
        var sourcePath = AbsolutePath(sourceMod.FileName);
        var patchPath = AbsolutePath(patchMod.FileName);
        var pageRequests = new List<MajorRecordListRequest>();
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
            (request, _) =>
            {
                pageRequests.Add(request);
                var records = request.ContinuationToken is null
                    ? new[] { Match(bookKey, "Book", "OriginalBook", sourceMod, sourcePath, 0, PluginRole.Source) }
                    : new[] { Match(keywordKey, "Keyword", "ExampleKeyword", sourceMod, sourcePath, 0, PluginRole.Source) };
                return ValueTask.FromResult(EngineResult<MajorRecordListPage>.Success(
                    new MajorRecordListPage(records, request.ContinuationToken is null ? "page-two" : null),
                    workspaceId,
                    resultRevision: revision));
            },
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
            });
        var coordinator = new RecordingWorkspaceCoordinator();
        coordinator.Publish(CreateDescriptor(workspaceId, revision, sourcePath), workspace);
        using var viewModel = new MajorRecordBrowserViewModel(
            coordinator,
            new RecordJsonTreeProjectionService(),
            new InlineUiDispatcher());

        await viewModel.StartAsync();

        pageRequests.Count.ShouldBe(1);
        pageRequests[0].MaximumResults.ShouldBe(MajorRecordBrowserViewModel.PageSize);
        pageRequests[0].Scope.ShouldBe(RecordScope.Source);
        viewModel.Records.ShouldHaveSingleItem().RecordType.ShouldBe("Book");
        viewModel.RecordGroups.ShouldHaveSingleItem().Label.ShouldBe("Book (1)");
        viewModel.HasMoreRecords.ShouldBeTrue();

        await viewModel.LoadMoreAsync();

        pageRequests.Count.ShouldBe(2);
        pageRequests[1].ContinuationToken.ShouldBe("page-two");
        viewModel.Records.Select(record => record.RecordType).ShouldBe(["Book", "Keyword"]);
        viewModel.RecordGroups.Select(group => group.Label).ShouldBe(["Book (1)", "Keyword (1)"]);
        viewModel.HasMoreRecords.ShouldBeFalse();

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
        return new WorkspaceDescriptor(
            workspaceId,
            SupportedGame.Starfield,
            GameRelease.Starfield,
            sourcePath,
            [sourcePath],
            null,
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
