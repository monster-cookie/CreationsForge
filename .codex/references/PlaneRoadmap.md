# Plane roadmap extraction

Use this procedure only when the task requests public roadmap content derived from Plane. Read the project identity in [`AGENT-REPO-CONTEXT.md`](../../AGENT-REPO-CONTEXT.md) and the repository rules in [`AGENTS.md`](../../AGENTS.md). Roadmap work is read-only and does not require unrelated repository references or implementation work.

## Current pending snapshot

1. Query only the canonical project and refresh its complete current work-item inventory.
2. Treat items whose current state group is `backlog` or `unstarted` as pending by default; exclude `started`, `completed`, and `cancelled` items unless the user explicitly requests those sections.
3. Resolve and apply the relevant product label, such as `minimalist`; title matching alone is not sufficient.
4. Retrieve every selected item in full and verify its identifier, title, state, type, labels, parent, description, dependencies, and relationships as applicable. Preserve Epic and Task hierarchy and do not list one outcome again as both an Epic and an ungrouped Task.
5. Translate internal implementation wording into clear player-facing language without changing the promised outcome. Do not invent dates, release versions, ordering, commitments, compatibility, acceptance criteria, or other delivery promises.
6. Refresh the canonical inventory and fully read the selected items immediately before publication. Reconcile state, label, hierarchy, dependency, relationship, title, and description changes; if the snapshot changed, update the selection or report the evidence gap.

The result is a current snapshot, not a promise that every pending item will ship. If the canonical project, inventory, selected-item reads, or final refresh cannot be verified, report the affected scope and do not substitute Codecks, historical memory, another project, guessed requirements, or a local backlog draft.