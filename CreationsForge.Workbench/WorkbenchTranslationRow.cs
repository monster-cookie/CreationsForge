using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CreationsForge.Workbench;

/// <summary>One language and its text in a translated-string table.</summary>
public sealed class WorkbenchTranslationRow : INotifyPropertyChanged
{
    private readonly Action<WorkbenchTranslationRow> _remove;
    private string _language;
    private string _value;

    /// <summary>Initializes a language row.</summary>
    /// <param name="language">The language name accepted by the output tools.</param>
    /// <param name="value">The text for <paramref name="language"/>.</param>
    /// <param name="languages">The language names offered in the row dropdown.</param>
    /// <param name="remove">Removes this row from its field.</param>
    public WorkbenchTranslationRow(
        string language,
        string value,
        IReadOnlyList<string> languages,
        Action<WorkbenchTranslationRow> remove)
    {
        _language = language ?? string.Empty;
        _value = value ?? string.Empty;
        Languages = languages;
        _remove = remove;
    }

    /// <summary>Raised when the language or text changes.</summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>Gets the language names offered in the dropdown.</summary>
    public IReadOnlyList<string> Languages { get; }

    /// <summary>Gets or sets the language name for this row.</summary>
    public string Language
    {
        get => _language;
        set => Set(ref _language, value ?? string.Empty);
    }

    /// <summary>Gets or sets the text stored for <see cref="Language"/>.</summary>
    public string Value
    {
        get => _value;
        set => Set(ref _value, value ?? string.Empty);
    }

    /// <summary>Removes this row from the translated field that owns it.</summary>
    public void Remove() => _remove(this);

    private void Set(ref string field, string value, [CallerMemberName] string? propertyName = null)
    {
        if (string.Equals(field, value, StringComparison.Ordinal))
        {
            return;
        }

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
