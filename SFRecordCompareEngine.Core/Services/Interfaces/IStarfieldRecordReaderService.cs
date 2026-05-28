using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Services.Interfaces;

public interface IStarfieldRecordReaderService
{
    IReadOnlyList<FormKey> GetFormListFormKeys(PluginDTO plugin);
    FormListDTO? GetFormList(ModKey modKey, FormKey formKey);
}
