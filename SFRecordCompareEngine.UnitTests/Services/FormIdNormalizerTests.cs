using SFRecordCompareEngine.Core.Services;
using Shouldly;

namespace SFRecordCompareEngine.UnitTests.Services;

public class FormIdNormalizerTests
{
    [Theory]
    [InlineData("03F551:Starfield.esm", "03F551")]
    [InlineData("F551:Starfield.esm", "00F551")]
    [InlineData("1:SomePlugin.esm", "000001")]
    [InlineData("formid:2f7c8:Starfield.esm <Starfield.IStarfieldMajorRecordGetter>", "02F7C8")]
    public void NormalizeFromFormKey_WhenValueIsValid_ReturnsSixCharacterUppercaseFormId(string formKey, string expected)
    {
        var result = FormIdNormalizer.NormalizeFromFormKey(formKey);

        result.ShouldBe(expected);
    }

    [Theory]
    [InlineData("")]
    [InlineData("NotHex:Starfield.esm")]
    [InlineData("1234567:Starfield.esm")]
    public void NormalizeFromFormKey_WhenValueIsInvalid_Throws(string formKey)
    {
        Should.Throw<ArgumentException>(() => FormIdNormalizer.NormalizeFromFormKey(formKey));
    }
}
