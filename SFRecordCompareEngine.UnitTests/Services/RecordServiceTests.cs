using SFRecordCompareEngine.Core.Database;
using SFRecordCompareEngine.Core.Database.Interfaces;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Models.Database;
using SFRecordCompareEngine.Core.Repositories;
using SFRecordCompareEngine.Core.Services;
using SFRecordCompareEngine.Migrations;
using Shouldly;

namespace SFRecordCompareEngine.UnitTests.Services;

public class RecordServiceTests : IDisposable
{
    private readonly string DatabaseDirectory = Path.Combine(Path.GetTempPath(), "SFRecordCompareEngineTests", Guid.NewGuid().ToString("N"));
    private readonly ISqliteConnectionFactory ConnectionFactory;
    private readonly PluginRepository PluginRepository = new();
    private readonly RecordHeaderRepository RecordHeaderRepository = new();
    private readonly RecordService Sut;

    public RecordServiceTests()
    {
        var options = new SqliteDatabaseOptions
        {
            DatabaseDirectory = DatabaseDirectory
        };

        ConnectionFactory = new SqliteConnectionFactory(options);
        new DatabaseSchemaInitializer(ConnectionFactory, new DatabaseMigrationRunner()).Initialize();
        Sut = new RecordService(ConnectionFactory, RecordHeaderRepository);
    }

    public void Dispose()
    {
        if (Directory.Exists(DatabaseDirectory))
        {
            Directory.Delete(DatabaseDirectory, true);
        }
    }

    [Fact]
    public void ResolveReferenceDisplayValue_WhenRecordHeaderHasEditorId_ReturnsEditorId()
    {
        using var database = ConnectionFactory.OpenDatabase();
        InsertRecordHeader(database, "02F7C8:Starfield.esm", "ChargenPreset");

        var result = Sut.ResolveReferenceDisplayValue("02F7C8:Starfield.esm");

        result.ShouldBe("ChargenPreset");
    }

    [Theory]
    [InlineData("formid:02F7C8:Starfield.esm", "ChargenPreset")]
    [InlineData("02F7C8:Starfield.esm <Starfield.IStarfieldMajorRecordGetter>", "ChargenPreset")]
    [InlineData("02F7C8:Starfield.esm<Starfield.IStarfieldMajorRecordGetter>", "ChargenPreset")]
    public void ResolveReferenceDisplayValue_WhenReferenceHasMutagenFormatting_ReturnsEditorId(string referenceValue, string expected)
    {
        using var database = ConnectionFactory.OpenDatabase();
        InsertRecordHeader(database, "02F7C8:Starfield.esm", "ChargenPreset");

        var result = Sut.ResolveReferenceDisplayValue(referenceValue);

        result.ShouldBe(expected);
    }

    [Fact]
    public void ResolveReferenceDisplayValue_WhenRecordHeaderHasNoEditorId_ReturnsNormalizedReference()
    {
        using var database = ConnectionFactory.OpenDatabase();
        InsertRecordHeader(database, "02F7C8:Starfield.esm", null);

        var result = Sut.ResolveReferenceDisplayValue("02F7C8:Starfield.esm<Starfield.IStarfieldMajorRecordGetter>");

        result.ShouldBe("02F7C8:Starfield.esm");
    }

    [Fact]
    public void ResolveReferenceDisplayValue_WhenReferenceIsNotFound_ReturnsNormalizedReference()
    {
        var result = Sut.ResolveReferenceDisplayValue("02F7C8:Starfield.esm<Starfield.IStarfieldMajorRecordGetter>");

        result.ShouldBe("02F7C8:Starfield.esm");
    }

    [Fact]
    public void ResolveReferenceDisplayValue_WhenReferenceIsBlank_ReturnsNull()
    {
        var result = Sut.ResolveReferenceDisplayValue(" ");

        result.ShouldBeNull();
    }

    private void InsertRecordHeader(NPoco.IDatabase database, string formKey, string? editorId)
    {
        PluginRepository.UpsertPlugin(database, new PluginMetadataDTO
        {
            ModKey = "Starfield.esm",
            GameRelease = "Starfield",
            LoadOrderIndex = 0,
            PluginFileName = "Starfield.esm",
            Enabled = true,
            ExistsOnDisk = true,
            ImportState = PluginImportState.Current.ToString(),
            LastCheckedUtc = DateTimeOffset.UtcNow.ToString("O")
        });

        RecordHeaderRepository.Upsert(database, new RecordHeaderDTO
        {
            ModKey = "Starfield.esm",
            FormID = FormIdNormalizer.NormalizeFromFormKey(formKey),
            RecordType = "FormList",
            FormKey = formKey,
            EditorID = editorId,
            PluginFileName = "Starfield.esm",
            ImportedAtUtc = DateTimeOffset.UtcNow.ToString("O")
        });
    }
}
