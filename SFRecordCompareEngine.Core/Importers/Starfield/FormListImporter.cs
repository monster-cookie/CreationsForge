using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Serilog;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.DTOs.Results;
using SFRecordCompareEngine.Core.Helpers;
using SFRecordCompareEngine.Core.Importers.Interfaces;
using SFRecordCompareEngine.Core.Repositories.Interfaces;
using SFRecordCompareEngine.Core.Services.Interfaces;

namespace SFRecordCompareEngine.Core.Importers.Starfield;

public class FormListImporter : ITypedRecordDetailImporter
{
    private readonly ILogger Logger = Log.ForContext<FormListImporter>();
   
    private readonly IFormListRepository FormListRepository;
    
    private readonly IFormListItemRepository FormListItemRepository;

    private readonly IStarfieldRecordReaderService StarfieldRecordReaderService;
    
    public GameRelease GameRelease => GameRelease.Starfield;

    public RecordType RecordType => new RecordType(RecordTypeCatalog.FormList.RecordID);

    public string TableName => RecordTypeCatalog.FormList.TableName;

    public FormListImporter(
        IFormListRepository formListRepository,
        IFormListItemRepository formListItemRepository,
        IStarfieldRecordReaderService starfieldRecordReaderService
    )
    {
        FormListRepository = formListRepository;
        FormListItemRepository = formListItemRepository;
        StarfieldRecordReaderService = starfieldRecordReaderService;
    }
    
    public void Import(ModKey modKey, FormKey formKey, RecordImportResultDTO resultDTO)
    {
        var record = StarfieldRecordReaderService.GetFormList(modKey, formKey);
        if (record == null)
        {
            Logger.Error("Failed to load FormList record with FormKey '{FormKey}' from mod '{ModKey}'", formKey, modKey);
            throw new FileNotFoundException($"Failed to load FormList record with FormKey '{formKey}' from mod '{modKey}'");
        }
        record.ImportedAtUTC = DateTime.UtcNow;
        
        FormListRepository.Save(record);

        foreach (var item in record.Items)
        {
            var formListItemDTO = new FormListItemDTO
            {
                ModKey = modKey,
                FormKey = record.FormKey,
                ItemModKey = item.ItemModKey,
                ItemFormKey = item.ItemFormKey,
                ImportedAtUTC = DateTime.UtcNow
            };
            FormListItemRepository.Save(formListItemDTO);
        }
        
    }
}
