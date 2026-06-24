using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Repositories.Interfaces;
using NPoco;

namespace CreationsForge.Core.Repositories;

public class StaticRepository : TypedRecordRepositoryBase, IStaticRepository
{
    private readonly IModelRepository ModelRepository;
    private readonly IKeywordMappingRepository KeywordMappingRepository;
    private readonly IRawRecordPayloadRepository RawRecordPayloadRepository;
    private readonly IRecordLocalizedStringRepository RecordLocalizedStringRepository;

    public StaticRepository(
        IDatabase database,
        IRecordInstanceRepository recordInstanceRepository,
        IModelRepository modelRepository,
        IKeywordMappingRepository keywordMappingRepository,
        IRawRecordPayloadRepository rawRecordPayloadRepository,
        IRecordLocalizedStringRepository recordLocalizedStringRepository)
        : base(database, recordInstanceRepository)
    {
        ModelRepository = modelRepository;
        KeywordMappingRepository = keywordMappingRepository;
        RawRecordPayloadRepository = rawRecordPayloadRepository;
        RecordLocalizedStringRepository = recordLocalizedStringRepository;
    }

    public override string RecordType => RecordTypeCatalog.Static.RecordID;

    protected override string TableName => RecordTypeCatalog.Static.TableName;

    public IReadOnlyList<StaticDTO> GetByFormKey(SupportedGame game, FormKeyDTO formKey)
    {
        var records = FetchByFormKey<StaticRow>(
                game,
                formKey,
                [
                    SelectColumn("Name"),
                    SelectColumn("Version2"),
                    SelectColumn("ObjectBounds_First", "ObjectBoundsFirst"),
                    SelectColumn("ObjectBounds_Second", "ObjectBoundsSecond"),
                    SelectColumn("MaxAngle"),
                    SelectColumn("UnknownDNAMFloat"),
                    SelectColumn("LeafAmplitude"),
                    SelectColumn("LeafFrequency"),
                    SelectColumn("Unused"),
                    SelectColumn("DNAMDataTypeState"),
                    SelectColumn("DirtinessScale"),
                    SelectColumn("SnapTemplate_ModKey_Name", "SnapTemplateModKeyName"),
                    SelectColumn("SnapTemplate_ModKey_Type", "SnapTemplateModKeyType"),
                    SelectColumn("SnapTemplate_ModKey_FileName", "SnapTemplateModKeyFileName"),
                    SelectColumn("SnapTemplate_FormKey_ID", "SnapTemplateFormKeyId"),
                    SelectColumn("PreviewTransform_ModKey_Name", "PreviewTransformModKeyName"),
                    SelectColumn("PreviewTransform_ModKey_Type", "PreviewTransformModKeyType"),
                    SelectColumn("PreviewTransform_ModKey_FileName", "PreviewTransformModKeyFileName"),
                    SelectColumn("PreviewTransform_FormKey_ID", "PreviewTransformFormKeyId"),
                    SelectColumn("Material_ModKey_Name", "MaterialModKeyName"),
                    SelectColumn("Material_ModKey_Type", "MaterialModKeyType"),
                    SelectColumn("Material_ModKey_FileName", "MaterialModKeyFileName"),
                    SelectColumn("Material_FormKey_ID", "MaterialFormKeyId"),
                    SelectColumn("Lod_Level0", "LodLevel0"),
                    SelectColumn("Lod_Level1", "LodLevel1"),
                    SelectColumn("Lod_Level2", "LodLevel2"),
                    SelectColumn("Lod_Level3", "LodLevel3")
                ])
            .Select(record => ToDTO(record, game))
            .ToList();
        var models = ModelRepository.GetByFormKey(game, RecordTypeCatalog.Static.RecordID, formKey);
        var keywords = KeywordMappingRepository.GetByFormKey(game, RecordTypeCatalog.Static.RecordID, formKey);
        var rawPayloads = RawRecordPayloadRepository.GetByFormKey(game, RecordTypeCatalog.Static.RecordID, formKey);
        var localizedStrings = RecordLocalizedStringRepository.GetByFormKey(game, RecordTypeCatalog.Static.RecordID, formKey);
        var properties = FetchPropertiesByFormKey(game, formKey);
        var navmeshGeometries = FetchNavmeshGeometriesByFormKey(game, formKey);
        foreach (var record in records)
        {
            ApplyLocalizedStrings(record, localizedStrings.Where(localizedString => IsSameModKey(localizedString.ModKey, record.ModKey)).ToList());
            record.Models = models.Where(model => IsSameModKey(model.ModKey, record.ModKey)).OrderBy(model => model.ModelSlot).ThenBy(model => model.ModelGender).ToList();
            record.Keywords = keywords.Where(keyword => IsSameModKey(keyword.ModKey, record.ModKey)).OrderBy(keyword => keyword.KeywordIndex).ToList();
            record.Properties = properties.Where(property => IsSameModKey(property.ModKey, record.ModKey)).OrderBy(property => property.PropertyIndex).ToList();
            record.RawPayloads = rawPayloads.Where(payload => IsSameModKey(payload.ModKey, record.ModKey)).OrderBy(payload => payload.PayloadSlot).ThenBy(payload => payload.PayloadIndex).ToList();
            record.NavmeshGeometry = navmeshGeometries.FirstOrDefault(navmesh => IsSameModKey(navmesh.ModKey, record.ModKey))?.Geometry;
        }

        return records;
    }

    public void Save(StaticDTO dto)
    {
        SaveRecordInstance(dto);
        Database.Execute(
            """
            INSERT OR REPLACE INTO Statics (
                Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                EditorID, FormVersion, MajorRecordFlags, ImportedAtUTC, Name, Version2, ObjectBounds_First, ObjectBounds_Second, MaxAngle,
                UnknownDNAMFloat, LeafAmplitude, LeafFrequency, Unused, DNAMDataTypeState, DirtinessScale,
                SnapTemplate_ModKey_Name, SnapTemplate_ModKey_Type, SnapTemplate_ModKey_FileName, SnapTemplate_FormKey_ID,
                PreviewTransform_ModKey_Name, PreviewTransform_ModKey_Type, PreviewTransform_ModKey_FileName, PreviewTransform_FormKey_ID,
                Material_ModKey_Name, Material_ModKey_Type, Material_ModKey_FileName, Material_FormKey_ID,
                Lod_Level0, Lod_Level1, Lod_Level2, Lod_Level3)
            VALUES (
                @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                @EditorId, @FormVersion, @MajorRecordFlags, @ImportedAtUTC, @Name, @Version2, @ObjectBoundsFirst, @ObjectBoundsSecond, @MaxAngle,
                @UnknownDNAMFloat, @LeafAmplitude, @LeafFrequency, @Unused, @DNAMDataTypeState, @DirtinessScale,
                @SnapTemplateModKeyName, @SnapTemplateModKeyType, @SnapTemplateModKeyFileName, @SnapTemplateFormKeyId,
                @PreviewTransformModKeyName, @PreviewTransformModKeyType, @PreviewTransformModKeyFileName, @PreviewTransformFormKeyId,
                @MaterialModKeyName, @MaterialModKeyType, @MaterialModKeyFileName, @MaterialFormKeyId,
                @LodLevel0, @LodLevel1, @LodLevel2, @LodLevel3);
            """,
            new
            {
                Game = dto.Game.ToString(),
                ModKeyName = dto.ModKey.Name,
                ModKeyType = dto.ModKey.Type,
                ModKeyFileName = dto.ModKey.FileName,
                FormKeyModKeyName = dto.FormKey.ModKey.Name,
                FormKeyModKeyType = dto.FormKey.ModKey.Type,
                FormKeyModKeyFileName = dto.FormKey.ModKey.FileName,
                FormKeyId = dto.FormKey.Id,
                EditorId = dto.EditorID,
                dto.FormVersion,
                dto.MajorRecordFlags,
                dto.ImportedAtUTC,
                Name = GetEnglishText(dto.Name),
                dto.Version2,
                dto.ObjectBoundsFirst,
                dto.ObjectBoundsSecond,
                dto.MaxAngle,
                dto.UnknownDNAMFloat,
                dto.LeafAmplitude,
                dto.LeafFrequency,
                dto.Unused,
                dto.DNAMDataTypeState,
                dto.DirtinessScale,
                SnapTemplateModKeyName = dto.SnapTemplate?.ModKey.Name,
                SnapTemplateModKeyType = dto.SnapTemplate?.ModKey.Type,
                SnapTemplateModKeyFileName = dto.SnapTemplate?.ModKey.FileName,
                SnapTemplateFormKeyId = dto.SnapTemplate?.Id,
                PreviewTransformModKeyName = dto.PreviewTransform?.ModKey.Name,
                PreviewTransformModKeyType = dto.PreviewTransform?.ModKey.Type,
                PreviewTransformModKeyFileName = dto.PreviewTransform?.ModKey.FileName,
                PreviewTransformFormKeyId = dto.PreviewTransform?.Id,
                MaterialModKeyName = dto.Material?.ModKey.Name,
                MaterialModKeyType = dto.Material?.ModKey.Type,
                MaterialModKeyFileName = dto.Material?.ModKey.FileName,
                MaterialFormKeyId = dto.Material?.Id,
                dto.LodLevel0,
                dto.LodLevel1,
                dto.LodLevel2,
                dto.LodLevel3
            });
        DeleteProperties(dto);
        SaveProperties(dto);
        DeleteNavmeshGeometry(dto);
        SaveNavmeshGeometry(dto);
    }

    private static StaticDTO ToDTO(StaticRow record, SupportedGame game)
    {
        var dto = new StaticDTO
        {
            Game = game,
            ModKey = new ModKeyDTO { Name = string.Empty, Type = 0, FileName = string.Empty },
            FormKey = new FormKeyDTO { ModKey = new ModKeyDTO { Name = string.Empty, Type = 0, FileName = string.Empty }, Id = 0 },
            EditorID = string.Empty,
            FormVersion = 0,
            MajorRecordFlags = 0,
            ImportedAtUTC = record.ImportedAtUTC,
            Name = FromEnglish(record.Name),
            Version2 = record.Version2,
            ObjectBoundsFirst = record.ObjectBoundsFirst,
            ObjectBoundsSecond = record.ObjectBoundsSecond,
            MaxAngle = record.MaxAngle,
            UnknownDNAMFloat = record.UnknownDNAMFloat,
            LeafAmplitude = record.LeafAmplitude,
            LeafFrequency = record.LeafFrequency,
            Unused = record.Unused,
            DNAMDataTypeState = record.DNAMDataTypeState,
            DirtinessScale = record.DirtinessScale,
            SnapTemplate = CreateNullableFormKey(record.SnapTemplateModKeyName, record.SnapTemplateModKeyType, record.SnapTemplateModKeyFileName, record.SnapTemplateFormKeyId),
            PreviewTransform = CreateNullableFormKey(record.PreviewTransformModKeyName, record.PreviewTransformModKeyType, record.PreviewTransformModKeyFileName, record.PreviewTransformFormKeyId),
            Material = CreateNullableFormKey(record.MaterialModKeyName, record.MaterialModKeyType, record.MaterialModKeyFileName, record.MaterialFormKeyId),
            LodLevel0 = record.LodLevel0,
            LodLevel1 = record.LodLevel1,
            LodLevel2 = record.LodLevel2,
            LodLevel3 = record.LodLevel3
        };
        ApplyCommonFields(dto, record, game);
        return dto;
    }

    private static void ApplyLocalizedStrings(StaticDTO record, IReadOnlyList<LocalizedStringDTO> localizedStrings)
    {
        record.LocalizedStrings = localizedStrings.ToList();
        record.Name = BuildTranslatedString(localizedStrings, nameof(StaticDTO.Name), record.Name);
    }

    private static bool IsSameModKey(ModKeyDTO first, ModKeyDTO second)
    {
        return first.Type == second.Type &&
            string.Equals(first.Name, second.Name, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(first.FileName, second.FileName, StringComparison.OrdinalIgnoreCase);
    }

    private IReadOnlyList<StaticPropertyDTO> FetchPropertiesByFormKey(SupportedGame game, FormKeyDTO formKey)
    {
        return Database.Fetch<StaticPropertyRow>(
            """
            SELECT
                Game,
                ModKey_Name AS ModKeyName,
                ModKey_Type AS ModKeyType,
                ModKey_FileName AS ModKeyFileName,
                FormKey_ModKey_Name AS FormKeyModKeyName,
                FormKey_ModKey_Type AS FormKeyModKeyType,
                FormKey_ModKey_FileName AS FormKeyModKeyFileName,
                FormKey_ID AS FormKeyId,
                Property_Index AS PropertyIndex,
                ActorValue_ModKey_Name AS ActorValueModKeyName,
                ActorValue_ModKey_Type AS ActorValueModKeyType,
                ActorValue_ModKey_FileName AS ActorValueModKeyFileName,
                ActorValue_FormKey_ID AS ActorValueFormKeyId,
                Value,
                ImportedAtUTC
            FROM StaticProperties
            WHERE Game = @Game
              AND FormKey_ModKey_Name = @FormKeyModKeyName
              AND FormKey_ModKey_Type = @FormKeyModKeyType
              AND FormKey_ModKey_FileName = @FormKeyModKeyFileName
              AND FormKey_ID = @FormKeyId
            ORDER BY ModKey_FileName COLLATE NOCASE, Property_Index;
            """,
            new
            {
                Game = game.ToString(),
                FormKeyModKeyName = formKey.ModKey.Name,
                FormKeyModKeyType = formKey.ModKey.Type,
                FormKeyModKeyFileName = formKey.ModKey.FileName,
                FormKeyId = formKey.Id
            })
            .Select(ToDTO)
            .ToList();
    }

    private void DeleteProperties(StaticDTO dto)
    {
        Database.Execute(
            """
            DELETE FROM StaticProperties
            WHERE Game = @Game
              AND ModKey_Name = @ModKeyName
              AND ModKey_Type = @ModKeyType
              AND ModKey_FileName = @ModKeyFileName
              AND FormKey_ModKey_Name = @FormKeyModKeyName
              AND FormKey_ModKey_Type = @FormKeyModKeyType
              AND FormKey_ModKey_FileName = @FormKeyModKeyFileName
              AND FormKey_ID = @FormKeyId;
            """,
            new
            {
                Game = dto.Game.ToString(),
                ModKeyName = dto.ModKey.Name,
                ModKeyType = dto.ModKey.Type,
                ModKeyFileName = dto.ModKey.FileName,
                FormKeyModKeyName = dto.FormKey.ModKey.Name,
                FormKeyModKeyType = dto.FormKey.ModKey.Type,
                FormKeyModKeyFileName = dto.FormKey.ModKey.FileName,
                FormKeyId = dto.FormKey.Id
            });
    }

    private void SaveProperties(StaticDTO dto)
    {
        foreach (var property in dto.Properties)
        {
            Database.Execute(
                """
                INSERT OR REPLACE INTO StaticProperties (
                    Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                    Property_Index, ActorValue_ModKey_Name, ActorValue_ModKey_Type, ActorValue_ModKey_FileName, ActorValue_FormKey_ID, Value, ImportedAtUTC)
                VALUES (
                    @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                    @PropertyIndex, @ActorValueModKeyName, @ActorValueModKeyType, @ActorValueModKeyFileName, @ActorValueFormKeyId, @Value, @ImportedAtUTC);
                """,
                new
                {
                    Game = dto.Game.ToString(),
                    ModKeyName = dto.ModKey.Name,
                    ModKeyType = dto.ModKey.Type,
                    ModKeyFileName = dto.ModKey.FileName,
                    FormKeyModKeyName = dto.FormKey.ModKey.Name,
                    FormKeyModKeyType = dto.FormKey.ModKey.Type,
                    FormKeyModKeyFileName = dto.FormKey.ModKey.FileName,
                    FormKeyId = dto.FormKey.Id,
                    property.PropertyIndex,
                    ActorValueModKeyName = property.ActorValue?.ModKey.Name,
                    ActorValueModKeyType = property.ActorValue?.ModKey.Type,
                    ActorValueModKeyFileName = property.ActorValue?.ModKey.FileName,
                    ActorValueFormKeyId = property.ActorValue?.Id,
                    property.Value,
                    property.ImportedAtUTC
                });
        }
    }

    private IReadOnlyList<StaticNavmeshGeometryRow> FetchNavmeshGeometriesByFormKey(SupportedGame game, FormKeyDTO formKey)
    {
        var roots = Database.Fetch<StaticNavmeshGeometryRootRow>(
            """
            SELECT
                Game,
                ModKey_Name AS ModKeyName,
                ModKey_Type AS ModKeyType,
                ModKey_FileName AS ModKeyFileName,
                FormKey_ModKey_Name AS FormKeyModKeyName,
                FormKey_ModKey_Type AS FormKeyModKeyType,
                FormKey_ModKey_FileName AS FormKeyModKeyFileName,
                FormKey_ID AS FormKeyId,
                GridMin,
                GridMax,
                GridMaxDistance,
                GridSize,
                Parent_MutagenObjectType AS ParentMutagenObjectType,
                Parent_ModKey_Name AS ParentModKeyName,
                Parent_ModKey_Type AS ParentModKeyType,
                Parent_ModKey_FileName AS ParentModKeyFileName,
                Parent_FormKey_ID AS ParentFormKeyId,
                ImportedAtUTC
            FROM StaticNavmeshGeometries
            WHERE Game = @Game
              AND FormKey_ModKey_Name = @FormKeyModKeyName
              AND FormKey_ModKey_Type = @FormKeyModKeyType
              AND FormKey_ModKey_FileName = @FormKeyModKeyFileName
              AND FormKey_ID = @FormKeyId
            ORDER BY ModKey_FileName COLLATE NOCASE;
            """,
            new
            {
                Game = game.ToString(),
                FormKeyModKeyName = formKey.ModKey.Name,
                FormKeyModKeyType = formKey.ModKey.Type,
                FormKeyModKeyFileName = formKey.ModKey.FileName,
                FormKeyId = formKey.Id
            });
        if (roots.Count == 0)
        {
            return new List<StaticNavmeshGeometryRow>();
        }

        var cover = FetchNavmeshCoverByFormKey(game, formKey);
        var coverTriangleMappings = FetchNavmeshCoverTriangleMappingsByFormKey(game, formKey);
        var gridCells = FetchNavmeshGridCellsByFormKey(game, formKey);
        var triangles = FetchNavmeshTrianglesByFormKey(game, formKey);
        var versioning = FetchNavmeshVersioningByFormKey(game, formKey);
        var vertices = FetchNavmeshVerticesByFormKey(game, formKey);

        return roots.Select(root => new StaticNavmeshGeometryRow
        {
            ModKey = new ModKeyDTO
            {
                Name = root.ModKeyName,
                Type = root.ModKeyType,
                FileName = root.ModKeyFileName
            },
            Geometry = ToNavmeshGeometryDTO(root, cover, coverTriangleMappings, gridCells, triangles, versioning, vertices)
        }).ToList();
    }

    private IReadOnlyList<StaticNavmeshCoverRow> FetchNavmeshCoverByFormKey(SupportedGame game, FormKeyDTO formKey)
    {
        return Database.Fetch<StaticNavmeshCoverRow>(
            """
            SELECT
                ModKey_Name AS ModKeyName,
                ModKey_Type AS ModKeyType,
                ModKey_FileName AS ModKeyFileName,
                Cover_Index AS CoverIndex,
                Data,
                Vertex1,
                Vertex2
            FROM StaticNavmeshCover
            WHERE Game = @Game
              AND FormKey_ModKey_Name = @FormKeyModKeyName
              AND FormKey_ModKey_Type = @FormKeyModKeyType
              AND FormKey_ModKey_FileName = @FormKeyModKeyFileName
              AND FormKey_ID = @FormKeyId
            ORDER BY ModKey_FileName COLLATE NOCASE, Cover_Index;
            """,
            NavmeshFormKeyParameters(game, formKey));
    }

    private IReadOnlyList<StaticNavmeshCoverTriangleMappingRow> FetchNavmeshCoverTriangleMappingsByFormKey(SupportedGame game, FormKeyDTO formKey)
    {
        return Database.Fetch<StaticNavmeshCoverTriangleMappingRow>(
            """
            SELECT
                ModKey_Name AS ModKeyName,
                ModKey_Type AS ModKeyType,
                ModKey_FileName AS ModKeyFileName,
                Mapping_Index AS MappingIndex,
                Cover,
                Triangle,
                Value
            FROM StaticNavmeshCoverTriangleMappings
            WHERE Game = @Game
              AND FormKey_ModKey_Name = @FormKeyModKeyName
              AND FormKey_ModKey_Type = @FormKeyModKeyType
              AND FormKey_ModKey_FileName = @FormKeyModKeyFileName
              AND FormKey_ID = @FormKeyId
            ORDER BY ModKey_FileName COLLATE NOCASE, Mapping_Index;
            """,
            NavmeshFormKeyParameters(game, formKey));
    }

    private IReadOnlyList<StaticNavmeshGridCellRow> FetchNavmeshGridCellsByFormKey(SupportedGame game, FormKeyDTO formKey)
    {
        return Database.Fetch<StaticNavmeshGridCellRow>(
            """
            SELECT
                ModKey_Name AS ModKeyName,
                ModKey_Type AS ModKeyType,
                ModKey_FileName AS ModKeyFileName,
                GridArray_Index AS GridArrayIndex,
                GridCell_Index AS GridCellIndex,
                Value
            FROM StaticNavmeshGridCells
            WHERE Game = @Game
              AND FormKey_ModKey_Name = @FormKeyModKeyName
              AND FormKey_ModKey_Type = @FormKeyModKeyType
              AND FormKey_ModKey_FileName = @FormKeyModKeyFileName
              AND FormKey_ID = @FormKeyId
            ORDER BY ModKey_FileName COLLATE NOCASE, GridArray_Index, GridCell_Index;
            """,
            NavmeshFormKeyParameters(game, formKey));
    }

    private IReadOnlyList<StaticNavmeshTriangleRow> FetchNavmeshTrianglesByFormKey(SupportedGame game, FormKeyDTO formKey)
    {
        return Database.Fetch<StaticNavmeshTriangleRow>(
            """
            SELECT
                ModKey_Name AS ModKeyName,
                ModKey_Type AS ModKeyType,
                ModKey_FileName AS ModKeyFileName,
                Triangle_Index AS TriangleIndex,
                EdgeLink_0_1 AS EdgeLink01,
                EdgeLink_1_2 AS EdgeLink12,
                EdgeLink_2_0 AS EdgeLink20,
                Height,
                Vertices,
                CoverFlags,
                Flags
            FROM StaticNavmeshTriangles
            WHERE Game = @Game
              AND FormKey_ModKey_Name = @FormKeyModKeyName
              AND FormKey_ModKey_Type = @FormKeyModKeyType
              AND FormKey_ModKey_FileName = @FormKeyModKeyFileName
              AND FormKey_ID = @FormKeyId
            ORDER BY ModKey_FileName COLLATE NOCASE, Triangle_Index;
            """,
            NavmeshFormKeyParameters(game, formKey));
    }

    private IReadOnlyList<StaticNavmeshVersioningRow> FetchNavmeshVersioningByFormKey(SupportedGame game, FormKeyDTO formKey)
    {
        return Database.Fetch<StaticNavmeshVersioningRow>(
            """
            SELECT
                ModKey_Name AS ModKeyName,
                ModKey_Type AS ModKeyType,
                ModKey_FileName AS ModKeyFileName,
                Versioning_Index AS VersioningIndex,
                Value
            FROM StaticNavmeshVersioning
            WHERE Game = @Game
              AND FormKey_ModKey_Name = @FormKeyModKeyName
              AND FormKey_ModKey_Type = @FormKeyModKeyType
              AND FormKey_ModKey_FileName = @FormKeyModKeyFileName
              AND FormKey_ID = @FormKeyId
            ORDER BY ModKey_FileName COLLATE NOCASE, Versioning_Index;
            """,
            NavmeshFormKeyParameters(game, formKey));
    }

    private IReadOnlyList<StaticNavmeshVertexRow> FetchNavmeshVerticesByFormKey(SupportedGame game, FormKeyDTO formKey)
    {
        return Database.Fetch<StaticNavmeshVertexRow>(
            """
            SELECT
                ModKey_Name AS ModKeyName,
                ModKey_Type AS ModKeyType,
                ModKey_FileName AS ModKeyFileName,
                Vertex_Index AS VertexIndex,
                Point
            FROM StaticNavmeshVertices
            WHERE Game = @Game
              AND FormKey_ModKey_Name = @FormKeyModKeyName
              AND FormKey_ModKey_Type = @FormKeyModKeyType
              AND FormKey_ModKey_FileName = @FormKeyModKeyFileName
              AND FormKey_ID = @FormKeyId
            ORDER BY ModKey_FileName COLLATE NOCASE, Vertex_Index;
            """,
            NavmeshFormKeyParameters(game, formKey));
    }

    private static object NavmeshFormKeyParameters(SupportedGame game, FormKeyDTO formKey)
    {
        return new
        {
            Game = game.ToString(),
            FormKeyModKeyName = formKey.ModKey.Name,
            FormKeyModKeyType = formKey.ModKey.Type,
            FormKeyModKeyFileName = formKey.ModKey.FileName,
            FormKeyId = formKey.Id
        };
    }

    private static StaticNavmeshGeometryDTO ToNavmeshGeometryDTO(
        StaticNavmeshGeometryRootRow root,
        IReadOnlyList<StaticNavmeshCoverRow> cover,
        IReadOnlyList<StaticNavmeshCoverTriangleMappingRow> coverTriangleMappings,
        IReadOnlyList<StaticNavmeshGridCellRow> gridCells,
        IReadOnlyList<StaticNavmeshTriangleRow> triangles,
        IReadOnlyList<StaticNavmeshVersioningRow> versioning,
        IReadOnlyList<StaticNavmeshVertexRow> vertices)
    {
        return new StaticNavmeshGeometryDTO
        {
            GridMin = root.GridMin,
            GridMax = root.GridMax,
            GridMaxDistance = root.GridMaxDistance,
            GridSize = root.GridSize,
            Parent = root.ParentMutagenObjectType != null || root.ParentModKeyName != null
                ? new StaticNavmeshParentDTO
                {
                    MutagenObjectType = root.ParentMutagenObjectType,
                    Parent = CreateNullableFormKey(root.ParentModKeyName, root.ParentModKeyType, root.ParentModKeyFileName, root.ParentFormKeyId)
                }
                : null,
            Cover = cover
                .Where(coverEntry => RowModKeysMatch(root, coverEntry))
                .OrderBy(coverEntry => coverEntry.CoverIndex)
                .Select(coverEntry => new StaticNavmeshCoverDTO
                {
                    CoverIndex = coverEntry.CoverIndex,
                    Data = coverEntry.Data,
                    Vertex1 = coverEntry.Vertex1,
                    Vertex2 = coverEntry.Vertex2
                })
                .ToList(),
            CoverTriangleMappings = coverTriangleMappings
                .Where(mapping => RowModKeysMatch(root, mapping))
                .OrderBy(mapping => mapping.MappingIndex)
                .Select(mapping => new StaticNavmeshCoverTriangleMappingDTO
                {
                    MappingIndex = mapping.MappingIndex,
                    Cover = mapping.Cover,
                    Triangle = mapping.Triangle,
                    Value = mapping.Value
                })
                .ToList(),
            GridArrays = gridCells
                .Where(cell => RowModKeysMatch(root, cell))
                .GroupBy(cell => cell.GridArrayIndex)
                .OrderBy(group => group.Key)
                .Select(group => new StaticNavmeshGridArrayDTO
                {
                    GridArrayIndex = group.Key,
                    GridCell = group.OrderBy(cell => cell.GridCellIndex).Select(cell => cell.Value).ToList()
                })
                .ToList(),
            Triangles = triangles
                .Where(triangle => RowModKeysMatch(root, triangle))
                .OrderBy(triangle => triangle.TriangleIndex)
                .Select(triangle => new StaticNavmeshTriangleDTO
                {
                    TriangleIndex = triangle.TriangleIndex,
                    EdgeLink_0_1 = triangle.EdgeLink01,
                    EdgeLink_1_2 = triangle.EdgeLink12,
                    EdgeLink_2_0 = triangle.EdgeLink20,
                    Height = triangle.Height,
                    Vertices = triangle.Vertices,
                    CoverFlags = triangle.CoverFlags,
                    Flags = triangle.Flags
                })
                .ToList(),
            Versioning = versioning
                .Where(value => RowModKeysMatch(root, value))
                .OrderBy(value => value.VersioningIndex)
                .Select(value => value.Value)
                .ToList(),
            Vertices = vertices
                .Where(vertex => RowModKeysMatch(root, vertex))
                .OrderBy(vertex => vertex.VertexIndex)
                .Select(vertex => new StaticNavmeshVertexDTO
                {
                    VertexIndex = vertex.VertexIndex,
                    Point = vertex.Point
                })
                .ToList()
        };
    }

    private void DeleteNavmeshGeometry(StaticDTO dto)
    {
        Database.Execute(
            """
            DELETE FROM StaticNavmeshGeometries
            WHERE Game = @Game
              AND ModKey_Name = @ModKeyName
              AND ModKey_Type = @ModKeyType
              AND ModKey_FileName = @ModKeyFileName
              AND FormKey_ModKey_Name = @FormKeyModKeyName
              AND FormKey_ModKey_Type = @FormKeyModKeyType
              AND FormKey_ModKey_FileName = @FormKeyModKeyFileName
              AND FormKey_ID = @FormKeyId;
            """,
            new
            {
                Game = dto.Game.ToString(),
                ModKeyName = dto.ModKey.Name,
                ModKeyType = dto.ModKey.Type,
                ModKeyFileName = dto.ModKey.FileName,
                FormKeyModKeyName = dto.FormKey.ModKey.Name,
                FormKeyModKeyType = dto.FormKey.ModKey.Type,
                FormKeyModKeyFileName = dto.FormKey.ModKey.FileName,
                FormKeyId = dto.FormKey.Id
            });
    }

    private void SaveNavmeshGeometry(StaticDTO dto)
    {
        if (dto.NavmeshGeometry == null)
        {
            return;
        }

        SaveNavmeshGeometryRoot(dto);
        SaveNavmeshCover(dto);
        SaveNavmeshCoverTriangleMappings(dto);
        SaveNavmeshGridCells(dto);
        SaveNavmeshTriangles(dto);
        SaveNavmeshVersioning(dto);
        SaveNavmeshVertices(dto);
    }

    private void SaveNavmeshGeometryRoot(StaticDTO dto)
    {
        var geometry = dto.NavmeshGeometry!;
        Database.Execute(
            """
            INSERT OR REPLACE INTO StaticNavmeshGeometries (
                Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                GridMin, GridMax, GridMaxDistance, GridSize, Parent_MutagenObjectType, Parent_ModKey_Name, Parent_ModKey_Type,
                Parent_ModKey_FileName, Parent_FormKey_ID, ImportedAtUTC)
            VALUES (
                @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                @GridMin, @GridMax, @GridMaxDistance, @GridSize, @ParentMutagenObjectType, @ParentModKeyName, @ParentModKeyType,
                @ParentModKeyFileName, @ParentFormKeyId, @ImportedAtUTC);
            """,
            new
            {
                Game = dto.Game.ToString(),
                ModKeyName = dto.ModKey.Name,
                ModKeyType = dto.ModKey.Type,
                ModKeyFileName = dto.ModKey.FileName,
                FormKeyModKeyName = dto.FormKey.ModKey.Name,
                FormKeyModKeyType = dto.FormKey.ModKey.Type,
                FormKeyModKeyFileName = dto.FormKey.ModKey.FileName,
                FormKeyId = dto.FormKey.Id,
                geometry.GridMin,
                geometry.GridMax,
                geometry.GridMaxDistance,
                geometry.GridSize,
                ParentMutagenObjectType = geometry.Parent?.MutagenObjectType,
                ParentModKeyName = geometry.Parent?.Parent?.ModKey.Name,
                ParentModKeyType = geometry.Parent?.Parent?.ModKey.Type,
                ParentModKeyFileName = geometry.Parent?.Parent?.ModKey.FileName,
                ParentFormKeyId = geometry.Parent?.Parent?.Id,
                dto.ImportedAtUTC
            });
    }

    private void SaveNavmeshCover(StaticDTO dto)
    {
        foreach (var coverEntry in dto.NavmeshGeometry!.Cover)
        {
            Database.Execute(
                """
                INSERT OR REPLACE INTO StaticNavmeshCover (
                    Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                    Cover_Index, Data, Vertex1, Vertex2, ImportedAtUTC)
                VALUES (
                    @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                    @CoverIndex, @Data, @Vertex1, @Vertex2, @ImportedAtUTC);
                """,
                new
                {
                    Game = dto.Game.ToString(),
                    ModKeyName = dto.ModKey.Name,
                    ModKeyType = dto.ModKey.Type,
                    ModKeyFileName = dto.ModKey.FileName,
                    FormKeyModKeyName = dto.FormKey.ModKey.Name,
                    FormKeyModKeyType = dto.FormKey.ModKey.Type,
                    FormKeyModKeyFileName = dto.FormKey.ModKey.FileName,
                    FormKeyId = dto.FormKey.Id,
                    coverEntry.CoverIndex,
                    coverEntry.Data,
                    coverEntry.Vertex1,
                    coverEntry.Vertex2,
                    dto.ImportedAtUTC
                });
        }
    }

    private void SaveNavmeshCoverTriangleMappings(StaticDTO dto)
    {
        foreach (var mapping in dto.NavmeshGeometry!.CoverTriangleMappings)
        {
            Database.Execute(
                """
                INSERT OR REPLACE INTO StaticNavmeshCoverTriangleMappings (
                    Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                    Mapping_Index, Cover, Triangle, Value, ImportedAtUTC)
                VALUES (
                    @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                    @MappingIndex, @Cover, @Triangle, @Value, @ImportedAtUTC);
                """,
                new
                {
                    Game = dto.Game.ToString(),
                    ModKeyName = dto.ModKey.Name,
                    ModKeyType = dto.ModKey.Type,
                    ModKeyFileName = dto.ModKey.FileName,
                    FormKeyModKeyName = dto.FormKey.ModKey.Name,
                    FormKeyModKeyType = dto.FormKey.ModKey.Type,
                    FormKeyModKeyFileName = dto.FormKey.ModKey.FileName,
                    FormKeyId = dto.FormKey.Id,
                    mapping.MappingIndex,
                    mapping.Cover,
                    mapping.Triangle,
                    mapping.Value,
                    dto.ImportedAtUTC
                });
        }
    }

    private void SaveNavmeshGridCells(StaticDTO dto)
    {
        foreach (var gridArray in dto.NavmeshGeometry!.GridArrays)
        {
            for (var gridCellIndex = 0; gridCellIndex < gridArray.GridCell.Count; gridCellIndex++)
            {
                Database.Execute(
                    """
                    INSERT OR REPLACE INTO StaticNavmeshGridCells (
                        Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                        GridArray_Index, GridCell_Index, Value, ImportedAtUTC)
                    VALUES (
                        @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                        @GridArrayIndex, @GridCellIndex, @Value, @ImportedAtUTC);
                    """,
                    new
                    {
                        Game = dto.Game.ToString(),
                        ModKeyName = dto.ModKey.Name,
                        ModKeyType = dto.ModKey.Type,
                        ModKeyFileName = dto.ModKey.FileName,
                        FormKeyModKeyName = dto.FormKey.ModKey.Name,
                        FormKeyModKeyType = dto.FormKey.ModKey.Type,
                        FormKeyModKeyFileName = dto.FormKey.ModKey.FileName,
                        FormKeyId = dto.FormKey.Id,
                        gridArray.GridArrayIndex,
                        GridCellIndex = gridCellIndex,
                        Value = gridArray.GridCell[gridCellIndex],
                        dto.ImportedAtUTC
                    });
            }
        }
    }

    private void SaveNavmeshTriangles(StaticDTO dto)
    {
        foreach (var triangle in dto.NavmeshGeometry!.Triangles)
        {
            Database.Execute(
                """
                INSERT OR REPLACE INTO StaticNavmeshTriangles (
                    Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                    Triangle_Index, EdgeLink_0_1, EdgeLink_1_2, EdgeLink_2_0, Height, Vertices, CoverFlags, Flags, ImportedAtUTC)
                VALUES (
                    @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                    @TriangleIndex, @EdgeLink01, @EdgeLink12, @EdgeLink20, @Height, @Vertices, @CoverFlags, @Flags, @ImportedAtUTC);
                """,
                new
                {
                    Game = dto.Game.ToString(),
                    ModKeyName = dto.ModKey.Name,
                    ModKeyType = dto.ModKey.Type,
                    ModKeyFileName = dto.ModKey.FileName,
                    FormKeyModKeyName = dto.FormKey.ModKey.Name,
                    FormKeyModKeyType = dto.FormKey.ModKey.Type,
                    FormKeyModKeyFileName = dto.FormKey.ModKey.FileName,
                    FormKeyId = dto.FormKey.Id,
                    triangle.TriangleIndex,
                    EdgeLink01 = triangle.EdgeLink_0_1,
                    EdgeLink12 = triangle.EdgeLink_1_2,
                    EdgeLink20 = triangle.EdgeLink_2_0,
                    triangle.Height,
                    triangle.Vertices,
                    triangle.CoverFlags,
                    triangle.Flags,
                    dto.ImportedAtUTC
                });
        }
    }

    private void SaveNavmeshVersioning(StaticDTO dto)
    {
        for (var versioningIndex = 0; versioningIndex < dto.NavmeshGeometry!.Versioning.Count; versioningIndex++)
        {
            Database.Execute(
                """
                INSERT OR REPLACE INTO StaticNavmeshVersioning (
                    Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                    Versioning_Index, Value, ImportedAtUTC)
                VALUES (
                    @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                    @VersioningIndex, @Value, @ImportedAtUTC);
                """,
                new
                {
                    Game = dto.Game.ToString(),
                    ModKeyName = dto.ModKey.Name,
                    ModKeyType = dto.ModKey.Type,
                    ModKeyFileName = dto.ModKey.FileName,
                    FormKeyModKeyName = dto.FormKey.ModKey.Name,
                    FormKeyModKeyType = dto.FormKey.ModKey.Type,
                    FormKeyModKeyFileName = dto.FormKey.ModKey.FileName,
                    FormKeyId = dto.FormKey.Id,
                    VersioningIndex = versioningIndex,
                    Value = dto.NavmeshGeometry.Versioning[versioningIndex],
                    dto.ImportedAtUTC
                });
        }
    }

    private void SaveNavmeshVertices(StaticDTO dto)
    {
        foreach (var vertex in dto.NavmeshGeometry!.Vertices)
        {
            Database.Execute(
                """
                INSERT OR REPLACE INTO StaticNavmeshVertices (
                    Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                    Vertex_Index, Point, ImportedAtUTC)
                VALUES (
                    @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                    @VertexIndex, @Point, @ImportedAtUTC);
                """,
                new
                {
                    Game = dto.Game.ToString(),
                    ModKeyName = dto.ModKey.Name,
                    ModKeyType = dto.ModKey.Type,
                    ModKeyFileName = dto.ModKey.FileName,
                    FormKeyModKeyName = dto.FormKey.ModKey.Name,
                    FormKeyModKeyType = dto.FormKey.ModKey.Type,
                    FormKeyModKeyFileName = dto.FormKey.ModKey.FileName,
                    FormKeyId = dto.FormKey.Id,
                    vertex.VertexIndex,
                    vertex.Point,
                    dto.ImportedAtUTC
                });
        }
    }

    private static StaticPropertyDTO ToDTO(StaticPropertyRow row)
    {
        return new StaticPropertyDTO
        {
            Game = Enum.Parse<SupportedGame>(row.Game),
            ModKey = new ModKeyDTO
            {
                Name = row.ModKeyName,
                Type = row.ModKeyType,
                FileName = row.ModKeyFileName
            },
            FormKey = new FormKeyDTO
            {
                ModKey = new ModKeyDTO
                {
                    Name = row.FormKeyModKeyName,
                    Type = row.FormKeyModKeyType,
                    FileName = row.FormKeyModKeyFileName
                },
                Id = (uint)row.FormKeyId
            },
            PropertyIndex = row.PropertyIndex,
            ActorValue = CreateNullableFormKey(row.ActorValueModKeyName, row.ActorValueModKeyType, row.ActorValueModKeyFileName, row.ActorValueFormKeyId),
            Value = row.Value,
            ImportedAtUTC = row.ImportedAtUTC
        };
    }

    private sealed class StaticRow : RecordRow
    {
        public string? Name { get; set; }

        public int? Version2 { get; set; }

        public string? ObjectBoundsFirst { get; set; }

        public string? ObjectBoundsSecond { get; set; }

        public double? MaxAngle { get; set; }

        public double? UnknownDNAMFloat { get; set; }

        public double? LeafAmplitude { get; set; }

        public double? LeafFrequency { get; set; }

        public string? Unused { get; set; }

        public string? DNAMDataTypeState { get; set; }

        public double? DirtinessScale { get; set; }

        public string? SnapTemplateModKeyName { get; set; }

        public int? SnapTemplateModKeyType { get; set; }

        public string? SnapTemplateModKeyFileName { get; set; }

        public long? SnapTemplateFormKeyId { get; set; }

        public string? PreviewTransformModKeyName { get; set; }

        public int? PreviewTransformModKeyType { get; set; }

        public string? PreviewTransformModKeyFileName { get; set; }

        public long? PreviewTransformFormKeyId { get; set; }

        public string? MaterialModKeyName { get; set; }

        public int? MaterialModKeyType { get; set; }

        public string? MaterialModKeyFileName { get; set; }

        public long? MaterialFormKeyId { get; set; }

        public string? LodLevel0 { get; set; }

        public string? LodLevel1 { get; set; }

        public string? LodLevel2 { get; set; }

        public string? LodLevel3 { get; set; }

    }

    private sealed class StaticNavmeshGeometryRow
    {
        public ModKeyDTO ModKey { get; set; } = new() { Name = string.Empty, Type = 0, FileName = string.Empty };

        public StaticNavmeshGeometryDTO Geometry { get; set; } = new();
    }

    private interface IStaticNavmeshModKeyRow
    {
        string ModKeyName { get; }

        int ModKeyType { get; }

        string ModKeyFileName { get; }
    }

    private sealed class StaticNavmeshGeometryRootRow : IStaticNavmeshModKeyRow
    {
        public string ModKeyName { get; set; } = string.Empty;

        public int ModKeyType { get; set; }

        public string ModKeyFileName { get; set; } = string.Empty;

        public string? GridMin { get; set; }

        public string? GridMax { get; set; }

        public string? GridMaxDistance { get; set; }

        public string? GridSize { get; set; }

        public string? ParentMutagenObjectType { get; set; }

        public string? ParentModKeyName { get; set; }

        public int? ParentModKeyType { get; set; }

        public string? ParentModKeyFileName { get; set; }

        public long? ParentFormKeyId { get; set; }
    }

    private sealed class StaticNavmeshCoverRow : IStaticNavmeshModKeyRow
    {
        public string ModKeyName { get; set; } = string.Empty;

        public int ModKeyType { get; set; }

        public string ModKeyFileName { get; set; } = string.Empty;

        public int CoverIndex { get; set; }

        public string? Data { get; set; }

        public string? Vertex1 { get; set; }

        public string? Vertex2 { get; set; }
    }

    private sealed class StaticNavmeshCoverTriangleMappingRow : IStaticNavmeshModKeyRow
    {
        public string ModKeyName { get; set; } = string.Empty;

        public int ModKeyType { get; set; }

        public string ModKeyFileName { get; set; } = string.Empty;

        public int MappingIndex { get; set; }

        public string? Cover { get; set; }

        public string? Triangle { get; set; }

        public string? Value { get; set; }
    }

    private sealed class StaticNavmeshGridCellRow : IStaticNavmeshModKeyRow
    {
        public string ModKeyName { get; set; } = string.Empty;

        public int ModKeyType { get; set; }

        public string ModKeyFileName { get; set; } = string.Empty;

        public int GridArrayIndex { get; set; }

        public int GridCellIndex { get; set; }

        public string Value { get; set; } = string.Empty;
    }

    private sealed class StaticNavmeshTriangleRow : IStaticNavmeshModKeyRow
    {
        public string ModKeyName { get; set; } = string.Empty;

        public int ModKeyType { get; set; }

        public string ModKeyFileName { get; set; } = string.Empty;

        public int TriangleIndex { get; set; }

        public string? EdgeLink01 { get; set; }

        public string? EdgeLink12 { get; set; }

        public string? EdgeLink20 { get; set; }

        public string? Height { get; set; }

        public string? Vertices { get; set; }

        public string? CoverFlags { get; set; }

        public string? Flags { get; set; }
    }

    private sealed class StaticNavmeshVersioningRow : IStaticNavmeshModKeyRow
    {
        public string ModKeyName { get; set; } = string.Empty;

        public int ModKeyType { get; set; }

        public string ModKeyFileName { get; set; } = string.Empty;

        public int VersioningIndex { get; set; }

        public string Value { get; set; } = string.Empty;
    }

    private sealed class StaticNavmeshVertexRow : IStaticNavmeshModKeyRow
    {
        public string ModKeyName { get; set; } = string.Empty;

        public int ModKeyType { get; set; }

        public string ModKeyFileName { get; set; } = string.Empty;

        public int VertexIndex { get; set; }

        public string? Point { get; set; }
    }

    private static bool RowModKeysMatch(IStaticNavmeshModKeyRow first, IStaticNavmeshModKeyRow second)
    {
        return first.ModKeyType == second.ModKeyType &&
            string.Equals(first.ModKeyName, second.ModKeyName, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(first.ModKeyFileName, second.ModKeyFileName, StringComparison.OrdinalIgnoreCase);
    }

    private sealed class StaticPropertyRow
    {
        public string Game { get; set; } = string.Empty;

        public string ModKeyName { get; set; } = string.Empty;

        public int ModKeyType { get; set; }

        public string ModKeyFileName { get; set; } = string.Empty;

        public string FormKeyModKeyName { get; set; } = string.Empty;

        public int FormKeyModKeyType { get; set; }

        public string FormKeyModKeyFileName { get; set; } = string.Empty;

        public long FormKeyId { get; set; }

        public int PropertyIndex { get; set; }

        public string? ActorValueModKeyName { get; set; }

        public int? ActorValueModKeyType { get; set; }

        public string? ActorValueModKeyFileName { get; set; }

        public long? ActorValueFormKeyId { get; set; }

        public double? Value { get; set; }

        public DateTime ImportedAtUTC { get; set; }
    }
}
