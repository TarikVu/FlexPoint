using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace UI.ViewModels
{
    public class RelayCommand<T> : ICommand
    {
        private readonly Func<T, Task>? _executeAsync;
        private readonly Action<T>? _execute;
        private readonly Func<bool>? _canExecute;
        public RelayCommand(Func<T, Task> executeAsync, Func<bool>? canExecute = null)
        {
            _executeAsync = executeAsync;
            _canExecute = canExecute;
        }

        public RelayCommand(Action<T> execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;

        public async void Execute(object? parameter)
        {
            if (_executeAsync != null)
            {
                await ExecuteAsyncCommand(parameter);
            }
            else if (_execute != null)
            {
                ExecuteSyncCommand(parameter);
            }
        }

        private async Task ExecuteAsyncCommand(object? parameter)
        {
            if (parameter is T castParam)
            {
                await _executeAsync!(castParam);
            }
            else if (parameter == null && typeof(T) == typeof(object))
            {
                await _executeAsync!(default!);
            }
        }

        private void ExecuteSyncCommand(object? parameter)
        {
            if (parameter is T castParam)
            {
                _execute!(castParam);
            }
            else if (parameter == null && typeof(T) == typeof(object))
            {
                _execute!(default!);
            }
        }

        public event EventHandler? CanExecuteChanged;

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
