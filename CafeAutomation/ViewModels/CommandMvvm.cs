using System;
using System.Windows.Input;

namespace CafeAutomation.ViewModels
{
    public class CommandMvvm : ICommand
    {
        private readonly Action<object?> execute;
        private readonly Func<object?, bool> canExecute;

        public CommandMvvm(Action<object?> execute, Func<object?, bool>? canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute ?? (_ => true);
        }

        public bool CanExecute(object? parameter) => canExecute(parameter);

        public void Execute(object? parameter) => execute(parameter);

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
