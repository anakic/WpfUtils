using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Thingie.WPF.Controls.ObjectExplorer
{
    public class NodeVM : INotifyPropertyChanged
    {
        private string text;
        private Uri imageUri;

        bool isEditing;
        public bool IsEditing { get => isEditing; set { isEditing = value; OnPropertyChanged(nameof(IsEditing)); } }

        #region private fields
        List<NodeVM> nodes;
        #endregion

        #region user interaction properties
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
        public virtual bool IsExpanded { get => isExpanded; set { isExpanded = value; OnPropertyChanged(nameof(IsExpanded)); } }
        public virtual IEnumerable<NodeVM> VisibleNodes => Nodes.Where(n => n.IsVisible);
        #endregion

        #region actions
        public Action<string> RenameAction { get; set; }
        public Action SelectAction { get; set; }

        #endregion

        #region core properties
        public int Order { get; set; }
        public object Source { get; set; }
        public Uri ImageURI { get => imageUri; set { imageUri = value; OnPropertyChanged(nameof(ImageURI)); } }
        public string Text { get => text; set { if (text != value) { RenameAction?.Invoke(value); text = value; OnPropertyChanged(nameof(Text)); } } }
        public virtual string ToolTip { get; set; }
        public List<NodeVM> Nodes { get => nodes; set { nodes = value; OnPropertyChanged(nameof(Nodes)); } }
        public virtual List<ContextCommand> ContextCommands { get; set; }
        #endregion

        public NodeVM(object source, Uri imageUri, string text, string description = null, params ContextCommand [] contextCommands)
        {
            Source = source;
            ImageURI = imageUri;
            Text = text;
            ToolTip = description;
            ContextCommands = new List<ContextCommand>(contextCommands ?? new ContextCommand[0]);
            Nodes = new List<NodeVM>();
        }

        #region filter
        public void Filter(string search)
        {
            DoFilter(new string[0], search.Split(' '));
            OnPropertyChanged(nameof(VisibleNodes));
        }

        protected void DoFilter(IEnumerable<string> path, IEnumerable<string> filterSegments)
        {
            List<string> myPath = new List<string>(path) { Text };

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

        public override string ToString()
        {
            return Text;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        internal void Visit(Action<NodeVM> n)
        {
            n(this);

            if (Nodes != null)
            {
                foreach (var child in Nodes)
                    child.Visit(n);
            }
        }
    }
}
