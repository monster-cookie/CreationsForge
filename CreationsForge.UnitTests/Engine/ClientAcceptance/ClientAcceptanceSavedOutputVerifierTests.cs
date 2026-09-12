using System.IO.Abstractions;
using System.Security.Cryptography;
using System.Text.Json;
using CreationsForge.Bootstrap.Composition;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Enums;
using CreationsForge.Fallout4.Native.NativeInspection;
using CreationsForge.Skyrim.Native.NativeInspection;
using CreationsForge.Starfield.Native.NativeInspection;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using Fallout4Mod = Mutagen.Bethesda.Fallout4.Fallout4Mod;
using Fallout4Release = Mutagen.Bethesda.Fallout4.Fallout4Release;
using SkyrimMod = Mutagen.Bethesda.Skyrim.SkyrimMod;
using SkyrimRelease = Mutagen.Bethesda.Skyrim.SkyrimRelease;
using StarfieldMod = Mutagen.Bethesda.Starfield.StarfieldMod;
using StarfieldRelease = Mutagen.Bethesda.Starfield.StarfieldRelease;

namespace CreationsForge.UnitTests.Engine.ClientAcceptance;

/// <summary>Independently verifies output authored through a real Codex MCP client against its retained manifest.</summary>
public sealed class ClientAcceptanceSavedOutputVerifierTests
{
    /// <summary>Loads the closed retained manifest without accepting undeclared fields.</summary>
    /// <param name="acceptanceRoot">The validated retained root.</param>
    /// <param name="cancellationToken">The propagated read token.</param>
    /// <returns>The non-null manifest document.</returns>
    private static async Task<ClientAcceptanceManifest> LoadManifestAsync(string acceptanceRoot, CancellationToken cancellationToken)
    {
        var path = Path.Combine(acceptanceRoot, ClientAcceptanceFixtureExportTests.ManifestFileName);
        try
        {
            await using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            return await JsonSerializer.DeserializeAsync<ClientAcceptanceManifest>(
                stream,
                ClientAcceptanceFixtureExportTests.SerializerOptions,
                cancellationToken)
                ?? throw new InvalidDataException("The retained client-acceptance manifest contained JSON null.");
        }
        catch (JsonException exception)
        {
            throw new InvalidDataException($"The retained client-acceptance manifest is invalid: {exception.Message}", exception);
        }
    }

    /// <summary>Validates the closed three-game manifest identity, paths, and expected result bounds.</summary>
    /// <param name="manifest">The deserialized manifest.</param>
    /// <param name="acceptanceRoot">The canonical environment-selected root.</param>
    /// <exception cref="InvalidDataException">Thrown when the retained contract is incomplete or inconsistent.</exception>
    private static void ValidateManifest(ClientAcceptanceManifest manifest, string acceptanceRoot)
    {
        if (manifest.SchemaVersion != 1)
        {
            throw new InvalidDataException("Client-acceptance schemaVersion must be 1.");
        }

        RequireSamePath(manifest.AcceptanceRoot, acceptanceRoot, "acceptanceRoot");
        RequireSamePath(manifest.RepositoryRoot, ClientAcceptancePaths.FindRepositoryRoot(), "repositoryRoot");
        if (!string.Equals(manifest.Server.Command, "dotnet", StringComparison.Ordinal)
            || manifest.Server.Arguments.Length != 2
            || !string.Equals(manifest.Server.Arguments[1], "mcp", StringComparison.Ordinal)
            || !Path.IsPathFullyQualified(manifest.Server.Arguments[0]))
        {
            throw new InvalidDataException("The retained server launch must identify the built CreationsForge.Console assembly followed by the 'mcp' argument.");
        }

        ClientAcceptancePaths.RequireExistingRegularFile(manifest.Server.Arguments[0], "manifest server assembly");
        RequireSamePath(manifest.Server.WorkingDirectory, manifest.RepositoryRoot, "server.workingDirectory");
        var expectedGames = new[] { "fallout4", "skyrim", "starfield" };
        var actualGames = manifest.Cases.Select(item => item.Game).OrderBy(item => item, StringComparer.Ordinal).ToArray();
        if (!actualGames.SequenceEqual(expectedGames, StringComparer.Ordinal))
        {
            throw new InvalidDataException("The retained manifest must contain exactly one Starfield, Fallout 4, and Skyrim case.");
        }

        var workspaceIds = manifest.Cases.SelectMany(item => new[] { item.WorkspaceId, item.ReopenWorkspaceId }).ToArray();
        if (workspaceIds.Any(id => id == Guid.Empty) || workspaceIds.Distinct().Count() != workspaceIds.Length)
        {
            throw new InvalidDataException("Every first-run and reopen workspace identity must be non-empty and distinct.");
        }

        foreach (var item in manifest.Cases)
        {
            ValidateCase(item, acceptanceRoot);
        }
    }

    /// <summary>Validates one case's paths, identities, plan coverage, and bounded expected output.</summary>
    /// <param name="acceptanceCase">The retained case.</param>
    /// <param name="acceptanceRoot">The canonical retained root.</param>
    /// <exception cref="InvalidDataException">Thrown when the case is inconsistent.</exception>
    private static void ValidateCase(ClientAcceptanceCase acceptanceCase, string acceptanceRoot)
    {
        if (!string.Equals(acceptanceCase.CaseId, acceptanceCase.Game, StringComparison.Ordinal)
            || acceptanceCase.WorkspaceId != acceptanceCase.WorkspaceOpen.WorkspaceId
            || !string.Equals(acceptanceCase.Game, acceptanceCase.WorkspaceOpen.Game, StringComparison.Ordinal)
            || !string.Equals(acceptanceCase.Release, acceptanceCase.WorkspaceOpen.Release, StringComparison.Ordinal))
        {
            throw new InvalidDataException($"Case '{acceptanceCase.CaseId}' has inconsistent workspace identity or game fields.");
        }

        var expectedRelease = acceptanceCase.Game switch
        {
            "starfield" => "starfield",
            "fallout4" => "fallout4",
            "skyrim" => "skyrim_se",
            _ => throw new InvalidDataException($"Unsupported client-acceptance game '{acceptanceCase.Game}'."),
        };
        if (!string.Equals(acceptanceCase.Release, expectedRelease, StringComparison.Ordinal))
        {
            throw new InvalidDataException($"Case '{acceptanceCase.CaseId}' has release '{acceptanceCase.Release}' instead of '{expectedRelease}'.");
        }

        if (acceptanceCase.Expected.OutputFormListCount != 2
            || acceptanceCase.Expected.OverrideRecord.FormKey is null
            || acceptanceCase.Expected.NewRecord.FormKey is not null)
        {
            throw new InvalidDataException($"Case '{acceptanceCase.CaseId}' must expect one discovered new record and one exact source override.");
        }

        var sourceFormKey = ParseFormKey(acceptanceCase.Source.FormListFormKey, "source.formListFormKey");
        if (!string.Equals(sourceFormKey.ToString(), acceptanceCase.Expected.OverrideRecord.FormKey, StringComparison.Ordinal))
        {
            throw new InvalidDataException($"Case '{acceptanceCase.CaseId}' override expectation does not target the selected source FormKey.");
        }

        var sourceModKey = ModKey.FromNameAndExtension(acceptanceCase.Source.ModKey);
        if (sourceFormKey.ModKey != sourceModKey
            || !string.Equals(sourceModKey.ToString(), ModKey.FromNameAndExtension(Path.GetFileName(acceptanceCase.WorkspaceOpen.SourcePluginPath)).ToString(), StringComparison.Ordinal)
            || !acceptanceCase.Source.Baseline.TryGetProperty("FormKey", out var baselineFormKey)
            || !string.Equals(baselineFormKey.GetString(), sourceFormKey.ToString(), StringComparison.Ordinal))
        {
            throw new InvalidDataException($"Case '{acceptanceCase.CaseId}' source ModKey, path, FormKey, and complete baseline do not identify one record.");
        }

        var bookFormKey = ParseFormKey(acceptanceCase.CrossFamilyTargets.BookFormKey, "crossFamilyTargets.bookFormKey");
        var keywordFormKey = ParseFormKey(acceptanceCase.CrossFamilyTargets.KeywordFormKey, "crossFamilyTargets.keywordFormKey");
        var expectedNewItems = new[] { bookFormKey.ToString(), keywordFormKey.ToString(), bookFormKey.ToString() };
        var expectedOverrideItems = new[] { keywordFormKey.ToString(), bookFormKey.ToString(), keywordFormKey.ToString() };
        if (!acceptanceCase.Expected.NewRecord.Items.SequenceEqual(expectedNewItems, StringComparer.Ordinal)
            || !acceptanceCase.Expected.OverrideRecord.Items.SequenceEqual(expectedOverrideItems, StringComparer.Ordinal))
        {
            throw new InvalidDataException($"Case '{acceptanceCase.CaseId}' expected items do not preserve the exported Book/Keyword cross-family order and duplicate positions.");
        }

        RequireDescendantFile(acceptanceCase.WorkspaceOpen.SourcePluginPath, acceptanceRoot, mustExist: true, "sourcePluginPath");
        foreach (var path in acceptanceCase.WorkspaceOpen.LoadOrderPluginPaths)
        {
            RequireDescendantFile(path, acceptanceRoot, mustExist: true, "loadOrderPluginPaths entry");
        }

        if (!acceptanceCase.WorkspaceOpen.LoadOrderPluginPaths.Contains(acceptanceCase.WorkspaceOpen.SourcePluginPath, PathComparer))
        {
            throw new InvalidDataException($"Case '{acceptanceCase.CaseId}' source path is absent from its explicit load order.");
        }

        RequireDescendantDirectory(acceptanceCase.WorkspaceOpen.DataDirectoryPath, acceptanceRoot, "dataDirectoryPath");
        foreach (var path in acceptanceCase.WorkspaceOpen.StringDirectoryPaths)
        {
            RequireDescendantDirectory(path, acceptanceRoot, "stringDirectoryPaths entry");
        }

        var pluginArtifacts = acceptanceCase.Artifacts
            .Where(artifact => string.Equals(artifact.Role, "load_order_plugin", StringComparison.Ordinal))
            .OrderBy(artifact => artifact.LoadOrderIndex)
            .ToArray();
        if (pluginArtifacts.Length != acceptanceCase.WorkspaceOpen.LoadOrderPluginPaths.Length
            || pluginArtifacts.Where((artifact, index) => artifact.LoadOrderIndex != index).Any()
            || pluginArtifacts.Select(artifact => Path.GetFullPath(Path.Combine(acceptanceRoot, artifact.RelativePath.Replace('/', Path.DirectorySeparatorChar))))
                .Where((path, index) => !PathComparer.Equals(path, acceptanceCase.WorkspaceOpen.LoadOrderPluginPaths[index]))
                .Any()
            || acceptanceCase.Artifacts.Any(artifact => !string.Equals(artifact.Role, "load_order_plugin", StringComparison.Ordinal)
                && !string.Equals(artifact.Role, "localized_string", StringComparison.Ordinal))
            || acceptanceCase.Artifacts.Any(artifact => artifact.Role == "localized_string" && artifact.LoadOrderIndex is not null))
        {
            throw new InvalidDataException($"Case '{acceptanceCase.CaseId}' artifact roles and load-order indexes do not match the exact workspace inputs.");
        }

        RequireDescendantFile(acceptanceCase.Output.PluginPath, acceptanceRoot, mustExist: true, "output.pluginPath");
        if (acceptanceCase.WorkspaceOpen.LoadOrderPluginPaths.Contains(acceptanceCase.Output.PluginPath, PathComparer)
            || !string.Equals(ModKey.FromNameAndExtension(Path.GetFileName(acceptanceCase.Output.PluginPath)).ToString(), acceptanceCase.Output.ModKey, StringComparison.Ordinal)
            || !string.Equals(acceptanceCase.Output.LocalizedOutputMode, "embedded", StringComparison.Ordinal)
            || !string.Equals(acceptanceCase.Output.MasterStyle, "full", StringComparison.Ordinal))
        {
            throw new InvalidDataException($"Case '{acceptanceCase.CaseId}' output association is not a distinct embedded full-master plugin.");
        }

        var requiredSteps = new[]
        {
            "begin-new", "replay-begin-new", "set-new-editor-id", "replay-set-new-editor-id",
            "reject-changed-payload-same-id", "reject-malformed-apply", "reject-stale-revision-apply",
            "begin-override", "preview-staged-changes", "save", "replay-save", "close-first-workspace", "open-fresh-workspace",
            "open-existing-output", "inspect-new", "inspect-override", "preview-fresh-workspace", "close-fresh-workspace",
        };
        var stepNames = acceptanceCase.AuthoringPlan.Select(step => step.Name).ToArray();
        if (stepNames.Distinct(StringComparer.Ordinal).Count() != stepNames.Length
            || requiredSteps.Any(required => !stepNames.Contains(required, StringComparer.Ordinal)))
        {
            throw new InvalidDataException($"Case '{acceptanceCase.CaseId}' authoring plan is missing required replay, diagnostic, or reopen steps.");
        }


        AssertExactReplay(acceptanceCase, "replay-begin-new", "begin-new");
        AssertExactReplay(acceptanceCase, "replay-set-new-editor-id", "set-new-editor-id");
        AssertExactReplay(acceptanceCase, "replay-save", "save");
        var originalApply = acceptanceCase.AuthoringPlan.Single(step => string.Equals(step.Name, "set-new-editor-id", StringComparison.Ordinal));
        var changedPayload = acceptanceCase.AuthoringPlan.Single(step => string.Equals(step.Name, "reject-changed-payload-same-id", StringComparison.Ordinal));
        if (originalApply.StaticArguments.GetProperty("operationId").GetGuid()
            != changedPayload.StaticArguments.GetProperty("operationId").GetGuid())
        {
            throw new InvalidDataException($"Case '{acceptanceCase.CaseId}' changed-payload probe must reuse the successful Apply operationId.");
        }
    }

    /// <summary>Requires one replay step to preserve the complete static request and identify its original request.</summary>
    /// <param name="acceptanceCase">The case containing both steps.</param>
    /// <param name="replayName">The replay step name.</param>
    /// <param name="originalName">The original successful step name.</param>
    /// <exception cref="InvalidDataException">Thrown when replay metadata or static request bytes differ.</exception>
    private static void AssertExactReplay(ClientAcceptanceCase acceptanceCase, string replayName, string originalName)
    {
        var replay = acceptanceCase.AuthoringPlan.Single(step => string.Equals(step.Name, replayName, StringComparison.Ordinal));
        var original = acceptanceCase.AuthoringPlan.Single(step => string.Equals(step.Name, originalName, StringComparison.Ordinal));
        if (!string.Equals(replay.ExactReplayOf, originalName, StringComparison.Ordinal)
            || !JsonElement.DeepEquals(replay.StaticArguments, original.StaticArguments))
        {
            throw new InvalidDataException($"Case '{acceptanceCase.CaseId}' replay '{replayName}' does not preserve the complete static request from '{originalName}'.");
        }
    }

    /// <summary>Checks that the manifest covers every input file and that no byte or length changed.</summary>
    /// <param name="acceptanceRoot">The canonical retained root.</param>
    /// <param name="acceptanceCase">The case whose copied inputs must remain immutable.</param>
    /// <exception cref="InvalidDataException">Thrown when an artifact is absent, added, or changed.</exception>
    private static void AssertImmutableArtifacts(string acceptanceRoot, ClientAcceptanceCase acceptanceCase)
    {
        var expectedPaths = acceptanceCase.Artifacts
            .Select(artifact => Path.GetFullPath(Path.Combine(acceptanceRoot, artifact.RelativePath.Replace('/', Path.DirectorySeparatorChar))))
            .OrderBy(path => path, PathComparer)
            .ToArray();
        var actualPaths = new[] { acceptanceCase.WorkspaceOpen.DataDirectoryPath }
            .Concat(acceptanceCase.WorkspaceOpen.StringDirectoryPaths)
            .SelectMany(path => Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories))
            .Select(Path.GetFullPath)
            .OrderBy(path => path, PathComparer)
            .ToArray();
        if (!actualPaths.SequenceEqual(expectedPaths, PathComparer))
        {
            throw new InvalidDataException($"Case '{acceptanceCase.CaseId}' immutable input file set changed after export.");
        }

        foreach (var artifact in acceptanceCase.Artifacts)
        {
            var path = Path.GetFullPath(Path.Combine(acceptanceRoot, artifact.RelativePath.Replace('/', Path.DirectorySeparatorChar)));
            RequireDescendantFile(path, acceptanceRoot, mustExist: true, "artifact.relativePath");
            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            if (stream.Length != artifact.Length
                || !string.Equals(Convert.ToHexString(SHA256.HashData(stream)), artifact.Sha256, StringComparison.Ordinal))
            {
                throw new InvalidDataException($"Retained immutable input changed: '{artifact.RelativePath}'.");
            }
        }
    }

    /// <summary>Directly parses one output with game-native readers and checks its exact record set and requested values.</summary>
    /// <param name="acceptanceCase">The retained game case.</param>
    /// <param name="cancellationToken">A token observed during native parsing and inspection.</param>
    /// <returns>The newly allocated output-owned FormKey discovered by unique EditorID.</returns>
    /// <exception cref="InvalidDataException">Thrown when direct native state differs from the contract.</exception>
    private static DirectVerificationResult VerifyDirectNativeOutput(ClientAcceptanceCase acceptanceCase, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var gameRelease = ParseRelease(acceptanceCase);
        var outputModKey = ModKey.FromNameAndExtension(Path.GetFileName(acceptanceCase.Output.PluginPath));
        var masterFlags = CreateMasterFlags(acceptanceCase, gameRelease, outputModKey);
        var metadata = ParsingMeta.Factory(
            new BinaryReadParameters { MasterFlagsLookup = masterFlags, ThrowOnUnknownSubrecord = true },
            gameRelease,
            new ModPath(outputModKey, acceptanceCase.Output.PluginPath));
        using var fileStream = new FileStream(acceptanceCase.Output.PluginPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var binaryStream = new MutagenBinaryReadStream(fileStream, metadata, bufferSize: 4096, dispose: true, offsetReference: 0);
        var frame = new MutagenFrame(binaryStream);
        IReadOnlyDictionary<FormKey, JsonElement> records;
        IReadOnlyList<ModKey> masters;
        switch (acceptanceCase.Game)
        {
            case "starfield":
            {
                var mod = StarfieldMod.CreateFromBinary(frame, StarfieldRelease.Starfield, new Mutagen.Bethesda.Starfield.GroupMask(true));
                RequireExactStarfieldRecordSet(mod, acceptanceCase.Expected.OutputFormListCount);
                records = mod.FormLists.ToDictionary(record => record.FormKey, record => WriteReadView(new StarfieldFormListNativeInspector(), record, cancellationToken));
                masters = mod.ModHeader.MasterReferences.Select(reference => reference.Master).ToArray();
                break;
            }
            case "fallout4":
            {
                var mod = Fallout4Mod.CreateFromBinary(frame, Fallout4Release.Fallout4, new Mutagen.Bethesda.Fallout4.GroupMask(true));
                RequireExactFallout4RecordSet(mod, acceptanceCase.Expected.OutputFormListCount);
                records = mod.FormLists.ToDictionary(record => record.FormKey, record => WriteReadView(new Fallout4FormListNativeInspector(), record, cancellationToken));
                masters = mod.ModHeader.MasterReferences.Select(reference => reference.Master).ToArray();
                break;
            }
            case "skyrim":
            {
                var mod = SkyrimMod.CreateFromBinary(frame, SkyrimRelease.SkyrimSE, new Mutagen.Bethesda.Skyrim.GroupMask(true));
                RequireExactSkyrimRecordSet(mod, acceptanceCase.Expected.OutputFormListCount);
                records = mod.FormLists.ToDictionary(record => record.FormKey, record => WriteReadView(new SkyrimFormListNativeInspector(), record, cancellationToken));
                masters = mod.ModHeader.MasterReferences.Select(reference => reference.Master).ToArray();
                break;
            }
            default:
                throw new InvalidDataException($"Unsupported client-acceptance game '{acceptanceCase.Game}'.");
        }

        if (records.Count != acceptanceCase.Expected.OutputFormListCount)
        {
            throw new InvalidDataException($"Direct {acceptanceCase.Game} output contained {records.Count} FormLists instead of {acceptanceCase.Expected.OutputFormListCount}; duplicate retry allocation is not accepted.");
        }

        var expectedMaster = ModKey.FromNameAndExtension(acceptanceCase.Source.ModKey);
        if (!masters.SequenceEqual(new[] { expectedMaster }))
        {
            throw new InvalidDataException($"Direct {acceptanceCase.Game} output masters differed from the exact selected source closure.");
        }

        var newMatches = records.Where(pair => string.Equals(ReadEditorId(pair.Value), acceptanceCase.Expected.NewRecord.EditorId, StringComparison.Ordinal)).ToArray();
        if (newMatches.Length != 1 || newMatches[0].Key.ModKey != outputModKey)
        {
            throw new InvalidDataException($"Direct {acceptanceCase.Game} output did not contain exactly one output-owned new FormList with EditorID '{acceptanceCase.Expected.NewRecord.EditorId}'.");
        }

        AssertExpectedRecord(newMatches[0].Value, acceptanceCase.Expected.NewRecord, "direct native new record");
        var overrideFormKey = ParseFormKey(acceptanceCase.Source.FormListFormKey, "source.formListFormKey");
        if (!records.TryGetValue(overrideFormKey, out var overrideRecord))
        {
            throw new InvalidDataException($"Direct {acceptanceCase.Game} output did not contain source override '{overrideFormKey}'.");
        }

        AssertExpectedRecord(overrideRecord, acceptanceCase.Expected.OverrideRecord, "direct native override");
        AssertUneditedFields(acceptanceCase.Source.Baseline, overrideRecord, acceptanceCase.Source.AllowedChangedProperties, "direct native override");
        return new DirectVerificationResult(newMatches[0].Key);
    }

    /// <summary>Requires a Starfield output to contain exactly the expected FormLists and no record from another family.</summary>
    /// <param name="mod">The directly parsed or constructed Starfield mod.</param>
    /// <param name="expectedFormListCount">The exact accepted FormList count.</param>
    /// <exception cref="InvalidDataException">Thrown when total major-record count or any record family differs.</exception>
    internal static void RequireExactStarfieldRecordSet(Mutagen.Bethesda.Starfield.IStarfieldModGetter mod, int expectedFormListCount)
    {
        ArgumentNullException.ThrowIfNull(mod);
        RequireOnlyExpectedFormLists(
            mod.EnumerateMajorRecords(),
            expectedFormListCount,
            static record => record is Mutagen.Bethesda.Starfield.IFormListGetter,
            "Starfield");
    }

    /// <summary>Requires a Fallout 4 output to contain exactly the expected FormLists and no record from another family.</summary>
    /// <param name="mod">The directly parsed or constructed Fallout 4 mod.</param>
    /// <param name="expectedFormListCount">The exact accepted FormList count.</param>
    /// <exception cref="InvalidDataException">Thrown when total major-record count or any record family differs.</exception>
    internal static void RequireExactFallout4RecordSet(Mutagen.Bethesda.Fallout4.IFallout4ModGetter mod, int expectedFormListCount)
    {
        ArgumentNullException.ThrowIfNull(mod);
        RequireOnlyExpectedFormLists(
            mod.EnumerateMajorRecords(),
            expectedFormListCount,
            static record => record is Mutagen.Bethesda.Fallout4.IFormListGetter,
            "Fallout 4");
    }

    /// <summary>Requires a Skyrim output to contain exactly the expected FormLists and no record from another family.</summary>
    /// <param name="mod">The directly parsed or constructed Skyrim Special Edition mod.</param>
    /// <param name="expectedFormListCount">The exact accepted FormList count.</param>
    /// <exception cref="InvalidDataException">Thrown when total major-record count or any record family differs.</exception>
    internal static void RequireExactSkyrimRecordSet(Mutagen.Bethesda.Skyrim.ISkyrimModGetter mod, int expectedFormListCount)
    {
        ArgumentNullException.ThrowIfNull(mod);
        RequireOnlyExpectedFormLists(
            mod.EnumerateMajorRecords(),
            expectedFormListCount,
            static record => record is Mutagen.Bethesda.Skyrim.IFormListGetter,
            "Skyrim Special Edition");
    }

    /// <summary>Enumerates every major record and rejects extra counts or non-FormList families.</summary>
    /// <param name="records">Every major record from one native mod enumerator.</param>
    /// <param name="expectedFormListCount">The exact accepted FormList count.</param>
    /// <param name="isFormList">The game-specific FormList type predicate.</param>
    /// <param name="game">The diagnostic game name.</param>
    /// <exception cref="InvalidDataException">Thrown when the complete native record set is not exactly the expected FormLists.</exception>
    private static void RequireOnlyExpectedFormLists(
        IEnumerable<IMajorRecordGetter> records,
        int expectedFormListCount,
        Func<IMajorRecordGetter, bool> isFormList,
        string game)
    {
        var allRecords = records.ToArray();
        if (allRecords.Length != expectedFormListCount || allRecords.Any(record => !isFormList(record)))
        {
            throw new InvalidDataException($"Direct {game} output must contain exactly {expectedFormListCount} FormLists and no other major records.");
        }
    }

    /// <summary>Reopens one saved output through a new production engine composition and repeats semantic assertions.</summary>
    /// <param name="acceptanceCase">The retained game case.</param>
    /// <param name="newFormKey">The output-owned key discovered by direct native parsing.</param>
    /// <param name="cancellationToken">The propagated operation token.</param>
    /// <returns>A task that completes after fresh engine reads and an empty preview.</returns>
    private static async Task VerifyFreshEngineReopenAsync(ClientAcceptanceCase acceptanceCase, FormKey newFormKey, CancellationToken cancellationToken)
    {
        await using var services = NativeEngineComposition.Create();
        var openRequest = new WorkspaceOpenRequest(
            acceptanceCase.ReopenWorkspaceId,
            ParseGame(acceptanceCase.Game),
            ParseRelease(acceptanceCase),
            acceptanceCase.WorkspaceOpen.SourcePluginPath,
            acceptanceCase.WorkspaceOpen.LoadOrderPluginPaths,
            acceptanceCase.WorkspaceOpen.DataDirectoryPath,
            acceptanceCase.WorkspaceOpen.StringDirectoryPaths);
        var opened = await services.WorkspaceFactory.OpenAsync(openRequest, cancellationToken);
        RequireSucceeded(opened, $"Fresh {acceptanceCase.Game} engine workspace open");
        await using var workspace = opened.Value!;
        var association = new OutputAssociation(
            acceptanceCase.Output.PluginPath,
            ModKey.FromNameAndExtension(acceptanceCase.Output.ModKey),
            LocalizedOutputMode.Embedded,
            OutputMasterStyle.Full);
        var selected = await workspace.SelectOutputAsync(
            new SelectOutputRequest(Guid.NewGuid(), workspace.Revision, OutputSelectionMode.OpenExisting, association),
            cancellationToken);
        RequireSucceeded(selected, $"Fresh {acceptanceCase.Game} output selection");

        var listed = await workspace.ListFormListsAsync(RecordScope.StagedOutput, cancellationToken);
        RequireSucceeded(listed, $"Fresh {acceptanceCase.Game} output list");
        if (listed.Value!.Count != acceptanceCase.Expected.OutputFormListCount
            || listed.Value.Count(item => string.Equals(item.EditorId, acceptanceCase.Expected.NewRecord.EditorId, StringComparison.Ordinal)) != 1)
        {
            throw new InvalidDataException($"Fresh {acceptanceCase.Game} engine reopen did not expose the exact two-record output without duplicate retries.");
        }

        var newRecord = await ReadEngineRecordAsync(workspace, association.ModKey, newFormKey, cancellationToken);
        AssertExpectedRecord(newRecord, acceptanceCase.Expected.NewRecord, "fresh engine new record");
        var overrideFormKey = ParseFormKey(acceptanceCase.Source.FormListFormKey, "source.formListFormKey");
        var overrideRecord = await ReadEngineRecordAsync(workspace, association.ModKey, overrideFormKey, cancellationToken);
        AssertExpectedRecord(overrideRecord, acceptanceCase.Expected.OverrideRecord, "fresh engine override");
        AssertUneditedFields(acceptanceCase.Source.Baseline, overrideRecord, acceptanceCase.Source.AllowedChangedProperties, "fresh engine override");

        var sourceRead = await workspace.ReadFormListViewAsync(
            new ReferenceRequest(overrideFormKey, RecordScope.Source, ModKey.FromNameAndExtension(acceptanceCase.Source.ModKey)),
            cancellationToken);
        RequireSucceeded(sourceRead, $"Fresh {acceptanceCase.Game} source preservation read");
        if (sourceRead.Value?.Record is null || !JsonElement.DeepEquals(sourceRead.Value.Record.Value, acceptanceCase.Source.Baseline))
        {
            throw new InvalidDataException($"Fresh {acceptanceCase.Game} engine source view differed from the exported complete baseline.");
        }

        var preview = await workspace.PreviewAsync(cancellationToken);
        RequireSucceeded(preview, $"Fresh {acceptanceCase.Game} preview");
        if (preview.Value!.Comparisons.Count != 0)
        {
            throw new InvalidDataException($"Fresh {acceptanceCase.Game} reopened output unexpectedly contained staged changes.");
        }
    }

    /// <summary>Reads one exact output record through the fresh engine's complete native inspector view.</summary>
    /// <param name="workspace">The fresh workspace.</param>
    /// <param name="outputModKey">The containing output identity.</param>
    /// <param name="formKey">The exact record identity.</param>
    /// <param name="cancellationToken">The propagated read token.</param>
    /// <returns>The detached complete inspector JSON.</returns>
    private static async Task<JsonElement> ReadEngineRecordAsync(IFormListWorkspace workspace, ModKey outputModKey, FormKey formKey, CancellationToken cancellationToken)
    {
        var read = await workspace.ReadFormListViewAsync(new ReferenceRequest(formKey, RecordScope.StagedOutput, outputModKey), cancellationToken);
        RequireSucceeded(read, $"Fresh engine read for '{formKey}'");
        if (read.Value?.Record is null)
        {
            throw new InvalidDataException($"Fresh engine read for '{formKey}' succeeded without a record.");
        }

        return read.Value.Record.Value;
    }

    /// <summary>Creates strict parsing master-style metadata from every copied plugin header.</summary>
    /// <param name="acceptanceCase">The explicit case inputs.</param>
    /// <param name="release">The exact native release.</param>
    /// <param name="outputModKey">The full-master output identity.</param>
    /// <returns>A complete native master-style lookup.</returns>
    private static Cache<IModMasterStyledGetter, ModKey> CreateMasterFlags(ClientAcceptanceCase acceptanceCase, GameRelease release, ModKey outputModKey)
    {
        var fileSystem = new FileSystem();
        var result = new Cache<IModMasterStyledGetter, ModKey>(metadata => metadata.ModKey);
        foreach (var path in acceptanceCase.WorkspaceOpen.LoadOrderPluginPaths)
        {
            var modKey = ModKey.FromNameAndExtension(Path.GetFileName(path));
            var header = ModHeaderFrame.FromPath(new ModPath(modKey, path), release, fileSystem);
            result.Set(new KeyedMasterStyle(modKey, header.MasterStyle));
        }

        result.Set(new KeyedMasterStyle(outputModKey, MasterStyle.Full));
        return result;
    }

    /// <summary>Writes one direct native record through the production complete inspector.</summary>
    /// <param name="inspector">The stateless game inspector.</param>
    /// <param name="record">The direct native FormList.</param>
    /// <param name="cancellationToken">The propagated traversal token.</param>
    /// <returns>A detached complete inspector value.</returns>
    private static JsonElement WriteReadView(IFormListNativeInspector inspector, IMajorRecordGetter record, CancellationToken cancellationToken)
    {
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream))
        {
            inspector.WriteReadView(record, writer, cancellationToken);
        }

        using var document = JsonDocument.Parse(stream.ToArray());
        return document.RootElement.Clone();
    }

    /// <summary>Checks one detached record's EditorID, optional English name, and exact ordered item identities.</summary>
    /// <param name="record">The complete native inspector value.</param>
    /// <param name="expected">The independent semantic expectation.</param>
    /// <param name="context">The diagnostic verification context.</param>
    /// <exception cref="InvalidDataException">Thrown when any requested field differs.</exception>
    private static void AssertExpectedRecord(JsonElement record, ClientAcceptanceExpectedRecord expected, string context)
    {
        if (!string.Equals(ReadEditorId(record), expected.EditorId, StringComparison.Ordinal))
        {
            throw new InvalidDataException($"{context} EditorID differed from '{expected.EditorId}'.");
        }

        var actualName = ReadName(record);
        if (!string.Equals(actualName, expected.Name, StringComparison.Ordinal))
        {
            throw new InvalidDataException($"{context} Name differed from '{expected.Name ?? "<unsupported>"}'.");
        }

        var actualItems = record.GetProperty("Items").EnumerateArray()
            .Select(item => item.GetProperty("formKey").GetString())
            .ToArray();
        if (!actualItems.SequenceEqual(expected.Items, StringComparer.Ordinal))
        {
            throw new InvalidDataException($"{context} ordered Items differed from the manifest.");
        }
    }

    /// <summary>Checks every complete source field except the explicitly requested override properties.</summary>
    /// <param name="baseline">The complete exported source view.</param>
    /// <param name="actual">The complete saved override view.</param>
    /// <param name="allowedChangedProperties">Root properties intentionally edited by the real client.</param>
    /// <param name="context">The diagnostic verification context.</param>
    /// <exception cref="InvalidDataException">Thrown when an unedited field is missing, added, or changed.</exception>
    private static void AssertUneditedFields(JsonElement baseline, JsonElement actual, IReadOnlyCollection<string> allowedChangedProperties, string context)
    {
        var allowed = allowedChangedProperties.ToHashSet(StringComparer.Ordinal);
        var baselineProperties = baseline.EnumerateObject().Where(property => !allowed.Contains(property.Name)).ToArray();
        var actualProperties = actual.EnumerateObject().Where(property => !allowed.Contains(property.Name)).ToArray();
        if (!baselineProperties.Select(property => property.Name).SequenceEqual(actualProperties.Select(property => property.Name), StringComparer.Ordinal))
        {
            throw new InvalidDataException($"{context} changed the complete unedited property set.");
        }

        foreach (var property in baselineProperties)
        {
            if (!actual.TryGetProperty(property.Name, out var actualValue) || !JsonElement.DeepEquals(property.Value, actualValue))
            {
                throw new InvalidDataException($"{context} changed unrequested property '{property.Name}'.");
            }
        }
    }

    /// <summary>Gets the nullable EditorID from a complete inspector object.</summary>
    /// <param name="record">The complete inspector value.</param>
    /// <returns>The EditorID, or <see langword="null"/>.</returns>
    private static string? ReadEditorId(JsonElement record)
    {
        var value = record.GetProperty("EditorID");
        return value.ValueKind == JsonValueKind.Null ? null : value.GetString();
    }

    /// <summary>Gets the English translated name from a complete inspector object when the game supports it.</summary>
    /// <param name="record">The complete inspector value.</param>
    /// <returns>The translated name value, or <see langword="null"/> when absent or unsupported.</returns>
    private static string? ReadName(JsonElement record)
    {
        if (!record.TryGetProperty("Name", out var name) || name.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        var value = name.GetProperty("value");
        return value.ValueKind == JsonValueKind.Null ? null : value.GetString();
    }

    /// <summary>Parses one exact supported game token.</summary>
    /// <param name="game">The manifest game token.</param>
    /// <returns>The supported engine game.</returns>
    /// <exception cref="InvalidDataException">Thrown when the token is unsupported.</exception>
    private static SupportedGame ParseGame(string game)
    {
        return game switch
        {
            "starfield" => SupportedGame.Starfield,
            "fallout4" => SupportedGame.Fallout4,
            "skyrim" => SupportedGame.Skyrim,
            _ => throw new InvalidDataException($"Unsupported client-acceptance game '{game}'."),
        };
    }

    /// <summary>Parses and verifies one game-specific native release token.</summary>
    /// <param name="acceptanceCase">The case carrying both game and release.</param>
    /// <returns>The exact native release.</returns>
    /// <exception cref="InvalidDataException">Thrown when the game or release pair is unsupported.</exception>
    private static GameRelease ParseRelease(ClientAcceptanceCase acceptanceCase)
    {
        return (acceptanceCase.Game, acceptanceCase.Release) switch
        {
            ("starfield", "starfield") => GameRelease.Starfield,
            ("fallout4", "fallout4") => GameRelease.Fallout4,
            ("skyrim", "skyrim_se") => GameRelease.SkyrimSE,
            _ => throw new InvalidDataException($"Unsupported client-acceptance game/release pair '{acceptanceCase.Game}/{acceptanceCase.Release}'."),
        };
    }

    /// <summary>Parses one canonical native FormKey string.</summary>
    /// <param name="value">The manifest value.</param>
    /// <param name="field">The diagnostic field name.</param>
    /// <returns>The non-null native identity.</returns>
    /// <exception cref="InvalidDataException">Thrown when the value is malformed or null.</exception>
    private static FormKey ParseFormKey(string value, string field)
    {
        if (!FormKey.TryFactory(value.AsSpan(), out var formKey) || formKey.IsNull || !string.Equals(formKey.ToString(), value, StringComparison.Ordinal))
        {
            throw new InvalidDataException($"Manifest field '{field}' is not a canonical non-null FormKey: '{value}'.");
        }

        return formKey;
    }

    /// <summary>Requires one generic engine result to succeed with a non-null value.</summary>
    /// <typeparam name="T">The engine result value type.</typeparam>
    /// <param name="result">The engine result.</param>
    /// <param name="context">The diagnostic operation context.</param>
    /// <exception cref="InvalidDataException">Thrown when the result failed or omitted its value.</exception>
    private static void RequireSucceeded<T>(EngineResult<T> result, string context)
    {
        if (!result.Succeeded || result.Value is null)
        {
            throw new InvalidDataException($"{context} failed: {(result.Error is null ? "no engine error was supplied" : $"{result.Error.Code}: {result.Error.Message}")}");
        }
    }

    /// <summary>Requires two path strings to resolve to the same canonical path.</summary>
    /// <param name="actual">The manifest path.</param>
    /// <param name="expected">The independently resolved path.</param>
    /// <param name="field">The diagnostic field name.</param>
    /// <exception cref="InvalidDataException">Thrown when the paths differ or are malformed.</exception>
    private static void RequireSamePath(string actual, string expected, string field)
    {
        if (!string.Equals(Path.GetFullPath(actual), Path.GetFullPath(expected), PathComparison))
        {
            throw new InvalidDataException($"Manifest field '{field}' does not match the independently resolved path.");
        }
    }

    /// <summary>Requires an existing directory to remain beneath the retained root.</summary>
    /// <param name="path">The supplied directory path.</param>
    /// <param name="root">The retained root.</param>
    /// <param name="field">The diagnostic field name.</param>
    /// <exception cref="InvalidDataException">Thrown when the directory is absent or outside the root.</exception>
    private static void RequireDescendantDirectory(string path, string root, string field)
    {
        RequireDescendant(path, root, field);
        ClientAcceptancePaths.RequireExistingDirectory(path, $"Manifest field '{field}' directory");
    }

    /// <summary>Requires a file path to remain beneath the retained root and optionally exist.</summary>
    /// <param name="path">The supplied file path.</param>
    /// <param name="root">The retained root.</param>
    /// <param name="mustExist">Whether the file must already exist.</param>
    /// <param name="field">The diagnostic field name.</param>
    /// <exception cref="InvalidDataException">Thrown when the path is outside the root or required file is absent.</exception>
    private static void RequireDescendantFile(string path, string root, bool mustExist, string field)
    {
        RequireDescendant(path, root, field);
        if (mustExist)
        {
            ClientAcceptancePaths.RequireExistingRegularFile(path, $"Manifest field '{field}' file");
        }
        else
        {
            ClientAcceptancePaths.RejectReparseAncestry(path, $"Manifest field '{field}' file");
        }
    }

    /// <summary>Requires an absolute path to be a strict descendant of the retained root.</summary>
    /// <param name="path">The supplied path.</param>
    /// <param name="root">The retained root.</param>
    /// <param name="field">The diagnostic field name.</param>
    /// <exception cref="InvalidDataException">Thrown when the path is relative or escapes the root.</exception>
    private static void RequireDescendant(string path, string root, string field)
    {
        if (!Path.IsPathFullyQualified(path))
        {
            throw new InvalidDataException($"Manifest field '{field}' must be an absolute path.");
        }

        var relative = Path.GetRelativePath(Path.GetFullPath(root), Path.GetFullPath(path));
        if (string.Equals(relative, ".", StringComparison.Ordinal)
            || Path.IsPathRooted(relative)
            || string.Equals(relative, "..", StringComparison.Ordinal)
            || relative.StartsWith($"..{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
        {
            throw new InvalidDataException($"Manifest field '{field}' must be a strict descendant of the retained root.");
        }
    }

    /// <summary>Gets the platform-appropriate path comparer.</summary>
    private static StringComparer PathComparer => OperatingSystem.IsWindows() ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;

    /// <summary>Gets the platform-appropriate path string comparison.</summary>
    private static StringComparison PathComparison => OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

    /// <summary>Returns the direct parser's unique new-record identity.</summary>
    /// <param name="newFormKey">The output-owned allocation.</param>
    private sealed class DirectVerificationResult
    {
        /// <summary>Initializes a direct-parser result with the unique output-owned allocation.</summary>
        /// <param name="newFormKey">The output-owned allocation.</param>
        internal DirectVerificationResult(FormKey newFormKey)
        {
            NewFormKey = newFormKey;
        }

        /// <summary>Gets the output-owned FormKey discovered from the unique expected EditorID.</summary>
        public FormKey NewFormKey { get; }
    }
}
