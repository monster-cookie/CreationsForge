# Linear team documentation

Use this procedure when technical project documentation, design decisions, research notes, validation evidence, or maintainer runbooks need a durable home.

## Documentation boundary

- Keep user and public documentation in the repository when it supports product discovery, installation, usage, security reporting, release history, or user-facing known issues.
- Store technical contracts, architecture, domain design, implementation guidance, research findings, validation evidence, and maintainer runbooks in documents verified to belong to the canonical Creations Forge team in the Venworks Linear workspace.
- Treat `CHANGELOG.md`, `Documentation/KNOWN-ISSUES.md`, and migrated human-maintained naming content as approval-gated documentation. Moving or indexing that content does not authorize edits.
- Keep repository agent instructions, credential and tooling policies, and Linear lifecycle procedures local because they govern repository and tool execution.
- Do not create a second local technical copy after a document has been migrated to Linear. This procedure does not authorize Linear document creation, edits, comments, or attachments; verify the target and obtain explicit authorization before any mutation.

## Team identity, document access, and privacy

- Verify the consuming app user, workspace UUID, and canonical team UUID from `AGENT-REPO-CONTEXT.md` before dependent document operations. Use team scoping when available and verify the returned document's team identity after an unscoped retrieval.
- Resolve current document IDs and URLs from Linear readback. Do not invent or rely on remembered document names, URLs, or list positions. Re-read the selected document before a consequential write and after any authorized mutation.
- A team visibility value or an accessible document URL does not prove that a document is private, owner-only, or unpublished on the web. Treat web-publishing status as unverified when the consuming interface does not expose it; verify any required privacy property through a supported surface before a write that depends on it.
- Migrated documents may still say Plane owns current work or contain old Plane links. Treat their original-source notes as historical provenance and reconcile active requirements with current Linear issues and documents before decisions or edits.

## Current document index

The verified team index is [CreationsForge engineering documentation](https://linear.app/venworks/document/engineering-documentation-fa3d2c85e329). Use current Linear readback and that index to select a destination; the index's migrated prose itself still needs terminology cleanup. The migrated source snapshot was repository commit `4d084a0ac0153d027ef56aa6b2beb3310f6483fd`.

| Linear destination | Repository material covered |
| --- | --- |
| [Native FormList MVP Contracts](https://linear.app/venworks/document/native-formlist-mvp-contracts-e708102301b2) | Native engine contract and cross-game field and operation requirements; planned contracts are separate from completed acceptance evidence. |
| [Native Backend Replacement Plan](https://linear.app/venworks/document/native-backend-replacement-plan-e8e86b562d53) | Superseded SQLite backend inventory, implementation sequence, and remaining decisions. |
| [CI and Native FormList Validation Handoff](https://linear.app/venworks/document/ci-and-native-formlist-validation-handoff-ddd4408d99c4) | Prior source-baseline evidence and acceptance instructions, with snapshots kept distinct. |
| [Presentation Test Guide](https://linear.app/venworks/document/presentation-test-guide-3a0d0ca522a3) | Avalonia/headless presentation test practices. |
| [Arch Packaging Guide](https://linear.app/venworks/document/arch-packaging-guide-8e825fb60fc2) | Maintainer Arch release-package operations. |
| [OpenRouter Chat Authoring Architecture and Implementation Plan](https://linear.app/venworks/document/openrouter-chat-authoring-architecture-and-implementation-plan-832e4061b004) | Proposed chat-authoring design and decisions; current issue scope must be read separately. |

`Documentation/ROADMAP.md` remains a public summary, not an independent current backlog. Preserve implemented behavior, proposed design, historical findings, and unverified acceptance as separate claims. Link to source files and tests where authoritative, but do not duplicate large implementation listings in Linear.

## Migration and handoff

Remove local links to intentionally deleted technical documents only within an authorized documentation scope and replace them with verified Linear links after readback. A documentation handoff should identify the destination, source, preserved sections, changed terminology, verified links, and remaining evidence or privacy gaps. A document created or updated without successful readback is not a completed migration.
