using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Thingie.WPF;
using Thingie.WPF.Controls.PropertiesEditor.DefaultFactory.Attributes;

namespace Thingie.WPF.Controls.PropertiesEditor
{
    /// <summary>
    /// Interaction logic for NewBatchDialog.xaml
    /// </summary>
    public partial class EditObjectDialog : Window
    {
        public EditObjectDialog()
        {
            InitializeComponent();
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
