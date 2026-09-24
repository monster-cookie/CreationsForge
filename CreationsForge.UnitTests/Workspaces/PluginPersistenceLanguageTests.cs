using CreationsForge.Engine.Persistence;
using CreationsForge.Engine.Records;
using CreationsForge.Engine.Workspaces;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Strings;

namespace CreationsForge.UnitTests.Workspaces;

/// <summary>Verifies translated-string persistence language handling.</summary>
public sealed partial class PluginWorkspaceTests
{
    /// <summary>Normalizes a native translated string whose selected target is absent without losing its available language map.</summary>
    [Fact]
    public void ReadsNativeTranslationWithUnavailableTargetLanguage()
    {
        using var directory = new TemporaryDirectory();
        var outputModKey = ModKey.FromNameAndExtension("Output.esp");
        using var workspace = CreateFactory().Open(
            CreateNewRequest(directory.Path, GameRelease.Fallout4, [], outputModKey));
        var output = Assert.IsAssignableFrom<Mutagen.Bethesda.Fallout4.IFallout4Mod>(workspace.MutableOutput);
        var message = output.Messages.AddNew();
        message.Name = new TranslatedString(Language.French);
        message.Name.Set(Language.English, "Available English value");

        var snapshot = workspace.Records.Read(new RecordLocator("Message", message.FormKey, outputModKey));

        var name = Assert.IsType<RecordValue.TranslatedStringRecordValue>(snapshot.Values["Name"]);
        Assert.Equal(Language.English, name.TargetLanguage);
        Assert.Equal("Available English value", name.Values[Language.English]);
    }

    /// <summary>Uses the configured target language for localized native export and reopen.</summary>
    [Fact]
    public async Task LocalizedSaveUsesConfiguredTargetLanguage()
    {
        using var directory = new TemporaryDirectory();
        var outputModKey = ModKey.FromNameAndExtension("French.esp");
        var outputPath = Path.Combine(directory.Path, outputModKey.ToString());
        var translated = RecordValue.FromTranslatedString(
            Language.French,
            new Dictionary<Language, string> { [Language.French] = "Valeur française" });
        FormKey messageFormKey;

        using (var workspace = CreateFactory().Open(new PluginWorkspaceOpenRequest(
            GameRelease.Fallout4,
            directory.Path,
            [],
            new PluginOutputDefinition(
                outputPath,
                outputModKey,
                MasterStyle.Full,
                PluginTextStorageMode.Localized,
                createNew: true,
                targetLanguage: Language.French))))
        {
            messageFormKey = workspace.Records.Apply(new RecordChangeSet(0,
            [
                RecordMutation.Create("Message",
                [
                    Set("EditorID", RecordValue.FromString("FrenchMessage")),
                    Set("Name", translated),
                    Set("Description", translated),
                ]),
            ])).Records.Single().FormKey;

            var result = await workspace.SaveAsync(cancellationToken: TestContext.Current.CancellationToken);

            Assert.True(result.Status == PluginSaveStatus.Succeeded, result.Diagnostic);
        }

        using var reopened = CreateFactory().Open(new PluginWorkspaceOpenRequest(
            GameRelease.Fallout4,
            directory.Path,
            [],
            new PluginOutputDefinition(
                outputPath,
                outputModKey,
                MasterStyle.Full,
                PluginTextStorageMode.Localized,
                createNew: false,
                targetLanguage: Language.French)));
        var snapshot = reopened.Records.Read(new RecordLocator("Message", messageFormKey, outputModKey));
        var name = Assert.IsType<RecordValue.TranslatedStringRecordValue>(snapshot.Values["Name"]);
        Assert.Equal(Language.French, name.TargetLanguage);
        Assert.Equal("Valeur française", name.Values[Language.French]);
    }
}
