# CreationsForge tooling policy

Policy-Version: 1

This repository policy applies only to this checkout of `CreationsForge`, as identified by the current task and configured origin. Combine it with applicable shared tooling and credential policies. Repository entries replace shared entries with the same ID in full. Unspecified services keep their existing authorized workflow.

Per the user's direction for this repository, apply these tool and identity selections to work authorized by the current task without requiring a separate adoption record, policy-hash baseline, or execution-review artifact. This instruction also applies when an AgentKit procedure describes such a prerequisite. Verify the actual account and target before authenticated operations; ask only when required task authorization is missing or a concrete identity, target, or permission problem prevents the operation. Continue independent authorized work.

## Tool: github

| Field | Value |
| --- | --- |
| Service | github |
| Roles | all |
| Requirement | preferred |
| Tool | GitHub MCP integration (`mcp__github__*`) using the Codex AI service account. Use the exact GitHub host and repository from Target and verify the actual consuming session's identity before the operation. |
| Identity | github-automation |
| Target | The exact GitHub repository resolved from this checkout's configured origin. Require host=github.com and API endpoint=https://api.github.com; resolve owner and canonical repository at runtime and require agreement with the current task's intended CreationsForge repository. A changed origin does not authorize a different target. Stop on a concrete target mismatch or inability to verify. |
| Operations | task-scoped; repository inspection and GitHub operations only when authorized by the current task and applicable repository instructions. Pull requests must be ready for review. This entry grants no standing permission to publish, change repository settings, manage credentials, merge, approve, deploy, or release. Prohibited: draft pull requests, force updates, and commits or pushes directly to a protected branch. |
| Fallbacks | Installed GitHub CLI (`gh`) when the MCP service-account context is unavailable and the CLI context independently verifies as the same Codex AI account. The same target, identity, and operation limits apply. Existing CLI browser authentication is permitted; an API token stored in Proton Pass is not mandatory for that authentication method. No personal-account fallback. |

## Tool: plane

| Field | Value |
| --- | --- |
| Service | plane |
| Roles | all |
| Requirement | required |
| Tool | The Plane MCP integration exposed as `mcp__plane__*`, using the Codex AI service-account context from the `Plane (Codex AI)` Proton Pass item. |
| Identity | plane-automation |
| Target | MCP endpoint=https://mcp.plane.so/http/api-key/mcp; only the Plane workspace and project for CreationsForge authorized by the current task or existing user setup. Resolve workspace metadata from the named Plane item when available, and resolve the project through a scoped Plane lookup. Require a unique project match and agreement with the task and any item-stored workspace metadata before using the returned project ID. If the mapping is missing or ambiguous, defer only the dependent Plane operation and continue independent authorized work. Do not save resolved identifiers in this policy or require a nonexistent repository context file. |
| Operations | task-scoped; read current requirements and perform only explicitly authorized operations on related work items in the Target project. Supply the resolved project UUID whenever the tool supports project scoping, and verify the returned project for unscoped retrieval. Comments, assignments, state changes, and completion actions require explicit task authorization; this entry supplies no standing grant for them. Prohibited: unrelated workspace maintenance. |
| Fallbacks | none |

## Tool: dotnet

| Field | Value |
| --- | --- |
| Service | local-build |
| Roles | all |
| Requirement | preferred |
| Tool | Installed .NET SDK via `dotnet`, matching the repository's current project target frameworks and CI SDK selection. Use the existing `CreationsForge.sln` and repository-owned project files. The current CI build sequence is `dotnet restore ./CreationsForge.sln` followed by `dotnet build ./CreationsForge.sln --configuration Release --no-restore`; select existing test projects and filters according to the affected scope and repository instructions. |
| Identity | none |
| Target | This CreationsForge checkout, its existing solution and projects, configured dependency sources, and task-authorized local build/test outputs. Verify the selected solution, project, configuration, SDK, and any required prerequisites before execution. |
| Operations | task-scoped; credential-free restore of existing dependencies, build, relevant tests, and documented formatting checks or authorized formatting edits. An authenticated package feed or other remote service requires its own verified credential policy and task authorization. Publishing packages or releases, changing dependencies or feed configuration, installing toolchains, runtime imports or resets, database migrations, and operations against live game data require their own applicable task authorization. A successful build is source/build evidence and does not establish application runtime or game-platform acceptance. |
| Fallbacks | none |

## Codex and personal application separation

The Codex AI account is required only for Codex's GitHub MCP connection and Codex-initiated GitHub CLI operations. Preserve the user's personal browser sessions, GitKraken connection, and ordinary GitHub CLI login.

For Codex-initiated `gh` calls, prefer a service-account token injected only into the child process as `GH_TOKEN`, with a dedicated `GH_CONFIG_DIR`. These process settings must not be persisted as Windows User or Machine environment variables. Do not run `gh auth switch`, `gh auth logout`, or `gh auth setup-git` against the user's ordinary configuration, and do not change shared Git credential helpers, signing settings, or commit authorship as part of service-account API setup.

Supply the MCP credential only to Codex's configured GitHub connection. A username/password in the Proton Pass item can support separately authorized service-account sign-in, but it is not the API bearer token. If browser authentication is needed for setup, use a separate service-account browser profile or private session so the user's regular browser stays signed in personally. Reverify the service account through the actual MCP and CLI contexts independently.

## Codex-created commit identity

Every commit that Codex is separately authorized to create in this repository must use `MonsterCookieAI <venworksai@venworkscreations.com>` as both its Git author and committer. Apply this identity only to the individual commit invocation:

```text
git -c user.name="MonsterCookieAI" -c user.email="venworksai@venworkscreations.com" commit ...
```

Do not set or change Git identity through `git config --global`, `git config --system`, `git config --local`, worktree configuration, or direct configuration-file edits. Preserve the human user's normal Git identity.

Git commit authorship and GitHub authentication are separate identity layers. The command-scoped `MonsterCookieAI` identity neither authenticates GitHub nor replaces the dedicated Codex GitHub App. Codex-initiated GitHub authentication, pushes, pull-request operations, and GitHub API and MCP operations must continue to use the dedicated Codex GitHub App required by this policy. Do not obtain or use `monstercookieai` credentials merely to produce commit attribution, and never silently fall back to the human user's personal GitHub credentials.

Before pushing a Codex-created commit, run `git show --no-patch --format=fuller HEAD` and verify that both the author and committer are exactly `MonsterCookieAI <venworksai@venworkscreations.com>`. If either identity differs, stop and report the mismatch. Do not amend, reset, rebase, or otherwise rewrite the commit unless that operation is separately and explicitly approved. Do not rewrite existing commit history merely to change cosmetic attribution.

Never expose or commit GitHub App private keys, installation tokens, access tokens, or other credentials. The fixed commit email is public identity metadata, not an authentication secret.

This rule controls identity only for an otherwise authorized commit. All existing protected-branch, staging, commit, push, pull-request, destination-verification, and Git safety requirements remain in force.

## Git transport and credential boundaries

GitHub identity verification establishes only the MCP session or GitHub CLI context that was checked. It does not establish the identity used by another tool, a Git CLI transport, GitKraken, an SSH key, or an HTTPS credential helper. For a fetch, push, or other authenticated Git operation authorized by the user, verify the actual transport's account and destination. Use the same Codex AI account without changing the user's personal application authentication. No additional transport-policy document or execution-review record is required.

Do not substitute an ambient account or switch a shared login when the required integration or identity is unavailable. Continue independent credential-free work within the task's authorization and report the affected operation.
