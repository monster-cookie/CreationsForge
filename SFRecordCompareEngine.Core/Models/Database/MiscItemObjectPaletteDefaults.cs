using Mutagen.Bethesda.Plugins;
using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Models.Database;

[TableName("MiscItemObjectPaletteDefaults")]
[PrimaryKey("ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID", AutoIncrement = false)]
public class MiscItemObjectPaletteDefaults
{
    public MiscItemObjectPaletteDefaults()
    { }

    public MiscItemObjectPaletteDefaults(MiscItemDTO parent)
    {
        var dto = parent.ObjectPaletteDefaults!;
        ModKeyName = parent.ModKey.Name;
        ModKeyType = (int)parent.ModKey.Type;
        ModKeyFileName = parent.ModKey.FileName;
        FormKeyModKeyName = parent.FormKey.ModKey.Name;
        FormKeyModKeyType = (int)parent.FormKey.ModKey.Type;
        FormKeyModKeyFileName = parent.FormKey.ModKey.FileName;
        FormKeyId = (int)parent.FormKey.ID;
        Flags = dto.Flags;
        SinkMeters = dto.SinkMeters;
        SinkVariance = dto.SinkVariance;
        XYOffsetVariance = dto.XYOffsetVariance;
        FootprintSize = dto.FootprintSize;
        ScalePercent = dto.ScalePercent;
        ScaleVariance = dto.ScaleVariance;
        AngleXDegrees = dto.AngleXDegrees;
        AngleXVariance = dto.AngleXVariance;
        AngleYDegrees = dto.AngleYDegrees;
        AngleYVariance = dto.AngleYVariance;
        AngleZDegrees = dto.AngleZDegrees;
        AngleZVariance = dto.AngleZVariance;
        SlopePercent = dto.SlopePercent;
        SlopePercentVariance = dto.SlopePercentVariance;
        Density = dto.Density;
        FrequencyPercent = dto.FrequencyPercent;
        SlopeLimit = dto.SlopeLimit;
        DistanceBelowWater = dto.DistanceBelowWater;
        DistanceAboveWater = dto.DistanceAboveWater;
        ImportedAtUTC = parent.ImportedAtUTC;
    }

    [Column("ModKey_Name")] public string ModKeyName { get; set; } = string.Empty;
    [Column("ModKey_Type")] public int ModKeyType { get; set; } = (int)ModType.Master;
    [Column("ModKey_FileName")] public string ModKeyFileName { get; set; } = string.Empty;
    [Column("FormKey_ModKey_Name")] public string FormKeyModKeyName { get; set; } = string.Empty;
    [Column("FormKey_ModKey_Type")] public int FormKeyModKeyType { get; set; } = (int)ModType.Master;
    [Column("FormKey_ModKey_FileName")] public string FormKeyModKeyFileName { get; set; } = string.Empty;
    [Column("FormKey_ID")] public int FormKeyId { get; set; }
    [Column("Flags")] public string? Flags { get; set; }
    [Column("SinkMeters")] public float? SinkMeters { get; set; }
    [Column("SinkVariance")] public float? SinkVariance { get; set; }
    [Column("XYOffsetVariance")] public float? XYOffsetVariance { get; set; }
    [Column("FootprintSize")] public string? FootprintSize { get; set; }
    [Column("ScalePercent")] public float? ScalePercent { get; set; }
    [Column("ScaleVariance")] public float? ScaleVariance { get; set; }
    [Column("AngleXDegrees")] public float? AngleXDegrees { get; set; }
    [Column("AngleXVariance")] public float? AngleXVariance { get; set; }
    [Column("AngleYDegrees")] public float? AngleYDegrees { get; set; }
    [Column("AngleYVariance")] public float? AngleYVariance { get; set; }
    [Column("AngleZDegrees")] public float? AngleZDegrees { get; set; }
    [Column("AngleZVariance")] public float? AngleZVariance { get; set; }
    [Column("SlopePercent")] public float? SlopePercent { get; set; }
    [Column("SlopePercentVariance")] public float? SlopePercentVariance { get; set; }
    [Column("Density")] public float? Density { get; set; }
    [Column("FrequencyPercent")] public float? FrequencyPercent { get; set; }
    [Column("SlopeLimit")] public float? SlopeLimit { get; set; }
    [Column("DistanceBelowWater")] public float? DistanceBelowWater { get; set; }
    [Column("DistanceAboveWater")] public float? DistanceAboveWater { get; set; }
    [Column("ImportedAtUTC")] public DateTime ImportedAtUTC { get; set; }
}
