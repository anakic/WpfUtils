using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Threading;

namespace Thingie.WPF.Controls.ObjectExplorer
{
    public abstract class ContextCommand : ICommand, IContextMenuItem
    {
        // todo: add ability to set description i.e. tooltip

        public abstract string Text { get; }

        public virtual ObservableCollection<IContextMenuItem> SubmenuCommands { get; } = new ObservableCollection<IContextMenuItem>();

        public abstract object Image { get; }

        public event EventHandler CanExecuteChanged;

        public virtual bool CanExecute(object parameter) => true;

        public abstract void Execute(object parameter);

        protected virtual void OnCanExecuteChanged()
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() => CanExecuteChanged?.Invoke(this, EventArgs.Empty)));
        }
    }
}
