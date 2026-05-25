using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using NPoco;

namespace SFRecordCompareEngine.Core.Importers.Interfaces;

public interface ITypedRecordDetailImporter
{
    string RecordType { get; }
    
    string TableName { get; }

    void Import(IDatabase database, ModKey modKey, FormKey formKey, string importedAtUtc);
}
