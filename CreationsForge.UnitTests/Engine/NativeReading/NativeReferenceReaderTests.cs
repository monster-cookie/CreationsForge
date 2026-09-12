using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeReading;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Starfield;
using Shouldly;

namespace CreationsForge.UnitTests.Engine.NativeReading;

/// <summary>
/// Verifies shared direct native-group lookup, provenance, paging, copy isolation, and cancellation behavior.
/// </summary>
public sealed class NativeReferenceReaderTests
{
    /// <summary>Verifies exact FormKey lookup separates origin identity from the containing override plugin.</summary>
    [Fact]
    public void Resolve_SelectsWinningOverrideWithoutNumericFormIdCollision()
    {
        var source = new StarfieldMod("Origin.esm", StarfieldRelease.Starfield);
        var collision = new StarfieldMod("Collision.esm", StarfieldRelease.Starfield);
        var patch = new StarfieldMod("Patch.esp", StarfieldRelease.Starfield);
        var originKey = new FormKey(source.ModKey, 0x800);
        var collisionKey = new FormKey(collision.ModKey, originKey.ID);
        source.FormLists.Add(new FormList(originKey, StarfieldRelease.Starfield)
        {
            EditorID = "OriginList",
        });
        collision.FormLists.Add(new FormList(collisionKey, StarfieldRelease.Starfield)
        {
            EditorID = "NumericCollision",
        });
        patch.FormLists.Add(new FormList(originKey, StarfieldRelease.Starfield)
        {
            EditorID = "WinningOverride",
        });

        var reader = CreateReader(source, collision, patch);
        var result = reader.Resolve(new ReferenceRequest(originKey, RecordScope.WinningOverrides));

        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        result.Value!.Status.ShouldBe(ReferenceResolutionStatus.Resolved);
        result.Value.FormKey.ShouldBe(originKey);
        result.Value.Record!.FormKey.ShouldBe(originKey);
        result.Value.Record.EditorID.ShouldBe("WinningOverride");
        result.Value.ContainingModKey.ShouldBe(patch.ModKey);
        result.Value.LoadOrderIndex.ShouldBe(2);
        result.Value.Role.ShouldBe(PluginRole.LoadOrder);
    }

    /// <summary>Verifies multi-context exact reads require explicit disambiguation and preserve selected provenance.</summary>
    [Fact]
    public void Resolve_AllContextsRequiresContainingPluginWhenOverridesExist()
    {
        var source = new StarfieldMod("Origin.esm", StarfieldRelease.Starfield);
        var patch = new StarfieldMod("Patch.esp", StarfieldRelease.Starfield);
        var formKey = new FormKey(source.ModKey, 0x800);
        source.FormLists.Add(new FormList(formKey, StarfieldRelease.Starfield)
        {
            EditorID = "OriginList",
        });
        patch.FormLists.Add(new FormList(formKey, StarfieldRelease.Starfield)
        {
            EditorID = "OverrideList",
        });
        var reader = CreateReader(source, patch);

        var ambiguous = reader.Resolve(new ReferenceRequest(formKey, RecordScope.AllContexts));
        var selected = reader.Resolve(new ReferenceRequest(formKey, RecordScope.AllContexts, source.ModKey));

        ambiguous.Value!.Status.ShouldBe(ReferenceResolutionStatus.Ambiguous);
        ambiguous.Value.Record.ShouldBeNull();
        selected.Value!.Status.ShouldBe(ReferenceResolutionStatus.Resolved);
        selected.Value.Record!.EditorID.ShouldBe("OriginList");
        selected.Value.ContainingModKey.ShouldBe(source.ModKey);
        selected.Value.SourcePath.ShouldBe("C:\\Native\\0-Origin.esm");
    }

    /// <summary>Verifies resolved records retain their concrete family and ordered native fields while remaining detached.</summary>
    [Fact]
    public void Resolve_ReturnsDetachedNativeSubtypeAndOrderedFields()
    {
        var source = new StarfieldMod("Origin.esm", StarfieldRelease.Starfield);
        var listKey = new FormKey(source.ModKey, 0x800);
        var bookKey = new FormKey(source.ModKey, 0x801);
        var keywordKey = new FormKey(source.ModKey, 0x802);
        var sourceList = new FormList(listKey, StarfieldRelease.Starfield)
        {
            EditorID = "OrderedList",
        };
        sourceList.Items.Add(new FormLink<IStarfieldMajorRecordGetter>(bookKey));
        sourceList.Items.Add(new FormLink<IStarfieldMajorRecordGetter>(keywordKey));
        source.FormLists.Add(sourceList);
        source.Books.Add(new Book(bookKey, StarfieldRelease.Starfield)
        {
            EditorID = "CrossFamilyBook",
        });
        source.Keywords.Add(new Keyword(keywordKey, StarfieldRelease.Starfield)
        {
            EditorID = "CrossFamilyKeyword",
        });
        var reader = CreateReader(source);

        var listResult = reader.Resolve(new ReferenceRequest(listKey, RecordScope.Source));
        var bookResult = reader.Resolve(new ReferenceRequest(bookKey, RecordScope.Source));
        var detached = listResult.Value!.Record.ShouldBeOfType<FormList>();

        detached.ShouldNotBeSameAs(sourceList);
        detached.Items.Select(item => item.FormKey)
            .ShouldBe(new[] { bookKey, keywordKey });
        bookResult.Value!.Record.ShouldBeAssignableTo<IBookGetter>();
        detached.EditorID = "MutatedCopy";
        detached.Items.Clear();
        sourceList.EditorID.ShouldBe("OrderedList");
        sourceList.Items.Select(item => item.FormKey)
            .ShouldBe(new[] { bookKey, keywordKey });
    }

    /// <summary>Verifies a deleted winner is reported without falling back to an older live context.</summary>
    [Fact]
    public void Resolve_DeletedWinnerReturnsTypedDeletedStatus()
    {
        var source = new StarfieldMod("Origin.esm", StarfieldRelease.Starfield);
        var patch = new StarfieldMod("Patch.esp", StarfieldRelease.Starfield);
        var formKey = new FormKey(source.ModKey, 0x800);
        source.FormLists.Add(new FormList(formKey, StarfieldRelease.Starfield)
        {
            EditorID = "LiveOrigin",
        });
        patch.FormLists.Add(new FormList(formKey, StarfieldRelease.Starfield)
        {
            EditorID = "DeletedOverride",
            IsDeleted = true,
        });

        var result = CreateReader(source, patch).Resolve(
            new ReferenceRequest(formKey, RecordScope.WinningOverrides));

        result.Value!.Status.ShouldBe(ReferenceResolutionStatus.Deleted);
        result.Value.Record.ShouldBeNull();
        result.Value.ContainingModKey.ShouldBe(patch.ModKey);
        result.Value.RecordType.ShouldBe("FormList");
    }

    /// <summary>Verifies bounded pages preserve every context exactly once in deterministic native order.</summary>
    [Fact]
    public void Search_AllContextsPagesWithoutDuplicatesOrSkips()
    {
        var source = new StarfieldMod("Origin.esm", StarfieldRelease.Starfield);
        var patch = new StarfieldMod("Patch.esp", StarfieldRelease.Starfield);
        var sharedKey = new FormKey(source.ModKey, 0x800);
        source.FormLists.Add(new FormList(sharedKey, StarfieldRelease.Starfield) { EditorID = "NeedleOrigin" });
        source.Books.Add(new Book(new FormKey(source.ModKey, 0x801), StarfieldRelease.Starfield) { EditorID = "NeedleBook" });
        source.Keywords.Add(new Keyword(new FormKey(source.ModKey, 0x802), StarfieldRelease.Starfield) { EditorID = "NeedleKeyword" });
        patch.FormLists.Add(new FormList(sharedKey, StarfieldRelease.Starfield) { EditorID = "NeedleOverride" });
        patch.Books.Add(new Book(new FormKey(patch.ModKey, 0x800), StarfieldRelease.Starfield) { EditorID = "NeedleOtherOrigin" });
        var reader = CreateReader(source, patch);
        var workspaceId = Guid.NewGuid();
        var revision = new WorkspaceRevision(Guid.NewGuid(), 0);
        var matches = new List<ReferenceSearchMatch>();
        string? token = null;

        do
        {
            var page = reader.Search(
                new ReferenceSearchRequest("Needle", 2, token, RecordScope.AllContexts),
                workspaceId,
                revision);
            page.Succeeded.ShouldBeTrue(page.Error?.Message);
            matches.AddRange(page.Value!.Matches);
            token = page.Value.ContinuationToken;
        }
        while (token is not null);

        matches.Count.ShouldBe(5);
        matches.Select(MatchIdentity).Distinct(StringComparer.Ordinal).Count().ShouldBe(5);
        matches.Select(match => match.ContainingModKey)
            .ShouldBe(new ModKey?[] { source.ModKey, source.ModKey, source.ModKey, patch.ModKey, patch.ModKey });
        matches.ShouldAllBe(match => !string.IsNullOrWhiteSpace(match.SourcePath));

        var winning = reader.Search(
            new ReferenceSearchRequest("Needle", 10),
            workspaceId,
            revision);
        winning.Value!.Matches.Count.ShouldBe(4);
        winning.Value.Matches.ShouldContain(match =>
            match.FormKey == sharedKey &&
            match.ContainingModKey == patch.ModKey &&
            match.EditorId == "NeedleOverride");
        winning.Value.Matches.ShouldNotContain(match =>
            match.FormKey == sharedKey &&
            match.ContainingModKey == source.ModKey);
    }

    /// <summary>Verifies cursor integrity binds workspace, revision, query, scope, filter, and page size.</summary>
    [Fact]
    public void Search_RejectsContinuationTokenBindingMismatch()
    {
        var source = new StarfieldMod("Origin.esm", StarfieldRelease.Starfield);
        source.FormLists.Add(new FormList(new FormKey(source.ModKey, 0x800), StarfieldRelease.Starfield) { EditorID = "NeedleOne" });
        source.FormLists.Add(new FormList(new FormKey(source.ModKey, 0x801), StarfieldRelease.Starfield) { EditorID = "NeedleTwo" });
        var reader = CreateReader(source);
        var workspaceId = Guid.NewGuid();
        var revision = new WorkspaceRevision(Guid.NewGuid(), 3);
        var first = reader.Search(
            new ReferenceSearchRequest("Needle", 1, scope: RecordScope.Source),
            workspaceId,
            revision);
        var token = first.Value!.ContinuationToken.ShouldNotBeNull();

        var changedQuery = reader.Search(
            new ReferenceSearchRequest("Two", 1, token, RecordScope.Source),
            workspaceId,
            revision);
        var changedWorkspace = reader.Search(
            new ReferenceSearchRequest("Needle", 1, token, RecordScope.Source),
            Guid.NewGuid(),
            revision);
        var changedRevision = reader.Search(
            new ReferenceSearchRequest("Needle", 1, token, RecordScope.Source),
            workspaceId,
            revision.Next());
        var changedBaseline = reader.Search(
            new ReferenceSearchRequest("Needle", 1, token, RecordScope.Source),
            workspaceId,
            new WorkspaceRevision(Guid.NewGuid(), revision.Sequence));
        var changedScope = reader.Search(
            new ReferenceSearchRequest("Needle", 1, token, RecordScope.AllContexts),
            workspaceId,
            revision);
        var changedFilter = reader.Search(
            new ReferenceSearchRequest("Needle", 1, token, RecordScope.Source, source.ModKey),
            workspaceId,
            revision);
        var changedPageSize = reader.Search(
            new ReferenceSearchRequest("Needle", 2, token, RecordScope.Source),
            workspaceId,
            revision);
        var tokenCharacters = token.ToCharArray();
        tokenCharacters[^1] = tokenCharacters[^1] == 'A' ? 'B' : 'A';
        var nonCanonical = reader.Search(
            new ReferenceSearchRequest("Needle", 1, new string(tokenCharacters), RecordScope.Source),
            workspaceId,
            revision);
        tokenCharacters = token.ToCharArray();
        var middleIndex = tokenCharacters.Length / 2;
        tokenCharacters[middleIndex] = tokenCharacters[middleIndex] == 'A' ? 'B' : 'A';
        var corrupted = reader.Search(
            new ReferenceSearchRequest("Needle", 1, new string(tokenCharacters), RecordScope.Source),
            workspaceId,
            revision);

        changedQuery.Error!.Code.ShouldBe(EngineErrorCode.InvalidRequest);
        changedWorkspace.Error!.Code.ShouldBe(EngineErrorCode.InvalidRequest);
        changedRevision.Error!.Code.ShouldBe(EngineErrorCode.InvalidRequest);
        changedBaseline.Error!.Code.ShouldBe(EngineErrorCode.InvalidRequest);
        changedScope.Error!.Code.ShouldBe(EngineErrorCode.InvalidRequest);
        changedFilter.Error!.Code.ShouldBe(EngineErrorCode.InvalidRequest);
        changedPageSize.Error!.Code.ShouldBe(EngineErrorCode.InvalidRequest);
        nonCanonical.Error!.Code.ShouldBe(EngineErrorCode.InvalidRequest);
        corrupted.Error!.Code.ShouldBe(EngineErrorCode.InvalidRequest);
    }

    /// <summary>Verifies maximum-length Unicode cursor fields remain decodable across deterministic pages.</summary>
    [Fact]
    public void Search_LongUnicodeQueryAndContainingPluginAdvancesPage()
    {
        var query = new string('\u754c', ReferenceSearchRequest.MaximumQueryLength);
        var source = new StarfieldMod($"{new string('\u754c', 250)}.esm", StarfieldRelease.Starfield);
        source.FormLists.Add(new FormList(new FormKey(source.ModKey, 0x800), StarfieldRelease.Starfield)
        {
            EditorID = $"{query}One",
        });
        source.FormLists.Add(new FormList(new FormKey(source.ModKey, 0x801), StarfieldRelease.Starfield)
        {
            EditorID = $"{query}Two",
        });
        var reader = CreateReader(source);
        var workspaceId = Guid.NewGuid();
        var revision = new WorkspaceRevision(Guid.NewGuid(), 5);

        var first = reader.Search(
            new ReferenceSearchRequest(
                query,
                1,
                scope: RecordScope.Source,
                containingModKey: source.ModKey),
            workspaceId,
            revision);
        var token = first.Value!.ContinuationToken.ShouldNotBeNull();
        var second = reader.Search(
            new ReferenceSearchRequest(
                query,
                1,
                token,
                RecordScope.Source,
                source.ModKey),
            workspaceId,
            revision);

        token.Length.ShouldBeGreaterThan(2048);
        first.Value.Matches.Single().EditorId.ShouldBe($"{query}One");
        second.Succeeded.ShouldBeTrue(second.Error?.Message);
        second.Value!.Matches.Single().EditorId.ShouldBe($"{query}Two");
        second.Value.ContinuationToken.ShouldBeNull();
    }

    /// <summary>Verifies invalid scopes and unbounded page sizes fail at the request boundary.</summary>
    [Fact]
    public void Requests_RejectUndefinedScopeAndUnboundedPageSize()
    {
        Should.Throw<ArgumentOutOfRangeException>(() => new ReferenceRequest(
            FormKey.Null,
            (RecordScope)999));
        Should.Throw<ArgumentOutOfRangeException>(() => new ReferenceSearchRequest(
            "query",
            ReferenceSearchRequest.MaximumPageSize + 1));
        Should.Throw<ArgumentException>(() => new ReferenceSearchRequest(
            "query",
            1,
            scope: RecordScope.WinningOverrides,
            containingModKey: new ModKey("Origin", ModType.Master)));
    }

    /// <summary>Verifies cancellation is observed after a supported native deep-copy boundary.</summary>
    [Fact]
    public void Resolve_CancellationDuringCopyDoesNotPublishRecord()
    {
        var source = new StarfieldMod("Origin.esm", StarfieldRelease.Starfield);
        var formKey = new FormKey(source.ModKey, 0x800);
        source.FormLists.Add(new FormList(formKey, StarfieldRelease.Starfield) { EditorID = "CopyTarget" });
        using var cancellationSource = new CancellationTokenSource();
        var nativeSource = new NativeReferenceSource(
            source,
            "C:\\Native\\Origin.esm",
            0,
            PluginRole.Source,
            record =>
            {
                var copy = record.DeepCopy();
                cancellationSource.Cancel();
                return copy;
            });
        var reader = new NativeReferenceReader(new[] { nativeSource });

        Should.Throw<OperationCanceledException>(() => reader.Resolve(
            new ReferenceRequest(formKey, RecordScope.Source),
            cancellationSource.Token));
        source.FormLists[formKey].EditorID.ShouldBe("CopyTarget");
    }

    /// <summary>Verifies contextual reads retain a detached deleted getter while ordinary resolution suppresses it.</summary>
    [Fact]
    public void Read_DeletedWinnerRetainsGetterWhileResolveSuppressesIt()
    {
        var source = new StarfieldMod("Origin.esm", StarfieldRelease.Starfield);
        var patch = new StarfieldMod("Patch.esp", StarfieldRelease.Starfield);
        var formKey = new FormKey(source.ModKey, 0x800);
        source.FormLists.Add(new FormList(formKey, StarfieldRelease.Starfield) { EditorID = "Origin" });
        var deleted = new FormList(formKey, StarfieldRelease.Starfield)
        {
            EditorID = "DeletedOverride",
            IsDeleted = true,
        };
        patch.FormLists.Add(deleted);
        var reader = CreateReader(source, patch);
        var request = new ReferenceRequest(formKey, RecordScope.WinningOverrides);

        var read = reader.Read(request);
        var resolution = reader.Resolve(request);

        read.Succeeded.ShouldBeTrue(read.Error?.Message);
        read.Value!.Context.Status.ShouldBe(ReferenceResolutionStatus.Deleted);
        read.Value.Context.ContainingModKey.ShouldBe(patch.ModKey);
        read.Value.Record.ShouldNotBeNull();
        read.Value.Record.ShouldNotBeSameAs(deleted);
        read.Value.Record.IsDeleted.ShouldBeTrue();
        resolution.Value!.Status.ShouldBe(ReferenceResolutionStatus.Deleted);
        resolution.Value.Record.ShouldBeNull();
    }

    /// <summary>Verifies contextual reads report missing direct links with a game-supplied typed field path.</summary>
    [Fact]
    public void Read_MissingDirectReferenceReportsTypedPathWarning()
    {
        var source = new StarfieldMod("Origin.esm", StarfieldRelease.Starfield);
        var listKey = new FormKey(source.ModKey, 0x800);
        var missingKey = new FormKey(source.ModKey, 0x801);
        source.FormLists.Add(new FormList(listKey, StarfieldRelease.Starfield)
        {
            EditorID = "MissingLinkList",
        });
        var nativeSource = new NativeReferenceSource(
            source,
            "C:\\Native\\Origin.esm",
            0,
            PluginRole.Source);
        var reader = new NativeReferenceReader(
            new[] { nativeSource },
            _ => new[] { new NativeFormLinkReference(missingKey, "Items[0]") });

        var result = reader.Read(new ReferenceRequest(listKey, RecordScope.Source));

        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        result.Warnings.Single().Code.ShouldBe("missing-native-reference");
        result.Warnings.Single().Message.ShouldContain(missingKey.ToString());
        result.Warnings.Single().Message.ShouldContain("Items[0]");
    }

    /// <summary>Verifies the public fallback read retains native null links while warning only for real unresolved targets at their original position.</summary>
    [Fact]
    public void Read_DefaultDirectLinksSkipsNativeNullSentinelAndRetainsOriginalWarningPosition()
    {
        var source = new StarfieldMod("Origin.esm", StarfieldRelease.Starfield);
        var listKey = new FormKey(source.ModKey, 0x800);
        var bookKey = new FormKey(source.ModKey, 0x801);
        var missingKey = new FormKey(source.ModKey, 0x802);
        var list = new FormList(listKey, StarfieldRelease.Starfield)
        {
            EditorID = "NullAndMissingLinks",
        };
        list.Items.Add(new FormLink<IStarfieldMajorRecordGetter>(bookKey));
        list.Items.Add(new FormLink<IStarfieldMajorRecordGetter>(FormKey.Null));
        list.Items.Add(new FormLink<IStarfieldMajorRecordGetter>(missingKey));
        source.FormLists.Add(list);
        source.Books.Add(new Book(bookKey, StarfieldRelease.Starfield)
        {
            EditorID = "ResolvedBook",
        });

        var result = CreateReader(source).Read(new ReferenceRequest(listKey, RecordScope.Source));

        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        result.Value!.Record.ShouldBeOfType<FormList>().Items.Select(item => item.FormKey)
            .ShouldBe([bookKey, FormKey.Null, missingKey]);
        result.Warnings.ShouldHaveSingleItem().Code.ShouldBe("missing-native-reference");
        result.Warnings[0].Message.ShouldContain(missingKey.ToString());
        result.Warnings[0].Message.ShouldContain("FormLinks[2]");
    }

    /// <summary>Verifies override counts include every non-origin context even when the origin context is absent.</summary>
    [Fact]
    public void CountOverrides_UsesContainingPluginRatherThanContextsMinusOne()
    {
        var origin = new StarfieldMod("Origin.esm", StarfieldRelease.Starfield);
        var firstPatch = new StarfieldMod("FirstPatch.esp", StarfieldRelease.Starfield);
        var secondPatch = new StarfieldMod("SecondPatch.esp", StarfieldRelease.Starfield);
        var formKey = new FormKey(origin.ModKey, 0x800);
        firstPatch.FormLists.Add(new FormList(formKey, StarfieldRelease.Starfield) { EditorID = "First" });
        secondPatch.FormLists.Add(new FormList(formKey, StarfieldRelease.Starfield)
        {
            EditorID = "DeletedSecond",
            IsDeleted = true,
        });

        var count = CreateReader(origin, firstPatch, secondPatch).CountOverrides(
            formKey,
            typeof(IFormListGetter));

        count.ShouldBe(2);
    }

    /// <summary>Creates shared native sources from generated in-memory Starfield mods.</summary>
    /// <param name="mods">The native mods in explicit load-order order.</param>
    /// <returns>A shared reader that borrows the supplied native mods.</returns>
    private static NativeReferenceReader CreateReader(params StarfieldMod[] mods)
    {
        return new NativeReferenceReader(mods
            .Select((mod, index) => new NativeReferenceSource(
                mod,
                $"C:\\Native\\{index}-{mod.ModKey.FileName}",
                index,
                index == 0 ? PluginRole.Source : PluginRole.LoadOrder))
            .ToArray());
    }

    /// <summary>Creates a stable composite identity for one origin and containing-plugin context.</summary>
    /// <param name="match">The native reference match.</param>
    /// <returns>The composite context identity.</returns>
    private static string MatchIdentity(ReferenceSearchMatch match)
    {
        return $"{match.FormKey}|{match.ContainingModKey}|{match.RecordType}|{match.EditorId}";
    }
}
