using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using CreationsForge.RecordEditing.Drafts;
using CreationsForge.RecordEditing.Schema;

namespace CreationsForge.Views.RecordEditing;

/// <summary>Builds bounded Avalonia controls for the closed plugin wire draft-node set.</summary>
internal static partial class RecordWireDraftControlFactory
{
    /// <summary>The maximum number of lightweight union choices shown by one search.</summary>
    private const int MaximumUnionSearchResults = 100;

    /// <summary>Creates the root typed draft form without inspecting schema JSON.</summary>
    /// <param name="node">The typed root node.</param>
    /// <param name="pickReferenceAsync">The optional existing reference-picker bridge.</param>
    /// <returns>The scrollable editor content for the node.</returns>
    internal static Control Create(
        RecordWireDraftNode node,
        Func<RecordWireFormLinkDraftNode, Task>? pickReferenceAsync = null)
    {
        ArgumentNullException.ThrowIfNull(node);
        return CreateNode(node, pickReferenceAsync, isRoot: true, forceReadOnly: false);
    }

    /// <summary>Creates a directly labeled record field without a command-style root header or JSON path.</summary>
    internal static Control CreateFormField(
        RecordWireDraftNode node,
        Func<RecordWireFormLinkDraftNode, Task>? pickReferenceAsync = null)
    {
        ArgumentNullException.ThrowIfNull(node);
        return new StackPanel
        {
            Spacing = 5,
            Children =
            {
                CreateNodeEditor(node, pickReferenceAsync, forceReadOnly: false),
                CreateIssueList(node)
            }
        };
    }

    /// <summary>Creates one typed node and its local issue surface.</summary>
    /// <param name="node">The exact typed node.</param>
    /// <param name="pickReferenceAsync">The optional existing reference-picker bridge.</param>
    /// <param name="isRoot">Whether this is the command root.</param>
    /// <param name="forceReadOnly">Whether the containing owner makes this projection inactive.</param>
    /// <returns>The complete node control.</returns>
    private static Control CreateNode(
        RecordWireDraftNode node,
        Func<RecordWireFormLinkDraftNode, Task>? pickReferenceAsync,
        bool isRoot,
        bool forceReadOnly)
    {
        var editor = CreateNodeEditor(node, pickReferenceAsync, forceReadOnly);

        if (isRoot)
        {
            return new StackPanel
            {
                Spacing = 8,
                Children =
                {
                    CreateNodeHeader(node),
                    editor,
                    CreateIssueList(node)
                }
            };
        }

        return new Border
        {
            BorderBrush = App.GetApplicationBrush(App.BorderBrushKey),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(3),
            Padding = new Thickness(10),
            Opacity = forceReadOnly ? 0.58 : 1,
            Child = new StackPanel
            {
                Spacing = 7,
                Children =
                {
                    CreateNodeHeader(node),
                    editor,
                    CreateIssueList(node)
                }
            }
        };
    }

    /// <summary>Builds the typed input control shared by command and direct-field presentations.</summary>
    private static Control CreateNodeEditor(
        RecordWireDraftNode node,
        Func<RecordWireFormLinkDraftNode, Task>? pickReferenceAsync,
        bool forceReadOnly)
    {
        return node.Kind switch
        {
            RecordWireDraftNodeKind.Object => CreateObjectEditor((RecordWireObjectDraftNode)node, pickReferenceAsync, forceReadOnly),
            RecordWireDraftNodeKind.Array => CreateArrayEditor((RecordWireArrayDraftNode)node, pickReferenceAsync, forceReadOnly),
            RecordWireDraftNodeKind.Union => CreateUnionEditor((RecordWireUnionDraftNode)node, pickReferenceAsync, forceReadOnly),
            RecordWireDraftNodeKind.Nullable => CreateNullableEditor((RecordWireNullableDraftNode)node, pickReferenceAsync, forceReadOnly),
            RecordWireDraftNodeKind.Boolean => CreateBooleanEditor((RecordWireBooleanDraftNode)node, forceReadOnly),
            RecordWireDraftNodeKind.Integer => CreateIntegerEditor((RecordWireIntegerDraftNode)node, forceReadOnly),
            RecordWireDraftNodeKind.String => CreateStringEditor((RecordWireStringDraftNode)node, forceReadOnly, acceptsReturn: false),
            RecordWireDraftNodeKind.Enum => CreateEnumEditor((RecordWireEnumDraftNode)node, forceReadOnly),
            RecordWireDraftNodeKind.FormLink => CreateFormLinkEditor((RecordWireFormLinkDraftNode)node, pickReferenceAsync, forceReadOnly),
            RecordWireDraftNodeKind.FormLinkOrIndex => CreateFormLinkOrIndexEditor((RecordWireFormLinkOrIndexDraftNode)node, pickReferenceAsync, forceReadOnly),
            RecordWireDraftNodeKind.FloatBits => CreateSpecializedObjectEditor((RecordWireFloatBitsDraftNode)node, "Exact floating-point bits are authoritative.", pickReferenceAsync, forceReadOnly),
            RecordWireDraftNodeKind.ByteArray => CreateSpecializedObjectEditor((RecordWireByteArrayDraftNode)node, "Base64 bytes and decoded length are validated together.", pickReferenceAsync, forceReadOnly),
            RecordWireDraftNodeKind.TranslatedString => CreateTranslatedStringEditor((RecordWireTranslatedStringDraftNode)node, pickReferenceAsync, forceReadOnly),
            RecordWireDraftNodeKind.Asset => CreateSpecializedObjectEditor((RecordWireAssetDraftNode)node, "The given path and explicit null state are authoritative.", pickReferenceAsync, forceReadOnly),
            RecordWireDraftNodeKind.Color => CreateSpecializedObjectEditor((RecordWireColorDraftNode)node, "Authoritative and redundant color values must remain consistent.", pickReferenceAsync, forceReadOnly),
            RecordWireDraftNodeKind.Array2D => CreateSpecializedObjectEditor((RecordWireArray2DDraftNode)node, "Dimensions, row boundaries, and cell order are preserved.", pickReferenceAsync, forceReadOnly),
            _ => throw new ArgumentOutOfRangeException(nameof(node), node.Kind, "Unsupported plugin wire draft node kind.")
        };
    }

    /// <summary>Creates one field header with exact path and value authority.</summary>
    /// <param name="node">The typed field.</param>
    /// <returns>The field header.</returns>
    private static Control CreateNodeHeader(RecordWireDraftNode node)
    {
        var authority = node.IsReadOnly
            ? "Derived / read only"
            : node.ValueState == RecordWireDraftValueState.RequiredUnset
                ? "Required value needed"
                : node.IsRequired
                    ? "Required"
                    : "Optional";
        var title = CreateText(node.DisplayName, 13, FontWeight.SemiBold);
        var badge = CreateText(authority, 11, FontWeight.Normal);
        badge.Opacity = node.ValueState == RecordWireDraftValueState.RequiredUnset ? 1 : 0.68;
        var heading = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 8,
            Children =
            {
                title,
                badge
            }
        };
        var path = CreateText(string.Empty, 10, FontWeight.Normal);
        path.DataContext = node;
        path.Bind(TextBlock.TextProperty, new Binding(nameof(RecordWireDraftNode.Path)));
        path.Opacity = 0.62;
        path.TextWrapping = TextWrapping.Wrap;
        var panel = new StackPanel
        {
            Spacing = 2,
            Children =
            {
                heading,
                path
            }
        };
        SetAutomationId(panel, node, "Header");
        return panel;
    }

    /// <summary>Creates a lazily expanded closed object editor.</summary>
    /// <param name="node">The object draft.</param>
    /// <param name="pickReferenceAsync">The optional reference-picker bridge.</param>
    /// <param name="forceReadOnly">Whether the containing owner prevents editing.</param>
    /// <returns>The object fields.</returns>
    private static Control CreateObjectEditor(
        RecordWireObjectDraftNode node,
        Func<RecordWireFormLinkDraftNode, Task>? pickReferenceAsync,
        bool forceReadOnly)
    {
        var fields = new StackPanel { Spacing = 8 };
        foreach (var property in node.Properties)
        {
            fields.Children.Add(CreateDeferredChild(property.Node, pickReferenceAsync, forceReadOnly || node.IsReadOnly));
        }

        if (fields.Children.Count == 0)
        {
            var empty = CreateText("This command has no editable arguments.", 12, FontWeight.Normal);
            empty.Opacity = 0.7;
            fields.Children.Add(empty);
        }

        SetAutomationId(fields, node, "ObjectFields");
        return fields;
    }

    /// <summary>Creates one child immediately for scalars or on first expansion for nested structures.</summary>
    /// <param name="node">The child node.</param>
    /// <param name="pickReferenceAsync">The optional reference-picker bridge.</param>
    /// <param name="forceReadOnly">Whether the containing owner prevents editing.</param>
    /// <returns>The direct or deferred child control.</returns>
    private static Control CreateDeferredChild(
        RecordWireDraftNode node,
        Func<RecordWireFormLinkDraftNode, Task>? pickReferenceAsync,
        bool forceReadOnly = false)
    {
        if (!IsExpandable(node.Kind))
        {
            return CreateNode(node, pickReferenceAsync, isRoot: false, forceReadOnly);
        }

        var expander = new Expander
        {
            Header = $"{node.DisplayName}  ·  {node.Kind}",
            HorizontalAlignment = HorizontalAlignment.Stretch
        };
        SetAutomationId(expander, node, "Expander");
        expander.Expanded += (_, _) =>
        {
            expander.Content ??= CreateNode(node, pickReferenceAsync, isRoot: false, forceReadOnly);
        };
        return expander;
    }

    /// <summary>Gets whether a node should defer creation of its nested controls.</summary>
    /// <param name="kind">The closed node kind.</param>
    /// <returns><see langword="true"/> for container and specialized graph nodes.</returns>
    private static bool IsExpandable(RecordWireDraftNodeKind kind)
    {
        return kind is RecordWireDraftNodeKind.Object
            or RecordWireDraftNodeKind.Array
            or RecordWireDraftNodeKind.Union
            or RecordWireDraftNodeKind.Nullable
            or RecordWireDraftNodeKind.FloatBits
            or RecordWireDraftNodeKind.TranslatedString
            or RecordWireDraftNodeKind.Asset
            or RecordWireDraftNodeKind.Color
            or RecordWireDraftNodeKind.Array2D;
    }

    /// <summary>Creates an ordered array editor with explicit insert, remove, move, replace, and clear actions.</summary>
    /// <param name="node">The ordered array draft.</param>
    /// <param name="pickReferenceAsync">The optional reference-picker bridge.</param>
    /// <param name="forceReadOnly">Whether the containing owner prevents editing.</param>
    /// <returns>The ordered collection workflow.</returns>
    private static Control CreateArrayEditor(
        RecordWireArrayDraftNode node,
        Func<RecordWireFormLinkDraftNode, Task>? pickReferenceAsync,
        bool forceReadOnly)
    {
        var operationError = CreateOperationError(node);
        var items = new ItemsControl
        {
            ItemsSource = node.Items,
            ItemTemplate = new FuncDataTemplate<RecordWireDraftNode>((item, _) =>
                item is null
                    ? new TextBlock()
                    : CreateArrayItem(node, item, pickReferenceAsync, operationError, forceReadOnly))
        };
        SetAutomationId(items, node, "Items");

        var append = CreateActionButton("Append item", node, "Append");
        append.IsEnabled = !node.IsReadOnly && !forceReadOnly;
        append.Click += (_, _) => PublishOperationResult(node.Insert(node.Items.Count), operationError);
        var clear = CreateActionButton("Clear", node, "Clear");
        clear.IsEnabled = !node.IsReadOnly && !forceReadOnly && node.Items.Count > 0;
        clear.Click += (_, _) =>
        {
            node.Clear();
            operationError.Text = string.Empty;
        };
        node.PropertyChanged += (_, eventArgs) =>
        {
            if (string.Equals(eventArgs.PropertyName, nameof(RecordWireArrayDraftNode.Items), StringComparison.Ordinal))
            {
                clear.IsEnabled = !node.IsReadOnly && !forceReadOnly && node.Items.Count > 0;
            }
        };

        var actions = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 8,
            Children =
            {
                append,
                clear
            }
        };
        var collectionName = node.DisplayName.Contains("conditional", StringComparison.OrdinalIgnoreCase)
            ? "Conditional Entries"
            : node.DisplayName;
        return new StackPanel
        {
            Spacing = 8,
            Children =
            {
                CreateCollectionCount(node, collectionName),
                actions,
                operationError,
                items
            }
        };
    }

    /// <summary>Creates one ordered array item and its position-aware actions.</summary>
    /// <param name="owner">The owning array.</param>
    /// <param name="item">The current typed item.</param>
    /// <param name="pickReferenceAsync">The optional reference-picker bridge.</param>
    /// <param name="operationError">The collection operation error target.</param>
    /// <param name="forceReadOnly">Whether the containing owner prevents editing.</param>
    /// <returns>The item editor and actions.</returns>
    private static Control CreateArrayItem(
        RecordWireArrayDraftNode owner,
        RecordWireDraftNode item,
        Func<RecordWireFormLinkDraftNode, Task>? pickReferenceAsync,
        TextBlock operationError,
        bool forceReadOnly)
    {
        var insert = CreateActionButton("Insert before", item, "InsertBefore");
        var up = CreateActionButton("Move up", item, "MoveUp");
        var down = CreateActionButton("Move down", item, "MoveDown");
        var replace = CreateActionButton("Replace", item, "Replace");
        var remove = CreateActionButton("Remove", item, "Remove");
        foreach (var button in new[] { insert, up, down, replace, remove })
        {
            button.IsEnabled = !owner.IsReadOnly && !forceReadOnly;
        }

        insert.Click += (_, _) =>
        {
            var index = owner.Items.IndexOf(item);
            PublishOperationResult(owner.Insert(index), operationError);
        };
        up.Click += (_, _) =>
        {
            var index = owner.Items.IndexOf(item);
            owner.Move(index, index - 1);
        };
        down.Click += (_, _) =>
        {
            var index = owner.Items.IndexOf(item);
            owner.Move(index, index + 1);
        };
        replace.Click += (_, _) =>
        {
            var index = owner.Items.IndexOf(item);
            PublishOperationResult(owner.Replace(index), operationError);
        };
        remove.Click += (_, _) => owner.Remove(owner.Items.IndexOf(item));

        var actions = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 6,
            Children =
            {
                insert,
                up,
                down,
                replace,
                remove
            }
        };
        var panel = new StackPanel
        {
            Spacing = 6,
            Margin = new Thickness(0, 0, 0, 8),
            Children =
            {
                actions,
                CreateDeferredChild(item, pickReferenceAsync, forceReadOnly || owner.IsReadOnly)
            }
        };
        SetAutomationId(panel, item, "ArrayItem");
        return panel;
    }

    /// <summary>Creates a searchable union selector that materializes only an explicit choice.</summary>
    /// <param name="node">The lazy union draft.</param>
    /// <param name="pickReferenceAsync">The optional reference-picker bridge.</param>
    /// <param name="forceReadOnly">Whether the containing owner prevents editing.</param>
    /// <returns>The union search and selected-value host.</returns>
    private static Control CreateUnionEditor(
        RecordWireUnionDraftNode node,
        Func<RecordWireFormLinkDraftNode, Task>? pickReferenceAsync,
        bool forceReadOnly)
    {
        var query = new TextBox { PlaceholderText = "Search available types" };
        SetAutomationId(query, node, "UnionSearch");
        var choices = new ComboBox
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            MaxDropDownHeight = 280,
            ItemTemplate = new FuncDataTemplate<RecordWireSchemaUnionOption>((option, _) =>
                CreateText(option is null ? string.Empty : FormatUnionOption(option), 12, FontWeight.Normal))
        };
        SetAutomationId(choices, node, "UnionOptions");
        var useChoice = CreateActionButton("Use selected type", node, "SelectUnionOption");
        useChoice.IsEnabled = !node.IsReadOnly && !forceReadOnly;
        var search = CreateActionButton("Search", node, "SearchUnionOptions");
        var operationError = CreateOperationError(node);
        var selectedHost = new StackPanel { Spacing = 6 };
        SetAutomationId(selectedHost, node, "SelectedUnionValue");

        void RefreshChoices()
        {
            var matches = node.SearchOptions(query.Text, MaximumUnionSearchResults).ToList();
            if (node.SelectedOption is not null && matches.All(option => option.Key != node.SelectedOption.Key))
            {
                matches.Insert(0, node.SelectedOption);
            }

            choices.ItemsSource = matches;
            choices.SelectedItem = node.SelectedOption is null
                ? null
                : matches.First(option => option.Key == node.SelectedOption.Key);
        }

        void RefreshSelectedValue()
        {
            selectedHost.Children.Clear();
            if (node.SelectedValue is null)
            {
                var required = CreateText("Choose a type to create this value.", 12, FontWeight.Normal);
                required.Opacity = 0.72;
                selectedHost.Children.Add(required);
                return;
            }

            selectedHost.Children.Add(CreateDeferredChild(node.SelectedValue, pickReferenceAsync, forceReadOnly || node.IsReadOnly));
        }

        search.Click += (_, _) => RefreshChoices();
        useChoice.Click += (_, _) =>
        {
            if (choices.SelectedItem is not RecordWireSchemaUnionOption option)
            {
                operationError.Text = "Choose one available type first.";
                return;
            }

            PublishOperationResult(node.SelectOption(option.Key), operationError);
        };
        node.PropertyChanged += (_, eventArgs) =>
        {
            if (string.Equals(eventArgs.PropertyName, nameof(RecordWireUnionDraftNode.SelectedValue), StringComparison.Ordinal))
            {
                RefreshChoices();
                RefreshSelectedValue();
            }
        };
        RefreshChoices();
        RefreshSelectedValue();

        var searchRow = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("*,Auto"),
            ColumnSpacing = 8,
            Children =
            {
                query,
                search
            }
        };
        Grid.SetColumn(search, 1);
        var choiceRow = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("*,Auto"),
            ColumnSpacing = 8,
            Children =
            {
                choices,
                useChoice
            }
        };
        Grid.SetColumn(useChoice, 1);
        return new StackPanel
        {
            Spacing = 8,
            Children =
            {
                searchRow,
                choiceRow,
                operationError,
                selectedHost
            }
        };
    }

    /// <summary>Creates the whole-null versus present wrapper selector without collapsing nested null identity.</summary>
    /// <param name="node">The nullable wrapper.</param>
    /// <param name="pickReferenceAsync">The optional reference-picker bridge.</param>
    /// <param name="forceReadOnly">Whether the containing owner prevents editing.</param>
    /// <returns>The nullable control.</returns>
    private static Control CreateNullableEditor(
        RecordWireNullableDraftNode node,
        Func<RecordWireFormLinkDraftNode, Task>? pickReferenceAsync,
        bool forceReadOnly)
    {
        var state = new ComboBox
        {
            ItemsSource = new[] { "Whole value is JSON null", "Present value" },
            SelectedIndex = node.IsNull ? 0 : 1,
            IsEnabled = !node.IsReadOnly && !forceReadOnly
        };
        SetAutomationId(state, node, "NullableState");
        var operationError = CreateOperationError(node);
        var valueHost = new StackPanel();
        SetAutomationId(valueHost, node, "NullableValue");

        void RefreshValue()
        {
            valueHost.Children.Clear();
            if (!node.IsNull && node.Value is not null)
            {
                valueHost.Children.Add(CreateDeferredChild(node.Value, pickReferenceAsync, forceReadOnly || node.IsReadOnly));
            }
        }

        state.SelectionChanged += (_, _) =>
        {
            if (state.SelectedIndex == 0)
            {
                node.SetNull();
                operationError.Text = string.Empty;
            }
            else if (state.SelectedIndex == 1)
            {
                PublishOperationResult(node.SetPresent(), operationError);
            }
        };
        node.PropertyChanged += (_, eventArgs) =>
        {
            if (string.Equals(eventArgs.PropertyName, nameof(RecordWireNullableDraftNode.IsNull), StringComparison.Ordinal)
                || string.Equals(eventArgs.PropertyName, nameof(RecordWireNullableDraftNode.Value), StringComparison.Ordinal))
            {
                state.SelectedIndex = node.IsNull ? 0 : 1;
                RefreshValue();
            }
        };
        RefreshValue();
        return new StackPanel
        {
            Spacing = 8,
            Children =
            {
                state,
                operationError,
                valueHost
            }
        };
    }

    /// <summary>Creates one Boolean editor.</summary>
    /// <param name="node">The Boolean node.</param>
    /// <param name="forceReadOnly">Whether owner context prevents editing.</param>
    /// <returns>The Boolean checkbox.</returns>
    private static Control CreateBooleanEditor(RecordWireBooleanDraftNode node, bool forceReadOnly)
    {
        var checkBox = new CheckBox
        {
            Content = "Enabled",
            DataContext = node,
            IsEnabled = !node.IsReadOnly && !forceReadOnly
        };
        checkBox.Bind(ToggleButton.IsCheckedProperty, new Binding(nameof(RecordWireBooleanDraftNode.Value))
        {
            Mode = BindingMode.TwoWay
        });
        SetAutomationId(checkBox, node, "Boolean");
        return checkBox;
    }

    /// <summary>Creates one canonical integer text editor.</summary>
    /// <param name="node">The integer node.</param>
    /// <param name="forceReadOnly">Whether owner context prevents editing.</param>
    /// <returns>The exact decimal text editor.</returns>
    private static Control CreateIntegerEditor(RecordWireIntegerDraftNode node, bool forceReadOnly)
    {
        var textBox = CreateBoundTextBox(node, nameof(RecordWireIntegerDraftNode.Text), forceReadOnly, acceptsReturn: false);
        textBox.PlaceholderText = "Canonical decimal integer";
        SetAutomationId(textBox, node, "Integer");
        return textBox;
    }

    /// <summary>Creates one bounded string or exact Base64 text editor.</summary>
    /// <param name="node">The string node.</param>
    /// <param name="forceReadOnly">Whether owner context prevents editing.</param>
    /// <param name="acceptsReturn">Whether multiline text is appropriate.</param>
    /// <returns>The string editor.</returns>
    private static Control CreateStringEditor(RecordWireStringDraftNode node, bool forceReadOnly, bool acceptsReturn)
    {
        var textBox = CreateBoundTextBox(node, nameof(RecordWireStringDraftNode.Value), forceReadOnly, acceptsReturn);
        if (node.Descriptor.MaximumLength.HasValue)
        {
            textBox.MaxLength = node.Descriptor.MaximumLength.Value;
        }

        textBox.PlaceholderText = node.Kind == RecordWireDraftNodeKind.ByteArray ? "Base64 bytes" : node.Descriptor.Format;
        SetAutomationId(textBox, node, node.Kind == RecordWireDraftNodeKind.ByteArray ? "Bytes" : "String");
        return textBox;
    }

    /// <summary>Creates a known-symbol selector alongside canonical unknown-enum integer input.</summary>
    /// <param name="node">The enum node.</param>
    /// <param name="forceReadOnly">Whether owner context prevents editing.</param>
    /// <returns>The enum selector and exact integer editor.</returns>
    private static Control CreateEnumEditor(RecordWireEnumDraftNode node, bool forceReadOnly)
    {
        var known = new ComboBox
        {
            ItemsSource = node.KnownValues,
            IsEnabled = !node.IsReadOnly && !forceReadOnly,
            ItemTemplate = new FuncDataTemplate<RecordWireSchemaEnumValue>((value, _) =>
                CreateText(value is null ? string.Empty : $"{value.Name} ({value.Value})", 12, FontWeight.Normal))
        };
        SetAutomationId(known, node, "KnownEnum");
        var exact = CreateBoundTextBox(node, nameof(RecordWireEnumDraftNode.Text), forceReadOnly, acceptsReturn: false);
        exact.PlaceholderText = "Known or unknown canonical integer";
        SetAutomationId(exact, node, "EnumInteger");

        void RefreshKnownSelection()
        {
            known.SelectedItem = node.KnownValues.FirstOrDefault(value => string.Equals(value.Value, node.Text, StringComparison.Ordinal));
        }

        known.SelectionChanged += (_, _) =>
        {
            if (known.SelectedItem is RecordWireSchemaEnumValue value)
            {
                node.Text = value.Value;
            }
        };
        node.PropertyChanged += (_, eventArgs) =>
        {
            if (string.Equals(eventArgs.PropertyName, nameof(RecordWireEnumDraftNode.Text), StringComparison.Ordinal))
            {
                RefreshKnownSelection();
            }
        };
        RefreshKnownSelection();
        return new StackPanel
        {
            Spacing = 6,
            Children =
            {
                known,
                exact
            }
        };
    }

    /// <summary>Creates a FormLink editor that preserves JSON-null, canonical-null, and concrete identities.</summary>
    /// <param name="node">The FormLink node.</param>
    /// <param name="pickReferenceAsync">The optional existing reference-picker bridge.</param>
    /// <param name="forceReadOnly">Whether owner context prevents editing.</param>
    /// <returns>The complete FormLink editor.</returns>
    private static Control CreateFormLinkEditor(
        RecordWireFormLinkDraftNode node,
        Func<RecordWireFormLinkDraftNode, Task>? pickReferenceAsync,
        bool forceReadOnly)
    {
        var isEnabled = !node.IsReadOnly && !forceReadOnly;
        var isNull = new CheckBox
        {
            Content = "Plugin link is null",
            DataContext = node,
            IsEnabled = isEnabled
        };
        isNull.Bind(ToggleButton.IsCheckedProperty, new Binding(nameof(RecordWireFormLinkDraftNode.IsNull))
        {
            Mode = BindingMode.TwoWay
        });
        SetAutomationId(isNull, node, "LinkIsNull");

        var identity = new ComboBox
        {
            ItemsSource = new[] { "JSON null", "Canonical Null string", "Concrete FormKey" },
            IsEnabled = isEnabled
        };
        SetAutomationId(identity, node, "LinkIdentity");
        var formKey = new TextBox
        {
            DataContext = node,
            PlaceholderText = "000123:Plugin.esm",
            IsEnabled = isEnabled
        };
        formKey.Bind(TextBox.TextProperty, new Binding(nameof(RecordWireFormLinkDraftNode.FormKey))
        {
            Mode = BindingMode.TwoWay
        });
        SetAutomationId(formKey, node, "FormKey");
        var pick = CreateActionButton("Find reference...", node, "PickReference");
        pick.IsEnabled = isEnabled && pickReferenceAsync is not null;
        if (pickReferenceAsync is not null)
        {
            pick.Click += async (_, _) => await pickReferenceAsync(node);
        }

        void RefreshIdentity()
        {
            identity.SelectedIndex = node.FormKey is null ? 0 : string.Equals(node.FormKey, "Null", StringComparison.Ordinal) ? 1 : 2;
            formKey.IsEnabled = isEnabled && identity.SelectedIndex == 2;
        }

        identity.SelectionChanged += (_, _) =>
        {
            if (!isEnabled)
            {
                return;
            }

            node.FormKey = identity.SelectedIndex switch
            {
                0 => null,
                1 => "Null",
                _ when node.FormKey is null || string.Equals(node.FormKey, "Null", StringComparison.Ordinal) => string.Empty,
                _ => node.FormKey
            };
            RefreshIdentity();
        };
        node.PropertyChanged += (_, eventArgs) =>
        {
            if (string.Equals(eventArgs.PropertyName, nameof(RecordWireFormLinkDraftNode.FormKey), StringComparison.Ordinal))
            {
                RefreshIdentity();
            }
        };
        RefreshIdentity();
        var input = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("*,Auto"),
            ColumnSpacing = 8,
            Children =
            {
                formKey,
                pick
            }
        };
        Grid.SetColumn(pick, 1);
        return new StackPanel
        {
            Spacing = 6,
            Children =
            {
                isNull,
                identity,
                input
            }
        };
    }

    /// <summary>Creates active and subordinate projections for an owner-driven FormLink-or-index value.</summary>
    /// <param name="node">The owner-mode-sensitive value.</param>
    /// <param name="pickReferenceAsync">The optional existing reference-picker bridge.</param>
    /// <param name="forceReadOnly">Whether a containing owner prevents editing.</param>
    /// <returns>The active-branch visualization.</returns>
    private static Control CreateFormLinkOrIndexEditor(
        RecordWireFormLinkOrIndexDraftNode node,
        Func<RecordWireFormLinkDraftNode, Task>? pickReferenceAsync,
        bool forceReadOnly)
    {
        var active = CreateText(string.Empty, 12, FontWeight.SemiBold);
        SetAutomationId(active, node, "ActiveBranch");
        var indexIsNull = new CheckBox
        {
            Content = "Index is null",
            DataContext = node
        };
        indexIsNull.Bind(ToggleButton.IsCheckedProperty, new Binding(nameof(RecordWireFormLinkOrIndexDraftNode.IsIndexNull))
        {
            Mode = BindingMode.TwoWay
        });
        SetAutomationId(indexIsNull, node, "IndexIsNull");
        var indexText = CreateBoundTextBox(node, nameof(RecordWireFormLinkOrIndexDraftNode.IndexText), forceReadOnly, acceptsReturn: false);
        indexText.PlaceholderText = "Canonical unsigned index";
        SetAutomationId(indexText, node, "Index");
        var indexPane = new Border
        {
            Padding = new Thickness(8),
            BorderBrush = App.GetApplicationBrush(App.BorderBrushKey),
            BorderThickness = new Thickness(1),
            Child = new StackPanel
            {
                Spacing = 5,
                Children =
                {
                    CreateText("Index projection", 12, FontWeight.SemiBold),
                    indexIsNull,
                    indexText
                }
            }
        };
        SetAutomationId(indexPane, node, "IndexProjection");
        var linkPane = new Border
        {
            Padding = new Thickness(8),
            BorderBrush = App.GetApplicationBrush(App.BorderBrushKey),
            BorderThickness = new Thickness(1),
            Child = new StackPanel
            {
                Spacing = 5,
                Children =
                {
                    CreateText("FormLink projection", 12, FontWeight.SemiBold),
                    CreateFormLinkEditor(node.Link, pickReferenceAsync, forceReadOnly: false)
                }
            }
        };
        SetAutomationId(linkPane, node, "LinkProjection");

        void RefreshAuthority()
        {
            active.Text = $"Active plugin branch: {FormatActiveBranch(node.ActiveBranch)}";
            var linkActive = node.UsesLink && !forceReadOnly;
            var indexActive = !node.UsesLink && !forceReadOnly;
            indexPane.Opacity = indexActive ? 1 : 0.5;
            linkPane.Opacity = linkActive ? 1 : 0.5;
            indexIsNull.IsEnabled = indexActive && !node.IsReadOnly;
            indexText.IsEnabled = indexActive && !node.IsReadOnly && !node.IsIndexNull;
            linkPane.IsEnabled = linkActive && !node.IsReadOnly;
        }

        node.PropertyChanged += (_, eventArgs) =>
        {
            if (string.Equals(eventArgs.PropertyName, nameof(RecordWireFormLinkOrIndexDraftNode.ActiveBranch), StringComparison.Ordinal)
                || string.Equals(eventArgs.PropertyName, nameof(RecordWireFormLinkOrIndexDraftNode.IsIndexNull), StringComparison.Ordinal))
            {
                RefreshAuthority();
            }
        };
        RefreshAuthority();
        return new StackPanel
        {
            Spacing = 7,
            Children =
            {
                active,
                indexPane,
                linkPane
            }
        };
    }

    /// <summary>Creates a specialized graph hint followed by its closed object fields.</summary>
    /// <param name="node">The specialized object node.</param>
    /// <param name="hint">The exact authoring guidance.</param>
    /// <param name="pickReferenceAsync">The optional reference-picker bridge.</param>
    /// <param name="forceReadOnly">Whether the containing owner prevents editing.</param>
    /// <returns>The specialized object editor.</returns>
    private static Control CreateSpecializedObjectEditor(
        RecordWireObjectDraftNode node,
        string hint,
        Func<RecordWireFormLinkDraftNode, Task>? pickReferenceAsync,
        bool forceReadOnly)
    {
        var guidance = CreateText(hint, 11, FontWeight.Normal);
        guidance.Opacity = 0.72;
        guidance.TextWrapping = TextWrapping.Wrap;
        return new StackPanel
        {
            Spacing = 7,
            Children =
            {
                guidance,
                CreateObjectEditor(node, pickReferenceAsync, forceReadOnly)
            }
        };
    }

    /// <summary>Creates a node-local validation issue list.</summary>
    /// <param name="node">The issue-owning typed node.</param>
    /// <returns>The bound issue list.</returns>
    private static Control CreateIssueList(RecordWireDraftNode node)
    {
        var issues = new ItemsControl
        {
            DataContext = node,
            ItemTemplate = new FuncDataTemplate<RecordWireDraftIssue>((issue, _) =>
            {
                var text = CreateText(issue is null ? string.Empty : $"{issue.Code}: {issue.Message}", 11, FontWeight.Normal);
                text.Foreground = new SolidColorBrush(Color.FromRgb(204, 73, 73));
                text.TextWrapping = TextWrapping.Wrap;
                return text;
            })
        };
        issues.Bind(ItemsControl.ItemsSourceProperty, new Binding(nameof(RecordWireDraftNode.Issues)));
        SetAutomationId(issues, node, "Issues");
        return issues;
    }

    /// <summary>Creates one hidden-until-used local collection or union operation error.</summary>
    /// <param name="node">The operation-owning node.</param>
    /// <returns>The local error text.</returns>
    private static TextBlock CreateOperationError(RecordWireDraftNode node)
    {
        var error = CreateText(string.Empty, 11, FontWeight.Normal);
        error.Foreground = new SolidColorBrush(Color.FromRgb(204, 73, 73));
        error.TextWrapping = TextWrapping.Wrap;
        SetAutomationId(error, node, "OperationError");
        return error;
    }

    /// <summary>Publishes a typed node mutation result without interpreting its payload.</summary>
    /// <typeparam name="T">The successful typed result.</typeparam>
    /// <param name="result">The node-owned operation result.</param>
    /// <param name="error">The local error surface.</param>
    private static void PublishOperationResult<T>(CreationsForge.Core.Engine.Contracts.EngineResult<T> result, TextBlock error)
    {
        error.Text = result.Succeeded ? string.Empty : result.Error?.Message ?? "The typed draft operation failed.";
    }

    /// <summary>Creates one two-way text editor bound directly to a typed node property.</summary>
    /// <param name="node">The typed binding source.</param>
    /// <param name="propertyName">The exact typed property name.</param>
    /// <param name="forceReadOnly">Whether owner context prevents editing.</param>
    /// <param name="acceptsReturn">Whether the editor is multiline.</param>
    /// <returns>The configured text box.</returns>
    private static TextBox CreateBoundTextBox(
        RecordWireDraftNode node,
        string propertyName,
        bool forceReadOnly,
        bool acceptsReturn)
    {
        var textBox = new TextBox
        {
            DataContext = node,
            IsEnabled = !node.IsReadOnly && !forceReadOnly,
            AcceptsReturn = acceptsReturn,
            TextWrapping = acceptsReturn ? TextWrapping.Wrap : TextWrapping.NoWrap,
            MinHeight = acceptsReturn ? 64 : 0
        };
        textBox.Bind(TextBox.TextProperty, new Binding(propertyName)
        {
            Mode = BindingMode.TwoWay
        });
        return textBox;
    }

    /// <summary>Creates an observable ordered-collection count.</summary>
    /// <param name="node">The ordered array draft.</param>
    /// <param name="collectionName">The visible collection name.</param>
    /// <returns>The live ordered-value count.</returns>
    private static TextBlock CreateCollectionCount(RecordWireArrayDraftNode node, string collectionName)
    {
        var count = CreateText(string.Empty, 12, FontWeight.SemiBold);
        count.DataContext = node;
        count.Bind(TextBlock.TextProperty, new Binding($"{nameof(RecordWireArrayDraftNode.Items)}.Count")
        {
            StringFormat = $"{collectionName}: {{0}} ordered value(s)"
        });
        return count;
    }

    /// <summary>Creates one compact action button with path-bound automation identity.</summary>
    /// <param name="content">The visible action text.</param>
    /// <param name="node">The typed node that owns the action.</param>
    /// <param name="role">The action role within the node.</param>
    /// <returns>The configured action button.</returns>
    private static Button CreateActionButton(string content, RecordWireDraftNode node, string role)
    {
        var button = new Button
        {
            Content = content,
            Padding = new Thickness(10, 5),
            HorizontalAlignment = HorizontalAlignment.Left
        };
        SetAutomationId(button, node, role);
        return button;
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

    /// <summary>Formats one lightweight union option without resolving its graph.</summary>
    /// <param name="option">The lightweight option.</param>
    /// <returns>The searchable display identity.</returns>
    private static string FormatUnionOption(RecordWireSchemaUnionOption option)
    {
        return string.IsNullOrWhiteSpace(option.Discriminator)
            ? option.DisplayName
            : $"{option.DisplayName} · {option.Discriminator}";
    }

    /// <summary>Formats the owner-selected active FormLink-or-index branch.</summary>
    /// <param name="branch">The owner-derived branch.</param>
    /// <returns>The visible branch label.</returns>
    private static string FormatActiveBranch(RecordWireFormLinkOrIndexActiveBranch branch)
    {
        return branch switch
        {
            RecordWireFormLinkOrIndexActiveBranch.Link => "FormLink",
            RecordWireFormLinkOrIndexActiveBranch.AliasIndex => "Alias index",
            RecordWireFormLinkOrIndexActiveBranch.PackageDataIndex => "Package-data index",
            _ => branch.ToString()
        };
    }

    /// <summary>Binds a stable path-based automation identity to one interactive or diagnostic control.</summary>
    /// <param name="control">The control that exposes the identity.</param>
    /// <param name="node">The exact typed node.</param>
    /// <param name="role">The control role within the node.</param>
    private static void SetAutomationId(Control control, RecordWireDraftNode node, string role)
    {
        control.Bind(AutomationProperties.AutomationIdProperty, new Binding(nameof(RecordWireDraftNode.Path))
        {
            Source = node,
            StringFormat = $"RecordWire{role}:{{0}}"
        });
    }
}
