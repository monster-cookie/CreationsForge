# Repository agent guidance

Read [repository context](AGENT-REPO-CONTEXT.md) when establishing the task. It holds this repository's toolchain, integrations, and non-secret identity settings. Load an optional linked procedure only when the corresponding operation is needed.

## Work on the requested outcome

- State the goal, intended changes, and relevant verification before edits. Scale the plan to the task; use a goal-tracking tool only when requested.
- Use existing authorization. Ask when missing information changes correctness, scope, compatibility, or a consequential action; investigate routine implementation choices yourself. Analysis or a plan alone does not authorize implementation.
- Inspect the real implementation and its consumers. Preserve established contracts and unrelated user changes. Include supporting changes necessary for a complete result, without speculative abstractions or unrelated cleanup.
- Missing runtime access limits what can be validated. It does not by itself prevent a well-supported implementation. Stop dependent work only when missing requirements or technical evidence prevent a sound decision; continue independent work.
- Retrieve current external requirements when they govern the task. A configured tracker alone does not make every local task tracker-dependent. Use the [configured tracker and conventions](AGENT-REPO-CONTEXT.md#optional-issue-tracker) when needed.
- Propose changes to agent-instruction files before editing them; apply only explicitly approved changes. New third-party dependencies, frameworks, build tools, package managers, and CI actions also require explicit approval.

## Verification and communication

Choose proportionate checks that can detect defects in the delivered implementation. Prefer existing tests and tools; add a regression test when it exercises the relevant production contract. Do not recreate production logic in another language merely to test the recreation. Ask whether the relevant production behavior could be broken while a proposed test still passes, and improve the assertion or narrow its claimed coverage. A source-pattern check establishes only that pattern; a separate model establishes only the model's behavior.

Run the narrowest relevant checks and expand for changed interactions, failures, or concrete uncovered risks. Reuse passing results while their inputs remain unchanged. Do not create a framework, fixture collection, or permanent report to satisfy a workflow stage.

Report the result, relevant checks actually performed, and material limitations. Source inspection, compilation, packaging, runtime behavior, and platform acceptance establish different things. Never claim a check passed unless it ran successfully. When a check fails or cannot run, report the command or action and concrete reason. Give a supported manual scenario and expected observation when that helps resolve a runtime gap; do not invent commands or test names. A separate testing guide is useful only when the reader needs additional instructions.

Describe the final behavior and actual validation in PRs using the repository's template when present. Use a small diagram with real component names only when it makes a relationship clearer than prose. Keep it aligned with the final implementation and use existing rendering tools when needed; a simple change does not need a diagram or a new rendering service.

Use `.work` for disposable project artifacts when practical. Keep secrets and authentication state out of the repository and reports.

Before retrying an interrupted edit or external action, inspect the resulting state. An uncertain outcome is not evidence that nothing happened.

## Documentation placement

Choose documentation placement by audience and purpose using the [documentation destinations](AGENT-REPO-CONTEXT.md#documentation-destinations). Documentation that helps users or public integrators install, configure, use, extend, or troubleshoot supported behavior belongs in the configured public location. Internal research, architecture investigations, design rationale, implementation plans, and internal validation evidence belong in the configured internal project location. Put temporary execution instructions and handoffs on the relevant work item or configured alternative. Repository agent instructions and their non-secret settings remain local.

Do not duplicate internal documents into public repository content or publish internal material without explicit authorization for that material. Public delivery summaries may describe the approved change and relevant validation without reproducing internal research. Preparing content does not authorize external writes or publication. Respect any configured additional edit restrictions without requesting approval again for an already-authorized change.

Verify the intended external destination and its scope before writing, and read back authorized changes. Document accessibility or team visibility does not by itself establish privacy or web-publication status; verify a privacy property when the action depends on it. If a required destination is unavailable or unconfigured, continue independent work and report the affected documentation step; do not invent a destination or create an unauthorized tracked substitute.

## External tools and identities

Use the service configuration in [repository context](AGENT-REPO-CONTEXT.md). Verify the intended target and expected identity through the actual consuming connection before authenticated operations; do not infer the expected account solely from the active session. Reuse a correct session and reverify after authentication or target changes or ambiguous failures. Ordinary local inspection does not require credential discovery, and unavailable access blocks only dependent work.

Preserve personal browser, GitKraken, and ordinary CLI sessions. Use dedicated connections or process-scoped credentials for authorized setup and only explicitly permitted fallbacks with the same identity and target. Never silently switch to a personal account. Supply secrets through protected channels to the consumer, not through model-visible output, command arguments, logs, or repository files. A password-manager login does not verify a downstream account. Git authorship, Git transport, and hosting API authentication are separate boundaries; verify each when used.

When injecting credentials into a child process, explicitly limit its environment to required settings and credentials; exclude unrelated secrets, credential references, and manager bootstrap tokens the consumer does not need. Output masking does not replace this isolation. Clean up only session state and temporary credentials owned by the task.

Local editing permission does not authorize external messages, record updates, publication, or completion. Perform only the actions authorized by the request or approved plan, verify their target, and read back the result. Record completion only when authorized and its criteria are met; a build or PR alone does not establish human acceptance. Preserve unrelated ownership fields and settings. Pass relevant configuration and authorization to workers only when delegation is part of the task, without making them rediscover unrelated setup.

## Git and GitHub boundaries

These boundaries govern repository Git delivery. Tool configuration supplies identity and target information, not additional authorization.

- Read-only Git and GitHub inspection is allowed when relevant to the task.
- Before Git or GitHub mutations, verify the current branch. Treat `main`, `master`, `trunk`, and the remote default branch as protected. Use `refs/remotes/origin/HEAD` to establish the default when available; if the branch or required destination cannot be established, stop the affected mutation.
- Do not edit on a protected branch without explicit approval of that exceptional scope. Never commit or push directly to a protected branch.
- Creating and switching to a working branch requires an approved task-specific plan naming the exact branch, intended base, and both operations. Verify the base commit, target branch, and working-tree state first. Prefer the `codex/` prefix and preserve unrelated changes.
- Local editing does not authorize staging, committing, pushing, or PR operations. When the request or approved plan explicitly authorizes delivery, perform those steps without repeated confirmation on the selected non-protected branch.
- Stage explicit task-owned paths or hunks. Inspect the full staged diff and status before committing; preserve unrelated staged, unstaged, and untracked work.
- Push only the current working branch to the same-named branch on `origin`, setting its upstream only when authorized delivery needs it. Verify the destination is not protected. PRs must use that working branch as head and the protected default branch as base; create only ready-for-review PRs. Update only the task's PR within authorization.
- Never force-push, push tags, delete remote refs, merge/close/approve PRs, or delete/rename branches. Do not create/delete tags or stashes, or modify remotes, persistent repository configuration, hooks, worktrees, submodules, branch protection, rulesets, secrets, releases, or repository settings.
- Merge, rebase, cherry-pick, revert, reset, amend, restore, and file checkout require separate explicit approval in the task-specific plan. Do not use them to hide or discard unrelated work.

For authorized delivery, inspect the base-to-head and outgoing history as well as the complete diff so unrelated commits are not published. Check the staged candidate with relevant tools; if unrelated local files could affect the result, resolve or report that limitation. Inspect the resulting commit and any hook changes, refreshing affected checks without bypassing required failures. If ownership or history cannot be separated within authorization, report the blocker rather than creating a worktree or rewriting history as a workaround.

Verify the remote commit and the PR's head, base, URL, and ready status before reporting successful delivery. Report the commit, pushed branch, and PR link as applicable. Assign one delivery owner when multiple workers are involved. If delivery was not authorized, leave the result local; provide a suggested commit message when useful. Do not manufacture an empty commit or PR for a no-change result.

## File and Markdown conventions

When a source file grows beyond roughly 500 lines, consider decomposition by responsibility. Avoid mechanical splits of generated, data-heavy, or intentionally centralized files. Preserve behavior, APIs, and local structure.

Keep Markdown paragraphs and list items on one physical line. Use line breaks for semantic structure, not fixed-width wrapping. Do not reflow unrelated prose.
