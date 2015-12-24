using Thingie.WPF.Controls.PropertiesEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Thingie.WPF.Attributes
{
    public sealed class ProxyFactoryAttribute : Attribute
    {
        public Type ProxyFactoryType { get; set; }

        public ProxyFactoryAttribute(Type proxyFactoryType)
        {
            if (!typeof(IPropertyProxyFactory).IsAssignableFrom(proxyFactoryType))
                throw new ArgumentException("The type provided to [ProxyFactoryAttribute] must implement IPropertyProxyFactory!");
            else if (proxyFactoryType.GetConstructors().All(ctor => ctor.GetParameters().Count() > 0))
                throw new ArgumentException("The custom proxy factory must have a parameterless constructor");//todo: dodati parametre ako zatreba

            ProxyFactoryType = proxyFactoryType;
        }
    }
}
