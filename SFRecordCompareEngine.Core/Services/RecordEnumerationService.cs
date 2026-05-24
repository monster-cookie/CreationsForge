using System.Collections;
using System.Reflection;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Starfield;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Services.Interfaces;

namespace SFRecordCompareEngine.Core.Services;

public class RecordEnumerationService : IRecordEnumerationService
{
    private const string CellRecordType = "Cell";
    private const string WorldspaceRecordType = "Worldspace";
    private const string InteriorCellLocationKind = "InteriorCell";
    private const string WorldspaceTopCellLocationKind = "WorldspaceTopCell";
    private const string WorldspaceSubCellLocationKind = "WorldspaceSubCell";

    public IEnumerable<RecordEnumerationDTO>? GetRecords(IStarfieldModGetter plugin, string recordType)
    {
        if (recordType.Equals(CellRecordType, StringComparison.Ordinal))
        {
            return GetCellRecords(plugin);
        }

        if (recordType.Equals(WorldspaceRecordType, StringComparison.Ordinal))
        {
            return plugin.Worldspaces.Select(worldspace => new RecordEnumerationDTO
            {
                RecordType = recordType,
                Record = worldspace
            });
        }

        return GetRawRecords(plugin, recordType)?
            .Cast<object>()
            .Select(record => new RecordEnumerationDTO
            {
                RecordType = recordType,
                Record = record
            });
    }

    public IEnumerable? GetRawRecords(IStarfieldModGetter plugin, string recordType)
    {
        if (recordType.Equals(CellRecordType, StringComparison.Ordinal)
            || recordType.Equals(WorldspaceRecordType, StringComparison.Ordinal))
        {
            return GetRecords(plugin, recordType)?.Select(record => record.Record);
        }

        return GetRecordsFromMutagenTypeOption(plugin, recordType) ?? GetRecordsFromPluginProperty(plugin, recordType);
    }

    private static IEnumerable<RecordEnumerationDTO> GetCellRecords(IStarfieldModGetter plugin)
    {
        var recordsByFormKey = new Dictionary<string, RecordEnumerationDTO>(StringComparer.OrdinalIgnoreCase);
        AddInteriorCells(plugin, recordsByFormKey);
        AddWorldspaceCells(plugin, recordsByFormKey);
        return recordsByFormKey.Values;
    }

    private static void AddInteriorCells(IStarfieldModGetter plugin, IDictionary<string, RecordEnumerationDTO> recordsByFormKey)
    {
        foreach (var block in plugin.Cells)
        {
            foreach (var subBlock in block.SubBlocks)
            {
                foreach (var item in subBlock.Cells.Select((cell, index) => new { cell, index }))
                {
                    AddCell(recordsByFormKey, item.cell, new CellGroupLocationDTO
                    {
                        ModKey = string.Empty,
                        CellFormID = string.Empty,
                        LocationKind = InteriorCellLocationKind,
                        BlockNumber = block.BlockNumber,
                        SubBlockNumber = subBlock.BlockNumber,
                        CellIndex = item.index,
                        BlockGroupType = block.GroupType.ToString(),
                        SubBlockGroupType = subBlock.GroupType.ToString(),
                        BlockLastModified = block.LastModified,
                        SubBlockLastModified = subBlock.LastModified,
                        BlockUnknown = block.Unknown,
                        SubBlockUnknown = subBlock.Unknown,
                        ImportedAtUtc = string.Empty
                    });
                }
            }
        }
    }

    private static void AddWorldspaceCells(IStarfieldModGetter plugin, IDictionary<string, RecordEnumerationDTO> recordsByFormKey)
    {
        foreach (var worldspace in plugin.Worldspaces)
        {
            var worldspaceFormId = GetFormId(worldspace);
            if (worldspace.TopCell is not null)
            {
                AddCell(recordsByFormKey, worldspace.TopCell, new CellGroupLocationDTO
                {
                    ModKey = string.Empty,
                    CellFormID = string.Empty,
                    LocationKind = WorldspaceTopCellLocationKind,
                    WorldspaceFormID = worldspaceFormId,
                    CellIndex = 0,
                    ImportedAtUtc = string.Empty
                });
            }

            foreach (var block in worldspace.SubCells)
            {
                foreach (var subBlock in block.Items)
                {
                    foreach (var item in subBlock.Items.Select((cell, index) => new { cell, index }))
                    {
                        AddCell(recordsByFormKey, item.cell, new CellGroupLocationDTO
                        {
                            ModKey = string.Empty,
                            CellFormID = string.Empty,
                            LocationKind = WorldspaceSubCellLocationKind,
                            WorldspaceFormID = worldspaceFormId,
                            BlockX = block.BlockNumberX,
                            BlockY = block.BlockNumberY,
                            SubBlockX = subBlock.BlockNumberX,
                            SubBlockY = subBlock.BlockNumberY,
                            CellIndex = item.index,
                            BlockGroupType = block.GroupType.ToString(),
                            SubBlockGroupType = subBlock.GroupType.ToString(),
                            BlockLastModified = block.LastModified,
                            SubBlockLastModified = subBlock.LastModified,
                            BlockUnknown = block.Unknown,
                            SubBlockUnknown = subBlock.Unknown,
                            ImportedAtUtc = string.Empty
                        });
                    }
                }
            }
        }
    }

    private static void AddCell(IDictionary<string, RecordEnumerationDTO> recordsByFormKey, ICellGetter cell, CellGroupLocationDTO location)
    {
        var formKey = GetFormKey(cell);
        if (formKey is null) return;

        var formId = FormIdNormalizer.NormalizeFromFormKey(formKey);
        location.CellFormID = formId;
        if (!recordsByFormKey.TryGetValue(formKey, out var existingRecord))
        {
            existingRecord = new RecordEnumerationDTO
            {
                RecordType = CellRecordType,
                Record = cell
            };
            recordsByFormKey.Add(formKey, existingRecord);
        }

        existingRecord.CellGroupLocations.Add(location);
    }

    private static string? GetFormId(IFormKeyGetter record)
    {
        var formKey = GetFormKey(record);
        return formKey is null ? null : FormIdNormalizer.NormalizeFromFormKey(formKey);
    }

    private static string? GetFormKey(IFormKeyGetter record)
    {
        var formKey = record.FormKey.ToString();
        return string.IsNullOrWhiteSpace(formKey)
            ? null
            : FormKeyTextNormalizer.NormalizeReferenceValue(formKey);
    }

    private static IEnumerable? GetRecordsFromMutagenTypeOption(IStarfieldModGetter plugin, string recordType)
    {
        var method = typeof(TypeOptionSolidifierMixIns)
            .GetMethods()
            .Where(method => method.Name.Equals(recordType, StringComparison.Ordinal))
            .FirstOrDefault(method =>
            {
                var parameters = method.GetParameters();
                return parameters.Length == 1
                       && parameters[0].ParameterType.IsAssignableFrom(typeof(IEnumerable<IStarfieldModGetter>));
            });

        return method?.Invoke(null, [new[] { plugin }]) as IEnumerable;
    }

    private static IEnumerable? GetRecordsFromPluginProperty(IStarfieldModGetter plugin, string recordType)
    {
        var propertyNames = new[]
        {
            recordType,
            $"{recordType}s",
            recordType.EndsWith("y", StringComparison.OrdinalIgnoreCase)
                ? $"{recordType[..^1]}ies"
                : $"{recordType}s"
        };

        foreach (var propertyName in propertyNames.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            var property = plugin.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
            if (property?.GetValue(plugin) is IEnumerable records) return records;
        }

        return null;
    }
}
