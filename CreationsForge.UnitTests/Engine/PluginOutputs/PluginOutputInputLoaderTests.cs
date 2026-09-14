using System.Runtime.InteropServices;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.PluginInputs;
using CreationsForge.Core.Engine.PluginOutputs;
using CreationsForge.UnitTests.Engine.Starfield;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Starfield;
using Mutagen.Bethesda.Strings;
using Shouldly;
using System.IO.Abstractions;

namespace CreationsForge.UnitTests.Engine.PluginOutputs;

/// <summary>Verifies bounded plugin output admission independently of game-specific mutable output state.</summary>
public sealed class PluginOutputInputLoaderTests
{
    /// <summary>CreateNew records an absent complete output set without creating the plugin or sibling Strings directory.</summary>
    /// <returns>A task that completes after output admission and baseline verification.</returns>
    [Fact]
    public async Task PrepareAsync_CreateNew_CapturesAbsenceWithoutCreatingOutputArtifacts()
    {
        using var fixture = StarfieldPluginTestFixture.Create();
        await using var sources = await PrepareSourcesAsync(fixture);
        var outputDirectory = fixture.RootDirectory.CreateSubdirectory("NewOutput");
        var outputPath = Path.Combine(outputDirectory.FullName, "Created.esp");
        var association = CreateAssociation(outputPath, "Created.esp", LocalizedOutputMode.SeparateStringFiles);

        var result = await new PluginOutputInputLoader().PrepareAsync(
            sources,
            association,
            OutputSelectionMode.CreateNew,
            TestContext.Current.CancellationToken);

        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        await using var inputs = result.Value!;
        inputs.Release.ShouldBe(GameRelease.Starfield);
        inputs.Output.Path.ShouldBe(Path.GetFullPath(outputPath));
        inputs.Output.ModKey.ShouldBe(association.ModKey);
        inputs.Output.Exists.ShouldBeFalse();
        inputs.Output.UsesLocalization.ShouldBeTrue();
        Should.Throw<InvalidOperationException>(() => inputs.OpenReadStream(TestContext.Current.CancellationToken));

        var baseline = await inputs.CompleteOpenAsync(TestContext.Current.CancellationToken);
        baseline.Succeeded.ShouldBeTrue(baseline.Error?.Message);
        baseline.Value!.Artifacts.Count.ShouldBeGreaterThan(1);
        baseline.Value.Artifacts.ShouldAllBe(artifact => !artifact.Fingerprint.Exists);
        File.Exists(outputPath).ShouldBeFalse();
        Directory.Exists(Path.Combine(outputDirectory.FullName, "Strings")).ShouldBeFalse();
    }

    /// <summary>Output selection enforces explicit present-or-absent intent and requires an existing parent directory.</summary>
    /// <returns>A task that completes after all invalid selection modes are rejected.</returns>
    [Fact]
    public async Task PrepareAsync_EnforcesSelectionIntentAndExistingParentDirectory()
    {
        using var fixture = StarfieldPluginTestFixture.Create();
        await using var sources = await PrepareSourcesAsync(fixture);
        var outputDirectory = fixture.RootDirectory.CreateSubdirectory("SelectionIntent");
        var existingPath = WriteOutput(outputDirectory, "Existing.esp", localized: false);
        var existing = CreateAssociation(existingPath, "Existing.esp", LocalizedOutputMode.Embedded);

        var createExisting = await new PluginOutputInputLoader().PrepareAsync(
            sources,
            existing,
            OutputSelectionMode.CreateNew,
            TestContext.Current.CancellationToken);
        createExisting.Succeeded.ShouldBeFalse();
        createExisting.Error!.Code.ShouldBe(EngineErrorCode.InvalidRequest);

        var absentPath = Path.Combine(outputDirectory.FullName, "Absent.esp");
        var absent = CreateAssociation(absentPath, "Absent.esp", LocalizedOutputMode.Embedded);
        var openAbsent = await new PluginOutputInputLoader().PrepareAsync(
            sources,
            absent,
            OutputSelectionMode.OpenExisting,
            TestContext.Current.CancellationToken);
        openAbsent.Succeeded.ShouldBeFalse();
        openAbsent.Error!.Code.ShouldBe(EngineErrorCode.InvalidRequest);

        var missingParentPath = Path.Combine(fixture.RootDirectory.FullName, "Missing", "Created.esp");
        var missingParent = CreateAssociation(missingParentPath, "Created.esp", LocalizedOutputMode.Embedded);
        var missingDirectory = await new PluginOutputInputLoader().PrepareAsync(
            sources,
            missingParent,
            OutputSelectionMode.CreateNew,
            TestContext.Current.CancellationToken);
        missingDirectory.Succeeded.ShouldBeFalse();
        missingDirectory.Error!.Code.ShouldBe(EngineErrorCode.OutputOpenFailed);
        Directory.Exists(Path.GetDirectoryName(missingParentPath)).ShouldBeFalse();
    }

    /// <summary>CreateNew rejects a directory that obstructs the requested plugin file path.</summary>
    /// <returns>A task that completes after wrong-entry-kind validation.</returns>
    [Fact]
    public async Task PrepareAsync_WithDirectoryAtPluginPath_ReturnsInvalidRequest()
    {
        using var fixture = StarfieldPluginTestFixture.Create();
        await using var sources = await PrepareSourcesAsync(fixture);
        var outputDirectory = fixture.RootDirectory.CreateSubdirectory("PluginObstruction");
        var outputPath = Path.Combine(outputDirectory.FullName, "Obstructed.esp");
        Directory.CreateDirectory(outputPath);
        var association = CreateAssociation(outputPath, "Obstructed.esp", LocalizedOutputMode.Embedded);

        var result = await new PluginOutputInputLoader().PrepareAsync(
            sources,
            association,
            OutputSelectionMode.CreateNew,
            TestContext.Current.CancellationToken);

        result.Succeeded.ShouldBeFalse();
        result.Error!.Code.ShouldBe(EngineErrorCode.InvalidRequest);
        result.Error.Message.ShouldContain("directory instead of a file");
    }

    /// <summary>CreateNew rejects a regular file that obstructs the derived sibling Strings directory.</summary>
    /// <returns>A task that completes after wrong-entry-kind validation.</returns>
    [Fact]
    public async Task PrepareAsync_WithFileAtStringsDirectoryPath_ReturnsInvalidRequest()
    {
        using var fixture = StarfieldPluginTestFixture.Create();
        await using var sources = await PrepareSourcesAsync(fixture);
        var outputDirectory = fixture.RootDirectory.CreateSubdirectory("StringsObstruction");
        var outputPath = Path.Combine(outputDirectory.FullName, "Obstructed.esp");
        await File.WriteAllTextAsync(
            Path.Combine(outputDirectory.FullName, "Strings"),
            "not a directory",
            TestContext.Current.CancellationToken);
        var association = CreateAssociation(outputPath, "Obstructed.esp", LocalizedOutputMode.Embedded);

        var result = await new PluginOutputInputLoader().PrepareAsync(
            sources,
            association,
            OutputSelectionMode.CreateNew,
            TestContext.Current.CancellationToken);

        result.Succeeded.ShouldBeFalse();
        result.Error!.Code.ShouldBe(EngineErrorCode.InvalidRequest);
        result.Error.Message.ShouldContain("file instead of a directory");
    }

    /// <summary>An output ModKey already present anywhere in the explicit source load order is rejected.</summary>
    /// <returns>A task that completes after collision validation.</returns>
    [Fact]
    public async Task PrepareAsync_WithSourceModKeyCollision_ReturnsInvalidRequest()
    {
        using var fixture = StarfieldPluginTestFixture.Create();
        await using var sources = await PrepareSourcesAsync(fixture);
        var outputDirectory = fixture.RootDirectory.CreateSubdirectory("Collision");
        var outputPath = Path.Combine(outputDirectory.FullName, fixture.FullModKey.FileName.ToString());
        var association = new OutputAssociation(
            outputPath,
            fixture.FullModKey,
            LocalizedOutputMode.Embedded,
            OutputMasterStyle.Full);

        var result = await new PluginOutputInputLoader().PrepareAsync(
            sources,
            association,
            OutputSelectionMode.CreateNew,
            TestContext.Current.CancellationToken);

        result.Succeeded.ShouldBeFalse();
        result.Error!.Code.ShouldBe(EngineErrorCode.InvalidRequest);
        result.Error.Message.ShouldContain("already present");
    }

    /// <summary>An existing output whose plugin header disagrees with its requested association is rejected.</summary>
    /// <returns>A task that completes after plugin header validation.</returns>
    [Fact]
    public async Task PrepareAsync_WithHeaderAssociationMismatch_ReturnsInvalidRequest()
    {
        using var fixture = StarfieldPluginTestFixture.Create();
        await using var sources = await PrepareSourcesAsync(fixture);
        var outputDirectory = fixture.RootDirectory.CreateSubdirectory("HeaderMismatch");
        var outputPath = WriteOutput(outputDirectory, "Mismatch.esp", localized: false);
        var association = CreateAssociation(
            outputPath,
            "Mismatch.esp",
            LocalizedOutputMode.SeparateStringFiles);

        var result = await new PluginOutputInputLoader().PrepareAsync(
            sources,
            association,
            OutputSelectionMode.OpenExisting,
            TestContext.Current.CancellationToken);

        result.Succeeded.ShouldBeFalse();
        result.Error!.Code.ShouldBe(EngineErrorCode.InvalidRequest);
        result.Error.Message.ShouldContain("localization state");
    }

    /// <summary>Unused loose sidecars remain part of an embedded output baseline and later drift is detected.</summary>
    /// <returns>A task that completes after drift verification.</returns>
    [Fact]
    public async Task VerifyUnchangedAsync_WithUnusedSidecarChange_ReturnsExternalChange()
    {
        using var fixture = StarfieldPluginTestFixture.Create();
        await using var sources = await PrepareSourcesAsync(fixture);
        var outputDirectory = fixture.RootDirectory.CreateSubdirectory("UnusedSidecar");
        var outputPath = WriteOutput(outputDirectory, "Unused.esp", localized: false);
        var stringsDirectory = outputDirectory.CreateSubdirectory("Strings");
        var unusedPath = Path.Combine(stringsDirectory.FullName, "Unused_en.STRINGS");
        await File.WriteAllBytesAsync(unusedPath, [1, 2, 3], TestContext.Current.CancellationToken);
        var association = CreateAssociation(outputPath, "Unused.esp", LocalizedOutputMode.Embedded);

        var result = await new PluginOutputInputLoader().PrepareAsync(
            sources,
            association,
            OutputSelectionMode.OpenExisting,
            TestContext.Current.CancellationToken);
        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        await using var inputs = result.Value!;
        var completed = await inputs.CompleteOpenAsync(TestContext.Current.CancellationToken);
        completed.Succeeded.ShouldBeTrue(completed.Error?.Message);
        completed.Value!.Artifacts.Single(artifact =>
            string.Equals(artifact.Path, unusedPath, StringComparison.OrdinalIgnoreCase)).Fingerprint.Exists.ShouldBeTrue();

        await File.AppendAllTextAsync(unusedPath, "changed", TestContext.Current.CancellationToken);
        var verification = await inputs.VerifyUnchangedAsync(TestContext.Current.CancellationToken);
        verification.Succeeded.ShouldBeFalse();
        verification.Error!.Code.ShouldBe(EngineErrorCode.ExternalChangeDetected);
    }

    /// <summary>An existing localized output opens only through explicit loose sidecars and preserves strict parsing metadata.</summary>
    /// <returns>A task that completes after parsing and baseline completion.</returns>
    [Fact]
    public async Task PrepareAsync_WithExistingLocalizedLooseOutput_ProvidesStrictReadScope()
    {
        using var fixture = StarfieldPluginTestFixture.Create();
        await using var sources = await PrepareSourcesAsync(fixture);
        var outputDirectory = fixture.RootDirectory.CreateSubdirectory("LocalizedOutput");
        var outputPath = WriteOutput(outputDirectory, "Localized.esp", localized: true);
        var association = CreateAssociation(
            outputPath,
            "Localized.esp",
            LocalizedOutputMode.SeparateStringFiles);

        var result = await new PluginOutputInputLoader().PrepareAsync(
            sources,
            association,
            OutputSelectionMode.OpenExisting,
            TestContext.Current.CancellationToken);

        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        await using var inputs = result.Value!;
        inputs.Output.Exists.ShouldBeTrue();
        inputs.Output.UsesLocalization.ShouldBeTrue();
        inputs.CreateParsingMeta().StringsLookup.ShouldNotBeNull();
        using (var stream = inputs.OpenReadStream(TestContext.Current.CancellationToken))
        {
            stream.Length.ShouldBeGreaterThan(0);
        }

        var completed = await inputs.CompleteOpenAsync(TestContext.Current.CancellationToken);
        completed.Succeeded.ShouldBeTrue(completed.Error?.Message);
    }

    /// <summary>An archive-only localized output is rejected when no explicit loose sidecar can be preserved.</summary>
    /// <returns>A task that completes after the preservation boundary is enforced.</returns>
    [Fact]
    public async Task PrepareAsync_WithLocalizedOutputWithoutLooseSidecar_ReturnsUnsupportedInput()
    {
        using var fixture = StarfieldPluginTestFixture.Create();
        await using var sources = await PrepareSourcesAsync(fixture);
        var outputDirectory = fixture.RootDirectory.CreateSubdirectory("ArchiveOnly");
        var outputPath = WriteOutput(outputDirectory, "ArchiveOnly.esp", localized: true, writeStrings: false);
        var association = CreateAssociation(
            outputPath,
            "ArchiveOnly.esp",
            LocalizedOutputMode.SeparateStringFiles);

        var result = await new PluginOutputInputLoader().PrepareAsync(
            sources,
            association,
            OutputSelectionMode.OpenExisting,
            TestContext.Current.CancellationToken);

        result.Succeeded.ShouldBeFalse();
        result.Error!.Code.ShouldBe(EngineErrorCode.UnsupportedInput);
        result.Error.Message.ShouldContain("no explicit loose strings sidecar");
    }

    /// <summary>Disposing the output parsing scope invalidates it without disposing the borrowed plugin sources.</summary>
    /// <returns>A task that completes after cancellation and ownership checks.</returns>
    [Fact]
    public async Task OutputScope_WithCancellationAndDisposal_DoesNotOwnBorrowedSources()
    {
        using var fixture = StarfieldPluginTestFixture.Create();
        await using var sources = await PrepareSourcesAsync(fixture);
        var outputDirectory = fixture.RootDirectory.CreateSubdirectory("Lifecycle");
        var outputPath = Path.Combine(outputDirectory.FullName, "Lifecycle.esp");
        var association = CreateAssociation(outputPath, "Lifecycle.esp", LocalizedOutputMode.Embedded);
        using var canceledSource = new CancellationTokenSource();
        canceledSource.Cancel();

        await Should.ThrowAsync<OperationCanceledException>(() => new PluginOutputInputLoader().PrepareAsync(
            sources,
            association,
            OutputSelectionMode.CreateNew,
            canceledSource.Token));

        var result = await new PluginOutputInputLoader().PrepareAsync(
            sources,
            association,
            OutputSelectionMode.CreateNew,
            TestContext.Current.CancellationToken);
        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        var inputs = result.Value!;
        await inputs.DisposeAsync();
        Should.Throw<ObjectDisposedException>(() => inputs.CreateParsingMeta());

        var sourceVerification = await sources.VerifyUnchangedAsync(TestContext.Current.CancellationToken);
        sourceVerification.Succeeded.ShouldBeTrue(sourceVerification.Error?.Message);
    }

    /// <summary>A hard-linked existing output that physically aliases an admitted source is still rejected.</summary>
    /// <returns>A task that completes after the Windows physical-alias check, or immediately on other platforms.</returns>
    [Fact]
    public async Task PrepareAsync_WithHardLinkedSourceAlias_ReturnsInvalidRequestOnWindows()
    {
        if (!OperatingSystem.IsWindows())
        {
            return;
        }

        using var fixture = StarfieldPluginTestFixture.Create();
        var outputDirectory = fixture.RootDirectory.CreateSubdirectory("HardLink");
        var outputPath = Path.Combine(outputDirectory.FullName, "Alias.esp");
        if (!CreateHardLink(outputPath, fixture.SourcePluginPath, IntPtr.Zero))
        {
            throw new InvalidOperationException($"Could not create the hard-link fixture: {Marshal.GetLastPInvokeError()}.");
        }

        await using var sources = await PrepareSourcesAsync(fixture);
        var association = new OutputAssociation(
            outputPath,
            new ModKey("Alias", ModType.Plugin),
            LocalizedOutputMode.SeparateStringFiles,
            OutputMasterStyle.Small);

        var result = await new PluginOutputInputLoader().PrepareAsync(
            sources,
            association,
            OutputSelectionMode.OpenExisting,
            TestContext.Current.CancellationToken);

        result.Succeeded.ShouldBeFalse();
        result.Error!.Code.ShouldBe(EngineErrorCode.InvalidRequest);
        result.Error.Message.ShouldContain("aliases another admitted plugin artifact");
    }

    /// <summary>Prepares and completes the generated plugin source input lifetime used by output tests.</summary>
    /// <param name="fixture">The generated Starfield source fixture.</param>
    /// <returns>The prepared source input lifetime.</returns>
    private static async Task<PluginSourceInputs> PrepareSourcesAsync(StarfieldPluginTestFixture fixture)
    {
        var prepared = await new PluginSourceInputLoader().PrepareAsync(
            fixture.CreateOpenRequest(),
            TestContext.Current.CancellationToken);
        prepared.Succeeded.ShouldBeTrue(prepared.Error?.Message);
        var inputs = prepared.Value!;
        var completed = await inputs.CompleteOpenAsync(TestContext.Current.CancellationToken);
        if (!completed.Succeeded)
        {
            await inputs.DisposeAsync();
        }

        completed.Succeeded.ShouldBeTrue(completed.Error?.Message);
        return inputs;
    }

    /// <summary>Creates one full-master output association.</summary>
    /// <param name="path">The output plugin path.</param>
    /// <param name="fileName">The output plugin file name.</param>
    /// <param name="localizedOutputMode">The requested localization mode.</param>
    /// <returns>The output association.</returns>
    private static OutputAssociation CreateAssociation(
        string path,
        string fileName,
        LocalizedOutputMode localizedOutputMode)
    {
        return new OutputAssociation(
            path,
            ModKey.FromNameAndExtension(fileName),
            localizedOutputMode,
            OutputMasterStyle.Full);
    }

    /// <summary>Writes one generated full-master Starfield output plugin and optional loose strings files.</summary>
    /// <param name="outputDirectory">The existing output parent directory.</param>
    /// <param name="fileName">The output plugin file name.</param>
    /// <param name="localized">Whether the output header selects separate localized strings.</param>
    /// <param name="writeStrings">Whether localized values are written to the sibling Strings directory.</param>
    /// <returns>The written output plugin path.</returns>
    private static string WriteOutput(
        DirectoryInfo outputDirectory,
        string fileName,
        bool localized,
        bool writeStrings = true)
    {
        var mod = new StarfieldMod(ModKey.FromNameAndExtension(fileName), StarfieldRelease.Starfield)
        {
            UsingLocalization = localized
        };
        var formList = new FormList(mod, "OutputList")
        {
            Name = new TranslatedString(Language.English, "Output list")
        };
        mod.FormLists.Add(formList);
        var outputPath = Path.Combine(outputDirectory.FullName, fileName);
        if (localized)
        {
            var stringsDirectory = outputDirectory.CreateSubdirectory("Strings");
            using (var stringsWriter = new StringsWriter(
                       GameRelease.Starfield,
                       mod.ModKey,
                       stringsDirectory.FullName,
                       new StarfieldTestEncodingProvider(),
                       new FileSystem()))
            {
                ((IModGetter)mod).WriteToBinary(
                    outputPath,
                    BinaryWriteParameters.Default with { StringsWriter = stringsWriter });
            }

            if (!writeStrings)
            {
                stringsDirectory.Delete(true);
            }
        }
        else
        {
            ((IModGetter)mod).WriteToBinary(outputPath, BinaryWriteParameters.Default);
        }

        return outputPath;
    }

    /// <summary>Creates a Windows hard link for physical-alias validation.</summary>
    /// <param name="fileName">The new hard-link path.</param>
    /// <param name="existingFileName">The existing source file path.</param>
    /// <param name="securityAttributes">Reserved security attributes, always zero.</param>
    /// <returns><see langword="true"/> when Windows creates the link.</returns>
    [DllImport("kernel32.dll", EntryPoint = "CreateHardLinkW", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool CreateHardLink(
        string fileName,
        string existingFileName,
        IntPtr securityAttributes);
}
