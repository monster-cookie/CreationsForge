using System.IO.Abstractions;
using System.Security.Cryptography;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.PluginOutputs;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Strings;
using Mutagen.Bethesda.Strings.DI;
using Noggog;
using MutagenMasterStyle = Mutagen.Bethesda.Plugins.MasterStyle;

namespace CreationsForge.Skyrim.PluginAdapter;

/// <summary>
/// Implements guarded private Skyrim serialization and strict Mutagen reopen validation for the output service.
/// </summary>
public sealed partial class SkyrimPluginOutputService
{
    /// <summary>
    /// Writes a complete private Skyrim output set, reopens it strictly, and proves Mutagen and FormList semantic equality before destination commit.
    /// </summary>
    /// <param name="sources">The borrowed complete Skyrim source lifetime.</param>
    /// <param name="output">The borrowed complete staged Skyrim output state.</param>
    /// <param name="request">The exclusive empty private staging directory, exact selected output identity, and current destination baseline.</param>
    /// <param name="cancellationToken">A token that cancels private work before any destination mutation.</param>
    /// <returns>A validated changed artifact set, an existing-output no-op with no artifacts, or a typed precommit failure.</returns>
    /// <exception cref="ArgumentNullException">Thrown when a required argument is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public async Task<EngineResult<StagedPluginOutputSet>> WriteAndValidateAsync(
        SkyrimPluginSourceSet sources,
        SkyrimPluginOutputState output,
        PluginWriteRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(sources);
        ArgumentNullException.ThrowIfNull(output);
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        var associationFailure = ValidateWriteAssociation(output.Association, request.Output);
        if (associationFailure is not null)
        {
            return WriteFailure(sources, associationFailure.Value.Code, associationFailure.Value.Message);
        }

        string stagingDirectoryPath;
        try
        {
            stagingDirectoryPath = Path.GetFullPath(request.StagingDirectoryPath);
        }
        catch (Exception exception) when (
            exception is ArgumentException or NotSupportedException or PathTooLongException)
        {
            return WriteFailure(
                sources,
                EngineErrorCode.InvalidRequest,
                $"The private Skyrim staging directory path is invalid: {exception.Message}");
        }

        var stagingFailure = ValidateStagingDirectory(stagingDirectoryPath, output.Association.PluginPath);
        if (stagingFailure is not null)
        {
            return WriteFailure(sources, stagingFailure.Value.Code, stagingFailure.Value.Message);
        }

        var freshness = await ReopenAsync(
            sources,
            output.Association,
            request.ExpectedOutputBaseline,
            cancellationToken).ConfigureAwait(false);
        if (!freshness.Succeeded || freshness.Value?.Output is not SkyrimPluginOutputState freshOutput)
        {
            var freshnessError = freshness.Error ?? new EngineError(
                EngineErrorCode.OutputOpenFailed,
                "The selected Skyrim output could not be reverified before staging.");
            return WriteFailure(
                sources,
                freshnessError.Code,
                freshnessError.Message,
                freshness.Warnings);
        }

        await freshOutput.DisposeAsync().ConfigureAwait(false);
        cancellationToken.ThrowIfCancellationRequested();

        SkyrimMod candidate;
        SkyrimMod original;
        IReadOnlyList<RecordEditProvenance> provenance;
        try
        {
            candidate = output.CreateSnapshot(cancellationToken);
            original = output.CreateOriginalSnapshot(cancellationToken);
            provenance = output.GetEditProvenance();
        }
        catch (ObjectDisposedException exception)
        {
            return WriteFailure(
                sources,
                EngineErrorCode.WorkspaceDisposed,
                exception.Message,
                freshness.Warnings);
        }

        var candidateFailure = ValidateCandidate(
            sources,
            output,
            candidate,
            original,
            provenance,
            cancellationToken);
        if (candidateFailure is not null)
        {
            return WriteFailure(sources, candidateFailure.Value.Code, candidateFailure.Value.Message, freshness.Warnings);
        }

        var existingOutput = output.Baseline.Artifacts.Single(
            artifact => artifact.Role == PluginArtifactRole.Plugin).Fingerprint.Exists;
        if (existingOutput && ArePluginOutputsEquivalent(candidate, original, cancellationToken))
        {
            return WriteSuccess(
                sources,
                new StagedPluginOutputSet(
                    PluginWriteDisposition.Unchanged,
                    stagedOutput: null,
                    artifactMappings: Array.Empty<StagedPluginArtifactMapping>()),
                freshness.Warnings);
        }

        if (existingOutput
            && output.Association.LocalizedOutputMode == LocalizedOutputMode.SeparateStringFiles)
        {
            return WriteFailure(
                sources,
                EngineErrorCode.UnsupportedInput,
                "An existing localized Skyrim output cannot be materially rewritten without losing Mutagen localized-string identity.",
                freshness.Warnings);
        }

        if (output.Association.LocalizedOutputMode == LocalizedOutputMode.SeparateStringFiles
            && !ContainsOnlySupportedLocalizedRecords(candidate, cancellationToken))
        {
            return WriteFailure(
                sources,
                EngineErrorCode.UnsupportedInput,
                "A new localized Skyrim output can contain only supported FormList records because unrelated translated fields cannot be validated losslessly.",
                freshness.Warnings);
        }

        var expected = CopyMod(candidate, cancellationToken);
        var masterResult = NormalizeWriterOwnedHeader(sources, expected, cancellationToken);
        if (!masterResult.Succeeded)
        {
            return WriteFailure(
                sources,
                masterResult.Error!.Code,
                masterResult.Error.Message,
                freshness.Warnings);
        }

        SkyrimPluginOutputState? stagedState = null;
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            var stagedPluginPath = Path.GetFullPath(Path.Combine(
                stagingDirectoryPath,
                output.Association.ModKey.FileName.String));
            WritePrivateOutput(
                sources,
                expected,
                stagedPluginPath,
                output.Association,
                masterResult.Value!,
                cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            var stagedAssociation = new OutputAssociation(
                stagedPluginPath,
                output.Association.ModKey,
                output.Association.LocalizedOutputMode,
                output.Association.MasterStyle);
            var stagedOpen = await OpenCoreAsync(
                sources,
                stagedAssociation,
                OutputSelectionMode.OpenExisting,
                expectedBaseline: null,
                operationId: null,
                baseRevision: sources.Revision,
                cancellationToken).ConfigureAwait(false);
            if (!stagedOpen.Succeeded || stagedOpen.Value?.Output is not SkyrimPluginOutputState openedState)
            {
                var stagedOpenError = stagedOpen.Error ?? new EngineError(
                    EngineErrorCode.ValidationFailed,
                    "The private Skyrim output could not be reopened after writing.");
                return WriteFailure(
                    sources,
                    stagedOpenError.Code,
                    stagedOpenError.Message,
                    freshness.Warnings.Concat(stagedOpen.Warnings).ToArray());
            }

            stagedState = openedState;
            var reopened = stagedState.GetMutableMod();
            var reopenFailure = ValidateReopened(
                expected,
                reopened,
                masterResult.Value!,
                cancellationToken);
            if (reopenFailure is not null)
            {
                return WriteFailure(
                    sources,
                    reopenFailure.Value.Code,
                    reopenFailure.Value.Message,
                    freshness.Warnings.Concat(stagedOpen.Warnings).ToArray());
            }

            if (output.Association.LocalizedOutputMode == LocalizedOutputMode.SeparateStringFiles
                && !ContainsOnlyEmptyLocalizedTable(stagedState.Baseline))
            {
                return WriteFailure(
                    sources,
                    EngineErrorCode.ValidationFailed,
                    "A FormList-only localized Skyrim output must contain exactly the explicit empty English strings table.",
                    freshness.Warnings.Concat(stagedOpen.Warnings).ToArray());
            }

            var mappingsResult = CreateArtifactMappings(
                request.ExpectedOutputBaseline,
                stagedState.Baseline);
            if (!mappingsResult.Succeeded)
            {
                return WriteFailure(
                    sources,
                    mappingsResult.Error!.Code,
                    mappingsResult.Error.Message,
                    freshness.Warnings.Concat(stagedOpen.Warnings).ToArray());
            }

            var stagedLifetime = new SkyrimStagedPluginOutputSet(stagedState);
            stagedState = null;
            return WriteSuccess(
                sources,
                new StagedPluginOutputSet(
                    PluginWriteDisposition.StagedChanges,
                    stagedLifetime,
                    mappingsResult.Value!),
                freshness.Warnings.Concat(stagedOpen.Warnings).ToArray());
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            return WriteFailure(
                sources,
                EngineErrorCode.ValidationFailed,
                $"Skyrim private output writing or Mutagen validation failed: {exception.Message}",
                freshness.Warnings);
        }
        finally
        {
            if (stagedState is not null)
            {
                await stagedState.DisposeAsync().ConfigureAwait(false);
            }
        }
    }

    /// <summary>Writes one complete Skyrim plugin and its supported localized sidecars into the private staging directory.</summary>
    /// <param name="sources">The borrowed source lifetime used for master style and load-order metadata.</param>
    /// <param name="mod">The complete staged plugin output to serialize.</param>
    /// <param name="stagedPluginPath">The canonical private plugin path.</param>
    /// <param name="association">The exact output representation and master style.</param>
    /// <param name="retainedMasters">The exact admitted master sequence normalized onto <paramref name="mod"/>.</param>
    /// <param name="cancellationToken">A token checked around the synchronous plugin writer.</param>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    private static void WritePrivateOutput(
        SkyrimPluginSourceSet sources,
        SkyrimMod mod,
        string stagedPluginPath,
        OutputAssociation association,
        IReadOnlyList<ModKey> retainedMasters,
        CancellationToken cancellationToken)
    {
        var sourceMods = sources.GetMutagenMods();
        var masterFlags = new Cache<IModMasterStyledGetter, ModKey>(mutagenMod => mutagenMod.ModKey);
        foreach (var sourceMod in sourceMods)
        {
            cancellationToken.ThrowIfCancellationRequested();
            masterFlags.Set(sourceMod);
        }

        masterFlags.Set(new KeyedMasterStyle(
            association.ModKey,
            association.MasterStyle == OutputMasterStyle.Small
                ? MutagenMasterStyle.Small
                : MutagenMasterStyle.Full));
        var loadOrder = sourceMods.Select(sourceMod => sourceMod.ModKey)
            .Append(mod.ModKey)
            .ToArray();
        var writeParameters = new BinaryWriteParameters
        {
            MasterFlagsLookup = masterFlags,
            MastersListContent = MastersListContentOption.NoCheck,
            MastersListOrdering = new MastersListOrderingByLoadOrder(loadOrder)
            {
                Strict = true,
            },
            RecordCount = RecordCountOption.NoCheck,
            NextFormID = NextFormIDOption.NoCheck,
        };

        var actualMasters = ((IModGetter)mod).MasterReferences.Select(reference => reference.Master).ToArray();
        if (!actualMasters.SequenceEqual(retainedMasters))
        {
            throw new InvalidOperationException(
                "The normalized Skyrim master list changed before Mutagen serialization.");
        }

        cancellationToken.ThrowIfCancellationRequested();
        if (association.LocalizedOutputMode == LocalizedOutputMode.SeparateStringFiles)
        {
            var stringsDirectory = Path.Combine(Path.GetDirectoryName(stagedPluginPath)!, "Strings");
            Directory.CreateDirectory(stringsDirectory);
            using (var stringsWriter = new StringsWriter(
                GameRelease.SkyrimSE,
                association.ModKey,
                stringsDirectory,
                new SkyrimPluginEncodingProvider(),
                new FileSystem()))
            {
                ((IModGetter)mod).WriteToBinary(
                    stagedPluginPath,
                    writeParameters with
                    {
                        StringsWriter = stringsWriter,
                    });
            }

            StagedPluginStringTables.EnsureExplicitTable(
                GameRelease.SkyrimSE,
                association.ModKey,
                stringsDirectory,
                sources.BorrowInputs().RecordTextLanguage,
                cancellationToken);
        }
        else
        {
            ((IModGetter)mod).WriteToBinary(stagedPluginPath, writeParameters);
        }

        cancellationToken.ThrowIfCancellationRequested();
    }

    /// <summary>Validates complete candidate identity, provenance, and trackable FormList mutation coverage.</summary>
    /// <param name="sources">The immutable source lifetime.</param>
    /// <param name="output">The selected output identity owner.</param>
    /// <param name="candidate">The detached complete current candidate.</param>
    /// <param name="original">The detached complete selection-time output.</param>
    /// <param name="provenance">The immutable staged-edit provenance entries.</param>
    /// <param name="cancellationToken">A token observed throughout Mutagen traversal.</param>
    /// <returns>A typed validation failure, or <see langword="null"/>.</returns>
    private (EngineErrorCode Code, string Message)? ValidateCandidate(
        SkyrimPluginSourceSet sources,
        SkyrimPluginOutputState output,
        SkyrimMod candidate,
        SkyrimMod original,
        IReadOnlyList<RecordEditProvenance> provenance,
        CancellationToken cancellationToken)
    {
        var expectedMaster = original.IsMaster;
        var expectedSmall = output.Association.MasterStyle == OutputMasterStyle.Small;
        var expectedLocalization = output.Association.LocalizedOutputMode == LocalizedOutputMode.SeparateStringFiles;
        if (candidate.ModKey != output.Association.ModKey
            || candidate.IsMaster != expectedMaster
            || candidate.IsSmallMaster != expectedSmall
            || candidate.IsMediumMaster
            || candidate.UsingLocalization != expectedLocalization)
        {
            return (
                EngineErrorCode.ValidationFailed,
                "The complete Skyrim candidate identity or plugin header flags differ from the selected output.");
        }

        var allKeys = new HashSet<FormKey>();
        foreach (var record in candidate.EnumerateMajorRecords())
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!allKeys.Add(record.FormKey))
            {
                return (
                    EngineErrorCode.ValidationFailed,
                    $"Skyrim candidate identity '{record.FormKey}' is duplicated across record families.");
            }
        }

        var provenanceKeys = new HashSet<FormKey>();
        foreach (var entry in provenance)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!provenanceKeys.Add(entry.TargetFormKey))
            {
                return (
                    EngineErrorCode.ValidationFailed,
                    $"Skyrim edit provenance contains duplicate target '{entry.TargetFormKey}'.");
            }

            var matches = candidate.EnumerateMajorRecords()
                .Where(record => record.FormKey == entry.TargetFormKey)
                .ToArray();
            if (matches.Length != 1 || matches[0] is not IFormListGetter)
            {
                return (
                    EngineErrorCode.ValidationFailed,
                    $"Skyrim edit provenance target '{entry.TargetFormKey}' is missing, duplicated, or belongs to another Mutagen family.");
            }

            if (entry.BaselineKind == EditBaselineKind.SourceContext
                && entry.SourceBaselineId != sources.Baseline.BaselineId)
            {
                return (
                    EngineErrorCode.ValidationFailed,
                    $"Skyrim edit provenance target '{entry.TargetFormKey}' refers to another source baseline.");
            }
        }

        var originalByKey = original.FormLists.ToDictionary(record => record.FormKey);
        var candidateByKey = candidate.FormLists.ToDictionary(record => record.FormKey);
        foreach (var formKey in originalByKey.Keys.Union(candidateByKey.Keys))
        {
            cancellationToken.ThrowIfCancellationRequested();
            originalByKey.TryGetValue(formKey, out var before);
            candidateByKey.TryGetValue(formKey, out var after);
            if ((before is null
                    || after is null
                    || Inspector.Compare(before, after, cancellationToken).Count != 0)
                && !provenanceKeys.Contains(formKey))
            {
                return (
                    EngineErrorCode.ValidationFailed,
                    $"Skyrim FormList '{formKey}' changed without exact staged-edit provenance.");
            }
        }

        return null;
    }

    /// <summary>Applies only verified writer-owned record-count and retained-master normalization to a detached expected copy.</summary>
    /// <param name="sources">The exact admitted source load order.</param>
    /// <param name="expected">The detached expected plugin output to normalize.</param>
    /// <param name="cancellationToken">A token observed throughout reference traversal.</param>
    /// <returns>The retained exact master sequence or a typed missing-master failure.</returns>
    private static EngineResult<IReadOnlyList<ModKey>> NormalizeWriterOwnedHeader(
        SkyrimPluginSourceSet sources,
        SkyrimMod expected,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var existingMasters = ((IModGetter)expected).MasterReferences.ToDictionary(
            reference => reference.Master,
            reference => new MasterReference
            {
                Master = reference.Master,
                FileSize = reference.FileSize,
            });
        var required = new HashSet<ModKey>(existingMasters.Keys);
        foreach (var record in expected.EnumerateMajorRecords())
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (record.FormKey.ModKey != expected.ModKey)
            {
                required.Add(record.FormKey.ModKey);
            }
        }

        foreach (var link in expected.EnumerateFormLinks(false))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (link.FormKeyNullable is { } formKey
                && !formKey.IsNull
                && formKey.ModKey != expected.ModKey)
            {
                required.Add(formKey.ModKey);
            }
        }

        var sourceModKeys = sources.GetMutagenMods().Select(mod => mod.ModKey).ToArray();
        var sourceModKeySet = sourceModKeys.ToHashSet();
        var missing = required.Where(master => !sourceModKeySet.Contains(master)).ToArray();
        if (missing.Length != 0)
        {
            return EngineResult<IReadOnlyList<ModKey>>.Failure(new EngineError(
                EngineErrorCode.MissingMaster,
                $"Skyrim candidate requires an unadmitted master: {string.Join(", ", missing.Select(master => master.FileName))}."));
        }

        var retained = sourceModKeys.Where(required.Contains).ToArray();
        var mutable = (IMod)expected;
        mutable.MasterReferences.Clear();
        foreach (var master in retained)
        {
            mutable.MasterReferences.Add(existingMasters.TryGetValue(master, out var existing)
                ? existing
                : new MasterReference
                {
                    Master = master,
                    FileSize = 0,
                });
        }

        expected.SyncRecordCount();
        return EngineResult<IReadOnlyList<ModKey>>.Success(Array.AsReadOnly(retained));
    }

    /// <summary>Validates complete Mutagen equality, exact master retention, and all-field FormList equality after strict reopen.</summary>
    /// <param name="expected">The detached normalized output supplied to the writer.</param>
    /// <param name="reopened">The strictly reopened staged output.</param>
    /// <param name="retainedMasters">The exact expected admitted master sequence.</param>
    /// <param name="cancellationToken">A token observed throughout Mutagen comparison.</param>
    /// <returns>A typed preservation failure, or <see langword="null"/>.</returns>
    private (EngineErrorCode Code, string Message)? ValidateReopened(
        SkyrimMod expected,
        SkyrimMod reopened,
        IReadOnlyList<ModKey> retainedMasters,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var reopenedMasters = ((IModGetter)reopened).MasterReferences.Select(reference => reference.Master).ToArray();
        if (!reopenedMasters.SequenceEqual(retainedMasters))
        {
            return (
                EngineErrorCode.ValidationFailed,
                "The staged Skyrim output did not retain the exact admitted master order and content.");
        }

        if (!SkyrimModMixIn.Equals(expected, reopened, equalsMask: null))
        {
            return (
                EngineErrorCode.ValidationFailed,
                "The staged Skyrim output changed one or more complete record fields during serialization or strict reopen.");
        }

        if (!FormListsEqual(expected, reopened, cancellationToken))
        {
            return (
                EngineErrorCode.ValidationFailed,
                "The staged Skyrim output changed one or more FormList fields during serialization or strict reopen.");
        }

        return null;
    }

    /// <summary>Compares complete Mutagen state and every FormList field through the shared Skyrim inspector.</summary>
    /// <param name="expected">The complete expected plugin output.</param>
    /// <param name="actual">The complete actual plugin output.</param>
    /// <param name="cancellationToken">A token observed throughout FormList comparison.</param>
    /// <returns><see langword="true"/> only when the complete model and every FormList field are equal.</returns>
    private bool ArePluginOutputsEquivalent(
        ISkyrimModGetter expected,
        ISkyrimModGetter actual,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!SkyrimModMixIn.Equals(expected, actual, equalsMask: null))
        {
            return false;
        }

        return FormListsEqual(expected, actual, cancellationToken);
    }

    /// <summary>Compares every FormList in Mutagen order through the explicit all-field inspector.</summary>
    /// <param name="left">The first complete plugin output.</param>
    /// <param name="right">The second complete plugin output.</param>
    /// <param name="cancellationToken">A token observed throughout record comparison.</param>
    /// <returns><see langword="true"/> when every FormList identity and inspected field is semantically equal.</returns>
    private bool FormListsEqual(
        ISkyrimModGetter left,
        ISkyrimModGetter right,
        CancellationToken cancellationToken)
    {
        var leftRecords = left.FormLists.ToArray();
        var rightRecords = right.FormLists.ToArray();
        if (leftRecords.Length != rightRecords.Length)
        {
            return false;
        }

        for (var index = 0; index < leftRecords.Length; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (leftRecords[index].FormKey != rightRecords[index].FormKey
                || Inspector.Compare(leftRecords[index], rightRecords[index], cancellationToken).Count != 0)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>Rejects unrelated records from the bounded new localized-output preservation surface.</summary>
    /// <param name="mod">The complete new localized output.</param>
    /// <param name="cancellationToken">A token observed for every record.</param>
    /// <returns><see langword="true"/> when every Mutagen major record is a supported FormList.</returns>
    private static bool ContainsOnlySupportedLocalizedRecords(
        ISkyrimModGetter mod,
        CancellationToken cancellationToken)
    {
        foreach (var record in mod.EnumerateMajorRecords())
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (record is not IFormListGetter)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>Accepts only the explicit empty English table needed to reopen a localized output with no translated fields.</summary>
    /// <param name="baseline">The complete artifact baseline captured by strict Mutagen staged-output admission.</param>
    /// <returns><see langword="true"/> only when the sole present sidecar is the exact eight-byte empty English STRINGS table.</returns>
    private static bool ContainsOnlyEmptyLocalizedTable(OutputArtifactSetBaseline baseline)
    {
        var sidecars = baseline.Artifacts
            .Where(artifact => artifact.Role != PluginArtifactRole.Plugin && artifact.Fingerprint.Exists)
            .ToArray();
        return sidecars.Length == 1
            && sidecars[0].Role == PluginArtifactRole.Strings
            && string.Equals(sidecars[0].Language, Language.English.ToString(), StringComparison.Ordinal)
            && sidecars[0].Fingerprint.Length == 8
            && string.Equals(
                sidecars[0].Fingerprint.Sha256,
                Convert.ToHexString(SHA256.HashData(new byte[8])),
                StringComparison.Ordinal);
    }

    /// <summary>Maps every staged plugin and possible sidecar observation to its exact current destination artifact.</summary>
    /// <param name="destination">The workspace's current complete destination baseline.</param>
    /// <param name="staged">The complete strictly reopened staged baseline.</param>
    /// <returns>Every explicit staged write or intended sidecar deletion mapping, or a typed ambiguity failure.</returns>
    private static EngineResult<IReadOnlyList<StagedPluginArtifactMapping>> CreateArtifactMappings(
        OutputArtifactSetBaseline destination,
        OutputArtifactSetBaseline staged)
    {
        var mappings = new List<StagedPluginArtifactMapping>(staged.Artifacts.Count);
        foreach (var stagedArtifact in staged.Artifacts)
        {
            var destinations = destination.Artifacts
                .Where(candidate => candidate.Role == stagedArtifact.Role
                    && string.Equals(
                        candidate.Language,
                        stagedArtifact.Language,
                        StringComparison.OrdinalIgnoreCase))
                .ToArray();
            if (destinations.Length != 1)
            {
                return EngineResult<IReadOnlyList<StagedPluginArtifactMapping>>.Failure(new EngineError(
                    EngineErrorCode.ValidationFailed,
                    $"The staged Skyrim artifact '{stagedArtifact.Path}' has no unique current destination mapping."));
            }

            mappings.Add(new StagedPluginArtifactMapping(stagedArtifact, destinations[0].Path));
        }

        if (mappings.Count == 0)
        {
            return EngineResult<IReadOnlyList<StagedPluginArtifactMapping>>.Failure(new EngineError(
                EngineErrorCode.ValidationFailed,
                "The staged Skyrim output produced no verified artifacts."));
        }

        return EngineResult<IReadOnlyList<StagedPluginArtifactMapping>>.Success(
            Array.AsReadOnly(mappings.ToArray()));
    }

    /// <summary>Validates that the write request still identifies the selected output exactly.</summary>
    /// <param name="selected">The selected canonical output identity.</param>
    /// <param name="requested">The output identity carried by the save coordinator.</param>
    /// <returns>A typed failure when any identity or representation differs; otherwise <see langword="null"/>.</returns>
    private static (EngineErrorCode Code, string Message)? ValidateWriteAssociation(
        OutputAssociation selected,
        OutputAssociation requested)
    {
        var comparison = OperatingSystem.IsWindows()
            ? StringComparison.OrdinalIgnoreCase
            : StringComparison.Ordinal;
        if (selected.ModKey != requested.ModKey
            || selected.LocalizedOutputMode != requested.LocalizedOutputMode
            || selected.MasterStyle != requested.MasterStyle
            || !string.Equals(
                Path.GetFullPath(selected.PluginPath),
                Path.GetFullPath(requested.PluginPath),
                comparison))
        {
            return (
                EngineErrorCode.InvalidRequest,
                "The Skyrim staged-write request does not match the exact selected output association.");
        }

        return null;
    }

    /// <summary>Validates the coordinator-owned staging directory without changing it.</summary>
    /// <param name="stagingDirectoryPath">The canonical private staging directory.</param>
    /// <param name="destinationPluginPath">The selected canonical destination plugin path.</param>
    /// <returns>A typed failure when the directory is absent, aliases the destination directory, or is not empty; otherwise <see langword="null"/>.</returns>
    private static (EngineErrorCode Code, string Message)? ValidateStagingDirectory(
        string stagingDirectoryPath,
        string destinationPluginPath)
    {
        if (!Directory.Exists(stagingDirectoryPath))
        {
            return (
                EngineErrorCode.InvalidRequest,
                $"The private Skyrim staging directory does not exist: '{stagingDirectoryPath}'.");
        }

        var comparison = OperatingSystem.IsWindows()
            ? StringComparison.OrdinalIgnoreCase
            : StringComparison.Ordinal;
        if (string.Equals(
            Path.TrimEndingDirectorySeparator(stagingDirectoryPath),
            Path.TrimEndingDirectorySeparator(Path.GetDirectoryName(destinationPluginPath)!),
            comparison))
        {
            return (
                EngineErrorCode.InvalidRequest,
                "The private Skyrim staging directory must differ from the destination directory.");
        }

        if (Directory.EnumerateFileSystemEntries(stagingDirectoryPath).Any())
        {
            return (
                EngineErrorCode.InvalidRequest,
                "The private Skyrim staging directory must be empty before Mutagen writing begins.");
        }

        return null;
    }

    /// <summary>Creates a successful staged-write result bound to the source lifetime.</summary>
    /// <param name="sources">The typed source lifetime.</param>
    /// <param name="value">The staged-write result.</param>
    /// <param name="warnings">Optional nonfatal warnings.</param>
    /// <returns>A successful source-bound result.</returns>
    private static EngineResult<StagedPluginOutputSet> WriteSuccess(
        SkyrimPluginSourceSet sources,
        StagedPluginOutputSet value,
        IReadOnlyList<EngineWarning>? warnings = null)
    {
        return EngineResult<StagedPluginOutputSet>.Success(
            value,
            workspaceId: sources.WorkspaceId,
            resultRevision: sources.Revision,
            warnings: warnings);
    }

    /// <summary>Creates a typed private-write failure bound to the source lifetime.</summary>
    /// <param name="sources">The typed source lifetime.</param>
    /// <param name="code">The stable adapter failure category.</param>
    /// <param name="message">The actionable failure description.</param>
    /// <param name="warnings">Optional warnings produced before the failure.</param>
    /// <returns>The typed staged-output failure.</returns>
    private static EngineResult<StagedPluginOutputSet> WriteFailure(
        SkyrimPluginSourceSet sources,
        EngineErrorCode code,
        string message,
        IReadOnlyList<EngineWarning>? warnings = null)
    {
        return EngineResult<StagedPluginOutputSet>.Failure(
            new EngineError(code, message),
            workspaceId: sources.WorkspaceId,
            resultRevision: sources.Revision,
            warnings: warnings ?? Array.Empty<EngineWarning>());
    }

    /// <summary>Supplies Mutagen's registered Skyrim encodings to the public localized strings writer.</summary>
    private sealed class SkyrimPluginEncodingProvider : IMutagenEncodingProvider
    {
        /// <summary>Gets the Mutagen encoding registered for one Skyrim release and language.</summary>
        /// <param name="release">The Mutagen game release.</param>
        /// <param name="language">The localized strings language.</param>
        /// <returns>The registered Mutagen encoding.</returns>
        public IMutagenEncoding GetEncoding(GameRelease release, Language language)
        {
            return MutagenEncoding.GetEncoding(release, language);
        }
    }
}
