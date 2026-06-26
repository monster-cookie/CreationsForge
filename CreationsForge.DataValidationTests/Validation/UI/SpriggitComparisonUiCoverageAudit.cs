using System.Globalization;
using CreationsForge.DataValidationTests.Validation.Specs;

namespace CreationsForge.DataValidationTests.Validation.UI;

/// <summary>
/// Audits whether Spriggit validation specs have meaningful comparison UI coverage for validated DTO data.
/// </summary>
public static class SpriggitComparisonUiCoverageAudit
{
    private static readonly IReadOnlyDictionary<string, string> SingularRowNames =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["ActorEffects"] = "ActorEffect",
            ["Activities"] = "Activity",
            ["BodyTexts"] = "BodyText",
            ["Categories"] = "Category",
            ["Components"] = "Component",
            ["Conditions"] = "Condition",
            ["Effects"] = "Effect",
            ["FaceDialPositions"] = "FaceDialPosition",
            ["FaceMorphGroups"] = "FaceMorph",
            ["FaceMorphs"] = "FaceMorph",
            ["FaceTintingLayers"] = "FaceTintingLayer",
            ["Factions"] = "Faction",
            ["ForcedLocations"] = "ForcedLocation",
            ["HeadParts"] = "HeadPart",
            ["Items"] = "Item",
            ["MenuItems"] = "MenuItem",
            ["MorphBlends"] = "MorphBlend",
            ["MorphGroups"] = "MorphGroup",
            ["Morphs"] = "Morph",
            ["Packages"] = "Package",
            ["Perks"] = "Perk",
            ["Properties"] = "Property",
            ["Ranks"] = "Rank",
            ["RecipeFilters"] = "RecipeFilter",
            ["Relations"] = "Relation",
            ["Scripts"] = "Script",
            ["Stages"] = "Stage",
            ["Tints"] = "Tint",
            ["TintLayers"] = "TintLayer"
        };

    private static readonly IReadOnlySet<string> IgnoredDtoPathPrefixes =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "LocalizedStrings",
            "RecordType",
            "RawPayloads",
            "StructuredValues"
        };

    private static readonly IReadOnlySet<string> IgnoredDtoPathSuffixes =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".Count",
            ".FormKey",
            ".Game",
            ".ImportedAtUTC",
            ".ModKey"
        };

    /// <summary>
    /// Audits a set of validation specs against rendered comparison rows.
    /// </summary>
    /// <param name="specs">The validation specs to audit.</param>
    /// <param name="fixture">The UI comparison fixture used to resolve records and render rows.</param>
    /// <returns>Coverage diagnostics grouped by validation sample.</returns>
    public static IReadOnlyList<SpriggitComparisonUiCoverageDiagnostic> GetDiagnostics(
        IEnumerable<ValidationSpec> specs,
        SpriggitComparisonUiFixture fixture)
    {
        var diagnostics = new List<SpriggitComparisonUiCoverageDiagnostic>();
        foreach (var spec in specs.OrderBy(spec => spec.Game).ThenBy(spec => spec.RecordType.RecordID, StringComparer.Ordinal).ThenBy(spec => spec.SampleName, StringComparer.Ordinal))
        {
            AddSpecDiagnostics(diagnostics, spec, fixture);
        }

        return diagnostics;
    }

    /// <summary>
    /// Adds audit diagnostics for one validation spec.
    /// </summary>
    /// <param name="diagnostics">The diagnostic list to append to.</param>
    /// <param name="spec">The validation spec being audited.</param>
    /// <param name="fixture">The UI comparison fixture used to resolve records and render rows.</param>
    private static void AddSpecDiagnostics(
        IList<SpriggitComparisonUiCoverageDiagnostic> diagnostics,
        ValidationSpec spec,
        SpriggitComparisonUiFixture fixture)
    {
        var sample = fixture.CreateSample(spec);
        var specDiagnostics = ValidationSpecRunner.GetCoverageDiagnostics(spec, sample.Record);
        if (specDiagnostics.Count > 0)
        {
            diagnostics.Add(CreateDiagnostic(
                spec,
                "SpecCoverageFailed",
                null,
                true,
                "DTO validation coverage diagnostics must be fixed before UI coverage can be audited: " +
                string.Join(" | ", specDiagnostics)));
            return;
        }

        var assertionCases = ValidationSpecRunner.GetAssertionCases(spec, sample.Record)
            .Where(assertion => IsAuditableDtoPath(assertion.DtoPath))
            .ToList();
        if (assertionCases.Count == 0)
        {
            return;
        }

        if (spec.UiComparisonExpectations.Count == 0)
        {
            diagnostics.Add(CreateDiagnostic(
                spec,
                "SpecOnlyNoUiExpectations",
                null,
                true,
                "Spec validates " + assertionCases.Count.ToString(CultureInfo.InvariantCulture) +
                " DTO value(s), but the headless UI test falls back to the default EditorID-only assertion. Examples: " +
                string.Join(", ", assertionCases.Select(assertion => assertion.DtoPath).Distinct(StringComparer.OrdinalIgnoreCase).Take(8))));
            return;
        }

        var renderedRows = SpriggitComparisonUiSpecRunner.GetRenderedRows(spec, fixture);
        var renderedPaths = renderedRows.Select(row => row.FormattedPath).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var explicitDtoPaths = spec.UiComparisonExpectations
            .Where(expectation => !string.IsNullOrWhiteSpace(expectation.DtoPath))
            .Select(expectation => expectation.DtoPath!)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        AddMissingComparisonRowDiagnostics(diagnostics, spec, assertionCases, renderedPaths, explicitDtoPaths);
    }

    /// <summary>
    /// Adds diagnostics for validated DTO paths whose conservative row-path candidates are absent from the comparison UI.
    /// </summary>
    /// <param name="diagnostics">The diagnostic list to append to.</param>
    /// <param name="spec">The validation spec being audited.</param>
    /// <param name="assertionCases">The auditable DTO assertion cases produced by the spec runner.</param>
    /// <param name="renderedPaths">The rendered comparison row paths.</param>
    /// <param name="explicitDtoPaths">DTO paths already covered by explicit UI expectations.</param>
    private static void AddMissingComparisonRowDiagnostics(
        IList<SpriggitComparisonUiCoverageDiagnostic> diagnostics,
        ValidationSpec spec,
        IReadOnlyList<ValidationAssertionCase> assertionCases,
        IReadOnlySet<string> renderedPaths,
        IReadOnlySet<string> explicitDtoPaths)
    {
        foreach (var assertion in assertionCases)
        {
            if (explicitDtoPaths.Contains(assertion.DtoPath))
            {
                continue;
            }

            var candidatePaths = GetCandidateRowPaths(assertion.DtoPath)
                .Select(path => string.Join("/", path))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
            if (candidatePaths.Count == 0 || candidatePaths.Any(renderedPaths.Contains))
            {
                continue;
            }

            diagnostics.Add(CreateDiagnostic(
                spec,
                "ValidatedDtoFieldMissingFromComparison",
                assertion.DtoPath,
                false,
                "No rendered comparison row matched conservative candidate path(s): " + string.Join(", ", candidatePaths)));
        }
    }

    /// <summary>
    /// Determines whether a DTO assertion path should be considered for comparison UI display coverage.
    /// </summary>
    /// <param name="dtoPath">The flattened DTO assertion path.</param>
    /// <returns><c>true</c> when the path represents user-facing record data.</returns>
    private static bool IsAuditableDtoPath(string dtoPath)
    {
        if (IgnoredDtoPathPrefixes.Any(prefix =>
                dtoPath.Equals(prefix, StringComparison.OrdinalIgnoreCase) ||
                dtoPath.StartsWith(prefix + ".", StringComparison.OrdinalIgnoreCase) ||
                dtoPath.StartsWith(prefix + "[", StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        return !IgnoredDtoPathSuffixes.Any(suffix => dtoPath.EndsWith(suffix, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Builds conservative comparison row path candidates for a flattened DTO assertion path.
    /// </summary>
    /// <param name="dtoPath">The flattened DTO assertion path.</param>
    /// <returns>Possible comparison row paths that preserve DTO path order and indexed collection positions.</returns>
    private static IReadOnlyList<IReadOnlyList<string>> GetCandidateRowPaths(string dtoPath)
    {
        var segments = dtoPath.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (segments.Length == 0)
        {
            return [];
        }

        var candidates = new List<IReadOnlyList<string>>
        {
            new List<string> { dtoPath }
        };
        if (segments.Length > 1)
        {
            candidates.Add(segments);
        }

        var expanded = new List<string>();
        foreach (var segment in segments)
        {
            AddExpandedSegment(expanded, segment);
        }

        candidates.Add(expanded);
        return candidates
            .Where(candidate => candidate.Count > 0)
            .Distinct(new StringListComparer())
            .ToList();
    }

    /// <summary>
    /// Adds comparison-row path segments for one flattened DTO path segment.
    /// </summary>
    /// <param name="segments">The expanded comparison path being built.</param>
    /// <param name="dtoSegment">The DTO path segment to expand.</param>
    private static void AddExpandedSegment(ICollection<string> segments, string dtoSegment)
    {
        var bracketIndex = dtoSegment.IndexOf('[', StringComparison.Ordinal);
        if (bracketIndex < 0 || !dtoSegment.EndsWith("]", StringComparison.Ordinal))
        {
            segments.Add(dtoSegment);
            return;
        }

        var collectionName = dtoSegment[..bracketIndex];
        var indexText = dtoSegment[(bracketIndex + 1)..^1];
        if (!int.TryParse(indexText, NumberStyles.Integer, CultureInfo.InvariantCulture, out var index))
        {
            segments.Add(dtoSegment);
            return;
        }

        segments.Add(collectionName);
        segments.Add(GetSingularRowName(collectionName) + " [" + index.ToString(CultureInfo.InvariantCulture) + "]");
    }

    /// <summary>
    /// Gets a comparison row label for one indexed collection item.
    /// </summary>
    /// <param name="collectionName">The DTO collection name.</param>
    /// <returns>The singular row label used by comparison output when known.</returns>
    private static string GetSingularRowName(string collectionName)
    {
        if (SingularRowNames.TryGetValue(collectionName, out var rowName))
        {
            return rowName;
        }

        return collectionName.EndsWith("ies", StringComparison.Ordinal)
            ? collectionName[..^3] + "y"
            : collectionName.TrimEnd('s');
    }

    /// <summary>
    /// Creates one audit diagnostic.
    /// </summary>
    /// <param name="spec">The validation spec that produced the diagnostic.</param>
    /// <param name="category">The audit category.</param>
    /// <param name="dtoPath">The DTO assertion path related to the diagnostic, when available.</param>
    /// <param name="isBlocking">Whether the diagnostic should fail the audit test.</param>
    /// <param name="message">The diagnostic message.</param>
    /// <returns>A coverage diagnostic with sample identity attached.</returns>
    private static SpriggitComparisonUiCoverageDiagnostic CreateDiagnostic(
        ValidationSpec spec,
        string category,
        string? dtoPath,
        bool isBlocking,
        string message)
    {
        return new SpriggitComparisonUiCoverageDiagnostic
        {
            Game = spec.Game.ToString(),
            RecordType = spec.RecordType.RecordID,
            SampleName = spec.SampleName,
            Category = category,
            DtoPath = dtoPath,
            IsBlocking = isBlocking,
            Message = message
        };
    }

    /// <summary>
    /// Compares string-list path candidates using ordinal-ignore-case segment comparison.
    /// </summary>
    private sealed class StringListComparer : IEqualityComparer<IReadOnlyList<string>>
    {
        /// <inheritdoc />
        public bool Equals(IReadOnlyList<string>? x, IReadOnlyList<string>? y)
        {
            if (x is null || y is null || x.Count != y.Count)
            {
                return false;
            }

            return x.Zip(y).All(pair => string.Equals(pair.First, pair.Second, StringComparison.OrdinalIgnoreCase));
        }

        /// <inheritdoc />
        public int GetHashCode(IReadOnlyList<string> obj)
        {
            var hash = new HashCode();
            foreach (var segment in obj)
            {
                hash.Add(segment, StringComparer.OrdinalIgnoreCase);
            }

            return hash.ToHashCode();
        }
    }
}
