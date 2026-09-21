---
name: code-review
description: Perform an independent read-only review for correctness, regressions, compatibility, lifecycle, error handling, maintainability, and meaningful test gaps.
---

# Code review

Review the requested change against its requirements and actual consumers. Trace relevant inputs, state changes, side effects, and outputs through surrounding code. Look for reachable correctness failures, regressions, compatibility breaks, and lifecycle or ownership mistakes.

Follow failure, retry, cleanup, and cancellation paths when they can affect the result. Check tests as evidence: could the relevant production behavior be broken while these assertions still pass? Distinguish source-pattern checks, surrogate models, and tests that execute the implementation. Missing target-runtime access is a coverage limit, not an invitation to demand a fake test suite.

Report only actionable findings with a concrete triggering condition, affected path and line or symbol, impact, supporting evidence, and a proportionate correction direction. Calibrate severity to actual reachability and consequences. Separate uncertainty and preferences from defects; do not invent findings or demand unrelated redesign.

Keep the review read-only. Use focused probes only when they provide useful evidence within scope. Return findings first, followed by meaningful coverage limits. If no supported defect is found, say so. A clean source review does not establish runtime acceptance, and review does not authorize implementation or delivery.
