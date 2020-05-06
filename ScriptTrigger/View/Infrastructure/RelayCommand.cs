using System;
using System.Windows.Input;

namespace ScriptTrigger.View.Infrastructure
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _action;
        private readonly Func<object, bool> _checkCanExecute;
        private bool _lastCanExecuteValue = false;

        public RelayCommand(Action<object> action, Func<object, bool> checkCanExecute = null)
        {
            this._action = action;
            this._checkCanExecute = checkCanExecute;
        }

        public bool CanExecute(object parameter)
        {
            var newCanExecuteValue = true;
            if (this._checkCanExecute != null)
            {
                newCanExecuteValue = this._checkCanExecute(parameter);
            }

            if (this._lastCanExecuteValue != newCanExecuteValue)
            {
                this._lastCanExecuteValue = newCanExecuteValue;
                this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }

            return this._lastCanExecuteValue;
        }

        public void Execute(object parameter)
        {
            this._action?.Invoke(parameter);
        }

        public event EventHandler CanExecuteChanged;
    }
}
