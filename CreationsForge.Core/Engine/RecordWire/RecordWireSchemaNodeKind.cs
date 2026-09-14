namespace CreationsForge.Core.Engine.RecordWire;

/// <summary>Identifies the compiled catalog role of one record wire schema node.</summary>
public enum RecordWireSchemaNodeKind
{
    /// <summary>The node describes one stable FormList edit command and its closed argument shape.</summary>
    Command,

    /// <summary>The node describes one concrete record type and its complete field construction contract.</summary>
    Type
}
