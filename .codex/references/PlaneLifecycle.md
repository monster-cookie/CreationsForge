# Plane-backed lifecycle

Use this procedure after the repository context identifies the task as Plane-backed. Read the shared identity, ownership, mutation, and failure boundaries in [`AGENT-REPO-CONTEXT.md`](../../AGENT-REPO-CONTEXT.md) and [`AGENTS.md`](../../AGENTS.md) first. This reference supplies the operational sequence for startup, implementation, recovery, handoff, and completion.

In this reference, an external action is authorized only when the current user request or an approved task-specific plan explicitly permits it. Permission to edit local files is not Plane permission. Final completion also requires the separate action-time user confirmation described below.

## Establish the current identity

1. Verify that Plane MCP is available and authenticated, then list projects and match the canonical project UUID configured in `AGENT-REPO-CONTEXT.md`.
2. Verify the returned project, record its current identifier and name, and use the canonical UUID for every operation that accepts `project_id`. A verified identifier or name change does not change the canonical UUID and does not justify an automatic context-file edit.
3. When work is governed by a particular work item, retrieve it with project scope when supported, verify its returned project matches the canonical UUID, and retain its current human-readable identifier and full UUID before dependent decisions or mutation. Inventory-only roadmap work does not require inventing a governing Task; verify each selected item's project instead.
4. For an operation without `project_id`, retrieve the complete identifier and perform the same project check before reading related data or mutating anything. Verify the type, state, assignees, labels, parent, relationships, and dependencies relevant to the task.

A missing or wrong UUID, ambiguous project identity, wrong-project work item, or unresolved ownership conflict stops the affected dependent operation. Continue authorized local analysis that does not depend on the missing evidence.

## Validate ownership and implementation prerequisites

Verify current project members before assignment. The authenticating member returned by `member me` is not automatically the intended work-item assignee. Inspect current assignees and stop the affected work when another person or agent has conflicting ownership; assignment or unassignment requires explicit authorization.

While preparing the plan, identify the governing Task, ready dependencies, intended Codex ownership, and verified `In Progress` state. These are mandatory prerequisites before dependent implementation.

For a new intake, verify that the Task is in `Backlog` or `Todo`, inspect its parent, description, comments, labels, dependencies, relationships, assignment, and definition of done, and then move it to `In Progress` only through an explicitly authorized transition followed by readback. If the Task is already in a manually established or existing `In Progress` state, verify that state, ownership, requirements, and dependencies directly; do not require a new `Backlog` or `Todo` step.

If a prerequisite is missing, unauthorized to change, or cannot be verified, pause dependent implementation and continue only authorized independent analysis. Do not claim that a skipped transition fulfilled the prerequisite.

For continuation, re-read the Task, verify that it remains `In Progress`, confirm that assignment and requirements still match the active work, and refresh comments, relationships, and dependencies before proceeding.

## Recover partial mutations and blockers

After an uncertain or partial assignment, state, comment, relationship, or other mutation, stop dependent operations and re-read the affected work item. Record the exact resulting state or error and do not retry a consequential action with an unknown outcome. Resume dependent work only after the required current state and ownership are verified; unrelated local analysis may continue.

Use configured native workflow states. Do not invent a `Blocked` state, claim, lock, host label, lock label, or relationship to simulate one. An explicitly authorized factual blocker comment may record the condition, evidence, needed person or event, and preserved repository, branch, commit, and validation state, but a comment does not create a state or lock. Ask how to represent a blocker before changing state or assignment.

## Prepare and record a handoff

Handoff preparation is read-only and may proceed without Plane mutation permission. Inspect existing comments for duplicates and establish the exact branch, baseline, commit, diff, and pull-request target. Prepare the behavior and scope, changed files and artifacts, material decisions, actual validation and runtime results, remaining checks, limitations, blockers, and available commit or pull-request identifiers.

A complete recorded handoff requires a verified `In Review` state. That evidence may come from a verified manual or existing state, or from an explicitly authorized state transition followed by readback. Add a handoff comment only when communication is explicitly authorized. If the item is not verified `In Review` and no authorized or manual transition is available, report the prepared handoff and pending transition without claiming that the recorded handoff is complete.

Keep the item in `In Review` while independent review or human acceptance remains. Plane records the handoff state; the repository or pull-request review remains authoritative for findings.

## Complete after action-time acceptance

Only the user may approve final completion. Immediately before recording final acceptance, moving the item from `In Review` to `Done`, or removing its active assignee as part of completion, obtain explicit action-time confirmation. Plan approval does not replace this confirmation.

After confirmation, re-read the item and comments, record acceptance only when communication is authorized, perform only the completion actions covered by the user's confirmation, and re-read the item to verify each result. Removing the assignee requires explicit authorization for that action; approval to mark the item Done alone does not include it. Do not request another approval for the same already-confirmed action. If confirmation or required authorization is absent, leave the item in its verified state and report completion as pending.

Use the failure boundary in the repository context for unavailable, stale, or inconsistent Plane evidence. Do not substitute Codecks, historical memory, local roadmap drafts, guessed requirements, or another task system, and do not let an external failure stop unrelated local analysis.