using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Enums;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;

namespace CreationsForge.UnitTests.Engine.Fallout4;

/// <summary>
/// Owns a disposable generated Fallout 4 source/load-order fixture with full and small plugin master styles.
/// </summary>
internal sealed class Fallout4PluginTestFixture : IDisposable
{
    /// <summary>The generated small, full, and override plugin paths in explicit load-order order.</summary>
    private readonly IReadOnlyList<string> _orderedPluginPaths;

    /// <summary>Tracks whether fixture cleanup has already run.</summary>
    private int _disposed;

    /// <summary>
    /// Initializes a generated Fallout 4 fixture and records its plugin identities.
    /// </summary>
    /// <param name="rootDirectory">The task-owned temporary root directory.</param>
    /// <param name="dataDirectory">The explicit fixture data directory.</param>
    /// <param name="stringsDirectory">The explicit localized-string directory.</param>
    /// <param name="orderedPluginPaths">The plugin files in strict plugin load-order order.</param>
    /// <param name="sourceModKey">The selected small-master source identity.</param>
    /// <param name="fullModKey">The generated full-master identity.</param>
    /// <param name="patchModKey">The generated overriding patch identity.</param>
    /// <param name="sourceListFormKey">The selected source FormList identity.</param>
    /// <param name="fullListFormKey">The full-master FormList identity with the same numeric ID.</param>
    /// <param name="bookFormKey">The non-FormList Book identity referenced by the selected list.</param>
    /// <param name="keywordFormKey">The non-FormList Keyword identity referenced by the selected list.</param>
    /// <param name="deletedListFormKey">The source FormList identity deleted by the final load-order context.</param>
    /// <param name="missingReferenceListFormKey">The FormList identity containing one deliberately unresolved direct item.</param>
    /// <param name="missingReferenceFormKey">The deliberately unresolved record identity referenced by the fixture.</param>
    private Fallout4PluginTestFixture(
        DirectoryInfo rootDirectory,
        DirectoryInfo dataDirectory,
        DirectoryInfo stringsDirectory,
        IReadOnlyList<string> orderedPluginPaths,
        ModKey sourceModKey,
        ModKey fullModKey,
        ModKey patchModKey,
        FormKey sourceListFormKey,
        FormKey fullListFormKey,
        FormKey bookFormKey,
        FormKey keywordFormKey,
        FormKey deletedListFormKey,
        FormKey missingReferenceListFormKey,
        FormKey missingReferenceFormKey)
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
        BookFormKey = bookFormKey;
        KeywordFormKey = keywordFormKey;
        DeletedListFormKey = deletedListFormKey;
        MissingReferenceListFormKey = missingReferenceListFormKey;
        MissingReferenceFormKey = missingReferenceFormKey;
    }

    /// <summary>Gets the fixture's task-owned temporary root.</summary>
    public DirectoryInfo RootDirectory { get; }

    /// <summary>Gets the explicit data directory containing generated plugins.</summary>
    public DirectoryInfo DataDirectory { get; }

    /// <summary>Gets the explicit localized-string directory.</summary>
    public DirectoryInfo StringsDirectory { get; }

    /// <summary>Gets the selected small-master source plugin path.</summary>
    public string SourcePluginPath => _orderedPluginPaths[0];

    /// <summary>Gets the overriding patch plugin path.</summary>
    public string PatchPluginPath => _orderedPluginPaths[^1];

    /// <summary>Gets the selected small-master source identity.</summary>
    public ModKey SourceModKey { get; }

    /// <summary>Gets the full-master load-order identity.</summary>
    public ModKey FullModKey { get; }

    /// <summary>Gets the overriding patch identity.</summary>
    public ModKey PatchModKey { get; }

    /// <summary>Gets the source FormList identity that is overridden by the patch.</summary>
    public FormKey SourceListFormKey { get; }

    /// <summary>Gets the full-master FormList identity with the same numeric ID as the source list.</summary>
    public FormKey FullListFormKey { get; }

    /// <summary>Gets the non-FormList Book identity referenced by the selected list.</summary>
    public FormKey BookFormKey { get; }

    /// <summary>Gets the non-FormList Keyword identity referenced by the selected list.</summary>
    public FormKey KeywordFormKey { get; }

    /// <summary>Gets the source FormList identity deleted by the final load-order context.</summary>
    public FormKey DeletedListFormKey { get; }

    /// <summary>Gets the FormList identity containing one deliberately unresolved direct item.</summary>
    public FormKey MissingReferenceListFormKey { get; }

    /// <summary>Gets the deliberately unresolved record identity referenced by the fixture.</summary>
    public FormKey MissingReferenceFormKey { get; }

    /// <summary>
    /// Creates a generated fixture without consulting installed game or load-order state.
    /// </summary>
    /// <returns>A fixture containing full, small, referenced, and overriding records.</returns>
    public static Fallout4PluginTestFixture Create()
    {
        var rootDirectory = Directory.CreateTempSubdirectory("CreationsForge-Fallout4Plugin-");
        var dataDirectory = rootDirectory.CreateSubdirectory("Data");
        var stringsDirectory = rootDirectory.CreateSubdirectory("Strings");

        var source = new Fallout4Mod("PluginSmall.esm", Fallout4Release.Fallout4)
        {
            IsSmallMaster = true,
        };
        var full = new Fallout4Mod("PluginFull.esm", Fallout4Release.Fallout4);
        var patch = new Fallout4Mod("PluginPatch.esp", Fallout4Release.Fallout4);

        var sourceList = new FormList(source, "SharedListSmall")
        {
            Name = "Small source list",
        };
        source.FormLists.Add(sourceList);

        var book = new Book(source, "SharedBook")
        {
            Name = "Referenced book",
        };
        source.Books.Add(book);

        var keyword = new Keyword(source, "SharedKeyword");
        source.Keywords.Add(keyword);
        sourceList.Items.Add(new FormLink<IFallout4MajorRecordGetter>(book.FormKey));
        sourceList.Items.Add(new FormLink<IFallout4MajorRecordGetter>(keyword.FormKey));

        var deletedList = new FormList(source, "DeletedListSource")
        {
            Name = "Deleted source list",
        };
        source.FormLists.Add(deletedList);

        var missingReferenceFormKey = new FormKey(source.ModKey, 0x0F00);
        var missingReferenceList = new FormList(source, "MissingReferenceList")
        {
            Name = "Missing reference list",
        };
        missingReferenceList.Items.Add(new FormLink<IFallout4MajorRecordGetter>(book.FormKey));
        missingReferenceList.Items.Add(new FormLink<IFallout4MajorRecordGetter>(FormKey.Null));
        missingReferenceList.Items.Add(new FormLink<IFallout4MajorRecordGetter>(missingReferenceFormKey));
        source.FormLists.Add(missingReferenceList);

        var fullList = new FormList(full, "SharedListFull")
        {
            Name = "Full list",
        };
        full.FormLists.Add(fullList);

        if (sourceList.FormKey.ID != fullList.FormKey.ID)
        {
            throw new InvalidOperationException("The plugin fixture allocator did not produce the intended cross-plugin numeric ID collision.");
        }

        patch.FormLists.Add(new FormList(sourceList.FormKey, Fallout4Release.Fallout4)
        {
            EditorID = "SharedListOverride",
            Name = "Winning override",
            Items = new ExtendedList<IFormLinkGetter<IFallout4MajorRecordGetter>>
            {
                new FormLink<IFallout4MajorRecordGetter>(book.FormKey),
                new FormLink<IFallout4MajorRecordGetter>(keyword.FormKey),
            },
        });
        patch.FormLists.Add(new FormList(deletedList.FormKey, Fallout4Release.Fallout4)
        {
            EditorID = "DeletedListOverride",
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
            ((IModGetter)mod).WriteToBinary(pluginPath, writeParameters);
        }

        return new Fallout4PluginTestFixture(
            rootDirectory,
            dataDirectory,
            stringsDirectory,
            orderedPluginPaths,
            source.ModKey,
            full.ModKey,
            patch.ModKey,
            sourceList.FormKey,
            fullList.FormKey,
            book.FormKey,
            keyword.FormKey,
            deletedList.FormKey,
            missingReferenceList.FormKey,
            missingReferenceFormKey);
    }

    /// <summary>
    /// Creates a normal source-open request over all generated plugins.
    /// </summary>
    /// <param name="workspaceId">An optional non-empty workspace identity, or <see langword="null"/> to generate one.</param>
    /// <returns>An explicit Fallout 4 request containing no installed-game defaults.</returns>
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
    /// <returns>An explicit Fallout 4 request containing no installed-game defaults.</returns>
    public WorkspaceOpenRequest CreateOpenRequest(
        string sourcePluginPath,
        IReadOnlyList<string> pluginPaths,
        Guid? workspaceId = null)
    {
        return new WorkspaceOpenRequest(
            workspaceId ?? Guid.NewGuid(),
            SupportedGame.Fallout4,
            GameRelease.Fallout4,
            sourcePluginPath,
            pluginPaths,
            DataDirectory.FullName,
            [StringsDirectory.FullName]);
    }

    /// <summary>
    /// Gets a snapshot of every generated plugin for byte-preservation assertions.
    /// </summary>
    /// <returns>The ordered file paths and their exact bytes.</returns>
    public IReadOnlyDictionary<string, byte[]> SnapshotArtifacts()
    {
        return DataDirectory
            .EnumerateFiles("*", SearchOption.AllDirectories)
            .OrderBy(file => file.FullName, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(file => file.FullName, file => File.ReadAllBytes(file.FullName), StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Creates a second path containing the selected source bytes to exercise duplicate record identity diagnostics.
    /// </summary>
    /// <returns>The alternate plugin path with the same record identity as the source.</returns>
    public string CreateAmbiguousSourceCopy()
    {
        var alternateDirectory = RootDirectory.CreateSubdirectory("Alternate");
        var alternatePath = Path.Combine(alternateDirectory.FullName, SourceModKey.FileName.String);
        File.Copy(SourcePluginPath, alternatePath);
        return alternatePath;
    }

    /// <summary>
    /// Deletes the task-owned generated fixture directory and every contained plugin artifact.
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
}
