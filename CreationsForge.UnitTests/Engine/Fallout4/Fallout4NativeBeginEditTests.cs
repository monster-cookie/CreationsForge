using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeInputs;
using CreationsForge.Core.Engine.NativeOutputs;
using CreationsForge.Fallout4.Native;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Plugins;
using Shouldly;

namespace CreationsForge.UnitTests.Engine.Fallout4;

/// <summary>Verifies native Fallout 4 FormList allocation, override, and existing-output selection.</summary>
public sealed class Fallout4NativeBeginEditTests
{
    /// <summary>New edits allocate output-owned identities that remain unique across every native record family.</summary>
    /// <returns>A task that completes after the native source and output lifetimes are released.</returns>
    [Fact]
    public async Task BeginEdit_New_AllocatesUniqueOutputOwnedFormLists()
    {
        using var fixture = Fallout4NativeOutputTestFixture.Create();
        var sourceBytes = fixture.Sources.SnapshotArtifacts();
        await using var sources = await OpenSourcesAsync(fixture);
        var service = CreateService();
        await using var state = await CreateNewStateAsync(
            fixture,
            sources,
            service,
            "NewEdits.esm");

        var firstRequest = new BeginEditRequest(
            Guid.NewGuid(),
            sources.Revision,
            FormListEditRole.New);
        var first = service.BeginEdit(
            sources,
            state,
            firstRequest,
            TestContext.Current.CancellationToken);
        var secondRequest = new BeginEditRequest(
            Guid.NewGuid(),
            sources.Revision,
            FormListEditRole.New);
        var second = service.BeginEdit(
            sources,
            state,
            secondRequest,
            TestContext.Current.CancellationToken);

        first.Succeeded.ShouldBeTrue(first.Error?.Message);
        second.Succeeded.ShouldBeTrue(second.Error?.Message);
        first.Value!.EditId.ShouldBe(firstRequest.OperationId);
        second.Value!.EditId.ShouldBe(secondRequest.OperationId);
        first.Value.FormKey.ModKey.ShouldBe(state.Association.ModKey);
        second.Value.FormKey.ModKey.ShouldBe(state.Association.ModKey);
        second.Value.FormKey.ShouldNotBe(first.Value.FormKey);
        first.Value.OriginFormKey.ShouldBeNull();
        first.Value.Role.ShouldBe(FormListEditRole.New);

        var snapshot = state.CreateSnapshot(TestContext.Current.CancellationToken);
        snapshot.FormLists.Count.ShouldBe(2);
        snapshot.FormLists.Select(record => record.FormKey)
            .ShouldBe([first.Value.FormKey, second.Value.FormKey], ignoreOrder: false);
        var allKeys = snapshot.EnumerateMajorRecords().Select(record => record.FormKey).ToArray();
        allKeys.Distinct().Count().ShouldBe(allKeys.Length);
        File.Exists(state.Association.PluginPath).ShouldBeFalse();
        AssertArtifactsUnchanged(sourceBytes);
    }

    /// <summary>Override edits use exact live source contexts, reject deleted or unsupported origins, and never mutate sources.</summary>
    /// <returns>A task that completes after all independent candidates are released.</returns>
    [Fact]
    public async Task BeginEdit_Override_UsesExactLiveSourceAndRejectsUnusableOrigins()
    {
        using var fixture = Fallout4NativeOutputTestFixture.Create();
        var sourceBytes = fixture.Sources.SnapshotArtifacts();
        await using var sources = await OpenSourcesAsync(fixture);
        var service = CreateService();
        await using var state = await CreateNewStateAsync(
            fixture,
            sources,
            service,
            "Overrides.esm");

        await using var winningCandidate = service.Clone(state, TestContext.Current.CancellationToken);
        var winningRequest = new BeginEditRequest(
            Guid.NewGuid(),
            sources.Revision,
            FormListEditRole.Override,
            fixture.Sources.SourceListFormKey);
        var winning = service.BeginEdit(
            sources,
            winningCandidate,
            winningRequest,
            TestContext.Current.CancellationToken);
        winning.Succeeded.ShouldBeTrue(winning.Error?.Message);
        winning.Value!.EditId.ShouldBe(winningRequest.OperationId);
        winning.Value.FormKey.ShouldBe(fixture.Sources.SourceListFormKey);
        winning.Value.OriginFormKey.ShouldBe(fixture.Sources.SourceListFormKey);
        var winningSnapshot = winningCandidate.CreateSnapshot(TestContext.Current.CancellationToken);
        var winningRecord = winningSnapshot.FormLists.ShouldHaveSingleItem();
        winningRecord.EditorID.ShouldBe("SharedListOverride");
        winningRecord.Items.Select(item => item.FormKey).ShouldBe(
            [fixture.Sources.BookFormKey, fixture.Sources.KeywordFormKey],
            ignoreOrder: false);

        var reuse = service.BeginEdit(
            sources,
            winningCandidate,
            new BeginEditRequest(
                Guid.NewGuid(),
                sources.Revision,
                FormListEditRole.Override,
                fixture.Sources.SourceListFormKey,
                new ReferenceRequest(
                    fixture.Sources.SourceListFormKey,
                    RecordScope.AllContexts,
                    fixture.Sources.SourceModKey)),
            TestContext.Current.CancellationToken);
        reuse.Succeeded.ShouldBeTrue(reuse.Error?.Message);
        var reusedRecord = winningCandidate.CreateSnapshot().FormLists.ShouldHaveSingleItem();
        reusedRecord.EditorID.ShouldBe("SharedListOverride");

        await using var exactSourceCandidate = service.Clone(state, TestContext.Current.CancellationToken);
        var exactSource = service.BeginEdit(
            sources,
            exactSourceCandidate,
            new BeginEditRequest(
                Guid.NewGuid(),
                sources.Revision,
                FormListEditRole.Override,
                fixture.Sources.SourceListFormKey,
                new ReferenceRequest(
                    fixture.Sources.SourceListFormKey,
                    RecordScope.AllContexts,
                    fixture.Sources.SourceModKey)),
            TestContext.Current.CancellationToken);
        exactSource.Succeeded.ShouldBeTrue(exactSource.Error?.Message);
        exactSourceCandidate.CreateSnapshot().FormLists.ShouldHaveSingleItem()
            .EditorID.ShouldBe("SharedListSmall");

        var deleted = service.BeginEdit(
            sources,
            exactSourceCandidate,
            new BeginEditRequest(
                Guid.NewGuid(),
                sources.Revision,
                FormListEditRole.Override,
                fixture.Sources.DeletedListFormKey),
            TestContext.Current.CancellationToken);
        deleted.Succeeded.ShouldBeFalse();
        deleted.Error!.Code.ShouldBe(EngineErrorCode.ValidationFailed);
        deleted.Error.Message.ShouldContain("deleted");

        var wrongFamily = service.BeginEdit(
            sources,
            exactSourceCandidate,
            new BeginEditRequest(
                Guid.NewGuid(),
                sources.Revision,
                FormListEditRole.Override,
                fixture.Sources.BookFormKey),
            TestContext.Current.CancellationToken);
        wrongFamily.Succeeded.ShouldBeFalse();
        wrongFamily.Error!.Code.ShouldBe(EngineErrorCode.UnsupportedOperation);

        var missingFormKey = new FormKey(fixture.Sources.SourceModKey, 0x0F01);
        var missing = service.BeginEdit(
            sources,
            exactSourceCandidate,
            new BeginEditRequest(
                Guid.NewGuid(),
                sources.Revision,
                FormListEditRole.Override,
                missingFormKey),
            TestContext.Current.CancellationToken);
        missing.Succeeded.ShouldBeFalse();
        missing.Error!.Code.ShouldBe(EngineErrorCode.RecordNotFound);
        exactSourceCandidate.CreateSnapshot().FormLists.ShouldHaveSingleItem();
        AssertArtifactsUnchanged(sourceBytes);
    }

    /// <summary>Existing-output edits select exact records including deleted records and reject missing, wrong-family, or duplicated identities.</summary>
    /// <returns>A task that completes after both valid and deliberately ambiguous outputs are released.</returns>
    [Fact]
    public async Task BeginEdit_ExistingOutput_SelectsExactFormListWithoutChangingNativeState()
    {
        using var fixture = Fallout4NativeOutputTestFixture.Create();
        var existing = fixture.WriteExistingOutput();
        var sourceBytes = fixture.Sources.SnapshotArtifacts();
        var outputBytes = File.ReadAllBytes(existing.Association.PluginPath);
        await using var sources = await OpenSourcesAsync(fixture);
        var service = CreateService();
        await using var state = await OpenExistingStateAsync(
            sources,
            service,
            existing.Association);

        AssertExistingSelection(service, sources, state, existing.OwnListFormKey);
        AssertExistingSelection(service, sources, state, existing.SourceOverrideFormKey);
        AssertExistingSelection(service, sources, state, existing.DeletedListFormKey);
        state.CreateSnapshot().FormLists.Single(record => record.FormKey == existing.DeletedListFormKey)
            .IsDeleted.ShouldBeTrue();
        state.CreateSnapshot().FormLists.Single(record => record.FormKey == existing.SourceOverrideFormKey)
            .EditorID.ShouldBe("ExistingSourceOverride");

        var reusedOverride = service.BeginEdit(
            sources,
            state,
            new BeginEditRequest(
                Guid.NewGuid(),
                sources.Revision,
                FormListEditRole.Override,
                fixture.Sources.SourceListFormKey),
            TestContext.Current.CancellationToken);
        reusedOverride.Succeeded.ShouldBeTrue(reusedOverride.Error?.Message);
        state.CreateSnapshot().FormLists.Single(record => record.FormKey == existing.SourceOverrideFormKey)
            .EditorID.ShouldBe("ExistingSourceOverride");

        var wrongFamily = service.BeginEdit(
            sources,
            state,
            new BeginEditRequest(
                Guid.NewGuid(),
                sources.Revision,
                FormListEditRole.ExistingOutput,
                targetFormKey: existing.KeywordFormKey),
            TestContext.Current.CancellationToken);
        wrongFamily.Succeeded.ShouldBeFalse();
        wrongFamily.Error!.Code.ShouldBe(EngineErrorCode.ValidationFailed);

        var missing = service.BeginEdit(
            sources,
            state,
            new BeginEditRequest(
                Guid.NewGuid(),
                sources.Revision,
                FormListEditRole.ExistingOutput,
                targetFormKey: new FormKey(existing.Association.ModKey, 0x0F01)),
            TestContext.Current.CancellationToken);
        missing.Succeeded.ShouldBeFalse();
        missing.Error!.Code.ShouldBe(EngineErrorCode.RecordNotFound);
        File.ReadAllBytes(existing.Association.PluginPath).ShouldBe(outputBytes);
        AssertArtifactsUnchanged(sourceBytes);

        var collisionOutput = fixture.WriteExistingOutput(
            "AllocatorCollision.esm",
            collidingNextFormId: true);
        await using var collisionState = await OpenExistingStateAsync(
            sources,
            service,
            collisionOutput.Association);
        var collision = service.BeginEdit(
            sources,
            collisionState,
            new BeginEditRequest(
                Guid.NewGuid(),
                sources.Revision,
                FormListEditRole.New),
            TestContext.Current.CancellationToken);
        collision.Succeeded.ShouldBeFalse();
        collision.Error!.Code.ShouldBe(EngineErrorCode.ValidationFailed);
        collision.Error.Message.ShouldContain("duplicate");
    }

    /// <summary>Asserts selection of one existing-output FormList returns its exact identity and does not assign an origin.</summary>
    /// <param name="service">The native output service under test.</param>
    /// <param name="sources">The open native source lifetime.</param>
    /// <param name="state">The complete existing output state.</param>
    /// <param name="formKey">The exact output-contained FormList identity.</param>
    private static void AssertExistingSelection(
        Fallout4NativeOutputService service,
        Fallout4NativeSourceSet sources,
        Fallout4NativeOutputState state,
        FormKey formKey)
    {
        var request = new BeginEditRequest(
            Guid.NewGuid(),
            sources.Revision,
            FormListEditRole.ExistingOutput,
            targetFormKey: formKey);
        var result = service.BeginEdit(
            sources,
            state,
            request,
            TestContext.Current.CancellationToken);
        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        result.Value!.EditId.ShouldBe(request.OperationId);
        result.Value.FormKey.ShouldBe(formKey);
        result.Value.OriginFormKey.ShouldBeNull();
        result.Value.Role.ShouldBe(FormListEditRole.ExistingOutput);
    }

    /// <summary>Creates and opens one absent output as a complete in-memory native state.</summary>
    /// <param name="fixture">The generated output fixture.</param>
    /// <param name="sources">The open native sources.</param>
    /// <param name="service">The output service under test.</param>
    /// <param name="fileName">The absent output plugin name.</param>
    /// <returns>The independently owned complete new output state.</returns>
    private static async Task<Fallout4NativeOutputState> CreateNewStateAsync(
        Fallout4NativeOutputTestFixture fixture,
        Fallout4NativeSourceSet sources,
        Fallout4NativeOutputService service,
        string fileName)
    {
        var result = await service.OpenAsync(
            sources,
            new SelectOutputRequest(
                Guid.NewGuid(),
                sources.Revision,
                OutputSelectionMode.CreateNew,
                fixture.CreateAssociation(fileName)),
            TestContext.Current.CancellationToken);
        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        return result.Value!.Output.ShouldBeOfType<Fallout4NativeOutputState>();
    }

    /// <summary>Opens one generated existing output as a complete native state.</summary>
    /// <param name="sources">The open native sources.</param>
    /// <param name="service">The output service under test.</param>
    /// <param name="association">The exact existing output association.</param>
    /// <returns>The independently owned complete existing output state.</returns>
    private static async Task<Fallout4NativeOutputState> OpenExistingStateAsync(
        Fallout4NativeSourceSet sources,
        Fallout4NativeOutputService service,
        OutputAssociation association)
    {
        var result = await service.OpenAsync(
            sources,
            new SelectOutputRequest(
                Guid.NewGuid(),
                sources.Revision,
                OutputSelectionMode.OpenExisting,
                association),
            TestContext.Current.CancellationToken);
        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        return result.Value!.Output.ShouldBeOfType<Fallout4NativeOutputState>();
    }

    /// <summary>Opens the generated Fallout 4 sources through the complete native source loader.</summary>
    /// <param name="fixture">The generated output and source fixture.</param>
    /// <returns>The independently owned native source lifetime.</returns>
    private static async Task<Fallout4NativeSourceSet> OpenSourcesAsync(
        Fallout4NativeOutputTestFixture fixture)
    {
        var open = await new Fallout4NativeSourceLoader(new NativeSourceInputLoader()).OpenAsync(
            fixture.Sources.CreateOpenRequest(),
            TestContext.Current.CancellationToken);
        open.Succeeded.ShouldBeTrue(open.Error?.Message);
        return open.Value!.Sources.ShouldBeOfType<Fallout4NativeSourceSet>();
    }

    /// <summary>Creates a Fallout 4 output service with the production admission boundary.</summary>
    /// <returns>A production output service.</returns>
    private static Fallout4NativeOutputService CreateService()
    {
        return new Fallout4NativeOutputService(new NativeOutputInputLoader());
    }

    /// <summary>Asserts every generated source plugin retains its exact bytes.</summary>
    /// <param name="expected">The source artifact bytes captured before output work.</param>
    private static void AssertArtifactsUnchanged(IReadOnlyDictionary<string, byte[]> expected)
    {
        foreach (var path in expected.Keys.OrderBy(path => path, StringComparer.OrdinalIgnoreCase))
        {
            File.ReadAllBytes(path).ShouldBe(expected[path]);
        }
    }
}
