using System.Collections;
using System.Globalization;
using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using Mutagen.Bethesda.Plugins;

namespace CreationsForge.Core.Utilities;

public static class StaticNavmeshGeometryDTOMapper
{
    public static StaticNavmeshGeometryDTO? FromNavmeshGeometry(
        SupportedGame game,
        ModKeyDTO modKey,
        FormKey formKey,
        object? navmeshGeometry,
        DateTime importedAtUTC)
    {
        if (navmeshGeometry == null)
        {
            return null;
        }

        var dto = new StaticNavmeshGeometryDTO
        {
            GridMin = FormatNavmeshValue(GetPropertyValue(navmeshGeometry, "GridMin")),
            GridMax = FormatNavmeshValue(GetPropertyValue(navmeshGeometry, "GridMax")),
            GridMaxDistance = FormatNavmeshValue(GetPropertyValue(navmeshGeometry, "GridMaxDistance")),
            GridSize = FormatNavmeshValue(GetPropertyValue(navmeshGeometry, "GridSize")),
            Parent = CreateParent(GetPropertyValue(navmeshGeometry, "Parent"))
        };

        dto.Cover = GetCover(GetPropertyValue(navmeshGeometry, "Cover")).ToList();
        dto.CoverTriangleMappings = GetCoverTriangleMappings(GetPropertyValue(navmeshGeometry, "CoverTriangleMappings")).ToList();
        dto.GridArrays = GetGridArrays(GetPropertyValue(navmeshGeometry, "GridArrays")).ToList();
        dto.Triangles = GetTriangles(GetPropertyValue(navmeshGeometry, "Triangles")).ToList();
        dto.Versioning = GetScalarList(GetPropertyValue(navmeshGeometry, "Versioning")).ToList();
        dto.Vertices = GetVertices(GetPropertyValue(navmeshGeometry, "Vertices")).ToList();
        return HasNavmeshData(dto) ? dto : null;
    }

    private static StaticNavmeshParentDTO? CreateParent(object? parent)
    {
        if (parent == null)
        {
            return null;
        }

        var dto = new StaticNavmeshParentDTO
        {
            MutagenObjectType = parent.GetType().Name,
            Parent = GetFormKeyFromObject(GetPropertyValue(parent, "Parent"))
        };
        return dto.MutagenObjectType != null || dto.Parent != null ? dto : null;
    }

    private static IEnumerable<StaticNavmeshCoverDTO> GetCover(object? cover)
    {
        return ToObjectList(cover)
            .Select((coverEntry, coverIndex) => new StaticNavmeshCoverDTO
            {
                CoverIndex = coverIndex,
                Data = FormatNavmeshValue(GetPropertyValue(coverEntry, "Data")),
                Vertex1 = FormatNavmeshValue(GetPropertyValue(coverEntry, "Vertex1")),
                Vertex2 = FormatNavmeshValue(GetPropertyValue(coverEntry, "Vertex2"))
            });
    }

    private static IEnumerable<StaticNavmeshCoverTriangleMappingDTO> GetCoverTriangleMappings(object? coverTriangleMappings)
    {
        return ToObjectList(coverTriangleMappings)
            .Select((mapping, mappingIndex) => new StaticNavmeshCoverTriangleMappingDTO
            {
                MappingIndex = mappingIndex,
                Cover = FormatNavmeshValue(GetPropertyValue(mapping, "Cover")),
                Triangle = FormatNavmeshValue(GetPropertyValue(mapping, "Triangle")),
                Value = FormatCoverTriangleMappingValue(mapping)
            });
    }

    private static IEnumerable<StaticNavmeshGridArrayDTO> GetGridArrays(object? gridArrays)
    {
        var arrays = ToObjectList(gridArrays);
        for (var gridArrayIndex = 0; gridArrayIndex < arrays.Count; gridArrayIndex++)
        {
            var gridArray = arrays[gridArrayIndex];
            var gridCells = GetScalarList(GetPropertyValue(gridArray, "GridCell")).ToList();
            if (gridCells.Count == 0)
            {
                gridCells = GetScalarList(gridArray).ToList();
            }

            yield return new StaticNavmeshGridArrayDTO
            {
                GridArrayIndex = gridArrayIndex,
                GridCell = gridCells
            };
        }
    }

    private static IEnumerable<StaticNavmeshTriangleDTO> GetTriangles(object? triangles)
    {
        return ToObjectList(triangles)
            .Select((triangle, triangleIndex) => new StaticNavmeshTriangleDTO
            {
                TriangleIndex = triangleIndex,
                EdgeLink_0_1 = FormatNavmeshValue(GetPropertyValue(triangle, "EdgeLink_0_1")),
                EdgeLink_1_2 = FormatNavmeshValue(GetPropertyValue(triangle, "EdgeLink_1_2")),
                EdgeLink_2_0 = FormatNavmeshValue(GetPropertyValue(triangle, "EdgeLink_2_0")),
                Height = FormatNavmeshValue(GetPropertyValue(triangle, "Height")),
                Vertices = FormatNavmeshValue(GetPropertyValue(triangle, "Vertices")),
                CoverFlags = FormatNavmeshValue(GetPropertyValue(triangle, "CoverFlags")),
                Flags = FormatNavmeshValue(GetPropertyValue(triangle, "Flags"))
            });
    }

    private static IEnumerable<StaticNavmeshVertexDTO> GetVertices(object? vertices)
    {
        return ToObjectList(vertices)
            .Select((vertex, vertexIndex) => new StaticNavmeshVertexDTO
            {
                VertexIndex = vertexIndex,
                Point = FormatNavmeshValue(GetPropertyValue(vertex, "Point")) ?? FormatNavmeshValue(vertex)
            });
    }

    private static IEnumerable<string> GetScalarList(object? value)
    {
        if (value is IEnumerable enumerable && value is not string)
        {
            foreach (var item in enumerable.Cast<object>())
            {
                if (FormatNavmeshValue(item) is { } formatted)
                {
                    yield return formatted;
                }
            }
        }
        else if (FormatNavmeshValue(value) is { } formatted)
        {
            yield return formatted;
        }
    }

    private static List<object> ToObjectList(object? value)
    {
        if (value == null)
        {
            return new List<object>();
        }

        if (value is IEnumerable enumerable && value is not string)
        {
            return enumerable.Cast<object>().ToList();
        }

        return new List<object> { value };
    }

    private static bool HasNavmeshData(StaticNavmeshGeometryDTO dto)
    {
        return dto.GridMin != null ||
            dto.GridMax != null ||
            dto.GridMaxDistance != null ||
            dto.GridSize != null ||
            dto.Parent != null ||
            dto.Cover.Count > 0 ||
            dto.CoverTriangleMappings.Count > 0 ||
            dto.GridArrays.Count > 0 ||
            dto.Triangles.Count > 0 ||
            dto.Versioning.Count > 0 ||
            dto.Vertices.Count > 0;
    }

    private static string? FormatNavmeshValue(object? value)
    {
        if (value == null)
        {
            return null;
        }

        if (TryFormatCoordinates(value, out var coordinates))
        {
            return coordinates;
        }

        if (value is IEnumerable enumerable && value is not string)
        {
            var items = enumerable
                .Cast<object>()
                .Select(FormatNavmeshValue)
                .Where(item => !string.IsNullOrWhiteSpace(item))
                .ToList();
            return items.Count == 0 ? null : string.Join(", ", items);
        }

        return SpriggitValueFormatter.Format(value);
    }

    private static string? FormatCoverTriangleMappingValue(object mapping)
    {
        var cover = FormatNavmeshValue(GetPropertyValue(mapping, "Cover"));
        var triangle = FormatNavmeshValue(GetPropertyValue(mapping, "Triangle"));
        if (!string.IsNullOrWhiteSpace(cover) && !string.IsNullOrWhiteSpace(triangle))
        {
            return cover + ", " + triangle;
        }

        return HasProperty(mapping, "Cover") || HasProperty(mapping, "Triangle")
            ? null
            : FormatNavmeshValue(mapping);
    }

    private static bool TryFormatCoordinates(object value, out string coordinates)
    {
        var x = GetPropertyValue(value, "X");
        var y = GetPropertyValue(value, "Y");
        var z = GetPropertyValue(value, "Z");
        if (x != null && y != null && z != null)
        {
            coordinates = string.Join(", ", FormatCoordinate(x), FormatCoordinate(y), FormatCoordinate(z));
            return true;
        }

        if (x != null && y != null)
        {
            coordinates = string.Join(", ", FormatCoordinate(x), FormatCoordinate(y));
            return true;
        }

        coordinates = string.Empty;
        return false;
    }

    private static string FormatCoordinate(object value)
    {
        return value is IConvertible convertible
            ? Convert.ToString(convertible, CultureInfo.InvariantCulture) ?? string.Empty
            : value.ToString() ?? string.Empty;
    }

    private static FormKeyDTO MapFormKey(FormKey formKey)
    {
        return new FormKeyDTO
        {
            ModKey = ModKeyDTOMapper.FromModKey(formKey.ModKey),
            Id = formKey.ID
        };
    }

    private static FormKeyDTO? GetFormKeyFromObject(object? value)
    {
        if (value == null)
        {
            return null;
        }

        if (value is FormKey formKey)
        {
            return MapFormKey(formKey);
        }

        if (GetPropertyValue(value, "FormKey") is FormKey linkedFormKey)
        {
            return MapFormKey(linkedFormKey);
        }

        return GetPropertyValue(value, "FormKeyNullable") is FormKey nullableFormKey ? MapFormKey(nullableFormKey) : null;
    }

    private static object? GetPropertyValue(object? source, string propertyName)
    {
        return source?.GetType().GetProperty(propertyName)?.GetValue(source);
    }

    private static bool HasProperty(object? source, string propertyName)
    {
        return source?.GetType().GetProperty(propertyName) != null;
    }
}
