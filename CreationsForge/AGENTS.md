# CreationsForge presentation rules

This folder contains the Avalonia presentation application: views, view models, application startup UI code, dialogs,
navigation coordination, and presentation-only services.

## Boundaries

- UI framework code belongs here, not in CreationsForge.Core.
- Do not put SQL, NPoco access, Mutagen parsing, import orchestration, or database migration logic in views, code-behind, or view models.
- View models may coordinate UI state, commands, dialogs, navigation, and calls into application services.
- Business rules belong in Core or game-specific services.
- Keep code-behind minimal. Use it only for UI composition or framework integration that is awkward to express elsewhere.
- Do not move view models, commands, dialog services, navigation services, Avalonia controls, or binding helpers into Core.

## Avalonia and MVVM

- Follow the existing Avalonia and MVVM patterns in this project.
- Preserve existing view, view model, command, and service naming conventions.
- Long-running work must not block the UI thread.
- Use async commands where existing project patterns support them.
- UI-bound collection changes must happen on the UI thread.
- Avoid broad UI rewrites, restyling, or layout churn unless explicitly approved in the plan.
- Do not change user workflows, keyboard/mouse behavior, or navigation behavior without calling it out in the plan.

## Asset preview UI

- Keep rendering and preview UI defensive. Asset preview failures should not crash the whole app.
- Prefer a clear fallback state over throwing from UI rendering paths.
- Do not load large assets synchronously on the UI thread.
- Dispose native, graphics, stream, and file handles deterministically.

## Validation

When this project is touched, the plan should consider:

- CreationsForge.PresentationTests for view model or headless UI behavior.
- Manual smoke validation of affected UI workflows when automated coverage is not practical.
