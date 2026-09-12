using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeInspection;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Strings;
using Shouldly;

namespace CreationsForge.UnitTests.Engine.NativeInspection;

/// <summary>Verifies exact shared native leaf JSON and semantic comparison behavior.</summary>
public sealed class NativeInspectionLeafTests
{
    /// <summary>Verifies signed zero and distinct NaN payloads remain observable comparison differences.</summary>
    [Fact]
    public void BitwiseEquals_PreservesExactFloatingPointBits()
    {
        var firstNaN = BitConverter.Int32BitsToSingle(unchecked((int)0x7FC00001));
        var secondNaN = BitConverter.Int32BitsToSingle(unchecked((int)0x7FC00002));

        NativeSemanticComparer.BitwiseEquals(0F, -0F).ShouldBeFalse();
        NativeSemanticComparer.BitwiseEquals(firstNaN, firstNaN).ShouldBeTrue();
        NativeSemanticComparer.BitwiseEquals(firstNaN, secondNaN).ShouldBeFalse();
        NativeSemanticComparer.BitwiseEquals(0D, -0D).ShouldBeFalse();
    }

    /// <summary>Verifies float JSON carries exact bits and omits an invalid JSON number for NaN.</summary>
    [Fact]
    public void WriteSingle_PreservesBitsForFiniteAndNonFiniteValues()
    {
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream))
        {
            writer.WriteStartArray();
            NativeJsonLeafWriter.WriteSingle(writer, -0F);
            NativeJsonLeafWriter.WriteSingle(writer, BitConverter.Int32BitsToSingle(unchecked((int)0x7FC00001)));
            writer.WriteEndArray();
            writer.Flush();
        }

        using var document = JsonDocument.Parse(stream.ToArray());
        var values = document.RootElement.EnumerateArray().ToArray();
        values[0].GetProperty("bits").GetString().ShouldBe("0x80000000");
        values[0].GetProperty("number").GetSingle().ShouldBe(-0F);
        values[1].GetProperty("bits").GetString().ShouldBe("0x7FC00001");
        values[1].TryGetProperty("number", out _).ShouldBeFalse();
    }

    /// <summary>Verifies canonical native identities ignore ModKey filename casing while preserving numeric identity and ordinary string case.</summary>
    [Fact]
    public void NativeEditFingerprintFactory_CanonicalizesOnlyNativeIdentityCasing()
    {
        var upperKey = new FormKey(ModKey.FromNameAndExtension("Source.ESP"), 0x800);
        var lowerKey = new FormKey(ModKey.FromNameAndExtension("source.esp"), 0x800);
        var differentId = new FormKey(ModKey.FromNameAndExtension("SOURCE.esp"), 0x801);

        var upperIdentity = CreateFormKeyFingerprint(upperKey);
        var lowerIdentity = CreateFormKeyFingerprint(lowerKey);
        var differentIdentity = CreateFormKeyFingerprint(differentId);
        var upperString = CreateStringFingerprint("Value");
        var lowerString = CreateStringFingerprint("value");

        upperKey.ModKey.Name.ShouldNotBe(lowerKey.ModKey.Name);
        upperIdentity.Fingerprint.ShouldBe(lowerIdentity.Fingerprint);
        upperIdentity.IsValid.ShouldBeTrue();
        differentIdentity.Fingerprint.ShouldNotBe(upperIdentity.Fingerprint);
        upperString.Fingerprint.ShouldNotBe(lowerString.Fingerprint);
    }

    /// <summary>Verifies malformed UTF-16 produces a stable rejected identity without collapsing distinct invalid payloads or later fields.</summary>
    [Fact]
    public void NativeEditFingerprintFactory_WithMalformedUtf16_HashesCompleteRejectedPayload()
    {
        var malformed = string.Concat("A", (char)0xD800, "B");
        var distinctMalformed = string.Concat("A", (char)0xD801, "B");

        var first = CreateStringFingerprint(malformed, "tail");
        var replay = CreateStringFingerprint(malformed, "tail");
        var distinctInvalid = CreateStringFingerprint(distinctMalformed, "tail");
        var distinctTail = CreateStringFingerprint(malformed, "different-tail");

        first.ValidationError.ShouldNotBeNull();
        first.ValidationError.Code.ShouldBe(EngineErrorCode.ValidationFailed);
        replay.Fingerprint.ShouldBe(first.Fingerprint);
        distinctInvalid.Fingerprint.ShouldNotBe(first.Fingerprint);
        distinctTail.Fingerprint.ShouldNotBe(first.Fingerprint);
    }

    /// <summary>Verifies ordered comparison preserves null versus empty, duplicate positions, and collection order.</summary>
    [Fact]
    public void CompareOrdered_DetectsNullEmptyDuplicatesAndOrder()
    {
        var nullEmptyChanges = new List<SemanticChangeDescriptor>();
        NativeSemanticComparer.CompareOrdered<string>(
            "Items",
            null,
            Array.Empty<string>(),
            StringComparer.Ordinal.Equals,
            nullEmptyChanges,
            CancellationToken.None);
        var orderedChanges = new List<SemanticChangeDescriptor>();
        NativeSemanticComparer.CompareOrdered(
            "Items",
            new[] { "A", "A", "B" },
            new[] { "A", "B", "A" },
            StringComparer.Ordinal.Equals,
            orderedChanges,
            CancellationToken.None);

        nullEmptyChanges.Single().Kind.ShouldBe(SemanticChangeKind.ValueChanged);
        orderedChanges.Count.ShouldBe(2);
        orderedChanges.Select(change => change.BeforePosition).ShouldBe(new int?[] { 1, 2 });
        orderedChanges.Select(change => change.AfterPosition).ShouldBe(new int?[] { 1, 2 });
        orderedChanges.All(change => change.Kind == SemanticChangeKind.ItemChanged).ShouldBeTrue();
    }

    /// <summary>Verifies translated-string equality and JSON use language keys rather than insertion history.</summary>
    [Fact]
    public void TranslatedStrings_UseDeterministicLanguageKeySemantics()
    {
        var before = new TranslatedString(
            Language.English,
            new KeyValuePair<Language, string>(Language.French, string.Empty),
            new KeyValuePair<Language, string>(Language.English, "English"));
        var reordered = new TranslatedString(
            Language.English,
            new KeyValuePair<Language, string>(Language.English, "English"),
            new KeyValuePair<Language, string>(Language.French, string.Empty));
        var changed = new TranslatedString(
            Language.English,
            new KeyValuePair<Language, string>(Language.English, "English"),
            new KeyValuePair<Language, string>(Language.French, "Français"));

        NativeSemanticComparer.TranslatedStringEquals(before, reordered, CancellationToken.None).ShouldBeTrue();
        NativeSemanticComparer.TranslatedStringEquals(before, changed, CancellationToken.None).ShouldBeFalse();
        WriteTranslatedStringJson(before).ShouldBe(WriteTranslatedStringJson(reordered));

        using var document = JsonDocument.Parse(WriteTranslatedStringJson(before));
        var expectedLanguages = new[] { Language.English, Language.French }
            .OrderBy(language => (int)language)
            .Select(language => language.ToString())
            .ToArray();
        document.RootElement.GetProperty("translations")
            .EnumerateArray()
            .Select(translation => translation.GetProperty("language").GetString())
            .ShouldBe(expectedLanguages);
    }

    /// <summary>Verifies root record presence produces the canonical insertion and removal locations.</summary>
    [Fact]
    public void CompareRecordPresence_UsesCanonicalRootDescriptor()
    {
        var inserted = new List<SemanticChangeDescriptor>();
        var removed = new List<SemanticChangeDescriptor>();

        NativeSemanticComparer.CompareRecordPresence(null, Moq.Mock.Of<Mutagen.Bethesda.Plugins.Records.IMajorRecordGetter>(), inserted, CancellationToken.None).ShouldBeFalse();
        NativeSemanticComparer.CompareRecordPresence(Moq.Mock.Of<Mutagen.Bethesda.Plugins.Records.IMajorRecordGetter>(), null, removed, CancellationToken.None).ShouldBeFalse();

        inserted.Single().FieldIdentifier.ShouldBe("$record");
        inserted.Single().Kind.ShouldBe(SemanticChangeKind.ItemInserted);
        removed.Single().FieldIdentifier.ShouldBe("$record");
        removed.Single().Kind.ShouldBe(SemanticChangeKind.ItemRemoved);
    }

    /// <summary>Writes one translated string through the shared leaf writer for deterministic JSON assertions.</summary>
    /// <param name="value">The native translated string to serialize.</param>
    /// <returns>The complete UTF-8 JSON text.</returns>
    private static string WriteTranslatedStringJson(ITranslatedStringGetter value)
    {
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream))
        {
            NativeJsonLeafWriter.WriteTranslatedString(writer, value, CancellationToken.None);
            writer.Flush();
        }

        return System.Text.Encoding.UTF8.GetString(stream.ToArray());
    }

    /// <summary>Creates a canonical fingerprint containing one nullable native FormKey.</summary>
    /// <param name="formKey">The exact structural native identity.</param>
    /// <returns>The complete fingerprint and validation result.</returns>
    private static NativeEditFingerprintResult CreateFormKeyFingerprint(FormKey formKey)
    {
        return NativeEditFingerprintFactory.Create(
            "test.form-key",
            (writer, context) => NativeJsonLeafWriter.WriteFormKey(writer, formKey, context));
    }

    /// <summary>Creates a canonical fingerprint containing one exact string and an optional trailing field.</summary>
    /// <param name="value">The exact string payload, including malformed UTF-16 when supplied.</param>
    /// <param name="tail">An optional later field used to prove complete-payload hashing.</param>
    /// <returns>The complete fingerprint and validation result.</returns>
    private static NativeEditFingerprintResult CreateStringFingerprint(string value, string? tail = null)
    {
        return NativeEditFingerprintFactory.Create(
            "test.string",
            (writer, context) =>
            {
                writer.WriteStartObject();
                writer.WritePropertyName("value");
                NativeJsonLeafWriter.WriteString(writer, value, context);
                writer.WritePropertyName("tail");
                NativeJsonLeafWriter.WriteString(writer, tail, context);
                writer.WriteEndObject();
            });
    }
}
