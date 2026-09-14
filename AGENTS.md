# Repository Agent Rules

These rules apply throughout the repository.

## Repository-specific context

- When establishing the task, read `AGENT-REPO-CONTEXT.md` in the repository root when present. It contains the repository identity, applicable boundaries, and an index of supporting procedures, subordinate to this file and all higher-priority instructions.
- Load a linked procedure when its workflow enters scope and before performing the operations it governs. Refresh relevant context when it may have changed; do not reload unrelated procedures before every edit.
- Repository mappings, project identifiers, local paths, repository URLs, and external-system identities belong in `AGENT-REPO-CONTEXT.md`, not this shared instruction file.
- Do not assume repository-specific rules, project identifiers, external systems, paths, or conventions from another repository.
- If the task depends on a declared external source of truth, retrieve the relevant current information before making decisions or implementation changes that depend on it. Independent local inspection and provisional planning do not require unrelated external information; identify unresolved inputs explicitly.
- If required context or external information cannot be verified, stop the affected operations and report the blocker. Continue authorized independent work that does not depend on the missing information; do not invent requirements or substitute another source of truth.

## Questions, goals, and approval

- State the task's concrete goal and plan before edits, with detail proportional to the task. Use a goal-tracking tool only when the user requests it and the host permits it.
- Ask clarifying questions only when missing information materially affects correctness, scope, constraints, or acceptance. Use information already provided instead of asking the user to repeat it.
- Proceed with edits authorized by the user's current request or earlier conversation. A plan does not require another approval when the work is already authorized; a request for analysis or a plan alone does not authorize implementation.
- For substantial work, use `AGENT-PLAN-TEMPLATE.md` when present, including only applicable sections. For small changes, use a brief scope, intended change, and relevant validation.
- Keep edits within the authorized goal and scope. Expected file paths guide implementation; discovering another necessary supporting file does not by itself require another approval.
- Ask before materially expanding scope, changing the goal, or taking a consequential action outside existing authorization. Apply the specific instruction-file, dependency, Git, and external-system approval boundaries below and in repository context.
- Investigate uncertainty and use established patterns for routine implementation choices. Ask when an unresolved decision materially changes requirements, compatibility, ownership, risk, or acceptance; an unfamiliar approach alone is not a stop condition.

## Repo-wide safety rules

- Git and GitHub mutations are governed exclusively by the Git and GitHub boundaries section below.
- Changes to `AGENTS.md`, `AGENTS.override.md`, `AGENT-PLAN-TEMPLATE.md`, or other agent-instruction files require explicit user approval of the proposed changes. Do not modify them incidentally during other work.
- Keep changes surgical and consistent with existing patterns and naming.
- Avoid unrelated formatting churn, project-wide cleanup, or broad rewrites.
- Do not introduce new third-party dependencies, frameworks, build tools, package managers, or CI actions without explicit approval in the plan.
- Do not claim build, test, packaging, migration, import, or validation success unless the command actually ran successfully.
- If validation cannot run, report the exact command, the failure or blocker, and whether it appears environmental.
- Do not add secrets, credentials, tokens, connection strings, private keys, personal paths, or machine-specific data to source files, documentation, test fixtures, logs, generated output, or workflow files.

## Markdown line wrapping

- Never hard-wrap Markdown prose at a fixed column width.
- Keep each paragraph and list item on one physical line, regardless of length.
- Let Markdown renderers wrap text responsively.
- Use line breaks only for semantic structure, such as headings, separate paragraphs, lists, tables, and code blocks.
- Do not reflow existing Markdown unless explicitly requested.

## Git and GitHub boundaries

### Read-only inspection

- Clearly read-only Git and GitHub inspection commands are allowed without case-by-case approval when needed to understand repository state, history, tracked files, CI results, pull requests, or repository configuration.
- Permitted read-only Git commands include:
  - `git status`
  - `git diff`
  - `git log`
  - `git show`
  - `git blame`
  - `git ls-files`
  - `git rev-list`
  - `git rev-parse`
  - `git branch --show-current`
  - `git symbolic-ref`
  - `git cat-file`
  - `git grep`
  - `git remote -v`
  - `git submodule status`
- Permitted read-only GitHub operations include repository, workflow-run, check, issue, pull-request, ruleset, branch-protection, and security-setting queries. GitHub API calls must use read-only methods such as `GET`.

### Protected branches

- Treat the repository's remote default branch as protected.
- Always treat branches named `main`, `master`, and `trunk` as protected, even if one is not currently the remote default.
- Before any Git or GitHub mutation, determine the current branch with `git branch --show-current`.
- Determine the remote default branch from `refs/remotes/origin/HEAD` when available.
- If the current branch is empty, detached, or cannot be determined confidently, do not perform mutations and stop for user direction.
- If the current branch is protected, do not edit, commit to, or push it. The agent may create and switch to a non-protected working branch only when the exact branch name, intended base, and branch-creation steps are included in an approved task-specific plan.
- Do not make implementation edits directly on a protected branch unless the user explicitly approves that exceptional scope. Even with approval to edit, never commit directly to or push directly to a protected branch.

### Allowed working-branch delivery

- On a non-protected working branch that was either already selected or created and selected under an approved task-specific plan, the agent may perform the following operations only when explicitly authorized by the user's request or approved plan:
  - stage files within the approved task scope;
  - create new commits containing only the approved changes;
  - push the current branch to a same-named branch on `origin`;
  - set the upstream for that same-named remote branch when necessary;
  - create a ready-for-review pull request from the current working branch into the protected default branch;
  - update the title or description of the pull request created for the current task.
- Once the user authorizes these delivery steps, no additional case-by-case confirmation is required for those operations within the same scope. Authorization to edit local files alone does not authorize staging, committing, pushing, or pull-request operations.
- Stage explicit approved paths. Do not use `git add .`, `git add -A`, or equivalent broad staging unless inspection confirms that every included change belongs to the approved task.
- Before committing, inspect `git status --short` and the staged diff.
- Before pushing, verify again that the destination is the same-named working branch and is not protected.
- Before opening a pull request, verify that its head is the current working branch and its base is the protected default branch.
- Create only ready-for-review pull requests. Do not create draft pull requests.

### Always prohibited

- Never commit directly to, push directly to, or force-update a protected branch.
- Never push the current commit to a differently named remote branch.
- Never use `--force`, `--force-with-lease`, remote ref deletion, or tag pushing.
- Never merge, close, or approve a pull request.
- Never merge, rebase, cherry-pick, revert, reset, amend, restore, or check out files unless separately and explicitly approved in the task-specific plan.
- The agent may create and switch to a non-protected working branch only when the exact branch name and both operations are listed in an approved task-specific plan. Before doing so, verify the current branch, remote default branch, intended base commit, target branch name, and worktree state; confirm the target is not protected; and preserve unrelated changes. Use the `codex/` prefix by default unless the user approves another name.
- Never delete or rename branches.
- Never create or delete tags or stashes.
- Never modify remotes, repository configuration, hooks, worktrees, submodules, branch protection, rulesets, secrets, releases, or repository settings.
- Preserve unrelated staged, unstaged, and untracked user changes.
- If any required branch or destination check fails, stop before mutation and report the exact blocker.

## Delivery and commit-message handoff

- If the user's request or approved plan authorizes working-branch delivery, perform only the authorized delivery steps: stage approved paths, create the commit, push the same-named working branch, and create or update its ready-for-review pull request as applicable.
- Report the resulting commit hash, pushed remote branch, and pull-request URL.
- Do not claim that a commit, push, or pull request succeeded unless the corresponding command actually completed successfully.
- If delivery is not authorized, provide a suggested Git commit title and body instead of staging or committing.
- Use a concise imperative commit title that summarizes the goal.
- In the body, summarize the major implementation, configuration, documentation, staging, and validation changes.
- When providing a suggested commit message, format the title and body in separate code blocks for easy copying.

## Planning requirements

Before edits, identify the scope, intended change, expected files, and relevant validation. A small change needs only a brief plan.

For substantial changes, also describe applicable implementation steps; UI, data, persistence, configuration, dependency, workflow, and documentation impacts; material risks and rollback; and specific validation commands or manual checks. Omit inapplicable sections instead of adding boilerplate.

Identify operations that require separate authorization and any external prerequisites before dependent implementation. Keep required checks and validation proportional to the change; report actual results and unverified limitations.