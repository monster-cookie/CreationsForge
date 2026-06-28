using Avalonia.Headless.XUnit;
using CreationsForge.Specification.Validation.Specs.FormList;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.UI.Tests.FormList.Starfield;

/// <summary>
/// Validates Starfield form list Spriggit samples against the rendered comparison UI.
/// </summary>
public class StarfieldFormListSpriggitUiValidationTests :
    SpriggitComparisonUiTestBase,
    IClassFixture<SpriggitComparisonUiFixture>
{
    private readonly SpriggitComparisonUiFixture fixture;

    /// <summary>
    /// Initializes the UI validation tests with shared comparison fixture state.
    /// </summary>
    /// <param name="fixture">The fixture used to resolve imported records and comparison services.</param>
    public StarfieldFormListSpriggitUiValidationTests(SpriggitComparisonUiFixture fixture)
    {
        this.fixture = fixture;
    }

    /// <summary>
    /// Validates the Starfield <c>AkilaVendorVeryHighOrganicResources</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "FLST")]
    [Trait("FormKey", "2117E6:Starfield.esm")]
    [Trait("EditorID", "AkilaVendorVeryHighOrganicResources")]
    [Trait("SpriggitFile", "FormLists/AkilaVendorVeryHighOrganicResources - 2117E6_Starfield.esm.yaml")]
    public void Starfield_FLST_ComparisonUi_ShouldRenderSpriggitSample_AkilaVendorVeryHighOrganicResources()
    {
        var spec = FormListValidationSpecs.Starfield_AkilaVendorVeryHighOrganicResources();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Starfield <c>AkilaVendorVeryLowOrganicResources</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "FLST")]
    [Trait("FormKey", "2117EC:Starfield.esm")]
    [Trait("EditorID", "AkilaVendorVeryLowOrganicResources")]
    [Trait("SpriggitFile", "FormLists/AkilaVendorVeryLowOrganicResources - 2117EC_Starfield.esm.yaml")]
    public void Starfield_FLST_ComparisonUi_ShouldRenderSpriggitSample_AkilaVendorVeryLowOrganicResources()
    {
        var spec = FormListValidationSpecs.Starfield_AkilaVendorVeryLowOrganicResources();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Starfield <c>AlikaVendorLowOrganicResources</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "FLST")]
    [Trait("FormKey", "2117F0:Starfield.esm")]
    [Trait("EditorID", "AlikaVendorLowOrganicResources")]
    [Trait("SpriggitFile", "FormLists/AlikaVendorLowOrganicResources - 2117F0_Starfield.esm.yaml")]
    public void Starfield_FLST_ComparisonUi_ShouldRenderSpriggitSample_AlikaVendorLowOrganicResources()
    {
        var spec = FormListValidationSpecs.Starfield_AlikaVendorLowOrganicResources();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Starfield <c>COND_imgui_1_Assorted</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "FLST")]
    [Trait("FormKey", "0C3830:Starfield.esm")]
    [Trait("EditorID", "COND_imgui_1_Assorted")]
    [Trait("SpriggitFile", "FormLists/COND_imgui_1_Assorted - 0C3830_Starfield.esm.yaml")]
    public void Starfield_FLST_ComparisonUi_ShouldRenderSpriggitSample_COND_imgui_1_Assorted()
    {
        var spec = FormListValidationSpecs.Starfield_COND_imgui_1_Assorted();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }
}
