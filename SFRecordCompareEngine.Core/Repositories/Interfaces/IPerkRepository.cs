using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Repositories.Interfaces;

public interface IPerkRepository
{
    IList<PerkDTO> GetByModKey(ModKey modKey);
    IList<PerkDTO> GetByFormKey(FormKey formKey);
    void Save(PerkDTO dto);
}