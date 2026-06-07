using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using CreationsForge.ViewModels;

namespace CreationsForge.Views;

public class SettingsView : UserControl
{
    private readonly SettingsViewModel ViewModel;

    public SettingsView(SettingsViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = ViewModel;
        Content = BuildContent();
    }

    private Control BuildContent()
    {
        return new Border
        {
            Background = App.GetApplicationBrush(App.ApplicationSurfaceBrushKey),
            Padding = new Thickness(32),
            Child = new Grid
            {
                RowDefinitions = new RowDefinitions("Auto,Auto,*"),
                RowSpacing = 24,
                Children =
                {
                    CreateHeader(),
                    CreateSettingsForm(),
                    CreateButtonRow()
                }
            }
        };
    }

    private static Control CreateHeader()
    {
        var title = new TextBlock
        {
            Text = "Settings",
            FontSize = 28,
            FontWeight = FontWeight.SemiBold
        };
        App.ApplyApplicationTextForeground(title);

        var description = new TextBlock
        {
            Text = "Choose the active game and display mode.",
            FontSize = 15,
            TextWrapping = TextWrapping.Wrap
        };
        App.ApplyApplicationTextForeground(description);

        var header = new StackPanel
        {
            Spacing = 6,
            Children =
            {
                title,
                description
            }
        };
        Grid.SetRow(header, 0);
        return header;
    }

    private Control CreateSettingsForm()
    {
        var gameBox = CreateComboBox(nameof(SettingsViewModel.GameOptions), nameof(SettingsViewModel.SelectedGameDisplayName));
        var themeFamilyBox = CreateComboBox(nameof(SettingsViewModel.ThemeFamilyOptions), nameof(SettingsViewModel.SelectedThemeFamily));
        var themeBox = CreateComboBox(nameof(SettingsViewModel.ThemeModeOptions), nameof(SettingsViewModel.SelectedThemeMode));

        var form = new Grid
        {
            MaxWidth = 620,
            HorizontalAlignment = HorizontalAlignment.Left,
            RowDefinitions = new RowDefinitions("Auto,Auto,Auto"),
            RowSpacing = 18,
            Children =
            {
                CreateField("Active game", gameBox, 0),
                CreateField("Theme", themeFamilyBox, 1),
                CreateField("Display mode", themeBox, 2)
            }
        };
        Grid.SetRow(form, 1);
        return form;
    }

    private Control CreateButtonRow()
    {
        var saveButton = new Button
        {
            Content = "Save",
            Padding = new Thickness(18, 8)
        };
        saveButton.Bind(Button.CommandProperty, new Binding(nameof(SettingsViewModel.SaveCommand)));

        var cancelButton = new Button
        {
            Content = "Cancel",
            Padding = new Thickness(18, 8)
        };
        cancelButton.Bind(Button.CommandProperty, new Binding(nameof(SettingsViewModel.CancelCommand)));

        var row = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 10,
            VerticalAlignment = VerticalAlignment.Top,
            Children =
            {
                saveButton,
                cancelButton
            }
        };
        Grid.SetRow(row, 2);
        return row;
    }

    private static Control CreateField(string label, Control input, int row)
    {
        var labelBlock = new TextBlock
        {
            Text = label,
            FontWeight = FontWeight.SemiBold
        };
        App.ApplyApplicationTextForeground(labelBlock);

        var field = new StackPanel
        {
            Spacing = 6,
            Children =
            {
                labelBlock,
                input
            }
        };
        Grid.SetRow(field, row);
        return field;
    }

    private static ComboBox CreateComboBox(string itemsSourceProperty, string selectedItemProperty)
    {
        var comboBox = new ComboBox
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            MinWidth = 360
        };
        comboBox.Bind(ItemsControl.ItemsSourceProperty, new Binding(itemsSourceProperty));
        comboBox.Bind(SelectingItemsControl.SelectedItemProperty, new Binding(selectedItemProperty)
        {
            Mode = BindingMode.TwoWay
        });
        return comboBox;
    }
}
