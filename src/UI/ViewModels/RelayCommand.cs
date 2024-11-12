using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace UI.ViewModels
{
    /// <summary>
    /// Relays A command to execute based of a condition
    /// </summary>
    /// <param name="execute"></param>
    /// <param name="condition"></param>
    public class RelayCommand(Action execute, Func<bool>? condition = null) 
        : RelayCommand<object>(execute, condition)
    {
    }
    public class RelayCommand<T> : ICommand
    {
        private readonly Func<T, Task>? _executeAsync;          
        private readonly Action<T>? _execute;                 

        private readonly Func<Task>? _executeAsyncNoParam;      
        private readonly Action? _executeNoParam;        

        private readonly Func<bool>? _executeCondition;              


        public RelayCommand(Action<T> execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _executeCondition = canExecute;
        }


        public RelayCommand(Func<T, Task> executeAsync, Func<bool>? canExecute = null)
        {
            _executeAsync = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));
            _executeCondition = canExecute;
        }


        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _executeNoParam = execute ?? throw new ArgumentNullException(nameof(execute));
            _executeCondition = canExecute;
        }


        public RelayCommand(Func<Task> executeAsync, Func<bool>? canExecute = null)
        {
            _executeAsyncNoParam = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));
            _executeCondition = canExecute;
        }

        public bool CanExecute(object? parameter) => _executeCondition?.Invoke() ?? true;

        public async void Execute(object? parameter)
        {
          
            if (_executeAsync != null && parameter is T castParam)
            {
                await _executeAsync(castParam);
            }

            else if (_executeAsyncNoParam != null)
            {
                await _executeAsyncNoParam();
            }
         
            else if (_execute != null && parameter is T castParamSync)
            {
                _execute(castParamSync);
            }

            else if (_executeNoParam != null)
            {
                _executeNoParam();
            }
        }

        public event EventHandler? CanExecuteChanged;

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
