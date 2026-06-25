using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Repositories.Interfaces;
using NPoco;

namespace CreationsForge.Core.Repositories;

public class DoorRepository : TypedRecordRepositoryBase, IDoorRepository
{
    private readonly IModelRepository ModelRepository;
    private readonly IKeywordMappingRepository KeywordMappingRepository;
    private readonly ISoundMappingRepository SoundMappingRepository;
    private readonly IScriptingAdapterRepository ScriptingAdapterRepository;
    private readonly IRecordComponentRepository RecordComponentRepository;
    private readonly IReflectionRepository ReflectionRepository;

    public DoorRepository(
        IDatabase database,
        IRecordInstanceRepository recordInstanceRepository,
        IModelRepository modelRepository,
        IKeywordMappingRepository keywordMappingRepository,
        ISoundMappingRepository soundMappingRepository,
        IScriptingAdapterRepository scriptingAdapterRepository,
        IRecordComponentRepository recordComponentRepository,
        IReflectionRepository reflectionRepository)
        : base(database, recordInstanceRepository)
    {
        ModelRepository = modelRepository;
        KeywordMappingRepository = keywordMappingRepository;
        SoundMappingRepository = soundMappingRepository;
        ScriptingAdapterRepository = scriptingAdapterRepository;
        RecordComponentRepository = recordComponentRepository;
        ReflectionRepository = reflectionRepository;
    }

    public override string RecordType => RecordTypeCatalog.Door.RecordID;

    protected override string TableName => RecordTypeCatalog.Door.TableName;

    public IReadOnlyList<DoorDTO> GetByFormKey(SupportedGame game, FormKeyDTO formKey)
    {
        var records = FetchByFormKey<DoorRow>(
                game,
                formKey,
                [
                    SelectColumn("Version2"),
                    SelectColumn("VersionControl"),
                    SelectColumn("ObjectBounds_First", "ObjectBoundsFirst"),
                    SelectColumn("ObjectBounds_Second", "ObjectBoundsSecond"),
                    SelectColumn("Name"),
                    SelectColumn("Flags"),
                    SelectColumn("MajorFlags"),
                    SelectColumn("NativeTerminal_ModKey_Name", "NativeTerminalModKeyName"),
                    SelectColumn("NativeTerminal_ModKey_Type", "NativeTerminalModKeyType"),
                    SelectColumn("NativeTerminal_ModKey_FileName", "NativeTerminalModKeyFileName"),
                    SelectColumn("NativeTerminal_FormKey_ID", "NativeTerminalFormKeyId"),
                    SelectColumn("SoundLevel"),
                    SelectColumn("FacingAxisOverride"),
                    SelectColumn("AnimationGraph"),
                    SelectColumn("AnimationSkeleton"),
                    SelectColumn("AnimationDirectory"),
                    SelectColumn("AnimationFile")
                ])
            .Select(record => ToDTO(record, game))
            .ToList();
        var models = ModelRepository.GetByFormKey(game, RecordTypeCatalog.Door.RecordID, formKey);
        var keywords = KeywordMappingRepository.GetByFormKey(game, RecordTypeCatalog.Door.RecordID, formKey);
        var sounds = SoundMappingRepository.GetByFormKey(game, RecordTypeCatalog.Door.RecordID, formKey);
        var scriptingAdapters = ScriptingAdapterRepository.GetByFormKey(game, RecordTypeCatalog.Door.RecordID, formKey);
        var components = RecordComponentRepository.GetByFormKey(game, RecordTypeCatalog.Door.RecordID, formKey);
        var reflections = ReflectionRepository.GetByFormKey(game, RecordTypeCatalog.Door.RecordID, formKey);
        var forcedLocations = GetForcedLocationsByFormKey(game, formKey);
        var navmeshGeometries = FetchNavmeshGeometriesByFormKey(game, formKey);
        foreach (var record in records)
        {
            record.Models = models.Where(model => IsSameModKey(model.ModKey, record.ModKey)).OrderBy(model => model.ModelSlot).ThenBy(model => model.ModelGender).ToList();
            record.Keywords = keywords.Where(keyword => IsSameModKey(keyword.ModKey, record.ModKey)).OrderBy(keyword => keyword.KeywordIndex).ToList();
            record.Sounds = sounds.Where(sound => IsSameModKey(sound.ModKey, record.ModKey)).OrderBy(sound => sound.SoundIndex).ToList();
            record.ScriptingAdapters = scriptingAdapters.Where(adapter => IsSameModKey(adapter.ModKey, record.ModKey)).OrderBy(adapter => adapter.ScriptIndex).ToList();
            record.Components = components.Where(component => IsSameModKey(component.ModKey, record.ModKey)).OrderBy(component => component.ComponentIndex).ToList();
            record.Reflections = reflections.Where(reflection => IsSameModKey(reflection.ModKey, record.ModKey)).OrderBy(reflection => reflection.ComponentIndex).ToList();
            record.ForcedLocations = forcedLocations
                .Where(location => IsSameModKey(location.ModKey, record.ModKey))
                .OrderBy(location => location.ForcedLocationIndex)
                .Select(location => location.ForcedLocation)
                .ToList();
            record.NavmeshGeometry = navmeshGeometries.FirstOrDefault(navmesh => IsSameModKey(navmesh.ModKey, record.ModKey))?.Geometry;
        }

        return records;
    }

    public void Save(DoorDTO dto)
    {
        SaveRecordInstance(dto);
        Database.Execute(
            """
            INSERT OR REPLACE INTO Doors (
                Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                EditorID, FormVersion, MajorRecordFlags, ImportedAtUTC, Version2, VersionControl, ObjectBounds_First, ObjectBounds_Second, Name, Flags, MajorFlags,
                NativeTerminal_ModKey_Name, NativeTerminal_ModKey_Type, NativeTerminal_ModKey_FileName, NativeTerminal_FormKey_ID,
                SoundLevel, FacingAxisOverride, AnimationGraph, AnimationSkeleton, AnimationDirectory, AnimationFile)
            VALUES (
                @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                @EditorId, @FormVersion, @MajorRecordFlags, @ImportedAtUTC, @Version2, @VersionControl, @ObjectBoundsFirst, @ObjectBoundsSecond, @Name, @Flags, @MajorFlags,
                @NativeTerminalModKeyName, @NativeTerminalModKeyType, @NativeTerminalModKeyFileName, @NativeTerminalFormKeyId,
                @SoundLevel, @FacingAxisOverride, @AnimationGraph, @AnimationSkeleton, @AnimationDirectory, @AnimationFile);
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
                dto.Version2,
                dto.VersionControl,
                dto.ObjectBoundsFirst,
                dto.ObjectBoundsSecond,
                Name = GetEnglishText(dto.Name),
                dto.Flags,
                dto.MajorFlags,
                NativeTerminalModKeyName = dto.NativeTerminalFormKey?.ModKey.Name,
                NativeTerminalModKeyType = dto.NativeTerminalFormKey?.ModKey.Type,
                NativeTerminalModKeyFileName = dto.NativeTerminalFormKey?.ModKey.FileName,
                NativeTerminalFormKeyId = dto.NativeTerminalFormKey?.Id,
                dto.SoundLevel,
                dto.FacingAxisOverride,
                dto.AnimationGraph,
                dto.AnimationSkeleton,
                dto.AnimationDirectory,
                dto.AnimationFile
            });
        ReplaceDoorForcedLocations(dto);
        DeleteNavmeshGeometry(dto);
        SaveNavmeshGeometry(dto);
    }

    private static DoorDTO ToDTO(DoorRow record, SupportedGame game)
    {
        var dto = new DoorDTO
        {
            Game = game,
            ModKey = new ModKeyDTO { Name = string.Empty, Type = 0, FileName = string.Empty },
            FormKey = new FormKeyDTO { ModKey = new ModKeyDTO { Name = string.Empty, Type = 0, FileName = string.Empty }, Id = 0 },
            EditorID = string.Empty,
            FormVersion = 0,
            MajorRecordFlags = 0,
            ImportedAtUTC = record.ImportedAtUTC,
            Version2 = record.Version2,
            VersionControl = record.VersionControl,
            ObjectBoundsFirst = record.ObjectBoundsFirst,
            ObjectBoundsSecond = record.ObjectBoundsSecond,
            Name = FromEnglish(record.Name),
            Flags = record.Flags,
            MajorFlags = record.MajorFlags,
            NativeTerminalFormKey = CreateNullableFormKey(record.NativeTerminalModKeyName, record.NativeTerminalModKeyType, record.NativeTerminalModKeyFileName, record.NativeTerminalFormKeyId),
            SoundLevel = record.SoundLevel,
            FacingAxisOverride = record.FacingAxisOverride,
            AnimationGraph = record.AnimationGraph,
            AnimationSkeleton = record.AnimationSkeleton,
            AnimationDirectory = record.AnimationDirectory,
            AnimationFile = record.AnimationFile
        };
        ApplyCommonFields(dto, record, game);
        return dto;
    }

    private IReadOnlyList<DoorForcedLocationRow> GetForcedLocationsByFormKey(SupportedGame game, FormKeyDTO formKey)
    {
        return Database.Fetch<DoorForcedLocationRow>(
                """
                SELECT
                    ModKey_Name AS ModKeyName,
                    ModKey_Type AS ModKeyType,
                    ModKey_FileName AS ModKeyFileName,
                    ForcedLocation_ModKey_Name AS ForcedLocationModKeyName,
                    ForcedLocation_ModKey_Type AS ForcedLocationModKeyType,
                    ForcedLocation_ModKey_FileName AS ForcedLocationModKeyFileName,
                    ForcedLocation_FormKey_ID AS ForcedLocationFormKeyId,
                    ForcedLocation_Index AS ForcedLocationIndex
                FROM DoorForcedLocations
                WHERE Game = @Game
                  AND FormKey_ModKey_Name = @FormKeyModKeyName COLLATE NOCASE
                  AND FormKey_ModKey_Type = @FormKeyModKeyType
                  AND FormKey_ModKey_FileName = @FormKeyModKeyFileName COLLATE NOCASE
                  AND FormKey_ID = @FormKeyId
                ORDER BY ModKey_FileName COLLATE NOCASE, ForcedLocation_Index;
                """,
                new
                {
                    Game = game.ToString(),
                    FormKeyModKeyName = formKey.ModKey.Name,
                    FormKeyModKeyType = formKey.ModKey.Type,
                    FormKeyModKeyFileName = formKey.ModKey.FileName,
                    FormKeyId = formKey.Id
                })
            .Select(row =>
            {
                row.ModKey = new ModKeyDTO { Name = row.ModKeyName, Type = row.ModKeyType, FileName = row.ModKeyFileName };
                row.ForcedLocation = CreateFormKey(row.ForcedLocationModKeyName, row.ForcedLocationModKeyType, row.ForcedLocationModKeyFileName, row.ForcedLocationFormKeyId);
                return row;
            })
            .ToList();
    }

    private void ReplaceDoorForcedLocations(DoorDTO dto)
    {
        Database.Execute(
            """
            DELETE FROM DoorForcedLocations
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

        for (var forcedLocationIndex = 0; forcedLocationIndex < dto.ForcedLocations.Count; forcedLocationIndex++)
        {
            var forcedLocation = dto.ForcedLocations[forcedLocationIndex];
            Database.Execute(
                """
                INSERT OR REPLACE INTO DoorForcedLocations (
                    Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                    ForcedLocation_ModKey_Name, ForcedLocation_ModKey_Type, ForcedLocation_ModKey_FileName, ForcedLocation_FormKey_ID, ForcedLocation_Index, ImportedAtUTC)
                VALUES (
                    @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                    @ForcedLocationModKeyName, @ForcedLocationModKeyType, @ForcedLocationModKeyFileName, @ForcedLocationFormKeyId, @ForcedLocationIndex, @ImportedAtUTC);
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
                    ForcedLocationModKeyName = forcedLocation.ModKey.Name,
                    ForcedLocationModKeyType = forcedLocation.ModKey.Type,
                    ForcedLocationModKeyFileName = forcedLocation.ModKey.FileName,
                    ForcedLocationFormKeyId = forcedLocation.Id,
                    ForcedLocationIndex = forcedLocationIndex,
                    dto.ImportedAtUTC
                });
        }
    }

    private IReadOnlyList<DoorNavmeshGeometryRow> FetchNavmeshGeometriesByFormKey(SupportedGame game, FormKeyDTO formKey)
    {
        var roots = Database.Fetch<DoorNavmeshGeometryRootRow>(
            """
            SELECT
                ModKey_Name AS ModKeyName,
                ModKey_Type AS ModKeyType,
                ModKey_FileName AS ModKeyFileName,
                GridMin,
                GridMax,
                GridMaxDistance,
                GridSize,
                Parent_MutagenObjectType AS ParentMutagenObjectType,
                Parent_ModKey_Name AS ParentModKeyName,
                Parent_ModKey_Type AS ParentModKeyType,
                Parent_ModKey_FileName AS ParentModKeyFileName,
                Parent_FormKey_ID AS ParentFormKeyId
            FROM DoorNavmeshGeometries
            WHERE Game = @Game
              AND FormKey_ModKey_Name = @FormKeyModKeyName COLLATE NOCASE
              AND FormKey_ModKey_Type = @FormKeyModKeyType
              AND FormKey_ModKey_FileName = @FormKeyModKeyFileName COLLATE NOCASE
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
            return new List<DoorNavmeshGeometryRow>();
        }

        var gridCells = FetchNavmeshGridCellsByFormKey(game, formKey);
        var triangles = FetchNavmeshTrianglesByFormKey(game, formKey);
        var versioning = FetchNavmeshVersioningByFormKey(game, formKey);
        var vertices = FetchNavmeshVerticesByFormKey(game, formKey);
        return roots.Select(root => new DoorNavmeshGeometryRow
        {
            ModKey = new ModKeyDTO { Name = root.ModKeyName, Type = root.ModKeyType, FileName = root.ModKeyFileName },
            Geometry = ToNavmeshGeometryDTO(root, gridCells, triangles, versioning, vertices)
        }).ToList();
    }

    private IReadOnlyList<DoorNavmeshGridCellRow> FetchNavmeshGridCellsByFormKey(SupportedGame game, FormKeyDTO formKey)
    {
        return Database.Fetch<DoorNavmeshGridCellRow>(
            """
            SELECT ModKey_Name AS ModKeyName, ModKey_Type AS ModKeyType, ModKey_FileName AS ModKeyFileName,
                   GridArray_Index AS GridArrayIndex, GridCell_Index AS GridCellIndex, Value
            FROM DoorNavmeshGridCells
            WHERE Game = @Game
              AND FormKey_ModKey_Name = @FormKeyModKeyName COLLATE NOCASE
              AND FormKey_ModKey_Type = @FormKeyModKeyType
              AND FormKey_ModKey_FileName = @FormKeyModKeyFileName COLLATE NOCASE
              AND FormKey_ID = @FormKeyId
            ORDER BY ModKey_FileName COLLATE NOCASE, GridArray_Index, GridCell_Index;
            """,
            new { Game = game.ToString(), FormKeyModKeyName = formKey.ModKey.Name, FormKeyModKeyType = formKey.ModKey.Type, FormKeyModKeyFileName = formKey.ModKey.FileName, FormKeyId = formKey.Id });
    }

    private IReadOnlyList<DoorNavmeshTriangleRow> FetchNavmeshTrianglesByFormKey(SupportedGame game, FormKeyDTO formKey)
    {
        return Database.Fetch<DoorNavmeshTriangleRow>(
            """
            SELECT ModKey_Name AS ModKeyName, ModKey_Type AS ModKeyType, ModKey_FileName AS ModKeyFileName,
                   Triangle_Index AS TriangleIndex, EdgeLink_0_1 AS EdgeLink01, EdgeLink_1_2 AS EdgeLink12,
                   EdgeLink_2_0 AS EdgeLink20, Height, Vertices, CoverFlags, Flags
            FROM DoorNavmeshTriangles
            WHERE Game = @Game
              AND FormKey_ModKey_Name = @FormKeyModKeyName COLLATE NOCASE
              AND FormKey_ModKey_Type = @FormKeyModKeyType
              AND FormKey_ModKey_FileName = @FormKeyModKeyFileName COLLATE NOCASE
              AND FormKey_ID = @FormKeyId
            ORDER BY ModKey_FileName COLLATE NOCASE, Triangle_Index;
            """,
            new { Game = game.ToString(), FormKeyModKeyName = formKey.ModKey.Name, FormKeyModKeyType = formKey.ModKey.Type, FormKeyModKeyFileName = formKey.ModKey.FileName, FormKeyId = formKey.Id });
    }

    private IReadOnlyList<DoorNavmeshVersioningRow> FetchNavmeshVersioningByFormKey(SupportedGame game, FormKeyDTO formKey)
    {
        return Database.Fetch<DoorNavmeshVersioningRow>(
            """
            SELECT ModKey_Name AS ModKeyName, ModKey_Type AS ModKeyType, ModKey_FileName AS ModKeyFileName,
                   Versioning_Index AS VersioningIndex, Value
            FROM DoorNavmeshVersioning
            WHERE Game = @Game
              AND FormKey_ModKey_Name = @FormKeyModKeyName COLLATE NOCASE
              AND FormKey_ModKey_Type = @FormKeyModKeyType
              AND FormKey_ModKey_FileName = @FormKeyModKeyFileName COLLATE NOCASE
              AND FormKey_ID = @FormKeyId
            ORDER BY ModKey_FileName COLLATE NOCASE, Versioning_Index;
            """,
            new { Game = game.ToString(), FormKeyModKeyName = formKey.ModKey.Name, FormKeyModKeyType = formKey.ModKey.Type, FormKeyModKeyFileName = formKey.ModKey.FileName, FormKeyId = formKey.Id });
    }

    private IReadOnlyList<DoorNavmeshVertexRow> FetchNavmeshVerticesByFormKey(SupportedGame game, FormKeyDTO formKey)
    {
        return Database.Fetch<DoorNavmeshVertexRow>(
            """
            SELECT ModKey_Name AS ModKeyName, ModKey_Type AS ModKeyType, ModKey_FileName AS ModKeyFileName,
                   Vertex_Index AS VertexIndex, Point
            FROM DoorNavmeshVertices
            WHERE Game = @Game
              AND FormKey_ModKey_Name = @FormKeyModKeyName COLLATE NOCASE
              AND FormKey_ModKey_Type = @FormKeyModKeyType
              AND FormKey_ModKey_FileName = @FormKeyModKeyFileName COLLATE NOCASE
              AND FormKey_ID = @FormKeyId
            ORDER BY ModKey_FileName COLLATE NOCASE, Vertex_Index;
            """,
            new { Game = game.ToString(), FormKeyModKeyName = formKey.ModKey.Name, FormKeyModKeyType = formKey.ModKey.Type, FormKeyModKeyFileName = formKey.ModKey.FileName, FormKeyId = formKey.Id });
    }

    private static StaticNavmeshGeometryDTO ToNavmeshGeometryDTO(
        DoorNavmeshGeometryRootRow root,
        IReadOnlyList<DoorNavmeshGridCellRow> gridCells,
        IReadOnlyList<DoorNavmeshTriangleRow> triangles,
        IReadOnlyList<DoorNavmeshVersioningRow> versioning,
        IReadOnlyList<DoorNavmeshVertexRow> vertices)
    {
        return new StaticNavmeshGeometryDTO
        {
            GridMin = root.GridMin,
            GridMax = root.GridMax,
            GridMaxDistance = root.GridMaxDistance,
            GridSize = root.GridSize,
            Parent = root.ParentModKeyName == null || root.ParentModKeyType == null || root.ParentModKeyFileName == null || root.ParentFormKeyId == null
                ? null
                : new StaticNavmeshParentDTO
                {
                    MutagenObjectType = root.ParentMutagenObjectType,
                    Parent = CreateFormKey(root.ParentModKeyName, root.ParentModKeyType.Value, root.ParentModKeyFileName, root.ParentFormKeyId.Value)
                },
            GridArrays = gridCells
                .Where(row => IsSameNavmeshModKey(root, row))
                .GroupBy(row => row.GridArrayIndex)
                .Select(group => new StaticNavmeshGridArrayDTO
                {
                    GridArrayIndex = group.Key,
                    GridCell = group.OrderBy(row => row.GridCellIndex).Select(row => row.Value).ToList()
                })
                .ToList(),
            Triangles = triangles
                .Where(row => IsSameNavmeshModKey(root, row))
                .Select(row => new StaticNavmeshTriangleDTO
                {
                    TriangleIndex = row.TriangleIndex,
                    EdgeLink_0_1 = row.EdgeLink01,
                    EdgeLink_1_2 = row.EdgeLink12,
                    EdgeLink_2_0 = row.EdgeLink20,
                    Height = row.Height,
                    Vertices = row.Vertices,
                    CoverFlags = row.CoverFlags,
                    Flags = row.Flags
                })
                .ToList(),
            Versioning = versioning.Where(row => IsSameNavmeshModKey(root, row)).OrderBy(row => row.VersioningIndex).Select(row => row.Value).ToList(),
            Vertices = vertices
                .Where(row => IsSameNavmeshModKey(root, row))
                .Select(row => new StaticNavmeshVertexDTO { VertexIndex = row.VertexIndex, Point = row.Point })
                .ToList()
        };
    }

    private void DeleteNavmeshGeometry(DoorDTO dto)
    {
        Database.Execute(
            """
            DELETE FROM DoorNavmeshGeometries
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

    private void SaveNavmeshGeometry(DoorDTO dto)
    {
        if (dto.NavmeshGeometry == null)
        {
            return;
        }

        var geometry = dto.NavmeshGeometry;
        Database.Execute(
            """
            INSERT OR REPLACE INTO DoorNavmeshGeometries (
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
        SaveNavmeshGridCells(dto);
        SaveNavmeshTriangles(dto);
        SaveNavmeshVersioning(dto);
        SaveNavmeshVertices(dto);
    }

    private void SaveNavmeshGridCells(DoorDTO dto)
    {
        foreach (var gridArray in dto.NavmeshGeometry!.GridArrays)
        {
            for (var gridCellIndex = 0; gridCellIndex < gridArray.GridCell.Count; gridCellIndex++)
            {
                Database.Execute(
                    """
                    INSERT OR REPLACE INTO DoorNavmeshGridCells (
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

    private void SaveNavmeshTriangles(DoorDTO dto)
    {
        foreach (var triangle in dto.NavmeshGeometry!.Triangles)
        {
            Database.Execute(
                """
                INSERT OR REPLACE INTO DoorNavmeshTriangles (
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

    private void SaveNavmeshVersioning(DoorDTO dto)
    {
        for (var versioningIndex = 0; versioningIndex < dto.NavmeshGeometry!.Versioning.Count; versioningIndex++)
        {
            Database.Execute(
                """
                INSERT OR REPLACE INTO DoorNavmeshVersioning (
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

    private void SaveNavmeshVertices(DoorDTO dto)
    {
        foreach (var vertex in dto.NavmeshGeometry!.Vertices)
        {
            Database.Execute(
                """
                INSERT OR REPLACE INTO DoorNavmeshVertices (
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

    private static bool IsSameModKey(ModKeyDTO first, ModKeyDTO second)
    {
        return string.Equals(first.Name, second.Name, StringComparison.OrdinalIgnoreCase) &&
               first.Type == second.Type &&
               string.Equals(first.FileName, second.FileName, StringComparison.OrdinalIgnoreCase);
    }

    private static FormKeyDTO CreateFormKey(string modKeyName, int modKeyType, string modKeyFileName, long formKeyId)
    {
        return new FormKeyDTO
        {
            ModKey = new ModKeyDTO
            {
                Name = modKeyName,
                Type = modKeyType,
                FileName = modKeyFileName
            },
            Id = (uint)formKeyId
        };
    }

    private static bool IsSameNavmeshModKey(IDoorNavmeshModKeyRow first, IDoorNavmeshModKeyRow second)
    {
        return first.ModKeyType == second.ModKeyType &&
            string.Equals(first.ModKeyName, second.ModKeyName, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(first.ModKeyFileName, second.ModKeyFileName, StringComparison.OrdinalIgnoreCase);
    }

    private sealed class DoorRow : RecordRow
    {
        public int? Version2 { get; set; }

        public int? VersionControl { get; set; }

        public string? ObjectBoundsFirst { get; set; }

        public string? ObjectBoundsSecond { get; set; }

        public string? Name { get; set; }

        public string? Flags { get; set; }

        public string? MajorFlags { get; set; }

        public string? NativeTerminalModKeyName { get; set; }

        public int? NativeTerminalModKeyType { get; set; }

        public string? NativeTerminalModKeyFileName { get; set; }

        public long? NativeTerminalFormKeyId { get; set; }

        public string? SoundLevel { get; set; }

        public string? FacingAxisOverride { get; set; }

        public string? AnimationGraph { get; set; }

        public string? AnimationSkeleton { get; set; }

        public string? AnimationDirectory { get; set; }

        public string? AnimationFile { get; set; }
    }

    private sealed class DoorForcedLocationRow
    {
        public string ModKeyName { get; set; } = string.Empty;

        public int ModKeyType { get; set; }

        public string ModKeyFileName { get; set; } = string.Empty;

        public ModKeyDTO ModKey { get; set; } = new() { Name = string.Empty, Type = 0, FileName = string.Empty };

        public string ForcedLocationModKeyName { get; set; } = string.Empty;

        public int ForcedLocationModKeyType { get; set; }

        public string ForcedLocationModKeyFileName { get; set; } = string.Empty;

        public long ForcedLocationFormKeyId { get; set; }

        public int ForcedLocationIndex { get; set; }

        public FormKeyDTO ForcedLocation { get; set; } = new() { ModKey = new ModKeyDTO { Name = string.Empty, Type = 0, FileName = string.Empty }, Id = 0 };
    }

    private sealed class DoorNavmeshGeometryRow
    {
        public ModKeyDTO ModKey { get; set; } = new() { Name = string.Empty, Type = 0, FileName = string.Empty };

        public StaticNavmeshGeometryDTO Geometry { get; set; } = new();
    }

    private interface IDoorNavmeshModKeyRow
    {
        string ModKeyName { get; }

        int ModKeyType { get; }

        string ModKeyFileName { get; }
    }

    private sealed class DoorNavmeshGeometryRootRow : IDoorNavmeshModKeyRow
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

    private sealed class DoorNavmeshGridCellRow : IDoorNavmeshModKeyRow
    {
        public string ModKeyName { get; set; } = string.Empty;

        public int ModKeyType { get; set; }

        public string ModKeyFileName { get; set; } = string.Empty;

        public int GridArrayIndex { get; set; }

        public int GridCellIndex { get; set; }

        public string Value { get; set; } = string.Empty;
    }

    private sealed class DoorNavmeshTriangleRow : IDoorNavmeshModKeyRow
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

    private sealed class DoorNavmeshVersioningRow : IDoorNavmeshModKeyRow
    {
        public string ModKeyName { get; set; } = string.Empty;

        public int ModKeyType { get; set; }

        public string ModKeyFileName { get; set; } = string.Empty;

        public int VersioningIndex { get; set; }

        public string Value { get; set; } = string.Empty;
    }

    private sealed class DoorNavmeshVertexRow : IDoorNavmeshModKeyRow
    {
        public string ModKeyName { get; set; } = string.Empty;

        public int ModKeyType { get; set; }

        public string ModKeyFileName { get; set; } = string.Empty;

        public int VertexIndex { get; set; }

        public string? Point { get; set; }
    }
}
