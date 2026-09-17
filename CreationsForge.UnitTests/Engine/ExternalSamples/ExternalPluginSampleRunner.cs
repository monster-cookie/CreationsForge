using System.Runtime.ExceptionServices;
using System.Security.Cryptography;
using System.Text.Json;
using CreationsForge.Bootstrap.Composition;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Enums;
using Mutagen.Bethesda.Plugins;

namespace CreationsForge.UnitTests.Engine.ExternalSamples;

/// <summary>Executes retained, serial external-source authoring cases through the production engine composition.</summary>
internal static class ExternalPluginSampleRunner
{
    /// <summary>The maximum post-failure interval spent recapturing source evidence.</summary>
    private static readonly TimeSpan PostFailureEvidenceTimeout = TimeSpan.FromMinutes(2);

    /// <summary>The maximum independent interval spent writing the final retained report.</summary>
    private static readonly TimeSpan FinalReportWriteTimeout = TimeSpan.FromSeconds(30);

    /// <summary>The maximum number of deterministically inspected candidates when no FormKeys are supplied.</summary>
    private const int MaximumDiscoveryCandidates = 256;

    /// <summary>The maximum item count admitted for a selected external FormList.</summary>
    private const int MaximumSelectedItems = 512;

    /// <summary>Runs every explicitly requested output representation without overlapping plugin source lifetimes.</summary>
    /// <param name="manifest">The validated process-local manifest.</param>
    /// <returns>A task that completes after retained reporting and source-preservation verification.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="manifest"/> is null.</exception>
    /// <exception cref="OperationCanceledException">Thrown after bounded failure evidence and report handling when the coordinated test is cancelled.</exception>
    /// <exception cref="InvalidDataException">Thrown when external content or engine behavior violates the harness contract.</exception>
    internal static async Task RunAsync(ExternalPluginSampleManifest manifest)
    {
        ArgumentNullException.ThrowIfNull(manifest);
        var cancellationToken = TestContext.Current.CancellationToken;
        var preflightBefore = ExternalPluginSamplePreflight.Inspect(manifest);
        var runDirectory = ExternalPluginSamplePreflight.CreateRunDirectory(manifest, "execution");
        var report = new ExternalPluginSampleExecutionReport
        {
            SchemaVersion = 1,
            StartedAtUtc = DateTimeOffset.UtcNow,
            RunDirectory = runDirectory,
            PreflightBefore = preflightBefore,
            Status = "Running",
        };
        Exception? failure = null;
        try
        {
            await ExternalPluginSamplePreflight.WriteReportAsync(
                runDirectory,
                "preflight-report.json",
                preflightBefore,
                cancellationToken);
            report.SourcePluginHashesBefore = await CapturePluginHashesAsync(manifest, cancellationToken);
            foreach (var mode in manifest.LocalizedOutputModes)
            {
                report.Cases.Add(await RunCaseAsync(
                    manifest,
                    preflightBefore,
                    runDirectory,
                    mode,
                    report.AcceptanceGaps,
                    cancellationToken));
            }

            report.PreflightAfter = ExternalPluginSamplePreflight.Inspect(manifest);
            report.SourcePluginHashesAfter = await CapturePluginHashesAsync(manifest, cancellationToken);
            VerifySourceEvidence(report);
            report.Status = "Passed";
        }
        catch (Exception exception)
        {
            failure = exception;
            report.Status = "Failed";
            report.Failure = exception.ToString();
            try
            {
                using var evidenceCancellation = new CancellationTokenSource(PostFailureEvidenceTimeout);
                report.PreflightAfter ??= ExternalPluginSamplePreflight.Inspect(manifest);
                report.SourcePluginHashesAfter ??= await CapturePluginHashesAsync(manifest, evidenceCancellation.Token);
            }
            catch (Exception evidenceException)
            {
                report.Failure += $"{Environment.NewLine}Post-failure evidence capture also failed: {evidenceException}";
            }
        }
        finally
        {
            report.CompletedAtUtc = DateTimeOffset.UtcNow;
            try
            {
                using var reportCancellation = new CancellationTokenSource(FinalReportWriteTimeout);
                await ExternalPluginSamplePreflight.WriteReportAsync(
                    runDirectory,
                    "execution-report.json",
                    report,
                    reportCancellation.Token);
            }
            catch (Exception reportException)
            {
                if (failure is null)
                {
                    failure = reportException;
                }
                else
                {
                    report.Failure += $"{Environment.NewLine}Final report write also failed: {reportException}";
                }
            }
        }

        if (failure is not null)
        {
            ExceptionDispatchInfo.Capture(failure).Throw();
        }
    }

    /// <summary>Runs one embedded or localized output case through initial save, fresh engine reopen, and direct plugin verification.</summary>
    /// <param name="manifest">The validated external input manifest.</param>
    /// <param name="preflight">The header-only metadata captured before parsing.</param>
    /// <param name="runDirectory">The fresh retained execution directory.</param>
    /// <param name="mode">The output representation under test.</param>
    /// <param name="acceptanceGaps">The report-owned acceptance-gap collector.</param>
    /// <param name="cancellationToken">A token observed throughout bounded engine operations.</param>
    /// <returns>Complete compact evidence for this output case.</returns>
    private static async Task<ExternalPluginSampleCaseReport> RunCaseAsync(
        ExternalPluginSampleManifest manifest,
        ExternalPluginSamplePreflightReport preflight,
        string runDirectory,
        LocalizedOutputMode mode,
        ICollection<string> acceptanceGaps,
        CancellationToken cancellationToken)
    {
        var caseDirectory = Path.Combine(runDirectory, mode.ToString());
        Directory.CreateDirectory(caseDirectory);
        var outputPath = Path.Combine(caseDirectory, manifest.OutputModKey.FileName.String);
        var association = new OutputAssociation(
            outputPath,
            manifest.OutputModKey,
            mode,
            OutputMasterStyle.Full);
        var caseReport = new ExternalPluginSampleCaseReport
        {
            OutputMode = mode.ToString(),
            OutputPluginPath = outputPath,
        };
        IReadOnlyDictionary<FormKey, string> expectedRecords;
        IReadOnlyDictionary<FormKey, ExternalIntendedRecordEdit> intendedEdits;

        await using (var services = EngineComposition.Create())
        {
            var open = await services.WorkspaceFactory.OpenAsync(
                manifest.CreateWorkspaceOpenRequest(Guid.NewGuid()),
                cancellationToken);
            var workspace = RequireSuccess(open, "opening external plugin sources");
            await using (workspace)
            {
                caseReport.InitialSourceBaselineId = workspace.Revision.BaselineId;
                var selected = await SelectRecordsAsync(workspace, manifest, mode, acceptanceGaps, cancellationToken);
                caseReport.SelectedRecords.AddRange(selected.Select(CreateSelectedRecordReport));
                var selection = RequireSuccess(
                    await workspace.SelectOutputAsync(
                        new SelectOutputRequest(Guid.NewGuid(), workspace.Revision, OutputSelectionMode.CreateNew, association),
                        cancellationToken),
                    $"selecting new {mode} output");

                var sentinel = await BeginEditAsync(workspace, FormListEditRole.New, cancellationToken);
                await ApplyEditAsync(workspace, sentinel.EditId, new SetEditorIdEdit("CFExternalSentinel"), cancellationToken);
                string sentinelExpected;
                OutputArtifactSetBaseline saveBaseline;
                if (mode == LocalizedOutputMode.Embedded)
                {
                    var sentinelPreview = RequireSuccess(
                        await workspace.PreviewAsync(cancellationToken),
                        "previewing the new embedded sentinel");
                    RequireComparisonSet(sentinelPreview, [sentinel.FormKey]);
                    var firstSave = await SaveCommittedAsync(workspace, selection.Baseline, cancellationToken);
                    caseReport.FirstCommittedBaselineId = firstSave.CommittedBaseline!.BaselineId;
                    sentinelExpected = await ReadStagedRecordAsync(workspace, association, sentinel.FormKey, cancellationToken);
                    saveBaseline = firstSave.CommittedBaseline;
                }
                else
                {
                    sentinelExpected = string.Empty;
                    saveBaseline = selection.Baseline;
                }

                var selectedForCase = selected;
                if (mode == LocalizedOutputMode.Embedded)
                {
                    var ineligible = selected.Where(record => !IsEmbeddedTranslationEligible(record.Json)).ToArray();
                    selectedForCase = selected.Where(record => IsEmbeddedTranslationEligible(record.Json)).ToArray();
                    foreach (var excluded in ineligible)
                    {
                        acceptanceGaps.Add($"Embedded case excluded multilingual FormList '{excluded.FormKey}' because its complete inspector view contains more than one translation in at least one translated field.");
                    }

                    if (selectedForCase.Count == 0)
                    {
                        throw new InvalidDataException("The Embedded case requires at least one selected external FormList satisfying the single-translation embedded-output policy.");
                    }
                }

                var intendedRecords = new List<ExternalIntendedRecordEdit>(selectedForCase.Count);
                foreach (var sourceRecord in selectedForCase)
                {
                    var intendedItems = RotateItems(sourceRecord.Items);
                    var intended = new ExternalIntendedRecordEdit(
                        sourceRecord,
                        CreateEditedEditorId(sourceRecord, intendedRecords.Count),
                        intendedItems,
                        replaceItems: !intendedItems.SequenceEqual(sourceRecord.Items));
                    ExternalPluginSampleAssertions.RequireMaterialIntention(intended);
                    intendedRecords.Add(intended);
                    var edit = await BeginEditAsync(
                        workspace,
                        FormListEditRole.Override,
                        cancellationToken,
                        sourceRecord.FormKey);
                    await ApplyEditAsync(
                        workspace,
                        edit.EditId,
                        new SetEditorIdEdit(intended.EditorId),
                        cancellationToken);
                    if (intended.ReplaceItems)
                    {
                        await ApplyEditAsync(
                            workspace,
                            edit.EditId,
                            new ReplaceItemsEdit(intended.Items),
                            cancellationToken);
                    }
                }

                intendedEdits = intendedRecords.ToDictionary(record => record.Source.FormKey);

                var preview = RequireSuccess(await workspace.PreviewAsync(cancellationToken), $"previewing {mode} external overrides");
                var expectedComparisonKeys = intendedRecords.Select(record => record.Source.FormKey).ToList();
                if (mode == LocalizedOutputMode.SeparateStringFiles)
                {
                    expectedComparisonKeys.Insert(0, sentinel.FormKey);
                }

                RequireComparisonSet(preview, expectedComparisonKeys);
                VerifyOverrideComparisons(preview, intendedRecords);
                var expectedAfterPreview = preview.Comparisons
                    .Where(comparison => intendedEdits.ContainsKey(comparison.FormKey))
                    .ToDictionary(
                        comparison => comparison.FormKey,
                        comparison => comparison.After!.Value.GetRawText());

                var committed = await SaveCommittedAsync(workspace, saveBaseline, cancellationToken);
                caseReport.FinalCommittedBaselineId = committed.CommittedBaseline!.BaselineId;
                caseReport.PresentOutputArtifacts = committed.CommittedBaseline.Artifacts
                    .Where(artifact => artifact.Fingerprint.Exists)
                    .Select(artifact => artifact.Path)
                    .ToArray();
                if (mode == LocalizedOutputMode.SeparateStringFiles)
                {
                    caseReport.FirstCommittedBaselineId = committed.CommittedBaseline.BaselineId;
                    sentinelExpected = await ReadStagedRecordAsync(workspace, association, sentinel.FormKey, cancellationToken);
                    if (!committed.CommittedBaseline.Artifacts.Any(
                        artifact => artifact.Role != PluginArtifactRole.Plugin && artifact.Fingerprint.Exists))
                    {
                        throw new InvalidDataException("The localized first save did not retain any present record string sidecar.");
                    }
                }

                var finalRecords = new Dictionary<FormKey, string>
                {
                    [sentinel.FormKey] = sentinelExpected,
                };
                foreach (var expected in expectedAfterPreview)
                {
                    var saved = await ReadStagedRecordAsync(workspace, association, expected.Key, cancellationToken);
                    RequireJsonEqual(expected.Value, saved, $"saved external FormList '{expected.Key}'");
                    ExternalPluginSampleAssertions.RequireInspectorValues(
                        saved,
                        intendedEdits[expected.Key],
                        "Saved engine output");
                    finalRecords.Add(expected.Key, saved);
                }

                if (mode == LocalizedOutputMode.Embedded)
                {
                    var sentinelAfterOverride = await ReadStagedRecordAsync(workspace, association, sentinel.FormKey, cancellationToken);
                    RequireJsonEqual(sentinelExpected, sentinelAfterOverride, "embedded sentinel after external override save");
                }
                else
                {
                    var materialEdit = await BeginEditAsync(
                        workspace,
                        FormListEditRole.ExistingOutput,
                        cancellationToken,
                        targetFormKey: sentinel.FormKey);
                    await ApplyEditAsync(
                        workspace,
                        materialEdit.EditId,
                        new SetEditorIdEdit("CFExternalSentinelMaterialRewrite"),
                        cancellationToken);
                    var rejected = await workspace.SaveAsync(
                        new SaveRequest(Guid.NewGuid(), workspace.Revision, committed.CommittedBaseline),
                        cancellationToken);
                    if (rejected.Status != SaveCommitStatus.NotCommitted
                        || rejected.Error?.Code != EngineErrorCode.UnsupportedInput)
                    {
                        throw new InvalidDataException(
                            $"Existing localized output material rewrite did not fail closed as UnsupportedInput: {DescribeSave(rejected)}");
                    }

                    caseReport.MaterialRewriteFailClosed = true;
                    caseReport.MaterialRewriteFailure = rejected.Error.Message;
                    acceptanceGaps.Add($"{manifest.SupportedGame} existing SeparateStringFiles output remains intentionally fail-closed for material rewrites; localized external coverage is therefore one combined first save followed by a verified rejection case.");
                }

                expectedRecords = finalRecords;
            }
        }

        caseReport.LocalizedNameEvidence = ExternalPluginSampleLocalizationVerifier.Verify(
            manifest, preflight, intendedEdits.Values.Select(edit => edit.Source).ToArray(), acceptanceGaps, cancellationToken);
        await using (var freshServices = EngineComposition.Create())
        {
            var reopen = await freshServices.WorkspaceFactory.OpenAsync(
                manifest.CreateWorkspaceOpenRequest(Guid.NewGuid()),
                cancellationToken);
            var freshWorkspace = RequireSuccess(reopen, "opening a fresh engine source lifetime");
            await using (freshWorkspace)
            {
                caseReport.FreshSourceBaselineId = freshWorkspace.Revision.BaselineId;
                if (caseReport.FreshSourceBaselineId != caseReport.InitialSourceBaselineId)
                {
                    throw new InvalidDataException("Fresh engine open produced a different source baseline identity after external validation.");
                }

                RequireSuccess(
                    await freshWorkspace.SelectOutputAsync(
                        new SelectOutputRequest(Guid.NewGuid(), freshWorkspace.Revision, OutputSelectionMode.OpenExisting, association),
                        cancellationToken),
                    $"freshly reopening committed {mode} output");
                foreach (var expected in expectedRecords)
                {
                    var actual = await ReadStagedRecordAsync(freshWorkspace, association, expected.Key, cancellationToken);
                    RequireJsonEqual(expected.Value, actual, $"fresh engine record '{expected.Key}'");
                    if (intendedEdits.TryGetValue(expected.Key, out var intended))
                    {
                        ExternalPluginSampleAssertions.RequireInspectorValues(
                            actual,
                            intended,
                            "Fresh engine reopen");
                    }
                }
            }
        }

        var expectedMasters = CollectExpectedMasters(expectedRecords.Values, manifest);
        caseReport.ExpectedMasters = expectedMasters.Select(master => master.ToString()).ToArray();
        caseReport.DirectVerification = ExternalPluginSampleVerifier.Verify(
            manifest,
            preflight,
            association,
            expectedRecords,
            intendedEdits,
            expectedMasters,
            cancellationToken);
        return caseReport;
    }

    /// <summary>Selects explicit FormKeys or one deterministic rich record with bounded resolvable item checks.</summary>
    /// <param name="workspace">The opened production workspace.</param>
    /// <param name="manifest">The validated selection manifest.</param>
    /// <param name="mode">The output representation, used to prefer embedded-compatible automatic candidates.</param>
    /// <param name="acceptanceGaps">The report-owned acceptance-gap collector.</param>
    /// <param name="cancellationToken">A token observed throughout bounded candidate reads.</param>
    /// <returns>Selected complete detached record snapshots.</returns>
    private static async Task<IReadOnlyList<ExternalSelectedRecord>> SelectRecordsAsync(
        IPluginWorkspace workspace,
        ExternalPluginSampleManifest manifest,
        LocalizedOutputMode mode,
        ICollection<string> acceptanceGaps,
        CancellationToken cancellationToken)
    {
        if (manifest.FormKeys.Count > 0)
        {
            var explicitRecords = new List<ExternalSelectedRecord>(manifest.FormKeys.Count);
            foreach (var formKey in manifest.FormKeys)
            {
                explicitRecords.Add(await ReadSelectionAsync(workspace, formKey, cancellationToken));
            }

            await RequireResolvableItemsAsync(workspace, explicitRecords, cancellationToken);
            return explicitRecords;
        }

        var listed = RequireSuccess(
            await workspace.ListFormListsAsync(RecordScope.WinningOverrides, cancellationToken),
            "listing external FormLists for deterministic selection");
        var candidates = new List<ExternalSelectedRecord>();
        foreach (var summary in listed
            .OrderBy(summary => summary.FormKey.ToString(), StringComparer.Ordinal)
            .Take(MaximumDiscoveryCandidates))
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                var record = await ReadSelectionAsync(workspace, summary.FormKey, cancellationToken);
                if (record.Items.Count == 0 || record.Items.Count > MaximumSelectedItems)
                {
                    continue;
                }

                if (mode == LocalizedOutputMode.Embedded && !IsEmbeddedTranslationEligible(record.Json))
                {
                    continue;
                }

                if (await AllRealItemsResolveAsync(workspace, record.Items, cancellationToken))
                {
                    candidates.Add(record);
                }
            }
            catch (InvalidDataException)
            {
                continue;
            }
        }

        var selected = candidates
            .OrderByDescending(record => ScoreRichFields(record.Json))
            .ThenBy(record => record.FormKey.ToString(), StringComparer.Ordinal)
            .FirstOrDefault()
            ?? throw new InvalidDataException(
                $"No deterministic nondeleted FormList with bounded resolvable items was found among the first {MaximumDiscoveryCandidates} ordered candidates. Supply selectedFormKeys explicitly.");
        acceptanceGaps.Add($"No selectedFormKeys were supplied; the harness deterministically chose '{selected.FormKey}' after inspecting at most {MaximumDiscoveryCandidates} ordered candidates.");
        return [selected];
    }

    /// <summary>Reads one winning external FormList into a complete detached JSON snapshot.</summary>
    /// <param name="workspace">The opened workspace.</param>
    /// <param name="formKey">The exact requested FormKey.</param>
    /// <param name="cancellationToken">A token observed by the plugin inspector.</param>
    /// <returns>The resolved nondeleted external record snapshot.</returns>
    private static async Task<ExternalSelectedRecord> ReadSelectionAsync(
        IPluginWorkspace workspace,
        FormKey formKey,
        CancellationToken cancellationToken)
    {
        var result = RequireSuccess(
            await workspace.ReadFormListViewAsync(
                new ReferenceRequest(formKey, RecordScope.WinningOverrides),
                cancellationToken),
            $"reading selected external FormList '{formKey}'");
        if (result.Context.Status != ReferenceResolutionStatus.Resolved || !result.Record.HasValue)
        {
            throw new InvalidDataException($"Selected external FormList '{formKey}' is not a resolved nondeleted winning context.");
        }

        var items = ParseItems(result.Record.Value);
        if (items.Count > MaximumSelectedItems)
        {
            throw new InvalidDataException($"Selected external FormList '{formKey}' contains {items.Count} items, exceeding the {MaximumSelectedItems}-item harness bound.");
        }

        return new ExternalSelectedRecord
        {
            FormKey = formKey,
            ContainingModKey = result.Context.ContainingModKey?.ToString() ?? string.Empty,
            SourcePath = result.Context.Path ?? string.Empty,
            Json = result.Record.Value.GetRawText(),
            Items = items,
        };
    }

    /// <summary>Requires at least one real resolvable item and no unresolved real items across explicit selections.</summary>
    /// <param name="workspace">The opened workspace.</param>
    /// <param name="records">Explicitly selected records.</param>
    /// <param name="cancellationToken">A token observed during reference resolution.</param>
    /// <returns>A task that completes after the selection contract is proven.</returns>
    private static async Task RequireResolvableItemsAsync(
        IPluginWorkspace workspace,
        IReadOnlyList<ExternalSelectedRecord> records,
        CancellationToken cancellationToken)
    {
        if (!records.Any(record => record.Items.Any(item => !item.IsNull)))
        {
            throw new InvalidDataException("Explicit selectedFormKeys must include at least one FormList with a real item target.");
        }

        foreach (var record in records)
        {
            if (!await AllRealItemsResolveAsync(workspace, record.Items, cancellationToken))
            {
                throw new InvalidDataException($"Selected external FormList '{record.FormKey}' contains a real item target that does not resolve in the explicit load order.");
            }
        }
    }

    /// <summary>Determines whether every non-null item resolves through the exact admitted winning load order.</summary>
    /// <param name="workspace">The opened workspace.</param>
    /// <param name="items">Ordered FormList items.</param>
    /// <param name="cancellationToken">A token observed between resolutions.</param>
    /// <returns><see langword="true"/> when at least one real item exists and every real item resolves.</returns>
    private static async Task<bool> AllRealItemsResolveAsync(
        IPluginWorkspace workspace,
        IReadOnlyList<FormKey> items,
        CancellationToken cancellationToken)
    {
        var realItems = items.Where(item => !item.IsNull).Distinct().ToArray();
        if (realItems.Length == 0)
        {
            return false;
        }

        foreach (var item in realItems)
        {
            var resolution = await workspace.ResolveReferenceAsync(
                new ReferenceRequest(item, RecordScope.WinningOverrides),
                cancellationToken);
            if (!resolution.Succeeded || resolution.Value!.Status != ReferenceResolutionStatus.Resolved)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>Begins one record edit session through the public workspace contract.</summary>
    /// <param name="workspace">The selected workspace.</param>
    /// <param name="role">The new, override, or existing-output role.</param>
    /// <param name="cancellationToken">A token observed before publication.</param>
    /// <param name="originFormKey">The source identity for an override.</param>
    /// <param name="targetFormKey">The output identity for an existing-output edit.</param>
    /// <returns>The successful edit receipt.</returns>
    private static async Task<EditReceipt> BeginEditAsync(
        IPluginWorkspace workspace,
        FormListEditRole role,
        CancellationToken cancellationToken,
        FormKey? originFormKey = null,
        FormKey? targetFormKey = null)
    {
        return RequireSuccess(
            await workspace.BeginEditAsync(
                new BeginEditRequest(
                    Guid.NewGuid(),
                    workspace.Revision,
                    role,
                    originFormKey,
                    originFormKey.HasValue ? new ReferenceRequest(originFormKey.Value, RecordScope.WinningOverrides) : null,
                    targetFormKey),
                cancellationToken),
            $"beginning {role} FormList edit");
    }

    /// <summary>Applies one typed edit through the public workspace contract.</summary>
    /// <param name="workspace">The workspace that owns the staged edit.</param>
    /// <param name="editId">The stable edit identity.</param>
    /// <param name="edit">The typed domain mutation.</param>
    /// <param name="cancellationToken">A token observed before publication.</param>
    /// <returns>A task that completes when the mutation is published.</returns>
    private static async Task ApplyEditAsync(
        IPluginWorkspace workspace,
        Guid editId,
        FormListEdit edit,
        CancellationToken cancellationToken)
    {
        RequireSuccess(
            await workspace.ApplyFormListEditAsync(
                new FormListEditRequest(Guid.NewGuid(), workspace.Revision, editId, edit),
                cancellationToken),
            $"applying '{edit.CommandName}'");
    }

    /// <summary>Runs a guarded save and requires a known committed outcome.</summary>
    /// <param name="workspace">The dirty selected workspace.</param>
    /// <param name="baseline">The exact current output baseline.</param>
    /// <param name="cancellationToken">A token honored by the guarded save contract.</param>
    /// <returns>The committed save result.</returns>
    private static async Task<SaveResult> SaveCommittedAsync(
        IPluginWorkspace workspace,
        OutputArtifactSetBaseline baseline,
        CancellationToken cancellationToken)
    {
        var result = await workspace.SaveAsync(
            new SaveRequest(Guid.NewGuid(), workspace.Revision, baseline),
            cancellationToken);
        if (result.Status != SaveCommitStatus.Committed || result.CommittedBaseline is null)
        {
            throw new InvalidDataException($"Guarded plugin save did not commit: {DescribeSave(result)}");
        }

        return result;
    }

    /// <summary>Reads one exact staged-output FormList as complete inspector JSON.</summary>
    /// <param name="workspace">The selected output workspace.</param>
    /// <param name="association">The exact output identity.</param>
    /// <param name="formKey">The exact output record identity.</param>
    /// <param name="cancellationToken">A token observed during plugin traversal.</param>
    /// <returns>The complete detached inspector JSON text.</returns>
    private static async Task<string> ReadStagedRecordAsync(
        IPluginWorkspace workspace,
        OutputAssociation association,
        FormKey formKey,
        CancellationToken cancellationToken)
    {
        var result = RequireSuccess(
            await workspace.ReadFormListViewAsync(
                new ReferenceRequest(formKey, RecordScope.StagedOutput, association.ModKey),
                cancellationToken),
            $"reading output FormList '{formKey}'");
        if (result.Context.Status != ReferenceResolutionStatus.Resolved || !result.Record.HasValue)
        {
            throw new InvalidDataException($"Output FormList '{formKey}' was not a resolved record.");
        }

        return result.Record.Value.GetRawText();
    }

    /// <summary>Verifies preview contains exactly the requested dirty FormList identities.</summary>
    /// <param name="preview">The complete workspace preview.</param>
    /// <param name="expected">Expected dirty identities.</param>
    /// <exception cref="InvalidDataException">Thrown when a dirty record is missing or unexpected.</exception>
    private static void RequireComparisonSet(WorkspacePreview preview, IReadOnlyList<FormKey> expected)
    {
        var actualKeys = preview.Comparisons.Select(comparison => comparison.FormKey).OrderBy(key => key.ToString(), StringComparer.Ordinal);
        var expectedKeys = expected.OrderBy(key => key.ToString(), StringComparer.Ordinal);
        if (!actualKeys.SequenceEqual(expectedKeys))
        {
            throw new InvalidDataException("Workspace preview did not contain exactly the intended dirty FormLists.");
        }
    }

    /// <summary>Verifies each external override changed only EditorID and optional ordered Items.</summary>
    /// <param name="preview">The complete dirty workspace preview.</param>
    /// <param name="intendedRecords">The independently derived intended external edits.</param>
    /// <exception cref="InvalidDataException">Thrown when prior content changes or an unintended field is modified.</exception>
    private static void VerifyOverrideComparisons(
        WorkspacePreview preview,
        IReadOnlyList<ExternalIntendedRecordEdit> intendedRecords)
    {
        foreach (var intended in intendedRecords)
        {
            var source = intended.Source;
            var comparison = preview.Comparisons.Single(candidate => candidate.FormKey == source.FormKey);
            if (!comparison.Before.HasValue || !comparison.After.HasValue)
            {
                throw new InvalidDataException($"External override comparison for '{source.FormKey}' lacked complete before/after plugin state.");
            }

            RequireJsonEqual(source.Json, comparison.Before.Value.GetRawText(), $"source comparison baseline '{source.FormKey}'");
            ExternalPluginSampleAssertions.RequireInspectorValues(
                comparison.After.Value.GetRawText(),
                intended,
                "Workspace preview");
            var editorIdChanged = comparison.Changes.Any(change => change.FieldIdentifier == "EditorID");
            var itemsChanged = comparison.Changes.Any(change => change.FieldIdentifier.StartsWith("Items", StringComparison.Ordinal));
            if (!editorIdChanged
                || itemsChanged != intended.ReplaceItems
                || comparison.Changes.Any(change => change.FieldIdentifier != "EditorID" && !change.FieldIdentifier.StartsWith("Items", StringComparison.Ordinal)))
            {
                throw new InvalidDataException($"External override '{source.FormKey}' did not produce the exact intended EditorID and ordered Items change set.");
            }
        }
    }

    /// <summary>Parses ordered FormList item identities from inspector JSON.</summary>
    /// <param name="record">The complete inspector record.</param>
    /// <returns>Ordered plugin FormKeys including duplicates and null sentinels.</returns>
    private static IReadOnlyList<FormKey> ParseItems(JsonElement record)
    {
        return Array.AsReadOnly(record.GetProperty("Items")
            .EnumerateArray()
            .Select(item => FormKey.Factory(item.GetProperty("formKey").GetString()!))
            .ToArray());
    }

    /// <summary>Rotates a bounded item list by one position when that produces a material ordered change.</summary>
    /// <param name="items">The exact original item sequence.</param>
    /// <returns>A caller-owned intended item sequence.</returns>
    private static IReadOnlyList<FormKey> RotateItems(IReadOnlyList<FormKey> items)
    {
        if (items.Count < 2)
        {
            return items.ToArray();
        }

        var rotated = items.Skip(1).Append(items[0]).ToArray();
        return rotated.SequenceEqual(items) ? items.ToArray() : rotated;
    }

    /// <summary>Creates a deterministic non-empty edited EditorID within conservative plugin length bounds.</summary>
    /// <param name="record">The selected source record.</param>
    /// <param name="index">The zero-based selected-record index.</param>
    /// <returns>The intended edited EditorID.</returns>
    private static string CreateEditedEditorId(ExternalSelectedRecord record, int index)
    {
        var suffix = Convert.ToHexString(SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(record.FormKey.ToString())))[..8];
        var editorId = $"CFExternal{index}_{suffix}";
        using var document = JsonDocument.Parse(record.Json);
        return document.RootElement.GetProperty("EditorID").GetString() == editorId
            ? editorId + "X"
            : editorId;
    }

    /// <summary>Determines whether every translated field contains at most one language value for embedded serialization.</summary>
    /// <param name="json">The complete inspector record JSON.</param>
    /// <returns><see langword="true"/> when no translated-string node is multilingual.</returns>
    private static bool IsEmbeddedTranslationEligible(string json)
    {
        using var document = JsonDocument.Parse(json);
        return IsEmbeddedTranslationEligible(document.RootElement);
    }

    /// <summary>Recursively evaluates translated-string arrays without imposing a game-specific field shape.</summary>
    /// <param name="value">The current complete inspector node.</param>
    /// <returns><see langword="true"/> when this subtree contains no multilingual translated value.</returns>
    private static bool IsEmbeddedTranslationEligible(JsonElement value)
    {
        if (value.ValueKind == JsonValueKind.Object)
        {
            foreach (var property in value.EnumerateObject())
            {
                if (property.NameEquals("translations")
                    && property.Value.ValueKind == JsonValueKind.Array
                    && property.Value.GetArrayLength() > 1)
                {
                    return false;
                }

                if (!IsEmbeddedTranslationEligible(property.Value))
                {
                    return false;
                }
            }
        }
        else if (value.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in value.EnumerateArray())
            {
                if (!IsEmbeddedTranslationEligible(item))
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>Scores available rich field categories for deterministic selection.</summary>
    /// <param name="json">The complete inspector record JSON.</param>
    /// <returns>A stable richness score used only after resolvability checks.</returns>
    private static int ScoreRichFields(string json)
    {
        using var document = JsonDocument.Parse(json);
        var record = document.RootElement;
        var score = record.GetProperty("Items").GetArrayLength() > 0 ? 1 : 0;
        score += record.TryGetProperty("EditorID", out var editorId) && editorId.ValueKind == JsonValueKind.String ? 1 : 0;
        score += record.TryGetProperty("Name", out var name)
            && name.ValueKind == JsonValueKind.Object
            && name.GetProperty("translations").GetArrayLength() > 0 ? 32 : 0;
        score += record.TryGetProperty("Components", out var components) && components.GetArrayLength() > 0 ? 8 : 0;
        score += record.TryGetProperty("ConditionalEntries", out var conditions) && conditions.GetArrayLength() > 0 ? 8 : 0;
        score += record.TryGetProperty("AddToList", out var addToList)
            && addToList.ValueKind == JsonValueKind.Object
            && !addToList.GetProperty("isNull").GetBoolean() ? 4 : 0;
        return score;
    }

    /// <summary>Builds a report entry for one exact selected external record and its present field categories.</summary>
    /// <param name="record">The selected complete record snapshot.</param>
    /// <returns>A compact selection report.</returns>
    private static ExternalSelectedRecordReport CreateSelectedRecordReport(ExternalSelectedRecord record)
    {
        using var document = JsonDocument.Parse(record.Json);
        var root = document.RootElement;
        var categories = new List<string>
        {
            root.GetProperty("EditorID").ValueKind == JsonValueKind.Null ? "EditorID:absent" : "EditorID:present",
            $"Items:present:{record.Items.Count}",
        };
        foreach (var property in new[] { "Name", "Components", "ConditionalEntries", "AddToList" })
        {
            if (!root.TryGetProperty(property, out var value))
            {
                categories.Add($"{property}:not-applicable");
            }
            else
            {
                var present = value.ValueKind switch
                {
                    JsonValueKind.Null => false,
                    JsonValueKind.Array => value.GetArrayLength() > 0,
                    JsonValueKind.Object when value.TryGetProperty("isNull", out var isNull) => !isNull.GetBoolean(),
                    _ => true,
                };
                categories.Add($"{property}:{(present ? "present" : "absent")}");
            }
        }

        return new ExternalSelectedRecordReport
        {
            FormKey = record.FormKey.ToString(),
            ContainingModKey = record.ContainingModKey,
            SourcePath = record.SourcePath,
            FieldCategories = categories.ToArray(),
            InspectorJsonSha256 = Convert.ToHexString(SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(record.Json))),
        };
    }

    /// <summary>Collects all non-output FormKey origins from complete expected records and orders them by explicit load order.</summary>
    /// <param name="recordJson">Complete expected output records.</param>
    /// <param name="manifest">The explicit source order and output identity.</param>
    /// <returns>The exact expected output master order.</returns>
    private static IReadOnlyList<ModKey> CollectExpectedMasters(
        IEnumerable<string> recordJson,
        ExternalPluginSampleManifest manifest)
    {
        var referenced = new HashSet<ModKey>();
        foreach (var json in recordJson)
        {
            using var document = JsonDocument.Parse(json);
            CollectFormKeys(document.RootElement, referenced, manifest.OutputModKey);
        }

        var orderedSources = manifest.CanonicalLoadOrderPluginPaths
            .Select(path => ModKey.FromNameAndExtension(Path.GetFileName(path)))
            .Where(referenced.Contains)
            .ToArray();
        if (orderedSources.Length != referenced.Count)
        {
            throw new InvalidDataException("Expected output records reference a ModKey outside the explicit load order.");
        }

        return orderedSources;
    }

    /// <summary>Recursively collects plugin FormKey JSON leaves from one inspector subtree.</summary>
    /// <param name="value">The current inspector node.</param>
    /// <param name="modKeys">The caller-owned referenced-ModKey set.</param>
    /// <param name="outputModKey">The output identity excluded from its own masters.</param>
    private static void CollectFormKeys(JsonElement value, ISet<ModKey> modKeys, ModKey outputModKey)
    {
        if (value.ValueKind == JsonValueKind.Object)
        {
            foreach (var property in value.EnumerateObject())
            {
                if ((property.NameEquals("FormKey") || property.NameEquals("formKey"))
                    && property.Value.ValueKind == JsonValueKind.String)
                {
                    var formKey = FormKey.Factory(property.Value.GetString()!);
                    if (!formKey.IsNull && formKey.ModKey != outputModKey)
                    {
                        modKeys.Add(formKey.ModKey);
                    }
                }
                else
                {
                    CollectFormKeys(property.Value, modKeys, outputModKey);
                }
            }
        }
        else if (value.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in value.EnumerateArray())
            {
                CollectFormKeys(item, modKeys, outputModKey);
            }
        }
    }

    /// <summary>Captures streaming SHA-256 identities for explicit plugin inputs without buffering file contents.</summary>
    /// <param name="manifest">The explicit source manifest.</param>
    /// <param name="cancellationToken">A token observed by streaming hashing.</param>
    /// <returns>Ordered source plugin hash evidence.</returns>
    private static async Task<List<ExternalSourceHashEvidence>> CapturePluginHashesAsync(
        ExternalPluginSampleManifest manifest,
        CancellationToken cancellationToken)
    {
        var evidence = new List<ExternalSourceHashEvidence>(manifest.CanonicalLoadOrderPluginPaths.Count);
        foreach (var path in manifest.CanonicalLoadOrderPluginPaths)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var info = new FileInfo(path);
            await using var stream = new FileStream(
                path,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read | FileShare.Delete,
                131072,
                FileOptions.Asynchronous | FileOptions.SequentialScan);
            var digest = await SHA256.HashDataAsync(stream, cancellationToken);
            evidence.Add(new ExternalSourceHashEvidence
            {
                Path = path,
                Exists = true,
                Length = info.Length,
                Sha256 = Convert.ToHexString(digest),
            });
        }

        return evidence;
    }

    /// <summary>Verifies manual plugin hashes, metadata absence states, and engine source baseline identities remained unchanged.</summary>
    /// <param name="report">The complete execution report.</param>
    /// <exception cref="InvalidDataException">Thrown when any source evidence changes.</exception>
    private static void VerifySourceEvidence(ExternalPluginSampleExecutionReport report)
    {
        if (report.SourcePluginHashesAfter is null
            || report.SourcePluginHashesBefore.Count != report.SourcePluginHashesAfter.Count
            || !report.SourcePluginHashesBefore.Zip(report.SourcePluginHashesAfter).All(pair =>
                pair.First.Path == pair.Second.Path
                && pair.First.Exists == pair.Second.Exists
                && pair.First.Length == pair.Second.Length
                && pair.First.Sha256 == pair.Second.Sha256))
        {
            throw new InvalidDataException("Streaming source plugin identity evidence changed during execution.");
        }

        if (report.PreflightAfter is null
            || !MetadataMatches(report.PreflightBefore.LooseStringSidecars, report.PreflightAfter.LooseStringSidecars)
            || !MetadataMatches(report.PreflightBefore.ApplicableArchives, report.PreflightAfter.ApplicableArchives))
        {
            throw new InvalidDataException("Source string-sidecar or applicable-archive metadata, including absence states, changed during execution.");
        }

        if (report.Cases.Any(testCase => testCase.InitialSourceBaselineId != testCase.FreshSourceBaselineId))
        {
            throw new InvalidDataException("Engine-owned complete source artifact baseline identity changed during a fresh reopen.");
        }
    }

    /// <summary>Compares ordered file metadata including explicit absence states.</summary>
    /// <param name="before">Metadata captured before plugin parsing.</param>
    /// <param name="after">Metadata captured after all plugin lifetimes were released.</param>
    /// <returns><see langword="true"/> when every observation is identical.</returns>
    private static bool MetadataMatches(
        IReadOnlyList<ExternalPluginFileMetadata> before,
        IReadOnlyList<ExternalPluginFileMetadata> after)
    {
        return before.Count == after.Count && before.Zip(after).All(pair =>
            pair.First.Path == pair.Second.Path
            && pair.First.Exists == pair.Second.Exists
            && pair.First.Length == pair.Second.Length
            && pair.First.LastWriteTimeUtc == pair.Second.LastWriteTimeUtc
            && pair.First.IsReparsePoint == pair.Second.IsReparsePoint);
    }

    /// <summary>Requires a successful engine result and returns its non-null value.</summary>
    /// <typeparam name="T">The engine result payload type.</typeparam>
    /// <param name="result">The engine operation result.</param>
    /// <param name="operation">The human-readable operation used in diagnostics.</param>
    /// <returns>The successful non-null engine value.</returns>
    /// <exception cref="InvalidDataException">Thrown when the engine result failed or omitted its value.</exception>
    private static T RequireSuccess<T>(EngineResult<T> result, string operation)
    {
        if (!result.Succeeded || result.Value is null)
        {
            throw new InvalidDataException($"Engine failed while {operation}: {DescribeError(result.Error)}");
        }

        return result.Value;
    }

    /// <summary>Requires exact JSON text emitted by the deterministic plugin inspector.</summary>
    /// <param name="expected">The expected complete inspector JSON.</param>
    /// <param name="actual">The actual complete inspector JSON.</param>
    /// <param name="context">The comparison context used in diagnostics.</param>
    /// <exception cref="InvalidDataException">Thrown when the values differ.</exception>
    private static void RequireJsonEqual(string expected, string actual, string context)
    {
        if (!string.Equals(expected, actual, StringComparison.Ordinal))
        {
            throw new InvalidDataException($"Complete plugin inspector JSON differed for {context}.");
        }
    }

    /// <summary>Formats one typed engine error.</summary>
    /// <param name="error">The optional engine error.</param>
    /// <returns>A stable code-and-message diagnostic.</returns>
    private static string DescribeError(EngineError? error)
    {
        return error is null ? "<no typed error>" : $"{error.Code}: {error.Message}";
    }

    /// <summary>Formats one guarded save outcome.</summary>
    /// <param name="save">The complete save result.</param>
    /// <returns>A stable status-and-error diagnostic.</returns>
    private static string DescribeSave(SaveResult save)
    {
        return $"Status={save.Status}; Error={DescribeError(save.Error)}; Recovery={save.RecoveryEvidenceToken?.Value ?? "<none>"}";
    }
}
