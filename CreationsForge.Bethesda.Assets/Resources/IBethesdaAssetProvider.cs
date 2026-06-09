namespace CreationsForge.Bethesda.Assets.Resources;

public interface IBethesdaAssetProvider
{
    BethesdaAssetReadResult TryReadAsset(BethesdaAssetReadRequest request);
}
