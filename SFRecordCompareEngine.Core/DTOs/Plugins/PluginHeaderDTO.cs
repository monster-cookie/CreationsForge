using Mutagen.Bethesda.Starfield;

namespace SFRecordCompareEngine.Core.DTOs.Plugins;

public class PluginHeaderDTO
{
    public PluginHeaderDTO(string pluginName, IStarfieldModHeaderGetter pluginModHeader)
    {
        Name = pluginName;
        Author = pluginModHeader.Author ?? "Unknown";
        Version = pluginModHeader.Version;
        Description = pluginModHeader.Description ?? string.Empty;
        Masters = pluginModHeader.MasterReferences.Select(masterRef => masterRef.Master.FileName.ToString()).ToList();
    }

    public PluginHeaderDTO(PluginMetadataDTO plugin, IList<PluginMasterReferenceDTO> masterReferences)
    {
        Name = plugin.PluginFileName;
        Author = plugin.Author ?? "Unknown";
        Version = plugin.FormVersion ?? 0;
        Description = string.Empty;
        Masters = masterReferences
            .OrderBy(masterReference => masterReference.MasterReferenceIndex)
            .Select(masterReference => masterReference.ParentModKey)
            .ToList();
    }

    /// <summary>
    ///     The name of the plugin, typically the file name with extension (e.g. "MyMod.esm")
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///     The author of the plugin
    /// </summary>
    public string Author { get; set; }

    /// <summary>
    ///     The version of the plugin
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    ///     The description of the plugin
    /// </summary>
    public string Description { get; set; }

    public List<string> Masters { get; set; }
}
