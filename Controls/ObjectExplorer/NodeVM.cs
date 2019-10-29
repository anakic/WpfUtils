using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Thingie.WPF.Controls.ObjectExplorer
{
    public abstract class NodeVM : INotifyPropertyChanged
    {
        #region private fields
        IEnumerable<NodeVM> nodes = new List<NodeVM>();
        #endregion

        #region user interaction properties
        private bool isEditing;
        private bool isExpanded;
        private bool satisfiesFilter = true;
        
        public virtual bool IsVisible
        {
            get { return satisfiesFilter; }
            set
            {
                if (satisfiesFilter != value)
                {
                    satisfiesFilter = value;
                    OnPropertyChanged(nameof(IsVisible));
                }
            }
        }

        public bool IsEditing { get => isEditing; set { isEditing = value; OnPropertyChanged(nameof(IsEditing)); } }
        
        public virtual bool IsExpanded { get => isExpanded; set { isExpanded = value; OnPropertyChanged(nameof(IsExpanded)); } }
        
        public virtual IEnumerable<NodeVM> VisibleNodes => Nodes.Where(n => n.IsVisible);
        #endregion

        #region actions
        public virtual bool CanRename() => false;

        public virtual bool CanSelect() => false;
        public virtual void Select() { }

        public virtual bool CanActivate() => false;
        public virtual void Activate() {  }

        public virtual bool CanMove(NodeVM proposedParent) => false;
        public virtual void Move(NodeVM newParent) {  }

        public virtual bool CanDelete() => false;
        public virtual void Delete() { }
        #endregion

        #region core properties
        public abstract Uri ImageURI { get; }
        public abstract string Name { get; set; }
        public virtual string ToolTip { get; }
        public virtual object Badge { get; }
        public virtual int Order { get; } = 1;
        public virtual IEnumerable<ContextCommand> ContextCommands { get; } = new List<ContextCommand>();
        #endregion

        public IEnumerable<NodeVM> Nodes { get => nodes; set { nodes = value; OnPropertyChanged(nameof(Nodes)); } }

        #region filter
        public void Filter(string search)
        {
            DoFilter(new string[0], search.Split(' '));
            OnPropertyChanged(nameof(VisibleNodes));
        }

        protected void DoFilter(IEnumerable<string> path, IEnumerable<string> filterSegments)
        {
            List<string> myPath = new List<string>(path) { Name ?? "" };

            bool satisfiesFilterDirectly = filterSegments.All(seg => myPath.Any(p => p.ToUpper().Contains(seg.ToUpper())));

            bool hasChildrenThatSatisfyFilter = false;
            if (Nodes != null)
            {
                foreach (NodeVM child in Nodes)
                {
                    child.DoFilter(myPath, filterSegments);
                    if (child.IsVisible)
                        hasChildrenThatSatisfyFilter = true;
                }
            }

            IsVisible = satisfiesFilterDirectly || hasChildrenThatSatisfyFilter;
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Visit(Action<NodeVM> n)
        {
            n(this);

            if (Nodes != null)
            {
                foreach (var child in Nodes)
                    child.Visit(n);
            }
        }

        public override string ToString()
        {
            return Name;
        }

    }
}
