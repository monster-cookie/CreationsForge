using CreationsForge.Core.DTOs.Assets;
using CreationsForge.Core.Enums;

namespace CreationsForge.Core.Repositories.Interfaces;

public interface IAssetArchiveIndexRepository
{
    AssetArchiveFileDTO? GetArchiveFile(SupportedGame game, string archivePath);

    AssetArchiveEntryDTO? FindEntry(SupportedGame game, string archivePath, IReadOnlyList<string> normalizedEntryPaths);

    IReadOnlyList<AssetArchiveEntryDTO> FindEntries(SupportedGame game, string dataFolder, IReadOnlyList<string> normalizedEntryPaths);

    void SaveArchiveFile(AssetArchiveFileDTO archiveFile);

    long ReplaceArchiveEntries(SupportedGame game, string archivePath, IEnumerable<AssetArchiveEntryDTO> entries);

    void DeleteArchive(SupportedGame game, string archivePath);
}
