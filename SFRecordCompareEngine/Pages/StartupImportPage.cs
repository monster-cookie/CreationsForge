using SFRecordCompareEngine.ViewModels;

namespace SFRecordCompareEngine.Pages;

public partial class StartupImportPage : ContentPage
{
    private readonly StartupImportViewModel ViewModel;

    public StartupImportPage(StartupImportViewModel viewModel)
    {
        ViewModel = viewModel;
        BindingContext = ViewModel;
        Title = "Starfield Record Compare Engine";
        Padding = new Thickness(32);
        BackgroundColor = Colors.White;
        Content = CreateContent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ViewModel.StartImportAsync();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        ViewModel.CancelImport();
    }

    private static View CreateContent()
    {
        var statusLabel = new Label
        {
            FontAttributes = FontAttributes.Bold,
            FontSize = 18,
            HorizontalTextAlignment = TextAlignment.Center
        };
        statusLabel.SetBinding(Label.TextProperty, nameof(StartupImportViewModel.StatusText));

        var pluginLabel = new Label
        {
            FontSize = 13,
            TextColor = Color.FromArgb("#555555"),
            HorizontalTextAlignment = TextAlignment.Center,
            LineBreakMode = LineBreakMode.TailTruncation
        };
        pluginLabel.SetBinding(Label.TextProperty, nameof(StartupImportViewModel.CurrentPluginText));

        var progressBar = new ProgressBar
        {
            HeightRequest = 18
        };
        progressBar.SetBinding(ProgressBar.ProgressProperty, nameof(StartupImportViewModel.ProgressPercentage));

        var activityIndicator = new ActivityIndicator
        {
            Color = Color.FromArgb("#2F5D50")
        };
        activityIndicator.SetBinding(ActivityIndicator.IsRunningProperty, nameof(StartupImportViewModel.IsIndeterminate));
        activityIndicator.SetBinding(ActivityIndicator.IsVisibleProperty, nameof(StartupImportViewModel.IsIndeterminate));

        return new Grid
        {
            Children =
            {
                new Border
                {
                    Stroke = Color.FromArgb("#CCCCCC"),
                    StrokeThickness = 1,
                    BackgroundColor = Colors.White,
                    Padding = new Thickness(24),
                    WidthRequest = 460,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Content = new VerticalStackLayout
                    {
                        Spacing = 12,
                        Children =
                        {
                            statusLabel,
                            pluginLabel,
                            progressBar,
                            activityIndicator
                        }
                    }
                }
            }
        };
    }

}
