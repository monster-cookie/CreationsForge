using System.Text.Json.Nodes;
using CreationsForge.Mcp.Protocol;

namespace CreationsForge.UnitTests.McpHost;

/// <summary>Proves the production stdio host against the installed Starfield Data directory.</summary>
/// <remarks>
/// These tests write a disposable plugin, its string files, and a lock sidecar into the real game Data directory and remove
/// them afterwards, so they are explicit and skip when Starfield is not installed. Fresh host processes reopen the physical
/// output; a success response alone is never treated as proof that the file was written.
/// </remarks>
public sealed class McpInstalledStarfieldTests
{
    private const string Family = "Keyword";
    private const string StarfieldMaster = "Starfield.esm";
    private static readonly TimeSpan Budget = TimeSpan.FromMinutes(20);

    /// <summary>Saves an empty plugin in the installed Data directory and reopens it in a fresh MCP session.</summary>
    [Fact(Explicit = true)]
    public async Task EmptyPluginSavedInInstalledDataReopensInFreshSession()
    {
        using var timeout = new CancellationTokenSource(Budget);
        var cancellationToken = timeout.Token;
        using var output = RequireInstalledOutput();

        await using (var client = await McpStdioClient.StartAsync(cancellationToken))
        {
            var workspaceId = await OpenAsync(client, output, "empty-open", cancellationToken);
            var created = (await client.CallAsync(
                McpAuthoringContract.OutputCreateTool,
                McpRequests.OutputCreate("empty-create", workspaceId, output.PluginPath),
                cancellationToken)).RequireSuccess();
            Assert.True(created.RequiredBool("isNewOutput"));
            Assert.True(File.Exists(output.LockPath));
            Assert.False(File.Exists(output.PluginPath));

            var saved = await SaveAsync(client, workspaceId, "empty-save", cancellationToken);
            Assert.Equal("Succeeded", saved.RequiredString("status"));
            Assert.Equal(output.PluginPath, Path.GetFullPath(saved.RequiredString("destinationPath")), ignoreCase: true);
            await CloseAsync(client, workspaceId, "empty-close", discardUnsaved: false, cancellationToken);
            await client.ShutdownAsync(cancellationToken);
        }

        Assert.True(File.Exists(output.PluginPath));
        Assert.False(File.Exists(output.LockPath));
        Assert.Empty(output.StringFiles());
        Assert.Empty(await InstalledOutputReopenCheck.CheckAsync(output, "Embedded", [], cancellationToken));
        output.AssertInstalledPluginsUnchanged();
        Assert.Empty(output.NewOperationDirectories());
        Assert.Empty(output.Cleanup());
        Assert.False(File.Exists(output.PluginPath));
    }

    /// <summary>
    /// Creates, edits, overrides, compares, saves, and reopens localized Keywords in the installed Data directory, then proves
    /// that validation failure, save failure, and discard leave the file unchanged and that an incorrect or missing output
    /// fails the reopen check.
    /// </summary>
    [Fact(Explicit = true)]
    public async Task LocalizedAuthoringInInstalledDataSurvivesFreshSessions()
    {
        using var timeout = new CancellationTokenSource(Budget);
        var cancellationToken = timeout.Token;
        using var output = RequireInstalledOutput();
        var prefix = output.EditorIdPrefix;
        var plugin = output.PluginName;
        string createdKey;
        string sourceKey;
        Dictionary<string, JsonNode?> overrideFields;
        IReadOnlyDictionary<string, JsonNode?> unrelatedFields;
        string unrelatedKey;

        await using (var client = await McpStdioClient.StartAsync(cancellationToken))
        {
            var workspaceId = await OpenAsync(client, output, "author-open", cancellationToken);
            var attached = (await client.CallAsync(
                McpAuthoringContract.OutputCreateTool,
                McpRequests.OutputCreate("author-create", workspaceId, output.PluginPath, "Localized"),
                cancellationToken)).RequireSuccess();
            Assert.Equal("Localized", attached.RequiredString("textStorageMode"));
            await AssertTranslatedNameAsync(client, cancellationToken);

            var source = await FindMasterKeywordAsync(client, workspaceId, cancellationToken);
            sourceKey = source.RequiredString("formKey");
            overrideFields = new Dictionary<string, JsonNode?>(
                await ReadRequiredFieldsAsync(client, workspaceId, sourceKey, StarfieldMaster, cancellationToken),
                StringComparer.Ordinal)
            {
                ["Name"] = Translated("Override name", "Nom remplacé"),
            };

            var batch = (await client.CallAsync(
                McpAuthoringContract.RecordApplyTool,
                Apply("author-batch", workspaceId, 0,
                    Create(EditorId(prefix + "Unrelated"), Name(Translated("Unrelated name", "Nom sans rapport"))),
                    Create(EditorId(prefix + "Created"), Name(Translated("Created name", "Nom créé")))),
                cancellationToken)).RequireSuccess();
            Assert.Equal(1ul, batch.RequiredUInt64("revision"));
            var records = batch.Array("records");
            Assert.Equal(2, records.Count);
            Assert.All(records, record => Assert.Equal(plugin, record.RequiredString("containingModKey")));
            unrelatedKey = records[0].RequiredString("formKey");
            createdKey = records[1].RequiredString("formKey");

            var firstSave = await SaveAsync(client, workspaceId, "author-save-1", cancellationToken);
            Assert.Equal("Succeeded", firstSave.RequiredString("status"));
            unrelatedFields = await ReadRequiredFieldsAsync(client, workspaceId, unrelatedKey, plugin, cancellationToken);
            Assert.True(JsonNode.DeepEquals(Translated("Unrelated name", "Nom sans rapport"), unrelatedFields["Name"]));

            var edited = (await client.CallAsync(
                McpAuthoringContract.RecordApplyTool,
                Apply("author-edit", workspaceId, 1,
                    Override(createdKey, plugin,
                        EditorId(prefix + "Edited"),
                        Name(Translated("Edited name", "Nom modifié")))),
                cancellationToken)).RequireSuccess();
            Assert.Equal(2ul, edited.RequiredUInt64("revision"));

            var overridden = (await client.CallAsync(
                McpAuthoringContract.RecordOverrideTool,
                new JsonObject
                {
                    ["operationId"] = "author-override",
                    ["workspaceId"] = workspaceId,
                    ["familyId"] = Family,
                    ["formKey"] = sourceKey,
                    ["containingModKey"] = StarfieldMaster,
                    ["expectedRevision"] = McpRequests.Revision(2),
                    ["changes"] = new JsonArray(Name(Translated("Override name", "Nom remplacé"))),
                },
                cancellationToken)).RequireSuccess();
            Assert.Equal(3ul, overridden.RequiredUInt64("revision"));
            Assert.Equal(plugin, Assert.Single(overridden.Array("records")).RequiredString("containingModKey"));

            var differences = await CompareAsync(client, workspaceId, sourceKey, plugin, cancellationToken);
            Assert.Equal(new[] { "Name" }, differences.Keys);
            Assert.True(JsonNode.DeepEquals(Translated("Override name", "Nom remplacé"), differences["Name"]));

            var secondSave = await SaveAsync(client, workspaceId, "author-save-2", cancellationToken);
            Assert.Equal("Succeeded", secondSave.RequiredString("status"));
            Assert.Equal(3ul, secondSave.RequiredUInt64("revision"));
            var published = secondSave.Strings("publishedPaths").Select(Path.GetFullPath).ToArray();
            Assert.Contains(output.PluginPath, published, StringComparer.OrdinalIgnoreCase);
            Assert.Contains(published, path => output.StringFiles().Contains(path, StringComparer.OrdinalIgnoreCase));
            await CloseAsync(client, workspaceId, "author-close", discardUnsaved: false, cancellationToken);
            await client.ShutdownAsync(cancellationToken);
        }

        Assert.True(File.Exists(output.PluginPath));
        Assert.NotEmpty(output.StringFiles());
        Assert.False(File.Exists(output.LockPath));
        var savedFiles = output.ReadFileSet();
        ExpectedOutputRecord[] expected =
        [
            new ExpectedOutputRecord("unrelated", unrelatedKey, unrelatedFields),
            new ExpectedOutputRecord(
                "created",
                createdKey,
                new Dictionary<string, JsonNode?>
                {
                    ["EditorID"] = StringValue(prefix + "Edited"),
                    ["Name"] = Translated("Edited name", "Nom modifié"),
                }),
            new ExpectedOutputRecord("override", sourceKey, overrideFields),
        ];
        Assert.Empty(await InstalledOutputReopenCheck.CheckAsync(output, "Localized", expected, cancellationToken));

        await ProveFailuresLeaveOutputUnchangedAsync(output, createdKey, savedFiles, cancellationToken);
        output.AssertFileSetUnchanged(savedFiles);
        Assert.Empty(await InstalledOutputReopenCheck.CheckAsync(output, "Localized", expected, cancellationToken));
        var wrongMode = await InstalledOutputReopenCheck.CheckAsync(output, "Embedded", expected, cancellationToken);
        Assert.Contains(wrongMode, problem =>
            problem.StartsWith("output_open failed", StringComparison.Ordinal)
            && problem.Contains("text storage", StringComparison.Ordinal));

        await SaveIncorrectNameAsync(output, createdKey, cancellationToken);
        var incorrect = await InstalledOutputReopenCheck.CheckAsync(output, "Localized", expected, cancellationToken);
        Assert.NotEmpty(incorrect);
        Assert.All(incorrect, problem => Assert.Contains(createdKey, problem, StringComparison.OrdinalIgnoreCase));

        Assert.Empty(output.DeleteOutputFiles());
        var missing = await InstalledOutputReopenCheck.CheckAsync(output, "Localized", expected, cancellationToken);
        Assert.Contains(missing, problem => problem.StartsWith("output_open failed", StringComparison.Ordinal));

        output.AssertInstalledPluginsUnchanged();
        Assert.Empty(output.NewOperationDirectories());
        Assert.Empty(output.Cleanup());
        Assert.False(File.Exists(output.PluginPath));
        Assert.Empty(output.StringFiles());
    }

    /// <summary>In a fresh session, proves validation failure, discard, and an external-change save failure leave the saved file set unchanged and never report a save.</summary>
    private static async Task ProveFailuresLeaveOutputUnchangedAsync(
        InstalledStarfieldOutput output,
        string createdKey,
        IReadOnlyDictionary<string, byte[]> savedFiles,
        CancellationToken cancellationToken)
    {
        var prefix = output.EditorIdPrefix;
        await using var client = await McpStdioClient.StartAsync(cancellationToken);
        var workspaceId = await OpenExistingAsync(client, output, "failure", cancellationToken);

        var rejected = await client.CallAsync(
            McpAuthoringContract.RecordApplyTool,
            Apply("invalid-batch", workspaceId, 0,
                Create(EditorId(prefix + "Rejected")),
                Override(createdKey, output.PluginName, new JsonObject
                {
                    ["path"] = "EditorID",
                    ["operation"] = "Set",
                    ["value"] = new JsonObject { ["kind"] = "Boolean", ["boolean"] = true },
                })),
            cancellationToken);
        Assert.Equal("invalid_field", rejected.ErrorCode());
        var afterRejected = await StateAsync(client, workspaceId, cancellationToken);
        Assert.Equal(0ul, afterRejected.RequiredUInt64("revision"));
        Assert.False(afterRejected.RequiredBool("isDirty"));
        Assert.Empty(await McpRequests.SearchAllAsync(client, workspaceId, prefix + "Rejected", cancellationToken));
        var cleanSave = await SaveAsync(client, workspaceId, "save-after-invalid", cancellationToken);
        Assert.Equal("NoChanges", cleanSave.RequiredString("status"));
        output.AssertFileSetUnchanged(savedFiles);

        var pending = (await client.CallAsync(
            McpAuthoringContract.RecordApplyTool,
            Apply("discarded-create", workspaceId, 0, Create(EditorId(prefix + "Discarded"))),
            cancellationToken)).RequireSuccess();
        Assert.Equal(1ul, pending.RequiredUInt64("revision"));
        var discarded = (await client.CallAsync(
            McpAuthoringContract.WorkspaceDiscardTool,
            new JsonObject { ["operationId"] = "discard", ["workspaceId"] = workspaceId },
            cancellationToken)).RequireSuccess();
        Assert.True(discarded.RequiredBool("changed"));
        Assert.False(discarded.RequiredBool("isDirty"));
        Assert.Empty(await McpRequests.SearchAllAsync(client, workspaceId, prefix + "Discarded", cancellationToken));
        var discardSave = await SaveAsync(client, workspaceId, "save-after-discard", cancellationToken);
        Assert.Equal("NoChanges", discardSave.RequiredString("status"));
        output.AssertFileSetUnchanged(savedFiles);

        var revision = discarded.RequiredUInt64("revision");
        (await client.CallAsync(
            McpAuthoringContract.RecordApplyTool,
            Apply("unsaved-create", workspaceId, revision, Create(EditorId(prefix + "Unsaved"))),
            cancellationToken)).RequireSuccess();
        var savedWriteTime = File.GetLastWriteTimeUtc(output.PluginPath);
        File.SetLastWriteTimeUtc(output.PluginPath, savedWriteTime.AddMinutes(-5));
        McpCallResult failedSave;
        try
        {
            failedSave = await client.CallAsync(
                McpAuthoringContract.WorkspaceSaveTool,
                new JsonObject { ["operationId"] = "save-after-external-change", ["workspaceId"] = workspaceId },
                cancellationToken);
        }
        finally
        {
            File.SetLastWriteTimeUtc(output.PluginPath, savedWriteTime);
        }

        Assert.Equal("save_failed", failedSave.ErrorCode());
        var details = failedSave.Structured!.Object("details");
        Assert.Equal("Failed", details.RequiredString("status"));
        Assert.Equal("Unchanged", details.RequiredString("publicationState"));
        Assert.Contains("changed outside this workspace", failedSave.Structured.RequiredString("message"), StringComparison.Ordinal);
        output.AssertFileSetUnchanged(savedFiles);
        Assert.True((await StateAsync(client, workspaceId, cancellationToken)).RequiredBool("isDirty"));

        await CloseAsync(client, workspaceId, "failure-close", discardUnsaved: true, cancellationToken);
        await client.ShutdownAsync(cancellationToken);
    }

    /// <summary>Saves a different Name on the created record through the production host so the physical output no longer matches expectations.</summary>
    private static async Task SaveIncorrectNameAsync(
        InstalledStarfieldOutput output,
        string createdKey,
        CancellationToken cancellationToken)
    {
        await using var client = await McpStdioClient.StartAsync(cancellationToken);
        var workspaceId = await OpenExistingAsync(client, output, "incorrect", cancellationToken);
        (await client.CallAsync(
            McpAuthoringContract.RecordApplyTool,
            Apply("incorrect-edit", workspaceId, 0,
                Override(createdKey, output.PluginName, Name(Translated("Wrong name", "Mauvais nom")))),
            cancellationToken)).RequireSuccess();
        var saved = await SaveAsync(client, workspaceId, "incorrect-save", cancellationToken);
        Assert.Equal("Succeeded", saved.RequiredString("status"));
        await CloseAsync(client, workspaceId, "incorrect-close", discardUnsaved: false, cancellationToken);
        await client.ShutdownAsync(cancellationToken);
    }

    /// <summary>Resolves the installed Data directory or skips the test when Starfield is not installed.</summary>
    private static InstalledStarfieldOutput RequireInstalledOutput()
    {
        var output = InstalledStarfieldOutput.TryCreate();
        Assert.SkipWhen(output is null, "Starfield is not installed, so the installed Data directory cannot be exercised.");
        return output!;
    }

    /// <summary>Opens a pending Starfield session on the installed Data directory with no selected plugins and returns its identifier.</summary>
    private static async Task<string> OpenAsync(
        McpStdioClient client,
        InstalledStarfieldOutput output,
        string operationId,
        CancellationToken cancellationToken)
    {
        var opened = (await client.CallAsync(
            McpAuthoringContract.WorkspaceOpenTool,
            McpRequests.WorkspaceOpen(operationId, "Starfield", output.DataDirectory),
            cancellationToken)).RequireSuccess();
        Assert.Equal(output.DataDirectory, Path.GetFullPath(opened.RequiredString("dataDirectory")), ignoreCase: true);
        return opened.RequiredString("workspaceId");
    }

    /// <summary>Opens a fresh session on the existing localized output and requires a clean revision-zero workspace.</summary>
    private static async Task<string> OpenExistingAsync(
        McpStdioClient client,
        InstalledStarfieldOutput output,
        string operationPrefix,
        CancellationToken cancellationToken)
    {
        var workspaceId = await OpenAsync(client, output, operationPrefix + "-open", cancellationToken);
        var attached = (await client.CallAsync(
            McpAuthoringContract.OutputOpenTool,
            McpRequests.OutputOpen(operationPrefix + "-output", workspaceId, output.PluginPath, "Localized"),
            cancellationToken)).RequireSuccess();
        Assert.False(attached.RequiredBool("isNewOutput"));
        Assert.False(attached.RequiredBool("isDirty"));
        Assert.Equal(0ul, attached.RequiredUInt64("revision"));
        return workspaceId;
    }

    /// <summary>Calls <c>workspace_save</c> and requires a successful result.</summary>
    private static async Task<McpJson> SaveAsync(
        McpStdioClient client,
        string workspaceId,
        string operationId,
        CancellationToken cancellationToken)
    {
        return (await client.CallAsync(
            McpAuthoringContract.WorkspaceSaveTool,
            new JsonObject { ["operationId"] = operationId, ["workspaceId"] = workspaceId },
            cancellationToken)).RequireSuccess();
    }

    /// <summary>Returns the current workspace state.</summary>
    private static async Task<McpJson> StateAsync(
        McpStdioClient client,
        string workspaceId,
        CancellationToken cancellationToken)
    {
        return (await client.CallAsync(
            McpAuthoringContract.WorkspaceStateTool,
            new JsonObject { ["workspaceId"] = workspaceId },
            cancellationToken)).RequireSuccess();
    }

    /// <summary>Closes a session and requires the host to report it closed.</summary>
    private static async Task CloseAsync(
        McpStdioClient client,
        string workspaceId,
        string operationId,
        bool discardUnsaved,
        CancellationToken cancellationToken)
    {
        var closed = (await client.CallAsync(
            McpAuthoringContract.WorkspaceCloseTool,
            new JsonObject
            {
                ["operationId"] = operationId,
                ["workspaceId"] = workspaceId,
                ["discardUnsaved"] = discardUnsaved,
            },
            cancellationToken)).RequireSuccess();
        Assert.True(closed.RequiredBool("closed"));
    }

    /// <summary>Requires the Starfield Keyword schema to expose <c>Name</c> as a translated string.</summary>
    private static async Task AssertTranslatedNameAsync(McpStdioClient client, CancellationToken cancellationToken)
    {
        var schema = (await client.CallAsync(
            McpAuthoringContract.RecordSchemaTool,
            new JsonObject { ["release"] = "Starfield", ["familyId"] = Family },
            cancellationToken)).RequireSuccess();
        var name = schema.Array("fields").FirstOrDefault(field => field.RequiredString("path") == "Name");
        Assert.NotNull(name);
        Assert.Equal("TranslatedString", name.RequiredString("kind"));
    }

    /// <summary>Returns the first Keyword that <c>Starfield.esm</c> both contains and wins, with an EditorID.</summary>
    private static async Task<McpJson> FindMasterKeywordAsync(
        McpStdioClient client,
        string workspaceId,
        CancellationToken cancellationToken)
    {
        var page = (await client.CallAsync(
            McpAuthoringContract.RecordsSearchTool,
            new JsonObject { ["workspaceId"] = workspaceId, ["familyId"] = Family },
            cancellationToken)).RequireSuccess();
        var source = page.Array("records").FirstOrDefault(record =>
            record.RequiredString("containingModKey") == StarfieldMaster
            && record.RequiredString("winningModKey") == StarfieldMaster
            && !string.IsNullOrWhiteSpace(record.OptionalString("editorId")));
        Assert.True(source is not null, $"No {StarfieldMaster} Keyword with an EditorID was found: {page}");
        return source;
    }

    /// <summary>Compares the master version of a Keyword with its output override and returns the right-hand value of each difference by path.</summary>
    private static async Task<IReadOnlyDictionary<string, JsonNode?>> CompareAsync(
        McpStdioClient client,
        string workspaceId,
        string formKey,
        string outputPlugin,
        CancellationToken cancellationToken)
    {
        var differences = new Dictionary<string, JsonNode?>(StringComparer.Ordinal);
        string? cursor = null;
        for (var page = 0; page < 20; page++)
        {
            var arguments = new JsonObject
            {
                ["workspaceId"] = workspaceId,
                ["familyId"] = Family,
                ["leftFormKey"] = formKey,
                ["leftContainingModKey"] = StarfieldMaster,
                ["rightFormKey"] = formKey,
                ["rightContainingModKey"] = outputPlugin,
            };
            if (cursor is not null)
            {
                arguments["cursor"] = cursor;
            }

            var compared = (await client.CallAsync(McpAuthoringContract.RecordCompareTool, arguments, cancellationToken))
                .RequireSuccess();
            foreach (var difference in compared.Array("differences"))
            {
                differences[difference.RequiredString("path")] = difference.Node["right"]?.DeepClone();
            }

            cursor = compared.OptionalString("nextCursor");
            if (string.IsNullOrWhiteSpace(cursor))
            {
                return differences;
            }
        }

        throw new InvalidOperationException("Comparison paging did not finish.");
    }

    /// <summary>Reads every registered field of one exact Keyword version and fails the test when it cannot be read.</summary>
    private static async Task<IReadOnlyDictionary<string, JsonNode?>> ReadRequiredFieldsAsync(
        McpStdioClient client,
        string workspaceId,
        string formKey,
        string containingModKey,
        CancellationToken cancellationToken)
    {
        var (fields, error) = await InstalledOutputReopenCheck.ReadFieldsAsync(
            client,
            workspaceId,
            formKey,
            containingModKey,
            cancellationToken);
        Assert.True(fields is not null, error);
        return fields;
    }

    /// <summary>Builds <c>record_apply</c> arguments for an ordered mutation batch.</summary>
    private static JsonObject Apply(string operationId, string workspaceId, ulong expectedRevision, params JsonObject[] mutations)
    {
        return new JsonObject
        {
            ["operationId"] = operationId,
            ["workspaceId"] = workspaceId,
            ["expectedRevision"] = McpRequests.Revision(expectedRevision),
            ["mutations"] = new JsonArray(mutations),
        };
    }

    /// <summary>Builds a Keyword create mutation.</summary>
    private static JsonObject Create(params JsonObject[] changes)
    {
        return new JsonObject
        {
            ["kind"] = "Create",
            ["familyId"] = Family,
            ["changes"] = new JsonArray(changes),
        };
    }

    /// <summary>Builds a Keyword override mutation for one exact containing plugin.</summary>
    private static JsonObject Override(string formKey, string containingModKey, params JsonObject[] changes)
    {
        return new JsonObject
        {
            ["kind"] = "Override",
            ["familyId"] = Family,
            ["formKey"] = formKey,
            ["containingModKey"] = containingModKey,
            ["changes"] = new JsonArray(changes),
        };
    }

    /// <summary>Builds a field change that sets <c>EditorID</c>.</summary>
    private static JsonObject EditorId(string editorId)
    {
        return new JsonObject { ["path"] = "EditorID", ["operation"] = "Set", ["value"] = StringValue(editorId) };
    }

    /// <summary>Builds a field change that sets the translated <c>Name</c>.</summary>
    private static JsonObject Name(JsonObject translated)
    {
        return new JsonObject { ["path"] = "Name", ["operation"] = "Set", ["value"] = translated };
    }

    /// <summary>Builds a host String value.</summary>
    private static JsonObject StringValue(string value)
    {
        return new JsonObject { ["kind"] = "String", ["string"] = value };
    }

    /// <summary>Builds a host TranslatedString value targeting English with English and French text.</summary>
    private static JsonObject Translated(string english, string french)
    {
        return new JsonObject
        {
            ["kind"] = "TranslatedString",
            ["targetLanguage"] = "English",
            ["translations"] = new JsonObject
            {
                ["English"] = english,
                ["French"] = french,
            },
        };
    }
}
