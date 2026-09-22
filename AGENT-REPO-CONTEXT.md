# Repository context

These repository-owned settings apply to CreationsForge. Unconfigured or unavailable external services affect only work that needs them; there is no global policy discovery or override system.

## Repository and toolchain

| Setting | Value |
| --- | --- |
| Project name | `CreationsForge` |
| Repository URL | `https://github.com/monster-cookie/CreationsForge` |
| Supported games | Starfield, Fallout 4, and Skyrim Special Edition |
| Toolchain | .NET 10, Avalonia desktop application, and local stdio MCP server backed by the shared headless engine |

Use the current checkout as the repository path. Verify its remote against the configured repository before publishing. Keep machine-specific paths and secrets in protected local configuration outside the repository.

## Build and verification entry points

Choose existing checks that exercise the changed behavior. Inspect the selected project, script, SDK requirements, and side effects before execution; these entries do not authorize installation, publication, live game-data changes, or dependency changes.

| Entry point | Purpose |
| --- | --- |
| `dotnet restore ./CreationsForge.sln` | Restore existing solution dependencies, matching the CI build sequence. |
| `dotnet build ./CreationsForge.sln --configuration Release --no-restore` | Build the solution after restore, matching CI. |
| [CreationsForge.UnitTests](CreationsForge.UnitTests/CreationsForge.UnitTests.csproj) | Select focused unit tests for affected non-UI behavior. |
| [CreationsForge.PresentationTests](CreationsForge.PresentationTests/CreationsForge.PresentationTests.csproj) | Select Avalonia/headless presentation checks and relevant workflow fixtures. |
| [.github/workflows/ci.yml](.github/workflows/ci.yml) | Inspect current CI configurations and commands before relying on their coverage. |

A build or fixture pass does not establish packaged, installed-game, or live desktop-plus-MCP acceptance. Apply the shared [verification guidance](AGENTS.md#verification-and-communication) and the application-specific checks below.

## GitHub

The target is the repository URL above, verified against this checkout and the task. A changed origin does not authorize a different target.

| Setting | Value |
| --- | --- |
| Tool | Configured local GitHub MCP (`mcp__github__*`) running `github-mcp-server` over stdio; wrapped GitHub CLI as the fallback below |
| Authentication method | GitHub App installation tokens minted internally by the MCP server. CLI operations use the installed `Invoke-GitHubAppGh.ps1` wrapper, which mints a fresh installation token, passes it only to the child as `GH_TOKEN` with a dedicated `GH_CONFIG_DIR`, and discards and revokes it after the command. Authorized Git transport uses the same wrapper with `-Git`, a process-only `gh auth git-credential` helper, and interactive prompting disabled. |
| Expected identity | The GitHub App identified by `GITHUB_APP_ID` through the installation identified by `GITHUB_APP_INSTALLATION_ID`, with access to this repository |
| Connection / credential source | Protected local environment variables `GITHUB_APP_ID`, `GITHUB_APP_INSTALLATION_ID`, and `GITHUB_APP_PRIVATE_KEY_PATH`. Resolve values inside the consumer, require an existing PEM file, and never print or persist resolved IDs, paths, keys, or tokens. |
| Verification | Through the actual consuming MCP or wrapped CLI connection, verify the expected App installation and its access with a read-only installation-repository query and a query of the exact repository. Require `github.com`, `https://api.github.com`, and agreement with the configured repository and task. Installation tokens have no GitHub user identity; `get_me` and `gh api user` do not verify them. Before authenticated Git mutation, verify transport and the exact destination through a read-only operation using the wrapper's `-Git` mode. |
| Fallback | Installed `Invoke-GitHubAppGh.ps1` with `gh` when MCP is unavailable, using the same App identity and target. No direct or ambient CLI authentication, personal-account fallback, PAT, Proton Pass, or browser-login substitution. |
| Commit author and committer | `MonsterCookieAI <venworksai@venworkscreations.com>` |

Apply attribution only to the individual authorized commit command with `git -c user.name="MonsterCookieAI" -c user.email="venworksai@venworkscreations.com" commit ...`. Do not combine it with conflicting author/committer overrides. Verify both identities in the resulting commit before pushing; a mismatch does not authorize rewriting history. Attribution does not establish API or transport identity.

Preserve personal browser, GitKraken, ordinary CLI authentication, signing, and persistent Git settings. Do not invoke authenticated Git outside the wrapper, run `gh` directly, change shared credential helpers, or persist User/Machine `GH_TOKEN` or `GITHUB_TOKEN`. Stop dependent operations on an identity, target, credential, or transport failure and continue independent work under the shared [identity and delivery boundaries](AGENTS.md#external-tools-and-identities).

## Optional issue tracker

| Setting | Value |
| --- | --- |
| Provider | Linear |
| Workspace / organization | Venworks |
| Team / repository scope | Creations Forge (`VWCF`) |
| Project scope | No project configured; resolve current issue relationships when needed rather than inferring a project or hierarchy |
| Tool | Configured `mcp__linear_codex__*` connection |
| Authentication method | OAuth app-user connection; reuse after verifying its actual identity. Browser, shell, GitKraken, and Proton Pass sessions do not change this connection. |
| Expected identity | Active `Venworks AI Agent User` in the Venworks workspace |
| Connection / credential source | Managed Linear OAuth connection; no repository token or Proton Pass reference |
| Verification | Through the same consuming connection, call `get_user` with `query="me"` and confirm the active expected account; call `get_workspace` for Venworks and `get_team` with `query="VWCF"` for Creations Forge. Resolve provider IDs through that connection, use team scoping wherever supported, and verify each target issue or document belongs to the resolved team. Stop affected work on mismatched or ambiguous identity or scope. Verify assignment eligibility separately when assigning work. |
| Fallback | None |

Use stable identifiers or canonical URLs and only the scopes required by the selected provider. Do not assume UUIDs, a parent/child hierarchy, or specific MCP names or endpoints. For no tracker, set provider and tool to `none` and the remaining configurable tracker fields to `not applicable`.

When an issue governs the task, verify that it belongs to the intended scope and read its requirements, acceptance criteria, relevant discussion, and dependencies. The issue supplies current task requirements; repository source and documentation supply technical contracts and recorded evidence. Resolve material conflicts before dependent work, and refresh issue information when relevant changes may affect the result. A fully specified local request needs no invented issue or tracker bookkeeping.

Use the provider's actual workflow and the user's requested actions. No fixed state transition is required before coding unless the project or task requires it. Resolve real ownership conflicts, but do not treat empty assignments as blockers. Preserve assignee and agent-delegate fields unless changing them is explicitly authorized; connector attribution is separate from ownership. A prepared handoff does not require a status change. Use the shared [external-action boundaries](AGENTS.md#external-tools-and-identities) for comments, updates, and completion, without inventing claims, locks, or substitute tracker state.

### Tracker-derived roadmaps

When requested, select issues using the project's actual statuses, labels, milestones, and the requested criteria; clarify ambiguous selection only when it matters. Preserve scope, dependencies, and meaningful grouping without counting a parent and its children as separate promises for the same outcome. Present a current snapshot, not invented release dates or commitments. Refresh when relevant changes are expected and identify incomplete retrieval. Preparing content does not authorize publication.

## Documentation destinations

Follow the shared [documentation placement rules](AGENTS.md#documentation-placement). Linear issues supply current governing requirements; source, tests, and configuration establish implemented behavior. Historical Plane annotations are provenance, not current hierarchy or requirements. Do not substitute Plane or Codecks for current Linear information.

| Setting | Value |
| --- | --- |
| Public/user documentation | `README.md`, `SECURITY.md`, `CHANGELOG.md`, `Documentation/KNOWN-ISSUES.md`, and the retained `Documentation/ROADMAP.md` summary |
| Public developer/integration documentation | Approved user-facing MCP/API integration guidance in `README.md` or `Documentation/`; internal architecture and maintainer guidance use the internal destination |
| Internal project documentation | Documents verified to belong to the Creations Forge team in Venworks Linear, using the existing [engineering documentation index](https://linear.app/venworks/document/engineering-documentation-fa3d2c85e329) as a discovery entry point |
| Temporary plans, execution notes, and handoffs | Relevant Linear issue when one governs the work; otherwise the current task conversation. Disposable local artifacts may use ignored `.work/`. |
| Additional edit restrictions | `CHANGELOG.md`, `Documentation/KNOWN-ISSUES.md`, and migrated human-maintained naming content require an explicit user request and approved scope. Existing authorization for those changes is sufficient. |

Read relevant current internal contracts and guidance before dependent application changes. Resolve current document destinations through the configured Linear connection and verify team scope before use; listed links are discovery references, not proof of current access, content, or privacy. Preserve the distinction between proposals, implemented behavior, historical evidence, and remaining acceptance. The public roadmap summarizes current tracker information and is not an independent backlog.

Keep internal technical contracts, architecture, domain design, implementation guidance, research, durable validation evidence, and maintainer runbooks in the configured internal destination. Do not create local mirrors of migrated technical documents or restore the deleted `Documentation/DESIGN-DECISIONS.md` as a source of truth. Agent instructions and execution settings remain local. Public integration guidance should explain supported consumer behavior without publishing internal research.

Include documentation impacts when architecture, domain behavior, schema, persistence, DI, logging, workflows, interfaces, or validation behavior changes. If none apply, state `Documentation impacts: None.` Resolve material code/documentation conflicts before dependent changes, and reference source rather than duplicating large listings.

## Credential setup

Use each service's connection / credential source entry above to identify its managed connection or selected credential manager. These are non-secret configuration descriptions, not executable login commands or credential values. Keep private credential selectors and authentication state in protected local configuration and follow the shared [identity boundaries](AGENTS.md#external-tools-and-identities). Credential-manager setup is needed only when an authorized operation cannot use an existing verified connection.

For a service using Proton Pass CLI (`pass-cli`), the bootstrap credential is the protected `PROTON_PASS_PERSONAL_ACCESS_TOKEN` environment variable supplied by local setup. It is separate from the downstream service credential and must never be stored as a Proton Pass item or represented by a `pass://` reference. The service's expected identity above names the downstream account or app, not the credential-manager session. Optional token-name metadata is not a prerequisite for a healthy session.

For authorized setup or recovery, consult the installed CLI's help and current provider documentation, such as the [Proton Pass CLI documentation](https://protonpass.github.io/pass-cli/). Use task-owned session state without logging out or changing the user's default session. Detailed login, credential-transfer, and cleanup commands depend on the selected tool and local setup; they are not part of the mod-development workflow.

## Application context and project layout

CreationsForge currently targets .NET 10 in `CreationsForge.sln` and provides an Avalonia desktop application plus a dedicated local stdio MCP server for Starfield, Fallout 4, and Skyrim Special Edition. The former SQLite import backend and Console import harness have been retired. A general-purpose Console project may be reintroduced later for one-off plugin command-line operations that do not warrant launching the desktop application.

The implemented product architecture provides a reusable MCP interface for creating and editing Bethesda plugins, including ESMs, backed by a headless engine shared with the retained Avalonia UI. The current plugin contract is recorded in [Native FormList MVP Contracts](https://linear.app/venworks/document/native-formlist-mvp-contracts-e708102301b2). Keep implementation claims scoped to completed and recorded validation; generated-fixture evidence does not establish installed-game, packaged, mod-manager, gameplay, or live desktop-plus-MCP conflict acceptance.

Use `CreationsForge` consistently in code, comments, documentation, examples, paths, and user-facing text. When touching stale internal project names, correct them within the approved scope. Preserve exact external project names and historical references when they identify an external source.

| Project or directory | Current responsibility |
| --- | --- |
| `CreationsForge` | Avalonia views, view models, commands, navigation, dialogs, and asset preview presentation. |
| `CreationsForge.Mcp` | Local stdio MCP transport, closed protocol schemas, domain tools, workspace registry, metadata paging, and protocol lifecycle and exit behavior. |
| `CreationsForge.Bootstrap` | Autofac composition, shared startup registration, and Serilog configuration. |
| `CreationsForge.Core` | UI-neutral contracts, configuration, and existing legacy services pending replacement; not the record authority. |
| `CreationsForge.Specification` | Production game and record metadata, record-family specifications, and reusable validation specifications. |
| `CreationsForge.Migrations` | Existing legacy DbUp migration execution and SQLite schema scripts pending replacement. |
| `CreationsForge.Bethesda.Assets` | UI-neutral Bethesda archive and asset IO, lookup contracts, and preview readers. |
| `CreationsForge.Starfield` | Starfield-specific Mutagen integration and record mapping. |
| `CreationsForge.Fallout4` | Fallout 4-specific Mutagen integration and record mapping. |
| `CreationsForge.Skyrim` | Skyrim-specific Mutagen integration and record mapping. |
| `CreationsForge.UnitTests` | Unit tests for testable non-UI behavior. |
| `CreationsForge.PresentationTests` | Avalonia/headless tests, view-model workflows, and presentation harnesses. |
| `CreationsForge.DataValidationTests` | Existing legacy validation harness; it does not establish plugin authoring acceptance. |
| `Documentation` | Retained user-facing documentation and links to technical content maintained in Linear team documents. |
| `.github` | CI, release packaging, and repository automation. |

The root `AGENTS.md` is the only directory-level `AGENTS.md` file and governs repository-wide instructions. Directory-specific agent files are not part of the current contract; do not create or rely on them without explicit authorization. Use this context and the shared root guidance for current rules; task-specific skills live in `.agents/skills`.

## C# implementation conventions

- Do not use C# primary constructors unless explicitly requested.
- Use one class per file unless an established local pattern requires otherwise.
- Use braces for conditionals and loops, preserve existing line endings, and avoid unrelated formatting changes.
- Prefer clear implementations and interfaces that represent behavior actually consumed by shared infrastructure.
- All new or modified C# types and members must have meaningful `///` XML documentation. Explain purpose and behavior, parameters and type parameters, return values, nullable behavior, important side effects, and expected exceptions where relevant. Keep existing documentation accurate; do not add placeholder comments or comments that merely repeat a symbol's name.

## Application boundaries, dependency injection, and logging

- Keep Avalonia controls, bindings, view models, commands, and navigation in presentation projects. Presentation code must call the engine through UI-neutral contracts and must not own record I/O or Mutagen state.
- Keep backend contracts and result objects UI-neutral. Game-specific behavior belongs in the relevant game adapter unless the behavior is truly shared.
- Preserve existing UI interactions and rendering unless the approved plan changes them. Replacing a backend may require reworking view-model dependencies without restyling the views.
- Long-running work must not block the UI thread. Update bound collections on the UI thread and use the existing asynchronous command and dispatcher patterns.
- Keep asset preview failures isolated from the rest of the application. Dispose graphics resources, streams, native handles, and preview lifetimes deterministically.
- Use Autofac and constructor injection. Keep container resolution in composition roots and make dependency lifetimes explicit.
- Use Serilog with structured logging templates rather than interpolated messages. Services own workflow summaries; repositories and stores remain persistence-focused and do not log unless an applicable existing local rule explicitly permits it.
- Do not log full binary payloads or large serialized records. The shared rules also prohibit logging secrets and credentials.

## Consolidated project boundaries

- `CreationsForge.Core` remains UI-neutral and game-agnostic where behavior is truly shared. Core may expose UI-neutral contracts, result objects, progress callbacks, events, asynchronous methods, and collection interfaces, but must not reference Avalonia, console entry-point concerns, or game-specific Mutagen packages. Record state and mutation belong to the proposed headless engine rather than a Core repository or DTO layer.
- `CreationsForge.Bootstrap` owns shared Autofac composition, configuration, and logging setup. Keep registrations centralized, avoid duplicate registrations, use explicit lifetimes, avoid captive dependencies, and do not manually instantiate services where DI is available. Use `SingleInstance` only for stateless infrastructure, durable app-wide state, or existing singleton contracts. Configuration paths, defaults, environment variables, and ProgramData locations require plan coverage when changed. Bootstrap changes should include an application or console startup smoke path when practical.
- `CreationsForge.Mcp` owns local stdio transport, closed MCP input and output schemas, domain-tool adapters, MCP workspace ownership, metadata paging, protocol diagnostics, and process exit behavior. It calls the shared headless engine through UI-neutral contracts, keeps protocol frames on standard output and diagnostics on standard error, and must not contain game-specific record mutation or persistence rules. MCP behavior changes require protocol-level tests and copyable client launch examples in the validation plan.
- `CreationsForge` owns Avalonia views, view models, commands, dialogs, navigation, and presentation-only services. Keep code-behind minimal, preserve existing user workflows unless the plan calls for a change, keep UI-bound updates on the UI thread, and keep long-running operations asynchronous. Presentation code must not call Mutagen directly or own record state.
- `CreationsForge.Bethesda.Assets` owns UI-neutral BA2/BSA archive parsing, normalized lookup, and asset metadata. Prefer streaming and indexed lookup, dispose archive and decompression resources deterministically, preserve path normalization, support only inspected compression variants, do not extract into the repository, and identify temporary-file location and cleanup in the plan. Asset changes need focused fixture coverage and manual validation against a known archive when automated coverage is not practical.
- `CreationsForge.Starfield`, `CreationsForge.Fallout4`, and `CreationsForge.Skyrim` own game-specific Mutagen APIs, record quirks, and record mapping. Before using a property or collection, inspect the installed package, current repository usage, and authoritative Mutagen sources; do not infer APIs from record type names. Preserve game-specific differences and plan equivalent plugin support or an explicit approved exclusion when a record exists across games.
- `CreationsForge.PresentationTests` owns headless Avalonia and UI-facing validation helpers. Use deterministic dispatcher synchronization, avoid arbitrary sleeps and machine-specific paths, clean up temporary UI and database resources, and keep test-only helpers out of Core.

## Bethesda record references and modeling

Use these primary references when working with record shapes:

- [Mutagen documentation](https://mutagen-modding.github.io/Mutagen/)
- [Mutagen source](https://github.com/Mutagen-Modding/Mutagen)
- [Spriggit source](https://github.com/Mutagen-Modding/Spriggit) when serialized field compatibility is in scope.

- Inspect the installed Mutagen packages, actual APIs, existing code, and source references before using a property or record collection. Do not infer record fields from names alone.
- Use canonical Mutagen, Spriggit, xEdit, and Creation Kit field names. Explain source-name conflicts in the plan before selecting a CreationsForge-specific alternative.
- Keep game-specific fields game-specific and handle Starfield, Fallout 4, and Skyrim consistently where a record family exists. Identify game-specific behavior and proposed exclusions explicitly in the approved scope.
- Use typed fields, collections, references, and serialization paths for readable record data. Do not introduce a custom record model, shadow DTO store, cache, index, or Mutagen `LinkCache` for plugin authoring.

## Record completeness

A record change must cover the applicable Mutagen source read, typed plugin mutation, guarded output save, reopen verification, and preservation paths for Starfield, Fallout 4, and Skyrim when the record exists in each game. The FormList contract is proposed architecture and does not itself authorize implementation changes.

- Do not mark missing child data, comparison rows, UI behavior, validation coverage, or required documentation as deferred, a follow-up, or out of scope without an explicitly approved exclusion.
- Do not add TODO, placeholder, or not-yet-implemented statements as substitutes for approved behavior.

## Testing and validation

- Use xUnit, Moq, and Shouldly according to the applicable project patterns. Test applicable engine contracts, services, factories, validators, and pure business behavior with small deterministic fixtures.
- Use `CreationsForge.PresentationTests` for Avalonia/headless behavior and UI-facing workflows. Keep UI test helpers out of Core.
- Unit tests must not depend on local game installations, user-profile paths, ProgramData state, or private data. Identify external-data and disposable-game-fixture prerequisites separately for plugin integration or manual acceptance checks, and skip or clearly mark those checks when the applicable harness permits it.
- Explain when tests are not added, and identify the appropriate manual or integration validation.
- FormList acceptance must cover the proposed Mutagen-backed contract across Starfield, Fallout 4, and Skyrim, including guarded output save, reopen verification, preservation of source plugins and unedited output data, and evidence appropriate to the actual implementation. Build, packaging, documentation, or startup smoke checks alone do not prove plugin serialization or game-runtime acceptance.

Use check-only formatting where available for verification. Scope any approved formatting fixes to touched files; do not run solution-wide formatting as an automatic cleanup step. Instruction-only or documentation-only changes need proportional content, link, and diff checks rather than an unrelated application build.
