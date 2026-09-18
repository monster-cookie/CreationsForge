using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.ViewModels;

namespace CreationsForge.Views;

/// <summary>Presents complete major-record families, context comparison, and record-tree authoring actions.</summary>
public sealed class MajorRecordBrowserView : UserControl
{
    /// <summary>The navigation-scope major-record browser workflow.</summary>
    private readonly MajorRecordBrowserViewModel ViewModel;

    /// <summary>Retains the proven FormList edit lifecycle until typed editing is available for other families.</summary>
    private readonly FormListBrowserViewModel FormListBrowser;

    /// <summary>The comparison surface shown for record selection.</summary>
    private Control? ComparisonPane;

    /// <summary>The editor surface shown after a supported tree action.</summary>
    private Control? EditorPane;

    /// <summary>The direct native float-setting form hosted beside the record tree.</summary>
    private GameSettingFloatEditorView? NativeEditor;

    /// <summary>The existing FormList editor hosted in the same record-selection pane.</summary>
    private Control? FormListEditorPane;

    /// <summary>Remembers which editor was active when comparison was opened.</summary>
    private bool NativeEditorSelected;

    /// <summary>Returns to an active editor after inspecting a comparison.</summary>
    private Button? ReturnToEditorButton;

    /// <summary>Reports a tree-action failure without terminating the UI.</summary>
    private TextBlock? ActionErrorText;

    /// <summary>The workspace whose editor navigation is currently displayed.</summary>
    private Guid? DisplayedWorkspaceId;

    /// <summary>Tracks whether initial loading started for this attached view.</summary>
    private bool Started;

    /// <summary>Initializes the major-record browser view.</summary>
    /// <param name="viewModel">The navigation-scope browser workflow.</param>
    /// <param name="formListBrowser">The navigation-scope FormList edit lifecycle.</param>
    /// <exception cref="ArgumentNullException">Thrown when a required dependency is <see langword="null"/>.</exception>
    public MajorRecordBrowserView(MajorRecordBrowserViewModel viewModel, FormListBrowserViewModel formListBrowser)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        ArgumentNullException.ThrowIfNull(formListBrowser);
        ViewModel = viewModel;
        FormListBrowser = formListBrowser;
        DisplayedWorkspaceId = FormListBrowser.CurrentWorkspaceId;
        DataContext = ViewModel;
        AutomationProperties.SetAutomationId(this, "MajorRecordBrowserView");
        Content = BuildContent();
        FormListBrowser.PropertyChanged += (_, eventArgs) =>
        {
            if (eventArgs.PropertyName == nameof(FormListBrowserViewModel.IsEditingWorkspace))
            {
                var currentWorkspaceId = FormListBrowser.CurrentWorkspaceId;
                if (currentWorkspaceId != DisplayedWorkspaceId)
                {
                    DisplayedWorkspaceId = currentWorkspaceId;
                    NativeEditor?.Clear();
                    ShowComparison(clearEditorNavigation: true);
                }
            }
        };
        FormListBrowser.Editor.StagedRecordChanged += formKey =>
        {
            _ = ViewModel.RefreshAsync();
        };
        FormListBrowser.PersistenceRefreshed += () =>
        {
            if (Started)
            {
                _ = ViewModel.RefreshAsync();
            }
        };
    }

    /// <summary>Gets the browser presentation state hosted by this view for shell-level status bindings.</summary>
    internal MajorRecordBrowserViewModel BrowserViewModel => ViewModel;

    /// <summary>Gets whether the record editor is currently displayed beside the tree.</summary>
    internal bool IsEditorOpen => EditorPane?.IsVisible == true;

    /// <summary>Starts record loading once this navigation-owned view enters the visual tree.</summary>
    /// <param name="eventArgs">The visual-tree attachment event.</param>
    protected override async void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs eventArgs)
    {
        base.OnAttachedToVisualTree(eventArgs);
        if (Started)
        {
            return;
        }

        Started = true;
        await Task.WhenAll(ViewModel.StartAsync(), FormListBrowser.StartAsync());
    }

    /// <summary>Builds the family list and comparison layout.</summary>
    /// <returns>The complete browser control tree.</returns>
    private Control BuildContent()
    {
        var records = BuildRecordPane();
        Grid.SetColumn(records, 0);
        ComparisonPane = BuildComparisonPane();
        EditorPane = BuildEditorPane();
        EditorPane.IsVisible = false;
        var details = new Grid { Children = { ComparisonPane, EditorPane } };
        Grid.SetColumn(details, 1);
        var content = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("3*,7*"),
            ColumnSpacing = 18,
            Children =
            {
                records,
                details
            }
        };
        AutomationProperties.SetAutomationId(content, "MajorRecordBrowserLayout");
        return content;
    }

    /// <summary>Builds complete family-grouped record discovery with visible loading progress.</summary>
    /// <returns>The record navigation pane.</returns>
    private Control BuildRecordPane()
    {
        var refresh = new Button
        {
            Content = "Refresh",
            Padding = new Thickness(12, 7)
        };
        refresh.Bind(Button.CommandProperty, new Binding(nameof(MajorRecordBrowserViewModel.RefreshCommand)));
        AutomationProperties.SetAutomationId(refresh, "MajorRecordRefreshButton");
        var count = CreateBoundText(nameof(MajorRecordBrowserViewModel.LoadedRecordCountText), 12, FontWeight.Normal);
        count.VerticalAlignment = VerticalAlignment.Center;
        AutomationProperties.SetAutomationId(count, "MajorRecordLoadedCountText");
        var sortLabel = CreateText("Sort records:", 12, FontWeight.Normal);
        sortLabel.VerticalAlignment = VerticalAlignment.Center;
        var sortSelector = new ComboBox
        {
            MinWidth = 100,
            ItemsSource = Enum.GetValues<MajorRecordSortMode>(),
            ItemTemplate = new FuncDataTemplate<MajorRecordSortMode>(
                (mode, _) => CreateText(mode == MajorRecordSortMode.FormId ? "FormID" : "EditorID", 12, FontWeight.Normal))
        };
        sortSelector.Bind(
            SelectingItemsControl.SelectedItemProperty,
            new Binding(nameof(MajorRecordBrowserViewModel.RecordSortMode)) { Mode = BindingMode.TwoWay });
        AutomationProperties.SetAutomationId(sortSelector, "MajorRecordSortSelector");
        var actions = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 8,
            Children =
            {
                refresh,
                count,
                sortLabel,
                sortSelector
            }
        };
        var filters = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("*,*"),
            ColumnSpacing = 8,
            Children =
            {
                CreateFilterTextBox(nameof(MajorRecordBrowserViewModel.FormIdFilter), "FormID", "MajorRecordFormIdFilter", 0),
                CreateFilterTextBox(nameof(MajorRecordBrowserViewModel.EditorIdFilter), "EditorID", "MajorRecordEditorIdFilter", 1)
            }
        };

        var tree = new TreeDataGrid
        {
            CanUserResizeColumns = true,
            CanUserSortColumns = false,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch
        };
        tree.Bind(TreeDataGrid.SourceProperty, new Binding(nameof(MajorRecordBrowserViewModel.RecordTreeSource)));
        tree.SelectionChanged += async (_, eventArgs) =>
        {
            var selected = eventArgs.SelectedItems.OfType<MajorRecordViewModel>().LastOrDefault();
            if (selected is not null)
            {
                await ViewModel.SelectRecordAsync(selected);
            }
        };
        tree.RowPrepared += (_, eventArgs) =>
        {
            var row = eventArgs.Row;
            var menu = new ContextMenu();
            menu.ItemsSource = BuildRecordMenuItems(row.Model as IRecordTreeNodeViewModel);
            menu.Opening += (_, _) => menu.ItemsSource = BuildRecordMenuItems(row.Model as IRecordTreeNodeViewModel);
            row.ContextMenu = menu;
        };
        AutomationProperties.SetAutomationId(tree, "MajorRecordTree");
        var progressText = CreateBoundText(nameof(MajorRecordBrowserViewModel.StatusText), 14, FontWeight.SemiBold);
        progressText.TextWrapping = TextWrapping.Wrap;
        AutomationProperties.SetAutomationId(progressText, "MajorRecordLoadingStatus");
        var loading = new Border
        {
            Background = App.GetApplicationBrush(App.PanelSurfaceBrushKey),
            Child = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Spacing = 12,
                Children =
                {
                    progressText,
                    new ProgressBar { IsIndeterminate = true, MinWidth = 280, MinHeight = 4 }
                }
            }
        };
        loading.Bind(IsVisibleProperty, new Binding(nameof(MajorRecordBrowserViewModel.IsBusy)));
        AutomationProperties.SetAutomationId(loading, "MajorRecordLoadingScreen");
        var treeArea = new Grid { Children = { tree, loading } };
        Grid.SetRow(actions, 1);
        Grid.SetRow(filters, 2);
        Grid.SetRow(treeArea, 3);
        return new Border
        {
            Background = App.GetApplicationBrush(App.PanelSurfaceBrushKey),
            BorderBrush = App.GetApplicationBrush(App.BorderBrushKey),
            BorderThickness = new Thickness(0, 0, 1, 0),
            Padding = new Thickness(12),
            Child = new Grid
            {
                RowDefinitions = new RowDefinitions("Auto,Auto,Auto,*"),
                RowSpacing = 10,
                Children =
                {
                    CreateText("All Major Records", 18, FontWeight.SemiBold),
                    actions,
                    filters,
                    treeArea
                }
            }
        };
    }

    /// <summary>Builds actions for the exact family or record under the pointer, including explicit unsupported states.</summary>
    /// <param name="node">The realized record-tree node.</param>
    /// <returns>Menu actions whose enabled state reflects the current workspace and editor lifecycle.</returns>
    internal IReadOnlyList<MenuItem> BuildRecordMenuItems(IRecordTreeNodeViewModel? node)
    {
        var recordType = node switch
        {
            MajorRecordViewModel record => record.RecordType,
            RecordTypeGroupViewModel group => group.RecordType,
            _ => null
        };
        if (recordType is null)
        {
            return Array.Empty<MenuItem>();
        }

        var hasTypedEditor = recordType is "FormList" or "GameSettingFloat";
        var isNativeEditor = recordType == "GameSettingFloat";
        var canEdit = FormListBrowser.IsEditingWorkspace && hasTypedEditor;
        var unavailableReason = !hasTypedEditor
            ? "Typed authoring for this record type is not available yet."
            : "Open a plugin for editing to create or override records.";
        var newItem = new MenuItem
        {
            Header = $"New {recordType}",
            IsEnabled = canEdit && (isNativeEditor || FormListBrowser.Editor.CanBeginNew)
        };
        ToolTip.SetTip(newItem, canEdit ? "Create a new record in the active plugin." : unavailableReason);
        newItem.Click += async (_, _) => await BeginNewAsync(recordType);
        AutomationProperties.SetAutomationId(newItem, "MajorRecordNewMenuItem");
        if (node is not MajorRecordViewModel selected)
        {
            return new[] { newItem };
        }

        var isOutput = selected.Role == PluginRole.Output;
        var overrideItem = new MenuItem
        {
            Header = isOutput ? "Edit in active plugin" : "Create override",
            IsEnabled = canEdit && (isNativeEditor || (isOutput
                ? FormListBrowser.Editor.CanBeginExistingOutput || FormListBrowser.Editor.CanBeginNew
                : FormListBrowser.Editor.CanBeginNew))
        };
        ToolTip.SetTip(overrideItem, canEdit ? "Edit this exact record in the active plugin." : unavailableReason);
        overrideItem.Click += async (_, _) => await BeginOverrideAsync(selected);
        AutomationProperties.SetAutomationId(overrideItem, "MajorRecordOverrideMenuItem");
        return new[] { newItem, overrideItem };
    }

    /// <summary>Starts a supported new-record edit from a record-family tree action.</summary>
    /// <param name="recordType">The canonical family chosen by the user.</param>
    /// <returns>A task that completes after the editor opens or reports a failure.</returns>
    private async Task BeginNewAsync(string recordType)
    {
        if (!FormListBrowser.IsEditingWorkspace ||
            recordType is not ("FormList" or "GameSettingFloat"))
        {
            return;
        }

        if (recordType == "GameSettingFloat")
        {
            ShowEditor(native: true);
            await NativeEditor!.BeginAsync(null);
            return;
        }

        if (!FormListBrowser.Editor.CanBeginNew)
        {
            return;
        }

        ShowEditor(native: false);
        try
        {
            await FormListBrowser.StartAsync();
            await FormListBrowser.Editor.BeginNewAsync();
        }
        catch (Exception exception)
        {
            ShowActionError($"Could not start a new {recordType}: {exception.Message}");
        }
    }

    /// <summary>Starts an override or output edit for the exact right-clicked FormList row.</summary>
    /// <param name="record">The row under the pointer when the menu opened.</param>
    /// <returns>A task that completes after the editor opens or reports a failure.</returns>
    private async Task BeginOverrideAsync(MajorRecordViewModel record)
    {
        if (!FormListBrowser.IsEditingWorkspace ||
            record.RecordType is not ("FormList" or "GameSettingFloat"))
        {
            return;
        }

        ShowEditor(native: record.RecordType == "GameSettingFloat");
        if (!ViewModel.Records.Any(candidate => ReferenceEquals(candidate, record)))
        {
            ShowActionError("The selected record belongs to an older record tree. Refresh and select it again.");
            return;
        }

        if (record.RecordType == "GameSettingFloat")
        {
            await NativeEditor!.BeginAsync(record);
            return;
        }

        if (!FormListBrowser.Editor.CanBeginNew)
        {
            return;
        }

        try
        {
            await FormListBrowser.StartAsync();
            await FormListBrowser.RefreshAsync(record.FormKey);
            var root = FormListBrowser.Records.FirstOrDefault(candidate => candidate.FormKey == record.FormKey);
            var context = root?.Contexts.LastOrDefault(candidate =>
                candidate.ContainingModKey == record.ContainingModKey &&
                candidate.Context.LoadOrderIndex == record.LoadOrderIndex);
            if (context is null)
            {
                ShowActionError("This record is no longer available in the active workspace. Refresh the record tree and try again.");
                return;
            }

            await FormListBrowser.SelectRecordAsync(context);
            if (context.Context.Role == PluginRole.Output)
            {
                await FormListBrowser.Editor.BeginExistingOutputAsync();
            }
            else
            {
                await FormListBrowser.Editor.BeginOverrideAsync();
            }
        }
        catch (Exception exception)
        {
            ShowActionError($"Could not edit {record.FormKey}: {exception.Message}");
        }
    }

    /// <summary>Displays the editor beside the record tree while preserving the current comparison.</summary>
    internal void ShowEditor(bool? native = null)
    {
        if (ComparisonPane is null || EditorPane is null)
        {
            return;
        }

        ComparisonPane.IsVisible = false;
        EditorPane.IsVisible = true;
        NativeEditorSelected = native ?? NativeEditorSelected;
        if (NativeEditor is not null)
        {
            NativeEditor.IsVisible = NativeEditorSelected;
        }

        if (FormListEditorPane is not null)
        {
            FormListEditorPane.IsVisible = !NativeEditorSelected;
        }
        if (ReturnToEditorButton is not null)
        {
            ReturnToEditorButton.IsVisible = true;
        }

        ShowActionError(string.Empty);
    }

    /// <summary>Returns to comparison and optionally clears navigation to an editor from a previous workspace.</summary>
    /// <param name="clearEditorNavigation">Whether the active edit shortcut must be removed.</param>
    private void ShowComparison(bool clearEditorNavigation = false)
    {
        if (ComparisonPane is not null && EditorPane is not null)
        {
            EditorPane.IsVisible = false;
            ComparisonPane.IsVisible = true;
        }

        if (clearEditorNavigation && ReturnToEditorButton is not null)
        {
            ReturnToEditorButton.IsVisible = false;
        }
    }

    /// <summary>Displays a context-action error in the editor pane.</summary>
    /// <param name="message">The user-facing error, or empty text to clear it.</param>
    private void ShowActionError(string message)
    {
        if (ActionErrorText is not null)
        {
            ActionErrorText.Text = message;
            ActionErrorText.IsVisible = !string.IsNullOrEmpty(message);
        }
    }

    /// <summary>Builds the editor as a sibling of comparison within the same record-selection screen.</summary>
    /// <returns>The edit pane and its comparison navigation.</returns>
    private Control BuildEditorPane()
    {
        var compare = new Button { Content = "Back to comparison", Padding = new Thickness(12, 6) };
        compare.Click += (_, _) => ShowComparison();
        AutomationProperties.SetAutomationId(compare, "MajorRecordBackToComparisonButton");
        ActionErrorText = CreateText(string.Empty, 12, FontWeight.Normal);
        ActionErrorText.Foreground = new SolidColorBrush(Color.FromRgb(204, 73, 73));
        ActionErrorText.IsVisible = false;
        var header = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 12,
            Children = { compare, ActionErrorText }
        };
        FormListEditorPane = new FormListEditorView(FormListBrowser.Editor, showBeginActions: false);
        NativeEditor = new GameSettingFloatEditorView(ViewModel) { IsVisible = false };
        var editors = new Grid { Children = { FormListEditorPane, NativeEditor } };
        Grid.SetRow(editors, 1);
        return new Grid
        {
            RowDefinitions = new RowDefinitions("Auto,*"),
            RowSpacing = 8,
            Children = { header, editors }
        };
    }

    /// <summary>Creates a two-way metadata filter for the complete major-record tree.</summary>
    /// <param name="propertyName">The browser filter property.</param>
    /// <param name="placeholder">The visible filter hint.</param>
    /// <param name="automationId">The stable control identity.</param>
    /// <param name="column">The filter grid column.</param>
    /// <returns>The bound text box.</returns>
    private static TextBox CreateFilterTextBox(string propertyName, string placeholder, string automationId, int column)
    {
        var filter = new TextBox { PlaceholderText = placeholder };
        filter.Bind(TextBox.TextProperty, new Binding(propertyName) { Mode = BindingMode.TwoWay });
        AutomationProperties.SetAutomationId(filter, automationId);
        Grid.SetColumn(filter, column);
        return filter;
    }

    /// <summary>Builds exact context selectors, complete field trees, changes, warnings, and progress.</summary>
    /// <returns>The comparison pane.</returns>
    private Control BuildComparisonPane()
    {
        var selectors = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("*,*"),
            ColumnSpacing = 12,
            Children =
            {
                BuildContextSelector(
                    "Prior context",
                    nameof(MajorRecordBrowserViewModel.SelectedBeforeContext),
                    nameof(MajorRecordBrowserViewModel.BeforeProvenanceText),
                    "MajorRecordBeforeContextSelector",
                    ViewModel.SelectBeforeContextAsync,
                    0),
                BuildContextSelector(
                    "Resulting context",
                    nameof(MajorRecordBrowserViewModel.SelectedAfterContext),
                    nameof(MajorRecordBrowserViewModel.AfterProvenanceText),
                    "MajorRecordAfterContextSelector",
                    ViewModel.SelectAfterContextAsync,
                    1)
            }
        };
        var fields = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("*,*"),
            ColumnSpacing = 12,
            Children =
            {
                BuildFieldPane("Prior fields", nameof(MajorRecordBrowserViewModel.BeforeFieldSource), "MajorRecordBeforeFieldTree", 0),
                BuildFieldPane("Resulting fields", nameof(MajorRecordBrowserViewModel.AfterFieldSource), "MajorRecordAfterFieldTree", 1)
            }
        };
        var diagnostics = BuildDiagnosticsPane();
        Grid.SetRow(selectors, 0);
        Grid.SetRow(fields, 1);
        Grid.SetRow(diagnostics, 2);
        var body = new Grid
        {
            RowDefinitions = new RowDefinitions("Auto,*,Auto"),
            RowSpacing = 12,
            Children =
            {
                selectors,
                fields,
                diagnostics
            }
        };

        var progress = new ProgressBar
        {
            IsIndeterminate = true,
            MinWidth = 280,
            MinHeight = 4
        };
        var loading = new Border
        {
            Background = App.GetApplicationBrush(App.PanelSurfaceBrushKey),
            Child = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Spacing = 12,
                Children =
                {
                    CreateText("Resolving native record fields...", 16, FontWeight.SemiBold),
                    progress
                }
            }
        };
        loading.Bind(IsVisibleProperty, new Binding(nameof(MajorRecordBrowserViewModel.IsComparisonBusy)));
        AutomationProperties.SetAutomationId(loading, "MajorRecordComparisonLoadingView");
        var content = new Grid
        {
            Children =
            {
                body,
                loading
            }
        };
        ReturnToEditorButton = new Button
        {
            Content = "Return to edit",
            Padding = new Thickness(12, 6),
            HorizontalAlignment = HorizontalAlignment.Left,
            IsVisible = false
        };
        ReturnToEditorButton.Click += (_, _) => ShowEditor();
        AutomationProperties.SetAutomationId(ReturnToEditorButton, "MajorRecordReturnToEditorButton");
        Grid.SetRow(content, 1);
        return new Grid
        {
            RowDefinitions = new RowDefinitions("Auto,*"),
            RowSpacing = 8,
            Children = { ReturnToEditorButton, content }
        };
    }

    /// <summary>Builds one exact-context selector and provenance label.</summary>
    /// <param name="title">The selector title.</param>
    /// <param name="selectedProperty">The selected context property.</param>
    /// <param name="provenanceProperty">The provenance text property.</param>
    /// <param name="automationId">The stable selector automation identity.</param>
    /// <param name="selectionAction">The asynchronous selection workflow.</param>
    /// <param name="column">The parent grid column.</param>
    /// <returns>The configured selector panel.</returns>
    private Control BuildContextSelector(
        string title,
        string selectedProperty,
        string provenanceProperty,
        string automationId,
        Func<MajorRecordContextOption?, Task> selectionAction,
        int column)
    {
        var selector = new ComboBox
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            MaxDropDownHeight = 260,
            ItemTemplate = new FuncDataTemplate<MajorRecordContextOption>(
                (option, _) => new TextBlock { Text = option?.Label ?? string.Empty })
        };
        selector.Bind(ItemsControl.ItemsSourceProperty, new Binding(nameof(MajorRecordBrowserViewModel.ContextOptions)));
        selector.Bind(SelectingItemsControl.SelectedItemProperty, new Binding(selectedProperty) { Mode = BindingMode.OneWay });
        selector.SelectionChanged += async (_, _) => await selectionAction(selector.SelectedItem as MajorRecordContextOption);
        AutomationProperties.SetAutomationId(selector, automationId);
        var provenance = CreateBoundText(provenanceProperty, 12, FontWeight.Normal);
        provenance.TextWrapping = TextWrapping.Wrap;
        AutomationProperties.SetAutomationId(provenance, automationId + "Provenance");
        var panel = new StackPanel
        {
            Spacing = 6,
            Children =
            {
                CreateText(title, 14, FontWeight.SemiBold),
                selector,
                provenance
            }
        };
        Grid.SetColumn(panel, column);
        return panel;
    }

    /// <summary>Builds one independent hierarchical field pane.</summary>
    /// <param name="title">The field-pane title.</param>
    /// <param name="sourceProperty">The hierarchical source property.</param>
    /// <param name="automationId">The stable tree automation identity.</param>
    /// <param name="column">The parent grid column.</param>
    /// <returns>The configured field pane.</returns>
    private static Control BuildFieldPane(string title, string sourceProperty, string automationId, int column)
    {
        var tree = new TreeDataGrid
        {
            CanUserResizeColumns = true,
            CanUserSortColumns = false,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch
        };
        tree.Bind(TreeDataGrid.SourceProperty, new Binding(sourceProperty));
        AutomationProperties.SetAutomationId(tree, automationId);
        Grid.SetRow(tree, 1);
        var pane = new Grid
        {
            RowDefinitions = new RowDefinitions("Auto,*"),
            RowSpacing = 6,
            Children =
            {
                CreateText(title, 14, FontWeight.SemiBold),
                tree
            }
        };
        Grid.SetColumn(pane, column);
        return pane;
    }

    /// <summary>Builds warnings, typed errors, retry, and status without duplicating field-level comparison details.</summary>
    /// <returns>The complete diagnostics pane.</returns>
    private Control BuildDiagnosticsPane()
    {
        var warnings = new ItemsControl
        {
            ItemTemplate = new FuncDataTemplate<EngineWarning>(
                (warning, _) => CreateWrappedText(
                    warning is null ? string.Empty : $"{warning.Code}: {warning.Message}",
                    12))
        };
        warnings.Bind(ItemsControl.ItemsSourceProperty, new Binding(nameof(MajorRecordBrowserViewModel.Warnings)));
        var warningDetails = new StackPanel
        {
            Spacing = 6,
            Children =
            {
                CreateText("Warnings", 13, FontWeight.SemiBold),
                warnings
            }
        };
        AutomationProperties.SetAutomationId(warningDetails, "MajorRecordWarnings");
        var scroller = new ScrollViewer
        {
            MaxHeight = 130,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            Content = warningDetails
        };
        scroller.Bind(IsVisibleProperty, new Binding(nameof(MajorRecordBrowserViewModel.HasWarnings)));
        var error = CreateBoundText(nameof(MajorRecordBrowserViewModel.ErrorMessage), 12, FontWeight.Normal);
        error.TextWrapping = TextWrapping.Wrap;
        var retry = new Button
        {
            Content = "Retry",
            Padding = new Thickness(12, 6)
        };
        retry.Bind(Button.CommandProperty, new Binding(nameof(MajorRecordBrowserViewModel.RetryCommand)));
        var errorPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 8,
            Children =
            {
                error,
                retry
            }
        };
        errorPanel.Bind(IsVisibleProperty, new Binding(nameof(MajorRecordBrowserViewModel.HasError)));
        var status = CreateBoundText(nameof(MajorRecordBrowserViewModel.StatusText), 12, FontWeight.Normal);
        status.TextWrapping = TextWrapping.Wrap;
        status.Bind(IsVisibleProperty, new Binding(nameof(MajorRecordBrowserViewModel.HasStatusText)));
        AutomationProperties.SetAutomationId(status, "MajorRecordBrowserStatusText");
        return new StackPanel
        {
            Spacing = 6,
            Children =
            {
                scroller,
                errorPanel,
                status
            }
        };
    }

    /// <summary>Creates unbound application-styled text.</summary>
    /// <param name="text">The visible text.</param>
    /// <param name="fontSize">The font size.</param>
    /// <param name="fontWeight">The font weight.</param>
    /// <returns>The configured text block.</returns>
    private static TextBlock CreateText(string text, double fontSize, FontWeight fontWeight)
    {
        var block = new TextBlock
        {
            Text = text,
            FontSize = fontSize,
            FontWeight = fontWeight
        };
        App.ApplyApplicationTextForeground(block);
        return block;
    }

    /// <summary>Creates application-styled text bound to one view-model property.</summary>
    /// <param name="propertyName">The bound property.</param>
    /// <param name="fontSize">The font size.</param>
    /// <param name="fontWeight">The font weight.</param>
    /// <returns>The configured text block.</returns>
    private static TextBlock CreateBoundText(string propertyName, double fontSize, FontWeight fontWeight)
    {
        var block = new TextBlock
        {
            FontSize = fontSize,
            FontWeight = fontWeight
        };
        block.Bind(TextBlock.TextProperty, new Binding(propertyName));
        App.ApplyApplicationTextForeground(block);
        return block;
    }

    /// <summary>Creates wrapped application-styled text.</summary>
    /// <param name="text">The visible text.</param>
    /// <param name="fontSize">The font size.</param>
    /// <returns>The configured text block.</returns>
    private static TextBlock CreateWrappedText(string text, double fontSize)
    {
        var block = CreateText(text, fontSize, FontWeight.Normal);
        block.TextWrapping = TextWrapping.Wrap;
        return block;
    }
}
