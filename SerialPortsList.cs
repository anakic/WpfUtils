using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using System.Text;
using System.ComponentModel;
using System.IO.Ports;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading;
using System.Windows;

namespace Riz.Common.WPF
{
    //todo: ucinit trazenje portova statickim 
    public class SerialPortsList : BindingList<string>, IDisposable
    {
        int _refreshInterval = 5000;
        public int RefreshInterval
        {
            get { return _refreshInterval; }
        }

        Timer _refreshTimer;
        public SerialPortsList()
        {
            _refreshTimer = new Timer(p => RefreshList(), null, 0, RefreshInterval);
        }

        private void RefreshList()
        {
            List<string> ports = SerialPort.GetPortNames().OrderBy(p=>p).ToList();

            if (Application.Current != null)
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    for (int i = this.Count - 1; i >= 0; i--)
                    {
                        if (!ports.Contains(this[i]))
                            this.RemoveAt(i);
                    }
                    for (int i = 0; i < ports.Count(); i++)
                    {
                        if (!this.Contains(ports[i]))
                            this.Insert(i, ports[i]);
                    }
                }));
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            //oslobodi timer ako se disposa ovaj objekt.
            //-Nisam dodavao unistavanje timera u destruktoru, jer 
            //ce se ionako pozvat timerov destruktor u kojem ce on 
            //oslobodit native timer, pa nema potrebe
            //radit poseban destruktor ovdje.
            _refreshTimer.Dispose();
        }

        #endregion
    }
}
