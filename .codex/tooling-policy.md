# CreationsForge Tooling Policy

Policy-Version: 1

This repository policy applies only to this checkout of `CreationsForge`, as identified by its existing repository context and configured origin. Combine it with applicable shared tooling and credential policies. Repository entries replace shared entries with the same ID in full. Unspecified services keep their existing authorized workflow.

Per the user's direction for this repository, apply these tool and identity selections to work authorized by the current task without requiring a separate adoption record, policy-hash baseline, or execution-review artifact. This instruction also applies when an AgentKit procedure describes such a prerequisite. Verify the actual account and target before authenticated operations; ask only when required task authorization is missing or a concrete identity, target, or permission problem prevents the operation. Continue independent authorized work.

## Tool: github

| Field | Value |
| --- | --- |
| Service | github |
| Roles | all |
| Requirement | preferred |
| Tool | The configured local GitHub MCP integration (`mcp__github__*`), running `github-mcp-server` over stdio with the `github-automation` GitHub App installation. Use the exact GitHub host and repository from Target and verify the actual consuming session's App installation and repository access before the operation. |
| Identity | github-automation |
| Target | The exact GitHub repository resolved from this checkout's configured origin and existing `AGENT-REPO-CONTEXT.md`. Require host=github.com and API endpoint=https://api.github.com; resolve owner and canonical repository at runtime. Require agreement with the repository context and current task. A changed origin does not authorize a different target. Stop if context, origin, or task target disagree. |
| Operations | task-scoped; repository inspection and GitHub operations only when authorized by the current task and applicable repository instructions. Pull requests must be ready for review. This entry grants no standing permission to publish, change repository settings, manage credentials, merge, approve, deploy, or release. Prohibited: draft pull requests, force updates, and commits or pushes directly to a protected branch. |
| Fallbacks | The installed `Invoke-GitHubAppGh.ps1` wrapper with GitHub CLI (`gh`) when the MCP context is unavailable. The wrapper must mint a fresh installation token from the configured GitHub App environment, supply it only as child-process `GH_TOKEN`, isolate `GH_CONFIG_DIR`, and discard and revoke the token after the command. Invoke `gh` only through that wrapper for Codex GitHub operations; direct or ambient CLI authentication is not a permitted fallback. The same target, identity, and operation limits apply. No personal-account fallback. |

## Tool: linear

| Field | Value |
| --- | --- |
| Service | linear |
| Roles | all |
| Requirement | required |
| Tool | The configured Linear connection exposed as `mcp__linear_codex__*`, using its Codex OAuth app-user context. Verify the consuming connection's identity before dependent operations. |
| Identity | linear-automation |
| Target | Only the Venworks workspace and Creations Forge team mapped in `AGENT-REPO-CONTEXT.md`. Verify both UUIDs through the current Linear connection, supply the team UUID wherever team scoping is supported, and verify each unscoped issue or document belongs to that team. The current migration has no Linear project; do not infer one from a name or historical Plane annotation. |
| Operations | Task-scoped reads of current requirements and only explicitly authorized mutations of related team issues or documents. Comments, assignments, state changes, document writes, and completion actions require explicit task authorization; this entry supplies no standing grant for them. Read back every mutation. Prohibited: unrelated workspace maintenance. |
| Fallbacks | none |

## Documentation audience and internal research

Choose documentation placement by its intended audience, not by how technical the subject is. Modders and HUD/add-on authors using Canvas are end users. Maintain documentation that helps them install, configure, integrate with, use, or troubleshoot supported Canvas behavior in the repository's `docs` folder. README and changelog content must describe useful end-user guidance or actual user-visible changes, not advertise internal research as a product feature or modder guide.

Keep internal technical research for the project owner and development agents in Plane project pages. This includes source inventories and audits, architecture investigations, design rationale, exploratory ideas, implementation plans, and internal evidence-gap analysis. Put temporary testing instructions and execution handoffs on the relevant Plane work item rather than creating a permanent page or tracked Markdown document. These placement rules supersede broader repository guidance that assigns technical or historical findings to repository documentation; they do not authorize migrating unrelated existing documents.

Do not publish internal research in tracked repository files, commits, pull-request titles or descriptions, GitHub issues or comments, changelogs, or other public artifacts without the user's explicit authorization to publish that specific material. General permission to implement, document, commit, push, or open a pull request is not permission to publish internal research. Keep public delivery summaries limited to the necessary approved change and validation facts, without copying private findings or design details from Plane.

Check the intended audience and the complete staged content before committing, and inspect outgoing commits and public review copy before publishing. Removing a file from the current tree does not erase it from earlier commits or remote history. If internal material has already been published, report the exposure accurately; do not claim that deleting current files makes it private again, and do not rewrite history or change repository visibility without separate explicit authorization.

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

## Tool: local-git

| Field | Value |
| --- | --- |
| Service | local-git |
| Roles | all |
| Requirement | required |
| Tool | Installed Git CLI for credential-free local repository inspection and explicitly authorized local working-branch changes. |
| Identity | none |
| Target | This repository checkout; verify its root, current branch, and configured origin against its existing repository context and current task before a task-scoped mutation. |
| Operations | task-scoped; read-only inspection and local branch, staging, or commit operations only within existing task authorization and repository instructions. Git transport operations are outside this entry. Prohibited: commits directly to protected branches and destructive history changes without separate explicit authorization. |
| Fallbacks | none |

## Tool: repository-powershell

| Field | Value |
| --- | --- |
| Service | local-build |
| Roles | all |
| Requirement | preferred |
| Tool | PowerShell 7 via `pwsh -NoProfile`, using the repository's existing `Tools/` scripts and documented parameters. Relevant entry points include `Tools/verifyCanvas.ps1`, `Tools/compileScripts.ps1`, `Tools/buildScaleform.ps1`, and `Tools/createPackages.ps1`. Inspect the selected script, its imports, configuration, and side effects before execution. |
| Identity | none |
| Target | This checkout of `venworks-canvas` and the specific local inputs and output paths authorized by the task. |
| Operations | task-scoped; credential-free local checks, compilation, and packaging when authorized. A tool name does not authorize its side effects. Downloads, tool installation, live game staging, junction changes, or authenticated operations require their own applicable authorization and policy resolution. |
| Fallbacks | none |

## Tool: diagrams

| Field | Value |
| --- | --- |
| Service | diagrams |
| Roles | all |
| Requirement | preferred |
| Tool | Mermaid fenced Markdown diagrams. |
| Identity | none |
| Target | Architecture documentation, technical documentation, and authorized pull-request descriptions for `venworks-canvas`. |
| Operations | task-scoped; add or update diagrams when they clarify structure, dependencies, data flow, or lifecycle. Creating a diagram does not authorize publishing it. |
| Fallbacks | none |

## Codex-created commit identity

Every commit created by Codex must use `MonsterCookieAI <venworksai@venworkscreations.com>` as both its Git author and committer. Apply this identity only to the individual commit command:

```powershell
git -c user.name="MonsterCookieAI" -c user.email="venworksai@venworkscreations.com" commit ...
```

Do not set or change this identity through `git config --global`, `git config --system`, `git config --local`, worktree configuration, direct configuration-file edits, or other persistent configuration. Preserve the human user's normal Git identity for the user's own commits. Do not combine the per-command override with `--author`, `GIT_AUTHOR_*`, `GIT_COMMITTER_*`, or another override that changes either required identity.

Git commit authorship and GitHub authentication are separate identity layers. The dedicated Git commit name and email above are attribution metadata, not authentication credentials. They do not replace the `github-automation` GitHub App, which remains required for Codex GitHub MCP and API operations, wrapped GitHub CLI and pull-request operations, and explicitly authorized Git transport including pushes. Do not obtain or use `monstercookieai` login credentials merely to obtain commit attribution, and never silently fall back to the human user's personal GitHub credentials. Never expose or commit GitHub App private keys, installation tokens, access tokens, or other credentials.

Before pushing a Codex-created commit, verify its committed author and committer identities with:

```powershell
git show --no-patch --format=fuller HEAD
```

Require both identities to be `MonsterCookieAI <venworksai@venworkscreations.com>`. If either identity differs, stop and report the mismatch. Do not amend, reset, rebase, or otherwise rewrite the commit unless that operation is separately and explicitly approved. Do not rewrite existing commit history merely to change cosmetic attribution.

Never expose or commit GitHub App private keys, installation tokens, access tokens, or other credentials. The fixed commit email is public identity metadata, not an authentication secret.

This rule controls identity only for an otherwise authorized commit. All existing protected-branch, staging, commit, push, pull-request, destination-verification, and Git safety requirements remain in force.

## GitHub App and personal application separation

The `github-automation` GitHub App installation is required only for Codex's local GitHub MCP connection, Codex-initiated GitHub CLI operations, and explicitly authorized Codex Git transport. Preserve the user's personal browser sessions, GitKraken connection, and ordinary GitHub CLI login.

The local MCP server reads `GITHUB_APP_ID`, `GITHUB_APP_INSTALLATION_ID`, and `GITHUB_APP_PRIVATE_KEY_PATH` and mints installation tokens internally. Codex-initiated `gh` calls must use the installed `Invoke-GitHubAppGh.ps1` wrapper, which injects a freshly minted installation token only into the child process as `GH_TOKEN` and uses a dedicated `GH_CONFIG_DIR`. Explicitly authorized Codex Git transport must use the same wrapper with `-Git`; that mode supplies `gh auth git-credential` through process-only Git configuration and disables interactive prompting without changing any persisted credential helper. Do not persist App credentials or installation tokens as Windows User or Machine `GH_TOKEN` or `GITHUB_TOKEN` values. Do not run `gh auth switch`, `gh auth login`, `gh auth logout`, or `gh auth setup-git` against the user's ordinary configuration, and do not change shared Git credential helpers, signing settings, or commit authorship as part of App API or Git transport setup.

Do not use Proton Pass, a PAT, a login password, interactive browser authentication, or the hosted GitHub MCP bearer-token endpoint for this GitHub App workflow. Verify the App installation and exact repository through each actual MCP or wrapped CLI context independently. Installation tokens do not represent a GitHub user, so `get_me` and `gh api user` are not valid App-installation identity checks.

## Git transport and credential boundaries

GitHub App installation verification establishes only the MCP session or wrapped GitHub CLI context that was checked. It does not establish the identity used by another tool, GitKraken, an SSH key, or an independently configured HTTPS credential helper. For a fetch, push, or other authenticated Git operation authorized by the user, invoke the installed wrapper with `-Git` so the Git child receives the same App installation token and a process-only `gh auth git-credential` helper. Verify the actual transport and destination with a read-only Git operation through that mode before mutation. Do not run authenticated Git transport directly or change the user's persisted Git or GitHub authentication. No additional transport-policy document or execution-review record is required.

Do not substitute an ambient account or switch a shared login when the required integration or identity is unavailable. Continue independent credential-free work within the task's authorization and report the affected operation.
