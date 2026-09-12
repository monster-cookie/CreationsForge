using System.Diagnostics;
using System.Text.Json;
using CreationsForge.Mcp;
using CreationsForge.TestSupport;
using Shouldly;

namespace CreationsForge.UnitTests.McpHost;

/// <summary>
/// Verifies the real SDK client and stdio process boundary of the CreationsForge MCP host.
/// </summary>
public sealed class McpHostProtocolTests
{
    /// <summary>Verifies initialization, exact discovery, structured invocation, and structured argument rejection.</summary>
    [Fact]
    public async Task StdioHost_InitializesDiscoversAndInvokesBoundedTool()
    {
        using var timeoutSource = CancellationTokenSource.CreateLinkedTokenSource(TestContext.Current.CancellationToken);
        timeoutSource.CancelAfter(TimeSpan.FromSeconds(30));
        var cancellationToken = timeoutSource.Token;
        await using var processFixture = await McpStdioProcessFixture.StartAsync(
            typeof(McpHostRunner).Assembly.Location,
            cancellationToken);
        var client = processFixture.Client;

        var tools = await client.ListToolsAsync(cancellationToken: cancellationToken);

        tools.Count.ShouldBe(22);
        tools.Select(candidate => candidate.Name).Distinct(StringComparer.Ordinal).Count().ShouldBe(22);
        var tool = tools.Single(candidate => candidate.Name == "creationsforge_server_info");
        tool.JsonSchema.GetProperty("type").GetString().ShouldBe("object");
        tool.JsonSchema.GetProperty("additionalProperties").GetBoolean().ShouldBeFalse();
        tool.JsonSchema.GetProperty("properties").EnumerateObject().ShouldBeEmpty();

        var result = await client.CallToolAsync(
            tool.Name,
            new Dictionary<string, object?>(),
            cancellationToken: cancellationToken);

        result.IsError.ShouldNotBe(true);
        result.StructuredContent.ShouldNotBeNull();
        var structuredContent = result.StructuredContent.Value;
        structuredContent.GetProperty("ok").GetBoolean().ShouldBeTrue();
        var serverResult = structuredContent.GetProperty("result");
        serverResult.GetProperty("serverName").GetString().ShouldBe("CreationsForge");
        serverResult.GetProperty("transport").GetString().ShouldBe("stdio");
        var lifecycle = serverResult.GetProperty("lifecycle");
        lifecycle.GetProperty("state").GetString().ShouldBe("running");
        lifecycle.GetProperty("activeWorkspaceCount").GetInt32().ShouldBe(0);
        lifecycle.GetProperty("activeWorkspaceIds").GetArrayLength().ShouldBe(0);
        serverResult.GetProperty("capabilities").GetArrayLength().ShouldBe(23);
        var outputSchema = await GetOutputSchemaAsync();
        MatchesSchema(outputSchema, structuredContent).ShouldBeTrue();

        var rejectedResult = await client.CallToolAsync(
            tool.Name,
            new Dictionary<string, object?>
            {
                ["unknown"] = true,
            },
            cancellationToken: cancellationToken);

        rejectedResult.IsError.ShouldBe(true);
        rejectedResult.StructuredContent.ShouldNotBeNull();
        var rejectedContent = rejectedResult.StructuredContent.Value;
        var error = rejectedContent.GetProperty("error");
        error.GetProperty("code").GetString().ShouldBe("invalid_arguments");
        MatchesSchema(outputSchema, rejectedContent).ShouldBeTrue();

        var completion = await processFixture.CompleteAsync(cancellationToken);
        completion.ExitCode.ShouldBe(0, completion.StandardError);
    }

    /// <summary>Verifies the explicit output schema and all read-only/idempotent tool annotations.</summary>
    [Fact]
    public async Task ServerInfoTool_AdvertisesClosedSchemasAndSafeAnnotations()
    {
        await using var registry = new McpWorkspaceRegistry();
        var tool = new ServerInfoTool(registry, "1.2.3-test");

        tool.ProtocolTool.InputSchema.GetProperty("additionalProperties").GetBoolean().ShouldBeFalse();
        tool.ProtocolTool.OutputSchema.ShouldNotBeNull();
        var outputSchema = tool.ProtocolTool.OutputSchema.Value;
        var alternatives = outputSchema.GetProperty("oneOf").EnumerateArray().ToArray();
        alternatives.Length.ShouldBe(2);
        alternatives.ShouldAllBe(alternative => AllObjectSchemasAreClosed(alternative));
        tool.ProtocolTool.Annotations.ShouldNotBeNull();
        tool.ProtocolTool.Annotations.ReadOnlyHint.ShouldBe(true);
        tool.ProtocolTool.Annotations.IdempotentHint.ShouldBe(true);
        tool.ProtocolTool.Annotations.DestructiveHint.ShouldBe(false);
        tool.ProtocolTool.Annotations.OpenWorldHint.ShouldBe(false);
    }

    /// <summary>Verifies that closing standard input exits successfully without contaminating protocol output.</summary>
    [Fact]
    public async Task StdioHost_WhenInputCloses_ExitsZeroWithPristineStandardOutput()
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };
        startInfo.ArgumentList.Add(typeof(McpHostRunner).Assembly.Location);

        using var process = new Process
        {
            StartInfo = startInfo,
        };
        Task<string>? standardOutputTask = null;
        Task<string>? standardErrorTask = null;
        var started = false;
        try
        {
            started = process.Start();
            started.ShouldBeTrue();
            standardOutputTask = process.StandardOutput.ReadToEndAsync();
            standardErrorTask = process.StandardError.ReadToEndAsync();
            process.StandardInput.Close();
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(15));

            await process.WaitForExitAsync(timeout.Token);
            var standardOutput = await standardOutputTask.WaitAsync(timeout.Token);
            var standardError = await standardErrorTask.WaitAsync(timeout.Token);

            process.ExitCode.ShouldBe(0, standardError);
            standardOutput.ShouldBeEmpty();
            standardError.ShouldNotContain("database", Case.Insensitive);
            standardError.ShouldNotContain("migration", Case.Insensitive);
        }
        finally
        {
            if (started)
            {
                try
                {
                    process.StandardInput.Close();
                }
                catch (IOException)
                {
                }
                catch (ObjectDisposedException)
                {
                }

                if (!process.HasExited)
                {
                    try
                    {
                        process.Kill(entireProcessTree: true);
                    }
                    catch (InvalidOperationException) when (process.HasExited)
                    {
                    }
                }

                using var cleanupSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                if (!process.HasExited)
                {
                    await process.WaitForExitAsync(cleanupSource.Token);
                }

                if (standardOutputTask is not null && standardErrorTask is not null)
                {
                    await Task.WhenAll(standardOutputTask, standardErrorTask).WaitAsync(cleanupSource.Token);
                }
            }
        }
    }

    /// <summary>Verifies the dedicated MCP executable rejects command-line arguments before starting its stdio host.</summary>
    [Fact]
    public async Task McpExecutable_WithCommandLineArgument_ExitsTwoWithoutProtocolOutput()
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };
        startInfo.ArgumentList.Add(typeof(McpHostRunner).Assembly.Location);
        startInfo.ArgumentList.Add("unexpected");

        using var process = Process.Start(startInfo).ShouldNotBeNull();
        var standardOutputTask = process.StandardOutput.ReadToEndAsync();
        var standardErrorTask = process.StandardError.ReadToEndAsync();
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(15));
        await process.WaitForExitAsync(timeout.Token);

        process.ExitCode.ShouldBe(2);
        (await standardOutputTask.WaitAsync(timeout.Token)).ShouldBeEmpty();
        (await standardErrorTask.WaitAsync(timeout.Token)).ShouldContain("does not accept command-line arguments");
    }

    /// <summary>Gets the exact output schema advertised by the production server information tool.</summary>
    /// <returns>The detached output schema element.</returns>
    private static async ValueTask<JsonElement> GetOutputSchemaAsync()
    {
        await using var registry = new McpWorkspaceRegistry();
        return new ServerInfoTool(registry, "schema-test", domainToolsAvailable: true, authoringToolsAvailable: true).ProtocolTool.OutputSchema!.Value;
    }

    /// <summary>Validates an instance against the closed JSON Schema subset used by the MCP tool catalog.</summary>
    /// <param name="schema">The advertised schema or schema branch.</param>
    /// <param name="instance">The structured content to validate.</param>
    /// <returns><see langword="true"/> when the instance satisfies exactly one alternative and every declared constraint.</returns>
    private static bool MatchesSchema(JsonElement schema, JsonElement instance)
    {
        if (schema.TryGetProperty("oneOf", out var alternatives))
        {
            return alternatives.EnumerateArray().Count(alternative => MatchesSchema(alternative, instance)) == 1;
        }

        if (schema.TryGetProperty("const", out var constant)
            && constant.GetRawText() != instance.GetRawText())
        {
            return false;
        }

        if (schema.TryGetProperty("enum", out var allowedValues)
            && !allowedValues.EnumerateArray().Any(value => value.GetRawText() == instance.GetRawText()))
        {
            return false;
        }

        if (schema.TryGetProperty("type", out var type)
            && !MatchesType(type.GetString(), instance.ValueKind))
        {
            return false;
        }

        if (schema.TryGetProperty("type", out type)
            && type.GetString() == "integer"
            && !instance.TryGetInt64(out _))
        {
            return false;
        }

        if (instance.ValueKind == JsonValueKind.Object)
        {
            if (!MatchesObjectSchema(schema, instance))
            {
                return false;
            }
        }

        if (instance.ValueKind == JsonValueKind.Array
            && schema.TryGetProperty("items", out var itemSchema))
        {
            var items = instance.EnumerateArray().ToArray();
            if (items.Any(item => !MatchesSchema(itemSchema, item)))
            {
                return false;
            }

            if (schema.TryGetProperty("uniqueItems", out var uniqueItems)
                && uniqueItems.GetBoolean()
                && items.Select(item => item.GetRawText()).Distinct(StringComparer.Ordinal).Count() != items.Length)
            {
                return false;
            }
        }

        if (instance.ValueKind == JsonValueKind.String
            && schema.TryGetProperty("minLength", out var minimumLength)
            && instance.GetString()!.Length < minimumLength.GetInt32())
        {
            return false;
        }

        if (instance.ValueKind == JsonValueKind.String
            && schema.TryGetProperty("format", out var format)
            && format.GetString() == "uuid"
            && !Guid.TryParseExact(instance.GetString(), "D", out _))
        {
            return false;
        }

        if (instance.ValueKind == JsonValueKind.Number
            && schema.TryGetProperty("minimum", out var minimum)
            && instance.GetDecimal() < minimum.GetDecimal())
        {
            return false;
        }

        return true;
    }

    /// <summary>Checks that every object schema in one alternative explicitly rejects undeclared properties.</summary>
    /// <param name="schema">The schema node to inspect recursively.</param>
    /// <returns><see langword="true"/> when every reachable object schema is closed.</returns>
    private static bool AllObjectSchemasAreClosed(JsonElement schema)
    {
        if (schema.TryGetProperty("type", out var type)
            && type.GetString() == "object")
        {
            if (!schema.TryGetProperty("additionalProperties", out var additionalProperties)
                || additionalProperties.GetBoolean())
            {
                return false;
            }

            if (schema.TryGetProperty("properties", out var properties)
                && properties.EnumerateObject().Any(property => !AllObjectSchemasAreClosed(property.Value)))
            {
                return false;
            }
        }

        if (schema.TryGetProperty("items", out var items)
            && !AllObjectSchemasAreClosed(items))
        {
            return false;
        }

        if (schema.TryGetProperty("oneOf", out var alternatives)
            && alternatives.EnumerateArray().Any(alternative => !AllObjectSchemasAreClosed(alternative)))
        {
            return false;
        }

        return true;
    }

    /// <summary>Validates required, declared, and closed properties for one object instance.</summary>
    /// <param name="schema">The object schema.</param>
    /// <param name="instance">The object instance.</param>
    /// <returns><see langword="true"/> when the object satisfies its declared property constraints.</returns>
    private static bool MatchesObjectSchema(JsonElement schema, JsonElement instance)
    {
        if (!schema.TryGetProperty("properties", out var properties))
        {
            return true;
        }

        if (schema.TryGetProperty("required", out var required)
            && required.EnumerateArray().Any(name => !instance.TryGetProperty(name.GetString()!, out _)))
        {
            return false;
        }

        foreach (var property in instance.EnumerateObject())
        {
            if (!properties.TryGetProperty(property.Name, out var propertySchema))
            {
                return !schema.TryGetProperty("additionalProperties", out var additionalProperties)
                    || additionalProperties.GetBoolean();
            }

            if (!MatchesSchema(propertySchema, property.Value))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>Checks one JSON value kind against a schema primitive type name.</summary>
    /// <param name="type">The schema type name.</param>
    /// <param name="valueKind">The structured value kind.</param>
    /// <returns><see langword="true"/> when the primitive type matches.</returns>
    private static bool MatchesType(string? type, JsonValueKind valueKind)
    {
        return type switch
        {
            "array" => valueKind == JsonValueKind.Array,
            "boolean" => valueKind is JsonValueKind.True or JsonValueKind.False,
            "integer" => valueKind == JsonValueKind.Number,
            "object" => valueKind == JsonValueKind.Object,
            "string" => valueKind == JsonValueKind.String,
            _ => false,
        };
    }
}
