using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace CreationsForge.PresentationTests.Headless;

public static class ControlFinder
{
    public static TControl? FindByAutomationId<TControl>(Control root, string automationId)
        where TControl : Control
    {
        if (IsMatch<TControl>(root, automationId))
        {
            return (TControl)root;
        }

        return root.GetVisualDescendants()
            .OfType<TControl>()
            .FirstOrDefault(control => IsMatch<TControl>(control, automationId));
    }

    private static bool IsMatch<TControl>(Control control, string automationId)
        where TControl : Control
    {
        return control is TControl &&
            string.Equals(control.GetValue(AutomationProperties.AutomationIdProperty), automationId, StringComparison.Ordinal);
    }
}
