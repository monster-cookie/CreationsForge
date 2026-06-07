using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Starfield;
using CreationsForge.Starfield.Interfaces;
using Moq;
using Shouldly;

namespace CreationsForge.UnitTests.Services;

public class StarfieldRecordReaderTests
{
    [Fact]
    public void ReadPluginRecords_DelegatesToService()
    {
        var plugin = CreatePlugin();
        var formLists = new List<FormListDTO> { CreateFormList(plugin) };
        var gameSettings = new List<GameSettingDTO> { CreateGameSetting(plugin) };
        var globals = new List<GlobalDTO> { CreateGlobal(plugin) };
        var recordSet = new PluginRecordSetDTO
        {
            FormLists = formLists,
            GameSettings = gameSettings,
            Globals = globals
        };
        var service = new Mock<IStarfieldRecordReaderService>();
        service.Setup(reader => reader.ReadPluginRecords(plugin, It.IsAny<CancellationToken>())).Returns(recordSet);
        var sut = new StarfieldRecordReader(service.Object);

        var result = sut.ReadPluginRecords(plugin);

        result.ShouldBe(recordSet);
    }

    private static PluginDTO CreatePlugin()
    {
        return new PluginDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey("Test", "Test.esm"),
            LoadOrderIndex = 0,
            Enabled = true,
            ExistsOnDisk = true,
            ImportState = PluginImportState.Current,
            HeaderFlags = 0,
            FormVersion = 1,
            RecordCount = 0,
            SourceLastWriteUTCTicks = 0,
            SourceFileSizeBytes = 0,
            LastCheckedUTC = DateTime.UtcNow
        };
    }

    private static FormListDTO CreateFormList(PluginDTO plugin)
    {
        return new FormListDTO
        {
            Game = plugin.Game,
            ModKey = plugin.ModKey,
            FormKey = CreateFormKey(plugin.ModKey, 1),
            EditorID = "FormList",
            FormVersion = 1,
            MajorRecordFlags = 0,
            ImportedAtUTC = DateTime.UtcNow
        };
    }

    private static GameSettingDTO CreateGameSetting(PluginDTO plugin)
    {
        return new GameSettingDTO
        {
            Game = plugin.Game,
            ModKey = plugin.ModKey,
            FormKey = CreateFormKey(plugin.ModKey, 2),
            EditorID = "GameSetting",
            FormVersion = 1,
            MajorRecordFlags = 0,
            ImportedAtUTC = DateTime.UtcNow
        };
    }

    private static GlobalDTO CreateGlobal(PluginDTO plugin)
    {
        return new GlobalDTO
        {
            Game = plugin.Game,
            ModKey = plugin.ModKey,
            FormKey = CreateFormKey(plugin.ModKey, 3),
            EditorID = "Global",
            FormVersion = 1,
            MajorRecordFlags = 0,
            ImportedAtUTC = DateTime.UtcNow
        };
    }

    private static FormKeyDTO CreateFormKey(ModKeyDTO modKey, uint id)
    {
        return new FormKeyDTO
        {
            ModKey = modKey,
            Id = id
        };
    }

    private static ModKeyDTO CreateModKey(string name, string fileName)
    {
        return new ModKeyDTO
        {
            Name = name,
            Type = 0,
            FileName = fileName
        };
    }
}
