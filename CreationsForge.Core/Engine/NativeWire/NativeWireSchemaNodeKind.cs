namespace CreationsForge.Core.Engine.NativeWire;

/// <summary>Identifies the compiled catalog role of one native wire schema node.</summary>
public enum NativeWireSchemaNodeKind
{
    /// <summary>The node describes one stable FormList edit command and its closed argument shape.</summary>
    Command,

    /// <summary>The node describes one concrete native type and its complete field construction contract.</summary>
    Type
}
