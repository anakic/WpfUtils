using System;
using Thingie.WPF.Controls.PropertiesEditor.CustomEditing;

namespace Thingie.WPF.Controls.PropertiesEditor.Proxies
{
	public class CustomPropertyProxy : TextPropertyProxy
    {
        public ICustomEditor CustomEditorControl
        {
            get
            {
                return (ICustomEditor)Activator.CreateInstance(_customEditorControlType, _constructorArgs);
            }
        }

        readonly Type _customEditorControlType;
        readonly object[] _constructorArgs;

        public CustomPropertyProxy(Type customEditorControlType, params object[] constructorArgs)
        {
            _constructorArgs = constructorArgs;
            _customEditorControlType = customEditorControlType;
        }
    }
}
