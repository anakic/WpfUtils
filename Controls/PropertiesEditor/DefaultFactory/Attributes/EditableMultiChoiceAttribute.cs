using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Thingie.WPF.Controls.PropertiesEditor.DefaultFactory.Attributes
{
    public class EditableMultiChoiceAttribute : EditableChoiceAttribute
    {
        public EditableMultiChoiceAttribute(string choicesProperty):base(choicesProperty)
        {

        }

        public EditableMultiChoiceAttribute(Type typeOfCollectionWithChoices) : base(typeOfCollectionWithChoices)
        {

        }
    }
}
