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
/// Contains record comparison scenarios for condition forms, books, doors, terminals, and shared comparison behavior.
/// </summary>
public partial class RecordComparisonServiceTests
{
    [Fact]
    public void GetRecordComparison_ForConditionForm_MapsVersion2AndConditions()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x246E86);
        var firstParameter = CreateFormKey("Starfield.esm", 0x258350);
        var patchFirstParameter = CreateFormKey("Starfield.esm", 0x2CC9F2);
        var conditionFormRepository = new TestConditionFormRepository
        {
            Records =
            [
                CreateConditionForm("Base.esm", formKey, 1, firstParameter, "1"),
                CreateConditionForm("Patch.esp", formKey, 2, patchFirstParameter, null)
            ]
        };
        var service = CreateService(conditionFormRepository: conditionFormRepository);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.ConditionForm.RecordID, formKey);

        comparison.RecordType.ShouldBe(RecordTypeCatalog.ConditionForm.RecordID);
        comparison.Fields.Single(field => field.FieldName == "Version2").Values.Select(value => value.DisplayValue).ShouldBe(["1", "2"]);
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Raw Payloads");
        var conditions = comparison.Fields.Single(field => field.FieldName == "Conditions");
        var condition = conditions.Children.Single(field => field.FieldName == "Condition [0]");
        condition.Values.Select(value => value.DisplayValue).ShouldBe(["Subject: HasKeyword(Starfield.esm:00258350, 0) EqualTo 1", "Subject: HasKeyword(Starfield.esm:002CC9F2, 0)"]);
        condition.State.ShouldBe(RecordComparisonValueState.Conflict);
        condition.Values.Select(value => value.State).ShouldBe([RecordComparisonValueState.Conflict, RecordComparisonValueState.WinningOverride]);
        condition.Children.ShouldBeEmpty();
    }

    /// <summary>
    /// Verifies that Condition Form scalar rows are selected from the injected comparison specification while
    /// undeclared condition rows remain outside the metadata path.
    /// </summary>
    [Fact]
    public void GetRecordComparison_ForConditionForm_UsesInjectedComparisonSpecification()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x246E87);
        var firstParameter = CreateFormKey("Starfield.esm", 0x258350);
        var patchFirstParameter = CreateFormKey("Starfield.esm", 0x2CC9F2);
        var conditionFormRepository = new TestConditionFormRepository
        {
            Records =
            [
                CreateConditionForm("Base.esm", formKey, 1, firstParameter, "1"),
                CreateConditionForm("Patch.esp", formKey, 2, patchFirstParameter, null)
            ]
        };
        var provider = new TestRecordSpecificationProvider(
            new RecordSpecification
            {
                RecordID = SupportedRecordSpecifications.ConditionForm.RecordID,
                RecordType = SupportedRecordSpecifications.ConditionForm.RecordType,
                TableName = SupportedRecordSpecifications.ConditionForm.TableName,
                FriendlyName = SupportedRecordSpecifications.ConditionForm.FriendlyName,
                GameSupport = SupportedRecordSpecifications.ConditionForm.GameSupport,
                Fields = SupportedRecordSpecifications.ConditionForm.Fields,
                Comparison = new RecordComparisonSpecification
                {
                    Fields =
                    [
                        new RecordComparisonFieldSpecification
                        {
                            FieldName = "Version2",
                            SourcePath = "Version2",
                            ValueKind = RecordFieldValueKind.Number
                        }
                    ]
                },
                ImplementationNote = "Test specification."
            });
        var service = CreateService(
            conditionFormRepository: conditionFormRepository,
            recordSpecificationProvider: provider);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.ConditionForm.RecordID, formKey);

        comparison.Fields.Single(field => field.FieldName == "Version2").Values.Select(value => value.DisplayValue)
            .ShouldBe(["1", "2"]);
        comparison.Fields.ShouldNotContain(field => field.FieldName == "OwnerQuest");
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Conditions");
    }

    [Fact]
    public void GetRecordComparison_ForConditionForm_PreservesMultipleConditionRules()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x246E86);
        var conditionFormRepository = new TestConditionFormRepository
        {
            Records =
            [
                CreateActorIsPreyConditionForm(formKey)
            ]
        };
        var service = CreateService(conditionFormRepository: conditionFormRepository);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.ConditionForm.RecordID, formKey);

        var conditions = comparison.Fields.Single(field => field.FieldName == "Conditions");
        conditions.Children.Select(field => field.FieldName).ShouldBe([
            "Condition [0]",
            "Condition [1]"
        ]);
        conditions.Children.Select(field => field.Values.Single().DisplayValue).ShouldBe([
            "Subject: HasKeyword(Starfield.esm:00258350, 0) EqualTo 1",
            "Subject: HasKeyword(Starfield.esm:002CC9F2, 0) EqualTo 0"
        ]);
        conditions.Children.Select(field => field.Children.Count).ShouldBe([0, 0]);
    }

    [Fact]
    public void GetRecordComparison_ForBook_MapsBookFieldsAndChildren()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x3000);
        var bookRepository = new TestBookRepository
        {
            Records =
            [
                CreateBook("Base.esm", formKey, "Captain's Log", 100),
                CreateBook("Patch.esp", formKey, "Captain's Log", 150)
            ]
        };
        var modelRepository = new TestModelRepository
        {
            Records =
            [
                CreateModel("Base.esm", RecordTypeCatalog.Book.RecordID, formKey, "Meshes\\SetDressing\\Books\\Book01.nif"),
                CreateModel("Patch.esp", RecordTypeCatalog.Book.RecordID, formKey, "Meshes\\SetDressing\\Books\\Book01.nif")
            ]
        };
        var keywordMappingRepository = new TestKeywordMappingRepository
        {
            Records =
            [
                CreateKeywordMapping("Base.esm", RecordTypeCatalog.Book.RecordID, formKey, CreateFormKey("Starfield.esm", 0x101), 0),
                CreateKeywordMapping("Patch.esp", RecordTypeCatalog.Book.RecordID, formKey, CreateFormKey("Starfield.esm", 0x101), 0)
            ]
        };
        var soundMappingRepository = new TestSoundMappingRepository
        {
            Records =
            [
                CreateSoundMapping("Base.esm", RecordTypeCatalog.Book.RecordID, formKey, "PickupSound", 0, "pickup"),
                CreateSoundMapping("Patch.esp", RecordTypeCatalog.Book.RecordID, formKey, "PickupSound", 0, "pickup")
            ]
        };
        var service = CreateService(
            bookRepository: bookRepository,
            modelRepository: modelRepository,
            keywordMappingRepository: keywordMappingRepository,
            soundMappingRepository: soundMappingRepository);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.Book.RecordID, formKey);

        comparison.Fields.Single(field => field.FieldName == "Name").Values.Select(value => value.DisplayValue).ShouldBe(["Captain's Log", "Captain's Log"]);
        comparison.Fields.Single(field => field.FieldName == "Value").Values.Select(value => value.DisplayValue).ShouldBe(["100", "150"]);
        comparison.Fields.Single(field => field.FieldName == "Transforms.Inventory").Values.Select(value => value.DisplayValue).ShouldBe(["Starfield.esm:00000999", "Starfield.esm:00000999"]);
        comparison.Fields.Single(field => field.FieldName == "InventoryArt").Values.Select(value => value.DisplayValue).ShouldBe(["Starfield.esm:00000998", "Starfield.esm:00000998"]);
        comparison.Fields.Single(field => field.FieldName == "PreviewTransform").Values.Select(value => value.DisplayValue).ShouldBe(["Starfield.esm:00000888", "Starfield.esm:00000888"]);
        comparison.Fields.Single(field => field.FieldName == "FeaturedItemMessage").Values.Select(value => value.DisplayValue).ShouldBe(["Starfield.esm:00000777", "Starfield.esm:00000777"]);
        comparison.Fields.Single(field => field.FieldName == "Text").Values.Select(value => value.DisplayValue).ShouldBe(["Base text", "Patch text"]);
        comparison.Fields.Single(field => field.FieldName == "Teaches.MutagenObjectType").Values.Select(value => value.DisplayValue).ShouldBe(["Skill", "Skill"]);
        comparison.Fields.Single(field => field.FieldName == "Teaches.Perk").Values.Select(value => value.DisplayValue).ShouldBe(["Starfield.esm:00000666", "Starfield.esm:00000666"]);
        var keywords = comparison.Fields.Single(field => field.FieldName == "Keywords");
        keywords.Children.Single(field => field.FieldName == "Keyword [0]").Values.Select(value => value.DisplayValue).ShouldBe(["Starfield.esm:00000101", "Starfield.esm:00000101"]);
        var model = comparison.Fields.Single(field => field.FieldName == "Model");
        model.Children.Single(field => field.FieldName == "File").Values.Select(value => value.DisplayValue).ShouldBe(["Meshes\\SetDressing\\Books\\Book01.nif", "Meshes\\SetDressing\\Books\\Book01.nif"]);
        var sounds = comparison.Fields.Single(field => field.FieldName == "Sounds");
        sounds.Children.Single(field => field.FieldName == "PickupSound").Children.Single(field => field.FieldName == "Start").Values.Select(value => value.DisplayValue).ShouldBe(["pickup", "pickup"]);
    }

    [Fact]
    public void GetRecordComparison_ForDoor_MapsDoorFieldsAndChildren()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x4000);
        var nativeTerminalFormKey = CreateFormKey("Starfield.esm", 0x555);
        var doorRepository = new TestDoorRepository
        {
            Records =
            [
                CreateDoor("Base.esm", formKey, "Airlock", nativeTerminalFormKey, "Both"),
                CreateDoor("Patch.esp", formKey, "Airlock", nativeTerminalFormKey, "Positive")
            ]
        };
        var modelRepository = new TestModelRepository
        {
            Records =
            [
                CreateModel("Base.esm", RecordTypeCatalog.Door.RecordID, formKey, "Meshes\\Architecture\\Door01.nif"),
                CreateModel("Patch.esp", RecordTypeCatalog.Door.RecordID, formKey, "Meshes\\Architecture\\Door01.nif")
            ]
        };
        var service = CreateService(
            doorRepository: doorRepository,
            modelRepository: modelRepository);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.Door.RecordID, formKey);

        comparison.Fields.Single(field => field.FieldName == "Name").Values.Select(value => value.DisplayValue).ShouldBe(["Airlock", "Airlock"]);
        comparison.Fields.Single(field => field.FieldName == "NativeTerminalFormKey").Values.Select(value => value.DisplayValue).ShouldBe(["Starfield.esm:00000555", "Starfield.esm:00000555"]);
        comparison.Fields.Single(field => field.FieldName == "FacingAxisOverride").Values.Select(value => value.DisplayValue).ShouldBe(["Both", "Positive"]);
        var model = comparison.Fields.Single(field => field.FieldName == "Model");
        model.Children.Single(field => field.FieldName == "File").Values.Select(value => value.DisplayValue).ShouldBe(["Meshes\\Architecture\\Door01.nif", "Meshes\\Architecture\\Door01.nif"]);
    }

    /// <summary>
    /// Verifies that Terminal comparison renders scalar fields, marker parameters, and specification-declared script
    /// fragments.
    /// </summary>
    [Fact]
    public void GetRecordComparison_ForTerminal_MapsTerminalFieldsAndMarkerParameters()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x5000);
        var terminalRepository = new TestTerminalRepository
        {
            Records =
            [
                CreateTerminal("Base.esm", formKey, "Kiosk", "0x1", "BaseEntry"),
                CreateTerminal("Patch.esp", formKey, "Kiosk", "0x2", "PatchEntry")
            ]
        };
        var service = CreateService(terminalRepository: terminalRepository);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.Terminal.RecordID, formKey);

        comparison.Fields.Single(field => field.FieldName == "Name").Values.Select(value => value.DisplayValue).ShouldBe(["Kiosk", "Kiosk"]);
        comparison.Fields.Single(field => field.FieldName == "MarkerFlags").Values.Select(value => value.DisplayValue).ShouldBe(["1", "2"]);
        var markerParameters = comparison.Fields.Single(field => field.FieldName == "Marker Parameters");
        var firstParameter = markerParameters.Children.Single(field => field.FieldName == "Marker Parameter [0]");
        firstParameter.Children.Single(field => field.FieldName == "EntryTypes").Values.Select(value => value.DisplayValue).ShouldBe(["BaseEntry", "PatchEntry"]);
        var scriptFragments = comparison.Fields.Single(field => field.FieldName == "Script Fragments");
        var scriptFragment = scriptFragments.Children.Single(field => field.FieldName == "MenuItem");
        scriptFragment.Children.Single(field => field.FieldName == "FragmentName").Values.Select(value => value.DisplayValue)
            .ShouldBe(["BaseFragment", "PatchFragment"]);
    }

    /// <summary>
    /// Verifies that Terminal scalar parent rows are selected from the injected comparison specification while
    /// terminal child rows remain strategy-based.
    /// </summary>
    [Fact]
    public void GetRecordComparison_ForTerminal_UsesInjectedComparisonSpecification()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x5001);
        var terminalRepository = new TestTerminalRepository
        {
            Records =
            [
                CreateTerminal("Base.esm", formKey, "Kiosk", "0x1", "BaseEntry"),
                CreateTerminal("Patch.esp", formKey, "Kiosk", "0x2", "PatchEntry")
            ]
        };
        var provider = new TestRecordSpecificationProvider(
            new RecordSpecification
            {
                RecordID = SupportedRecordSpecifications.Terminal.RecordID,
                RecordType = SupportedRecordSpecifications.Terminal.RecordType,
                TableName = SupportedRecordSpecifications.Terminal.TableName,
                FriendlyName = SupportedRecordSpecifications.Terminal.FriendlyName,
                GameSupport = SupportedRecordSpecifications.Terminal.GameSupport,
                Fields = SupportedRecordSpecifications.Terminal.Fields,
                Comparison = new RecordComparisonSpecification
                {
                    Fields =
                    [
                        new RecordComparisonFieldSpecification
                        {
                            FieldName = "MarkerFlags",
                            SourcePath = "MarkerFlags",
                            ValueKind = RecordFieldValueKind.FlagSet
                        }
                    ]
                },
                ImplementationNote = "Test specification."
            });
        var service = CreateService(terminalRepository: terminalRepository, recordSpecificationProvider: provider);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.Terminal.RecordID, formKey);

        comparison.Fields.Single(field => field.FieldName == "MarkerFlags").Values.Select(value => value.DisplayValue)
            .ShouldBe(["1", "2"]);
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Name");
        comparison.Fields.ShouldNotContain(field => field.FieldName == "MenuFormKey");
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Script Fragments");
        var markerParameters = comparison.Fields.Single(field => field.FieldName == "Marker Parameters");
        markerParameters.Children.Single(field => field.FieldName == "Marker Parameter [0]")
            .Children.Single(field => field.FieldName == "EntryTypes")
            .Values.Select(value => value.DisplayValue)
            .ShouldBe(["BaseEntry", "PatchEntry"]);
    }

    /// <summary>
    /// Verifies that Terminal localized scalar rows resolve through specification metadata and the selected record
    /// text language.
    /// </summary>
    [Fact]
    public void GetRecordComparison_ForTerminal_UsesSpecificationLocalizedDisplay()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x5002);
        var terminalRepository = new TestTerminalRepository
        {
            Records =
            [
                CreateTerminal("Base.esm", formKey, "Kiosk", "0x1", "BaseEntry"),
                CreateTerminal("Patch.esp", formKey, "Kiosk", "0x2", "PatchEntry")
            ]
        };
        terminalRepository.Records[0].HeaderText = Text("Base Header");
        terminalRepository.Records[0].WelcomeText = Text("Base Welcome");
        terminalRepository.Records[1].HeaderText = Text("Patch Header");
        terminalRepository.Records[1].WelcomeText = Text("Patch Welcome");
        var localizedStringRepository = new TestRecordLocalizedStringRepository
        {
            Records =
            [
                CreateLocalizedString("Base.esm", formKey, "Name", "German", "Basis Terminal"),
                CreateLocalizedString("Patch.esp", formKey, "Name", "German", "Patch Terminal"),
                CreateLocalizedString("Base.esm", formKey, "HeaderText", "German", "Basis Kopfzeile"),
                CreateLocalizedString("Patch.esp", formKey, "HeaderText", "German", "Patch Kopfzeile"),
                CreateLocalizedString("Base.esm", formKey, "WelcomeText", "German", "Basis Willkommen"),
                CreateLocalizedString("Patch.esp", formKey, "WelcomeText", "German", "Patch Willkommen")
            ]
        };
        var gameSelectionService = new TestGameSelectionService { RecordTextLanguage = Language.German };
        var service = CreateService(
            terminalRepository: terminalRepository,
            recordLocalizedStringRepository: localizedStringRepository,
            gameSelectionService: gameSelectionService);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.Terminal.RecordID, formKey);

        comparison.Fields.Single(field => field.FieldName == "Name").Values.Select(value => value.DisplayValue)
            .ShouldBe(["Basis Terminal", "Patch Terminal"]);
        comparison.Fields.Single(field => field.FieldName == "HeaderText").Values.Select(value => value.DisplayValue)
            .ShouldBe(["Basis Kopfzeile", "Patch Kopfzeile"]);
        comparison.Fields.Single(field => field.FieldName == "WelcomeText").Values.Select(value => value.DisplayValue)
            .ShouldBe(["Basis Willkommen", "Patch Willkommen"]);
    }

    [Fact]
    public void GetRecordComparison_ForSingleColumn_KeepsValuesNeutral()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x321);
        var globalRepository = new TestGlobalRepository
        {
            Records =
            [
                CreateGlobal("Base.esm", formKey, "MyGlobal", 1.5)
            ]
        };
        var service = CreateService(globalRepository: globalRepository);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.Global.RecordID, formKey);

        comparison.Fields.Single(field => field.FieldName == "Data").State.ShouldBe(RecordComparisonValueState.Neutral);
        comparison.Fields.Single(field => field.FieldName == "Data").Values.Single().State.ShouldBe(RecordComparisonValueState.Neutral);
    }

    [Fact]
    public void GetRecordComparison_ForNonComparableCommonFields_KeepsValuesNeutral()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x654);
        var globalRepository = new TestGlobalRepository
        {
            Records =
            [
                CreateGlobal("Base.esm", formKey, "MyGlobal", 1.5),
                CreateGlobal("Patch.esp", formKey, "MyGlobal", 2.5)
            ]
        };
        var service = CreateService(globalRepository: globalRepository);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.Global.RecordID, formKey);

        comparison.Fields.Single(field => field.FieldName == "FormVersion").State.ShouldBe(RecordComparisonValueState.Neutral);
        comparison.Fields.Single(field => field.FieldName == "MajorRecordFlags").State.ShouldBe(RecordComparisonValueState.Neutral);
    }

    [Fact]
    public void GetRecordComparison_ForUnsupportedRecordType_ReturnsEmptyComparison()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x999);
        var service = CreateService();

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, "ARMO", formKey);

        comparison.RecordType.ShouldBe("ARMO");
        comparison.FormKey.ShouldBeSameAs(formKey);
        comparison.Columns.ShouldBeEmpty();
        comparison.Fields.ShouldBeEmpty();
    }

}
