using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.NativeEditing.Drafts;
using CreationsForge.NativeEditing.Schema;
using CreationsForge.Views.NativeEditing;
using Shouldly;

namespace CreationsForge.PresentationTests.Headless;

/// <summary>Verifies typed native draft controls preserve lazy, ordered, and exact-value interactions.</summary>
[Collection(AvaloniaControlTestCollection.Name)]
public sealed class NativeWireDraftControlHeadlessTests
{
    /// <summary>Verifies every closed node kind has a code-native typed control path.</summary>
    [AvaloniaFact]
    public void NativeWireDraftControlFactory_WithEveryClosedNodeKind_CreatesTypedControls()
    {
        foreach (var node in CreateEveryNodeKind())
        {
            NativeWireDraftControlFactory.Create(node).ShouldNotBeNull();
        }
    }

    /// <summary>Verifies union search remains descriptor-only until one explicit type is selected.</summary>
    [AvaloniaFact]
    public void UnionControl_SearchLargeChoiceSet_MaterializesOnlySelectedOption()
    {
        var materializations = 0;
        var options = Enumerable.Range(0, 608)
            .Select(index => new NativeWireSchemaUnionOption(
                $"option-{index:D3}",
                $"Condition data {index:D3}",
                $"ConditionData{index:D3}",
                (_, _) => EngineResult<NativeWireSchemaDescriptor>.Success(CreateDescriptor(NativeWireSchemaValueKind.String))))
            .ToArray();
        var union = new NativeWireUnionDraftNode(
            "$.data",
            "Data",
            CreateDescriptor(NativeWireSchemaValueKind.Union, unionOptions: options),
            isRequired: true,
            isReadOnly: false,
            NativeWireDraftValueState.RequiredUnset,
            options,
            selectedOption: null,
            selectedValue: null,
            (_, option, _) =>
            {
                materializations++;
                return EngineResult<NativeWireDraftNode>.Success(CreateString("$.data.value", option.DisplayName));
            });
        var view = NativeWireDraftControlFactory.Create(union);
        var window = Show(view);

        try
        {
            var choices = ControlFinder.FindByAutomationId<ComboBox>(view, "NativeWireUnionOptions:$.data").ShouldNotBeNull();
            choices.ItemCount.ShouldBe(100);
            materializations.ShouldBe(0);
            var searchText = ControlFinder.FindByAutomationId<TextBox>(view, "NativeWireUnionSearch:$.data").ShouldNotBeNull();
            searchText.Text = "607";
            RaiseClick(ControlFinder.FindByAutomationId<Button>(view, "NativeWireSearchUnionOptions:$.data").ShouldNotBeNull());
            choices.ItemCount.ShouldBe(1);
            choices.SelectedIndex = 0;

            RaiseClick(ControlFinder.FindByAutomationId<Button>(view, "NativeWireSelectUnionOption:$.data").ShouldNotBeNull());

            materializations.ShouldBe(1);
            union.SelectedOption.ShouldNotBeNull().Key.ShouldBe("option-607");
            union.SelectedValue.ShouldNotBeNull();
        }
        finally
        {
            window.Close();
        }
    }

    /// <summary>Verifies array buttons preserve ordered identities while inserting, moving, replacing, removing, and clearing.</summary>
    [AvaloniaFact]
    public void ArrayControl_OrderedActions_MutateThroughTypedArrayOperations()
    {
        var first = CreateString("$.items[0]", "First");
        var second = CreateString("$.items[1]", "Second");
        var created = 0;
        var array = new NativeWireArrayDraftNode(
            "$.items",
            "Items",
            CreateDescriptor(NativeWireSchemaValueKind.Array, itemDescriptor: CreateDescriptor(NativeWireSchemaValueKind.String)),
            isRequired: true,
            isReadOnly: false,
            NativeWireDraftValueState.Seeded,
            [first, second],
            (index, _) => EngineResult<NativeWireDraftNode>.Success(CreateString($"$.items[new-{created++}]", $"Created at {index}")));
        var view = NativeWireDraftControlFactory.Create(array);
        var window = Show(view);

        try
        {
            RaiseClick(ControlFinder.FindByAutomationId<Button>(view, "NativeWireAppend:$.items").ShouldNotBeNull());
            Dispatcher.UIThread.RunJobs();
            array.Items.Count.ShouldBe(3);

            RaiseClick(ControlFinder.FindByAutomationId<Button>(view, "NativeWireMoveUp:$.items[1]").ShouldNotBeNull());
            Dispatcher.UIThread.RunJobs();
            array.Items[0].ShouldBeSameAs(second);
            ControlFinder.FindByAutomationId<Button>(view, "NativeWireMoveDown:$.items[0]").ShouldNotBeNull();

            RaiseClick(ControlFinder.FindByAutomationId<Button>(view, "NativeWireReplace:$.items[0]").ShouldNotBeNull());
            Dispatcher.UIThread.RunJobs();
            array.Items[0].ShouldNotBeSameAs(second);

            RaiseClick(ControlFinder.FindByAutomationId<Button>(view, "NativeWireClear:$.items").ShouldNotBeNull());
            array.Items.ShouldBeEmpty();
        }
        finally
        {
            window.Close();
        }
    }

    /// <summary>Verifies the nullable wrapper and FormLink editor retain all three null-related identities.</summary>
    [AvaloniaFact]
    public void FormLinkControls_ExplicitStates_PreserveWholeAndNestedNullIdentity()
    {
        var link = CreateFormLink("$.value", isNull: true, formKey: "Null");
        var nullable = new NativeWireNullableDraftNode(
            "$.nullable",
            "Nullable link",
            CreateDescriptor(NativeWireSchemaValueKind.Nullable, nonNullDescriptor: link.Descriptor, allowsNull: true),
            isRequired: true,
            isReadOnly: false,
            NativeWireDraftValueState.Seeded,
            isNull: false,
            link,
            (_, _) => EngineResult<NativeWireDraftNode>.Success(link));
        var view = NativeWireDraftControlFactory.Create(nullable);
        var window = Show(view);

        try
        {
            var wrapperState = ControlFinder.FindByAutomationId<ComboBox>(view, "NativeWireNullableState:$.nullable").ShouldNotBeNull();
            wrapperState.SelectedIndex = 0;
            nullable.IsNull.ShouldBeTrue();
            wrapperState.SelectedIndex = 1;
            nullable.IsNull.ShouldBeFalse();
            nullable.Value.ShouldBeSameAs(link);

            var identity = ControlFinder.FindByAutomationId<ComboBox>(view, "NativeWireLinkIdentity:$.value").ShouldNotBeNull();
            identity.SelectedIndex = 0;
            link.FormKey.ShouldBeNull();
            identity.SelectedIndex = 1;
            link.FormKey.ShouldBe("Null");
            identity.SelectedIndex = 2;
            link.FormKey.ShouldBe(string.Empty);
            var formKey = ControlFinder.FindByAutomationId<TextBox>(view, "NativeWireFormKey:$.value").ShouldNotBeNull();
            formKey.Text = "000123:Source.esm";
            link.FormKey.ShouldBe("000123:Source.esm");
        }
        finally
        {
            window.Close();
        }
    }

    /// <summary>Verifies FormLink-or-index authority follows the owner-published branch and subordinates the inactive projection.</summary>
    [AvaloniaFact]
    public void FormLinkOrIndexControl_WhenOwnerModeChanges_RefreshesActiveProjection()
    {
        var node = new NativeWireFormLinkOrIndexDraftNode(
            "$.parameter",
            "Parameter",
            CreateDescriptor(NativeWireSchemaValueKind.FormLinkOrIndex),
            isRequired: true,
            isReadOnly: false,
            NativeWireDraftValueState.Seeded,
            isIndexNull: false,
            "4",
            CreateFormLink("$.parameter.link", isNull: false, formKey: "000123:Source.esm"),
            NativeWireFormLinkOrIndexActiveBranch.Link);
        var view = NativeWireDraftControlFactory.Create(node);
        var window = Show(view);

        try
        {
            var active = ControlFinder.FindByAutomationId<TextBlock>(view, "NativeWireActiveBranch:$.parameter").ShouldNotBeNull();
            var index = ControlFinder.FindByAutomationId<Border>(view, "NativeWireIndexProjection:$.parameter").ShouldNotBeNull();
            var link = ControlFinder.FindByAutomationId<Border>(view, "NativeWireLinkProjection:$.parameter").ShouldNotBeNull();
            active.Text.ShouldBe("Active native branch: FormLink");
            index.Opacity.ShouldBe(0.5);
            link.Opacity.ShouldBe(1);
            link.IsEnabled.ShouldBeTrue();

            node.RefreshOwnerMode(usesAlias: true, usesPackageData: false);

            active.Text.ShouldBe("Active native branch: Alias index");
            index.Opacity.ShouldBe(1);
            link.Opacity.ShouldBe(0.5);
            link.IsEnabled.ShouldBeFalse();
        }
        finally
        {
            window.Close();
        }
    }

    /// <summary>Verifies a nullable unsigned index retains exact text while toggling explicit JSON-null presence.</summary>
    [AvaloniaFact]
    public void FormLinkOrIndexControl_NullableUnsignedIndex_PreservesNullAndRetainedText()
    {
        var node = new NativeWireFormLinkOrIndexDraftNode(
            "$.parameter",
            "Parameter",
            CreateDescriptor(NativeWireSchemaValueKind.FormLinkOrIndex),
            isRequired: true,
            isReadOnly: false,
            NativeWireDraftValueState.Seeded,
            isIndexNull: true,
            string.Empty,
            CreateFormLink("$.parameter.link", isNull: false, formKey: "000123:Source.esm"),
            NativeWireFormLinkOrIndexActiveBranch.AliasIndex);
        var view = NativeWireDraftControlFactory.Create(node);
        var window = Show(view);

        try
        {
            var isNull = ControlFinder.FindByAutomationId<CheckBox>(view, "NativeWireIndexIsNull:$.parameter").ShouldNotBeNull();
            var index = ControlFinder.FindByAutomationId<TextBox>(view, "NativeWireIndex:$.parameter").ShouldNotBeNull();
            isNull.IsChecked.ShouldBe(true);
            index.IsEnabled.ShouldBeFalse();

            isNull.IsChecked = false;
            Dispatcher.UIThread.RunJobs();
            node.IsIndexNull.ShouldBeFalse();
            index.IsEnabled.ShouldBeTrue();
            index.Text = uint.MaxValue.ToString(System.Globalization.CultureInfo.InvariantCulture);
            Dispatcher.UIThread.RunJobs();
            node.IndexText.ShouldBe("4294967295");

            isNull.IsChecked = true;
            Dispatcher.UIThread.RunJobs();
            node.IsIndexNull.ShouldBeTrue();
            node.IndexText.ShouldBe("4294967295");
            index.IsEnabled.ShouldBeFalse();
        }
        finally
        {
            window.Close();
        }
    }

    /// <summary>Verifies a populated nested typed draft remains inspectable and optionally writes its rendered review artifact.</summary>
    [AvaloniaFact]
    public void NativeWireDraftControl_PopulatedNestedDraft_RendersReviewArtifact()
    {
        var draft = CreatePopulatedReviewDraft();
        var view = NativeWireDraftControlFactory.Create(draft);
        var window = new Window
        {
            Width = 1200,
            Height = 1100,
            Content = new ScrollViewer
            {
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Content = view
            }
        };

        try
        {
            window.Show();
            ExpandAll(view);

            ControlFinder.FindByAutomationId<ItemsControl>(view, "NativeWireItems:$.items").ShouldNotBeNull();
            ControlFinder.FindByAutomationId<ComboBox>(view, "NativeWireUnionOptions:$.component").ShouldNotBeNull();
            ControlFinder.FindByAutomationId<TextBlock>(view, "NativeWireActiveBranch:$.component.parameter").ShouldNotBeNull();
            var issueText = view.GetVisualDescendants()
                .OfType<TextBlock>()
                .Single(text => text.Text?.Contains("EditorID is required", StringComparison.Ordinal) == true);
            issueText.IsEffectivelyVisible.ShouldBeTrue();

            if (HeadlessTestApp.UsesRenderedArtifactRenderer)
            {
                using var bitmap = window.CaptureRenderedFrame().ShouldNotBeNull();
                var screenshotPath = GetReviewScreenshotPath();
                Directory.CreateDirectory(Path.GetDirectoryName(screenshotPath)!);
                bitmap.Save(screenshotPath, PngBitmapEncoderOptions.Default);
                new FileInfo(screenshotPath).Length.ShouldBeGreaterThan(0L);
            }
        }
        finally
        {
            window.Close();
        }
    }

    /// <summary>Creates one representative node for every closed control-factory branch.</summary>
    /// <returns>The complete closed node-kind set.</returns>
    private static IReadOnlyList<NativeWireDraftNode> CreateEveryNodeKind()
    {
        var stringNode = CreateString("$.string", "Text");
        var option = new NativeWireSchemaUnionOption(
            "text",
            "Text",
            discriminator: null,
            (_, _) => EngineResult<NativeWireSchemaDescriptor>.Success(CreateDescriptor(NativeWireSchemaValueKind.String)));
        var formLink = CreateFormLink("$.link", isNull: true, formKey: null);
        return
        [
            new NativeWireObjectDraftNode(NativeWireDraftNodeKind.Object, "$.object", "Object", CreateDescriptor(NativeWireSchemaValueKind.Object), false, false, NativeWireDraftValueState.Seeded, []),
            new NativeWireArrayDraftNode("$.array", "Array", CreateDescriptor(NativeWireSchemaValueKind.Array, itemDescriptor: stringNode.Descriptor), false, false, NativeWireDraftValueState.Seeded, [], (_, _) => EngineResult<NativeWireDraftNode>.Success(CreateString("$.array[0]", "Item"))),
            new NativeWireUnionDraftNode("$.union", "Union", CreateDescriptor(NativeWireSchemaValueKind.Union, unionOptions: [option]), true, false, NativeWireDraftValueState.RequiredUnset, [option], null, null, (_, _, _) => EngineResult<NativeWireDraftNode>.Success(CreateString("$.union.value", "Choice"))),
            new NativeWireNullableDraftNode("$.nullable", "Nullable", CreateDescriptor(NativeWireSchemaValueKind.Nullable, nonNullDescriptor: stringNode.Descriptor, allowsNull: true), false, false, NativeWireDraftValueState.Seeded, true, stringNode, (_, _) => EngineResult<NativeWireDraftNode>.Success(stringNode)),
            new NativeWireBooleanDraftNode("$.boolean", "Boolean", CreateDescriptor(NativeWireSchemaValueKind.Boolean), false, false, NativeWireDraftValueState.Defaulted, false),
            new NativeWireIntegerDraftNode(NativeWireDraftNodeKind.Integer, "$.integer", "Integer", CreateDescriptor(NativeWireSchemaValueKind.Integer), false, false, NativeWireDraftValueState.Defaulted, "0"),
            stringNode,
            new NativeWireEnumDraftNode("$.enum", "Enum", CreateDescriptor(NativeWireSchemaValueKind.Enum, enumValues: [new NativeWireSchemaEnumValue("Known", "1")]), false, false, NativeWireDraftValueState.Defaulted, "1"),
            formLink,
            new NativeWireFormLinkOrIndexDraftNode("$.linkOrIndex", "Link or index", CreateDescriptor(NativeWireSchemaValueKind.FormLinkOrIndex), false, false, NativeWireDraftValueState.Seeded, false, "0", CreateFormLink("$.linkOrIndex.link", false, "000123:Source.esm"), NativeWireFormLinkOrIndexActiveBranch.Link),
            new NativeWireFloatBitsDraftNode("$.floatBits", "Float bits", CreateDescriptor(NativeWireSchemaValueKind.FloatBits), false, false, NativeWireDraftValueState.Seeded, []),
            new NativeWireByteArrayDraftNode("$.bytes", "Bytes", CreateDescriptor(NativeWireSchemaValueKind.ByteArray), false, false, NativeWireDraftValueState.Seeded, []),
            new NativeWireTranslatedStringDraftNode("$.translated", "Translated", CreateDescriptor(NativeWireSchemaValueKind.TranslatedString), false, false, NativeWireDraftValueState.Seeded, []),
            new NativeWireAssetDraftNode("$.asset", "Asset", CreateDescriptor(NativeWireSchemaValueKind.Asset), false, false, NativeWireDraftValueState.Seeded, []),
            new NativeWireColorDraftNode("$.color", "Color", CreateDescriptor(NativeWireSchemaValueKind.Color), false, false, NativeWireDraftValueState.Seeded, []),
            new NativeWireArray2DDraftNode("$.array2d", "Array 2D", CreateDescriptor(NativeWireSchemaValueKind.Array2D), false, false, NativeWireDraftValueState.Seeded, [])
        ];
    }

    /// <summary>Creates the populated nested typed draft used for visual review.</summary>
    /// <returns>A draft containing ordered links, a selected component union, owner-driven projections, and a mapped issue.</returns>
    private static NativeWireObjectDraftNode CreatePopulatedReviewDraft()
    {
        var editorId = new NativeWireStringDraftNode(
            NativeWireDraftNodeKind.String,
            "$.editorId",
            "Editor ID",
            CreateDescriptor(NativeWireSchemaValueKind.String),
            isRequired: true,
            isReadOnly: false,
            NativeWireDraftValueState.RequiredUnset,
            string.Empty);
        editorId.SetIssues(
        [
            new NativeWireDraftIssue(
                NativeWireDraftIssueCode.RequiredValueUnset,
                editorId.Path,
                "EditorID is required before this typed command can be applied.",
                editorId)
        ]);

        var items = new NativeWireArrayDraftNode(
            "$.items",
            "Items",
            CreateDescriptor(NativeWireSchemaValueKind.Array, itemDescriptor: CreateDescriptor(NativeWireSchemaValueKind.FormLink)),
            isRequired: true,
            isReadOnly: false,
            NativeWireDraftValueState.Seeded,
            [
                CreateFormLink("$.items[0]", isNull: false, formKey: "001234:Starfield.esm"),
                CreateFormLink("$.items[1]", isNull: true, formKey: "Null")
            ],
            (index, _) => EngineResult<NativeWireDraftNode>.Success(CreateFormLink($"$.items[{index}]", isNull: true, formKey: null)));

        var componentOption = new NativeWireSchemaUnionOption(
            "example-component",
            "Example component",
            "ExampleComponent",
            (_, _) => EngineResult<NativeWireSchemaDescriptor>.Success(CreateDescriptor(NativeWireSchemaValueKind.Object)));
        var parameter = new NativeWireFormLinkOrIndexDraftNode(
            "$.component.parameter",
            "Parameter",
            CreateDescriptor(NativeWireSchemaValueKind.FormLinkOrIndex),
            isRequired: true,
            isReadOnly: false,
            NativeWireDraftValueState.Seeded,
            isIndexNull: false,
            "3",
            CreateFormLink("$.component.parameter.link", isNull: false, formKey: "00ABCD:Example.esm"),
            NativeWireFormLinkOrIndexActiveBranch.Link);
        var component = new NativeWireObjectDraftNode(
            NativeWireDraftNodeKind.Object,
            "$.component",
            "Example component",
            CreateDescriptor(NativeWireSchemaValueKind.Object),
            isRequired: true,
            isReadOnly: false,
            NativeWireDraftValueState.Seeded,
            [
                new NativeWireObjectDraftProperty("label", CreateString("$.component.label", "Populated nested component")),
                new NativeWireObjectDraftProperty("parameter", parameter)
            ]);
        var componentUnion = new NativeWireUnionDraftNode(
            "$.component",
            "Component",
            CreateDescriptor(NativeWireSchemaValueKind.Union, unionOptions: [componentOption]),
            isRequired: true,
            isReadOnly: false,
            NativeWireDraftValueState.Seeded,
            [componentOption],
            componentOption,
            component,
            (_, _, _) => EngineResult<NativeWireDraftNode>.Success(component));

        return new NativeWireObjectDraftNode(
            NativeWireDraftNodeKind.Object,
            "$",
            "Replace Items and inspect Component",
            CreateDescriptor(NativeWireSchemaValueKind.Object),
            isRequired: true,
            isReadOnly: false,
            NativeWireDraftValueState.Seeded,
            [
                new NativeWireObjectDraftProperty("editorId", editorId),
                new NativeWireObjectDraftProperty("items", items),
                new NativeWireObjectDraftProperty("component", componentUnion)
            ]);
    }

    /// <summary>Creates one minimal schema descriptor for a typed test node.</summary>
    /// <param name="kind">The closed schema kind.</param>
    /// <param name="unionOptions">Optional lightweight union options.</param>
    /// <param name="itemDescriptor">Optional array item descriptor.</param>
    /// <param name="nonNullDescriptor">Optional nullable non-null descriptor.</param>
    /// <param name="enumValues">Optional known enum values.</param>
    /// <param name="allowsNull">Whether whole JSON null is permitted.</param>
    /// <returns>The immutable descriptor.</returns>
    private static NativeWireSchemaDescriptor CreateDescriptor(
        NativeWireSchemaValueKind kind,
        IReadOnlyList<NativeWireSchemaUnionOption>? unionOptions = null,
        NativeWireSchemaDescriptor? itemDescriptor = null,
        NativeWireSchemaDescriptor? nonNullDescriptor = null,
        IReadOnlyList<NativeWireSchemaEnumValue>? enumValues = null,
        bool allowsNull = false)
    {
        return new NativeWireSchemaDescriptor(
            kind,
            kind.ToString(),
            description: null,
            unionOptions: unionOptions,
            itemDescriptor: itemDescriptor,
            nonNullDescriptor: nonNullDescriptor,
            enumValues: enumValues,
            allowsNull: allowsNull);
    }

    /// <summary>Creates one editable string draft.</summary>
    /// <param name="path">The exact test path.</param>
    /// <param name="value">The initial value.</param>
    /// <returns>The string draft.</returns>
    private static NativeWireStringDraftNode CreateString(string path, string value)
    {
        return new NativeWireStringDraftNode(
            NativeWireDraftNodeKind.String,
            path,
            "Text",
            CreateDescriptor(NativeWireSchemaValueKind.String),
            isRequired: true,
            isReadOnly: false,
            NativeWireDraftValueState.Seeded,
            value);
    }

    /// <summary>Creates one exact FormLink draft.</summary>
    /// <param name="path">The exact test path.</param>
    /// <param name="isNull">Whether the native link is null.</param>
    /// <param name="formKey">The exact JSON-null, canonical-null, or concrete FormKey representation.</param>
    /// <returns>The FormLink draft.</returns>
    private static NativeWireFormLinkDraftNode CreateFormLink(string path, bool isNull, string? formKey)
    {
        return new NativeWireFormLinkDraftNode(
            path,
            "Reference",
            CreateDescriptor(NativeWireSchemaValueKind.FormLink),
            isRequired: true,
            isReadOnly: false,
            NativeWireDraftValueState.Seeded,
            isNull,
            formKey);
    }

    /// <summary>Shows one control in a deterministic headless owner.</summary>
    /// <param name="content">The control to host.</param>
    /// <returns>The visible test window.</returns>
    private static Window Show(Control content)
    {
        var window = new Window
        {
            Width = 1100,
            Height = 800,
            Content = content
        };
        window.Show();
        Dispatcher.UIThread.RunJobs();
        return window;
    }

    /// <summary>Expands every currently materialized nested draft container for visual review.</summary>
    /// <param name="root">The typed draft control tree.</param>
    private static void ExpandAll(Control root)
    {
        for (var pass = 0; pass < 8; pass++)
        {
            var collapsed = root.GetVisualDescendants()
                .OfType<Expander>()
                .Where(expander => !expander.IsExpanded)
                .ToArray();
            if (collapsed.Length == 0)
            {
                return;
            }

            foreach (var expander in collapsed)
            {
                expander.IsExpanded = true;
            }

            Dispatcher.UIThread.RunJobs();
        }
    }

    /// <summary>Gets the repository-relative path for the populated typed-editor review image.</summary>
    /// <returns>The fully qualified PNG destination under the task-owned work directory.</returns>
    private static string GetReviewScreenshotPath()
    {
        return Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "..",
            "..",
            "..",
            "..",
            ".work",
            "NativeReplacement",
            "screenshots",
            "native-form-list-editor-populated.png"));
    }

    /// <summary>Raises one routed button click and drains synchronous UI work.</summary>
    /// <param name="button">The enabled action button.</param>
    private static void RaiseClick(Button button)
    {
        button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        Dispatcher.UIThread.RunJobs();
    }
}
