using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeInputs;
using CreationsForge.Core.Engine.NativeOutputs;
using CreationsForge.Skyrim.Native;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Shouldly;

namespace CreationsForge.UnitTests.Engine.Skyrim;

/// <summary>
/// Verifies Skyrim native FormList target allocation and exact source and output selection inside unpublished candidates.
/// </summary>
public sealed class SkyrimNativeBeginEditTests
{
    /// <summary>
    /// Verifies native allocation advances past every occupied output record family and leaves current and physical state unchanged.
    /// </summary>
    /// <returns>A task that completes after current and candidate output states are independently released.</returns>
    [Fact]
    public async Task BeginEdit_NewAllocatesUniqueFormKeyAcrossCompleteOutput()
    {
        using var fixture = SkyrimNativeOutputTestFixture.Create();
        var sourceArtifactsBefore = fixture.Sources.SnapshotArtifacts();
        var outputArtifactsBefore = fixture.SnapshotOutputArtifacts();
        var sources = await OpenSourcesAsync(fixture.Sources);
        var service = new SkyrimNativeOutputService(new NativeOutputInputLoader());
        var output = await OpenExistingOutputAsync(service, sources, fixture);
        try
        {
            var currentKeys = output.CreateSnapshot(TestContext.Current.CancellationToken)
                .EnumerateMajorRecords().Select(record => record.FormKey).ToArray();
            var candidate = service.Clone(output, TestContext.Current.CancellationToken);
            try
            {
                var request = new BeginEditRequest(
                    Guid.NewGuid(),
                    sources.Revision.Next(),
                    FormListEditRole.New);
                var result = service.BeginEdit(
                    sources,
                    candidate,
                    request,
                    TestContext.Current.CancellationToken);

                result.Succeeded.ShouldBeTrue(result.Error?.Message);
                var identity = result.Value.ShouldNotBeNull();
                identity.EditId.ShouldBe(request.OperationId);
                identity.Role.ShouldBe(FormListEditRole.New);
                identity.OriginFormKey.ShouldBeNull();
                identity.FormKey.ModKey.ShouldBe(fixture.ExistingOutputModKey);
                currentKeys.ShouldNotContain(identity.FormKey);
                result.WorkspaceId.ShouldBe(sources.WorkspaceId);
                result.OperationId.ShouldBe(request.OperationId);
                result.BaseRevision.ShouldBe(request.ExpectedRevision);
                result.ResultRevision.ShouldBe(request.ExpectedRevision);

                var candidateRecords = candidate.CreateSnapshot(TestContext.Current.CancellationToken)
                    .EnumerateMajorRecords().ToArray();
                candidateRecords.Count(record => record.FormKey == identity.FormKey).ShouldBe(1);
                candidateRecords.Select(record => record.FormKey).Distinct().Count().ShouldBe(candidateRecords.Length);
                candidateRecords.ShouldContain(record => record.FormKey == fixture.OutputKeywordFormKey);
                candidateRecords.ShouldContain(record => record.FormKey == fixture.OutputBookFormKey);
                output.CreateSnapshot(TestContext.Current.CancellationToken)
                    .EnumerateMajorRecords().ShouldNotContain(record => record.FormKey == identity.FormKey);
            }
            finally
            {
                await candidate.DisposeAsync();
            }
        }
        finally
        {
            await output.DisposeAsync();
            await sources.DisposeAsync();
        }

        AssertArtifactsEqual(sourceArtifactsBefore, fixture.Sources.SnapshotArtifacts());
        AssertArtifactsEqual(outputArtifactsBefore, fixture.SnapshotOutputArtifacts());
    }

    /// <summary>
    /// Verifies exact source selection differs from winning selection and a deleted winner is rejected without fallback.
    /// </summary>
    /// <returns>A task that completes after each independent candidate is released.</returns>
    [Fact]
    public async Task BeginEdit_OverrideUsesExactOrWinningSourceAndRejectsDeletedWinner()
    {
        using var fixture = SkyrimNativeOutputTestFixture.Create();
        var sources = await OpenSourcesAsync(fixture.Sources);
        var service = new SkyrimNativeOutputService(new NativeOutputInputLoader());
        var output = await OpenNewOutputAsync(service, sources, fixture, "OverrideTargets.esp");
        var sourceBefore = ReadSourceFormList(
            sources,
            new ReferenceRequest(
                fixture.Sources.SourceListFormKey,
                RecordScope.AllContexts,
                fixture.Sources.SourceModKey));
        try
        {
            var exactCandidate = service.Clone(output, TestContext.Current.CancellationToken);
            try
            {
                var exact = service.BeginEdit(
                    sources,
                    exactCandidate,
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

                exact.Succeeded.ShouldBeTrue(exact.Error?.Message);
                exact.Value!.OriginFormKey.ShouldBe(fixture.Sources.SourceListFormKey);
                exactCandidate.CreateSnapshot(TestContext.Current.CancellationToken)
                    .FormLists.Single(record => record.FormKey == fixture.Sources.SourceListFormKey)
                    .EditorID.ShouldBe("SharedListSmall");
            }
            finally
            {
                await exactCandidate.DisposeAsync();
            }

            var winnerCandidate = service.Clone(output, TestContext.Current.CancellationToken);
            try
            {
                var winner = service.BeginEdit(
                    sources,
                    winnerCandidate,
                    new BeginEditRequest(
                        Guid.NewGuid(),
                        sources.Revision,
                        FormListEditRole.Override,
                        fixture.Sources.SourceListFormKey),
                    TestContext.Current.CancellationToken);

                winner.Succeeded.ShouldBeTrue(winner.Error?.Message);
                winnerCandidate.CreateSnapshot(TestContext.Current.CancellationToken)
                    .FormLists.Single(record => record.FormKey == fixture.Sources.SourceListFormKey)
                    .EditorID.ShouldBe("SharedListOverride");
            }
            finally
            {
                await winnerCandidate.DisposeAsync();
            }

            var deletedCandidate = service.Clone(output, TestContext.Current.CancellationToken);
            try
            {
                var deleted = service.BeginEdit(
                    sources,
                    deletedCandidate,
                    new BeginEditRequest(
                        Guid.NewGuid(),
                        sources.Revision,
                        FormListEditRole.Override,
                        fixture.Sources.DeletedListFormKey),
                    TestContext.Current.CancellationToken);

                deleted.Succeeded.ShouldBeFalse();
                deleted.Error!.Code.ShouldBe(EngineErrorCode.ValidationFailed);
                deleted.Error.Message.ShouldContain("deleted", Case.Insensitive);
                deletedCandidate.CreateSnapshot(TestContext.Current.CancellationToken)
                    .FormLists.ShouldNotContain(record => record.FormKey == fixture.Sources.DeletedListFormKey);
            }
            finally
            {
                await deletedCandidate.DisposeAsync();
            }

            var sourceAfter = ReadSourceFormList(
                sources,
                new ReferenceRequest(
                    fixture.Sources.SourceListFormKey,
                    RecordScope.AllContexts,
                    fixture.Sources.SourceModKey));
            service.Inspector.Compare(
                sourceBefore,
                sourceAfter,
                TestContext.Current.CancellationToken).ShouldBeEmpty();
        }
        finally
        {
            await output.DisposeAsync();
            await sources.DisposeAsync();
        }
    }

    /// <summary>
    /// Verifies an existing contained override is reused without resetting staged fields and unusable source families are typed failures.
    /// </summary>
    /// <returns>A task that completes after each transactional candidate is released.</returns>
    [Fact]
    public async Task BeginEdit_OverrideReusesContainedRecordAndRejectsWrongOrMissingFamily()
    {
        using var fixture = SkyrimNativeOutputTestFixture.Create();
        var sources = await OpenSourcesAsync(fixture.Sources);
        var service = new SkyrimNativeOutputService(new NativeOutputInputLoader());
        var output = await OpenExistingOutputAsync(service, sources, fixture);
        try
        {
            var reuseCandidate = service.Clone(output, TestContext.Current.CancellationToken);
            try
            {
                var result = service.BeginEdit(
                    sources,
                    reuseCandidate,
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

                result.Succeeded.ShouldBeTrue(result.Error?.Message);
                var matching = reuseCandidate.CreateSnapshot(TestContext.Current.CancellationToken)
                    .FormLists.Where(record => record.FormKey == fixture.Sources.SourceListFormKey).ToArray();
                matching.Length.ShouldBe(1);
                matching[0].EditorID.ShouldBe("ExistingStagedOverride");
                matching[0].FormVersion.ShouldBe((ushort)43);
            }
            finally
            {
                await reuseCandidate.DisposeAsync();
            }

            var wrongFamilyCandidate = service.Clone(output, TestContext.Current.CancellationToken);
            try
            {
                var wrongFamily = service.BeginEdit(
                    sources,
                    wrongFamilyCandidate,
                    new BeginEditRequest(
                        Guid.NewGuid(),
                        sources.Revision,
                        FormListEditRole.Override,
                        fixture.Sources.BookFormKey,
                        new ReferenceRequest(
                            fixture.Sources.BookFormKey,
                            RecordScope.AllContexts,
                            fixture.Sources.SourceModKey)),
                    TestContext.Current.CancellationToken);
                wrongFamily.Succeeded.ShouldBeFalse();
                wrongFamily.Error!.Code.ShouldBe(EngineErrorCode.ValidationFailed);
            }
            finally
            {
                await wrongFamilyCandidate.DisposeAsync();
            }

            var missingCandidate = service.Clone(output, TestContext.Current.CancellationToken);
            try
            {
                var missing = service.BeginEdit(
                    sources,
                    missingCandidate,
                    new BeginEditRequest(
                        Guid.NewGuid(),
                        sources.Revision,
                        FormListEditRole.Override,
                        fixture.Sources.MissingFormKey),
                    TestContext.Current.CancellationToken);
                missing.Succeeded.ShouldBeFalse();
                missing.Error!.Code.ShouldBe(EngineErrorCode.RecordNotFound);
            }
            finally
            {
                await missingCandidate.DisposeAsync();
            }
        }
        finally
        {
            await output.DisposeAsync();
            await sources.DisposeAsync();
        }
    }

    /// <summary>
    /// Verifies exact output-owned FormLists, including deleted records, can be selected without allocation or implicit undelete.
    /// </summary>
    /// <returns>A task that completes after selection, rejection, cancellation, and disposal checks.</returns>
    [Fact]
    public async Task BeginEdit_ExistingOutputSelectsExactDeletedTargetWithoutMutation()
    {
        using var fixture = SkyrimNativeOutputTestFixture.Create();
        var sources = await OpenSourcesAsync(fixture.Sources);
        var service = new SkyrimNativeOutputService(new NativeOutputInputLoader());
        var output = await OpenExistingOutputAsync(service, sources, fixture);
        try
        {
            var candidate = service.Clone(output, TestContext.Current.CancellationToken);
            try
            {
                var result = service.BeginEdit(
                    sources,
                    candidate,
                    new BeginEditRequest(
                        Guid.NewGuid(),
                        sources.Revision,
                        FormListEditRole.ExistingOutput,
                        targetFormKey: fixture.OutputDeletedListFormKey),
                    TestContext.Current.CancellationToken);
                result.Succeeded.ShouldBeTrue(result.Error?.Message);
                result.Value!.Role.ShouldBe(FormListEditRole.ExistingOutput);
                result.Value.OriginFormKey.ShouldBeNull();
                result.Value.FormKey.ShouldBe(fixture.OutputDeletedListFormKey);
                candidate.CreateSnapshot(TestContext.Current.CancellationToken)
                    .FormLists.Single(record => record.FormKey == fixture.OutputDeletedListFormKey)
                    .IsDeleted.ShouldBeTrue();

                var wrongFamily = service.BeginEdit(
                    sources,
                    candidate,
                    new BeginEditRequest(
                        Guid.NewGuid(),
                        sources.Revision,
                        FormListEditRole.ExistingOutput,
                        targetFormKey: fixture.OutputBookFormKey),
                    TestContext.Current.CancellationToken);
                wrongFamily.Succeeded.ShouldBeFalse();
                wrongFamily.Error!.Code.ShouldBe(EngineErrorCode.ValidationFailed);

                var missing = service.BeginEdit(
                    sources,
                    candidate,
                    new BeginEditRequest(
                        Guid.NewGuid(),
                        sources.Revision,
                        FormListEditRole.ExistingOutput,
                        targetFormKey: new FormKey(fixture.ExistingOutputModKey, 0x0FFF)),
                    TestContext.Current.CancellationToken);
                missing.Succeeded.ShouldBeFalse();
                missing.Error!.Code.ShouldBe(EngineErrorCode.RecordNotFound);

                using var cancellationSource = new CancellationTokenSource();
                cancellationSource.Cancel();
                Should.Throw<OperationCanceledException>(() => service.BeginEdit(
                    sources,
                    candidate,
                    new BeginEditRequest(
                        Guid.NewGuid(),
                        sources.Revision,
                        FormListEditRole.ExistingOutput,
                        targetFormKey: fixture.OutputOwnListFormKey),
                    cancellationSource.Token));
            }
            finally
            {
                await candidate.DisposeAsync();
            }
        }
        finally
        {
            await output.DisposeAsync();
            await sources.DisposeAsync();
        }
    }

    /// <summary>Reads one resolved detached Skyrim FormList or fails the current test with precise context.</summary>
    /// <param name="sources">The materialized native source set.</param>
    /// <param name="selection">The exact source selection.</param>
    /// <returns>The detached source FormList getter.</returns>
    private static IFormListGetter ReadSourceFormList(
        SkyrimNativeSourceSet sources,
        ReferenceRequest selection)
    {
        var result = sources.ReadFormListContext(selection, TestContext.Current.CancellationToken);
        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        result.Value!.Context.Status.ShouldBe(ReferenceResolutionStatus.Resolved);
        return result.Value.Record.ShouldBeAssignableTo<IFormListGetter>()!;
    }

    /// <summary>Opens the complete explicit Skyrim source set.</summary>
    /// <param name="fixture">The generated source fixture.</param>
    /// <returns>The independently disposable materialized source set.</returns>
    private static async Task<SkyrimNativeSourceSet> OpenSourcesAsync(SkyrimNativeTestFixture fixture)
    {
        var result = await new SkyrimNativeSourceLoader(new NativeSourceInputLoader()).OpenAsync(
            fixture.CreateOpenRequest(),
            TestContext.Current.CancellationToken);
        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        return result.Value!.Sources.ShouldBeOfType<SkyrimNativeSourceSet>();
    }

    /// <summary>Opens the fixture's complete localized existing output.</summary>
    /// <param name="service">The Skyrim output service.</param>
    /// <param name="sources">The materialized native source set.</param>
    /// <param name="fixture">The generated output fixture.</param>
    /// <returns>The independently disposable materialized output state.</returns>
    private static async Task<SkyrimNativeOutputState> OpenExistingOutputAsync(
        SkyrimNativeOutputService service,
        SkyrimNativeSourceSet sources,
        SkyrimNativeOutputTestFixture fixture)
    {
        var result = await service.OpenAsync(
            sources,
            new SelectOutputRequest(
                Guid.NewGuid(),
                sources.Revision,
                OutputSelectionMode.OpenExisting,
                fixture.CreateExistingAssociation()),
            TestContext.Current.CancellationToken);
        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        return result.Value!.Output.ShouldBeOfType<SkyrimNativeOutputState>();
    }

    /// <summary>Opens one absent embedded-string output entirely in memory.</summary>
    /// <param name="service">The Skyrim output service.</param>
    /// <param name="sources">The materialized native source set.</param>
    /// <param name="fixture">The generated output fixture.</param>
    /// <param name="fileName">The absent output plugin name.</param>
    /// <returns>The independently disposable empty output state.</returns>
    private static async Task<SkyrimNativeOutputState> OpenNewOutputAsync(
        SkyrimNativeOutputService service,
        SkyrimNativeSourceSet sources,
        SkyrimNativeOutputTestFixture fixture,
        string fileName)
    {
        var result = await service.OpenAsync(
            sources,
            new SelectOutputRequest(
                Guid.NewGuid(),
                sources.Revision,
                OutputSelectionMode.CreateNew,
                fixture.CreateNewAssociation(fileName)),
            TestContext.Current.CancellationToken);
        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        return result.Value!.Output.ShouldBeOfType<SkyrimNativeOutputState>();
    }

    /// <summary>Asserts two canonical artifact snapshots have identical paths and bytes.</summary>
    /// <param name="expected">The prior exact snapshot.</param>
    /// <param name="actual">The resulting exact snapshot.</param>
    private static void AssertArtifactsEqual(
        IReadOnlyDictionary<string, byte[]> expected,
        IReadOnlyDictionary<string, byte[]> actual)
    {
        actual.Keys.ShouldBe(expected.Keys, ignoreOrder: false);
        foreach (var artifact in expected)
        {
            actual[artifact.Key].ShouldBe(artifact.Value);
        }
    }
}
