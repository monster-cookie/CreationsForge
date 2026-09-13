using System.ComponentModel;
using System.Globalization;
using CreationsForge.RecordEditing.Schema;

namespace CreationsForge.RecordEditing.Drafts;

/// <summary>Identifies which projection of a record FormLink-or-index value is authoritative for its containing owner.</summary>
public enum RecordWireFormLinkOrIndexActiveBranch
{
    /// <summary>The FormLink projection is active.</summary>
    Link,
    /// <summary>The alias index projection is active.</summary>
    AliasIndex,
    /// <summary>The package-data index projection is active.</summary>
    PackageDataIndex,
}
