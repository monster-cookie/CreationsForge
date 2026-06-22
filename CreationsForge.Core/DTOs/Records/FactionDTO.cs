using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records.Interfaces;

namespace CreationsForge.Core.DTOs.Records;

public class FactionDTO : RecordDTO, IKeywords, IHasComponentsRecordDTO, IHasConditionsRecordDTO
{
    public TranslatedStringDTO? Name { get; set; }

    public string? Flags { get; set; }

    public double? FormationRadius { get; set; }

    public FormKeyDTO? KeywordFormKey { get; set; }

    public FormKeyDTO? HerdFormKey { get; set; }

    public FormKeyDTO? VoiceTypeFormKey { get; set; }

    public FormKeyDTO? SharedCrimeFactionListFormKey { get; set; }

    public FormKeyDTO? VendorBuySellListFormKey { get; set; }

    public FormKeyDTO? MerchantContainerFormKey { get; set; }

    public FormKeyDTO? ExteriorJailMarkerFormKey { get; set; }

    public FormKeyDTO? FollowerWaitMarkerFormKey { get; set; }

    public FormKeyDTO? StolenGoodsContainerFormKey { get; set; }

    public FormKeyDTO? PlayerInventoryContainerFormKey { get; set; }

    public FormKeyDTO? JailOutfitFormKey { get; set; }

    public bool? CrimeArrest { get; set; }

    public bool? CrimeAttackOnSight { get; set; }

    public int? CrimeMurder { get; set; }

    public int? CrimeAssault { get; set; }

    public int? CrimeTrespass { get; set; }

    public int? CrimePickpocket { get; set; }

    public int? CrimeSteal { get; set; }

    public double? CrimeStealMult { get; set; }

    public int? CrimeEscape { get; set; }

    public int? CrimeWerewolf { get; set; }

    public int? CrimeUnknown { get; set; }

    public double? VendorStartHour { get; set; }

    public double? VendorEndHour { get; set; }

    public int? VendorRadius { get; set; }

    public bool? VendorBuysStolenItems { get; set; }

    public bool? VendorBuysNonStolenItems { get; set; }

    public bool? VendorBuySellEverythingNotInList { get; set; }

    public string? VendorLocationMutagenObjectType { get; set; }

    public string? VendorLocationType { get; set; }

    public FormKeyDTO? VendorLocationLinkFormKey { get; set; }

    public IList<FactionRelationDTO> Relations { get; set; } = new List<FactionRelationDTO>();

    public IList<FactionRankDTO> Ranks { get; set; } = new List<FactionRankDTO>();

    public IList<ConditionFormConditionDTO> Conditions { get; set; } = new List<ConditionFormConditionDTO>();

    public IList<RecordComponentDTO> Components { get; set; } = new List<RecordComponentDTO>();

    public IList<KeywordMappingDTO> Keywords { get; set; } = new List<KeywordMappingDTO>();
}
