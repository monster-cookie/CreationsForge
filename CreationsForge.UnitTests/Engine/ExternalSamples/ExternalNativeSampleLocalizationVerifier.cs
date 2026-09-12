using System.IO.Abstractions;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using CreationsForge.Core.Engine.NativeInputs;
using CreationsForge.Core.Enums;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Archives;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Starfield;
using Mutagen.Bethesda.Strings;
using Mutagen.Bethesda.Strings.DI;
using Noggog;

namespace CreationsForge.UnitTests.Engine.ExternalSamples;

/// <summary>Proves selected source names against explicit loose or archive-backed native string tables.</summary>
internal static class ExternalNativeSampleLocalizationVerifier
{
    /// <summary>Sequentially parses only FormLists from selected source plugins and verifies every claimed localized name value.</summary>
    /// <param name="manifest">The validated explicit source manifest.</param>
    /// <param name="preflight">The exact preflight archive and plugin scope.</param>
    /// <param name="selectedRecords">The selected detached source records.</param>
    /// <param name="acceptanceGaps">The report-owned acceptance-gap collector.</param>
    /// <param name="cancellationToken">A token observed before and between source and table reads.</param>
    /// <returns>Compact physical-table evidence for every verified claimed translation.</returns>
    /// <exception cref="InvalidDataException">Thrown when source identity, string keys, translations, or explicit tables cannot be verified exactly.</exception>
    /// <exception cref="OperationCanceledException">Thrown when verification is cancelled.</exception>
    internal static IReadOnlyList<ExternalLocalizedNameEvidence> Verify(
        ExternalNativeSampleManifest manifest,
        ExternalNativeSamplePreflightReport preflight,
        IReadOnlyList<ExternalSelectedRecord> selectedRecords,
        ICollection<string> acceptanceGaps,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(manifest);
        ArgumentNullException.ThrowIfNull(preflight);
        ArgumentNullException.ThrowIfNull(selectedRecords);
        ArgumentNullException.ThrowIfNull(acceptanceGaps);
        if (manifest.SupportedGame == SupportedGame.Skyrim)
        {
            foreach (var selected in selectedRecords)
            {
                acceptanceGaps.Add($"Selected Skyrim FormList '{selected.FormKey}' has no game-native Name field; localized-name table coverage is not applicable.");
            }

            return [];
        }

        var evidence = new List<ExternalLocalizedNameEvidence>();
        foreach (var group in selectedRecords.GroupBy(record => record.ContainingModKey, StringComparer.OrdinalIgnoreCase))
        {
            cancellationToken.ThrowIfCancellationRequested();
            var plugin = preflight.Plugins.SingleOrDefault(candidate => string.Equals(candidate.ModKey, group.Key, StringComparison.OrdinalIgnoreCase))
                ?? throw new InvalidDataException($"Selected source plugin '{group.Key}' was absent from preflight metadata.");
            if (!plugin.UsesLocalization)
            {
                foreach (var selected in group)
                {
                    acceptanceGaps.Add($"Selected FormList '{selected.FormKey}' comes from nonlocalized plugin '{group.Key}'; archive-backed name coverage is not applicable.");
                }

                continue;
            }

            VerifyPlugin(manifest, preflight, plugin, group.ToArray(), evidence, acceptanceGaps, cancellationToken);
        }

        return Array.AsReadOnly(evidence.ToArray());
    }

    /// <summary>Parses one selected localized plugin with the production lookup ordering and verifies its selected records.</summary>
    /// <param name="manifest">The validated explicit source manifest.</param>
    /// <param name="preflight">The exact preflight archive and plugin scope.</param>
    /// <param name="plugin">The selected plugin metadata.</param>
    /// <param name="selectedRecords">Selected records contained by this plugin.</param>
    /// <param name="evidence">The mutable successful-evidence collector.</param>
    /// <param name="acceptanceGaps">The mutable explicit-gap collector.</param>
    /// <param name="cancellationToken">A token observed during native and table reads.</param>
    /// <exception cref="InvalidDataException">Thrown when selected localized values cannot be verified exactly.</exception>
    private static void VerifyPlugin(
        ExternalNativeSampleManifest manifest,
        ExternalNativeSamplePreflightReport preflight,
        ExternalNativePluginMetadata plugin,
        IReadOnlyList<ExternalSelectedRecord> selectedRecords,
        ICollection<ExternalLocalizedNameEvidence> evidence,
        ICollection<string> acceptanceGaps,
        CancellationToken cancellationToken)
    {
        var sourcePaths = selectedRecords.Select(record => Path.GetFullPath(record.SourcePath)).Distinct(PathComparer).ToArray();
        if (sourcePaths.Length != 1)
        {
            throw new InvalidDataException($"Selected records attributed to '{plugin.ModKey}' did not share one exact source path.");
        }

        var sourcePath = sourcePaths[0];
        var modKey = ModKey.FromNameAndExtension(plugin.ModKey);
        var lookup = CreateStringsLookup(manifest, modKey);
        var metadata = CreateParsingMeta(manifest, preflight, modKey, sourcePath, lookup);
        using var fileStream = new FileStream(sourcePath, FileMode.Open, FileAccess.Read, FileShare.Read, 131072, FileOptions.SequentialScan);
        using var binaryStream = new MutagenBinaryReadStream(
            fileStream,
            metadata,
            bufferSize: 4096,
            dispose: true,
            offsetReference: 0);
        var frame = new MutagenFrame(binaryStream);
        switch (manifest.SupportedGame)
        {
            case SupportedGame.Starfield:
            {
                var starfield = StarfieldMod.CreateFromBinary(
                    frame,
                    StarfieldRelease.Starfield,
                    new Mutagen.Bethesda.Starfield.GroupMask(false) { FormLists = true });
                VerifyNamedRecords(
                    starfield.FormLists.ToDictionary(record => record.FormKey),
                    record => record.Name,
                    manifest,
                    preflight,
                    lookup,
                    selectedRecords,
                    evidence,
                    acceptanceGaps,
                    cancellationToken);
                break;
            }
            case SupportedGame.Fallout4:
            {
                var fallout4 = Fallout4Mod.CreateFromBinary(
                    frame,
                    Fallout4Release.Fallout4,
                    new Mutagen.Bethesda.Fallout4.GroupMask(false) { FormLists = true });
                VerifyNamedRecords(
                    fallout4.FormLists.ToDictionary(record => record.FormKey),
                    record => record.Name,
                    manifest,
                    preflight,
                    lookup,
                    selectedRecords,
                    evidence,
                    acceptanceGaps,
                    cancellationToken);
                break;
            }
            default:
                throw new InvalidDataException($"Unsupported localized FormList verifier game '{manifest.SupportedGame}'.");
        }
    }

    /// <summary>Verifies selected records against their direct native names and explicit physical tables.</summary>
    /// <typeparam name="TRecord">The game-specific native FormList getter.</typeparam>
    /// <param name="records">Directly parsed FormLists keyed by exact identity.</param>
    /// <param name="getName">Gets the game-specific translated Name.</param>
    /// <param name="manifest">The validated explicit source manifest.</param>
    /// <param name="preflight">The exact preflight archive scope.</param>
    /// <param name="lookup">The production-ordered explicit strings lookup.</param>
    /// <param name="selectedRecords">Selected detached inspector records.</param>
    /// <param name="evidence">The mutable successful-evidence collector.</param>
    /// <param name="acceptanceGaps">The mutable explicit-gap collector.</param>
    /// <param name="cancellationToken">A token observed between table reads.</param>
    /// <exception cref="InvalidDataException">Thrown when a selected source name cannot be verified exactly.</exception>
    private static void VerifyNamedRecords<TRecord>(
        IReadOnlyDictionary<FormKey, TRecord> records,
        Func<TRecord, TranslatedString?> getName,
        ExternalNativeSampleManifest manifest,
        ExternalNativeSamplePreflightReport preflight,
        NativeStringsFolderLookup lookup,
        IReadOnlyList<ExternalSelectedRecord> selectedRecords,
        ICollection<ExternalLocalizedNameEvidence> evidence,
        ICollection<string> acceptanceGaps,
        CancellationToken cancellationToken)
    {
        foreach (var selected in selectedRecords)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!records.TryGetValue(selected.FormKey, out var record))
            {
                throw new InvalidDataException($"Direct localized source parse did not contain selected FormList '{selected.FormKey}'.");
            }

            using var document = JsonDocument.Parse(selected.Json);
            var root = document.RootElement;
            if (!root.TryGetProperty("Name", out var claimedName) || claimedName.ValueKind == JsonValueKind.Null)
            {
                acceptanceGaps.Add($"Selected localized FormList '{selected.FormKey}' has no Name; localized-name table coverage remains untested for this record.");
                continue;
            }

            var translations = claimedName.GetProperty("translations").EnumerateArray().ToArray();
            if (translations.Length == 0)
            {
                acceptanceGaps.Add($"Selected localized FormList '{selected.FormKey}' has an empty Name; localized-name table coverage remains untested for this record.");
                continue;
            }

            var name = getName(record)
                ?? throw new InvalidDataException($"Selected localized FormList '{selected.FormKey}' claimed a Name but the direct source Name was absent.");
            var key = name.StringsKey
                ?? throw new InvalidDataException($"Selected localized FormList '{selected.FormKey}' claimed translations but its direct source Name had no strings key.");
            foreach (var claimedTranslation in translations)
            {
                var languageText = claimedTranslation.GetProperty("language").GetString();
                if (!Enum.TryParse<Language>(languageText, ignoreCase: false, out var language))
                {
                    throw new InvalidDataException($"Selected localized FormList '{selected.FormKey}' claimed unknown language '{languageText}'.");
                }

                var claimedText = claimedTranslation.GetProperty("value").GetString()
                    ?? throw new InvalidDataException($"Selected localized FormList '{selected.FormKey}' claimed a null {language} Name translation.");
                if (!name.TryLookup(language, out var nativeText)
                    || !string.Equals(nativeText, claimedText, StringComparison.Ordinal))
                {
                    throw new InvalidDataException($"Direct source Name lookup differed from the claimed {language} translation for '{selected.FormKey}'.");
                }

                if (!lookup.TryLookup(StringsSource.Normal, language, key, out var selectedText, out var selectedSourcePath)
                    || !string.Equals(selectedText, claimedText, StringComparison.Ordinal))
                {
                    throw new InvalidDataException($"The explicit production strings lookup could not resolve source Name key {key} for '{selected.FormKey}' in {language}.");
                }

                var verifiedTable = RequirePhysicalTableValue(
                    manifest,
                    preflight,
                    ModKey.FromNameAndExtension(selected.ContainingModKey),
                    language,
                    key,
                    claimedText,
                    cancellationToken);
                evidence.Add(new ExternalLocalizedNameEvidence
                {
                    FormKey = selected.FormKey.ToString(),
                    Language = language.ToString(),
                    StringsKey = key,
                    SelectedSourcePath = selectedSourcePath,
                    VerifiedTable = verifiedTable,
                    TextSha256 = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(claimedText))),
                });
            }
        }
    }

    /// <summary>Requires exactly one explicit loose or archive table to contain the expected key and ordinal text.</summary>
    /// <param name="manifest">The validated explicit source manifest.</param>
    /// <param name="preflight">The exact admitted archive scope.</param>
    /// <param name="modKey">The localized source plugin identity.</param>
    /// <param name="language">The claimed translation language.</param>
    /// <param name="key">The original source strings key.</param>
    /// <param name="expectedText">The exact expected text.</param>
    /// <param name="cancellationToken">A token observed between physical table reads.</param>
    /// <returns>The unique verified loose path or archive-entry identity.</returns>
    /// <exception cref="InvalidDataException">Thrown when no unique admitted physical table proves the key and text.</exception>
    private static string RequirePhysicalTableValue(
        ExternalNativeSampleManifest manifest,
        ExternalNativeSamplePreflightReport preflight,
        ModKey modKey,
        Language language,
        uint key,
        string expectedText,
        CancellationToken cancellationToken)
    {
        var format = GameConstants.Get(manifest.GameRelease).StringsLanguageFormat
            ?? throw new InvalidDataException($"Release '{manifest.GameRelease}' does not define native strings file names.");
        var fileName = StringsUtility.GetFileName(format, modKey, language, StringsSource.Normal);
        var encoding = MutagenEncoding.GetEncoding(manifest.GameRelease, language);
        var matches = new List<string>();
        foreach (var directory in manifest.CanonicalStringDirectoryPaths)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var path = Path.GetFullPath(Path.Combine(directory, fileName));
            if (!File.Exists(path))
            {
                continue;
            }

            var lookup = new StringsLookupOverlay(path, StringsSource.Normal, encoding, new FileSystem());
            if (lookup.TryLookup(key, out var value) && string.Equals(value, expectedText, StringComparison.Ordinal))
            {
                matches.Add(path);
            }
        }

        var entryPath = "strings/" + fileName.Replace('\\', '/');
        foreach (var archive in preflight.ApplicableArchives)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var reader = Archive.CreateReader(manifest.GameRelease, archive.Path);
            try
            {
                foreach (var file in reader.Files.Where(file => string.Equals(
                    file.Path.Replace('\\', '/'),
                    entryPath,
                    StringComparison.OrdinalIgnoreCase)))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var lookup = new StringsLookupOverlay(file.GetMemorySlice(), StringsSource.Normal, encoding);
                    if (lookup.TryLookup(key, out var value) && string.Equals(value, expectedText, StringComparison.Ordinal))
                    {
                        matches.Add($"{archive.Path}!/{entryPath}");
                    }
                }
            }
            finally
            {
                (reader as IDisposable)?.Dispose();
            }
        }

        return matches.Count == 1
            ? matches[0]
            : throw new InvalidDataException(
                $"Expected exactly one admitted physical strings table to prove Name key {key} for '{modKey}' in {language}, but found {matches.Count}.");
    }

    /// <summary>Creates the same ordered explicit strings lookup used by the production source loader.</summary>
    /// <param name="manifest">The validated explicit source manifest.</param>
    /// <param name="modKey">The localized source plugin identity.</param>
    /// <returns>A production-equivalent ordered lookup over explicit loose directories and the explicit Data directory.</returns>
    private static NativeStringsFolderLookup CreateStringsLookup(ExternalNativeSampleManifest manifest, ModKey modKey)
    {
        var fileSystem = new FileSystem();
        var lookups = manifest.CanonicalStringDirectoryPaths.Select(directoryPath =>
        {
            var parameters = new StringsReadParameters
            {
                StringsFolderOverride = directoryPath,
                BsaFolderOverride = manifest.CanonicalDataDirectoryPath,
                TargetLanguage = Language.English,
            };
            return StringsFolderLookupOverlay.TypicalFactory(
                manifest.GameRelease,
                modKey,
                directoryPath,
                parameters,
                fileSystem);
        }).ToArray();
        return new NativeStringsFolderLookup(lookups);
    }

    /// <summary>Creates source parsing metadata with exact master styles and the explicit production strings lookup.</summary>
    /// <param name="manifest">The validated explicit source manifest.</param>
    /// <param name="preflight">The exact plugin header metadata.</param>
    /// <param name="modKey">The source plugin identity.</param>
    /// <param name="sourcePath">The exact source plugin path.</param>
    /// <param name="lookup">The explicit production strings lookup.</param>
    /// <returns>Fresh read metadata for one selected localized source plugin.</returns>
    private static ParsingMeta CreateParsingMeta(
        ExternalNativeSampleManifest manifest,
        ExternalNativeSamplePreflightReport preflight,
        ModKey modKey,
        string sourcePath,
        NativeStringsFolderLookup lookup)
    {
        var masterFlags = new Cache<IModMasterStyledGetter, ModKey>(metadata => metadata.ModKey);
        foreach (var plugin in preflight.Plugins)
        {
            masterFlags.Set(new KeyedMasterStyle(
                ModKey.FromNameAndExtension(plugin.ModKey),
                Enum.Parse<MasterStyle>(plugin.MasterStyle, ignoreCase: false)));
        }

        var metadata = ParsingMeta.Factory(
            new BinaryReadParameters
            {
                MasterFlagsLookup = masterFlags,
            },
            manifest.GameRelease,
            new ModPath(modKey, sourcePath));
        metadata.StringsLookup = lookup;
        return metadata;
    }

    /// <summary>Gets platform path equality semantics.</summary>
    private static StringComparer PathComparer { get; } = OperatingSystem.IsWindows()
        ? StringComparer.OrdinalIgnoreCase
        : StringComparer.Ordinal;
}
