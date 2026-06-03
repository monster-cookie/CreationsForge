using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Services.Interfaces;

public interface IMagicEffectService
{
    IList<MagicEffectDTO> GetByModKey(ModKey modKey);
    IList<MagicEffectDTO> GetByFormKey(FormKey formKey);
}