# Plane project documentation

Use this procedure when technical project documentation, design decisions, research notes, validation evidence, or maintainer runbooks need a durable home.

## Documentation boundary

- Keep user and public documentation in the repository when it supports product discovery, installation, usage, security reporting, release history, or user-facing known issues.
- Store technical contracts, architecture, domain design, implementation guidance, research findings, validation evidence, and maintainer runbooks on non-web-published Plane project pages within the canonical CreationsForge project.
- Treat `CHANGELOG.md`, `Documentation/KNOWN-ISSUES.md`, and the migrated human-maintained naming content as approval-gated documentation. Moving or indexing that content does not authorize edits.
- Keep repository agent instructions, credential and tooling policies, and Plane lifecycle procedures local because they govern repository and tool execution.
- Do not create a second local technical copy after a document has been migrated to Plane.
- This procedure does not authorize Plane page creation or edits. Verify the target page and obtain explicit authorization before any Plane mutation.

## Canonical project and page privacy

- Use the canonical project UUID from `AGENT-REPO-CONTEXT.md` for every page operation that accepts a project scope.
- Verify that each destination is a Plane project page in the canonical project before reading or writing it.
- Verify `is_deployed=false` on the canonical Plane project. This is a project-level deployment setting, not a page-level privacy result.
- Read and record each destination page's access value when available. Page access describes workspace visibility and does not prove web publishing status.
- Treat page-level web-publishing status as unverified when the consuming Plane interface does not expose it. Do not claim a page is private, owner-only, or web-unpublished from an access value alone.
- Resolve page IDs and URLs from current Plane readback. Do not invent or rely on remembered page names, URLs, or list positions.
- The coordinator maintains verified page IDs and URLs. Until that map is available, use the semantic destinations in the index below and leave external links unresolved.

## Page index

The verified Plane engineering documentation index is [CreationsForge engineering documentation](https://app.plane.so/venworks/projects/874929c3-5c2e-4f0f-b0b3-fbef7b74bc5e/pages/179d4a01-77d8-45a4-ba5a-9d8d1b0d8ad5). Use that project page to resolve the current destination page IDs and URLs before making a migration handoff. The migrated source snapshot was repository commit `4d084a0ac0153d027ef56aa6b2beb3310f6483fd`.

| Plane destination | Repository material covered |
| --- | --- |
| Native backend replacement | [Replacement plan](https://app.plane.so/venworks/projects/874929c3-5c2e-4f0f-b0b3-fbef7b74bc5e/pages/e176bb4d-ad0c-4e6d-bb14-748467023614); superseded SQLite architecture, domain models, naming guidance, and import-extension pages are archived and are not active requirements |
| Application presentation | [UI and MVVM reference](https://app.plane.so/venworks/projects/874929c3-5c2e-4f0f-b0b3-fbef7b74bc5e/pages/8f4728ea-f09f-44b4-adf1-fcd039284ef8), pending revision for the native consumer, and [presentation test guide](https://app.plane.so/venworks/projects/874929c3-5c2e-4f0f-b0b3-fbef7b74bc5e/pages/1374e534-25b4-449f-b8bb-cca1e570d66c) |
| Native FormList engine | [Native FormList MVP](https://app.plane.so/venworks/projects/874929c3-5c2e-4f0f-b0b3-fbef7b74bc5e/pages/49570f3b-2fb8-4189-a053-9104a7ce6848), based on `Documentation/Engine/FORMLIST-MVP-CONTRACTS.md` and related native authoring evidence |
| Contributor and validation workflows | [Validation handoff](https://app.plane.so/venworks/projects/874929c3-5c2e-4f0f-b0b3-fbef7b74bc5e/pages/4985353a-f01a-4805-8089-d54733734de1) and the remaining migrated workflow sources indexed on the engineering page |
| Roadmap and active delivery | Plane roadmap and work items; `Documentation/ROADMAP.md` remains a public summary and is not an independent current source of truth |
| Packaging and release operations | [Arch packaging guide](https://app.plane.so/venworks/projects/874929c3-5c2e-4f0f-b0b3-fbef7b74bc5e/pages/b2e76ec3-3315-4f49-95f5-e47da51f5ef2), migrated from `Packaging/Arch/README.md` |

## Migration and maintenance

- Preserve repository terminology, source paths, command behavior, version qualifiers, evidence status, and explicit limitations when moving content.
- Separate implemented behavior, proposed design, historical findings, and unverified acceptance evidence so a page cannot be read as a stronger claim than its source supports.
- Link to source files and tests where they remain authoritative, but do not duplicate large implementation listings on the Plane page.
- Remove local links to migrated or intentionally deleted technical documents and replace them with verified Plane links only after page readback.
- Re-read the completed Plane page and compare its headings, links, code paths, commands, and evidence status with the source before declaring migration complete.
- When documenting a design decision, preserve its date, status, context, decision, rationale, alternatives considered, consequences, and related files.

## Documentation handoff

A migration handoff should identify the destination page, source file, preserved sections, changed terminology, verified links, and claims that remain unverified. A page that is created or updated without successful readback is not a completed migration.
