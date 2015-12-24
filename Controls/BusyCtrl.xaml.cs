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

namespace Thingie.WPF.Controls
{
    /// <summary>
    /// Interaction logic for BusyCtrl.xaml
    /// </summary>
    public partial class BusyCtrl : UserControl
    {
        public string Message
        {
            get { return txt.Text; }
            set { txt.Text = value; }
        }

        public BusyCtrl()
        {
            InitializeComponent();
        }
    }
}
