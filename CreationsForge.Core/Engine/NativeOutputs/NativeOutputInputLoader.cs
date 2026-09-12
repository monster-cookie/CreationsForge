using System.IO.Abstractions;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeInputs;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Archives;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Strings;
using Noggog;

namespace CreationsForge.Core.Engine.NativeOutputs;

/// <summary>Admits one explicit native output without creating or changing any destination artifact.</summary>
public sealed class NativeOutputInputLoader
{
    /// <summary>The localized flag shared by the supported native TES4 plugin headers.</summary>
    private const uint LocalizedHeaderFlag = 0x00000080;

    /// <summary>Compares canonical paths according to the current platform's file-name semantics.</summary>
    private static readonly StringComparer PathComparer = OperatingSystem.IsWindows()
        ? StringComparer.OrdinalIgnoreCase
        : StringComparer.Ordinal;

    /// <summary>Initializes a stateless native output-input loader.</summary>
    public NativeOutputInputLoader()
    { }

    /// <summary>Validates and prepares an independently disposable output-only input lifetime.</summary>
    /// <param name="borrowedSources">The already opened native source lifetime, which remains caller-owned.</param>
    /// <param name="association">The exact output path, identity, localization mode, and master style.</param>
    /// <param name="mode">Whether the output must be absent or must already exist.</param>
    /// <param name="cancellationToken">The token checked during source verification, output discovery, header reads, and hashing.</param>
    /// <returns>A prepared output input set or a stable typed failure.</returns>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is requested.</exception>
    public async Task<EngineResult<NativeOutputInputs>> PrepareAsync(
        NativeSourceInputs borrowedSources,
        OutputAssociation association,
        OutputSelectionMode mode,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(borrowedSources);
        ArgumentNullException.ThrowIfNull(association);
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!Enum.IsDefined(mode))
            {
                throw new NativeSourceInputException(
                    EngineErrorCode.InvalidRequest,
                    $"The output selection mode '{mode}' is undefined.");
            }

            var sourceBefore = await borrowedSources.VerifyUnchangedAsync(cancellationToken).ConfigureAwait(false);
            if (!sourceBefore.Succeeded)
            {
                return EngineResult<NativeOutputInputs>.Failure(sourceBefore.Error!);
            }

            var outputPath = CanonicalizeOutputPath(association.PluginPath);
            var outputDirectory = Path.GetDirectoryName(outputPath)!;
            if (!Directory.Exists(outputDirectory))
            {
                throw new NativeSourceInputException(
                    EngineErrorCode.OutputOpenFailed,
                    $"The output plugin directory does not exist and selection will not create it: '{outputDirectory}'.");
            }

            NativeFileInspector.VerifyDirectory(outputDirectory, "output plugin directory");
            ValidateOutputIdentity(association, outputPath, borrowedSources.Plugins);
            var initialPluginArtifact = await NativeFileInspector.InspectAsync(
                outputPath,
                NativeArtifactRole.Plugin,
                null,
                mustExist: false,
                cancellationToken).ConfigureAwait(false);
            var exists = initialPluginArtifact.Fingerprint.Exists;
            ValidateSelectionMode(mode, exists, outputPath);

            var fileSystem = new FileSystem();
            NativeOutputPluginInput output;
            if (exists)
            {
                var modPath = new ModPath(association.ModKey, outputPath);
                var header = ModHeaderFrame.FromPath(modPath, borrowedSources.Release, fileSystem);
                var usesLocalization = ((uint)header.Flags & LocalizedHeaderFlag) != 0;
                ValidateMasterStyleCapability(borrowedSources.Release, association.ModKey, header.MasterStyle);
                var expectedMasterStyle = MapMasterStyle(association.MasterStyle);
                if (header.MasterStyle != expectedMasterStyle)
                {
                    throw new NativeSourceInputException(
                        EngineErrorCode.InvalidRequest,
                        $"Existing output '{association.ModKey.FileName}' uses {header.MasterStyle} master style instead of requested {expectedMasterStyle}.");
                }

                var expectedLocalization = association.LocalizedOutputMode == LocalizedOutputMode.SeparateStringFiles;
                if (usesLocalization != expectedLocalization)
                {
                    throw new NativeSourceInputException(
                        EngineErrorCode.InvalidRequest,
                        $"Existing output '{association.ModKey.FileName}' localization state does not match {association.LocalizedOutputMode}.");
                }

                output = new NativeOutputPluginInput(
                    outputPath,
                    association.ModKey,
                    exists: true,
                    header.MasterStyle,
                    usesLocalization);
                var declaredMasters = Array.AsReadOnly(MasterReferenceCollection
                    .FromModHeader(association.ModKey, header)
                    .Masters
                    .Select(master => master.Master)
                    .ToArray());
                ValidateDeclaredMasters(output, declaredMasters, borrowedSources.Plugins);
                var pluginAfterHeader = await NativeFileInspector.InspectAsync(
                    outputPath,
                    NativeArtifactRole.Plugin,
                    null,
                    mustExist: true,
                    cancellationToken).ConfigureAwait(false);
                if (!NativeArtifactSetUtilities.Match([initialPluginArtifact], [pluginAfterHeader]))
                {
                    throw new NativeSourceInputException(
                        EngineErrorCode.ExternalChangeDetected,
                        "The existing output plugin changed while its native header was being validated.");
                }
            }
            else
            {
                var masterStyle = MapMasterStyle(association.MasterStyle);
                ValidateMasterStyleCapability(borrowedSources.Release, association.ModKey, masterStyle);
                output = new NativeOutputPluginInput(
                    outputPath,
                    association.ModKey,
                    exists: false,
                    masterStyle,
                    association.LocalizedOutputMode == LocalizedOutputMode.SeparateStringFiles);
            }

            var collector = new NativeOutputArtifactCollector(borrowedSources.Release, output);
            var initialArtifacts = await collector.CaptureAsync(cancellationToken).ConfigureAwait(false);
            var observedPlugin = initialArtifacts.Single(artifact => artifact.Role == NativeArtifactRole.Plugin);
            if (!NativeArtifactSetUtilities.Match([initialPluginArtifact], [observedPlugin]))
            {
                throw new NativeSourceInputException(
                    EngineErrorCode.ExternalChangeDetected,
                    "The explicit output plugin changed while its complete artifact set was being admitted.");
            }

            ValidateNoAliases(initialArtifacts, sourceBefore.Value!.Artifacts);
            var stringsLookup = CreateExistingLooseStringsLookup(
                borrowedSources.Release,
                output,
                initialArtifacts,
                fileSystem);

            var sourceAfter = await borrowedSources.VerifyUnchangedAsync(cancellationToken).ConfigureAwait(false);
            if (!sourceAfter.Succeeded)
            {
                return EngineResult<NativeOutputInputs>.Failure(sourceAfter.Error!);
            }

            var currentArtifacts = await collector.CaptureAsync(cancellationToken).ConfigureAwait(false);
            if (!NativeArtifactSetUtilities.Match(initialArtifacts, currentArtifacts))
            {
                throw new NativeSourceInputException(
                    EngineErrorCode.ExternalChangeDetected,
                    "The explicit native output changed while it was being admitted.");
            }

            var masterFlags = CreateMasterFlagsLookup(borrowedSources.Plugins, output);
            return EngineResult<NativeOutputInputs>.Success(new NativeOutputInputs(
                borrowedSources,
                output,
                masterFlags,
                stringsLookup,
                collector,
                currentArtifacts));
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (NativeSourceInputException exception)
        {
            var code = exception.Code == EngineErrorCode.SourceOpenFailed
                ? EngineErrorCode.OutputOpenFailed
                : exception.Code;
            return EngineResult<NativeOutputInputs>.Failure(new EngineError(code, exception.Message));
        }
        catch (ObjectDisposedException exception)
        {
            return EngineResult<NativeOutputInputs>.Failure(new EngineError(
                EngineErrorCode.SourceOpenFailed,
                $"The borrowed native source lifetime is unavailable: {exception.Message}"));
        }
        catch (Exception exception)
        {
            return EngineResult<NativeOutputInputs>.Failure(new EngineError(
                EngineErrorCode.OutputOpenFailed,
                $"The explicit native output could not be prepared: {exception.Message}"));
        }
    }

    /// <summary>Canonicalizes an output plugin path without requiring the file to exist.</summary>
    /// <param name="path">The caller-supplied output path.</param>
    /// <returns>The canonical absolute output path.</returns>
    /// <exception cref="NativeSourceInputException">Thrown when the path is empty or invalid.</exception>
    private static string CanonicalizeOutputPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new NativeSourceInputException(EngineErrorCode.InvalidRequest, "An output plugin path is required.");
        }

        try
        {
            return Path.GetFullPath(path);
        }
        catch (Exception exception) when (
            exception is ArgumentException or NotSupportedException or PathTooLongException)
        {
            throw new NativeSourceInputException(
                EngineErrorCode.InvalidRequest,
                $"The output plugin path is invalid: '{path}'.",
                exception);
        }
    }

    /// <summary>Enforces the requested absent-or-present output intent.</summary>
    /// <param name="mode">The requested output selection mode.</param>
    /// <param name="exists">Whether the canonical output plugin exists.</param>
    /// <param name="path">The canonical output plugin path.</param>
    /// <exception cref="NativeSourceInputException">Thrown when the observed existence conflicts with the request.</exception>
    private static void ValidateSelectionMode(OutputSelectionMode mode, bool exists, string path)
    {
        if (mode == OutputSelectionMode.CreateNew && exists)
        {
            throw new NativeSourceInputException(
                EngineErrorCode.InvalidRequest,
                $"CreateNew requires an absent output plugin, but '{path}' already exists.");
        }

        if (mode == OutputSelectionMode.OpenExisting && !exists)
        {
            throw new NativeSourceInputException(
                EngineErrorCode.InvalidRequest,
                $"OpenExisting requires an existing output plugin, but '{path}' is absent.");
        }
    }

    /// <summary>Validates canonical output identity against the association and every admitted source ModKey.</summary>
    /// <param name="association">The requested output association.</param>
    /// <param name="outputPath">The canonical output plugin path.</param>
    /// <param name="sources">The admitted source plugin descriptors.</param>
    /// <exception cref="NativeSourceInputException">Thrown when the output identity is ambiguous or collides with a source.</exception>
    private static void ValidateOutputIdentity(
        OutputAssociation association,
        string outputPath,
        IReadOnlyList<NativeSourcePluginInput> sources)
    {
        if (!ModKey.TryFromNameAndExtension(Path.GetFileName(outputPath), out var pathModKey, out var error))
        {
            throw new NativeSourceInputException(
                EngineErrorCode.InvalidRequest,
                $"The output plugin file name is invalid: {error}");
        }

        if (pathModKey != association.ModKey)
        {
            throw new NativeSourceInputException(
                EngineErrorCode.InvalidRequest,
                "The canonical output plugin path does not match its requested native ModKey.");
        }

        if (sources.Any(source => source.ModKey == association.ModKey))
        {
            throw new NativeSourceInputException(
                EngineErrorCode.InvalidRequest,
                $"Output ModKey '{association.ModKey.FileName}' is already present in the admitted source load order.");
        }
    }

    /// <summary>Maps a contract output style to Mutagen's native master style.</summary>
    /// <param name="style">The requested output style.</param>
    /// <returns>The corresponding native master style.</returns>
    /// <exception cref="NativeSourceInputException">Thrown when the style is undefined.</exception>
    private static MasterStyle MapMasterStyle(OutputMasterStyle style)
    {
        return style switch
        {
            OutputMasterStyle.Full => MasterStyle.Full,
            OutputMasterStyle.Small => MasterStyle.Small,
            OutputMasterStyle.Medium => MasterStyle.Medium,
            _ => throw new NativeSourceInputException(
                EngineErrorCode.InvalidRequest,
                $"The output master style '{style}' is undefined.")
        };
    }

    /// <summary>Rejects a requested output master style unsupported by the native release.</summary>
    /// <param name="release">The selected native release.</param>
    /// <param name="modKey">The requested output identity.</param>
    /// <param name="style">The requested native master style.</param>
    /// <exception cref="NativeSourceInputException">Thrown when the release cannot represent the style.</exception>
    private static void ValidateMasterStyleCapability(GameRelease release, ModKey modKey, MasterStyle style)
    {
        var constants = GameConstants.Get(release);
        var supported = style switch
        {
            MasterStyle.Full => true,
            MasterStyle.Small => constants.SmallMasterFlag.HasValue,
            MasterStyle.Medium => constants.MediumMasterFlag.HasValue,
            _ => false
        };
        if (!supported)
        {
            throw new NativeSourceInputException(
                EngineErrorCode.UnsupportedInput,
                $"Output '{modKey.FileName}' requests {style} master style, which release {release} does not support.");
        }
    }

    /// <summary>Validates every existing output master against the exact admitted source order and native style metadata.</summary>
    /// <param name="output">The existing output descriptor.</param>
    /// <param name="declaredMasters">The ordered master list read from its native header.</param>
    /// <param name="sources">The admitted source plugins in load-order order.</param>
    /// <exception cref="NativeSourceInputException">Thrown when a master is missing, duplicated, or out of order.</exception>
    private static void ValidateDeclaredMasters(
        NativeOutputPluginInput output,
        IReadOnlyList<ModKey> declaredMasters,
        IReadOnlyList<NativeSourcePluginInput> sources)
    {
        var indexByModKey = sources.ToDictionary(source => source.ModKey, source => source.LoadOrderIndex);
        var previousIndex = -1;
        foreach (var master in declaredMasters)
        {
            if (!indexByModKey.TryGetValue(master, out var index))
            {
                throw new NativeSourceInputException(
                    EngineErrorCode.MissingMaster,
                    $"Existing output '{output.ModKey.FileName}' requires missing admitted master '{master.FileName}'.");
            }

            if (index <= previousIndex)
            {
                throw new NativeSourceInputException(
                    EngineErrorCode.OutputOpenFailed,
                    $"Existing output '{output.ModKey.FileName}' declares master '{master.FileName}' outside admitted load-order order.");
            }

            previousIndex = index;
        }
    }

    /// <summary>Rejects canonical, physical, or hard-link aliases between output artifacts and admitted sources.</summary>
    /// <param name="outputArtifacts">The complete output artifact observations.</param>
    /// <param name="sourceArtifacts">The complete admitted source artifact observations.</param>
    /// <exception cref="NativeSourceInputException">Thrown when an output artifact aliases another path.</exception>
    private static void ValidateNoAliases(
        IReadOnlyList<NativeArtifactAssociation> outputArtifacts,
        IReadOnlyList<NativeArtifactAssociation> sourceArtifacts)
    {
        var outputIdentities = new HashSet<NativeFileIdentity>();
        foreach (var outputArtifact in outputArtifacts)
        {
            if (sourceArtifacts.Any(sourceArtifact => PathComparer.Equals(sourceArtifact.Path, outputArtifact.Path)))
            {
                throw new NativeSourceInputException(
                    EngineErrorCode.InvalidRequest,
                    $"Output artifact '{outputArtifact.Path}' overlaps an admitted source artifact path.");
            }

            if (outputArtifact.FileIdentity is not { } identity)
            {
                continue;
            }

            if (identity.LinkCount is null or > 1)
            {
                throw new NativeSourceInputException(
                    EngineErrorCode.UnsupportedInput,
                    $"Output artifact '{outputArtifact.Path}' has unverified or multiple hard-link identities.");
            }

            if (!outputIdentities.Add(identity)
                || sourceArtifacts.Any(sourceArtifact => Equals(sourceArtifact.FileIdentity, identity)))
            {
                throw new NativeSourceInputException(
                    EngineErrorCode.InvalidRequest,
                    $"Output artifact '{outputArtifact.Path}' aliases another admitted native artifact.");
            }
        }
    }

    /// <summary>Creates an existing output's bounded loose-only native strings lookup.</summary>
    /// <param name="release">The selected native game release.</param>
    /// <param name="output">The output descriptor.</param>
    /// <param name="artifacts">The complete output artifact observations.</param>
    /// <param name="fileSystem">The native file-system adapter.</param>
    /// <returns>A loose strings lookup, or <see langword="null"/> for new or embedded-string output.</returns>
    /// <exception cref="NativeSourceInputException">Thrown when an existing localized output cannot be preserved through explicit loose files.</exception>
    private static StringsFolderLookupOverlay? CreateExistingLooseStringsLookup(
        GameRelease release,
        NativeOutputPluginInput output,
        IReadOnlyList<NativeArtifactAssociation> artifacts,
        IFileSystem fileSystem)
    {
        if (!output.Exists || !output.UsesLocalization)
        {
            return null;
        }

        if (!artifacts.Any(artifact => artifact.Role != NativeArtifactRole.Plugin && artifact.Fingerprint.Exists))
        {
            throw new NativeSourceInputException(
                EngineErrorCode.UnsupportedInput,
                $"Existing localized output '{output.ModKey.FileName}' has no explicit loose strings sidecar to preserve; archive-only localized output is not supported.");
        }

        var outputDirectory = Path.GetDirectoryName(output.Path)!;
        var stringsDirectory = Path.GetFullPath(Path.Combine(outputDirectory, "Strings"));
        NativeFileInspector.VerifyDirectory(stringsDirectory, "output localized-string directory");
        RejectApplicableArchives(release, output.ModKey, outputDirectory, fileSystem);
        RejectApplicableArchives(release, output.ModKey, stringsDirectory, fileSystem);
        var parameters = new StringsReadParameters
        {
            StringsFolderOverride = stringsDirectory,
            BsaFolderOverride = stringsDirectory,
            TargetLanguage = Language.English
        };
        return StringsFolderLookupOverlay.TypicalFactory(
            release,
            output.ModKey,
            stringsDirectory,
            parameters,
            fileSystem);
    }

    /// <summary>Rejects applicable output archives because this admission boundary cannot preserve their internal entries.</summary>
    /// <param name="release">The selected native game release.</param>
    /// <param name="modKey">The output plugin identity.</param>
    /// <param name="directoryPath">The explicit output directory to inspect.</param>
    /// <param name="fileSystem">The native file-system adapter.</param>
    /// <exception cref="NativeSourceInputException">Thrown when an applicable archive is present.</exception>
    private static void RejectApplicableArchives(
        GameRelease release,
        ModKey modKey,
        string directoryPath,
        IFileSystem fileSystem)
    {
        if (fileSystem.Directory
            .EnumerateFiles(directoryPath)
            .Any(path => Archive.IsApplicable(
                release,
                modKey,
                new FileName(Path.GetFileName(path)))))
        {
            throw new NativeSourceInputException(
                EngineErrorCode.UnsupportedInput,
                $"Existing localized output '{modKey.FileName}' has an applicable archive that this loose-output boundary cannot preserve.");
        }
    }

    /// <summary>Creates immutable-surface native master metadata for output parsing.</summary>
    /// <param name="sources">The admitted source plugins.</param>
    /// <param name="output">The selected output plugin.</param>
    /// <returns>A native cache containing every admitted plugin style.</returns>
    private static IReadOnlyCache<IModMasterStyledGetter, ModKey> CreateMasterFlagsLookup(
        IReadOnlyList<NativeSourcePluginInput> sources,
        NativeOutputPluginInput output)
    {
        var masterFlags = new Cache<IModMasterStyledGetter, ModKey>(metadata => metadata.ModKey);
        foreach (var source in sources)
        {
            masterFlags.Set(new KeyedMasterStyle(source.ModKey, source.MasterStyle));
        }

        masterFlags.Set(new KeyedMasterStyle(output.ModKey, output.MasterStyle));
        return masterFlags;
    }
}
