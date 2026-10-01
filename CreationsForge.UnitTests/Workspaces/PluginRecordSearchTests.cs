using CreationsForge.Engine.Workspaces;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using StarfieldKeyword = Mutagen.Bethesda.Starfield.Keyword;
using StarfieldMod = Mutagen.Bethesda.Starfield.StarfieldMod;
using StarfieldRelease = Mutagen.Bethesda.Starfield.StarfieldRelease;

namespace CreationsForge.UnitTests.Workspaces;

/// <summary>Verifies bounded winning-record search for the first editable family.</summary>
public sealed partial class PluginWorkspaceTests
{
    /// <summary>Pages winning keywords without assuming enumeration order and reports the winning plugin for an override.</summary>
    [Fact]
    public void SearchPagesWinningKeywordsWithoutAssumingOrder()
    {
        using var directory = new TemporaryDirectory();
        var masterModKey = ModKey.FromNameAndExtension("Starfield.esm");
        var selectedModKey = ModKey.FromNameAndExtension("Selected.esp");
        var master = new StarfieldMod(masterModKey, StarfieldRelease.Starfield);
        var alpha = AddKeyword(master, "AlphaOverride");
        AddKeyword(master, "BetaKeyword");
        AddKeyword(master, "GammaKeyword");
        var alphaKey = alpha.FormKey;
        WritePlugin(master, Path.Combine(directory.Path, masterModKey.ToString()));

        var selected = new StarfieldMod(selectedModKey, StarfieldRelease.Starfield);
        ((IMod)selected).MasterReferences.Add(new MasterReference { Master = masterModKey });
        selected.Keywords.Add((StarfieldKeyword)alpha.DeepCopy());
        WritePlugin(selected, Path.Combine(directory.Path, selectedModKey.ToString()), master);

        var outputModKey = ModKey.FromNameAndExtension("Output.esp");
        using var workspace = CreateFactory().Open(
            CreateNewRequest(directory.Path, GameRelease.Starfield, [selectedModKey], outputModKey));

        var paged = new Dictionary<FormKey, PluginRecordSummary>();
        for (var skip = 0; skip < 3; skip++)
        {
            var page = workspace.SearchWinningRecords("Keyword", editorId: null, skip, take: 1);
            var summary = Assert.Single(page.Records);
            Assert.Equal(skip < 2, page.HasMore);
            Assert.True(paged.TryAdd(summary.OriginFormKey, summary));
            Assert.Equal(summary.WinningModKey, summary.ContainingModKey);
        }

        var exhausted = workspace.SearchWinningRecords("Keyword", editorId: null, skip: 3, take: 1);
        Assert.Empty(exhausted.Records);
        Assert.False(exhausted.HasMore);

        var all = workspace.SearchWinningRecords("Keyword", editorId: " ", skip: 0, take: 50);
        Assert.False(all.HasMore);
        Assert.Equal(paged.Keys.ToHashSet(), all.Records.Select(record => record.OriginFormKey).ToHashSet());

        var overridden = Assert.Single(workspace.SearchWinningRecords("Keyword", "alpha", skip: 0, take: 50).Records);
        Assert.Equal(alphaKey, overridden.OriginFormKey);
        Assert.Equal("AlphaOverride", overridden.EditorId);
        Assert.Equal(selectedModKey, overridden.WinningModKey);
        Assert.Equal(selectedModKey, overridden.ContainingModKey);

        var filtered = workspace.SearchWinningRecords("Keyword", "KEYWORD", skip: 0, take: 50).Records;
        Assert.Equal(2, filtered.Count);
        Assert.All(filtered, summary => Assert.Equal(masterModKey, summary.WinningModKey));
        Assert.Equal(
            new[] { "BetaKeyword", "GammaKeyword" },
            filtered.Select(summary => summary.EditorId).OrderBy(id => id, StringComparer.Ordinal).ToArray());
    }

    /// <summary>Rejects a search page that is outside the closed bounds or names an unknown family.</summary>
    [Fact]
    public void SearchRejectsInvalidPageAndFamily()
    {
        using var directory = new TemporaryDirectory();
        var masterModKey = ModKey.FromNameAndExtension("Starfield.esm");
        WriteEmptyPlugin(Path.Combine(directory.Path, masterModKey.ToString()), masterModKey, GameRelease.Starfield);
        var outputModKey = ModKey.FromNameAndExtension("Output.esp");
        using var workspace = CreateFactory().Open(
            CreateNewRequest(directory.Path, GameRelease.Starfield, [], outputModKey));

        Assert.Throws<ArgumentOutOfRangeException>(() => workspace.SearchWinningRecords("Keyword", null, skip: 0, take: 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => workspace.SearchWinningRecords("Keyword", null, skip: 0, take: 51));
        var unknown = Assert.Throws<ArgumentException>(() =>
            workspace.SearchWinningRecords("NotAFamily", null, skip: 0, take: 1));
        Assert.Contains("NotAFamily", unknown.Message, StringComparison.Ordinal);
    }

    private static StarfieldKeyword AddKeyword(StarfieldMod mod, string editorId)
    {
        var keyword = new StarfieldKeyword(mod.GetNextFormKey(), StarfieldRelease.Starfield)
        {
            EditorID = editorId,
        };
        mod.Keywords.Add(keyword);
        return keyword;
    }
}
