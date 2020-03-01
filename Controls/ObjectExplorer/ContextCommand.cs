﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Thingie.WPF.Controls.ObjectExplorer
{
    public abstract class ContextCommand : ICommand
    {
        public abstract string Text { get; }

        public virtual ObservableCollection<ContextCommand> SubmenuCommands { get; } = new ObservableCollection<ContextCommand>();

        public abstract Uri ImageUri { get; }

        public event EventHandler CanExecuteChanged;

        public virtual bool CanExecute(object parameter) => true;

        public abstract void Execute(object parameter);

        protected virtual void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
