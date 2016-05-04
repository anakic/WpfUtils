using System.Collections.Generic;
using Thingie.WPF.Controls.PropertiesEditor.Proxies.Base;

namespace Thingie.WPF.Controls.PropertiesEditor
{
	/// <summary>
	/// This interface needs to be implemented by classes that wish to be used
	/// when creating property proxies for displaying a target object with the property editor.
	/// The instance is then provided to the PropertyEditor which then uses it 
	/// to create the property proxies for the object it is displaying.
	/// </summary>
	public interface IPropertyProxyFactory
    {
        IEnumerable<PropertyProxy> CreatePropertyItems(object target);
    }
}
