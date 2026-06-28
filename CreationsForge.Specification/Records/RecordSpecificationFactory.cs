namespace CreationsForge.Specification.Records;

/// <summary>
/// Provides shared factory helpers used by individual record specification definitions.
/// </summary>
internal static class RecordSpecificationFactory
{
    /// <summary>
    /// Creates a specification for a record family whose import and reader metadata are active while comparison
    /// metadata remains owned by record-specific Core code.
    /// </summary>
    /// <param name="recordID">The Bethesda record identifier used to resolve the typed detail importer.</param>
    /// <param name="recordType">The canonical CreationsForge record type name.</param>
    /// <param name="tableName">The current typed detail table name used in import results.</param>
    /// <param name="friendlyName">The human-readable record family name used for diagnostics and display.</param>
    /// <param name="pluginRecordSetPropertyName">The <c>PluginRecordSetDTO</c> collection property containing DTOs.</param>
    /// <param name="importOrder">The import order that preserves the existing record-dispatch sequence.</param>
    /// <param name="isRequired">A value indicating whether an import result should be emitted for empty unsupported families.</param>
    /// <param name="gamesRequiringFullBinaryMod">
    /// The supported games that must read this record family through a full binary Mutagen mod.
    /// </param>
    /// <param name="usesOverlaySafeMod">
    /// A value indicating whether the normal overlay-safe reader path is valid for games without a full-binary
    /// override.
    /// </param>
    /// <param name="isOptionalCollection">
    /// A value indicating whether a missing reader collection is an expected adapter capability gap.
    /// </param>
    /// <param name="gameSupport">Optional explicit game-support metadata for record families with limited support.</param>
    /// <returns>The import-only record specification used by transitional record families.</returns>
    internal static RecordSpecification CreateImportOnlySpecification(
        string recordID,
        string recordType,
        string tableName,
        string friendlyName,
        string pluginRecordSetPropertyName,
        int importOrder,
        bool isRequired,
        IReadOnlySet<SpecificationGame>? gamesRequiringFullBinaryMod = null,
        bool usesOverlaySafeMod = true,
        bool isOptionalCollection = false,
        IReadOnlyList<RecordGameSupportSpecification>? gameSupport = null)
    {
        return new RecordSpecification
        {
            RecordID = recordID,
            RecordType = recordType,
            TableName = tableName,
            FriendlyName = friendlyName,
            GameSupport = gameSupport ?? CreateCurrentGameSupport(pluginRecordSetPropertyName, pluginRecordSetPropertyName),
            Import = new RecordImportSpecification
            {
                PluginRecordSetPropertyName = pluginRecordSetPropertyName,
                ImportOrder = importOrder,
                IsRequired = isRequired
            },
            Reader = CreateReaderSpecification(
                pluginRecordSetPropertyName,
                pluginRecordSetPropertyName,
                gamesRequiringFullBinaryMod,
                usesOverlaySafeMod,
                isOptionalCollection),
            ImplementationNote = "Import dispatch metadata is active; comparison remains record-specific."
        };
    }

    /// <summary>
    /// Creates reader metadata for the current game-adapter record mapping path.
    /// </summary>
    /// <param name="pluginRecordSetPropertyName">The <c>PluginRecordSetDTO</c> collection property that receives mapped DTOs.</param>
    /// <param name="defaultMutagenCollectionName">The default Mutagen mod collection property read by game adapters.</param>
    /// <param name="gamesRequiringFullBinaryMod">
    /// The supported games that must read this record family through a full binary Mutagen mod.
    /// </param>
    /// <param name="usesOverlaySafeMod">
    /// A value indicating whether the normal overlay-safe reader path is valid for games without a full-binary
    /// override.
    /// </param>
    /// <param name="isOptionalCollection">
    /// A value indicating whether a missing reader collection is an expected adapter capability gap.
    /// </param>
    /// <returns>The reader metadata used as the next specification-driven reader migration target.</returns>
    internal static RecordReaderSpecification CreateReaderSpecification(
        string pluginRecordSetPropertyName,
        string defaultMutagenCollectionName,
        IReadOnlySet<SpecificationGame>? gamesRequiringFullBinaryMod = null,
        bool usesOverlaySafeMod = true,
        bool isOptionalCollection = false)
    {
        return new RecordReaderSpecification
        {
            PluginRecordSetPropertyName = pluginRecordSetPropertyName,
            DefaultMutagenCollectionName = defaultMutagenCollectionName,
            GamesRequiringFullBinaryMod = gamesRequiringFullBinaryMod ?? new HashSet<SpecificationGame>(),
            UsesOverlaySafeMod = usesOverlaySafeMod,
            IsOptionalCollection = isOptionalCollection,
            UsesGameSpecificMapper = true
        };
    }

    /// <summary>
    /// Creates support metadata for the currently implemented CreationsForge game adapters.
    /// </summary>
    /// <param name="mutagenCollectionName">The Mutagen collection property name shared by the current adapters.</param>
    /// <param name="spriggitRecordDirectoryName">The Spriggit record-family directory name used by validation.</param>
    /// <returns>The current Starfield, Fallout 4, and Skyrim support metadata.</returns>
    internal static IReadOnlyList<RecordGameSupportSpecification> CreateCurrentGameSupport(
        string mutagenCollectionName,
        string spriggitRecordDirectoryName)
    {
        return CreateGameSupport(
            mutagenCollectionName,
            spriggitRecordDirectoryName,
            SpecificationGame.Starfield,
            SpecificationGame.Fallout4,
            SpecificationGame.Skyrim);
    }

    /// <summary>
    /// Creates support metadata for the specified game adapters.
    /// </summary>
    /// <param name="mutagenCollectionName">The Mutagen collection property name exposed by the selected adapters.</param>
    /// <param name="spriggitRecordDirectoryName">The Spriggit record-family directory name used by validation.</param>
    /// <param name="games">The supported games that expose the record family through current adapters.</param>
    /// <returns>The requested game support metadata.</returns>
    internal static IReadOnlyList<RecordGameSupportSpecification> CreateGameSupport(
        string mutagenCollectionName,
        string spriggitRecordDirectoryName,
        params SpecificationGame[] games)
    {
        return
        [
            .. games.Select(game => new RecordGameSupportSpecification
            {
                Game = game,
                MutagenCollectionName = mutagenCollectionName,
                SpriggitRecordDirectoryName = spriggitRecordDirectoryName
            })
        ];
    }
}
