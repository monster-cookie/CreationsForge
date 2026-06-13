using CreationsForge.Services;
using Shouldly;
using System.Reflection;

namespace CreationsForge.PresentationTests.Services;

public class UserDialogServiceTests
{
    [Fact]
    public void FormatHexPayload_AddsPrintableStringColumn()
    {
        var formattedPayload = FormatHexPayload("42455448080000000400000009000000");

        formattedPayload.ShouldBe("00000000  42 45 54 48 08 00 00 00 04 00 00 00 09 00 00 00  BETH............");
    }

    private static string FormatHexPayload(string payloadValue)
    {
        var method = typeof(UserDialogService).GetMethod("FormatHexPayload", BindingFlags.NonPublic | BindingFlags.Static);
        method.ShouldNotBeNull();
        return (string)method.Invoke(null, [payloadValue])!;
    }
}
