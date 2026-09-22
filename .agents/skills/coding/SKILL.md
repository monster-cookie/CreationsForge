---
name: coding
description: Implement, debug, refactor, build, and validate bounded software changes while preserving repository contracts and scope.
---

# Coding

Deliver the requested behavior in the actual implementation. Start with the relevant entry points, callers, state ownership, and existing tests. Verify assumptions about interfaces and execution order against the code before changing it.

Choose the smallest complete change that fits the repository's language, architecture, and conventions. Preserve public APIs, serialized data, compatibility, and user-visible behavior unless the task changes them. Keep error handling at meaningful boundaries, and consider nearby failure paths that could invalidate the result. Avoid speculative abstractions, unrelated fixes, and mechanically copying existing defects.

An unavailable runtime limits execution evidence, not necessarily implementation. Proceed when source and contract evidence support the change; report the runtime gap. If a missing platform fact is essential to correctness, investigate or ask about that fact before making the dependent change.

Use the shared [verification guidance](../../../AGENTS.md#verification-and-communication) with the repository's [build and verification entry points](../../../AGENT-REPO-CONTEXT.md#build-and-verification-entry-points). Choose checks that exercise the changed behavior within the available environment.

Return the behavior changed, actual verification, and material remaining risks. Include manual acceptance steps when needed; a separate testing artifact is not required for every edit. Delivery and authentication follow the repository's shared boundaries, without expanding the task.
