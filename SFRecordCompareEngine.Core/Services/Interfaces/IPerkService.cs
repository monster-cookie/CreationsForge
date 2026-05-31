using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
namespace SFRecordCompareEngine.Core.Services.Interfaces;
public interface IPerkService { IList<PerkDTO> GetByModKey(ModKey modKey); IList<PerkDTO> GetByFormKeyID(uint formKeyID); }
