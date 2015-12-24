using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Thingie.WPF.Controls.PropertiesEditor;
using Thingie.WPF.Controls.PropertiesEditor.Proxies;
using Thingie.WPF.Controls.PropertiesEditor.Proxies.Base;

namespace Thingie.WPF.KeyboardShortcuts
{
    internal sealed class ShortcutServiceProxyFactory : IPropertyProxyFactory
    {
        public IEnumerable<PropertyProxy> CreatePropertyItems(object target)
        {
            var shortcutService = target as ShortcutService;
            if (shortcutService == null)
                throw new ArgumentException("This proxy factory can only be used on ShortcutService");

            PropertyInfo property = typeof(ShortcutHandle).GetProperty("KeyGesture", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            if (property == null)
                throw new Exception("This should never happen, where is the 'KeyGesture' property on 'ShortcutHandle'?!");

            int order = 0;
            foreach (var shortcutHandle in shortcutService.Shortcuts)
            {
                yield return new ShortcutPropertyProxy()
                {
                    Order = order++,
                    Target = shortcutHandle,
                    Name = shortcutHandle.Name,
                    Category = shortcutHandle.Category,
                    Property = property
                };
            }
        }
    }
}
