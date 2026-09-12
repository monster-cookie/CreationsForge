using System.Reflection;
using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeInspection;
using CreationsForge.Starfield.Native.NativeInspection;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;
using Mutagen.Bethesda.Strings;
using Noggog;
using Shouldly;

namespace CreationsForge.UnitTests.Engine.Starfield.NativeInspection.Nested;

/// <summary>
/// Verifies exact semantic behavior at representative Starfield nested-field boundaries.
/// </summary>
public sealed class StarfieldNestedFieldCodecSemanticTests
{
    /// <summary>
    /// Verifies signed-zero bits remain visible in read output, semantic comparison, and canonical fingerprints.
    /// </summary>
    [Fact]
    public void Component_PreservesSignedZeroBits()
    {
        var before = CreatePropertySheetComponent(-0.0f);
        var after = CreatePropertySheetComponent(0.0f);

        using var document = WriteComponent(before);
        document.RootElement.GetProperty("Properties")[0]
            .GetProperty("Value")
            .GetProperty("bits")
            .GetString()
            .ShouldBe("0x80000000");

        var change = CompareComponent(before, after, "Component").ShouldHaveSingleItem();
        change.FieldIdentifier.ShouldBe("Component.Properties[0].Value");
        change.Kind.ShouldBe(SemanticChangeKind.ValueChanged);

        var beforeFingerprint = FingerprintComponent(before);
        var afterFingerprint = FingerprintComponent(after);
        beforeFingerprint.IsValid.ShouldBeTrue();
        afterFingerprint.IsValid.ShouldBeTrue();
        beforeFingerprint.Fingerprint.ShouldNotBe(afterFingerprint.Fingerprint);
    }

    /// <summary>
    /// Verifies a nullable collection remains distinct from an allocated empty collection.
    /// </summary>
    [Fact]
    public void Component_DistinguishesNullAndEmptyCollection()
    {
        var before = new ActivityTrackerComponent
        {
            Activities = null,
        };
        var after = new ActivityTrackerComponent
        {
            Activities = new ExtendedList<Activity>(),
        };

        using var beforeDocument = WriteComponent(before);
        using var afterDocument = WriteComponent(after);
        beforeDocument.RootElement.GetProperty("Activities").ValueKind.ShouldBe(JsonValueKind.Null);
        afterDocument.RootElement.GetProperty("Activities").GetArrayLength().ShouldBe(0);

        var change = CompareComponent(before, after, "Component").ShouldHaveSingleItem();
        change.FieldIdentifier.ShouldBe("Component.Activities");
        change.Kind.ShouldBe(SemanticChangeKind.ValueChanged);
    }

    /// <summary>
    /// Verifies every translated entry is emitted and a non-selected language difference is detected.
    /// </summary>
    [Fact]
    public void Component_PreservesAndComparesAllTranslations()
    {
        var before = CreateActivityTrackerComponent("Avant");
        var after = CreateActivityTrackerComponent("Après");

        using var document = WriteComponent(before);
        var translations = document.RootElement.GetProperty("Activities")[0]
            .GetProperty("Name")
            .GetProperty("value")
            .GetProperty("translations")
            .EnumerateArray()
            .Select(translation => $"{translation.GetProperty("language").GetString()}={translation.GetProperty("value").GetString()}")
            .ToArray();
        translations.ShouldBe(new[] { "English=Inspection", "French=Avant" });

        var change = CompareComponent(before, after, "Component").ShouldHaveSingleItem();
        change.FieldIdentifier.ShouldBe("Component.Activities[0].Name");
        change.Kind.ShouldBe(SemanticChangeKind.ValueChanged);
    }

    /// <summary>
    /// Verifies link-or-index read output and fingerprints retain inactive state while semantic comparison ignores it.
    /// </summary>
    /// <param name="useAliases">Whether the native owner selects alias-index mode.</param>
    /// <param name="usePackageData">Whether the native owner selects package-index mode.</param>
    /// <param name="varyInactiveIndex">Whether the test varies the inactive index instead of the inactive link.</param>
    [Theory]
    [InlineData(false, false, true)]
    [InlineData(true, false, false)]
    [InlineData(false, true, false)]
    public void Condition_PreservesInactiveLinkOrIndexStateWithoutSemanticChange(
        bool useAliases,
        bool usePackageData,
        bool varyInactiveIndex)
    {
        var firstLink = new FormKey(ModKey.FromNameAndExtension("Inactive.esm"), 0x00000123);
        var secondLink = new FormKey(ModKey.FromNameAndExtension("Inactive.esm"), 0x00000124);
        var before = CreateCondition(17, firstLink, useAliases: useAliases, usePackageData: usePackageData);
        var after = CreateCondition(
            varyInactiveIndex ? 18u : 17u,
            varyInactiveIndex ? firstLink : secondLink,
            useAliases: useAliases,
            usePackageData: usePackageData);

        using var beforeDocument = WriteCondition(before);
        using var afterDocument = WriteCondition(after);
        var beforeParameter = beforeDocument.RootElement.GetProperty("Data").GetProperty("FirstParameter");
        var afterParameter = afterDocument.RootElement.GetProperty("Data").GetProperty("FirstParameter");
        beforeParameter.GetProperty("usesLink").GetBoolean().ShouldBe(!useAliases && !usePackageData);
        beforeParameter.GetProperty("usesAlias").GetBoolean().ShouldBe(useAliases);
        beforeParameter.GetProperty("usesPackageData").GetBoolean().ShouldBe(usePackageData);
        beforeParameter.GetRawText().ShouldNotBe(afterParameter.GetRawText());

        CompareCondition(before, after, "Condition").ShouldBeEmpty();

        var beforeFingerprint = FingerprintCondition(before);
        var afterFingerprint = FingerprintCondition(after);
        beforeFingerprint.IsValid.ShouldBeTrue();
        afterFingerprint.IsValid.ShouldBeTrue();
        beforeFingerprint.Fingerprint.ShouldNotBe(afterFingerprint.Fingerprint);
    }

    /// <summary>Verifies active link, active index, and alias-versus-package mode changes remain semantic changes.</summary>
    /// <param name="scenario">The active value or native owner mode to change.</param>
    [Theory]
    [InlineData("link-value")]
    [InlineData("alias-index")]
    [InlineData("package-index")]
    [InlineData("alias-package-mode")]
    public void Condition_ActiveLinkOrIndexStateChange_IsSemanticChange(string scenario)
    {
        var firstLink = new FormKey(ModKey.FromNameAndExtension("Active.esm"), 0x00000123);
        var secondLink = new FormKey(ModKey.FromNameAndExtension("Active.esm"), 0x00000124);
        (ConditionFloat Before, ConditionFloat After) conditions = scenario switch
        {
            "link-value" => (CreateCondition(17, firstLink), CreateCondition(17, secondLink)),
            "alias-index" => (
                CreateCondition(17, firstLink, useAliases: true),
                CreateCondition(18, firstLink, useAliases: true)),
            "package-index" => (
                CreateCondition(17, firstLink, usePackageData: true),
                CreateCondition(18, firstLink, usePackageData: true)),
            "alias-package-mode" => (
                CreateCondition(17, firstLink, useAliases: true),
                CreateCondition(17, firstLink, usePackageData: true)),
            _ => throw new InvalidOperationException($"Unknown link-or-index semantic scenario '{scenario}'."),
        };

        var changes = CompareCondition(conditions.Before, conditions.After, "Condition");
        if (string.Equals(scenario, "alias-package-mode", StringComparison.Ordinal))
        {
            changes.Select(change => change.FieldIdentifier).ShouldBe(new[]
            {
                "Condition.Data.UseAliases",
                "Condition.Data.UsePackageData",
                "Condition.Data.FirstParameter",
            });
            changes.All(change => change.Kind == SemanticChangeKind.ValueChanged).ShouldBeTrue();
            return;
        }

        var change = changes.ShouldHaveSingleItem();
        change.FieldIdentifier.ShouldBe("Condition.Data.FirstParameter");
        change.Kind.ShouldBe(SemanticChangeKind.ValueChanged);
    }

    /// <summary>
    /// Verifies canonical link identities ignore ModKey casing while ordinary native strings retain exact casing.
    /// </summary>
    [Fact]
    public void Condition_CanonicalFingerprintNormalizesLinkIdentityButPreservesOrdinaryStringCase()
    {
        var canonicalBefore = CreateCondition(
            17,
            new FormKey(ModKey.FromNameAndExtension("Identity.ESM"), 0x00000123),
            "Raw Value");
        var canonicalAfter = CreateCondition(
            17,
            new FormKey(ModKey.FromNameAndExtension("identity.esm"), 0x00000123),
            "Raw Value");
        var rawStringAfter = CreateCondition(
            17,
            new FormKey(ModKey.FromNameAndExtension("IDENTITY.esm"), 0x00000123),
            "raw value");

        using var beforeReadView = WriteCondition(canonicalBefore);
        using var afterReadView = WriteCondition(canonicalAfter);
        GetFirstParameterFormKey(beforeReadView).ShouldNotBe(GetFirstParameterFormKey(afterReadView));

        var beforeFingerprint = FingerprintCondition(canonicalBefore);
        var canonicalAfterFingerprint = FingerprintCondition(canonicalAfter);
        var rawStringAfterFingerprint = FingerprintCondition(rawStringAfter);

        beforeFingerprint.IsValid.ShouldBeTrue();
        canonicalAfterFingerprint.IsValid.ShouldBeTrue();
        rawStringAfterFingerprint.IsValid.ShouldBeTrue();
        beforeFingerprint.Fingerprint.ShouldBe(canonicalAfterFingerprint.Fingerprint);
        beforeFingerprint.Fingerprint.ShouldNotBe(rawStringAfterFingerprint.Fingerprint);
    }

    /// <summary>
    /// Verifies malformed UTF-16 in a generated ordinary string records a typed canonical validation failure after traversal.
    /// </summary>
    [Fact]
    public void Condition_CanonicalFingerprintReportsMalformedGeneratedString()
    {
        var condition = CreateCondition(
            17,
            formKey: null,
            firstUnusedStringParameter: "\uD800",
            secondUnusedStringParameter: "tail");
        var laterFieldChanged = CreateCondition(
            17,
            formKey: null,
            firstUnusedStringParameter: "\uD800",
            secondUnusedStringParameter: "different tail");

        var result = FingerprintCondition(condition);
        var laterFieldChangedResult = FingerprintCondition(laterFieldChanged);

        result.IsValid.ShouldBeFalse();
        result.ValidationError.ShouldNotBeNull();
        result.ValidationError!.Code.ShouldBe(EngineErrorCode.ValidationFailed);
        laterFieldChangedResult.IsValid.ShouldBeFalse();
        laterFieldChangedResult.ValidationError.ShouldNotBeNull();
        laterFieldChangedResult.ValidationError!.Code.ShouldBe(EngineErrorCode.ValidationFailed);
        result.Fingerprint.ShouldNotBe(laterFieldChangedResult.Fingerprint);
    }

    /// <summary>Creates a property-sheet component with one exact floating-point value.</summary>
    /// <param name="value">The floating-point value to preserve.</param>
    /// <returns>A native component containing the requested value.</returns>
    private static PropertySheetComponent CreatePropertySheetComponent(float value)
    {
        return new PropertySheetComponent
        {
            Properties = new ExtendedList<ObjectProperty>
            {
                new ObjectProperty
                {
                    Value = value,
                },
            },
        };
    }

    /// <summary>Creates an activity component with two deterministic translations.</summary>
    /// <param name="french">The French name translation.</param>
    /// <returns>A native activity component containing the translated value.</returns>
    private static ActivityTrackerComponent CreateActivityTrackerComponent(string french)
    {
        return new ActivityTrackerComponent
        {
            Activities = new ExtendedList<Activity>
            {
                new Activity
                {
                    Name = CreateTranslatedString("Inspection", french),
                    Description = CreateTranslatedString("Description", "Description française"),
                },
            },
        };
    }

    /// <summary>Creates a deterministic translated native string.</summary>
    /// <param name="english">The selected English value.</param>
    /// <param name="french">The preserved French translation.</param>
    /// <returns>A mutable translated string.</returns>
    private static TranslatedString CreateTranslatedString(string english, string french)
    {
        var value = new TranslatedString(Language.English, english);
        value.Set(Language.French, french);
        return value;
    }

    /// <summary>Creates a concrete condition with the requested complete link-or-index projection and native owner mode.</summary>
    /// <param name="index">The retained index, active in alias or package mode and inactive in link mode.</param>
    /// <param name="formKey">The optional retained link, active in link mode and inactive in alias or package mode.</param>
    /// <param name="firstUnusedStringParameter">The optional ordinary string stored by the native condition data.</param>
    /// <param name="secondUnusedStringParameter">The optional later ordinary string stored by the native condition data.</param>
    /// <param name="useAliases">Whether the native owner selects alias-index mode.</param>
    /// <param name="usePackageData">Whether the native owner selects package-index mode.</param>
    /// <returns>A native float condition.</returns>
    private static ConditionFloat CreateCondition(
        uint index,
        FormKey? formKey = null,
        string? firstUnusedStringParameter = null,
        string? secondUnusedStringParameter = null,
        bool useAliases = false,
        bool usePackageData = false)
    {
        var data = new BiomeHasKeywordConditionData
        {
            FirstUnusedStringParameter = firstUnusedStringParameter,
            SecondUnusedStringParameter = secondUnusedStringParameter,
            UseAliases = useAliases,
            UsePackageData = usePackageData,
        };
        data.FirstParameter.Index = index;
        if (formKey.HasValue)
        {
            data.FirstParameter.Link.SetTo(formKey.Value);
        }

        return new ConditionFloat
        {
            ComparisonValue = 1.0f,
            Data = data,
        };
    }

    /// <summary>Writes one component through the internal generated root entrypoint.</summary>
    /// <param name="component">The native component to write.</param>
    /// <returns>An owned parsed JSON document.</returns>
    private static JsonDocument WriteComponent(IAComponentGetter component)
    {
        return Write("WriteComponent", typeof(IAComponentGetter), component);
    }

    /// <summary>Writes one condition through the internal generated root entrypoint.</summary>
    /// <param name="condition">The native condition to write.</param>
    /// <returns>An owned parsed JSON document.</returns>
    private static JsonDocument WriteCondition(IConditionGetter condition)
    {
        return Write("WriteCondition", typeof(IConditionGetter), condition);
    }

    /// <summary>Reads the exact read-view FormKey text stored by the first condition parameter.</summary>
    /// <param name="document">The complete generated condition read view.</param>
    /// <returns>The non-null active-link FormKey text.</returns>
    private static string GetFirstParameterFormKey(JsonDocument document)
    {
        return document.RootElement
            .GetProperty("Data")
            .GetProperty("FirstParameter")
            .GetProperty("link")
            .GetProperty("formKey")
            .GetString()
            ?? throw new InvalidOperationException("The condition parameter FormKey was null.");
    }

    /// <summary>Creates one canonical edit fingerprint through the generated component traversal.</summary>
    /// <param name="component">The concrete native component to traverse.</param>
    /// <returns>The completed fingerprint and any per-call validation diagnostic.</returns>
    private static NativeEditFingerprintResult FingerprintComponent(IAComponentGetter component)
    {
        var method = GetCodecMethod(
            "WriteComponent",
            typeof(Utf8JsonWriter),
            typeof(IAComponentGetter),
            typeof(CancellationToken),
            typeof(NativeJsonWriteContext));
        return NativeEditFingerprintFactory.Create(
            "starfield-component",
            (writer, context) => Invoke(
                method,
                writer,
                component,
                TestContext.Current.CancellationToken,
                context));
    }

    /// <summary>Creates one canonical edit fingerprint through the generated condition traversal.</summary>
    /// <param name="condition">The concrete native condition to traverse.</param>
    /// <returns>The completed fingerprint and any per-call validation diagnostic.</returns>
    private static NativeEditFingerprintResult FingerprintCondition(IConditionGetter condition)
    {
        var method = GetCodecMethod(
            "WriteCondition",
            typeof(Utf8JsonWriter),
            typeof(IConditionGetter),
            typeof(CancellationToken),
            typeof(NativeJsonWriteContext));
        return NativeEditFingerprintFactory.Create(
            "starfield-condition",
            (writer, context) => Invoke(
                method,
                writer,
                condition,
                TestContext.Current.CancellationToken,
                context));
    }

    /// <summary>Invokes one generated write entrypoint and parses its object output.</summary>
    /// <param name="methodName">The exact generated entrypoint name.</param>
    /// <param name="valueType">The native root getter parameter type.</param>
    /// <param name="value">The native root getter value.</param>
    /// <returns>An owned parsed JSON document.</returns>
    private static JsonDocument Write(string methodName, Type valueType, object value)
    {
        var method = GetCodecMethod(methodName, typeof(Utf8JsonWriter), valueType, typeof(CancellationToken));
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream))
        {
            Invoke(method, writer, value, TestContext.Current.CancellationToken);
        }

        return JsonDocument.Parse(stream.ToArray());
    }

    /// <summary>Compares two components through the internal generated root entrypoint.</summary>
    /// <param name="before">The prior native component.</param>
    /// <param name="after">The resulting native component.</param>
    /// <param name="path">The stable component path.</param>
    /// <returns>The ordered semantic changes.</returns>
    private static IReadOnlyList<SemanticChangeDescriptor> CompareComponent(
        IAComponentGetter before,
        IAComponentGetter after,
        string path)
    {
        return Compare("CompareComponent", typeof(IAComponentGetter), before, after, path);
    }

    /// <summary>Compares two conditions through the internal generated root entrypoint.</summary>
    /// <param name="before">The prior native condition.</param>
    /// <param name="after">The resulting native condition.</param>
    /// <param name="path">The stable condition path.</param>
    /// <returns>The ordered semantic changes.</returns>
    private static IReadOnlyList<SemanticChangeDescriptor> CompareCondition(
        IConditionGetter before,
        IConditionGetter after,
        string path)
    {
        return Compare("CompareCondition", typeof(IConditionGetter), before, after, path);
    }

    /// <summary>Invokes one generated comparison entrypoint.</summary>
    /// <param name="methodName">The exact generated entrypoint name.</param>
    /// <param name="valueType">The native root getter parameter type.</param>
    /// <param name="before">The prior native root value.</param>
    /// <param name="after">The resulting native root value.</param>
    /// <param name="path">The stable native root path.</param>
    /// <returns>The ordered semantic changes.</returns>
    private static IReadOnlyList<SemanticChangeDescriptor> Compare(
        string methodName,
        Type valueType,
        object before,
        object after,
        string path)
    {
        var method = GetCodecMethod(
            methodName,
            valueType,
            valueType,
            typeof(string),
            typeof(ICollection<SemanticChangeDescriptor>),
            typeof(CancellationToken));
        var changes = new List<SemanticChangeDescriptor>();
        Invoke(method, before, after, path, changes, TestContext.Current.CancellationToken);
        return changes;
    }

    /// <summary>Resolves one exact nonpublic generated codec method.</summary>
    /// <param name="methodName">The exact generated method name.</param>
    /// <param name="parameterTypes">The exact generated parameter types.</param>
    /// <returns>The matching generated method.</returns>
    private static MethodInfo GetCodecMethod(string methodName, params Type[] parameterTypes)
    {
        var codecType = typeof(StarfieldFormListNativeInspector).Assembly.GetType(
            "CreationsForge.Starfield.Native.NativeInspection.StarfieldNestedFieldCodec",
            throwOnError: true)!;
        return codecType.GetMethod(
                methodName,
                BindingFlags.NonPublic | BindingFlags.Static,
                binder: null,
                types: parameterTypes,
                modifiers: null)
            ?? throw new InvalidOperationException($"Generated method {methodName} was not found.");
    }

    /// <summary>Invokes one generated method and preserves its concrete inner failure.</summary>
    /// <param name="method">The generated method to invoke.</param>
    /// <param name="arguments">The generated method arguments.</param>
    private static void Invoke(MethodInfo method, params object?[] arguments)
    {
        try
        {
            method.Invoke(null, arguments);
        }
        catch (TargetInvocationException exception)
        {
            throw exception.InnerException ?? exception;
        }
    }
}
