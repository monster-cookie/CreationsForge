# CreationsForge.DataValidationTests rules

This folder contains data validation tests for validating that the Mutagen/SQLite-loaded DTOs match known-good Spriggit YAML data.

## Test scope

- Use xUnit and Shouldly according to existing patterns.
- Avoid sleeps, timing-sensitive tests, and tests that depend on file ordering unless ordering is part of the behavior.
- Data validation tests must use spec-driven validation for Spriggit-to-DTO comparisons.
- Do not add new hand-written field-by-field Spriggit comparison tests.
- Existing non-spec validation tests should be converted to spec-driven validation when touched for record-shape, mapper, DTO, repository, schema, or validation-harness work.

## Spriggit validation coverage rules

- Unmatched Spriggit fields reported by validation specs must be treated as missing implementation by default.
- Do not fix validation failures by adding broad Spriggit ignore rules, prefix suppressions, or “currently unmodeled” exemptions.
- A Spriggit field may only be marked covered-by-alias/duplicate when the PLAN lists:
  - the exact Spriggit path,
  - the DTO/import/persistence/readback path preserving the same data,
  - why the Spriggit path is a duplicate representation rather than missing data.
- If no preserving path exists, the field must be modeled end-to-end: DTO, importer, persistence, readback, comparison/render path where applicable, and validation coverage.
- Do not delete modeled record data and replace it with validation ignores unless the user explicitly approves that exact removal.

## Spec-driven validation

Spec-driven validation tests define sample-specific mapping rules in validation specs and execute those specs through
an approved validation spec runner.

- Specs must name intentional Spriggit-to-DTO path differences explicitly.
- Specs must preserve unmatched Spriggit and DTO coverage so missing source fields and hallucinated DTO fields remain visible.
- Specs should be sample-specific when optional fields, collections, localized strings, unions, or game-specific fields vary by sample.
- Specs may use shared builders when the builder keeps rule definitions explicit and readable.
- Spec builders and validators may enforce required fields, duplicate rules, malformed paths, missing sample metadata, and other harness invariants.
- Spec runners may use reflection and raw Spriggit YAML field loading to produce comparison data.
- Spec runners must not call assertion libraries.

## Test assertions

Test assertions must live directly in the `[Fact]` / `[Theory]` test method body.

- Do not put `ShouldBe`, `ShouldBeEmpty`, xUnit `Assert`, FluentAssertions, or other assertion-library calls in shared helpers, spec runners, validators, builders, or test-only assertion helper methods.
- Shared helpers must return values, assertion cases, comparison results, diagnostics, or unmatched-field messages.
- The test method decides what to assert.
- Spec-driven tests may iterate assertion cases returned by an approved spec runner when each case contains:
  - Spriggit path
  - DTO path
  - expected value
  - actual value
  - failure message
- Coverage diagnostics returned by the spec runner must be asserted in the test method.
- If legacy unmatched-field helpers are still used for a record type, they are coverage backstops only and must not replace spec assertion cases.

## Approved validation shape

A typical spec-driven validation test should follow this shape:

```csharp
var spec = BookValidationSpecs.Starfield_NH_SouvenirSlate();
var dto = Helpers.GetDTO<BookDTO>(spec.Game, spec.RecordType, spec.FormKey);
var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(spec.Game, spec.RecordType, spec.SampleName);

var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
foreach (var assertion in assertions)
{
    assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
}

ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
```

When a record type no longer needs the legacy unmatched-field helpers because equivalent spec-runner coverage exists, the plan must call that out explicitly before removing them.

When GetCoverageDiagnostics reports unmatched Spriggit fields, first trace importer, DTO, repository save/readback,
comparison output, and validation spec coverage. Ignore rules are not an acceptable first fix for missing Spriggit data.

## Imported validation database freshness

Data validation tests read DTOs from the imported SQLite database, not directly from the current mapper code.

When production import mapping, repository readback, DTO persistence, migrations, or validation schema assumptions change, the agent must explicitly state whether the existing validation database can be reused or must be reset/reimported.

A database reset/reimport is required when:

- a mapper fix changes values already persisted in typed record tables;
- a migration is amended before release and an existing local database already recorded that migration as applied;
- repository readback depends on newly added tables or columns;
- validation failures may be caused by stale imported rows rather than current code.

The agent must call this out in the plan and final validation notes. Building the solution is not enough to refresh imported DTO data.
