using SFRecordCompareEngine.ViewModels;

namespace SFRecordCompareEngine.Pages;

public partial class OpenPluginDialogPage : ContentPage
{
    public OpenPluginDialogPage(OpenPluginDialogViewModel viewModel)
    {
        BindingContext = viewModel;
        Title = "Open";
        BackgroundColor = Colors.White;
        Padding = new Thickness(24);
        Content = CreateContent();
    }

    private static View CreateContent()
    {
        var closeButton = new Button
        {
            Text = "Close",
            WidthRequest = 100,
            HorizontalOptions = LayoutOptions.End
        };
        closeButton.SetBinding(Button.CommandProperty, nameof(OpenPluginDialogViewModel.CloseCommand));

        return new Border
        {
            Stroke = Color.FromArgb("#CCCCCC"),
            StrokeThickness = 1,
            Padding = new Thickness(20),
            WidthRequest = 420,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Content = new VerticalStackLayout
            {
                Spacing = 18,
                Children =
                {
                    new Label
                    {
                        Text = "Open dialog placeholder",
                        FontAttributes = FontAttributes.Bold,
                        FontSize = 18
                    },
                    new Label
                    {
                        Text = "This dialog will be configured in a later change.",
                        TextColor = Color.FromArgb("#555555")
                    },
                    closeButton
                }
            }
        };
    }
}
