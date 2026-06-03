# Domain Model

## Core Concepts

Plugin: A Starfield plugin file discovered from the local load order. Plugins are identified by Mutagen `ModKey` values 
and persisted in the `Plugins` table.

Load order entry: A discovered plugin plus its file name, path, load order index, and enabled state. Represented by 
`PluginLoadOrderEntryDTO`.

ModKey: Mutagen identifier for a plugin file. The database stores it as name, type, and file name columns. On typed
record tables, `ModKey_*` columns identify the plugin file containing the imported record row.

FormKey: Mutagen identifier for an individual record. Typed record tables persist the origin `FormKey` as
`FormKey_ModKey_Name`, `FormKey_ModKey_Type`, `FormKey_ModKey_FileName`, and `FormKey_ID`.

The containing plugin's `ModKey` columns and the record's origin `FormKey` serve different purposes. A comparison
lookup finds matching typed rows by the full origin `FormKey`, then uses each row's containing-plugin `ModKey` to
identify and order the sources.

FormID: Plugin-context-relative record identifier shown in the main record tree. The presentation layer uses Mutagen's
Starfield separated-master helpers to translate between stored `FormKey` values and displayed or filtered `FormID`
values. Core service and repository boundaries continue to use `FormKey`.

Master reference: A relationship edge between a declaring plugin and a master plugin declared in its header.
Represented by `PluginMasterReferenceDTO` and persisted in `PluginMasterReferences`. Load-order indexes are derived
from the related `Plugins` rows instead of being duplicated on the relationship.

Record type: A Starfield major record category. `RecordTypeCatalog` contains only runtime metadata for record types the 
current import path persists: `FormList`, `GameSetting`, `Global`, `MiscObject`, `Keyword`, `NPC`,
`ActorValueInformation`, `MagicEffect`, and `Perk`. Broader Mutagen record type reference data belongs in this
documentation, not executable code.

## Plugin Import States

`PluginImportState` contains:

- `Current`: the plugin exists and was imported for the current source fingerprint.
- `Changed`: the plugin source differs from the stored fingerprint. The current implementation counts changed plugins
  during import and saves successfully reimported plugins as `Current`.
- `Missing`: the plugin was present in load order data but the source file was not found on disk.
- `Failed`: plugin metadata import failed.
- `Unsupported`: the plugin is intentionally skipped. Current logic skips `BlueprintShips*.esm`.

## Plugin Metadata

`PluginDTO` carries:

- `ModKey`
- load order index
- enabled and exists-on-disk flags
- import state
- Starfield header flags
- form version
- author and branch
- interior cell count
- header record count
- source last-write ticks and source file size
- checked, imported, and invalidated timestamps

`StarfieldPluginReaderService` reads metadata from Mutagen using
`StarfieldMod.Create(...).FromPath(...).WithLoadOrderFromHeaderMasters().WithDataFolder(...).Construct()`.
The persisted record count comes from `mod.ModHeader.Stats.NumRecords`. The persisted `HeaderFlags` value uses
`StarfieldModHeader.HeaderFlag`, including `Master`, `Light`, `Medium`, and `Overlay`.

## Record Import

`RecordImportService` returns `RecordImportResultDTO` for a plugin. The result aggregates per-record-type counts from
`RecordTypeImportResultDTO`, including discovered headers, typed detail rows, form list item rows, failed records, and
unsupported typed detail import paths.

Record import progress is reported through `PluginImportProgressDTO`. Plugin-level progress remains based on load-order
position, while record-type fields identify the active record type and record index during long-running detail import
phases.

The active typed detail import path includes Starfield `FLST`:

- `StarfieldRecordReaderService.GetFormLists` reads form list DTOs from a plugin with one Mutagen mod load.
- `FormListImporter` saves each `FormListDTO`.
- `FormListRepository` saves the form list row.
- `FormListItemRepository` saves each item row.

The active typed detail import path also includes Starfield `GMST`:

- `StarfieldRecordReaderService.GetGameSettings` reads game setting DTOs from a plugin with one Mutagen mod load.
- `GameSettingImporter` saves each `GameSettingDTO`.
- `GameSettingRepository` saves the game setting row.

The active typed detail import path also includes Starfield `GLOB`, `MISC`, `KYWD`, `NPC_`, `AVIF`, `MGEF`, and
`PERK`. Each type has an explicit DTO, database model, repository, service, and importer. New record DTOs persist
clearly understood scalar values and direct `FormKey` references. Complex nested child structures are deferred until
they can be represented with normalized typed models.

The current typed detail import path also persists supported VMAD scripting data for the supported record types that
expose `VirtualMachineAdapter` through Mutagen: `Global`, `MiscItem`, `Keyword`, `NPC`, `ActorValueInformation`,
`MagicEffect`, and `Perk`.

`ScriptingAdapterDTO` represents one attached VMAD script:

- owning plugin `ModKey`
- owning record type name
- owning record `FormKey`
- script name
- script order index
- imported timestamp

`ScriptingAdapterPropertyDTO` represents one VMAD property attached to a script:

- parent script identity
- property order index
- property name
- `MutagenObjectType`
- supported scalar data fields
- supported object reference fields
- zero or more list items for supported list property shapes

`ScriptingAdapterPropertyListItemDTO` represents one ordered item inside a supported VMAD list property.

Unsupported VMAD property families remain deferred:

- `ScriptStructProperty`
- `ScriptStructListProperty`
- `ScriptVariableProperty`
- `ScriptVariableListProperty`

Localized record fields store only the resolved English text as nullable values. The database does not store
translation catalogs or JSON payloads.

For record comparison, typed rows for the same record can be located across containing plugins by querying the full
origin `FormKey`. The containing-plugin columns remain available on each result for load-order sorting and display.

## Starfield Record Type Reference

The following reference lists came from Mutagen record type names observed during implementation. They are
documentation
only and do not define application import support.

Record types currently treated as known supported Mutagen types:

- AcousticSpace
- ActionRecord
- Activator
- ActorValueInformation
- ActorValueModulation
- AddonNode
- AffinityEvent
- AimAssistModel
- AimAssistPose
- AimModel
- AimOpticalSightMarker
- AmbienceSet
- Ammunition
- AnimatedObject
- AnimationSoundTagSet
- AObjectModification
- APlacedTrap
- Armor
- ArmorAddon
- ArmorModification
- ArtObject
- AStoryManagerNode
- Atmosphere
- AttractionRule
- AudioOcclusionPrimitive
- BendableSpline
- Biome
- BiomeMarker
- BodyPartData
- BoneModifier
- Book
- CameraPath
- CameraShot
- Cell
- Challenge
- Class
- Climate
- Clouds
- CollisionLayer
- ColorRecord
- CombatStyle
- ConditionRecord
- ConstructibleObject
- Container
- ContainerModification
- Curve3D
- CurveTable
- DamageType
- Debris
- DefaultObject
- DefaultObjectManager
- DialogBranch
- DialogResponses
- DialogTopic
- Door
- EffectSequence
- EffectShader
- EquipType
- Explosion
- FacialExpression
- Faction
- Flora
- FloraModification
- FogVolume
- Footstep
- FootstepSet
- ForceData
- FormFolderKeywordList
- FormList (FLST)
- Furniture
- GameplayOption
- GameplayOptionsGroup
- GameSetting (GMST)
- GameSettingBool (GMST Child for Boolean Data)
- GameSettingFloat (GMST Child for Float Data)
- GameSettingInt (GMST Child for Integer Data)
- GameSettingString (GMST Child for String Data)
- GameSettingUInt (GMST Child for Unsigned Integer Data)
- GenericBaseForm
- GenericBaseFormTemplate
- Global
- Grass
- GroundCover
- Hazard
- HeadPart
- IdleAnimation
- IdleMarker
- ImageSpace
- ImageSpaceAdapter
- Impact
- ImpactDataSet
- Ingestible
- InstanceNamingRules
- Key
- Keyword
- LandscapeTexture
- Layer
- LayeredMaterialSwap
- LegendaryItem
- LensFlare
- LeveledBaseForm
- LeveledItem
- LeveledNpc
- LeveledPackIn
- LeveledSpaceCell
- Light
- LightingTemplate
- LoadScreen
- Location
- LocationReferenceType
- MagicEffect
- MaterialPath
- MaterialType
- MeleeAimAssistModel
- Message
- MiscItem
- MorphableObject
- MoveableStatic
- MovementType
- MusicTrack
- MusicType
- NavigationMesh
- NavigationMeshInfoMap
- NavigationMeshObstacleCoverManager
- Note
- Npc
- NpcModification
- ObjectEffect
- ObjectModification
- ObjectSwap
- ObjectVisibilityManager
- Outfit
- Package
- PackIn
- ParticleSystemDefineCollision
- Perk
- PERS
- PhotoModeFeature
- PlacedArrow
- PlacedBarrier
- PlacedBeam
- PlacedCone
- PlacedFlame
- PlacedHazard
- PlacedMissile
- PlacedNpc
- PlacedObject
- PlacedTrap
- Planet
- PlanetContentManagerBranchNode
- PlanetContentManagerContentNode
- PlanetContentManagerTree
- ProjectedDecal
- Projectile
- Quest
- Race
- ReferenceGroup
- Region
- ResearchProject
- Resource
- ResourceGenerationData
- ReverbParameters
- Scene
- SceneCollection
- SecondaryDamageList
- ShaderParticleGeometry
- SnapTemplate
- SnapTemplateBehavior
- SnapTemplateNode
- SoundEchoMarker
- SoundKeywordMapping
- SoundMarker
- SpeechChallenge
- Spell
- Star
- Static
- StaticCollection
- StoryManagerBranchNode
- StoryManagerEventNode
- StoryManagerQuestNode
- SunPreset
- SurfaceBlock
- SurfacePattern
- SurfacePatternConfig
- SurfacePatternStyle
- SurfaceTree
- Terminal
- TerminalMenu
- TextureSet
- TimeOfDayRecord
- Transform
- Traversal
- UnknownObjectModification
- VoiceType
- VolumetricLighting
- Water
- Weapon
- WeaponBarrelModel
- WeaponModification
- Weather
- WeatherSetting
- Worldspace
- WWiseEventData
- WWiseKeywordMapping
- Zoom

Record types currently treated as known unsupported Mutagen types:

`ArmorModification`, `ContainerModification`, `FloraModification`, `GameSettingBool`, `GameSettingFloat`, 
`GameSettingInt`, `GameSettingString`, `GameSettingUInt`, `NpcModification`, `PlacedArrow`, `PlacedBarrier`, 
`PlacedBeam`, `PlacedCone`, `PlacedFlame`, `PlacedHazard`, `PlacedMissile`, `PlacedTrap`, `UnknownObjectModification`, 
`WeaponModification`.

## Form List Data

`FormListDTO` represents a Starfield form list record with common header fields and form-list-specific data:

- owning `ModKey`
- record `FormKey`
- editor ID
- form version
- Starfield major record flags
- version fields
- imported timestamp
- optional `AddToListFormKey`
- item references

`FormListItemDTO` represents an item reference inside a form list:

- owning plugin `ModKey`
- owning form list `FormKey`
- item plugin `ModKey`
- item `FormKey`
- item index preserving source enumeration and display order
- imported timestamp

Form list items are an ordered sequence. Duplicate item references are valid and remain separate occurrences through
their `Item_Index` values.

## Game Setting Data

`GameSettingDTO` represents a Starfield game setting record with common header fields and game-setting-specific data:

- owning `ModKey`
- record `FormKey`
- editor ID
- form version
- Starfield major record flags
- version fields
- imported timestamp
- setting type such as `GameSettingFloat`, `GameSettingInt`, `GameSettingUInt`, `GameSettingString`, or
  `GameSettingBool`
- optional data, raw data, and `XALG`
- compression and deletion flags persisted as integer values

`TitleString` is not persisted for game settings because Mutagen's Starfield game-setting records do not expose that
field. The comparison workspace displays `Data` and hides raw diagnostic fields.

## Configuration

`ApplicationConfiguration` stores the selected game name and application theme. The theme is represented by 
`ApplicationThemeMode` and defaults to `Dark` when the configuration file is missing the theme value.

`ApplicationConfigurationStore` loads and saves the JSON configuration file and reports whether configuration is
required when no selected game is present.
