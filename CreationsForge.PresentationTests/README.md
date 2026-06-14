# CreationsForge.PresentationTests

Presentation tests cover UI-facing services, view models, commands, render-data preparation, and Avalonia headless
smoke tests. They must not start the real desktop app lifetime, open live windows outside Avalonia headless, require
GPU/OpenGL output, or depend on machine-specific paths.

Run the tests from the repository root:

```powershell
dotnet test ./CreationsForge.PresentationTests/CreationsForge.PresentationTests.csproj
```

Avalonia control tests should use `[AvaloniaFact]` from `Avalonia.Headless.XUnit` and should flush dispatcher work with
`Dispatcher.UIThread.RunJobs()` before making assertions when bindings, attachment, or layout work may be pending. The
test projects use xUnit v3.

Prefer stable `AutomationProperties.AutomationId` values when locating production controls. Avoid assertions against
visible text, exact layout positions, colors, pixel output, operating-system file paths, or OpenGL-rendered content.
