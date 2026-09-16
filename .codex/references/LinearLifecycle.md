# Linear-backed lifecycle

Use this procedure after the repository context identifies the task as governed by a Linear issue. Read the identity, ownership, mutation, and failure boundaries in [`AGENT-REPO-CONTEXT.md`](../../AGENT-REPO-CONTEXT.md) and [`AGENTS.md`](../../AGENTS.md) first. This reference supplies the operational sequence for intake, implementation, recovery, handoff, and completion.

An external action is authorized only when the current user request or an approved task-specific plan explicitly permits it. Permission to edit local files is not Linear permission. Final completion also requires the separate action-time user confirmation below. Technical contracts, design, research, validation evidence, and maintainer runbooks follow [Linear documentation](LinearDocumentation.md).

## Establish the current identity and issue

1. Verify the consuming Linear connection with `mcp__linear_codex__get_user` using `query="me"`, then retrieve the workspace and canonical team and compare their UUIDs with `AGENT-REPO-CONTEXT.md`. Stop dependent work if the user, workspace, or team does not match.
2. Retrieve team-scoped issue inventory when discovering work. For a governing issue, retrieve its details with relations, verify its returned team UUID, and retain its full issue UUID and current human-readable identifier before dependent decisions or mutation. The issue-detail tool may not accept a team filter; the returned team check is mandatory.
3. Refresh the governing issue's description, status and type, assignee, labels, comments, attachments, native parent, relations, and dependencies relevant to the task. Historical Plane source or parent annotations in migrated descriptions are provenance, not current Linear hierarchy or dependency evidence.
4. If a project or native parent is absent, record that absence. Do not create or infer one to satisfy an old procedure. Resolve status and user IDs through current Linear readback before any authorized mutation.

A missing or wrong UUID, ambiguous team or issue identity, missing required relation, or unresolved ownership conflict stops the affected dependent operation. Continue authorized independent local analysis that does not require the missing evidence.

## Validate ownership and implementation prerequisites

Inspect current assignees and verify that the intended app user is eligible for any planned assignment. The authenticating user returned by `get_user me` is not automatically the issue assignee or an eligible team member. Assignment or unassignment requires explicit authorization.

While preparing the plan, identify the governing issue, ready dependencies, intended ownership, and verified In Progress state. These are prerequisites before issue-governed implementation. For intake, verify Backlog or Todo, read the current issue and related context, and transition to In Progress only through an explicitly authorized operation followed by readback. If an issue is already In Progress, verify current ownership, requirements, and dependencies directly. A skipped or unauthorized transition does not fulfill the prerequisite.

For continuation, re-read the issue, confirm that state and ownership still match the active work, and refresh comments and relations before proceeding. If a prerequisite cannot be verified or authorized, pause dependent implementation and continue independent analysis.

## Recover partial mutations and blockers

After an uncertain or partial assignment, state, comment, relation, or document mutation, stop dependent operations and re-read the affected object. Record its actual state or error and do not retry a consequential action with an unknown outcome. Resume only after the required state and ownership are verified.

Use native Linear states and relations. Do not invent a Blocked state, claim, lock label, host label, or comment that pretends to reserve exclusive ownership. An explicitly authorized factual blocker comment may record the condition, evidence, needed person or event, and preserved branch, commit, and validation state; a comment does not create a state or lock. Ask how to represent a blocker before changing state or assignment.

## Prepare and record a handoff

Handoff preparation is read-only and may proceed without Linear mutation permission. Inspect existing comments for duplicates and establish the exact branch, baseline, commit, diff, and pull-request target. Prepare the behavior and scope, changed files and artifacts, material decisions, actual validation and runtime results, remaining checks, limitations, blockers, and available commit or pull-request identifiers.

A complete recorded handoff requires verified In Review state. That state may be existing or manually established, or reached through an explicitly authorized transition followed by readback. Add a handoff comment only when communication is explicitly authorized. If state or comment authorization is missing, report the prepared handoff and pending action without claiming a recorded handoff.

Keep the issue In Review while independent review or human acceptance remains. Linear records handoff state; the repository or pull-request review remains authoritative for code findings.

## Complete after action-time acceptance

Only the user may approve final completion. Immediately before recording final acceptance, moving an issue from In Review to Done, or removing its active assignee as part of completion, obtain explicit action-time confirmation. Plan approval does not replace that confirmation.

After confirmation, re-read the issue and comments, perform only the completion actions covered by the user's confirmation, and re-read the issue to verify each result. Recording acceptance in a comment and removing an assignee each require explicit authorization for those actions; approval to mark Done alone does not include them. Do not request another approval for the same already-confirmed action.

Use the failure boundary in the repository context for unavailable, stale, or inconsistent Linear evidence. Do not substitute Plane, Codecks, historical memory, local roadmap drafts, or guessed requirements for current Linear state.
