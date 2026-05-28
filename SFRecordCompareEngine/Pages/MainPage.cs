using SFRecordCompareEngine.ViewModels;

namespace SFRecordCompareEngine.Pages;

public partial class MainPage : ContentPage
{
    public MainPage(MainPageViewModel viewModel)
    {
        BindingContext = viewModel;
        Title = "Starfield Record Compare Engine";
        BackgroundColor = Colors.White;
        MenuBarItems.Add(CreateFileMenu(viewModel));
        ToolbarItems.Add(CreateOpenToolbarItem(viewModel));
        Content = CreateContent();
    }

    private static MenuBarItem CreateFileMenu(MainPageViewModel viewModel)
    {
        var fileMenu = new MenuBarItem
        {
            Text = "File"
        };

        fileMenu.Add(new MenuFlyoutItem
        {
            Text = "Open",
            Command = viewModel.OpenCommand
        });
        fileMenu.Add(new MenuFlyoutSeparator());
        fileMenu.Add(new MenuFlyoutItem
        {
            Text = "Exit",
            Command = viewModel.ExitCommand
        });

        return fileMenu;
    }

    private static ToolbarItem CreateOpenToolbarItem(MainPageViewModel viewModel)
    {
        return new ToolbarItem
        {
            Text = "Open",
            Command = viewModel.OpenCommand,
            Order = ToolbarItemOrder.Primary,
            Priority = 0
        };
    }

    private static View CreateContent()
    {
        var statusLabel = new Label
        {
            FontSize = 14,
            TextColor = Color.FromArgb("#555555")
        };
        statusLabel.SetBinding(Label.TextProperty, nameof(MainPageViewModel.StatusText));

        var statusBorder = new Border
        {
            Stroke = Color.FromArgb("#DDDDDD"),
            StrokeThickness = 1,
            Padding = new Thickness(12),
            Content = statusLabel
        };
        Grid.SetRow(statusBorder, 1);

        return new Grid
        {
            RowDefinitions =
            {
                new RowDefinition(GridLength.Star),
                new RowDefinition(GridLength.Auto)
            },
            Padding = new Thickness(20),
            Children =
            {
                new Label
                {
                    Text = "Record comparison workspace",
                    FontSize = 20,
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Start
                },
                statusBorder
            }
        };
    }
}
