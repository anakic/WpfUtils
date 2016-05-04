using System;
using System.Reflection;
using System.ComponentModel;

namespace Thingie.WPF.Controls.PropertiesEditor.Proxies.Base
{
	public class PropertyProxy : ChangeNotifierBase
    {
        public int Order { get; set; }

        object _target;
        public object Target
        {
            get { return _target; }
            set 
            {
                if ((_target != null)&&(_target is INotifyPropertyChanged))
                    (_target as INotifyPropertyChanged).PropertyChanged -= new PropertyChangedEventHandler(PropertyProxy_PropertyChanged);

                _target = value;

                if ((_target != null) && (_target is INotifyPropertyChanged))
                    (_target as INotifyPropertyChanged).PropertyChanged += new PropertyChangedEventHandler(PropertyProxy_PropertyChanged);
            }
        }

        void PropertyProxy_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnTargetPropertyChanged(e.PropertyName);
            if(e.PropertyName == Property.Name)
                OnPropertyChanged(() => this.Value);
        }

        public PropertyInfo Property { get; set; }

        public string Name { get; set; }

        public string Category { get; set; }

        public string Description { get; set; }

        public virtual object Value
        {
            get
            {
                return Property.GetValue(Target, null);
            }
            set
            {
                //Hack:
                //Ne moze ne postojati setter ako ga zelimo u naslijedjenim klasama
                //imati, a ova bazna klasa ne podrazumjeva pisanje u setter 
                //(ako je property definirian kao Viewable, a ne Editable)
                throw new InvalidOperationException("");
            }
        }

        protected virtual void OnTargetPropertyChanged(string property){}
    }
}
