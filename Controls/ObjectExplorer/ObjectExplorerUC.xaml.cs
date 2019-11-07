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

            protected override IEnumerable<NodeVM> GetInitialNodes() => nodes;

            IEnumerable<NodeVM> nodes;
            public RootNodeVM(IEnumerable<NodeVM> nodes)
            {
                this.nodes = nodes;
            }
        }

        public IEnumerable<NodeVM> Nodes
        {
            get { return (IEnumerable<NodeVM>)GetValue(NodesProperty); }
            set { SetValue(NodesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Items. This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NodesProperty =
            DependencyProperty.Register("Nodes", typeof(IEnumerable<NodeVM>), typeof(ObjectExplorerUC), new PropertyMetadata(new PropertyChangedCallback(NodeSourceSet)));

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
            if (Nodes == null)
                tree.DataContext = rootNode = null;
            else
            {
                // a root node to host content
                rootNode = new RootNodeVM(Nodes);
                tree.DataContext = rootNode;
            }
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
                    if ((e.OriginalSource as FrameworkElement).TemplatedParent is System.Windows.Controls.Primitives.ScrollBar == false)
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
            else if (e.Key == Key.Delete && e.OriginalSource is TextBox != true)
            {
                var node = (tree.SelectedItem as NodeVM);
                if (node.CanDelete())
                    node.Delete();
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
                node.IsEditing = false;
                var be = textBox.GetBindingExpression(TextBox.TextProperty);
                if (be.IsDirty)
                {
                    be.UpdateSource();
                    if (be.HasValidationError)
                    {
                        ShowError(be.ValidationError.Exception.Message);
                        be.UpdateTarget();
                        be.UpdateSource();
                    }
                    else
                        Dispatcher.BeginInvoke(new Action(() => tree.Items.Refresh()));
                }
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                var node = (tree.SelectedItem as NodeVM);
                var be = textBox.GetBindingExpression(TextBox.TextProperty);
                if (be.IsDirty)
                {
                    be.UpdateTarget();
                    be.UpdateSource();
                }
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
                node.IsEditing = false;

                var textBox = ((TextBox)sender);
                var be = textBox.GetBindingExpression(TextBox.TextProperty);
                if (be.IsDirty)
                {
                    be.UpdateSource();
                    if (be.HasValidationError)
                    {
                        ShowError(be.ValidationError.Exception.Message);
                        be.UpdateTarget();
                        be.UpdateSource();
                    }
                    else
                        Dispatcher.BeginInvoke(new Action(() => tree.Items.Refresh()));
                }
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
                try
                {
                    node.Move(proposedParentNode);
                }
                catch (Exception ex)
                {
                    ShowError(ex.Message);
                }
            }
        }

        // todo: allow using an event for handling errors
        private void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
