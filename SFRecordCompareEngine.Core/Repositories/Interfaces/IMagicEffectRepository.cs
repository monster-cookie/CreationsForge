using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Repositories.Interfaces;

public interface IMagicEffectRepository
{
    IList<MagicEffectDTO> GetByModKey(ModKey modKey);
    IList<MagicEffectDTO> GetByFormKey(FormKey formKey);
    void Save(MagicEffectDTO dto);
}