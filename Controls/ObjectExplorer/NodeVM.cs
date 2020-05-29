using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace Thingie.WPF.Controls.ObjectExplorer
{
    public abstract class NodeVM : INotifyPropertyChanged
    {
        string name;
        int order = 1;
        bool? isChecked;
        private ObservableCollection<NodeVM> nodes;


        /// <summary>
        /// If true, child Nodes will not be movable outside of this folder.
        /// </summary>
        public virtual bool IsScopeRoot => false;

        #region user interaction properties
        private bool isEditing;
        private bool isExpanded;
        private bool isSelected;
        private bool satisfiesFilter = true;

        public virtual bool IsVisible
        {
            get { return satisfiesFilter; }
            set
            {
                if (satisfiesFilter != value)
                {
                    satisfiesFilter = value;
                    UpdateVisibility();
                }
            }
        }

        protected void UpdateVisibility()
        {
            OnPropertyChanged(nameof(IsVisible));
            Parent?.OnPropertyChanged(nameof(Nodes)); // re-apply filtering on parent Nodes collection
        }

        public bool IsEditing { get => isEditing; set { isEditing = value; OnPropertyChanged(nameof(IsEditing)); } }

        public virtual bool IsExpanded { get => isExpanded; set { isExpanded = value; OnPropertyChanged(nameof(IsExpanded)); } }
        public virtual bool IsSelected { get => isSelected; set { isSelected = value; OnPropertyChanged(nameof(IsSelected)); } }
        #endregion

        public NodeVM(string name)
        {
            this.name = name;
        }

        public virtual void Initialize() { }

        #region actions
        public virtual bool CanRename() => false;
        protected virtual void DoRename(string oldName, string newName) { }

        public virtual bool CanSelect() => false;
        public virtual void Select() { }

        public virtual bool CanActivate() => false;
        public virtual void Activate() { }

        public virtual bool CanMove(NodeVM proposedParent) => false;
        public virtual void Move(NodeVM newParent) { }




        public virtual bool CanDelete() => false;
        public virtual void Delete() { }
        #endregion

        public virtual string DisplayText { get => Name; }

        public virtual string Name
        {
            get => name;
            set
            {
                if (CanRename())
                {
                    var oldName = name;
                    name = value;
                    try
                    {
                        DoRename(oldName, name);
                        OnPropertyChanged(nameof(Name));
                        OnPropertyChanged(nameof(DisplayText));
                        Parent.OnPropertyChanged(nameof(Nodes));
                    }
                    catch
                    {
                        name = oldName;
                        throw;
                    }
                }
                else
                    throw new InvalidOperationException("Unable to rename");
            }
        }

        #region core properties
        public abstract Uri ImageURI { get; }
        public virtual string ToolTip { get => Name; }
        public virtual object Badge { get; }
        public int Order { get => order; set { order = value; OnPropertyChanged(nameof(Order)); Parent?.OnPropertyChanged(nameof(Nodes)); } }
        public virtual bool CheckboxEnabled => false;
        public virtual bool? IsChecked { get => isChecked; set { isChecked = value; OnPropertyChanged(nameof(IsChecked)); } }
        public virtual bool IsThreeState { get; } = false;
        public ObservableCollection<IContextMenuItem> ContextCommands { get; } = new ObservableCollection<IContextMenuItem>();
        #endregion

        public NodeVM Parent { get; private set; }

        public T FindNode<T>() where T : NodeVM
        {
            if (this is T p)
                return p;
            else
                return Parent?.FindNode<T>();
        }

        public IEnumerable<NodeVM> GetNodesRecursive(bool includeSelf = false)
        {
            if(includeSelf)
                yield return this;

            foreach (NodeVM childNode in Nodes)
            {
                foreach (var childSubNode in childNode.GetNodesRecursive(true))
                    yield return childSubNode;
            }
        }

        public ObservableCollection<NodeVM> Nodes
        {
            get
            {
                if (nodes == null)
                {
                    nodes = new ObservableCollection<NodeVM>();
                    BindingOperations.EnableCollectionSynchronization(nodes, new object());
                    nodes.CollectionChanged += (s, e) => { if (e.NewItems != null) e.NewItems.OfType<NodeVM>().ToList().ForEach(n => n.Parent = this); };
                    PopulateNodes(nodes as ObservableCollection<NodeVM>);
                }
                return nodes;
            }
        }

        protected virtual void PopulateNodes(ObservableCollection<NodeVM> nodes) { }

        #region filter
        public void Filter(string search)
        {
            DoFilter(Array.Empty<string>(), search.Split(' '));
            OnPropertyChanged(nameof(Nodes));
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
        protected virtual void OnPropertyChanged(string propertyName)
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

        #region helpers

        // Internal prerequisites for allowing a node move. These need to be satisfied before CanMove is checked.
        internal bool CanMoveInternal(NodeVM proposedParent)
        {
            return 
                proposedParent != this // cannot move to self
                && proposedParent != this.Parent // cannot move to the same parent
                && FindScopeRoot(this) == FindScopeRoot(proposedParent) // move must be inside the same scope
                && IsAncestorOf(this, proposedParent) == false; // cannot move to its own descendant
        }

        private NodeVM FindScopeRoot(NodeVM node)
        {
            if (node == null)
                return null;
            else if (node.IsScopeRoot)
                return node;
            else
                return FindScopeRoot(node.Parent);
        }

        private bool IsAncestorOf(NodeVM node, NodeVM other)
        {
            if (other.Parent == null)
                return false;
            else if (other.Parent == node)
                return true;
            else
                return IsAncestorOf(node, other.Parent);
        }

        #endregion
    }
}
