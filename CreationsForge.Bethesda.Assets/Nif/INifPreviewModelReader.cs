namespace CreationsForge.Bethesda.Assets.Nif;

public interface INifPreviewModelReader
{
    NifPreviewReadResult TryRead(NifPreviewReadRequest request);
}
