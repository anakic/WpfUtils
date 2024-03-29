﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Thingie.WPF.Controls.ObjectExplorer
{
    /// <summary>
    /// Interaction logic for ObjectExplorerUC.xaml
    /// </summary>
    public partial class ObjectExplorerUC : UserControl
    {
        DispatcherFrame dispatcherFrame;
        private class RootNodeVM : NodeVM
        {
            public override object Image => default(Uri);

            public override string ToolTip => null;

            protected override void PopulateNodes(ObservableCollection<NodeVM> nodes)
            {
                nodes.Clear();
                childNodes.ToList().ForEach(Nodes.Add);
            }

            readonly IEnumerable<NodeVM> childNodes;
            public RootNodeVM(IEnumerable<NodeVM> nodes)
                : base("")
            {
                this.childNodes = nodes;
                if (nodes is INotifyCollectionChanged observable)
                    observable.CollectionChanged += Observable_CollectionChanged;
            }

            private void Observable_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
            {
                PopulateNodes(Nodes);
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

        public NodeVM SelectedNode
        {
            get { return (NodeVM)GetValue(SelectedNodeProperty); }
            set { SetValue(SelectedNodeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedNode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedNodeProperty =
            DependencyProperty.Register("SelectedNode", typeof(NodeVM), typeof(ObjectExplorerUC), new FrameworkPropertyMetadata(defaultValue: null, flags: FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, propertyChangedCallback: new PropertyChangedCallback(SelectedNodeeSet)));

        public static void SelectedNodeeSet(DependencyObject target, DependencyPropertyChangedEventArgs args)
        {
            var tree = (target as ObjectExplorerUC).tree;

            if (args.NewValue != tree.SelectedItem)
            {
                if (tree.IsLoaded)
                    tree.SelectItem(args.NewValue);
                else
                {
                    var selectedValue = args.NewValue;
                    RoutedEventHandler loadHandler = null;
                    loadHandler = (s, e) => 
                    {
                        tree.SelectItem(selectedValue);
                        tree.Loaded -= loadHandler;
                    };
                    tree.Loaded += loadHandler;
                }

            }
        }

        private NodeVM rootNode;

        public ObjectExplorerUC()
        {
            this.AssemblySensitive_InitializeComponent("component/controls/objectexplorer/objectexploreruc.xaml");
            tree.SelectedItemChanged += (s, e) => SelectedNode = (NodeVM)tree.SelectedItem;
        }

        private void Update()
        {
            if (Nodes == null)
                tree.DataContext = rootNode = null;
            else
            {
                // a root node to host content
                rootNode = new RootNodeVM(Nodes);
                rootNode.Visit(n => n.Initialize());
                tree.DataContext = rootNode;

                // Note/hack: wpf will not call Closing on menu items that belong to TreeView items 
                // that we just trashed (by replacing them with a new rootNode)
                contextMenu_Closing(null, null);
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

            // todo: implement debounce as infrastrcture and just use it here
            Task.Delay(300, _cts.Token)
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
            if (e.LeftButton == MouseButtonState.Pressed && !_IsDragging && !isContextMenuOpened)
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

        private void node_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                NodeVM originalEventSourceVm = (e.OriginalSource as FrameworkElement)?.DataContext as NodeVM;
                NodeVM n = (sender as FrameworkElement)?.DataContext as NodeVM;

                if (n != null && originalEventSourceVm != null && n.Equals(originalEventSourceVm))
                {
                    if (Keyboard.Modifiers == ModifierKeys.Control && n.CanSelect())
                    {
                        n.Select();
                    }

                    if (e.ClickCount == 2 && n.CanActivate())
                    {
                        n.Activate();
                        e.Handled = true;
                    }
                }
            }
            catch(Exception ex)
            {
                Debug.Fail(ex.Message);
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
            var node = (tree.SelectedItem as NodeVM);

            if (e.Key == Key.F2)
            {
                if (node?.CanRename() == true)
                    node.IsEditing = true;
            }
            else if (e.Key == Key.Space)
            {
                // todo: use ctrl+space to select, use space to toggle ischecked

                if (node?.CanSelect() == true)
                    node.Select();
            }
            else if (e.Key == Key.Delete && e.OriginalSource is TextBox != true)
            {
                if (node?.CanDelete() == true)
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
            var node = (tree.SelectedItem as NodeVM);

            if (e.Key == Key.Enter)
            {
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
                        Validation.ClearInvalid(be);
                    }
                    else
                    {
                        tree.Items.Refresh();
                    }
                }
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
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

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
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
                            Validation.ClearInvalid(be);
                        }
                        else
                            tree.Items.Refresh();
                    }
                }
            }));
        }

        private void StackPanel_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("Node"))
            {
                var node = (NodeVM)e.Data.GetData("Node");
                var proposedParentNode = (sender as StackPanel).DataContext as NodeVM;
                try
                {
                    // note: it's possible that this executes without StackPanel_DragOver being called
                    // in case of quick drag&drop with minimal movement, so we must repeat the check
                    if (node.CanMoveInternal(proposedParentNode) && node.CanMove(proposedParentNode))
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

                if (node.CanMoveInternal(proposedParentNode) && node.CanMove(proposedParentNode))
                    e.Effects = DragDropEffects.Move;
                else
                    e.Effects = DragDropEffects.None;
                e.Handled = true;
            }
        }

        private void node_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            NodeVM n = (sender as FrameworkElement).DataContext as NodeVM;

            if (n != null)
            {
                if (e.Key == Key.Enter && n.CanActivate())
                    n.Activate();
            }
        }

        // Used to prevent node moving after opening context menu and clicking on a parent folder node
        // Note: The PreviewMouseMove event is "faster" than the ContextMenuClosing event, which resulted in the node being processed like it is being moved
        bool isContextMenuOpened;

        private void contextMenu_Opening(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (dispatcherFrame == null)
                {
                    Trace.WriteLine("Pushing frame");
                    isContextMenuOpened = true;
                    Dispatcher.PushFrame(dispatcherFrame = new DispatcherFrame());
                }
            }));
        }

        private void contextMenu_Closing(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (dispatcherFrame != null)
                {
                    isContextMenuOpened = false;
                    dispatcherFrame.Continue = false;
                    dispatcherFrame = null;
                    Trace.WriteLine("Cleared frame");
                }
            }));
        }

        private void node_Unselected(object sender, RoutedEventArgs e)
        {
            NodeVM n = (sender as FrameworkElement).DataContext as NodeVM;

            if (n != null)
            {
                if (n.IsEditing)
                    n.IsEditing = false;
            }
            e.Handled = true;
        }
    }
}
