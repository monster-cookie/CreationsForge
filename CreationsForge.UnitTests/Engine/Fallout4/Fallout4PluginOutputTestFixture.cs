using CreationsForge.Core.Engine.Contracts;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using MutagenMasterStyle = Mutagen.Bethesda.Plugins.MasterStyle;

namespace CreationsForge.UnitTests.Engine.Fallout4;

/// <summary>Owns generated Fallout 4 sources and a distinct disposable output directory.</summary>
internal sealed class Fallout4PluginOutputTestFixture : IDisposable
{
    /// <summary>Tracks whether fixture cleanup has already run.</summary>
    private int _disposed;

    /// <summary>Initializes a generated output fixture around an owned source fixture.</summary>
    /// <param name="sources">The generated source and load-order fixture.</param>
    /// <param name="outputDirectory">The distinct existing output parent directory.</param>
    private Fallout4PluginOutputTestFixture(
        Fallout4PluginTestFixture sources,
        DirectoryInfo outputDirectory)
    {
        Sources = sources;
        OutputDirectory = outputDirectory;
    }

    /// <summary>Gets the owned generated source and load-order fixture.</summary>
    public Fallout4PluginTestFixture Sources { get; }

    /// <summary>Gets the output parent directory used only by this fixture.</summary>
    public DirectoryInfo OutputDirectory { get; }

    /// <summary>Creates isolated generated Fallout 4 source and output directories.</summary>
    /// <returns>A fixture with no selected output artifacts.</returns>
    public static Fallout4PluginOutputTestFixture Create()
    {
        var sources = Fallout4PluginTestFixture.Create();
        try
        {
            var outputDirectory = sources.RootDirectory.CreateSubdirectory("Output");
            return new Fallout4PluginOutputTestFixture(sources, outputDirectory);
        }
        catch
        {
            sources.Dispose();
            throw;
        }
    }

    /// <summary>Creates an output association without creating its plugin or strings artifacts.</summary>
    /// <param name="fileName">The output plugin file name.</param>
    /// <param name="masterStyle">The requested plugin master style.</param>
    /// <param name="localizedOutputMode">The requested embedded or separate strings representation.</param>
    /// <returns>An association whose parent output directory already exists.</returns>
    public OutputAssociation CreateAssociation(
        string fileName,
        OutputMasterStyle masterStyle = OutputMasterStyle.Full,
        LocalizedOutputMode localizedOutputMode = LocalizedOutputMode.Embedded)
    {
        var modKey = ModKey.FromNameAndExtension(fileName);
        return new OutputAssociation(
            Path.Combine(OutputDirectory.FullName, fileName),
            modKey,
            localizedOutputMode,
            masterStyle);
    }

    /// <summary>Writes a complete existing output with headers, unrelated records, output-owned lists, and one source override.</summary>
    /// <param name="fileName">The output plugin file name.</param>
    /// <param name="masterStyle">The full or small plugin master style.</param>
    /// <param name="author">The header author used to distinguish rewritten baselines.</param>
    /// <param name="collidingNextFormId">Whether to preserve an allocator value already occupied by an unrelated Keyword.</param>
    /// <param name="includeMissingReference">Whether the output-owned FormList includes one unresolved output-local item.</param>
    /// <returns>The association and plugin identities needed for preservation and selection assertions.</returns>
    public (
        OutputAssociation Association,
        FormKey OwnListFormKey,
        FormKey SourceOverrideFormKey,
        FormKey DeletedListFormKey,
        FormKey KeywordFormKey,
        FormKey BookFormKey) WriteExistingOutput(
        string fileName = "ExistingOutput.esm",
        OutputMasterStyle masterStyle = OutputMasterStyle.Full,
        string author = "Output fixture author",
        bool collidingNextFormId = false,
        bool includeMissingReference = false)
    {
        if (masterStyle == OutputMasterStyle.Medium)
        {
            throw new ArgumentException("The Fallout 4 output fixture cannot write a medium master.", nameof(masterStyle));
        }

        var association = CreateAssociation(fileName, masterStyle);
        var mod = new Fallout4Mod(association.ModKey, Fallout4Release.Fallout4)
        {
            IsMaster = association.ModKey.Type == ModType.Master,
            IsSmallMaster = masterStyle == OutputMasterStyle.Small,
        };
        mod.ModHeader.Author = author;
        mod.ModHeader.Description = "Complete Fallout 4 output fixture";
        mod.ModHeader.Version = 13;
        mod.ModHeader.FormVersion = 44;
        mod.ModHeader.Version2 = 7;
        mod.ModHeader.INCC = 3;

        var keyword = new Keyword(mod, "OutputKeyword");
        mod.Keywords.Add(keyword);
        var ownList = new FormList(mod, "OutputOwnedList")
        {
            Name = "Output list",
            MajorRecordFlagsRaw = unchecked((int)0x40000000),
        };
        ownList.Items.Add(new FormLink<IFallout4MajorRecordGetter>(keyword.FormKey));
        ownList.Items.Add(new FormLink<IFallout4MajorRecordGetter>(keyword.FormKey));
        ownList.Items.Add(new FormLink<IFallout4MajorRecordGetter>(Sources.BookFormKey));
        if (includeMissingReference)
        {
            ownList.Items.Add(new FormLink<IFallout4MajorRecordGetter>(
                new FormKey(mod.ModKey, 0x0F01)));
        }

        mod.FormLists.Add(ownList);

        var sourceOverride = new FormList(Sources.SourceListFormKey, Fallout4Release.Fallout4)
        {
            EditorID = "ExistingSourceOverride",
            Name = "Existing output override",
            Items = new ExtendedList<IFormLinkGetter<IFallout4MajorRecordGetter>>
            {
                new FormLink<IFallout4MajorRecordGetter>(Sources.BookFormKey),
                new FormLink<IFallout4MajorRecordGetter>(Sources.KeywordFormKey),
            },
        };
        mod.FormLists.Add(sourceOverride);

        var deletedList = new FormList(mod, "DeletedOutputList")
        {
            IsDeleted = true,
        };
        mod.FormLists.Add(deletedList);

        if (collidingNextFormId)
        {
            ((IModHeaderCommon)mod.ModHeader).NextFormID = keyword.FormKey.ID;
        }

        var masterFlags = new Cache<IModMasterStyledGetter, ModKey>(metadata => metadata.ModKey);
        masterFlags.Set(new KeyedMasterStyle(Sources.SourceModKey, MutagenMasterStyle.Small));
        masterFlags.Set(new KeyedMasterStyle(Sources.FullModKey, MutagenMasterStyle.Full));
        masterFlags.Set(new KeyedMasterStyle(Sources.PatchModKey, MutagenMasterStyle.Full));
        masterFlags.Set(new KeyedMasterStyle(
            mod.ModKey,
            masterStyle == OutputMasterStyle.Small ? MutagenMasterStyle.Small : MutagenMasterStyle.Full));
        var ordering = new MastersListOrderingByLoadOrder(
        [
            Sources.SourceModKey,
            Sources.FullModKey,
            Sources.PatchModKey,
            mod.ModKey,
        ])
        {
            Strict = true,
        };
        var writeParameters = new BinaryWriteParameters
        {
            MasterFlagsLookup = masterFlags,
            MastersListOrdering = ordering,
        };
        if (collidingNextFormId)
        {
            writeParameters = writeParameters with { NextFormID = NextFormIDOption.NoCheck };
        }

        ((IModGetter)mod).WriteToBinary(
            association.PluginPath,
            writeParameters);

        return (
            association,
            ownList.FormKey,
            sourceOverride.FormKey,
            deletedList.FormKey,
            keyword.FormKey,
            Sources.BookFormKey);
    }

    /// <summary>Disposes the owned source fixture and recursively removes every generated output artifact.</summary>
    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
        {
            return;
        }

        Sources.Dispose();
    }
}
