# CreationsForge.PresentationTests rules

This folder contains Avalonia/headless presentation tests and UI-facing validation helpers.

## Scope

- Use these tests for view models, headless Avalonia workflows, UI composition behavior, and manual harness validation where the UI layer is part of the behavior.
- Keep test-only helpers in this project.
- Do not move headless UI test helpers into CreationsForge.Core.
- Do not change production UI code only to make tests easier unless the plan calls out the product impact.

## Avalonia headless rules

- Initialize Avalonia test infrastructure using the existing Headless patterns.
- Avoid arbitrary sleeps. Prefer dispatcher/test framework synchronization helpers.
- Keep UI-bound assertions deterministic.
- Do not run long imports on the UI thread.
- Clean up windows, controls, temporary databases, and temp files after tests.

## Spriggit comparison harness

- Spriggit comparison tests may use these environment variables when applicable:
  - SPRIGGIT_STARFIELD_EXTRACTIONS
  - SPRIGGIT_FALLOUT_EXTRACTIONS
  - SPRIGGIT_SKYRIM_EXTRACTIONS
- Tests that require external Spriggit extraction folders must be skipped or clearly marked when the environment variables are missing.
- Compare DTO and UI comparison output against Spriggit data, but allow explicitly documented ModKey/FormKey formatting differences when approved.
- Do not store binary payloads or reflection data merely to satisfy a test unless Spriggit exposes equivalent data and the plan justifies it.

## Validation

Presentation test changes should run after the application and Core build successfully.
