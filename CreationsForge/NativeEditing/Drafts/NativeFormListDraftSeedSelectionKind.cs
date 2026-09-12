using System.ComponentModel;
using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeWire;
using CreationsForge.Services;
using Mutagen.Bethesda.Plugins;

namespace CreationsForge.NativeEditing.Drafts;

/// <summary>Identifies the fixed command-specific seed extraction intent.</summary>
public enum NativeFormListDraftSeedSelectionKind
{
    /// <summary>Use the command's complete current record value.</summary>
    CurrentValue,
    /// <summary>Use one existing collection element.</summary>
    AtIndex,
    /// <summary>Create a new collection element at an explicit insertion position.</summary>
    InsertAt,
    /// <summary>Use explicit source and destination positions for a move command.</summary>
    Move,
}
