namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Identifies a named, typed FormList mutation whose payload is prepared by the selected game adapter.
/// </summary>
public abstract class FormListEdit
{
    /// <summary>Initializes a typed FormList edit.</summary>
    /// <param name="commandName">The stable command discriminator used for diagnostics and canonical replay identity.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="commandName"/> is empty or whitespace.</exception>
    protected FormListEdit(string commandName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(commandName);
        CommandName = commandName;
    }

    /// <summary>Gets the stable command discriminator.</summary>
    public string CommandName { get; }
}
