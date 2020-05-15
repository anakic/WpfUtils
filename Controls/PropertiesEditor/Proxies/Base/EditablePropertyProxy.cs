using System;
using System.Reflection;
using System.Windows.Controls;
using Thingie.WPF.Controls.PropertiesEditor.CustomEditing;
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

        public bool AutoCommit { get; set; } = true;

        public Func<ICustomEditor> CustomEditorFactory { get; set; }

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
                object retVal = null;
                if (HasUnsavedChanges)
                    retVal = _intermediateValue;
                else
                {
                    try
                    {
                        retVal = Property.GetValue(Target, null);
                    }
                    catch (Exception ex)
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
                if (result.IsValid == false)
                    return result;
            }
            return ValidationResult.ValidResult;
        }

        public bool IsAvailable
        {
            get { return _availabilityProperty == null || ((bool)_availabilityProperty.GetValue(Target) == _availabilityValue); }
        }


        bool _availabilityValue = true;
        PropertyInfo _availabilityProperty;
        public void SetAvailabilityCondition(string availabilityCondition)
        {
            string availabilityPropertyName = availabilityCondition;
            if (availabilityCondition.StartsWith("!"))
            {
                _availabilityValue = false;
                availabilityPropertyName = availabilityCondition.Substring(1);
            }
            _availabilityProperty = Target.GetType().GetProperty(availabilityPropertyName);
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

        protected override void OnTargetPropertyChanged(string property)
        {
            base.OnTargetPropertyChanged(property);
            if (_availabilityProperty != null && property == _availabilityProperty.Name)
                OnPropertyChanged(nameof(IsAvailable));
        }
    }
}
