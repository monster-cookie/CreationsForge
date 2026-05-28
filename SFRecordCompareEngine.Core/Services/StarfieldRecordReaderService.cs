using System.IO;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Services.Interfaces;

namespace SFRecordCompareEngine.Core.Services;

public class StarfieldRecordReaderService : IStarfieldRecordReaderService
{
    public IReadOnlyList<FormKey> GetFormListFormKeys(PluginDTO plugin)
    {
        var mod = LoadMod(plugin.ModKey);
        return mod.FormLists.Select(formList => formList.FormKey).ToList();
    }

    public FormListDTO? GetFormList(ModKey modKey, FormKey formKey)
    {
        var mod = LoadMod(modKey);
        mod.FormLists.TryGetValue(formKey, out var record);
        if (record == null) return null;

        return new FormListDTO
        {
            ModKey = modKey,
            FormKey = record.FormKey,
            EditorID = record.EditorID ?? string.Empty,
            FormVersion = record.FormVersion,
            StarfieldMajorRecordFlags = record.StarfieldMajorRecordFlags,
            Version2 = record.Version2,
            VersionControl = (int)record.VersionControl,
            ImportedAtUTC = DateTime.UtcNow,
            AddToListFormKey = record.AddToList.FormKey,
            Items = record.Items.Select(item =>
            {
                item.TryGetModKey(out var itemModKey);
                return new FormListItemDataDTO
                {
                    ItemModKey = itemModKey,
                    ItemFormKey = item.FormKey
                };
            }).ToList()
        };
    }

    private static IStarfieldModGetter LoadMod(ModKey modKey)
    {
        var environment = GameEnvironment.Typical.Starfield(StarfieldRelease.Starfield);
        return StarfieldMod.Create(StarfieldRelease.Starfield)
            .FromPath(Path.Join(environment.DataFolderPath, modKey.FileName))
            .WithLoadOrderFromHeaderMasters()
            .WithDataFolder(environment.DataFolderPath)
            .Construct();
    }
}
