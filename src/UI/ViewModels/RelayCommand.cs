using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace UI.ViewModels
{
    public class RelayCommand(Func<object?, Task> execute, Func<bool>? canExecute = null) : ICommand
    {
        private readonly Func<object?, Task> _execute = execute;
        private readonly Func<bool>? _canExecute = canExecute;

        public bool CanExecute(object? parameter) => _canExecute == null || _canExecute();

        public async void Execute(object? parameter) => await _execute(parameter);

        public event EventHandler? CanExecuteChanged;

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
