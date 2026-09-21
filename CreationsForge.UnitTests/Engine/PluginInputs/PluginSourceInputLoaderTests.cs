using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.PluginInputs;
using CreationsForge.Core.Enums;
using CreationsForge.UnitTests.Engine.Starfield;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Starfield;
using Mutagen.Bethesda.Strings;
using Shouldly;

namespace CreationsForge.UnitTests.Engine.PluginInputs;

/// <summary>
/// Verifies bounded explicit plugin input preparation, localized lookup, physical baselines, cancellation, and ownership.
/// </summary>
public sealed class PluginSourceInputLoaderTests
{
    /// <summary>Verifies mixed plugin styles and complete source evidence produce the same baseline on repeated opens.</summary>
    [Fact]
    public async Task PrepareAndCompleteAsync_WithSameExplicitInputs_ProducesDeterministicCompleteBaseline()
    {
        using var fixture = StarfieldPluginTestFixture.Create();
        var loader = new PluginSourceInputLoader();
        var request = fixture.CreateOpenRequest(Guid.NewGuid());

        var firstPreparation = await loader.PrepareAsync(request, TestContext.Current.CancellationToken);
        var secondPreparation = await loader.PrepareAsync(request, TestContext.Current.CancellationToken);

        firstPreparation.Succeeded.ShouldBeTrue(firstPreparation.Error?.Message);
        secondPreparation.Succeeded.ShouldBeTrue(secondPreparation.Error?.Message);
        await using var first = firstPreparation.Value!;
        await using var second = secondPreparation.Value!;
        first.Plugins.Select(plugin => plugin.MasterStyle).ShouldBe(
            [MasterStyle.Small, MasterStyle.Full, MasterStyle.Medium, MasterStyle.Full]);
        first.Plugins.Single(plugin => plugin.Role == PluginRole.Source).Path.ShouldBe(fixture.SourcePluginPath);
        first.CreateParsingMeta(first.Plugins[0]).StringsLookup.ShouldNotBeNull();
        first.CreateParsingMeta(first.Plugins[1]).StringsLookup.ShouldBeNull();

        var firstBaseline = await first.CompleteOpenAsync(TestContext.Current.CancellationToken);
        var secondBaseline = await second.CompleteOpenAsync(TestContext.Current.CancellationToken);

        firstBaseline.Succeeded.ShouldBeTrue(firstBaseline.Error?.Message);
        secondBaseline.Succeeded.ShouldBeTrue(secondBaseline.Error?.Message);
        firstBaseline.Value!.BaselineId.ShouldNotBe(Guid.Empty);
        firstBaseline.Value.BaselineId.ShouldBe(secondBaseline.Value!.BaselineId);
        firstBaseline.Value.Artifacts.Take(4).Select(artifact => artifact.Role)
            .ShouldAllBe(role => role == PluginArtifactRole.Plugin);
        firstBaseline.Value.Artifacts.ShouldContain(artifact =>
            artifact.Role == PluginArtifactRole.Strings
            && artifact.Fingerprint.Exists
            && string.Equals(artifact.Language, Language.English.ToString(), StringComparison.Ordinal));
        firstBaseline.Value.Artifacts.ShouldContain(artifact =>
            artifact.Role == PluginArtifactRole.Strings
            && artifact.Fingerprint.Exists
            && string.Equals(artifact.Language, Language.French.ToString(), StringComparison.Ordinal));
    }

    /// <summary>Verifies that supported hosts capture stable physical identity for the source plugin through an open descriptor.</summary>
    /// <returns>A task that completes after physical source identity is captured and validated.</returns>
    [Fact]
    public async Task CompleteOpenAsync_OnSupportedHost_CapturesStablePhysicalPluginIdentity()
    {
        Assert.SkipUnless(
            OperatingSystem.IsWindows() || OperatingSystem.IsLinux() || OperatingSystem.IsMacOS(),
            "Physical plugin identity is supported only on Windows, Linux, and macOS.");

        using var fixture = StarfieldPluginTestFixture.Create();
        var preparation = await new PluginSourceInputLoader().PrepareAsync(
            fixture.CreateOpenRequest(),
            TestContext.Current.CancellationToken);
        preparation.Succeeded.ShouldBeTrue(preparation.Error?.Message);
        await using var inputs = preparation.Value!;

        var completion = await inputs.CompleteOpenAsync(TestContext.Current.CancellationToken);

        completion.Succeeded.ShouldBeTrue(completion.Error?.Message);
        var sourceArtifact = completion.Value!.Artifacts.Single(artifact =>
            artifact.Role == PluginArtifactRole.Plugin
            && string.Equals(artifact.Path, fixture.SourcePluginPath, StringComparison.Ordinal));
        var identity = sourceArtifact.FileIdentity.ShouldNotBeNull();
        var expectedProvider = OperatingSystem.IsWindows()
            ? "windows-file-id-v1"
            : OperatingSystem.IsLinux()
                ? "linux-statx-v1"
                : "macos-fstat-v1";
        identity.Provider.ShouldBe(expectedProvider);
        identity.VolumeId.ShouldNotBeNullOrWhiteSpace();
        identity.FileId.ShouldNotBeNullOrWhiteSpace();
        identity.LinkCount.ShouldNotBeNull();
        identity.LinkCount.Value.ShouldBeGreaterThanOrEqualTo(1UL);
    }

    /// <summary>Verifies an empty first directory falls through to the second plugin lookup without losing the record string key or source path.</summary>
    [Fact]
    public async Task TryLookupString_WithEmptyFirstDirectory_PreservesKeyLanguagesAndWinningSourcePath()
    {
        using var fixture = StarfieldPluginTestFixture.Create();
        var firstDirectory = fixture.RootDirectory.CreateSubdirectory("Strings-Empty");
        var secondDirectory = fixture.RootDirectory.CreateSubdirectory("Strings-Localized");
        WriteStringsFile(secondDirectory.FullName, fixture.SourceModKey, Language.English, 1, "Second English");
        WriteStringsFile(secondDirectory.FullName, fixture.SourceModKey, Language.French, 1, "Deuxième français");
        var request = new WorkspaceOpenRequest(
            Guid.NewGuid(),
            SupportedGame.Starfield,
            GameRelease.Starfield,
            fixture.SourcePluginPath,
            [fixture.SourcePluginPath],
            fixture.DataDirectory.FullName,
            [firstDirectory.FullName, secondDirectory.FullName]);

        var preparation = await new PluginSourceInputLoader().PrepareAsync(
            request,
            TestContext.Current.CancellationToken);

        preparation.Succeeded.ShouldBeTrue(preparation.Error?.Message);
        await using var inputs = preparation.Value!;
        var plugin = inputs.Plugins.Single();
        inputs.SupportsBinaryOverlay(plugin).ShouldBeFalse();
        Should.Throw<InvalidOperationException>(() => inputs.CreateOverlayReadParameters(plugin));
        var metadata = inputs.CreateParsingMeta(plugin);
        metadata.StringsLookup.ShouldNotBeNull();
        var translated = metadata.StringsLookup.CreateString(StringsSource.Normal, 1, Language.English);
        translated.StringsKey.ShouldBe((uint?)1);
        translated.TryLookup(Language.English, out var english).ShouldBeTrue();
        english.ShouldBe("Second English");
        translated.TryLookup(Language.French, out var french).ShouldBeTrue();
        french.ShouldBe("Deuxième français");

        inputs.TryLookupString(
            plugin,
            StringsSource.Normal,
            Language.French,
            1,
            out var value,
            out var sourcePath).ShouldBeTrue();
        value.ShouldBe("Deuxième français");
        sourcePath.ShouldBe(Path.Combine(
            secondDirectory.FullName,
            StringsUtility.GetFileName(
                StringsLanguageFormat.Iso,
                fixture.SourceModKey,
                Language.French,
                StringsSource.Normal)));
    }

    /// <summary>Verifies localized lookup selects the first explicit directory that contains a value for each language.</summary>
    [Fact]
    public async Task TryLookupString_WithValuesInBothDirectories_UsesFirstDirectoryPriorityPerLanguage()
    {
        using var fixture = StarfieldPluginTestFixture.Create();
        var firstDirectory = fixture.RootDirectory.CreateSubdirectory("Strings-First");
        var secondDirectory = fixture.RootDirectory.CreateSubdirectory("Strings-Second");
        WriteStringsFile(firstDirectory.FullName, fixture.SourceModKey, Language.English, 9, "First English");
        WriteStringsFile(secondDirectory.FullName, fixture.SourceModKey, Language.English, 9, "Second English");
        WriteStringsFile(secondDirectory.FullName, fixture.SourceModKey, Language.French, 9, "Second French");
        var request = new WorkspaceOpenRequest(
            Guid.NewGuid(),
            SupportedGame.Starfield,
            GameRelease.Starfield,
            fixture.SourcePluginPath,
            [fixture.SourcePluginPath],
            fixture.DataDirectory.FullName,
            [firstDirectory.FullName, secondDirectory.FullName]);
        var preparation = await new PluginSourceInputLoader().PrepareAsync(
            request,
            TestContext.Current.CancellationToken);

        preparation.Succeeded.ShouldBeTrue(preparation.Error?.Message);
        await using var inputs = preparation.Value!;
        var plugin = inputs.Plugins.Single();
        inputs.TryLookupString(
            plugin,
            StringsSource.Normal,
            Language.English,
            9,
            out var english,
            out var englishPath).ShouldBeTrue();
        inputs.TryLookupString(
            plugin,
            StringsSource.Normal,
            Language.French,
            9,
            out var french,
            out var frenchPath).ShouldBeTrue();

        english.ShouldBe("First English");
        englishPath.ShouldStartWith(firstDirectory.FullName);
        french.ShouldBe("Second French");
        frenchPath.ShouldStartWith(secondDirectory.FullName);
    }

    /// <summary>Verifies a plugin master is not discovered from the data directory when it is omitted from the explicit load order.</summary>
    [Fact]
    public async Task PrepareAsync_WithMasterOnlyPresentOutsideExplicitLoadOrder_ReturnsMissingMaster()
    {
        using var fixture = StarfieldPluginTestFixture.Create();
        var request = fixture.CreateOpenRequest(
            fixture.PatchPluginPath,
            [fixture.PatchPluginPath],
            Guid.NewGuid());

        var result = await new PluginSourceInputLoader().PrepareAsync(
            request,
            TestContext.Current.CancellationToken);

        result.Succeeded.ShouldBeFalse();
        result.Error!.Code.ShouldBe(EngineErrorCode.MissingMaster);
        result.Error.Message.ShouldContain(fixture.SourceModKey.FileName.ToString());
    }

    /// <summary>Verifies localized inputs fail closed without an explicit loose directory while nonlocalized inputs need no invented host path.</summary>
    [Fact]
    public async Task PrepareAsync_WithNoStringsDirectories_RequiresOneOnlyForLocalizedPlugins()
    {
        using var fixture = StarfieldPluginTestFixture.Create();
        var localizedRequest = new WorkspaceOpenRequest(
            Guid.NewGuid(),
            SupportedGame.Starfield,
            GameRelease.Starfield,
            fixture.SourcePluginPath,
            [fixture.SourcePluginPath],
            fixture.DataDirectory.FullName,
            []);
        var fullPluginPath = Path.Combine(
            fixture.DataDirectory.FullName,
            fixture.FullModKey.FileName.ToString());
        var nonlocalizedRequest = new WorkspaceOpenRequest(
            Guid.NewGuid(),
            SupportedGame.Starfield,
            GameRelease.Starfield,
            fullPluginPath,
            [fullPluginPath],
            fixture.DataDirectory.FullName,
            []);

        var localizedResult = await new PluginSourceInputLoader().PrepareAsync(
            localizedRequest,
            TestContext.Current.CancellationToken);
        var nonlocalizedResult = await new PluginSourceInputLoader().PrepareAsync(
            nonlocalizedRequest,
            TestContext.Current.CancellationToken);

        localizedResult.Succeeded.ShouldBeFalse();
        localizedResult.Error!.Code.ShouldBe(EngineErrorCode.InvalidRequest);
        localizedResult.Error.Message.ShouldContain("explicit localized-string directory");
        nonlocalizedResult.Succeeded.ShouldBeTrue(nonlocalizedResult.Error?.Message);
        await nonlocalizedResult.Value!.DisposeAsync();
    }

    /// <summary>Verifies source locks deny writers throughout the input lifetime and release on disposal.</summary>
    [Fact]
    public async Task SourceReadLock_BlocksWritersUntilDisposal()
    {
        using var fixture = StarfieldPluginTestFixture.Create();
        var preparation = await new PluginSourceInputLoader().PrepareAsync(
            fixture.CreateOpenRequest(),
            TestContext.Current.CancellationToken);
        preparation.Succeeded.ShouldBeTrue(preparation.Error?.Message);
        var inputs = preparation.Value!;

        try
        {
            SourceFileLockAssertions.AssertWriteDenied(fixture.SourcePluginPath);
            using (var reader = new FileStream(
                       fixture.SourcePluginPath,
                       FileMode.Open,
                       FileAccess.Read,
                       FileShare.Read))
            {
                reader.Length.ShouldBeGreaterThan(0);
            }

            var completion = await inputs.CompleteOpenAsync(TestContext.Current.CancellationToken);
            completion.Succeeded.ShouldBeTrue(completion.Error?.Message);
            var verification = await inputs.VerifyUnchangedAsync(TestContext.Current.CancellationToken);
            verification.Succeeded.ShouldBeTrue(verification.Error?.Message);
        }
        finally
        {
            await inputs.DisposeAsync();
        }

        SourceFileLockAssertions.AssertWriteAllowed(fixture.SourcePluginPath);
    }

    /// <summary>Verifies independent read-only workspaces can retain locks for the same source set concurrently.</summary>
    [Fact]
    public async Task PrepareAsync_WithSameSources_AllowsConcurrentReadOnlyLifetimes()
    {
        using var fixture = StarfieldPluginTestFixture.Create();
        var loader = new PluginSourceInputLoader();
        var firstPreparation = await loader.PrepareAsync(
            fixture.CreateOpenRequest(),
            TestContext.Current.CancellationToken);
        firstPreparation.Succeeded.ShouldBeTrue(firstPreparation.Error?.Message);
        await using var first = firstPreparation.Value!;

        var secondPreparation = await loader.PrepareAsync(
            fixture.CreateOpenRequest(),
            TestContext.Current.CancellationToken);
        secondPreparation.Succeeded.ShouldBeTrue(secondPreparation.Error?.Message);
        await using var second = secondPreparation.Value!;

        (await first.CompleteOpenAsync(TestContext.Current.CancellationToken))
            .Succeeded.ShouldBeTrue();
        (await second.CompleteOpenAsync(TestContext.Current.CancellationToken))
            .Succeeded.ShouldBeTrue();
    }

    /// <summary>Verifies applicable archives are retained under read locks without content digests.</summary>
    [Fact]
    public async Task PrepareAsync_WithApplicableArchive_LocksWithoutHashingContent()
    {
        using var fixture = StarfieldPluginTestFixture.Create();
        var archivePath = Path.Combine(fixture.DataDirectory.FullName, "PluginSmall - Main.ba2");
        var archiveBytes = new byte[]
        {
            0x42, 0x54, 0x44, 0x58,
            0x01, 0x00, 0x00, 0x00,
            0x47, 0x4E, 0x52, 0x4C,
            0x00, 0x00, 0x00, 0x00,
            0x18, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x10, 0x20, 0x30, 0x40
        };
        await File.WriteAllBytesAsync(archivePath, archiveBytes, TestContext.Current.CancellationToken);

        var preparation = await new PluginSourceInputLoader().PrepareAsync(
            fixture.CreateOpenRequest(),
            TestContext.Current.CancellationToken);

        preparation.Succeeded.ShouldBeTrue(preparation.Error?.Message);
        await using var inputs = preparation.Value!;
        var completion = await inputs.CompleteOpenAsync(TestContext.Current.CancellationToken);
        completion.Succeeded.ShouldBeTrue(completion.Error?.Message);
        var archive = completion.Value!.Artifacts
            .Single(artifact => string.Equals(artifact.Path, archivePath, StringComparison.OrdinalIgnoreCase));
        archive.Fingerprint.Exists.ShouldBeTrue();
        archive.Fingerprint.Length.ShouldBe(archiveBytes.LongLength);
        archive.Fingerprint.Sha256.ShouldBeNull();
        SourceFileLockAssertions.AssertWriteDenied(archivePath);
    }

    /// <summary>Verifies source verification trusts retained locks and does not rescan directories for new optional paths.</summary>
    [Fact]
    public async Task VerifyUnchangedAsync_DoesNotRescanSourceInventory()
    {
        using var fixture = StarfieldPluginTestFixture.Create();
        var emptyDirectory = fixture.RootDirectory.CreateSubdirectory("Strings-Explicit-Empty");
        var request = new WorkspaceOpenRequest(
            Guid.NewGuid(),
            SupportedGame.Starfield,
            GameRelease.Starfield,
            fixture.SourcePluginPath,
            [fixture.SourcePluginPath],
            fixture.DataDirectory.FullName,
            [emptyDirectory.FullName]);
        var preparation = await new PluginSourceInputLoader().PrepareAsync(
            request,
            TestContext.Current.CancellationToken);
        preparation.Succeeded.ShouldBeTrue(preparation.Error?.Message);
        await using var inputs = preparation.Value!;
        var completion = await inputs.CompleteOpenAsync(TestContext.Current.CancellationToken);
        completion.Succeeded.ShouldBeTrue(completion.Error?.Message);
        emptyDirectory.Delete();
        await File.WriteAllBytesAsync(
            Path.Combine(fixture.DataDirectory.FullName, "PluginSmall - Main.ba2"),
            [0x42],
            TestContext.Current.CancellationToken);

        var verification = await inputs.VerifyUnchangedAsync(TestContext.Current.CancellationToken);

        verification.Succeeded.ShouldBeTrue(verification.Error?.Message);
        verification.Value.ShouldBeSameAs(completion.Value);
    }

    /// <summary>Verifies a retained source lock blocks same-path replacement while the workspace is open.</summary>
    [Fact]
    public async Task SourceReadLock_BlocksPathReplacement()
    {
        using var fixture = StarfieldPluginTestFixture.Create();
        var preparation = await new PluginSourceInputLoader().PrepareAsync(
            fixture.CreateOpenRequest(),
            TestContext.Current.CancellationToken);
        preparation.Succeeded.ShouldBeTrue(preparation.Error?.Message);
        await using var inputs = preparation.Value!;
        var completion = await inputs.CompleteOpenAsync(TestContext.Current.CancellationToken);
        completion.Succeeded.ShouldBeTrue(completion.Error?.Message);
        if (!OperatingSystem.IsWindows())
        {
            Assert.Skip("Mandatory deletion and path-replacement exclusion is provided by Windows share modes; Unix source locks are cooperative advisory locks.");
        }

        var replacementPath = Path.Combine(fixture.DataDirectory.FullName, "replacement.tmp");
        File.Copy(fixture.SourcePluginPath, replacementPath);

        Should.Throw<IOException>(() => File.Delete(fixture.SourcePluginPath));
        File.Exists(fixture.SourcePluginPath).ShouldBeTrue();
        var verification = await inputs.VerifyUnchangedAsync(TestContext.Current.CancellationToken);
        verification.Succeeded.ShouldBeTrue(verification.Error?.Message);
    }

    /// <summary>Verifies cancellation remains scoped to one open operation and disposal invalidates later input use.</summary>
    [Fact]
    public async Task SourceInputLifetime_WithCancellationAndDisposal_EnforcesOperationOwnership()
    {
        using var fixture = StarfieldPluginTestFixture.Create();
        using var canceledSource = new CancellationTokenSource();
        canceledSource.Cancel();
        await Should.ThrowAsync<OperationCanceledException>(() =>
            new PluginSourceInputLoader().PrepareAsync(fixture.CreateOpenRequest(), canceledSource.Token));

        var preparation = await new PluginSourceInputLoader().PrepareAsync(
            fixture.CreateOpenRequest(),
            TestContext.Current.CancellationToken);
        preparation.Succeeded.ShouldBeTrue(preparation.Error?.Message);
        var inputs = preparation.Value!;
        Should.Throw<OperationCanceledException>(() =>
            inputs.OpenReadStream(inputs.Plugins[0], canceledSource.Token));

        using (var stream = inputs.OpenReadStream(inputs.Plugins[0], TestContext.Current.CancellationToken))
        {
            stream.ReadUInt8().ShouldBe((byte)'T');
        }

        await inputs.DisposeAsync();
        Should.Throw<ObjectDisposedException>(() => inputs.CreateParsingMeta(inputs.Plugins[0]));
    }

    /// <summary>Verifies a strings archive association requires no invented language identifier.</summary>
    [Fact]
    public void PluginArtifactAssociation_WithStringsArchive_AllowsNullLanguageOnly()
    {
        var fingerprint = new PluginArtifactFingerprint(true, 3, new string('A', 64));
        var archivePath = Path.GetFullPath("PluginSmall - Main.ba2");

        var association = new PluginArtifactAssociation(
            archivePath,
            PluginArtifactRole.StringsArchive,
            null,
            fingerprint);

        association.Language.ShouldBeNull();
        Should.Throw<ArgumentException>(() => new PluginArtifactAssociation(
            archivePath,
            PluginArtifactRole.StringsArchive,
            "English",
            fingerprint));
    }

    /// <summary>Writes one deterministic one-entry STRINGS sidecar for an explicit Starfield lookup fixture.</summary>
    /// <param name="directoryPath">The explicit strings directory.</param>
    /// <param name="modKey">The plugin identity used in the plugin sidecar file name.</param>
    /// <param name="language">The sidecar language.</param>
    /// <param name="key">The plugin numeric string key.</param>
    /// <param name="value">The null-terminated UTF-8 fixture value.</param>
    private static void WriteStringsFile(
        string directoryPath,
        ModKey modKey,
        Language language,
        uint key,
        string value)
    {
        var path = Path.Combine(
            directoryPath,
            StringsUtility.GetFileName(
                StringsLanguageFormat.Iso,
                modKey,
                language,
                StringsSource.Normal));
        var valueBytes = System.Text.Encoding.UTF8.GetBytes(value);
        using var stream = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None);
        using var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, leaveOpen: false);
        writer.Write((uint)1);
        writer.Write((uint)(valueBytes.Length + 1));
        writer.Write(key);
        writer.Write((uint)0);
        writer.Write(valueBytes);
        writer.Write((byte)0);
    }
}
