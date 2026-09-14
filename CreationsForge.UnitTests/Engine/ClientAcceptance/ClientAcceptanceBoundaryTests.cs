using System.Diagnostics;
using Shouldly;
using Fallout4Book = Mutagen.Bethesda.Fallout4.Book;
using Fallout4FormList = Mutagen.Bethesda.Fallout4.FormList;
using Fallout4Mod = Mutagen.Bethesda.Fallout4.Fallout4Mod;
using Fallout4Release = Mutagen.Bethesda.Fallout4.Fallout4Release;
using SkyrimBook = Mutagen.Bethesda.Skyrim.Book;
using SkyrimFormList = Mutagen.Bethesda.Skyrim.FormList;
using SkyrimMod = Mutagen.Bethesda.Skyrim.SkyrimMod;
using SkyrimRelease = Mutagen.Bethesda.Skyrim.SkyrimRelease;
using StarfieldBook = Mutagen.Bethesda.Starfield.Book;
using StarfieldFormList = Mutagen.Bethesda.Starfield.FormList;
using StarfieldMod = Mutagen.Bethesda.Starfield.StarfieldMod;
using StarfieldRelease = Mutagen.Bethesda.Starfield.StarfieldRelease;

namespace CreationsForge.UnitTests.Engine.ClientAcceptance;

/// <summary>Exercises physical-path and complete plugin-record boundaries used by retained client acceptance.</summary>
public sealed class ClientAcceptanceBoundaryTests
{
    /// <summary>Verifies an apparent export root junction is rejected before any file reaches its sibling physical target.</summary>
    /// <returns>A task that completes after junction creation, rejection, and target-preserving unlink cleanup.</returns>
    [Fact]
    public async Task ExportRootJunctionToSiblingTargetIsRejectedBeforeWrite()
    {
        if (!OperatingSystem.IsWindows())
        {
            Assert.Skip("This regression requires a Windows directory junction.");
        }

        var testRoot = Path.Combine(
            ClientAcceptancePaths.FindRepositoryRoot(),
            ".work",
            "ClientAcceptancePathTests",
            Guid.NewGuid().ToString("N"));
        var physicalTarget = Path.Combine(testRoot, "PhysicalTarget");
        var apparentRoot = Path.Combine(testRoot, "ApparentRoot");
        Directory.CreateDirectory(physicalTarget);
        var junctionCreated = false;
        try
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                },
            };
            process.StartInfo.ArgumentList.Add("/d");
            process.StartInfo.ArgumentList.Add("/c");
            process.StartInfo.ArgumentList.Add("mklink");
            process.StartInfo.ArgumentList.Add("/J");
            process.StartInfo.ArgumentList.Add(apparentRoot);
            process.StartInfo.ArgumentList.Add(physicalTarget);
            process.Start();
            await process.WaitForExitAsync(TestContext.Current.CancellationToken);
            if (process.ExitCode != 0)
            {
                var output = await process.StandardOutput.ReadToEndAsync(TestContext.Current.CancellationToken);
                var error = await process.StandardError.ReadToEndAsync(TestContext.Current.CancellationToken);
                throw new InvalidOperationException($"The host failed to create the directory junction fixture with exit code {process.ExitCode}: {output} {error}");
            }

            junctionCreated = true;
            var resolved = new DirectoryInfo(apparentRoot).ResolveLinkTarget(returnFinalTarget: false);
            resolved.ShouldNotBeNull();
            string.Equals(Path.GetFullPath(resolved.FullName), Path.GetFullPath(physicalTarget), StringComparison.OrdinalIgnoreCase).ShouldBeTrue();
            Directory.EnumerateFileSystemEntries(physicalTarget).ShouldBeEmpty();

            Should.Throw<InvalidDataException>(() => ClientAcceptancePaths.PrepareExplicitExportRoot(apparentRoot));

            Directory.EnumerateFileSystemEntries(physicalTarget).ShouldBeEmpty();
        }
        finally
        {
            if (junctionCreated && Directory.Exists(apparentRoot))
            {
                var resolved = new DirectoryInfo(apparentRoot).ResolveLinkTarget(returnFinalTarget: false);
                if (resolved is not null
                    && string.Equals(Path.GetFullPath(resolved.FullName), Path.GetFullPath(physicalTarget), StringComparison.OrdinalIgnoreCase))
                {
                    Directory.Delete(apparentRoot, recursive: false);
                }
            }

            if (!Directory.Exists(apparentRoot) && Directory.Exists(testRoot))
            {
                Directory.Delete(testRoot, recursive: true);
            }
        }
    }

    /// <summary>Verifies each game-specific complete-record enumerator rejects a Book hidden beside two expected FormLists.</summary>
    /// <param name="game">The plugin game whose real in-memory mod is inspected.</param>
    [Theory]
    [InlineData("starfield")]
    [InlineData("fallout4")]
    [InlineData("skyrim")]
    public void CompleteRecordSetRejectsNonFormList(string game)
    {
        switch (game)
        {
            case "starfield":
            {
                var mod = new StarfieldMod("ClientAcceptanceExtra.esm", StarfieldRelease.Starfield);
                mod.FormLists.Add(new StarfieldFormList(mod, "FirstList"));
                mod.FormLists.Add(new StarfieldFormList(mod, "SecondList"));
                mod.Books.Add(new StarfieldBook(mod, "UnexpectedBook"));
                Should.Throw<InvalidDataException>(() => ClientAcceptanceSavedOutputVerifierTests.RequireExactStarfieldRecordSet(mod, 2));
                break;
            }
            case "fallout4":
            {
                var mod = new Fallout4Mod("ClientAcceptanceExtra.esm", Fallout4Release.Fallout4);
                mod.FormLists.Add(new Fallout4FormList(mod, "FirstList"));
                mod.FormLists.Add(new Fallout4FormList(mod, "SecondList"));
                mod.Books.Add(new Fallout4Book(mod, "UnexpectedBook"));
                Should.Throw<InvalidDataException>(() => ClientAcceptanceSavedOutputVerifierTests.RequireExactFallout4RecordSet(mod, 2));
                break;
            }
            case "skyrim":
            {
                var mod = new SkyrimMod("ClientAcceptanceExtra.esm", SkyrimRelease.SkyrimSE);
                mod.FormLists.Add(new SkyrimFormList(mod, "FirstList"));
                mod.FormLists.Add(new SkyrimFormList(mod, "SecondList"));
                mod.Books.Add(new SkyrimBook(mod, "UnexpectedBook"));
                Should.Throw<InvalidDataException>(() => ClientAcceptanceSavedOutputVerifierTests.RequireExactSkyrimRecordSet(mod, 2));
                break;
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(game), game, "The boundary test requires a supported plugin game.");
        }
    }
}
