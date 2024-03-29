﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Thingie.WPF.Controls.PropertiesEditor.CustomEditing;
using System.ComponentModel;

namespace Thingie.WPF.Controls
{
	/// <summary>
	/// Interaction logic for FlagsEnumPicker.xaml
	/// </summary>
	public partial class FlagsEnumPicker : UserControl, ICustomEditor, INotifyPropertyChanged
    {
        public FlagsEnumPicker()
        {
            this.AssemblySensitive_InitializeComponent("component/controls/flagsenumpicker.xaml");
        }

        #region ICustomEditor Members

        FlagsEnumVM _vm;
        public object Value
        {
            get
            {
                return _vm.GetValue();
            }
            set
            {
                _vm = new FlagsEnumVM(value.GetType(), value);
                this.DataContext = _vm;
            }
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
        #endregion
    }



    class SelectableEnumItem
    {
        public bool IsSelected { get; set; }
        public object Value { get; set; }
    }

    class FlagsEnumVM : ChangeNotifierBase
    {
        private IEnumerable<SelectableEnumItem> _Items;
        public IEnumerable<SelectableEnumItem> Items
        {
            get
            {
                return _Items;
            }
            set
            {
                _Items = value;
                OnPropertyChanged(() => this.Items);
            }
        }

        readonly Type _enumType;
        public FlagsEnumVM(Type enumType, object value)
        {
            _enumType = enumType;
            List<SelectableEnumItem> _list = new List<SelectableEnumItem>();
            foreach(object enumVal in Enum.GetValues(enumType))
            {
                SelectableEnumItem item = new SelectableEnumItem();
                item.IsSelected = ((int)value & (int)enumVal) != 0;
                item.Value = enumVal;
                _list.Add(item);
            }
            _Items = _list.AsEnumerable();
        }

        public object GetValue()
        {
            int res = 0;
            foreach (SelectableEnumItem item in Items.Where(it=>it.IsSelected))
                res = res | (int)item.Value;
            return res;
        }
    }
}
