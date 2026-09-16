namespace CreationsForge.RecordFieldGenerator;

/// <content>Surveys native major-record roots against the existing typed visitor shape policies.</content>
internal sealed partial class CodecEmitter
{
    /// <summary>Reports whether concrete native major-record roots and reachable indexed fields fit the existing Starfield visitor policies.</summary>
    /// <param name="majorRecordType">The game's abstract native major-record base class.</param>
    /// <param name="validateModels">Whether to also validate installed getter and construction shapes for reachable types.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="majorRecordType"/> is <see langword="null"/>.</exception>
    /// <remarks>This survey uses package metadata at generation time and does not create or modify repository artifacts.</remarks>
    internal void ScanMajorRecords(Type majorRecordType, bool validateModels)
    {
        ArgumentNullException.ThrowIfNull(majorRecordType);
        var roots = ConcreteTypes
            .Where(majorRecordType.IsAssignableFrom)
            .OrderBy(static type => type.FullName, StringComparer.Ordinal)
            .ToArray();
        var covered = new List<Type>();
        var reachable = new HashSet<Type>();
        var gaps = new List<(string Path, string Reason)>();
        foreach (var root in roots)
        {
            var rootGapCount = gaps.Count;
            var visited = new HashSet<Type>();
            var fieldIndex = GetFieldIndex(root);
            if (fieldIndex is null)
            {
                gaps.Add((root.FullName ?? root.Name, "The major-record root has no installed FieldIndex."));
                continue;
            }

            foreach (var field in GetMutableFields(root, fieldIndex))
            {
                if (field.PropertyType.FullName == "Mutagen.Bethesda.Plugins.FormKey")
                {
                    continue;
                }

                try
                {
                    var fieldVisited = new HashSet<Type>();
                    var polymorphic = new HashSet<Type>();
                    Visit(field.PropertyType, fieldVisited, polymorphic);
                    visited.UnionWith(fieldVisited);
                }
                catch (Exception exception)
                {
                    gaps.Add(((root.FullName ?? root.Name) + "." + field.Name, exception.Message));
                }
            }

            if (validateModels)
            {
                foreach (var type in visited)
                {
                    try
                    {
                        var model = CreateModel(type);
                        foreach (var field in model.Fields)
                        {
                            try
                            {
                                ValidateValueShape(
                                    field.MutableProperty.PropertyType,
                                    field.GetterProperty.PropertyType,
                                    type.FullName + "." + field.Name);
                            }
                            catch (Exception exception)
                            {
                                gaps.Add(((type.FullName ?? type.Name) + "." + field.Name, exception.Message));
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        gaps.Add((type.FullName ?? type.Name, exception.Message));
                    }
                }
            }

            reachable.UnionWith(visited);
            if (gaps.Count == rootGapCount)
            {
                covered.Add(root);
            }
        }

        var groupedGaps = gaps
            .GroupBy(static gap => gap.Reason, StringComparer.Ordinal)
            .OrderByDescending(static group => group.Count())
            .ThenBy(static group => group.Key, StringComparer.Ordinal)
            .ToArray();
        Console.WriteLine($"{majorRecordType.Assembly.GetName().Name} major roots: {roots.Length}; survey-covered: {covered.Count}; reachable indexed types: {reachable.Count}; field gaps: {gaps.Count}; gap categories: {groupedGaps.Length}; getter-shapes-validated: {validateModels}.");
        foreach (var group in groupedGaps.Take(80))
        {
            Console.WriteLine($"{group.Count()} × {group.Key}; example: {group.First().Path}");
        }
    }
}
