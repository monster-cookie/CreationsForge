using System.Diagnostics;
using CreationsForge.Engine;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Fallout4Mod = Mutagen.Bethesda.Fallout4.Fallout4Mod;
using Fallout4Release = Mutagen.Bethesda.Fallout4.Fallout4Release;
using SkyrimCell = Mutagen.Bethesda.Skyrim.Cell;
using SkyrimCellBlock = Mutagen.Bethesda.Skyrim.CellBlock;
using SkyrimCellGetter = Mutagen.Bethesda.Skyrim.ICellGetter;
using SkyrimCellSubBlock = Mutagen.Bethesda.Skyrim.CellSubBlock;
using SkyrimMod = Mutagen.Bethesda.Skyrim.SkyrimMod;
using SkyrimPlacedObject = Mutagen.Bethesda.Skyrim.PlacedObject;
using SkyrimPlacedObjectGetter = Mutagen.Bethesda.Skyrim.IPlacedObjectGetter;
using SkyrimRelease = Mutagen.Bethesda.Skyrim.SkyrimRelease;

namespace CreationsForge.UnitTests.Engine;

/// <summary>Verifies native source resolution, output ownership, and cleanup for every supported game.</summary>
public sealed partial class NativeWorkspaceTests
{
    /// <summary>Opens selected plugins with only their recursive masters and resolves exact and winning contexts.</summary>
    [Theory]
    [InlineData(GameRelease.Starfield)]
    [InlineData(GameRelease.Fallout4)]
    [InlineData(GameRelease.SkyrimSE)]
    public void OpensExactRecursiveSourceClosureForEveryGame(GameRelease release)
    {
        using var directory = new TemporaryDirectory();
        var fixture = CreateFixture(directory.Path, release);
        var unrelatedModKey = ModKey.FromNameAndExtension("Unrelated.esp");
        WriteEmptyPlugin(Path.Combine(directory.Path, unrelatedModKey.ToString()), unrelatedModKey, release);
        var sourceSnapshots = new[] { fixture.MasterModKey, fixture.SelectedModKey }
            .ToDictionary(
                modKey => modKey,
                modKey => File.ReadAllBytes(Path.Combine(directory.Path, modKey.ToString())));

        var outputModKey = ModKey.FromNameAndExtension("Output.esp");
        var request = CreateNewRequest(directory.Path, release, [fixture.SelectedModKey], outputModKey);
        using var workspace = CreateFactory().Open(request);

        Assert.Equal([fixture.MasterModKey, fixture.SelectedModKey], workspace.Sources.Select(source => source.ModKey));
        Assert.DoesNotContain(workspace.Sources, source => source.ModKey == unrelatedModKey);
        Assert.DoesNotContain(
            workspace.Sources,
            source => File.Exists(Path.Combine(directory.Path, source.ModKey + ".creationsforge.lock")));
        foreach (var sourceSnapshot in sourceSnapshots)
        {
            Assert.Equal(
                sourceSnapshot.Value,
                File.ReadAllBytes(Path.Combine(directory.Path, sourceSnapshot.Key.ToString())));
        }

        var resolution = workspace.ResolveRecord(fixture.FormKey, fixture.RecordGetterType, fixture.MasterModKey);
        Assert.Equal(fixture.FormKey, resolution.OriginFormKey);
        Assert.Equal(fixture.MasterModKey, resolution.ContainingModKey);
        Assert.Equal(fixture.SelectedModKey, resolution.WinningModKey);
        Assert.Equal(fixture.MasterModKey, resolution.ExactContext.ModKey);
        Assert.Equal(fixture.SelectedModKey, resolution.WinningContext.ModKey);

        var browsedContexts = workspace.BrowseWinningRecords(fixture.RecordGetterType);
        var browsedContext = Assert.Single(browsedContexts);
        Assert.Equal(fixture.SelectedModKey, browsedContext.ModKey);
    }

    /// <summary>Creates a dirty empty output and applies each game's required initial masters.</summary>
    [Theory]
    [InlineData(GameRelease.Starfield)]
    [InlineData(GameRelease.Fallout4)]
    [InlineData(GameRelease.SkyrimSE)]
    public void NewOutputHasExplicitConstantTimeState(GameRelease release)
    {
        using var directory = new TemporaryDirectory();
        if (release == GameRelease.Starfield)
        {
            var starfieldModKey = ModKey.FromNameAndExtension("Starfield.esm");
            WriteEmptyPlugin(Path.Combine(directory.Path, starfieldModKey.ToString()), starfieldModKey, release);
        }

        var outputModKey = ModKey.FromNameAndExtension("Output.esp");
        using var workspace = CreateFactory().Open(CreateNewRequest(directory.Path, release, [], outputModKey));

        Assert.True(workspace.State.IsNewOutput);
        Assert.True(workspace.State.IsDirty);
        Assert.Equal(0UL, workspace.State.Revision);
        Assert.Equal(outputModKey, workspace.State.OutputModKey);
        Assert.Equal(MasterStyle.Full, workspace.State.MasterStyle);
        Assert.Equal(NativeTextStorageMode.Embedded, workspace.State.TextStorageMode);
        Assert.Equal(
            release == GameRelease.Starfield ? [ModKey.FromNameAndExtension("Starfield.esm")] : [],
            workspace.Output.MasterReferences.Select(reference => reference.Master));
        Assert.False(File.Exists(Path.Combine(directory.Path, outputModKey.ToString())));
    }

    /// <summary>Loads an existing output as a complete mutable plugin without initially marking it dirty.</summary>
    [Theory]
    [InlineData(GameRelease.Starfield)]
    [InlineData(GameRelease.Fallout4)]
    [InlineData(GameRelease.SkyrimSE)]
    public void ExistingOutputIsMutableAndInitiallyClean(GameRelease release)
    {
        using var directory = new TemporaryDirectory();
        var fixture = CreateFixture(directory.Path, release);
        var outputModKey = fixture.SelectedModKey;
        var outputPath = Path.Combine(directory.Path, outputModKey.ToString());
        var definition = new NativeOutputDefinition(
            outputPath,
            outputModKey,
            MasterStyle.Full,
            NativeTextStorageMode.Embedded,
            createNew: false);
        var request = new NativeWorkspaceOpenRequest(release, directory.Path, [], definition);

        using var workspace = CreateFactory().Open(request);

        Assert.False(workspace.State.IsNewOutput);
        Assert.False(workspace.State.IsDirty);
        Assert.Equal(0UL, workspace.State.Revision);
        Assert.Equal(outputModKey, workspace.Output.ModKey);
        Assert.Equal([fixture.MasterModKey], workspace.Sources.Select(source => source.ModKey));
        Assert.True(workspace.Output.GetRecordCount() > 0);
    }

    /// <summary>Reports the exact missing recursive master instead of scanning or silently skipping it.</summary>
    [Fact]
    public void MissingRecursiveMasterIsActionable()
    {
        using var directory = new TemporaryDirectory();
        var selectedModKey = ModKey.FromNameAndExtension("Selected.esp");
        var missingModKey = ModKey.FromNameAndExtension("Missing.esm");
        var selected = new Fallout4Mod(selectedModKey, Fallout4Release.Fallout4);
        ((IMod)selected).MasterReferences.Add(new MasterReference { Master = missingModKey });
        WritePlugin(selected, Path.Combine(directory.Path, selectedModKey.ToString()));

        var outputModKey = ModKey.FromNameAndExtension("Output.esp");
        var exception = Assert.Throws<NativeWorkspaceException>(() =>
            CreateFactory().Open(CreateNewRequest(directory.Path, GameRelease.Fallout4, [selectedModKey], outputModKey)));

        Assert.Contains(missingModKey.ToString(), exception.Message, StringComparison.Ordinal);
        Assert.Contains(directory.Path, exception.Message, StringComparison.Ordinal);
    }

    /// <summary>Rejects cyclic master graphs with the complete identity chain.</summary>
    [Fact]
    public void CyclicMasterGraphIsActionable()
    {
        using var directory = new TemporaryDirectory();
        var firstModKey = ModKey.FromNameAndExtension("First.esm");
        var secondModKey = ModKey.FromNameAndExtension("Second.esm");
        var first = new Fallout4Mod(firstModKey, Fallout4Release.Fallout4);
        ((IMod)first).MasterReferences.Add(new MasterReference { Master = secondModKey });
        WritePlugin(first, Path.Combine(directory.Path, firstModKey.ToString()));
        var second = new Fallout4Mod(secondModKey, Fallout4Release.Fallout4);
        ((IMod)second).MasterReferences.Add(new MasterReference { Master = firstModKey });
        WritePlugin(second, Path.Combine(directory.Path, secondModKey.ToString()));

        var outputModKey = ModKey.FromNameAndExtension("Output.esp");
        var exception = Assert.Throws<NativeWorkspaceException>(() =>
            CreateFactory().Open(CreateNewRequest(directory.Path, GameRelease.Fallout4, [firstModKey], outputModKey)));

        Assert.Contains($"{firstModKey} -> {secondModKey} -> {firstModKey}", exception.Message, StringComparison.Ordinal);
    }

    /// <summary>Rejects competing output ownership immediately and releases ownership on close.</summary>
    [Fact]
    public void OutputOwnershipFailsPromptlyAndReleasesOnClose()
    {
        using var directory = new TemporaryDirectory();
        var outputModKey = ModKey.FromNameAndExtension("Output.esp");
        var request = CreateNewRequest(directory.Path, GameRelease.Fallout4, [], outputModKey);
        var factory = CreateFactory();
        using var firstWorkspace = factory.Open(request);

        var started = DateTime.UtcNow;
        var exception = Assert.Throws<NativeWorkspaceLockException>(() => factory.Open(request));
        Assert.True(DateTime.UtcNow - started < TimeSpan.FromSeconds(2));
        Assert.Contains("output identity", exception.Message, StringComparison.OrdinalIgnoreCase);

        firstWorkspace.Dispose();
        using var reopened = factory.Open(request);
        Assert.Equal(outputModKey, reopened.Output.ModKey);
    }

    /// <summary>Prevents a source held for reading from being reopened as somebody else's mutable output.</summary>
    [Fact]
    public void SourceOwnershipBlocksWritersAndReleasesOnClose()
    {
        using var directory = new TemporaryDirectory();
        var sourceModKey = ModKey.FromNameAndExtension("Base.esm");
        WriteEmptyPlugin(Path.Combine(directory.Path, sourceModKey.ToString()), sourceModKey, GameRelease.Fallout4);
        var browsingOutputModKey = ModKey.FromNameAndExtension("BrowsingOutput.esp");
        var factory = CreateFactory();
        using var browsingWorkspace = factory.Open(
            CreateNewRequest(directory.Path, GameRelease.Fallout4, [sourceModKey], browsingOutputModKey));
        var writerDefinition = new NativeOutputDefinition(
            Path.Combine(directory.Path, sourceModKey.ToString()),
            sourceModKey,
            MasterStyle.Full,
            NativeTextStorageMode.Embedded,
            createNew: false);
        var writerRequest = new NativeWorkspaceOpenRequest(GameRelease.Fallout4, directory.Path, [], writerDefinition);

        var exception = Assert.Throws<NativeWorkspaceLockException>(() => factory.Open(writerRequest));
        Assert.Contains("output plugin", exception.Message, StringComparison.OrdinalIgnoreCase);

        browsingWorkspace.Dispose();
        using var reopenedForWriting = factory.Open(writerRequest);
        Assert.Equal(sourceModKey, reopenedForWriting.Output.ModKey);
    }

    /// <summary>Exercises the real operating-system ownership primitive across process boundaries.</summary>
    [Fact]
    public async Task OutputOwnershipIsExclusiveAcrossProcesses()
    {
        using var directory = new TemporaryDirectory();
        var readyPath = Path.Combine(directory.Path, "holder.ready");
        var configuration = Directory.GetParent(Path.TrimEndingDirectorySeparator(AppContext.BaseDirectory))!.Name;
        var probePath = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "..",
            "..",
            "..",
            "LockProbe",
            "bin",
            configuration,
            "net10.0",
            "CreationsForge.LockProbe.dll"));
        var startInfo = new ProcessStartInfo("dotnet")
        {
            WorkingDirectory = Path.GetDirectoryName(probePath)!,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };
        startInfo.ArgumentList.Add(probePath);
        startInfo.ArgumentList.Add(directory.Path);
        startInfo.ArgumentList.Add(readyPath);

        using var holder = Process.Start(startInfo) ?? throw new InvalidOperationException("Could not start the cross-process lock holder.");
        var standardOutput = holder.StandardOutput.ReadToEndAsync(TestContext.Current.CancellationToken);
        var standardError = holder.StandardError.ReadToEndAsync(TestContext.Current.CancellationToken);
        try
        {
            using var readyTimeout = CancellationTokenSource.CreateLinkedTokenSource(TestContext.Current.CancellationToken);
            readyTimeout.CancelAfter(TimeSpan.FromSeconds(15));
            while (!File.Exists(readyPath) && !holder.HasExited)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(50), readyTimeout.Token);
            }

            if (holder.HasExited)
            {
                Assert.Fail($"The cross-process lock holder exited early. Output: {await standardOutput} Error: {await standardError}");
            }

            var outputModKey = ModKey.FromNameAndExtension("CrossProcess.esp");
            var exception = Assert.Throws<NativeWorkspaceLockException>(() =>
                CreateFactory().Open(CreateNewRequest(directory.Path, GameRelease.Fallout4, [], outputModKey)));
            Assert.Contains("output identity", exception.Message, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            if (!holder.HasExited)
            {
                await holder.StandardInput.WriteLineAsync();
                holder.StandardInput.Close();
            }

            using var exitTimeout = CancellationTokenSource.CreateLinkedTokenSource(TestContext.Current.CancellationToken);
            exitTimeout.CancelAfter(TimeSpan.FromSeconds(10));
            try
            {
                await holder.WaitForExitAsync(exitTimeout.Token);
            }
            catch (OperationCanceledException) when (!TestContext.Current.CancellationToken.IsCancellationRequested)
            {
                holder.Kill(entireProcessTree: true);
                await holder.WaitForExitAsync(TestContext.Current.CancellationToken);
            }
        }

        Assert.True(
            holder.ExitCode == 0,
            $"The cross-process lock probe failed. Output: {await standardOutput} Error: {await standardError}");
    }

    /// <summary>Rejects output styles that are not legal for the selected game or extension.</summary>
    [Fact]
    public void RejectsIllegalOutputStyle()
    {
        using var directory = new TemporaryDirectory();
        var outputModKey = ModKey.FromNameAndExtension("Output.esl");
        var output = new NativeOutputDefinition(
            Path.Combine(directory.Path, outputModKey.ToString()),
            outputModKey,
            MasterStyle.Full,
            NativeTextStorageMode.Embedded,
            createNew: true);
        var request = new NativeWorkspaceOpenRequest(GameRelease.Fallout4, directory.Path, [], output);

        var exception = Assert.Throws<NativeWorkspaceException>(() => CreateFactory().Open(request));

        Assert.Contains("Small", exception.Message, StringComparison.Ordinal);
    }

    /// <summary>Requires an existing output's native text mode to match the explicit request.</summary>
    [Fact]
    public void ExistingOutputTextStorageMustMatchRequest()
    {
        using var directory = new TemporaryDirectory();
        var outputModKey = ModKey.FromNameAndExtension("Localized.esp");
        var outputPath = Path.Combine(directory.Path, outputModKey.ToString());
        IMod output = new Fallout4Mod(outputModKey, Fallout4Release.Fallout4)
        {
            UsingLocalization = true,
        };
        WritePlugin(output, outputPath);
        var definition = new NativeOutputDefinition(
            outputPath,
            outputModKey,
            MasterStyle.Full,
            NativeTextStorageMode.Embedded,
            createNew: false);
        var request = new NativeWorkspaceOpenRequest(GameRelease.Fallout4, directory.Path, [], definition);

        var exception = Assert.Throws<NativeWorkspaceException>(() => CreateFactory().Open(request));

        Assert.Contains("Localized text storage", exception.Message, StringComparison.Ordinal);
    }

    /// <summary>Applies legal small and medium styles through native game flags.</summary>
    [Theory]
    [InlineData(GameRelease.Starfield, "Output.esm", MasterStyle.Medium)]
    [InlineData(GameRelease.Starfield, "Output.esl", MasterStyle.Small)]
    [InlineData(GameRelease.Fallout4, "Output.esl", MasterStyle.Small)]
    [InlineData(GameRelease.SkyrimSE, "Output.esl", MasterStyle.Small)]
    public void AppliesLegalOutputStyle(GameRelease release, string outputName, MasterStyle masterStyle)
    {
        using var directory = new TemporaryDirectory();
        if (release == GameRelease.Starfield)
        {
            var starfieldModKey = ModKey.FromNameAndExtension("Starfield.esm");
            WriteEmptyPlugin(Path.Combine(directory.Path, starfieldModKey.ToString()), starfieldModKey, release);
        }

        var outputModKey = ModKey.FromNameAndExtension(outputName);
        var output = new NativeOutputDefinition(
            Path.Combine(directory.Path, outputModKey.ToString()),
            outputModKey,
            masterStyle,
            NativeTextStorageMode.Embedded,
            createNew: true);
        var request = new NativeWorkspaceOpenRequest(release, directory.Path, [], output);

        using var workspace = CreateFactory().Open(request);

        Assert.Equal(masterStyle, workspace.State.MasterStyle);
        Assert.Equal(masterStyle, ((IModMasterStyledGetter)workspace.Output).MasterStyle);
    }

    /// <summary>Preserves Mutagen's native parent chain for records nested beneath a cell group.</summary>
    [Fact]
    public void NestedRecordResolutionPreservesNativeParentContext()
    {
        using var directory = new TemporaryDirectory();
        var sourceModKey = ModKey.FromNameAndExtension("Base.esm");
        var source = new SkyrimMod(sourceModKey, SkyrimRelease.SkyrimSE);
        var cell = new SkyrimCell(source.GetNextFormKey(), SkyrimRelease.SkyrimSE) { EditorID = "NestedCell" };
        var placedObject = new SkyrimPlacedObject(source.GetNextFormKey(), SkyrimRelease.SkyrimSE);
        cell.Persistent.Add(placedObject);
        var subBlock = new SkyrimCellSubBlock();
        subBlock.Cells.Add(cell);
        var block = new SkyrimCellBlock();
        block.SubBlocks.Add(subBlock);
        source.Cells.Add(block);
        WritePlugin(source, Path.Combine(directory.Path, sourceModKey.ToString()));

        var outputModKey = ModKey.FromNameAndExtension("Output.esp");
        using var workspace = CreateFactory().Open(
            CreateNewRequest(directory.Path, GameRelease.SkyrimSE, [sourceModKey], outputModKey));

        var resolution = workspace.ResolveRecord(placedObject.FormKey, typeof(SkyrimPlacedObjectGetter), sourceModKey);

        Assert.NotNull(resolution.ExactContext.Parent);
        var parentCell = Assert.IsAssignableFrom<SkyrimCellGetter>(resolution.ExactContext.Parent.Record);
        Assert.Equal(cell.FormKey, parentCell.FormKey);
    }

}
