using CreationsForge.Engine.Persistence;
using CreationsForge.Engine.Records;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;

namespace CreationsForge.UnitTests.Workspaces;

/// <summary>Verifies content-based destination identity across external changes and successive publications.</summary>
public sealed partial class PluginWorkspaceTests
{
    /// <summary>Refuses same-size and same-timestamp destination bytes that changed after the workspace opened.</summary>
    [Fact]
    public async Task SaveRejectsExternalDestinationReplacement()
    {
        using var directory = new TemporaryDirectory();
        var outputModKey = ModKey.FromNameAndExtension("Output.esp");
        var outputPath = Path.Combine(directory.Path, outputModKey.ToString());
        WriteEmptyPlugin(outputPath, outputModKey, GameRelease.Fallout4);
        using var workspace = CreateFactory().Open(CreateExistingRequest(directory.Path, GameRelease.Fallout4, outputModKey));
        workspace.Records.Apply(new RecordChangeSet(0,
        [
            RecordMutation.Create("Keyword", [Set("EditorID", RecordValue.FromString("PendingKeyword"))]),
        ]));
        var originalBytes = File.ReadAllBytes(outputPath);
        var originalWriteTimeUtc = File.GetLastWriteTimeUtc(outputPath);
        var externallyChanged = originalBytes.ToArray();
        externallyChanged[^1] ^= 0xFF;
        var replacementPath = Path.Combine(directory.Path, "replacement.tmp");
        File.WriteAllBytes(replacementPath, externallyChanged);
        File.Move(replacementPath, outputPath, overwrite: true);
        File.SetLastWriteTimeUtc(outputPath, originalWriteTimeUtc);
        Assert.Equal(originalBytes.Length, new FileInfo(outputPath).Length);
        Assert.Equal(originalWriteTimeUtc, File.GetLastWriteTimeUtc(outputPath));

        var result = await workspace.SaveAsync(cancellationToken: TestContext.Current.CancellationToken);

        Assert.Equal(PluginSaveStatus.Failed, result.Status);
        Assert.Equal(PluginPublicationState.Unchanged, result.PublicationState);
        Assert.True(workspace.State.IsDirty);
        Assert.False(workspace.State.RequiresReopen);
        Assert.Equal(externallyChanged, File.ReadAllBytes(outputPath));
        Assert.Contains("changed outside this workspace", result.Diagnostic, StringComparison.Ordinal);
    }

    /// <summary>Refreshes the expected content digest after publication so a later save does not conflict with the workspace's own bytes.</summary>
    [Fact]
    public async Task SecondSaveUsesPublishedContentDigestBaseline()
    {
        using var directory = new TemporaryDirectory();
        var outputModKey = ModKey.FromNameAndExtension("Output.esp");
        using var workspace = CreateFactory().Open(
            CreateNewRequest(directory.Path, GameRelease.Fallout4, [], outputModKey));
        var created = workspace.Records.Apply(new RecordChangeSet(0,
        [
            RecordMutation.Create("Keyword", [Set("EditorID", RecordValue.FromString("FirstValue"))]),
        ])).Records.Single();

        var firstSave = await workspace.SaveAsync(cancellationToken: TestContext.Current.CancellationToken);
        Assert.True(firstSave.Status == PluginSaveStatus.Succeeded, firstSave.Diagnostic);
        workspace.Records.Apply(new RecordChangeSet(1,
        [
            RecordMutation.Override("Keyword", created.FormKey, outputModKey,
            [
                Set("EditorID", RecordValue.FromString("SecondValue")),
            ]),
        ]));

        var secondSave = await workspace.SaveAsync(cancellationToken: TestContext.Current.CancellationToken);

        Assert.True(secondSave.Status == PluginSaveStatus.Succeeded, secondSave.Diagnostic);
        Assert.Equal(PluginPublicationState.Published, secondSave.PublicationState);
        Assert.False(workspace.State.IsDirty);
    }
}
