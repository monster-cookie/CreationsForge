using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Services;
using CreationsForge.ViewModels;

namespace CreationsForge.Views;

/// <summary>Presents bounded reference search, exact provenance, paging, and explicit selection actions.</summary>
public sealed class ReferencePickerView : UserControl
{
    /// <summary>The per-dialog reference search and resolution workflow.</summary>
    private readonly ReferencePickerViewModel ViewModel;

    /// <summary>Closes the owning modal with a resolved selection or cancellation.</summary>
    private readonly Action<ReferencePickerSelection?> CloseAction;

    /// <summary>Initializes the reference picker view.</summary>
    /// <param name="viewModel">The bounded plugin search and resolution workflow.</param>
    /// <param name="closeAction">The callback that closes the owning modal with the final result.</param>
    /// <exception cref="ArgumentNullException">Thrown when a required dependency is <see langword="null"/>.</exception>
    public ReferencePickerView(
        ReferencePickerViewModel viewModel,
        Action<ReferencePickerSelection?> closeAction)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        ArgumentNullException.ThrowIfNull(closeAction);
        ViewModel = viewModel;
        CloseAction = closeAction;
        DataContext = ViewModel;
        AutomationProperties.SetAutomationId(this, "ReferencePickerView");
        Content = BuildContent();
    }

    /// <summary>Builds the complete search, result, status, and action surface.</summary>
    /// <returns>The reference picker control tree.</returns>
    private Control BuildContent()
    {
        var body = new Grid
        {
            RowDefinitions = new RowDefinitions("Auto,Auto,*,Auto"),
            RowSpacing = 12,
            Margin = new Thickness(22),
            Children =
            {
                BuildContextHeader(),
                BuildSearchBar(),
                BuildResults(),
                BuildStatusPanel()
            }
        };
        Grid.SetRow(body.Children[1], 1);
        Grid.SetRow(body.Children[2], 2);
        Grid.SetRow(body.Children[3], 3);

        var footer = BuildFooter();
        Grid.SetRow(footer, 1);
        return new Grid
        {
            RowDefinitions = new RowDefinitions("*,Auto"),
            Children =
            {
                body,
                footer
            }
        };
    }

    /// <summary>Builds immutable selection purpose and plugin-context guidance.</summary>
    /// <returns>The picker context header.</returns>
    private Control BuildContextHeader()
    {
        var scopeText = ViewModel.ContainingModKey.HasValue
            ? $"{ViewModel.RecordScope}; containing plugin {ViewModel.ContainingModKey.Value.FileName}"
            : ViewModel.RecordScope.ToString();
        var currentText = ViewModel.CurrentFormKey.HasValue
            ? $" Current FormList: {ViewModel.CurrentFormKey.Value}."
            : string.Empty;
        return new StackPanel
        {
            Spacing = 5,
            Children =
            {
                CreateText("Select Plugin Reference", 21, FontWeight.SemiBold),
                CreateText(ViewModel.Purpose, 14, FontWeight.Normal),
                CreateText($"Scope: {scopeText}.{currentText}", 12, FontWeight.Normal)
            }
        };
    }

    /// <summary>Builds the major-record type filter, bounded query input, and first-page search action.</summary>
    /// <returns>The search toolbar.</returns>
    private Control BuildSearchBar()
    {
        var recordType = new ComboBox
        {
            MinWidth = 220,
            MinHeight = 34
        };
        recordType.Bind(ItemsControl.ItemsSourceProperty, new Binding(nameof(ReferencePickerViewModel.RecordTypes)));
        recordType.Bind(SelectingItemsControl.SelectedItemProperty, new Binding(nameof(ReferencePickerViewModel.SelectedRecordType))
        {
            Mode = BindingMode.TwoWay
        });
        AutomationProperties.SetName(recordType, "Major record type");
        AutomationProperties.SetAutomationId(recordType, "ReferencePickerRecordTypeFilter");

        var query = new TextBox
        {
            PlaceholderText = "FormKey or EditorID fragment",
            MaxLength = ReferenceSearchRequest.MaximumQueryLength,
            MinHeight = 34
        };
        query.Bind(TextBox.TextProperty, new Binding(nameof(ReferencePickerViewModel.Query))
        {
            Mode = BindingMode.TwoWay
        });
        AutomationProperties.SetAutomationId(query, "ReferencePickerQuery");

        var search = new Button
        {
            Content = "Search",
            MinWidth = 100,
            Padding = new Thickness(14, 7)
        };
        search.Bind(IsEnabledProperty, new Binding(nameof(ReferencePickerViewModel.CanSearch)));
        search.Click += async (_, _) => await ViewModel.SearchAsync();
        AutomationProperties.SetAutomationId(search, "ReferencePickerSearchButton");

        var panel = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("220,*,Auto"),
            ColumnSpacing = 8,
            Children =
            {
                recordType,
                query,
                search
            }
        };
        Grid.SetColumn(query, 1);
        Grid.SetColumn(search, 2);
        return panel;
    }

    /// <summary>Builds the visible bounded result page and explicit next-page action.</summary>
    /// <returns>The result list and paging controls.</returns>
    private Control BuildResults()
    {
        var results = new ListBox
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            MinHeight = 240,
            ItemTemplate = new FuncDataTemplate<ReferenceSearchMatch>(
                (match, _) => match is null ? new TextBlock() : BuildResultRow(match))
        };
        results.Bind(ItemsControl.ItemsSourceProperty, new Binding(nameof(ReferencePickerViewModel.Matches)));
        results.Bind(SelectingItemsControl.SelectedItemProperty, new Binding(nameof(ReferencePickerViewModel.SelectedMatch))
        {
            Mode = BindingMode.TwoWay
        });
        ScrollViewer.SetVerticalScrollBarVisibility(results, ScrollBarVisibility.Auto);
        ScrollViewer.SetHorizontalScrollBarVisibility(results, ScrollBarVisibility.Disabled);
        AutomationProperties.SetAutomationId(results, "ReferencePickerResults");

        var next = new Button
        {
            Content = "Next page",
            HorizontalAlignment = HorizontalAlignment.Right,
            MinWidth = 110,
            Padding = new Thickness(12, 6)
        };
        next.Bind(IsEnabledProperty, new Binding(nameof(ReferencePickerViewModel.CanLoadNextPage)));
        next.Click += async (_, _) => await ViewModel.LoadNextPageAsync();
        AutomationProperties.SetAutomationId(next, "ReferencePickerNextPageButton");
        Grid.SetRow(next, 1);

        return new Grid
        {
            RowDefinitions = new RowDefinitions("*,Auto"),
            RowSpacing = 8,
            Children =
            {
                new Border
                {
                    BorderBrush = App.GetApplicationBrush(App.BorderBrushKey),
                    BorderThickness = new Thickness(1),
                    Child = results
                },
                next
            }
        };
    }

    /// <summary>Builds one result row with record identity and complete containing-plugin provenance.</summary>
    /// <param name="match">The immutable search match to render.</param>
    /// <returns>The result row.</returns>
    private static Control BuildResultRow(ReferenceSearchMatch match)
    {
        var editorId = string.IsNullOrWhiteSpace(match.EditorId) ? "(no EditorID)" : match.EditorId;
        var containingMod = match.ContainingModKey?.FileName.ToString() ?? "(unknown plugin)";
        var sourcePath = string.IsNullOrWhiteSpace(match.SourcePath) ? "(source path unavailable)" : match.SourcePath;
        var loadOrder = match.LoadOrderIndex.HasValue ? match.LoadOrderIndex.Value.ToString() : "?";
        var role = match.Role?.ToString() ?? "Unknown role";
        var deletion = match.IsDeleted ? " | Deleted" : string.Empty;
        return new StackPanel
        {
            Margin = new Thickness(8, 6),
            Spacing = 3,
            Children =
            {
                CreateText($"{editorId} | {match.FormKey} | {match.RecordType}", 13, FontWeight.SemiBold),
                CreateText($"Containing plugin: {containingMod} | Load order: {loadOrder} | {role}{deletion}", 12, FontWeight.Normal),
                CreateText(sourcePath, 11, FontWeight.Normal)
            }
        };
    }

    /// <summary>Builds operation progress, status, and typed error feedback.</summary>
    /// <returns>The status feedback panel.</returns>
    private Control BuildStatusPanel()
    {
        var progress = new ProgressBar
        {
            IsIndeterminate = true,
            MinHeight = 4
        };
        progress.Bind(IsVisibleProperty, new Binding(nameof(ReferencePickerViewModel.IsBusy)));
        AutomationProperties.SetAutomationId(progress, "ReferencePickerProgress");

        var status = CreateText(string.Empty, 12, FontWeight.Normal);
        status.Bind(TextBlock.TextProperty, new Binding(nameof(ReferencePickerViewModel.StatusText)));
        AutomationProperties.SetAutomationId(status, "ReferencePickerStatusText");

        var error = CreateText(string.Empty, 12, FontWeight.Normal);
        error.Foreground = new SolidColorBrush(Color.FromRgb(204, 73, 73));
        error.Bind(TextBlock.TextProperty, new Binding(nameof(ReferencePickerViewModel.ErrorText)));
        error.Bind(IsVisibleProperty, new Binding(nameof(ReferencePickerViewModel.HasError)));
        AutomationProperties.SetAutomationId(error, "ReferencePickerErrorText");

        return new StackPanel
        {
            Spacing = 5,
            Children =
            {
                progress,
                status,
                error
            }
        };
    }

    /// <summary>Builds explicit null, cancellation, and resolved-selection actions.</summary>
    /// <returns>The dialog footer.</returns>
    private Control BuildFooter()
    {
        var selectNull = new Button
        {
            Content = "Select null",
            IsVisible = ViewModel.AllowNull,
            MinWidth = 110,
            Padding = new Thickness(14, 7)
        };
        selectNull.Bind(IsEnabledProperty, new Binding(nameof(ReferencePickerViewModel.CanSelectNull)));
        selectNull.Click += async (_, _) =>
        {
            var selection = await ViewModel.SelectNullAsync();
            if (selection is not null)
            {
                CloseAction(selection);
            }
        };
        AutomationProperties.SetAutomationId(selectNull, "ReferencePickerNullButton");

        var cancel = new Button
        {
            Content = "Cancel",
            MinWidth = 100,
            Padding = new Thickness(14, 7)
        };
        cancel.Click += (_, _) => CloseAction(null);
        AutomationProperties.SetAutomationId(cancel, "ReferencePickerCancelButton");

        var select = new Button
        {
            Content = "Select reference",
            MinWidth = 140,
            Padding = new Thickness(14, 7)
        };
        select.Bind(IsEnabledProperty, new Binding(nameof(ReferencePickerViewModel.CanConfirmSelection)));
        select.Click += async (_, _) =>
        {
            var selection = await ViewModel.ConfirmSelectionAsync();
            if (selection is not null)
            {
                CloseAction(selection);
            }
        };
        AutomationProperties.SetAutomationId(select, "ReferencePickerSelectButton");

        var footer = new Border
        {
            Background = App.GetApplicationBrush(App.PanelSurfaceBrushKey),
            BorderBrush = App.GetApplicationBrush(App.BorderBrushKey),
            BorderThickness = new Thickness(0, 1, 0, 0),
            Padding = new Thickness(18, 12),
            Child = new Grid
            {
                ColumnDefinitions = new ColumnDefinitions("Auto,*,Auto,Auto"),
                ColumnSpacing = 8,
                Children =
                {
                    selectNull,
                    cancel,
                    select
                }
            }
        };
        Grid.SetColumn(cancel, 2);
        Grid.SetColumn(select, 3);
        return footer;
    }

    /// <summary>Creates wrapping text with the application foreground brush.</summary>
    /// <param name="text">The initial text.</param>
    /// <param name="fontSize">The requested font size.</param>
    /// <param name="fontWeight">The requested font weight.</param>
    /// <returns>The styled text block.</returns>
    private static TextBlock CreateText(string text, double fontSize, FontWeight fontWeight)
    {
        var block = new TextBlock
        {
            Text = text,
            FontSize = fontSize,
            FontWeight = fontWeight,
            TextWrapping = TextWrapping.Wrap
        };
        App.ApplyApplicationTextForeground(block);
        return block;
    }
}
