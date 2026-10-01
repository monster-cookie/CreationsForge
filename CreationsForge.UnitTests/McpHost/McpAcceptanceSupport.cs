using System.Diagnostics;
using System.Text.Json.Nodes;
using CreationsForge.Mcp.Protocol;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using StarfieldKeyword = Mutagen.Bethesda.Starfield.Keyword;
using StarfieldMod = Mutagen.Bethesda.Starfield.StarfieldMod;
using StarfieldRelease = Mutagen.Bethesda.Starfield.StarfieldRelease;

namespace CreationsForge.UnitTests.McpHost;

internal static class McpHostPaths
{
    public static string RepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "CreationsForge.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException(
            "Could not locate the CreationsForge repository root from the test output directory.");
    }

    public static string Configuration()
    {
        return new DirectoryInfo(AppContext.BaseDirectory).Parent?.Name
            ?? throw new InvalidOperationException("Could not determine the active build configuration.");
    }

    public static string McpAssembly()
    {
        return Path.Combine(
            RepositoryRoot(),
            "CreationsForge.Mcp",
            "bin",
            Configuration(),
            "net10.0",
            "CreationsForge.Mcp.dll");
    }

    public static string LockProbeAssembly()
    {
        return Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "..",
            "..",
            "..",
            "LockProbe",
            "bin",
            Configuration(),
            "net10.0",
            "CreationsForge.LockProbe.dll"));
    }
}

internal sealed class TestDirectory : IDisposable
{
    public TestDirectory()
    {
        DirectoryPath = Path.Combine(Path.GetTempPath(), "creationsforge-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(DirectoryPath);
    }

    public string DirectoryPath { get; }

    public void Dispose()
    {
        try
        {
            if (Directory.Exists(DirectoryPath))
            {
                Directory.Delete(DirectoryPath, recursive: true);
            }
        }
        catch (IOException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
    }
}

internal static class McpPluginFixtures
{
    public const string StarfieldMaster = "Starfield.esm";

    public const string SourcePlugin = "Source.esm";

    public const string SelectedPlugin = "Selected.esp";

    public const string OutputPlugin = "Output.esp";

    public const string BusyOutput = "CrossProcess.esp";

    public const string SourceEditorId = "SourceKeyword";

    public static void WriteStarfieldMaster(string directory)
    {
        var modKey = ModKey.FromNameAndExtension(StarfieldMaster);
        WritePlugin(new StarfieldMod(modKey, StarfieldRelease.Starfield), Path.Combine(directory, StarfieldMaster));
    }

    public static void WriteSourceKeyword(string directory)
    {
        WriteStarfieldMaster(directory);
        var masterKey = ModKey.FromNameAndExtension(StarfieldMaster);
        var sourceKey = ModKey.FromNameAndExtension(SourcePlugin);
        var source = new StarfieldMod(sourceKey, StarfieldRelease.Starfield);
        ((IMod)source).MasterReferences.Add(new MasterReference { Master = masterKey });
        var keyword = new StarfieldKeyword(source.GetNextFormKey(), StarfieldRelease.Starfield)
        {
            EditorID = SourceEditorId,
        };
        source.Keywords.Add(keyword);
        var master = new StarfieldMod(masterKey, StarfieldRelease.Starfield);
        WritePlugin(source, Path.Combine(directory, SourcePlugin), master);
    }

    public static void WriteMissingMasterSelection(string directory)
    {
        WriteStarfieldMaster(directory);
        var selectedKey = ModKey.FromNameAndExtension(SelectedPlugin);
        var selected = new StarfieldMod(selectedKey, StarfieldRelease.Starfield);
        var missingKey = ModKey.FromNameAndExtension("Missing.esm");
        ((IMod)selected).MasterReferences.Add(new MasterReference { Master = missingKey });
        var missing = new StarfieldMod(missingKey, StarfieldRelease.Starfield);
        WritePlugin(selected, Path.Combine(directory, SelectedPlugin), missing);
    }

    public static void WriteUnreadableSelection(string directory)
    {
        WriteStarfieldMaster(directory);
        File.WriteAllBytes(Path.Combine(directory, SelectedPlugin), [0x00, 0x01, 0x02, 0x03]);
    }

    public static IReadOnlyList<string?> ReadKeywordEditorIds(string directory)
    {
        var starfieldPath = Path.Combine(directory, StarfieldMaster);
        var sourcePath = Path.Combine(directory, SourcePlugin);
        var outputPath = Path.Combine(directory, OutputPlugin);
        using var starfield = StarfieldMod.Create(StarfieldRelease.Starfield)
            .FromPath(starfieldPath)
            .WithNoLoadOrder()
            .Construct();
        using var source = StarfieldMod.Create(StarfieldRelease.Starfield)
            .FromPath(sourcePath)
            .WithKnownMasters(starfield)
            .Construct();
        using var output = StarfieldMod.Create(StarfieldRelease.Starfield)
            .FromPath(outputPath)
            .WithKnownMasters(starfield, source)
            .Construct();
        return output.Keywords.Select(keyword => keyword.EditorID).ToArray();
    }

    private static void WritePlugin(IModGetter plugin, string path, params IModMasterStyledGetter[] masters)
    {
        var masterFlags = new Cache<IModMasterStyledGetter, ModKey>(
            master => master.ModKey,
            EqualityComparer<ModKey>.Default);
        foreach (var master in masters)
        {
            masterFlags.Add(master);
        }

        plugin.WriteToBinary(path, new BinaryWriteParameters
        {
            MastersListContent = MastersListContentOption.NoCheck,
            MasterFlagsLookup = masterFlags,
        });
    }
}

internal sealed class CrossProcessLock : IAsyncDisposable
{
    private readonly Process _process;
    private readonly Task<string> _standardOutput;
    private readonly Task<string> _standardError;
    private bool _released;

    private CrossProcessLock(Process process, Task<string> standardOutput, Task<string> standardError)
    {
        _process = process;
        _standardOutput = standardOutput;
        _standardError = standardError;
    }

    public static async Task<CrossProcessLock> AcquireAsync(string dataDirectory, CancellationToken cancellationToken)
    {
        var probePath = McpHostPaths.LockProbeAssembly();
        Assert.True(File.Exists(probePath), $"The lock probe was not found at '{probePath}'.");
        var readyPath = Path.Combine(dataDirectory, "holder.ready");
        var startInfo = new ProcessStartInfo("dotnet")
        {
            WorkingDirectory = Path.GetDirectoryName(probePath)!,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };
        startInfo.ArgumentList.Add(probePath);
        startInfo.ArgumentList.Add(dataDirectory);
        startInfo.ArgumentList.Add(readyPath);
        var process = Process.Start(startInfo) ?? throw new InvalidOperationException("Could not start the cross-process lock holder.");
        var holder = new CrossProcessLock(
            process,
            process.StandardOutput.ReadToEndAsync(cancellationToken),
            process.StandardError.ReadToEndAsync(cancellationToken));
        using var readyTimeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        readyTimeout.CancelAfter(TimeSpan.FromSeconds(20));
        while (!File.Exists(readyPath) && !process.HasExited)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(50), readyTimeout.Token).ConfigureAwait(false);
        }

        if (process.HasExited)
        {
            var output = await holder._standardOutput.ConfigureAwait(false);
            var error = await holder._standardError.ConfigureAwait(false);
            throw new InvalidOperationException($"The cross-process lock holder exited early. Output: {output} Error: {error}");
        }

        return holder;
    }

    public async Task ReleaseAsync(CancellationToken cancellationToken)
    {
        if (_released)
        {
            return;
        }

        _released = true;
        if (!_process.HasExited)
        {
            await _process.StandardInput.WriteLineAsync().ConfigureAwait(false);
            _process.StandardInput.Close();
            await _process.WaitForExitAsync(cancellationToken).ConfigureAwait(false);
        }

        var output = await _standardOutput.WaitAsync(cancellationToken).ConfigureAwait(false);
        var error = await _standardError.WaitAsync(cancellationToken).ConfigureAwait(false);
        Assert.True(
            _process.ExitCode == 0,
            $"The cross-process lock probe failed. Output: {output} Error: {error}");
    }

    public async ValueTask DisposeAsync()
    {
        if (!_released && !_process.HasExited)
        {
            try
            {
                _process.Kill(entireProcessTree: true);
                await _process.WaitForExitAsync().ConfigureAwait(false);
            }
            catch (InvalidOperationException)
            {
            }
        }

        _process.Dispose();
    }
}

internal sealed class McpJson
{
    private readonly JsonObject _node;

    public McpJson(JsonObject node)
    {
        _node = node;
    }

    public string RequiredString(string name)
    {
        return OptionalString(name) ?? throw new InvalidOperationException($"Missing string '{name}' in {this}.");
    }

    public bool RequiredBool(string name)
    {
        var node = Property(name) ?? throw new InvalidOperationException($"Missing bool '{name}' in {this}.");
        return node.GetValue<bool>();
    }

    public ulong RequiredUInt64(string name)
    {
        var node = Property(name) ?? throw new InvalidOperationException($"Missing revision '{name}' in {this}.");
        if (node is JsonValue value && value.TryGetValue<ulong>(out var revision))
        {
            return revision;
        }

        return node.GetValue<long>() switch
        {
            < 0 => throw new InvalidOperationException($"Negative revision '{name}' in {this}."),
            var parsed => (ulong)parsed,
        };
    }

    public string? OptionalString(string name)
    {
        var node = Property(name);
        if (node is null)
        {
            return null;
        }

        return node.GetValue<string?>();
    }

    public McpJson Object(string name)
    {
        return Property(name) as JsonObject is { } child
            ? new McpJson(child)
            : throw new InvalidOperationException($"Missing object '{name}' in {this}.");
    }

    public IReadOnlyList<McpJson> Array(string name)
    {
        if (Property(name) is not JsonArray array)
        {
            throw new InvalidOperationException($"Missing array '{name}' in {this}.");
        }

        return array.OfType<JsonObject>().Select(item => new McpJson(item)).ToArray();
    }

    public IReadOnlyList<string> Strings(string name)
    {
        if (Property(name) is not JsonArray array)
        {
            throw new InvalidOperationException($"Missing array '{name}' in {this}.");
        }

        return array.Select(item => item?.GetValue<string>() ?? string.Empty).ToArray();
    }

    public override string ToString()
    {
        return _node.ToJsonString();
    }

    private JsonNode? Property(string name)
    {
        return _node.TryGetPropertyValue(name, out var value) ? value : null;
    }
}

internal sealed class McpCallResult
{
    private McpCallResult(
        bool isError,
        int? protocolErrorCode,
        string? protocolErrorMessage,
        McpJson? structured,
        string raw)
    {
        IsError = isError;
        ProtocolErrorCode = protocolErrorCode;
        ProtocolErrorMessage = protocolErrorMessage;
        Structured = structured;
        Raw = raw;
    }

    public bool IsError { get; }

    public int? ProtocolErrorCode { get; }

    public string? ProtocolErrorMessage { get; }

    public McpJson? Structured { get; }

    public string Raw { get; }

    public static McpCallResult FromRpc(JsonObject message)
    {
        var raw = message.ToJsonString();
        if (message["error"] is JsonObject error)
        {
            int? code = error["code"] is JsonValue codeValue && codeValue.TryGetValue<int>(out var parsed)
                ? parsed
                : null;
            return new McpCallResult(true, code, error["message"]?.GetValue<string>(), null, raw);
        }

        if (message["result"] is not JsonObject result)
        {
            throw new InvalidOperationException($"MCP response had no result: {raw}");
        }

        var isError = result["isError"] is JsonValue errorFlag && errorFlag.TryGetValue<bool>(out var flag) && flag;
        return new McpCallResult(isError, null, null, ReadStructured(result), raw);
    }

    public McpJson RequireSuccess()
    {
        Assert.False(IsError, Describe());
        return Structured ?? throw new InvalidOperationException($"Successful MCP call had no structured content: {Raw}");
    }

    public string ErrorCode()
    {
        Assert.True(IsError, Describe());
        if (Structured is not null)
        {
            return Structured.RequiredString("code");
        }

        return ProtocolErrorMessage ?? ProtocolErrorCode?.ToString() ?? Raw;
    }

    public string Describe()
    {
        return Raw;
    }

    private static McpJson? ReadStructured(JsonObject result)
    {
        if (!result.TryGetPropertyValue("structuredContent", out var node) || node is null)
        {
            return ReadTextJson(result);
        }

        if (node is JsonObject obj)
        {
            return new McpJson(obj);
        }

        if (node is JsonValue value && value.TryGetValue<string>(out var text) && !string.IsNullOrWhiteSpace(text))
        {
            return JsonNode.Parse(text) is JsonObject parsed ? new McpJson(parsed) : null;
        }

        return null;
    }

    private static McpJson? ReadTextJson(JsonObject result)
    {
        if (result["content"] is not JsonArray content)
        {
            return null;
        }

        foreach (var block in content.OfType<JsonObject>())
        {
            var text = block["text"]?.GetValue<string>();
            if (string.IsNullOrWhiteSpace(text))
            {
                continue;
            }

            try
            {
                if (JsonNode.Parse(text) is JsonObject parsed)
                {
                    return new McpJson(parsed);
                }
            }
            catch (System.Text.Json.JsonException)
            {
            }
        }

        return null;
    }
}

internal static class McpRequests
{
    public static JsonNode Revision(ulong revision)
    {
        return JsonValue.Create(revision) ?? throw new InvalidOperationException("Could not encode a revision.");
    }

    public static JsonObject WorkspaceOpen(string operationId, string release, string dataDirectory, params string[] plugins)
    {
        var selected = new JsonArray();
        foreach (var plugin in plugins)
        {
            selected.Add(plugin);
        }

        return new JsonObject
        {
            ["operationId"] = operationId,
            ["release"] = release,
            ["dataDirectory"] = dataDirectory,
            ["selectedPlugins"] = selected,
        };
    }

    public static JsonObject OutputCreate(string operationId, string workspaceId, string outputPath)
    {
        return new JsonObject
        {
            ["operationId"] = operationId,
            ["workspaceId"] = workspaceId,
            ["outputPath"] = outputPath,
            ["masterStyle"] = "Full",
            ["textStorageMode"] = "Embedded",
        };
    }

    public static JsonObject OutputOpen(string operationId, string workspaceId, string outputPath)
    {
        var request = OutputCreate(operationId, workspaceId, outputPath);
        return request;
    }

    public static JsonObject EditorIdChange(string editorId)
    {
        return new JsonObject
        {
            ["path"] = "EditorID",
            ["operation"] = "Set",
            ["value"] = new JsonObject
            {
                ["kind"] = "String",
                ["string"] = editorId,
            },
        };
    }

    public static JsonObject RecordCreate(
        string operationId,
        string workspaceId,
        ulong expectedRevision,
        string editorId)
    {
        return new JsonObject
        {
            ["operationId"] = operationId,
            ["workspaceId"] = workspaceId,
            ["familyId"] = "Keyword",
            ["expectedRevision"] = Revision(expectedRevision),
            ["changes"] = new JsonArray(EditorIdChange(editorId)),
        };
    }

    public static JsonObject RecordOverride(
        string operationId,
        string workspaceId,
        string formKey,
        string containingModKey,
        ulong expectedRevision,
        string editorId)
    {
        return new JsonObject
        {
            ["operationId"] = operationId,
            ["workspaceId"] = workspaceId,
            ["familyId"] = "Keyword",
            ["formKey"] = formKey,
            ["containingModKey"] = containingModKey,
            ["expectedRevision"] = Revision(expectedRevision),
            ["changes"] = new JsonArray(EditorIdChange(editorId)),
        };
    }

    public static JsonObject RecordApply(
        string operationId,
        string workspaceId,
        ulong expectedRevision,
        string formKey,
        string containingModKey,
        string editorId)
    {
        return new JsonObject
        {
            ["operationId"] = operationId,
            ["workspaceId"] = workspaceId,
            ["expectedRevision"] = Revision(expectedRevision),
            ["mutations"] = new JsonArray(new JsonObject
            {
                ["kind"] = "Override",
                ["familyId"] = "Keyword",
                ["formKey"] = formKey,
                ["containingModKey"] = containingModKey,
                ["changes"] = new JsonArray(EditorIdChange(editorId)),
            }),
        };
    }

    public static async Task<IReadOnlyList<McpJson>> SearchAllAsync(
        McpStdioClient client,
        string workspaceId,
        string? editorId,
        CancellationToken cancellationToken)
    {
        var records = new List<McpJson>();
        string? cursor = null;
        for (var page = 0; page < 10; page++)
        {
            var arguments = new JsonObject
            {
                ["workspaceId"] = workspaceId,
                ["familyId"] = "Keyword",
            };
            if (!string.IsNullOrWhiteSpace(editorId))
            {
                arguments["editorId"] = editorId;
            }

            if (cursor is not null)
            {
                arguments["cursor"] = cursor;
            }

            var result = (await client.CallAsync(McpAuthoringContract.RecordsSearchTool, arguments, cancellationToken)
                .ConfigureAwait(false)).RequireSuccess();
            records.AddRange(result.Array("records"));
            cursor = result.OptionalString("nextCursor");
            if (string.IsNullOrWhiteSpace(cursor))
            {
                return records;
            }
        }

        throw new InvalidOperationException("Search paging did not finish.");
    }

    public static async Task<string?> ReadEditorIdAsync(
        McpStdioClient client,
        string workspaceId,
        string formKey,
        string containingModKey,
        CancellationToken cancellationToken)
    {
        string? cursor = null;
        for (var page = 0; page < 20; page++)
        {
            var arguments = new JsonObject
            {
                ["workspaceId"] = workspaceId,
                ["familyId"] = "Keyword",
                ["formKey"] = formKey,
                ["containingModKey"] = containingModKey,
            };
            if (cursor is not null)
            {
                arguments["cursor"] = cursor;
            }

            var record = (await client.CallAsync(McpAuthoringContract.RecordReadTool, arguments, cancellationToken)
                .ConfigureAwait(false)).RequireSuccess().Object("record");
            foreach (var field in record.Array("fields"))
            {
                if (field.RequiredString("path") == "EditorID")
                {
                    return field.Object("value").OptionalString("string");
                }
            }

            cursor = record.OptionalString("nextFieldCursor");
            if (string.IsNullOrWhiteSpace(cursor))
            {
                return null;
            }
        }

        throw new InvalidOperationException("Record field paging did not finish.");
    }
}
