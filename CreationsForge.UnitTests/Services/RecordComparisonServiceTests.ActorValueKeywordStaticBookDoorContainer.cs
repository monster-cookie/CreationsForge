using CreationsForge.Core.DTOs.Games;
using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Models.Configuration;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services;
using CreationsForge.Core.Services.Interfaces;
using CreationsForge.Specification.Records;
using Mutagen.Bethesda.Strings;
using Shouldly;

namespace CreationsForge.UnitTests.Services;

/// <summary>
/// Contains record comparison scenarios for actor values, keywords, statics, books, doors, and containers.
/// </summary>
public partial class RecordComparisonServiceTests
{
    [Fact]
    public void GetRecordComparison_ForActorValueInformation_MapsScalarFieldsAndPerkTree()
    {
        var formKey = CreateFormKey("Skyrim.esm", 0x150);
        var associatedSkill = CreateFormKey("Skyrim.esm", 0x201);
        var perk = CreateFormKey("Skyrim.esm", 0x202);
        var baseActorValue = CreateActorValueInformation("Base.esm", formKey, "Archery", "ARC", "Base description", "BaseCNAM", 1.1, 2.2, 3.3, "Base notes", 10, "BaseFlags", "Skill", 0, 100);
        baseActorValue.PerkTree.Add(CreateActorValueInformationPerkTreeEntry("Base.esm", formKey, associatedSkill, perk, 0, "BaseFNAM", 4, 5));
        var patchActorValue = CreateActorValueInformation("Patch.esp", formKey, "Archery", "ARC", "Patch description", "PatchCNAM", 1.5, 2.5, 3.5, "Patch notes", 20, "PatchFlags", "Skill", -10, 110);
        patchActorValue.PerkTree.Add(CreateActorValueInformationPerkTreeEntry("Patch.esp", formKey, associatedSkill, perk, 0, "PatchFNAM", 6, 7));
        var actorValueInformationRepository = new TestActorValueInformationRepository
        {
            Records =
            [
                baseActorValue,
                patchActorValue
            ]
        };
        var service = CreateService(actorValueInformationRepository: actorValueInformationRepository);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.ActorValueInformation.RecordID, formKey);

        comparison.Columns.Select(column => column.Header).ShouldBe(["Base.esm", "Patch.esp"]);
        comparison.Fields.Single(field => field.FieldName == "Name").Values.Select(value => value.DisplayValue).ShouldBe(["Archery", "Archery"]);
        comparison.Fields.Single(field => field.FieldName == "Abbreviation").Values.Select(value => value.DisplayValue).ShouldBe(["ARC", "ARC"]);
        comparison.Fields.Single(field => field.FieldName == "Description").Values.Select(value => value.DisplayValue).ShouldBe(["Base description", "Patch description"]);
        comparison.Fields.Single(field => field.FieldName == "CNAM").Values.Select(value => value.DisplayValue).ShouldBe(["BaseCNAM", "PatchCNAM"]);
        comparison.Fields.Single(field => field.FieldName == "Skill.ImproveMult").Values.Select(value => value.DisplayValue).ShouldBe(["1.1", "1.5"]);
        comparison.Fields.Single(field => field.FieldName == "Skill.ImproveOffset").Values.Select(value => value.DisplayValue).ShouldBe(["2.2", "2.5"]);
        comparison.Fields.Single(field => field.FieldName == "Skill.UseMult").Values.Select(value => value.DisplayValue).ShouldBe(["3.3", "3.5"]);
        comparison.Fields.Single(field => field.FieldName == "ContextNotes").Values.Select(value => value.DisplayValue).ShouldBe(["Base notes", "Patch notes"]);
        comparison.Fields.Single(field => field.FieldName == "DefaultValue").Values.Select(value => value.DisplayValue).ShouldBe(["10", "20"]);
        comparison.Fields.Single(field => field.FieldName == "Flags").Values.Select(value => value.DisplayValue).ShouldBe(["BaseFlags", "PatchFlags"]);
        comparison.Fields.Single(field => field.FieldName == "Type").Values.Select(value => value.DisplayValue).ShouldBe(["Skill", "Skill"]);
        comparison.Fields.Single(field => field.FieldName == "Min").Values.Select(value => value.DisplayValue).ShouldBe(["0", "-10"]);
        comparison.Fields.Single(field => field.FieldName == "Max").Values.Select(value => value.DisplayValue).ShouldBe(["100", "110"]);
        var perkTree = comparison.Fields.Single(field => field.FieldName == "PerkTree");
        var perkTreeEntry = perkTree.Children.Single(field => field.FieldName == "PerkTree [0]");
        perkTreeEntry.Children.Single(field => field.FieldName == "AssociatedSkill").Values.Select(value => value.DisplayValue).ShouldBe(["Skyrim.esm:00000201", "Skyrim.esm:00000201"]);
        perkTreeEntry.Children.Single(field => field.FieldName == "FNAM").Values.Select(value => value.DisplayValue).ShouldBe(["BaseFNAM", "PatchFNAM"]);
        perkTreeEntry.Children.Single(field => field.FieldName == "ConnectionLineToIndices").Values.Select(value => value.DisplayValue).ShouldBe(["4", "6"]);
    }

    /// <summary>
    /// Verifies that Actor Value Information scalar rows are selected from the injected comparison specification while
    /// perk-tree rows remain strategy-based.
    /// </summary>
    [Fact]
    public void GetRecordComparison_ForActorValueInformation_UsesInjectedComparisonSpecification()
    {
        var formKey = CreateFormKey("Skyrim.esm", 0x151);
        var associatedSkill = CreateFormKey("Skyrim.esm", 0x201);
        var perk = CreateFormKey("Skyrim.esm", 0x202);
        var baseActorValue = CreateActorValueInformation("Base.esm", formKey, "Archery", "ARC", "Base description", "BaseCNAM", 1.1, 2.2, 3.3, "Base notes", 10, "BaseFlags", "Skill", 0, 100);
        baseActorValue.PerkTree.Add(CreateActorValueInformationPerkTreeEntry("Base.esm", formKey, associatedSkill, perk, 0, "BaseFNAM", 4, 5));
        var patchActorValue = CreateActorValueInformation("Patch.esp", formKey, "Archery", "ARC", "Patch description", "PatchCNAM", 1.5, 2.5, 3.5, "Patch notes", 20, "PatchFlags", "Skill", -10, 110);
        patchActorValue.PerkTree.Add(CreateActorValueInformationPerkTreeEntry("Patch.esp", formKey, associatedSkill, perk, 0, "PatchFNAM", 6, 7));
        var actorValueInformationRepository = new TestActorValueInformationRepository
        {
            Records =
            [
                baseActorValue,
                patchActorValue
            ]
        };
        var provider = new TestRecordSpecificationProvider(
            new RecordSpecification
            {
                RecordID = SupportedRecordSpecifications.ActorValueInformation.RecordID,
                RecordType = SupportedRecordSpecifications.ActorValueInformation.RecordType,
                TableName = SupportedRecordSpecifications.ActorValueInformation.TableName,
                FriendlyName = SupportedRecordSpecifications.ActorValueInformation.FriendlyName,
                GameSupport = SupportedRecordSpecifications.ActorValueInformation.GameSupport,
                Fields = SupportedRecordSpecifications.ActorValueInformation.Fields,
                Comparison = new RecordComparisonSpecification
                {
                    Fields =
                    [
                        new RecordComparisonFieldSpecification
                        {
                            FieldName = "DefaultValue",
                            SourcePath = "DefaultValue",
                            ValueKind = RecordFieldValueKind.Number
                        }
                    ]
                },
                ImplementationNote = "Test specification."
            });
        var service = CreateService(
            actorValueInformationRepository: actorValueInformationRepository,
            recordSpecificationProvider: provider);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.ActorValueInformation.RecordID, formKey);

        comparison.Fields.Single(field => field.FieldName == "DefaultValue").Values.Select(value => value.DisplayValue)
            .ShouldBe(["10", "20"]);
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Name");
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Skill.ImproveMult");
        comparison.Fields.Single(field => field.FieldName == "PerkTree").Children.ShouldNotBeEmpty();
    }

    /// <summary>
    /// Verifies that specification-declared localized Actor Value Information rows use the selected record text
    /// language.
    /// </summary>
    [Fact]
    public void GetRecordComparison_ForActorValueInformation_UsesSpecificationLocalizedDisplay()
    {
        var formKey = CreateFormKey("Skyrim.esm", 0x152);
        var actorValueInformationRepository = new TestActorValueInformationRepository
        {
            Records =
            [
                CreateActorValueInformation("Base.esm", formKey, "Archery", "ARC", "Base description", "BaseCNAM", 1.1, 2.2, 3.3, "Base notes", 10, "BaseFlags", "Skill", 0, 100),
                CreateActorValueInformation("Patch.esp", formKey, "Archery", "ARC", "Patch description", "PatchCNAM", 1.5, 2.5, 3.5, "Patch notes", 20, "PatchFlags", "Skill", -10, 110)
            ]
        };
        var localizedStringRepository = new TestRecordLocalizedStringRepository
        {
            Records =
            [
                CreateLocalizedString("Base.esm", formKey, "Name", "German", "Basis Akteurwert"),
                CreateLocalizedString("Patch.esp", formKey, "Name", "German", "Patch Akteurwert"),
                CreateLocalizedString("Base.esm", formKey, "Abbreviation", "German", "BAW"),
                CreateLocalizedString("Patch.esp", formKey, "Abbreviation", "German", "PAW"),
                CreateLocalizedString("Base.esm", formKey, "Description", "German", "Basis Beschreibung"),
                CreateLocalizedString("Patch.esp", formKey, "Description", "German", "Patch Beschreibung")
            ]
        };
        var gameSelectionService = new TestGameSelectionService { RecordTextLanguage = Language.German };
        var service = CreateService(
            actorValueInformationRepository: actorValueInformationRepository,
            recordLocalizedStringRepository: localizedStringRepository,
            gameSelectionService: gameSelectionService);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.ActorValueInformation.RecordID, formKey);

        comparison.Fields.Single(field => field.FieldName == "Name").Values.Select(value => value.DisplayValue)
            .ShouldBe(["Basis Akteurwert", "Patch Akteurwert"]);
        comparison.Fields.Single(field => field.FieldName == "Abbreviation").Values.Select(value => value.DisplayValue)
            .ShouldBe(["BAW", "PAW"]);
        comparison.Fields.Single(field => field.FieldName == "Description").Values.Select(value => value.DisplayValue)
            .ShouldBe(["Basis Beschreibung", "Patch Beschreibung"]);
    }

    /// <summary>
    /// Verifies that Keyword scalar rows are selected from the injected comparison specification.
    /// </summary>
    [Fact]
    public void GetRecordComparison_ForKeyword_UsesInjectedComparisonSpecification()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x125);
        var keywordRepository = new TestKeywordRepository
        {
            Records =
            [
                CreateKeyword("Base.esm", formKey, "BaseType", "Blue"),
                CreateKeyword("Patch.esp", formKey, "PatchType", "Red")
            ]
        };
        var provider = new TestRecordSpecificationProvider(
            new RecordSpecification
            {
                RecordID = SupportedRecordSpecifications.Keyword.RecordID,
                RecordType = SupportedRecordSpecifications.Keyword.RecordType,
                TableName = SupportedRecordSpecifications.Keyword.TableName,
                FriendlyName = SupportedRecordSpecifications.Keyword.FriendlyName,
                GameSupport = SupportedRecordSpecifications.Keyword.GameSupport,
                Fields = SupportedRecordSpecifications.Keyword.Fields,
                Comparison = new RecordComparisonSpecification
                {
                    Fields =
                    [
                        new RecordComparisonFieldSpecification
                        {
                            FieldName = "Type",
                            SourcePath = "Type",
                            ValueKind = RecordFieldValueKind.Text
                        }
                    ]
                },
                ImplementationNote = "Test specification."
            });
        var service = CreateService(keywordRepository: keywordRepository, recordSpecificationProvider: provider);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.Keyword.RecordID, formKey);

        comparison.Fields.Single(field => field.FieldName == "Type").Values.Select(value => value.DisplayValue)
            .ShouldBe(["BaseType", "PatchType"]);
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Color");
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Name");
    }

    /// <summary>
    /// Verifies that specification-declared localized Keyword rows use the selected record text language.
    /// </summary>
    [Fact]
    public void GetRecordComparison_ForKeyword_UsesSpecificationLocalizedDisplay()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x127);
        var keywordRepository = new TestKeywordRepository
        {
            Records =
            [
                CreateKeyword("Base.esm", formKey, "BaseType", "Blue"),
                CreateKeyword("Patch.esp", formKey, "PatchType", "Red")
            ]
        };
        var localizedStringRepository = new TestRecordLocalizedStringRepository
        {
            Records =
            [
                CreateLocalizedString("Base.esm", formKey, "Name", "German", "Basis Schluesselwort"),
                CreateLocalizedString("Patch.esp", formKey, "Name", "German", "Patch Schluesselwort")
            ]
        };
        var gameSelectionService = new TestGameSelectionService { RecordTextLanguage = Language.German };
        var service = CreateService(
            keywordRepository: keywordRepository,
            recordLocalizedStringRepository: localizedStringRepository,
            gameSelectionService: gameSelectionService);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.Keyword.RecordID, formKey);

        comparison.Fields.Single(field => field.FieldName == "Name").Values.Select(value => value.DisplayValue)
            .ShouldBe(["Basis Schluesselwort", "Patch Schluesselwort"]);
    }

    /// <summary>
    /// Verifies that Static scalar rows are selected from the injected comparison specification while undeclared
    /// model child rows remain outside the metadata path.
    /// </summary>
    [Fact]
    public void GetRecordComparison_ForStatic_UsesInjectedComparisonSpecification()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x126);
        var staticRepository = new TestStaticRepository
        {
            Records =
            [
                CreateStatic("Base.esm", formKey, 35, "0, 0, 0", null),
                CreateStatic("Patch.esp", formKey, 45, "1, 1, 1", 1.25)
            ]
        };
        var modelRepository = new TestModelRepository
        {
            Records =
            [
                CreateModel("Base.esm", RecordTypeCatalog.Static.RecordID, formKey, "Meshes\\SetDressing\\Rock01.nif"),
                CreateModel("Patch.esp", RecordTypeCatalog.Static.RecordID, formKey, "Meshes\\SetDressing\\Rock01.nif")
            ]
        };
        var provider = new TestRecordSpecificationProvider(
            new RecordSpecification
            {
                RecordID = SupportedRecordSpecifications.Static.RecordID,
                RecordType = SupportedRecordSpecifications.Static.RecordType,
                TableName = SupportedRecordSpecifications.Static.TableName,
                FriendlyName = SupportedRecordSpecifications.Static.FriendlyName,
                GameSupport = SupportedRecordSpecifications.Static.GameSupport,
                Fields = SupportedRecordSpecifications.Static.Fields,
                Comparison = new RecordComparisonSpecification
                {
                    Fields =
                    [
                        new RecordComparisonFieldSpecification
                        {
                            FieldName = "MaxAngle",
                            SourcePath = "MaxAngle",
                            ValueKind = RecordFieldValueKind.Number
                        }
                    ]
                },
                ImplementationNote = "Test specification."
            });
        var service = CreateService(
            staticRepository: staticRepository,
            modelRepository: modelRepository,
            recordSpecificationProvider: provider);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.Static.RecordID, formKey);

        comparison.Fields.Single(field => field.FieldName == "MaxAngle").Values.Select(value => value.DisplayValue)
            .ShouldBe(["35", "45"]);
        comparison.Fields.ShouldNotContain(field => field.FieldName == "ObjectBoundsFirst");
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Name");
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Model");
    }

    /// <summary>
    /// Verifies that specification-declared localized Static rows use the selected record text language.
    /// </summary>
    [Fact]
    public void GetRecordComparison_ForStatic_UsesSpecificationLocalizedDisplay()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x128);
        var staticRepository = new TestStaticRepository
        {
            Records =
            [
                CreateStatic("Base.esm", formKey, 35, "0, 0, 0", null),
                CreateStatic("Patch.esp", formKey, 45, "1, 1, 1", 1.25)
            ]
        };
        var localizedStringRepository = new TestRecordLocalizedStringRepository
        {
            Records =
            [
                CreateLocalizedString("Base.esm", formKey, "Name", "German", "Basis Statik"),
                CreateLocalizedString("Patch.esp", formKey, "Name", "German", "Patch Statik")
            ]
        };
        var gameSelectionService = new TestGameSelectionService { RecordTextLanguage = Language.German };
        var service = CreateService(
            staticRepository: staticRepository,
            recordLocalizedStringRepository: localizedStringRepository,
            gameSelectionService: gameSelectionService);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.Static.RecordID, formKey);

        comparison.Fields.Single(field => field.FieldName == "Name").Values.Select(value => value.DisplayValue)
            .ShouldBe(["Basis Statik", "Patch Statik"]);
    }

    /// <summary>
    /// Verifies that Book scalar rows are selected from the injected comparison specification.
    /// </summary>
    [Fact]
    public void GetRecordComparison_ForBook_UsesInjectedComparisonSpecification()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x129);
        var bookRepository = new TestBookRepository
        {
            Records =
            [
                CreateBook("Base.esm", formKey, "Captain's Log", 100),
                CreateBook("Patch.esp", formKey, "Captain's Log", 150)
            ]
        };
        var provider = new TestRecordSpecificationProvider(
            new RecordSpecification
            {
                RecordID = SupportedRecordSpecifications.Book.RecordID,
                RecordType = SupportedRecordSpecifications.Book.RecordType,
                TableName = SupportedRecordSpecifications.Book.TableName,
                FriendlyName = SupportedRecordSpecifications.Book.FriendlyName,
                GameSupport = SupportedRecordSpecifications.Book.GameSupport,
                Fields = SupportedRecordSpecifications.Book.Fields,
                Comparison = new RecordComparisonSpecification
                {
                    Fields =
                    [
                        new RecordComparisonFieldSpecification
                        {
                            FieldName = "Value",
                            SourcePath = "Value",
                            ValueKind = RecordFieldValueKind.Number
                        }
                    ]
                },
                ImplementationNote = "Test specification."
            });
        var service = CreateService(bookRepository: bookRepository, recordSpecificationProvider: provider);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.Book.RecordID, formKey);

        comparison.Fields.Single(field => field.FieldName == "Value").Values.Select(value => value.DisplayValue)
            .ShouldBe(["100", "150"]);
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Name");
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Flags");
    }

    /// <summary>
    /// Verifies that Door scalar rows are selected from the injected comparison specification.
    /// </summary>
    [Fact]
    public void GetRecordComparison_ForDoor_UsesInjectedComparisonSpecification()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x12A);
        var nativeTerminalFormKey = CreateFormKey("Starfield.esm", 0x555);
        var doorRepository = new TestDoorRepository
        {
            Records =
            [
                CreateDoor("Base.esm", formKey, "Airlock", nativeTerminalFormKey, "Both"),
                CreateDoor("Patch.esp", formKey, "Airlock", nativeTerminalFormKey, "Positive")
            ]
        };
        var provider = new TestRecordSpecificationProvider(
            new RecordSpecification
            {
                RecordID = SupportedRecordSpecifications.Door.RecordID,
                RecordType = SupportedRecordSpecifications.Door.RecordType,
                TableName = SupportedRecordSpecifications.Door.TableName,
                FriendlyName = SupportedRecordSpecifications.Door.FriendlyName,
                GameSupport = SupportedRecordSpecifications.Door.GameSupport,
                Fields = SupportedRecordSpecifications.Door.Fields,
                Comparison = new RecordComparisonSpecification
                {
                    Fields =
                    [
                        new RecordComparisonFieldSpecification
                        {
                            FieldName = "FacingAxisOverride",
                            SourcePath = "FacingAxisOverride",
                            ValueKind = RecordFieldValueKind.Text
                        }
                    ]
                },
                ImplementationNote = "Test specification."
            });
        var service = CreateService(doorRepository: doorRepository, recordSpecificationProvider: provider);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.Door.RecordID, formKey);

        comparison.Fields.Single(field => field.FieldName == "FacingAxisOverride").Values.Select(value => value.DisplayValue)
            .ShouldBe(["Both", "Positive"]);
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Name");
        comparison.Fields.ShouldNotContain(field => field.FieldName == "NativeTerminalFormKey");
    }

    /// <summary>
    /// Verifies that Container scalar rows are selected from the injected comparison specification.
    /// </summary>
    [Fact]
    public void GetRecordComparison_ForContainer_UsesInjectedComparisonSpecification()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x12B);
        var itemFormKey = CreateFormKey("Starfield.esm", 0x333);
        var terminalFormKey = CreateFormKey("Starfield.esm", 0x444);
        var containerRepository = new TestContainerRepository
        {
            Records =
            [
                CreateContainer("Base.esm", formKey, "Storage Crate", terminalFormKey, [CreateContainerItem("Base.esm", formKey, itemFormKey, 0, 2)], "meshes\\base.anim"),
                CreateContainer("Patch.esp", formKey, "Storage Crate", terminalFormKey, [CreateContainerItem("Patch.esp", formKey, itemFormKey, 0, 4)], "meshes\\patch.anim")
            ]
        };
        var provider = new TestRecordSpecificationProvider(
            new RecordSpecification
            {
                RecordID = SupportedRecordSpecifications.Container.RecordID,
                RecordType = SupportedRecordSpecifications.Container.RecordType,
                TableName = SupportedRecordSpecifications.Container.TableName,
                FriendlyName = SupportedRecordSpecifications.Container.FriendlyName,
                GameSupport = SupportedRecordSpecifications.Container.GameSupport,
                Fields = SupportedRecordSpecifications.Container.Fields,
                Comparison = new RecordComparisonSpecification
                {
                    Fields =
                    [
                        new RecordComparisonFieldSpecification
                        {
                            FieldName = "AnimationGraph",
                            SourcePath = "AnimationGraph",
                            ValueKind = RecordFieldValueKind.Text
                        }
                    ]
                },
                ImplementationNote = "Test specification."
            });
        var service = CreateService(containerRepository: containerRepository, recordSpecificationProvider: provider);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.Container.RecordID, formKey);

        comparison.Fields.Single(field => field.FieldName == "AnimationGraph").Values.Select(value => value.DisplayValue)
            .ShouldBe(["meshes\\base.anim", "meshes\\patch.anim"]);
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Name");
        comparison.Fields.ShouldNotContain(field => field.FieldName == "NativeTerminalFormKey");
    }

}
