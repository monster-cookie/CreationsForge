namespace CreationsForge.Services.Interfaces;

/// <summary>
/// Opens an existing local asset with the configured external viewer or the operating-system association.
/// </summary>
public interface IExternalAssetOpenService
{
    /// <summary>
    /// Opens an existing local asset path outside CreationsForge.
    /// </summary>
    /// <param name="assetPath">The absolute path of the local asset to open.</param>
    /// <returns><see langword="true" /> when the operating system accepted the open request; otherwise, <see langword="false" />.</returns>
    bool OpenExternally(string assetPath);
}
