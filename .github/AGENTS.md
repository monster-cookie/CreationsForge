# GitHub workflow rules

This folder contains GitHub configuration and automation.

## Safety

- Do not change release, signing, packaging, publishing, deployment, token, or permission behavior without explicit approval in the plan.
- Do not add secrets to repository files, logs, workflow output, or generated artifacts.
- Keep workflow permission blocks minimal and explicit.
- Prefer pinned major versions for GitHub actions unless the repository already pins more tightly.
- Do not introduce third-party actions without approval and rationale.
- Do not change branch, tag, release, or artifact naming conventions without calling it out in the plan.

## Validation workflows

- Keep validation aligned with the repository's standard commands:
  - dotnet restore
  - dotnet build
  - dotnet test
- Do not make CI pass by weakening tests, suppressing analyzer failures, or skipping meaningful validation unless the plan explains why and the user approves.
- Avoid caches that can leak secrets or create stale build behavior.
- Keep package/release workflows separate from pull-request validation unless the repository already combines them.

## Documentation

Workflow changes that affect contributors, releases, signing, packaging, artifacts, or validation expectations should update the relevant documentation in the same approved task.
