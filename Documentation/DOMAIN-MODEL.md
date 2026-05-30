# Domain Model

## Core Concepts

Plugin: A Starfield plugin file discovered from the local load order. Plugins are identified by Mutagen `ModKey` values 
and persisted in the `Plugins` table.

Load order entry: A discovered plugin plus its file name, path, load order index, and enabled state. Represented by 
`PluginLoadOrderEntryDTO`.

ModKey: Mutagen identifier for a plugin file. The database stores it as name, type, and file name columns. On typed
record tables, these columns identify the plugin file containing the imported record row.

FormKey: Mutagen identifier for an individual record. Typed record tables persist its numeric record identifier as
`FormKey_ID`. Multiple containing plugins can persist rows with the same `FormKey_ID`, which is how the current schema
represents a record that appears in more than one plugin.

The containing plugin's `ModKey` columns and the record's `FormKey_ID` serve different purposes. A comparison lookup
finds matching typed rows by `FormKey_ID`, then uses each row's containing-plugin `ModKey` to identify and order the
sources. `FormKey_ID` does not need an additional persisted `ModKey` tuple for this workflow.

FormID: Plugin-context-relative record identifier shown in the main record tree. The presentation layer uses Mutagen's
Starfield separated-master helpers to translate between stored `FormKey` values and displayed or filtered `FormID`
values. Core service and repository boundaries continue to use `FormKey`.

Master reference: A relationship between a plugin and a master plugin declared in the plugin header. Represented by 
`PluginMasterReferenceDTO` and persisted in `PluginMasterReferences`.

Record type: A Starfield major record category. `RecordTypeCatalog` contains only runtime metadata for record types the 
current import path persists: `FormList` and `GameSetting`. Broader Mutagen record type reference data belongs in this 
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
- source last-write ticks and source file size
- checked, imported, and invalidated timestamps

`StarfieldPluginReaderService` reads metadata from Mutagen using
`StarfieldMod.Create(...).FromPath(...).WithLoadOrderFromHeaderMasters().WithDataFolder(...).Construct()`.

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

For record comparison, typed rows for the same record can be located across containing plugins by querying
`FormKey_ID`. The containing-plugin columns remain available on each result for load-order sorting and display.

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
- imported timestamp

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
- optional title string, data, raw data, and `XALG`
- compression and deletion flags persisted as integer values

## Configuration

`ApplicationConfiguration` stores the selected game name and application theme. The theme is represented by 
`ApplicationThemeMode` and defaults to `Dark` when the configuration file is missing the theme value.

`ApplicationConfigurationStore` loads and saves the JSON configuration file and reports whether configuration is
required when no selected game is present.
