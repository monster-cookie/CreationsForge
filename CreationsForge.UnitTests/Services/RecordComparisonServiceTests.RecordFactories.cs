using CreationsForge.Core.DTOs.Games;
using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Models.Configuration;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services;
using CreationsForge.Core.Services.Interfaces;
using CreationsForge.Specification.Records;
using Mutagen.Bethesda.Strings;
using Shouldly;

namespace CreationsForge.UnitTests.Services;

/// <summary>
/// Contains record DTO factory helpers for record comparison tests.
/// </summary>
public partial class RecordComparisonServiceTests
{
    private static NPCDTO CreateNPC(string fileName, FormKeyDTO formKey, double heightMin, double heightMax)
    {
        return new NPCDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey(fileName),
            FormKey = formKey,
            EditorID = "TestNPC",
            FormVersion = 1,
            MajorRecordFlags = 0,
            ImportedAtUTC = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Aggression = "Unaggressive",
            Confidence = "Average",
            Responsibility = "NoCrime",
            Assistance = "HelpsNobody",
            HeightMin = heightMin,
            HeightMax = heightMax
        };
    }

    private static GlobalDTO CreateGlobal(
        string fileName,
        FormKeyDTO formKey,
        string editorID,
        double data,
        string? mutagenObjectType = null,
        string? majorFlags = null)
    {
        return new GlobalDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey(fileName),
            FormKey = formKey,
            EditorID = editorID,
            FormVersion = 1,
            MajorRecordFlags = 2,
            ImportedAtUTC = DateTime.UtcNow,
            MutagenObjectType = mutagenObjectType,
            MajorFlags = majorFlags,
            Data = data
        };
    }

    private static GameSettingDTO CreateGameSetting(
        string fileName,
        FormKeyDTO formKey,
        string editorID,
        GameSettingDataType dataType,
        string? stringData = null,
        double? floatData = null)
    {
        return new GameSettingDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey(fileName),
            FormKey = formKey,
            EditorID = editorID,
            FormVersion = 1,
            MajorRecordFlags = 2,
            ImportedAtUTC = DateTime.UtcNow,
            DataType = dataType,
            Data = new GameSettingDataDTO
            {
                DataType = dataType,
                String = dataType == GameSettingDataType.String ? Text(stringData ?? string.Empty) : null,
                Float = dataType == GameSettingDataType.Float ? floatData : null
            }
        };
    }

    private static LocalizedStringDTO CreateLocalizedString(
        string fileName,
        FormKeyDTO formKey,
        string sourceField,
        string language,
        string value)
    {
        return new LocalizedStringDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey(fileName),
            RecordType = RecordTypeCatalog.GameSetting.RecordID,
            FormKey = formKey,
            SourceField = sourceField,
            Language = language,
            Value = value,
            ImportedAtUTC = DateTime.UtcNow
        };
    }

    private static FormListDTO CreateFormList(string fileName, FormKeyDTO formKey, IList<FormListItemDTO> items)
    {
        return new FormListDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey(fileName),
            FormKey = formKey,
            EditorID = "MyFormList",
            FormVersion = 1,
            MajorRecordFlags = 2,
            ImportedAtUTC = DateTime.UtcNow,
            Items = items
        };
    }

    private static FormListItemDTO CreateFormListItem(string fileName, FormKeyDTO formKey, FormKeyDTO itemFormKey, int itemIndex)
    {
        return new FormListItemDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey(fileName),
            FormKey = formKey,
            Item = itemFormKey,
            ItemIndex = itemIndex,
            ImportedAtUTC = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Creates a minimal Class record for comparison-service tests that exercise scalar metadata dispatch.
    /// </summary>
    /// <param name="fileName">The plugin file name that contributed the test record.</param>
    /// <param name="formKey">The origin FormKey shared by compared records.</param>
    /// <param name="name">The translated class name to place on the DTO.</param>
    /// <param name="description">The translated class description to place on the DTO.</param>
    /// <param name="teaches">The class teaching target value to place on the DTO.</param>
    /// <param name="maxTrainingLevel">The maximum training level to place on the DTO.</param>
    /// <param name="bleedoutDefault">The bleedout default value to place on the DTO.</param>
    /// <param name="voicePoints">The voice-points value to place on the DTO.</param>
    /// <param name="unknown">The first unknown scalar value to place on the DTO.</param>
    /// <param name="unknown2">The second unknown scalar value to place on the DTO.</param>
    /// <returns>The populated Class DTO.</returns>
    private static ClassDTO CreateClass(
        string fileName,
        FormKeyDTO formKey,
        string name,
        string description,
        string teaches,
        int maxTrainingLevel,
        double bleedoutDefault,
        double voicePoints,
        double unknown,
        double unknown2)
    {
        return new ClassDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey(fileName),
            FormKey = formKey,
            EditorID = "MyClass",
            FormVersion = 1,
            MajorRecordFlags = 2,
            Version2 = 3,
            ImportedAtUTC = DateTime.UtcNow,
            Name = Text(name),
            Description = Text(description),
            Teaches = teaches,
            MaxTrainingLevel = maxTrainingLevel,
            BleedoutDefault = bleedoutDefault,
            VoicePoints = voicePoints,
            Unknown = unknown,
            Unknown2 = unknown2
        };
    }

    /// <summary>
    /// Creates a Class property row for comparison-service tests that exercise strategy-owned child groups.
    /// </summary>
    /// <param name="fileName">The plugin file name that contributed the child row.</param>
    /// <param name="formKey">The parent Class FormKey.</param>
    /// <param name="actorValueFormKey">The actor value FormKey referenced by the property.</param>
    /// <param name="propertyIndex">The stable property index used for cross-plugin alignment.</param>
    /// <param name="value">The property numeric value to place on the DTO.</param>
    /// <returns>The populated Class property DTO.</returns>
    private static ClassPropertyDTO CreateClassProperty(
        string fileName,
        FormKeyDTO formKey,
        FormKeyDTO actorValueFormKey,
        int propertyIndex,
        double value)
    {
        return new ClassPropertyDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey(fileName),
            FormKey = formKey,
            PropertyIndex = propertyIndex,
            ActorValueFormKey = actorValueFormKey,
            Value = value,
            ImportedAtUTC = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Creates a Class weight row for comparison-service tests that exercise strategy-owned child groups.
    /// </summary>
    /// <param name="fileName">The plugin file name that contributed the child row.</param>
    /// <param name="formKey">The parent Class FormKey.</param>
    /// <param name="weightType">The class weight type, such as Skill or Stat.</param>
    /// <param name="weightIndex">The stable weight index used for cross-plugin alignment.</param>
    /// <param name="key">The class weight key to place on the DTO.</param>
    /// <param name="value">The class weight numeric value to place on the DTO.</param>
    /// <returns>The populated Class weight DTO.</returns>
    private static ClassWeightDTO CreateClassWeight(
        string fileName,
        FormKeyDTO formKey,
        string weightType,
        int weightIndex,
        string key,
        double value)
    {
        return new ClassWeightDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey(fileName),
            FormKey = formKey,
            WeightType = weightType,
            WeightIndex = weightIndex,
            Key = key,
            Value = value,
            ImportedAtUTC = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Creates a minimal Faction record for comparison-service tests that exercise scalar metadata dispatch.
    /// </summary>
    /// <param name="fileName">The plugin file name that contributed the test record.</param>
    /// <param name="formKey">The origin FormKey shared by compared records.</param>
    /// <param name="name">The translated faction name to place on the DTO.</param>
    /// <param name="flags">The faction flags text to place on the DTO.</param>
    /// <param name="formationRadius">The formation radius value to place on the DTO.</param>
    /// <param name="keyword">The keyword FormKey used for scalar and vendor-location references.</param>
    /// <returns>The populated Faction DTO.</returns>
    private static FactionDTO CreateFaction(
        string fileName,
        FormKeyDTO formKey,
        string name,
        string flags,
        double formationRadius,
        FormKeyDTO keyword)
    {
        return new FactionDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey(fileName),
            FormKey = formKey,
            EditorID = "MyFaction",
            FormVersion = 1,
            MajorRecordFlags = 2,
            Version2 = 3,
            ImportedAtUTC = DateTime.UtcNow,
            Name = Text(name),
            Flags = flags,
            FormationRadius = formationRadius,
            Keyword = keyword,
            Herd = keyword,
            VoiceType = keyword,
            SharedCrimeFactionList = keyword,
            VendorBuySellList = keyword,
            MerchantContainer = keyword,
            ExteriorJailMarker = keyword,
            FollowerWaitMarker = keyword,
            StolenGoodsContainer = keyword,
            PlayerInventoryContainer = keyword,
            JailOutfit = keyword,
            CrimeValues = new FactionDTO.CrimeValuesDTO
            {
                Arrest = true,
                AttackOnSight = false,
                Murder = (int)(formationRadius / 10),
                Assault = 20,
                Trespass = 30,
                Pickpocket = 40,
                Steal = 50,
                StealMult = 1.5,
                StealMultiplier = 2.5,
                Escape = 60,
                Werewolf = 70,
                WerewolfUnused = 80,
                Unknown = 90,
                Piracy = 100,
                SmuggleMultiplier = 3.5
            },
            VendorValues = new FactionDTO.VendorValuesDTO
            {
                StartHour = formationRadius / 50 + 6,
                EndHour = 18,
                Radius = 512,
                BuysStolenItems = true,
                BuysNonStolenItems = false,
                BuySellEverythingNotInList = true
            },
            VendorLocation = new FactionDTO.VendorLocationDTO
            {
                MutagenObjectType = "FactionVendorLocation",
                Target = new FactionDTO.VendorLocationTargetDTO
                {
                    MutagenObjectType = "ConditionTarget",
                    Type = "LinkedReference",
                    Link = keyword
                }
            }
        };
    }

    /// <summary>
    /// Adds representative strategy-owned child rows to a Faction DTO used by comparison-service tests.
    /// </summary>
    /// <param name="faction">The faction DTO that should receive child rows.</param>
    /// <param name="fileName">The plugin file name that contributed the child rows.</param>
    /// <param name="formKey">The parent Faction FormKey.</param>
    /// <param name="reaction">The relation reaction text to place on the child row.</param>
    /// <param name="rankTitle">The rank title text to place on the child row.</param>
    /// <param name="rankNumber">The rank number to place on the child row.</param>
    /// <param name="componentValue">The component item numeric value to place on the child row.</param>
    private static void AddFactionChildren(
        FactionDTO faction,
        string fileName,
        FormKeyDTO formKey,
        string reaction,
        string rankTitle,
        int rankNumber,
        double componentValue)
    {
        var childFormKey = CreateFormKey("Starfield.esm", 0x302);
        faction.Relations.Add(new FactionRelationDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey(fileName),
            FormKey = formKey,
            RelationIndex = 0,
            Target = childFormKey,
            Reaction = reaction,
            ImportedAtUTC = DateTime.UtcNow
        });
        faction.Ranks.Add(new FactionRankDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey(fileName),
            FormKey = formKey,
            RankIndex = 0,
            Number = rankNumber,
            Title = new FactionRankDTO.TitleDTO
            {
                Male = Text(rankTitle),
                Female = Text(rankTitle)
            },
            ImportedAtUTC = DateTime.UtcNow
        });
        faction.Conditions.Add(CreateCondition(fileName, formKey, 0, childFormKey, "1"));
        faction.Components.Add(CreateRecordComponent(fileName, RecordTypeCatalog.Faction.RecordID, formKey, childFormKey, componentValue));
    }

    /// <summary>
    /// Creates a shared record component row for comparison-service tests that exercise strategy-owned child groups.
    /// </summary>
    /// <param name="fileName">The plugin file name that contributed the child row.</param>
    /// <param name="recordType">The owning record type ID.</param>
    /// <param name="formKey">The parent record FormKey.</param>
    /// <param name="displayFilter">The display filter FormKey to place on the component item.</param>
    /// <param name="itemValue">The numeric value to place on the component item.</param>
    /// <returns>The populated shared record component DTO.</returns>
    private static RecordComponentDTO CreateRecordComponent(
        string fileName,
        string recordType,
        FormKeyDTO formKey,
        FormKeyDTO displayFilter,
        double itemValue)
    {
        return new RecordComponentDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey(fileName),
            FormKey = formKey,
            RecordType = recordType,
            ComponentIndex = 0,
            MutagenObjectType = "Component",
            DCED = [1, 2],
            ImportedAtUTC = DateTime.UtcNow,
            Items =
            [
                new RecordComponentItemDTO
                {
                    Game = SupportedGame.Starfield,
                    ModKey = CreateModKey(fileName),
                    FormKey = formKey,
                    RecordType = recordType,
                    ComponentIndex = 0,
                    ItemIndex = 0,
                    DisplayFilter = displayFilter,
                    Unknown1 = itemValue,
                    ImportedAtUTC = DateTime.UtcNow
                }
            ]
        };
    }

    /// <summary>
    /// Creates a minimal Actor Value Information record for comparison-service tests that exercise scalar metadata
    /// dispatch.
    /// </summary>
    /// <param name="fileName">The plugin file name that contributed the test record.</param>
    /// <param name="formKey">The origin FormKey shared by compared records.</param>
    /// <param name="name">The translated actor value name to place on the DTO.</param>
    /// <param name="abbreviation">The translated actor value abbreviation to place on the DTO.</param>
    /// <param name="description">The translated actor value description to place on the DTO.</param>
    /// <param name="cnam">The CNAM text to place on the DTO.</param>
    /// <param name="improveMult">The skill improve multiplier to place on the DTO.</param>
    /// <param name="improveOffset">The skill improve offset to place on the DTO.</param>
    /// <param name="useMult">The skill use multiplier to place on the DTO.</param>
    /// <param name="contextNotes">The context notes to place on the DTO.</param>
    /// <param name="defaultValue">The default value scalar to place on the DTO.</param>
    /// <param name="flags">The flags text to place on the DTO.</param>
    /// <param name="type">The type text to place on the DTO.</param>
    /// <param name="min">The minimum value scalar to place on the DTO.</param>
    /// <param name="max">The maximum value scalar to place on the DTO.</param>
    /// <returns>The populated Actor Value Information DTO.</returns>
    private static ActorValueInformationDTO CreateActorValueInformation(
        string fileName,
        FormKeyDTO formKey,
        string name,
        string abbreviation,
        string description,
        string cnam,
        double improveMult,
        double improveOffset,
        double useMult,
        string contextNotes,
        double defaultValue,
        string flags,
        string type,
        double min,
        double max)
    {
        return new ActorValueInformationDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey(fileName),
            FormKey = formKey,
            EditorID = "MyActorValueInformation",
            FormVersion = 1,
            MajorRecordFlags = 2,
            ImportedAtUTC = DateTime.UtcNow,
            Name = Text(name),
            Abbreviation = Text(abbreviation),
            Description = Text(description),
            CNAM = cnam,
            Skill = new ActorValueInformationSkillDTO
            {
                ImproveMult = improveMult,
                ImproveOffset = improveOffset,
                UseMult = useMult
            },
            ContextNotes = contextNotes,
            DefaultValue = defaultValue,
            Flags = flags,
            Type = type,
            Min = min,
            Max = max
        };
    }

    /// <summary>
    /// Creates an Actor Value Information perk-tree entry for comparison-service tests that exercise strategy-owned
    /// child groups.
    /// </summary>
    /// <param name="fileName">The plugin file name that contributed the child row.</param>
    /// <param name="formKey">The parent Actor Value Information FormKey.</param>
    /// <param name="associatedSkill">The associated skill FormKey referenced by the perk-tree entry.</param>
    /// <param name="perk">The perk FormKey referenced by the perk-tree entry.</param>
    /// <param name="perkTreeIndex">The stable perk-tree index used for cross-plugin alignment.</param>
    /// <param name="fnam">The FNAM text to place on the entry.</param>
    /// <param name="connectionTargetIndex">The target index to place in the first connection-line row.</param>
    /// <param name="perkGridX">The grid X coordinate to place on the entry.</param>
    /// <returns>The populated Actor Value Information perk-tree entry DTO.</returns>
    private static ActorValueInformationPerkTreeEntryDTO CreateActorValueInformationPerkTreeEntry(
        string fileName,
        FormKeyDTO formKey,
        FormKeyDTO associatedSkill,
        FormKeyDTO perk,
        int perkTreeIndex,
        string fnam,
        int connectionTargetIndex,
        int perkGridX)
    {
        return new ActorValueInformationPerkTreeEntryDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey(fileName),
            FormKey = formKey,
            PerkTreeIndex = perkTreeIndex,
            AssociatedSkill = associatedSkill,
            FNAM = fnam,
            HorizontalPosition = 1.5,
            Index = 2,
            PerkGridX = perkGridX,
            PerkGridY = 3,
            VerticalPosition = 4.5,
            Perk = perk,
            ConnectionLineToIndices =
            [
                new ActorValueInformationConnectionLineIndexDTO
                {
                    Game = SupportedGame.Starfield,
                    ModKey = CreateModKey(fileName),
                    FormKey = formKey,
                    PerkTreeIndex = perkTreeIndex,
                    ConnectionLineIndex = 0,
                    TargetIndex = connectionTargetIndex,
                    ImportedAtUTC = DateTime.UtcNow
                }
            ],
            ImportedAtUTC = DateTime.UtcNow
        };
    }

    private static MiscItemDTO CreateMiscItem(string fileName, FormKeyDTO formKey, string name, int value, float weight, FormKeyDTO? featuredItemMessageFormKey)
    {
        return new MiscItemDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey(fileName),
            FormKey = formKey,
            EditorID = "MyMiscItem",
            FormVersion = 1,
            MajorRecordFlags = 2,
            ImportedAtUTC = DateTime.UtcNow,
            Name = Text(name),
            ShortName = Text("ShortName"),
            Value = value,
            Weight = weight,
            DirtinessScale = 1,
            FeaturedItemMessage = featuredItemMessageFormKey,
            Flag = "None"
        };
    }

    /// <summary>
    /// Creates a minimal Keyword record for comparison-service tests that exercise scalar metadata dispatch.
    /// </summary>
    /// <param name="fileName">The plugin file name that contributed the test record.</param>
    /// <param name="formKey">The origin FormKey shared by compared records.</param>
    /// <param name="type">The keyword type value to place on the DTO.</param>
    /// <param name="color">The keyword color value to place on the DTO.</param>
    /// <returns>The populated Keyword DTO.</returns>
    private static KeywordDTO CreateKeyword(string fileName, FormKeyDTO formKey, string type, string color)
    {
        return new KeywordDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey(fileName),
            FormKey = formKey,
            EditorID = "MyKeyword",
            FormVersion = 1,
            MajorRecordFlags = 2,
            ImportedAtUTC = DateTime.UtcNow,
            Type = type,
            Color = color
        };
    }

    private static MiscItemComponentDTO CreateMiscItemComponent(
        string fileName,
        FormKeyDTO formKey,
        FormKeyDTO componentFormKey,
        int componentIndex,
        int displayIndex,
        int count)
    {
        return new MiscItemComponentDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey(fileName),
            FormKey = formKey,
            Component = componentFormKey,
            ComponentIndex = componentIndex,
            DisplayIndex = displayIndex,
            Count = count,
            ImportedAtUTC = DateTime.UtcNow
        };
    }

    private static MiscItemDestructibleDTO CreateMiscItemDestructible(
        FormKeyDTO explosionFormKey,
        int health,
        int destCount,
        string modelFile,
        string modelData)
    {
        return new MiscItemDestructibleDTO
        {
            Data = new MiscItemDestructibleDataDTO
            {
                Health = health,
                DESTCount = destCount
            },
            Stages =
            {
                new MiscItemDestructibleStageDTO
                {
                    StageIndex = 0,
                    HealthPercent = 90,
                    ModelDamageStage = 1,
                    Flags = "CapDamage",
                    SelfDamagePerSecond = 45,
                    Explosion = explosionFormKey,
                    Model = new MiscItemDestructibleStageModelDTO
                    {
                        File = modelFile,
                        Data = modelData
                    }
                }
            }
        };
    }

    private static ModelDTO CreateModel(string fileName, FormKeyDTO formKey, string file)
    {
        return CreateModel(fileName, RecordTypeCatalog.MiscItem.RecordID, formKey, file);
    }

    private static ModelDTO CreateModel(string fileName, string recordType, FormKeyDTO formKey, string file, string? data = null)
    {
        return new ModelDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey(fileName),
            RecordType = recordType,
            FormKey = formKey,
            ModelSlot = "Model",
            ModelGender = string.Empty,
            File = file,
            Data = data,
            ImportedAtUTC = DateTime.UtcNow
        };
    }

    private static StaticDTO CreateStatic(string fileName, FormKeyDTO formKey, double maxAngle, string objectBoundsFirst, double? unknownDNAMFloat)
    {
        return new StaticDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey(fileName),
            FormKey = formKey,
            EditorID = "MyStatic",
            FormVersion = 1,
            MajorRecordFlags = 2,
            ImportedAtUTC = DateTime.UtcNow,
            ObjectBoundsFirst = objectBoundsFirst,
            ObjectBoundsSecond = "1, 1, 1",
            MaxAngle = maxAngle,
            UnknownDNAMFloat = unknownDNAMFloat,
            DNAMDataTypeState = "Enabled"
        };
    }

    /// <summary>
    /// Creates an indexed static property row for comparison tests.
    /// </summary>
    /// <param name="fileName">The plugin filename used to build the row's mod key.</param>
    /// <param name="formKey">The owning static record form key.</param>
    /// <param name="actorValue">The actor value reference assigned to the property.</param>
    /// <param name="propertyIndex">The property index used for comparison row alignment.</param>
    /// <param name="value">The numeric property value.</param>
    /// <returns>A populated static property DTO suitable for static comparison fixtures.</returns>
    private static StaticPropertyDTO CreateStaticProperty(
        string fileName,
        FormKeyDTO formKey,
        FormKeyDTO actorValue,
        int propertyIndex,
        double value)
    {
        return new StaticPropertyDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey(fileName),
            FormKey = formKey,
            ActorValue = actorValue,
            PropertyIndex = propertyIndex,
            Value = value,
            ImportedAtUTC = DateTime.UtcNow
        };
    }

    private static BookDTO CreateBook(string fileName, FormKeyDTO formKey, string name, int value)
    {
        return new BookDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey(fileName),
            FormKey = formKey,
            EditorID = "MyBook",
            FormVersion = 1,
            MajorRecordFlags = 2,
            ImportedAtUTC = DateTime.UtcNow,
            Version2 = 3,
            ObjectBounds = new ObjectBoundsDTO
            {
                First = "0, 0, 0",
                Second = "1, 1, 1"
            },
            Transforms = new BookTransformsDTO
            {
                Inventory = CreateFormKey("Starfield.esm", 0x999)
            },
            InventoryArt = CreateFormKey("Starfield.esm", 0x998),
            PreviewTransform = CreateFormKey("Starfield.esm", 0x888),
            FeaturedItemMessage = CreateFormKey("Starfield.esm", 0x777),
            XALG = 7,
            Name = Text(name),
            Text = Text(fileName.StartsWith("Base", StringComparison.Ordinal) ? "Base text" : "Patch text"),
            Value = value,
            Weight = 1.25f,
            Flags = "Takeable",
            Teaches = new BookTeachesDTO
            {
                MutagenObjectType = "Skill",
                Perk = CreateFormKey("Starfield.esm", 0x666),
                RawContent = "Piloting"
            },
            DataSlateType = "None",
            Description = Text("Book description"),
            DataSlateHeaderLeft = Text("Left"),
            DataSlateHeaderRight = Text("Right")
        };
    }

    private static DoorDTO CreateDoor(string fileName, FormKeyDTO formKey, string name, FormKeyDTO? nativeTerminalFormKey, string facingAxisOverride)
    {
        return new DoorDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey(fileName),
            FormKey = formKey,
            EditorID = "MyDoor",
            FormVersion = 1,
            MajorRecordFlags = 2,
            ImportedAtUTC = DateTime.UtcNow,
            Version2 = 1,
            ObjectBoundsFirst = "0, 0, 0",
            ObjectBoundsSecond = "1, 1, 1",
            Name = Text(name),
            Flags = "Automatic",
            NativeTerminalFormKey = nativeTerminalFormKey,
            SoundLevel = "Normal",
            FacingAxisOverride = facingAxisOverride
        };
    }

    private static ContainerDTO CreateContainer(
        string fileName,
        FormKeyDTO formKey,
        string name,
        FormKeyDTO? nativeTerminalFormKey,
        IList<ContainerItemDTO> items,
        string? animationGraph = null)
    {
        return new ContainerDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey(fileName),
            FormKey = formKey,
            EditorID = "MyContainer",
            FormVersion = 1,
            MajorRecordFlags = 2,
            ImportedAtUTC = DateTime.UtcNow,
            Version2 = 15,
            ObjectBoundsFirst = "0, 0, 0",
            ObjectBoundsSecond = "1, 1, 1",
            Name = Text(name),
            Flags = "Respawns",
            NativeTerminalFormKey = nativeTerminalFormKey,
            AnimationGraph = animationGraph,
            Items = items
        };
    }

    /// <summary>
    /// Creates a Terminal test record with marker parameters and one VMAD script fragment so comparison tests can
    /// exercise both strategy-owned and specification-declared child groups.
    /// </summary>
    /// <param name="fileName">The plugin filename used to build the record's mod key and fixture-specific values.</param>
    /// <param name="formKey">The shared origin form key used by all compared overrides.</param>
    /// <param name="name">The localized terminal display name.</param>
    /// <param name="markerFlags">The marker flags text stored on the parent terminal row.</param>
    /// <param name="entryTypes">The marker parameter entry-type text for the generated marker parameter row.</param>
    /// <returns>A populated terminal DTO suitable for comparison service tests.</returns>
    private static TerminalDTO CreateTerminal(string fileName, FormKeyDTO formKey, string name, string markerFlags, string entryTypes)
    {
        return new TerminalDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey(fileName),
            FormKey = formKey,
            EditorID = "MyTerminal",
            FormVersion = 1,
            MajorRecordFlags = 2,
            ImportedAtUTC = DateTime.UtcNow,
            Version2 = 4,
            ObjectBoundsFirst = "0, 0, 0",
            ObjectBoundsSecond = "1, 1, 1",
            MenuFormKey = CreateFormKey("Starfield.esm", 0x111),
            Background = "BackgroundA",
            Name = Text(name),
            Pnam = "PNAM",
            Fnam = "FNAM",
            Jnam = "JNAM",
            MarkerFlags = markerFlags,
            Gnam = "GNAM",
            WorkbenchData = "WorkbenchData",
            FurnitureTemplateFormKey = CreateFormKey("Starfield.esm", 0x222),
            MarkerModel = "MarkerModel.nif",
            MarkerParameters =
            [
                new TerminalMarkerParameterDTO
                {
                    Game = SupportedGame.Starfield,
                    ModKey = CreateModKey(fileName),
                    FormKey = formKey,
                    ParameterIndex = 0,
                    Offset = "0,0,0",
                    EntryTypes = entryTypes,
                    ExitTypes = "ExitType",
                    ImportedAtUTC = DateTime.UtcNow
                }
            ],
            ScriptFragments =
            [
                new ScriptFragmentDTO
                {
                    Game = SupportedGame.Starfield,
                    ModKey = CreateModKey(fileName),
                    RecordType = RecordTypeCatalog.Terminal.RecordID,
                    FormKey = formKey,
                    FragmentSlot = "MenuItem",
                    FragmentIndex = 0,
                    SourceFragmentIndex = 0,
                    MutagenObjectType = "TerminalScriptFragment",
                    ScriptName = "TerminalMenuScript",
                    FragmentName = fileName.StartsWith("Base", StringComparison.Ordinal) ? "BaseFragment" : "PatchFragment",
                    Unknown2 = 2,
                    ExtraBindDataVersion = 1,
                    ImportedAtUTC = DateTime.UtcNow
                }
            ]
        };
    }

    private static ConstructibleObjectDTO CreateConstructibleObject(
        string fileName,
        FormKeyDTO formKey,
        FormKeyDTO createdObjectFormKey,
        FormKeyDTO workbenchKeywordFormKey,
        FormKeyDTO componentFormKey,
        FormKeyDTO recipeFilterFormKey,
        int amountProduced)
    {
        return new ConstructibleObjectDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey(fileName),
            FormKey = formKey,
            EditorID = "MyRecipe",
            FormVersion = 1,
            MajorRecordFlags = 2,
            ImportedAtUTC = DateTime.UtcNow,
            Version2 = 2,
            Description = Text("Recipe description"),
            CreatedObjectFormKey = createdObjectFormKey,
            WorkbenchKeywordFormKey = workbenchKeywordFormKey,
            CreatedObjectCount = amountProduced,
            AmountProduced = amountProduced,
            LearnMethod = "DefaultOrConditions",
            Flags = "None",
            Components =
            {
                new ConstructibleObjectComponentDTO
                {
                    Game = SupportedGame.Starfield,
                    ModKey = CreateModKey(fileName),
                    FormKey = formKey,
                    ComponentFormKey = componentFormKey,
                    ComponentIndex = 0,
                    Count = 3,
                    ImportedAtUTC = DateTime.UtcNow
                }
            },
            RecipeFilters =
            {
                new ConstructibleObjectRecipeFilterDTO
                {
                    Game = SupportedGame.Starfield,
                    ModKey = CreateModKey(fileName),
                    FormKey = formKey,
                    RecipeFilterFormKey = recipeFilterFormKey,
                    RecipeFilterIndex = 0,
                    ImportedAtUTC = DateTime.UtcNow
                }
            },
            Conditions =
            {
                new ConditionFormConditionDTO
                {
                    Game = SupportedGame.Starfield,
                    ModKey = CreateModKey(fileName),
                    FormKey = formKey,
                    ConditionIndex = 0,
                    MutagenObjectType = "ConditionFloat",
                    DataMutagenObjectType = "GetItemCountConditionData",
                    CompareOperator = "EqualTo",
                    ComparisonValue = amountProduced.ToString(),
                    ImportedAtUTC = DateTime.UtcNow
                }
            }
        };
    }

    private static ConditionFormDTO CreateConditionForm(string fileName, FormKeyDTO formKey, int version2, FormKeyDTO firstParameter, string? comparisonValue)
    {
        return new ConditionFormDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey(fileName),
            FormKey = formKey,
            EditorID = "MyConditionForm",
            FormVersion = 1,
            MajorRecordFlags = 2,
            ImportedAtUTC = DateTime.UtcNow,
            Version2 = version2,
            Conditions =
            {
                new ConditionFormConditionDTO
                {
                    Game = SupportedGame.Starfield,
                    ModKey = CreateModKey(fileName),
                    FormKey = formKey,
                    ConditionIndex = 0,
                    MutagenObjectType = "ConditionFloat",
                    DataMutagenObjectType = "HasKeywordConditionData",
                    CompareOperator = "EqualTo",
                    ComparisonValue = comparisonValue,
                    ImportedAtUTC = DateTime.UtcNow,
                    Parameters =
                    {
                        new ConditionFormConditionParameterDTO
                        {
                            Game = SupportedGame.Starfield,
                            ModKey = CreateModKey(fileName),
                            FormKey = formKey,
                            ConditionIndex = 0,
                            ParameterName = "FirstParameter",
                            ParameterValue = FormatFormKey(firstParameter),
                            ParameterFormKey = firstParameter,
                            ImportedAtUTC = DateTime.UtcNow
                        },
                        new ConditionFormConditionParameterDTO
                        {
                            Game = SupportedGame.Starfield,
                            ModKey = CreateModKey(fileName),
                            FormKey = formKey,
                            ConditionIndex = 0,
                            ParameterName = "RunOnType",
                            ParameterValue = "Subject",
                            ImportedAtUTC = DateTime.UtcNow
                        },
                        new ConditionFormConditionParameterDTO
                        {
                            Game = SupportedGame.Starfield,
                            ModKey = CreateModKey(fileName),
                            FormKey = formKey,
                            ConditionIndex = 0,
                            ParameterName = "SecondParameter",
                            ParameterValue = "0",
                            ImportedAtUTC = DateTime.UtcNow
                        }
                    }
                }
            }
        };
    }

    private static ConditionFormDTO CreateActorIsPreyConditionForm(FormKeyDTO formKey)
    {
        return new ConditionFormDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey("Starfield.esm"),
            FormKey = formKey,
            EditorID = "ActorIsPrey",
            FormVersion = 581,
            MajorRecordFlags = 0,
            ImportedAtUTC = DateTime.UtcNow,
            Version2 = 1,
            Conditions =
            {
                CreateCondition("Starfield.esm", formKey, 0, CreateFormKey("Starfield.esm", 0x258350), "1"),
                CreateCondition("Starfield.esm", formKey, 1, CreateFormKey("Starfield.esm", 0x2CC9F2), "0")
            }
        };
    }

    private static ConditionFormConditionDTO CreateCondition(string fileName, FormKeyDTO formKey, int conditionIndex, FormKeyDTO firstParameter, string comparisonValue)
    {
        return new ConditionFormConditionDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey(fileName),
            FormKey = formKey,
            ConditionIndex = conditionIndex,
            MutagenObjectType = "ConditionFloat",
            DataMutagenObjectType = "HasKeywordConditionData",
            CompareOperator = "EqualTo",
            ComparisonValue = comparisonValue,
            ImportedAtUTC = DateTime.UtcNow,
            Parameters =
            {
                new ConditionFormConditionParameterDTO
                {
                    Game = SupportedGame.Starfield,
                    ModKey = CreateModKey(fileName),
                    FormKey = formKey,
                    ConditionIndex = conditionIndex,
                    ParameterName = "RunOnType",
                    ParameterValue = "Subject",
                    ImportedAtUTC = DateTime.UtcNow
                },
                new ConditionFormConditionParameterDTO
                {
                    Game = SupportedGame.Starfield,
                    ModKey = CreateModKey(fileName),
                    FormKey = formKey,
                    ConditionIndex = conditionIndex,
                    ParameterName = "FirstParameter",
                    ParameterValue = FormatFormKey(firstParameter),
                    ParameterFormKey = firstParameter,
                    ImportedAtUTC = DateTime.UtcNow
                },
                new ConditionFormConditionParameterDTO
                {
                    Game = SupportedGame.Starfield,
                    ModKey = CreateModKey(fileName),
                    FormKey = formKey,
                    ConditionIndex = conditionIndex,
                    ParameterName = "SecondParameter",
                    ParameterValue = "0",
                    ImportedAtUTC = DateTime.UtcNow
                }
            }
        };
    }

    private static ContainerItemDTO CreateContainerItem(string fileName, FormKeyDTO formKey, FormKeyDTO itemFormKey, int itemIndex, int count)
    {
        return new ContainerItemDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey(fileName),
            FormKey = formKey,
            ItemFormKey = itemFormKey,
            ItemIndex = itemIndex,
            Count = count,
            ImportedAtUTC = DateTime.UtcNow
        };
    }

    private static ReflectionDTO CreateReflection(string fileName, FormKeyDTO formKey, int componentIndex, string componentType, string sourcePath, string refl)
    {
        return new ReflectionDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey(fileName),
            RecordType = RecordTypeCatalog.Static.RecordID,
            FormKey = formKey,
            ComponentIndex = componentIndex,
            ComponentType = componentType,
            SourcePath = sourcePath,
            REFL = refl,
            ImportedAtUTC = DateTime.UtcNow
        };
    }

    private static string FormatFormKey(FormKeyDTO formKey)
    {
        return $"{formKey.ModKey.FileName}:{formKey.Id:X8}";
    }

    private static ScriptingAdapterDTO CreateScriptingAdapter(string fileName, FormKeyDTO formKey, string name, string propertyName, string propertyValue)
    {
        return CreateScriptingAdapter(fileName, RecordTypeCatalog.MiscItem.RecordID, formKey, name, propertyName, propertyValue);
    }

    private static ScriptingAdapterDTO CreateScriptingAdapter(string fileName, string recordType, FormKeyDTO formKey, string name, string propertyName, string propertyValue)
    {
        return new ScriptingAdapterDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey(fileName),
            RecordType = recordType,
            FormKey = formKey,
            Name = name,
            ScriptIndex = 0,
            ImportedAtUTC = DateTime.UtcNow,
            Properties =
            {
                new ScriptingAdapterPropertyDTO
                {
                    Game = SupportedGame.Starfield,
                    ModKey = CreateModKey(fileName),
                    RecordType = recordType,
                    FormKey = formKey,
                    ScriptingAdapterName = name,
                    PropertyIndex = 0,
                    Name = propertyName,
                    MutagenObjectType = "String",
                    DataString = propertyValue,
                    ImportedAtUTC = DateTime.UtcNow
                }
            }
        };
    }

    private static KeywordMappingDTO CreateKeywordMapping(string fileName, string recordType, FormKeyDTO formKey, FormKeyDTO keywordFormKey, int keywordIndex)
    {
        return new KeywordMappingDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey(fileName),
            RecordType = recordType,
            FormKey = formKey,
            KeywordIndex = keywordIndex,
            Keyword = keywordFormKey,
            ImportedAtUTC = DateTime.UtcNow
        };
    }

    private static SoundMappingDTO CreateSoundMapping(string fileName, string recordType, FormKeyDTO formKey, string soundSlot, int soundIndex, string start, string? versioning = null, string? unknown = null)
    {
        return new SoundMappingDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey(fileName),
            RecordType = recordType,
            FormKey = formKey,
            SoundSlot = soundSlot,
            SoundIndex = soundIndex,
            Start = start,
            Versioning = versioning,
            Unknown = unknown,
            ImportedAtUTC = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Creates a minimal Magic Effect record for comparison-service tests that exercise scalar metadata dispatch.
    /// </summary>
    /// <param name="fileName">The plugin file name that contributed the test record.</param>
    /// <param name="formKey">The origin FormKey shared by compared records.</param>
    /// <param name="name">The translated magic effect name to place on the DTO.</param>
    /// <param name="archetype">The magic effect archetype value to place on the DTO.</param>
    /// <param name="unknownInt2">The unknown integer value to place on the DTO.</param>
    /// <param name="description">The translated magic effect description to place on the DTO.</param>
    /// <param name="flags">The flags text to place on the DTO.</param>
    /// <param name="castType">The cast type text to place on the DTO.</param>
    /// <param name="targetType">The target type text to place on the DTO.</param>
    /// <param name="actorValue2FormKey">The actor value FormKey to place on the DTO.</param>
    /// <param name="resistValueFormKey">The resist value FormKey to place on the DTO.</param>
    /// <param name="perkToApplyFormKey">The perk-to-apply FormKey to place on the DTO.</param>
    /// <param name="equipAbilityFormKey">The equip ability FormKey to place on the DTO.</param>
    /// <param name="explosionFormKey">The explosion FormKey to place on the DTO.</param>
    /// <param name="castingArtFormKey">The casting art FormKey to place on the DTO.</param>
    /// <param name="hitEffectArtFormKey">The hit effect art FormKey to place on the DTO.</param>
    /// <param name="hitShaderFormKey">The hit shader FormKey to place on the DTO.</param>
    /// <param name="imageSpaceModifierFormKey">The image space modifier FormKey to place on the DTO.</param>
    /// <param name="impactDataFormKey">The impact data FormKey to place on the DTO.</param>
    /// <param name="projectileFormKey">The projectile FormKey to place on the DTO.</param>
    /// <param name="unknownFloat3">The unknown float value to place on the DTO.</param>
    /// <param name="unknown">The first unknown text value to place on the DTO.</param>
    /// <param name="unknown2">The second unknown text value to place on the DTO.</param>
    /// <param name="dataTypeState">The data type state text to place on the DTO.</param>
    /// <returns>The populated Magic Effect DTO.</returns>
    private static MagicEffectDTO CreateMagicEffect(
        string fileName,
        FormKeyDTO formKey,
        string name,
        string archetype,
        int unknownInt2,
        string description = "Magic effect description",
        string flags = "None",
        string? castType = null,
        string? targetType = null,
        FormKeyDTO? actorValue2FormKey = null,
        FormKeyDTO? resistValueFormKey = null,
        FormKeyDTO? perkToApplyFormKey = null,
        FormKeyDTO? equipAbilityFormKey = null,
        FormKeyDTO? explosionFormKey = null,
        FormKeyDTO? castingArtFormKey = null,
        FormKeyDTO? hitEffectArtFormKey = null,
        FormKeyDTO? hitShaderFormKey = null,
        FormKeyDTO? imageSpaceModifierFormKey = null,
        FormKeyDTO? impactDataFormKey = null,
        FormKeyDTO? projectileFormKey = null,
        float? unknownFloat3 = null,
        string? unknown = null,
        string? unknown2 = null,
        string? dataTypeState = null)
    {
        return new MagicEffectDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey(fileName),
            FormKey = formKey,
            EditorID = "MyMagicEffect",
            FormVersion = 1,
            MajorRecordFlags = 2,
            ImportedAtUTC = DateTime.UtcNow,
            Name = Text(name),
            Description = Text(description),
            Flags = flags,
            CastType = castType,
            TargetType = targetType,
            ActorValue2FormKey = actorValue2FormKey,
            ResistValueFormKey = resistValueFormKey,
            PerkToApplyFormKey = perkToApplyFormKey,
            EquipAbilityFormKey = equipAbilityFormKey,
            ExplosionFormKey = explosionFormKey,
            CastingArtFormKey = castingArtFormKey,
            HitEffectArtFormKey = hitEffectArtFormKey,
            HitShaderFormKey = hitShaderFormKey,
            ImageSpaceModifierFormKey = imageSpaceModifierFormKey,
            ImpactDataFormKey = impactDataFormKey,
            ProjectileFormKey = projectileFormKey,
            Archetype = archetype,
            UnknownFloat3 = unknownFloat3,
            UnknownInt2 = unknownInt2,
            Unknown = unknown,
            Unknown2 = unknown2,
            DataTypeState = dataTypeState
        };
    }

    /// <summary>
    /// Creates a Perk test record with rank, background-skill, and VMAD script-fragment rows used by comparison
    /// service tests.
    /// </summary>
    /// <param name="fileName">The plugin filename used to build the record's mod key and fixture-specific values.</param>
    /// <param name="formKey">The shared origin form key used by all compared overrides.</param>
    /// <param name="name">The localized perk display name.</param>
    /// <param name="unknownStaticFormKey">The static form key assigned to the generated perk rank.</param>
    /// <param name="backgroundSkillFormKey">The skill form key assigned to the generated background-skill row.</param>
    /// <param name="rankDescription">The localized description assigned to the generated rank.</param>
    /// <param name="buttonLabel">The localized button label assigned to the generated rank effect.</param>
    /// <returns>A populated perk DTO suitable for comparison service tests.</returns>
    private static PerkDTO CreatePerk(string fileName, FormKeyDTO formKey, string name, FormKeyDTO unknownStaticFormKey, FormKeyDTO backgroundSkillFormKey, string rankDescription, string buttonLabel)
    {
        return new PerkDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey(fileName),
            FormKey = formKey,
            EditorID = "MyPerk",
            FormVersion = 1,
            MajorRecordFlags = 2,
            ImportedAtUTC = DateTime.UtcNow,
            Name = Text(name),
            Description = Text("Perk description"),
            Flags = "PcPlayable",
            SkillGroup = "Expert",
            CrewAssignment = "None",
            PerkIcon = "Patch_Science_Chemistry",
            Category = "Science",
            MajorFlags = "0",
            Ranks =
            {
                new PerkRankDTO
                {
                    ModKey = CreateModKey(fileName),
                    FormKey = formKey,
                    RankIndex = 0,
                    Description = Text(rankDescription),
                    UnknownStaticFormKey = unknownStaticFormKey,
                    ConditionCount = 1,
                    ActivityCount = 2,
                    ImportedAtUTC = DateTime.UtcNow,
                    Effects =
                    {
                        new PerkRankEffectDTO
                        {
                            ModKey = CreateModKey(fileName),
                            FormKey = formKey,
                            RankIndex = 0,
                            EffectIndex = 0,
                            MutagenObjectType = "PerkEntryPointModifyValue",
                            Rank = 1,
                            Priority = 10,
                            PerkEntryId = 20,
                            Flags = "None",
                            ButtonLabel = Text(buttonLabel),
                            ConditionCount = 3,
                            EntryPoint = "ModSkillUse",
                            PerkConditionTabCount = 4,
                            Modification = "Add",
                            Value = fileName.StartsWith("Base", StringComparison.Ordinal) ? 1.5 : 2.5,
                            ImportedAtUTC = DateTime.UtcNow
                        }
                    }
                }
            },
            BackgroundSkills =
            {
                new PerkBackgroundSkillDTO
                {
                    ModKey = CreateModKey(fileName),
                    FormKey = formKey,
                    SkillFormKey = backgroundSkillFormKey,
                    SkillIndex = 0,
                    ImportedAtUTC = DateTime.UtcNow
                }
            },
            ScriptFragments =
            {
                new ScriptFragmentDTO
                {
                    Game = SupportedGame.Starfield,
                    ModKey = CreateModKey(fileName),
                    RecordType = RecordTypeCatalog.Perk.RecordID,
                    FormKey = formKey,
                    FragmentSlot = "Rank",
                    FragmentIndex = 0,
                    SourceFragmentIndex = 0,
                    MutagenObjectType = "PerkScriptFragment",
                    ScriptName = "PerkRankScript",
                    FragmentName = fileName.StartsWith("Base", StringComparison.Ordinal) ? "BaseRankFragment" : "PatchRankFragment",
                    Unknown2 = 2,
                    ExtraBindDataVersion = 1,
                    ImportedAtUTC = DateTime.UtcNow
                }
            }
        };
    }

}
