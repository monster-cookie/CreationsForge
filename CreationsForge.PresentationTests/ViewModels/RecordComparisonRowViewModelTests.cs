using CreationsForge.Core.DTOs.Records;
using CreationsForge.ViewModels;
using Shouldly;

namespace CreationsForge.PresentationTests.ViewModels;

public class RecordComparisonRowViewModelTests
{
    [Fact]
    public void Constructor_AssignsParentFieldNameToChildRows()
    {
        var row = new RecordComparisonRowViewModel(
            "Components.AnimationGraphComponent.REFL",
            [],
            [
                new RecordComparisonFieldDTO
                {
                    FieldName = "Value",
                    Values = [],
                    Children = []
                }
            ]);

        row.Children.Single().ParentFieldName.ShouldBe("Components.AnimationGraphComponent.REFL");
    }
}
