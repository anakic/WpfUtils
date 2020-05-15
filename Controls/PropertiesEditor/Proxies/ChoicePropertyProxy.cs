using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using Thingie.WPF.Controls.PropertiesEditor.Proxies.Base;

namespace Thingie.WPF.Controls.PropertiesEditor.Proxies
{
	public class ChoicePropertyProxy : EditablePropertyProxy
    {
        public bool IsEditable { get; set; }
        public bool IsAsync { get; set; }

        public string DisplayMemberPath { get; set; }
        public IEnumerable Values { get { return _getChoicesFunc(); } }

        readonly Func<IEnumerable> _getChoicesFunc;

        public ChoicePropertyProxy(IEnumerable choices)
        {
            _getChoicesFunc = () => choices;
        }

        readonly string _choicePropertyName;
        public ChoicePropertyProxy(string choicePropertyName)
        {
            _choicePropertyName = choicePropertyName;
            _getChoicesFunc = () => 
            {
                try
                {
                    return ((IEnumerable)Target.GetType().GetProperty(choicePropertyName).GetValue(Target, null)).OfType<object>().ToArray();
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
