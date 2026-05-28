using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;
using Moq;
using NPoco;
using SFRecordCompareEngine.Core.Database.Interfaces;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.DTOs.Results;
using SFRecordCompareEngine.Core.Enums;
using SFRecordCompareEngine.Core.Repositories.Interfaces;
using SFRecordCompareEngine.Core.Services;
using SFRecordCompareEngine.Core.Services.Interfaces;
using Shouldly;

namespace SFRecordCompareEngine.UnitTests.Services;

public class PluginImportServiceTests
{
    [Fact]
    public async Task InitializeAndImportAsync_WhenLoadOrderIsEmpty_InitializesSchemaAndCompletesTransaction()
    {
        var context = CreateContext();
        context.PluginService.Setup(x => x.GetLoadOrder()).Returns(new List<PluginLoadOrderEntryDTO>());

        var result = await context.Sut.InitializeAndImportAsync(null, CancellationToken.None);

        result.PluginsDiscovered.ShouldBe(0);
        context.DatabaseSchemaInitializer.Verify(x => x.Initialize(), Times.Once);
        context.Transaction.Verify(x => x.Complete(), Times.Once);
    }

    [Fact]
    public async Task InitializeAndImportAsync_WhenPluginIsUnsupported_SavesUnsupportedPlugin()
    {
        var context = CreateContext();
        var entry = CreateLoadOrderEntry("BlueprintShips-Starfield.esm");
        context.PluginService.Setup(x => x.GetLoadOrder()).Returns(new List<PluginLoadOrderEntryDTO> { entry });
        context.PluginReader.Setup(x => x.GetSourceInfo(entry.PluginPath)).Returns(new PluginSourceInfoDTO
        {
            Exists = true,
            LastWriteUTCTicks = 123,
            FileSizeBytes = 456
        });
        PluginDTO? savedPlugin = null;
        context.PluginRepository.Setup(x => x.Save(It.IsAny<PluginDTO>())).Callback<PluginDTO>(dto => savedPlugin = dto);

        var result = await context.Sut.InitializeAndImportAsync(null, CancellationToken.None);

        result.PluginsUnsupported.ShouldBe(1);
        savedPlugin.ShouldNotBeNull();
        savedPlugin.ImportState.ShouldBe(nameof(PluginImportState.Unsupported));
        savedPlugin.SourceLastWriteUTCTicks.ShouldBe(123);
        savedPlugin.SourceFileSizeBytes.ShouldBe(456);
        context.RecordImportService.Verify(x => x.ImportPluginRecords(It.IsAny<PluginDTO>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task InitializeAndImportAsync_WhenPluginIsMissing_SavesMissingPlugin()
    {
        var context = CreateContext();
        var entry = CreateLoadOrderEntry("Example.esm");
        context.PluginService.Setup(x => x.GetLoadOrder()).Returns(new List<PluginLoadOrderEntryDTO> { entry });
        context.PluginReader.Setup(x => x.GetSourceInfo(entry.PluginPath)).Returns(new PluginSourceInfoDTO
        {
            Exists = false
        });
        PluginDTO? savedPlugin = null;
        context.PluginRepository.Setup(x => x.Save(It.IsAny<PluginDTO>())).Callback<PluginDTO>(dto => savedPlugin = dto);

        var result = await context.Sut.InitializeAndImportAsync(null, CancellationToken.None);

        result.PluginsMissing.ShouldBe(1);
        savedPlugin.ShouldNotBeNull();
        savedPlugin.ImportState.ShouldBe(nameof(PluginImportState.Missing));
        savedPlugin.ExistsOnDisk.ShouldBeFalse();
        context.PluginReader.Verify(x => x.GetMetadata(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task InitializeAndImportAsync_WhenPluginIsUnchanged_UpdatesLastCheckedAndSkipsImport()
    {
        var context = CreateContext();
        var entry = CreateLoadOrderEntry("Example.esm");
        var existingPlugin = new PluginDTO
        {
            ModKey = entry.ModKey,
            SourceLastWriteUTCTicks = 123,
            SourceFileSizeBytes = 456
        };
        context.PluginService.Setup(x => x.GetLoadOrder()).Returns(new List<PluginLoadOrderEntryDTO> { entry });
        context.PluginRepository.Setup(x => x.GetByModKey(entry.ModKey)).Returns(existingPlugin);
        context.PluginReader.Setup(x => x.GetSourceInfo(entry.PluginPath)).Returns(new PluginSourceInfoDTO
        {
            Exists = true,
            LastWriteUTCTicks = 123,
            FileSizeBytes = 456
        });

        var result = await context.Sut.InitializeAndImportAsync(null, CancellationToken.None);

        result.PluginsUnchanged.ShouldBe(1);
        existingPlugin.LastCheckedUTC.ShouldNotBe(default);
        context.PluginRepository.Verify(x => x.Save(existingPlugin), Times.Once);
        context.RecordImportService.Verify(x => x.ImportPluginRecords(It.IsAny<PluginDTO>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task InitializeAndImportAsync_WhenPluginIsNew_SavesCurrentPluginAndAggregatesRecordResults()
    {
        var context = CreateContext();
        var entry = CreateLoadOrderEntry("Example.esm");
        context.PluginService.Setup(x => x.GetLoadOrder()).Returns(new List<PluginLoadOrderEntryDTO> { entry });
        context.PluginReader.Setup(x => x.GetSourceInfo(entry.PluginPath)).Returns(new PluginSourceInfoDTO
        {
            Exists = true,
            LastWriteUTCTicks = 123,
            FileSizeBytes = 456
        });
        context.PluginReader.Setup(x => x.GetMetadata(entry.PluginPath)).Returns(CreateMetadata(entry.ModKey));
        context.RecordImportService.Setup(x => x.ImportPluginRecords(It.IsAny<PluginDTO>(), It.IsAny<CancellationToken>())).Returns(new RecordImportResultDTO
        {
            ModKey = entry.ModKey,
            RecordTypes = new List<RecordTypeImportResultDTO>
            {
                new()
                {
                    RecordType = "FLST",
                    HeaderImportSupported = true,
                    HeadersImported = 2,
                    DetailRowsImported = 3,
                    FormListItemsImported = 4,
                    RecordsFailed = 5
                },
                new()
                {
                    RecordType = "UNSP",
                    HeaderImportSupported = false
                }
            }
        });
        PluginDTO? savedPlugin = null;
        context.PluginRepository.Setup(x => x.Save(It.IsAny<PluginDTO>())).Callback<PluginDTO>(dto => savedPlugin = dto);

        var result = await context.Sut.InitializeAndImportAsync(null, CancellationToken.None);

        result.PluginsImported.ShouldBe(1);
        result.RecordHeadersImported.ShouldBe(2);
        result.TypedRecordDetailRowsImported.ShouldBe(3);
        result.FormListItemsImported.ShouldBe(4);
        result.RecordImportFailures.ShouldBe(5);
        result.UnsupportedRecordTypes.ShouldBe(1);
        savedPlugin.ShouldNotBeNull();
        savedPlugin.ImportState.ShouldBe(nameof(PluginImportState.Current));
        savedPlugin.SourceLastWriteUTCTicks.ShouldBe(123);
        savedPlugin.SourceFileSizeBytes.ShouldBe(456);
        savedPlugin.InteriorCellCount.ShouldBe(9);
    }

    [Fact]
    public async Task InitializeAndImportAsync_WhenPluginChanged_IncrementsChangedAndInvalidated()
    {
        var context = CreateContext();
        var entry = CreateLoadOrderEntry("Example.esm");
        context.PluginService.Setup(x => x.GetLoadOrder()).Returns(new List<PluginLoadOrderEntryDTO> { entry });
        context.PluginRepository.Setup(x => x.GetByModKey(entry.ModKey)).Returns(new PluginDTO
        {
            ModKey = entry.ModKey,
            SourceLastWriteUTCTicks = 1,
            SourceFileSizeBytes = 2
        });
        context.PluginReader.Setup(x => x.GetSourceInfo(entry.PluginPath)).Returns(new PluginSourceInfoDTO
        {
            Exists = true,
            LastWriteUTCTicks = 123,
            FileSizeBytes = 456
        });
        context.PluginReader.Setup(x => x.GetMetadata(entry.PluginPath)).Returns(CreateMetadata(entry.ModKey));

        var result = await context.Sut.InitializeAndImportAsync(null, CancellationToken.None);

        result.PluginsChanged.ShouldBe(1);
        result.PluginsInvalidated.ShouldBe(1);
        result.PluginsImported.ShouldBe(1);
    }

    [Fact]
    public async Task InitializeAndImportAsync_WhenMasterReferencePluginExists_SavesMasterReference()
    {
        var context = CreateContext();
        var entry = CreateLoadOrderEntry("Example.esm");
        var masterModKey = new ModKey("Master", ModType.Master);
        context.PluginService.Setup(x => x.GetLoadOrder()).Returns(new List<PluginLoadOrderEntryDTO> { entry });
        context.PluginReader.Setup(x => x.GetSourceInfo(entry.PluginPath)).Returns(new PluginSourceInfoDTO
        {
            Exists = true,
            LastWriteUTCTicks = 123,
            FileSizeBytes = 456
        });
        context.PluginReader.Setup(x => x.GetMetadata(entry.PluginPath)).Returns(CreateMetadata(entry.ModKey, masterModKey));
        context.PluginRepository.Setup(x => x.GetByModKey(masterModKey)).Returns(new PluginDTO
        {
            ModKey = masterModKey,
            LoadOrderIndex = 2
        });

        var result = await context.Sut.InitializeAndImportAsync(null, CancellationToken.None);

        result.MasterReferencesImported.ShouldBe(1);
        context.PluginMasterReferencesRepository.Verify(x => x.Save(It.Is<PluginMasterReferenceDTO>(dto =>
            dto.ModKey == masterModKey &&
            dto.ParentModKey == entry.ModKey &&
            dto.MasterReferenceIndex == 2 &&
            dto.ParentLoadOrderIndex == entry.LoadOrderIndex)), Times.Once);
    }

    private static PluginImportServiceTestContext CreateContext()
    {
        var databaseSchemaInitializer = new Mock<IDatabaseSchemaInitializer>();
        var database = new Mock<IDatabase>();
        var transaction = new Mock<ITransaction>();
        database.Setup(x => x.GetTransaction()).Returns(transaction.Object);
        var pluginService = new Mock<IPluginService>();
        var pluginRepository = new Mock<IPluginRepository>();
        var pluginMasterReferencesRepository = new Mock<IPluginMasterReferencesRepository>();
        var recordImportService = new Mock<IRecordImportService>();
        var pluginReader = new Mock<IStarfieldPluginReaderService>();
        recordImportService.Setup(x => x.ImportPluginRecords(It.IsAny<PluginDTO>(), It.IsAny<CancellationToken>())).Returns<PluginDTO, CancellationToken>((plugin, _) => new RecordImportResultDTO
        {
            ModKey = plugin.ModKey
        });

        return new PluginImportServiceTestContext(
            new PluginImportService(
                databaseSchemaInitializer.Object,
                database.Object,
                pluginService.Object,
                pluginRepository.Object,
                pluginMasterReferencesRepository.Object,
                recordImportService.Object,
                pluginReader.Object),
            databaseSchemaInitializer,
            database,
            transaction,
            pluginService,
            pluginRepository,
            pluginMasterReferencesRepository,
            recordImportService,
            pluginReader);
    }

    private static PluginLoadOrderEntryDTO CreateLoadOrderEntry(string pluginFileName)
    {
        var modKey = new ModKey(Path.GetFileNameWithoutExtension(pluginFileName), ModType.Master);
        return new PluginLoadOrderEntryDTO
        {
            ModKey = modKey,
            PluginFileName = pluginFileName,
            PluginPath = pluginFileName,
            LoadOrderIndex = 1,
            Enabled = true
        };
    }

    private static StarfieldPluginMetadataDTO CreateMetadata(ModKey modKey, params ModKey[] masterReferences)
    {
        return new StarfieldPluginMetadataDTO
        {
            ModKey = modKey,
            HeaderFlags = (StarfieldModHeader.HeaderFlag)7,
            FormVersion = 44,
            Author = "Author",
            InteriorCellCount = 9,
            MasterReferences = masterReferences.ToList()
        };
    }

    private sealed record PluginImportServiceTestContext(
        PluginImportService Sut,
        Mock<IDatabaseSchemaInitializer> DatabaseSchemaInitializer,
        Mock<IDatabase> Database,
        Mock<ITransaction> Transaction,
        Mock<IPluginService> PluginService,
        Mock<IPluginRepository> PluginRepository,
        Mock<IPluginMasterReferencesRepository> PluginMasterReferencesRepository,
        Mock<IRecordImportService> RecordImportService,
        Mock<IStarfieldPluginReaderService> PluginReader);
}
