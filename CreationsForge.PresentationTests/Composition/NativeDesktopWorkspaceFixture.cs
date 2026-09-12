using Autofac;
using CreationsForge.Composition;
using CreationsForge.Core.Configuration;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Models.Configuration;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Records;
using Serilog;
using Fallout4FormList = Mutagen.Bethesda.Fallout4.FormList;
using Fallout4MajorRecordGetter = Mutagen.Bethesda.Fallout4.IFallout4MajorRecordGetter;
using Fallout4Mod = Mutagen.Bethesda.Fallout4.Fallout4Mod;
using Fallout4Release = Mutagen.Bethesda.Fallout4.Fallout4Release;
using SkyrimFormList = Mutagen.Bethesda.Skyrim.FormList;
using SkyrimMajorRecordGetter = Mutagen.Bethesda.Skyrim.ISkyrimMajorRecordGetter;
using SkyrimMod = Mutagen.Bethesda.Skyrim.SkyrimMod;
using SkyrimRelease = Mutagen.Bethesda.Skyrim.SkyrimRelease;
using StarfieldFormList = Mutagen.Bethesda.Starfield.FormList;
using StarfieldMajorRecordGetter = Mutagen.Bethesda.Starfield.IStarfieldMajorRecordGetter;
using StarfieldMod = Mutagen.Bethesda.Starfield.StarfieldMod;
using StarfieldRelease = Mutagen.Bethesda.Starfield.StarfieldRelease;

namespace CreationsForge.PresentationTests.Composition;

/// <summary>
/// Owns minimal generated source and output plugins for exercising the production native desktop container without installed-game state.
/// </summary>
internal sealed class NativeDesktopWorkspaceFixture : IDisposable
{
    /// <summary>Tracks whether the fixture has already deleted its temporary tree.</summary>
    private int _disposed;

    /// <summary>Initializes the common fixture view after the game-specific plugins have been written.</summary>
    /// <param name="rootDirectory">The fixture-owned temporary root.</param>
    /// <param name="dataDirectory">The explicit native source data directory.</param>
    /// <param name="outputDirectory">The parent for independently selected outputs.</param>
    /// <param name="game">The selected CreationsForge game.</param>
    /// <param name="release">The exact Mutagen game release.</param>
    /// <param name="sourcePluginPath">The generated read-only source plugin.</param>
    /// <param name="sourceFormKey">The generated source FormList identity.</param>
    /// <param name="existingOutput">The generated existing output association.</param>
    /// <param name="existingOutputFormKey">The generated output-owned FormList identity.</param>
    private NativeDesktopWorkspaceFixture(
        DirectoryInfo rootDirectory,
        DirectoryInfo dataDirectory,
        DirectoryInfo outputDirectory,
        SupportedGame game,
        GameRelease release,
        string sourcePluginPath,
        FormKey sourceFormKey,
        OutputAssociation existingOutput,
        FormKey existingOutputFormKey)
    {
        RootDirectory = rootDirectory;
        DataDirectory = dataDirectory;
        OutputDirectory = outputDirectory;
        Game = game;
        Release = release;
        SourcePluginPath = sourcePluginPath;
        SourceFormKey = sourceFormKey;
        ExistingOutput = existingOutput;
        ExistingOutputFormKey = existingOutputFormKey;
    }

    /// <summary>Gets the fixture-owned temporary root.</summary>
    public DirectoryInfo RootDirectory { get; }

    /// <summary>Gets the explicit source data directory.</summary>
    public DirectoryInfo DataDirectory { get; }

    /// <summary>Gets the parent directory for generated and newly selected outputs.</summary>
    public DirectoryInfo OutputDirectory { get; }

    /// <summary>Gets the selected CreationsForge game.</summary>
    public SupportedGame Game { get; }

    /// <summary>Gets the exact Mutagen release.</summary>
    public GameRelease Release { get; }

    /// <summary>Gets the generated read-only source plugin path.</summary>
    public string SourcePluginPath { get; }

    /// <summary>Gets the generated source FormList identity.</summary>
    public FormKey SourceFormKey { get; }

    /// <summary>Gets the complete association for the generated existing output.</summary>
    public OutputAssociation ExistingOutput { get; }

    /// <summary>Gets the generated output-owned FormList identity.</summary>
    public FormKey ExistingOutputFormKey { get; }

    /// <summary>Creates minimal embedded source and existing-output plugins for one supported game.</summary>
    /// <param name="game">The game whose native adapters must open the generated plugins.</param>
    /// <returns>A disposable fixture containing explicit source and output paths.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="game"/> is not supported.</exception>
    public static NativeDesktopWorkspaceFixture Create(SupportedGame game)
    {
        var rootDirectory = Directory.CreateTempSubdirectory("CreationsForge-NativeDesktopWorkspace-");
        try
        {
            var dataDirectory = rootDirectory.CreateSubdirectory("Data");
            var outputDirectory = rootDirectory.CreateSubdirectory("Output");
            var sourcePluginPath = Path.Combine(dataDirectory.FullName, "PresentationSource.esm");
            var existingOutputDirectory = outputDirectory.CreateSubdirectory("Existing");
            var existingOutputPath = Path.Combine(existingOutputDirectory.FullName, "PresentationExistingOutput.esm");

            var records = game switch
            {
                SupportedGame.Starfield => WriteStarfieldPlugins(sourcePluginPath, existingOutputPath),
                SupportedGame.Fallout4 => WriteFallout4Plugins(sourcePluginPath, existingOutputPath),
                SupportedGame.Skyrim => WriteSkyrimPlugins(sourcePluginPath, existingOutputPath),
                _ => throw new ArgumentOutOfRangeException(nameof(game), game, "A native desktop fixture requires a supported game."),
            };

            var release = game switch
            {
                SupportedGame.Starfield => GameRelease.Starfield,
                SupportedGame.Fallout4 => GameRelease.Fallout4,
                SupportedGame.Skyrim => GameRelease.SkyrimSE,
                _ => throw new ArgumentOutOfRangeException(nameof(game), game, "A native desktop fixture requires a supported game."),
            };
            var existingOutput = CreateAssociation(existingOutputPath);
            return new NativeDesktopWorkspaceFixture(
                rootDirectory,
                dataDirectory,
                outputDirectory,
                game,
                release,
                sourcePluginPath,
                records.SourceFormKey,
                existingOutput,
                records.OutputFormKey);
        }
        catch
        {
            DeleteOwnedDirectory(rootDirectory);
            throw;
        }
    }

    /// <summary>Creates an isolated production desktop container whose configuration and diagnostics paths remain inside this fixture.</summary>
    /// <param name="ownerName">A filesystem-safe name that distinguishes independent container state.</param>
    /// <returns>The application-owned production container.</returns>
    public IContainer CreateContainer(string ownerName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ownerName);
        var applicationDirectory = RootDirectory.CreateSubdirectory($"Application-{ownerName}");
        var configurationStore = new ApplicationConfigurationStore(
            Path.Combine(applicationDirectory.FullName, "CreationsForge.Config.json"));
        configurationStore.Save(new ApplicationConfiguration
        {
            ApplicationDataDirectory = applicationDirectory.FullName,
            LoggingDirectory = applicationDirectory.FullName,
        });
        return NativeDesktopComposition.Create(configurationStore, new LoggerConfiguration().CreateLogger());
    }

    /// <summary>Creates a complete source-open request with a fresh workspace identity.</summary>
    /// <returns>The explicit native request containing no installed-game defaults.</returns>
    public WorkspaceOpenRequest CreateSourceRequest()
    {
        return new WorkspaceOpenRequest(
            Guid.NewGuid(),
            Game,
            Release,
            SourcePluginPath,
            [SourcePluginPath],
            DataDirectory.FullName,
            []);
    }

    /// <summary>Creates an absent full-master embedded output under an isolated fixture-owned directory.</summary>
    /// <param name="ownerName">A filesystem-safe name that distinguishes the output selection.</param>
    /// <returns>An association whose output plugin does not exist.</returns>
    public OutputAssociation CreateNewOutput(string ownerName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ownerName);
        var directory = OutputDirectory.CreateSubdirectory($"New-{ownerName}");
        return CreateAssociation(Path.Combine(directory.FullName, "PresentationCreatedOutput.esm"));
    }

    /// <summary>Reads the exact source plugin bytes for preservation assertions.</summary>
    /// <returns>A new byte array containing the complete source plugin.</returns>
    public byte[] SnapshotSourceBytes()
    {
        return File.ReadAllBytes(SourcePluginPath);
    }

    /// <summary>Reads the complete artifact membership of this fixture's embedded existing output.</summary>
    /// <returns>The plugin bytes keyed by normalized absolute path; embedded fixtures have no localized sidecars.</returns>
    public IReadOnlyDictionary<string, byte[]> SnapshotExistingOutputArtifacts()
    {
        return new Dictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase)
        {
            [Path.GetFullPath(ExistingOutput.PluginPath)] = File.ReadAllBytes(ExistingOutput.PluginPath),
        };
    }

    /// <summary>Releases the fixture by deleting only its verified task-owned temporary root.</summary>
    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
        {
            return;
        }

        DeleteOwnedDirectory(RootDirectory);
    }

    /// <summary>Writes a minimal Starfield source and distinct existing output.</summary>
    /// <param name="sourcePluginPath">The source plugin destination.</param>
    /// <param name="existingOutputPath">The existing output plugin destination.</param>
    /// <returns>The native source and output FormList identities.</returns>
    private static FixtureRecords WriteStarfieldPlugins(string sourcePluginPath, string existingOutputPath)
    {
        var source = new StarfieldMod("PresentationSource.esm", StarfieldRelease.Starfield)
        {
            IsMaster = true,
        };
        var sourceList = new StarfieldFormList(source, "PresentationSourceList");
        sourceList.Items.Add(new FormLink<StarfieldMajorRecordGetter>(sourceList.FormKey));
        sourceList.Items.Add(new FormLink<StarfieldMajorRecordGetter>(sourceList.FormKey));
        sourceList.Items.Add(new FormLink<StarfieldMajorRecordGetter>(FormKey.Null));
        source.FormLists.Add(sourceList);
        WritePlugin(source, sourcePluginPath);

        var output = new StarfieldMod("PresentationExistingOutput.esm", StarfieldRelease.Starfield)
        {
            IsMaster = true,
        };
        var outputList = new StarfieldFormList(output, "PresentationOutputList");
        output.FormLists.Add(outputList);
        WritePlugin(output, existingOutputPath);
        return new FixtureRecords(sourceList.FormKey, outputList.FormKey);
    }

    /// <summary>Writes a minimal Fallout 4 source and distinct existing output.</summary>
    /// <param name="sourcePluginPath">The source plugin destination.</param>
    /// <param name="existingOutputPath">The existing output plugin destination.</param>
    /// <returns>The native source and output FormList identities.</returns>
    private static FixtureRecords WriteFallout4Plugins(string sourcePluginPath, string existingOutputPath)
    {
        var source = new Fallout4Mod("PresentationSource.esm", Fallout4Release.Fallout4)
        {
            IsMaster = true,
        };
        var sourceList = new Fallout4FormList(source, "PresentationSourceList");
        sourceList.Items.Add(new FormLink<Fallout4MajorRecordGetter>(sourceList.FormKey));
        sourceList.Items.Add(new FormLink<Fallout4MajorRecordGetter>(sourceList.FormKey));
        sourceList.Items.Add(new FormLink<Fallout4MajorRecordGetter>(FormKey.Null));
        source.FormLists.Add(sourceList);
        WritePlugin(source, sourcePluginPath);

        var output = new Fallout4Mod("PresentationExistingOutput.esm", Fallout4Release.Fallout4)
        {
            IsMaster = true,
        };
        var outputList = new Fallout4FormList(output, "PresentationOutputList");
        output.FormLists.Add(outputList);
        WritePlugin(output, existingOutputPath);
        return new FixtureRecords(sourceList.FormKey, outputList.FormKey);
    }

    /// <summary>Writes a minimal Skyrim Special Edition source and distinct existing output.</summary>
    /// <param name="sourcePluginPath">The source plugin destination.</param>
    /// <param name="existingOutputPath">The existing output plugin destination.</param>
    /// <returns>The native source and output FormList identities.</returns>
    private static FixtureRecords WriteSkyrimPlugins(string sourcePluginPath, string existingOutputPath)
    {
        var source = new SkyrimMod("PresentationSource.esm", SkyrimRelease.SkyrimSE)
        {
            IsMaster = true,
        };
        var sourceList = new SkyrimFormList(source, "PresentationSourceList");
        sourceList.Items.Add(new FormLink<SkyrimMajorRecordGetter>(sourceList.FormKey));
        sourceList.Items.Add(new FormLink<SkyrimMajorRecordGetter>(sourceList.FormKey));
        sourceList.Items.Add(new FormLink<SkyrimMajorRecordGetter>(FormKey.Null));
        source.FormLists.Add(sourceList);
        WritePlugin(source, sourcePluginPath);

        var output = new SkyrimMod("PresentationExistingOutput.esm", SkyrimRelease.SkyrimSE)
        {
            IsMaster = true,
        };
        var outputList = new SkyrimFormList(output, "PresentationOutputList");
        output.FormLists.Add(outputList);
        WritePlugin(output, existingOutputPath);
        return new FixtureRecords(sourceList.FormKey, outputList.FormKey);
    }

    /// <summary>Writes one complete native plugin using Mutagen's ordinary binary writer.</summary>
    /// <param name="plugin">The native plugin to serialize.</param>
    /// <param name="pluginPath">The destination path.</param>
    private static void WritePlugin(IModGetter plugin, string pluginPath)
    {
        plugin.WriteToBinary(pluginPath, BinaryWriteParameters.Default);
    }

    /// <summary>Creates a full-master embedded output association for one fixture path.</summary>
    /// <param name="pluginPath">The existing or absent output plugin path.</param>
    /// <returns>The complete native output association.</returns>
    private static OutputAssociation CreateAssociation(string pluginPath)
    {
        return new OutputAssociation(
            pluginPath,
            ModKey.FromNameAndExtension(Path.GetFileName(pluginPath)),
            LocalizedOutputMode.Embedded,
            OutputMasterStyle.Full);
    }

    /// <summary>Deletes a verified fixture-owned temporary directory recursively.</summary>
    /// <param name="directory">The fixture root to delete.</param>
    /// <exception cref="InvalidOperationException">Thrown when the supplied directory is not a recognized fixture root.</exception>
    private static void DeleteOwnedDirectory(DirectoryInfo directory)
    {
        var rootPath = Path.GetFullPath(directory.FullName);
        var temporaryPath = Path.GetFullPath(Path.GetTempPath());
        if (!rootPath.StartsWith(temporaryPath, StringComparison.OrdinalIgnoreCase) ||
            !directory.Name.StartsWith("CreationsForge-NativeDesktopWorkspace-", StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"Refusing to delete an unrecognized native desktop fixture root: {rootPath}");
        }

        directory.Refresh();
        if (directory.Exists)
        {
            directory.Delete(recursive: true);
        }
    }

    /// <summary>Stores the generated native record identities shared by game-specific fixture writers.</summary>
    private sealed class FixtureRecords
    {
        /// <summary>Initializes a pair of generated native FormList identities.</summary>
        /// <param name="sourceFormKey">The source FormList identity.</param>
        /// <param name="outputFormKey">The existing output FormList identity.</param>
        public FixtureRecords(FormKey sourceFormKey, FormKey outputFormKey)
        {
            SourceFormKey = sourceFormKey;
            OutputFormKey = outputFormKey;
        }

        /// <summary>Gets the source FormList identity.</summary>
        public FormKey SourceFormKey { get; }

        /// <summary>Gets the existing output FormList identity.</summary>
        public FormKey OutputFormKey { get; }
    }
}
