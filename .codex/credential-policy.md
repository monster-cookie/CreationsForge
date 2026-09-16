# CreationsForge service-account credential policy

Policy-Version: 1

Use the Codex AI service accounts for GitHub and Linear operations selected by this repository's tooling policy. Proton Pass item resolution below applies to the GitHub service account; the configured Linear connection uses an OAuth app user and has no Proton Pass credential reference in this policy. Resolve Proton-managed account identities, usernames, email addresses, vault and share selectors, item and field identifiers, and token fields from the selected item at runtime. Do not copy those resolved values into this policy or other repository files.

The fixed Git author and committer identity declared in `.codex/tooling-policy.md` and the fixed public Linear provider IDs approved in `AGENT-REPO-CONTEXT.md` are identity mappings, not credentials resolved under this policy. Their inclusion does not permit any other resolved identity or credential value to be copied into repository files.

Per the user's direction for this repository, apply these tool and identity selections to work authorized by the current task without requiring a separate adoption record, policy-hash baseline, or execution-review artifact. This instruction also applies when an AgentKit procedure describes such a prerequisite. Verify the actual account and target before authenticated operations; ask only when required task authorization is missing or a concrete identity, target, or permission problem prevents the operation. Continue independent authorized work.

For Proton-managed operations, resolve each exact item title uniquely within the Proton Pass resources made available to the user's Codex agent. Include directly shared items when applicable; access through a vault is not mandatory. Obtain share, item, and field selectors at runtime and explicitly scope subsequent CLI lookups to that resolved item. Do not assume a vault, share, field name, or field ID, or select the first duplicate title. Defer the dependent operation if the item, expected identity, or required credential field is missing or ambiguous. Do not create or modify item fields implicitly. Keep resolved values inside the credential-processing context; report only verification outcomes.

## Proton Pass session and credential handling

Use Proton Pass CLI (`pass-cli`) with a session dedicated to the task and worker. Bootstrap authentication uses the protected `PROTON_PASS_PERSONAL_ACCESS_TOKEN` environment variable supplied by the user's existing local setup. This PAT is not stored in Proton Pass and must never be represented as a policy item or `pass://` reference, because that would create a circular dependency. Before inspecting or changing a session, select a task-owned `PROTON_PASS_SESSION_DIR` in a private OS temporary location outside the repository. Preserve the user's default Proton Pass session.

Before each authenticated Proton Pass command, run `pass-cli info`, check its exit code, and confirm that the dedicated session is healthy. Compare a reported token name only when the user explicitly supplied an expected nonsecret name. This check does not recursively apply to `info`, local version/help commands, or login/logout required for recovery. If login is needed and authorized, supply the manager PAT only to the dedicated login process through `PROTON_PASS_PERSONAL_ACCESS_TOKEN`, run `pass-cli login`, and verify `info` again. Diagnose failures before changing authentication state. Recover only a confirmed expired or stale task session, and check whether an interrupted operation completed before retrying. Missing manager setup defers only Proton-dependent access; it does not block credential-free work or an already-verified consuming session.

Set a task-specific, non-empty `PROTON_PASS_AGENT_REASON` of at most 300 characters before item or field access and before `pass-cli run`. Use a supported direct transfer into the intended consuming process. For environment-based credentials, construct the `pass://` reference from the selectors resolved in memory and use `pass-cli run` with output masking retained. It resolves references in inherited environment variables as well as explicit inputs, so launch it with a cleared, explicitly allowlisted environment containing only required platform settings, the verified session path, the access reason, dedicated consumer settings, and the intended service reference. Exclude the manager PAT, unrelated credentials, and unrelated `pass://` references from that environment and from the consumer. Do not run a secret-bearing `pass-cli item view` command with stdout exposed to the terminal tool or model, or write a credential env file into the repository.

Keep authentication session files private and outside the repository. When finished, log out only the owned task session and remove only task-created session state and process-scoped credential variables after checking ownership. Reuse and verify an already-correct consuming-tool context without rereading its token merely because this policy exists. A successful Proton Pass login does not verify any downstream tool's identity.

## Identity: github-automation

| Field | Value |
| --- | --- |
| Service | github |
| Expected identity | The Codex AI GitHub service account represented by the named item. Read its login, email, and any saved provider identity fields at runtime. Establish the expected public login and stable provider ID from those fields or a provider-verified association with the item's identity; do not derive the expected account solely from the current tool session or repository owner. Stop if the association cannot be verified. |
| Manager | Proton Pass via Proton Pass CLI (`pass-cli`), using the dedicated and verified agent session described above for runtime item resolution and authorized credential setup. |
| Reference | item=GitHub (Codex AI); resolve the accessible share, item, identity fields, and any required API-token field at runtime. The item title is a lookup reference, not a credential value or proof of the current tool's identity. |
| Authentication | Apply the Proton Pass session and credential-handling rules above. Reuse an already-correct GitHub MCP or policy-permitted CLI context and verify its identity through that tool. Separately authorized CLI browser sign-in may establish managed OAuth authentication using the account represented by the item; this method does not require a PAT stored in the item. For the configured MCP bearer-token method, resolve the service-account API-token field from the item and supply it only to the consuming host's `GITHUB_MCP_TOKEN` for `https://api.githubcopilot.com/mcp/`. If a user-designated PAT is used with GitHub CLI, `pass-cli run` may provide it as child-process `GH_TOKEN`. Stop if a required token field is unavailable. Never substitute a login password for an API token or assume one tool's authentication configures another. |
| Verification | For GitHub MCP, call `mcp__github__get_me` through the same connection used for the operation. For the permitted CLI context, run `gh api --hostname github.com --method GET user`. Compare the returned public login and stable ID against the expected identity resolved from the item and any verified provider association in the protected runtime context. Verify the tooling policy's resolved repository target separately. Stop on mismatch or inability to verify. A successful check in one context does not verify another context or Git transport. |
| Fallback | none |

## Identity: linear-automation

| Field | Value |
| --- | --- |
| Service | linear |
| Expected identity | The intended Codex OAuth app user mapped by its fixed public UUID in `AGENT-REPO-CONTEXT.md`. A display name alone or a different tool's session does not establish identity. Stop if the consuming connection resolves to a different user. |
| Manager | The configured Codex Linear OAuth connection; Proton Pass is not used to resolve a Linear token under this policy. |
| Reference | No saved credential item or token field is declared for Linear. Do not invent one or substitute the former Plane item. |
| Authentication | Reuse the configured `mcp__linear_codex__*` connection only after verifying its actual app user. Do not assume a shell environment change, browser login, GitKraken connection, or Proton Pass session changes that connection. Credential setup or renewal requires its own authorized workflow; defer dependent operations if the correct connection is unavailable. |
| Verification | Call `mcp__linear_codex__get_user` with `query="me"` through the connection used for the operation and compare the returned UUID with the intended app-user UUID in `AGENT-REPO-CONTEXT.md`. Verify the workspace through `get_workspace`, the team through `get_team`, and returned issue or document team identity before dependent operations. Inspect assignment eligibility and existing assignees before assignment or issue-governed implementation. |
| Fallback | none |

## Preserve personal authentication

The user requires the browser and GitKraken to remain on the personal account. Codex's GitHub MCP and Codex-initiated GitHub CLI calls must use the separate Codex AI service account. Prefer child-process-only `GH_TOKEN` for Codex's CLI calls and keep any dedicated `GH_CONFIG_DIR` selection process-scoped. Do not overwrite the ordinary CLI login, change shared Git credential helpers, persist a service token in global `GH_TOKEN` or `GITHUB_TOKEN` variables, log the browser out, or reconnect GitKraken. A separately authorized browser sign-in for the service account must use a separate profile or private session. Authentication setup does not authorize changing Git commit authorship or signing.

Keep resolved usernames, emails, unapproved account/member/workspace identifiers, vault/share/item identifiers, token-field identifiers, passwords, tokens, recovery codes, private keys, and saved one-time codes out of repository files, model-visible output, logs, and public review artifacts. The approved fixed public Linear identity mappings in `AGENT-REPO-CONTEXT.md` are the sole exception for provider IDs, not for credentials. Do not silently fall back to a personal account.
