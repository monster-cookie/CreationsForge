using System.ComponentModel;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.RecordEditing;
using CreationsForge.RecordEditing.Drafts;

namespace CreationsForge.ViewModels;

public sealed partial class FormListEditorViewModel
{
    private static readonly (string Command, string Title, string? ClearCommand)[] VisibleFields =
    [
        ("form-list.set-editor-id", "Editor ID", "form-list.clear-editor-id"),
        ("starfield.form-list.set-name", "Name", "starfield.form-list.clear-name"),
        ("fallout4.form-list.set-name", "Name", "fallout4.form-list.clear-name"),
        ("form-list.replace-items", "Items", null),
        ("starfield.form-list.set-add-to-list", "Add to List", "starfield.form-list.clear-add-to-list"),
        ("starfield.form-list.replace-components", "Components", null),
        ("starfield.form-list.set-conditional-entries", "Conditional Entries", null),
        ("starfield.form-list.set-major-flags", "Record Flags", null),
        ("fallout4.form-list.set-major-record-flags", "Record Flags", null),
        ("skyrim.form-list.set-major-record-flags", "Record Flags", null),
        ("form-list.set-compressed", "Compressed", null),
        ("form-list.set-deleted", "Deleted", null),
        ("form-list.set-form-version", "Form Version", null),
        ("form-list.set-version-2", "Secondary Version", null),
        ("form-list.set-version-control", "Version Control", null)
    ];

    /// <summary>Builds direct field drafts from the current receipt-bound record seed and exact typed catalog.</summary>
    /// <returns>A typed failure when any supported field cannot be shown safely.</returns>
    private EngineError? RebuildFormFields()
    {
        ClearFormFields();
        if (SessionValue is null || CatalogContextValue is null ||
            !IsCurrentSeed(SessionValue, SeedValue))
        {
            return new EngineError(EngineErrorCode.ValidationFailed,
                "The record fields cannot be opened without the current staged record seed.");
        }

        var fields = new List<FormListFieldDraft>();
        foreach (var (commandName, title, clearCommandName) in VisibleFields)
        {
            var command = AvailableCommandsValue.FirstOrDefault(candidate =>
                string.Equals(candidate.CommandName, commandName, StringComparison.Ordinal));
            if (command is null)
            {
                continue;
            }

            var result = DraftFactory.Create(
                CatalogContextValue,
                command.Key,
                SeedValue,
                FormListDraftSeedSelection.CurrentValue(),
                ReadLimits);
            if (!result.Succeeded || result.Value is null)
            {
                foreach (var field in fields)
                {
                    field.Detach();
                }

                return result.Error ?? new EngineError(EngineErrorCode.ValidationFailed,
                    $"The {title} field could not be opened from the staged record.");
            }

            if (result.Value.Root is not RecordWireObjectDraftNode root || root.Properties.Count != 1)
            {
                foreach (var field in fields)
                {
                    field.Detach();
                }

                return new EngineError(EngineErrorCode.ValidationFailed,
                    $"The {title} field does not have a supported direct editor.");
            }

            var clearCommand = clearCommandName is null ? null : AvailableCommandsValue.FirstOrDefault(candidate =>
                string.Equals(candidate.CommandName, clearCommandName, StringComparison.Ordinal));
            var fieldDraft = new FormListFieldDraft(title, command, result.Value, clearCommand);
            fieldDraft.PropertyChanged += OnFormFieldPropertyChanged;
            fields.Add(fieldDraft);
        }

        FieldDraftsValue = Array.AsReadOnly(fields.ToArray());
        PublishFormFieldState(listChanged: true);
        return null;
    }

    /// <summary>Releases old direct field observations when the session or staged seed changes.</summary>
    private void ClearFormFields()
    {
        foreach (var field in FieldDraftsValue)
        {
            field.PropertyChanged -= OnFormFieldPropertyChanged;
            field.Detach();
        }

        FieldDraftsValue = Array.Empty<FormListFieldDraft>();
        PublishFormFieldState(listChanged: true);
    }

    private void OnFormFieldPropertyChanged(object? sender, PropertyChangedEventArgs eventArgs)
    {
        if (eventArgs.PropertyName == nameof(FormListFieldDraft.HasChanges))
        {
            PublishFormFieldState();
        }
    }

    private void PublishFormFieldState(bool listChanged = false)
    {
        if (listChanged)
        {
            OnPropertyChanged(nameof(FieldDrafts));
        }
        OnPropertyChanged(nameof(HasFormChanges));
        OnPropertyChanged(nameof(HasDraftChanges));
        OnPropertyChanged(nameof(CanMutateDraft));
        OnPropertyChanged(nameof(CanSaveForm));
        OnPropertyChanged(nameof(CanDiscardFormChanges));
        OnPropertyChanged(nameof(CanBeginNew));
        OnPropertyChanged(nameof(CanBeginOverride));
        OnPropertyChanged(nameof(CanBeginExistingOutput));
        RaiseCommandStates();
    }

    /// <summary>Applies changed visible fields through the existing typed FormList commands in display order.</summary>
    /// <returns>A task that completes after all changed fields are staged or one failure is reported.</returns>
    public async Task SaveFormAsync()
    {
        if (!CanSaveForm || SessionValue is not { } session || CatalogContextValue is null)
        {
            return;
        }

        IsSavingFormValue = true;
        PublishFormFieldState();
        var generation = Volatile.Read(ref WorkspaceGeneration);
        var completed = true;
        var stagedAny = false;
        try
        {
            foreach (var field in FieldDraftsValue.Where(candidate => candidate.HasChanges).ToArray())
            {
                if (!IsCurrentGeneration(generation, session.WorkspaceId) ||
                    !ReferenceEquals(SessionValue, session))
                {
                    completed = false;
                    break;
                }

                var command = field.ClearRequested ? field.ClearCommand : field.Command;
                if (command is null)
                {
                    completed = false;
                    await UiDispatcher.InvokeAsync(() => PublishError(new EngineError(
                        EngineErrorCode.InvalidRequest,
                        $"{field.Title} cannot be cleared in this game."))).ConfigureAwait(false);
                    break;
                }

                var draft = field.Draft;
                if (field.ClearRequested)
                {
                    var clear = DraftFactory.Create(
                        CatalogContextValue,
                        command.Key,
                        SeedValue,
                        FormListDraftSeedSelection.CurrentValue(),
                        ReadLimits);
                    if (!clear.Succeeded || clear.Value is null)
                    {
                        completed = false;
                        await UiDispatcher.InvokeAsync(() => PublishError(clear.Error)).ConfigureAwait(false);
                        break;
                    }

                    draft = clear.Value;
                }

                await UiDispatcher.InvokeAsync(() =>
                {
                    SelectedCommandValue = command;
                    DraftSelectionValue = FormListDraftSeedSelection.CurrentValue();
                    AttachDraft(draft);
                    ValidateDraft();
                    ClearError();
                    OnPropertyChanged(nameof(SelectedCommand));
                    RaiseCommandStates();
                }).ConfigureAwait(false);
                if (!CanApply)
                {
                    completed = false;
                    await UiDispatcher.InvokeAsync(() => PublishError(new EngineError(
                        EngineErrorCode.ValidationFailed,
                        $"Fix the {field.Title} field before saving this record."))).ConfigureAwait(false);
                    break;
                }

                var before = session.ExpectedRevision;
                await ApplyAsync().ConfigureAwait(false);
                if (session.ExpectedRevision != before)
                {
                    stagedAny = true;
                    await UiDispatcher.InvokeAsync(field.MarkApplied).ConfigureAwait(false);
                }

                if (session.ExpectedRevision == before || HasPendingOperation || HasError)
                {
                    completed = false;
                    break;
                }
            }
        }
        finally
        {
            Exception? refreshError = null;
            if (stagedAny && IsCurrentGeneration(generation, session.WorkspaceId))
            {
                try
                {
                    Task refreshTask = Task.CompletedTask;
                    await UiDispatcher.InvokeAsync(() =>
                        refreshTask = Host.RefreshAsync(session.FormKey, CancellationToken.None)).ConfigureAwait(false);
                    await refreshTask.ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    refreshError = exception;
                }

                await UiDispatcher.InvokeAsync(() =>
                {
                    if (IsCurrentGeneration(generation, session.WorkspaceId))
                    {
                        StagedRecordChanged?.Invoke(session.FormKey);
                    }
                }).ConfigureAwait(false);
            }

            await UiDispatcher.InvokeAsync(() =>
            {
                IsSavingFormValue = false;
                if (completed && IsCurrentGeneration(generation, session.WorkspaceId))
                {
                    var error = RebuildFormFields();
                    if (error is null && refreshError is null)
                    {
                        SetStatus("Record saved to the workspace. Use Save Changes to write the plugin file.");
                    }
                    else if (error is not null)
                    {
                        PublishError(new EngineError(error.Code,
                            $"The record was staged, but its fields could not be refreshed: {error.Message}"));
                    }
                }

                if (refreshError is not null && IsCurrentGeneration(generation, session.WorkspaceId) && !HasError)
                {
                    PublishError(new EngineError(EngineErrorCode.UnexpectedFailure,
                        $"The record was staged, but the browser could not be refreshed: {refreshError.Message}"));
                }

                PublishFormFieldState();
            }).ConfigureAwait(false);
        }
    }
}
