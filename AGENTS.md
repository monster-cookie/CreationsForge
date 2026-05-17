Repo: SFREcordCompareEngine (DotNet 10 WPF Desktop Application)

Enterprise-grade service that serves as the primary gateway API for the Vant4gePoint front end. Solution layout:

    /SFREcordCompareEngine (WPF Desktop Application)
    /SFREcordCompareEngine.Core (Models, DTOs, Services, Repositories, Stores, and Factories)
    /SFREcordCompareEngine.UnitTests (Unit Tests)

HARD RULES

    NEVER run git (init, add, commit, stash, merge, rebase, push, pull, fetch, tag, etc.).
    NEVER modify repo history or open PRs.
    READ/WRITE SCOPE: only within /SFREcordCompareEngine unless explicitly told otherwise.
    ALWAYS show a PLAN first and wait for explicit approval before editing files.
    KEEP CHANGES SURGICAL and consistent with existing patterns & naming.
    NO breaking changes to existing services, factories, or stores without explicit approval (document migration impact and routing/versioning consequences in the PLAN).

PLAN → EXECUTE → VALIDATE

    PLAN (required prior to any edits)
        Include: scope, exact file paths, code-level checklist, data model/schema impacts, config/env changes, public HTTP API impact, logging additions, risks/rollbacks, and test plan.
        Call out whether organization scoping is affected (see “Security & Access”).
    EXECUTE (after approval only)
        Make only the approved edits.
        Show minimal diffs per file and keep edits focused.
        Do not introduce new conventions or external deps unless approved in PLAN.
    VALIDATE
        dotnet build the solution with analyzers (warnings are errors).
        Run unit tests in /SFREcordCompareEngine.UnitTests.
        Summarize: build/test results, public API changes, config/migration notes, and any SemVer/compat considerations for clients.

ARCHITECTURE & CONVENTIONS

    Contracts-first: define interfaces, DTOs, validators, and tests before implementation.
    Class-per-file. Primary constructors for controllers, services, factories, stores, and repositories where possible.
    Async: every public method is async IF it awaits anything. No sync-over-async. Accept CancellationToken on public async APIs.
    No statics for app code. Prefer DI; register singletons only when appropriate.
    Organization scoping: all requests that operate on resources containing an OrganizationID must be scoped/enforced.
    Source layout (authoritative):
        /SFREcordCompareEngine (WPF Desktop Application)
        /SFREcordCompareEngine.Core (Stores, Services, Repositories, and Factories)
        /SFREcordCompareEngine.UnitTests (xUnit + Moq + Shouldly)
    No repeated code: Refactor existing methods as needed to avoid repeating code in new methods.

TECH CONSTRAINTS

    Dependency injection: Use Autofac.
    Observability: Use existing Serilog conventions.

LOGGING & DIAGNOSTICS

    Use existing logging conventions. Prefer Information level in services (over Debug).
    No logs in repositories.

CODE QUALITY

    Analyzer warnings are treated as errors.
    Follow existing conventions in the repo. Do not introduce new naming or patterns.
    Curly braces on all conditionals and methods (no single-line omission).

TESTING

    Unit tests live in /SFREcordCompareEngine.UnitTests (xUnit + Moq + Shouldly).
    For new features/bugfixes, include tests in the PLAN and add them alongside code changes.

DATABASE & SCHEMA CHANGES

    Use parameterized SQL everywhere.

SECURITY & ACCESS

    All requests that operate on resources with OrganizationID must enforce organization scoping at the controller or service boundary.
    Respect existing authorization policies; do not widen access without explicit approval.
    Validate and sanitize all inbound parameters/filters.

OUTPUT STYLE (FOR THE AGENT)

    First output a brief PLAN (use /.github/AGENT-PLAN-TEMPLATE.md).
    Upon approval, provide file-by-file minimal diffs or full files when replacing/adding.
    Keep changes minimal and consistent with existing naming and structure.

OUT-OF-SCOPE (without explicit approval)

    Adding third-party dependencies.
    Introducing EF or changing data access technology.
    New messaging/cache frameworks.
    Any CI/CD or git actions.

REVIEW CHECKLIST (Agent)

    PLAN approved; files & impact listed
    Org scoping considered/enforced
    DTO + validator added/updated
    Service/repo interfaces updated first; implementation after
    Logging at service layer; numeric parameter templates
    No repository logs; parameterized SQL only
    Timeouts & Polly applied to I/O paths
    Tests added/updated and pass locally
    Build passes with analyzers (warnings as errors)
    Public HTTP API changes documented (and approved)
    Schema and migrations documented (and approved)
