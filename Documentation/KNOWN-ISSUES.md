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
  - MiscItem (MISC) does not import nested object bounds, model, destructible, keyword, or virtual-machine adapter data.
  - Keyword (KYWD) does not import virtual-machine adapter script data.
  - NPC (NPC_) currently imports selected scalar fields and direct references only. Inventory, abilities, perks,
    factions, keywords, AI packages, scripts, appearance data, object templates, and other nested structures are not
    imported.
  - ActorValueInformation (AVIF) does not import virtual-machine adapter data.
  - MagicEffect (MGEF) currently imports selected scalar fields and direct references only. Conditions, keywords,
    sounds, components, scripts, archetype data, and some direct references are not imported.
  - Perk (PERK) currently imports selected scalar fields only. Ranks, background skills, script adapter data,
    restriction and training references, category, and major flags are not imported.
