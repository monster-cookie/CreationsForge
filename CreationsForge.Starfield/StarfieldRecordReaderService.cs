using System.Globalization;
using System.Collections;
using System.Reflection;
using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Services;
using CreationsForge.Core.Services.Interfaces;
using CreationsForge.Core.Utilities;
using CreationsForge.Starfield.Interfaces;
using CreationsForge.Specification.Records;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;
using Mutagen.Bethesda.Strings;

namespace CreationsForge.Starfield;

/// <summary>
/// Reads Starfield plugin records through Mutagen and maps the currently supported record families into Core DTOs.
/// </summary>
public class StarfieldRecordReaderService : IStarfieldRecordReaderService
{
    /// <summary>
    /// Maps Starfield-supported record identifiers to the existing Mutagen-to-DTO mapping functions.
    /// </summary>
    private static readonly IReadOnlyDictionary<string, Func<PluginDTO, IStarfieldModGetter, object>> RecordMappers =
        new Dictionary<string, Func<PluginDTO, IStarfieldModGetter, object>>(StringComparer.OrdinalIgnoreCase)
        {
            [RecordTypeCatalog.FormList.RecordID] = (plugin, mod) => MapFormLists(plugin, mod),
            [RecordTypeCatalog.GameSetting.RecordID] = (plugin, mod) => MapGameSettings(plugin, mod),
            [RecordTypeCatalog.Global.RecordID] = (plugin, mod) => MapGlobals(plugin, mod),
            [RecordTypeCatalog.Class.RecordID] = (plugin, mod) => MapClasses(plugin, mod),
            [RecordTypeCatalog.Faction.RecordID] = (plugin, mod) => MapFactions(plugin, mod),
            [RecordTypeCatalog.MiscItem.RecordID] = (plugin, mod) => MapMiscItems(plugin, mod),
            [RecordTypeCatalog.Keyword.RecordID] = (plugin, mod) => MapKeywords(plugin, mod),
            [RecordTypeCatalog.ActorValueInformation.RecordID] = (plugin, mod) => MapActorValueInformation(plugin, mod),
            [RecordTypeCatalog.NPC.RecordID] = (plugin, mod) => MapNPCs(plugin, mod),
            [RecordTypeCatalog.MagicEffect.RecordID] = (plugin, mod) => MapMagicEffects(plugin, mod),
            [RecordTypeCatalog.Perk.RecordID] = (plugin, mod) => MapPerks(plugin, mod),
            [RecordTypeCatalog.Static.RecordID] = (plugin, mod) => MapStaticModelRecords(plugin, mod),
            [RecordTypeCatalog.Book.RecordID] = (plugin, mod) => MapBooks(plugin, mod),
            [RecordTypeCatalog.Door.RecordID] = (plugin, mod) => MapDoors(plugin, mod),
            [RecordTypeCatalog.Container.RecordID] = (plugin, mod) => MapContainers(plugin, mod),
            [RecordTypeCatalog.ConditionForm.RecordID] = (plugin, mod) => MapConditionForms(plugin, mod),
            [RecordTypeCatalog.ConstructibleObject.RecordID] = (plugin, mod) => MapConstructibleObjects(plugin, mod),
            [RecordTypeCatalog.Terminal.RecordID] = (plugin, mod) => MapTerminals(plugin, mod)
        };

    /// <summary>
    /// Provides Starfield installation metadata used to locate plugin files and game data.
    /// </summary>
    private readonly StarfieldGameMetadataService GameMetadataService;

    /// <summary>
    /// Builds the final plugin record set from mapped record-family collections and specification reader metadata.
    /// </summary>
    private readonly IRecordSetSpecificationBuilder RecordSetBuilder;

    /// <summary>
    /// Provides the record specifications that drive Starfield record-family dispatch.
    /// </summary>
    private readonly IRecordSpecificationProvider RecordSpecificationProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="StarfieldRecordReaderService"/> class.
    /// </summary>
    /// <param name="gameMetadataService">The service used to discover Starfield installation metadata.</param>
    public StarfieldRecordReaderService(StarfieldGameMetadataService gameMetadataService)
        : this(gameMetadataService, RecordSetSpecificationBuilder.CreateDefault(), new RecordSpecificationProvider())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StarfieldRecordReaderService"/> class.
    /// </summary>
    /// <param name="gameMetadataService">The service used to discover Starfield installation metadata.</param>
    /// <param name="recordSetSpecificationBuilder">
    /// The builder used to assemble mapped record collections into a record set.
    /// </param>
    public StarfieldRecordReaderService(
        StarfieldGameMetadataService gameMetadataService,
        IRecordSetSpecificationBuilder recordSetSpecificationBuilder)
        : this(gameMetadataService, recordSetSpecificationBuilder, new RecordSpecificationProvider())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StarfieldRecordReaderService"/> class.
    /// </summary>
    /// <param name="gameMetadataService">The service used to discover Starfield installation metadata.</param>
    /// <param name="recordSetSpecificationBuilder">
    /// The builder used to assemble mapped record collections into a record set.
    /// </param>
    /// <param name="recordSpecificationProvider">The provider used to select Starfield-supported record mappers.</param>
    public StarfieldRecordReaderService(
        StarfieldGameMetadataService gameMetadataService,
        IRecordSetSpecificationBuilder recordSetSpecificationBuilder,
        IRecordSpecificationProvider recordSpecificationProvider)
    {
        GameMetadataService = gameMetadataService;
        RecordSetBuilder = recordSetSpecificationBuilder;
        RecordSpecificationProvider = recordSpecificationProvider;
    }

    /// <summary>
    /// Reads all currently supported Starfield record families from a plugin and returns them in a Core record set.
    /// </summary>
    /// <param name="plugin">The plugin metadata that identifies the Starfield plugin file to read.</param>
    /// <param name="cancellationToken">A token used to stop the read between record-family mapping steps.</param>
    /// <returns>The mapped Starfield record families assembled according to record specification metadata.</returns>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public PluginRecordSetDTO ReadPluginRecords(PluginDTO plugin, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var mod = LoadMod(plugin);
        var recordsByRecordID = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        var specifications = RecordSpecificationProvider
            .GetSupportedByGame(SpecificationGame.Starfield)
            .OrderBy(specification => specification.Import.ImportOrder);

        foreach (var specification in specifications)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!RecordMappers.TryGetValue(specification.RecordID, out var mapper))
            {
                throw new InvalidOperationException(
                    $"No Starfield record mapper is registered for specification '{specification.RecordID}'.");
            }

            if (specification.Reader.RequiresFullBinaryModForGame(SpecificationGame.Starfield))
            {
                throw new InvalidOperationException(
                    $"Starfield record specification '{specification.RecordID}' requires a full binary mod reader, " +
                    "but the Starfield reader does not provide a full-binary dispatch path.");
            }

            if (!specification.Reader.UsesOverlaySafeMod)
            {
                throw new InvalidOperationException(
                    $"Starfield record specification '{specification.RecordID}' does not allow the overlay-safe " +
                    "reader path.");
            }

            recordsByRecordID[specification.RecordID] = mapper(plugin, mod);
        }

        return RecordSetBuilder.Build(
            SupportedGame.Starfield,
            recordsByRecordID);
    }

    public IReadOnlyList<FormListDTO> ReadFormLists(PluginDTO plugin)
    {
        var mod = LoadMod(plugin);
        return MapFormLists(plugin, mod);
    }

    private static IReadOnlyList<FormListDTO> MapFormLists(PluginDTO plugin, IStarfieldModGetter mod)
    {
        return mod.FormLists
            .Select(record => LocalizedStringDTOMapper.AddLocalizedStrings(new FormListDTO
            {
                Game = SupportedGame.Starfield,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(record.FormKey),
                EditorID = record.EditorID ?? string.Empty,
                FormVersion = record.FormVersion,
                MajorRecordFlags = (int)record.StarfieldMajorRecordFlags,
                Version2 = GetPropertyNullableInt(record, "Version2"),
                VersionControl = GetPropertyNullableInt(record, "VersionControl"),
                Name = LocalizedStringDTOMapper.ToTranslatedStringDTO(GetPropertyValue(record, "Name")),
                ImportedAtUTC = DateTime.UtcNow,
                AddToList = GetFormKeyFromObject(GetPropertyValue(record, "AddToList")),
                Items = record.Items.Select((item, itemIndex) => new FormListItemDTO
                {
                    Game = SupportedGame.Starfield,
                    ModKey = plugin.ModKey,
                    FormKey = MapFormKey(record.FormKey),
                    Item = MapFormKey(item.FormKey),
                    ItemIndex = itemIndex,
                    ImportedAtUTC = DateTime.UtcNow
                }).ToList()
            }, record))
            .ToList();
    }

    public IReadOnlyList<GameSettingDTO> ReadGameSettings(PluginDTO plugin)
    {
        var mod = LoadMod(plugin);
        return MapGameSettings(plugin, mod);
    }

    private static IReadOnlyList<GameSettingDTO> MapGameSettings(PluginDTO plugin, IStarfieldModGetter mod)
    {
        return mod.GameSettings
            .Select(record => LocalizedStringDTOMapper.AddLocalizedStrings(new GameSettingDTO
            {
                Game = SupportedGame.Starfield,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(record.FormKey),
                EditorID = record.EditorID ?? string.Empty,
                FormVersion = record.FormVersion,
                MajorRecordFlags = (int)record.StarfieldMajorRecordFlags,
                Version2 = GetPropertyNullableInt(record, "Version2"),
                VersionControl = GetPropertyNullableInt(record, "VersionControl"),
                ImportedAtUTC = DateTime.UtcNow,
                DataType = GetGameSettingDataType(record),
                Data = GetGameSettingData(record)
            }, record))
            .ToList();
    }

    public IReadOnlyList<GlobalDTO> ReadGlobals(PluginDTO plugin)
    {
        var mod = LoadMod(plugin);
        return MapGlobals(plugin, mod);
    }

    private static IReadOnlyList<GlobalDTO> MapGlobals(PluginDTO plugin, IStarfieldModGetter mod)
    {
        return mod.Globals
            .Select(record => new GlobalDTO
            {
                Game = SupportedGame.Starfield,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(record.FormKey),
                EditorID = record.EditorID ?? string.Empty,
                FormVersion = record.FormVersion,
                MajorRecordFlags = (int)record.StarfieldMajorRecordFlags,
                Version2 = GetPropertyNullableInt(record, "Version2"),
                VersionControl = GetPropertyNullableInt(record, "VersionControl"),
                MajorFlags = FormatEnumerable(GetPropertyValue(record, "MajorFlags")),
                ImportedAtUTC = DateTime.UtcNow,
                Data = record.Data,
                ScriptingAdapters = GetScriptingAdapters(plugin, RecordTypeCatalog.Global.RecordID, record)
            })
            .ToList();
    }

    private static IReadOnlyList<KeywordDTO> MapKeywords(PluginDTO plugin, IStarfieldModGetter mod)
    {
        return mod.Keywords
            .Select(record => LocalizedStringDTOMapper.AddLocalizedStrings(new KeywordDTO
            {
                Game = SupportedGame.Starfield,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(record.FormKey),
                EditorID = record.EditorID ?? string.Empty,
                FormVersion = record.FormVersion,
                MajorRecordFlags = (int)record.StarfieldMajorRecordFlags,
                Version2 = GetPropertyNullableInt(record, "Version2"),
                VersionControl = GetPropertyNullableInt(record, "VersionControl"),
                ImportedAtUTC = DateTime.UtcNow,
                Name = GetTranslatedString(record.Name),
                Color = FormatSpriggitColor(record.Color),
                Type = record.Type.ToString() ?? string.Empty,
                Notes = record.Notes,
                FlashLinkageName = record.FlashLinkageName,
                FNAM = FormatSpriggitHexValue(GetPropertyValue(record, "FNAM")),
                WAIM = GetFirstComponentHexValue(record, "WAIM"),
                WFIR = GetFirstComponentHexValue(record, "WFIR"),
                AttractionRule = record.AttractionRule.IsNull ? null : MapFormKey(record.AttractionRule.FormKey),
                ScriptingAdapters = GetScriptingAdapters(plugin, RecordTypeCatalog.Keyword.RecordID, record)
            }, record))
            .ToList();
    }

    private static IReadOnlyList<ClassDTO> MapClasses(PluginDTO plugin, IStarfieldModGetter mod)
    {
        return GetRecordCollection(mod, "Classes")
            .Select(record => CreateClass(plugin, SupportedGame.Starfield, record, "StarfieldMajorRecordFlags"))
            .ToList();
    }

    private static IReadOnlyList<FactionDTO> MapFactions(PluginDTO plugin, IStarfieldModGetter mod)
    {
        return GetRecordCollection(mod, "Factions")
            .Select(record => CreateFaction(plugin, SupportedGame.Starfield, record, "StarfieldMajorRecordFlags"))
            .ToList();
    }

    private static IReadOnlyList<MiscItemDTO> MapMiscItems(PluginDTO plugin, IStarfieldModGetter mod)
    {
        return mod.MiscItems
            .Select(record => LocalizedStringDTOMapper.AddLocalizedStrings(new MiscItemDTO
            {
                Game = SupportedGame.Starfield,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(record.FormKey),
                EditorID = record.EditorID ?? string.Empty,
                FormVersion = record.FormVersion,
                MajorRecordFlags = (int)record.StarfieldMajorRecordFlags,
                Version2 = GetPropertyNullableInt(record, "Version2"),
                VersionControl = GetPropertyNullableInt(record, "VersionControl"),
                ImportedAtUTC = DateTime.UtcNow,
                ObjectBounds = new ObjectBoundsDTO
                {
                    First = FormatObjectBoundsPoint(GetPropertyValue(record, "ObjectBounds"), "First"),
                    Second = FormatObjectBoundsPoint(GetPropertyValue(record, "ObjectBounds"), "Second")
                },
                Transforms = new BookTransformsDTO
                {
                    Inventory = GetFormKeyFromObject(GetPropertyValue(GetPropertyValue(record, "Transforms"), "Inventory"))
                },
                Name = GetTranslatedString(record.Name),
                ShortName = GetTranslatedString(record.ShortName),
                Value = record.Value,
                Weight = record.Weight,
                DirtinessScale = (float)record.DirtinessScale,
                FeaturedItemMessage = record.FeaturedItemMessage.IsNull ? null : MapFormKey(record.FeaturedItemMessage.FormKey),
                Flag = record.FLAG == null ? null : Convert.ToHexString(record.FLAG.Value.ToArray()),
                Models = GetModels(plugin, RecordTypeCatalog.MiscItem.RecordID, record.FormKey, record.Model),
                Keywords = GetKeywordMappings(plugin, RecordTypeCatalog.MiscItem.RecordID, record.FormKey, record.Keywords),
                Sounds = GetNamedSounds(plugin, RecordTypeCatalog.MiscItem.RecordID, record.FormKey, record, "CraftingSound", "PickupSound", "DropdownSound"),
                ScriptingAdapters = GetScriptingAdapters(plugin, RecordTypeCatalog.MiscItem.RecordID, record),
                Resources = GetMiscItemResources(plugin, record.FormKey, record)
            }, record))
            .ToList();
    }

    private static IReadOnlyList<ActorValueInformationDTO> MapActorValueInformation(PluginDTO plugin, IStarfieldModGetter mod)
    {
        return mod.ActorValueInformation
            .Select(record => LocalizedStringDTOMapper.AddLocalizedStrings(new ActorValueInformationDTO
            {
                Game = SupportedGame.Starfield,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(record.FormKey),
                EditorID = record.EditorID ?? string.Empty,
                FormVersion = record.FormVersion,
                MajorRecordFlags = (int)record.StarfieldMajorRecordFlags,
                Version2 = GetPropertyNullableInt(record, "Version2"),
                VersionControl = GetPropertyNullableInt(record, "VersionControl"),
                ImportedAtUTC = DateTime.UtcNow,
                Name = GetTranslatedString(record.Name),
                Abbreviation = GetTranslatedString(record.Abbreviation),
                ContextNotes = record.ContextNotes,
                DefaultValue = record.DefaultValue,
                Flags = record.Flags.ToString(),
                Type = record.Type?.ToString(),
                Min = record.Min,
                Max = record.Max,
                ScriptingAdapters = GetScriptingAdapters(plugin, RecordTypeCatalog.ActorValueInformation.RecordID, record)
            }, record))
            .ToList();
    }

    private static IReadOnlyList<StaticDTO> MapStaticModelRecords(PluginDTO plugin, IStarfieldModGetter mod)
    {
        return mod.Statics
            .Select(record => LocalizedStringDTOMapper.AddLocalizedStrings(new StaticDTO
            {
                Game = SupportedGame.Starfield,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(record.FormKey),
                EditorID = record.EditorID ?? string.Empty,
                FormVersion = record.FormVersion,
                MajorRecordFlags = (int)record.StarfieldMajorRecordFlags,
                Name = GetTranslatedString(GetPropertyValue(record, "Name")),
                Version2 = GetPropertyNullableInt(record, "Version2"),
                VersionControl = GetPropertyNullableInt(record, "VersionControl"),
                ImportedAtUTC = DateTime.UtcNow,
                ObjectBoundsFirst = FormatObjectBoundsPoint(GetPropertyValue(record, "ObjectBounds"), "First"),
                ObjectBoundsSecond = FormatObjectBoundsPoint(GetPropertyValue(record, "ObjectBounds"), "Second"),
                MaxAngle = GetPropertyNullableDouble(record, "MaxAngle"),
                UnknownDNAMFloat = GetPropertyNullableDouble(record, "UnknownDNAMFloat"),
                DNAMDataTypeState = FormatEnumerable(GetPropertyValue(record, "DNAMDataTypeState")),
                DirtinessScale = GetPropertyNullableDouble(record, "DirtinessScale"),
                SnapTemplate = GetLinkedFormKey(record, "SnapTemplate"),
                PreviewTransform = GetLinkedFormKey(record, "PreviewTransform"),
                Material = GetLinkedFormKey(record, "Material"),
                LodLevel0 = GetPropertyStringOrNull(GetPropertyValue(record, "Lod"), "Level0"),
                LodLevel1 = GetPropertyStringOrNull(GetPropertyValue(record, "Lod"), "Level1"),
                LodLevel2 = GetPropertyStringOrNull(GetPropertyValue(record, "Lod"), "Level2"),
                LodLevel3 = GetPropertyStringOrNull(GetPropertyValue(record, "Lod"), "Level3"),
                Models = GetModels(plugin, RecordTypeCatalog.Static.RecordID, record.FormKey, record.Model),
                Keywords = GetKeywordMappingsFromNestedKeywordLists(plugin, RecordTypeCatalog.Static.RecordID, record.FormKey, GetPropertyValue(record, "Components")),
                Reflections = GetComponentReflections(plugin, RecordTypeCatalog.Static.RecordID, record.FormKey, GetPropertyValue(record, "Components")),
                NavmeshGeometry = StaticNavmeshGeometryDTOMapper.FromNavmeshGeometry(SupportedGame.Starfield, plugin.ModKey, record.FormKey, GetPropertyValue(record, "NavmeshGeometry"), DateTime.UtcNow)
            }, record))
            .ToList();
    }

    private static IReadOnlyList<BookDTO> MapBooks(PluginDTO plugin, IStarfieldModGetter mod)
    {
        return mod.Books
            .Select(record => LocalizedStringDTOMapper.AddLocalizedStrings(new BookDTO
            {
                Game = SupportedGame.Starfield,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(record.FormKey),
                EditorID = record.EditorID ?? string.Empty,
                FormVersion = record.FormVersion,
                MajorRecordFlags = (int)record.StarfieldMajorRecordFlags,
                Version2 = GetPropertyNullableInt(record, "Version2"),
                VersionControl = GetPropertyNullableInt(record, "VersionControl"),
                ImportedAtUTC = DateTime.UtcNow,
                ObjectBounds = new ObjectBoundsDTO
                {
                    First = FormatObjectBoundsPoint(GetPropertyValue(record, "ObjectBounds"), "First"),
                    Second = FormatObjectBoundsPoint(GetPropertyValue(record, "ObjectBounds"), "Second")
                },
                Transforms = new BookTransformsDTO
                {
                    Inventory = GetFormKeyFromObject(GetPropertyValue(GetPropertyValue(record, "Transforms"), "Inventory"))
                },
                InventoryArt = GetFormKeyFromObject(GetPropertyValue(record, "InventoryArt")),
                PreviewTransform = GetFormKeyFromObject(GetPropertyValue(record, "PreviewTransform")),
                FeaturedItemMessage = GetFormKeyFromObject(GetPropertyValue(record, "FeaturedItemMessage")),
                XALG = GetPropertyNullableInt(record, "XALG"),
                Name = GetTranslatedString(record.Name),
                Text = GetTranslatedString(GetPropertyValue(record, "BookTextOverride"))
                    ?? GetTranslatedString(GetPropertyValue(record, "Text")),
                Value = GetPropertyNullableInt(record, "Value"),
                Weight = GetPropertyNullableFloat(record, "Weight"),
                Flags = FormatEnumerable(GetPropertyValue(record, "Flags")),
                Teaches = new BookTeachesDTO
                {
                    MutagenObjectType = GetPropertyValue(GetPropertyValue(record, "Teaches"), "Type")?.ToString() ?? GetPropertyValue(record, "Teaches")?.GetType().Name,
                    Perk = GetFormKeyFromObject(GetPropertyValue(GetPropertyValue(record, "Teaches"), "Perk")),
                    RawContent = FormatEnumerable(GetPropertyValue(GetPropertyValue(record, "Teaches"), "RawContent"))
                },
                DataSlateType = GetPropertyValue(record, "DataSlateType")?.ToString(),
                Description = GetTranslatedString(GetPropertyValue(record, "Description")),
                DataSlateHeaderLeft = GetTranslatedString(GetPropertyValue(record, "DataSlateHeaderLeft")),
                DataSlateHeaderRight = GetTranslatedString(GetPropertyValue(record, "DataSlateHeaderRight")),
                Models = GetModels(plugin, RecordTypeCatalog.Book.RecordID, record.FormKey, record.Model),
                Keywords = GetKeywordMappings(plugin, RecordTypeCatalog.Book.RecordID, record.FormKey, record.Keywords),
                Sounds = GetNamedSounds(plugin, RecordTypeCatalog.Book.RecordID, record.FormKey, record, "PickupSound", "DropdownSound"),
                ScriptingAdapters = GetScriptingAdapters(plugin, RecordTypeCatalog.Book.RecordID, record),
                Components = GetRecordComponents(plugin, SupportedGame.Starfield, RecordTypeCatalog.Book.RecordID, record.FormKey, GetPropertyValue(record, "Components")),
                Reflections = GetComponentReflections(plugin, RecordTypeCatalog.Book.RecordID, record.FormKey, GetPropertyValue(record, "Components"))
            }, record))
            .ToList();
    }

    private static IReadOnlyList<DoorDTO> MapDoors(PluginDTO plugin, IStarfieldModGetter mod)
    {
        return mod.Doors
            .Select(record => LocalizedStringDTOMapper.AddLocalizedStrings(new DoorDTO
            {
                Game = SupportedGame.Starfield,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(record.FormKey),
                EditorID = record.EditorID ?? string.Empty,
                FormVersion = record.FormVersion,
                MajorRecordFlags = (int)record.StarfieldMajorRecordFlags,
                Version2 = GetPropertyNullableInt(record, "Version2"),
                VersionControl = GetPropertyNullableInt(record, "VersionControl"),
                ImportedAtUTC = DateTime.UtcNow,
                ObjectBoundsFirst = FormatObjectBoundsPoint(GetPropertyValue(record, "ObjectBounds"), "First"),
                ObjectBoundsSecond = FormatObjectBoundsPoint(GetPropertyValue(record, "ObjectBounds"), "Second"),
                Name = GetTranslatedString(record.Name),
                Flags = FormatEnumerable(GetPropertyValue(record, "Flags")),
                MajorFlags = FormatEnumerable(GetPropertyValue(record, "MajorFlags")),
                NativeTerminalFormKey = record.NativeTerminal.IsNull ? null : MapFormKey(record.NativeTerminal.FormKey),
                SoundLevel = GetPropertyValue(record, "SoundLevel")?.ToString(),
                FacingAxisOverride = GetPropertyValue(record, "FacingAxisOverride")?.ToString(),
                Models = GetModels(plugin, RecordTypeCatalog.Door.RecordID, record.FormKey, record.Model),
                Keywords = GetKeywordMappings(plugin, RecordTypeCatalog.Door.RecordID, record.FormKey, record.Keywords),
                Sounds = GetNamedSounds(plugin, RecordTypeCatalog.Door.RecordID, record.FormKey, record, "OpenSound", "CloseSound"),
                ScriptingAdapters = GetScriptingAdapters(plugin, RecordTypeCatalog.Door.RecordID, record),
                Components = GetRecordComponents(plugin, SupportedGame.Starfield, RecordTypeCatalog.Door.RecordID, record.FormKey, GetPropertyValue(record, "Components")),
                AnimationGraph = GetAnimationComponentValue(GetPropertyValue(record, "Components"), "ANAM"),
                AnimationSkeleton = GetAnimationComponentValue(GetPropertyValue(record, "Components"), "BNAM"),
                AnimationDirectory = GetAnimationComponentValue(GetPropertyValue(record, "Components"), "CNAM"),
                AnimationFile = GetAnimationComponentValue(GetPropertyValue(record, "Components"), "FNAM"),
                Reflections = GetComponentReflections(plugin, RecordTypeCatalog.Door.RecordID, record.FormKey, GetPropertyValue(record, "Components")),
                ForcedLocations = GetFormKeys(GetPropertyValue(record, "ForcedLocations")),
                NavmeshGeometry = StaticNavmeshGeometryDTOMapper.FromNavmeshGeometry(SupportedGame.Starfield, plugin.ModKey, record.FormKey, GetPropertyValue(record, "NavmeshGeometry"), DateTime.UtcNow)
            }, record))
            .ToList();
    }

    private static IReadOnlyList<ContainerDTO> MapContainers(PluginDTO plugin, IStarfieldModGetter mod)
    {
        return mod.Containers
            .Select(record => LocalizedStringDTOMapper.AddLocalizedStrings(new ContainerDTO
            {
                Game = SupportedGame.Starfield,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(record.FormKey),
                EditorID = record.EditorID ?? string.Empty,
                FormVersion = record.FormVersion,
                MajorRecordFlags = (int)record.StarfieldMajorRecordFlags,
                Version2 = GetPropertyNullableInt(record, "Version2"),
                VersionControl = GetPropertyNullableInt(record, "VersionControl"),
                ImportedAtUTC = DateTime.UtcNow,
                ObjectBoundsFirst = FormatObjectBoundsPoint(GetPropertyValue(record, "ObjectBounds"), "First"),
                ObjectBoundsSecond = FormatObjectBoundsPoint(GetPropertyValue(record, "ObjectBounds"), "Second"),
                Name = GetTranslatedString(record.Name),
                Flags = FormatEnumerable(GetPropertyValue(record, "Flags")),
                MajorFlags = FormatEnumerable(GetPropertyValue(record, "MajorFlags")),
                NativeTerminalFormKey = record.NativeTerminal.IsNull ? null : MapFormKey(record.NativeTerminal.FormKey),
                SnapTemplate = GetLinkedFormKey(record, "SnapTemplate"),
                ContainsOnlyFilter = GetLinkedFormKey(record, "ContainsOnlyFilter"),
                Transforms = new ContainerTransformsDTO
                {
                    Outpost = GetFormKeyFromObject(GetPropertyValue(GetPropertyValue(record, "Transforms"), "Outpost")),
                    Preview = GetFormKeyFromObject(GetPropertyValue(GetPropertyValue(record, "Transforms"), "Preview"))
                },
                Items = GetContainerItems(plugin, record.FormKey, GetPropertyValue(record, "Items")),
                Properties = GetContainerProperties(plugin, record.FormKey, GetPropertyValue(record, "Properties")),
                ForcedLocations = GetFormKeys(GetPropertyValue(record, "ForcedLocations")),
                Models = GetModels(plugin, RecordTypeCatalog.Container.RecordID, record.FormKey, record.Model),
                Keywords = GetKeywordMappings(plugin, RecordTypeCatalog.Container.RecordID, record.FormKey, record.Keywords)
                    .Concat(GetKeywordMappingsFromNestedKeywordLists(plugin, RecordTypeCatalog.Container.RecordID, record.FormKey, GetPropertyValue(record, "Components")))
                    .Select((keyword, keywordIndex) =>
                    {
                        keyword.KeywordIndex = keywordIndex;
                        return keyword;
                    })
                    .ToList(),
                Sounds = GetNamedSounds(plugin, RecordTypeCatalog.Container.RecordID, record.FormKey, record, "OpenSound", "CloseSound"),
                ScriptingAdapters = GetScriptingAdapters(plugin, RecordTypeCatalog.Container.RecordID, record),
                Components = GetRecordComponents(plugin, SupportedGame.Starfield, RecordTypeCatalog.Container.RecordID, record.FormKey, GetPropertyValue(record, "Components")),
                AnimationGraph = GetAnimationComponentValue(GetPropertyValue(record, "Components"), "ANAM"),
                AnimationSkeleton = GetAnimationComponentValue(GetPropertyValue(record, "Components"), "BNAM"),
                AnimationDirectory = GetAnimationComponentValue(GetPropertyValue(record, "Components"), "CNAM"),
                AnimationFile = GetAnimationComponentValue(GetPropertyValue(record, "Components"), "FNAM"),
                Reflections = GetComponentReflections(plugin, RecordTypeCatalog.Container.RecordID, record.FormKey, GetPropertyValue(record, "Components"))
            }, record))
            .ToList();
    }

    private static IReadOnlyList<ConstructibleObjectDTO> MapConstructibleObjects(PluginDTO plugin, IStarfieldModGetter mod)
    {
        return mod.ConstructibleObjects
            .Select(record => LocalizedStringDTOMapper.AddLocalizedStrings(new ConstructibleObjectDTO
            {
                Game = SupportedGame.Starfield,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(record.FormKey),
                EditorID = record.EditorID ?? string.Empty,
                FormVersion = record.FormVersion,
                MajorRecordFlags = (int)record.StarfieldMajorRecordFlags,
                Version2 = GetPropertyNullableInt(record, "Version2"),
                VersionControl = GetPropertyNullableInt(record, "VersionControl"),
                ImportedAtUTC = DateTime.UtcNow,
                Description = GetTranslatedString(GetPropertyValue(record, "Description")),
                CreatedObjectFormKey = GetFormKeyFromObject(GetPropertyValue(record, "CreatedObject")),
                WorkbenchKeywordFormKey = GetFormKeyFromObject(GetPropertyValue(record, "WorkbenchKeyword")),
                AmountProduced = GetPropertyNullableInt(record, "AmountProduced"),
                Value = GetPropertyNullableInt(record, "Value"),
                MenuSortOrder = GetPropertyNullableDouble(record, "MenuSortOrder"),
                LearnMethod = GetPropertyValue(record, "LearnMethod")?.ToString(),
                Flags = FormatEnumFlagsWithUnknownBits(record.Flags) ?? string.Empty,
                MajorFlags = FormatMajorFlags(GetPropertyValue(record, "MajorFlags")),
                Components = GetConstructibleObjectComponents(plugin, record.FormKey, GetPropertyValue(record, "ConstructableComponents")),
                RecipeFilters = GetConstructibleObjectRecipeFilters(plugin, record.FormKey, GetPropertyValue(record, "RecipeFilters")),
                Conditions = GetConditionRules(plugin, SupportedGame.Starfield, record.FormKey, GetPropertyValue(record, "Conditions")),
                Sounds = GetNamedSounds(plugin, RecordTypeCatalog.ConstructibleObject.RecordID, record.FormKey, record, "DropdownSound", "PickupSound"),
                ScriptingAdapters = GetScriptingAdapters(plugin, RecordTypeCatalog.ConstructibleObject.RecordID, record)
            }, record))
            .ToList();
    }

    private static IReadOnlyList<ConditionFormDTO> MapConditionForms(PluginDTO plugin, IStarfieldModGetter mod)
    {
        return GetRecordCollection(mod, "ConditionRecords")
            .Select(record =>
            {
                var formKey = GetRequiredFormKey(record);
                return new ConditionFormDTO
                {
                    Game = SupportedGame.Starfield,
                    ModKey = plugin.ModKey,
                    FormKey = MapFormKey(formKey),
                    EditorID = GetPropertyValue(record, "EditorID")?.ToString() ?? string.Empty,
                    FormVersion = GetPropertyNullableInt(record, "FormVersion") ?? 0,
                    MajorRecordFlags = GetPropertyNullableInt(record, "StarfieldMajorRecordFlags") ?? 0,
                    Version2 = GetPropertyNullableInt(record, "Version2"),
                    VersionControl = GetPropertyNullableInt(record, "VersionControl"),
                    OwnerQuest = GetFormKeyFromObject(GetPropertyValue(record, "OwnerQuest")),
                    ImportedAtUTC = DateTime.UtcNow,
                    Conditions = GetConditionRules(plugin, SupportedGame.Starfield, formKey, GetPropertyValue(record, "Conditions"))
                };
            })
            .ToList();
    }

    private static IReadOnlyList<TerminalDTO> MapTerminals(PluginDTO plugin, IStarfieldModGetter mod)
    {
        return mod.Terminals
            .Select(record => LocalizedStringDTOMapper.AddLocalizedStrings(new TerminalDTO
            {
                Game = SupportedGame.Starfield,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(record.FormKey),
                EditorID = record.EditorID ?? string.Empty,
                FormVersion = record.FormVersion,
                MajorRecordFlags = (int)record.StarfieldMajorRecordFlags,
                Version2 = GetPropertyNullableInt(record, "Version2"),
                VersionControl = GetPropertyNullableInt(record, "VersionControl"),
                ImportedAtUTC = DateTime.UtcNow,
                ObjectBoundsFirst = FormatObjectBoundsPoint(GetPropertyValue(record, "ObjectBounds"), "First"),
                ObjectBoundsSecond = FormatObjectBoundsPoint(GetPropertyValue(record, "ObjectBounds"), "Second"),
                MenuFormKey = GetFormKeyFromObject(GetPropertyValue(record, "Menu")),
                Background = GetPropertyValue(record, "Background")?.ToString(),
                Name = GetTranslatedString(record.Name),
                Pnam = FormatHexValue(GetPropertyValue(record, "PNAM")),
                Fnam = FormatHexValue(GetPropertyValue(record, "FNAM")),
                MajorFlags = FormatMajorFlags(GetPropertyValue(record, "MajorFlags")),
                Jnam = FormatHexValue(GetPropertyValue(record, "JNAM")),
                MarkerFlags = GetPropertyValue(record, "MarkerFlags")?.ToString(),
                Gnam = FormatHexValue(GetPropertyValue(record, "GNAM")),
                WorkbenchData = FormatHexValue(GetPropertyValue(record, "WorkbenchData")),
                FurnitureTemplateFormKey = GetFormKeyFromObject(GetPropertyValue(record, "FurnitureTemplate")),
                MarkerModel = GetPropertyValue(record, "MarkerModel")?.ToString(),
                Models = GetModels(plugin, RecordTypeCatalog.Terminal.RecordID, record.FormKey, record.Model),
                Keywords = GetKeywordMappings(plugin, RecordTypeCatalog.Terminal.RecordID, record.FormKey, record.Keywords),
                ScriptingAdapters = GetScriptingAdapters(plugin, RecordTypeCatalog.Terminal.RecordID, record),
                Reflections = GetComponentReflections(plugin, RecordTypeCatalog.Terminal.RecordID, record.FormKey, GetPropertyValue(record, "Components")),
                ScriptFragments = GetScriptFragments(SupportedGame.Starfield, plugin, RecordTypeCatalog.Terminal.RecordID, record.FormKey, record),
                AnimationGraph = GetAnimationComponentValue(GetPropertyValue(record, "Components"), "ANAM"),
                AnimationSkeleton = GetAnimationComponentValue(GetPropertyValue(record, "Components"), "BNAM"),
                AnimationDirectory = GetAnimationComponentValue(GetPropertyValue(record, "Components"), "CNAM"),
                AnimationFile = GetAnimationComponentValue(GetPropertyValue(record, "Components"), "FNAM"),
                MarkerParameters = GetTerminalMarkerParameters(plugin, record.FormKey, GetPropertyValue(record, "MarkerParameters")),
                ForcedLocations = GetFormKeys(GetPropertyValue(record, "ForcedLocations"))
            }, record))
            .ToList();
    }

    private static IReadOnlyList<NPCDTO> MapNPCs(PluginDTO plugin, IStarfieldModGetter mod)
    {
        return mod.Npcs
            .Select(record => LocalizedStringDTOMapper.AddLocalizedStrings(new NPCDTO
            {
                Game = SupportedGame.Starfield,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(record.FormKey),
                EditorID = record.EditorID ?? string.Empty,
                FormVersion = record.FormVersion,
                MajorRecordFlags = (int)record.StarfieldMajorRecordFlags,
                IsCompressed = GetPropertyNullableBool(record, "IsCompressed"),
                ObjectBoundsFirst = GetPropertyValue(GetPropertyValue(record, "ObjectBounds"), "First")?.ToString(),
                ObjectBoundsSecond = GetPropertyValue(GetPropertyValue(record, "ObjectBounds"), "Second")?.ToString(),
                Version2 = GetPropertyNullableInt(record, "Version2"),
                VersionControl = GetPropertyNullableInt(record, "VersionControl"),
                ImportedAtUTC = DateTime.UtcNow,
                Name = GetTranslatedString(record.Name),
                ShortName = GetTranslatedString(record.ShortName),
                LongName = GetTranslatedString(record.LongName),
                Flags = FormatEnumerable(GetPropertyValue(record, "Flags")),
                MajorFlags = FormatMajorFlags(GetPropertyValue(record, "MajorFlags")),
                Level = GetNPCLevel(GetPropertyValue(record, "Level")),
                DispositionBase = record.DispositionBase,
                UseTemplateActors = GetPropertyValue(record, "UseTemplateActors")?.ToString(),
                Aggression = record.Aggression.ToString(),
                Confidence = record.Confidence.ToString(),
                EnergyLevel = record.EnergyLevel,
                Responsibility = record.Responsibility.ToString(),
                Assistance = record.Assistance.ToString(),
                Mood = GetPropertyStringOrNull(GetPropertyValue(record, "AIData") ?? record, "Mood"),
                GearedUpWeapons = record.GearedUpWeapons,
                HeightMin = record.HeightMin,
                HeightMax = record.HeightMax,
                SkinToneIndex = record.SkinToneIndex,
                Pronoun = record.Pronoun?.ToString(),
                VoiceFormKey = record.Voice.IsNull ? null : MapFormKey(record.Voice.FormKey),
                RaceFormKey = record.Race.IsNull ? null : MapFormKey(record.Race.FormKey),
                AttackRace = GetFormKeyFromObject(GetPropertyValue(record, "AttackRace")),
                CombatOverridePackageListFormKey = record.CombatOverridePackageList.IsNull ? null : MapFormKey(record.CombatOverridePackageList.FormKey),
                CombatStyleFormKey = record.CombatStyle.IsNull ? null : MapFormKey(record.CombatStyle.FormKey),
                DefaultPackageListFormKey = record.DefaultPackageList.IsNull ? null : MapFormKey(record.DefaultPackageList.FormKey),
                CrimeFactionFormKey = record.CrimeFaction.IsNull ? null : MapFormKey(record.CrimeFaction.FormKey),
                Class = GetFormKeyFromObject(GetPropertyValue(record, "Class")),
                DeathItem = GetFormKeyFromObject(GetPropertyValue(record, "DeathItem")),
                DefaultOutfit = GetFormKeyFromObject(GetPropertyValue(record, "DefaultOutfit")),
                SleepingOutfit = GetFormKeyFromObject(GetPropertyValue(record, "SleepingOutfit")),
                WornArmor = GetFormKeyFromObject(GetPropertyValue(record, "WornArmor")),
                SpaceOutfit = GetFormKeyFromObject(GetPropertyValue(record, "SpaceOutfit")),
                HeadTexture = GetFormKeyFromObject(GetPropertyValue(record, "HeadTexture")),
                Template = GetFormKeyFromObject(GetPropertyValue(record, "Template")),
                DefaultTemplate = GetFormKeyFromObject(GetPropertyValue(record, "DefaultTemplate")),
                TemplateActors = GetNPCTemplateActors(GetPropertyValue(record, "TemplateActors")),
                CalculatedHealth = GetPropertyNonZeroInt(record, "CalculatedHealth"),
                CalculatedActionPoints = GetPropertyNonZeroInt(record, "CalculatedActionPoints"),
                XpValueOffset = GetPropertyNonZeroInt(record, "XpValueOffset"),
                Unknown = GetPropertyNullableInt(record, "Unknown"),
                Unused = GetPropertyNonZeroInt(record, "Unused"),
                NAM5 = FormatSpriggitHexValue(GetPropertyValue(record, "NAM5")),
                Height = GetPropertyNullableDouble(record, "Height"),
                Weight = GetNPCWeight(GetPropertyValue(record, "Weight")),
                SoundLevel = GetPropertyValue(record, "SoundLevel")?.ToString(),
                TextureLighting = GetPropertyValue(record, "TextureLighting")?.ToString(),
                HairColor = FormatFormKeyOrString(GetPropertyValue(record, "HairColor")),
                FacialHairColor = GetPropertyValue(record, "FacialHairColor")?.ToString(),
                EyebrowColor = GetPropertyValue(record, "EyebrowColor")?.ToString(),
                EyeColor = GetPropertyValue(record, "EyeColor")?.ToString(),
                Keywords = GetKeywordMappings(plugin, RecordTypeCatalog.NPC.RecordID, record.FormKey, record.Keywords),
                Sounds = GetNamedSounds(plugin, RecordTypeCatalog.NPC.RecordID, record.FormKey, record, "Sound"),
                ScriptingAdapters = GetScriptingAdapters(plugin, RecordTypeCatalog.NPC.RecordID, record),
                BodyMorphRegionValues = SpriggitValueFormatter.Format(GetPropertyValue(record, "BodyMorphRegionValues")),
                ObjectTemplates = SpriggitValueFormatter.Format(GetPropertyValue(record, "ObjectTemplates")),
                AIData = SpriggitValueFormatter.Format(GetPropertyValue(record, "AIData")),
                Factions = GetNPCFactions(plugin, SupportedGame.Starfield, record.FormKey, GetPropertyValue(record, "Factions")),
                Properties = GetNPCProperties(plugin, SupportedGame.Starfield, record.FormKey, GetPropertyValue(record, "Properties")),
                Items = GetNPCItems(plugin, SupportedGame.Starfield, record.FormKey, GetPropertyValue(record, "Items")),
                Packages = GetFormKeys(GetPropertyValue(record, "Packages")),
                Perks = GetNPCPerks(plugin, SupportedGame.Starfield, record.FormKey, GetPropertyValue(record, "Perks")),
                ForcedLocations = GetFormKeys(GetPropertyValue(record, "ForcedLocations")),
                HeadParts = GetFormKeys(GetPropertyValue(record, "HeadParts")),
                ActorEffects = GetFormKeys(GetPropertyValue(record, "ActorEffect"))
                    .OrderBy(formKey => formKey.ModKey.FileName, StringComparer.Ordinal)
                    .ThenBy(formKey => formKey.Id)
                    .ToList(),
                Morphs = GetNPCMorphs(plugin, SupportedGame.Starfield, record.FormKey, GetPropertyValue(record, "Morphs")),
                FaceDialPositions = GetNPCFaceDialPositions(plugin, SupportedGame.Starfield, record.FormKey, GetPropertyValue(record, "FaceDialPositions")),
                FaceMorphGroups = GetNPCFaceMorphGroups(plugin, SupportedGame.Starfield, record.FormKey, GetPropertyValue(record, "FaceMorphs")),
                MorphBlends = GetNPCMorphBlends(plugin, SupportedGame.Starfield, record.FormKey, GetPropertyValue(record, "MorphBlends")),
                Tints = GetNPCTints(plugin, SupportedGame.Starfield, record.FormKey, GetPropertyValue(record, "Tints")),
                TintLayers = GetNPCTintLayers(plugin, SupportedGame.Starfield, record.FormKey, GetPropertyValue(record, "TintLayers")),
                PlayerSkills = GetNPCPlayerSkills(GetPropertyValue(record, "PlayerSkills")),
                FaceMorph = GetNPCFaceMorph(GetPropertyValue(record, "FaceMorph")),
                FaceParts = GetNPCFaceParts(GetPropertyValue(record, "FaceParts"))
            }, record))
            .ToList();
    }

    private static ClassDTO CreateClass(PluginDTO plugin, SupportedGame game, object record, string majorFlagsProperty)
    {
        var formKey = GetRequiredFormKey(record);
        return LocalizedStringDTOMapper.AddLocalizedStrings(new ClassDTO
        {
            Game = game,
            ModKey = plugin.ModKey,
            FormKey = MapFormKey(formKey),
            EditorID = GetPropertyValue(record, "EditorID")?.ToString() ?? string.Empty,
            FormVersion = GetPropertyNullableInt(record, "FormVersion") ?? 0,
            MajorRecordFlags = GetPropertyNullableInt(record, majorFlagsProperty) ?? 0,
            Version2 = GetPropertyNullableInt(record, "Version2"),
            VersionControl = GetPropertyNullableInt(record, "VersionControl"),
            ImportedAtUTC = DateTime.UtcNow,
            Name = GetTranslatedString(GetPropertyValue(record, "Name")),
            Description = GetTranslatedString(GetPropertyValue(record, "Description")),
            Teaches = GetPropertyValue(record, "Teaches")?.ToString(),
            MaxTrainingLevel = GetPropertyNullableInt(record, "MaxTrainingLevel"),
            BleedoutDefault = GetPropertyNullableDouble(record, "BleedoutDefault"),
            VoicePoints = GetPropertyNullableDouble(record, "VoicePoints"),
            Unknown = GetPropertyNullableDouble(record, "Unknown"),
            Unknown2 = GetPropertyNullableDouble(record, "Unknown2"),
            Properties = GetClassProperties(plugin, game, formKey, GetPropertyValue(record, "Properties"))
        }, record);
    }

    private static FactionDTO CreateFaction(PluginDTO plugin, SupportedGame game, object record, string majorFlagsProperty)
    {
        var formKey = GetRequiredFormKey(record);
        return LocalizedStringDTOMapper.AddLocalizedStrings(new FactionDTO
        {
            Game = game,
            ModKey = plugin.ModKey,
            FormKey = MapFormKey(formKey),
            EditorID = GetPropertyValue(record, "EditorID")?.ToString() ?? string.Empty,
            FormVersion = GetPropertyNullableInt(record, "FormVersion") ?? 0,
            MajorRecordFlags = GetPropertyNullableInt(record, majorFlagsProperty) ?? 0,
            Version2 = GetPropertyNullableInt(record, "Version2"),
            VersionControl = GetPropertyNullableInt(record, "VersionControl"),
            ImportedAtUTC = DateTime.UtcNow,
            Name = GetTranslatedString(GetPropertyValue(record, "Name")),
            Flags = FormatFactionFlags(GetPropertyValue(record, "Flags")),
            FormationRadius = GetPropertyNullableDouble(record, "FormationRadius"),
            Keyword = GetFormKeyFromObject(GetPropertyValue(record, "Keyword")),
            Herd = GetFormKeyFromObject(GetPropertyValue(record, "Herd")),
            VoiceType = GetFormKeyFromObject(GetPropertyValue(record, "VoiceType")),
            SharedCrimeFactionList = GetFormKeyFromObject(GetPropertyValue(record, "SharedCrimeFactionList")),
            VendorBuySellList = GetFormKeyFromObject(GetPropertyValue(record, "VendorBuySellList")),
            MerchantContainer = GetFormKeyFromObject(GetPropertyValue(record, "MerchantContainer")),
            ExteriorJailMarker = GetFormKeyFromObject(GetPropertyValue(record, "ExteriorJailMarker")),
            FollowerWaitMarker = GetFormKeyFromObject(GetPropertyValue(record, "FollowerWaitMarker")),
            StolenGoodsContainer = GetFormKeyFromObject(GetPropertyValue(record, "StolenGoodsContainer")),
            PlayerInventoryContainer = GetFormKeyFromObject(GetPropertyValue(record, "PlayerInventoryContainer")),
            JailOutfit = GetFormKeyFromObject(GetPropertyValue(record, "JailOutfit")),
            CrimeValues = CreateFactionCrimeValues(GetPropertyValue(record, "CrimeValues")),
            VendorValues = CreateFactionVendorValues(GetPropertyValue(record, "VendorValues")),
            VendorLocation = CreateFactionVendorLocation(
                GetPropertyValue(record, "VendorLocation"),
                GetPropertyValue(GetPropertyValue(record, "VendorLocation"), "Target")),
            Relations = GetFactionRelations(plugin, game, formKey, GetPropertyValue(record, "Relations")),
            Ranks = GetFactionRanks(plugin, game, formKey, GetPropertyValue(record, "Ranks")),
            Conditions = GetConditionRules(plugin, game, formKey, GetPropertyValue(record, "Conditions")),
            Components = GetRecordComponents(plugin, game, RecordTypeCatalog.Faction.RecordID, formKey, GetPropertyValue(record, "Components")),
            Keywords = GetSingleKeywordMapping(plugin, RecordTypeCatalog.Faction.RecordID, formKey, GetPropertyValue(record, "Keyword"))
        }, record);
    }

    private static List<ClassPropertyDTO> GetClassProperties(PluginDTO plugin, SupportedGame game, FormKey formKey, object? properties)
    {
        return properties is not IEnumerable enumerable
            ? new List<ClassPropertyDTO>()
            : enumerable.Cast<object>().Select((property, propertyIndex) => new ClassPropertyDTO
            {
                Game = game,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(formKey),
                PropertyIndex = propertyIndex,
                ActorValueFormKey = GetFormKeyFromObject(GetPropertyValue(property, "ActorValue")),
                Value = GetPropertyNullableDouble(property, "Value"),
                ImportedAtUTC = DateTime.UtcNow
            }).ToList();
    }

    private static NPCLevelDTO? GetNPCLevel(object? level)
    {
        if (level == null)
        {
            return null;
        }

        return new NPCLevelDTO
        {
            MutagenObjectType = GetPropertyValue(level, "MutagenObjectType")?.ToString() ?? level.GetType().Name,
            Level = GetPropertyNullableInt(level, "Level"),
            LevelMult = GetPropertyNullableDouble(level, "LevelMult")
        };
    }

    private static NPCTemplateActorsDTO? GetNPCTemplateActors(object? templateActors)
    {
        if (templateActors == null)
        {
            return null;
        }

        return new NPCTemplateActorsDTO
        {
            TraitTemplate = GetFormKeyFromObject(GetPropertyValue(templateActors, "TraitTemplate")),
            StatsTemplate = GetFormKeyFromObject(GetPropertyValue(templateActors, "StatsTemplate")),
            FactionsTemplate = GetFormKeyFromObject(GetPropertyValue(templateActors, "FactionsTemplate")),
            SpellListTemplate = GetFormKeyFromObject(GetPropertyValue(templateActors, "SpellListTemplate")),
            AiPackagesTemplate = GetFormKeyFromObject(GetPropertyValue(templateActors, "AiPackagesTemplate")),
            AiDataTemplate = GetFormKeyFromObject(GetPropertyValue(templateActors, "AiDataTemplate")),
            BaseDataTemplate = GetFormKeyFromObject(GetPropertyValue(templateActors, "BaseDataTemplate")),
            InventoryTemplate = GetFormKeyFromObject(GetPropertyValue(templateActors, "InventoryTemplate")),
            ScriptTemplate = GetFormKeyFromObject(GetPropertyValue(templateActors, "ScriptTemplate")),
            DefPackListTemplate = GetFormKeyFromObject(GetPropertyValue(templateActors, "DefPackListTemplate")),
            AttackDataTemplate = GetFormKeyFromObject(GetPropertyValue(templateActors, "AttackDataTemplate")),
            KeywordsTemplate = GetFormKeyFromObject(GetPropertyValue(templateActors, "KeywordsTemplate")),
            Unknown1 = GetFormKeyFromObject(GetPropertyValue(templateActors, "Unknown1")),
            Unknown2 = GetFormKeyFromObject(GetPropertyValue(templateActors, "Unknown2"))
        };
    }

    private static NPCWeightDTO? GetNPCWeight(object? weight)
    {
        if (weight == null)
        {
            return null;
        }

        if (double.TryParse(weight.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out var scalarWeight))
        {
            return new NPCWeightDTO { Value = scalarWeight };
        }

        var dto = new NPCWeightDTO
        {
            Thin = GetPropertyNonZeroDouble(weight, "Thin"),
            Muscular = GetPropertyNonZeroDouble(weight, "Muscular"),
            Fat = GetPropertyNonZeroDouble(weight, "Fat")
        };
        return dto.Thin == null && dto.Muscular == null && dto.Fat == null ? new NPCWeightDTO() : dto;
    }

    private static List<NPCFactionDTO> GetNPCFactions(PluginDTO plugin, SupportedGame game, FormKey formKey, object? factions)
    {
        return factions is not IEnumerable enumerable
            ? new List<NPCFactionDTO>()
            : enumerable.Cast<object>()
                .Select(faction => new
                {
                    Faction = faction,
                    FactionFormKey = GetFormKeyFromObject(GetPropertyValue(faction, "Faction")) ?? GetFormKeyFromObject(faction)
                })
                .OrderBy(faction => faction.FactionFormKey?.ModKey.FileName ?? string.Empty, StringComparer.Ordinal)
                .ThenBy(faction => faction.FactionFormKey?.Id ?? 0)
                .Select((faction, factionIndex) => new NPCFactionDTO
            {
                Game = game,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(formKey),
                FactionIndex = factionIndex,
                Faction = faction.FactionFormKey,
                Rank = GetPropertyNullableInt(faction.Faction, "Rank"),
                Fluff = FormatSpriggitHexValue(GetPropertyValue(faction.Faction, "Fluff")),
                ImportedAtUTC = DateTime.UtcNow
            }).ToList();
    }

    private static List<NPCPropertyDTO> GetNPCProperties(PluginDTO plugin, SupportedGame game, FormKey formKey, object? properties)
    {
        return properties is not IEnumerable enumerable
            ? new List<NPCPropertyDTO>()
            : enumerable.Cast<object>().Select((property, propertyIndex) => new NPCPropertyDTO
            {
                Game = game,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(formKey),
                PropertyIndex = propertyIndex,
                ActorValue = GetFormKeyFromObject(GetPropertyValue(property, "ActorValue")),
                Value = GetPropertyNullableDouble(property, "Value"),
                ImportedAtUTC = DateTime.UtcNow
            }).ToList();
    }

    private static List<NPCItemDTO> GetNPCItems(PluginDTO plugin, SupportedGame game, FormKey formKey, object? items)
    {
        if (items is not IEnumerable enumerable)
        {
            return new List<NPCItemDTO>();
        }

        return enumerable.Cast<object>()
            .Select(item =>
            {
                var itemData = GetPropertyValue(item, "Item") ?? item;
                return new
                {
                    Item = item,
                    ItemData = itemData,
                    ItemFormKey = GetFormKeyFromObject(GetPropertyValue(itemData, "Item"))
                };
            })
            .Where(item => item.ItemFormKey != null && item.ItemFormKey.Id != 0)
            .Select((item, itemIndex) => new NPCItemDTO
            {
                Game = game,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(formKey),
                ItemIndex = itemIndex,
                Item = item.ItemFormKey,
                Count = GetPropertyNullableInt(item.ItemData, "Count") ?? GetPropertyNullableInt(item.Item, "Count"),
                ImportedAtUTC = DateTime.UtcNow
            }).ToList();
    }

    private static List<NPCPerkDTO> GetNPCPerks(PluginDTO plugin, SupportedGame game, FormKey formKey, object? perks)
    {
        return perks is not IEnumerable enumerable
            ? new List<NPCPerkDTO>()
            : enumerable.Cast<object>().Select((perk, perkIndex) => new NPCPerkDTO
            {
                Game = game,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(formKey),
                PerkIndex = perkIndex,
                Perk = GetFormKeyFromObject(GetPropertyValue(perk, "Perk")) ?? GetFormKeyFromObject(perk),
                Rank = GetPropertyNullableInt(perk, "Rank"),
                Fluff = FormatSpriggitHexValue(GetPropertyValue(perk, "Fluff")),
                ImportedAtUTC = DateTime.UtcNow
            }).ToList();
    }

    private static List<NPCMorphDTO> GetNPCMorphs(PluginDTO plugin, SupportedGame game, FormKey formKey, object? morphs)
    {
        return morphs is not IEnumerable enumerable
            ? new List<NPCMorphDTO>()
            : enumerable.Cast<object>().Select((morph, morphIndex) => new NPCMorphDTO
            {
                Game = game,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(formKey),
                MorphIndex = morphIndex,
                Key = GetPropertyNullableLong(morph, "Key"),
                Value = GetPropertyNullableDouble(morph, "Value"),
                ImportedAtUTC = DateTime.UtcNow
            }).ToList();
    }

    private static List<NPCFaceMorphPositionDTO> GetNPCFaceMorphPositions(PluginDTO plugin, SupportedGame game, FormKey formKey, object? faceMorphs)
    {
        return faceMorphs is not IEnumerable enumerable
            ? new List<NPCFaceMorphPositionDTO>()
            : enumerable.Cast<object>().Select((faceMorph, faceMorphIndex) => new NPCFaceMorphPositionDTO
            {
                Game = game,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(formKey),
                FaceMorphIndex = faceMorphIndex,
                Index = GetPropertyNullableInt(faceMorph, "Index"),
                Position = GetPropertyValue(faceMorph, "Position")?.ToString(),
                Scale = GetPropertyNullableDouble(faceMorph, "Scale"),
                ImportedAtUTC = DateTime.UtcNow
            }).ToList();
    }

    private static List<NPCFaceDialPositionDTO> GetNPCFaceDialPositions(PluginDTO plugin, SupportedGame game, FormKey formKey, object? positions)
    {
        return positions is not IEnumerable enumerable
            ? new List<NPCFaceDialPositionDTO>()
            : enumerable.Cast<object>().Select((position, positionIndex) => new NPCFaceDialPositionDTO
            {
                Game = game,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(formKey),
                FaceDialPositionIndex = positionIndex,
                Index = GetPropertyNullableInt(position, "Index"),
                Position = GetPropertyNullableDouble(position, "Position"),
                ImportedAtUTC = DateTime.UtcNow
            }).ToList();
    }

    private static List<NPCFaceMorphGroupSetDTO> GetNPCFaceMorphGroups(PluginDTO plugin, SupportedGame game, FormKey formKey, object? faceMorphs)
    {
        if (faceMorphs is not IEnumerable enumerable)
        {
            return new List<NPCFaceMorphGroupSetDTO>();
        }

        return enumerable.Cast<object>().Select((faceMorph, faceMorphIndex) => new NPCFaceMorphGroupSetDTO
        {
            Game = game,
            ModKey = plugin.ModKey,
            FormKey = MapFormKey(formKey),
            FaceMorphIndex = faceMorphIndex,
            Index = GetPropertyNullableInt(faceMorph, "Index"),
            MorphGroups = GetNPCFaceMorphGroupRows(plugin, game, formKey, faceMorphIndex, GetPropertyValue(faceMorph, "MorphGroups")),
            ImportedAtUTC = DateTime.UtcNow
        }).ToList();
    }

    private static List<NPCFaceMorphGroupDTO> GetNPCFaceMorphGroupRows(PluginDTO plugin, SupportedGame game, FormKey formKey, int faceMorphIndex, object? morphGroups)
    {
        return morphGroups is not IEnumerable enumerable
            ? new List<NPCFaceMorphGroupDTO>()
            : enumerable.Cast<object>()
                .OrderBy(morphGroup => GetPropertyValue(morphGroup, "MorphGroup")?.ToString(), StringComparer.Ordinal)
                .Select((morphGroup, morphGroupIndex) => new NPCFaceMorphGroupDTO
            {
                Game = game,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(formKey),
                FaceMorphIndex = faceMorphIndex,
                MorphGroupIndex = morphGroupIndex,
                MorphGroup = GetPropertyValue(morphGroup, "MorphGroup")?.ToString(),
                BlendIntensity = GetPropertyNullableDouble(morphGroup, "BlendIntensity"),
                ImportedAtUTC = DateTime.UtcNow
            }).ToList();
    }

    private static List<NPCMorphBlendDTO> GetNPCMorphBlends(PluginDTO plugin, SupportedGame game, FormKey formKey, object? morphBlends)
    {
        return morphBlends is not IEnumerable enumerable
            ? new List<NPCMorphBlendDTO>()
            : enumerable.Cast<object>()
                .OrderBy(morphBlend => GetPropertyValue(morphBlend, "BlendName")?.ToString(), StringComparer.Ordinal)
                .Select((morphBlend, morphBlendIndex) => new NPCMorphBlendDTO
            {
                Game = game,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(formKey),
                MorphBlendIndex = morphBlendIndex,
                BlendName = GetPropertyValue(morphBlend, "BlendName")?.ToString(),
                Intensity = GetPropertyNullableDouble(morphBlend, "Intensity"),
                ImportedAtUTC = DateTime.UtcNow
            }).ToList();
    }

    private static List<NPCTintDTO> GetNPCTints(PluginDTO plugin, SupportedGame game, FormKey formKey, object? tints)
    {
        return tints is not IEnumerable enumerable
            ? new List<NPCTintDTO>()
            : enumerable.Cast<object>().Select((tint, tintIndex) => new NPCTintDTO
            {
                Game = game,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(formKey),
                TintIndex = tintIndex,
                TintType = GetPropertyValue(tint, "TintType")?.ToString(),
                TintGroup = GetPropertyValue(tint, "TintGroup")?.ToString(),
                TintName = GetPropertyValue(tint, "TintName")?.ToString(),
                TintTexture = GetPropertyValue(tint, "TintTexture")?.ToString(),
                TintColor = GetPropertyValue(tint, "TintColor")?.ToString(),
                TintIntensity = GetPropertyNullableDouble(tint, "TintIntensity"),
                ImportedAtUTC = DateTime.UtcNow
            }).ToList();
    }

    private static List<NPCTintLayerDTO> GetNPCTintLayers(PluginDTO plugin, SupportedGame game, FormKey formKey, object? tintLayers)
    {
        return tintLayers is not IEnumerable enumerable
            ? new List<NPCTintLayerDTO>()
            : enumerable.Cast<object>().Select((tintLayer, tintLayerIndex) => new NPCTintLayerDTO
            {
                Game = game,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(formKey),
                TintLayerIndex = tintLayerIndex,
                Index = GetPropertyNullableInt(tintLayer, "Index"),
                Color = GetPropertyValue(tintLayer, "Color")?.ToString(),
                InterpolationValue = GetPropertyNullableDouble(tintLayer, "InterpolationValue"),
                Preset = GetPropertyNullableInt(tintLayer, "Preset"),
                ImportedAtUTC = DateTime.UtcNow
            }).ToList();
    }

    private static NPCPlayerSkillsDTO? GetNPCPlayerSkills(object? playerSkills)
    {
        if (playerSkills == null)
        {
            return null;
        }

        return new NPCPlayerSkillsDTO
        {
            SkillValues = GetNPCPlayerSkillValues(GetPropertyValue(playerSkills, "SkillValues")),
            SkillOffsets = GetNPCPlayerSkillValues(GetPropertyValue(playerSkills, "SkillOffsets")),
            Health = GetPropertyNullableInt(playerSkills, "Health"),
            Magicka = GetPropertyNullableInt(playerSkills, "Magicka"),
            Stamina = GetPropertyNullableInt(playerSkills, "Stamina"),
            GearedUpWeapons = GetPropertyNullableInt(playerSkills, "GearedUpWeapons")
        };
    }

    private static List<NPCPlayerSkillValueDTO> GetNPCPlayerSkillValues(object? skillValues)
    {
        return skillValues is not IEnumerable enumerable
            ? new List<NPCPlayerSkillValueDTO>()
            : enumerable.Cast<object>().Select((skillValue, skillIndex) => new NPCPlayerSkillValueDTO
            {
                SkillIndex = skillIndex,
                Key = GetPropertyValue(skillValue, "Key")?.ToString(),
                Value = GetPropertyNullableInt(skillValue, "Value")
            }).ToList();
    }

    private static NPCFaceMorphDTO? GetNPCFaceMorph(object? faceMorph)
    {
        if (faceMorph == null)
        {
            return null;
        }

        return new NPCFaceMorphDTO
        {
            NoseLongVsShort = GetPropertyNullableDouble(faceMorph, "NoseLongVsShort"),
            NoseUpVsDown = GetPropertyNullableDouble(faceMorph, "NoseUpVsDown"),
            JawUpVsDown = GetPropertyNullableDouble(faceMorph, "JawUpVsDown"),
            JawNarrowVsWide = GetPropertyNullableDouble(faceMorph, "JawNarrowVsWide"),
            JawForwardVsBack = GetPropertyNullableDouble(faceMorph, "JawForwardVsBack"),
            CheeksUpVsDown = GetPropertyNullableDouble(faceMorph, "CheeksUpVsDown"),
            CheeksForwardVsBack = GetPropertyNullableDouble(faceMorph, "CheeksForwardVsBack"),
            EyesUpVsDown = GetPropertyNullableDouble(faceMorph, "EyesUpVsDown"),
            EyesInVsOut = GetPropertyNullableDouble(faceMorph, "EyesInVsOut"),
            BrowsUpVsDown = GetPropertyNullableDouble(faceMorph, "BrowsUpVsDown"),
            BrowsInVsOut = GetPropertyNullableDouble(faceMorph, "BrowsInVsOut"),
            BrowsForwardVsBack = GetPropertyNullableDouble(faceMorph, "BrowsForwardVsBack"),
            LipsUpVsDown = GetPropertyNullableDouble(faceMorph, "LipsUpVsDown"),
            LipsInVsOut = GetPropertyNullableDouble(faceMorph, "LipsInVsOut"),
            ChinNarrowVsWide = GetPropertyNullableDouble(faceMorph, "ChinNarrowVsWide"),
            ChinUpVsDown = GetPropertyNullableDouble(faceMorph, "ChinUpVsDown"),
            ChinUnderbiteVsOverbite = GetPropertyNullableDouble(faceMorph, "ChinUnderbiteVsOverbite"),
            EyesForwardVsBack = GetPropertyNullableDouble(faceMorph, "EyesForwardVsBack"),
            Unknown = GetPropertyNullableDouble(faceMorph, "Unknown")
        };
    }

    private static NPCFacePartsDTO? GetNPCFaceParts(object? faceParts)
    {
        if (faceParts == null)
        {
            return null;
        }

        return new NPCFacePartsDTO
        {
            Nose = GetPropertyNullableLong(faceParts, "Nose"),
            Unknown = GetPropertyNullableLong(faceParts, "Unknown"),
            Eyes = GetPropertyNullableLong(faceParts, "Eyes"),
            Mouth = GetPropertyNullableLong(faceParts, "Mouth")
        };
    }

    private static List<FactionRelationDTO> GetFactionRelations(PluginDTO plugin, SupportedGame game, FormKey formKey, object? relations)
    {
        return relations is not IEnumerable enumerable
            ? new List<FactionRelationDTO>()
            : enumerable.Cast<object>().Select((relation, relationIndex) => new FactionRelationDTO
            {
                Game = game,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(formKey),
                RelationIndex = relationIndex,
                Target = GetFormKeyFromObject(GetPropertyValue(relation, "Target")),
                Reaction = GetFactionOptionalString(relation, "Reaction", "Neutral"),
                ImportedAtUTC = DateTime.UtcNow
            }).ToList();
    }

    private static List<FactionRankDTO> GetFactionRanks(PluginDTO plugin, SupportedGame game, FormKey formKey, object? ranks)
    {
        return ranks is not IEnumerable enumerable
            ? new List<FactionRankDTO>()
            : enumerable.Cast<object>().Select((rank, rankIndex) => new FactionRankDTO
            {
                Game = game,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(formKey),
                RankIndex = rankIndex,
                Number = GetPropertyNullableInt(rank, "Number") ?? GetPropertyNullableInt(rank, "Rank"),
                Title = new FactionRankDTO.TitleDTO
                {
                    Male = GetTranslatedString(GetPropertyValue(rank, "MaleTitle")),
                    Female = GetTranslatedString(GetPropertyValue(rank, "FemaleTitle"))
                },
                ImportedAtUTC = DateTime.UtcNow
            }).ToList();
    }

    private static FactionDTO.CrimeValuesDTO? CreateFactionCrimeValues(object? crimeValues)
    {
        if (crimeValues is null)
        {
            return null;
        }

        var dto = new FactionDTO.CrimeValuesDTO
        {
            Arrest = GetFactionOptionalBool(crimeValues, "Arrest"),
            AttackOnSight = GetFactionOptionalBool(crimeValues, "AttackOnSight"),
            Murder = GetFactionOptionalInt(crimeValues, "Murder"),
            Assault = GetFactionOptionalInt(crimeValues, "Assault"),
            Trespass = GetFactionOptionalInt(crimeValues, "Trespass"),
            Pickpocket = GetFactionOptionalInt(crimeValues, "Pickpocket"),
            Steal = GetFactionOptionalInt(crimeValues, "Steal"),
            StealMult = GetFactionOptionalDouble(crimeValues, "StealMult"),
            StealMultiplier = GetFactionOptionalDouble(crimeValues, "StealMultiplier"),
            Escape = GetFactionOptionalInt(crimeValues, "Escape"),
            Werewolf = GetFactionOptionalInt(crimeValues, "Werewolf"),
            WerewolfUnused = GetFactionOptionalInt(crimeValues, "WerewolfUnused"),
            Unknown = GetFactionOptionalInt(crimeValues, "Unknown"),
            Piracy = GetFactionOptionalInt(crimeValues, "Piracy"),
            SmuggleMultiplier = GetFactionOptionalDouble(crimeValues, "SmuggleMultiplier")
        };
        return HasFactionCrimeValues(dto) ? dto : null;
    }

    private static FactionDTO.VendorValuesDTO? CreateFactionVendorValues(object? vendorValues)
    {
        if (vendorValues is null)
        {
            return null;
        }

        var dto = new FactionDTO.VendorValuesDTO
        {
            StartHour = GetFactionOptionalDouble(vendorValues, "StartHour"),
            EndHour = GetFactionOptionalDouble(vendorValues, "EndHour"),
            Radius = GetFactionOptionalInt(vendorValues, "Radius"),
            BuysStolenItems = GetFactionOptionalBool(vendorValues, "BuysStolenItems"),
            BuysNonStolenItems = GetFactionOptionalBool(vendorValues, "BuysNonStolenItems"),
            BuySellEverythingNotInList = GetFactionOptionalBool(vendorValues, "BuySellEverythingNotInList")
        };
        return HasFactionVendorValues(dto) ? dto : null;
    }

    private static int? GetFactionOptionalInt(object? source, string propertyName)
    {
        var value = GetPropertyNullableInt(source, propertyName);
        return value == 0 ? null : value;
    }

    private static double? GetFactionOptionalDouble(object? source, string propertyName)
    {
        var value = GetPropertyNullableDouble(source, propertyName);
        return value == 0 ? null : value;
    }

    private static bool? GetFactionOptionalBool(object? source, string propertyName)
    {
        var value = GetPropertyNullableBool(source, propertyName);
        return value == false ? null : value;
    }

    private static string? GetFactionOptionalString(object? source, string propertyName, string defaultValue)
    {
        var value = GetPropertyValue(source, propertyName)?.ToString();
        return string.Equals(value, defaultValue, StringComparison.OrdinalIgnoreCase) ? null : value;
    }

    private static bool HasFactionCrimeValues(FactionDTO.CrimeValuesDTO dto)
    {
        return dto.Arrest.HasValue ||
            dto.AttackOnSight.HasValue ||
            dto.Murder.HasValue ||
            dto.Assault.HasValue ||
            dto.Trespass.HasValue ||
            dto.Pickpocket.HasValue ||
            dto.Steal.HasValue ||
            dto.StealMult.HasValue ||
            dto.StealMultiplier.HasValue ||
            dto.Escape.HasValue ||
            dto.Werewolf.HasValue ||
            dto.WerewolfUnused.HasValue ||
            dto.Unknown.HasValue ||
            dto.Piracy.HasValue ||
            dto.SmuggleMultiplier.HasValue;
    }

    private static bool HasFactionVendorValues(FactionDTO.VendorValuesDTO dto)
    {
        return dto.StartHour.HasValue ||
            dto.EndHour.HasValue ||
            dto.Radius.HasValue ||
            dto.BuysStolenItems.HasValue ||
            dto.BuysNonStolenItems.HasValue ||
            dto.BuySellEverythingNotInList.HasValue;
    }

    private static FactionDTO.VendorLocationDTO? CreateFactionVendorLocation(object? vendorLocation, object? vendorLocationTarget)
    {
        return vendorLocation is null
            ? null
            : new FactionDTO.VendorLocationDTO
            {
                MutagenObjectType = GetPropertyValue(vendorLocation, "MutagenObjectType")?.ToString(),
                Target = vendorLocationTarget is null
                    ? null
                    : new FactionDTO.VendorLocationTargetDTO
                    {
                        MutagenObjectType = GetPropertyValue(vendorLocationTarget, "MutagenObjectType")?.ToString() ??
                            GetSpriggitMutagenObjectType(vendorLocationTarget),
                        Type = GetPropertyValue(vendorLocationTarget, "Type")?.ToString(),
                        Link = GetFormKeyFromObject(GetPropertyValue(vendorLocationTarget, "Link"))
                    }
        };
    }

    private static List<RecordComponentDTO> GetRecordComponents(PluginDTO plugin, SupportedGame game, string recordType, FormKey formKey, object? components)
    {
        return components is not IEnumerable enumerable
            ? new List<RecordComponentDTO>()
            : enumerable.Cast<object>().Select((component, componentIndex) => new RecordComponentDTO
            {
                Game = game,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(formKey),
                RecordType = recordType,
                ComponentIndex = componentIndex,
                MutagenObjectType = GetPropertyValue(component, "MutagenObjectType")?.ToString() ?? component.GetType().Name,
                DCED = GetIntList(GetPropertyValue(component, "DCED")),
                ImportedAtUTC = DateTime.UtcNow,
                Items = GetRecordComponentItems(plugin, game, recordType, formKey, componentIndex, GetPropertyValue(component, "Items"))
            }).ToList();
    }

    /// <summary>
    /// Converts a reflected enumerable of numeric values into an integer list while preserving source order.
    /// </summary>
    /// <param name="value">The reflected enumerable value to convert.</param>
    /// <returns>The converted integer values, or an empty list when the source is absent or not enumerable.</returns>
    private static List<int> GetIntList(object? value)
    {
        return value is not IEnumerable enumerable
            ? new List<int>()
            : enumerable
                .Cast<object>()
                .Select(item => Convert.ToInt32(item, CultureInfo.InvariantCulture))
                .ToList();
    }

    private static List<RecordComponentItemDTO> GetRecordComponentItems(PluginDTO plugin, SupportedGame game, string recordType, FormKey formKey, int componentIndex, object? items)
    {
        return items is not IEnumerable enumerable
            ? new List<RecordComponentItemDTO>()
            : enumerable.Cast<object>().Select((item, itemIndex) => new RecordComponentItemDTO
            {
                Game = game,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(formKey),
                RecordType = recordType,
                ComponentIndex = componentIndex,
                ItemIndex = itemIndex,
                DisplayFilter = GetFormKeyFromObject(GetPropertyValue(item, "DisplayFilter")),
                Unknown1 = GetPropertyNullableDouble(item, "Unknown1"),
                Unknown2 = GetPropertyNullableDouble(item, "Unknown2"),
                Unknown3 = GetPropertyNullableDouble(item, "Unknown3"),
                Unknown4 = GetPropertyNullableDouble(item, "Unknown4"),
                Unknown5 = GetPropertyNullableDouble(item, "Unknown5"),
                ImportedAtUTC = DateTime.UtcNow
            }).ToList();
    }

    private static List<MiscItemResourceDTO> GetMiscItemResources(PluginDTO plugin, FormKey formKey, object record)
    {
        return GetChildObjects(record, "Resources", "Resource")
            .Select((resource, resourceIndex) => CreateMiscItemResource(plugin, formKey, resource, resourceIndex))
            .Where(resource => resource != null)
            .Cast<MiscItemResourceDTO>()
            .ToList();
    }

    private static MiscItemResourceDTO? CreateMiscItemResource(PluginDTO plugin, FormKey formKey, object resource, int resourceIndex)
    {
        var resourceData = GetPropertyValue(resource, "Resource") ?? resource;
        var resourceFormKey = GetFormKeyFromObject(GetPropertyValue(resourceData, "Resource")) ?? GetFormKeyFromObject(resourceData);
        if (resourceFormKey == null)
        {
            return null;
        }

        return new MiscItemResourceDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = plugin.ModKey,
            FormKey = MapFormKey(formKey),
            Resource = resourceFormKey,
            ResourceIndex = resourceIndex,
            Count = GetPropertyNullableInt(resource, "Count") ?? GetPropertyNullableInt(resourceData, "Count"),
            ImportedAtUTC = DateTime.UtcNow
        };
    }

    private static List<KeywordMappingDTO> GetSingleKeywordMapping(PluginDTO plugin, string recordType, FormKey formKey, object? keyword)
    {
        if (GetFormKeyFromObject(keyword) is not { } keywordFormKey)
        {
            return new List<KeywordMappingDTO>();
        }

        return
        [
            new KeywordMappingDTO
            {
                Game = plugin.Game,
                ModKey = plugin.ModKey,
                RecordType = recordType,
                FormKey = MapFormKey(formKey),
                Keyword = keywordFormKey,
                KeywordIndex = 0,
                ImportedAtUTC = DateTime.UtcNow
            }
        ];
    }

    private static IReadOnlyList<MagicEffectDTO> MapMagicEffects(PluginDTO plugin, IStarfieldModGetter mod)
    {
        return mod.MagicEffects
            .Select(record => LocalizedStringDTOMapper.AddLocalizedStrings(new MagicEffectDTO
            {
                Game = SupportedGame.Starfield,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(record.FormKey),
                EditorID = record.EditorID ?? string.Empty,
                FormVersion = record.FormVersion,
                MajorRecordFlags = (int)record.StarfieldMajorRecordFlags,
                Version2 = GetPropertyNullableInt(record, "Version2"),
                VersionControl = GetPropertyNullableInt(record, "VersionControl"),
                ImportedAtUTC = DateTime.UtcNow,
                Name = GetTranslatedString(() => record.Name),
                Description = GetTranslatedString(() => record.Description),
                Flags = FormatEnumFlagsWithUnknownBits(record.Flags) ?? string.Empty,
                CastType = record.CastType.ToString(),
                TargetType = GetNonDefaultString(record.TargetType),
                CastingSoundLevel = GetPropertyStringOrNull(record, "CastingSoundLevel"),
                DualCastScale = GetPropertyValue(record, "DualCastScale")?.ToString(),
                Unknown1 = GetNonDefaultString(GetPropertyValue(record, "Unknown1")),
                BaseCost = GetNonDefaultString(GetPropertyValue(record, "BaseCost")),
                MagicSkill = GetNonDefaultFormKeyOrString(record, "MagicSkill"),
                CastingLightFormKey = GetLinkedFormKey(record, "CastingLight"),
                MenuDisplayObjectFormKey = GetLinkedFormKey(record, "MenuDisplayObject"),
                MinimumSkillLevel = GetPropertyNonZeroInt(record, "MinimumSkillLevel"),
                SkillUsageMultiplier = GetNonDefaultString(GetPropertyValue(record, "SkillUsageMultiplier")),
                SpellmakingCastingTime = GetNonDefaultString(GetPropertyValue(record, "SpellmakingCastingTime")),
                TaperWeight = GetNonDefaultString(GetPropertyValue(record, "TaperWeight")),
                SecondActorValue = GetNonDefaultFormKeyOrString(record, "SecondActorValue"),
                SecondActorValueWeight = GetNonDefaultString(GetPropertyValue(record, "SecondActorValueWeight")),
                SpellmakingArea = GetPropertyNonZeroInt(record, "SpellmakingArea"),
                EnchantShaderFormKey = GetLinkedFormKey(record, "EnchantShader"),
                ActorValue2FormKey = record.ActorValue2.IsNull ? null : MapFormKey(record.ActorValue2.FormKey),
                ResistValueFormKey = record.ResistValue.IsNull ? null : MapFormKey(record.ResistValue.FormKey),
                ResistValue = GetFormKeyOrString(record, "ResistValue"),
                PerkToApplyFormKey = record.PerkToApply.IsNull ? null : MapFormKey(record.PerkToApply.FormKey),
                EquipAbilityFormKey = record.EquipAbility.IsNull ? null : MapFormKey(record.EquipAbility.FormKey),
                ExplosionFormKey = record.Explosion.IsNull ? null : MapFormKey(record.Explosion.FormKey),
                CastingArtFormKey = record.CastingArt.IsNull ? null : MapFormKey(record.CastingArt.FormKey),
                HitEffectArtFormKey = record.HitEffectArt.IsNull ? null : MapFormKey(record.HitEffectArt.FormKey),
                HitShaderFormKey = record.HitShader.IsNull ? null : MapFormKey(record.HitShader.FormKey),
                ImageSpaceModifierFormKey = record.ImageSpaceModifier.IsNull ? null : MapFormKey(record.ImageSpaceModifier.FormKey),
                ImpactDataFormKey = record.ImpactData.IsNull ? null : MapFormKey(record.ImpactData.FormKey),
                ProjectileFormKey = record.Projectile.IsNull ? null : MapFormKey(record.Projectile.FormKey),
                Archetype = GetMagicEffectArchetype(record),
                ArchetypeActorValue = GetFormKeyOrString(record.Archetype, "ActorValue"),
                ArchetypeAssociationFormKey = GetLinkedFormKey(record.Archetype, "Association"),
                UnknownFloat1 = GetPropertyNonZeroFloat(record, "UnknownFloat1"),
                UnknownFloat3 = record.UnknownFloat3 == 0 ? null : record.UnknownFloat3,
                UnknownFloat4 = GetPropertyNonZeroFloat(record, "UnknownFloat4"),
                UnknownInt2 = unchecked((int)record.UnknownInt2),
                UnknownInt3 = GetPropertyNonZeroLong(record, "UnknownInt3"),
                Unknown = Convert.ToHexString(record.Unknown.ToArray()),
                Unknown2 = Convert.ToHexString(record.Unknown2.ToArray()),
                DataTypeState = record.DATADataTypeState.ToString(),
                Keywords = GetKeywordMappings(plugin, RecordTypeCatalog.MagicEffect.RecordID, record.FormKey, record.Keywords),
                Sounds = GetIndexedSounds(plugin, RecordTypeCatalog.MagicEffect.RecordID, record.FormKey, record),
                Conditions = GetConditionRules(plugin, SupportedGame.Starfield, record.FormKey, GetPropertyValue(record, "Conditions")),
                ScriptingAdapters = GetScriptingAdapters(plugin, RecordTypeCatalog.MagicEffect.RecordID, record)
            }, record))
            .ToList();
    }

    private static IReadOnlyList<PerkDTO> MapPerks(PluginDTO plugin, IStarfieldModGetter mod)
    {
        return mod.Perks
            .Select(record => LocalizedStringDTOMapper.AddLocalizedStrings(new PerkDTO
            {
                Game = SupportedGame.Starfield,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(record.FormKey),
                EditorID = record.EditorID ?? string.Empty,
                FormVersion = record.FormVersion,
                MajorRecordFlags = (int)record.StarfieldMajorRecordFlags,
                Version2 = GetPropertyNullableInt(record, "Version2"),
                VersionControl = GetPropertyNullableInt(record, "VersionControl"),
                ImportedAtUTC = DateTime.UtcNow,
                Name = GetTranslatedString(record.Name),
                Description = GetTranslatedString(record.Description),
                Flags = record.Flags.ToString(),
                SkillGroup = record.SkillGroup.ToString(),
                CrewAssignment = record.CrewAssignment.ToString(),
                PerkIcon = record.PerkIcon,
                Category = record.Categroy.ToString(),
                RestrictionFormKey = record.Restriction.IsNull ? null : MapFormKey(record.Restriction.FormKey),
                TrainingFormKey = record.Training.IsNull ? null : MapFormKey(record.Training.FormKey),
                MajorFlags = record.MajorFlags.ToString(),
                Level = GetPropertyNullableInt(record, "Level"),
                NumRanks = GetPropertyNullableInt(record, "NumRanks"),
                Playable = GetPropertyNullableBool(record, "Playable"),
                Hidden = GetPropertyNullableBool(record, "Hidden"),
                NextPerk = GetLinkedFormKey(record, "NextPerk"),
                Effects = GetPerkEffects(plugin, SupportedGame.Starfield, record.FormKey, record),
                Ranks = GetPerkRanks(plugin, record),
                BackgroundSkills = GetPerkBackgroundSkills(plugin, record),
                Conditions = GetPerkConditionRules(plugin, SupportedGame.Starfield, record.FormKey, record),
                Sounds = GetNamedSounds(plugin, RecordTypeCatalog.Perk.RecordID, record.FormKey, record, "Sound"),
                ScriptFragments = GetScriptFragments(SupportedGame.Starfield, plugin, RecordTypeCatalog.Perk.RecordID, record.FormKey, record),
                ScriptingAdapters = GetScriptingAdapters(plugin, RecordTypeCatalog.Perk.RecordID, record)
            }, record))
            .ToList();
    }

    private static List<PerkRankDTO> GetPerkRanks(PluginDTO plugin, IPerkGetter record)
    {
        var importedAtUTC = DateTime.UtcNow;
        return record.Ranks
            .Select((rank, rankIndex) => new PerkRankDTO
            {
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(record.FormKey),
                RankIndex = rankIndex,
                Description = GetTranslatedString(() => rank.Description),
                UnknownStaticFormKey = rank.UnknownStatic.IsNull ? null : MapFormKey(rank.UnknownStatic.FormKey),
                ConditionCount = rank.Conditions?.Count ?? 0,
                ActivityCount = rank.Activities?.Count ?? 0,
                Conditions = GetConditionRules(plugin, SupportedGame.Starfield, record.FormKey, rank.Conditions, GetPerkRankConditionSlot(rankIndex)),
                ImportedAtUTC = importedAtUTC,
                Effects = GetPerkRankEffects(plugin, record.FormKey, rank, rankIndex, importedAtUTC),
                Activities = GetPerkRankActivities(plugin, record.FormKey, rank, rankIndex, importedAtUTC)
            })
            .ToList();
    }

    private static List<PerkRankActivityDTO> GetPerkRankActivities(PluginDTO plugin, FormKey formKey, IPerkRankGetter rank, int rankIndex, DateTime importedAtUTC)
    {
        if (rank.Activities == null)
        {
            return new List<PerkRankActivityDTO>();
        }

        return rank.Activities
            .Select((activity, activityIndex) => new PerkRankActivityDTO
            {
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(formKey),
                RankIndex = rankIndex,
                ActivityIndex = activityIndex,
                ATAN = GetPropertyValue(activity, "ATAN")?.ToString(),
                Name = GetTranslatedString(GetPropertyValue(activity, "Name")),
                Description = GetTranslatedString(GetPropertyValue(activity, "Description")),
                ANAM = GetPropertyValue(activity, "ANAM")?.ToString(),
                Configuration = GetPropertyValue(activity, "Configuration")?.ToString(),
                ProgressionEvalutor = GetPerkRankActivityProgressionEvaluators(plugin, formKey, rankIndex, activityIndex, activity, importedAtUTC),
                ImportedAtUTC = importedAtUTC
            })
            .ToList();
    }

    private static List<PerkRankActivityProgressionEvaluatorDTO> GetPerkRankActivityProgressionEvaluators(
        PluginDTO plugin,
        FormKey formKey,
        int rankIndex,
        int activityIndex,
        object activity,
        DateTime importedAtUTC)
    {
        return (GetPropertyValue(activity, "ProgressionEvalutor") as IEnumerable)?.Cast<object>()
            .Select((evaluator, evaluatorIndex) => new PerkRankActivityProgressionEvaluatorDTO
            {
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(formKey),
                RankIndex = rankIndex,
                ActivityIndex = activityIndex,
                EvaluatorIndex = evaluatorIndex,
                Name = GetPropertyValue(evaluator, "Name")?.ToString(),
                Conditions = GetConditionRules(plugin, SupportedGame.Starfield, formKey, GetPropertyValue(evaluator, "Conditions"), GetPerkActivityEvaluatorConditionSlot(rankIndex, activityIndex, evaluatorIndex)),
                ImportedAtUTC = importedAtUTC
            })
            .ToList() ?? new List<PerkRankActivityProgressionEvaluatorDTO>();
    }

    private static List<PerkRankEffectDTO> GetPerkRankEffects(PluginDTO plugin, FormKey formKey, IPerkRankGetter rank, int rankIndex, DateTime importedAtUTC)
    {
        return rank.Effects
            .Select((effect, effectIndex) =>
            {
                var dto = new PerkRankEffectDTO
                {
                    ModKey = plugin.ModKey,
                    FormKey = MapFormKey(formKey),
                    RankIndex = rankIndex,
                    EffectIndex = effectIndex,
                    MutagenObjectType = effect.GetType().Name,
                    Rank = effect.Rank,
                    Priority = effect.Priority,
                    PerkEntryId = effect.PerkEntryID,
                    Flags = FormatEnumerable(effect.Flags),
                    ButtonLabel = GetTranslatedString(() => effect.ButtonLabel),
                    ConditionCount = effect.Conditions.Count,
                    ActorValue = GetFormKeyOrString(effect, "ActorValue"),
                    Spell = GetFormKeyOrString(effect, "Spell"),
                    Quest = GetFormKeyOrString(effect, "Quest"),
                    Stage = GetPropertyNullableInt(effect, "Stage"),
                    Conditions = GetPerkEffectConditionTabs(plugin, SupportedGame.Starfield, formKey, rankIndex, effectIndex, effect, importedAtUTC),
                    ImportedAtUTC = importedAtUTC
                };

                if (effect is IAPerkEntryPointEffectGetter entryPointEffect)
                {
                    dto.EntryPoint = entryPointEffect.EntryPoint.ToString();
                    dto.PerkConditionTabCount = entryPointEffect.PerkConditionTabCount;
                }

                if (effect is IPerkEntryPointModifyValueGetter modifyValueEffect)
                {
                    dto.Modification = modifyValueEffect.Modification.ToString();
                    dto.Value = modifyValueEffect.Value;
                }

                return dto;
            })
            .ToList();
    }

    private static List<PerkEffectDTO> GetPerkEffects(PluginDTO plugin, SupportedGame game, FormKey formKey, object record)
    {
        var effects = GetOrderedPerkEffects(GetPropertyValue(record, "Effects"));
        if (effects.Count == 0) return new List<PerkEffectDTO>();

        var importedAtUTC = DateTime.UtcNow;
        return effects
            .Select((effect, effectIndex) => new PerkEffectDTO
            {
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(formKey),
                EffectIndex = effectIndex,
                MutagenObjectType = effect.GetType().Name,
                Rank = GetPropertyNullableInt(effect, "Rank"),
                Priority = GetPropertyNullableInt(effect, "Priority"),
                PerkEntryId = GetPropertyNullableInt(effect, "PerkEntryID"),
                Flags = FormatEnumerable(GetPropertyValue(effect, "Flags")),
                ButtonLabel = GetTranslatedString(GetPropertyValue(effect, "ButtonLabel")),
                ConditionCount = GetEnumerableCount(GetPropertyValue(effect, "Conditions")),
                EntryPoint = GetPropertyStringOrNull(effect, "EntryPoint"),
                PerkConditionTabCount = GetPropertyNullableInt(effect, "PerkConditionTabCount"),
                Modification = GetPropertyStringOrNull(effect, "Modification"),
                Value = GetPropertyNullableDouble(effect, "Value"),
                ActorValue = GetFormKeyOrString(effect, "ActorValue"),
                Spell = GetFormKeyOrString(effect, "Spell"),
                Quest = GetFormKeyOrString(effect, "Quest"),
                Stage = GetPropertyNullableInt(effect, "Stage"),
                Conditions = GetPerkEffectConditionTabs(plugin, game, formKey, null, effectIndex, effect, importedAtUTC),
                ImportedAtUTC = importedAtUTC
            })
            .ToList();
    }

    private static List<PerkEffectConditionTabDTO> GetPerkEffectConditionTabs(
        PluginDTO plugin,
        SupportedGame game,
        FormKey formKey,
        int? rankIndex,
        int effectIndex,
        object effect,
        DateTime importedAtUTC)
    {
        return (GetPropertyValue(effect, "Conditions") as IEnumerable)?.Cast<object>()
            .Select((conditionTab, conditionTabIndex) =>
            {
                var slot = GetPerkEffectConditionSlot(rankIndex, effectIndex, conditionTabIndex);
                return new PerkEffectConditionTabDTO
                {
                    ModKey = plugin.ModKey,
                    FormKey = MapFormKey(formKey),
                    RankIndex = rankIndex,
                    EffectIndex = effectIndex,
                    ConditionTabIndex = conditionTabIndex,
                    RunOnTabIndex = GetPropertyNullableInt(conditionTab, "RunOnTabIndex"),
                    ConditionCount = GetEnumerableCount(GetPropertyValue(conditionTab, "Conditions")),
                    Conditions = GetConditionRules(plugin, game, formKey, GetPropertyValue(conditionTab, "Conditions"), slot),
                    ImportedAtUTC = importedAtUTC
                };
            })
            .ToList() ?? new List<PerkEffectConditionTabDTO>();
    }

    private static List<ConditionFormConditionDTO> GetPerkConditionRules(PluginDTO plugin, SupportedGame game, FormKey formKey, object record)
    {
        var conditions = GetConditionRules(plugin, game, formKey, GetPropertyValue(record, "Conditions"));
        AddPerkEffectConditionRules(conditions, plugin, game, formKey, null, GetPropertyValue(record, "Effects"));

        var ranks = GetPropertyValue(record, "Ranks") as IEnumerable;
        if (ranks != null)
        {
            foreach (var rank in ranks.Cast<object>().Select((value, index) => new { value, index }))
            {
                conditions.AddRange(GetConditionRules(plugin, game, formKey, GetPropertyValue(rank.value, "Conditions"), GetPerkRankConditionSlot(rank.index)));
                AddPerkEffectConditionRules(conditions, plugin, game, formKey, rank.index, GetPropertyValue(rank.value, "Effects"));
                AddPerkActivityEvaluatorConditionRules(conditions, plugin, game, formKey, rank.index, GetPropertyValue(rank.value, "Activities"));
            }
        }

        return conditions;
    }

    private static void AddPerkActivityEvaluatorConditionRules(
        ICollection<ConditionFormConditionDTO> conditions,
        PluginDTO plugin,
        SupportedGame game,
        FormKey formKey,
        int rankIndex,
        object? activities)
    {
        if (activities is not IEnumerable activityList)
        {
            return;
        }

        foreach (var activity in activityList.Cast<object>().Select((value, index) => new { value, index }))
        {
            var evaluators = GetPropertyValue(activity.value, "ProgressionEvalutor") as IEnumerable;
            if (evaluators == null)
            {
                continue;
            }

            foreach (var evaluator in evaluators.Cast<object>().Select((value, index) => new { value, index }))
            {
                var slot = GetPerkActivityEvaluatorConditionSlot(rankIndex, activity.index, evaluator.index);
                foreach (var condition in GetConditionRules(plugin, game, formKey, GetPropertyValue(evaluator.value, "Conditions"), slot))
                {
                    conditions.Add(condition);
                }
            }
        }
    }

    private static void AddPerkEffectConditionRules(
        ICollection<ConditionFormConditionDTO> conditions,
        PluginDTO plugin,
        SupportedGame game,
        FormKey formKey,
        int? rankIndex,
        object? effects)
    {
        var effectList = rankIndex.HasValue
            ? (effects as IEnumerable)?.Cast<object>().ToList() ?? new List<object>()
            : GetOrderedPerkEffects(effects);
        if (effectList.Count == 0)
        {
            return;
        }

        foreach (var effect in effectList.Select((value, index) => new { value, index }))
        {
            var conditionTabs = GetPropertyValue(effect.value, "Conditions") as IEnumerable;
            if (conditionTabs == null)
            {
                continue;
            }

            foreach (var conditionTab in conditionTabs.Cast<object>().Select((value, index) => new { value, index }))
            {
                var slot = GetPerkEffectConditionSlot(rankIndex, effect.index, conditionTab.index);
                foreach (var condition in GetConditionRules(plugin, game, formKey, GetPropertyValue(conditionTab.value, "Conditions"), slot))
                {
                    conditions.Add(condition);
                }
            }
        }
    }

    private static string GetPerkRankConditionSlot(int rankIndex)
    {
        return $"Ranks[{rankIndex}].Conditions";
    }

    private static string GetPerkEffectConditionSlot(int? rankIndex, int effectIndex, int conditionTabIndex)
    {
        return rankIndex.HasValue
            ? $"Ranks[{rankIndex.Value}].Effects[{effectIndex}].Conditions[{conditionTabIndex}].Conditions"
            : $"Effects[{effectIndex}].Conditions[{conditionTabIndex}].Conditions";
    }

    private static string GetPerkActivityEvaluatorConditionSlot(int rankIndex, int activityIndex, int evaluatorIndex)
    {
        return $"Ranks[{rankIndex}].Activities[{activityIndex}].ProgressionEvalutor[{evaluatorIndex}].Conditions";
    }

    private static List<object> GetOrderedPerkEffects(object? effects)
    {
        return (effects as IEnumerable)?.Cast<object>()
            .Select((value, originalIndex) => new { value, originalIndex })
            .OrderBy(effect => GetPropertyNullableInt(effect.value, "Priority") ?? 0)
            .ThenBy(effect => effect.originalIndex)
            .Select(effect => effect.value)
            .ToList() ?? new List<object>();
    }

    private static List<PerkBackgroundSkillDTO> GetPerkBackgroundSkills(PluginDTO plugin, IPerkGetter record)
    {
        var importedAtUTC = DateTime.UtcNow;
        return record.BackgroundSkills
            .Select((skill, skillIndex) => new PerkBackgroundSkillDTO
            {
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(record.FormKey),
                SkillFormKey = MapFormKey(skill.FormKey),
                SkillIndex = skillIndex,
                ImportedAtUTC = importedAtUTC
            })
            .ToList();
    }

    private static TranslatedStringDTO? GetTranslatedString(Func<ITranslatedStringGetter?> valueFactory)
    {
        try
        {
            return LocalizedStringDTOMapper.ToTranslatedStringDTO(valueFactory());
        }
        catch (ArgumentException)
        {
            return null;
        }
    }

    private static TranslatedStringDTO? GetTranslatedString(object? value)
    {
        return LocalizedStringDTOMapper.ToTranslatedStringDTO(value);
    }

    private static List<ScriptingAdapterDTO> GetScriptingAdapters(PluginDTO plugin, string recordType, IHaveVirtualMachineAdapterGetter record)
    {
        if (record.VirtualMachineAdapter == null) return new List<ScriptingAdapterDTO>();

        var importedAtUTC = DateTime.UtcNow;
        var adapters = record.VirtualMachineAdapter.Scripts
            .Select((script, scriptIndex) => new ScriptingAdapterDTO
            {
                Game = SupportedGame.Starfield,
                ModKey = plugin.ModKey,
                RecordType = recordType,
                FormKey = MapFormKey(record.FormKey),
                Name = script.Name,
                ScriptIndex = scriptIndex,
                ImportedAtUTC = importedAtUTC,
                Properties = GetScriptingAdapterProperties(plugin, recordType, record.FormKey, script, importedAtUTC)
            })
            .ToList();
        AddScriptFragmentScriptingAdapter(adapters, plugin, recordType, record.FormKey, record.VirtualMachineAdapter, importedAtUTC);
        return adapters;
    }

    private static void AddScriptFragmentScriptingAdapter(
        ICollection<ScriptingAdapterDTO> adapters,
        PluginDTO plugin,
        string recordType,
        FormKey formKey,
        object? virtualMachineAdapter,
        DateTime importedAtUTC)
    {
        var script = GetPropertyValue(GetPropertyValue(virtualMachineAdapter, "ScriptFragments"), "Script") as IScriptEntryGetter;
        if (script == null)
        {
            return;
        }

        var scriptName = script.Name;
        if (string.IsNullOrWhiteSpace(scriptName))
        {
            return;
        }

        adapters.Add(new ScriptingAdapterDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = plugin.ModKey,
            RecordType = recordType,
            FormKey = MapFormKey(formKey),
            Name = scriptName,
            ScriptIndex = adapters.Count,
            ImportedAtUTC = importedAtUTC,
            Properties = GetScriptingAdapterProperties(plugin, recordType, formKey, script, importedAtUTC)
        });
    }

    private static List<ModelDTO> GetModels(PluginDTO plugin, string recordType, FormKey formKey, IModelGetter? model)
    {
        if (model == null) return new List<ModelDTO>();

        var importedAtUTC = DateTime.UtcNow;
        return new List<ModelDTO>
        {
            new ModelDTO
            {
                Game = SupportedGame.Starfield,
                ModKey = plugin.ModKey,
                RecordType = recordType,
                FormKey = MapFormKey(formKey),
                ModelSlot = "Model",
                ModelGender = string.Empty,
                File = FormatSpriggitModelFilePath(model.File?.ToString()),
                Data = FormatHexValue(GetPropertyValue(model, "Data")),
                TextureFileHashes = model.TextureFileHashes == null ? null : Convert.ToHexString(model.TextureFileHashes.Value.ToArray()),
                LightLayer = model.LightLayer,
                Flags = model.Flags?.ToString(),
                ColorRemappingIndex = model.ColorRemappingIndex,
                FlagsVestigial = model.FlagsVestigial?.ToString(),
                ImportedAtUTC = importedAtUTC,
                MaterialSwaps = GetModelMaterialSwaps(plugin, recordType, formKey, model, importedAtUTC)
            }
        };
    }

    private static List<ModelMaterialSwapDTO> GetModelMaterialSwaps(PluginDTO plugin, string recordType, FormKey formKey, object model, DateTime importedAtUTC)
    {
        var materialSwaps = (GetPropertyValue(model, "MaterialSwaps") as IEnumerable)?.Cast<object>().ToList() ?? new List<object>();
        if (GetPropertyValue(model, "AlternateTextures") is IEnumerable alternateTextures)
        {
            materialSwaps.AddRange(alternateTextures.Cast<object>());
        }

        var materialSwap = GetPropertyValue(model, "MaterialSwap");
        if (materialSwap != null)
        {
            materialSwaps.Add(materialSwap);
        }

        return materialSwaps
            .Select((materialSwap, materialSwapIndex) => (GetFormKeyFromObject(GetPropertyValue(materialSwap, "NewTexture")) ?? GetFormKeyFromObject(materialSwap)) is { } materialSwapFormKey
                ? new ModelMaterialSwapDTO
                {
                    Game = SupportedGame.Starfield,
                    ModKey = plugin.ModKey,
                    RecordType = recordType,
                    FormKey = MapFormKey(formKey),
                    ModelSlot = "Model",
                    ModelGender = string.Empty,
                    Name = GetPropertyValue(materialSwap, "Name")?.ToString(),
                    MaterialSwapFormKey = materialSwapFormKey,
                    MaterialSwapIndex = materialSwapIndex,
                    ImportedAtUTC = importedAtUTC
                }
                : null)
            .Where(materialSwap => materialSwap != null)
            .Cast<ModelMaterialSwapDTO>()
            .ToList();
    }

    private static string? FormatSpriggitModelFilePath(string? file)
    {
        if (file == null)
        {
            return null;
        }

        return file.StartsWith("Meshes\\", StringComparison.OrdinalIgnoreCase) ||
               file.StartsWith("Meshes/", StringComparison.OrdinalIgnoreCase)
            ? file[7..]
            : file;
    }

    private static List<KeywordMappingDTO> GetKeywordMappings(PluginDTO plugin, string recordType, FormKey formKey, IEnumerable<IFormLinkGetter<IKeywordGetter>>? keywords)
    {
        if (keywords == null) return new List<KeywordMappingDTO>();

        var importedAtUTC = DateTime.UtcNow;
        return keywords
            .Select((keyword, keywordIndex) => new KeywordMappingDTO
            {
                Game = SupportedGame.Starfield,
                ModKey = plugin.ModKey,
                RecordType = recordType,
                FormKey = MapFormKey(formKey),
                Keyword = MapFormKey(keyword.FormKey),
                KeywordIndex = keywordIndex,
                ImportedAtUTC = importedAtUTC
            })
            .ToList();
    }

    private static List<KeywordMappingDTO> GetKeywordMappingsFromNestedKeywordLists(PluginDTO plugin, string recordType, FormKey formKey, object? keywordSources)
    {
        if (keywordSources is not IEnumerable enumerable) return new List<KeywordMappingDTO>();

        var importedAtUTC = DateTime.UtcNow;
        var keywords = new List<KeywordMappingDTO>();
        foreach (var keywordSource in enumerable.Cast<object>())
        {
            var nestedKeywords = GetPropertyValue(keywordSource, "Keywords") as IEnumerable;
            if (nestedKeywords == null)
            {
                continue;
            }

            foreach (var keyword in nestedKeywords.Cast<object>())
            {
                if (GetFormKeyFromObject(keyword) is not { } keywordFormKey)
                {
                    continue;
                }

                keywords.Add(new KeywordMappingDTO
                {
                    Game = SupportedGame.Starfield,
                    ModKey = plugin.ModKey,
                    RecordType = recordType,
                    FormKey = MapFormKey(formKey),
                    Keyword = keywordFormKey,
                    KeywordIndex = keywords.Count,
                    ImportedAtUTC = importedAtUTC
                });
            }
        }

        return keywords;
    }

    private static List<ContainerItemDTO> GetContainerItems(PluginDTO plugin, FormKey formKey, object? items)
    {
        if (items is not IEnumerable enumerable) return new List<ContainerItemDTO>();

        var importedAtUTC = DateTime.UtcNow;
        return enumerable
            .Cast<object>()
            .Select((item, itemIndex) => CreateContainerItem(plugin, formKey, item, itemIndex, importedAtUTC))
            .Where(item => item != null)
            .Cast<ContainerItemDTO>()
            .ToList();
    }

    private static ContainerItemDTO? CreateContainerItem(PluginDTO plugin, FormKey formKey, object item, int itemIndex, DateTime importedAtUTC)
    {
        var itemData = GetPropertyValue(item, "Item") ?? item;
        var itemFormKey = GetFormKeyFromObject(GetPropertyValue(itemData, "Item")) ?? GetFormKeyFromObject(itemData);
        if (itemFormKey == null)
        {
            return null;
        }

        return new ContainerItemDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = plugin.ModKey,
            FormKey = MapFormKey(formKey),
            ItemIndex = itemIndex,
            ItemFormKey = itemFormKey,
            Count = GetPropertyNullableInt(item, "Count") ?? GetPropertyNullableInt(itemData, "Count"),
            ImportedAtUTC = importedAtUTC
        };
    }

    /// <summary>
    /// Maps Starfield container actor-value property entries from Mutagen into CreationsForge DTO rows.
    /// </summary>
    /// <param name="plugin">The plugin that owns the source record.</param>
    /// <param name="formKey">The source container form key.</param>
    /// <param name="properties">The reflected Mutagen properties collection.</param>
    /// <returns>The ordered container property DTO rows.</returns>
    private static List<ContainerPropertyDTO> GetContainerProperties(PluginDTO plugin, FormKey formKey, object? properties)
    {
        return properties is not IEnumerable enumerable
            ? new List<ContainerPropertyDTO>()
            : enumerable.Cast<object>().Select((property, propertyIndex) => new ContainerPropertyDTO
            {
                Game = SupportedGame.Starfield,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(formKey),
                PropertyIndex = propertyIndex,
                ActorValue = GetFormKeyFromObject(GetPropertyValue(property, "ActorValue")),
                Value = GetPropertyNullableDouble(property, "Value"),
                ImportedAtUTC = DateTime.UtcNow
            }).ToList();
    }

    private static List<ConstructibleObjectComponentDTO> GetConstructibleObjectComponents(PluginDTO plugin, FormKey formKey, object? components)
    {
        if (components is not IEnumerable enumerable) return new List<ConstructibleObjectComponentDTO>();

        var importedAtUTC = DateTime.UtcNow;
        return enumerable
            .Cast<object>()
            .Select((component, componentIndex) => CreateConstructibleObjectComponent(plugin, formKey, component, componentIndex, importedAtUTC))
            .Where(component => component != null)
            .Cast<ConstructibleObjectComponentDTO>()
            .ToList();
    }

    private static ConstructibleObjectComponentDTO? CreateConstructibleObjectComponent(PluginDTO plugin, FormKey formKey, object component, int componentIndex, DateTime importedAtUTC)
    {
        var componentFormKey = GetFormKeyFromObject(GetPropertyValue(component, "Component")) ?? GetFormKeyFromObject(component);
        if (componentFormKey == null)
        {
            return null;
        }

        return new ConstructibleObjectComponentDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = plugin.ModKey,
            FormKey = MapFormKey(formKey),
            ComponentFormKey = componentFormKey,
            ComponentIndex = componentIndex,
            Count = GetPropertyNullableInt(component, "RequiredCount") ?? GetPropertyNullableInt(component, "Count"),
            ImportedAtUTC = importedAtUTC
        };
    }

    private static List<ConstructibleObjectRecipeFilterDTO> GetConstructibleObjectRecipeFilters(PluginDTO plugin, FormKey formKey, object? recipeFilters)
    {
        if (recipeFilters is not IEnumerable enumerable) return new List<ConstructibleObjectRecipeFilterDTO>();

        var importedAtUTC = DateTime.UtcNow;
        return enumerable
            .Cast<object>()
            .Select((recipeFilter, recipeFilterIndex) => GetFormKeyFromObject(recipeFilter) is { } recipeFilterFormKey
                ? new ConstructibleObjectRecipeFilterDTO
                {
                    Game = SupportedGame.Starfield,
                    ModKey = plugin.ModKey,
                    FormKey = MapFormKey(formKey),
                    RecipeFilterFormKey = recipeFilterFormKey,
                    RecipeFilterIndex = recipeFilterIndex,
                    ImportedAtUTC = importedAtUTC
                }
                : null)
            .Where(recipeFilter => recipeFilter != null)
            .Cast<ConstructibleObjectRecipeFilterDTO>()
            .ToList();
    }

    private static List<ConditionFormConditionDTO> GetConditionRules(PluginDTO plugin, SupportedGame game, FormKey formKey, object? conditions, string conditionSlot = "Conditions")
    {
        if (conditions is not IEnumerable enumerable)
        {
            return new List<ConditionFormConditionDTO>();
        }

        var importedAtUTC = DateTime.UtcNow;
        return enumerable
            .Cast<object>()
            .Select((condition, conditionIndex) =>
            {
                var data = GetPropertyValue(condition, "Data");
                var comparisonValue = GetPropertyValue(condition, "ComparisonValue");
                return new ConditionFormConditionDTO
                {
                    Game = game,
                    ModKey = plugin.ModKey,
                    FormKey = MapFormKey(formKey),
                    ConditionIndex = conditionIndex,
                    ConditionSlot = conditionSlot,
                    MutagenObjectType = condition.GetType().Name,
                    DataMutagenObjectType = data?.GetType().Name,
                    CompareOperator = GetPropertyValue(condition, "CompareOperator")?.ToString(),
                    Flags = FormatEnumerable(GetPropertyValue(condition, "Flags")),
                    Unknown2 = GetPropertyNullableInt(condition, "Unknown2"),
                    ComparisonValue = FormatConditionValue(comparisonValue),
                    ComparisonValueFormKey = GetFormKeyFromObject(comparisonValue),
                    ImportedAtUTC = importedAtUTC,
                    Parameters = GetConditionRuleParameters(plugin, game, formKey, conditionSlot, conditionIndex, data, importedAtUTC)
                };
            })
            .ToList();
    }

    private static List<ConditionFormConditionParameterDTO> GetConditionRuleParameters(
        PluginDTO plugin,
        SupportedGame game,
        FormKey formKey,
        string conditionSlot,
        int conditionIndex,
        object? data,
        DateTime importedAtUTC)
    {
        if (data == null)
        {
            return new List<ConditionFormConditionParameterDTO>();
        }

        return data
            .GetType()
            .GetProperties()
            .Where(property => property.GetIndexParameters().Length == 0)
            .Select(property => new
            {
                property.Name,
                Value = property.GetValue(data)
            })
            .Where(parameter => !string.Equals(parameter.Name, "MutagenObjectType", StringComparison.Ordinal))
            .Select(parameter => new ConditionFormConditionParameterDTO
            {
                Game = game,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(formKey),
                ConditionIndex = conditionIndex,
                ConditionSlot = conditionSlot,
                ParameterName = parameter.Name,
                ParameterValue = FormatConditionValue(parameter.Value),
                ParameterFormKey = GetFormKeyFromObject(parameter.Value),
                ImportedAtUTC = importedAtUTC
            })
            .Where(parameter => !string.IsNullOrWhiteSpace(parameter.ParameterValue) || parameter.ParameterFormKey != null)
            .ToList();
    }

    private static List<TerminalMarkerParameterDTO> GetTerminalMarkerParameters(PluginDTO plugin, FormKey formKey, object? markerParameters)
    {
        if (markerParameters is not IEnumerable enumerable)
        {
            return new List<TerminalMarkerParameterDTO>();
        }

        var importedAtUTC = DateTime.UtcNow;
        return enumerable
            .Cast<object>()
            .Select((parameter, parameterIndex) => new TerminalMarkerParameterDTO
            {
                Game = SupportedGame.Starfield,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(formKey),
                ParameterIndex = parameterIndex,
                Offset = GetPropertyValue(parameter, "Offset")?.ToString(),
                EntryTypes = FormatEnumerable(GetPropertyValue(parameter, "EntryTypes")),
                ExitTypes = FormatEnumerable(GetPropertyValue(parameter, "ExitTypes")),
                ImportedAtUTC = importedAtUTC
            })
            .ToList();
    }

    /// <summary>
    /// Reads Spriggit component <c>REFL</c> payloads from Mutagen component overlays.
    /// </summary>
    /// <param name="plugin">The plugin that owns the parent record.</param>
    /// <param name="recordType">The Bethesda record type identifier for the parent record.</param>
    /// <param name="formKey">The parent record form key.</param>
    /// <param name="components">The Mutagen component collection to inspect.</param>
    /// <returns>Reflection rows for components that expose a non-empty <c>REFL</c> payload.</returns>
    private static List<ReflectionDTO> GetComponentReflections(
        PluginDTO plugin,
        string recordType,
        FormKey formKey,
        object? components)
    {
        var reflections = new List<ReflectionDTO>();
        var importedAtUTC = DateTime.UtcNow;
        if (components is not IEnumerable enumerable)
        {
            return reflections;
        }

        foreach (var component in enumerable.Cast<object>().Select((value, index) => new { value, index }))
        {
            var componentType = component.value.GetType().Name;
            var reflectionValue = FormatHexValue(GetPropertyValue(component.value, "REFL"));
            if (string.IsNullOrWhiteSpace(reflectionValue))
            {
                continue;
            }

            reflections.Add(new ReflectionDTO
            {
                Game = SupportedGame.Starfield,
                ModKey = plugin.ModKey,
                RecordType = recordType,
                FormKey = MapFormKey(formKey),
                ComponentIndex = component.index,
                ComponentType = NormalizeReflectionComponentType(componentType),
                SourcePath = "Components[" + component.index.ToString(System.Globalization.CultureInfo.InvariantCulture) + "].REFL",
                REFL = reflectionValue,
                ImportedAtUTC = importedAtUTC
            });
        }

        return reflections;
    }

    /// <summary>
    /// Converts Mutagen overlay implementation type names to Spriggit component type names.
    /// </summary>
    /// <param name="componentType">The Mutagen component runtime type name.</param>
    /// <returns>The component type name used by Spriggit field coverage.</returns>
    private static string NormalizeReflectionComponentType(string componentType)
    {
        const string binaryOverlaySuffix = "BinaryOverlay";
        return componentType.EndsWith(binaryOverlaySuffix, StringComparison.Ordinal)
            ? componentType[..^binaryOverlaySuffix.Length]
            : componentType;
    }

    private static string? GetAnimationComponentValue(object? components, string propertyName)
    {
        if (components is not IEnumerable enumerable)
        {
            return null;
        }

        foreach (var component in enumerable.Cast<object>())
        {
            if (!string.Equals(component.GetType().Name, "AnimationGraphComponent", StringComparison.Ordinal))
            {
                continue;
            }

            return GetPropertyValue(component, propertyName)?.ToString();
        }

        return null;
    }

    private static List<ScriptFragmentDTO> GetScriptFragments(
        SupportedGame game,
        PluginDTO plugin,
        string recordType,
        FormKey formKey,
        object record)
    {
        var importedAtUTC = DateTime.UtcNow;
        var scriptFragments = GetPropertyValue(GetPropertyValue(record, "VirtualMachineAdapter"), "ScriptFragments");
        return ScriptFragmentDTOMapper.FromScriptFragments(game, plugin.ModKey, recordType, formKey, scriptFragments, importedAtUTC);
    }

    private static List<SoundMappingDTO> GetNamedSounds(PluginDTO plugin, string recordType, FormKey formKey, object record, params string[] soundSlots)
    {
        var importedAtUTC = DateTime.UtcNow;
        return soundSlots
            .Select((soundSlot, soundIndex) => CreateSoundMapping(plugin, recordType, formKey, soundSlot, soundIndex, GetPropertyValue(record, soundSlot), importedAtUTC))
            .Where(sound => sound != null)
            .Cast<SoundMappingDTO>()
            .ToList();
    }

    private static List<SoundMappingDTO> GetIndexedSounds(PluginDTO plugin, string recordType, FormKey formKey, object record)
    {
        var sounds = GetPropertyValue(record, "Sounds") as IEnumerable;
        if (sounds == null) return new List<SoundMappingDTO>();

        var importedAtUTC = DateTime.UtcNow;
        return sounds
            .Cast<object>()
            .Select((sound, soundIndex) => CreateSoundMapping(plugin, recordType, formKey, GetPropertyValue(sound, "Type")?.ToString() ?? $"Sound [{soundIndex}]", soundIndex, sound, importedAtUTC))
            .Where(sound => sound != null)
            .Cast<SoundMappingDTO>()
            .ToList();
    }

    private static SoundMappingDTO? CreateSoundMapping(PluginDTO plugin, string recordType, FormKey formKey, string soundSlot, int soundIndex, object? soundSource, DateTime importedAtUTC)
    {
        if (soundSource == null)
        {
            return null;
        }

        var start = GetSoundStart(soundSource);
        var inheritsSoundsFrom = GetSoundInheritsSoundsFrom(soundSource);
        if (string.IsNullOrWhiteSpace(start) && string.IsNullOrWhiteSpace(inheritsSoundsFrom))
        {
            return null;
        }

        return new SoundMappingDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = plugin.ModKey,
            RecordType = recordType,
            FormKey = MapFormKey(formKey),
            SoundSlot = soundSlot,
            SoundIndex = soundIndex,
            Start = start,
            Stop = GetSoundStop(soundSource),
            MutagenObjectType = GetSoundMutagenObjectType(soundSlot, soundSource),
            InheritsSoundsFrom = inheritsSoundsFrom,
            Versioning = FormatEnumerable(GetPropertyValue(soundSource, "Versioning")),
            Unknown = FormatHexValue(GetPropertyValue(soundSource, "Unknown")),
            ImportedAtUTC = importedAtUTC
        };
    }

    private static string? GetSoundMutagenObjectType(string soundSlot, object soundSource)
    {
        if (!string.Equals(soundSlot, "Sound", StringComparison.OrdinalIgnoreCase) ||
            GetFormKeyFromObject(soundSource) != null)
        {
            return null;
        }

        return NormalizeMutagenObjectTypeName(soundSource.GetType().Name);
    }

    private static string? GetSoundStart(object soundSource)
    {
        var directStart = GetPropertyValue(soundSource, "Start")?.ToString();
        if (!string.IsNullOrWhiteSpace(directStart))
        {
            return directStart;
        }

        var sound = GetPropertyValue(soundSource, "Sound");
        if (sound == null)
        {
            return null;
        }

        if (GetFormKeyFromObject(sound) is { } soundFormKey)
        {
            return $"{soundFormKey.Id:X6}:{soundFormKey.ModKey.FileName}";
        }

        return GetPropertyValue(sound, "Start")?.ToString();
    }

    private static string? GetSoundStop(object soundSource)
    {
        var directStop = GetPropertyValue(soundSource, "Stop")?.ToString();
        if (!string.IsNullOrWhiteSpace(directStop))
        {
            return IsEmptyGuidText(directStop) ? null : directStop;
        }

        var sound = GetPropertyValue(soundSource, "Sound");
        var stop = sound == null ? null : GetPropertyValue(sound, "Stop")?.ToString();
        return IsEmptyGuidText(stop) ? null : stop;
    }

    private static string? GetSoundInheritsSoundsFrom(object soundSource)
    {
        return GetFormKeyFromObject(GetPropertyValue(soundSource, "InheritsSoundsFrom")) is { } formKey
            ? $"{formKey.Id:X6}:{formKey.ModKey.FileName}"
            : null;
    }

    private static string NormalizeMutagenObjectTypeName(string typeName)
    {
        const string binaryOverlaySuffix = "BinaryOverlay";
        return typeName.EndsWith(binaryOverlaySuffix, StringComparison.Ordinal)
            ? typeName[..^binaryOverlaySuffix.Length]
            : typeName;
    }

    private static bool IsEmptyGuidText(string? value)
    {
        return string.Equals(value, "00000000-0000-0000-0000-000000000000", StringComparison.OrdinalIgnoreCase);
    }

    private static object? GetPropertyValue(object? source, string propertyName)
    {
        if (source == null)
        {
            return null;
        }

        var sourceType = source.GetType();
        var property = sourceType.GetProperty(propertyName);
        if (property != null && property.GetIndexParameters().Length == 0)
        {
            return property.GetValue(source);
        }

        foreach (var interfaceType in sourceType.GetInterfaces())
        {
            property = interfaceType.GetProperty(propertyName);
            if (property != null && property.GetIndexParameters().Length == 0)
            {
                return property.GetValue(source);
            }
        }

        return null;
    }

    private static IEnumerable<object> GetRecordCollection(object source, string propertyName)
    {
        return GetPropertyValue(source, propertyName) is IEnumerable enumerable
            ? enumerable.Cast<object>()
            : [];
    }

    private static FormKey GetRequiredFormKey(object record)
    {
        return GetPropertyValue(record, "FormKey") is FormKey formKey
            ? formKey
            : throw new InvalidOperationException($"Record '{record.GetType().Name}' did not expose a FormKey.");
    }

    private static FormKeyDTO? GetFormKeyFromObject(object? value)
    {
        return GetFormKeyFromObject(value, 0);
    }

    private static FormKeyDTO? GetLinkedFormKey(object? source, string propertyName)
    {
        return GetFormKeyFromObject(GetPropertyValue(source, propertyName));
    }

    private static List<FormKeyDTO> GetFormKeys(object? value)
    {
        return (value as IEnumerable)?.Cast<object>()
            .Select(item => GetFormKeyFromObject(item))
            .Where(formKey => formKey != null)
            .Cast<FormKeyDTO>()
            .ToList() ?? new List<FormKeyDTO>();
    }

    private static FormKeyDTO? GetFormKeyFromObject(object? value, int depth)
    {
        if (value == null) return null;
        if (value is FormKey formKey) return MapFormKey(formKey);
        if (value is string) return null;
        if (depth > 2) return null;
        if (GetPropertyValue(value, "IsNull") is bool isNull && isNull) return null;
        if (GetPropertyValue(value, "FormKey") is FormKey linkedFormKey) return MapFormKey(linkedFormKey);
        if (GetPropertyValue(value, "FormKeyNullable") is FormKey nullableFormKey) return MapFormKey(nullableFormKey);
        foreach (var propertyName in new[] { "FormKeyOrIndex", "FormLinkOrIndex", "LinkOrIndex", "FormLinkGetter", "FormLink", "Link", "Value", "Reference", "Target", "Object", "Item" })
        {
            if (GetFormKeyFromObject(GetPropertyValue(value, propertyName), depth + 1) is { } nestedFormKey)
            {
                return nestedFormKey;
            }
        }

        return null;
    }

    private static string? GetPropertyStringOrNull(object? source, string propertyName)
    {
        return GetPropertyValue(source, propertyName)?.ToString();
    }

    private static string? GetNonDefaultFormKeyOrString(object? source, string propertyName)
    {
        return GetNonDefaultString(GetFormKeyOrString(source, propertyName));
    }

    private static string? GetNonDefaultString(object? value)
    {
        var text = value?.ToString();
        return string.IsNullOrWhiteSpace(text) ||
               text.StartsWith("Null<", StringComparison.Ordinal) ||
               string.Equals(text, "0", StringComparison.Ordinal) ||
               string.Equals(text, "0.0", StringComparison.Ordinal) ||
               string.Equals(text, "None", StringComparison.Ordinal) ||
               string.Equals(text, "Self", StringComparison.Ordinal)
            ? null
            : text;
    }

    private static int? GetPropertyNonZeroInt(object? source, string propertyName)
    {
        var value = GetPropertyNullableInt(source, propertyName);
        return value == 0 ? null : value;
    }

    private static long? GetPropertyNonZeroLong(object? source, string propertyName)
    {
        var value = GetPropertyValue(source, propertyName);
        if (value == null)
        {
            return null;
        }

        var longValue = Convert.ToInt64(value, CultureInfo.InvariantCulture);
        return longValue == 0 ? null : longValue;
    }

    /// <summary>
    /// Reads a reflected numeric property and returns <see langword="null" /> when Mutagen exposes the default zero
    /// that Spriggit omits from sparse object fields.
    /// </summary>
    /// <param name="source">The reflected Mutagen object that may expose the property.</param>
    /// <param name="propertyName">The property name to read.</param>
    /// <returns>The non-zero double value, or <see langword="null" /> when the property is absent or zero.</returns>
    private static double? GetPropertyNonZeroDouble(object? source, string propertyName)
    {
        var value = GetPropertyNullableDouble(source, propertyName);
        return value == 0 ? null : value;
    }

    private static float? GetPropertyNonZeroFloat(object? source, string propertyName)
    {
        var value = GetPropertyNullableFloat(source, propertyName);
        return value == 0 ? null : value;
    }

    private static string? GetFormKeyOrString(object? source, string propertyName)
    {
        var value = GetPropertyValue(source, propertyName);
        return FormatFormKeyOrString(value);
    }

    private static string? FormatFormKeyOrString(object? value)
    {
        if (GetFormKeyFromObject(value) is { } formKey)
        {
            return FormatFormKey(formKey);
        }

        var text = value?.ToString();
        return text != null && (text.StartsWith("Null<", StringComparison.Ordinal) || string.Equals(text, "None", StringComparison.Ordinal))
            ? null
            : text;
    }

    private static string FormatFormKey(FormKeyDTO formKey)
    {
        return formKey.Id.ToString("X6", CultureInfo.InvariantCulture) + ":" + formKey.ModKey.FileName;
    }

    private static int? GetPropertyNullableInt(object? source, string propertyName)
    {
        var value = GetPropertyValue(source, propertyName);
        return value == null ? null : Convert.ToInt32(value, CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Reads a nullable signed VMAD alias value from a reflected Mutagen object.
    /// </summary>
    private static short? GetPropertyNullableShort(object? source, string propertyName)
    {
        var value = GetPropertyValue(source, propertyName);
        return value == null ? null : Convert.ToInt16(value, CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Reads a nullable unsigned VMAD object payload value from a reflected Mutagen object.
    /// </summary>
    private static ushort? GetPropertyNullableUShort(object? source, string propertyName)
    {
        var value = GetPropertyValue(source, propertyName);
        return value == null ? null : Convert.ToUInt16(value, CultureInfo.InvariantCulture);
    }

    private static long? GetPropertyNullableLong(object? source, string propertyName)
    {
        var value = GetPropertyValue(source, propertyName);
        return value == null ? null : Convert.ToInt64(value, CultureInfo.InvariantCulture);
    }

    private static double? GetPropertyNullableDouble(object? source, string propertyName)
    {
        var value = GetPropertyValue(source, propertyName);
        return ConvertToNullableDouble(value);
    }

    private static bool? GetPropertyNullableBool(object? source, string propertyName)
    {
        var value = GetPropertyValue(source, propertyName);
        if (value is bool boolValue) return boolValue;
        return value == null ? null : Convert.ToBoolean(value, CultureInfo.InvariantCulture);
    }

    private static int GetEnumerableCount(object? value)
    {
        return value is IEnumerable enumerable ? enumerable.Cast<object>().Count() : 0;
    }

    private static float? GetPropertyNullableFloat(object? source, string propertyName)
    {
        var value = GetPropertyValue(source, propertyName);
        var doubleValue = ConvertToNullableDouble(value);
        return doubleValue == null ? null : Convert.ToSingle(doubleValue.Value, CultureInfo.InvariantCulture);
    }

    private static double? ConvertToNullableDouble(object? value)
    {
        if (value == null)
        {
            return null;
        }

        if (value is IConvertible convertible)
        {
            return convertible.ToDouble(CultureInfo.InvariantCulture);
        }

        foreach (var propertyName in new[] { "Value", "Amount", "Percent", "Normalized" })
        {
            var property = value.GetType().GetProperty(propertyName);
            if (property?.GetValue(value) is IConvertible propertyValue)
            {
                return propertyValue.ToDouble(CultureInfo.InvariantCulture);
            }

            var field = value.GetType().GetField(propertyName);
            if (field?.GetValue(value) is IConvertible fieldValue)
            {
                return fieldValue.ToDouble(CultureInfo.InvariantCulture);
            }
        }

        var text = value.ToString();
        if (!string.IsNullOrWhiteSpace(text) &&
            double.TryParse(text.TrimEnd('%'), NumberStyles.Float, CultureInfo.InvariantCulture, out var parsedValue))
        {
            return parsedValue;
        }

        throw new InvalidCastException(
            "Unable to convert value of type '" + value.GetType().FullName + "' to a numeric value.");
    }

    private static string? FormatObjectBoundsPoint(object? objectBounds, string propertyName)
    {
        return GetPropertyValue(objectBounds, propertyName)?.ToString();
    }

    private static string? FormatEnumerable(object? value)
    {
        if (value is string text)
        {
            return text;
        }

        if (TryFormatFlagObject(value, out var flagText))
        {
            return flagText;
        }

        return value is IEnumerable enumerable
            ? string.Join(", ", enumerable.Cast<object>().Select(item => item.ToString()))
            : value?.ToString();
    }

    private static string? FormatEnumFlagsWithUnknownBits(Enum? value)
    {
        if (value == null)
        {
            return null;
        }

        var remaining = Convert.ToUInt64(value, CultureInfo.InvariantCulture);
        if (remaining == 0)
        {
            return null;
        }

        var enumType = value.GetType();
        var namesByBit = Enum.GetValues(enumType)
            .Cast<Enum>()
            .Select(enumValue => new
            {
                Value = Convert.ToUInt64(enumValue, CultureInfo.InvariantCulture),
                Name = Enum.GetName(enumType, enumValue) ?? enumValue.ToString()
            })
            .Where(item => item.Value != 0 && (item.Value & (item.Value - 1)) == 0)
            .GroupBy(item => item.Value)
            .ToDictionary(group => group.Key, group => group.First().Name);
        var flags = new List<string>();
        var consumed = 0UL;
        for (var bit = 0; bit < 64; bit++)
        {
            var flag = 1UL << bit;
            if ((remaining & flag) == 0)
            {
                continue;
            }

            flags.Add(namesByBit.TryGetValue(flag, out var flagName)
                ? flagName
                : "0x" + flag.ToString("X", CultureInfo.InvariantCulture));
            consumed |= flag;
        }

        remaining &= ~consumed;
        foreach (var enumValue in Enum.GetValues(enumType).Cast<Enum>())
        {
            var numericValue = Convert.ToUInt64(enumValue, CultureInfo.InvariantCulture);
            if (numericValue == 0 || (numericValue & (numericValue - 1)) != 0 || (remaining & numericValue) != numericValue)
            {
                continue;
            }

            flags.Add(Enum.GetName(enumType, enumValue) ?? enumValue.ToString());
            remaining &= ~numericValue;
        }

        return flags.Count == 0 ? value.ToString() : string.Join(", ", flags);
    }

    private static string? FormatFactionFlags(object? value)
    {
        if (value == null)
        {
            return null;
        }

        if (!TryConvertToUInt64(value, out var numericValue))
        {
            return FormatEnumerable(value);
        }

        if (numericValue == 0)
        {
            return null;
        }

        var flags = new List<string>();
        AddFactionFlag(flags, ref numericValue, 0x00001, "HiddenFromPC");
        AddFactionFlag(flags, ref numericValue, 0x00002, "SpecialCombat");
        AddFactionFlag(flags, ref numericValue, 0x00040, "TrackCrime");
        AddFactionFlag(flags, ref numericValue, 0x00080, "IgnoreMurder");
        AddFactionFlag(flags, ref numericValue, 0x00100, "IgnoreAssault");
        AddFactionFlag(flags, ref numericValue, 0x00200, "IgnoreStealing");
        AddFactionFlag(flags, ref numericValue, 0x00400, "IgnoreTrespass");
        AddFactionFlag(flags, ref numericValue, 0x00800, "DoNotReportCrimesAgainstMembers");
        AddFactionFlag(flags, ref numericValue, 0x01000, "CrimeGoldUseDefaults");
        AddFactionFlag(flags, ref numericValue, 0x02000, "IgnorePickpocket");
        AddFactionFlag(flags, ref numericValue, 0x04000, "Vendor");
        AddFactionFlag(flags, ref numericValue, 0x08000, "CanBeOwner");
        AddFactionFlag(flags, ref numericValue, 0x10000, "IgnorePiracy");
        AddFactionFlag(flags, ref numericValue, 0x20000, "IgnoreSmuggling");

        for (var bit = 0; bit < 64; bit++)
        {
            var flag = 1UL << bit;
            if ((numericValue & flag) != 0)
            {
                flags.Add("0x" + flag.ToString("X", CultureInfo.InvariantCulture));
            }
        }

        return string.Join(", ", flags);
    }

    private static void AddFactionFlag(List<string> flags, ref ulong value, ulong flag, string name)
    {
        if ((value & flag) == 0)
        {
            return;
        }

        flags.Add(name);
        value &= ~flag;
    }

    private static string GetSpriggitMutagenObjectType(object record)
    {
        var typeName = record.GetType().Name;
        const string binaryOverlaySuffix = "BinaryOverlay";
        return typeName.EndsWith(binaryOverlaySuffix, StringComparison.Ordinal)
            ? typeName[..^binaryOverlaySuffix.Length]
            : typeName;
    }

    private static bool TryFormatFlagObject(object? value, out string? flagText)
    {
        flagText = null;
        if (value == null || value is string || value is IEnumerable || value.GetType().IsEnum || value.GetType().IsPrimitive)
        {
            return false;
        }

        var type = value.GetType();
        if (!type.Name.Contains("Flag", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var flags = new List<string>();
        foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(property => property.GetIndexParameters().Length == 0))
        {
            var propertyValue = property.GetValue(value);
            if (propertyValue is bool boolValue)
            {
                if (boolValue)
                {
                    flags.Add(property.Name);
                }

                continue;
            }

            if (propertyValue is Enum enumValue && Convert.ToUInt64(enumValue, CultureInfo.InvariantCulture) != 0)
            {
                flags.Add(enumValue.ToString());
                continue;
            }

            if (propertyValue is IEnumerable enumerableValue and not string)
            {
                foreach (var item in enumerableValue.Cast<object>().Select(item => item.ToString()).Where(item => !string.IsNullOrWhiteSpace(item)))
                {
                    flags.Add(item!);
                }
            }
        }

        flagText = flags.Count == 0 ? null : string.Join(", ", flags);
        return true;
    }

    private static string? FormatMajorFlags(object? value)
    {
        if (value == null)
        {
            return null;
        }

        if (value is IEnumerable enumerable && value is not string)
        {
            var flags = enumerable
                .Cast<object>()
                .Select(FormatSingleMajorFlag)
                .Where(flag => !string.IsNullOrWhiteSpace(flag))
                .ToList();
            return flags.Count == 0 ? null : string.Join(", ", flags);
        }

        if (!TryConvertToUInt64(value, out var numericValue) || numericValue == 0)
        {
            return null;
        }

        return FormatMajorFlagBits(numericValue);
    }

    private static string? FormatMajorFlagBits(ulong value)
    {
        var flags = new List<string>();
        for (var bit = 0; bit < 64; bit++)
        {
            var flag = 1UL << bit;
            if ((value & flag) != 0)
            {
                flags.Add("0x" + flag.ToString("X8", CultureInfo.InvariantCulture));
            }
        }

        return flags.Count == 0 ? null : string.Join(", ", flags);
    }

    private static string? FormatSingleMajorFlag(object value)
    {
        if (!TryConvertToUInt64(value, out var numericValue) || numericValue == 0)
        {
            return null;
        }

        return "0x" + numericValue.ToString("X8", CultureInfo.InvariantCulture);
    }

    private static bool TryConvertToUInt64(object value, out ulong result)
    {
        if (value is string text)
        {
            if (text.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                return ulong.TryParse(text[2..], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out result);
            }

            return ulong.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out result);
        }

        try
        {
            result = Convert.ToUInt64(value, CultureInfo.InvariantCulture);
            return true;
        }
        catch (InvalidCastException)
        {
            result = 0;
            return false;
        }
        catch (FormatException)
        {
            result = 0;
            return false;
        }
        catch (OverflowException)
        {
            result = 0;
            return false;
        }
    }

    private static string FormatSpriggitColor(object? value)
    {
        var text = value?.ToString() ?? string.Empty;
        if (text.Length == 9 &&
            text[0] == '#' &&
            byte.TryParse(text.AsSpan(1, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var alpha) &&
            byte.TryParse(text.AsSpan(3, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var red) &&
            byte.TryParse(text.AsSpan(5, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var green) &&
            byte.TryParse(text.AsSpan(7, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var blue))
        {
            return $"Color [A={alpha}, R={red}, G={green}, B={blue}]";
        }

        return text;
    }

    private static string? FormatHexValue(object? value)
    {
        if (value == null)
        {
            return null;
        }

        if (value is string text)
        {
            return text;
        }

        if (value is byte[] bytes)
        {
            return Convert.ToHexString(bytes);
        }

        var toArray = value.GetType().GetMethod("ToArray", Type.EmptyTypes);
        if (toArray?.Invoke(value, null) is byte[] arrayBytes)
        {
            return Convert.ToHexString(arrayBytes);
        }

        return value.ToString();
    }

    private static string? FormatReflectionPayload(object? value, int depth = 0)
    {
        if (value == null) return null;
        if (depth > 3) return value.GetType().Name;
        if (GetFormKeyFromObject(value) is { } formKey) return $"{formKey.ModKey.FileName}:{formKey.Id:X8}";
        if (value is string text) return text;
        if (value is byte[] bytes) return Convert.ToHexString(bytes);

        var type = value.GetType();
        if (type.IsPrimitive || value is decimal || value is DateTime || value is Guid || type.IsEnum)
        {
            return Convert.ToString(value, CultureInfo.InvariantCulture);
        }

        if (value is IEnumerable enumerable)
        {
            return string.Join(
                "; ",
                enumerable.Cast<object>()
                    .Select((item, index) => "[" + index.ToString(CultureInfo.InvariantCulture) + "]=" + (FormatReflectionPayload(item, depth + 1) ?? string.Empty)));
        }

        var parts = new List<string>();
        foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                     .Where(property => property.GetIndexParameters().Length == 0)
                     .OrderBy(property => property.Name, StringComparer.Ordinal))
        {
            object? propertyValue;
            try
            {
                propertyValue = property.GetValue(value);
            }
            catch (TargetInvocationException)
            {
                continue;
            }
            catch (TargetParameterCountException)
            {
                continue;
            }

            var formattedValue = FormatReflectionPayload(propertyValue, depth + 1);
            if (!string.IsNullOrWhiteSpace(formattedValue))
            {
                parts.Add(property.Name + "=" + formattedValue);
            }
        }

        return parts.Count == 0 ? value.ToString() : string.Join("; ", parts);
    }

    private static string? GetFirstComponentHexValue(object record, string propertyName)
    {
        foreach (var component in GetComponentObjects(record))
        {
            var value = GetPropertyValue(component, propertyName);
            if (value != null)
            {
                return FormatSpriggitHexValue(value);
            }
        }

        return null;
    }

    private static IEnumerable<object> GetComponentObjects(object record)
    {
        var directComponents = (GetPropertyValue(record, "Components") as IEnumerable)?.Cast<object>().ToList();
        if (directComponents != null)
        {
            foreach (var component in directComponents)
            {
                yield return component;
            }
        }

        var nestedComponents = new List<object>();
        AddMatchingChildObjects(
            nestedComponents,
            record,
            item => item.GetType().Name.Contains("Component", StringComparison.OrdinalIgnoreCase) ||
                    GetPropertyValue(item, "WAIM") != null ||
                    GetPropertyValue(item, "WFIR") != null,
            0);

        foreach (var component in nestedComponents)
        {
            if (directComponents == null || directComponents.All(directComponent => !ReferenceEquals(directComponent, component)))
            {
                yield return component;
            }
        }
    }

    private static IReadOnlyList<object> GetChildObjects(object record, string preferredPropertyName, string linkPropertyName)
    {
        var objects = new List<object>();
        AddEnumerablePropertyObjects(objects, GetPropertyValue(record, preferredPropertyName));
        if (objects.Count > 0)
        {
            return objects;
        }

        AddMatchingChildObjects(objects, record, item => GetPropertyValue(item, linkPropertyName) != null && GetPropertyValue(item, "Count") != null, 0);

        return objects;
    }

    private static void AddMatchingChildObjects(ICollection<object> objects, object? source, Func<object, bool> isMatch, int depth)
    {
        if (source == null || source is string || IsSimpleReflectionValue(source) || depth > 3)
        {
            return;
        }

        foreach (var item in GetEnumerableObjects(source))
        {
            AddMatchingChildObject(objects, item, isMatch, depth);
        }

        foreach (var property in source.GetType().GetProperties())
        {
            if (property.GetIndexParameters().Length > 0)
            {
                continue;
            }

            object? value;
            try
            {
                value = property.GetValue(source);
            }
            catch (TargetInvocationException)
            {
                continue;
            }

            foreach (var item in GetEnumerableObjects(value))
            {
                AddMatchingChildObject(objects, item, isMatch, depth + 1);
            }
        }
    }

    private static void AddMatchingChildObject(ICollection<object> objects, object item, Func<object, bool> isMatch, int depth)
    {
        if (IsSimpleReflectionValue(item))
        {
            return;
        }

        if (isMatch(item))
        {
            objects.Add(item);
            return;
        }

        AddMatchingChildObjects(objects, item, isMatch, depth + 1);
    }

    private static void AddEnumerablePropertyObjects(ICollection<object> objects, object? value)
    {
        foreach (var item in GetEnumerableObjects(value))
        {
            objects.Add(item);
        }
    }

    private static IEnumerable<object> GetEnumerableObjects(object? value)
    {
        return value is IEnumerable enumerable && value is not string
            ? enumerable.Cast<object>()
            : [];
    }

    private static bool IsSimpleReflectionValue(object value)
    {
        var type = value.GetType();
        return type.IsPrimitive ||
               type.IsEnum ||
               type == typeof(decimal) ||
               type == typeof(DateTime) ||
               type == typeof(Guid) ||
               type == typeof(FormKey) ||
               type == typeof(ModKey);
    }

    private static string? FormatSpriggitHexValue(object? value)
    {
        var hexValue = FormatHexValue(value)?.Trim();
        if (string.IsNullOrWhiteSpace(hexValue) || hexValue.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
        {
            return hexValue;
        }

        return "0x" + hexValue;
    }

    private static string? FormatConditionValue(object? value)
    {
        if (value == null)
        {
            return null;
        }

        var formKey = GetFormKeyFromObject(value);
        if (formKey != null)
        {
            return $"{formKey.ModKey.FileName}:{formKey.Id:X8}";
        }

        return value is IConvertible convertible
            ? Convert.ToString(convertible, CultureInfo.InvariantCulture)
            : FormatEnumerable(value);
    }

    private static string? GetMagicEffectArchetype(IMagicEffectGetter record)
    {
        var type = record.Archetype?.Type.ToString();
        return string.Equals(type, "ValueModifier", StringComparison.Ordinal) ||
               string.Equals(type, "PeakValueModifier", StringComparison.Ordinal)
            ? null
            : type;
    }

    private static List<ScriptingAdapterPropertyDTO> GetScriptingAdapterProperties(PluginDTO plugin, string recordType, FormKey formKey, IScriptEntryGetter script, DateTime importedAtUTC)
    {
        return script.Properties
            .OrderBy(property => property.Name, StringComparer.OrdinalIgnoreCase)
            .Select((property, propertyIndex) => CreateScriptingAdapterProperty(plugin, recordType, formKey, script.Name, property, propertyIndex, importedAtUTC))
            .Where(property => property != null)
            .Cast<ScriptingAdapterPropertyDTO>()
            .ToList();
    }

    private static ScriptingAdapterPropertyDTO? CreateScriptingAdapterProperty(PluginDTO plugin, string recordType, FormKey formKey, string scriptName, IScriptPropertyGetter property, int propertyIndex, DateTime importedAtUTC)
    {
        var dto = new ScriptingAdapterPropertyDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = plugin.ModKey,
            RecordType = recordType,
            FormKey = MapFormKey(formKey),
            ScriptingAdapterName = scriptName,
            PropertyIndex = propertyIndex,
            Name = property.Name,
            MutagenObjectType = property.GetType().Name,
            ImportedAtUTC = importedAtUTC
        };

        switch (property)
        {
            case ScriptBoolProperty boolProperty:
                dto.DataBool = boolProperty.Data;
                return dto;
            case ScriptIntProperty intProperty:
                dto.DataInt = intProperty.Data;
                return dto;
            case ScriptFloatProperty floatProperty:
                dto.DataFloat = floatProperty.Data;
                return dto;
            case ScriptStringProperty stringProperty:
                dto.DataString = stringProperty.Data;
                return dto;
            case ScriptObjectProperty objectProperty:
                dto.ObjectFormKey = objectProperty.Object.FormKeyNullable is { } objectFormKey ? MapFormKey(objectFormKey) : null;
                dto.ObjectAlias = objectProperty.Alias;
                dto.ObjectUnused = objectProperty.Unused;
                return dto;
            case ScriptBoolListProperty boolListProperty:
                dto.ListItems = boolListProperty.Data.Select((value, listItemIndex) => CreateScriptingAdapterPropertyListItem(plugin, recordType, formKey, scriptName, propertyIndex, listItemIndex, nameof(ScriptBoolProperty), importedAtUTC, dataBool: value)).ToList();
                return dto;
            case ScriptIntListProperty intListProperty:
                dto.ListItems = intListProperty.Data.Select((value, listItemIndex) => CreateScriptingAdapterPropertyListItem(plugin, recordType, formKey, scriptName, propertyIndex, listItemIndex, nameof(ScriptIntProperty), importedAtUTC, dataInt: value)).ToList();
                return dto;
            case ScriptFloatListProperty floatListProperty:
                dto.ListItems = floatListProperty.Data.Select((value, listItemIndex) => CreateScriptingAdapterPropertyListItem(plugin, recordType, formKey, scriptName, propertyIndex, listItemIndex, nameof(ScriptFloatProperty), importedAtUTC, dataFloat: value)).ToList();
                return dto;
            case ScriptStringListProperty stringListProperty:
                dto.ListItems = stringListProperty.Data.Select((value, listItemIndex) => CreateScriptingAdapterPropertyListItem(plugin, recordType, formKey, scriptName, propertyIndex, listItemIndex, nameof(ScriptStringProperty), importedAtUTC, dataString: value)).ToList();
                return dto;
            case ScriptObjectListProperty objectListProperty:
                dto.ListItems = objectListProperty.Objects.Select((value, listItemIndex) => CreateScriptingAdapterPropertyListItem(plugin, recordType, formKey, scriptName, propertyIndex, listItemIndex, nameof(ScriptObjectProperty), importedAtUTC, name: GetPropertyStringOrNull(value, "Name"), objectFormKey: value.Object.FormKeyNullable is { } objectFormKey ? MapFormKey(objectFormKey) : null, objectAlias: value.Alias, objectUnused: value.Unused)).ToList();
                return dto;
            case ScriptProperty when property.GetType().Name.Contains("StructList", StringComparison.OrdinalIgnoreCase):
                dto.Structs = GetScriptingAdapterPropertyStructs(plugin, recordType, formKey, scriptName, propertyIndex, property, importedAtUTC);
                return dto;
            case ScriptProperty:
                return dto;
            default:
                return null;
        }
    }

    private static ScriptingAdapterPropertyListItemDTO CreateScriptingAdapterPropertyListItem(
        PluginDTO plugin,
        string recordType,
        FormKey formKey,
        string scriptName,
        int propertyIndex,
        int listItemIndex,
        string mutagenObjectType,
        DateTime importedAtUTC,
        string? name = null,
        bool? dataBool = null,
        int? dataInt = null,
        double? dataFloat = null,
        string? dataString = null,
        FormKeyDTO? objectFormKey = null,
        short? objectAlias = null,
        ushort? objectUnused = null)
    {
        return new ScriptingAdapterPropertyListItemDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = plugin.ModKey,
            RecordType = recordType,
            FormKey = MapFormKey(formKey),
            ScriptingAdapterName = scriptName,
            PropertyIndex = propertyIndex,
            ListItemIndex = listItemIndex,
            Name = name,
            MutagenObjectType = mutagenObjectType,
            DataBool = dataBool,
            DataInt = dataInt,
            DataFloat = dataFloat,
            DataString = dataString,
            ObjectFormKey = objectFormKey,
            ObjectAlias = objectAlias,
            ObjectUnused = objectUnused,
            ImportedAtUTC = importedAtUTC
        };
    }

    /// <summary>
    /// Maps a VMAD script property struct list to first-class DTO rows.
    /// </summary>
    private static List<ScriptingAdapterPropertyStructDTO> GetScriptingAdapterPropertyStructs(
        PluginDTO plugin,
        string recordType,
        FormKey formKey,
        string scriptName,
        int propertyIndex,
        object property,
        DateTime importedAtUTC)
    {
        var structs = GetPropertyValue(property, "Structs") as IEnumerable;
        if (structs == null)
        {
            return new List<ScriptingAdapterPropertyStructDTO>();
        }

        return structs
            .Cast<object>()
            .Select((value, structIndex) => new ScriptingAdapterPropertyStructDTO
            {
                Game = SupportedGame.Starfield,
                ModKey = plugin.ModKey,
                RecordType = recordType,
                FormKey = MapFormKey(formKey),
                ScriptingAdapterName = scriptName,
                PropertyIndex = propertyIndex,
                StructIndex = structIndex,
                ImportedAtUTC = importedAtUTC,
                Members = GetScriptingAdapterPropertyStructMembers(plugin, recordType, formKey, scriptName, propertyIndex, structIndex, value, importedAtUTC)
            })
            .ToList();
    }

    /// <summary>
    /// Maps VMAD script struct members to first-class DTO rows.
    /// </summary>
    private static List<ScriptingAdapterPropertyStructMemberDTO> GetScriptingAdapterPropertyStructMembers(
        PluginDTO plugin,
        string recordType,
        FormKey formKey,
        string scriptName,
        int propertyIndex,
        int structIndex,
        object propertyStruct,
        DateTime importedAtUTC)
    {
        var members = GetPropertyValue(propertyStruct, "Members") as IEnumerable;
        if (members == null)
        {
            return new List<ScriptingAdapterPropertyStructMemberDTO>();
        }

        return members
            .Cast<object>()
            .Select((member, memberIndex) => CreateScriptingAdapterPropertyStructMember(
                plugin,
                recordType,
                formKey,
                scriptName,
                propertyIndex,
                structIndex,
                memberIndex,
                member,
                importedAtUTC))
            .ToList();
    }

    /// <summary>
    /// Maps a VMAD script struct member value to a first-class DTO row.
    /// </summary>
    private static ScriptingAdapterPropertyStructMemberDTO CreateScriptingAdapterPropertyStructMember(
        PluginDTO plugin,
        string recordType,
        FormKey formKey,
        string scriptName,
        int propertyIndex,
        int structIndex,
        int memberIndex,
        object member,
        DateTime importedAtUTC)
    {
        var data = GetPropertyValue(member, "Data");
        var objectValue = GetPropertyValue(member, "Object");
        return new ScriptingAdapterPropertyStructMemberDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = plugin.ModKey,
            RecordType = recordType,
            FormKey = MapFormKey(formKey),
            ScriptingAdapterName = scriptName,
            PropertyIndex = propertyIndex,
            StructIndex = structIndex,
            MemberIndex = memberIndex,
            Name = GetPropertyStringOrNull(member, "Name") ?? string.Empty,
            MutagenObjectType = member.GetType().Name,
            DataBool = data is bool boolValue ? boolValue : null,
            DataInt = data is int intValue ? intValue : null,
            DataFloat = data is float or double or decimal ? Convert.ToDouble(data, CultureInfo.InvariantCulture) : null,
            DataString = data is string stringValue ? stringValue : null,
            ObjectFormKey = GetFormKeyFromObject(objectValue),
            ObjectAlias = GetPropertyNullableShort(member, "Alias"),
            ObjectUnused = GetPropertyNullableUShort(member, "Unused"),
            ImportedAtUTC = importedAtUTC
        };
    }

    protected virtual IStarfieldModGetter LoadMod(PluginDTO plugin)
    {
        return StarfieldModConstruction.Load(plugin.ModKey);
    }

    private string GetDataFolderPath()
    {
        return StarfieldModConstruction.GetDataFolderPath();
    }

    private static FormKeyDTO MapFormKey(FormKey formKey)
    {
        return new FormKeyDTO
        {
            ModKey = ModKeyDTOMapper.FromModKey(formKey.ModKey),
            Id = formKey.ID
        };
    }

    private static GameSettingDataType GetGameSettingDataType(IGameSettingGetter record)
    {
        return record switch
        {
            IGameSettingBoolGetter => GameSettingDataType.Boolean,
            IGameSettingFloatGetter => GameSettingDataType.Float,
            IGameSettingIntGetter => GameSettingDataType.Integer,
            IGameSettingStringGetter => GameSettingDataType.String,
            IGameSettingUIntGetter => GameSettingDataType.UnsignedInteger,
            _ => throw new NotSupportedException($"Unsupported game setting type '{record.GetType().Name}'.")
        };
    }

    private static GameSettingDataDTO GetGameSettingData(IGameSettingGetter record)
    {
        var dataType = GetGameSettingDataType(record);
        return record switch
        {
            IGameSettingBoolGetter gameSetting => new GameSettingDataDTO { DataType = dataType, Boolean = gameSetting.Data },
            IGameSettingFloatGetter gameSetting => new GameSettingDataDTO { DataType = dataType, Float = gameSetting.Data },
            IGameSettingIntGetter gameSetting => new GameSettingDataDTO { DataType = dataType, Integer = gameSetting.Data },
            IGameSettingStringGetter gameSetting => new GameSettingDataDTO { DataType = dataType, String = LocalizedStringDTOMapper.ToTranslatedStringDTO(gameSetting.Data) },
            IGameSettingUIntGetter gameSetting => new GameSettingDataDTO { DataType = dataType, UnsignedInteger = gameSetting.Data },
            _ => new GameSettingDataDTO { DataType = dataType }
        };
    }
}
