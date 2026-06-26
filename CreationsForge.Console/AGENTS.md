# CreationsForge.Console rules

This folder contains command-line entry points and console orchestration.

## Boundaries

- Console code should parse arguments, configure the application, call services, report progress, and return meaningful exit codes.
- Do not put import business rules, SQL, repository logic, migration logic, or Mutagen field mapping directly in the console entry point.
- Do not reference Avalonia or presentation-only types.
- Reuse Bootstrap and Core registrations rather than creating a second composition pattern.

## CLI behavior

- Keep existing command names, arguments, and environment-variable behavior stable unless a change is explicitly approved.
- New arguments must be documented in the plan and reflected in user-facing documentation if they affect workflows.
- Validate arguments before starting long-running work.
- Use user-friendly console messages for expected problems and structured Serilog logs for diagnostics.
- Avoid writing output files to surprising locations. Temporary databases, exports, and logs must use explicit paths or existing project conventions.

## Imports and extraction

- Console-driven imports should use the same services and invalidation behavior as the application.
- Do not silently fall back to stale data after database, game path, load order, or Mutagen failures.
- If a command operates across Starfield, Fallout 4, and Skyrim, report per-game success, skip, and failure details.

## Validation

When console behavior changes, include command examples and expected exit behavior in the validation plan.
