using System.ComponentModel;
using System.Globalization;
using CreationsForge.NativeEditing.Schema;

namespace CreationsForge.NativeEditing.Drafts;

/// <summary>Identifies which projection of a native FormLink-or-index value is authoritative for its containing owner.</summary>
public enum NativeWireFormLinkOrIndexActiveBranch
{
    /// <summary>The FormLink projection is active.</summary>
    Link,
    /// <summary>The alias index projection is active.</summary>
    AliasIndex,
    /// <summary>The package-data index projection is active.</summary>
    PackageDataIndex,
}
