using Mutagen.Bethesda.Plugins;
using Noggog;

namespace SFRecordCompareEngine.Core.DTOs.Plugins;

public class PluginInformationDTO
{
    public string? Name { get; set; }
    
    public string? FileName { get; set; }
    
    public MasterStyle MasterStyle { get; set; }
    
    public string? Author { get; set; }
    
    public int Version { get; set; }
    
    public string? Description { get; set; }
    
    public List<FileName>? Masters { get; set; } = new();
    
    public bool IsValid { get; set; }
    
    public string? ErrorMessage { get; set; }
}