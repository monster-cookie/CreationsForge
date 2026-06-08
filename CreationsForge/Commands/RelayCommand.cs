using System.Windows.Input;
using Serilog;

namespace CreationsForge.Commands;

public class RelayCommand : ICommand
{
    private readonly Func<bool>? CanExecuteAction;
    private readonly Action ExecuteAction;
    private readonly ILogger Logger = Log.ForContext<RelayCommand>();

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
        if (!CanExecute(parameter))
        {
            return;
        }

        try
        {
            ExecuteAction();
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Command execution failed");
        }
    }

    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
