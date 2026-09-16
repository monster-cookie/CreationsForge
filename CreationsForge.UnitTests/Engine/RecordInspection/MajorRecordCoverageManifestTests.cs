using System.Globalization;
using System.Text.Json;
using Shouldly;

namespace CreationsForge.UnitTests.Engine.RecordInspection;

/// <summary>Verifies checked-in major-record coverage manifests against the installed Mutagen assemblies.</summary>
public sealed class MajorRecordCoverageManifestTests
{
    /// <summary>Verifies every concrete family and every generated FieldIndex member remains accounted for in all supported games.</summary>
    [Fact]
    public void Manifests_CoverInstalledConcreteFamiliesAndIndexedFields()
    {
        ValidateManifest(
            "CreationsForge.Starfield.MajorRecordCoverage.json",
            typeof(Mutagen.Bethesda.Starfield.StarfieldMajorRecord),
            "Mutagen.Bethesda.Starfield");
        ValidateManifest(
            "CreationsForge.Fallout4.MajorRecordCoverage.json",
            typeof(Mutagen.Bethesda.Fallout4.Fallout4MajorRecord),
            "Mutagen.Bethesda.Fallout4");
        ValidateManifest(
            "CreationsForge.Skyrim.MajorRecordCoverage.json",
            typeof(Mutagen.Bethesda.Skyrim.SkyrimMajorRecord),
            "Mutagen.Bethesda.Skyrim");
    }

    /// <summary>Validates one embedded manifest against its exact installed game assembly.</summary>
    /// <param name="resourceName">The embedded manifest resource name.</param>
    /// <param name="majorRecordBase">The installed abstract mutable major-record base.</param>
    /// <param name="packageId">The exact Mutagen package identifier.</param>
    private static void ValidateManifest(string resourceName, Type majorRecordBase, string packageId)
    {
        using var stream = typeof(MajorRecordCoverageManifestTests).Assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Embedded resource {resourceName} was not found.");
        using var document = JsonDocument.Parse(stream);
        var root = document.RootElement;
        root.GetProperty("SchemaVersion").GetInt32().ShouldBe(1);
        root.GetProperty("PackageId").GetString().ShouldBe(packageId);
        root.GetProperty("PackageVersion").GetString().ShouldBe("0.55.0-alpha.53");
        root.GetProperty("MajorRecordBase").GetString().ShouldBe(majorRecordBase.FullName);
        root.GetProperty("UnsupportedShapes").GetArrayLength().ShouldBe(0);

        var assembly = majorRecordBase.Assembly;
        var installedFamilies = assembly.GetTypes()
            .Where(type => type.IsClass && !type.IsAbstract && !type.IsGenericTypeDefinition && majorRecordBase.IsAssignableFrom(type))
            .Select(type => type.FullName.ShouldNotBeNull())
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();
        var manifestedFamilies = root.GetProperty("ConcreteFamilies")
            .EnumerateArray()
            .Select(family => family.GetProperty("MutableType").GetString().ShouldNotBeNull())
            .ToArray();
        manifestedFamilies.ShouldBe(installedFamilies);
        root.GetProperty("ConcreteFamilyCount").GetInt32().ShouldBe(installedFamilies.Length);

        var indexedContracts = root.GetProperty("IndexedContracts").EnumerateArray().ToArray();
        root.GetProperty("IndexedTypeCount").GetInt32().ShouldBe(indexedContracts.Length);
        var indexedFieldCount = 0;
        foreach (var contract in indexedContracts)
        {
            var mutableTypeName = contract.GetProperty("MutableType").GetString().ShouldNotBeNull();
            var mutableType = assembly.GetType(mutableTypeName).ShouldNotBeNull();
            var fieldIndex = assembly.GetType(mutableType.FullName + "_FieldIndex").ShouldNotBeNull();
            fieldIndex.IsEnum.ShouldBeTrue();
            var installedFields = Enum.GetNames(fieldIndex)
                .OrderBy(name => Convert.ToUInt64(Enum.Parse(fieldIndex, name), CultureInfo.InvariantCulture))
                .ToArray();
            var manifestedFields = contract.GetProperty("Fields")
                .EnumerateArray()
                .Select(field =>
                {
                    field.GetProperty("DeclaredType").ValueKind.ShouldNotBe(JsonValueKind.Null);
                    field.GetProperty("Shape").GetString().ShouldNotBe("unsupported");
                    return field.GetProperty("Name").GetString().ShouldNotBeNull();
                })
                .ToArray();
            manifestedFields.ShouldBe(installedFields, $"{mutableTypeName} FieldIndex coverage changed.");
            indexedFieldCount += manifestedFields.Length;
        }

        root.GetProperty("IndexedFieldCount").GetInt32().ShouldBe(indexedFieldCount);
        var additionalContracts = root.GetProperty("AdditionalContracts").EnumerateArray().ToArray();
        root.GetProperty("AdditionalContractCount").GetInt32().ShouldBe(additionalContracts.Length);
        additionalContracts.ShouldAllBe(contract =>
            contract.GetProperty("Fields").EnumerateArray().All(field =>
                field.GetProperty("Shape").GetString() != "unsupported"));
    }
}
