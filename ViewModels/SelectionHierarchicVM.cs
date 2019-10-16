using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;

namespace Thingie.WPF.ViewModels
{
    public class SelectionHierarchicVM : INotifyPropertyChanged
    {
        bool? _isChecked = false;
        SelectionHierarchicVM _parent;

        public IEnumerable<SelectionHierarchicVM> GetAllNodes()
        {
            return 
                new SelectionHierarchicVM[] { this }
                .Union(Children.SelectMany(h=>h.GetAllNodes()));
        }

        public SelectionHierarchicVM(string name, Uri iconUri, object model)
        {
            Name = name;
            IconUri = iconUri;
            Model = model;

            Children = new List<SelectionHierarchicVM>();
        }

        public void InitializeHierarchy()
        {
            foreach (SelectionHierarchicVM child in Children)
            {
                child._parent = this;
                child.InitializeHierarchy();
            }
        }

        public string Name { get; private set; }
        public Uri IconUri { get; private set; }
        public object Model { get; set; }

        public bool IsInitiallySelected { get; private set; }
        public IEnumerable<SelectionHierarchicVM> Children { get; set; }

        public bool? IsChecked
        {
            get { return _isChecked; }
            set { SetIsChecked(value, true, true); }
        }

        void SetIsChecked(bool? value, bool updateChildren, bool updateParent)
        {
            if (value == _isChecked)
                return;

            _isChecked = value;

            if (updateChildren && _isChecked.HasValue)
                Children.ToList().ForEach(c => c.SetIsChecked(_isChecked, true, false));

            if (updateParent && _parent != null)
                _parent.VerifyCheckState();

            OnPropertyChanged(nameof(IsChecked));
        }

        void VerifyCheckState()
        {
            bool? state = null;
            for (int i = 0; i < Children.Count(); ++i)
            {
                bool? current = Children.ElementAt(i).IsChecked;
                if (i == 0)
                {
                    state = current;
                }
                else if (state != current)
                {
                    state = null;
                    break;
                }
            }
            SetIsChecked(state, false, true);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
