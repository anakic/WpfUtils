using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Thingie.WPF.Controls.PropertiesEditor.CustomEditing;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections;
using System.Windows;

namespace Thingie.WPF.Controls.PropertiesEditor.CustomEditing
{
    public class ListEditor : ListBox, ICustomEditor
    {
        public ListEditor(IEnumerable items, string displayMemberPath)
        {
            this.ItemsSource = items;
            this.DisplayMemberPath = displayMemberPath;
            this.SelectionMode = SelectionMode.Multiple;
        }

        #region ICustomEditor Members

        object _value;
        public object Value
        {
            get
            {
                return this.SelectedItems;
            }
            set
            {
                this.SetSelectedItems((IEnumerable)value);
                this._value = value;
            }
        }

        #endregion
    }
}
