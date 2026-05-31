using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
namespace SFRecordCompareEngine.Core.Services.Interfaces;
public interface IMiscObjectService { IList<MiscObjectDTO> GetByModKey(ModKey modKey); IList<MiscObjectDTO> GetByFormKeyID(uint formKeyID); }
