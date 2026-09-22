---
name: adversarial-review
description: Challenge major new features or newly introduced frameworks/libraries, or perform an explicitly requested adversarial review. Not a default review for documentation, routine fixes, or minor changes.
---

# Adversarial review

Use this review for major new features, newly introduced frameworks or libraries, or an explicit request. Routine fixes and documentation changes do not need this stage solely because they are large or stateful. Follow-up review should address relevant corrections rather than restarting after unrelated edits.

Identify the invariants the real implementation must preserve, then try to find a reachable sequence that violates them. Focus on assumptions about initialization, duplicate callbacks, cancellation, retries, partial effects, cleanup, recovery, version skew, or interleaving where those conditions apply. Choose scenarios from the code; do not mechanically enumerate every imaginable edge case.

Challenge both implementation and evidence. A separately implemented simulation does not prove that production handles its scenarios. Use a focused executable or inspectable example when it helps establish a real failure, and label environment assumptions honestly.

Keep the review read-only. Report each supported finding with the triggering sequence, expected and actual behavior, impact, exact location, evidence, and correction direction. Distinguish confirmed failures from unresolved hypotheses. If no defect is supported, state what was examined and the remaining limits. A security-specific review or product scan requires its own user request.
