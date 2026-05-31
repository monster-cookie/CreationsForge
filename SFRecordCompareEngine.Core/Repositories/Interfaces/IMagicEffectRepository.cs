using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
namespace SFRecordCompareEngine.Core.Repositories.Interfaces;
public interface IMagicEffectRepository { IList<MagicEffectDTO> GetByModKey(ModKey modKey); IList<MagicEffectDTO> GetByFormKeyID(uint formKeyID); void Save(MagicEffectDTO dto); }
