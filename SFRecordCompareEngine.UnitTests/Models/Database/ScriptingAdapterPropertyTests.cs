using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Models.Database;
using Shouldly;

namespace SFRecordCompareEngine.UnitTests.Models.Database;

public class ScriptingAdapterPropertyTests
{
    [Fact]
    public void Constructor_MapsDTO()
    {
        var modKey = new ModKey("Example", ModType.Master);
        var formKey = new FormKey(modKey, 123);
        var objectFormKey = new FormKey(new ModKey("ObjectPlugin", ModType.Plugin), 456);
        var importedAtUtc = new DateTime(2026, 6, 2, 12, 30, 0, DateTimeKind.Utc);
        var dto = new ScriptingAdapterPropertyDTO
        {
            ModKey = modKey,
            RecordType = "NPC",
            FormKey = formKey,
            ScriptingAdapterName = "ScriptName",
            PropertyIndex = 2,
            Name = "PropertyName",
            MutagenObjectType = "ScriptObjectProperty",
            ObjectFormKey = objectFormKey,
            ObjectAlias = 7,
            ObjectUnused = 9,
            ImportedAtUTC = importedAtUtc
        };

        var result = new ScriptingAdapterProperty(dto);

        result.ModKeyName.ShouldBe(modKey.Name);
        result.ModKeyType.ShouldBe((int)modKey.Type);
        result.ModKeyFileName.ShouldBe(modKey.FileName);
        result.RecordType.ShouldBe("NPC");
        result.FormKeyId.ShouldBe((int)formKey.ID);
        result.ScriptingAdapterName.ShouldBe("ScriptName");
        result.PropertyIndex.ShouldBe(2);
        result.Name.ShouldBe("PropertyName");
        result.MutagenObjectType.ShouldBe("ScriptObjectProperty");
        result.ObjectModKeyName.ShouldBe(objectFormKey.ModKey.Name);
        result.ObjectModKeyType.ShouldBe((int)objectFormKey.ModKey.Type);
        result.ObjectModKeyFileName.ShouldBe(objectFormKey.ModKey.FileName);
        result.ObjectFormKeyId.ShouldBe((int)objectFormKey.ID);
        result.ObjectAlias.ShouldBe((short)7);
        result.ObjectUnused.ShouldBe((ushort)9);
        result.ImportedAtUTC.ShouldBe(importedAtUtc);
    }
}
