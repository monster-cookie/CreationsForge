using System.Diagnostics;
using System.Globalization;
using CreationsForge.Engine.Workspaces;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;

namespace CreationsForge.UnitTests.Workspaces;

/// <summary>Hosts the fresh-process persistence acceptance helper.</summary>
public sealed partial class PluginWorkspaceTests
{
    private static async Task AssertFreshProcessReopenAsync(
        string dataDirectory,
        GameRelease release,
        ModKey outputModKey,
        PluginTextStorageMode textStorageMode,
        int expectedRecordCount,
        string? familyId = null,
        FormKey? expectedFormKey = null,
        string? expectedFieldPath = null,
        string? expectedStringValue = null)
    {
        Assert.True(
            familyId is null && expectedFormKey is null && expectedFieldPath is null && expectedStringValue is null
                || familyId is not null && expectedFormKey is not null && expectedFieldPath is not null && expectedStringValue is not null,
            "Fresh-process record expectations must be supplied together.");
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
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };
        startInfo.ArgumentList.Add(probePath);
        startInfo.ArgumentList.Add("reopen");
        startInfo.ArgumentList.Add(dataDirectory);
        startInfo.ArgumentList.Add(release.ToString());
        startInfo.ArgumentList.Add(outputModKey.ToString());
        startInfo.ArgumentList.Add(textStorageMode.ToString());
        startInfo.ArgumentList.Add(expectedRecordCount.ToString(CultureInfo.InvariantCulture));
        if (familyId is not null
            && expectedFormKey is { } formKey
            && expectedFieldPath is not null
            && expectedStringValue is not null)
        {
            startInfo.ArgumentList.Add(familyId);
            startInfo.ArgumentList.Add(formKey.ModKey.ToString());
            startInfo.ArgumentList.Add(formKey.ID.ToString(CultureInfo.InvariantCulture));
            startInfo.ArgumentList.Add(expectedFieldPath);
            startInfo.ArgumentList.Add(expectedStringValue);
        }

        using var probe = Process.Start(startInfo)
            ?? throw new InvalidOperationException("Could not start the fresh-process reopen probe.");
        var standardOutput = probe.StandardOutput.ReadToEndAsync(TestContext.Current.CancellationToken);
        var standardError = probe.StandardError.ReadToEndAsync(TestContext.Current.CancellationToken);
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(TestContext.Current.CancellationToken);
        timeout.CancelAfter(TimeSpan.FromSeconds(15));
        try
        {
            await probe.WaitForExitAsync(timeout.Token);
        }
        catch (OperationCanceledException) when (!TestContext.Current.CancellationToken.IsCancellationRequested)
        {
            probe.Kill(entireProcessTree: true);
            await probe.WaitForExitAsync(TestContext.Current.CancellationToken);
            Assert.Fail($"The fresh-process reopen probe timed out. Output: {await standardOutput} Error: {await standardError}");
        }

        Assert.True(
            probe.ExitCode == 0,
            $"The fresh-process reopen probe failed. Output: {await standardOutput} Error: {await standardError}");
    }
}
