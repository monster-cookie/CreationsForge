using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.Importers.Interfaces;

namespace SFRecordCompareEngine.Core.Importers.Starfield;

public class FormListImporter : ITypedRecordDetailImporter
{
    public GameRelease GameRelease => GameRelease.Starfield;

    public RecordType RecordType => new RecordType("FLST");

    public string TableName => "FormList";

    public void Import(ModKey modKey, FormKey formKey)
    {
        // TODO: Need to handle the header row table

        // TODO: Need to handle the details data in the FormList table

        throw new NotImplementedException();
    }
}