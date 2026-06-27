using static CreationsForge.Specification.Records.RecordSpecificationFactory;

namespace CreationsForge.Specification.Records;

/// <summary>
/// Provides the Faction record specification metadata.
/// </summary>
internal static class FactionRecordSpecification
{

    /// <summary>
    /// Gets the Faction record specification.
    /// </summary>
    internal static RecordSpecification Instance { get; } = new()
    {
        RecordID = "FACT",
        RecordType = "Faction",
        TableName = "Factions",
        FriendlyName = "Faction",
        GameSupport = CreateCurrentGameSupport("Factions", "Factions"),
        Import = new RecordImportSpecification
        {
            PluginRecordSetPropertyName = "Factions",
            ImportOrder = 4,
            IsRequired = true
        },
        Reader = CreateReaderSpecification("Factions", "Factions"),
        Fields =
        [
            new RecordFieldSpecification
            {
                FieldName = "Name",
                SpriggitPath = "Name",
                ValueKind = RecordFieldValueKind.LocalizedString,
                IsLocalized = true,
                Description = "Optional translated faction display name."
            },
            new RecordFieldSpecification
            {
                FieldName = "Flags",
                SpriggitPath = "Flags",
                ValueKind = RecordFieldValueKind.FlagSet,
                Description = "Formatted faction flags."
            },
            new RecordFieldSpecification
            {
                FieldName = "FormationRadius",
                SpriggitPath = "FormationRadius",
                ValueKind = RecordFieldValueKind.Number,
                Description = "Optional faction formation radius scalar."
            },
            new RecordFieldSpecification
            {
                FieldName = "Keyword",
                SpriggitPath = "Keyword",
                ValueKind = RecordFieldValueKind.FormKey,
                Description = "Optional faction keyword FormKey reference."
            },
            new RecordFieldSpecification
            {
                FieldName = "Herd",
                SpriggitPath = "Herd",
                ValueKind = RecordFieldValueKind.FormKey,
                Description = "Optional faction herd FormKey reference."
            },
            new RecordFieldSpecification
            {
                FieldName = "VoiceType",
                SpriggitPath = "VoiceType",
                ValueKind = RecordFieldValueKind.FormKey,
                Description = "Optional faction voice type FormKey reference."
            },
            new RecordFieldSpecification
            {
                FieldName = "SharedCrimeFactionList",
                SpriggitPath = "SharedCrimeFactionList",
                ValueKind = RecordFieldValueKind.FormKey,
                Description = "Optional shared crime faction list FormKey reference."
            },
            new RecordFieldSpecification
            {
                FieldName = "VendorBuySellList",
                SpriggitPath = "VendorBuySellList",
                ValueKind = RecordFieldValueKind.FormKey,
                Description = "Optional vendor buy/sell list FormKey reference."
            },
            new RecordFieldSpecification
            {
                FieldName = "MerchantContainer",
                SpriggitPath = "MerchantContainer",
                ValueKind = RecordFieldValueKind.FormKey,
                Description = "Optional merchant container FormKey reference."
            },
            new RecordFieldSpecification
            {
                FieldName = "ExteriorJailMarker",
                SpriggitPath = "ExteriorJailMarker",
                ValueKind = RecordFieldValueKind.FormKey,
                Description = "Optional exterior jail marker FormKey reference."
            },
            new RecordFieldSpecification
            {
                FieldName = "FollowerWaitMarker",
                SpriggitPath = "FollowerWaitMarker",
                ValueKind = RecordFieldValueKind.FormKey,
                Description = "Optional follower wait marker FormKey reference."
            },
            new RecordFieldSpecification
            {
                FieldName = "StolenGoodsContainer",
                SpriggitPath = "StolenGoodsContainer",
                ValueKind = RecordFieldValueKind.FormKey,
                Description = "Optional stolen goods container FormKey reference."
            },
            new RecordFieldSpecification
            {
                FieldName = "PlayerInventoryContainer",
                SpriggitPath = "PlayerInventoryContainer",
                ValueKind = RecordFieldValueKind.FormKey,
                Description = "Optional player inventory container FormKey reference."
            },
            new RecordFieldSpecification
            {
                FieldName = "JailOutfit",
                SpriggitPath = "JailOutfit",
                ValueKind = RecordFieldValueKind.FormKey,
                Description = "Optional jail outfit FormKey reference."
            },
            new RecordFieldSpecification
            {
                FieldName = "CrimeValues",
                SpriggitPath = "CrimeValues",
                ValueKind = RecordFieldValueKind.Object,
                Description = "Optional crime values represented by scalar comparison rows."
            },
            new RecordFieldSpecification
            {
                FieldName = "VendorValues",
                SpriggitPath = "VendorValues",
                ValueKind = RecordFieldValueKind.Object,
                Description = "Optional vendor values represented by scalar comparison rows."
            },
            new RecordFieldSpecification
            {
                FieldName = "VendorLocation",
                SpriggitPath = "VendorLocation",
                ValueKind = RecordFieldValueKind.Object,
                Description = "Optional vendor location represented by scalar comparison rows."
            }
        ],
        Comparison = new RecordComparisonSpecification
        {
            Fields =
            [
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Version2",
                    SourcePath = "Version2",
                    ValueKind = RecordFieldValueKind.Number
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Name",
                    SourcePath = "Name",
                    ValueKind = RecordFieldValueKind.LocalizedString,
                    UsesLocalizedDisplay = true
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Flags",
                    SourcePath = "Flags",
                    ValueKind = RecordFieldValueKind.FlagSet
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "FormationRadius",
                    SourcePath = "FormationRadius",
                    ValueKind = RecordFieldValueKind.Number
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Keyword",
                    SourcePath = "Keyword",
                    ValueKind = RecordFieldValueKind.FormKey
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Herd",
                    SourcePath = "Herd",
                    ValueKind = RecordFieldValueKind.FormKey
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "VoiceType",
                    SourcePath = "VoiceType",
                    ValueKind = RecordFieldValueKind.FormKey
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "SharedCrimeFactionList",
                    SourcePath = "SharedCrimeFactionList",
                    ValueKind = RecordFieldValueKind.FormKey
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "VendorBuySellList",
                    SourcePath = "VendorBuySellList",
                    ValueKind = RecordFieldValueKind.FormKey
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "MerchantContainer",
                    SourcePath = "MerchantContainer",
                    ValueKind = RecordFieldValueKind.FormKey
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "ExteriorJailMarker",
                    SourcePath = "ExteriorJailMarker",
                    ValueKind = RecordFieldValueKind.FormKey
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "FollowerWaitMarker",
                    SourcePath = "FollowerWaitMarker",
                    ValueKind = RecordFieldValueKind.FormKey
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "StolenGoodsContainer",
                    SourcePath = "StolenGoodsContainer",
                    ValueKind = RecordFieldValueKind.FormKey
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "PlayerInventoryContainer",
                    SourcePath = "PlayerInventoryContainer",
                    ValueKind = RecordFieldValueKind.FormKey
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "JailOutfit",
                    SourcePath = "JailOutfit",
                    ValueKind = RecordFieldValueKind.FormKey
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "CrimeValues.Arrest",
                    SourcePath = "CrimeValues.Arrest",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "CrimeValues.AttackOnSight",
                    SourcePath = "CrimeValues.AttackOnSight",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "CrimeValues.Murder",
                    SourcePath = "CrimeValues.Murder",
                    ValueKind = RecordFieldValueKind.Number
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "CrimeValues.Assault",
                    SourcePath = "CrimeValues.Assault",
                    ValueKind = RecordFieldValueKind.Number
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "CrimeValues.Trespass",
                    SourcePath = "CrimeValues.Trespass",
                    ValueKind = RecordFieldValueKind.Number
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "CrimeValues.Pickpocket",
                    SourcePath = "CrimeValues.Pickpocket",
                    ValueKind = RecordFieldValueKind.Number
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "CrimeValues.Steal",
                    SourcePath = "CrimeValues.Steal",
                    ValueKind = RecordFieldValueKind.Number
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "CrimeValues.StealMult",
                    SourcePath = "CrimeValues.StealMult",
                    ValueKind = RecordFieldValueKind.Number
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "CrimeValues.StealMultiplier",
                    SourcePath = "CrimeValues.StealMultiplier",
                    ValueKind = RecordFieldValueKind.Number
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "CrimeValues.Escape",
                    SourcePath = "CrimeValues.Escape",
                    ValueKind = RecordFieldValueKind.Number
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "CrimeValues.Werewolf",
                    SourcePath = "CrimeValues.Werewolf",
                    ValueKind = RecordFieldValueKind.Number
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "CrimeValues.WerewolfUnused",
                    SourcePath = "CrimeValues.WerewolfUnused",
                    ValueKind = RecordFieldValueKind.Number
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "CrimeValues.Unknown",
                    SourcePath = "CrimeValues.Unknown",
                    ValueKind = RecordFieldValueKind.Number
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "CrimeValues.Piracy",
                    SourcePath = "CrimeValues.Piracy",
                    ValueKind = RecordFieldValueKind.Number
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "CrimeValues.SmuggleMultiplier",
                    SourcePath = "CrimeValues.SmuggleMultiplier",
                    ValueKind = RecordFieldValueKind.Number
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "VendorValues.StartHour",
                    SourcePath = "VendorValues.StartHour",
                    ValueKind = RecordFieldValueKind.Number
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "VendorValues.EndHour",
                    SourcePath = "VendorValues.EndHour",
                    ValueKind = RecordFieldValueKind.Number
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "VendorValues.Radius",
                    SourcePath = "VendorValues.Radius",
                    ValueKind = RecordFieldValueKind.Number
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "VendorValues.BuysStolenItems",
                    SourcePath = "VendorValues.BuysStolenItems",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "VendorValues.BuysNonStolenItems",
                    SourcePath = "VendorValues.BuysNonStolenItems",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "VendorValues.BuySellEverythingNotInList",
                    SourcePath = "VendorValues.BuySellEverythingNotInList",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "VendorLocation.MutagenObjectType",
                    SourcePath = "VendorLocation.MutagenObjectType",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "VendorLocation.Target.MutagenObjectType",
                    SourcePath = "VendorLocation.Target.MutagenObjectType",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "VendorLocation.Target.Type",
                    SourcePath = "VendorLocation.Target.Type",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "VendorLocation.Target.Link",
                    SourcePath = "VendorLocation.Target.Link",
                    ValueKind = RecordFieldValueKind.FormKey
                }
            ],
            ChildGroups =
            [
                new RecordComparisonChildGroupSpecification
                {
                    GroupKind = RecordComparisonChildGroupKind.ConditionRules,
                    GroupName = "Conditions",
                    Description = "Shared condition-rule rows appended after rank rows."
                },
                new RecordComparisonChildGroupSpecification
                {
                    GroupKind = RecordComparisonChildGroupKind.KeywordMappings,
                    GroupName = "Keywords",
                    Description = "Shared keyword mapping rows appended after scalar parent and record-specific rows."
                }
            ]
        },
        ImplementationNote = "Comparison metadata drives scalar parent rows and condition/keyword child-group " +
            "dispatch; relation, rank, and component rows remain strategy-based."
    };
}


