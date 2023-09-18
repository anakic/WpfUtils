using System.Windows;

namespace Thingie.WPF.Controls.PropertiesEditor
{
	/// <summary>
	/// Interaction logic for NewBatchDialog.xaml
	/// </summary>
	public partial class EditObjectDialog : Window
    {
        public EditObjectDialog()
        {
            this.AssemblySensitive_InitializeComponent("component/controls/propertieseditor/editobjectdialog.xaml");
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        public static bool EditModal(object target, bool autocommitProperties)
        {
            return EditModal(target, autocommitProperties, Application.Current.MainWindow);
        }

        public static bool EditModal(object target, bool autocommitProperties, Window owner)
        {
            EditObjectDialog dialog = new EditObjectDialog();
            dialog.DataContext = target;
            dialog.pe.AutoCommit = autocommitProperties;
            dialog.Owner = owner;
            return dialog.ShowDialog() == true;
        }
    }
}
