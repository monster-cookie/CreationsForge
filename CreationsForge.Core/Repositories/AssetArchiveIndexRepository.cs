using CreationsForge.Core.DTOs.Assets;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Repositories.Interfaces;
using NPoco;

namespace CreationsForge.Core.Repositories;

public class AssetArchiveIndexRepository : IAssetArchiveIndexRepository
{
    private readonly IDatabase Database;

    public AssetArchiveIndexRepository(IDatabase database)
    {
        Database = database;
    }

    public AssetArchiveFileDTO? GetArchiveFile(SupportedGame game, string archivePath)
    {
        var model = Database.FirstOrDefault<AssetArchiveFileDatabaseModel>(
            """
            SELECT *
            FROM AssetArchiveFiles
            WHERE Game = @Game
              AND ArchivePath = @ArchivePath COLLATE NOCASE;
            """,
            new
            {
                Game = game.ToString(),
                ArchivePath = archivePath
            });

        return model?.ToDTO();
    }

    public AssetArchiveEntryDTO? FindEntry(SupportedGame game, string archivePath, IReadOnlyList<string> normalizedEntryPaths)
    {
        foreach (var normalizedEntryPath in normalizedEntryPaths)
        {
            var model = Database.FirstOrDefault<AssetArchiveEntryDatabaseModel>(
                """
                SELECT *
                FROM AssetArchiveEntries
                WHERE Game = @Game
                  AND ArchivePath = @ArchivePath COLLATE NOCASE
                  AND NormalizedEntryPath = @NormalizedEntryPath COLLATE NOCASE;
                """,
                new
                {
                    Game = game.ToString(),
                    ArchivePath = archivePath,
                    NormalizedEntryPath = normalizedEntryPath
                });

            if (model != null)
            {
                return model.ToDTO();
            }
        }

        return null;
    }

    public IReadOnlyList<AssetArchiveEntryDTO> FindEntries(SupportedGame game, string dataFolder, IReadOnlyList<string> normalizedEntryPaths)
    {
        var entries = new List<AssetArchiveEntryDTO>();
        foreach (var normalizedEntryPath in normalizedEntryPaths)
        {
            var models = Database.Fetch<AssetArchiveEntryDatabaseModel>(
                """
                SELECT entries.*
                FROM AssetArchiveEntries entries
                INNER JOIN AssetArchiveFiles archives
                    ON archives.Game = entries.Game
                   AND archives.ArchivePath = entries.ArchivePath COLLATE NOCASE
                WHERE entries.Game = @Game
                  AND archives.DataFolder = @DataFolder COLLATE NOCASE
                  AND entries.NormalizedEntryPath = @NormalizedEntryPath COLLATE NOCASE
                ORDER BY archives.ArchiveFileName COLLATE NOCASE;
                """,
                new
                {
                    Game = game.ToString(),
                    DataFolder = Path.GetFullPath(dataFolder),
                    NormalizedEntryPath = normalizedEntryPath
                });

            entries.AddRange(models.Select(model => model.ToDTO()));
        }

        return entries;
    }

    public void SaveArchiveFile(AssetArchiveFileDTO archiveFile)
    {
        Database.Execute(
            """
            INSERT INTO AssetArchiveFiles (
                Game,
                DataFolder,
                ArchivePath,
                ArchiveFileName,
                ArchiveExtension,
                ArchiveType,
                SourceLastWriteUTCTicks,
                SourceFileSizeBytes,
                IndexedAtUTC)
            VALUES (
                @Game,
                @DataFolder,
                @ArchivePath,
                @ArchiveFileName,
                @ArchiveExtension,
                @ArchiveType,
                @SourceLastWriteUTCTicks,
                @SourceFileSizeBytes,
                @IndexedAtUTC)
            ON CONFLICT(Game, ArchivePath) DO UPDATE SET
                DataFolder = excluded.DataFolder,
                ArchiveFileName = excluded.ArchiveFileName,
                ArchiveExtension = excluded.ArchiveExtension,
                ArchiveType = excluded.ArchiveType,
                SourceLastWriteUTCTicks = excluded.SourceLastWriteUTCTicks,
                SourceFileSizeBytes = excluded.SourceFileSizeBytes,
                IndexedAtUTC = excluded.IndexedAtUTC;
            """,
            new
            {
                Game = archiveFile.Game.ToString(),
                archiveFile.DataFolder,
                archiveFile.ArchivePath,
                archiveFile.ArchiveFileName,
                archiveFile.ArchiveExtension,
                archiveFile.ArchiveType,
                archiveFile.SourceLastWriteUTCTicks,
                archiveFile.SourceFileSizeBytes,
                archiveFile.IndexedAtUTC
            });
    }

    public void ReplaceArchiveEntries(SupportedGame game, string archivePath, IReadOnlyList<AssetArchiveEntryDTO> entries)
    {
        Database.Execute(
            """
            DELETE FROM AssetArchiveEntries
            WHERE Game = @Game
              AND ArchivePath = @ArchivePath COLLATE NOCASE;
            """,
            new
            {
                Game = game.ToString(),
                ArchivePath = archivePath
            });

        foreach (var entry in entries)
        {
            Database.Execute(
                """
                INSERT INTO AssetArchiveEntries (
                    Game,
                    ArchivePath,
                    NormalizedEntryPath,
                    RootFolder,
                    Extension,
                    PackedSize,
                    UnpackedSize)
                VALUES (
                    @Game,
                    @ArchivePath,
                    @NormalizedEntryPath,
                    @RootFolder,
                    @Extension,
                    @PackedSize,
                    @UnpackedSize);
                """,
                new
                {
                    Game = entry.Game.ToString(),
                    entry.ArchivePath,
                    entry.NormalizedEntryPath,
                    entry.RootFolder,
                    entry.Extension,
                    entry.PackedSize,
                    entry.UnpackedSize
                });
        }
    }

    public void DeleteArchive(SupportedGame game, string archivePath)
    {
        Database.Execute(
            """
            DELETE FROM AssetArchiveFiles
            WHERE Game = @Game
              AND ArchivePath = @ArchivePath COLLATE NOCASE;
            """,
            new
            {
                Game = game.ToString(),
                ArchivePath = archivePath
            });
    }

    private class AssetArchiveFileDatabaseModel
    {
        public required string Game { get; set; }

        public required string DataFolder { get; set; }

        public required string ArchivePath { get; set; }

        public required string ArchiveFileName { get; set; }

        public required string ArchiveExtension { get; set; }

        public required string ArchiveType { get; set; }

        public long SourceLastWriteUTCTicks { get; set; }

        public long SourceFileSizeBytes { get; set; }

        public DateTime IndexedAtUTC { get; set; }

        public AssetArchiveFileDTO ToDTO()
        {
            return new AssetArchiveFileDTO
            {
                Game = Enum.Parse<SupportedGame>(Game),
                DataFolder = DataFolder,
                ArchivePath = ArchivePath,
                ArchiveFileName = ArchiveFileName,
                ArchiveExtension = ArchiveExtension,
                ArchiveType = ArchiveType,
                SourceLastWriteUTCTicks = SourceLastWriteUTCTicks,
                SourceFileSizeBytes = SourceFileSizeBytes,
                IndexedAtUTC = IndexedAtUTC
            };
        }
    }

    private class AssetArchiveEntryDatabaseModel
    {
        public required string Game { get; set; }

        public required string ArchivePath { get; set; }

        public required string NormalizedEntryPath { get; set; }

        public required string RootFolder { get; set; }

        public required string Extension { get; set; }

        public long PackedSize { get; set; }

        public long UnpackedSize { get; set; }

        public AssetArchiveEntryDTO ToDTO()
        {
            return new AssetArchiveEntryDTO
            {
                Game = Enum.Parse<SupportedGame>(Game),
                ArchivePath = ArchivePath,
                NormalizedEntryPath = NormalizedEntryPath,
                RootFolder = RootFolder,
                Extension = Extension,
                PackedSize = PackedSize,
                UnpackedSize = UnpackedSize
            };
        }
    }
}
