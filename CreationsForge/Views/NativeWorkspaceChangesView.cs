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

/// <summary>Presents detached native changes, active-editor drain decisions, exact persistence state, recovery actions, and closed leave choices.</summary>
public sealed class NativeWorkspaceChangesView : UserControl
{
    /// <summary>The navigation-scope change lifecycle displayed by this view.</summary>
    private readonly NativeWorkspaceChangesViewModel ViewModel;

    /// <summary>The immutable closed purpose for this modal session.</summary>
    private readonly NativeWorkspaceChangesDialogRequest Request;

    /// <summary>Forwards one closed final choice through the change lifecycle state machine.</summary>
    private readonly Func<NativeWorkspaceChangesDialogChoice, Task> ApplyChoiceAsync;

    /// <summary>The non-destructive title that receives initial dialog focus.</summary>
    private TextBlock? TitleText;

    /// <summary>Initializes the shared native workspace changes surface.</summary>
    /// <param name="viewModel">The change lifecycle and exact persistence state.</param>
    /// <param name="request">The immutable dialog purpose.</param>
    /// <param name="applyChoiceAsync">The awaited closed-choice callback owned by the dialog service.</param>
    /// <exception cref="ArgumentNullException">Thrown when a required dependency is <see langword="null"/>.</exception>
    internal NativeWorkspaceChangesView(
        NativeWorkspaceChangesViewModel viewModel,
        NativeWorkspaceChangesDialogRequest request,
        Func<NativeWorkspaceChangesDialogChoice, Task> applyChoiceAsync)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(applyChoiceAsync);
        ViewModel = viewModel;
        Request = request;
        ApplyChoiceAsync = applyChoiceAsync;
        DataContext = ViewModel;
        AutomationProperties.SetAutomationId(this, "NativeWorkspaceChangesView");
        Content = BuildContent();
    }

    /// <summary>Moves keyboard and screen-reader focus to the non-destructive dialog heading.</summary>
    /// <param name="eventArgs">The visual-tree attachment event.</param>
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs eventArgs)
    {
        base.OnAttachedToVisualTree(eventArgs);
        TitleText?.Focus();
    }

    /// <summary>Builds the review scroller, persistence state, recovery actions, and final action footer.</summary>
    /// <returns>The complete changes control tree.</returns>
    private Control BuildContent()
    {
        var review = new StackPanel
        {
            Spacing = 14,
            Children =
            {
                BuildHeader(),
                BuildDraftWarning(),
                BuildReviewSummary(),
                BuildChangeItems(),
                BuildWarnings(),
                BuildPersistenceStatus(),
                BuildRecoveryActions()
            }
        };
        var scrollViewer = new ScrollViewer
        {
            Content = review,
            Padding = new Thickness(24),
            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto
        };
        AutomationProperties.SetAutomationId(scrollViewer, "NativeWorkspaceChangesScroller");
        Grid.SetRow(scrollViewer, 0);
        var footer = BuildFooter();
        Grid.SetRow(footer, 1);
        return new Grid
        {
            RowDefinitions = new RowDefinitions("*,Auto"),
            Children =
            {
                scrollViewer,
                footer
            }
        };
    }

    /// <summary>Builds the fixed dialog title and purpose-specific leave explanation.</summary>
    /// <returns>The heading panel.</returns>
    private Control BuildHeader()
    {
        TitleText = CreateText("Review Changes", 22, FontWeight.SemiBold);
        TitleText.Focusable = true;
        AutomationProperties.SetAutomationId(TitleText, "NativeWorkspaceChangesTitle");
        AutomationProperties.SetName(TitleText, "Review Changes");
        var purpose = CreateText(CreatePurposeText(Request), 13, FontWeight.Normal);
        purpose.TextWrapping = TextWrapping.Wrap;
        AutomationProperties.SetAutomationId(purpose, "NativeWorkspaceChangesPurposeText");
        return new StackPanel
        {
            Spacing = 6,
            Children =
            {
                TitleText,
                purpose
            }
        };
    }

    /// <summary>Builds the visible warning that request-local form input is outside workspace save until applied.</summary>
    /// <returns>The bound unapplied-form warning.</returns>
    private Control BuildDraftWarning()
    {
        var warning = CreateText(
            "Form changes not applied. Apply them in the editor before saving, or explicitly discard the form changes.",
            13,
            FontWeight.SemiBold);
        warning.TextWrapping = TextWrapping.Wrap;
        warning.Bind(IsVisibleProperty, new Binding(nameof(NativeWorkspaceChangesViewModel.HasDraftChanges)));
        AutomationProperties.SetAutomationId(warning, "NativeWorkspaceUnappliedDraftWarning");
        return warning;
    }

    /// <summary>Builds authoritative staged-change and unresolved-reference summary text.</summary>
    /// <returns>The review summary panel.</returns>
    private Control BuildReviewSummary()
    {
        var heading = CreateText("Workspace changes", 17, FontWeight.SemiBold);
        var unresolved = CreateText(string.Empty, 13, FontWeight.Normal);
        unresolved.Bind(TextBlock.TextProperty, new Binding("CurrentReview.UnresolvedReferenceCount")
        {
            StringFormat = "Unresolved references: {0}"
        });
        AutomationProperties.SetAutomationId(unresolved, "NativeWorkspaceUnresolvedReferenceCount");
        var stale = CreateText(
            "This review is stale history and cannot be used to create a save or discard request.",
            13,
            FontWeight.SemiBold);
        stale.TextWrapping = TextWrapping.Wrap;
        stale.Bind(IsVisibleProperty, new Binding(nameof(NativeWorkspaceChangesViewModel.IsReviewStale)));
        AutomationProperties.SetAutomationId(stale, "NativeWorkspaceStaleReviewWarning");
        return new StackPanel
        {
            Spacing = 5,
            Children =
            {
                heading,
                unresolved,
                stale
            }
        };
    }

    /// <summary>Builds the scrollable per-FormList comparison collection.</summary>
    /// <returns>The bound record comparison list.</returns>
    private Control BuildChangeItems()
    {
        var items = new ItemsControl
        {
            ItemTemplate = new FuncDataTemplate<NativeWorkspaceChangeItemViewModel>(
                (item, _) => item is null ? new Border() : BuildChangeItem(item))
        };
        items.Bind(ItemsControl.ItemsSourceProperty, new Binding("CurrentReview.Items"));
        AutomationProperties.SetAutomationId(items, "NativeWorkspaceChangeItems");
        return items;
    }

    /// <summary>Builds one expandable detached before-and-after FormList comparison.</summary>
    /// <param name="item">The immutable projected comparison.</param>
    /// <returns>The configured comparison card.</returns>
    private static Control BuildChangeItem(NativeWorkspaceChangeItemViewModel item)
    {
        var stableId = CreateStableFormKeyId(item.FormKey.ToString());
        var contexts = CreateText(
            $"Prior: {FormatContext(item.BeforeContext)}{Environment.NewLine}Result: {FormatContext(item.AfterContext)}",
            12,
            FontWeight.Normal);
        contexts.TextWrapping = TextWrapping.Wrap;
        var trees = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("*,*"),
            ColumnSpacing = 12,
            Children =
            {
                BuildFieldPane("Prior fields", item.BeforeFields, $"NativeWorkspaceChange_{stableId}_PriorTree", 0),
                BuildFieldPane("Resulting fields", item.AfterFields, $"NativeWorkspaceChange_{stableId}_ResultTree", 1)
            }
        };
        var semanticChanges = new ItemsControl
        {
            ItemTemplate = new FuncDataTemplate<SemanticChangeDescriptor>(
                (change, _) => CreateWrappedText(change is null ? string.Empty : FormatSemanticChange(change), 12))
        };
        semanticChanges.ItemsSource = item.SemanticChanges;
        AutomationProperties.SetAutomationId(semanticChanges, $"NativeWorkspaceChange_{stableId}_SemanticChanges");
        var warnings = new ItemsControl
        {
            ItemTemplate = new FuncDataTemplate<EngineWarning>(
                (warning, _) => CreateWrappedText(
                    warning is null ? string.Empty : $"{warning.Code}: {warning.Message}",
                    12))
        };
        warnings.ItemsSource = item.Warnings;
        AutomationProperties.SetAutomationId(warnings, $"NativeWorkspaceChange_{stableId}_Warnings");
        var content = new StackPanel
        {
            Spacing = 8,
            Children =
            {
                contexts,
                CreateText("Semantic changes", 13, FontWeight.SemiBold),
                semanticChanges,
                trees,
                CreateText("Warnings", 13, FontWeight.SemiBold),
                warnings
            }
        };
        var expander = new Expander
        {
            Header = item.FormKey.ToString(),
            Content = content,
            IsExpanded = true,
            HorizontalAlignment = HorizontalAlignment.Stretch
        };
        AutomationProperties.SetAutomationId(expander, $"NativeWorkspaceChange_{stableId}");
        return new Border
        {
            BorderBrush = App.GetApplicationBrush(App.BorderBrushKey),
            BorderThickness = new Thickness(1),
            Padding = new Thickness(10),
            Margin = new Thickness(0, 0, 0, 10),
            Child = expander
        };
    }

    /// <summary>Builds one independent hierarchical detached JSON field pane.</summary>
    /// <param name="title">The visible pane title.</param>
    /// <param name="fields">The projected JSON field roots.</param>
    /// <param name="automationId">The deterministic tree identity.</param>
    /// <param name="column">The containing grid column.</param>
    /// <returns>The configured field pane.</returns>
    private static Control BuildFieldPane(
        string title,
        IReadOnlyList<NativeJsonFieldNodeViewModel> fields,
        string automationId,
        int column)
    {
        var source = new HierarchicalTreeDataGridSource<NativeJsonFieldNodeViewModel>(fields)
            .WithHierarchicalExpanderTextColumn(
                "Field",
                field => field.Name,
                field => field.Children,
                field => field.IsExpanded,
                field => field.HasChildren,
                options => options.BeginEditGestures = BeginEditGestures.None)
            .WithTextColumn("Kind", field => field.KindText, options => options.BeginEditGestures = BeginEditGestures.None)
            .WithTextColumn("Value", field => field.ValueText, options => options.BeginEditGestures = BeginEditGestures.None);
        var tree = new TreeDataGrid
        {
            Source = source,
            MinHeight = 180,
            MaxHeight = 300,
            CanUserResizeColumns = true,
            CanUserSortColumns = false,
            HorizontalAlignment = HorizontalAlignment.Stretch
        };
        AutomationProperties.SetAutomationId(tree, automationId);
        Grid.SetRow(tree, 1);
        var pane = new Grid
        {
            RowDefinitions = new RowDefinitions("Auto,*"),
            RowSpacing = 6,
            Children =
            {
                CreateText(title, 13, FontWeight.SemiBold),
                tree
            }
        };
        Grid.SetColumn(pane, column);
        return pane;
    }

    /// <summary>Builds the aggregate warning list retained from state, preview, and comparisons.</summary>
    /// <returns>The bound warnings panel.</returns>
    private Control BuildWarnings()
    {
        var warnings = new ItemsControl
        {
            ItemTemplate = new FuncDataTemplate<EngineWarning>(
                (warning, _) => CreateWrappedText(
                    warning is null ? string.Empty : $"{warning.Code}: {warning.Message}",
                    12))
        };
        warnings.Bind(ItemsControl.ItemsSourceProperty, new Binding(nameof(NativeWorkspaceChangesViewModel.Warnings)));
        AutomationProperties.SetAutomationId(warnings, "NativeWorkspaceWarnings");
        return new StackPanel
        {
            Spacing = 5,
            Children =
            {
                CreateText("Warnings", 17, FontWeight.SemiBold),
                warnings
            }
        };
    }

    /// <summary>Builds indeterminate activity, exact persistence status, and accessible typed failure feedback.</summary>
    /// <returns>The persistence status panel.</returns>
    private Control BuildPersistenceStatus()
    {
        var progress = new ProgressBar
        {
            IsIndeterminate = true,
            MinHeight = 4
        };
        progress.Bind(IsVisibleProperty, new Binding(nameof(NativeWorkspaceChangesViewModel.IsBusy)));
        var status = CreateText(string.Empty, 13, FontWeight.Normal);
        status.TextWrapping = TextWrapping.Wrap;
        status.Focusable = true;
        status.Bind(TextBlock.TextProperty, new Binding(nameof(NativeWorkspaceChangesViewModel.StatusText)));
        AutomationProperties.SetAutomationId(status, "NativeWorkspaceChangesStatusText");
        AutomationProperties.SetName(status, "Persistence status");
        var error = CreateText(string.Empty, 13, FontWeight.SemiBold);
        error.Foreground = new SolidColorBrush(Color.FromRgb(204, 73, 73));
        error.TextWrapping = TextWrapping.Wrap;
        error.Bind(TextBlock.TextProperty, new Binding(nameof(NativeWorkspaceChangesViewModel.ErrorMessage)));
        AutomationProperties.SetAutomationId(error, "NativeWorkspaceChangesErrorText");
        AutomationProperties.SetName(error, "Native workspace persistence error");
        return new StackPanel
        {
            Spacing = 5,
            Children =
            {
                CreateText("Persistence status", 17, FontWeight.SemiBold),
                progress,
                status,
                error
            }
        };
    }

    /// <summary>Builds the exact retry, recovery inspection, repair, and adoption actions.</summary>
    /// <returns>The bound recovery action panel.</returns>
    private Control BuildRecoveryActions()
    {
        return new WrapPanel
        {
            Orientation = Orientation.Horizontal,
            ItemWidth = double.NaN,
            ItemHeight = double.NaN,
            Children =
            {
                CreateOperationButton(
                    "Cancel operation",
                    "NativeWorkspaceCancelOperationButton",
                    ViewModel.CancelActiveOperationAndDrainAsync,
                    nameof(NativeWorkspaceChangesViewModel.CanRequestCancellation)),
                CreateOperationButton(
                    "Retry Refresh",
                    "NativeWorkspaceRetryCommittedRefreshButton",
                    ViewModel.RetryCommittedRefreshAsync,
                    nameof(NativeWorkspaceChangesViewModel.CanRetryCommittedRefresh)),
                CreateOperationButton(
                    "Inspect Save Outcome",
                    "NativeWorkspaceInspectRecoveryButton",
                    ViewModel.InspectSaveOutcomeAsync,
                    nameof(NativeWorkspaceChangesViewModel.CanInspectSaveOutcome)),
                CreateOperationButton(
                    "Complete Prepared Save",
                    "NativeWorkspaceCompletePreparedButton",
                    ViewModel.CompletePreparedSaveAsync,
                    nameof(NativeWorkspaceChangesViewModel.CanCompletePreparedSave)),
                CreateOperationButton(
                    "Restore Previous Output",
                    "NativeWorkspaceRestoreBaselineButton",
                    ViewModel.RestorePreviousOutputAsync,
                    nameof(NativeWorkspaceChangesViewModel.CanRestorePreviousOutput)),
                CreateOperationButton(
                    "Resume Unsaved Changes",
                    "NativeWorkspaceResumeStagedButton",
                    ViewModel.ResumeUnsavedChangesAsync,
                    nameof(NativeWorkspaceChangesViewModel.CanResumeUnsavedChanges)),
                CreateOperationButton(
                    ViewModel.ReopenResolvedOutputLabel,
                    "NativeWorkspaceReopenResolvedButton",
                    ViewModel.ReopenResolvedOutputAsync,
                    nameof(NativeWorkspaceChangesViewModel.CanReopenResolvedOutput),
                    nameof(NativeWorkspaceChangesViewModel.ReopenResolvedOutputLabel))
            }
        };
    }

    /// <summary>Builds non-destructive exit and explicit save, discard, pending-operation, or abandonment choices.</summary>
    /// <returns>The dialog footer.</returns>
    private Control BuildFooter()
    {
        var keepEditing = CreateChoiceButton(
            Request.Purpose == NativeWorkspaceChangesDialogPurpose.Review ? "Done" : "Keep Editing",
            "NativeWorkspaceKeepEditingButton",
            NativeWorkspaceChangesDialogChoice.KeepEditing,
            canExecuteProperty: null,
            visibleProperty: null);
        keepEditing.IsDefault = true;
        var activeEditorOperation = CreateText(
            "An editor operation is still running. Wait for it, request cancellation and wait for a safe result, or keep editing.",
            13,
            FontWeight.SemiBold);
        activeEditorOperation.TextWrapping = TextWrapping.Wrap;
        activeEditorOperation.Bind(
            IsVisibleProperty,
            new Binding(nameof(NativeWorkspaceChangesViewModel.ShowActiveEditorOperationDecision)));
        AutomationProperties.SetAutomationId(activeEditorOperation, "NativeWorkspaceActiveEditorOperationText");
        var waitForEditorOperation = CreateChoiceButton(
            "Wait for operation",
            "NativeWorkspaceWaitForEditorOperationButton",
            NativeWorkspaceChangesDialogChoice.WaitForEditorOperation,
            nameof(NativeWorkspaceChangesViewModel.CanWaitForEditorOperation),
            nameof(NativeWorkspaceChangesViewModel.ShowActiveEditorOperationDecision));
        var cancelEditorOperationAndWait = CreateChoiceButton(
            "Cancel operation and wait",
            "NativeWorkspaceCancelEditorOperationAndWaitButton",
            NativeWorkspaceChangesDialogChoice.CancelEditorOperationAndWait,
            nameof(NativeWorkspaceChangesViewModel.CanCancelEditorOperationAndWait),
            nameof(NativeWorkspaceChangesViewModel.ShowActiveEditorOperationDecision));
        var save = CreateChoiceButton(
            "Save and Proceed",
            "NativeWorkspaceSaveAndProceedButton",
            NativeWorkspaceChangesDialogChoice.SaveAndProceed,
            nameof(NativeWorkspaceChangesViewModel.CanSaveAndProceed),
            nameof(NativeWorkspaceChangesViewModel.ShowSaveAndProceed));
        var discard = CreateChoiceButton(
            "Discard and Proceed",
            "NativeWorkspaceDiscardAndProceedButton",
            NativeWorkspaceChangesDialogChoice.DiscardAndProceed,
            nameof(NativeWorkspaceChangesViewModel.CanDiscardAndProceed),
            nameof(NativeWorkspaceChangesViewModel.ShowDiscardAndProceed));
        var returnToEditor = CreateChoiceButton(
            "Return to editor",
            "NativeWorkspaceReturnToEditorButton",
            NativeWorkspaceChangesDialogChoice.ReturnToEditor,
            nameof(NativeWorkspaceChangesViewModel.CanReturnToEditor),
            nameof(NativeWorkspaceChangesViewModel.ShowReturnToEditor));
        var abandon = CreateChoiceButton(
            "Discard Current Workspace and Reopen...",
            "NativeWorkspaceConfirmAbandonmentButton",
            NativeWorkspaceChangesDialogChoice.ConfirmAbandonmentForOpen,
            nameof(NativeWorkspaceChangesViewModel.CanConfirmAbandonmentForOpen),
            nameof(NativeWorkspaceChangesViewModel.ShowConfirmedAbandonmentForOpen));
        var saveReason = CreateDisabledReason(
            nameof(NativeWorkspaceChangesViewModel.SaveAndProceedDisabledReason),
            nameof(NativeWorkspaceChangesViewModel.ShowSaveAndProceed),
            "NativeWorkspaceSaveDisabledReason");
        var discardReason = CreateDisabledReason(
            nameof(NativeWorkspaceChangesViewModel.DiscardAndProceedDisabledReason),
            nameof(NativeWorkspaceChangesViewModel.ShowDiscardAndProceed),
            "NativeWorkspaceDiscardDisabledReason");
        var abandonmentExplanation = CreateDisabledReason(
            nameof(NativeWorkspaceChangesViewModel.ConfirmedAbandonmentExplanation),
            nameof(NativeWorkspaceChangesViewModel.ShowConfirmedAbandonmentForOpen),
            "NativeWorkspaceAbandonmentExplanation");
        return new Border
        {
            Background = App.GetApplicationBrush(App.PanelSurfaceBrushKey),
            BorderBrush = App.GetApplicationBrush(App.BorderBrushKey),
            BorderThickness = new Thickness(0, 1, 0, 0),
            Padding = new Thickness(18, 12),
            Child = new StackPanel
            {
                Spacing = 8,
                Children =
                {
                    activeEditorOperation,
                    saveReason,
                    discardReason,
                    abandonmentExplanation,
                    new WrapPanel
                    {
                        Orientation = Orientation.Horizontal,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        Children =
                        {
                            keepEditing,
                            waitForEditorOperation,
                            cancelEditorOperationAndWait,
                            returnToEditor,
                            save,
                            discard,
                            abandon
                        }
                    }
                }
            }
        };
    }

    /// <summary>Creates one directly awaited recovery or cancellation button.</summary>
    /// <param name="text">The visible action label.</param>
    /// <param name="automationId">The stable automation identity.</param>
    /// <param name="operation">The public awaited view-model operation.</param>
    /// <param name="canExecuteProperty">The bound availability and visibility property.</param>
    /// <param name="contentProperty">The optional bound content property that replaces the initial text.</param>
    /// <returns>The configured operation button.</returns>
    private static Button CreateOperationButton(
        string text,
        string automationId,
        Func<CancellationToken, Task> operation,
        string canExecuteProperty,
        string? contentProperty = null)
    {
        var button = new Button
        {
            Content = text,
            Padding = new Thickness(12, 7),
            Margin = new Thickness(0, 0, 8, 8)
        };
        button.Bind(IsEnabledProperty, new Binding(canExecuteProperty));
        button.Bind(IsVisibleProperty, new Binding(canExecuteProperty));
        if (contentProperty is not null)
        {
            button.Bind(ContentControl.ContentProperty, new Binding(contentProperty));
        }

        button.Click += async (_, _) => await operation(CancellationToken.None);
        AutomationProperties.SetAutomationId(button, automationId);
        return button;
    }

    /// <summary>Creates one final choice button that forwards only a closed dialog choice.</summary>
    /// <param name="text">The visible action label.</param>
    /// <param name="automationId">The stable automation identity.</param>
    /// <param name="choice">The closed state-machine choice.</param>
    /// <param name="canExecuteProperty">The optional bound availability property.</param>
    /// <param name="visibleProperty">The optional bound visibility property.</param>
    /// <returns>The configured final choice button.</returns>
    private Button CreateChoiceButton(
        string text,
        string automationId,
        NativeWorkspaceChangesDialogChoice choice,
        string? canExecuteProperty,
        string? visibleProperty)
    {
        var button = new Button
        {
            Content = text,
            Padding = new Thickness(14, 8),
            Margin = new Thickness(8, 0, 0, 0)
        };
        if (canExecuteProperty is not null)
        {
            button.Bind(IsEnabledProperty, new Binding(canExecuteProperty));
        }

        if (visibleProperty is not null)
        {
            button.Bind(IsVisibleProperty, new Binding(visibleProperty));
        }

        button.Click += async (_, _) => await ApplyChoiceAsync(choice);
        AutomationProperties.SetAutomationId(button, automationId);
        return button;
    }

    /// <summary>Creates adjacent explanatory text for a conditionally shown destructive action.</summary>
    /// <param name="textProperty">The bound explanation property.</param>
    /// <param name="visibleProperty">The bound action visibility property.</param>
    /// <param name="automationId">The stable explanation identity.</param>
    /// <returns>The configured explanation text.</returns>
    private static TextBlock CreateDisabledReason(string textProperty, string visibleProperty, string automationId)
    {
        var text = CreateText(string.Empty, 12, FontWeight.Normal);
        text.TextWrapping = TextWrapping.Wrap;
        text.Bind(TextBlock.TextProperty, new Binding(textProperty));
        text.Bind(IsVisibleProperty, new Binding(visibleProperty));
        AutomationProperties.SetAutomationId(text, automationId);
        return text;
    }

    /// <summary>Creates purpose-specific guidance without changing the dialog's fixed user-visible title.</summary>
    /// <param name="request">The closed dialog purpose.</param>
    /// <returns>The exact guidance for review, save, discard, or one leave reason.</returns>
    private static string CreatePurposeText(NativeWorkspaceChangesDialogRequest request)
    {
        return request.Purpose switch
        {
            NativeWorkspaceChangesDialogPurpose.Review => "Review form changes and changes applied to the workspace.",
            NativeWorkspaceChangesDialogPurpose.Save => "Review every staged change before saving the complete selected output.",
            NativeWorkspaceChangesDialogPurpose.Discard => "Review form changes and changes applied to the workspace before discarding them.",
            NativeWorkspaceChangesDialogPurpose.Leave => CreateLeavePurposeText(request.LeaveReason),
            _ => throw new ArgumentOutOfRangeException(nameof(request))
        };
    }

    /// <summary>Creates exact guidance for the final application action blocked by the dialog.</summary>
    /// <param name="reason">The validated leave reason.</param>
    /// <returns>The visible guarded-leave explanation.</returns>
    private static string CreateLeavePurposeText(NativeWorkspaceLeaveReason? reason)
    {
        return reason switch
        {
            NativeWorkspaceLeaveReason.OpenWorkspace => "Resolve current work before selecting another workspace. A completed save or discard remains intentional if selection is later canceled. Confirmed abandonment keeps this workspace until replacement succeeds.",
            NativeWorkspaceLeaveReason.CloseWorkspace => "Resolve current work before closing the native workspace.",
            NativeWorkspaceLeaveReason.ShowSettings => "Resolve current work before opening Settings.",
            NativeWorkspaceLeaveReason.ExitApplication => "Resolve current work before exiting CreationsForge.",
            _ => throw new ArgumentOutOfRangeException(nameof(reason))
        };
    }

    /// <summary>Formats exact context selection and containing-plugin provenance.</summary>
    /// <param name="context">The detached native context.</param>
    /// <returns>The visible resolution status and provenance.</returns>
    private static string FormatContext(FormListContext context)
    {
        if (context.ContainingModKey is null)
        {
            return context.Status.ToString();
        }

        return $"{context.Status}; {context.ContainingModKey.Value.FileName}; load order {context.LoadOrderIndex}; {context.Role}; {context.Path}";
    }

    /// <summary>Formats one semantic change without interpreting its stable field identifier.</summary>
    /// <param name="change">The exact engine descriptor.</param>
    /// <returns>The visible identifier, kind, and optional collection positions.</returns>
    private static string FormatSemanticChange(SemanticChangeDescriptor change)
    {
        var before = change.BeforePosition?.ToString() ?? "-";
        var after = change.AfterPosition?.ToString() ?? "-";
        return $"{change.FieldIdentifier} | {change.Kind} | before {before} | after {after}";
    }

    /// <summary>Creates a deterministic automation-safe suffix from a FormKey.</summary>
    /// <param name="formKey">The exact FormKey text.</param>
    /// <returns>A non-empty identifier containing only letters, digits, and underscores.</returns>
    private static string CreateStableFormKeyId(string formKey)
    {
        var value = new string(formKey.Select(character => char.IsLetterOrDigit(character) ? character : '_').ToArray());
        return string.IsNullOrWhiteSpace(value) ? "Unknown" : value;
    }

    /// <summary>Creates unbound application-styled text.</summary>
    /// <param name="text">The visible text.</param>
    /// <param name="fontSize">The font size.</param>
    /// <param name="fontWeight">The font weight.</param>
    /// <returns>The configured text block.</returns>
    private static TextBlock CreateText(string text, double fontSize, FontWeight fontWeight)
    {
        var textBlock = new TextBlock
        {
            Text = text,
            FontSize = fontSize,
            FontWeight = fontWeight
        };
        App.ApplyApplicationTextForeground(textBlock);
        return textBlock;
    }

    /// <summary>Creates wrapping application-styled diagnostic text.</summary>
    /// <param name="text">The visible diagnostic text.</param>
    /// <param name="fontSize">The font size.</param>
    /// <returns>The configured wrapping text.</returns>
    private static TextBlock CreateWrappedText(string text, double fontSize)
    {
        var textBlock = CreateText(text, fontSize, FontWeight.Normal);
        textBlock.TextWrapping = TextWrapping.Wrap;
        textBlock.HorizontalAlignment = HorizontalAlignment.Stretch;
        return textBlock;
    }
}
