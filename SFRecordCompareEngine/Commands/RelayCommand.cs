using System.Windows.Input;

namespace SFRecordCompareEngine.Commands;

public class RelayCommand : ICommand
{
    private readonly Action ExecuteAction;
    private readonly Func<bool>? CanExecuteAction;

    public RelayCommand(Action execute, Func<bool>? canExecute = null)
    {
        ExecuteAction = execute;
        CanExecuteAction = canExecute;
    }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter)
    {
        return CanExecuteAction?.Invoke() ?? true;
    }

    public void Execute(object? parameter)
    {
        ExecuteAction();
    }

    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
