using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.VisualTree;
using CreationsForge.Core.DTOs.Games;
using CreationsForge.ViewModels;

namespace CreationsForge.Views;

public class ImportProgressView : UserControl
{
    private readonly ImportProgressViewModel ViewModel;
    private bool ImportStarted;

    public ImportProgressView(ImportProgressViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = ViewModel;
        Content = BuildContent();
    }

    public void Configure(SupportedGameDTO selectedGame, bool forceFullReimport)
    {
        ImportStarted = false;
        ViewModel.Configure(selectedGame, forceFullReimport);
    }

    public void ConfigureResetAndImportAll()
    {
        ImportStarted = false;
        ViewModel.ConfigureResetAndImportAll();
    }

    protected override async void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (ImportStarted)
        {
            return;
        }

        ImportStarted = true;
        await ViewModel.StartImportAsync();
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
        statusText.Bind(TextBlock.TextProperty, new Binding(nameof(ImportProgressViewModel.CurrentStatusText)));

        var detailText = new TextBlock
        {
            FontSize = 16,
            HorizontalAlignment = HorizontalAlignment.Center,
            TextWrapping = TextWrapping.Wrap
        };
        App.ApplyApplicationTextForeground(detailText);
        detailText.Bind(TextBlock.TextProperty, new Binding(nameof(ImportProgressViewModel.CurrentDetailText)));

        var progressBar = new ProgressBar
        {
            Width = 620,
            Height = 18,
            HorizontalAlignment = HorizontalAlignment.Center
        };
        progressBar.Bind(RangeBase.ValueProperty, new Binding(nameof(ImportProgressViewModel.CurrentProgressValue)));
        progressBar.Bind(RangeBase.MaximumProperty, new Binding(nameof(ImportProgressViewModel.CurrentProgressMaximum)));
        progressBar.Bind(ProgressBar.IsIndeterminateProperty, new Binding(nameof(ImportProgressViewModel.CurrentIsIndeterminate)));

        var cancelButton = new Button
        {
            Content = "Cancel",
            HorizontalAlignment = HorizontalAlignment.Center,
            Padding = new Thickness(20, 8)
        };
        cancelButton.Bind(Button.CommandProperty, new Binding(nameof(ImportProgressViewModel.CancelImportCommand)));

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
                    progressBar,
                    cancelButton
                }
            }
        };
    }
}
