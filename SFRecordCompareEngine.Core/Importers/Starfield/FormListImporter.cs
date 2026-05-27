using System.Configuration;
using System.IO;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;
using Serilog;
using SFRecordCompareEngine.Core.Configuration.Interfaces;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.DTOs.Results;
using SFRecordCompareEngine.Core.Helpers;
using SFRecordCompareEngine.Core.Importers.Interfaces;
using SFRecordCompareEngine.Core.Repositories.Interfaces;

namespace SFRecordCompareEngine.Core.Importers.Starfield;

public class FormListImporter : ITypedRecordDetailImporter
{
    private readonly ILogger Logger = Log.ForContext<FormListImporter>();
   
    private readonly IFormListRepository FormListRepository;
    
    private readonly IFormListItemRepository FormListItemRepository;
    
    public GameRelease GameRelease => GameRelease.Starfield;

    public RecordType RecordType => new RecordType(RecordTypeCatalog.FormList.RecordID);

    public string TableName => RecordTypeCatalog.FormList.TableName;

    public FormListImporter(
        IFormListRepository formListRepository,
        IFormListItemRepository formListItemRepository
    )
    {
        FormListRepository = formListRepository;
        FormListItemRepository = formListItemRepository;
    }
    
    public void Import(ModKey modKey, FormKey formKey, RecordImportResultDTO resultDTO)
    {
        var modPath = Path.Join(GameEnvironment.Typical.Starfield(StarfieldRelease.Starfield).DataFolderPath, modKey.FileName);
        var mod = StarfieldMod.Create(StarfieldRelease.Starfield)
            .FromPath(modPath)
            .WithLoadOrderFromHeaderMasters()
            .WithDataFolder(GameEnvironment.Typical.Starfield(StarfieldRelease.Starfield).DataFolderPath)
            .Construct();
        if (mod == null)
        {
            Logger.Error("Failed to load mod '{ModKey}' for FormList record with FormKey '{FormKey}' from path {Path}", modKey, formKey, modPath);
            throw new FileNotFoundException($"Failed to load mod '{modKey}' for FormList record with FormKey '{formKey}' from path {modPath}");
        }
        var modHear = mod.ModHeader;
        
        mod.FormLists.TryGetValue(formKey, out var record);
        if (record == null)
        {
            Logger.Error("Failed to load FormList record with FormKey '{FormKey}' from mod '{ModKey}'", formKey, modKey);
            throw new FileNotFoundException($"Failed to load FormList record with FormKey '{formKey}' from mod '{modKey}'");
        }

        var formListDTO = new FormListDTO
        {
            ModKey = modKey,
            FormKey = record.FormKey,
            EditorID = record.EditorID ?? string.Empty,
            FormVersion = record.FormVersion,
            StarfieldMajorRecordFlags = record.StarfieldMajorRecordFlags,
            Version2 = record.Version2,
            VersionControl = (int)record.VersionControl,
            ImportedAtUtc = DateTime.UtcNow,
            AddToListFormKey = record.AddToList.FormKey
        };
        FormListRepository.Save(formListDTO);
        
    }
}