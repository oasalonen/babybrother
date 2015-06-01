using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace BabyBrother.Base
{
    public class DelegateCommand : ICommand
    {
        private Func<object, bool> _canExecute;

        private Action<object> _execute;

        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _canExecute = canExecute;
            _execute = execute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute != null ? _canExecute(parameter) : true;
        }

        public void Execute(object parameter)
        {
            System.Diagnostics.Debug.Assert(_execute != null);
            _execute(parameter);
        }
    }
}
