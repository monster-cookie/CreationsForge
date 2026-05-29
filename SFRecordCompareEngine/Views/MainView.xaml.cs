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
}
