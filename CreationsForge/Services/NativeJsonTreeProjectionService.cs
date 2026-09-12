using System.Text.Json;
using CreationsForge.ViewModels;

namespace CreationsForge.Services;

/// <summary>
/// Projects detached native JSON into an ordered presentation tree without interpreting game-specific fields.
/// </summary>
public sealed class NativeJsonTreeProjectionService
{
    /// <summary>Projects one optional detached record into a single rooted hierarchy.</summary>
    /// <param name="record">The detached native JSON record, or <see langword="null"/> when the selected context has no inspectable record.</param>
    /// <param name="cancellationToken">A token observed before and throughout recursive projection.</param>
    /// <returns>An empty collection for no record, otherwise one root node containing every JSON value in source order.</returns>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public IReadOnlyList<NativeJsonFieldNodeViewModel> Project(
        JsonElement? record,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return record.HasValue
            ? Array.AsReadOnly(new[] { ProjectNode("$", record.Value, cancellationToken) })
            : Array.Empty<NativeJsonFieldNodeViewModel>();
    }

    /// <summary>Recursively projects one JSON value while retaining exact property and array enumeration order.</summary>
    /// <param name="name">The exact property name, array index label, or root label.</param>
    /// <param name="element">The JSON value to project.</param>
    /// <param name="cancellationToken">A token observed for this node and each child iteration.</param>
    /// <returns>The recursively projected presentation node.</returns>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    private static NativeJsonFieldNodeViewModel ProjectNode(
        string name,
        JsonElement element,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var children = new List<NativeJsonFieldNodeViewModel>();
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                foreach (var property in element.EnumerateObject())
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    children.Add(ProjectNode(property.Name, property.Value, cancellationToken));
                }

                break;
            case JsonValueKind.Array:
                var index = 0;
                foreach (var item in element.EnumerateArray())
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    children.Add(ProjectNode($"[{index}]", item, cancellationToken));
                    index++;
                }

                break;
        }

        return new NativeJsonFieldNodeViewModel(
            name,
            element.ValueKind,
            GetValueText(element, children.Count),
            children);
    }

    /// <summary>Returns exact scalar JSON text and an explicit marker for each container shape.</summary>
    /// <param name="element">The source JSON value.</param>
    /// <param name="childCount">The number of projected children.</param>
    /// <returns>The scalar JSON spelling or object or array marker.</returns>
    private static string GetValueText(JsonElement element, int childCount)
    {
        return element.ValueKind switch
        {
            JsonValueKind.Object => childCount == 0 ? "{}" : "{...}",
            JsonValueKind.Array => childCount == 0 ? "[]" : "[...]",
            JsonValueKind.String or
            JsonValueKind.Number or
            JsonValueKind.True or
            JsonValueKind.False or
            JsonValueKind.Null => element.GetRawText(),
            JsonValueKind.Undefined => "undefined",
            _ => element.GetRawText()
        };
    }
}
