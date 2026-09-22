---
name: tech-ops
description: Maintain infrastructure-as-code and environment configuration, plan and troubleshoot provisioning and CI/CD infrastructure workflows, and investigate drift using the project's actual toolchain.
---

# Tech ops

Work from the repository's actual infrastructure toolchain and intended target. Establish the account, environment, workspace or stack, state backend, and ownership relevant to the operation. Distinguish local configuration work, provider-backed planning, apply, import, state migration, and destroy.

Preserve state boundaries, locking, provider pins, and existing naming unless the task changes them. Inspect the actual command and wrappers for side effects. A local validation request does not authorize remote planning; an authorized plan includes its necessary provider reads and normal transient locking, subject to explicit task restrictions. Planning does not authorize apply, persistent state changes, import, destroy, or force-unlock.

Use existing formatters, validators, and plan commands. Check material replacements, deletions, dependency ordering, drift, and output sensitivity. Keep secrets and sensitive plans or state snapshots private. Do not create a replacement toolchain merely to obtain a local pass.

Before an authorized apply or state operation, confirm that its target and actions match the request. Preserve active locks. After an uncertain result, inspect provider/job state and partial effects before retrying. State recovery requires a supported rollback or backup path and the appropriate authorization.

Report local validation, planned actions, applied changes, and observed runtime results separately. Include meaningful recovery or acceptance steps when needed. Infrastructure work does not automatically require a security review or scan.
