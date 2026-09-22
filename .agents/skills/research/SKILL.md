---
name: research
description: Gather repository and authoritative external evidence for technical, version, dependency, compatibility, and implementation questions.
---

# Research

Identify the question and the decision it must support. Inspect the relevant local implementation, configuration, versions, and call sites before assuming how the system works. Verify that referenced symbols or features exist and are used as claimed.

Use primary sources for external facts: official documentation, specifications, vendor source, release notes, or original research. For version-sensitive conclusions, establish the relevant version, platform, and configuration. Resolve source disagreements by examining their scope and authority rather than combining incompatible claims.

Return the answer with concise supporting paths, symbols, or source links. Distinguish verified facts, inferences, and unresolved runtime questions. Missing local binaries or services are evidence limits; do not turn a likely behavior into a confirmed result.

Stop when the evidence is sufficient for the decision. Keep the work read-only unless a documentation update is authorized, and avoid accumulating unrelated background or producing a separate report file when the answer is enough.

When persisting or publishing content, follow the repository's [documentation destinations and edit restrictions](../../../AGENT-REPO-CONTEXT.md#documentation-destinations). Producing content does not authorize external writes or publication.
