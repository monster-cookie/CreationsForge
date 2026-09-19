using System.ComponentModel;
using CreationsForge.RecordEditing;

namespace CreationsForge.RecordEditing.Drafts;

/// <summary>Pairs one visible FormList field with its existing typed replacement command.</summary>
public sealed class FormListFieldDraft : INotifyPropertyChanged
{
    private bool ClearRequestedValue;
    private bool IsAppliedValue;

    internal FormListFieldDraft(
        string title,
        FormListCommandPresentation command,
        FormListDraft draft,
        FormListCommandPresentation? clearCommand = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(draft);
        if (!string.Equals(command.CommandName, draft.CommandName, StringComparison.Ordinal))
        {
            throw new ArgumentException("The visible field must use its exact typed command draft.", nameof(draft));
        }

        Title = title;
        Command = command;
        Draft = draft;
        ClearCommand = clearCommand;
        Draft.PropertyChanged += OnDraftPropertyChanged;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Title { get; }

    public FormListCommandPresentation Command { get; }

    public FormListCommandPresentation? ClearCommand { get; }

    public FormListDraft Draft { get; }

    public bool CanClear => ClearCommand is not null;

    public bool ClearRequested
    {
        get => ClearRequestedValue;
        set
        {
            if (!CanClear || ClearRequestedValue == value)
            {
                return;
            }

            ClearRequestedValue = value;
            IsAppliedValue = false;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ClearRequested)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasChanges)));
        }
    }

    public bool HasChanges => !IsAppliedValue && (ClearRequestedValue || Draft.HasChanges);

    internal void MarkApplied()
    {
        IsAppliedValue = true;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasChanges)));
    }

    internal void Detach()
    {
        Draft.PropertyChanged -= OnDraftPropertyChanged;
    }

    private void OnDraftPropertyChanged(object? sender, PropertyChangedEventArgs eventArgs)
    {
        if (eventArgs.PropertyName != nameof(FormListDraft.HasChanges))
        {
            return;
        }

        IsAppliedValue = false;
        if (ClearRequestedValue)
        {
            ClearRequestedValue = false;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ClearRequested)));
        }

        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasChanges)));
    }
}
