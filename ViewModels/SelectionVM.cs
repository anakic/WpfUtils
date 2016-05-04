namespace Thingie.WPF.ViewModels
{
	public sealed class SelectionVM<T> : VMBase<T> where T:class
    {
        private bool _IsSelected;
        public bool IsSelected
        {
            get { return _IsSelected; }
            set { _IsSelected = value; OnPropertyChanged("IsSelected"); }
        }

        public SelectionVM(T item, bool initialCheckState): base(item)
        {
            _IsSelected = initialCheckState;
        }
    }
}
