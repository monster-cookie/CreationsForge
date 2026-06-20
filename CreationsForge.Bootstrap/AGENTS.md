# CreationsForge.Bootstrap rules

This folder owns application composition and shared startup wiring.

## Boundaries

- This project should wire dependencies, configuration, logging, and application services.
- Do not put business logic, import logic, SQL, Mutagen field mapping, UI workflows, or test-only helpers here.
- Keep Autofac registrations centralized and discoverable.
- Avoid duplicate registrations across application, console, Core, and game-specific projects.

## Autofac

- Prefer constructor injection.
- Keep container resolution in composition roots only.
- Register services, factories, stores, repositories, importers, and game-specific modules according to existing patterns.
- Use SingleInstance only for stateless infrastructure, durable app-wide state, or services already treated as singletons.
- Avoid captive dependencies, especially singleton services depending on shorter-lived objects.
- Do not manually instantiate services where DI is available.

## Configuration and logging

- Configuration paths, default values, environment variables, and ProgramData locations must be called out in the plan when changed.
- Serilog setup belongs in startup/composition code, but logging behavior should remain consistent with the service layer.
- Do not log secrets, connection strings, full payloads, or large serialized records.

## Validation

When Bootstrap changes, run the normal restore/build/tests and include at least one application or console startup smoke path in manual validation if practical.
