using System;
using System.Windows.Controls;
using Thingie.WPF.Controls.PropertiesEditor.DefaultFactory.Attributes;

namespace Thingie.WPF.Controls.PropertiesEditor.Proxies.Base
{
	public class EditablePropertyProxy : PropertyProxy
    {
        object _intermediateValue;

        ValidationResult _ValidationResult = null;
        public ValidationResult ValidationResult
        {
            get 
            {
                if (_ValidationResult == null)
                    _ValidationResult = CheckValidity(Value);
                return _ValidationResult; 
            }
            set 
            {
                _ValidationResult = value;
                OnPropertyChanged(() => this.ValidationResult); 
            }
        }

        private bool _AutoCommit = true;
        public bool AutoCommit
        {
            get { return _AutoCommit; }
            set { _AutoCommit = value; }
        }

        private bool _hasUnsavedChanges;
        public bool HasUnsavedChanges
        {
            get
            {
                return _hasUnsavedChanges;
            }
            private set
            {
                _hasUnsavedChanges = value;
                OnPropertyChanged(() => HasUnsavedChanges);
            }
        }

        public override object Value
        {
            get
            {
                object retVal=null;
                if (HasUnsavedChanges)
                    retVal = _intermediateValue;
                else
                {
                    try
                    {
                        retVal = Property.GetValue(Target, null);
                    }
                    catch(Exception ex)
                    {
                        ValidationResult = new ValidationResult(false, ex.Message);
                    }
                }
                return retVal;
            }
            set
            {
                ValidationResult = CheckValidity(value);
                if (ValidationResult.IsValid)
                {
                    _intermediateValue = value;
                    HasUnsavedChanges = true;

                    if (AutoCommit)
                        Commit();

                    //jer se vise mjesta u gui-ju moze bindat na isti propertyitem
                    OnPropertyChanged(() => this.Value);
                }
            }
        }

        protected virtual ValidationResult CheckValidity(object val)
        {
            foreach (ValidationAttributeBase valAttribute in Property.GetCustomAttributes(typeof(ValidationAttributeBase), true))
            {
                ValidationResult result = valAttribute.Validate(val);
                if(result.IsValid==false)
                    return result;
            }
            return ValidationResult.ValidResult;
        }

        public virtual void Commit()
        {
            if (HasUnsavedChanges)
            {
                Property.SetValue(Target, _intermediateValue, null);
                HasUnsavedChanges = false;
            }
        }

        public virtual void Cancel()
        {
            //discardati intermediate value
            HasUnsavedChanges = false;
            //ocisti cachirani validation result
            ValidationResult = null;
            //javlja da se value promjenio (jer je discardan intermediate val)
            OnPropertyChanged(() => this.Value);
        }
    }
}
