using System.Reflection;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.UI;

/// <summary>
/// Guards the Spriggit DTO and comparison UI validation matrices from drifting apart.
/// </summary>
public class SpriggitUiValidationParityTests
{
    /// <summary>
    /// Verifies every DTO validation sample has one matching UI validation sample with the same identifying traits.
    /// </summary>
    [Fact]
    [Trait("Category", "SpriggitUiValidationParity")]
    public void SpriggitUiValidationMatrix_ShouldMatchDtoValidationMatrix()
    {
        var differences = GetParityDifferences();

        differences.ShouldBeEmpty(
            "The Spriggit comparison UI validation matrix should mirror the DTO validation matrix." +
            System.Environment.NewLine +
            string.Join(System.Environment.NewLine, differences));
    }

    /// <summary>
    /// Builds all parity diagnostics for the DTO and comparison UI validation matrices.
    /// </summary>
    /// <returns>A list of differences that should be fixed before adding new validation coverage.</returns>
    private static IReadOnlyList<string> GetParityDifferences()
    {
        var assembly = typeof(SpriggitUiValidationParityTests).Assembly;
        var dtoTypes = GetValidationTypes(assembly, ".Validation.Tests.", "SpriggitDataValidationTests");
        var uiTypes = GetValidationTypes(assembly, ".Validation.UI.Tests.", "SpriggitUiValidationTests");
        var differences = new List<string>();

        AddClassMatrixDifferences(differences, dtoTypes, uiTypes);
        AddSampleMatrixDifferences(differences, dtoTypes, uiTypes);
        return differences;
    }

    /// <summary>
    /// Gets validation test classes whose namespace and class suffix identify one validation matrix.
    /// </summary>
    /// <param name="assembly">The assembly containing validation test classes.</param>
    /// <param name="namespaceMarker">The namespace segment identifying the matrix.</param>
    /// <param name="classSuffix">The class-name suffix expected for the matrix.</param>
    /// <returns>The validation test classes ordered by namespace and name.</returns>
    private static IReadOnlyList<Type> GetValidationTypes(Assembly assembly, string namespaceMarker, string classSuffix)
    {
        return assembly.GetTypes()
            .Where(type => type.IsClass)
            .Where(type => type.Namespace?.Contains(namespaceMarker, StringComparison.Ordinal) == true)
            .Where(type => type.Name.EndsWith(classSuffix, StringComparison.Ordinal))
            .OrderBy(type => type.Namespace, StringComparer.Ordinal)
            .ThenBy(type => type.Name, StringComparer.Ordinal)
            .ToList();
    }

    /// <summary>
    /// Adds diagnostics for missing or extra game-and-record UI validation classes.
    /// </summary>
    /// <param name="differences">The diagnostic list to append to.</param>
    /// <param name="dtoTypes">The DTO validation classes.</param>
    /// <param name="uiTypes">The comparison UI validation classes.</param>
    private static void AddClassMatrixDifferences(IList<string> differences, IReadOnlyList<Type> dtoTypes, IReadOnlyList<Type> uiTypes)
    {
        var dtoKeys = dtoTypes.Select(type => GetMatrixKey(type, ".Validation.Tests.")).ToHashSet(StringComparer.Ordinal);
        var uiKeys = uiTypes.Select(type => GetMatrixKey(type, ".Validation.UI.Tests.")).ToHashSet(StringComparer.Ordinal);

        foreach (var key in dtoKeys.Except(uiKeys).OrderBy(value => value, StringComparer.Ordinal))
        {
            differences.Add("Missing UI validation class for " + key + ".");
        }

        foreach (var key in uiKeys.Except(dtoKeys).OrderBy(value => value, StringComparer.Ordinal))
        {
            differences.Add("Extra UI validation class without DTO validation class for " + key + ".");
        }

        var uiTypeNames = uiTypes.Select(type => type.FullName).ToHashSet(StringComparer.Ordinal);
        foreach (var dtoType in dtoTypes)
        {
            var expectedNamespace = dtoType.Namespace!.Replace(".Validation.Tests.", ".Validation.UI.Tests.", StringComparison.Ordinal);
            var expectedTypeName = dtoType.Name.Replace("SpriggitDataValidationTests", "SpriggitUiValidationTests", StringComparison.Ordinal);
            var expectedFullName = expectedNamespace + "." + expectedTypeName;
            if (!uiTypeNames.Contains(expectedFullName))
            {
                differences.Add("Missing expected UI validation type '" + expectedFullName + "' for DTO type '" + dtoType.FullName + "'.");
            }
        }
    }

    /// <summary>
    /// Adds diagnostics for missing, extra, or duplicate validation samples.
    /// </summary>
    /// <param name="differences">The diagnostic list to append to.</param>
    /// <param name="dtoTypes">The DTO validation classes.</param>
    /// <param name="uiTypes">The comparison UI validation classes.</param>
    private static void AddSampleMatrixDifferences(IList<string> differences, IReadOnlyList<Type> dtoTypes, IReadOnlyList<Type> uiTypes)
    {
        var dtoSamples = GetValidationSamples(dtoTypes, ".Validation.Tests.", "DTO");
        var uiSamples = GetValidationSamples(uiTypes, ".Validation.UI.Tests.", "UI");
        var dtoKeys = dtoSamples.Select(sample => sample.SampleKey).ToHashSet(StringComparer.Ordinal);
        var uiKeys = uiSamples.Select(sample => sample.SampleKey).ToHashSet(StringComparer.Ordinal);

        foreach (var key in dtoKeys.Except(uiKeys).OrderBy(value => value, StringComparer.Ordinal))
        {
            differences.Add("Missing UI validation sample for " + key + ".");
        }

        foreach (var key in uiKeys.Except(dtoKeys).OrderBy(value => value, StringComparer.Ordinal))
        {
            differences.Add("Extra UI validation sample without DTO validation sample for " + key + ".");
        }

        AddDuplicateSampleDifferences(differences, dtoSamples);
        AddDuplicateSampleDifferences(differences, uiSamples);
    }

    /// <summary>
    /// Gets the validation samples declared by a set of validation classes.
    /// </summary>
    /// <param name="types">The validation classes to inspect.</param>
    /// <param name="namespaceMarker">The namespace segment identifying the matrix.</param>
    /// <param name="matrixName">The human-readable matrix name for diagnostics.</param>
    /// <returns>The validation samples declared by the supplied classes.</returns>
    private static IReadOnlyList<ValidationSampleMetadata> GetValidationSamples(
        IEnumerable<Type> types,
        string namespaceMarker,
        string matrixName)
    {
        var samples = new List<ValidationSampleMetadata>();
        foreach (var type in types)
        {
            var classKey = GetMatrixKey(type, namespaceMarker);
            foreach (var method in GetValidationMethods(type))
            {
                var traits = GetTraits(method);
                samples.Add(new ValidationSampleMetadata
                {
                    MatrixName = matrixName,
                    ClassKey = classKey,
                    TypeName = type.FullName ?? type.Name,
                    MethodName = method.Name,
                    Game = GetRequiredTrait(traits, "Game", type, method),
                    RecordType = GetRequiredTrait(traits, "RecordType", type, method),
                    FormKey = GetRequiredTrait(traits, "FormKey", type, method),
                    EditorID = GetRequiredTrait(traits, "EditorID", type, method),
                    SpriggitFile = GetRequiredTrait(traits, "SpriggitFile", type, method)
                });
            }
        }

        return samples;
    }

    /// <summary>
    /// Gets public instance validation methods from one validation class.
    /// </summary>
    /// <param name="type">The validation class to inspect.</param>
    /// <returns>The validation methods declared on the class.</returns>
    private static IReadOnlyList<MethodInfo> GetValidationMethods(Type type)
    {
        return type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
            .Where(method => method.GetCustomAttributes().Any(IsFactLikeAttribute))
            .OrderBy(method => method.Name, StringComparer.Ordinal)
            .ToList();
    }

    /// <summary>
    /// Determines whether an attribute marks a validation method as an xUnit test.
    /// </summary>
    /// <param name="attribute">The method attribute to inspect.</param>
    /// <returns><c>true</c> when the attribute is a fact-like test attribute.</returns>
    private static bool IsFactLikeAttribute(object attribute)
    {
        var attributeName = attribute.GetType().Name;
        return string.Equals(attributeName, "FactAttribute", StringComparison.Ordinal) ||
            string.Equals(attributeName, "AvaloniaFactAttribute", StringComparison.Ordinal);
    }

    /// <summary>
    /// Gets xUnit trait values from a validation method.
    /// </summary>
    /// <param name="method">The validation method whose traits should be read.</param>
    /// <returns>A dictionary of trait names and values.</returns>
    private static IReadOnlyDictionary<string, string> GetTraits(MethodInfo method)
    {
        var values = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var attribute in method.GetCustomAttributes())
        {
            if (!string.Equals(attribute.GetType().Name, "TraitAttribute", StringComparison.Ordinal))
            {
                continue;
            }

            var name = attribute.GetType().GetProperty("Name")?.GetValue(attribute) as string;
            var value = attribute.GetType().GetProperty("Value")?.GetValue(attribute) as string;
            if (!string.IsNullOrWhiteSpace(name) && value is not null)
            {
                values[name] = value;
            }
        }

        return values;
    }

    /// <summary>
    /// Gets a required trait value or returns a diagnostic placeholder that will fail parity.
    /// </summary>
    /// <param name="traits">The trait dictionary for the method.</param>
    /// <param name="traitName">The required trait name.</param>
    /// <param name="type">The validation class containing the method.</param>
    /// <param name="method">The validation method being inspected.</param>
    /// <returns>The trait value or a diagnostic placeholder.</returns>
    private static string GetRequiredTrait(
        IReadOnlyDictionary<string, string> traits,
        string traitName,
        Type type,
        MethodInfo method)
    {
        return traits.TryGetValue(traitName, out var value)
            ? value
            : "<missing " + traitName + " on " + type.FullName + "." + method.Name + ">";
    }

    /// <summary>
    /// Gets the record/game matrix key from a validation class namespace.
    /// </summary>
    /// <param name="type">The validation class.</param>
    /// <param name="namespaceMarker">The namespace segment identifying the matrix.</param>
    /// <returns>A stable record/game matrix key.</returns>
    private static string GetMatrixKey(Type type, string namespaceMarker)
    {
        var namespaceText = type.Namespace ?? string.Empty;
        var markerIndex = namespaceText.IndexOf(namespaceMarker, StringComparison.Ordinal);
        if (markerIndex < 0)
        {
            return "<unknown namespace for " + type.FullName + ">";
        }

        return namespaceText[(markerIndex + namespaceMarker.Length)..].Replace('.', '/');
    }

    /// <summary>
    /// Adds diagnostics for duplicate sample identities within a validation matrix.
    /// </summary>
    /// <param name="differences">The diagnostic list to append to.</param>
    /// <param name="samples">The validation samples to inspect.</param>
    private static void AddDuplicateSampleDifferences(IList<string> differences, IReadOnlyList<ValidationSampleMetadata> samples)
    {
        foreach (var group in samples.GroupBy(sample => sample.SampleKey, StringComparer.Ordinal).Where(group => group.Count() > 1))
        {
            differences.Add(
                "Duplicate " + group.First().MatrixName + " validation sample for " + group.Key + ": " +
                string.Join(", ", group.Select(sample => sample.TypeName + "." + sample.MethodName).OrderBy(value => value, StringComparer.Ordinal)));
        }
    }

    /// <summary>
    /// Describes one validation test sample discovered from method traits.
    /// </summary>
    private sealed class ValidationSampleMetadata
    {
        /// <summary>
        /// Gets or sets the validation matrix name used in diagnostics.
        /// </summary>
        public string MatrixName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the record/game class matrix key.
        /// </summary>
        public string ClassKey { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the declaring validation class name.
        /// </summary>
        public string TypeName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the declaring validation method name.
        /// </summary>
        public string MethodName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the game trait value.
        /// </summary>
        public string Game { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the record type trait value.
        /// </summary>
        public string RecordType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the form key trait value.
        /// </summary>
        public string FormKey { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the editor ID trait value.
        /// </summary>
        public string EditorID { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Spriggit YAML file trait value.
        /// </summary>
        public string SpriggitFile { get; set; } = string.Empty;

        /// <summary>
        /// Gets the stable identity used to compare DTO and UI samples.
        /// </summary>
        public string SampleKey => ClassKey + "|" + Game + "|" + RecordType + "|" + FormKey + "|" + EditorID + "|" + SpriggitFile;
    }
}
