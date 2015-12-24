namespace Thingie.WPF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;
    using System.Windows.Input;

    public class ReflectiveCommand : ICommand
    {
        private readonly PropertyInfo _canExecute;
        private readonly MethodInfo _execute;
        private readonly object _model;

        public ReflectiveCommand(object model, MethodInfo execute, PropertyInfo canExecute)
        {
            _model = model;
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            if(_canExecute != null)
                return (bool)_canExecute.GetValue(_model, null);
            return true;
        }

        public void Execute(object parameter)
        {
            var returnValue = _execute.Invoke(_model, null);
        }
    }
}