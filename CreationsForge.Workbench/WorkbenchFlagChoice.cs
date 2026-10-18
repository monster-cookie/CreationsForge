using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CreationsForge.Workbench;

/// <summary>One named flag the guided editor can include in an enum value.</summary>
public sealed class WorkbenchFlagChoice : INotifyPropertyChanged
{
    private bool _isSelected;

    /// <summary>Initializes a flag choice.</summary>
    /// <param name="name">The exact enum name.</param>
    public WorkbenchFlagChoice(string name)
    {
        Name = name ?? string.Empty;
    }

    /// <summary>Raised when <see cref="IsSelected"/> changes.</summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>Gets the exact enum name.</summary>
    public string Name { get; }

    /// <summary>Gets or sets whether this flag is included in the combined value.</summary>
    public bool IsSelected
    {
        get => _isSelected;
        set => Set(ref _isSelected, value);
    }

    private void Set(ref bool field, bool value, [CallerMemberName] string? propertyName = null)
    {
        if (field == value)
        {
            return;
        }

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
