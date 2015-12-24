using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Thingie.WPF.Controls.PropertiesEditor.DefaultFactory.Attributes
{
    /// <summary>
    /// Marks the specified property or class as Viewable in a property grid.
    /// The property/ies will be displayed as a readonly string (converted by
    /// using the appropriate converter based on the type of the property) 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class ViewableAttribute : Attribute
    {
        //za naknadnu implementaciju... ako zatreba.
        //Ideja: da se omoguci da vise property editora gleda na isti objekt, 
        //       a svaki gleda na svoj skup propertyja objekta
        ///// <summary>
        ///// A name identifying the context in which a property is editable.
        ///// <example>
        ///// Two PropertyEditor controls can have the same target object, but display
        ///// different subsets of its properties depending on the context. 
        ///// </example>
        ///// </summary>
        //public IEnumerable<string> EditContextNames { get; set; }
    }
}
