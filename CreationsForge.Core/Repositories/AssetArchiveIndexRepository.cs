using CreationsForge.Core.DTOs.Assets;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Repositories.Interfaces;
using NPoco;
using System.Text;

namespace CreationsForge.Core.Repositories;

public class AssetArchiveIndexRepository : IAssetArchiveIndexRepository
{
    private const int InsertBatchSize = 100;
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

    public long RefreshArchiveIndex(AssetArchiveFileDTO archiveFile, IEnumerable<AssetArchiveEntryDTO> entries, Action<long>? insertedCountProgress = null)
    {
        using var transaction = Database.GetTransaction();
        SaveArchiveFile(archiveFile);
        var insertedCount = ReplaceArchiveEntriesCore(archiveFile.Game, archiveFile.ArchivePath, entries, insertedCountProgress);
        transaction.Complete();
        return insertedCount;
    }

    public long ReplaceArchiveEntries(SupportedGame game, string archivePath, IEnumerable<AssetArchiveEntryDTO> entries)
    {
        using var transaction = Database.GetTransaction();
        var insertedCount = ReplaceArchiveEntriesCore(game, archivePath, entries, null);
        transaction.Complete();
        return insertedCount;
    }

    private long ReplaceArchiveEntriesCore(SupportedGame game, string archivePath, IEnumerable<AssetArchiveEntryDTO> entries, Action<long>? insertedCountProgress)
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

        long insertedCount = 0;
        var batch = new List<AssetArchiveEntryDTO>(InsertBatchSize);
        foreach (var entry in entries)
        {
            batch.Add(entry);
            if (batch.Count == InsertBatchSize)
            {
                InsertArchiveEntryBatch(batch);
                insertedCount += batch.Count;
                insertedCountProgress?.Invoke(insertedCount);
                batch.Clear();
            }
        }

        if (batch.Count > 0)
        {
            InsertArchiveEntryBatch(batch);
            insertedCount += batch.Count;
            insertedCountProgress?.Invoke(insertedCount);
        }

        return insertedCount;
    }

    private void InsertArchiveEntryBatch(IReadOnlyList<AssetArchiveEntryDTO> entries)
    {
        if (Database is not NPoco.Database npocoDatabase)
        {
            throw new InvalidOperationException("Asset archive index repository requires an NPoco Database instance.");
        }

        using var command = npocoDatabase.Connection.CreateCommand();
        command.Transaction = npocoDatabase.Transaction;
        var sql = new StringBuilder("""
            INSERT INTO AssetArchiveEntries (
                Game,
                ArchivePath,
                NormalizedEntryPath,
                RootFolder,
                Extension,
                PackedSize,
                UnpackedSize)
            VALUES
            """);

        for (var index = 0; index < entries.Count; index++)
        {
            var entry = entries[index];
            if (index > 0)
            {
                sql.Append(",");
            }

            sql.AppendLine();
            sql.Append("(@Game");
            sql.Append(index);
            sql.Append(", @ArchivePath");
            sql.Append(index);
            sql.Append(", @NormalizedEntryPath");
            sql.Append(index);
            sql.Append(", @RootFolder");
            sql.Append(index);
            sql.Append(", @Extension");
            sql.Append(index);
            sql.Append(", @PackedSize");
            sql.Append(index);
            sql.Append(", @UnpackedSize");
            sql.Append(index);
            sql.Append(')');

            AddParameter(command, "@Game" + index, entry.Game.ToString());
            AddParameter(command, "@ArchivePath" + index, entry.ArchivePath);
            AddParameter(command, "@NormalizedEntryPath" + index, entry.NormalizedEntryPath);
            AddParameter(command, "@RootFolder" + index, entry.RootFolder);
            AddParameter(command, "@Extension" + index, entry.Extension);
            AddParameter(command, "@PackedSize" + index, entry.PackedSize);
            AddParameter(command, "@UnpackedSize" + index, entry.UnpackedSize);
        }

        command.CommandText = sql.ToString();
        command.ExecuteNonQuery();
    }

    private static void AddParameter(System.Data.Common.DbCommand command, string name, object value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value;
        command.Parameters.Add(parameter);
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
