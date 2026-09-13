using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeInspection;
using CreationsForge.Core.Engine.NativeOutputs;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Starfield;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Strings;
using Mutagen.Bethesda.Strings.DI;
using Noggog;
using System.IO.Abstractions;
using NativeMasterStyle = Mutagen.Bethesda.Plugins.MasterStyle;

namespace CreationsForge.Starfield.Native;

/// <summary>Writes complete Starfield output state only into private staging and proves its native semantic preservation before commit.</summary>
public sealed class StarfieldNativeWriter
{
    /// <summary>The typed output service used for destination freshness checks and strict staged reopen.</summary>
    private readonly StarfieldNativeOutputService _outputService;

    /// <summary>The complete multilingual FormList comparer.</summary>
    private readonly IFormListNativeInspector _inspector;

    /// <summary>Initializes the Starfield private staged-write boundary.</summary>
    /// <param name="outputService">The typed native output service used for strict reopen.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="outputService"/> is <see langword="null"/>.</exception>
    public StarfieldNativeWriter(StarfieldNativeOutputService outputService)
    {
        ArgumentNullException.ThrowIfNull(outputService);
        _outputService = outputService;
        _inspector = outputService.Inspector;
    }

    /// <summary>Writes a complete private plugin-and-strings set and strictly reopens it before returning any destination mappings.</summary>
    /// <param name="sources">The borrowed immutable Starfield source lifetime.</param>
    /// <param name="output">The borrowed complete staged output state.</param>
    /// <param name="request">The private staging directory and exact selected output identity.</param>
    /// <param name="cancellationToken">A token observed before and after synchronous native serialization and throughout validation.</param>
    /// <returns>A zero-write unchanged result or an independently owned validated staged artifact set.</returns>
    /// <exception cref="ArgumentNullException">Thrown when a required argument is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public async ValueTask<EngineResult<NativeStagedOutputSet>> WriteAndValidateAsync(
        StarfieldNativeSourceSet sources,
        StarfieldNativeOutputState output,
        NativeWriteRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(sources);
        ArgumentNullException.ThrowIfNull(output);
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        var associationError = ValidateAssociation(output.Association, request.Output);
        if (associationError is not null)
        {
            return Failure(sources, associationError);
        }

        var freshness = await _outputService.ReopenAsync(
            sources,
            output.Association,
            request.ExpectedOutputBaseline,
            cancellationToken).ConfigureAwait(false);
        if (!freshness.Succeeded || freshness.Value is null)
        {
            return Failure(
                sources,
                freshness.Error ?? new EngineError(EngineErrorCode.ExternalChangeDetected, "The Starfield destination could not be revalidated before staging."),
                freshness.Warnings);
        }

        await freshness.Value.Output.DisposeAsync().ConfigureAwait(false);
        cancellationToken.ThrowIfCancellationRequested();

        StarfieldMod candidate;
        StarfieldMod original;
        IReadOnlyList<NativeEditProvenance> provenance;
        try
        {
            candidate = (StarfieldMod)output.CreateSnapshot(cancellationToken);
            original = output.CreateOriginalSnapshot(cancellationToken);
            provenance = output.GetEditProvenance();
        }
        catch (ObjectDisposedException exception)
        {
            return Failure(sources, new EngineError(EngineErrorCode.WorkspaceDisposed, exception.Message));
        }

        var candidateError = ValidateCandidate(sources, output, candidate, original, provenance, cancellationToken);
        if (candidateError is not null)
        {
            return Failure(sources, candidateError);
        }

        var pluginExisted = request.ExpectedOutputBaseline.Artifacts.Single(
            artifact => artifact.Role == NativeArtifactRole.Plugin).Fingerprint.Exists;
        if (pluginExisted
            && StarfieldModMixIn.Equals(candidate, original)
            && FormListsEqual(candidate, original, cancellationToken))
        {
            return Success(sources, new NativeStagedOutputSet(
                NativeWriteDisposition.Unchanged,
                stagedOutput: null,
                artifactMappings: Array.Empty<NativeStagedArtifactMapping>()));
        }

        if (pluginExisted && output.Association.LocalizedOutputMode == LocalizedOutputMode.SeparateStringFiles)
        {
            return Failure(sources, new EngineError(
                EngineErrorCode.UnsupportedInput,
                $"Existing localized Starfield output '{output.Association.ModKey.FileName}' cannot be materially rewritten without risking untranslated string-key preservation."));
        }

        string stagingDirectory;
        try
        {
            stagingDirectory = Path.GetFullPath(request.StagingDirectoryPath);
        }
        catch (Exception exception) when (
            exception is ArgumentException or NotSupportedException or PathTooLongException)
        {
            return Failure(sources, new EngineError(
                EngineErrorCode.InvalidRequest,
                $"The Starfield private staging directory is invalid: {exception.Message}"));
        }

        var stagingError = ValidateStagingDirectory(stagingDirectory, output.Association);
        if (stagingError is not null)
        {
            return Failure(sources, stagingError);
        }

        var expected = (StarfieldMod)((IModGetter)candidate).DeepCopy();
        var masterResult = NormalizeWriterOwnedHeader(sources, expected, cancellationToken);
        if (!masterResult.Succeeded)
        {
            return Failure(sources, masterResult.Error!);
        }

        if (output.Association.LocalizedOutputMode == LocalizedOutputMode.SeparateStringFiles
            && expected.EnumerateMajorRecords().Any(record => record is not IFormListGetter))
        {
            return Failure(sources, new EngineError(
                EngineErrorCode.UnsupportedInput,
                $"Localized Starfield output '{output.Association.ModKey.FileName}' contains a native record family whose complete multilingual preservation is not supported."));
        }

        var stagedPluginPath = Path.GetFullPath(Path.Combine(stagingDirectory, output.Association.ModKey.FileName));
        var stagedAssociation = new OutputAssociation(
            stagedPluginPath,
            output.Association.ModKey,
            output.Association.LocalizedOutputMode,
            output.Association.MasterStyle);

        try
        {
            WriteNativeOutput(sources, expected, stagedAssociation, masterResult.Value!, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            return Failure(sources, new EngineError(
                EngineErrorCode.OutputOpenFailed,
                $"Starfield native serialization into private staging failed: {exception.Message}"));
        }

        cancellationToken.ThrowIfCancellationRequested();
        var stagedOpen = await _outputService.OpenAsync(
            sources,
            new SelectOutputRequest(
                Guid.NewGuid(),
                sources.Revision,
                OutputSelectionMode.OpenExisting,
                stagedAssociation),
            cancellationToken).ConfigureAwait(false);
        if (!stagedOpen.Succeeded || stagedOpen.Value is null)
        {
            return Failure(
                sources,
                stagedOpen.Error ?? new EngineError(EngineErrorCode.OutputOpenFailed, "The staged Starfield output could not be reopened natively."),
                stagedOpen.Warnings);
        }

        var stagedState = (StarfieldNativeOutputState)stagedOpen.Value.Output;
        var valid = false;
        try
        {
            var reopened = stagedState.CreateSnapshot(cancellationToken);
            var validationError = ValidateReopened(
                expected,
                reopened,
                masterResult.Value!,
                cancellationToken);
            if (validationError is not null)
            {
                return Failure(sources, validationError, stagedOpen.Warnings);
            }

            var mappingResult = CreateMappings(
                request.ExpectedOutputBaseline,
                stagedOpen.Value.Baseline);
            if (!mappingResult.Succeeded)
            {
                return Failure(sources, mappingResult.Error!, stagedOpen.Warnings);
            }

            var stagedLifetime = new StarfieldNativeStagedOutputSet(stagedState);
            valid = true;
            return Success(
                sources,
                new NativeStagedOutputSet(
                    NativeWriteDisposition.StagedChanges,
                    stagedLifetime,
                    mappingResult.Value!),
                stagedOpen.Warnings);
        }
        finally
        {
            if (!valid)
            {
                await stagedState.DisposeAsync().ConfigureAwait(false);
            }
        }
    }

    /// <summary>Validates that the write request still identifies the exact selected output.</summary>
    /// <param name="selected">The canonical selected output.</param>
    /// <param name="requested">The output carried by the save coordinator request.</param>
    /// <returns>A typed mismatch failure, or <see langword="null"/>.</returns>
    private static EngineError? ValidateAssociation(OutputAssociation selected, OutputAssociation requested)
    {
        var comparison = OperatingSystem.IsWindows()
            ? StringComparison.OrdinalIgnoreCase
            : StringComparison.Ordinal;
        return selected.ModKey == requested.ModKey
            && selected.LocalizedOutputMode == requested.LocalizedOutputMode
            && selected.MasterStyle == requested.MasterStyle
            && string.Equals(selected.PluginPath, requested.PluginPath, comparison)
                ? null
                : new EngineError(
                    EngineErrorCode.InvalidRequest,
                    "The Starfield staged-write request does not match the selected output association.");
    }

    /// <summary>Validates complete candidate identity, provenance, and trackable FormList mutation coverage.</summary>
    /// <param name="sources">The immutable source lifetime.</param>
    /// <param name="output">The selected output identity owner.</param>
    /// <param name="candidate">The detached complete current candidate.</param>
    /// <param name="original">The detached complete selection-time output.</param>
    /// <param name="provenance">The immutable staged-edit provenance entries.</param>
    /// <param name="cancellationToken">A token observed throughout native traversal.</param>
    /// <returns>A typed validation failure, or <see langword="null"/>.</returns>
    private EngineError? ValidateCandidate(
        StarfieldNativeSourceSet sources,
        StarfieldNativeOutputState output,
        StarfieldMod candidate,
        StarfieldMod original,
        IReadOnlyList<NativeEditProvenance> provenance,
        CancellationToken cancellationToken)
    {
        var expectedMaster = original.IsMaster;
        var expectedSmall = output.Association.MasterStyle == OutputMasterStyle.Small;
        var expectedMedium = output.Association.MasterStyle == OutputMasterStyle.Medium;
        var expectedLocalization = output.Association.LocalizedOutputMode == LocalizedOutputMode.SeparateStringFiles;
        if (candidate.ModKey != output.Association.ModKey
            || candidate.IsMaster != expectedMaster
            || candidate.IsSmallMaster != expectedSmall
            || candidate.IsMediumMaster != expectedMedium
            || candidate.UsingLocalization != expectedLocalization)
        {
            return new EngineError(
                EngineErrorCode.ValidationFailed,
                $"The Starfield candidate differs from its selected output: ModKey '{candidate.ModKey}'/'{output.Association.ModKey}', master {candidate.IsMaster}/{expectedMaster}, small {candidate.IsSmallMaster}/{expectedSmall}, medium {candidate.IsMediumMaster}/{expectedMedium}, localized {candidate.UsingLocalization}/{expectedLocalization} (actual/expected).");
        }

        var allKeys = new HashSet<FormKey>();
        foreach (var record in candidate.EnumerateMajorRecords())
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!allKeys.Add(record.FormKey))
            {
                return new EngineError(
                    EngineErrorCode.ValidationFailed,
                    $"Starfield candidate identity '{record.FormKey}' is duplicated across native record families.");
            }
        }

        var provenanceKeys = new HashSet<FormKey>();
        foreach (var entry in provenance)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!provenanceKeys.Add(entry.TargetFormKey))
            {
                return new EngineError(
                    EngineErrorCode.ValidationFailed,
                    $"Starfield edit provenance contains duplicate target '{entry.TargetFormKey}'.");
            }

            var matches = candidate.EnumerateMajorRecords()
                .Where(record => record.FormKey == entry.TargetFormKey)
                .ToArray();
            if (matches.Length != 1 || matches[0] is not IFormListGetter)
            {
                return new EngineError(
                    EngineErrorCode.ValidationFailed,
                    $"Starfield edit provenance target '{entry.TargetFormKey}' is missing, duplicated, or belongs to another native family.");
            }

            if (entry.BaselineKind == EditBaselineKind.SourceContext
                && entry.SourceBaselineId != sources.Baseline.BaselineId)
            {
                return new EngineError(
                    EngineErrorCode.ValidationFailed,
                    $"Starfield edit provenance target '{entry.TargetFormKey}' refers to another source baseline.");
            }

            var provenanceError = ValidateProvenanceContext(sources, output, original, entry);
            if (provenanceError is not null)
            {
                return provenanceError;
            }
        }

        var originalByKey = original.FormLists.ToDictionary(record => record.FormKey);
        var candidateByKey = candidate.FormLists.ToDictionary(record => record.FormKey);
        foreach (var formKey in originalByKey.Keys.Union(candidateByKey.Keys))
        {
            cancellationToken.ThrowIfCancellationRequested();
            originalByKey.TryGetValue(formKey, out var before);
            candidateByKey.TryGetValue(formKey, out var after);
            if ((before is null || after is null || _inspector.Compare(before, after, cancellationToken).Count != 0)
                && !provenanceKeys.Contains(formKey))
            {
                return new EngineError(
                    EngineErrorCode.ValidationFailed,
                    $"Starfield FormList '{formKey}' changed without exact staged-edit provenance.");
            }
        }

        return null;
    }

    /// <summary>Validates that one retained first-edit baseline still identifies its exact original native context.</summary>
    /// <param name="sources">The immutable source lifetime and admitted plugin provenance.</param>
    /// <param name="output">The selected output identity and path.</param>
    /// <param name="original">The detached complete output as it appeared at selection.</param>
    /// <param name="entry">The retained first-edit baseline to validate.</param>
    /// <returns>A typed provenance failure, or <see langword="null"/>.</returns>
    private static EngineError? ValidateProvenanceContext(
        StarfieldNativeSourceSet sources,
        StarfieldNativeOutputState output,
        IStarfieldModGetter original,
        NativeEditProvenance entry)
    {
        if (entry.BaselineKind == EditBaselineKind.Absent)
        {
            return null;
        }

        var context = entry.BaselineContext!;
        if (entry.BaselineKind == EditBaselineKind.OriginalOutput)
        {
            var originalMatches = original.FormLists
                .Where(record => record.FormKey == entry.TargetFormKey)
                .ToArray();
            if (originalMatches.Length != 1
                || context.Status != (originalMatches[0].IsDeleted
                    ? ReferenceResolutionStatus.Deleted
                    : ReferenceResolutionStatus.Resolved)
                || context.ContainingModKey != output.Association.ModKey
                || context.LoadOrderIndex != sources.GetNativeMods().Count
                || context.Role != PluginRole.Output
                || !PathsEqual(context.Path!, output.Association.PluginPath))
            {
                return new EngineError(
                    EngineErrorCode.ValidationFailed,
                    $"Starfield edit provenance target '{entry.TargetFormKey}' does not identify its exact original output context.");
            }

            return null;
        }

        var plugins = sources.BorrowInputs().Plugins;
        var mods = sources.GetNativeMods();
        var matchingSources = plugins.Select((plugin, index) => (plugin, index)).Count(candidate =>
            candidate.plugin.ModKey == context.ContainingModKey
            && candidate.plugin.LoadOrderIndex == context.LoadOrderIndex
            && candidate.plugin.Role == context.Role
            && PathsEqual(candidate.plugin.Path, context.Path!)
            && mods[candidate.index].FormLists.Any(record =>
                record.FormKey == entry.TargetFormKey
                && context.Status == (record.IsDeleted
                    ? ReferenceResolutionStatus.Deleted
                    : ReferenceResolutionStatus.Resolved)));
        return matchingSources == 1
            ? null
            : new EngineError(
                EngineErrorCode.ValidationFailed,
                $"Starfield edit provenance target '{entry.TargetFormKey}' does not identify one exact admitted source context.");
    }

    /// <summary>Validates the private staging directory without creating or removing coordinator-owned state.</summary>
    /// <param name="stagingDirectory">The canonical staging directory.</param>
    /// <param name="association">The selected destination association.</param>
    /// <returns>A typed staging failure, or <see langword="null"/>.</returns>
    private static EngineError? ValidateStagingDirectory(string stagingDirectory, OutputAssociation association)
    {
        if (!Path.IsPathFullyQualified(stagingDirectory) || !Directory.Exists(stagingDirectory))
        {
            return new EngineError(
                EngineErrorCode.InvalidRequest,
                $"The Starfield private staging directory must already exist: '{stagingDirectory}'.");
        }

        var stagedPluginPath = Path.GetFullPath(Path.Combine(stagingDirectory, association.ModKey.FileName));
        var comparison = OperatingSystem.IsWindows()
            ? StringComparison.OrdinalIgnoreCase
            : StringComparison.Ordinal;
        if (string.Equals(stagedPluginPath, association.PluginPath, comparison))
        {
            return new EngineError(
                EngineErrorCode.InvalidRequest,
                "The Starfield private staged plugin path overlaps its destination.");
        }

        if (Directory.EnumerateFileSystemEntries(stagingDirectory).Any())
        {
            return new EngineError(
                EngineErrorCode.InvalidRequest,
                $"The Starfield private staging directory is not empty: '{stagingDirectory}'.");
        }

        return null;
    }

    /// <summary>Applies only verified writer-owned record-count and retained-master normalizations to a detached expected copy.</summary>
    /// <param name="sources">The exact admitted source load order.</param>
    /// <param name="expected">The detached expected native output to normalize.</param>
    /// <param name="cancellationToken">A token observed throughout native reference traversal.</param>
    /// <returns>The retained exact master sequence or a typed missing-master failure.</returns>
    private static EngineResult<IReadOnlyList<ModKey>> NormalizeWriterOwnedHeader(
        StarfieldNativeSourceSet sources,
        StarfieldMod expected,
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

        var sourceModKeys = sources.GetNativeMods().Select(mod => mod.ModKey).ToArray();
        var missing = required.Where(master => !sourceModKeys.Contains(master)).ToArray();
        if (missing.Length != 0)
        {
            return EngineResult<IReadOnlyList<ModKey>>.Failure(new EngineError(
                EngineErrorCode.MissingMaster,
                $"Starfield candidate requires an unadmitted master: {string.Join(", ", missing.Select(master => master.FileName))}."));
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
                });
        }

        expected.SyncRecordCount();
        return EngineResult<IReadOnlyList<ModKey>>.Success(Array.AsReadOnly(retained));
    }

    /// <summary>Writes one detached normalized output using strict admitted master ordering and optional public strings serialization.</summary>
    /// <param name="sources">The exact admitted source load order and master-style authority.</param>
    /// <param name="expected">The detached normalized output.</param>
    /// <param name="association">The private staged output association.</param>
    /// <param name="retainedMasters">The exact retained master sequence.</param>
    /// <param name="cancellationToken">A token checked immediately before and after synchronous native serialization.</param>
    private static void WriteNativeOutput(
        StarfieldNativeSourceSet sources,
        StarfieldMod expected,
        OutputAssociation association,
        IReadOnlyList<ModKey> retainedMasters,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var masterFlags = new Cache<IModMasterStyledGetter, ModKey>(metadata => metadata.ModKey);
        foreach (var source in sources.GetNativeMods())
        {
            masterFlags.Set(source);
        }

        masterFlags.Set(new KeyedMasterStyle(
            association.ModKey,
            association.MasterStyle switch
            {
                OutputMasterStyle.Full => NativeMasterStyle.Full,
                OutputMasterStyle.Small => NativeMasterStyle.Small,
                OutputMasterStyle.Medium => NativeMasterStyle.Medium,
                _ => throw new ArgumentOutOfRangeException(nameof(association)),
            }));
        var loadOrder = sources.GetNativeMods()
            .Select(mod => mod.ModKey)
            .Append(association.ModKey)
            .ToArray();
        var parameters = new BinaryWriteParameters
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

        var actualMasters = ((IModGetter)expected).MasterReferences.Select(reference => reference.Master).ToArray();
        if (!actualMasters.SequenceEqual(retainedMasters))
        {
            throw new InvalidOperationException("The normalized Starfield master list changed before native serialization.");
        }

        if (association.LocalizedOutputMode == LocalizedOutputMode.SeparateStringFiles)
        {
            var stringsDirectory = Path.Combine(Path.GetDirectoryName(association.PluginPath)!, "Strings");
            Directory.CreateDirectory(stringsDirectory);
            using (var stringsWriter = new StringsWriter(
                GameRelease.Starfield,
                association.ModKey,
                stringsDirectory,
                new StarfieldEncodingProvider(),
                new FileSystem()))
            {
                ((IModGetter)expected).WriteToBinary(
                    association.PluginPath,
                    parameters with { StringsWriter = stringsWriter });
            }

            NativeStagedStringTables.EnsureExplicitTable(
                GameRelease.Starfield,
                association.ModKey,
                stringsDirectory,
                sources.BorrowInputs().RecordTextLanguage,
                cancellationToken);
        }
        else
        {
            ((IModGetter)expected).WriteToBinary(association.PluginPath, parameters);
        }

        cancellationToken.ThrowIfCancellationRequested();
    }

    /// <summary>Validates complete native equality, exact master retention, and multilingual FormList equality after strict reopen.</summary>
    /// <param name="expected">The detached normalized output supplied to the writer.</param>
    /// <param name="reopened">The strictly reopened staged output.</param>
    /// <param name="retainedMasters">The exact expected master sequence.</param>
    /// <param name="cancellationToken">A token observed throughout comparison.</param>
    /// <returns>A typed preservation failure, or <see langword="null"/>.</returns>
    private EngineError? ValidateReopened(
        StarfieldMod expected,
        IStarfieldModGetter reopened,
        IReadOnlyList<ModKey> retainedMasters,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var reopenedMasters = reopened.MasterReferences.Select(reference => reference.Master).ToArray();
        if (!reopenedMasters.SequenceEqual(retainedMasters))
        {
            return new EngineError(
                EngineErrorCode.ValidationFailed,
                "The staged Starfield output did not retain the exact admitted master order and content.");
        }

        if (!StarfieldModMixIn.Equals(expected, reopened))
        {
            return new EngineError(
                EngineErrorCode.ValidationFailed,
                "The staged Starfield output changed one or more complete native fields during serialization or strict reopen.");
        }

        if (!FormListsEqual(expected, reopened, cancellationToken))
        {
            return new EngineError(
                EngineErrorCode.ValidationFailed,
                "The staged Starfield output changed one or more FormList fields or nondefault-language translations.");
        }

        return null;
    }

    /// <summary>Compares every FormList in native order through the explicit all-field multilingual inspector.</summary>
    /// <param name="left">The first complete native output.</param>
    /// <param name="right">The second complete native output.</param>
    /// <param name="cancellationToken">A token observed throughout native record comparison.</param>
    /// <returns><see langword="true"/> when every FormList identity and inspected field is semantically equal.</returns>
    private bool FormListsEqual(
        IStarfieldModGetter left,
        IStarfieldModGetter right,
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
                || _inspector.Compare(leftRecords[index], rightRecords[index], cancellationToken).Count != 0)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>Maps every verified staged plugin and strings observation to its exact selected destination artifact.</summary>
    /// <param name="destination">The complete destination baseline established at output selection.</param>
    /// <param name="staged">The complete strictly reopened staged baseline.</param>
    /// <returns>Every explicit staged write or intended sidecar deletion mapping.</returns>
    private static EngineResult<IReadOnlyList<NativeStagedArtifactMapping>> CreateMappings(
        OutputArtifactSetBaseline destination,
        OutputArtifactSetBaseline staged)
    {
        var mappings = new List<NativeStagedArtifactMapping>(staged.Artifacts.Count);
        foreach (var stagedArtifact in staged.Artifacts)
        {
            var destinations = destination.Artifacts
                .Where(candidate => candidate.Role == stagedArtifact.Role
                    && string.Equals(candidate.Language, stagedArtifact.Language, StringComparison.OrdinalIgnoreCase))
                .ToArray();
            if (destinations.Length != 1)
            {
                return EngineResult<IReadOnlyList<NativeStagedArtifactMapping>>.Failure(new EngineError(
                    EngineErrorCode.ValidationFailed,
                    $"The staged Starfield artifact '{stagedArtifact.Path}' has no unique destination mapping."));
            }

            mappings.Add(new NativeStagedArtifactMapping(stagedArtifact, destinations[0].Path));
        }

        if (mappings.Count == 0)
        {
            return EngineResult<IReadOnlyList<NativeStagedArtifactMapping>>.Failure(new EngineError(
                EngineErrorCode.ValidationFailed,
                "The staged Starfield output produced no verified artifacts."));
        }

        if (mappings.Count != destination.Artifacts.Count)
        {
            return EngineResult<IReadOnlyList<NativeStagedArtifactMapping>>.Failure(new EngineError(
                EngineErrorCode.ValidationFailed,
                "The staged and destination Starfield artifact inventories do not have the same complete shape."));
        }

        return EngineResult<IReadOnlyList<NativeStagedArtifactMapping>>.Success(
            Array.AsReadOnly(mappings.ToArray()));
    }

    /// <summary>Compares canonical artifact paths under the current platform's file-name semantics.</summary>
    /// <param name="left">The first absolute path.</param>
    /// <param name="right">The second absolute path.</param>
    /// <returns><see langword="true"/> when both paths identify the same canonical artifact.</returns>
    private static bool PathsEqual(string left, string right)
    {
        return string.Equals(
            Path.GetFullPath(left),
            Path.GetFullPath(right),
            OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
    }

    /// <summary>Creates a successful staged-write result bound to the source lifetime.</summary>
    /// <param name="sources">The typed source lifetime.</param>
    /// <param name="value">The staged-write result.</param>
    /// <param name="warnings">Optional nonfatal warnings.</param>
    /// <returns>A successful source-bound result.</returns>
    private static EngineResult<NativeStagedOutputSet> Success(
        StarfieldNativeSourceSet sources,
        NativeStagedOutputSet value,
        IReadOnlyList<EngineWarning>? warnings = null)
    {
        return EngineResult<NativeStagedOutputSet>.Success(
            value,
            workspaceId: sources.WorkspaceId,
            resultRevision: sources.Revision,
            warnings: warnings);
    }

    /// <summary>Creates a failed staged-write result bound to the source lifetime.</summary>
    /// <param name="sources">The typed source lifetime.</param>
    /// <param name="error">The stable typed failure.</param>
    /// <param name="warnings">Optional nonfatal warnings.</param>
    /// <returns>A failed source-bound result.</returns>
    private static EngineResult<NativeStagedOutputSet> Failure(
        StarfieldNativeSourceSet sources,
        EngineError error,
        IReadOnlyList<EngineWarning>? warnings = null)
    {
        return EngineResult<NativeStagedOutputSet>.Failure(
            error,
            workspaceId: sources.WorkspaceId,
            resultRevision: sources.Revision,
            warnings: warnings);
    }

    /// <summary>Supplies Mutagen's public release-specific encodings to the public strings writer.</summary>
    private sealed class StarfieldEncodingProvider : IMutagenEncodingProvider
    {
        /// <summary>Returns the public Mutagen encoding for one Starfield strings language.</summary>
        /// <param name="release">The native game release.</param>
        /// <param name="language">The strings language.</param>
        /// <returns>The release-specific native text encoding.</returns>
        public IMutagenEncoding GetEncoding(GameRelease release, Language language)
        {
            return MutagenEncoding.GetEncoding(release, language);
        }
    }
}

/// <summary>Owns the strictly reopened Starfield staged output until the save coordinator completes or abandons commit.</summary>
internal sealed class StarfieldNativeStagedOutputSet : INativeStagedOutputSet
{
    /// <summary>The independently owned strictly reopened staged output.</summary>
    private StarfieldNativeOutputState? _output;

    /// <summary>Initializes ownership of one strictly reopened staged output.</summary>
    /// <param name="output">The independently owned staged output state.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="output"/> is <see langword="null"/>.</exception>
    internal StarfieldNativeStagedOutputSet(StarfieldNativeOutputState output)
    {
        ArgumentNullException.ThrowIfNull(output);
        _output = output;
    }

    /// <summary>Releases the strictly reopened staged output exactly once.</summary>
    /// <returns>A task that completes after the native staged state is disposed.</returns>
    public async ValueTask DisposeAsync()
    {
        var output = Interlocked.Exchange(ref _output, null);
        if (output is not null)
        {
            await output.DisposeAsync().ConfigureAwait(false);
        }
    }
}

