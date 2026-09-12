using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Enums;
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
/// Owns a disposable generated Skyrim Special Edition source fixture with full, small, localized, and overriding records.
/// </summary>
internal sealed class SkyrimNativeTestFixture : IDisposable
{
    /// <summary>The generated small, full, and override plugin paths in explicit load-order order.</summary>
    private readonly IReadOnlyList<string> _orderedPluginPaths;

    /// <summary>Tracks whether fixture cleanup has already run.</summary>
    private int _disposed;

    /// <summary>
    /// Initializes a generated Skyrim fixture and records its native identities.
    /// </summary>
    /// <param name="rootDirectory">The task-owned temporary root directory.</param>
    /// <param name="dataDirectory">The explicit fixture data directory.</param>
    /// <param name="stringsDirectory">The explicit localized-string directory.</param>
    /// <param name="orderedPluginPaths">The plugin files in strict native load-order order.</param>
    /// <param name="sourceModKey">The selected small-master source identity.</param>
    /// <param name="fullModKey">The generated full-master identity.</param>
    /// <param name="patchModKey">The generated overriding patch identity.</param>
    /// <param name="sourceListFormKey">The selected source FormList identity.</param>
    /// <param name="fullListFormKey">The full-master FormList identity with the same numeric ID.</param>
    /// <param name="deletedListFormKey">The source FormList identity whose winning override is deleted.</param>
    /// <param name="bookFormKey">The localized Book identity referenced by the source FormList.</param>
    /// <param name="keywordFormKey">The Keyword identity referenced by the source FormList.</param>
    /// <param name="missingFormKey">The intentionally unresolved identity referenced twice by the source FormList.</param>
    private SkyrimNativeTestFixture(
        DirectoryInfo rootDirectory,
        DirectoryInfo dataDirectory,
        DirectoryInfo stringsDirectory,
        IReadOnlyList<string> orderedPluginPaths,
        ModKey sourceModKey,
        ModKey fullModKey,
        ModKey patchModKey,
        FormKey sourceListFormKey,
        FormKey fullListFormKey,
        FormKey deletedListFormKey,
        FormKey bookFormKey,
        FormKey keywordFormKey,
        FormKey missingFormKey)
    {
        RootDirectory = rootDirectory;
        DataDirectory = dataDirectory;
        StringsDirectory = stringsDirectory;
        _orderedPluginPaths = Array.AsReadOnly(orderedPluginPaths.ToArray());
        SourceModKey = sourceModKey;
        FullModKey = fullModKey;
        PatchModKey = patchModKey;
        SourceListFormKey = sourceListFormKey;
        FullListFormKey = fullListFormKey;
        DeletedListFormKey = deletedListFormKey;
        BookFormKey = bookFormKey;
        KeywordFormKey = keywordFormKey;
        MissingFormKey = missingFormKey;
    }

    /// <summary>Gets the fixture's task-owned temporary root.</summary>
    public DirectoryInfo RootDirectory { get; }

    /// <summary>Gets the explicit data directory containing generated plugins.</summary>
    public DirectoryInfo DataDirectory { get; }

    /// <summary>Gets the explicit localized-string directory.</summary>
    public DirectoryInfo StringsDirectory { get; }

    /// <summary>Gets the selected small-master source plugin path.</summary>
    public string SourcePluginPath => _orderedPluginPaths[0];

    /// <summary>Gets the full-master plugin path.</summary>
    public string FullPluginPath => _orderedPluginPaths[1];

    /// <summary>Gets the overriding patch plugin path.</summary>
    public string PatchPluginPath => _orderedPluginPaths[^1];

    /// <summary>Gets all generated plugin paths in explicit native load-order order.</summary>
    public IReadOnlyList<string> OrderedPluginPaths => _orderedPluginPaths;

    /// <summary>Gets the selected small-master source identity.</summary>
    public ModKey SourceModKey { get; }

    /// <summary>Gets the full-master source identity.</summary>
    public ModKey FullModKey { get; }

    /// <summary>Gets the overriding patch identity.</summary>
    public ModKey PatchModKey { get; }

    /// <summary>Gets the source FormList identity overridden by the patch.</summary>
    public FormKey SourceListFormKey { get; }

    /// <summary>Gets the full-master FormList identity with the same numeric ID as the selected source list.</summary>
    public FormKey FullListFormKey { get; }

    /// <summary>Gets the source FormList identity whose winning override is a deleted record.</summary>
    public FormKey DeletedListFormKey { get; }

    /// <summary>Gets the localized Book identity referenced by the selected source list.</summary>
    public FormKey BookFormKey { get; }

    /// <summary>Gets the Keyword identity referenced by the selected source list.</summary>
    public FormKey KeywordFormKey { get; }

    /// <summary>Gets the intentionally unresolved identity referenced twice by the selected source list.</summary>
    public FormKey MissingFormKey { get; }

    /// <summary>
    /// Creates a generated fixture without consulting installed-game or ambient load-order state.
    /// </summary>
    /// <returns>A fixture containing full, small, localized, referenced, and overriding native records.</returns>
    public static SkyrimNativeTestFixture Create()
    {
        var rootDirectory = Directory.CreateTempSubdirectory("CreationsForge-SkyrimNative-");
        var dataDirectory = rootDirectory.CreateSubdirectory("Data");
        var stringsDirectory = rootDirectory.CreateSubdirectory("Strings");

        var source = new SkyrimMod("NativeSmall.esm", SkyrimRelease.SkyrimSE)
        {
            IsSmallMaster = true,
            UsingLocalization = true,
        };
        var full = new SkyrimMod("NativeFull.esm", SkyrimRelease.SkyrimSE);
        var patch = new SkyrimMod("NativePatch.esp", SkyrimRelease.SkyrimSE);

        var sourceList = new FormList(source, "SharedListSmall");
        source.FormLists.Add(sourceList);

        var fullList = new FormList(full, "SharedListFull");
        full.FormLists.Add(fullList);

        var deletedList = new FormList(source, "DeletedWinningList");
        source.FormLists.Add(deletedList);

        if (sourceList.FormKey.ID != fullList.FormKey.ID)
        {
            throw new InvalidOperationException("The native fixture allocator did not produce the intended cross-plugin numeric ID collision.");
        }

        var book = new Book(source, "SharedBook")
        {
            Name = CreateLocalizedName("Referenced book", "Livre référence"),
        };
        source.Books.Add(book);

        var keyword = new Keyword(source, "SharedKeyword");
        source.Keywords.Add(keyword);
        var missingFormKey = new FormKey(source.ModKey, 0x0FFF);
        sourceList.Items.Add(new FormLink<ISkyrimMajorRecordGetter>(book.FormKey));
        sourceList.Items.Add(new FormLink<ISkyrimMajorRecordGetter>(missingFormKey));
        sourceList.Items.Add(new FormLink<ISkyrimMajorRecordGetter>(keyword.FormKey));
        sourceList.Items.Add(new FormLink<ISkyrimMajorRecordGetter>(missingFormKey));

        patch.FormLists.Add(new FormList(sourceList.FormKey, SkyrimRelease.SkyrimSE)
        {
            EditorID = "SharedListOverride",
        });
        patch.FormLists.Add(new FormList(deletedList.FormKey, SkyrimRelease.SkyrimSE)
        {
            EditorID = "DeletedWinningOverride",
            IsDeleted = true,
        });

        var mods = new[] { source, full, patch };
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
            var pluginPath = Path.Combine(dataDirectory.FullName, mod.ModKey.FileName.String);
            orderedPluginPaths.Add(pluginPath);

            if (mod.UsingLocalization)
            {
                using var stringsWriter = new StringsWriter(
                    GameRelease.SkyrimSE,
                    mod.ModKey,
                    stringsDirectory.FullName,
                    new SkyrimTestEncodingProvider(),
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

        return new SkyrimNativeTestFixture(
            rootDirectory,
            dataDirectory,
            stringsDirectory,
            orderedPluginPaths,
            source.ModKey,
            full.ModKey,
            patch.ModKey,
            sourceList.FormKey,
            fullList.FormKey,
            deletedList.FormKey,
            book.FormKey,
            keyword.FormKey,
            missingFormKey);
    }

    /// <summary>
    /// Creates a normal source-open request over all generated plugins.
    /// </summary>
    /// <param name="workspaceId">An optional non-empty workspace identity, or <see langword="null"/> to generate one.</param>
    /// <returns>An explicit Skyrim Special Edition request containing no installed-game defaults.</returns>
    public WorkspaceOpenRequest CreateOpenRequest(Guid? workspaceId = null)
    {
        return CreateOpenRequest(SourcePluginPath, _orderedPluginPaths, workspaceId);
    }

    /// <summary>
    /// Creates a source-open request with caller-selected source and ordered plugin paths.
    /// </summary>
    /// <param name="sourcePluginPath">The explicit source plugin path.</param>
    /// <param name="pluginPaths">The exact admitted plugin paths in load-order order.</param>
    /// <param name="workspaceId">An optional non-empty workspace identity, or <see langword="null"/> to generate one.</param>
    /// <returns>An explicit Skyrim Special Edition request containing no installed-game defaults.</returns>
    public WorkspaceOpenRequest CreateOpenRequest(
        string sourcePluginPath,
        IReadOnlyList<string> pluginPaths,
        Guid? workspaceId = null)
    {
        return new WorkspaceOpenRequest(
            workspaceId ?? Guid.NewGuid(),
            SupportedGame.Skyrim,
            GameRelease.SkyrimSE,
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
    /// Replaces the full-master plugin bytes to simulate an external source change after opening.
    /// </summary>
    public void ChangeFullPlugin()
    {
        File.AppendAllText(FullPluginPath, "changed");
    }

    /// <summary>
    /// Deletes the task-owned generated fixture directory and every contained native artifact.
    /// </summary>
    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
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
