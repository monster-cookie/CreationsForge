using CreationsForge.Engine.Persistence;
using CreationsForge.Engine.Records;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;

namespace CreationsForge.UnitTests.Workspaces;

/// <summary>Verifies bounded save preflight failures before staging begins.</summary>
public sealed partial class PluginWorkspaceTests
{
    /// <summary>Returns a bounded failed result when destination inspection fails before staging begins.</summary>
    [Fact]
    public async Task PreflightStampFailurePreservesPendingWorkspace()
    {
        using var directory = new TemporaryDirectory();
        var outputModKey = ModKey.FromNameAndExtension("Output.esp");
        var outputPath = Path.Combine(directory.Path, outputModKey.ToString());
        var backend = new InstrumentedPersistenceBackend { FailCaptureStampCall = 1 };
        using var workspace = CreateFactory(backend).Open(
            CreateNewRequest(directory.Path, GameRelease.Fallout4, [], outputModKey));
        workspace.Records.Apply(new RecordChangeSet(0,
        [
            RecordMutation.Create("Keyword", [Set("EditorID", RecordValue.FromString("PendingKeyword"))]),
        ]));

        var result = await workspace.SaveAsync(cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(PluginSaveStatus.Failed, result.Status);
        Assert.Equal(PluginPublicationState.Unchanged, result.PublicationState);
        Assert.Equal(PluginSavePhase.Preflight, result.TerminalPhase);
        Assert.True(workspace.State.IsDirty);
        Assert.False(workspace.State.RequiresReopen);
        Assert.False(File.Exists(outputPath));
        Assert.Empty(Directory.EnumerateDirectories(directory.Path, ".CreationsForge-save-*"));
        Assert.Contains("could not be inspected before saving", result.Diagnostic, StringComparison.Ordinal);
    }
}
