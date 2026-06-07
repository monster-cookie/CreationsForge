using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.VisualTree;
using CreationsForge.Core.DTOs.Games;
using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.ViewModels;

namespace CreationsForge.Views;

public class ActivePluginLoadView : UserControl
{
    private readonly ActivePluginLoadViewModel ViewModel;
    private bool LoadStarted;

    public ActivePluginLoadView(ActivePluginLoadViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = ViewModel;
        Content = BuildContent();
    }

    public void Configure(SupportedGameDTO selectedGame, PluginDTO selectedPlugin)
    {
        LoadStarted = false;
        ViewModel.Configure(selectedGame, selectedPlugin);
    }

    protected override async void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (LoadStarted)
        {
            return;
        }

        LoadStarted = true;
        await ViewModel.StartLoadAsync();
    }

    private Control BuildContent()
    {
        var statusText = new TextBlock
        {
            FontSize = 28,
            FontWeight = FontWeight.SemiBold,
            HorizontalAlignment = HorizontalAlignment.Center,
            TextWrapping = TextWrapping.Wrap
        };
        App.ApplyApplicationTextForeground(statusText);
        statusText.Bind(TextBlock.TextProperty, new Binding(nameof(ActivePluginLoadViewModel.CurrentStatusText)));

        var detailText = new TextBlock
        {
            FontSize = 16,
            HorizontalAlignment = HorizontalAlignment.Center,
            TextWrapping = TextWrapping.Wrap
        };
        App.ApplyApplicationTextForeground(detailText);
        detailText.Bind(TextBlock.TextProperty, new Binding(nameof(ActivePluginLoadViewModel.CurrentDetailText)));

        var progressBar = new ProgressBar
        {
            Width = 620,
            Height = 18,
            HorizontalAlignment = HorizontalAlignment.Center,
            IsIndeterminate = true
        };

        return new Border
        {
            Background = App.GetApplicationBrush(App.ApplicationSurfaceBrushKey),
            Padding = new Thickness(48),
            Child = new StackPanel
            {
                Spacing = 18,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Children =
                {
                    statusText,
                    detailText,
                    progressBar
                }
            }
        };
    }
}
