using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Enums;
using CreationsForge.UnitTests.Engine.Fallout4;
using CreationsForge.UnitTests.Engine.Skyrim;
using CreationsForge.UnitTests.Engine.Starfield;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Starfield;
using Noggog;

namespace CreationsForge.UnitTests.Engine.Integration;

/// <summary>Provides a uniform owner for the existing generated native fixtures used by three-game workspace integration tests.</summary>
internal sealed class NativeWorkspaceIntegrationFixture : IDisposable
{
    /// <summary>The game-specific fixture lifetime that owns all generated source artifacts.</summary>
    private readonly IDisposable _fixture;

    /// <summary>Creates explicit source-open requests with caller-selected or fresh workspace identities.</summary>
    private readonly Func<Guid?, WorkspaceOpenRequest> _createOpenRequest;

    /// <summary>Creates a uniform view over one game-specific native fixture.</summary>
    /// <param name="fixture">The game-specific fixture lifetime.</param>
    /// <param name="game">The engine game represented by the fixture.</param>
    /// <param name="rootDirectory">The task-owned temporary root directory.</param>
    /// <param name="sourceModKey">The selected source plugin identity.</param>
    /// <param name="sourceListFormKey">The source FormList identity available for override edits.</param>
    /// <param name="bookFormKey">A valid source Book identity for FormList item edits.</param>
    /// <param name="keywordFormKey">A valid source Keyword identity for nested or direct reference edits.</param>
    /// <param name="createOpenRequest">Creates an explicit workspace-open request with a caller-selected or fresh workspace identity.</param>
    /// <param name="snapshotArtifacts">Captures every source plugin and localized-string artifact.</param>
    /// <param name="readEmbeddedOutputMasters">Reads the ordered native master list from a committed embedded output.</param>
    private NativeWorkspaceIntegrationFixture(
        IDisposable fixture,
        SupportedGame game,
        DirectoryInfo rootDirectory,
        ModKey sourceModKey,
        FormKey sourceListFormKey,
        FormKey bookFormKey,
        FormKey keywordFormKey,
        Func<Guid?, WorkspaceOpenRequest> createOpenRequest,
        Func<IReadOnlyDictionary<string, byte[]>> snapshotArtifacts,
        Func<string, IReadOnlyList<ModKey>> readEmbeddedOutputMasters)
    {
        _fixture = fixture;
        Game = game;
        RootDirectory = rootDirectory;
        SourceModKey = sourceModKey;
        SourceListFormKey = sourceListFormKey;
        BookFormKey = bookFormKey;
        KeywordFormKey = keywordFormKey;
        _createOpenRequest = createOpenRequest;
        SnapshotArtifacts = snapshotArtifacts;
        ReadEmbeddedOutputMasters = readEmbeddedOutputMasters;
    }

    /// <summary>Gets the engine game represented by the generated fixture.</summary>
    public SupportedGame Game { get; }

    /// <summary>Gets the task-owned temporary root containing source and output directories.</summary>
    public DirectoryInfo RootDirectory { get; }

    /// <summary>Gets the selected source plugin identity whose FormList can be overridden.</summary>
    public ModKey SourceModKey { get; }

    /// <summary>Gets the source FormList identity used by the cross-game override matrix.</summary>
    public FormKey SourceListFormKey { get; }

    /// <summary>Gets a valid source Book identity that every game can store in a FormList.</summary>
    public FormKey BookFormKey { get; }

    /// <summary>Gets a valid source Keyword identity used by Starfield nested component coverage.</summary>
    public FormKey KeywordFormKey { get; }

    /// <summary>Creates an explicit source-open request with a fresh workspace identity.</summary>
    /// <returns>The complete generated native source request.</returns>
    public WorkspaceOpenRequest CreateOpenRequest()
    {
        return _createOpenRequest(null);
    }

    /// <summary>Creates an explicit source-open request with the caller-selected workspace identity.</summary>
    /// <param name="workspaceId">The non-empty identity assigned to the opened workspace.</param>
    /// <returns>The complete generated native source request.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="workspaceId"/> is empty.</exception>
    public WorkspaceOpenRequest CreateOpenRequest(Guid workspaceId)
    {
        if (workspaceId == Guid.Empty)
        {
            throw new ArgumentException("A native integration workspace requires a non-empty identity.", nameof(workspaceId));
        }

        return _createOpenRequest(workspaceId);
    }

    /// <summary>Gets the delegate that captures every exact source artifact byte sequence.</summary>
    public Func<IReadOnlyDictionary<string, byte[]>> SnapshotArtifacts { get; }

    /// <summary>Gets the game-specific reader for the ordered native masters in a committed embedded output.</summary>
    public Func<string, IReadOnlyList<ModKey>> ReadEmbeddedOutputMasters { get; }

    /// <summary>Creates the existing generated native fixture for the requested game.</summary>
    /// <param name="game">The supported engine game.</param>
    /// <returns>A uniform fixture owner over the requested game's generated native sources.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="game"/> is undefined.</exception>
    public static NativeWorkspaceIntegrationFixture Create(SupportedGame game)
    {
        return game switch
        {
            SupportedGame.Starfield => CreateStarfield(),
            SupportedGame.Fallout4 => CreateFallout4(),
            SupportedGame.Skyrim => CreateSkyrim(),
            _ => throw new ArgumentOutOfRangeException(nameof(game), game, "The integration fixture requires a supported native game."),
        };
    }

    /// <summary>Releases the generated native fixture and deletes its task-owned temporary files.</summary>
    public void Dispose()
    {
        _fixture.Dispose();
    }

    /// <summary>Creates a uniform Starfield native fixture owner.</summary>
    /// <returns>A Starfield integration fixture.</returns>
    private static NativeWorkspaceIntegrationFixture CreateStarfield()
    {
        var fixture = StarfieldNativeTestFixture.Create();
        return new NativeWorkspaceIntegrationFixture(
            fixture,
            SupportedGame.Starfield,
            fixture.RootDirectory,
            fixture.SourceModKey,
            fixture.SourceListFormKey,
            fixture.BookFormKey,
            fixture.KeywordFormKey,
            workspaceId => fixture.CreateOpenRequest(workspaceId),
            fixture.SnapshotArtifacts,
            path => ReadStarfieldOutputMasters(path, fixture.SourceModKey));
    }

    /// <summary>Reads only the Starfield plugin header with the fixture's exact source and output master styles.</summary>
    /// <param name="path">The committed output plugin path.</param>
    /// <param name="sourceModKey">The fixture-owned small-master source identity.</param>
    /// <returns>The ordered native master list embedded in the output header.</returns>
    private static IReadOnlyList<ModKey> ReadStarfieldOutputMasters(string path, ModKey sourceModKey)
    {
        var outputModKey = ModKey.FromNameAndExtension(Path.GetFileName(path));
        var masterFlags = new Cache<IModMasterStyledGetter, ModKey>(metadata => metadata.ModKey);
        masterFlags.Set(new KeyedMasterStyle(sourceModKey, MasterStyle.Small));
        masterFlags.Set(new KeyedMasterStyle(outputModKey, MasterStyle.Full));
        var parameters = new BinaryReadParameters
        {
            MasterFlagsLookup = masterFlags,
            ThrowOnUnknownSubrecord = true,
        };
        var metadata = ParsingMeta.Factory(
            parameters,
            GameRelease.Starfield,
            new ModPath(outputModKey, path));
        using var fileStream = File.OpenRead(path);
        using var binaryStream = new MutagenBinaryReadStream(
            fileStream,
            metadata,
            bufferSize: 4096,
            dispose: true,
            offsetReference: 0);
        var mod = StarfieldMod.CreateFromBinary(
            new MutagenFrame(binaryStream),
            StarfieldRelease.Starfield,
            new Mutagen.Bethesda.Starfield.GroupMask(false));
        return mod.ModHeader.MasterReferences.Select(reference => reference.Master).ToArray();
    }

    /// <summary>Creates a uniform Fallout 4 native fixture owner.</summary>
    /// <returns>A Fallout 4 integration fixture.</returns>
    private static NativeWorkspaceIntegrationFixture CreateFallout4()
    {
        var fixture = Fallout4NativeTestFixture.Create();
        return new NativeWorkspaceIntegrationFixture(
            fixture,
            SupportedGame.Fallout4,
            fixture.RootDirectory,
            fixture.SourceModKey,
            fixture.SourceListFormKey,
            fixture.BookFormKey,
            fixture.KeywordFormKey,
            workspaceId => fixture.CreateOpenRequest(workspaceId),
            fixture.SnapshotArtifacts,
            path => Fallout4Mod.CreateFromBinary(path, Fallout4Release.Fallout4)
                .ModHeader.MasterReferences.Select(reference => reference.Master).ToArray());
    }

    /// <summary>Creates a uniform Skyrim Special Edition native fixture owner.</summary>
    /// <returns>A Skyrim Special Edition integration fixture.</returns>
    private static NativeWorkspaceIntegrationFixture CreateSkyrim()
    {
        var fixture = SkyrimNativeTestFixture.Create();
        return new NativeWorkspaceIntegrationFixture(
            fixture,
            SupportedGame.Skyrim,
            fixture.RootDirectory,
            fixture.SourceModKey,
            fixture.SourceListFormKey,
            fixture.BookFormKey,
            fixture.KeywordFormKey,
            workspaceId => fixture.CreateOpenRequest(workspaceId),
            fixture.SnapshotArtifacts,
            path => SkyrimMod.CreateFromBinary(path, SkyrimRelease.SkyrimSE)
                .ModHeader.MasterReferences.Select(reference => reference.Master).ToArray());
    }
}
