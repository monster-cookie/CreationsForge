using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
namespace SFRecordCompareEngine.Core.Repositories.Interfaces;
public interface IMiscObjectRepository { IList<MiscObjectDTO> GetByModKey(ModKey modKey); IList<MiscObjectDTO> GetByFormKeyID(uint formKeyID); void Save(MiscObjectDTO dto); }
