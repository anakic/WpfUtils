using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Thingie.WPF.Controls.PropertiesEditor.Proxies.Base;

namespace Thingie.WPF.Controls.PropertiesEditor.Proxies
{
	public class ChoicePropertyProxy : EditablePropertyProxy
    {
        public bool IsEditable { get; set; }
        public bool IsAsync { get; set; }

        public string DeselectItemText { get; set; }

        public string DisplayMemberPath { get; set; }
        public IEnumerable Values { get { return _getChoicesFunc(); } }

        readonly Func<IEnumerable> _getChoicesFunc;

        public ChoicePropertyProxy(IEnumerable choices)
        {
            _getChoicesFunc = () => choices;
        }

        struct DeselectItem
        {
            private readonly string text;

            public DeselectItem(string text)
            {
                this.text = text;
            }

            public override string ToString() => text;
        }

        public override object Value
        {
            get
            {
                var res = base.Value;
                if (DeselectItemText != null && res == defaultValueLazy.Value)
                    return new DeselectItem(DeselectItemText);
                else
                    return res;
            }
            set
            {
                if (value is DeselectItem)
                    base.Value = defaultValueLazy.Value;
                else
                    base.Value = value;
            }
        }

        private Lazy<object> defaultValueLazy;

        readonly string _choicePropertyName;
        public ChoicePropertyProxy(string choicePropertyName)
        {
            defaultValueLazy = new Lazy<object>(() => Property.PropertyType.IsValueType ? Activator.CreateInstance(Property.PropertyType) : null);

            _choicePropertyName = choicePropertyName;
            _getChoicesFunc = () => 
            {
                try
                {
                    var choices = ((IEnumerable)Target.GetType().GetProperty(choicePropertyName).GetValue(Target, null)).OfType<object>();
                    if (!IsEditable && DeselectItemText != null)
                    {
                        choices = choices.Prepend(new DeselectItem(DeselectItemText));
                    }
                    return choices.ToArray();
                }
                catch(Exception ex)
                {
                    Trace.WriteLine($"Exception thrown from '{choicePropertyName}' while fetching choices for '{DisplayMemberPath}'. Message = {ex.Message}");
                    return Array.Empty<object>();
                }
            };
        }
        protected override void OnTargetPropertyChanged(string propertyName)
        {
            base.OnTargetPropertyChanged(propertyName);
            if (propertyName == _choicePropertyName)
                OnPropertyChanged(() => Values);
        }

        public ChoicePropertyProxy(Type collectionType, object[] constructorArgs)
            : this (Activator.CreateInstance(collectionType, constructorArgs) as IEnumerable)
        {
        }
    }
}
