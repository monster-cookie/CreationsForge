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

Records that expose condition rules implement `IHasConditionsDTO`. The current shared DTO shape uses
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

Condition parameter import must treat Mutagen link wrapper values as references when they expose a nested `FormKey`,
`FormKeyNullable`, link, reference, target, object, item, or value property. The persisted parameter keeps both the
display value and the nullable FormKey when a reference is available.

Condition import must preserve every entry in a condition list by `Condition_Index`. For example, Starfield
`ActorIsPrey` (`CNDF:00246E86`) has two `HasKeyword` rules: one for `ActorTypePrey` and one for
`ActorTypePredator`. Both rules must persist and render independently.

Shared/Core workflow code should call `IRecordChildImportService`; that service dispatches `IHasConditionsDTO`
records to `IConditionRuleImportService`. New condition-bearing record types should use this path rather than adding
record-specific condition repositories or tables.

## Comparison Display

Record comparison output uses structural condition row labels such as `Condition [0]` so matching condition indexes
align across plugins. Each plugin value cell contains the readable condition expression, including the condition run-on
type, friendly condition data name, first and second parameters when present, compare operator, and comparison value.
Normal comparison output hides diagnostic child rows for Mutagen object type, condition data type, comparison fields,
and persisted parameters. Those details remain available in DTOs and persistence for future write/export behavior.
Existing imported rows that persisted a wrapper type name instead of a FormKey need reimport before the fixed parameter
extraction can display the real referenced form.
