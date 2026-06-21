namespace CreationsForge.Core.DTOs.Records.Metadata;

[AttributeUsage(AttributeTargets.Property)]
public sealed class FormKeyColumnPrefixAttribute : Attribute
{
    public FormKeyColumnPrefixAttribute(string prefix)
    {
        Prefix = prefix;
    }

    public string Prefix { get; }
}
