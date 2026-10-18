using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CreationsForge.Workbench;

/// <summary>One guided field editor built from a production <c>record_schema</c> field.</summary>
public sealed class WorkbenchFieldDraft : INotifyPropertyChanged
{
    private readonly IReadOnlyList<string> _languages;
    private string _stringValue = string.Empty;
    private string _numberText = string.Empty;
    private bool _booleanValue;
    private string _enumValue = string.Empty;
    private string _formKeyText = string.Empty;
    private string _targetLanguage;
    private bool _clearValue;
    private string _originalCanonical = "{\"kind\":\"Null\"}";

    /// <summary>Initializes a field editor.</summary>
    /// <param name="path">The registered field path.</param>
    /// <param name="kind">The closed value kind reported by the schema.</param>
    /// <param name="isNullable">Whether the schema accepts null.</param>
    /// <param name="isFlags">Whether enum choices combine.</param>
    /// <param name="isSupported">Whether the guided editor can change this kind.</param>
    /// <param name="choices">The legal enum names.</param>
    /// <param name="languages">The language names offered for a translated string.</param>
    /// <param name="referenceSummary">A short description of legal link targets, or an empty string.</param>
    /// <param name="unsupportedReason">Why the guided editor leaves this field unchanged.</param>
    public WorkbenchFieldDraft(
        string path,
        string kind,
        bool isNullable,
        bool isFlags,
        bool isSupported,
        IReadOnlyList<string> choices,
        IReadOnlyList<string> languages,
        string referenceSummary,
        string unsupportedReason)
    {
        Path = path;
        Kind = kind;
        IsNullable = isNullable;
        IsFlags = isFlags;
        IsSupported = isSupported;
        Choices = choices;
        _languages = languages;
        ReferenceSummary = referenceSummary ?? string.Empty;
        UnsupportedReason = unsupportedReason ?? string.Empty;
        _targetLanguage = languages.Count == 0 ? "English" : languages[0];
        Flags = new ObservableCollection<WorkbenchFlagChoice>(choices.Select(choice => new WorkbenchFlagChoice(choice)));
        foreach (var flag in Flags)
        {
            flag.PropertyChanged += (_, _) => ClearValue = false;
        }

        Translations = [];
        Translations.CollectionChanged += (_, args) =>
        {
            if (args.NewItems is null)
            {
                return;
            }

            foreach (WorkbenchTranslationRow row in args.NewItems)
            {
                row.PropertyChanged += (_, _) =>
                {
                    if (row.Value.Length > 0)
                    {
                        ClearValue = false;
                    }
                };
            }
        };
    }

    /// <summary>Raised when an edited value changes.</summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>Gets the registered field path.</summary>
    public string Path { get; }

    /// <summary>Gets the closed value kind.</summary>
    public string Kind { get; }

    /// <summary>Gets whether the schema accepts null.</summary>
    public bool IsNullable { get; }

    /// <summary>Gets whether enum choices combine.</summary>
    public bool IsFlags { get; }

    /// <summary>Gets whether the guided editor can change this field.</summary>
    public bool IsSupported { get; }

    /// <summary>Gets the legal enum names.</summary>
    public IReadOnlyList<string> Choices { get; }

    /// <summary>Gets the language names offered for a translated string.</summary>
    public IReadOnlyList<string> Languages => _languages;

    /// <summary>Gets a short description of legal link targets, or an empty string.</summary>
    public string ReferenceSummary { get; }

    /// <summary>Gets why the guided editor leaves this field unchanged.</summary>
    public string UnsupportedReason { get; }

    /// <summary>Gets whether a single-line string box is shown.</summary>
    public bool IsString => IsSupported && Kind == "String";

    /// <summary>Gets whether a checkbox is shown.</summary>
    public bool IsBoolean => IsSupported && Kind == "Boolean";

    /// <summary>Gets whether a number box is shown.</summary>
    public bool IsNumber => IsSupported && Kind is "SignedInteger" or "UnsignedInteger" or "FloatingPoint";

    /// <summary>Gets whether a single-choice dropdown is shown.</summary>
    public bool IsEnum => IsSupported && Kind == "Enum" && !IsFlags;

    /// <summary>Gets whether flag checkboxes are shown.</summary>
    public bool IsFlagsEnum => IsSupported && Kind == "Enum" && IsFlags;

    /// <summary>Gets whether a FormKey box is shown.</summary>
    public bool IsFormLink => IsSupported && Kind == "FormLink";

    /// <summary>Gets whether the language table is shown.</summary>
    public bool IsTranslated => IsSupported && Kind == "TranslatedString";

    /// <summary>Gets whether the unsupported-shape note is shown.</summary>
    public bool IsUnsupported => !IsSupported;

    /// <summary>Gets whether the clear checkbox is shown.</summary>
    public bool CanClear => IsSupported && IsNullable;

    /// <summary>Gets whether <see cref="ReferenceSummary"/> should be shown.</summary>
    public bool HasReferenceSummary => ReferenceSummary.Length > 0;

    /// <summary>Gets the flag checkboxes.</summary>
    public ObservableCollection<WorkbenchFlagChoice> Flags { get; }

    /// <summary>Gets the language and value rows.</summary>
    public ObservableCollection<WorkbenchTranslationRow> Translations { get; }

    /// <summary>Gets or sets the string value. A changed non-empty value clears <see cref="ClearValue"/>.</summary>
    public string StringValue
    {
        get => _stringValue;
        set => EditText(ref _stringValue, value);
    }

    /// <summary>Gets or sets the number text. A changed non-empty value clears <see cref="ClearValue"/>.</summary>
    public string NumberText
    {
        get => _numberText;
        set => EditText(ref _numberText, value);
    }

    /// <summary>Gets or sets the Boolean value. A changed value clears <see cref="ClearValue"/>.</summary>
    public bool BooleanValue
    {
        get => _booleanValue;
        set
        {
            if (_booleanValue != value)
            {
                ClearValue = false;
            }

            Set(ref _booleanValue, value);
        }
    }

    /// <summary>Gets or sets the single enum name. A changed non-empty value clears <see cref="ClearValue"/>.</summary>
    public string EnumValue
    {
        get => _enumValue;
        set => EditText(ref _enumValue, value);
    }

    /// <summary>Gets or sets the FormKey text. A changed non-empty value clears <see cref="ClearValue"/>.</summary>
    public string FormKeyText
    {
        get => _formKeyText;
        set => EditText(ref _formKeyText, value);
    }

    /// <summary>Gets or sets the translated string's target language. Changing it clears <see cref="ClearValue"/>.</summary>
    public string TargetLanguage
    {
        get => _targetLanguage;
        set
        {
            var text = string.IsNullOrWhiteSpace(value) ? "English" : value;
            if (!string.Equals(_targetLanguage, text, StringComparison.Ordinal))
            {
                ClearValue = false;
            }

            Set(ref _targetLanguage, text);
        }
    }

    /// <summary>Gets or sets whether Apply sends null for this nullable field. A loaded null starts checked; entering a value clears it.</summary>
    public bool ClearValue
    {
        get => _clearValue;
        set => Set(ref _clearValue, value);
    }

    /// <summary>Gets the canonical value captured when the field was loaded. Unchanged fields are not sent.</summary>
    internal string OriginalCanonical
    {
        get => _originalCanonical;
        set => _originalCanonical = value;
    }

    /// <summary>Adds a language row when that language is not already present.</summary>
    /// <param name="language">The language to add. A blank value uses the target language.</param>
    public void AddTranslation(string? language)
    {
        var name = string.IsNullOrWhiteSpace(language) ? TargetLanguage : language;
        if (Translations.Any(row => string.Equals(row.Language, name, StringComparison.Ordinal)))
        {
            return;
        }

        Translations.Add(new WorkbenchTranslationRow(name, string.Empty, _languages, RemoveTranslation));
    }

    /// <summary>Removes a language row.</summary>
    /// <param name="row">The row to remove.</param>
    public void RemoveTranslation(WorkbenchTranslationRow row)
    {
        Translations.Remove(row);
    }

    private void EditText(ref string field, string? value, [CallerMemberName] string? propertyName = null)
    {
        var text = value ?? string.Empty;
        if (text.Length > 0 && !string.Equals(field, text, StringComparison.Ordinal))
        {
            ClearValue = false;
        }

        Set(ref field, text, propertyName);
    }

    private void Set<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return;
        }

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
