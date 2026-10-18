using System.Collections.ObjectModel;
using System.Text.Json.Nodes;
using Serilog;

namespace CreationsForge.Workbench;

/// <summary>Guided record authoring for the standalone MCP Workbench.</summary>
public sealed partial class WorkbenchViewModel
{
    private const int SearchPageLimit = 4;
    private const int SchemaPageLimit = 8;
    private string? _publishedPath;
    private string? _familiesRelease;
    private string? _authoringMode = string.Empty;
    private string? _baselineFormKey;
    private string? _baselineContaining;
    private string? _compareLeftFormKey;
    private string? _compareLeftContaining;
    private string? _compareLeftLabel;
    private string? _compareRightFormKey;
    private string? _compareRightContaining;
    private string? _unrelatedFormKey;
    private string? _unrelatedContaining;
    private string? _unrelatedEditorId;
    private Dictionary<string, string>? _unrelatedFields;
    private string _selectedFamily = "Keyword";
    private string _editorIdFilter = string.Empty;
    private string _observations = string.Empty;
    private WorkbenchRecordHit? _selectedHit;
    private bool _authoringDirty;

    /// <summary>Gets the record families returned by <c>record_types</c>.</summary>
    public ObservableCollection<string> Families { get; } = [];

    /// <summary>Gets the records returned by the latest find.</summary>
    public ObservableCollection<WorkbenchRecordHit> SearchHits { get; } = [];

    /// <summary>Gets the field editors for the record being created or overridden.</summary>
    public ObservableCollection<WorkbenchFieldDraft> Fields { get; } = [];

    /// <summary>Gets or sets the family used by find, new, and override.</summary>
    public string SelectedFamily
    {
        get => _selectedFamily;
        set => Set(ref _selectedFamily, string.IsNullOrWhiteSpace(value) ? "Keyword" : value);
    }

    /// <summary>Gets or sets the EditorID filter. Blank lists every winning record in the family, up to the page limit.</summary>
    public string EditorIdFilter
    {
        get => _editorIdFilter;
        set => Set(ref _editorIdFilter, value ?? string.Empty);
    }

    /// <summary>Gets or sets the search hit that Override copies.</summary>
    public WorkbenchRecordHit? SelectedHit
    {
        get => _selectedHit;
        set
        {
            if (ReferenceEquals(_selectedHit, value))
            {
                return;
            }

            _selectedHit = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanOverrideRecord));
        }
    }

    /// <summary>Gets the observation log, including the published path and fresh-session reads.</summary>
    public string Observations
    {
        get => _observations;
        private set => Set(ref _observations, value);
    }

    /// <summary>Gets the published or opened output path, or text that says no path has been recorded.</summary>
    public string PublishedPathText =>
        string.IsNullOrWhiteSpace(_publishedPath)
            ? "No output path has been recorded."
            : "Output path: " + _publishedPath;

    /// <summary>Gets whether Apply changed the workspace after the last successful save or discard.</summary>
    public bool HasUnsavedEdits => _authoringDirty;

    /// <summary>Gets a short description of the current new or override draft.</summary>
    public string AuthoringModeText => _authoringMode switch
    {
        "Create" => "New " + SelectedFamily,
        "Override" => "Override " + (_compareLeftLabel ?? _baselineFormKey ?? "selected record"),
        _ => "Choose New or Override.",
    };

    /// <summary>Gets whether Find can call <c>records_search</c>.</summary>
    public bool CanFindRecords => CanAuthor;

    /// <summary>Gets whether New can load a schema and start a create draft.</summary>
    public bool CanCreateRecord => CanAuthor;

    /// <summary>Gets whether Override can read <see cref="SelectedHit"/>.</summary>
    public bool CanOverrideRecord => CanAuthor && SelectedHit is not null;

    /// <summary>Gets whether Apply can send the current draft.</summary>
    public bool CanApplyRecord => CanAuthor && Fields.Count > 0 && _authoringMode is "Create" or "Override";

    /// <summary>Gets whether Compare can call <c>record_compare</c> for the source and output versions.</summary>
    public bool CanCompareRecord =>
        CanAuthor
        && !string.IsNullOrWhiteSpace(_compareLeftFormKey)
        && !string.IsNullOrWhiteSpace(_compareRightFormKey);

    /// <summary>Gets whether Preview can call <c>workspace_preview</c>.</summary>
    public bool CanPreviewChanges => CanAuthor;

    /// <summary>Gets whether Save can call <c>workspace_save</c>.</summary>
    public bool CanSaveChanges => CanAuthor;

    /// <summary>Gets whether Discard can call <c>workspace_discard</c>.</summary>
    public bool CanDiscardChanges => CanAuthor;

    /// <summary>Gets whether a fresh MCP session can reopen the recorded output path.</summary>
    public bool CanReopenFresh =>
        !_busy
        && !_connecting
        && !_reopening
        && !_closing
        && !_hostExitObserved
        && !_authoringDirty
        && !string.IsNullOrWhiteSpace(_publishedPath);

    /// <summary>Searches the open output for winning records in <see cref="SelectedFamily"/>.</summary>
    /// <returns>A task that completes when the hits are shown or the failure is visible.</returns>
    public Task FindRecordsAsync() => RunSessionOperationAsync(FindCoreAsync, "Finding records");

    /// <summary>Loads the selected family's schema into an empty create draft.</summary>
    /// <returns>A task that completes when the draft is ready or the failure is visible.</returns>
    public Task NewRecordAsync() => RunSessionOperationAsync(NewCoreAsync, "Loading new record");

    /// <summary>Reads <see cref="SelectedHit"/> into an override draft and remembers an unrelated hit.</summary>
    /// <returns>A task that completes when the draft is ready or the failure is visible.</returns>
    public Task OverrideSelectedAsync() => RunSessionOperationAsync(OverrideCoreAsync, "Reading record");

    /// <summary>Sends the draft with <c>record_create</c> or <c>record_override</c>. A failure keeps the form and does not report a save.</summary>
    /// <returns>A task that completes when the result or the validation failure is visible.</returns>
    public Task ApplyRecordAsync() => RunSessionOperationAsync(ApplyCoreAsync, "Applying record");

    /// <summary>Compares the exact source version with the output version.</summary>
    /// <returns>A task that completes when the difference summary is visible or the call fails.</returns>
    public Task CompareRecordAsync() => RunSessionOperationAsync(CompareCoreAsync, "Comparing records");

    /// <summary>Shows unsaved records. Preview does not write the output file.</summary>
    /// <returns>A task that completes when the preview summary is visible or the call fails.</returns>
    public Task PreviewChangesAsync() => RunSessionOperationAsync(PreviewCoreAsync, "Previewing changes");

    /// <summary>Saves the workspace. Only a <c>Succeeded</c> status records the output path.</summary>
    /// <returns>A task that completes when the save result is visible.</returns>
    public Task SaveChangesAsync() => RunSessionOperationAsync(SaveCoreAsync, "Saving output");

    /// <summary>Discards unsaved edits. Discard does not write the output file.</summary>
    /// <returns>A task that completes when the discard result is visible.</returns>
    public Task DiscardChangesAsync() => RunSessionOperationAsync(DiscardCoreAsync, "Discarding changes");

    /// <summary>Closes the current host and opens the recorded output in a new MCP process.</summary>
    /// <returns>A task that completes when the fresh session has reported the file and any read-back, or when reopen was refused.</returns>
    public async Task ReopenFreshAsync()
    {
        if (_reopening || _busy || _connecting || _closing || _hostExitObserved)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(_publishedPath))
        {
            Report("Reopen refused.", "Save or open an output before reopening. No output path is recorded.");
            return;
        }

        if (_authoringDirty)
        {
            Report("Reopen refused.", "Unsaved edits are not on disk. Save or discard them before reopening. The output file was not saved by this action.");
            return;
        }

        var memory = CaptureMemory();
        var path = _publishedPath;
        _reopening = true;
        Publish(NotifyCommands);
        try
        {
            if (_client is not null && _workspaceId is not null)
            {
                var dirty = await ProbeDirtyAsync().ConfigureAwait(false);
                if (dirty != false || _closing)
                {
                    if (dirty == true)
                    {
                        Report("Reopen refused.", "The workspace still has unsaved changes. Save or discard them before reopening. The output file was not saved by this action.");
                    }

                    return;
                }
            }

            if (_client is not null)
            {
                await CloseAsync().ConfigureAwait(false);
            }

            AppendObservation("Starting a fresh MCP session for " + path + ".");
            await ConnectAsync().ConfigureAwait(false);
            if (_client is null || _closing)
            {
                AppendObservation("Fresh session did not start. Output path: " + path + ". File exists: " + File.Exists(path).ToString().ToLowerInvariant() + ".");
                return;
            }

            await OpenWorkspaceAsync().ConfigureAwait(false);
            if (_workspaceId is null || _closing)
            {
                AppendObservation("Fresh session did not open the workspace. Output path: " + path + ".");
                return;
            }

            await OpenOutputAsync(createNew: false).ConfigureAwait(false);
            if (!_outputAttached || _closing)
            {
                AppendObservation("Fresh session did not open " + path + ". File exists: " + File.Exists(path).ToString().ToLowerInvariant() + ". This step did not save the file.");
                return;
            }

            await ReadBackAsync(memory).ConfigureAwait(false);
        }
        finally
        {
            _reopening = false;
            Publish(NotifyCommands);
        }
    }

    private bool AuthoringSessionReady =>
        _client is not null && _outputAttached && !_closing && !_hostExitObserved;

    private bool CanAuthor => AuthoringSessionReady && !_busy && !_reopening;

    private async Task FindCoreAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (!Ready("Find"))
            {
                return;
            }

            await EnsureFamiliesAsync(cancellationToken).ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(SelectedFamily))
            {
                Report("Find failed.", "Choose a record family. The output file was not changed.");
                return;
            }

            var truncated = false;
            var hits = new List<WorkbenchRecordHit>();
            string? cursor = null;
            for (var page = 0; page < SearchPageLimit; page++)
            {
                var result = await CallAsync("records_search", SearchArguments(cursor), cancellationToken).ConfigureAwait(false);
                hits.AddRange(WorkbenchAuthoringCodec.ReadHits(result));
                cursor = WorkbenchAuthoringCodec.NextCursor(result);
                if (string.IsNullOrWhiteSpace(cursor))
                {
                    break;
                }

                truncated = page == SearchPageLimit - 1;
            }

            Publish(() =>
            {
                SearchHits.Clear();
                SelectedHit = null;
                foreach (var hit in hits)
                {
                    SearchHits.Add(hit);
                }

                Status = truncated
                    ? "Showing the first " + hits.Count + " matches. Narrow the EditorID filter."
                    : "Found " + hits.Count + " record(s).";
                Diagnostics = Status;
                OnPropertyChanged(nameof(CanOverrideRecord));
            });
        }
        catch (OperationCanceledException)
        {
            ReportCanceled("Find");
        }
        catch (Exception exception)
        {
            ReportFault("Find", exception);
        }
    }

    private async Task NewCoreAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (!Ready("New record"))
            {
                return;
            }

            await EnsureFamiliesAsync(cancellationToken).ConfigureAwait(false);
            var pages = await SchemaPagesAsync(SelectedFamily, cancellationToken).ConfigureAwait(false);
            var drafts = pages.SelectMany(page => WorkbenchAuthoringCodec.ReadSchemaFields(page, Languages)).ToArray();
            foreach (var field in drafts)
            {
                if (field.IsTranslated)
                {
                    field.TargetLanguage = Language;
                    field.AddTranslation(Language);
                }
            }

            WorkbenchAuthoringCodec.Snapshot(drafts);
            Publish(() => ReplaceDraft("Create", null, null, drafts));
            _baselineFormKey = null;
            _baselineContaining = null;
            _compareLeftFormKey = null;
            _compareLeftContaining = null;
            _compareLeftLabel = null;
            _compareRightFormKey = null;
            _compareRightContaining = null;
            Report("New " + SelectedFamily + " draft.", "Fill the fields and Apply. Apply does not write the output file.");
        }
        catch (OperationCanceledException)
        {
            ReportCanceled("New record");
        }
        catch (Exception exception)
        {
            ReportFault("New record", exception);
        }
    }

    private async Task OverrideCoreAsync(CancellationToken cancellationToken)
    {
        try
        {
            var selected = SelectedHit;
            if (!Ready("Override") || selected is null)
            {
                Report("Override needs a selected record.", "Find a record and select the exact source version to override.");
                return;
            }

            await EnsureFamiliesAsync(cancellationToken).ConfigureAwait(false);
            var schema = await SchemaPagesAsync(SelectedFamily, cancellationToken).ConfigureAwait(false);
            var drafts = schema.SelectMany(page => WorkbenchAuthoringCodec.ReadSchemaFields(page, Languages)).ToArray();
            var read = await ReadPagesAsync(selected.FormKey, selected.ContainingModKey, cancellationToken).ConfigureAwait(false);
            foreach (var page in read)
            {
                WorkbenchAuthoringCodec.ApplyReadPage(drafts, page);
            }

            foreach (var field in drafts.Where(field => field.IsTranslated && field.Translations.Count == 0))
            {
                field.TargetLanguage = Language;
                field.AddTranslation(Language);
            }

            WorkbenchAuthoringCodec.Snapshot(drafts);
            var witness = SearchHits.FirstOrDefault(hit => !string.Equals(hit.FormKey, selected.FormKey, StringComparison.Ordinal));
            Dictionary<string, string>? witnessFields = null;
            if (witness is not null)
            {
                witnessFields = await ReadFieldMapAsync(witness.FormKey, witness.ContainingModKey, cancellationToken).ConfigureAwait(false);
            }

            _baselineFormKey = selected.FormKey;
            _baselineContaining = selected.ContainingModKey;
            _compareLeftFormKey = selected.FormKey;
            _compareLeftContaining = selected.ContainingModKey;
            _compareLeftLabel = selected.EditorId.Length == 0 ? selected.FormKey : selected.EditorId;
            _compareRightFormKey = null;
            _compareRightContaining = null;
            _unrelatedFormKey = witness?.FormKey;
            _unrelatedContaining = witness?.ContainingModKey;
            _unrelatedEditorId = witness?.EditorId;
            _unrelatedFields = witnessFields;
            Publish(() => ReplaceDraft("Override", selected.FormKey, selected.ContainingModKey, drafts));
            Report(
                "Override draft for " + _compareLeftLabel + ".",
                "Text storage " + TextStorageMode + ", language " + Language + ". Apply copies this exact source and does not write the output file.");
        }
        catch (OperationCanceledException)
        {
            ReportCanceled("Override");
        }
        catch (Exception exception)
        {
            ReportFault("Override", exception);
        }
    }

    private async Task ApplyCoreAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (!Ready("Apply") || _authoringMode is not ("Create" or "Override"))
            {
                Report("Apply needs New or Override.", "Choose New or Override before applying fields. The output file was not saved.");
                return;
            }

            if (!TryRevision(out var revision))
            {
                Report("Apply failed.", "The session has no revision. Reopen the output and try again. The output file was not saved.");
                return;
            }

            var error = WorkbenchAuthoringCodec.TryBuildChanges(Fields.ToArray(), out var changes);
            if (error is not null)
            {
                Report("Validation failed.", error + " The form was kept. The output file was not saved.");
                return;
            }

            if (changes.Count == 0)
            {
                Report("Nothing to apply.", "No field values changed. The output file was not saved.");
                return;
            }

            var workspaceId = CurrentWorkspaceId();
            if (workspaceId is null)
            {
                Report("Apply failed.", "No workspace is open. The output file was not saved.");
                return;
            }

            JsonObject result;
            if (_authoringMode == "Create")
            {
                result = await CallAsync("record_create", new JsonObject
                {
                    ["operationId"] = OperationId("create"),
                    ["workspaceId"] = workspaceId,
                    ["familyId"] = SelectedFamily,
                    ["expectedRevision"] = JsonValue.Create(revision),
                    ["changes"] = changes,
                }, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(_baselineFormKey) || string.IsNullOrWhiteSpace(_baselineContaining))
                {
                    Report("Apply failed.", "The override has no exact source. The output file was not saved.");
                    return;
                }

                result = await CallAsync("record_override", new JsonObject
                {
                    ["operationId"] = OperationId("override"),
                    ["workspaceId"] = workspaceId,
                    ["familyId"] = SelectedFamily,
                    ["formKey"] = _baselineFormKey,
                    ["containingModKey"] = _baselineContaining,
                    ["expectedRevision"] = JsonValue.Create(revision),
                    ["changes"] = changes,
                }, cancellationToken).ConfigureAwait(false);
            }

            if (!WorkbenchAuthoringCodec.TryReadPublishedRecord(result, out var formKey, out var containing))
            {
                SetAuthoringDirty(true);
                Report("Apply did not return a record.", result.ToJsonString(IndentedJson) + Environment.NewLine + "The output file was not saved.");
                return;
            }

            _baselineFormKey = formKey;
            _baselineContaining = containing;
            _compareRightFormKey = formKey;
            _compareRightContaining = containing;
            _authoringMode = "Override";
            SetAuthoringDirty(true);
            WorkbenchAuthoringCodec.Snapshot(Fields.ToArray());
            Log.ForContext<WorkbenchViewModel>().Information(
                "Applied {FamilyId} {FormKey} from {ContainingModKey} in memory. The output file was not saved.",
                SelectedFamily,
                formKey,
                containing);
            Publish(() => OnPropertyChanged(nameof(AuthoringModeText)));
            Report(
                "Applied in memory. The output file was not saved.",
                "Applied " + SelectedFamily + " with text storage " + TextStorageMode + " and language " + Language + ". The output file was not saved.");
            AppendObservation("Applied " + SelectedFamily + " " + formKey + " in memory with text storage " + TextStorageMode + " and language " + Language + ". The output file was not saved.");
        }
        catch (OperationCanceledException)
        {
            ReportCanceled("Apply");
        }
        catch (Exception exception)
        {
            ReportFault("Apply", exception);
        }
    }

    private async Task CompareCoreAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (!Ready("Compare") || string.IsNullOrWhiteSpace(_compareLeftFormKey) || string.IsNullOrWhiteSpace(_compareRightFormKey))
            {
                Report("Compare needs an override.", "Override a source record and Apply it before comparing. Compare does not write the output file.");
                return;
            }

            var workspaceId = CurrentWorkspaceId();
            var result = await CallAsync("record_compare", new JsonObject
            {
                ["workspaceId"] = workspaceId,
                ["familyId"] = SelectedFamily,
                ["leftFormKey"] = _compareLeftFormKey,
                ["leftContainingModKey"] = _compareLeftContaining,
                ["rightFormKey"] = _compareRightFormKey,
                ["rightContainingModKey"] = _compareRightContaining,
            }, cancellationToken).ConfigureAwait(false);
            var summary = WorkbenchAuthoringCodec.SummarizeCompare(result);
            Report("Compared the source and the output.", summary);
            AppendObservation(summary);
        }
        catch (OperationCanceledException)
        {
            ReportCanceled("Compare");
        }
        catch (Exception exception)
        {
            ReportFault("Compare", exception);
        }
    }

    private async Task PreviewCoreAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (!Ready("Preview"))
            {
                return;
            }

            var result = await CallAsync(
                "workspace_preview",
                new JsonObject { ["workspaceId"] = CurrentWorkspaceId() },
                cancellationToken).ConfigureAwait(false);
            var summary = WorkbenchAuthoringCodec.SummarizePreview(result);
            Report("Preview does not write the output file.", summary);
            AppendObservation(summary);
        }
        catch (OperationCanceledException)
        {
            ReportCanceled("Preview");
        }
        catch (Exception exception)
        {
            ReportFault("Preview", exception);
        }
    }

    private async Task SaveCoreAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (!Ready("Save"))
            {
                return;
            }

            var workspaceId = CurrentWorkspaceId();
            var result = await CallAsync("workspace_save", new JsonObject
            {
                ["operationId"] = OperationId("save"),
                ["workspaceId"] = workspaceId,
            }, cancellationToken).ConfigureAwait(false);
            if (!WorkbenchAuthoringCodec.TryReadSuccessfulSave(result, out var path))
            {
                Log.ForContext<WorkbenchViewModel>().Warning("Save did not succeed. Result: {Result}", Truncate(result.ToJsonString(IndentedJson)));
                Report("Save failed.", result.ToJsonString(IndentedJson) + Environment.NewLine + "The output file was not updated.");
                AppendObservation("Save failed. The output file was not updated.");
                return;
            }

            RememberPublishedOutput(path, saved: true);
            SetAuthoringDirty(false);
            Log.ForContext<WorkbenchViewModel>().Information("Saved output {OutputPath}.", path);
            Publish(() =>
            {
                Status = "Saved " + path + ".";
                Diagnostics = result.ToJsonString(IndentedJson);
            });
        }
        catch (OperationCanceledException)
        {
            ReportCanceled("Save");
        }
        catch (Exception exception)
        {
            Log.ForContext<WorkbenchViewModel>().Warning(exception, "Save failed for {OutputPath}.", OutputPath);
            Report("Save failed.", FormatError("Save failed", exception) + " The output file was not updated.");
            AppendObservation("Save failed. The output file was not updated.");
        }
    }

    private async Task DiscardCoreAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (!Ready("Discard"))
            {
                return;
            }

            var result = await CallAsync("workspace_discard", new JsonObject
            {
                ["operationId"] = OperationId("discard"),
                ["workspaceId"] = CurrentWorkspaceId(),
            }, cancellationToken).ConfigureAwait(false);
            _baselineFormKey = null;
            _baselineContaining = null;
            _compareRightFormKey = null;
            _compareRightContaining = null;
            _authoringMode = string.Empty;
            SetAuthoringDirty(false);
            var summary = WorkbenchAuthoringCodec.SummarizeDiscard(result);
            Publish(() =>
            {
                Fields.Clear();
                OnPropertyChanged(nameof(AuthoringModeText));
                OnPropertyChanged(nameof(CanApplyRecord));
                OnPropertyChanged(nameof(CanCompareRecord));
            });
            Report("Discarded unsaved edits.", summary);
            AppendObservation(summary);
        }
        catch (OperationCanceledException)
        {
            ReportCanceled("Discard");
        }
        catch (Exception exception)
        {
            Report("Discard failed.", FormatError("Discard failed", exception) + " The output file was not written.");
        }
    }

    private async Task<bool?> ProbeDirtyAsync()
    {
        bool? dirty = null;
        var failed = false;
        await RunSessionOperationAsync(async cancellationToken =>
        {
            try
            {
                var result = await CallAsync(
                    "workspace_state",
                    new JsonObject { ["workspaceId"] = CurrentWorkspaceId() },
                    cancellationToken).ConfigureAwait(false);
                dirty = WorkbenchAuthoringCodec.IsDirty(result);
            }
            catch (OperationCanceledException)
            {
                failed = true;
                ReportCanceled("Reopen");
            }
            catch (Exception exception)
            {
                failed = true;
                ReportFault("Reopen", exception);
            }
        }, "Checking save state").ConfigureAwait(false);
        return failed ? null : dirty;
    }

    private async Task ReadBackAsync(AuthoringMemory memory)
    {
        await RunSessionOperationAsync(async cancellationToken =>
        {
            try
            {
                var path = _publishedPath ?? string.Empty;
                var exists = File.Exists(path);
                var lines = new List<string>
                {
                    "Fresh session output path: " + path + ".",
                    "File exists: " + exists.ToString().ToLowerInvariant() + ".",
                };
                if (!exists)
                {
                    lines.Add("The fresh session did not find the output file.");
                }

                _compareLeftFormKey = memory.CompareLeftFormKey;
                _compareLeftContaining = memory.CompareLeftContaining;
                _compareLeftLabel = memory.CompareLeftLabel;
                _compareRightFormKey = memory.OutputFormKey;
                _compareRightContaining = memory.OutputContaining;
                _baselineFormKey = memory.OutputFormKey;
                _baselineContaining = memory.OutputContaining;
                _unrelatedFormKey = memory.UnrelatedFormKey;
                _unrelatedContaining = memory.UnrelatedContaining;
                _unrelatedEditorId = memory.UnrelatedEditorId;
                _unrelatedFields = memory.UnrelatedFields;

                if (!string.IsNullOrWhiteSpace(memory.OutputFormKey) && !string.IsNullOrWhiteSpace(memory.OutputContaining))
                {
                    await EnsureFamiliesAsync(cancellationToken).ConfigureAwait(false);
                    if (!string.IsNullOrWhiteSpace(memory.FamilyId))
                    {
                        var familyId = memory.FamilyId;
                        Publish(() => SelectedFamily = familyId);
                    }

                    var schema = await SchemaPagesAsync(SelectedFamily, cancellationToken).ConfigureAwait(false);
                    var drafts = schema.SelectMany(page => WorkbenchAuthoringCodec.ReadSchemaFields(page, Languages)).ToArray();
                    var read = await ReadPagesAsync(memory.OutputFormKey, memory.OutputContaining, cancellationToken).ConfigureAwait(false);
                    foreach (var page in read)
                    {
                        WorkbenchAuthoringCodec.ApplyReadPage(drafts, page);
                        var name = WorkbenchAuthoringCodec.DescribeName(page);
                        if (name.Length > 0)
                        {
                            lines.Add(name);
                        }
                    }

                    WorkbenchAuthoringCodec.Snapshot(drafts);
                    _authoringMode = "Override";
                    Publish(() => ReplaceDraft("Override", memory.OutputFormKey, memory.OutputContaining, drafts));
                }

                if (memory.UnrelatedFields is not null
                    && !string.IsNullOrWhiteSpace(memory.UnrelatedFormKey)
                    && !string.IsNullOrWhiteSpace(memory.UnrelatedContaining))
                {
                    var after = await ReadFieldMapAsync(memory.UnrelatedFormKey, memory.UnrelatedContaining, cancellationToken).ConfigureAwait(false);
                    lines.Add(WorkbenchAuthoringCodec.CompareMaps(memory.UnrelatedEditorId ?? string.Empty, memory.UnrelatedFields, after));
                }

                var observation = string.Join(Environment.NewLine, lines);
                Report(exists ? "Reopened " + path + "." : "Reopen did not find the output file.", observation);
                AppendObservation(observation);
            }
            catch (OperationCanceledException)
            {
                ReportCanceled("Reopen");
            }
            catch (Exception exception)
            {
                ReportFault("Reopen", exception);
            }
        }, "Reading reopened output").ConfigureAwait(false);
    }

    private async Task EnsureFamiliesAsync(CancellationToken cancellationToken)
    {
        if (Families.Count > 0 && string.Equals(_familiesRelease, Release, StringComparison.Ordinal))
        {
            return;
        }

        var result = await CallAsync(
            "record_types",
            new JsonObject { ["release"] = Release },
            cancellationToken).ConfigureAwait(false);
        var names = WorkbenchAuthoringCodec.ReadFamilies(result);
        Publish(() =>
        {
            Families.Clear();
            foreach (var name in names)
            {
                Families.Add(name);
            }

            if (!Families.Contains(SelectedFamily))
            {
                SelectedFamily = Families.Contains("Keyword") ? "Keyword" : Families.FirstOrDefault() ?? SelectedFamily;
            }

            _familiesRelease = Release;
        });
    }

    private async Task<IReadOnlyList<JsonObject>> SchemaPagesAsync(string familyId, CancellationToken cancellationToken)
    {
        return await PagesAsync(
            cursor =>
            {
                var arguments = new JsonObject
                {
                    ["release"] = Release,
                    ["familyId"] = familyId,
                };
                if (!string.IsNullOrWhiteSpace(cursor))
                {
                    arguments["cursor"] = cursor;
                }

                return arguments;
            },
            "record_schema",
            SchemaPageLimit,
            cancellationToken).ConfigureAwait(false);
    }

    private async Task<IReadOnlyList<JsonObject>> ReadPagesAsync(string formKey, string containingModKey, CancellationToken cancellationToken)
    {
        return await PagesAsync(
            cursor =>
            {
                var arguments = new JsonObject
                {
                    ["workspaceId"] = CurrentWorkspaceId(),
                    ["familyId"] = SelectedFamily,
                    ["formKey"] = formKey,
                    ["containingModKey"] = containingModKey,
                };
                if (!string.IsNullOrWhiteSpace(cursor))
                {
                    arguments["cursor"] = cursor;
                }

                return arguments;
            },
            "record_read",
            SchemaPageLimit,
            cancellationToken).ConfigureAwait(false);
    }

    private async Task<Dictionary<string, string>> ReadFieldMapAsync(string formKey, string containingModKey, CancellationToken cancellationToken)
    {
        var map = new Dictionary<string, string>(StringComparer.Ordinal);
        var pages = await ReadPagesAsync(formKey, containingModKey, cancellationToken).ConfigureAwait(false);
        foreach (var page in pages)
        {
            WorkbenchAuthoringCodec.AddCanonicalFields(map, page);
        }

        return map;
    }

    private async Task<IReadOnlyList<JsonObject>> PagesAsync(
        Func<string?, JsonObject> arguments,
        string tool,
        int limit,
        CancellationToken cancellationToken)
    {
        var pages = new List<JsonObject>();
        string? cursor = null;
        for (var index = 0; index < limit; index++)
        {
            var page = await CallAsync(tool, arguments(cursor), cancellationToken).ConfigureAwait(false);
            pages.Add(page);
            cursor = WorkbenchAuthoringCodec.NextCursor(page);
            if (string.IsNullOrWhiteSpace(cursor))
            {
                break;
            }
        }

        return pages;
    }

    private JsonObject SearchArguments(string? cursor)
    {
        var arguments = new JsonObject
        {
            ["workspaceId"] = CurrentWorkspaceId(),
            ["familyId"] = SelectedFamily,
        };
        if (!string.IsNullOrWhiteSpace(EditorIdFilter))
        {
            arguments["editorId"] = EditorIdFilter;
        }

        if (!string.IsNullOrWhiteSpace(cursor))
        {
            arguments["cursor"] = cursor;
        }

        return arguments;
    }

    private void ReplaceDraft(string mode, string? baselineFormKey, string? baselineContaining, IReadOnlyList<WorkbenchFieldDraft> drafts)
    {
        _authoringMode = mode;
        if (baselineFormKey is not null)
        {
            _baselineFormKey = baselineFormKey;
            _baselineContaining = baselineContaining;
        }

        Fields.Clear();
        foreach (var draft in drafts)
        {
            Fields.Add(draft);
        }

        OnPropertyChanged(nameof(AuthoringModeText));
        OnPropertyChanged(nameof(CanApplyRecord));
        OnPropertyChanged(nameof(CanCompareRecord));
    }

    private void RememberPublishedOutput(string path, bool saved)
    {
        _publishedPath = path;
        Publish(() =>
        {
            OnPropertyChanged(nameof(PublishedPathText));
            OnPropertyChanged(nameof(CanReopenFresh));
        });
        AppendObservation(saved
            ? "Save published " + path + ". Text storage " + TextStorageMode + ", language " + Language + ". A fresh reopen reads this file back; this line is not that read."
            : "Opened existing output " + path + ". Text storage " + TextStorageMode + ", language " + Language + ".");
    }

    private void ClearAuthoringSelection()
    {
        SearchHits.Clear();
        SelectedHit = null;
        Fields.Clear();
        Families.Clear();
        _familiesRelease = null;
        _authoringMode = string.Empty;
        _baselineFormKey = null;
        _baselineContaining = null;
        _compareLeftFormKey = null;
        _compareLeftContaining = null;
        _compareLeftLabel = null;
        _compareRightFormKey = null;
        _compareRightContaining = null;
        _unrelatedFormKey = null;
        _unrelatedContaining = null;
        _unrelatedEditorId = null;
        _unrelatedFields = null;
        OnPropertyChanged(nameof(AuthoringModeText));
        NotifyAuthoringCommands();
    }

    private void ClearFamiliesForRelease()
    {
        if (string.Equals(_familiesRelease, _release, StringComparison.Ordinal))
        {
            return;
        }

        Families.Clear();
        _familiesRelease = null;
    }

    private void NotifyAuthoringCommands()
    {
        OnPropertyChanged(nameof(CanFindRecords));
        OnPropertyChanged(nameof(CanCreateRecord));
        OnPropertyChanged(nameof(CanOverrideRecord));
        OnPropertyChanged(nameof(CanApplyRecord));
        OnPropertyChanged(nameof(CanCompareRecord));
        OnPropertyChanged(nameof(CanPreviewChanges));
        OnPropertyChanged(nameof(CanSaveChanges));
        OnPropertyChanged(nameof(CanDiscardChanges));
        OnPropertyChanged(nameof(CanReopenFresh));
        OnPropertyChanged(nameof(HasUnsavedEdits));
        OnPropertyChanged(nameof(AuthoringModeText));
        OnPropertyChanged(nameof(PublishedPathText));
    }

    private AuthoringMemory CaptureMemory()
    {
        return new AuthoringMemory
        {
            FamilyId = SelectedFamily,
            CompareLeftFormKey = _compareLeftFormKey,
            CompareLeftContaining = _compareLeftContaining,
            CompareLeftLabel = _compareLeftLabel,
            OutputFormKey = _compareRightFormKey,
            OutputContaining = _compareRightContaining,
            UnrelatedFormKey = _unrelatedFormKey,
            UnrelatedContaining = _unrelatedContaining,
            UnrelatedEditorId = _unrelatedEditorId,
            UnrelatedFields = _unrelatedFields is null ? null : new Dictionary<string, string>(_unrelatedFields, StringComparer.Ordinal),
        };
    }

    private void SetAuthoringDirty(bool value)
    {
        _authoringDirty = value;
        Publish(() =>
        {
            OnPropertyChanged(nameof(HasUnsavedEdits));
            OnPropertyChanged(nameof(CanReopenFresh));
        });
    }

    private void AppendObservation(string text)
    {
        Publish(() =>
        {
            var combined = string.IsNullOrWhiteSpace(Observations)
                ? text
                : Observations + Environment.NewLine + Environment.NewLine + text;
            if (combined.Length > 8000)
            {
                combined = combined[^8000..];
            }

            Observations = combined;
        });
    }

    private bool Ready(string action)
    {
        if (AuthoringSessionReady)
        {
            return true;
        }

        Report(action + " is not available.", "Connect, open a workspace, and create or open an output first. The output file was not saved.");
        return false;
    }

    private bool TryRevision(out ulong revision)
    {
        IWorkbenchMcpSession? client;
        lock (_state)
        {
            client = _client;
        }

        if (client?.Revision is ulong value)
        {
            revision = value;
            return true;
        }

        revision = 0;
        return false;
    }

    private string? CurrentWorkspaceId()
    {
        lock (_state)
        {
            return _workspaceId;
        }
    }

    private void Report(string status, string diagnostics)
    {
        if (ShouldReportHostExit())
        {
            return;
        }

        Publish(() =>
        {
            Status = status;
            Diagnostics = diagnostics;
        });
    }

    private void ReportCanceled(string action)
    {
        if (ShouldReportHostExit())
        {
            return;
        }

        var status = StopText(action + " canceled.", action + " timed out.");
        Report(status, status + " The output file was not saved.");
    }

    private void ReportFault(string action, Exception exception)
    {
        if (ShouldReportHostExit())
        {
            return;
        }

        Report(action + " failed.", FormatError(action + " failed", exception) + " The output file was not saved.");
    }

    private static string OperationId(string verb)
    {
        return "workbench-" + verb + "-" + Guid.NewGuid().ToString("N");
    }

    /// <summary>Remembers the record identities needed to read a saved output in a later process.</summary>
    private sealed class AuthoringMemory
    {
        /// <summary>Gets the family that was edited.</summary>
        public string FamilyId { get; init; } = "Keyword";

        /// <summary>Gets the exact source FormKey used as the left side of compare.</summary>
        public string? CompareLeftFormKey { get; init; }

        /// <summary>Gets the plugin that contains the compare source.</summary>
        public string? CompareLeftContaining { get; init; }

        /// <summary>Gets the label of the compare source.</summary>
        public string? CompareLeftLabel { get; init; }

        /// <summary>Gets the output FormKey to read back.</summary>
        public string? OutputFormKey { get; init; }

        /// <summary>Gets the plugin that contains the output version.</summary>
        public string? OutputContaining { get; init; }

        /// <summary>Gets the unrelated record FormKey.</summary>
        public string? UnrelatedFormKey { get; init; }

        /// <summary>Gets the plugin that contains the unrelated record.</summary>
        public string? UnrelatedContaining { get; init; }

        /// <summary>Gets the unrelated record's EditorID.</summary>
        public string? UnrelatedEditorId { get; init; }

        /// <summary>Gets the unrelated record's field snapshot.</summary>
        public Dictionary<string, string>? UnrelatedFields { get; init; }
    }
}
