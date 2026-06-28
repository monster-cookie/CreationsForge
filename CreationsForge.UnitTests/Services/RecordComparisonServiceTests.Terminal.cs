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
/// Contains record comparison scenarios for Terminal records.
/// </summary>
public partial class RecordComparisonServiceTests
{
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
}
