---
name: software-architecture
description: Analyze existing system boundaries, interfaces, dependencies, data flow, lifecycle, and migration choices before a substantial technical change.
---

# Software architecture

Ground the design in the current implementation and the requested outcome. Trace enough of the system to identify state ownership, authoritative data, interfaces, lifecycle, and compatibility boundaries. Separate confirmed facts from assumptions and cite the source that supports material decisions.

First determine whether an existing boundary can support the change. Compare alternatives only where the tradeoff matters: correctness, coupling, operational complexity, migration cost, or failure recovery. Prefer a local change over a new abstraction or rewrite unless the existing design cannot meet the requirement.

Consider ordering, duplicate or partial operations, version skew, and recovery where they affect the proposed contract. Identify changes that must ship together and any dependency, schema, or public API decision that needs approval. Use a diagram when it makes the explanation clearer, not as a required deliverable.

Keep analysis read-only unless a documentation edit is authorized. Return a decision and its rationale, affected responsibilities, material implementation constraints, and remaining questions. Include migration or rollout guidance only when applicable. The result should help implementation proceed without prescribing every coding step or forcing an architecture document for a small change.

When persisting or publishing content, follow the repository's [documentation destinations and edit restrictions](../../../AGENT-REPO-CONTEXT.md#documentation-destinations). Producing content does not authorize external writes or publication.
