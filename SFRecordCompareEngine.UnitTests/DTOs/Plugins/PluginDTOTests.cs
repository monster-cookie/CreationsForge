using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.Models.Database;
using Shouldly;

namespace SFRecordCompareEngine.UnitTests.DTOs.Plugins;

public class PluginDTOTests
{
    [Fact]
    public void PluginConstructor_MapsPersistedMetadataFromDTO()
    {
        var dto = new PluginDTO
        {
            ModKey = new ModKey("Example", ModType.Master),
            LoadOrderIndex = 4,
            Enabled = true,
            ExistsOnDisk = true,
            ImportState = "Current",
            HeaderFlags = (StarfieldModHeader.HeaderFlag)7,
            FormVersion = 42,
            Author = "Author",
            Branch = "Branch",
            InteriorCellCount = 5,
            SourceLastWriteUTCTicks = 123,
            SourceFileSizeBytes = 456,
            LastCheckedUTC = new DateTime(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc),
            LastImportedUTC = new DateTime(2026, 2, 3, 4, 5, 6, DateTimeKind.Utc),
            InvalidatedAtUTC = new DateTime(2026, 3, 4, 5, 6, 7, DateTimeKind.Utc)
        };

        var model = new Plugin(dto);

        model.ModKeyName.ShouldBe(dto.ModKey.Name);
        model.ModKeyFileName.ShouldBe(dto.ModKey.FileName);
        model.ModKeyType.ShouldBe((int)dto.ModKey.Type);
        model.LoadOrderIndex.ShouldBe(dto.LoadOrderIndex);
        model.Enabled.ShouldBe(dto.Enabled);
        model.ExistsOnDisk.ShouldBe(dto.ExistsOnDisk);
        model.ImportState.ShouldBe(dto.ImportState);
        model.HeaderFlags.ShouldBe((int)dto.HeaderFlags);
        model.FormVersion.ShouldBe(dto.FormVersion);
        model.Author.ShouldBe(dto.Author);
        model.Branch.ShouldBe(dto.Branch);
        model.InteriorCellCount.ShouldBe(dto.InteriorCellCount.Value);
        model.SourceLastWriteUTCTicks.ShouldBe(dto.SourceLastWriteUTCTicks);
        model.SourceFileSizeBytes.ShouldBe(dto.SourceFileSizeBytes);
        model.LastCheckedUTC.ShouldBe(dto.LastCheckedUTC);
        model.LastImportedUTC.ShouldBe(dto.LastImportedUTC);
        model.InvalidatedAtUTC.ShouldBe(dto.InvalidatedAtUTC);
    }

    [Fact]
    public void PluginDTOConstructor_MapsPersistedMetadataFromModel()
    {
        var model = new Plugin
        {
            ModKeyName = "Example",
            ModKeyFileName = "Example.esm",
            ModKeyType = (int)ModType.Master,
            LoadOrderIndex = 4,
            Enabled = true,
            ExistsOnDisk = true,
            ImportState = "Current",
            HeaderFlags = 7,
            FormVersion = 42,
            Author = "Author",
            Branch = "Branch",
            InteriorCellCount = 5,
            SourceLastWriteUTCTicks = 123,
            SourceFileSizeBytes = 456,
            LastCheckedUTC = new DateTime(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc),
            LastImportedUTC = new DateTime(2026, 2, 3, 4, 5, 6, DateTimeKind.Utc),
            InvalidatedAtUTC = new DateTime(2026, 3, 4, 5, 6, 7, DateTimeKind.Utc)
        };

        var dto = new PluginDTO(model);

        dto.ModKey.Name.ShouldBe(model.ModKeyName);
        dto.ModKey.FileName.String.ShouldBe(model.ModKeyFileName);
        dto.ModKey.Type.ShouldBe((ModType)model.ModKeyType);
        dto.LoadOrderIndex.ShouldBe(model.LoadOrderIndex);
        dto.Enabled.ShouldBe(model.Enabled);
        dto.ExistsOnDisk.ShouldBe(model.ExistsOnDisk);
        dto.ImportState.ShouldBe(model.ImportState);
        dto.HeaderFlags.ShouldBe((StarfieldModHeader.HeaderFlag)model.HeaderFlags);
        dto.FormVersion.ShouldBe(model.FormVersion);
        dto.Author.ShouldBe(model.Author);
        dto.Branch.ShouldBe(model.Branch);
        dto.InteriorCellCount.ShouldBe(model.InteriorCellCount);
        dto.SourceLastWriteUTCTicks.ShouldBe(model.SourceLastWriteUTCTicks.Value);
        dto.SourceFileSizeBytes.ShouldBe(model.SourceFileSizeBytes.Value);
        dto.LastCheckedUTC.ShouldBe(model.LastCheckedUTC);
        dto.LastImportedUTC.ShouldBe(model.LastImportedUTC);
        dto.InvalidatedAtUTC.ShouldBe(model.InvalidatedAtUTC);
    }
}
