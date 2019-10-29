using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Thingie.WPF.Controls.ObjectExplorer
{
    /// <summary>
    /// Interaction logic for ObjectExplorerUC.xaml
    /// </summary>
    public partial class ObjectExplorerUC : UserControl
    {
        private class RootNodeVM : NodeVM
        {
            public override Uri ImageURI => default(Uri);

            public override string Name { get; set; }

            public override string ToolTip => null;
        }

        ConditionalWeakTable<object, NodeVM> NodesCache = new ConditionalWeakTable<object, NodeVM>();

        public IEnumerable<object> Items
        {
            get { return (IEnumerable<object>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Items. This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", typeof(IEnumerable<object>), typeof(ObjectExplorerUC));


        public INodeSource NodesSource
        {
            get { return (INodeSource)GetValue(NodesSourceProperty); }
            set { SetValue(NodesSourceProperty, value); }
        }
        public static readonly DependencyProperty NodesSourceProperty =
            DependencyProperty.Register(nameof(NodesSource), typeof(INodeSource), typeof(ObjectExplorerUC), new PropertyMetadata(new PropertyChangedCallback(NodeSourceSet)));

        public static void NodeSourceSet(DependencyObject target, DependencyPropertyChangedEventArgs args)
        {
            (target as ObjectExplorerUC).Update();
        }

        private NodeVM rootNode;

        public ObjectExplorerUC()
        {
            InitializeComponent();
        }

        private void Update()
        {
            var control = this;

            var Nodes = GetNodes(NodesSource.GetItems());

            if (Nodes == null)
                control.tree.DataContext = control.rootNode = null;
            else
            {
                // a root node to host content
                control.rootNode = new RootNodeVM();
                control.rootNode.Nodes = Nodes;
                control.tree.DataContext = control.rootNode;
            }
        }

        private List<NodeVM> GetNodes(IEnumerable<object> objects)
        {
            if (objects == null)
                return new List<NodeVM>();

            List<NodeVM> nodes = new List<NodeVM>();
            foreach (var obj in objects)
            {
                NodeVM node;
                if (NodesCache.TryGetValue(obj, out node) == false)
                {
                    node = NodesSource.GetNode(obj);
                    NodesCache.Add(obj, node);
                }
                node.Nodes = GetNodes(NodesSource.GetChildObjects(obj));
                nodes.Add(node);
            }
            return nodes;
        }

        private void btnClearFilter_Click(object sender, RoutedEventArgs e)
        {
            txtFilter.Text = string.Empty;
        }

        CancellationTokenSource _cts;
        private void txtFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();

            Task.Delay(500, _cts.Token)
                .ContinueWith(t =>
                {
                    if (t.Status == TaskStatus.RanToCompletion)
                    {
                        Dispatcher.Invoke(() => rootNode.Filter(txtFilter.Text));
                    }
                });
        }

        Point _startPoint;
        bool _IsDragging = false;
        private void tree_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(null);
        }

        private void tree_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed ||
                e.RightButton == MouseButtonState.Pressed && !_IsDragging)
            {
                Point position = e.GetPosition(null);
                if (Math.Abs(position.X - _startPoint.X) >
                        SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(position.Y - _startPoint.Y) >
                        SystemParameters.MinimumVerticalDragDistance)
                {
                    // todo: only do this if dragging TreeViewItem (this is interefering with scroll)
                    if((e.OriginalSource as FrameworkElement).TemplatedParent is System.Windows.Controls.Primitives.ScrollBar == false)
                        StartDrag(e);
                }
            }
        }

        private void StartDrag(MouseEventArgs e)
        {
            var source = (e.OriginalSource as FrameworkElement);
            NodeVM node = (tree.SelectedItem as NodeVM);
            if (node != null)
            {
                _IsDragging = true;
                DataObject data = new DataObject("Node", node);
                DragDropEffects de = DragDrop.DoDragDrop(source, data, DragDropEffects.All);
                _IsDragging = false;
            }
        }

        private void tree_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            NodeVM n = (sender as FrameworkElement).DataContext as NodeVM;
            if (n != null && Keyboard.Modifiers == ModifierKeys.Control && n.CanSelect())
            {
                n.Select();
            }
        }

        private void tree_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);
            if (treeViewItem != null)
            {
                treeViewItem.Focus();
                e.Handled = true;
            }
        }

        static TreeViewItem VisualUpwardSearch(DependencyObject source)
        {
            while (source != null && !(source is TreeViewItem))
                source = VisualTreeHelper.GetParent(source);

            return source as TreeViewItem;
        }

        private void TreeView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F2)
            {
                var node = (tree.SelectedItem as NodeVM);
                if (node?.CanRename() == true)
                    node.IsEditing = true;
            }
        }


        private void TextBox_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            TextBox textBox = (sender as TextBox);
            if (textBox.Visibility == Visibility.Visible)
            {
                textBox.Focus();
                textBox.SelectAll();
            }
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox textBox = (sender as TextBox);
            if (e.Key == Key.Enter)
            {
                var node = (tree.SelectedItem as NodeVM);
                textBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                node.IsEditing = false;
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                var node = (tree.SelectedItem as NodeVM);
                textBox.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                textBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                node.IsEditing = false;
                e.Handled = true;
            }
            else if (e.Key == Key.Subtract || e.Key == Key.Add)
            {
                e.Handled = true;

                var text = e.Key == Key.Subtract ? "-" : "+";
                var target = Keyboard.FocusedElement;
                var routedEvent = TextCompositionManager.TextInputEvent;

                target.RaiseEvent(
                    new TextCompositionEventArgs
                        (
                             InputManager.Current.PrimaryKeyboardDevice,
                            new TextComposition(InputManager.Current, target, text)
                        )
                    {
                        RoutedEvent = routedEvent
                    });
            }
        }

        private void TextBox_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            var node = (tree.SelectedItem as NodeVM);
            if (node != null)
            {
                ((TextBox)sender).GetBindingExpression(TextBox.TextProperty).UpdateSource();
                node.IsEditing = false;
                tree.Items.Refresh();
            }
        }

        private void StackPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var node = (tree.SelectedItem as NodeVM);

            if (e.ClickCount == 2 && node.CanActivate())
                node.Activate();
        }

        private void StackPanel_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("Node"))
            {
                var node = (NodeVM)e.Data.GetData("Node");
                var proposedParentNode = (sender as StackPanel).DataContext as NodeVM;
                Trace.WriteLine($"{node}->{proposedParentNode}");
            }
        }

        private void StackPanel_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("Node"))
            {
                var node = (NodeVM)e.Data.GetData("Node");
                var proposedParentNode = (sender as StackPanel).DataContext as NodeVM;

                if (node.CanMove(proposedParentNode))
                    e.Effects = DragDropEffects.Move;
                else
                    e.Effects = DragDropEffects.None;
                e.Handled = true;
            }
        }
    }
}
