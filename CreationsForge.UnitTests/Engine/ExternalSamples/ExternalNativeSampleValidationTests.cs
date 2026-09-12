namespace CreationsForge.UnitTests.Engine.ExternalSamples;

/// <summary>Defines the serial collection for resource-intensive opt-in external native sample checks.</summary>
[CollectionDefinition("External Native Sample Validation", DisableParallelization = true)]
public sealed class ExternalNativeSampleValidationCollection
{
    /// <summary>Initializes the marker collection used only by xUnit discovery.</summary>
    public ExternalNativeSampleValidationCollection()
    { }

    /// <summary>The stable xUnit collection name.</summary>
    public const string Name = "External Native Sample Validation";
}

/// <summary>Exposes separate metadata-only and native-execution entry points for an environment-selected external sample.</summary>
[Collection(ExternalNativeSampleValidationCollection.Name)]
[Trait("Category", "ExternalNativeSample")]
public sealed class ExternalNativeSampleValidationTests
{
    /// <summary>Initializes the stateless opt-in external native sample test owner.</summary>
    public ExternalNativeSampleValidationTests()
    { }

    /// <summary>Writes a retained metadata-only report after validating headers, master closure, localization inputs, and anticipated bytes.</summary>
    /// <returns>A task that completes after the compact report is retained under a fresh task-owned run root.</returns>
    [Fact]
    public async Task Preflight_ReportsMetadataWithoutOpeningPlugins()
    {
        var manifest = LoadOrSkip();
        var report = ExternalNativeSamplePreflight.Inspect(manifest);
        Assert.False(report.NativeRecordParsingPerformed);
        Assert.False(report.ContentHashingPerformed);
        var runDirectory = ExternalNativeSamplePreflight.CreateRunDirectory(manifest, "preflight");
        await ExternalNativeSamplePreflight.WriteReportAsync(
            runDirectory,
            "preflight-report.json",
            report,
            TestContext.Current.CancellationToken);
    }

    /// <summary>Runs requested output modes serially through save, fresh engine reopen, direct game-native verification, and source-preservation checks.</summary>
    /// <returns>A task that completes after retained execution evidence is written.</returns>
    [Fact]
    public async Task Execution_PreservesExternalFormListsAcrossNativeSaveAndFreshReopen()
    {
        await ExternalNativeSampleRunner.RunAsync(LoadOrSkip());
    }

    /// <summary>Loads the sole process-local manifest source and skips only when the opt-in variable is absent.</summary>
    /// <returns>The validated manifest.</returns>
    private static ExternalNativeSampleManifest LoadOrSkip()
    {
        var manifest = ExternalNativeSampleManifest.LoadFromEnvironment();
        if (manifest is null)
        {
            Assert.Skip(
                $"Set {ExternalNativeSampleManifest.EnvironmentVariableName} to an explicit manifest path to opt into external native sample validation.");
        }

        return manifest!;
    }
}
