using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace CreationsForge.DataValidationTests.Validation.UI;

/// <summary>
/// Finds controls in an Avalonia visual tree by automation identifier for headless validation tests.
/// </summary>
public static class ControlFinder
{
    /// <summary>
    /// Finds the first matching control with the supplied automation identifier.
    /// </summary>
    /// <typeparam name="TControl">The expected Avalonia control type.</typeparam>
    /// <param name="root">The root control whose visual tree should be searched.</param>
    /// <param name="automationId">The automation identifier assigned to the target control.</param>
    /// <returns>The matching control, or <c>null</c> when none is found.</returns>
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

    /// <summary>
    /// Determines whether the supplied control has the expected type and automation identifier.
    /// </summary>
    /// <typeparam name="TControl">The expected Avalonia control type.</typeparam>
    /// <param name="control">The control to inspect.</param>
    /// <param name="automationId">The automation identifier assigned to the target control.</param>
    /// <returns><c>true</c> when the control matches the expected type and automation identifier.</returns>
    private static bool IsMatch<TControl>(Control control, string automationId)
        where TControl : Control
    {
        return control is TControl &&
            string.Equals(control.GetValue(AutomationProperties.AutomationIdProperty), automationId, StringComparison.Ordinal);
    }
}
