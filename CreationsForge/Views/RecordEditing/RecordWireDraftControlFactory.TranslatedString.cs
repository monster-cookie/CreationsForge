using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using CreationsForge.Core.Engine.RecordWire;
using CreationsForge.RecordEditing.Drafts;

namespace CreationsForge.Views.RecordEditing;

internal static partial class RecordWireDraftControlFactory
{
    /// <summary>Builds a language-and-text editor over the existing exact translated-string wire draft.</summary>
    /// <param name="node">The translated-string draft.</param>
    /// <param name="pickReferenceAsync">The fallback editor's optional reference picker.</param>
    /// <param name="forceReadOnly">Whether the containing field prevents editing.</param>
    /// <returns>A table editor for the known wire shape, or the ordinary typed editor for another shape.</returns>
    private static Control CreateTranslatedStringEditor(
        RecordWireTranslatedStringDraftNode node,
        Func<RecordWireFormLinkDraftNode, Task>? pickReferenceAsync,
        bool forceReadOnly)
    {
        if (node.FindProperty("targetLanguage") is not RecordWireStringDraftNode targetLanguage
            || node.FindProperty("value") is not RecordWireNullableDraftNode value
            || node.FindProperty("translations") is not RecordWireArrayDraftNode translations)
        {
            return CreateSpecializedObjectEditor(node, "Translations retain their language keys and ordering.", pickReferenceAsync, forceReadOnly);
        }

        var languageNames = RecordWireLanguageOptions.GetNames();
        var canEdit = !forceReadOnly && !node.IsReadOnly;
        var targetSelector = new ComboBox
        {
            ItemsSource = languageNames,
            SelectedItem = targetLanguage.Value,
            IsEnabled = canEdit && !targetLanguage.IsReadOnly,
            MinWidth = 180,
            HorizontalAlignment = HorizontalAlignment.Left
        };
        SetAutomationId(targetSelector, targetLanguage, "TargetLanguage");

        var table = new StackPanel { Spacing = 5 };
        SetAutomationId(table, translations, "TranslationRows");
        var guidance = CreateText(string.Empty, 11, FontWeight.Normal);
        guidance.TextWrapping = TextWrapping.Wrap;
        guidance.Opacity = 0.72;
        var operationError = CreateOperationError(translations);
        var valueIssues = new StackPanel();
        void RefreshValueIssues()
        {
            valueIssues.Children.Clear();
            if (value.Value is not null)
            {
                valueIssues.Children.Add(CreateIssueList(value.Value));
            }
        }

        value.PropertyChanged += (_, args) =>
        {
            if (string.Equals(args.PropertyName, nameof(RecordWireNullableDraftNode.Value), StringComparison.Ordinal))
            {
                RefreshValueIssues();
            }
        };
        RefreshValueIssues();

        void RefreshGuidance()
        {
            guidance.Text = translations.Items.OfType<RecordWireObjectDraftNode>().Any(entry =>
                entry.FindProperty("language") is RecordWireStringDraftNode language
                && string.Equals(language.Value, targetLanguage.Value, StringComparison.Ordinal))
                ? "The selected language's text is the record name."
                : "Add a translation for the selected language to give this record a name.";
        }

        void SynchronizeTargetValue()
        {
            var matching = translations.Items.OfType<RecordWireObjectDraftNode>().FirstOrDefault(entry =>
                entry.FindProperty("language") is RecordWireStringDraftNode language
                && string.Equals(language.Value, targetLanguage.Value, StringComparison.Ordinal));
            if (matching?.FindProperty("value") is not RecordWireStringDraftNode text)
            {
                value.SetNull();
            }
            else
            {
                var present = value.SetPresent();
                if (!present.Succeeded || present.Value is not RecordWireStringDraftNode selectedText)
                {
                    operationError.Text = present.Error?.Message ?? "The selected-language value could not be prepared.";
                    return;
                }

                selectedText.Value = text.Value;
            }

            RefreshGuidance();
        }

        void RefreshRows()
        {
            table.Children.Clear();
            var header = new Grid { ColumnDefinitions = new ColumnDefinitions("190,*,Auto") };
            header.Children.Add(CreateText("Language", 12, FontWeight.SemiBold));
            var textHeader = CreateText("Name text", 12, FontWeight.SemiBold);
            Grid.SetColumn(textHeader, 1);
            header.Children.Add(textHeader);
            table.Children.Add(header);

            for (var index = 0; index < translations.Items.Count; index++)
            {
                if (translations.Items[index] is not RecordWireObjectDraftNode entry
                    || entry.FindProperty("language") is not RecordWireStringDraftNode language
                    || entry.FindProperty("value") is not RecordWireStringDraftNode text)
                {
                    table.Children.Add(CreateDeferredChild(translations.Items[index], pickReferenceAsync, forceReadOnly || node.IsReadOnly));
                    continue;
                }

                var rowIndex = index;
                var row = new Grid
                {
                    ColumnDefinitions = new ColumnDefinitions("190,*,Auto"),
                    ColumnSpacing = 8
                };
                var languageSelector = new ComboBox
                {
                    ItemsSource = languageNames,
                    SelectedItem = language.Value,
                    IsEnabled = canEdit && !translations.IsReadOnly && !language.IsReadOnly
                };
                SetAutomationId(languageSelector, language, "TranslationLanguage");
                languageSelector.SelectionChanged += (_, _) =>
                {
                    if (languageSelector.SelectedItem is not string selected
                        || string.Equals(selected, language.Value, StringComparison.Ordinal))
                    {
                        return;
                    }

                    if (translations.Items.OfType<RecordWireObjectDraftNode>().Any(other =>
                        !ReferenceEquals(other, entry)
                        && other.FindProperty("language") is RecordWireStringDraftNode otherLanguage
                        && string.Equals(otherLanguage.Value, selected, StringComparison.Ordinal)))
                    {
                        operationError.Text = $"{selected} already has a translation.";
                        languageSelector.SelectedItem = language.Value;
                        return;
                    }

                    language.Value = selected;
                    operationError.Text = string.Empty;
                    SynchronizeTargetValue();
                };
                row.Children.Add(languageSelector);

                var textBox = new TextBox
                {
                    Text = text.Value,
                    IsEnabled = canEdit && !translations.IsReadOnly && !text.IsReadOnly,
                    AcceptsReturn = true,
                    TextWrapping = TextWrapping.Wrap,
                    MinHeight = 32,
                    PlaceholderText = "Enter name text"
                };
                SetAutomationId(textBox, text, "TranslationText");
                Grid.SetColumn(textBox, 1);
                row.Children.Add(textBox);
                textBox.TextChanged += (_, _) =>
                {
                    text.Value = textBox.Text ?? string.Empty;
                    SynchronizeTargetValue();
                };

                var actions = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 4 };
                var moveUp = CreateActionButton("↑", entry, "TranslationMoveUp");
                moveUp.IsEnabled = canEdit && !translations.IsReadOnly && rowIndex > 0;
                moveUp.Click += (_, _) =>
                {
                    translations.Move(rowIndex, rowIndex - 1);
                    RefreshRows();
                };
                actions.Children.Add(moveUp);
                var moveDown = CreateActionButton("↓", entry, "TranslationMoveDown");
                moveDown.IsEnabled = canEdit && !translations.IsReadOnly && rowIndex < translations.Items.Count - 1;
                moveDown.Click += (_, _) =>
                {
                    translations.Move(rowIndex, rowIndex + 1);
                    RefreshRows();
                };
                actions.Children.Add(moveDown);
                var remove = CreateActionButton("Remove", entry, "TranslationRemove");
                remove.IsEnabled = canEdit && !translations.IsReadOnly;
                remove.Click += (_, _) =>
                {
                    translations.Remove(rowIndex);
                    SynchronizeTargetValue();
                    RefreshRows();
                };
                actions.Children.Add(remove);
                Grid.SetColumn(actions, 2);
                row.Children.Add(actions);
                table.Children.Add(row);
                table.Children.Add(CreateIssueList(entry));
                table.Children.Add(CreateIssueList(language));
                table.Children.Add(CreateIssueList(text));
            }

            RefreshGuidance();
        }

        targetSelector.SelectionChanged += (_, _) =>
        {
            if (targetSelector.SelectedItem is string selected
                && !string.Equals(selected, targetLanguage.Value, StringComparison.Ordinal))
            {
                targetLanguage.Value = selected;
                SynchronizeTargetValue();
            }
        };
        var add = CreateActionButton("Add translation", translations, "AddTranslation");
        add.IsEnabled = canEdit && !translations.IsReadOnly;
        add.Click += (_, _) =>
        {
            var used = translations.Items.OfType<RecordWireObjectDraftNode>()
                .Select(entry => (entry.FindProperty("language") as RecordWireStringDraftNode)?.Value)
                .ToHashSet(StringComparer.Ordinal);
            var next = languageNames.Contains(targetLanguage.Value, StringComparer.Ordinal) && !used.Contains(targetLanguage.Value)
                ? targetLanguage.Value
                : languageNames.FirstOrDefault(language => !used.Contains(language));
            if (next is null)
            {
                operationError.Text = "Every available language already has a translation.";
                return;
            }

            var inserted = translations.Insert(translations.Items.Count);
            if (!inserted.Succeeded || inserted.Value is not RecordWireObjectDraftNode entry
                || entry.FindProperty("language") is not RecordWireStringDraftNode language)
            {
                operationError.Text = inserted.Error?.Message ?? "A translation row could not be created.";
                return;
            }

            language.Value = next;
            operationError.Text = string.Empty;
            SynchronizeTargetValue();
            RefreshRows();
        };

        RefreshRows();
        return new StackPanel
        {
            Spacing = 8,
            Children =
            {
                CreateText("Displayed language", 12, FontWeight.SemiBold),
                targetSelector,
                CreateIssueList(targetLanguage),
                guidance,
                table,
                add,
                operationError,
                CreateIssueList(value),
                valueIssues,
                CreateIssueList(translations)
            }
        };
    }
}
