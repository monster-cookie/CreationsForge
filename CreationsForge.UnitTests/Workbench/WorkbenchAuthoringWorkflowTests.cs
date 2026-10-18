using System.Text.Json.Nodes;
using CreationsForge.Workbench;

namespace CreationsForge.UnitTests.Workbench;

/// <summary>Verifies the guided authoring actions against a scripted MCP session.</summary>
public sealed class WorkbenchAuthoringWorkflowTests
{
    /// <summary>Starts an empty translated field on the output language selected for the plugin.</summary>
    [Fact]
    public async Task NewRecordSeedsTranslatedTargetFromTheOutputLanguage()
    {
        var session = new ScriptedSession();
        var viewModel = await OpenOutputAsync(session);

        await viewModel.NewRecordAsync();

        var name = viewModel.Fields.Single(field => field.Path == "Name");
        Assert.Equal("French", name.TargetLanguage);
        Assert.Equal("French", Assert.Single(name.Translations).Language);
        await viewModel.CloseAsync();
    }

    /// <summary>Keeps an invalid FormKey, skips the host, and does not report a save.</summary>
    [Fact]
    public async Task ValidationFailureKeepsTheFormAndDoesNotSave()
    {
        var session = new ScriptedSession();
        var viewModel = await OpenOutputAsync(session);

        await viewModel.NewRecordAsync();
        var link = viewModel.Fields.Single(field => field.Path == "AttractionRule");
        link.FormKeyText = "bad";
        await viewModel.ApplyRecordAsync();

        Assert.Equal("Validation failed.", viewModel.Status);
        Assert.Contains("not a FormKey", viewModel.Diagnostics, StringComparison.Ordinal);
        Assert.Contains("not saved", viewModel.Diagnostics, StringComparison.OrdinalIgnoreCase);
        Assert.Equal("bad", link.FormKeyText);
        Assert.DoesNotContain(session.Calls, call => call.Tool is "record_create" or "record_override");
        Assert.False(viewModel.HasUnsavedEdits);
        await viewModel.CloseAsync();
    }

    /// <summary>A rejected save leaves the previous path in place and does not say the new edit was published.</summary>
    [Fact]
    public async Task SaveFailureDoesNotReportAPublishedEdit()
    {
        var session = new ScriptedSession { FailAfterFirstSave = true };
        var viewModel = await OpenOutputAsync(session);
        await EditAndApplyAsync(viewModel);

        await viewModel.SaveChangesAsync();

        Assert.Equal("Save failed.", viewModel.Status);
        Assert.Contains("not updated", viewModel.Observations, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Saved ", viewModel.Status, StringComparison.Ordinal);
        Assert.True(viewModel.HasUnsavedEdits);
        Assert.False(viewModel.CanReopenFresh);
        Assert.Contains(session.OutputPath, viewModel.PublishedPathText, StringComparison.Ordinal);
        await viewModel.CloseAsync();
    }

    /// <summary>Discard restores the baseline in memory and does not claim a file write.</summary>
    [Fact]
    public async Task DiscardDoesNotWriteTheOutput()
    {
        var session = new ScriptedSession();
        var viewModel = await OpenOutputAsync(session);
        await EditAndApplyAsync(viewModel);

        await viewModel.DiscardChangesAsync();

        Assert.Equal("Discarded unsaved edits.", viewModel.Status);
        Assert.Contains("does not write", viewModel.Diagnostics, StringComparison.OrdinalIgnoreCase);
        Assert.False(viewModel.HasUnsavedEdits);
        Assert.Empty(viewModel.Fields);
        Assert.Contains(session.Calls, call => call.Tool == "workspace_discard");
        await viewModel.CloseAsync();
    }

    /// <summary>Closing with an unsaved apply says the edits were not written.</summary>
    [Fact]
    public async Task CloseWithUnsavedEditsDoesNotReportASave()
    {
        var session = new ScriptedSession();
        var viewModel = await OpenOutputAsync(session);
        await EditAndApplyAsync(viewModel);

        await viewModel.CloseAsync();

        Assert.Equal("Disconnected", viewModel.Status);
        Assert.Contains("without saving", viewModel.Diagnostics, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("not written", viewModel.Diagnostics, StringComparison.OrdinalIgnoreCase);
        Assert.True(session.Disposed);
    }

    /// <summary>Reopen is refused while edits are dirty and does not start another host.</summary>
    [Fact]
    public async Task ReopenRefusesUnsavedEdits()
    {
        var starts = 0;
        var session = new ScriptedSession();
        var viewModel = new WorkbenchViewModel((_, _) =>
        {
            Interlocked.Increment(ref starts);
            return Task.FromResult<IWorkbenchMcpSession>(session);
        })
        {
            DataDirectory = Path.GetTempPath(),
            OutputPath = Path.Combine(Path.GetTempPath(), "CreationsForgeAuthoring.esp"),
        };
        await viewModel.ConnectAsync();
        await viewModel.OpenWorkspaceAsync();
        await viewModel.OpenOutputAsync(createNew: true);
        await EditAndApplyAsync(viewModel);

        await viewModel.ReopenFreshAsync();

        Assert.Equal(1, starts);
        Assert.False(session.Disposed);
        Assert.Equal("Reopen refused.", viewModel.Status);
        Assert.Contains("not on disk", viewModel.Diagnostics, StringComparison.OrdinalIgnoreCase);
        await viewModel.CloseAsync();
    }

    /// <summary>A clean save reopens in a second session and records the path, file check, and unrelated record.</summary>
    [Fact]
    public async Task ReopenReadsTheSavedRecordInAFreshSession()
    {
        var first = new ScriptedSession();
        var second = new ScriptedSession();
        var sessions = new Queue<IWorkbenchMcpSession>([first, second]);
        var outputPath = Path.Combine(Path.GetTempPath(), "creationsforge-reopen-" + Guid.NewGuid().ToString("N"), "Output.esp");
        var viewModel = new WorkbenchViewModel((_, _) => Task.FromResult(sessions.Dequeue()))
        {
            DataDirectory = Path.GetTempPath(),
            OutputPath = outputPath,
            TextStorageMode = "Localized",
            Language = "French",
        };
        await viewModel.ConnectAsync();
        await viewModel.OpenWorkspaceAsync();
        await viewModel.OpenOutputAsync(createNew: true);
        await EditAndApplyAsync(viewModel);
        var apply = Assert.Single(first.Calls, call => call.Tool == "record_override");
        Assert.Contains("\"expectedRevision\":0", apply.Arguments, StringComparison.Ordinal);
        var arguments = JsonNode.Parse(apply.Arguments)!.AsObject();
        var nameChange = arguments["changes"]!.AsArray()
            .Select(node => node!.AsObject())
            .Single(change => change["path"]!.GetValue<string>() == "Name");
        var nameValue = nameChange["value"]!.AsObject();
        Assert.Equal("French", nameValue["targetLanguage"]!.GetValue<string>());
        Assert.Equal("Nom modifié", nameValue["translations"]!["French"]!.GetValue<string>());
        Assert.Contains("Localized", viewModel.Observations, StringComparison.Ordinal);

        await viewModel.CompareRecordAsync();
        await viewModel.PreviewChangesAsync();
        await viewModel.SaveChangesAsync();
        Assert.StartsWith("Saved " + outputPath, viewModel.Status, StringComparison.Ordinal);
        Assert.False(viewModel.HasUnsavedEdits);

        await viewModel.ReopenFreshAsync();

        Assert.True(first.Disposed);
        Assert.False(second.Disposed);
        Assert.Contains(second.Calls, call => call.Tool == "output_open");
        Assert.DoesNotContain(second.Calls, call => call.Tool == "output_create");
        Assert.Contains(second.Calls, call => call.Tool == "record_read" && call.Arguments.Contains("000002:Source.esm", StringComparison.Ordinal));
        Assert.Contains("Fresh session output path: " + outputPath, viewModel.Observations, StringComparison.Ordinal);
        Assert.Contains("File exists: false", viewModel.Observations, StringComparison.Ordinal);
        Assert.Contains("Nom modifié", viewModel.Observations, StringComparison.Ordinal);
        Assert.Contains("UnrelatedKeyword is unchanged", viewModel.Observations, StringComparison.Ordinal);
        Assert.Equal("Reopen did not find the output file.", viewModel.Status);
        await viewModel.CloseAsync();
        Assert.True(second.Disposed);
    }

    private static async Task<WorkbenchViewModel> OpenOutputAsync(ScriptedSession session)
    {
        var viewModel = new WorkbenchViewModel((_, _) => Task.FromResult<IWorkbenchMcpSession>(session))
        {
            DataDirectory = Path.GetTempPath(),
            OutputPath = Path.Combine(Path.GetTempPath(), "CreationsForgeAuthoring.esp"),
            TextStorageMode = "Localized",
            Language = "French",
        };
        await viewModel.ConnectAsync();
        await viewModel.OpenWorkspaceAsync();
        await viewModel.OpenOutputAsync(createNew: true);
        return viewModel;
    }

    private static async Task EditAndApplyAsync(WorkbenchViewModel viewModel)
    {
        viewModel.EditorIdFilter = "Keyword";
        await viewModel.FindRecordsAsync();
        viewModel.SelectedHit = viewModel.SearchHits.Single(hit => hit.EditorId == "SourceKeyword");
        await viewModel.OverrideSelectedAsync();
        var name = viewModel.Fields.Single(field => field.Path == "Name");
        name.TargetLanguage = "French";
        name.AddTranslation("French");
        name.Translations.Single(row => row.Language == "French").Value = "Nom modifié";
        await viewModel.ApplyRecordAsync();
        Assert.Equal("Applied in memory. The output file was not saved.", viewModel.Status);
        Assert.True(viewModel.HasUnsavedEdits);
    }

    /// <summary>Returns canned authoring results and records the calls.</summary>
    private sealed class ScriptedSession : WorkbenchSessionDouble
    {
        private ulong _revision;

        /// <summary>Gets the recorded tool calls.</summary>
        public List<(string Tool, string Arguments)> Calls { get; } = [];

        /// <summary>Gets the output path from the latest output call.</summary>
        public string OutputPath { get; private set; } = string.Empty;

        /// <summary>Gets or sets whether every save after the first returns a failed status.</summary>
        public bool FailAfterFirstSave { get; set; }

        /// <summary>Gets the number of save calls.</summary>
        public int SaveCount { get; private set; }

        /// <inheritdoc />
        public override Task<JsonObject> CallAsync(string tool, JsonObject arguments, CancellationToken cancellationToken)
        {
            Calls.Add((tool, arguments.ToJsonString()));
            cancellationToken.ThrowIfCancellationRequested();
            if (arguments["outputPath"] is JsonValue output && output.TryGetValue<string>(out var path))
            {
                OutputPath = path;
            }

            var content = Result(tool);
            if (tool == "workspace_close")
            {
                WorkspaceId = null;
                Revision = null;
            }
            else
            {
                WorkspaceId = "session-1";
                Revision = _revision;
            }

            return Task.FromResult(new JsonObject { ["structuredContent"] = content });
        }

        private JsonObject Result(string tool)
        {
            switch (tool)
            {
                case "workspace_save":
                    SaveCount++;
                    var failed = FailAfterFirstSave && SaveCount > 1;
                    return new JsonObject
                    {
                        ["workspaceId"] = "session-1",
                        ["revision"] = _revision,
                        ["status"] = failed ? "Failed" : "Succeeded",
                        ["destinationPath"] = OutputPath,
                    };
                case "record_override":
                    _revision++;
                    return new JsonObject
                    {
                        ["workspaceId"] = "session-1",
                        ["revision"] = _revision,
                        ["records"] = new JsonArray(PublishedRecord()),
                    };
                case "record_types":
                    return new JsonObject
                    {
                        ["release"] = "Starfield",
                        ["families"] = new JsonArray("Keyword"),
                    };
                case "record_schema":
                    return new JsonObject
                    {
                        ["fields"] = new JsonArray(
                            Schema("EditorID", "String", true),
                            Schema("Name", "TranslatedString", false),
                            Schema("Type", "Enum", true, "Location"),
                            Schema("AttractionRule", "FormLink", true),
                            Schema("Items", "List", true)),
                    };
                case "records_search":
                    return new JsonObject
                    {
                        ["revision"] = _revision,
                        ["records"] = new JsonArray(Hit("000001:Source.esm", "SourceKeyword"), Hit("000002:Source.esm", "UnrelatedKeyword")),
                    };
                case "record_read":
                    return ReadRecord();
                case "record_compare":
                    return new JsonObject
                    {
                        ["differences"] = new JsonArray(new JsonObject { ["path"] = "Name" }),
                    };
                case "workspace_preview":
                    return new JsonObject
                    {
                        ["isDirty"] = true,
                        ["records"] = new JsonArray(PublishedRecord()),
                    };
                case "workspace_discard":
                    return new JsonObject
                    {
                        ["revision"] = _revision,
                        ["changed"] = true,
                        ["isDirty"] = false,
                    };
                case "workspace_state":
                    return new JsonObject
                    {
                        ["workspaceId"] = "session-1",
                        ["revision"] = _revision,
                        ["isDirty"] = false,
                    };
                default:
                    return new JsonObject
                    {
                        ["workspaceId"] = "session-1",
                        ["revision"] = _revision,
                    };
            }
        }

        private JsonObject ReadRecord()
        {
            var arguments = Calls[^1].Arguments;
            var unrelated = arguments.Contains("000002:Source.esm", StringComparison.Ordinal);
            var output = arguments.Contains("Output.esp", StringComparison.OrdinalIgnoreCase);
            var name = unrelated
                ? Translation("English", "Leave Alone")
                : output
                    ? new JsonObject
                    {
                        ["kind"] = "TranslatedString",
                        ["targetLanguage"] = "French",
                        ["translations"] = new JsonObject
                        {
                            ["English"] = "Source Name",
                            ["French"] = "Nom modifié",
                        },
                    }
                    : Translation("English", "Source Name");
            var editorId = unrelated ? "UnrelatedKeyword" : "SourceKeyword";
            return new JsonObject
            {
                ["record"] = new JsonObject
                {
                    ["formKey"] = unrelated ? "000002:Source.esm" : "000001:Source.esm",
                    ["containingModKey"] = output ? "Output.esp" : "Source.esm",
                    ["fields"] = new JsonArray(
                        new JsonObject
                        {
                            ["path"] = "EditorID",
                            ["value"] = new JsonObject { ["kind"] = "String", ["string"] = editorId },
                        },
                        new JsonObject { ["path"] = "Name", ["value"] = name }),
                },
            };
        }

        private JsonObject PublishedRecord()
        {
            return new JsonObject
            {
                ["formKey"] = "000001:Source.esm",
                ["containingModKey"] = "Output.esp",
                ["fields"] = new JsonArray(),
            };
        }

        private static JsonObject Hit(string formKey, string editorId)
        {
            return new JsonObject
            {
                ["formKey"] = formKey,
                ["containingModKey"] = "Source.esm",
                ["winningModKey"] = "Source.esm",
                ["editorId"] = editorId,
            };
        }

        private static JsonObject Schema(string path, string kind, bool nullable, string? choice = null)
        {
            var choices = new JsonArray();
            if (choice is not null)
            {
                choices.Add(choice);
            }

            return new JsonObject
            {
                ["path"] = path,
                ["kind"] = kind,
                ["isNullable"] = nullable,
                ["isFlags"] = false,
                ["choices"] = choices,
                ["referenceTargets"] = new JsonArray(),
            };
        }

        private static JsonObject Translation(string language, string text)
        {
            return new JsonObject
            {
                ["kind"] = "TranslatedString",
                ["targetLanguage"] = language,
                ["translations"] = new JsonObject { [language] = text },
            };
        }
    }
}
