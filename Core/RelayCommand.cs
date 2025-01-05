using System.Windows.Input;

namespace EPubReader.Commands
{
    public class RelayCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        Action<object> _execute { get; set; }
        Predicate<object> _canExecute { get; set; }

        public RelayCommand(Action<object> executeMethod, Predicate<object> canExecuteMethod)
        {
            _execute = executeMethod;
            _canExecute = canExecuteMethod;
        }

        public bool CanExecute(object? parameter)
        {
            return _canExecute(parameter);
        }

        public void Execute(object? parameter)
        {
            _execute(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
