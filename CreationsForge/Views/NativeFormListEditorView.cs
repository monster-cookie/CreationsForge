using System.ComponentModel;
using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.NativeEditing;
using CreationsForge.NativeEditing.Drafts;
using CreationsForge.ViewModels;
using CreationsForge.Views.NativeEditing;

namespace CreationsForge.Views;

/// <summary>Presents revision-bound FormList sessions and closed typed command drafts.</summary>
public sealed class NativeFormListEditorView : UserControl
{
    /// <summary>The exact Starfield command used by the one-step typed Clear Components workflow.</summary>
    private const string ReplaceComponentsCommandName = "starfield.form-list.replace-components";

    /// <summary>The browser-owned editor workflow.</summary>
    private readonly NativeFormListEditorViewModel ViewModel;

    /// <summary>The command group selector populated from catalog presentation metadata.</summary>
    private readonly ComboBox CommandGroupSelector;

    /// <summary>The exact command selector filtered by the chosen group.</summary>
    private readonly ComboBox CommandSelector;

    /// <summary>The selected command's complete presentation description.</summary>
    private readonly TextBlock CommandDescription;

    /// <summary>The command-specific index input surface.</summary>
    private readonly StackPanel SeedSelectionPanel;

    /// <summary>The selected existing or insertion index.</summary>
    private readonly TextBox IndexInput;

    /// <summary>The move source index.</summary>
    private readonly TextBox SourceIndexInput;

    /// <summary>The move destination index.</summary>
    private readonly TextBox DestinationIndexInput;

    /// <summary>The local command-opening diagnostic.</summary>
    private readonly TextBlock CommandError;

    /// <summary>The Starfield-only action that clears a fully seeded Components replacement draft.</summary>
    private readonly Button ClearComponentsButton;

    /// <summary>The scroll host that receives the current typed draft tree.</summary>
    private readonly ScrollViewer DraftScroller;

    /// <summary>Whether this view is currently subscribed to editor state.</summary>
    private bool IsSubscribed;

    /// <summary>Initializes the native FormList editor view.</summary>
    /// <param name="viewModel">The browser-owned editor workflow.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="viewModel"/> is <see langword="null"/>.</exception>
    public NativeFormListEditorView(NativeFormListEditorViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        ViewModel = viewModel;
        DataContext = ViewModel;
        AutomationProperties.SetAutomationId(this, "NativeFormListEditorView");
        CommandGroupSelector = CreateCommandGroupSelector();
        CommandSelector = CreateCommandSelector();
        CommandDescription = CreateText(string.Empty, 11, FontWeight.Normal);
        CommandDescription.TextWrapping = TextWrapping.Wrap;
        AutomationProperties.SetAutomationId(CommandDescription, "NativeFormListCommandDescription");
        IndexInput = CreateIndexInput("NativeFormListCommandIndex", "Index");
        SourceIndexInput = CreateIndexInput("NativeFormListCommandSourceIndex", "Source index");
        DestinationIndexInput = CreateIndexInput("NativeFormListCommandDestinationIndex", "Destination index");
        SeedSelectionPanel = new StackPanel { Spacing = 6 };
        AutomationProperties.SetAutomationId(SeedSelectionPanel, "NativeFormListCommandSeedSelection");
        CommandError = CreateErrorText("NativeFormListCommandError");
        ClearComponentsButton = CreateClearComponentsButton();
        DraftScroller = new ScrollViewer
        {
            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto
        };
        DraftScroller.Bind(IsEnabledProperty, new Binding(nameof(NativeFormListEditorViewModel.CanMutateDraft)));
        AutomationProperties.SetAutomationId(DraftScroller, "NativeFormListTypedDraftScroller");
        Content = BuildContent();
        Subscribe();
        RefreshFromViewModel();
    }

    /// <summary>Restores editor-state observation when the retained view is attached again.</summary>
    /// <param name="eventArgs">The visual-tree attachment event.</param>
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs eventArgs)
    {
        base.OnAttachedToVisualTree(eventArgs);
        Subscribe();
        RefreshFromViewModel();
    }

    /// <summary>Releases editor-state observation while the view is detached.</summary>
    /// <param name="eventArgs">The visual-tree detachment event.</param>
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs eventArgs)
    {
        Unsubscribe();
        base.OnDetachedFromVisualTree(eventArgs);
    }

    /// <summary>Builds session actions, typed command selection, form content, and lifecycle diagnostics.</summary>
    /// <returns>The complete editor surface.</returns>
    private Control BuildContent()
    {
        var header = BuildSessionHeader();
        var command = BuildCommandSelector();
        var diagnostics = BuildDiagnostics();
        Grid.SetRow(header, 0);
        Grid.SetRow(command, 1);
        Grid.SetRow(DraftScroller, 2);
        Grid.SetRow(diagnostics, 3);
        return new Grid
        {
            RowDefinitions = new RowDefinitions("Auto,Auto,*,Auto"),
            RowSpacing = 10,
            Margin = new Thickness(12),
            Children =
            {
                header,
                command,
                DraftScroller,
                diagnostics
            }
        };
    }

    /// <summary>Builds explicit New, Override, and staged-output session actions and identity state.</summary>
    /// <returns>The session header.</returns>
    private Control BuildSessionHeader()
    {
        var beginNew = CreateCommandButton("New FormList", "NativeFormListBeginNewButton", nameof(NativeFormListEditorViewModel.NewCommand));
        var beginOverride = CreateCommandButton("Override selected", "NativeFormListBeginOverrideButton", nameof(NativeFormListEditorViewModel.OverrideCommand));
        var beginExisting = CreateCommandButton("Edit staged output", "NativeFormListBeginExistingOutputButton", nameof(NativeFormListEditorViewModel.ExistingOutputCommand));
        var actions = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 8,
            Children =
            {
                beginNew,
                beginOverride,
                beginExisting
            }
        };
        var identity = CreateBoundText(nameof(NativeFormListEditorViewModel.SessionIdentityText), 12, FontWeight.SemiBold);
        AutomationProperties.SetAutomationId(identity, "NativeFormListEditorSessionIdentity");
        var revision = CreateBoundText(nameof(NativeFormListEditorViewModel.SessionRevisionText), 11, FontWeight.Normal);
        revision.TextWrapping = TextWrapping.Wrap;
        AutomationProperties.SetAutomationId(revision, "NativeFormListEditorSessionRevision");
        return new StackPanel
        {
            Spacing = 6,
            Children =
            {
                actions,
                identity,
                revision,
                BuildStateIndicators()
            }
        };
    }

    /// <summary>Builds read-only active, local-dirty, staged-dirty, pending, and operation indicators.</summary>
    /// <returns>The editor state strip.</returns>
    private Control BuildStateIndicators()
    {
        var active = CreateIndicator("Active session", nameof(NativeFormListEditorViewModel.IsSessionActive), "NativeFormListEditorActiveIndicator");
        var draftDirty = CreateIndicator("Local draft changes", nameof(NativeFormListEditorViewModel.HasDraftChanges), "NativeFormListEditorDraftDirtyIndicator");
        var stagedDirty = CreateIndicator("Staged changes", nameof(NativeFormListEditorViewModel.HasStagedChanges), "NativeFormListEditorStagedDirtyIndicator");
        var stagedKnown = CreateIndicator("Staged state known", nameof(NativeFormListEditorViewModel.IsStagedChangesKnown), "NativeFormListEditorStagedKnownIndicator");
        var pending = CreateIndicator("Pending outcome", nameof(NativeFormListEditorViewModel.HasPendingOperation), "NativeFormListEditorPendingIndicator");
        var operation = CreateBoundText(nameof(NativeFormListEditorViewModel.OperationState), 11, FontWeight.Normal);
        AutomationProperties.SetAutomationId(operation, "NativeFormListEditorOperationState");
        return new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 12,
            Children =
            {
                active,
                draftDirty,
                stagedDirty,
                stagedKnown,
                pending,
                operation
            }
        };
    }

    /// <summary>Builds grouped command selection and closed seed-intent inputs.</summary>
    /// <returns>The command workflow.</returns>
    private Control BuildCommandSelector()
    {
        var open = new Button
        {
            Content = "Open action",
            Padding = new Thickness(12, 6),
            HorizontalAlignment = HorizontalAlignment.Left
        };
        open.Bind(IsEnabledProperty, new Binding(nameof(NativeFormListEditorViewModel.CanMutateDraft)));
        open.Click += (_, _) => OpenSelectedCommand();
        AutomationProperties.SetAutomationId(open, "NativeFormListOpenCommandButton");
        var commandActions = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 8,
            Children =
            {
                open,
                ClearComponentsButton
            }
        };
        var selectors = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("2*,3*"),
            ColumnSpacing = 8,
            Children =
            {
                CommandGroupSelector,
                CommandSelector
            }
        };
        Grid.SetColumn(CommandSelector, 1);
        return new Border
        {
            BorderBrush = App.GetApplicationBrush(App.BorderBrushKey),
            BorderThickness = new Thickness(0, 0, 0, 1),
            Padding = new Thickness(0, 0, 0, 10),
            Child = new StackPanel
            {
                Spacing = 7,
                Children =
                {
                    CreateText("Edit action", 14, FontWeight.SemiBold),
                    selectors,
                    CommandDescription,
                    SeedSelectionPanel,
                    commandActions,
                    CommandError
                }
            }
        };
    }

    /// <summary>Builds validation issues, warnings, banner errors, status, Apply, and exact retry actions.</summary>
    /// <returns>The editor diagnostics footer.</returns>
    private Control BuildDiagnostics()
    {
        var issues = new ItemsControl
        {
            ItemTemplate = new FuncDataTemplate<NativeWireDraftIssue>((issue, _) =>
                CreateWrappedText(issue is null ? string.Empty : $"{issue.Code} · {issue.Path}: {issue.Message}", 11))
        };
        issues.Bind(ItemsControl.ItemsSourceProperty, new Binding(nameof(NativeFormListEditorViewModel.ValidationIssues)));
        AutomationProperties.SetAutomationId(issues, "NativeFormListEditorValidationIssues");
        var warnings = new ItemsControl
        {
            ItemTemplate = new FuncDataTemplate<EngineWarning>((warning, _) =>
                CreateWrappedText(warning is null ? string.Empty : $"{warning.Code}: {warning.Message}", 11))
        };
        warnings.Bind(ItemsControl.ItemsSourceProperty, new Binding(nameof(NativeFormListEditorViewModel.Warnings)));
        AutomationProperties.SetAutomationId(warnings, "NativeFormListEditorWarnings");
        var details = new StackPanel
        {
            Spacing = 5,
            Children =
            {
                issues,
                warnings
            }
        };
        var detailScroller = new ScrollViewer
        {
            MaxHeight = 100,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            Content = details
        };
        AutomationProperties.SetAutomationId(detailScroller, "NativeFormListEditorDiagnosticsScroller");
        var error = CreateBoundText(nameof(NativeFormListEditorViewModel.ErrorMessage), 12, FontWeight.Normal);
        error.Foreground = new SolidColorBrush(Color.FromRgb(204, 73, 73));
        error.TextWrapping = TextWrapping.Wrap;
        error.Bind(IsVisibleProperty, new Binding(nameof(NativeFormListEditorViewModel.HasError)));
        AutomationProperties.SetAutomationId(error, "NativeFormListEditorErrorText");
        var status = CreateBoundText(nameof(NativeFormListEditorViewModel.StatusText), 12, FontWeight.Normal);
        status.TextWrapping = TextWrapping.Wrap;
        AutomationProperties.SetAutomationId(status, "NativeFormListEditorStatusText");
        var progress = new ProgressBar
        {
            IsIndeterminate = true,
            MinHeight = 4
        };
        progress.Bind(IsVisibleProperty, new Binding(nameof(NativeFormListEditorViewModel.IsBusy)));
        AutomationProperties.SetAutomationId(progress, "NativeFormListEditorProgress");
        var apply = CreateCommandButton("Apply to staged output", "NativeFormListEditorApplyButton", nameof(NativeFormListEditorViewModel.ApplyCommand));
        apply.Bind(IsEnabledProperty, new Binding(nameof(NativeFormListEditorViewModel.CanApply)));
        var retry = CreateCommandButton("Retry exact pending operation", "NativeFormListEditorRetryPendingButton", nameof(NativeFormListEditorViewModel.RetryPendingOperationCommand));
        retry.Bind(IsVisibleProperty, new Binding(nameof(NativeFormListEditorViewModel.HasPendingOperation)));
        var discard = CreateCommandButton("Discard form changes", "NativeFormListDiscardFormChangesButton", nameof(NativeFormListEditorViewModel.DiscardFormChangesCommand));
        discard.Bind(IsEnabledProperty, new Binding(nameof(NativeFormListEditorViewModel.CanDiscardFormChanges)));
        var discardHelp = CreateWrappedText("Discards unapplied form input only; staged changes remain.", 11);
        discardHelp.Opacity = 0.7;
        AutomationProperties.SetAutomationId(discardHelp, "NativeFormListDiscardFormChangesHelp");
        var actions = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 8,
            Children =
            {
                apply,
                retry,
                discard
            }
        };
        return new StackPanel
        {
            Spacing = 6,
            Children =
            {
                detailScroller,
                error,
                status,
                progress,
                actions,
                discardHelp
            }
        };
    }

    /// <summary>Creates the catalog command-group selector.</summary>
    /// <returns>The group selector.</returns>
    private ComboBox CreateCommandGroupSelector()
    {
        var selector = new ComboBox
        {
            PlaceholderText = "Command group",
            HorizontalAlignment = HorizontalAlignment.Stretch
        };
        selector.Bind(IsEnabledProperty, new Binding(nameof(NativeFormListEditorViewModel.CanMutateDraft)));
        selector.SelectionChanged += (_, _) => RefreshCommandsInSelectedGroup();
        AutomationProperties.SetAutomationId(selector, "NativeFormListCommandGroupSelector");
        return selector;
    }

    /// <summary>Creates the exact command selector.</summary>
    /// <returns>The command selector.</returns>
    private ComboBox CreateCommandSelector()
    {
        var selector = new ComboBox
        {
            PlaceholderText = "Choose a command",
            HorizontalAlignment = HorizontalAlignment.Stretch,
            MaxDropDownHeight = 300,
            ItemTemplate = new FuncDataTemplate<NativeFormListCommandPresentation>((command, _) =>
                CreateText(command is null ? string.Empty : command.DisplayName, 12, FontWeight.Normal))
        };
        selector.Bind(IsEnabledProperty, new Binding(nameof(NativeFormListEditorViewModel.CanMutateDraft)));
        selector.SelectionChanged += (_, _) => RefreshSeedSelection();
        AutomationProperties.SetAutomationId(selector, "NativeFormListCommandSelector");
        return selector;
    }

    /// <summary>Creates the Starfield-only typed Clear Components action.</summary>
    /// <returns>The catalog-gated quick action.</returns>
    private Button CreateClearComponentsButton()
    {
        var button = new Button
        {
            Content = "Clear Components",
            Padding = new Thickness(12, 6),
            HorizontalAlignment = HorizontalAlignment.Left,
            IsVisible = false
        };
        button.Bind(IsEnabledProperty, new Binding(nameof(NativeFormListEditorViewModel.CanMutateDraft)));
        button.Click += (_, _) => OpenAndClearComponents();
        AutomationProperties.SetAutomationId(button, "NativeFormListClearComponentsButton");
        return button;
    }

    /// <summary>Creates one canonical non-negative command-index input.</summary>
    /// <param name="automationId">The stable automation identity.</param>
    /// <param name="placeholder">The visible input hint.</param>
    /// <returns>The index text box.</returns>
    private static TextBox CreateIndexInput(string automationId, string placeholder)
    {
        var input = new TextBox
        {
            PlaceholderText = placeholder,
            MaxLength = 10,
            HorizontalAlignment = HorizontalAlignment.Stretch
        };
        AutomationProperties.SetAutomationId(input, automationId);
        return input;
    }

    /// <summary>Refreshes command groups from the exact active catalog.</summary>
    /// <param name="preferViewModelSelection">Whether the current editor command overrides a retained visual group selection.</param>
    private void RefreshCommandCatalog(bool preferViewModelSelection = false)
    {
        var currentGroup = CommandGroupSelector.SelectedItem as string;
        var selectedGroup = ViewModel.SelectedCommand?.GroupName;
        var groups = ViewModel.AvailableCommands
            .Select(command => command.GroupName)
            .Distinct(StringComparer.Ordinal)
            .ToArray();
        ClearComponentsButton.IsVisible = ViewModel.AvailableCommands.Any(command =>
            string.Equals(command.CommandName, ReplaceComponentsCommandName, StringComparison.Ordinal));
        CommandGroupSelector.ItemsSource = groups;
        CommandGroupSelector.SelectedItem = preferViewModelSelection
            ? groups.FirstOrDefault(group => string.Equals(group, selectedGroup, StringComparison.Ordinal))
                ?? groups.FirstOrDefault(group => string.Equals(group, currentGroup, StringComparison.Ordinal))
                ?? groups.FirstOrDefault()
            : groups.FirstOrDefault(group => string.Equals(group, currentGroup, StringComparison.Ordinal))
                ?? groups.FirstOrDefault(group => string.Equals(group, selectedGroup, StringComparison.Ordinal))
                ?? groups.FirstOrDefault();
        RefreshCommandsInSelectedGroup(preferViewModelSelection);
    }

    /// <summary>Refreshes commands without losing the active command when its group remains selected.</summary>
    /// <param name="preferViewModelSelection">Whether the current editor command overrides a retained visual selection.</param>
    private void RefreshCommandsInSelectedGroup(bool preferViewModelSelection = false)
    {
        var group = CommandGroupSelector.SelectedItem as string;
        var prior = CommandSelector.SelectedItem as NativeFormListCommandPresentation;
        var selectedCommand = ViewModel.SelectedCommand;
        var commands = ViewModel.AvailableCommands
            .Where(command => string.Equals(command.GroupName, group, StringComparison.Ordinal))
            .ToArray();
        CommandSelector.ItemsSource = commands;
        CommandSelector.SelectedItem = preferViewModelSelection
            ? commands.FirstOrDefault(command =>
                    string.Equals(command.CommandName, selectedCommand?.CommandName, StringComparison.Ordinal))
                ?? commands.FirstOrDefault(command =>
                    string.Equals(command.CommandName, prior?.CommandName, StringComparison.Ordinal))
                ?? commands.FirstOrDefault()
            : commands.FirstOrDefault(command =>
                    string.Equals(command.CommandName, prior?.CommandName, StringComparison.Ordinal))
                ?? commands.FirstOrDefault(command =>
                    string.Equals(command.CommandName, selectedCommand?.CommandName, StringComparison.Ordinal))
                ?? commands.FirstOrDefault();
        RefreshSeedSelection();
    }

    /// <summary>Shows only the closed seed-intent inputs required by the selected exact command.</summary>
    private void RefreshSeedSelection()
    {
        SeedSelectionPanel.Children.Clear();
        CommandError.Text = string.Empty;
        if (CommandSelector.SelectedItem is not NativeFormListCommandPresentation command)
        {
            CommandDescription.Text = string.Empty;
            return;
        }

        CommandDescription.Text = $"{command.GroupName} · {command.Description}";
        switch (GetSeedInputKind(command.CommandName))
        {
            case CommandSeedInputKind.AtIndex:
                SeedSelectionPanel.Children.Add(CreateText("Select the existing ordered value to seed completely.", 11, FontWeight.Normal));
                SeedSelectionPanel.Children.Add(IndexInput);
                break;
            case CommandSeedInputKind.InsertAt:
                SeedSelectionPanel.Children.Add(CreateText("Choose the ordered insertion position for the new typed value.", 11, FontWeight.Normal));
                SeedSelectionPanel.Children.Add(IndexInput);
                break;
            case CommandSeedInputKind.Move:
                SeedSelectionPanel.Children.Add(CreateText("Choose exact source and destination positions.", 11, FontWeight.Normal));
                SeedSelectionPanel.Children.Add(SourceIndexInput);
                SeedSelectionPanel.Children.Add(DestinationIndexInput);
                break;
            case CommandSeedInputKind.CurrentValue:
                SeedSelectionPanel.Children.Add(CreateText("Start with the current field value.", 11, FontWeight.Normal));
                break;
        }
    }

    /// <summary>Creates the selected typed command draft with a closed seed intent.</summary>
    private void OpenSelectedCommand()
    {
        if (CommandSelector.SelectedItem is not NativeFormListCommandPresentation command)
        {
            CommandError.Text = "Choose an exact command first.";
            return;
        }

        if (!TryCreateSeedSelection(command.CommandName, out var selection, out var error))
        {
            CommandError.Text = error;
            return;
        }

        var result = ViewModel.CreateDraft(command, selection!);
        CommandError.Text = result.Succeeded ? string.Empty : result.Error?.Message ?? "The typed command could not be opened.";
    }

    /// <summary>Opens the exact seeded Components replacement and clears its typed ordered array.</summary>
    private void OpenAndClearComponents()
    {
        var command = ViewModel.AvailableCommands.SingleOrDefault(candidate =>
            string.Equals(candidate.CommandName, ReplaceComponentsCommandName, StringComparison.Ordinal));
        if (command is null)
        {
            CommandError.Text = "The exact Starfield Replace Components command is unavailable in this session.";
            return;
        }

        var result = ViewModel.CreateDraft(command, NativeFormListDraftSeedSelection.CurrentValue());
        if (!result.Succeeded || result.Value is null)
        {
            CommandError.Text = result.Error?.Message ?? "The seeded Components replacement could not be opened.";
            return;
        }

        if (result.Value.Root is not NativeWireObjectDraftNode root
            || root.FindProperty("components") is not NativeWireArrayDraftNode components)
        {
            CommandError.Text = "The exact Replace Components draft did not expose its typed components array.";
            return;
        }

        components.Clear();
        CommandError.Text = string.Empty;
    }

    /// <summary>Builds the closed draft seed selection from canonical index text.</summary>
    /// <param name="commandName">The exact command discriminator.</param>
    /// <param name="selection">The closed seed selection on success.</param>
    /// <param name="error">The local validation message on failure.</param>
    /// <returns><see langword="true"/> when all required indexes are valid.</returns>
    private bool TryCreateSeedSelection(
        string commandName,
        out NativeFormListDraftSeedSelection? selection,
        out string error)
    {
        switch (GetSeedInputKind(commandName))
        {
            case CommandSeedInputKind.AtIndex:
                if (!TryParseIndex(IndexInput.Text, out var existingIndex))
                {
                    selection = null;
                    error = "Enter a non-negative existing index.";
                    return false;
                }

                selection = NativeFormListDraftSeedSelection.AtIndex(existingIndex);
                break;
            case CommandSeedInputKind.InsertAt:
                if (!TryParseIndex(IndexInput.Text, out var insertionIndex))
                {
                    selection = null;
                    error = "Enter a non-negative insertion index.";
                    return false;
                }

                selection = NativeFormListDraftSeedSelection.InsertAt(insertionIndex);
                break;
            case CommandSeedInputKind.Move:
                if (!TryParseIndex(SourceIndexInput.Text, out var sourceIndex)
                    || !TryParseIndex(DestinationIndexInput.Text, out var destinationIndex))
                {
                    selection = null;
                    error = "Enter non-negative source and destination indexes.";
                    return false;
                }

                selection = NativeFormListDraftSeedSelection.Move(sourceIndex, destinationIndex);
                break;
            default:
                selection = NativeFormListDraftSeedSelection.CurrentValue();
                break;
        }

        error = string.Empty;
        return true;
    }

    /// <summary>Gets the command's fixed seed-input intent without accepting a caller-provided path.</summary>
    /// <param name="commandName">The exact command discriminator.</param>
    /// <returns>The closed visual index-input kind.</returns>
    private static CommandSeedInputKind GetSeedInputKind(string commandName)
    {
        return commandName switch
        {
            "form-list.insert-item" or "starfield.form-list.add-component" => CommandSeedInputKind.InsertAt,
            "form-list.remove-item" or "starfield.form-list.remove-component" or "starfield.form-list.replace-component" => CommandSeedInputKind.AtIndex,
            "form-list.move-item" => CommandSeedInputKind.Move,
            _ => CommandSeedInputKind.CurrentValue
        };
    }

    /// <summary>Parses one canonical non-negative decimal index.</summary>
    /// <param name="text">The exact user text.</param>
    /// <param name="index">The parsed non-negative index.</param>
    /// <returns><see langword="true"/> when parsing succeeds without normalization.</returns>
    private static bool TryParseIndex(string? text, out int index)
    {
        return int.TryParse(text, System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out index)
            && index >= 0
            && string.Equals(index.ToString(System.Globalization.CultureInfo.InvariantCulture), text, StringComparison.Ordinal);
    }

    /// <summary>Rebuilds only the selected typed draft root after editor publication.</summary>
    private void RefreshDraft()
    {
        DraftScroller.Content = ViewModel.DraftRoot is null
            ? CreateEmptyDraftState()
            : NativeWireDraftControlFactory.Create(ViewModel.DraftRoot, node => ViewModel.PickFormLinkAsync(node));
    }

    /// <summary>Rebuilds manually projected controls and reevaluates bound state from one current editor snapshot.</summary>
    private void RefreshFromViewModel()
    {
        DataContext = null;
        DataContext = ViewModel;
        RefreshCommandCatalog(preferViewModelSelection: true);
        RefreshDraft();
    }

    /// <summary>Creates the no-draft guidance shown before a command is opened.</summary>
    /// <returns>The guidance control.</returns>
    private static Control CreateEmptyDraftState()
    {
        var text = CreateText("Begin a session, choose a command group, and open one typed command.", 12, FontWeight.Normal);
        text.TextWrapping = TextWrapping.Wrap;
        text.Margin = new Thickness(0, 8);
        AutomationProperties.SetAutomationId(text, "NativeFormListEditorNoDraftText");
        return text;
    }

    /// <summary>Observes editor publications needed for local visual reconstruction.</summary>
    private void Subscribe()
    {
        if (IsSubscribed)
        {
            return;
        }

        ViewModel.PropertyChanged += OnViewModelPropertyChanged;
        IsSubscribed = true;
    }

    /// <summary>Stops observing editor publications while detached.</summary>
    private void Unsubscribe()
    {
        if (!IsSubscribed)
        {
            return;
        }

        ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
        IsSubscribed = false;
    }

    /// <summary>Refreshes catalog or draft controls only for the corresponding editor publication.</summary>
    /// <param name="sender">The editor workflow.</param>
    /// <param name="eventArgs">The published property.</param>
    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs eventArgs)
    {
        if (string.Equals(eventArgs.PropertyName, nameof(NativeFormListEditorViewModel.AvailableCommands), StringComparison.Ordinal))
        {
            RefreshCommandCatalog();
        }
        else if (string.Equals(eventArgs.PropertyName, nameof(NativeFormListEditorViewModel.SelectedCommand), StringComparison.Ordinal)
            && ViewModel.SelectedCommand is { } selected)
        {
            CommandGroupSelector.SelectedItem = selected.GroupName;
            RefreshCommandsInSelectedGroup(preferViewModelSelection: true);
        }
        else if (string.Equals(eventArgs.PropertyName, nameof(NativeFormListEditorViewModel.DraftRoot), StringComparison.Ordinal)
            || string.Equals(eventArgs.PropertyName, nameof(NativeFormListEditorViewModel.Draft), StringComparison.Ordinal))
        {
            RefreshDraft();
        }
    }

    /// <summary>Creates one command-bound action button.</summary>
    /// <param name="content">The visible action text.</param>
    /// <param name="automationId">The stable automation identity.</param>
    /// <param name="commandProperty">The editor command property.</param>
    /// <returns>The command button.</returns>
    private Button CreateCommandButton(string content, string automationId, string commandProperty)
    {
        var button = new Button
        {
            Content = content,
            Padding = new Thickness(12, 6),
            HorizontalAlignment = HorizontalAlignment.Left
        };
        button.Bind(Button.CommandProperty, new Binding(commandProperty));
        AutomationProperties.SetAutomationId(button, automationId);
        return button;
    }

    /// <summary>Creates one disabled Boolean state indicator.</summary>
    /// <param name="content">The indicator label.</param>
    /// <param name="propertyName">The Boolean editor property.</param>
    /// <param name="automationId">The stable automation identity.</param>
    /// <returns>The state indicator.</returns>
    private CheckBox CreateIndicator(string content, string propertyName, string automationId)
    {
        var indicator = new CheckBox
        {
            Content = content,
            IsHitTestVisible = false,
            Focusable = false
        };
        indicator.Bind(ToggleButton.IsCheckedProperty, new Binding(propertyName));
        AutomationProperties.SetAutomationId(indicator, automationId);
        return indicator;
    }

    /// <summary>Creates application-styled text bound to one editor property.</summary>
    /// <param name="propertyName">The editor property.</param>
    /// <param name="fontSize">The font size.</param>
    /// <param name="fontWeight">The font weight.</param>
    /// <returns>The bound text block.</returns>
    private TextBlock CreateBoundText(string propertyName, double fontSize, FontWeight fontWeight)
    {
        var text = CreateText(string.Empty, fontSize, fontWeight);
        text.Bind(TextBlock.TextProperty, new Binding(propertyName));
        return text;
    }

    /// <summary>Creates one application-styled error text block.</summary>
    /// <param name="automationId">The stable automation identity.</param>
    /// <returns>The error text.</returns>
    private static TextBlock CreateErrorText(string automationId)
    {
        var error = CreateText(string.Empty, 11, FontWeight.Normal);
        error.Foreground = new SolidColorBrush(Color.FromRgb(204, 73, 73));
        error.TextWrapping = TextWrapping.Wrap;
        AutomationProperties.SetAutomationId(error, automationId);
        return error;
    }

    /// <summary>Creates application-styled wrapped text.</summary>
    /// <param name="text">The visible text.</param>
    /// <param name="fontSize">The font size.</param>
    /// <returns>The wrapped text.</returns>
    private static TextBlock CreateWrappedText(string text, double fontSize)
    {
        var block = CreateText(text, fontSize, FontWeight.Normal);
        block.TextWrapping = TextWrapping.Wrap;
        return block;
    }

    /// <summary>Creates application-styled text.</summary>
    /// <param name="text">The visible text.</param>
    /// <param name="fontSize">The font size.</param>
    /// <param name="fontWeight">The font weight.</param>
    /// <returns>The styled text block.</returns>
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

    /// <summary>Identifies the closed index input required by one exact command.</summary>
    private enum CommandSeedInputKind
    {
        /// <summary>The command consumes the complete current revision-bound value.</summary>
        CurrentValue,
        /// <summary>The command consumes one existing ordered value.</summary>
        AtIndex,
        /// <summary>The command creates one value at an explicit insertion position.</summary>
        InsertAt,
        /// <summary>The command moves one existing value between two explicit positions.</summary>
        Move
    }
}
