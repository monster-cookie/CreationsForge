namespace CreationsForge.Core.Engine.Contracts;

/// <summary>Identifies how a staged edit selects or creates its FormList target.</summary>
public enum FormListEditRole
{
    /// <summary>Allocate a new FormList in the selected output.</summary>
    New,

    /// <summary>Create or reuse an override of an existing source or load-order FormList.</summary>
    Override,

    /// <summary>Edit a FormList already contained in the selected output without allocating or copying a source record.</summary>
    ExistingOutput
}
