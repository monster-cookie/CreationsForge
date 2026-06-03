using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Services.Interfaces;

public interface INPCService
{
    IList<NPCDTO> GetByModKey(ModKey modKey);
    IList<NPCDTO> GetByFormKey(FormKey formKey);
}