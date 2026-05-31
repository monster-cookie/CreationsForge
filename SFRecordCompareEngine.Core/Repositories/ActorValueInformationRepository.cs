using Mutagen.Bethesda.Plugins;
using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Enums;
using SFRecordCompareEngine.Core.Models.Database;
using SFRecordCompareEngine.Core.Repositories.Interfaces;

namespace SFRecordCompareEngine.Core.Repositories;

public class ActorValueInformationRepository : IActorValueInformationRepository
{
    private readonly IDatabase Database;

    public ActorValueInformationRepository(IDatabase database)
    {
        Database = database;
    }

    public IList<ActorValueInformationDTO> GetByModKey(ModKey modKey)
    {
        return Database.Fetch<ActorValueInformation>("SELECT * FROM ActorValueInformation WHERE ModKey_Name = @ModKeyName AND ModKey_Type = @ModKeyType AND ModKey_FileName = @ModKeyFileName COLLATE NOCASE ORDER BY FormKey_ID;", new { ModKeyName = modKey.Name, ModKeyType = (int)modKey.Type, ModKeyFileName = modKey.FileName }).Select(x => new ActorValueInformationDTO(x)).ToList();
    }

    public IList<ActorValueInformationDTO> GetByFormKeyID(uint formKeyID)
    {
        return Database.Fetch<ActorValueInformation>(
            "SELECT ActorValueInformation.* FROM ActorValueInformation INNER JOIN Plugins ON Plugins.ModKey_Name = ActorValueInformation.ModKey_Name AND Plugins.ModKey_Type = ActorValueInformation.ModKey_Type AND Plugins.ModKey_FileName = ActorValueInformation.ModKey_FileName WHERE ActorValueInformation.FormKey_ID = @FormKeyID AND Plugins.Enabled = 1 AND Plugins.ExistsOnDisk = 1 AND Plugins.ImportState = @ImportState ORDER BY Plugins.LoadOrderIndex;",
            new { FormKeyID = formKeyID, ImportState = nameof(PluginImportState.Current) }).Select(x => new ActorValueInformationDTO(x)).ToList();
    }

    public void Save(ActorValueInformationDTO dto)
    {
        Database.Save(new ActorValueInformation(dto));
    }
}