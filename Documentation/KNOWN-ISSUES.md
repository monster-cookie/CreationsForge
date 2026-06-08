# Known Issues

- Only a subset of the record types are supported at this time.
  - The supported record types are:
    - FormList (FLST)
    - GameSetting (GMST)
    - Global (GLOB)
    - MiscItem (MISC)
    - Keyword (KYWD)
    - NPC (NPC_)
    - ActorValueInformation (AVIF)
    - MagicEffect (MGEF)
    - Perk (PERK)

- Some supported record types have partial detail coverage. The current implementation intentionally persists clearly
  understood scalar fields and direct `FormKey` references first. The following omitted child structures are
  representative examples, not an exhaustive list:
  - MiscItem (MISC), resource mappings are deferred until Resource records are supported.
  - NPC (NPC_) currently imports selected scalar fields and direct references only. Inventory, abilities, perks,
    factions, keywords, AI packages, appearance data, object templates, and other nested structures are not
    imported.
  - FormKey and ModKey lists are showing raw information instead of the more helpful EditorID.
