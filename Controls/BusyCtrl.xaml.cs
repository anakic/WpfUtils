using System.Windows.Controls;

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
            this.AssemblySensitive_InitializeComponent("component/controls/busyctrl.xaml");
        }
    }
}
