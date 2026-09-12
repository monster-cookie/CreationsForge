using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;
using CreationsForge.Bootstrap.Composition;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Enums;
using CreationsForge.UnitTests.Engine.Integration;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;

namespace CreationsForge.UnitTests.Engine.ClientAcceptance;

/// <summary>Exports explicit retained three-game inputs for authoring by a real Codex MCP client.</summary>
public sealed class ClientAcceptanceFixtureExportTests
{
    /// <summary>The process-local opt-in root used by both export and verification.</summary>
    internal const string RootEnvironmentVariable = "CREATIONSFORGE_CLIENT_ACCEPTANCE_ROOT";

    /// <summary>The retained manifest file name.</summary>
    internal const string ManifestFileName = "client-acceptance-manifest.json";

    /// <summary>The only manifest schema emitted by this exporter.</summary>
    private const int SchemaVersion = 1;

    /// <summary>Serializer settings used to produce stable human-readable client input.</summary>
    internal static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = false,
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
        WriteIndented = true,
    };

    /// <summary>Exports one game fixture, captures its exact source view, and creates a closed client plan.</summary>
    /// <param name="acceptanceRoot">The validated retained root.</param>
    /// <param name="game">The supported game to export.</param>
    /// <param name="cancellationToken">A token observed throughout native inspection and file copying.</param>
    /// <returns>The complete retained case manifest.</returns>
    private static async Task<ClientAcceptanceCase> ExportCaseAsync(
        string acceptanceRoot,
        SupportedGame game,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        using var fixture = NativeWorkspaceIntegrationFixture.Create(game);
        var generatedRequest = fixture.CreateOpenRequest();
        ClientAcceptancePaths.RequireExistingDirectory(generatedRequest.DataDirectoryPath, $"generated {game} Data directory");
        foreach (var stringDirectory in generatedRequest.StringDirectoryPaths)
        {
            ClientAcceptancePaths.RequireExistingDirectory(stringDirectory, $"generated {game} string directory");
        }

        foreach (var pluginPath in generatedRequest.LoadOrderPluginPaths)
        {
            ClientAcceptancePaths.RequireExistingRegularFile(pluginPath, $"generated {game} load-order plugin");
        }

        var baseline = await ReadSourceBaselineAsync(fixture, generatedRequest, cancellationToken);
        var caseId = GameToken(game);
        var caseRoot = Path.Combine(acceptanceRoot, caseId);
        var dataDirectory = Path.Combine(caseRoot, "input", "Data");
        var stringsDirectory = Path.Combine(caseRoot, "input", "Strings");
        var outputDirectory = Path.Combine(caseRoot, "output");
        Directory.CreateDirectory(dataDirectory);
        Directory.CreateDirectory(stringsDirectory);
        Directory.CreateDirectory(outputDirectory);
        ClientAcceptancePaths.RequireExistingDirectory(dataDirectory, $"retained {game} Data directory");
        ClientAcceptancePaths.RequireExistingDirectory(stringsDirectory, $"retained {game} string directory");
        ClientAcceptancePaths.RequireExistingDirectory(outputDirectory, $"retained {game} output directory");

        var copiedPluginPaths = new List<string>(generatedRequest.LoadOrderPluginPaths.Count);
        var artifacts = new List<ClientAcceptanceArtifact>();
        for (var index = 0; index < generatedRequest.LoadOrderPluginPaths.Count; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var sourcePath = generatedRequest.LoadOrderPluginPaths[index];
            var destinationPath = Path.Combine(dataDirectory, Path.GetFileName(sourcePath));
            ClientAcceptancePaths.RequireExistingRegularFile(sourcePath, $"generated {game} load-order plugin");
            ClientAcceptancePaths.RequireExistingDirectory(Path.GetDirectoryName(destinationPath)!, $"retained {game} plugin destination directory");
            File.Copy(sourcePath, destinationPath, overwrite: false);
            ClientAcceptancePaths.RequireExistingRegularFile(destinationPath, $"retained {game} load-order plugin");
            copiedPluginPaths.Add(destinationPath);
            artifacts.Add(CreateArtifact(acceptanceRoot, destinationPath, "load_order_plugin", index));
        }

        foreach (var sourceDirectory in generatedRequest.StringDirectoryPaths)
        {
            foreach (var sourcePath in Directory.EnumerateFiles(sourceDirectory, "*", SearchOption.AllDirectories)
                .OrderBy(path => path, StringComparer.OrdinalIgnoreCase))
            {
                cancellationToken.ThrowIfCancellationRequested();
                var relativePath = Path.GetRelativePath(sourceDirectory, sourcePath);
                var destinationPath = Path.Combine(stringsDirectory, relativePath);
                ClientAcceptancePaths.RequireExistingRegularFile(sourcePath, $"generated {game} localized string");
                Directory.CreateDirectory(Path.GetDirectoryName(destinationPath)!);
                ClientAcceptancePaths.RequireExistingDirectory(Path.GetDirectoryName(destinationPath)!, $"retained {game} string destination directory");
                File.Copy(sourcePath, destinationPath, overwrite: false);
                ClientAcceptancePaths.RequireExistingRegularFile(destinationPath, $"retained {game} localized string");
                artifacts.Add(CreateArtifact(acceptanceRoot, destinationPath, "localized_string", loadOrderIndex: null));
            }
        }

        var sourceIndex = generatedRequest.LoadOrderPluginPaths
            .Select((path, index) => (path, index))
            .Single(pair => string.Equals(
                Path.GetFullPath(pair.path),
                Path.GetFullPath(generatedRequest.SourcePluginPath),
                StringComparison.OrdinalIgnoreCase))
            .index;
        var workspaceId = Guid.NewGuid();
        var reopenWorkspaceId = Guid.NewGuid();
        var outputFileName = $"CreationsForgeClientAcceptance{game}.esm";
        var output = new ClientAcceptanceOutput
        {
            PluginPath = Path.Combine(outputDirectory, outputFileName),
            ModKey = ModKey.FromNameAndExtension(outputFileName).ToString(),
            LocalizedOutputMode = "embedded",
            MasterStyle = "full",
        };
        var expected = CreateExpected(game, fixture);
        var workspaceOpen = new ClientAcceptanceWorkspaceOpen
        {
            ToolName = "creationsforge_workspace_open",
            WorkspaceId = workspaceId,
            Game = caseId,
            Release = ReleaseToken(game),
            SourcePluginPath = copiedPluginPaths[sourceIndex],
            LoadOrderPluginPaths = copiedPluginPaths.ToArray(),
            DataDirectoryPath = dataDirectory,
            StringDirectoryPaths = [stringsDirectory],
        };

        return new ClientAcceptanceCase
        {
            CaseId = caseId,
            Game = caseId,
            Release = ReleaseToken(game),
            WorkspaceId = workspaceId,
            ReopenWorkspaceId = reopenWorkspaceId,
            WorkspaceOpen = workspaceOpen,
            Artifacts = artifacts.OrderBy(artifact => artifact.RelativePath, StringComparer.Ordinal).ToArray(),
            Source = new ClientAcceptanceSource
            {
                ModKey = fixture.SourceModKey.ToString(),
                FormListFormKey = fixture.SourceListFormKey.ToString(),
                Baseline = baseline,
                AllowedChangedProperties = game == SupportedGame.Skyrim
                    ? ["EditorID", "Items"]
                    : ["EditorID", "Name", "Items"],
            },
            CrossFamilyTargets = new ClientAcceptanceCrossFamilyTargets
            {
                BookFormKey = fixture.BookFormKey.ToString(),
                KeywordFormKey = fixture.KeywordFormKey.ToString(),
            },
            Output = output,
            AuthoringPlan = CreateAuthoringPlan(workspaceOpen, reopenWorkspaceId, output, expected),
            Expected = expected,
        };
    }

    /// <summary>Reads a complete source-scoped inspector view before the generated fixture is disposed.</summary>
    /// <param name="fixture">The generated native fixture.</param>
    /// <param name="request">The explicit generated workspace request.</param>
    /// <param name="cancellationToken">The propagated operation token.</param>
    /// <returns>A detached complete source FormList JSON value.</returns>
    private static async Task<JsonElement> ReadSourceBaselineAsync(
        NativeWorkspaceIntegrationFixture fixture,
        WorkspaceOpenRequest request,
        CancellationToken cancellationToken)
    {
        await using var services = NativeEngineComposition.Create();
        var opened = await services.WorkspaceFactory.OpenAsync(request, cancellationToken);
        if (!opened.Succeeded || opened.Value is null)
        {
            throw new InvalidDataException($"Generated {fixture.Game} source workspace failed to open: {DescribeError(opened.Error)}");
        }

        await using var workspace = opened.Value;
        var read = await workspace.ReadFormListViewAsync(
            new ReferenceRequest(fixture.SourceListFormKey, RecordScope.Source, fixture.SourceModKey),
            cancellationToken);
        if (!read.Succeeded || read.Value?.Record is null)
        {
            throw new InvalidDataException($"Generated {fixture.Game} source baseline could not be read: {DescribeError(read.Error)}");
        }

        return read.Value.Record.Value.Clone();
    }

    /// <summary>Creates the requested new-record and source-override expectations for one game.</summary>
    /// <param name="game">The game whose supported fields are selected.</param>
    /// <param name="fixture">The generated identities used by item edits.</param>
    /// <returns>The exact saved-output expectations.</returns>
    private static ClientAcceptanceExpected CreateExpected(
        SupportedGame game,
        NativeWorkspaceIntegrationFixture fixture)
    {
        return new ClientAcceptanceExpected
        {
            OutputFormListCount = 2,
            NewRecord = new ClientAcceptanceExpectedRecord
            {
                FormKey = null,
                EditorId = $"CFClientAcceptance{game}New",
                Name = game == SupportedGame.Skyrim ? null : $"CreationsForge {game} client new",
                Items = [fixture.BookFormKey.ToString(), fixture.KeywordFormKey.ToString(), fixture.BookFormKey.ToString()],
            },
            OverrideRecord = new ClientAcceptanceExpectedRecord
            {
                FormKey = fixture.SourceListFormKey.ToString(),
                EditorId = $"CFClientAcceptance{game}Override",
                Name = game == SupportedGame.Skyrim ? null : $"CreationsForge {game} client override",
                Items = [fixture.KeywordFormKey.ToString(), fixture.BookFormKey.ToString(), fixture.KeywordFormKey.ToString()],
            },
        };
    }

    /// <summary>Builds the ordered domain-tool plan, including stable negative-probe and exact-replay identities.</summary>
    /// <param name="workspaceOpen">The exact first-run workspace-open request.</param>
    /// <param name="reopenWorkspaceId">The independent post-save workspace identity.</param>
    /// <param name="output">The selected output association.</param>
    /// <param name="expected">The intended record values.</param>
    /// <returns>Reusable tool parameters and receipt-sourced dynamic bindings in execution order.</returns>
    private static ClientAcceptancePlanStep[] CreateAuthoringPlan(
        ClientAcceptanceWorkspaceOpen workspaceOpen,
        Guid reopenWorkspaceId,
        ClientAcceptanceOutput output,
        ClientAcceptanceExpected expected)
    {
        var selectOperationId = Guid.NewGuid();
        var newBeginOperationId = Guid.NewGuid();
        var newEditorOperationId = Guid.NewGuid();
        var malformedApplyOperationId = Guid.NewGuid();
        var staleApplyOperationId = Guid.NewGuid();
        var newNameOperationId = Guid.NewGuid();
        var newItemsOperationId = Guid.NewGuid();
        var overrideBeginOperationId = Guid.NewGuid();
        var overrideEditorOperationId = Guid.NewGuid();
        var overrideNameOperationId = Guid.NewGuid();
        var overrideItemsOperationId = Guid.NewGuid();
        var saveOperationId = Guid.NewGuid();
        var reopenSelectOperationId = Guid.NewGuid();
        var steps = new List<ClientAcceptancePlanStep>
        {
            Step("select-output", "creationsforge_output_select", new
            {
                workspaceId = workspaceOpen.WorkspaceId,
                operationId = selectOperationId,
                mode = "create_new",
                output = new { pluginPath = output.PluginPath, modKey = output.ModKey, localizedOutputMode = output.LocalizedOutputMode, masterStyle = output.MasterStyle },
            }, ["expectedRevision <- workspace_open.revision"]),
            Step("begin-new", "creationsforge_formlist_begin_edit", new
            {
                workspaceId = workspaceOpen.WorkspaceId,
                operationId = newBeginOperationId,
                role = "new",
            }, ["expectedRevision <- select-output.revision"]),
            Replay("replay-begin-new", "creationsforge_formlist_begin_edit", "begin-new", new
            {
                workspaceId = workspaceOpen.WorkspaceId,
                operationId = newBeginOperationId,
                role = "new",
            }, ["expectedRevision <- begin-new original request expectedRevision"]),
            Apply("set-new-editor-id", workspaceOpen.WorkspaceId, newEditorOperationId, "form-list.set-editor-id", new { editorId = expected.NewRecord.EditorId }, "begin-new"),
            ReplayApply("replay-set-new-editor-id", "set-new-editor-id", workspaceOpen.WorkspaceId, newEditorOperationId, "form-list.set-editor-id", new { editorId = expected.NewRecord.EditorId }, "begin-new"),
            Step("reject-changed-payload-same-id", "creationsforge_formlist_apply_edit", new
            {
                workspaceId = workspaceOpen.WorkspaceId,
                operationId = newEditorOperationId,
                commandName = "form-list.set-editor-id",
                argumentsJson = JsonSerializer.Serialize(new { editorId = expected.NewRecord.EditorId + "Changed" }),
            }, ["expectedRevision <- set-new-editor-id original request expectedRevision", "editId <- begin-new.editId"], "error:operation_id_reuse"),
            Step("reject-malformed-apply", "creationsforge_formlist_apply_edit", new
            {
                workspaceId = workspaceOpen.WorkspaceId,
                operationId = malformedApplyOperationId,
                commandName = "form-list.replace-items",
                argumentsJson = "{",
            }, ["expectedRevision <- latest successful revision", "editId <- begin-new.editId"], "error:invalid_arguments"),
            Step("reject-stale-revision-apply", "creationsforge_formlist_apply_edit", new
            {
                workspaceId = workspaceOpen.WorkspaceId,
                operationId = staleApplyOperationId,
                commandName = "form-list.set-editor-id",
                argumentsJson = JsonSerializer.Serialize(new { editorId = expected.NewRecord.EditorId }),
            }, ["expectedRevision <- set-new-editor-id original request expectedRevision", "editId <- begin-new.editId"], "error:revision_conflict"),
        };

        if (expected.NewRecord.Name is not null)
        {
            steps.Add(ApplyName("set-new-name", workspaceOpen.WorkspaceId, newNameOperationId, workspaceOpen.Game, expected.NewRecord.Name, "begin-new"));
        }

        steps.Add(ApplyItems("set-new-items", workspaceOpen.WorkspaceId, newItemsOperationId, expected.NewRecord.Items, "begin-new"));
        steps.Add(Step("begin-override", "creationsforge_formlist_begin_edit", new
        {
            workspaceId = workspaceOpen.WorkspaceId,
            operationId = overrideBeginOperationId,
            role = "override",
            originFormKey = expected.OverrideRecord.FormKey,
            originSelection = new
            {
                formKey = expected.OverrideRecord.FormKey,
                scope = "source",
                containingModKey = ModKey.FromNameAndExtension(Path.GetFileName(workspaceOpen.SourcePluginPath)).ToString(),
            },
        }, ["expectedRevision <- latest successful revision"]));
        steps.Add(Apply("set-override-editor-id", workspaceOpen.WorkspaceId, overrideEditorOperationId, "form-list.set-editor-id", new { editorId = expected.OverrideRecord.EditorId }, "begin-override"));
        if (expected.OverrideRecord.Name is not null)
        {
            steps.Add(ApplyName("set-override-name", workspaceOpen.WorkspaceId, overrideNameOperationId, workspaceOpen.Game, expected.OverrideRecord.Name, "begin-override"));
        }

        steps.Add(ApplyItems("set-override-items", workspaceOpen.WorkspaceId, overrideItemsOperationId, expected.OverrideRecord.Items, "begin-override"));
        steps.Add(Step(
            "preview-staged-changes",
            "creationsforge_workspace_preview",
            new { workspaceId = workspaceOpen.WorkspaceId },
            ["expectedRevision <- latest successful revision"],
            "success:exactly-two-comparisons-for-expected-new-and-override-only"));
        steps.Add(Step("save", "creationsforge_save", new
        {
            workspaceId = workspaceOpen.WorkspaceId,
            operationId = saveOperationId,
        }, ["expectedRevision <- latest successful revision", "expectedOutputBaselineHandle <- select-output.baselineReference.handle"], "success:committed"));
        steps.Add(Replay("replay-save", "creationsforge_save", "save", new
        {
            workspaceId = workspaceOpen.WorkspaceId,
            operationId = saveOperationId,
        }, ["expectedRevision and expectedOutputBaselineHandle <- save original request"]));
        steps.Add(Step("close-first-workspace", "creationsforge_workspace_close", new { workspaceId = workspaceOpen.WorkspaceId }, []));
        steps.Add(Step("open-fresh-workspace", "creationsforge_workspace_open", new
        {
            workspaceId = reopenWorkspaceId,
            game = workspaceOpen.Game,
            release = workspaceOpen.Release,
            sourcePluginPath = workspaceOpen.SourcePluginPath,
            loadOrderPluginPaths = workspaceOpen.LoadOrderPluginPaths,
            dataDirectoryPath = workspaceOpen.DataDirectoryPath,
            stringDirectoryPaths = workspaceOpen.StringDirectoryPaths,
        }, []));
        steps.Add(Step("open-existing-output", "creationsforge_output_select", new
        {
            workspaceId = reopenWorkspaceId,
            operationId = reopenSelectOperationId,
            mode = "open_existing",
            output = new { pluginPath = output.PluginPath, modKey = output.ModKey, localizedOutputMode = output.LocalizedOutputMode, masterStyle = output.MasterStyle },
        }, ["expectedRevision <- open-fresh-workspace.revision"]));
        steps.Add(Step("list-saved-formlists", "creationsforge_formlists_list", new { workspaceId = reopenWorkspaceId, scope = "staged_output", maxResults = 10 }, []));
        steps.Add(Step("inspect-new", "creationsforge_formlist_inspect", new
        {
            workspaceId = reopenWorkspaceId,
            selection = new { scope = "staged_output", containingModKey = output.ModKey },
        }, ["selection.formKey <- unique list-saved-formlists item with expected new EditorID", "expectedRevision <- open-existing-output.revision"]));
        steps.Add(Step("inspect-override", "creationsforge_formlist_inspect", new
        {
            workspaceId = reopenWorkspaceId,
            selection = new { formKey = expected.OverrideRecord.FormKey, scope = "staged_output", containingModKey = output.ModKey },
        }, ["expectedRevision <- open-existing-output.revision"]));
        steps.Add(Step("preview-fresh-workspace", "creationsforge_workspace_preview", new { workspaceId = reopenWorkspaceId }, ["expectedRevision <- open-existing-output.revision"], "success:empty-comparisons"));
        steps.Add(Step("close-fresh-workspace", "creationsforge_workspace_close", new { workspaceId = reopenWorkspaceId }, []));
        return steps.ToArray();
    }

    /// <summary>Creates one common apply-edit plan step.</summary>
    /// <param name="name">The stable step name.</param>
    /// <param name="workspaceId">The first-run workspace identity.</param>
    /// <param name="operationId">The immutable idempotency identity.</param>
    /// <param name="commandName">The exact typed command discriminator.</param>
    /// <param name="arguments">The closed typed command arguments.</param>
    /// <param name="beginStep">The edit-producing step name.</param>
    /// <returns>A reusable apply-edit plan step.</returns>
    private static ClientAcceptancePlanStep Apply(string name, Guid workspaceId, Guid operationId, string commandName, object arguments, string beginStep)
    {
        return Step(name, "creationsforge_formlist_apply_edit", new
        {
            workspaceId,
            operationId,
            commandName,
            argumentsJson = JsonSerializer.Serialize(arguments),
        }, ["expectedRevision <- latest successful revision", $"editId <- {beginStep}.editId"]);
    }

    /// <summary>Creates one exact apply replay that retains the first request revision and edit identity.</summary>
    /// <param name="name">The replay step name.</param>
    /// <param name="originalStep">The original successful step name.</param>
    /// <param name="workspaceId">The workspace identity.</param>
    /// <param name="operationId">The original operation identity.</param>
    /// <param name="commandName">The original command name.</param>
    /// <param name="arguments">The original closed command arguments.</param>
    /// <param name="beginStep">The original edit-producing step.</param>
    /// <returns>An exact-replay plan step.</returns>
    private static ClientAcceptancePlanStep ReplayApply(string name, string originalStep, Guid workspaceId, Guid operationId, string commandName, object arguments, string beginStep)
    {
        return Replay(name, "creationsforge_formlist_apply_edit", originalStep, new
        {
            workspaceId,
            operationId,
            commandName,
            argumentsJson = JsonSerializer.Serialize(arguments),
        }, [$"expectedRevision and editId <- {originalStep} original request; editId originated at {beginStep}"]);
    }

    /// <summary>Creates a game-specific translated-name apply step.</summary>
    /// <param name="name">The stable step name.</param>
    /// <param name="workspaceId">The workspace identity.</param>
    /// <param name="operationId">The immutable operation identity.</param>
    /// <param name="game">The exact MCP game token used as the game-specific command prefix.</param>
    /// <param name="value">The exact English value.</param>
    /// <param name="beginStep">The edit-producing step.</param>
    /// <returns>A closed translated-name apply step.</returns>
    private static ClientAcceptancePlanStep ApplyName(string name, Guid workspaceId, Guid operationId, string game, string value, string beginStep)
    {
        return Apply(name, workspaceId, operationId, $"{game}.form-list.set-name", new
        {
            name = new
            {
                targetLanguage = "English",
                value,
                translations = new[] { new { language = "English", value } },
            },
        }, beginStep);
    }

    /// <summary>Creates a common ordered item-replacement apply step.</summary>
    /// <param name="name">The stable step name.</param>
    /// <param name="workspaceId">The workspace identity.</param>
    /// <param name="operationId">The immutable operation identity.</param>
    /// <param name="items">The exact item FormKeys.</param>
    /// <param name="beginStep">The edit-producing step.</param>
    /// <returns>A closed ordered item apply step.</returns>
    private static ClientAcceptancePlanStep ApplyItems(string name, Guid workspaceId, Guid operationId, IReadOnlyList<string> items, string beginStep)
    {
        return Apply(name, workspaceId, operationId, "form-list.replace-items", new
        {
            items = items.Select(formKey => new { isNull = false, formKey }).ToArray(),
        }, beginStep);
    }

    /// <summary>Creates one ordinary plan step with detached static JSON.</summary>
    /// <param name="name">The stable step name.</param>
    /// <param name="toolName">The exact domain-tool name.</param>
    /// <param name="staticArguments">Immutable request fields known at export time.</param>
    /// <param name="dynamicInputs">Receipt-derived fields supplied at execution time.</param>
    /// <param name="expectedResult">The required outcome classification.</param>
    /// <returns>A detached plan step.</returns>
    private static ClientAcceptancePlanStep Step(string name, string toolName, object staticArguments, string[] dynamicInputs, string expectedResult = "success")
    {
        return new ClientAcceptancePlanStep
        {
            Name = name,
            ToolName = toolName,
            StaticArguments = JsonSerializer.SerializeToElement(staticArguments),
            DynamicInputs = dynamicInputs,
            ExpectedResult = expectedResult,
        };
    }

    /// <summary>Creates an exact replay step whose request must retain the original dynamic values.</summary>
    /// <param name="name">The replay step name.</param>
    /// <param name="toolName">The exact domain-tool name.</param>
    /// <param name="originalStep">The earlier successful request to replay.</param>
    /// <param name="staticArguments">The original immutable static arguments.</param>
    /// <param name="dynamicInputs">Directions to preserve original receipt-derived inputs.</param>
    /// <returns>An exact replay plan step.</returns>
    private static ClientAcceptancePlanStep Replay(string name, string toolName, string originalStep, object staticArguments, string[] dynamicInputs)
    {
        var step = Step(name, toolName, staticArguments, dynamicInputs, "success:exact-replay");
        return new ClientAcceptancePlanStep
        {
            Name = step.Name,
            ToolName = step.ToolName,
            StaticArguments = step.StaticArguments,
            DynamicInputs = step.DynamicInputs,
            ExpectedResult = step.ExpectedResult,
            ExactReplayOf = originalStep,
        };
    }

    /// <summary>Creates a digest manifest entry for one copied immutable artifact.</summary>
    /// <param name="acceptanceRoot">The retained root used for relative paths.</param>
    /// <param name="path">The copied artifact path.</param>
    /// <param name="role">The stable artifact role.</param>
    /// <param name="loadOrderIndex">The plugin load-order index, when applicable.</param>
    /// <returns>The exact length and SHA-256 artifact entry.</returns>
    private static ClientAcceptanceArtifact CreateArtifact(string acceptanceRoot, string path, string role, int? loadOrderIndex)
    {
        using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        return new ClientAcceptanceArtifact
        {
            RelativePath = Path.GetRelativePath(acceptanceRoot, path).Replace('\\', '/'),
            Role = role,
            LoadOrderIndex = loadOrderIndex,
            Length = stream.Length,
            Sha256 = Convert.ToHexString(SHA256.HashData(stream)),
        };
    }

    /// <summary>Maps a supported game to its exact MCP token.</summary>
    /// <param name="game">The supported game.</param>
    /// <returns>The lowercase MCP game token.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the game is undefined.</exception>
    private static string GameToken(SupportedGame game)
    {
        return game switch
        {
            SupportedGame.Starfield => "starfield",
            SupportedGame.Fallout4 => "fallout4",
            SupportedGame.Skyrim => "skyrim",
            _ => throw new ArgumentOutOfRangeException(nameof(game), game, "Client acceptance requires a supported native game."),
        };
    }

    /// <summary>Maps a supported game to its exact MCP release token.</summary>
    /// <param name="game">The supported game.</param>
    /// <returns>The exact MCP release token.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the game is undefined.</exception>
    private static string ReleaseToken(SupportedGame game)
    {
        return game switch
        {
            SupportedGame.Starfield => "starfield",
            SupportedGame.Fallout4 => "fallout4",
            SupportedGame.Skyrim => "skyrim_se",
            _ => throw new ArgumentOutOfRangeException(nameof(game), game, "Client acceptance requires a supported native game."),
        };
    }

    /// <summary>Formats an optional engine error without claiming success.</summary>
    /// <param name="error">The optional engine error.</param>
    /// <returns>A stable diagnostic string.</returns>
    private static string DescribeError(EngineError? error)
    {
        return error is null ? "no engine error was supplied" : $"{error.Code}: {error.Message}";
    }
}
