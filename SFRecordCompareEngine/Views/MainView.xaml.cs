using Microsoft.UI.Xaml.Controls;
using SFRecordCompareEngine.ViewModels;

namespace SFRecordCompareEngine.Views;

public sealed partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    public MainView(MainPageViewModel viewModel)
        : this()
    {
        DataContext = viewModel;
    }

    private void RecordTree_SelectionChanged(TreeView sender, TreeViewSelectionChangedEventArgs args)
    {
        if (DataContext is not MainPageViewModel viewModel)
        {
            return;
        }

        viewModel.SelectRecord(args.AddedItems.FirstOrDefault() as RecordTreeItemViewModel);
    }
}