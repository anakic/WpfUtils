using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Diagnostics;
using System.Reflection;

namespace Thingie.WPF
{
	[Serializable]
    public class ChangeNotifierBase : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members

        [field:NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged()
        {
            string caller = new StackFrame(1, false).GetMethod().Name;
            if (caller.StartsWith("set_"))
                OnPropertyChanged(caller.Substring(4));
            else
                throw new InvalidOperationException("Can only be called from a setter!");
        }

        protected virtual void OnPropertyChanged<T>(Expression<Func<T>> propFunc)
        {
            string propName = ((propFunc.Body as MemberExpression).Member as PropertyInfo).Name;
            OnPropertyChanged(propName);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
			VerifyPropertyName(propertyName);
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public void VerifyPropertyName(string propertyName)
        {
            // Verify that the property name matches a real,  
            // public, instance property on this object.
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                string msg = "Invalid property name: " + propertyName;
                Debug.Fail(msg);
            }
        }
        #endregion
    }
}
