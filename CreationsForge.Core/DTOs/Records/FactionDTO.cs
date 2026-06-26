using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records.Interfaces;

namespace CreationsForge.Core.DTOs.Records;

public class FactionDTO : RecordDTO, IKeywords, IHasComponentsDTO, IHasConditionsDTO
{
    public TranslatedStringDTO? Name { get; set; }

    public string? Flags { get; set; }

    public double? FormationRadius { get; set; }

    public FormKeyDTO? Keyword { get; set; }

    public FormKeyDTO? Herd { get; set; }

    public FormKeyDTO? VoiceType { get; set; }

    public FormKeyDTO? SharedCrimeFactionList { get; set; }

    public FormKeyDTO? VendorBuySellList { get; set; }

    public FormKeyDTO? MerchantContainer { get; set; }

    public FormKeyDTO? ExteriorJailMarker { get; set; }

    public FormKeyDTO? FollowerWaitMarker { get; set; }

    public FormKeyDTO? StolenGoodsContainer { get; set; }

    public FormKeyDTO? PlayerInventoryContainer { get; set; }

    public FormKeyDTO? JailOutfit { get; set; }

    public CrimeValuesDTO? CrimeValues { get; set; }

    public VendorValuesDTO? VendorValues { get; set; }

    public VendorLocationDTO? VendorLocation { get; set; }

    public IList<FactionRelationDTO> Relations { get; set; } = new List<FactionRelationDTO>();

    public IList<FactionRankDTO> Ranks { get; set; } = new List<FactionRankDTO>();

    public IList<ConditionFormConditionDTO> Conditions { get; set; } = new List<ConditionFormConditionDTO>();

    public IList<RecordComponentDTO> Components { get; set; } = new List<RecordComponentDTO>();

    public IList<KeywordMappingDTO> Keywords { get; set; } = new List<KeywordMappingDTO>();

    public class CrimeValuesDTO
    {
        public bool? Arrest { get; set; }

        public bool? AttackOnSight { get; set; }

        public int? Murder { get; set; }

        public int? Assault { get; set; }

        public int? Trespass { get; set; }

        public int? Pickpocket { get; set; }

        public int? Steal { get; set; }

        public double? StealMult { get; set; }

        public double? StealMultiplier { get; set; }

        public int? Escape { get; set; }

        public int? Werewolf { get; set; }

        public int? WerewolfUnused { get; set; }

        public int? Unknown { get; set; }

        public int? Piracy { get; set; }

        public double? SmuggleMultiplier { get; set; }
    }

    public class VendorValuesDTO
    {
        public double? StartHour { get; set; }

        public double? EndHour { get; set; }

        public int? Radius { get; set; }

        public bool? BuysStolenItems { get; set; }

        public bool? BuysNonStolenItems { get; set; }

        public bool? BuySellEverythingNotInList { get; set; }
    }

    public class VendorLocationDTO
    {
        public string? MutagenObjectType { get; set; }

        public VendorLocationTargetDTO? Target { get; set; }
    }

    public class VendorLocationTargetDTO
    {
        public string? MutagenObjectType { get; set; }

        public string? Type { get; set; }

        public FormKeyDTO? Link { get; set; }
    }
}
