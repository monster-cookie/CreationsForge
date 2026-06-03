using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Models.Database;
using Shouldly;

namespace SFRecordCompareEngine.UnitTests.DTOs.Records;

public class ScriptingAdapterPropertyDTOTests
{
    [Fact]
    public void Constructor_MapsModel()
    {
        var importedAtUtc = new DateTime(2026, 6, 2, 12, 45, 0, DateTimeKind.Utc);
        var model = new ScriptingAdapterProperty
        {
            ModKeyName = "Example",
            ModKeyType = (int)ModType.Master,
            ModKeyFileName = "Example.esm",
            RecordType = "Perk",
            FormKeyModKeyName = "Origin",
            FormKeyModKeyType = (int)ModType.Master,
            FormKeyModKeyFileName = "Origin.esm",
            FormKeyId = 321,
            ScriptingAdapterName = "ExampleScript",
            PropertyIndex = 4,
            Name = "ChanceToSpawn",
            MutagenObjectType = "ScriptIntProperty",
            DataInt = 75,
            ImportedAtUTC = importedAtUtc
        };

        var result = new ScriptingAdapterPropertyDTO(model);

        result.ModKey.Name.ShouldBe("Example");
        result.ModKey.Type.ShouldBe(ModType.Master);
        result.FormKey.ModKey.Name.ShouldBe("Origin");
        result.FormKey.ModKey.Type.ShouldBe(ModType.Master);
        result.FormKey.ID.ShouldBe(321U);
        result.RecordType.ShouldBe("Perk");
        result.ScriptingAdapterName.ShouldBe("ExampleScript");
        result.PropertyIndex.ShouldBe(4);
        result.Name.ShouldBe("ChanceToSpawn");
        result.MutagenObjectType.ShouldBe("ScriptIntProperty");
        result.DataInt.ShouldBe(75);
        result.ImportedAtUTC.ShouldBe(importedAtUtc);
    }
}
