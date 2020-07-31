using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;
using System.Windows.Threading;

namespace Thingie.WPF.Controls.ObjectExplorer
{
    public class AsyncObservableCollection<T> : ObservableCollection<T>
    {
        private readonly Dispatcher dispatcher;

        public AsyncObservableCollection(Dispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        public AsyncObservableCollection(IEnumerable<T> list)
            : base(list)
        {
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            dispatcher.Invoke(new System.Action(() => base.OnCollectionChanged(e)));
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            dispatcher.Invoke(new System.Action(() => base.OnPropertyChanged(e)));
        }
    }
}
