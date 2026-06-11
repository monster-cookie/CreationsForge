using CreationsForge.Core.DTOs.Assets;
using CreationsForge.Core.Enums;

namespace CreationsForge.Core.Repositories.Interfaces;

public interface IAssetArchiveIndexRepository
{
    AssetArchiveFileDTO? GetArchiveFile(SupportedGame game, string archivePath);

    AssetArchiveEntryDTO? FindEntry(SupportedGame game, string archivePath, IReadOnlyList<string> normalizedEntryPaths);

    void SaveArchiveFile(AssetArchiveFileDTO archiveFile);

    void ReplaceArchiveEntries(SupportedGame game, string archivePath, IReadOnlyList<AssetArchiveEntryDTO> entries);

    void DeleteArchive(SupportedGame game, string archivePath);
}
