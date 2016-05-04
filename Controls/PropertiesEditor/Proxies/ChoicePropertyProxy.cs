using System;
using System.Collections;
using Thingie.WPF.Controls.PropertiesEditor.Proxies.Base;

namespace Thingie.WPF.Controls.PropertiesEditor.Proxies
{
	public class ChoicePropertyProxy : EditablePropertyProxy
    {
        public bool IsEditable { get; set; }
        public bool IsAsync { get; set; }

        public string DisplayMemberPath { get; set; }
        public IEnumerable Values { get { return _getChoicesFunc(); } }

        Func<IEnumerable> _getChoicesFunc;

        public ChoicePropertyProxy(IEnumerable choices)
        {
            _getChoicesFunc = () => choices;
        }

        string _choicePropertyName;
        public ChoicePropertyProxy(string choicePropertyName)
        {
            _choicePropertyName = choicePropertyName;
            _getChoicesFunc = () => (IEnumerable)Target.GetType().GetProperty(choicePropertyName).GetValue(Target, null);
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
