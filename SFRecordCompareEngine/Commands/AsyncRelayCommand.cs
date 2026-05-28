using System.Windows.Input;

namespace SFRecordCompareEngine.Commands;

public class AsyncRelayCommand : ICommand
{
    private readonly Func<Task> ExecuteAction;
    private readonly Func<bool>? CanExecuteAction;
    private bool IsExecuting;

    public AsyncRelayCommand(Func<Task> execute, Func<bool>? canExecute = null)
    {
        ExecuteAction = execute;
        CanExecuteAction = canExecute;
    }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter)
    {
        return !IsExecuting && (CanExecuteAction?.Invoke() ?? true);
    }

    public async void Execute(object? parameter)
    {
        if (!CanExecute(parameter)) return;

        IsExecuting = true;
        RaiseCanExecuteChanged();

        try
        {
            await ExecuteAction();
        }
        finally
        {
            IsExecuting = false;
            RaiseCanExecuteChanged();
        }
    }

    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
