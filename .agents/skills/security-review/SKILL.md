---
name: security-review
description: Perform an explicitly requested local security review of project code. Not an automatic workflow stage or a Codex Security scan.
---

# Security review

Use only when the user explicitly requests a security review. General code review, infrastructure work, or the presence of a trust boundary does not automatically activate it. This is a local read-only review, not a Codex Security product scan; do not launch that separate product workflow without its own explicit request.

Identify the assets, principals, trust boundaries, and realistic attacker capabilities. Trace attacker-controlled input to sensitive behavior through parsing, validation, authorization, and side effects. Check the actual deployment model before treating local configuration or installed extensions as untrusted input.

Seek concrete paths to unauthorized capability or data access. Consider command execution, path handling, deserialization, authentication, secret exposure, or cross-component authority where the code supports those concerns. A crash or malformed input alone is not proof of a security vulnerability.

For each supported finding, explain the attacker prerequisites, source-to-sink path, ineffective control, impact, location, and proportionate remediation. Calibrate severity to realistic reach and consequences. Keep hypotheses separate from findings and do not manufacture vulnerabilities to complete a checklist.

Return supported findings and meaningful coverage limits without editing production code. A source review does not establish exploitability in an unavailable environment or authorize a fix, publication, or external scan.
