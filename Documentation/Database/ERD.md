# SQLite Entity Relationship Diagram

## Diagram

This diagram includes relationships declared as SQLite foreign keys. Composite keys are shown by marking each
participating column as `PK` or `FK`.

```mermaid
erDiagram
    Plugins {
        TEXT ModKey_Name PK
        INTEGER ModKey_Type PK
        TEXT ModKey_FileName PK
        INTEGER LoadOrderIndex
        INTEGER Enabled
        INTEGER ExistsOnDisk
        TEXT ImportState
        INTEGER RecordCount
    }

    PluginMasterReferences {
        TEXT Master_ModKey_Name PK, FK
        INTEGER Master_ModKey_Type PK, FK
        TEXT Master_ModKey_FileName PK, FK
        TEXT Plugin_ModKey_Name PK, FK
        INTEGER Plugin_ModKey_Type PK, FK
        TEXT Plugin_ModKey_FileName PK, FK
    }

    FormList {
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        INTEGER FormKey_ID PK
        TEXT AddToListFormKey
    }

    FormListItems {
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        INTEGER FormKey_ID PK, FK
        TEXT Item_ModKey_Name PK
        INTEGER Item_ModKey_Type PK
        TEXT Item_ModKey_FileName PK
        INTEGER Item_FormKey_ID PK
        INTEGER Item_Index PK
    }

    GameSetting {
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        INTEGER FormKey_ID PK
    }

    Global {
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        INTEGER FormKey_ID PK
    }

    MiscItem {
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        INTEGER FormKey_ID PK
    }

    Keyword {
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        INTEGER FormKey_ID PK
    }

    NPC {
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        INTEGER FormKey_ID PK
    }

    ActorValueInformation {
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        INTEGER FormKey_ID PK
    }

    MagicEffect {
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        INTEGER FormKey_ID PK
    }

    Perk {
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        INTEGER FormKey_ID PK
    }

    Plugins ||--o{ PluginMasterReferences : "is declared master"
    Plugins ||--o{ PluginMasterReferences : "declares masters"
    Plugins ||--o{ FormList : contains
    FormList ||--o{ FormListItems : contains
    Plugins ||--o{ GameSetting : contains
    Plugins ||--o{ Global : contains
    Plugins ||--o{ MiscItem : contains
    Plugins ||--o{ Keyword : contains
    Plugins ||--o{ NPC : contains
    Plugins ||--o{ ActorValueInformation : contains
    Plugins ||--o{ MagicEffect : contains
    Plugins ||--o{ Perk : contains
```

## Important Indexes

The schema has no separately declared unique indexes. Composite primary keys provide row uniqueness.

- `Plugins`: indexes on `LoadOrderIndex`, `ImportState`, and the source fingerprint columns.
- `PluginMasterReferences`: indexes on the declared-master key and declaring-plugin key.
- `FormListItems`: indexes on referenced item ID plus owning form-list key, and on `Item_Index`.
- Each typed record table: a non-unique index on `FormKey_ID` for cross-plugin comparison lookup.

## Important Constraints

- Every declared foreign key uses `ON DELETE CASCADE`.
- `Plugins.Enabled` and `Plugins.ExistsOnDisk` must be `0` or `1`.
- `Plugins.ImportState` must be `Current`, `Changed`, `Missing`, `Failed`, or `Unsupported`.
- `Plugins.RecordCount`, every typed-record `FormKey_ID`, and `FormListItems.Item_FormKey_ID` must be non-negative.
- `GameSetting.IsCompressed` and `GameSetting.IsDeleted` must be `0` or `1`.

## Inferred Relationships

These columns contain record-reference data but are not declared SQLite foreign keys. They are intentionally omitted
from the Mermaid relationship lines:

- `FormList.AddToListFormKey`
- `FormListItems.Item_ModKey_Name`, `Item_ModKey_Type`, `Item_ModKey_FileName`, and `Item_FormKey_ID`
- `Keyword.AttractionRuleFormKey`
- `NPC.VoiceFormKey`, `RaceFormKey`, `CombatOverridePackageListFormKey`, `CombatStyleFormKey`,
  `DefaultPackageListFormKey`, and `CrimeFactionFormKey`
- `MagicEffect.ActorValue2FormKey`, `ResistValueFormKey`, `PerkToApplyFormKey`, `EquipAbilityFormKey`,
  `ExplosionFormKey`, `CastingArtFormKey`, `HitEffectArtFormKey`, `HitShaderFormKey`, `ImageSpaceModifierFormKey`,
  `ImpactDataFormKey`, and `ProjectileFormKey`
