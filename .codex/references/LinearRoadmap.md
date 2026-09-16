# Linear roadmap extraction

Use this procedure only when the task requests public roadmap content derived from Linear. Read the team identity in [`AGENT-REPO-CONTEXT.md`](../../AGENT-REPO-CONTEXT.md) and repository rules in [`AGENTS.md`](../../AGENTS.md). Roadmap work is read-only and does not require unrelated implementation work. Treat `Documentation/ROADMAP.md` as a retained public summary; current team issues remain the source of truth for state, scope, and sequencing.

## Current pending snapshot

1. Verify the consuming app user, Venworks workspace, and canonical Creations Forge team through the same Linear connection. Query the complete current team issue inventory, following every page, rather than assuming a Linear project exists.
2. Treat issues in backlog or unstarted state types as pending by default; exclude started, completed, canceled, and duplicate issues unless the user explicitly requests them. Resolve any requested product label through current Linear data; title matching alone is not enough to supply a missing label.
3. Retrieve every selected issue in full with relations. Verify identifier, UUID, title, team, state and type, labels, native parent, description, and blocking relationships. Preserve native hierarchy when present; the current migration has no native project or parent links, so do not promote old Plane parent annotations into current Epics.
4. Translate internal implementation wording into clear player-facing language without changing the promised outcome. Do not invent dates, release versions, ordering, commitments, compatibility, acceptance criteria, or other delivery promises.
5. Refresh the team inventory and selected issue details immediately before publication. Reconcile state, label, hierarchy, dependency, relation, title, and description changes; update the selection or report an evidence gap when the snapshot changed.

The result is a current snapshot, not a promise that every pending issue will ship. If the team, full inventory, selected issue reads, or final refresh cannot be verified, report the affected scope and do not substitute Plane, Codecks, historical memory, or a local backlog draft.
