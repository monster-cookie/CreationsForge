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
/// Contains record comparison scenarios for Constructible Object records.
/// </summary>
public partial class RecordComparisonServiceTests
{
    /// <summary>
    /// Verifies that record comparison constructible object maps components conditions scripts and created object count.
    /// </summary>
    [Fact]
    public void GetRecordComparison_ForConstructibleObject_MapsComponentsConditionsScriptsAndCreatedObjectCount()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x2500);
        var createdObjectFormKey = CreateFormKey("Starfield.esm", 0x111);
        var workbenchKeywordFormKey = CreateFormKey("Starfield.esm", 0x222);
        var componentFormKey = CreateFormKey("Starfield.esm", 0x333);
        var recipeFilterFormKey = CreateFormKey("Starfield.esm", 0x444);
        var constructibleObjectRepository = new TestConstructibleObjectRepository
        {
            Records =
            [
                CreateConstructibleObject("Base.esm", formKey, createdObjectFormKey, workbenchKeywordFormKey, componentFormKey, recipeFilterFormKey, 2),
                CreateConstructibleObject("Patch.esp", formKey, createdObjectFormKey, workbenchKeywordFormKey, componentFormKey, recipeFilterFormKey, 4)
            ]
        };
        var scriptingAdapterRepository = new TestScriptingAdapterRepository
        {
            Records =
            [
                CreateScriptingAdapter("Base.esm", RecordTypeCatalog.ConstructibleObject.RecordID, formKey, "RecipeScript", "Enabled", "True"),
                CreateScriptingAdapter("Patch.esp", RecordTypeCatalog.ConstructibleObject.RecordID, formKey, "RecipeScript", "Enabled", "False")
            ]
        };
        var service = CreateService(
            constructibleObjectRepository: constructibleObjectRepository,
            scriptingAdapterRepository: scriptingAdapterRepository);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.ConstructibleObject.RecordID, formKey);

        comparison.RecordType.ShouldBe(RecordTypeCatalog.ConstructibleObject.RecordID);
        comparison.Fields.Single(field => field.FieldName == "CreatedObjectFormKey").Values.Select(value => value.DisplayValue).ShouldBe(["Starfield.esm:00000111", "Starfield.esm:00000111"]);
        comparison.Fields.Single(field => field.FieldName == "WorkbenchKeywordFormKey").Values.Select(value => value.DisplayValue).ShouldBe(["Starfield.esm:00000222", "Starfield.esm:00000222"]);
        comparison.Fields.Single(field => field.FieldName == "CreatedObjectCount").Values.Select(value => value.DisplayValue).ShouldBe(["2", "4"]);
        comparison.Fields.Single(field => field.FieldName == "AmountProduced").Values.Select(value => value.DisplayValue).ShouldBe(["2", "4"]);
        var components = comparison.Fields.Single(field => field.FieldName == "Components");
        var component = components.Children.Single(field => field.FieldName == "Component [0]");
        component.Children.Single(field => field.FieldName == "ComponentFormKey").Values.Select(value => value.DisplayValue).ShouldBe(["Starfield.esm:00000333", "Starfield.esm:00000333"]);
        component.Children.Single(field => field.FieldName == "Count").Values.Select(value => value.DisplayValue).ShouldBe(["3", "3"]);
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Categories");
        var recipeFilters = comparison.Fields.Single(field => field.FieldName == "RecipeFilters");
        recipeFilters.Children.Single(field => field.FieldName == "RecipeFilter [0]").Children.Single(field => field.FieldName == "RecipeFilterFormKey").Values.Select(value => value.DisplayValue).ShouldBe(["Starfield.esm:00000444", "Starfield.esm:00000444"]);
        var conditions = comparison.Fields.Single(field => field.FieldName == "Conditions");
        var condition = conditions.Children.Single();
        condition.FieldName.ShouldBe("Condition [0]");
        condition.Values.Select(value => value.DisplayValue).ShouldBe(["GetItemCount() EqualTo 2", "GetItemCount() EqualTo 4"]);
        condition.State.ShouldBe(RecordComparisonValueState.Conflict);
        condition.Values.Select(value => value.State).ShouldBe([RecordComparisonValueState.Conflict, RecordComparisonValueState.WinningOverride]);
        condition.Children.ShouldBeEmpty();
        var scripts = comparison.Fields.Single(field => field.FieldName == "Scripts");
        scripts.Children.Single(field => field.FieldName == "Script [0]").Children.Single(field => field.FieldName == "Name").Values.Select(value => value.DisplayValue).ShouldBe(["RecipeScript", "RecipeScript"]);
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Created Object Counts");
    }

    /// <summary>
    /// Verifies that Constructible Object scalar rows are selected from the injected comparison specification while
    /// child rows remain strategy-based.
    /// </summary>
    [Fact]
    public void GetRecordComparison_ForConstructibleObject_UsesInjectedComparisonSpecification()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x2501);
        var createdObjectFormKey = CreateFormKey("Starfield.esm", 0x111);
        var workbenchKeywordFormKey = CreateFormKey("Starfield.esm", 0x222);
        var componentFormKey = CreateFormKey("Starfield.esm", 0x333);
        var recipeFilterFormKey = CreateFormKey("Starfield.esm", 0x444);
        var constructibleObjectRepository = new TestConstructibleObjectRepository
        {
            Records =
            [
                CreateConstructibleObject("Base.esm", formKey, createdObjectFormKey, workbenchKeywordFormKey, componentFormKey, recipeFilterFormKey, 2),
                CreateConstructibleObject("Patch.esp", formKey, createdObjectFormKey, workbenchKeywordFormKey, componentFormKey, recipeFilterFormKey, 4)
            ]
        };
        var provider = new TestRecordSpecificationProvider(
            new RecordSpecification
            {
                RecordID = SupportedRecordSpecifications.ConstructibleObject.RecordID,
                RecordType = SupportedRecordSpecifications.ConstructibleObject.RecordType,
                TableName = SupportedRecordSpecifications.ConstructibleObject.TableName,
                FriendlyName = SupportedRecordSpecifications.ConstructibleObject.FriendlyName,
                GameSupport = SupportedRecordSpecifications.ConstructibleObject.GameSupport,
                Fields = SupportedRecordSpecifications.ConstructibleObject.Fields,
                Comparison = new RecordComparisonSpecification
                {
                    Fields =
                    [
                        new RecordComparisonFieldSpecification
                        {
                            FieldName = "AmountProduced",
                            SourcePath = "AmountProduced",
                            ValueKind = RecordFieldValueKind.Number
                        }
                    ]
                },
                ImplementationNote = "Test specification."
            });
        var service = CreateService(
            constructibleObjectRepository: constructibleObjectRepository,
            recordSpecificationProvider: provider);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.ConstructibleObject.RecordID, formKey);

        comparison.Fields.Single(field => field.FieldName == "AmountProduced").Values.Select(value => value.DisplayValue)
            .ShouldBe(["2", "4"]);
        comparison.Fields.ShouldNotContain(field => field.FieldName == "CreatedObjectFormKey");
        comparison.Fields.ShouldNotContain(field => field.FieldName == "WorkbenchKeywordFormKey");
        comparison.Fields.Single(field => field.FieldName == "Components").Children.ShouldNotBeEmpty();
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Conditions");
    }
}
