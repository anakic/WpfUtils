using System.Windows.Controls;
using WinForms = System.Windows.Forms;
using System.Windows;
using System.Windows.Data;

namespace Thingie.WPF.Controls
{
	public abstract class BrowseControlBase : Control
    {
        RelayCommand _browseCommand;
        public RelayCommand BrowseCommand
        {
            get 
            {
                if (_browseCommand == null)
                    _browseCommand = new RelayCommand(p => Browse());
                return _browseCommand; 
            }
        }

        public string Path
        {
            get { return (string)GetValue(PathProperty); }
            set { SetValue(PathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Path.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PathProperty =
            DependencyProperty.Register("Path", typeof(string), typeof(BrowseControlBase), new FrameworkPropertyMetadata(){BindsTwoWayByDefault = true, DefaultUpdateSourceTrigger = UpdateSourceTrigger.LostFocus});

        public abstract void Browse();
    }

    public class BrowseFolderControl : BrowseControlBase
    {
        static BrowseFolderControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(BrowseFolderControl),
                new FrameworkPropertyMetadata(typeof(BrowseControlBase)));
        }

        public override void Browse()
        {
            WinForms.FolderBrowserDialog foldBrowseDlg = new WinForms.FolderBrowserDialog();
            foldBrowseDlg.SelectedPath = Path;
            foldBrowseDlg.ShowNewFolderButton = true;
            WinForms.DialogResult res = foldBrowseDlg.ShowDialog();
            if (res == WinForms.DialogResult.OK)
                Path = foldBrowseDlg.SelectedPath;
        }
    }

    public class BrowseFileControl : BrowseControlBase 
    {
        public string Filter
        {
            get { return (string)GetValue(FilterProperty); }
            set { SetValue(FilterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Filter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FilterProperty =
            DependencyProperty.Register("Filter", typeof(string), typeof(BrowseFileControl), new UIPropertyMetadata("*.*"));


        public bool CheckFileExists
        {
            get { return (bool)GetValue(CheckFileExistsProperty); }
            set { SetValue(CheckFileExistsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CheckFileExists.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CheckFileExistsProperty =
            DependencyProperty.Register("CheckFileExists", typeof(bool), typeof(BrowseFileControl), new UIPropertyMetadata(false));

        static BrowseFileControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(BrowseFileControl),
                new FrameworkPropertyMetadata(typeof(BrowseControlBase)));
        }

        public override void Browse()
        {
            WinForms.OpenFileDialog ofd = new WinForms.OpenFileDialog();
            ofd.ValidateNames = true;
            ofd.FileName = Path;
            ofd.CheckPathExists = true;
            ofd.CheckFileExists = CheckFileExists;
            ofd.Filter = Filter;
            WinForms.DialogResult res = ofd.ShowDialog();
            if (res == WinForms.DialogResult.OK)
                Path = ofd.FileName;
        }
    }
}
