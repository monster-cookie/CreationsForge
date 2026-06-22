# CreationsForge.DataValidationTests rules

This folder contains data validation tests for validating that the mutagen/sqlite loaded DTOs match the known good data from Spriggit.

## Test scope

- Use xUnit and Shouldly according to existing patterns.
- Avoid sleeps, timing-sensitive tests, and tests that depend on file ordering unless ordering is part of the behavior.
- Test assertions must live directly in the `[Fact]` / `[Theory]` test method body. Do not put `ShouldBe`, `ShouldBeEmpty`, xUnit `Assert`, FluentAssertions, or other assertion-library calls in helper methods, local functions, shared helpers, test-only assertion methods, or loops hidden behind helpers.
- Shared helpers must not call assertion libraries such as Shouldly, xUnit Assert, or FluentAssertions.
- Helper functions should return values, comparison results, diagnostics, or unmatched-field messages; the test method decides what to assert.

## Data Validation Test Assertions

Spriggit-to-DTO comparisons in data validation test methods must be explicit and sample-specific.

- Do not use loops, dictionary iteration, reflection, or broad helper assertions to compare matching Spriggit and DTO fields inside individual validation tests.
- Each expected field mapping must be asserted by name, for example: `spriggit.Fields["EditorID"].ShouldBe(dtoFields["EditorID"]);`
- Data validation tests must not use private assertion helpers such as `AssertTranslatedField`, `AssertOptionalField`, or `AssertRecordMatches`. Inline the assertion statements in the test method that owns the sample.
- Repeated Spriggit translated-string fields must be asserted explicitly by key in the test body. Do not use loops or helper methods to assert translated field counts, target language, entry language, or entry string values.
- Collection fields must assert expected counts and indexed values explicitly for the sample being tested.
- Optional fields must assert the expected presence or absence explicitly for that sample.
- The only approved generic unmatched-field coverage helpers are `Helpers.GetUnmatchedSpriggitFields(...)` and `Helpers.GetUnmatchedDtoFields(...)`.
- Those unmatched-field helpers are a coverage backstop only. They must not replace explicit field-by-field assertions in the test method.

### Spec-driven Validation Tests (Preview)

NOTE: This is currently in preview but will eventually replace most rules in this document.

Spec-driven validation tests may use an approved validation spec runner that loads raw Spriggit YAML fields and reflected DTO fields, then returns diagnostics for the test method to assert.

Spec rules must name intentional path differences explicitly. They must preserve unmatched Spriggit and DTO coverage so missing source fields and hallucinated DTO fields remain visible.

Spec runners and helpers must not call assertion libraries. The test method remains responsible for asserting returned diagnostics.

Spec-driven validation tests may iterate assertion cases returned by an approved spec runner when each case contains explicit Spriggit path, DTO path, expected value, actual value, and failure message. The runner must not call assertion libraries.

## Imported validation database freshness

Data validation tests read DTOs from the imported SQLite database, not directly from the current mapper code.

When production import mapping, repository read-back, DTO persistence, migrations, or validation schema assumptions change, the agent must explicitly state whether the existing validation database can be reused or must be reset/reimported.

A database reset/reimport is required when:

- a mapper fix changes values already persisted in typed record tables;
- a migration is amended before release and an existing local database already recorded that migration as applied;
- repository read-back depends on newly added tables or columns;
- validation failures may be caused by stale imported rows rather than current code.

The agent must call this out in the plan and final validation notes. Building the solution is not enough to refresh imported DTO data.
