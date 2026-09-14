using System.IO.Abstractions;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.PluginInputs;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Archives;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Masters;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Strings;
using Noggog;

namespace CreationsForge.Core.Engine.PluginOutputs;

/// <summary>Admits one explicit plugin output without creating or changing any destination artifact.</summary>
public sealed class PluginOutputInputLoader
{
    /// <summary>The localized flag shared by the supported plugin TES4 plugin headers.</summary>
    private const uint LocalizedHeaderFlag = 0x00000080;

    /// <summary>Compares canonical paths according to the current platform's file-name semantics.</summary>
    private static readonly StringComparer PathComparer = OperatingSystem.IsWindows()
        ? StringComparer.OrdinalIgnoreCase
        : StringComparer.Ordinal;

    /// <summary>Initializes a stateless plugin output-input loader.</summary>
    public PluginOutputInputLoader()
    { }

    /// <summary>Validates and prepares an independently disposable output-only input lifetime.</summary>
    /// <param name="borrowedSources">The already opened plugin source lifetime, which remains caller-owned.</param>
    /// <param name="association">The exact output path, identity, localization mode, and master style.</param>
    /// <param name="mode">Whether the output must be absent or must already exist.</param>
    /// <param name="cancellationToken">The token checked during source verification, output discovery, header reads, and hashing.</param>
    /// <returns>A prepared output input set or a stable typed failure.</returns>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is requested.</exception>
    public async Task<EngineResult<PluginOutputInputs>> PrepareAsync(
        PluginSourceInputs borrowedSources,
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
                throw new PluginSourceInputException(
                    EngineErrorCode.InvalidRequest,
                    $"The output selection mode '{mode}' is undefined.");
            }

            var sourceBefore = await borrowedSources.VerifyUnchangedAsync(cancellationToken).ConfigureAwait(false);
            if (!sourceBefore.Succeeded)
            {
                return EngineResult<PluginOutputInputs>.Failure(sourceBefore.Error!);
            }

            var outputPath = CanonicalizeOutputPath(association.PluginPath);
            var outputDirectory = Path.GetDirectoryName(outputPath)!;
            if (!Directory.Exists(outputDirectory))
            {
                throw new PluginSourceInputException(
                    EngineErrorCode.OutputOpenFailed,
                    $"The output plugin directory does not exist and selection will not create it: '{outputDirectory}'.");
            }

            PluginFileInspector.VerifyDirectory(outputDirectory, "output plugin directory");
            ValidateOutputIdentity(association, outputPath, borrowedSources.Plugins);
            var initialPluginArtifact = await PluginFileInspector.InspectAsync(
                outputPath,
                PluginArtifactRole.Plugin,
                null,
                mustExist: false,
                cancellationToken).ConfigureAwait(false);
            var exists = initialPluginArtifact.Fingerprint.Exists;
            ValidateSelectionMode(mode, exists, outputPath);

            var fileSystem = new FileSystem();
            PluginOutputPluginInput output;
            if (exists)
            {
                var modPath = new ModPath(association.ModKey, outputPath);
                var header = ModHeaderFrame.FromPath(modPath, borrowedSources.Release, fileSystem);
                var usesLocalization = ((uint)header.Flags & LocalizedHeaderFlag) != 0;
                ValidateMasterStyleCapability(borrowedSources.Release, association.ModKey, header.MasterStyle);
                var expectedMasterStyle = MapMasterStyle(association.MasterStyle);
                if (header.MasterStyle != expectedMasterStyle)
                {
                    throw new PluginSourceInputException(
                        EngineErrorCode.InvalidRequest,
                        $"Existing output '{association.ModKey.FileName}' uses {header.MasterStyle} master style instead of requested {expectedMasterStyle}.");
                }

                var expectedLocalization = association.LocalizedOutputMode == LocalizedOutputMode.SeparateStringFiles;
                if (usesLocalization != expectedLocalization)
                {
                    throw new PluginSourceInputException(
                        EngineErrorCode.InvalidRequest,
                        $"Existing output '{association.ModKey.FileName}' localization state does not match {association.LocalizedOutputMode}.");
                }

                output = new PluginOutputPluginInput(
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
                var pluginAfterHeader = await PluginFileInspector.InspectAsync(
                    outputPath,
                    PluginArtifactRole.Plugin,
                    null,
                    mustExist: true,
                    cancellationToken).ConfigureAwait(false);
                if (!PluginArtifactSetUtilities.Match([initialPluginArtifact], [pluginAfterHeader]))
                {
                    throw new PluginSourceInputException(
                        EngineErrorCode.ExternalChangeDetected,
                        "The existing output plugin changed while its plugin header was being validated.");
                }
            }
            else
            {
                var masterStyle = MapMasterStyle(association.MasterStyle);
                ValidateMasterStyleCapability(borrowedSources.Release, association.ModKey, masterStyle);
                output = new PluginOutputPluginInput(
                    outputPath,
                    association.ModKey,
                    exists: false,
                    masterStyle,
                    association.LocalizedOutputMode == LocalizedOutputMode.SeparateStringFiles);
            }

            var collector = new PluginOutputArtifactCollector(borrowedSources.Release, output);
            var initialArtifacts = await collector.CaptureAsync(cancellationToken).ConfigureAwait(false);
            var observedPlugin = initialArtifacts.Single(artifact => artifact.Role == PluginArtifactRole.Plugin);
            if (!PluginArtifactSetUtilities.Match([initialPluginArtifact], [observedPlugin]))
            {
                throw new PluginSourceInputException(
                    EngineErrorCode.ExternalChangeDetected,
                    "The explicit output plugin changed while its complete artifact set was being admitted.");
            }

            ValidateNoAliases(initialArtifacts, sourceBefore.Value!.Artifacts);
            var stringsLookup = CreateExistingLooseStringsLookup(
                borrowedSources.Release,
                output,
                initialArtifacts,
                borrowedSources.RecordTextLanguage,
                fileSystem);

            var sourceAfter = await borrowedSources.VerifyUnchangedAsync(cancellationToken).ConfigureAwait(false);
            if (!sourceAfter.Succeeded)
            {
                return EngineResult<PluginOutputInputs>.Failure(sourceAfter.Error!);
            }

            var currentArtifacts = await collector.CaptureAsync(cancellationToken).ConfigureAwait(false);
            if (!PluginArtifactSetUtilities.Match(initialArtifacts, currentArtifacts))
            {
                throw new PluginSourceInputException(
                    EngineErrorCode.ExternalChangeDetected,
                    "The explicit plugin output changed while it was being admitted.");
            }

            var masterFlags = CreateMasterFlagsLookup(borrowedSources.Plugins, output);
            return EngineResult<PluginOutputInputs>.Success(new PluginOutputInputs(
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
        catch (PluginSourceInputException exception)
        {
            var code = exception.Code == EngineErrorCode.SourceOpenFailed
                ? EngineErrorCode.OutputOpenFailed
                : exception.Code;
            return EngineResult<PluginOutputInputs>.Failure(new EngineError(code, exception.Message));
        }
        catch (ObjectDisposedException exception)
        {
            return EngineResult<PluginOutputInputs>.Failure(new EngineError(
                EngineErrorCode.SourceOpenFailed,
                $"The borrowed plugin source lifetime is unavailable: {exception.Message}"));
        }
        catch (Exception exception)
        {
            return EngineResult<PluginOutputInputs>.Failure(new EngineError(
                EngineErrorCode.OutputOpenFailed,
                $"The explicit plugin output could not be prepared: {exception.Message}"));
        }
    }

    /// <summary>Canonicalizes an output plugin path without requiring the file to exist.</summary>
    /// <param name="path">The caller-supplied output path.</param>
    /// <returns>The canonical absolute output path.</returns>
    /// <exception cref="PluginSourceInputException">Thrown when the path is empty or invalid.</exception>
    private static string CanonicalizeOutputPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new PluginSourceInputException(EngineErrorCode.InvalidRequest, "An output plugin path is required.");
        }

        try
        {
            return Path.GetFullPath(path);
        }
        catch (Exception exception) when (
            exception is ArgumentException or NotSupportedException or PathTooLongException)
        {
            throw new PluginSourceInputException(
                EngineErrorCode.InvalidRequest,
                $"The output plugin path is invalid: '{path}'.",
                exception);
        }
    }

    /// <summary>Enforces the requested absent-or-present output intent.</summary>
    /// <param name="mode">The requested output selection mode.</param>
    /// <param name="exists">Whether the canonical output plugin exists.</param>
    /// <param name="path">The canonical output plugin path.</param>
    /// <exception cref="PluginSourceInputException">Thrown when the observed existence conflicts with the request.</exception>
    private static void ValidateSelectionMode(OutputSelectionMode mode, bool exists, string path)
    {
        if (mode == OutputSelectionMode.CreateNew && exists)
        {
            throw new PluginSourceInputException(
                EngineErrorCode.InvalidRequest,
                $"CreateNew requires an absent output plugin, but '{path}' already exists.");
        }

        if (mode == OutputSelectionMode.OpenExisting && !exists)
        {
            throw new PluginSourceInputException(
                EngineErrorCode.InvalidRequest,
                $"OpenExisting requires an existing output plugin, but '{path}' is absent.");
        }
    }

    /// <summary>Validates canonical output identity against the association and every admitted source ModKey.</summary>
    /// <param name="association">The requested output association.</param>
    /// <param name="outputPath">The canonical output plugin path.</param>
    /// <param name="sources">The admitted source plugin descriptors.</param>
    /// <exception cref="PluginSourceInputException">Thrown when the output identity is ambiguous or collides with a source.</exception>
    private static void ValidateOutputIdentity(
        OutputAssociation association,
        string outputPath,
        IReadOnlyList<PluginSourcePluginInput> sources)
    {
        if (!ModKey.TryFromNameAndExtension(Path.GetFileName(outputPath), out var pathModKey, out var error))
        {
            throw new PluginSourceInputException(
                EngineErrorCode.InvalidRequest,
                $"The output plugin file name is invalid: {error}");
        }

        if (pathModKey != association.ModKey)
        {
            throw new PluginSourceInputException(
                EngineErrorCode.InvalidRequest,
                "The canonical output plugin path does not match its requested plugin ModKey.");
        }

        if (sources.Any(source => source.ModKey == association.ModKey))
        {
            throw new PluginSourceInputException(
                EngineErrorCode.InvalidRequest,
                $"Output ModKey '{association.ModKey.FileName}' is already present in the admitted source load order.");
        }
    }

    /// <summary>Maps a contract output style to Mutagen's plugin master style.</summary>
    /// <param name="style">The requested output style.</param>
    /// <returns>The corresponding plugin master style.</returns>
    /// <exception cref="PluginSourceInputException">Thrown when the style is undefined.</exception>
    private static MasterStyle MapMasterStyle(OutputMasterStyle style)
    {
        return style switch
        {
            OutputMasterStyle.Full => MasterStyle.Full,
            OutputMasterStyle.Small => MasterStyle.Small,
            OutputMasterStyle.Medium => MasterStyle.Medium,
            _ => throw new PluginSourceInputException(
                EngineErrorCode.InvalidRequest,
                $"The output master style '{style}' is undefined.")
        };
    }

    /// <summary>Rejects a requested output master style unsupported by the plugin release.</summary>
    /// <param name="release">The selected plugin release.</param>
    /// <param name="modKey">The requested output identity.</param>
    /// <param name="style">The requested plugin master style.</param>
    /// <exception cref="PluginSourceInputException">Thrown when the release cannot represent the style.</exception>
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
            throw new PluginSourceInputException(
                EngineErrorCode.UnsupportedInput,
                $"Output '{modKey.FileName}' requests {style} master style, which release {release} does not support.");
        }
    }

    /// <summary>Validates every existing output master against the exact admitted source order and plugin style metadata.</summary>
    /// <param name="output">The existing output descriptor.</param>
    /// <param name="declaredMasters">The ordered master list read from its plugin header.</param>
    /// <param name="sources">The admitted source plugins in load-order order.</param>
    /// <exception cref="PluginSourceInputException">Thrown when a master is missing, duplicated, or out of order.</exception>
    private static void ValidateDeclaredMasters(
        PluginOutputPluginInput output,
        IReadOnlyList<ModKey> declaredMasters,
        IReadOnlyList<PluginSourcePluginInput> sources)
    {
        var indexByModKey = sources.ToDictionary(source => source.ModKey, source => source.LoadOrderIndex);
        var previousIndex = -1;
        foreach (var master in declaredMasters)
        {
            if (!indexByModKey.TryGetValue(master, out var index))
            {
                throw new PluginSourceInputException(
                    EngineErrorCode.MissingMaster,
                    $"Existing output '{output.ModKey.FileName}' requires missing admitted master '{master.FileName}'.");
            }

            if (index <= previousIndex)
            {
                throw new PluginSourceInputException(
                    EngineErrorCode.OutputOpenFailed,
                    $"Existing output '{output.ModKey.FileName}' declares master '{master.FileName}' outside admitted load-order order.");
            }

            previousIndex = index;
        }
    }

    /// <summary>Rejects canonical, physical, or hard-link aliases between output artifacts and admitted sources.</summary>
    /// <param name="outputArtifacts">The complete output artifact observations.</param>
    /// <param name="sourceArtifacts">The complete admitted source artifact observations.</param>
    /// <exception cref="PluginSourceInputException">Thrown when an output artifact aliases another path.</exception>
    private static void ValidateNoAliases(
        IReadOnlyList<PluginArtifactAssociation> outputArtifacts,
        IReadOnlyList<PluginArtifactAssociation> sourceArtifacts)
    {
        var outputIdentities = new HashSet<ArtifactFileIdentity>();
        foreach (var outputArtifact in outputArtifacts)
        {
            if (sourceArtifacts.Any(sourceArtifact => PathComparer.Equals(sourceArtifact.Path, outputArtifact.Path)))
            {
                throw new PluginSourceInputException(
                    EngineErrorCode.InvalidRequest,
                    $"Output artifact '{outputArtifact.Path}' overlaps an admitted source artifact path.");
            }

            if (outputArtifact.FileIdentity is not { } identity)
            {
                continue;
            }

            if (identity.LinkCount is null)
            {
                throw new PluginSourceInputException(
                    EngineErrorCode.UnsupportedInput,
                    $"Output artifact '{outputArtifact.Path}' has no verified hard-link identity.");
            }

            if (!outputIdentities.Add(identity)
                || sourceArtifacts.Any(sourceArtifact => Equals(sourceArtifact.FileIdentity, identity)))
            {
                throw new PluginSourceInputException(
                    EngineErrorCode.InvalidRequest,
                    $"Output artifact '{outputArtifact.Path}' aliases another admitted plugin artifact.");
            }
        }
    }

    /// <summary>Creates an existing output's bounded loose-only record strings lookup.</summary>
    /// <param name="release">The selected plugin game release.</param>
    /// <param name="output">The output descriptor.</param>
    /// <param name="artifacts">The complete output artifact observations.</param>
    /// <param name="recordTextLanguage">The explicit language used for localized record text.</param>
    /// <param name="fileSystem">The plugin file-system adapter.</param>
    /// <returns>A loose strings lookup, or <see langword="null"/> for new or embedded-string output.</returns>
    /// <exception cref="PluginSourceInputException">Thrown when an existing localized output cannot be preserved through explicit loose files.</exception>
    private static StringsFolderLookupOverlay? CreateExistingLooseStringsLookup(
        GameRelease release,
        PluginOutputPluginInput output,
        IReadOnlyList<PluginArtifactAssociation> artifacts,
        Language recordTextLanguage,
        IFileSystem fileSystem)
    {
        if (!output.Exists || !output.UsesLocalization)
        {
            return null;
        }

        if (!artifacts.Any(artifact => artifact.Role != PluginArtifactRole.Plugin && artifact.Fingerprint.Exists))
        {
            throw new PluginSourceInputException(
                EngineErrorCode.UnsupportedInput,
                $"Existing localized output '{output.ModKey.FileName}' has no explicit loose strings sidecar to preserve; archive-only localized output is not supported.");
        }

        var outputDirectory = Path.GetDirectoryName(output.Path)!;
        var stringsDirectory = Path.GetFullPath(Path.Combine(outputDirectory, "Strings"));
        PluginFileInspector.VerifyDirectory(stringsDirectory, "output localized-string directory");
        RejectApplicableArchives(release, output.ModKey, outputDirectory, fileSystem);
        RejectApplicableArchives(release, output.ModKey, stringsDirectory, fileSystem);
        var parameters = new StringsReadParameters
        {
            StringsFolderOverride = stringsDirectory,
            BsaFolderOverride = stringsDirectory,
            TargetLanguage = recordTextLanguage
        };
        return StringsFolderLookupOverlay.TypicalFactory(
            release,
            output.ModKey,
            stringsDirectory,
            parameters,
            fileSystem);
    }

    /// <summary>Rejects applicable output archives because this admission boundary cannot preserve their internal entries.</summary>
    /// <param name="release">The selected plugin game release.</param>
    /// <param name="modKey">The output plugin identity.</param>
    /// <param name="directoryPath">The explicit output directory to inspect.</param>
    /// <param name="fileSystem">The plugin file-system adapter.</param>
    /// <exception cref="PluginSourceInputException">Thrown when an applicable archive is present.</exception>
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
            throw new PluginSourceInputException(
                EngineErrorCode.UnsupportedInput,
                $"Existing localized output '{modKey.FileName}' has an applicable archive that this loose-output boundary cannot preserve.");
        }
    }

    /// <summary>Creates immutable-surface plugin master metadata for output parsing.</summary>
    /// <param name="sources">The admitted source plugins.</param>
    /// <param name="output">The selected output plugin.</param>
    /// <returns>A plugin cache containing every admitted plugin style.</returns>
    private static IReadOnlyCache<IModMasterStyledGetter, ModKey> CreateMasterFlagsLookup(
        IReadOnlyList<PluginSourcePluginInput> sources,
        PluginOutputPluginInput output)
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
