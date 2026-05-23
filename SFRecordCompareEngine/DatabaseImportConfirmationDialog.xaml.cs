using System.Windows;

namespace SFRecordCompareEngine;

public partial class DatabaseImportConfirmationDialog
{
    public DatabaseImportConfirmationDialog()
    {
        InitializeComponent();
    }

    private void ContinueButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }

    private void ExitButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }
}
