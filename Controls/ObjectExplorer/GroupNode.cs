using System.ComponentModel;
using System.Linq;

namespace Thingie.WPF.Controls.ObjectExplorer
{
    public class GroupNode : NodeVM
    {
        string searchName = string.Empty;
        public override string SearchName => searchName;

        public override bool CheckboxEnabled => true;

        public override bool IsThreeState => false;

        public GroupNode(string groupName, bool participatesInSearch)
            : base(groupName)
        {
            IsExpanded = true;

            if (participatesInSearch)
                searchName = groupName;

            this.Nodes.CollectionChanged += Nodes_CollectionChanged;
        }

        private void Nodes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                e.NewItems.OfType<NodeVM>().ToList().ForEach(n => n.PropertyChanged += NodeChanged);
            if (e.OldItems != null)
                e.OldItems.OfType<NodeVM>().ToList().ForEach(n => n.PropertyChanged -= NodeChanged);
            UpdateByChildren();
        }

        public override bool? IsChecked
        {
            get => base.IsChecked;
            set
            {
                updatingByUser = true;
                base.IsChecked = value;
                this.Nodes.Where(n => n.IsVisible).ToList().ForEach(n => n.IsChecked = value);
                updatingByUser = false;
            }
        }

        bool updatingByUser;
        private void NodeChanged(object sender, PropertyChangedEventArgs e)
        {
            if (updatingByUser)
                return;

            if (e.PropertyName == nameof(NodeVM.IsVisible) || e.PropertyName == nameof(NodeVM.IsChecked))
                UpdateByChildren();
        }

        private void UpdateByChildren()
        {
            var visibleChildren = Nodes.Where(n => n.IsVisible).ToArray();
            var checkedCount = visibleChildren.Count(n => n.IsChecked == true);
            if (checkedCount == 0)
                base.IsChecked = false;
            else if (checkedCount == visibleChildren.Length)
                base.IsChecked = true;
            else
                base.IsChecked = null;
        }
    }
}
