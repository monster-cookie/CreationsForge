using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;

namespace SFRecordCompareEngine.Core.Importers.Interfaces;

public interface ITypedRecordDetailImporter
{
    /// <summary>
    /// The Game this importer supports
    /// </summary>
    GameRelease GameRelease { get; }
    
    /// <summary>
    /// The Major Record type it addresses
    /// </summary>
    RecordType RecordType { get; }

    /// <summary>
    /// The SQLite Database Table the record data is stored in.
    /// 
    /// NOTE: Not sure this is needed the model should handle this automatically.
    /// </summary>
    string TableName { get; }

    /// <summary>
    /// The import handler
    /// </summary>
    /// <param name="modKey">The ModKey for the plugin the record is in</param>
    /// <param name="formKey">The FormKey of the record</param>
    void Import(ModKey modKey, FormKey formKey);
}
