# Condition Rules

Condition rules are a shared Bethesda child-record system, not a `CNDF`, `FACT`, or `COBJ`-specific persistence
shape. Starfield condition forms are game-specific, but condition rule lists also exist on Fallout 4 and Skyrim
records.

## Current Storage

- Persist condition envelopes in `ConditionRules`.
- Persist condition data fields in `ConditionRuleParameters`.
- Use `RecordType` to identify the owning parent record type.
- Use `ConditionSlot` to identify the owning condition list on parent records. The default slot is `Conditions`.
- Do not persist condition rule data as raw binary or opaque raw payload rows when Mutagen exposes structured fields.

## Current DTO Path

Records that expose condition rules implement `IHasConditionsRecordDTO`. The current shared DTO shape uses
`ConditionFormConditionDTO` and `ConditionFormConditionParameterDTO` because those DTOs existed before the storage was
generalized. The persistence and import service names are intentionally `ConditionRules` and
`ConditionRuleParameters`.

## Current Users

- `CNDF` uses `RecordType = 'CNDF'` and `ConditionSlot = 'Conditions'`.
- `FACT` uses `RecordType = 'FACT'` and `ConditionSlot = 'Conditions'`.
- `COBJ` uses `RecordType = 'COBJ'` and `ConditionSlot = 'Conditions'`.

## Import Rules

Game-specific readers map Mutagen condition objects into the shared condition DTO shape. If a field cannot be
understood as a scalar string, enum string, numeric value, boolean, or FormKey reference, stop and identify the field
instead of storing it as raw binary.

Shared/Core workflow code should call `IRecordChildImportService`; that service dispatches `IHasConditionsRecordDTO`
records to `IConditionRuleImportService`. New condition-bearing record types should use this path rather than adding
record-specific condition repositories or tables.
