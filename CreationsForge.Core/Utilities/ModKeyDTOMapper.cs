using CreationsForge.Core.DTOs.Plugins;
using Mutagen.Bethesda.Plugins;

namespace CreationsForge.Core.Utilities;

public static class ModKeyDTOMapper
{
    public static ModKeyDTO FromModKey(ModKey modKey)
    {
        return new ModKeyDTO
        {
            Name = modKey.Name,
            Type = (int)modKey.Type,
            FileName = modKey.FileName
        };
    }
}
