using CreationsForge.Specification.Records;
using CreationsForge.Specification.Validation;

namespace CreationsForge.Specification.Validation.Specs.Faction;

public static class FactionValidationSpecs
{
    private static readonly IReadOnlyDictionary<string, string> NoPathReplacements =
        new Dictionary<string, string>(StringComparer.Ordinal);

    public static ValidationSpec Fallout4_DNFinancial_OpalVendorFaction()
    {
        return Fallout4Faction("DNFinancial_OpalVendorFaction", "0975FC:Fallout4.esm").Build();
    }

    public static ValidationSpec Fallout4_CaptiveFaction()
    {
        return Fallout4Faction("CaptiveFaction", "03E0C8:Fallout4.esm").Build();
    }

    public static ValidationSpec Fallout4_PlayerFaction()
    {
        return Fallout4Faction("PlayerFaction", "01C21C:Fallout4.esm").Build();
    }

    public static ValidationSpec Fallout4_DN049BakeryClerkFaction()
    {
        return Fallout4Faction("DN049BakeryClerkFaction", "157ACE:Fallout4.esm").Build();
    }

    public static ValidationSpec Fallout4_FarmVendorTheSlog()
    {
        return Fallout4Faction("FarmVendorTheSlog", "14EB97:Fallout4.esm").Build();
    }

    public static ValidationSpec Skyrim_CollegeofWinterholdArchMageFaction()
    {
        return SkyrimFaction("CollegeofWinterholdArchMageFaction", "103372:Skyrim.esm").Build();
    }

    public static ValidationSpec Skyrim_CollegeofWinterholdFaction()
    {
        return SkyrimFaction("CollegeofWinterholdFaction", "01F259:Skyrim.esm").Build();
    }

    public static ValidationSpec Skyrim_CompanionsFaction()
    {
        return SkyrimFaction("CompanionsFaction", "048362:Skyrim.esm").Build();
    }

    public static ValidationSpec Skyrim_DBSancBabetteBedFaction()
    {
        return SkyrimFaction("DBSancBabetteBedFaction", "0FFD65:Skyrim.esm").Build();
    }

    public static ValidationSpec Skyrim_ArenaFaction()
    {
        return SkyrimFaction("ArenaFaction", "040B60:Skyrim.esm").Build();
    }

    public static ValidationSpec Starfield_CrimeFactionCrimsonFleet()
    {
        var spec = StarfieldFaction("CrimeFactionCrimsonFleet", "010B30:Starfield.esm").Build();
        spec.UiComparisonExpectations.Add(ValidationUiComparisonExpectation.Literal(
            ["EditorID"],
            "CrimeFactionCrimsonFleet"));
        return spec;
    }

    /// <summary>
    /// Builds the Starfield <c>CaptiveFaction</c> faction validation spec, including a UI editor ID row expectation.
    /// </summary>
    /// <returns>The validation spec for the Starfield <c>CaptiveFaction</c> sample.</returns>
    public static ValidationSpec Starfield_CaptiveFaction()
    {
        var spec = StarfieldFaction("CaptiveFaction", "03E0C8:Starfield.esm").Build();
        spec.UiComparisonExpectations.Add(ValidationUiComparisonExpectation.Literal(["EditorID"], "CaptiveFaction"));
        return spec;
    }

    public static ValidationSpec Starfield_PlayerFaction()
    {
        return StarfieldFaction("PlayerFaction", "01C21C:Starfield.esm").Build();
    }

    public static ValidationSpec Starfield_LISTColonistFaction()
    {
        return StarfieldFaction("LISTColonistFaction", "1A2C9C:Starfield.esm").Build();
    }

    /// <summary>
    /// Builds the Starfield <c>Vendor_ShipServices_AkilaCityFaction</c> faction validation spec, including a UI editor ID row expectation.
    /// </summary>
    /// <returns>The validation spec for the Starfield <c>Vendor_ShipServices_AkilaCityFaction</c> sample.</returns>
    public static ValidationSpec Starfield_Vendor_ShipServices_AkilaCityFaction()
    {
        var spec = StarfieldFaction("Vendor_ShipServices_AkilaCityFaction", "3CAFBA:Starfield.esm").Build();
        spec.UiComparisonExpectations.Add(ValidationUiComparisonExpectation.Literal(
            ["EditorID"],
            "Vendor_ShipServices_AkilaCityFaction"));
        return spec;
    }

    private static ValidationSpecBuilder Fallout4Faction(string sampleName, string formKey)
    {
        return BaseFaction(SpecificationGame.Fallout4, sampleName, formKey)
            .AddRule(ValidationFieldRule.ScalarList("Fallout4MajorRecordFlags", "MajorRecordFlags", ValidationValueNormalizer.HexInteger))
            .AddRule(ValidationFieldRule.Field("MajorRecordFlagsRaw", "MajorRecordFlags"));
    }

    private static ValidationSpecBuilder SkyrimFaction(string sampleName, string formKey)
    {
        return BaseFaction(SpecificationGame.Skyrim, sampleName, formKey)
            .AddRule(ValidationFieldRule.ScalarList("SkyrimMajorRecordFlags", "MajorRecordFlags", ValidationValueNormalizer.HexInteger))
            .AddRule(ValidationFieldRule.Field("MajorRecordFlagsRaw", "MajorRecordFlags"));
    }

    private static ValidationSpecBuilder StarfieldFaction(string sampleName, string formKey)
    {
        return BaseFaction(SpecificationGame.Starfield, sampleName, formKey)
            .AddRule(ValidationFieldRule.ScalarList("StarfieldMajorRecordFlags", "MajorRecordFlags", ValidationValueNormalizer.HexInteger))
            .AddRule(ValidationFieldRule.ScalarList("MajorFlags", "MajorFlags"));
    }

    private static ValidationSpecBuilder BaseFaction(SpecificationGame game, string sampleName, string formKey)
    {
        return ValidationSpecBuilder
            .ForRecord(game, SupportedRecordSpecifications.Faction)
            .Sample(sampleName)
            .FormKey(formKey)
            .AddBaselineUiComparisonExpectations()
            .AddRule(ValidationFieldRule.TranslatedField("Name", "Name"))
            .AddRule(ValidationFieldRule.ScalarList("Flags", "Flags"))
            .AddRule(ValidationFieldRule.Field("FormationRadius", "FormationRadius", ValidationValueNormalizer.DecimalNumber))
            .AddRule(ValidationFieldRule.Field("Keyword", "Keyword"))
            .AddRule(ValidationFieldRule.Field("Herd", "Herd"))
            .AddRule(ValidationFieldRule.Field("VoiceType", "VoiceType"))
            .AddRule(ValidationFieldRule.Field("SharedCrimeFactionList", "SharedCrimeFactionList"))
            .AddRule(ValidationFieldRule.Field("VendorBuySellList", "VendorBuySellList"))
            .AddRule(ValidationFieldRule.Field("MerchantContainer", "MerchantContainer"))
            .AddRule(ValidationFieldRule.Field("ExteriorJailMarker", "ExteriorJailMarker"))
            .AddRule(ValidationFieldRule.Field("FollowerWaitMarker", "FollowerWaitMarker"))
            .AddRule(ValidationFieldRule.Field("StolenGoodsContainer", "StolenGoodsContainer"))
            .AddRule(ValidationFieldRule.Field("PlayerInventoryContainer", "PlayerInventoryContainer"))
            .AddRule(ValidationFieldRule.Field("JailOutfit", "JailOutfit"))
            .AddRules(GetCrimeRules())
            .AddRules(GetVendorRules())
            .AddRules(GetRelationRules())
            .AddRules(GetRankRules())
            .AddRules(GetConditionRules())
            .AddRules(GetComponentRules())
            .AddRule(ValidationFieldRule.IgnoreSpriggit("VendorValues", "Spriggit emits an empty VendorValues wrapper when all values are defaults."))
            .AddRule(ValidationFieldRule.IgnoreDto("VendorValues", "The DTO omits the empty VendorValues wrapper when every persisted value is null."))
            .AddRule(ValidationFieldRule.IgnoreDtoPrefix("Keywords", "Keywords is the shared keyword mapping projection of the root Keyword field."))
            .AddRule(ValidationFieldRule.IgnoreDtoPrefix("LocalizedStrings", "LocalizedStrings is the DTO projection of translated Spriggit fields."));
    }

    private static IEnumerable<ValidationFieldRule> GetCrimeRules()
    {
        yield return ValidationFieldRule.Field("CrimeValues.Arrest", "CrimeValues.Arrest");
        yield return ValidationFieldRule.Field("CrimeValues.AttackOnSight", "CrimeValues.AttackOnSight");
        yield return ValidationFieldRule.Field("CrimeValues.Murder", "CrimeValues.Murder");
        yield return ValidationFieldRule.Field("CrimeValues.Assault", "CrimeValues.Assault");
        yield return ValidationFieldRule.Field("CrimeValues.Trespass", "CrimeValues.Trespass");
        yield return ValidationFieldRule.Field("CrimeValues.Pickpocket", "CrimeValues.Pickpocket");
        yield return ValidationFieldRule.Field("CrimeValues.Steal", "CrimeValues.Steal");
        yield return ValidationFieldRule.Field("CrimeValues.StealMult", "CrimeValues.StealMult", ValidationValueNormalizer.DecimalNumber);
        yield return ValidationFieldRule.Field("CrimeValues.StealMultiplier", "CrimeValues.StealMultiplier", ValidationValueNormalizer.DecimalNumber);
        yield return ValidationFieldRule.Field("CrimeValues.Escape", "CrimeValues.Escape");
        yield return ValidationFieldRule.Field("CrimeValues.Werewolf", "CrimeValues.Werewolf");
        yield return ValidationFieldRule.Field("CrimeValues.WerewolfUnused", "CrimeValues.WerewolfUnused");
        yield return ValidationFieldRule.Field("CrimeValues.Unknown", "CrimeValues.Unknown");
        yield return ValidationFieldRule.Field("CrimeValues.Piracy", "CrimeValues.Piracy");
        yield return ValidationFieldRule.Field("CrimeValues.SmuggleMultiplier", "CrimeValues.SmuggleMultiplier", ValidationValueNormalizer.DecimalNumber);
    }

    private static IEnumerable<ValidationFieldRule> GetVendorRules()
    {
        yield return ValidationFieldRule.Field("VendorValues.StartHour", "VendorValues.StartHour", ValidationValueNormalizer.DecimalNumber);
        yield return ValidationFieldRule.Field("VendorValues.EndHour", "VendorValues.EndHour", ValidationValueNormalizer.DecimalNumber);
        yield return ValidationFieldRule.Field("VendorValues.Radius", "VendorValues.Radius");
        yield return ValidationFieldRule.Field("VendorValues.BuysStolenItems", "VendorValues.BuysStolenItems");
        yield return ValidationFieldRule.Field("VendorValues.BuysNonStolenItems", "VendorValues.BuysNonStolenItems");
        yield return ValidationFieldRule.Field("VendorValues.BuySellEverythingNotInList", "VendorValues.BuySellEverythingNotInList");
        yield return ValidationFieldRule.Field("VendorLocation.MutagenObjectType", "VendorLocation.MutagenObjectType");
        yield return ValidationFieldRule.Field("VendorLocation.Target.MutagenObjectType", "VendorLocation.Target.MutagenObjectType");
        yield return ValidationFieldRule.Field("VendorLocation.Target.Type", "VendorLocation.Target.Type");
        yield return ValidationFieldRule.Field("VendorLocation.Target.Link", "VendorLocation.Target.Link");
    }

    private static IEnumerable<ValidationFieldRule> GetRelationRules()
    {
        for (var relationIndex = 0; relationIndex <= 100; relationIndex++)
        {
            var indexText = relationIndex.ToString(System.Globalization.CultureInfo.InvariantCulture);
            yield return ValidationFieldRule.Field("Relations[" + indexText + "].Target", "Relations[" + indexText + "].Target");
            yield return ValidationFieldRule.Field("Relations[" + indexText + "].Reaction", "Relations[" + indexText + "].Reaction");
        }
    }

    private static IEnumerable<ValidationFieldRule> GetRankRules()
    {
        for (var rankIndex = 0; rankIndex <= 80; rankIndex++)
        {
            var indexText = rankIndex.ToString(System.Globalization.CultureInfo.InvariantCulture);
            yield return ValidationFieldRule.Field("Ranks[" + indexText + "].Number", "Ranks[" + indexText + "].Number");
            yield return ValidationFieldRule.TranslatedField("Ranks[" + indexText + "].Title.Male", "Ranks[" + indexText + "].Title.Male");
            yield return ValidationFieldRule.TranslatedField("Ranks[" + indexText + "].Title.Female", "Ranks[" + indexText + "].Title.Female");
            yield return ValidationFieldRule.IgnoreSpriggit("Ranks[" + indexText + "].Title.Male", "Spriggit emits an empty rank title wrapper when no translated title values are present.");
            yield return ValidationFieldRule.IgnoreSpriggit("Ranks[" + indexText + "].Title.Female", "Spriggit emits an empty rank title wrapper when no translated title values are present.");
            yield return ValidationFieldRule.IgnoreDto("Ranks[" + indexText + "].Title.Male", "The DTO omits the empty rank title wrapper when no translated title values are present.");
            yield return ValidationFieldRule.IgnoreDto("Ranks[" + indexText + "].Title.Female", "The DTO omits the empty rank title wrapper when no translated title values are present.");
        }
    }

    private static IEnumerable<ValidationFieldRule> GetConditionRules()
    {
        yield return ValidationFieldRule.PathPrefix("Conditions", "Conditions", NoPathReplacements);
        for (var conditionIndex = 0; conditionIndex <= 100; conditionIndex++)
        {
            var indexText = conditionIndex.ToString(System.Globalization.CultureInfo.InvariantCulture);
            yield return ValidationFieldRule.Field(
                "Conditions[" + indexText + "].Data.FirstParameter",
                "Conditions[" + indexText + "].Data.FirstParameter",
                ValidationValueNormalizer.DecimalFormKeyId);
            yield return ValidationFieldRule.Field(
                "Conditions[" + indexText + "].Data.SecondParameter",
                "Conditions[" + indexText + "].Data.SecondParameter",
                ValidationValueNormalizer.DecimalFormKeyId);
            yield return ValidationFieldRule.Field(
                "Conditions[" + indexText + "].Data.ThirdParameter",
                "Conditions[" + indexText + "].Data.ThirdParameter",
                ValidationValueNormalizer.DecimalFormKeyId);
        }
    }

    private static IEnumerable<ValidationFieldRule> GetComponentRules()
    {
        for (var componentIndex = 0; componentIndex <= 20; componentIndex++)
        {
            for (var itemIndex = 0; itemIndex <= 20; itemIndex++)
            {
                var itemPath = "Components[" +
                    componentIndex.ToString(System.Globalization.CultureInfo.InvariantCulture) +
                    "].Items[" +
                    itemIndex.ToString(System.Globalization.CultureInfo.InvariantCulture) +
                    "]";
                yield return ValidationFieldRule.Field(itemPath + ".Unknown1", itemPath + ".Unknown1", ValidationValueNormalizer.DecimalNumber);
                yield return ValidationFieldRule.Field(itemPath + ".Unknown4", itemPath + ".Unknown4", ValidationValueNormalizer.DecimalNumber);
                yield return ValidationFieldRule.Field(itemPath + ".Unknown5", itemPath + ".Unknown5", ValidationValueNormalizer.DecimalNumber);
            }
        }

        yield return ValidationFieldRule.PathPrefix("Components", "Components", NoPathReplacements);
    }
}
