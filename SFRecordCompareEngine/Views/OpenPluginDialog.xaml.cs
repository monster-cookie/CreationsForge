using Microsoft.UI.Xaml.Controls;
using SFRecordCompareEngine.ViewModels;

namespace SFRecordCompareEngine.Views;

public sealed partial class OpenPluginDialog : ContentDialog
{
    public OpenPluginDialog(OpenPluginDialogViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }

    private OpenPluginDialogViewModel ViewModel => (OpenPluginDialogViewModel)DataContext;

    private void PluginSearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (args.Reason != AutoSuggestionBoxTextChangeReason.UserInput)
        {
            return;
        }

        ViewModel.UpdateSearchText(sender.Text);
    }

    private void PluginSearchBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        if (args.SelectedItem is not string pluginFileName)
        {
            return;
        }

        ViewModel.ChooseSuggestion(pluginFileName);
        sender.Text = pluginFileName;
    }

    private void PluginSearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        var queryText = args.ChosenSuggestion is string pluginFileName ? pluginFileName : args.QueryText;
        ViewModel.SubmitQuery(queryText);
    }
}
