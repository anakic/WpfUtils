﻿using System.Windows;

namespace Thingie.WPF.Controls.PropertiesEditor.CustomEditing
{
	/// <summary>
	/// Interaction logic for EditDialog.xaml
	/// </summary>
	public partial class CustomEditDialog : Window
    {
        public CustomEditDialog()
        {
            this.AssemblySensitive_InitializeComponent("component/controls/propertieseditor/customediting/customeditdialog.xaml");
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
