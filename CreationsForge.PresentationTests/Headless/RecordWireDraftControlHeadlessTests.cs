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
using CreationsForge.Core.Engine.RecordWire;
using CreationsForge.RecordEditing.Drafts;
using CreationsForge.RecordEditing.Schema;
using CreationsForge.Views.RecordEditing;
using Shouldly;

namespace CreationsForge.PresentationTests.Headless;

/// <summary>Verifies typed plugin draft controls preserve lazy, ordered, and exact-value interactions.</summary>
[Collection(AvaloniaControlTestCollection.Name)]
public sealed class RecordWireDraftControlHeadlessTests
{
    /// <summary>Verifies every closed node kind has a code-typed control path.</summary>
    [AvaloniaFact]
    public void RecordWireDraftControlFactory_WithEveryClosedNodeKind_CreatesTypedControls()
    {
        foreach (var node in CreateEveryNodeKind())
        {
            RecordWireDraftControlFactory.Create(node).ShouldNotBeNull();
        }
    }

    /// <summary>Verifies translated names edit through language rows and maintain the target-language wire value without JSON input.</summary>
    [AvaloniaFact]
    public void TranslatedStringControl_EditRows_KeepsTargetValueAndOrderedTranslationsConsistent()
    {
        var target = CreateString("$.name.targetLanguage", "English");
        var selectedText = CreateString("$.name.value", "Old name");
        var value = new RecordWireNullableDraftNode(
            "$.name.value", "value",
            CreateDescriptor(RecordWireSchemaValueKind.Nullable, nonNullDescriptor: selectedText.Descriptor, allowsNull: true),
            true, false, RecordWireDraftValueState.Seeded, false, selectedText,
            (path, _) => EngineResult<RecordWireDraftNode>.Success(CreateString(path, string.Empty)));
        var english = CreateTranslation("$.name.translations[0]", "English", "Old name");
        var french = CreateTranslation("$.name.translations[1]", "French", "Ancien nom");
        var translations = new RecordWireArrayDraftNode(
            "$.name.translations", "translations", CreateDescriptor(RecordWireSchemaValueKind.Array),
            true, false, RecordWireDraftValueState.Seeded, [english, french],
            (index, _) => EngineResult<RecordWireDraftNode>.Success(CreateTranslation($"$.name.translations[{index}]", string.Empty, string.Empty)));
        var name = new RecordWireTranslatedStringDraftNode(
            "$.name", "Name", CreateDescriptor(RecordWireSchemaValueKind.TranslatedString),
            true, false, RecordWireDraftValueState.Seeded,
            [
                new RecordWireObjectDraftProperty("targetLanguage", target),
                new RecordWireObjectDraftProperty("value", value),
                new RecordWireObjectDraftProperty("translations", translations)
            ]);
        var view = RecordWireDraftControlFactory.CreateFormField(name);
        var window = Show(view);

        try
        {
            var targetSelector = ControlFinder.FindByAutomationId<ComboBox>(view, "RecordWireTargetLanguage:$.name.targetLanguage").ShouldNotBeNull();
            targetSelector.ItemsSource!.Cast<string>().ShouldContain("English");
            targetSelector.ItemsSource!.Cast<string>().ShouldContain("French");
            targetSelector.SelectedItem = "French";
            selectedText.Value.ShouldBe("Ancien nom");

            var englishText = ControlFinder.FindByAutomationId<TextBox>(view, "RecordWireTranslationText:$.name.translations[0].value").ShouldNotBeNull();
            englishText.Text = "New name";
            Dispatcher.UIThread.RunJobs();
            english.FindProperty("value").ShouldBeOfType<RecordWireStringDraftNode>().Value.ShouldBe("New name");
            targetSelector.SelectedItem = "English";
            selectedText.Value.ShouldBe("New name");

            var newLanguage = RecordWireLanguageOptions.GetNames().First(language => language is not ("English" or "French"));
            targetSelector.SelectedItem = newLanguage;
            value.IsNull.ShouldBeTrue();
            RaiseClick(ControlFinder.FindByAutomationId<Button>(view, "RecordWireAddTranslation:$.name.translations").ShouldNotBeNull());
            translations.Items.Count.ShouldBe(3);
            var added = translations.Items[2].ShouldBeOfType<RecordWireObjectDraftNode>();
            added.FindProperty("language").ShouldBeOfType<RecordWireStringDraftNode>().Value.ShouldBe(newLanguage);
            ControlFinder.FindByAutomationId<TextBox>(view, "RecordWireTranslationText:$.name.translations[2].value")
                .ShouldNotBeNull().Text = "Third name";
            Dispatcher.UIThread.RunJobs();

            var json = RecordWireDraftJsonWriter.WriteDetached(name, CancellationToken.None);
            json.GetProperty("targetLanguage").GetString().ShouldBe(newLanguage);
            json.GetProperty("value").GetString().ShouldBe("Third name");
            json.GetProperty("translations").EnumerateArray().Select(item => item.GetProperty("language").GetString())
                .ShouldBe(["English", "French", newLanguage]);

            RaiseClick(ControlFinder.FindByAutomationId<Button>(view, "RecordWireTranslationRemove:$.name.translations[2]").ShouldNotBeNull());
            RecordWireDraftJsonWriter.WriteDetached(name, CancellationToken.None).GetProperty("value").ValueKind.ShouldBe(System.Text.Json.JsonValueKind.Null);
        }
        finally
        {
            window.Close();
        }
    }

    /// <summary>Verifies union search remains descriptor-only until one explicit type is selected.</summary>
    [AvaloniaFact]
    public void UnionControl_SearchLargeChoiceSet_MaterializesOnlySelectedOption()
    {
        var materializations = 0;
        var options = Enumerable.Range(0, 608)
            .Select(index => new RecordWireSchemaUnionOption(
                $"option-{index:D3}",
                $"Condition data {index:D3}",
                $"ConditionData{index:D3}",
                (_, _) => EngineResult<RecordWireSchemaDescriptor>.Success(CreateDescriptor(RecordWireSchemaValueKind.String))))
            .ToArray();
        var union = new RecordWireUnionDraftNode(
            "$.data",
            "Data",
            CreateDescriptor(RecordWireSchemaValueKind.Union, unionOptions: options),
            isRequired: true,
            isReadOnly: false,
            RecordWireDraftValueState.RequiredUnset,
            options,
            selectedOption: null,
            selectedValue: null,
            (_, option, _) =>
            {
                materializations++;
                return EngineResult<RecordWireDraftNode>.Success(CreateString("$.data.value", option.DisplayName));
            });
        var view = RecordWireDraftControlFactory.Create(union);
        var window = Show(view);

        try
        {
            var choices = ControlFinder.FindByAutomationId<ComboBox>(view, "RecordWireUnionOptions:$.data").ShouldNotBeNull();
            choices.ItemCount.ShouldBe(100);
            materializations.ShouldBe(0);
            var searchText = ControlFinder.FindByAutomationId<TextBox>(view, "RecordWireUnionSearch:$.data").ShouldNotBeNull();
            searchText.Text = "607";
            RaiseClick(ControlFinder.FindByAutomationId<Button>(view, "RecordWireSearchUnionOptions:$.data").ShouldNotBeNull());
            choices.ItemCount.ShouldBe(1);
            choices.SelectedIndex = 0;

            RaiseClick(ControlFinder.FindByAutomationId<Button>(view, "RecordWireSelectUnionOption:$.data").ShouldNotBeNull());

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
        var array = new RecordWireArrayDraftNode(
            "$.items",
            "Items",
            CreateDescriptor(RecordWireSchemaValueKind.Array, itemDescriptor: CreateDescriptor(RecordWireSchemaValueKind.String)),
            isRequired: true,
            isReadOnly: false,
            RecordWireDraftValueState.Seeded,
            [first, second],
            (index, _) => EngineResult<RecordWireDraftNode>.Success(CreateString($"$.items[new-{created++}]", $"Created at {index}")));
        var view = RecordWireDraftControlFactory.Create(array);
        var window = Show(view);

        try
        {
            RaiseClick(ControlFinder.FindByAutomationId<Button>(view, "RecordWireAppend:$.items").ShouldNotBeNull());
            Dispatcher.UIThread.RunJobs();
            array.Items.Count.ShouldBe(3);

            RaiseClick(ControlFinder.FindByAutomationId<Button>(view, "RecordWireMoveUp:$.items[1]").ShouldNotBeNull());
            Dispatcher.UIThread.RunJobs();
            array.Items[0].ShouldBeSameAs(second);
            ControlFinder.FindByAutomationId<Button>(view, "RecordWireMoveDown:$.items[0]").ShouldNotBeNull();

            RaiseClick(ControlFinder.FindByAutomationId<Button>(view, "RecordWireReplace:$.items[0]").ShouldNotBeNull());
            Dispatcher.UIThread.RunJobs();
            array.Items[0].ShouldNotBeSameAs(second);

            RaiseClick(ControlFinder.FindByAutomationId<Button>(view, "RecordWireClear:$.items").ShouldNotBeNull());
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
        var nullable = new RecordWireNullableDraftNode(
            "$.nullable",
            "Nullable link",
            CreateDescriptor(RecordWireSchemaValueKind.Nullable, nonNullDescriptor: link.Descriptor, allowsNull: true),
            isRequired: true,
            isReadOnly: false,
            RecordWireDraftValueState.Seeded,
            isNull: false,
            link,
            (_, _) => EngineResult<RecordWireDraftNode>.Success(link));
        var view = RecordWireDraftControlFactory.Create(nullable);
        var window = Show(view);

        try
        {
            var wrapperState = ControlFinder.FindByAutomationId<ComboBox>(view, "RecordWireNullableState:$.nullable").ShouldNotBeNull();
            wrapperState.SelectedIndex = 0;
            nullable.IsNull.ShouldBeTrue();
            wrapperState.SelectedIndex = 1;
            nullable.IsNull.ShouldBeFalse();
            nullable.Value.ShouldBeSameAs(link);

            var identity = ControlFinder.FindByAutomationId<ComboBox>(view, "RecordWireLinkIdentity:$.value").ShouldNotBeNull();
            identity.SelectedIndex = 0;
            link.FormKey.ShouldBeNull();
            identity.SelectedIndex = 1;
            link.FormKey.ShouldBe("Null");
            identity.SelectedIndex = 2;
            link.FormKey.ShouldBe(string.Empty);
            var formKey = ControlFinder.FindByAutomationId<TextBox>(view, "RecordWireFormKey:$.value").ShouldNotBeNull();
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
        var node = new RecordWireFormLinkOrIndexDraftNode(
            "$.parameter",
            "Parameter",
            CreateDescriptor(RecordWireSchemaValueKind.FormLinkOrIndex),
            isRequired: true,
            isReadOnly: false,
            RecordWireDraftValueState.Seeded,
            isIndexNull: false,
            "4",
            CreateFormLink("$.parameter.link", isNull: false, formKey: "000123:Source.esm"),
            RecordWireFormLinkOrIndexActiveBranch.Link);
        var view = RecordWireDraftControlFactory.Create(node);
        var window = Show(view);

        try
        {
            var active = ControlFinder.FindByAutomationId<TextBlock>(view, "RecordWireActiveBranch:$.parameter").ShouldNotBeNull();
            var index = ControlFinder.FindByAutomationId<Border>(view, "RecordWireIndexProjection:$.parameter").ShouldNotBeNull();
            var link = ControlFinder.FindByAutomationId<Border>(view, "RecordWireLinkProjection:$.parameter").ShouldNotBeNull();
            active.Text.ShouldBe("Active plugin branch: FormLink");
            index.Opacity.ShouldBe(0.5);
            link.Opacity.ShouldBe(1);
            link.IsEnabled.ShouldBeTrue();

            node.RefreshOwnerMode(usesAlias: true, usesPackageData: false);

            active.Text.ShouldBe("Active plugin branch: Alias index");
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
        var node = new RecordWireFormLinkOrIndexDraftNode(
            "$.parameter",
            "Parameter",
            CreateDescriptor(RecordWireSchemaValueKind.FormLinkOrIndex),
            isRequired: true,
            isReadOnly: false,
            RecordWireDraftValueState.Seeded,
            isIndexNull: true,
            string.Empty,
            CreateFormLink("$.parameter.link", isNull: false, formKey: "000123:Source.esm"),
            RecordWireFormLinkOrIndexActiveBranch.AliasIndex);
        var view = RecordWireDraftControlFactory.Create(node);
        var window = Show(view);

        try
        {
            var isNull = ControlFinder.FindByAutomationId<CheckBox>(view, "RecordWireIndexIsNull:$.parameter").ShouldNotBeNull();
            var index = ControlFinder.FindByAutomationId<TextBox>(view, "RecordWireIndex:$.parameter").ShouldNotBeNull();
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
    public void RecordWireDraftControl_PopulatedNestedDraft_RendersReviewArtifact()
    {
        var draft = CreatePopulatedReviewDraft();
        var view = RecordWireDraftControlFactory.Create(draft);
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

            ControlFinder.FindByAutomationId<ItemsControl>(view, "RecordWireItems:$.items").ShouldNotBeNull();
            ControlFinder.FindByAutomationId<ComboBox>(view, "RecordWireUnionOptions:$.component").ShouldNotBeNull();
            ControlFinder.FindByAutomationId<TextBlock>(view, "RecordWireActiveBranch:$.component.parameter").ShouldNotBeNull();
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
    private static IReadOnlyList<RecordWireDraftNode> CreateEveryNodeKind()
    {
        var stringNode = CreateString("$.string", "Text");
        var option = new RecordWireSchemaUnionOption(
            "text",
            "Text",
            discriminator: null,
            (_, _) => EngineResult<RecordWireSchemaDescriptor>.Success(CreateDescriptor(RecordWireSchemaValueKind.String)));
        var formLink = CreateFormLink("$.link", isNull: true, formKey: null);
        return
        [
            new RecordWireObjectDraftNode(RecordWireDraftNodeKind.Object, "$.object", "Object", CreateDescriptor(RecordWireSchemaValueKind.Object), false, false, RecordWireDraftValueState.Seeded, []),
            new RecordWireArrayDraftNode("$.array", "Array", CreateDescriptor(RecordWireSchemaValueKind.Array, itemDescriptor: stringNode.Descriptor), false, false, RecordWireDraftValueState.Seeded, [], (_, _) => EngineResult<RecordWireDraftNode>.Success(CreateString("$.array[0]", "Item"))),
            new RecordWireUnionDraftNode("$.union", "Union", CreateDescriptor(RecordWireSchemaValueKind.Union, unionOptions: [option]), true, false, RecordWireDraftValueState.RequiredUnset, [option], null, null, (_, _, _) => EngineResult<RecordWireDraftNode>.Success(CreateString("$.union.value", "Choice"))),
            new RecordWireNullableDraftNode("$.nullable", "Nullable", CreateDescriptor(RecordWireSchemaValueKind.Nullable, nonNullDescriptor: stringNode.Descriptor, allowsNull: true), false, false, RecordWireDraftValueState.Seeded, true, stringNode, (_, _) => EngineResult<RecordWireDraftNode>.Success(stringNode)),
            new RecordWireBooleanDraftNode("$.boolean", "Boolean", CreateDescriptor(RecordWireSchemaValueKind.Boolean), false, false, RecordWireDraftValueState.Defaulted, false),
            new RecordWireIntegerDraftNode(RecordWireDraftNodeKind.Integer, "$.integer", "Integer", CreateDescriptor(RecordWireSchemaValueKind.Integer), false, false, RecordWireDraftValueState.Defaulted, "0"),
            stringNode,
            new RecordWireEnumDraftNode("$.enum", "Enum", CreateDescriptor(RecordWireSchemaValueKind.Enum, enumValues: [new RecordWireSchemaEnumValue("Known", "1")]), false, false, RecordWireDraftValueState.Defaulted, "1"),
            formLink,
            new RecordWireFormLinkOrIndexDraftNode("$.linkOrIndex", "Link or index", CreateDescriptor(RecordWireSchemaValueKind.FormLinkOrIndex), false, false, RecordWireDraftValueState.Seeded, false, "0", CreateFormLink("$.linkOrIndex.link", false, "000123:Source.esm"), RecordWireFormLinkOrIndexActiveBranch.Link),
            new RecordWireFloatBitsDraftNode("$.floatBits", "Float bits", CreateDescriptor(RecordWireSchemaValueKind.FloatBits), false, false, RecordWireDraftValueState.Seeded, []),
            new RecordWireByteArrayDraftNode("$.bytes", "Bytes", CreateDescriptor(RecordWireSchemaValueKind.ByteArray), false, false, RecordWireDraftValueState.Seeded, []),
            new RecordWireTranslatedStringDraftNode("$.translated", "Translated", CreateDescriptor(RecordWireSchemaValueKind.TranslatedString), false, false, RecordWireDraftValueState.Seeded, []),
            new RecordWireAssetDraftNode("$.asset", "Asset", CreateDescriptor(RecordWireSchemaValueKind.Asset), false, false, RecordWireDraftValueState.Seeded, []),
            new RecordWireColorDraftNode("$.color", "Color", CreateDescriptor(RecordWireSchemaValueKind.Color), false, false, RecordWireDraftValueState.Seeded, []),
            new RecordWireArray2DDraftNode("$.array2d", "Array 2D", CreateDescriptor(RecordWireSchemaValueKind.Array2D), false, false, RecordWireDraftValueState.Seeded, [])
        ];
    }

    /// <summary>Creates the populated nested typed draft used for visual review.</summary>
    /// <returns>A draft containing ordered links, a selected component union, owner-driven projections, and a mapped issue.</returns>
    private static RecordWireObjectDraftNode CreatePopulatedReviewDraft()
    {
        var editorId = new RecordWireStringDraftNode(
            RecordWireDraftNodeKind.String,
            "$.editorId",
            "Editor ID",
            CreateDescriptor(RecordWireSchemaValueKind.String),
            isRequired: true,
            isReadOnly: false,
            RecordWireDraftValueState.RequiredUnset,
            string.Empty);
        editorId.SetIssues(
        [
            new RecordWireDraftIssue(
                RecordWireDraftIssueCode.RequiredValueUnset,
                editorId.Path,
                "EditorID is required before this typed command can be applied.",
                editorId)
        ]);

        var items = new RecordWireArrayDraftNode(
            "$.items",
            "Items",
            CreateDescriptor(RecordWireSchemaValueKind.Array, itemDescriptor: CreateDescriptor(RecordWireSchemaValueKind.FormLink)),
            isRequired: true,
            isReadOnly: false,
            RecordWireDraftValueState.Seeded,
            [
                CreateFormLink("$.items[0]", isNull: false, formKey: "001234:Starfield.esm"),
                CreateFormLink("$.items[1]", isNull: true, formKey: "Null")
            ],
            (index, _) => EngineResult<RecordWireDraftNode>.Success(CreateFormLink($"$.items[{index}]", isNull: true, formKey: null)));

        var componentOption = new RecordWireSchemaUnionOption(
            "example-component",
            "Example component",
            "ExampleComponent",
            (_, _) => EngineResult<RecordWireSchemaDescriptor>.Success(CreateDescriptor(RecordWireSchemaValueKind.Object)));
        var parameter = new RecordWireFormLinkOrIndexDraftNode(
            "$.component.parameter",
            "Parameter",
            CreateDescriptor(RecordWireSchemaValueKind.FormLinkOrIndex),
            isRequired: true,
            isReadOnly: false,
            RecordWireDraftValueState.Seeded,
            isIndexNull: false,
            "3",
            CreateFormLink("$.component.parameter.link", isNull: false, formKey: "00ABCD:Example.esm"),
            RecordWireFormLinkOrIndexActiveBranch.Link);
        var component = new RecordWireObjectDraftNode(
            RecordWireDraftNodeKind.Object,
            "$.component",
            "Example component",
            CreateDescriptor(RecordWireSchemaValueKind.Object),
            isRequired: true,
            isReadOnly: false,
            RecordWireDraftValueState.Seeded,
            [
                new RecordWireObjectDraftProperty("label", CreateString("$.component.label", "Populated nested component")),
                new RecordWireObjectDraftProperty("parameter", parameter)
            ]);
        var componentUnion = new RecordWireUnionDraftNode(
            "$.component",
            "Component",
            CreateDescriptor(RecordWireSchemaValueKind.Union, unionOptions: [componentOption]),
            isRequired: true,
            isReadOnly: false,
            RecordWireDraftValueState.Seeded,
            [componentOption],
            componentOption,
            component,
            (_, _, _) => EngineResult<RecordWireDraftNode>.Success(component));

        return new RecordWireObjectDraftNode(
            RecordWireDraftNodeKind.Object,
            "$",
            "Replace Items and inspect Component",
            CreateDescriptor(RecordWireSchemaValueKind.Object),
            isRequired: true,
            isReadOnly: false,
            RecordWireDraftValueState.Seeded,
            [
                new RecordWireObjectDraftProperty("editorId", editorId),
                new RecordWireObjectDraftProperty("items", items),
                new RecordWireObjectDraftProperty("component", componentUnion)
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
    private static RecordWireSchemaDescriptor CreateDescriptor(
        RecordWireSchemaValueKind kind,
        IReadOnlyList<RecordWireSchemaUnionOption>? unionOptions = null,
        RecordWireSchemaDescriptor? itemDescriptor = null,
        RecordWireSchemaDescriptor? nonNullDescriptor = null,
        IReadOnlyList<RecordWireSchemaEnumValue>? enumValues = null,
        bool allowsNull = false)
    {
        return new RecordWireSchemaDescriptor(
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
    private static RecordWireStringDraftNode CreateString(string path, string value)
    {
        return new RecordWireStringDraftNode(
            RecordWireDraftNodeKind.String,
            path,
            "Text",
            CreateDescriptor(RecordWireSchemaValueKind.String),
            isRequired: true,
            isReadOnly: false,
            RecordWireDraftValueState.Seeded,
            value);
    }

    /// <summary>Creates one ordered translation row with editable language and text children.</summary>
    /// <param name="path">The row's current structural path.</param>
    /// <param name="language">The exact language name, or an unset new-row value.</param>
    /// <param name="text">The row's current translated text.</param>
    /// <returns>A closed typed translation entry.</returns>
    private static RecordWireObjectDraftNode CreateTranslation(string path, string language, string text)
    {
        return new RecordWireObjectDraftNode(
            RecordWireDraftNodeKind.Object, path, "Translation", CreateDescriptor(RecordWireSchemaValueKind.Object),
            true, false, RecordWireDraftValueState.Seeded,
            [
                new RecordWireObjectDraftProperty("language", CreateString($"{path}.language", language)),
                new RecordWireObjectDraftProperty("value", CreateString($"{path}.value", text))
            ]);
    }

    /// <summary>Creates one exact FormLink draft.</summary>
    /// <param name="path">The exact test path.</param>
    /// <param name="isNull">Whether the record link is null.</param>
    /// <param name="formKey">The exact JSON-null, canonical-null, or concrete FormKey representation.</param>
    /// <returns>The FormLink draft.</returns>
    private static RecordWireFormLinkDraftNode CreateFormLink(string path, bool isNull, string? formKey)
    {
        return new RecordWireFormLinkDraftNode(
            path,
            "Reference",
            CreateDescriptor(RecordWireSchemaValueKind.FormLink),
            isRequired: true,
            isReadOnly: false,
            RecordWireDraftValueState.Seeded,
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
            "PluginAuthoring",
            "screenshots",
            "plugin-form-list-editor-populated.png"));
    }

    /// <summary>Raises one routed button click and drains synchronous UI work.</summary>
    /// <param name="button">The enabled action button.</param>
    private static void RaiseClick(Button button)
    {
        button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        Dispatcher.UIThread.RunJobs();
    }
}
