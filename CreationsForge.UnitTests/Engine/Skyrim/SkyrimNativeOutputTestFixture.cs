using CreationsForge.Core.Engine.Contracts;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Strings;
using Noggog;
using System.IO.Abstractions;

namespace CreationsForge.UnitTests.Engine.Skyrim;

/// <summary>
/// Owns explicit Skyrim source inputs plus a complete existing output used by output-state tests.
/// </summary>
internal sealed class SkyrimNativeOutputTestFixture : IDisposable
{
    /// <summary>Tracks whether the fixture has already released its temporary source and output tree.</summary>
    private int _disposed;

    /// <summary>
    /// Initializes a complete output fixture around an owned source fixture.
    /// </summary>
    /// <param name="sources">The owned generated Skyrim source fixture.</param>
    /// <param name="outputDirectory">The explicit existing and new output directory.</param>
    /// <param name="outputStringsDirectory">The existing output's loose localized-string directory.</param>
    /// <param name="existingOutputPath">The complete localized existing output plugin path.</param>
    /// <param name="existingOutputModKey">The native existing output identity.</param>
    /// <param name="outputKeywordFormKey">The unrelated output-owned Keyword identity.</param>
    /// <param name="outputBookFormKey">The unrelated localized output-owned Book identity.</param>
    /// <param name="outputOwnListFormKey">The output-owned editable FormList identity.</param>
    /// <param name="outputDeletedListFormKey">The output-owned already-deleted FormList identity.</param>
    /// <param name="localizedOutputMode">The existing output's exact string representation.</param>
    private SkyrimNativeOutputTestFixture(
        SkyrimNativeTestFixture sources,
        DirectoryInfo outputDirectory,
        DirectoryInfo outputStringsDirectory,
        string existingOutputPath,
        ModKey existingOutputModKey,
        FormKey outputKeywordFormKey,
        FormKey outputBookFormKey,
        FormKey outputOwnListFormKey,
        FormKey outputDeletedListFormKey,
        LocalizedOutputMode localizedOutputMode)
    {
        Sources = sources;
        OutputDirectory = outputDirectory;
        OutputStringsDirectory = outputStringsDirectory;
        ExistingOutputPath = existingOutputPath;
        ExistingOutputModKey = existingOutputModKey;
        OutputKeywordFormKey = outputKeywordFormKey;
        OutputBookFormKey = outputBookFormKey;
        OutputOwnListFormKey = outputOwnListFormKey;
        OutputDeletedListFormKey = outputDeletedListFormKey;
        ExistingLocalizedOutputMode = localizedOutputMode;
    }

    /// <summary>Gets the owned explicit Skyrim source fixture.</summary>
    public SkyrimNativeTestFixture Sources { get; }

    /// <summary>Gets the explicit output directory.</summary>
    public DirectoryInfo OutputDirectory { get; }

    /// <summary>Gets the existing output's loose localized-string directory.</summary>
    public DirectoryInfo OutputStringsDirectory { get; }

    /// <summary>Gets the complete existing output plugin path.</summary>
    public string ExistingOutputPath { get; }

    /// <summary>Gets the existing output native identity.</summary>
    public ModKey ExistingOutputModKey { get; }

    /// <summary>Gets the unrelated output-owned Keyword identity.</summary>
    public FormKey OutputKeywordFormKey { get; }

    /// <summary>Gets the unrelated localized output-owned Book identity.</summary>
    public FormKey OutputBookFormKey { get; }

    /// <summary>Gets the output-owned editable FormList identity.</summary>
    public FormKey OutputOwnListFormKey { get; }

    /// <summary>Gets the output-owned already-deleted FormList identity.</summary>
    public FormKey OutputDeletedListFormKey { get; }

    /// <summary>Gets the existing output's exact string representation.</summary>
    public LocalizedOutputMode ExistingLocalizedOutputMode { get; }

    /// <summary>
    /// Creates complete generated source and output artifacts without consulting installed-game state.
    /// </summary>
    /// <param name="localizedOutputMode">The separate-file or embedded representation for the generated existing output.</param>
    /// <returns>An owned fixture with a full .esm output and unrelated native records.</returns>
    public static SkyrimNativeOutputTestFixture Create(
        LocalizedOutputMode localizedOutputMode = LocalizedOutputMode.SeparateStringFiles)
    {
        var sources = SkyrimNativeTestFixture.Create();
        try
        {
            var outputDirectory = sources.RootDirectory.CreateSubdirectory("Output");
            var outputStringsDirectory = outputDirectory.CreateSubdirectory("Strings");
            var outputModKey = ModKey.FromFileName("ExistingOutput.esm");
            var output = new SkyrimMod(outputModKey, SkyrimRelease.SkyrimSE)
            {
                IsMaster = true,
                UsingLocalization = localizedOutputMode == LocalizedOutputMode.SeparateStringFiles,
            };

            var keyword = new Keyword(output, "OutputKeyword");
            output.Keywords.Add(keyword);
            var book = new Book(output, "OutputBook")
            {
                Name = CreateLocalizedName("Output book", "Livre de sortie"),
            };
            output.Books.Add(book);
            var ownList = new FormList(output, "ExistingOwnList")
            {
                FormVersion = 44,
                Version2 = 7,
                VersionControl = 0x01020304,
                MajorRecordFlagsRaw = unchecked((int)0x40080000),
            };
            ownList.Items.Add(new FormLink<ISkyrimMajorRecordGetter>(sources.BookFormKey));
            ownList.Items.Add(new FormLink<ISkyrimMajorRecordGetter>(keyword.FormKey));
            ownList.Items.Add(new FormLink<ISkyrimMajorRecordGetter>(keyword.FormKey));
            ownList.Items.Add(new FormLink<ISkyrimMajorRecordGetter>(FormKey.Null));
            ownList.Items.Add(new FormLink<ISkyrimMajorRecordGetter>(sources.MissingFormKey));
            output.FormLists.Add(ownList);

            var deletedOwnList = new FormList(output, "ExistingDeletedList")
            {
                IsDeleted = true,
            };
            output.FormLists.Add(deletedOwnList);
            output.FormLists.Add(new FormList(sources.SourceListFormKey, SkyrimRelease.SkyrimSE)
            {
                EditorID = "ExistingStagedOverride",
                FormVersion = 43,
            });

            var masterFlags = new Cache<IModMasterStyledGetter, ModKey>(mod => mod.ModKey);
            masterFlags.Set(new KeyedMasterStyle(sources.SourceModKey, MasterStyle.Small));
            masterFlags.Set(new KeyedMasterStyle(sources.FullModKey, MasterStyle.Full));
            masterFlags.Set(new KeyedMasterStyle(sources.PatchModKey, MasterStyle.Full));
            masterFlags.Set(output);
            var loadOrder = new[]
            {
                sources.SourceModKey,
                sources.FullModKey,
                sources.PatchModKey,
                outputModKey,
            };
            var writeParameters = new BinaryWriteParameters
            {
                MasterFlagsLookup = masterFlags,
                MastersListOrdering = new MastersListOrderingByLoadOrder(loadOrder)
                {
                    Strict = true,
                },
            };
            var outputPath = Path.Combine(outputDirectory.FullName, outputModKey.FileName.String);
            if (localizedOutputMode == LocalizedOutputMode.SeparateStringFiles)
            {
                using var stringsWriter = new StringsWriter(
                    GameRelease.SkyrimSE,
                    outputModKey,
                    outputStringsDirectory.FullName,
                    new SkyrimTestEncodingProvider(),
                    new FileSystem());
                ((IModGetter)output).WriteToBinary(
                    outputPath,
                    writeParameters with
                    {
                        StringsWriter = stringsWriter,
                    });
            }
            else
            {
                ((IModGetter)output).WriteToBinary(outputPath, writeParameters);
            }

            return new SkyrimNativeOutputTestFixture(
                sources,
                outputDirectory,
                outputStringsDirectory,
                outputPath,
                outputModKey,
                keyword.FormKey,
                book.FormKey,
                ownList.FormKey,
                deletedOwnList.FormKey,
                localizedOutputMode);
        }
        catch
        {
            sources.Dispose();
            throw;
        }
    }

    /// <summary>Creates the full existing-output association.</summary>
    /// <returns>The exact association matching the generated existing output.</returns>
    public OutputAssociation CreateExistingAssociation()
    {
        return new OutputAssociation(
            ExistingOutputPath,
            ExistingOutputModKey,
            ExistingLocalizedOutputMode,
            OutputMasterStyle.Full);
    }

    /// <summary>Creates an absent output association inside the explicit existing output directory.</summary>
    /// <param name="fileName">The absent native output file name.</param>
    /// <param name="localizedOutputMode">The requested embedded or separate localized string mode.</param>
    /// <param name="masterStyle">The requested native master style.</param>
    /// <returns>The exact association for an absent output path.</returns>
    public OutputAssociation CreateNewAssociation(
        string fileName,
        LocalizedOutputMode localizedOutputMode = LocalizedOutputMode.Embedded,
        OutputMasterStyle masterStyle = OutputMasterStyle.Full)
    {
        return new OutputAssociation(
            Path.Combine(OutputDirectory.FullName, fileName),
            ModKey.FromFileName(fileName),
            localizedOutputMode,
            masterStyle);
    }

    /// <summary>Gets exact bytes for every existing output plugin and loose localized-string artifact.</summary>
    /// <returns>The canonically ordered output paths and their exact bytes.</returns>
    public IReadOnlyDictionary<string, byte[]> SnapshotOutputArtifacts()
    {
        return OutputDirectory
            .EnumerateFiles("*", SearchOption.AllDirectories)
            .OrderBy(file => file.FullName, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(file => file.FullName, file => File.ReadAllBytes(file.FullName), StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>Releases the complete task-owned source and output artifact tree.</summary>
    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) == 0)
        {
            Sources.Dispose();
        }
    }

    /// <summary>Creates one independent two-language native translated string.</summary>
    /// <param name="english">The English value.</param>
    /// <param name="french">The French value.</param>
    /// <returns>The complete translated native string.</returns>
    private static TranslatedString CreateLocalizedName(string english, string french)
    {
        var name = new TranslatedString(Language.English, english);
        name.Set(Language.French, french);
        return name;
    }
}
