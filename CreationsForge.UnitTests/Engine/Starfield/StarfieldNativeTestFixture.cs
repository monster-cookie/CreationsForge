using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Enums;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Starfield;
using Mutagen.Bethesda.Strings;
using Noggog;
using System.IO.Abstractions;

namespace CreationsForge.UnitTests.Engine.Starfield;

/// <summary>
/// Owns a disposable generated Starfield source/load-order fixture with mixed native master styles.
/// </summary>
internal sealed class StarfieldNativeTestFixture : IDisposable
{
    /// <summary>The generated full, small, medium, and override plugin paths in explicit load-order order.</summary>
    private readonly IReadOnlyList<string> OrderedPluginPaths;

    /// <summary>Tracks whether fixture cleanup has already run.</summary>
    private int IsDisposed;

    /// <summary>
    /// Initializes a generated fixture and records its native identities.
    /// </summary>
    /// <param name="rootDirectory">The task-owned temporary root directory.</param>
    /// <param name="dataDirectory">The explicit fixture data directory.</param>
    /// <param name="stringsDirectory">The explicit localized-string directory.</param>
    /// <param name="orderedPluginPaths">The plugin files in strict native load-order order.</param>
    /// <param name="sourceModKey">The selected small-master source identity.</param>
    /// <param name="fullModKey">The generated full-master identity.</param>
    /// <param name="mediumModKey">The generated medium-master identity.</param>
    /// <param name="patchModKey">The generated overriding patch identity.</param>
    /// <param name="sourceListFormKey">The selected source FormList identity.</param>
    /// <param name="fullListFormKey">The full-master FormList identity with the same numeric ID.</param>
    /// <param name="mediumListFormKey">The medium-master FormList identity with the same numeric ID.</param>
    /// <param name="bookFormKey">The non-FormList Book identity referenced by the selected list.</param>
    /// <param name="keywordFormKey">The non-FormList Keyword identity referenced by the selected list.</param>
    /// <param name="deletedListFormKey">The source FormList identity whose winning override is deleted.</param>
    /// <param name="warningListFormKey">The source FormList identity containing an unresolved native link.</param>
    /// <param name="missingFormKey">The unresolved native link target used for read warnings.</param>
    private StarfieldNativeTestFixture(
        DirectoryInfo rootDirectory,
        DirectoryInfo dataDirectory,
        DirectoryInfo stringsDirectory,
        IReadOnlyList<string> orderedPluginPaths,
        ModKey sourceModKey,
        ModKey fullModKey,
        ModKey mediumModKey,
        ModKey patchModKey,
        FormKey sourceListFormKey,
        FormKey fullListFormKey,
        FormKey mediumListFormKey,
        FormKey bookFormKey,
        FormKey keywordFormKey,
        FormKey deletedListFormKey,
        FormKey warningListFormKey,
        FormKey missingFormKey)
    {
        RootDirectory = rootDirectory;
        DataDirectory = dataDirectory;
        StringsDirectory = stringsDirectory;
        OrderedPluginPaths = Array.AsReadOnly(orderedPluginPaths.ToArray());
        SourceModKey = sourceModKey;
        FullModKey = fullModKey;
        MediumModKey = mediumModKey;
        PatchModKey = patchModKey;
        SourceListFormKey = sourceListFormKey;
        FullListFormKey = fullListFormKey;
        MediumListFormKey = mediumListFormKey;
        BookFormKey = bookFormKey;
        KeywordFormKey = keywordFormKey;
        DeletedListFormKey = deletedListFormKey;
        WarningListFormKey = warningListFormKey;
        MissingFormKey = missingFormKey;
    }

    /// <summary>Gets the fixture's task-owned temporary root.</summary>
    public DirectoryInfo RootDirectory { get; }

    /// <summary>Gets the explicit data directory containing generated plugins.</summary>
    public DirectoryInfo DataDirectory { get; }

    /// <summary>Gets the explicit localized-string directory.</summary>
    public DirectoryInfo StringsDirectory { get; }

    /// <summary>Gets the selected source plugin path.</summary>
    public string SourcePluginPath => OrderedPluginPaths[0];

    /// <summary>Gets the overriding patch plugin path.</summary>
    public string PatchPluginPath => OrderedPluginPaths[^1];

    /// <summary>Gets the selected small-master source identity.</summary>
    public ModKey SourceModKey { get; }

    /// <summary>Gets the full-master load-order identity.</summary>
    public ModKey FullModKey { get; }

    /// <summary>Gets the medium-master load-order identity.</summary>
    public ModKey MediumModKey { get; }

    /// <summary>Gets the overriding patch identity.</summary>
    public ModKey PatchModKey { get; }

    /// <summary>Gets the source FormList identity that is overridden by the patch.</summary>
    public FormKey SourceListFormKey { get; }

    /// <summary>Gets the full-master FormList identity with the same numeric ID as the other fixture lists.</summary>
    public FormKey FullListFormKey { get; }

    /// <summary>Gets the medium-master FormList identity with the same numeric ID as the other fixture lists.</summary>
    public FormKey MediumListFormKey { get; }

    /// <summary>Gets the non-FormList Book identity referenced by the selected list.</summary>
    public FormKey BookFormKey { get; }

    /// <summary>Gets the non-FormList Keyword identity referenced by the selected list.</summary>
    public FormKey KeywordFormKey { get; }

    /// <summary>Gets the source FormList identity whose winning patch context is deleted.</summary>
    public FormKey DeletedListFormKey { get; }

    /// <summary>Gets the source FormList identity containing an unresolved native link.</summary>
    public FormKey WarningListFormKey { get; }

    /// <summary>Gets the unresolved native link target used for contextual-read warnings.</summary>
    public FormKey MissingFormKey { get; }

    /// <summary>
    /// Creates a generated fixture without consulting installed game or load-order state.
    /// </summary>
    /// <returns>A fixture containing full, small, medium, localized, referenced, and overriding native records.</returns>
    public static StarfieldNativeTestFixture Create()
    {
        var rootDirectory = Directory.CreateTempSubdirectory("CreationsForge-StarfieldNative-");
        var dataDirectory = rootDirectory.CreateSubdirectory("Data");
        var stringsDirectory = rootDirectory.CreateSubdirectory("Strings");

        var source = new StarfieldMod("NativeSmall.esm", StarfieldRelease.Starfield)
        {
            IsSmallMaster = true,
            UsingLocalization = true,
        };
        var full = new StarfieldMod("NativeFull.esm", StarfieldRelease.Starfield);
        var medium = new StarfieldMod("NativeMedium.esm", StarfieldRelease.Starfield)
        {
            IsMediumMaster = true,
        };
        var patch = new StarfieldMod("NativePatch.esp", StarfieldRelease.Starfield);

        var sourceList = new FormList(source, "SharedListSmall");
        sourceList.Name = CreateLocalizedName("Small source list", "Liste source petite");
        source.FormLists.Add(sourceList);

        var book = new Book(source, "SharedBook");
        book.Name = CreateLocalizedName("Referenced book", "Livre référence");
        source.Books.Add(book);

        var keyword = new Keyword(source, "SharedKeyword");
        source.Keywords.Add(keyword);
        sourceList.Items.Add(new FormLink<IStarfieldMajorRecordGetter>(book.FormKey));
        sourceList.Items.Add(new FormLink<IStarfieldMajorRecordGetter>(keyword.FormKey));

        var deletedList = new FormList(source, "DeletedSourceList")
        {
            Name = CreateLocalizedName("Deleted source list", "Liste source supprimée"),
        };
        source.FormLists.Add(deletedList);

        var missingFormKey = new FormKey(source.ModKey, 0x00000FFF);
        var warningList = new FormList(source, "MissingReferenceList")
        {
            Name = CreateLocalizedName("Missing reference list", "Liste de référence manquante"),
        };
        warningList.Items.Add(new FormLink<IStarfieldMajorRecordGetter>(missingFormKey));
        source.FormLists.Add(warningList);

        var fullList = new FormList(full, "SharedListFull")
        {
            Name = "Full list",
        };
        full.FormLists.Add(fullList);

        var mediumList = new FormList(medium, "SharedListMedium")
        {
            Name = "Medium list",
        };
        medium.FormLists.Add(mediumList);

        if (sourceList.FormKey.ID != fullList.FormKey.ID || sourceList.FormKey.ID != mediumList.FormKey.ID)
        {
            throw new InvalidOperationException("The native fixture allocator did not produce the intended cross-plugin numeric ID collision.");
        }

        patch.FormLists.Add(new FormList(sourceList.FormKey, StarfieldRelease.Starfield)
        {
            EditorID = "SharedListOverride",
            Name = "Winning override",
        });
        patch.FormLists.Add(new FormList(deletedList.FormKey, StarfieldRelease.Starfield)
        {
            EditorID = "DeletedListOverride",
            IsDeleted = true,
        });

        var mods = new[] { source, full, medium, patch };
        var orderedModKeys = mods.Select(mod => mod.ModKey).ToArray();
        var masterFlags = new Cache<IModMasterStyledGetter, ModKey>(mod => mod.ModKey);
        foreach (var mod in mods)
        {
            masterFlags.Set(mod);
        }

        var writeParameters = new BinaryWriteParameters
        {
            MasterFlagsLookup = masterFlags,
            MastersListOrdering = new MastersListOrderingByLoadOrder(orderedModKeys)
            {
                Strict = true,
            },
        };

        var orderedPluginPaths = new List<string>(mods.Length);
        foreach (var mod in mods)
        {
            var pluginPath = Path.Combine(dataDirectory.FullName, mod.ModKey.FileName.ToString());
            orderedPluginPaths.Add(pluginPath);

            if (mod.UsingLocalization)
            {
                using var stringsWriter = new StringsWriter(
                    GameRelease.Starfield,
                    mod.ModKey,
                    stringsDirectory.FullName,
                    new StarfieldTestEncodingProvider(),
                    new FileSystem());
                ((IModGetter)mod).WriteToBinary(
                    pluginPath,
                    writeParameters with
                    {
                        StringsWriter = stringsWriter,
                    });
            }
            else
            {
                ((IModGetter)mod).WriteToBinary(pluginPath, writeParameters);
            }
        }

        return new StarfieldNativeTestFixture(
            rootDirectory,
            dataDirectory,
            stringsDirectory,
            orderedPluginPaths,
            source.ModKey,
            full.ModKey,
            medium.ModKey,
            patch.ModKey,
            sourceList.FormKey,
            fullList.FormKey,
            mediumList.FormKey,
            book.FormKey,
            keyword.FormKey,
            deletedList.FormKey,
            warningList.FormKey,
            missingFormKey);
    }

    /// <summary>
    /// Creates a normal source-open request over all generated plugins.
    /// </summary>
    /// <param name="workspaceId">An optional non-empty workspace identity, or <see langword="null"/> to generate one.</param>
    /// <returns>An explicit Starfield request containing no installed-game defaults.</returns>
    public WorkspaceOpenRequest CreateOpenRequest(Guid? workspaceId = null)
    {
        return CreateOpenRequest(SourcePluginPath, OrderedPluginPaths, workspaceId);
    }

    /// <summary>
    /// Creates a source-open request with caller-selected source and ordered plugin paths.
    /// </summary>
    /// <param name="sourcePluginPath">The explicit source plugin path.</param>
    /// <param name="pluginPaths">The exact admitted plugin paths in load-order order.</param>
    /// <param name="workspaceId">An optional non-empty workspace identity, or <see langword="null"/> to generate one.</param>
    /// <returns>An explicit Starfield request containing no installed-game defaults.</returns>
    public WorkspaceOpenRequest CreateOpenRequest(
        string sourcePluginPath,
        IReadOnlyList<string> pluginPaths,
        Guid? workspaceId = null)
    {
        return new WorkspaceOpenRequest(
            workspaceId ?? Guid.NewGuid(),
            SupportedGame.Starfield,
            GameRelease.Starfield,
            sourcePluginPath,
            pluginPaths,
            DataDirectory.FullName,
            [StringsDirectory.FullName]);
    }

    /// <summary>
    /// Gets a snapshot of every generated plugin and localized-string file for byte-preservation assertions.
    /// </summary>
    /// <returns>The ordered file paths and their exact bytes.</returns>
    public IReadOnlyDictionary<string, byte[]> SnapshotArtifacts()
    {
        return DataDirectory
            .EnumerateFiles("*", SearchOption.AllDirectories)
            .Concat(StringsDirectory.EnumerateFiles("*", SearchOption.AllDirectories))
            .OrderBy(file => file.FullName, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(file => file.FullName, file => File.ReadAllBytes(file.FullName), StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Creates a second path containing the selected source bytes to exercise duplicate native identity diagnostics.
    /// </summary>
    /// <returns>The alternate plugin path with the same embedded ModKey as the source.</returns>
    public string CreateAmbiguousSourceCopy()
    {
        var alternateDirectory = RootDirectory.CreateSubdirectory("Alternate");
        var alternatePath = Path.Combine(alternateDirectory.FullName, SourceModKey.FileName.ToString());
        File.Copy(SourcePluginPath, alternatePath);
        return alternatePath;
    }

    /// <summary>
    /// Deletes the task-owned generated fixture directory and every contained native artifact.
    /// </summary>
    public void Dispose()
    {
        if (Interlocked.Exchange(ref IsDisposed, 1) != 0)
        {
            return;
        }

        RootDirectory.Refresh();
        if (RootDirectory.Exists)
        {
            RootDirectory.Delete(true);
        }
    }

    /// <summary>
    /// Creates a two-language native translated string for localized source verification.
    /// </summary>
    /// <param name="english">The default English text.</param>
    /// <param name="french">The French translation.</param>
    /// <returns>A mutable native translated string containing both languages.</returns>
    private static TranslatedString CreateLocalizedName(string english, string french)
    {
        var name = new TranslatedString(Language.English, english);
        name.Set(Language.French, french);
        return name;
    }
}
