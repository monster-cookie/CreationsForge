using System.IO.Abstractions;
using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Enums;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Starfield;
using Mutagen.Bethesda.Strings;
using Noggog;

namespace CreationsForge.UnitTests.Engine.ExternalSamples;

/// <summary>Reopens retained output through direct game-native readers independently of the workspace lifecycle.</summary>
internal static class ExternalNativeSampleVerifier
{
    /// <summary>Directly parses one output and verifies its exact FormList set, header masters, common fields, names, and items.</summary>
    /// <param name="manifest">The validated source and release manifest.</param>
    /// <param name="preflight">The header-only source metadata used to construct exact master-style parsing metadata.</param>
    /// <param name="association">The retained output identity and representation.</param>
    /// <param name="expectedRecords">Complete expected inspector JSON keyed by exact output FormKey.</param>
    /// <param name="intendedEdits">Independent exact EditorID and ordered Items expectations for external overrides.</param>
    /// <param name="expectedMasters">Expected output masters in exact load-order order.</param>
    /// <param name="cancellationToken">A token observed before and between direct native record checks.</param>
    /// <returns>Compact evidence from the independent direct parser.</returns>
    /// <exception cref="ArgumentNullException">Thrown when a required input is null.</exception>
    /// <exception cref="OperationCanceledException">Thrown when direct verification is cancelled.</exception>
    /// <exception cref="InvalidDataException">Thrown when direct native values differ from the expected engine preview.</exception>
    internal static ExternalNativeVerificationResult Verify(
        ExternalNativeSampleManifest manifest,
        ExternalNativeSamplePreflightReport preflight,
        OutputAssociation association,
        IReadOnlyDictionary<FormKey, string> expectedRecords,
        IReadOnlyDictionary<FormKey, ExternalIntendedRecordEdit> intendedEdits,
        IReadOnlyList<ModKey> expectedMasters,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(manifest);
        ArgumentNullException.ThrowIfNull(preflight);
        ArgumentNullException.ThrowIfNull(association);
        ArgumentNullException.ThrowIfNull(expectedRecords);
        ArgumentNullException.ThrowIfNull(intendedEdits);
        ArgumentNullException.ThrowIfNull(expectedMasters);
        cancellationToken.ThrowIfCancellationRequested();

        var metadata = CreateParsingMeta(manifest, preflight, association);
        using var fileStream = new FileStream(
            association.PluginPath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            131072,
            FileOptions.SequentialScan);
        using var binaryStream = new MutagenBinaryReadStream(
            fileStream,
            metadata,
            bufferSize: 4096,
            dispose: true,
            offsetReference: 0);
        var frame = new MutagenFrame(binaryStream);

        return manifest.SupportedGame switch
        {
            SupportedGame.Starfield => VerifyStarfield(
                StarfieldMod.CreateFromBinary(frame, StarfieldRelease.Starfield, new Mutagen.Bethesda.Starfield.GroupMask(true)),
                expectedRecords,
                intendedEdits,
                expectedMasters,
                association,
                cancellationToken),
            SupportedGame.Fallout4 => VerifyFallout4(
                Fallout4Mod.CreateFromBinary(frame, Fallout4Release.Fallout4, new Mutagen.Bethesda.Fallout4.GroupMask(true)),
                expectedRecords,
                intendedEdits,
                expectedMasters,
                association,
                cancellationToken),
            SupportedGame.Skyrim => VerifySkyrim(
                SkyrimMod.CreateFromBinary(frame, SkyrimRelease.SkyrimSE, new Mutagen.Bethesda.Skyrim.GroupMask(true)),
                expectedRecords,
                intendedEdits,
                expectedMasters,
                association,
                cancellationToken),
            _ => throw new InvalidDataException($"Unsupported direct verifier game '{manifest.SupportedGame}'."),
        };
    }

    /// <summary>Builds strict direct-parser metadata from source headers and output-local string tables.</summary>
    /// <param name="manifest">The explicit sample manifest.</param>
    /// <param name="preflight">The validated source header metadata.</param>
    /// <param name="association">The output association.</param>
    /// <returns>Fresh parsing metadata independent of the workspace-owned reader.</returns>
    private static ParsingMeta CreateParsingMeta(
        ExternalNativeSampleManifest manifest,
        ExternalNativeSamplePreflightReport preflight,
        OutputAssociation association)
    {
        var masterFlags = new Cache<IModMasterStyledGetter, ModKey>(metadata => metadata.ModKey);
        foreach (var plugin in preflight.Plugins)
        {
            masterFlags.Set(new KeyedMasterStyle(
                ModKey.FromNameAndExtension(plugin.ModKey),
                Enum.Parse<MasterStyle>(plugin.MasterStyle, ignoreCase: false)));
        }

        masterFlags.Set(new KeyedMasterStyle(association.ModKey, MasterStyle.Full));
        var parameters = new BinaryReadParameters
        {
            MasterFlagsLookup = masterFlags,
            ThrowOnUnknownSubrecord = true,
        };
        var metadata = ParsingMeta.Factory(
            parameters,
            manifest.GameRelease,
            new ModPath(association.ModKey, association.PluginPath));
        if (association.LocalizedOutputMode == LocalizedOutputMode.SeparateStringFiles)
        {
            var stringsDirectory = Path.Combine(Path.GetDirectoryName(association.PluginPath)!, "Strings");
            var stringsParameters = new StringsReadParameters
            {
                StringsFolderOverride = stringsDirectory,
                BsaFolderOverride = stringsDirectory,
                TargetLanguage = Language.English,
            };
            metadata.StringsLookup = StringsFolderLookupOverlay.TypicalFactory(
                manifest.GameRelease,
                association.ModKey,
                stringsDirectory,
                stringsParameters,
                new FileSystem());
        }

        return metadata;
    }

    /// <summary>Verifies direct Starfield native state.</summary>
    /// <param name="mod">The independently parsed output.</param>
    /// <param name="expectedRecords">Expected complete records.</param>
    /// <param name="intendedEdits">Independent exact EditorID and ordered Items expectations.</param>
    /// <param name="expectedMasters">Expected masters in exact order.</param>
    /// <param name="association">The output association.</param>
    /// <param name="cancellationToken">A token observed between record checks.</param>
    /// <returns>Compact direct-reader evidence.</returns>
    private static ExternalNativeVerificationResult VerifyStarfield(
        IStarfieldModGetter mod,
        IReadOnlyDictionary<FormKey, string> expectedRecords,
        IReadOnlyDictionary<FormKey, ExternalIntendedRecordEdit> intendedEdits,
        IReadOnlyList<ModKey> expectedMasters,
        OutputAssociation association,
        CancellationToken cancellationToken)
    {
        VerifyHeader(mod.ModKey, mod.ModHeader.MasterReferences.Select(reference => reference.Master), expectedMasters, association);
        var records = mod.FormLists.ToDictionary(record => record.FormKey);
        VerifyRecordSet(records.Keys, expectedRecords.Keys);
        foreach (var expected in expectedRecords)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var record = records[expected.Key];
            using var document = JsonDocument.Parse(expected.Value);
            VerifyCommon(
                record,
                record.FormVersion,
                record.Version2,
                record.Items.Select(item => item.FormKeyNullable ?? FormKey.Null),
                record.Name,
                document.RootElement);
            if (intendedEdits.TryGetValue(expected.Key, out var intended))
            {
                ExternalNativeSampleAssertions.RequireNativeValues(
                    record.EditorID,
                    record.Items.Select(item => item.FormKeyNullable ?? FormKey.Null),
                    intended,
                    "Direct Starfield parse");
            }
        }

        return CreateResult(mod.ModKey, expectedMasters, records.Count, association);
    }

    /// <summary>Verifies direct Fallout 4 native state.</summary>
    /// <param name="mod">The independently parsed output.</param>
    /// <param name="expectedRecords">Expected complete records.</param>
    /// <param name="intendedEdits">Independent exact EditorID and ordered Items expectations.</param>
    /// <param name="expectedMasters">Expected masters in exact order.</param>
    /// <param name="association">The output association.</param>
    /// <param name="cancellationToken">A token observed between record checks.</param>
    /// <returns>Compact direct-reader evidence.</returns>
    private static ExternalNativeVerificationResult VerifyFallout4(
        IFallout4ModGetter mod,
        IReadOnlyDictionary<FormKey, string> expectedRecords,
        IReadOnlyDictionary<FormKey, ExternalIntendedRecordEdit> intendedEdits,
        IReadOnlyList<ModKey> expectedMasters,
        OutputAssociation association,
        CancellationToken cancellationToken)
    {
        VerifyHeader(mod.ModKey, mod.ModHeader.MasterReferences.Select(reference => reference.Master), expectedMasters, association);
        var records = mod.FormLists.ToDictionary(record => record.FormKey);
        VerifyRecordSet(records.Keys, expectedRecords.Keys);
        foreach (var expected in expectedRecords)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var record = records[expected.Key];
            using var document = JsonDocument.Parse(expected.Value);
            VerifyCommon(
                record,
                record.FormVersion,
                record.Version2,
                record.Items.Select(item => item.FormKeyNullable ?? FormKey.Null),
                record.Name,
                document.RootElement);
            if (intendedEdits.TryGetValue(expected.Key, out var intended))
            {
                ExternalNativeSampleAssertions.RequireNativeValues(
                    record.EditorID,
                    record.Items.Select(item => item.FormKeyNullable ?? FormKey.Null),
                    intended,
                    "Direct Fallout 4 parse");
            }
        }

        return CreateResult(mod.ModKey, expectedMasters, records.Count, association);
    }

    /// <summary>Verifies direct Skyrim Special Edition native state.</summary>
    /// <param name="mod">The independently parsed output.</param>
    /// <param name="expectedRecords">Expected complete records.</param>
    /// <param name="intendedEdits">Independent exact EditorID and ordered Items expectations.</param>
    /// <param name="expectedMasters">Expected masters in exact order.</param>
    /// <param name="association">The output association.</param>
    /// <param name="cancellationToken">A token observed between record checks.</param>
    /// <returns>Compact direct-reader evidence.</returns>
    private static ExternalNativeVerificationResult VerifySkyrim(
        ISkyrimModGetter mod,
        IReadOnlyDictionary<FormKey, string> expectedRecords,
        IReadOnlyDictionary<FormKey, ExternalIntendedRecordEdit> intendedEdits,
        IReadOnlyList<ModKey> expectedMasters,
        OutputAssociation association,
        CancellationToken cancellationToken)
    {
        VerifyHeader(mod.ModKey, mod.ModHeader.MasterReferences.Select(reference => reference.Master), expectedMasters, association);
        var records = mod.FormLists.ToDictionary(record => record.FormKey);
        VerifyRecordSet(records.Keys, expectedRecords.Keys);
        foreach (var expected in expectedRecords)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var record = records[expected.Key];
            using var document = JsonDocument.Parse(expected.Value);
            VerifyCommon(
                record,
                record.FormVersion,
                record.Version2,
                record.Items.Select(item => item.FormKeyNullable ?? FormKey.Null),
                name: null,
                document.RootElement);
            if (intendedEdits.TryGetValue(expected.Key, out var intended))
            {
                ExternalNativeSampleAssertions.RequireNativeValues(
                    record.EditorID,
                    record.Items.Select(item => item.FormKeyNullable ?? FormKey.Null),
                    intended,
                    "Direct Skyrim parse");
            }
        }

        return CreateResult(mod.ModKey, expectedMasters, records.Count, association);
    }

    /// <summary>Verifies common major-record fields, exact item order, duplicates, nulls, and optional translated name.</summary>
    /// <param name="record">The direct native major record.</param>
    /// <param name="formVersion">The game-specific FormVersion.</param>
    /// <param name="version2">The game-specific Version2.</param>
    /// <param name="items">Direct native item identities in order.</param>
    /// <param name="name">The optional direct native translated name.</param>
    /// <param name="expected">The complete expected inspector record.</param>
    /// <exception cref="InvalidDataException">Thrown when any directly checked value differs.</exception>
    private static void VerifyCommon(
        IMajorRecordGetter record,
        ushort formVersion,
        ushort version2,
        IEnumerable<FormKey> items,
        ITranslatedStringGetter? name,
        JsonElement expected)
    {
        RequireEqual(record.FormKey.ToString(), expected.GetProperty("FormKey").GetString(), "FormKey", record.FormKey);
        RequireEqual(record.EditorID, expected.GetProperty("EditorID").GetString(), "EditorID", record.FormKey);
        RequireEqual(record.MajorRecordFlagsRaw, expected.GetProperty("MajorRecordFlagsRaw").GetInt32(), "MajorRecordFlagsRaw", record.FormKey);
        RequireEqual(record.VersionControl, expected.GetProperty("VersionControl").GetUInt32(), "VersionControl", record.FormKey);
        RequireEqual(formVersion, expected.GetProperty("FormVersion").GetUInt16(), "FormVersion", record.FormKey);
        RequireEqual(version2, expected.GetProperty("Version2").GetUInt16(), "Version2", record.FormKey);

        var actualItems = items.Select(item => item.ToString()).ToArray();
        var expectedItems = expected.GetProperty("Items")
            .EnumerateArray()
            .Select(item => item.GetProperty("formKey").GetString())
            .ToArray();
        if (!actualItems.SequenceEqual(expectedItems, StringComparer.Ordinal))
        {
            throw new InvalidDataException($"Direct native Items differed for '{record.FormKey}'.");
        }

        if (!expected.TryGetProperty("Name", out var expectedName))
        {
            return;
        }

        if (expectedName.ValueKind == JsonValueKind.Null)
        {
            if (name is not null)
            {
                throw new InvalidDataException($"Direct native Name was present for '{record.FormKey}' but expected null.");
            }

            return;
        }

        if (name is null)
        {
            throw new InvalidDataException($"Direct native Name was absent for '{record.FormKey}'.");
        }

        RequireEqual(name.TargetLanguage.ToString(), expectedName.GetProperty("targetLanguage").GetString(), "Name.targetLanguage", record.FormKey);
        RequireEqual(name.String, expectedName.GetProperty("value").GetString(), "Name.value", record.FormKey);
        var actualTranslations = name.OrderBy(pair => pair.Key).Select(pair => (pair.Key.ToString(), pair.Value)).ToArray();
        var expectedTranslations = expectedName.GetProperty("translations")
            .EnumerateArray()
            .Select(translation => (
                translation.GetProperty("language").GetString()!,
                translation.GetProperty("value").GetString()!))
            .ToArray();
        if (!actualTranslations.SequenceEqual(expectedTranslations))
        {
            throw new InvalidDataException($"Direct native Name translations differed for '{record.FormKey}'.");
        }
    }

    /// <summary>Verifies output identity and exact master order.</summary>
    /// <param name="actualModKey">The direct parser's output identity.</param>
    /// <param name="actualMasters">The direct parser's header masters.</param>
    /// <param name="expectedMasters">Expected masters in load-order order.</param>
    /// <param name="association">The selected output association.</param>
    /// <exception cref="InvalidDataException">Thrown when identity or master order differs.</exception>
    private static void VerifyHeader(
        ModKey actualModKey,
        IEnumerable<ModKey> actualMasters,
        IReadOnlyList<ModKey> expectedMasters,
        OutputAssociation association)
    {
        if (actualModKey != association.ModKey)
        {
            throw new InvalidDataException($"Direct native output identity '{actualModKey}' differed from '{association.ModKey}'.");
        }

        if (!actualMasters.SequenceEqual(expectedMasters))
        {
            throw new InvalidDataException("Direct native output master identities or order differed from the expected source-derived set.");
        }
    }

    /// <summary>Verifies the output contains exactly the intended sentinel and external override FormLists.</summary>
    /// <param name="actual">Directly parsed FormList identities.</param>
    /// <param name="expected">Expected output FormList identities.</param>
    /// <exception cref="InvalidDataException">Thrown when records are missing or unexpected.</exception>
    private static void VerifyRecordSet(IEnumerable<FormKey> actual, IEnumerable<FormKey> expected)
    {
        var actualKeys = actual.OrderBy(key => key.ToString(), StringComparer.Ordinal).ToArray();
        var expectedKeys = expected.OrderBy(key => key.ToString(), StringComparer.Ordinal).ToArray();
        if (!actualKeys.SequenceEqual(expectedKeys))
        {
            throw new InvalidDataException("Direct native output FormList identities differed from the exact intended record set.");
        }
    }

    /// <summary>Requires equality for one directly inspected native field.</summary>
    /// <typeparam name="T">The comparable field type.</typeparam>
    /// <param name="actual">The directly parsed value.</param>
    /// <param name="expected">The expected engine-preview value.</param>
    /// <param name="field">The field path used in diagnostics.</param>
    /// <param name="formKey">The containing native identity.</param>
    /// <exception cref="InvalidDataException">Thrown when the values differ.</exception>
    private static void RequireEqual<T>(T actual, T expected, string field, FormKey formKey)
    {
        if (!EqualityComparer<T>.Default.Equals(actual, expected))
        {
            throw new InvalidDataException($"Direct native field '{field}' differed for '{formKey}'.");
        }
    }

    /// <summary>Creates compact successful direct-reader evidence.</summary>
    /// <param name="modKey">The verified output identity.</param>
    /// <param name="masters">The verified ordered masters.</param>
    /// <param name="recordCount">The exact output FormList count.</param>
    /// <param name="association">The selected output representation.</param>
    /// <returns>Serializable direct-verification evidence.</returns>
    private static ExternalNativeVerificationResult CreateResult(
        ModKey modKey,
        IReadOnlyList<ModKey> masters,
        int recordCount,
        OutputAssociation association)
    {
        var stringsDirectory = Path.Combine(Path.GetDirectoryName(association.PluginPath)!, "Strings");
        return new ExternalNativeVerificationResult
        {
            ModKey = modKey.ToString(),
            Masters = masters.Select(master => master.ToString()).ToArray(),
            FormListCount = recordCount,
            PresentStringSidecars = Directory.Exists(stringsDirectory)
                ? Directory.EnumerateFiles(stringsDirectory).OrderBy(path => path, StringComparer.Ordinal).ToArray()
                : [],
        };
    }
}

/// <summary>Reports evidence obtained by the independent direct game-native reader.</summary>
internal sealed class ExternalNativeVerificationResult
{
    /// <summary>Initializes an empty serializable direct-verification result.</summary>
    internal ExternalNativeVerificationResult()
    { }

    /// <summary>Gets or initializes the directly parsed output identity.</summary>
    public string ModKey { get; init; } = string.Empty;

    /// <summary>Gets or initializes directly parsed masters in exact header order.</summary>
    public string[] Masters { get; init; } = [];

    /// <summary>Gets or initializes the exact directly parsed FormList count.</summary>
    public int FormListCount { get; init; }

    /// <summary>Gets or initializes present output string-sidecar paths.</summary>
    public string[] PresentStringSidecars { get; init; } = [];
}
