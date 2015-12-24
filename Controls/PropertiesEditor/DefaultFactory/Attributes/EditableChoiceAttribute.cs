using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Thingie.WPF.Controls.PropertiesEditor.DefaultFactory.Attributes
{
    /// <summary>
    /// Marks the specified property as an editable choice. A combobox style
    /// editor will be used for editing the property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false)]
    public class EditableChoiceAttribute : EditableAttribute
    {
        public IEnumerable Choices { get; private set; }
        public string ChoicesProperty { get; private set; }
        public Type TypeOfCollectionWithChoices { get; private set; }
        public object[] ConstructorArgs { get; private set; }

        public bool IsAsync { get; set; }
        public bool IsTextEditable { get; set; }

        public string DisplayMemberPath { get; set; }

        public EditableChoiceAttribute(string choicesProperty)
        {
            ChoicesProperty = choicesProperty;
        }

        public EditableChoiceAttribute(params object [] choices)
        {
            Choices = choices;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeOfCollectionWithChoices">
        /// The type of the collection holding the items. If the collection
        /// needs to be refreshable, derive from BindingList<T> and fire
        /// OnCollectionChanged when needed.
        /// </param>
        public EditableChoiceAttribute(Type typeOfCollectionWithChoices, params object [] constructorArgs)
        {
            TypeOfCollectionWithChoices = typeOfCollectionWithChoices;
            ConstructorArgs = constructorArgs;
        }
    }
}
