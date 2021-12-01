using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Thingie.WPF.Controls.PropertiesEditor.Proxies;
using Thingie.WPF.Controls.PropertiesEditor.DefaultFactory;
using Thingie.WPF.Controls.PropertiesEditor.Proxies.Base;
using Thingie.WPF.Attributes;
using Thingie.WPF.Controls.PropertiesEditor.CustomEditing;

namespace Thingie.WPF.Controls.PropertiesEditor
{
    public interface ICustomEditorHost
    {
        void Show(string title, ICustomEditor controlToShow, Action okAction);
    }

    class ProxyTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            string key;
            if (item is ReadonlyPropertyProxy)
                key = "readonlyProxy";
            else if (item is ShortcutPropertyProxy)
                key = "shortcutProxy";
            
            else if (item is TextPropertyProxy)
                key = "textProxy";
            else if (item is ChoicePropertyProxy)
            {
                ChoicePropertyProxy choiceProperty = (item as ChoicePropertyProxy);
                if (!choiceProperty.IsAsync && !choiceProperty.IsEditable)
                    key = "choiceProxy";
                else if (choiceProperty.IsAsync && !choiceProperty.IsEditable)
                    key = "choiceAsyncProxy";
                else if (!choiceProperty.IsAsync && choiceProperty.IsEditable)
                    key = "choiceEditableProxy";
                else
                    key = "choiceEditableAsyncProxy";
            }
            else if (item is BrowseFilePropertyProxy)
                key = "fileProxy";
            else if (item is BrowseFolderPropertyProxy)
                key = "folderProxy";
            else if (item is BoolPropertyProxy)
                key = "boolProxy";
            else
                throw new ArgumentException("Can't locate appropriate template!");

            return (DataTemplate)(container as FrameworkElement).FindResource(key);
        }
    }

    /// <summary>
    /// Interaction logic for JobPropertyEditor.xaml
    /// </summary>
    public partial class PropertiesEditorUC : UserControl
    {
        IEnumerable<PropertyProxy> _proxies;

        bool _autoCommit = true;
        public bool AutoCommit
        {
            get { return _autoCommit; }
            set 
            { 
                _autoCommit = value;
                if(_proxies!=null)
                    _proxies.OfType<EditablePropertyProxy>().ToList().ForEach(pi => pi.AutoCommit = this.AutoCommit);
            }
        }

        public static RoutedCommand CommitCommand = new RoutedCommand();
        public static RoutedCommand CancelCommand = new RoutedCommand();

        /// <summary>
        /// The factory to use to create property proxies.
        /// </summary>
        public IPropertyProxyFactory ItemFactory { get; set; } = new DefaultPropertyItemsFactory();

        /// <summary>
        /// The object that is responsible for showing custom editors. Usually a window or a flyout.
        /// </summary>
        public ICustomEditorHost CustomEditorHost { get; set; }

        /// <summary>
        /// The object whose properties should be edited
        /// </summary>
        public object Target
        {
            get { return (object)GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }

        public static RoutedCommand CustomEditCommand = new RoutedCommand("Edit", typeof(PropertiesEditorUC));

        // Using a DependencyProperty as the backing store for Target.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register("Target", typeof(object), typeof(PropertiesEditorUC), new UIPropertyMetadata(null));

        public PropertiesEditorUC()
        {
            InitializeComponent();
            this.CommandBindings.Add(new CommandBinding(CommitCommand, CommitCommand_Executed, CommitCommand_CanExecute));
            this.CommandBindings.Add(new CommandBinding(CancelCommand, CancelCommand_Executed));
        }

        public void Commit()
        {
            _proxies.Where(pp => pp is EditablePropertyProxy).Cast<EditablePropertyProxy>().ToList().ForEach(pp => pp.Commit());
        }

        void CommitCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Commit();
        }

        public bool HasErrors()
        {
            return _proxies != null && _proxies.OfType<EditablePropertyProxy>().Any(pp => !pp.ValidationResult.IsValid);
        }

        void CommitCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !HasErrors();
        }

        void CancelCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _proxies.Where(pp => pp is EditablePropertyProxy).Cast<EditablePropertyProxy>().ToList().ForEach(pp => pp.Cancel());
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == TargetProperty)
            {
                CollectionViewSource propItemsSource = (CollectionViewSource)FindResource("propsCVS");
                if (e.NewValue == null)
                    propItemsSource.Source = null;
                else
                {
                    IPropertyProxyFactory factory = ItemFactory;
                    ProxyFactoryAttribute custFactoryAtt = e.NewValue.GetType().GetCustomAttributes(true).OfType<ProxyFactoryAttribute>().SingleOrDefault();
                    if (custFactoryAtt != null)
                        factory = (IPropertyProxyFactory)Activator.CreateInstance(custFactoryAtt.ProxyFactoryType);

                    _proxies = factory.CreatePropertyItems(e.NewValue);
                    _proxies.OfType<EditablePropertyProxy>().ToList().ForEach(pi => pi.AutoCommit = this.AutoCommit);
                    propItemsSource.Source = _proxies;
                } 
            }
        }

        private DependencyObject FindTemplateRoot(DependencyObject element)
        {
            if (LogicalTreeHelper.GetParent(element) == null)
                return element;
            else
                return FindTemplateRoot(LogicalTreeHelper.GetParent(element));
        }

        private static Window FindHostWindow(FrameworkElement control)
        {
            if (control is Window)
                return (control as Window);
            else
                return FindHostWindow((FrameworkElement)LogicalTreeHelper.GetParent(control));
        }

        private void customEdit_Click(object sender, RoutedEventArgs e)
        {
            var proxy = (e.OriginalSource as Button).DataContext as EditablePropertyProxy;
            var editor = proxy.CustomEditorFactory(); 
            editor.Value = proxy.RawValue;
            this.CustomEditorHost.Show(proxy.Name, editor, () => proxy.RawValue = editor.Value);
        }
    }
}
