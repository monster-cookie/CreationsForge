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
/// Contains record comparison scenarios for Condition Form records.
/// </summary>
public partial class RecordComparisonServiceTests
{
    /// <summary>
    /// Verifies that record comparison condition form maps version2 and conditions.
    /// </summary>
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

    /// <summary>
    /// Verifies that record comparison condition form preserves multiple condition rules.
    /// </summary>
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
}
