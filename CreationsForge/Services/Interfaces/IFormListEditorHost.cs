using System.ComponentModel;
using CreationsForge.ViewModels;
using Mutagen.Bethesda.Plugins;

namespace CreationsForge.Services.Interfaces;

/// <summary>Publishes one atomic browser selection and refreshes the retained FormList browser after editor mutations.</summary>
public interface IFormListEditorHost : INotifyPropertyChanged
{
    /// <summary>Gets the exact revision-bound browser selection, or <see langword="null"/> when no editable context is selected.</summary>
    FormListEditorSelection? Selection { get; }

    /// <summary>Refreshes the plugin browser and optionally reselects one FormList through its existing generation guard.</summary>
    /// <param name="reselect">The FormList to reselect after a successful fresh load, or <see langword="null"/>.</param>
    /// <param name="cancellationToken">A token that cancels refresh without permitting stale publication.</param>
    /// <returns>A task that completes after fresh browser state is published or superseded.</returns>
    Task RefreshAsync(FormKey? reselect = null, CancellationToken cancellationToken = default);
}
