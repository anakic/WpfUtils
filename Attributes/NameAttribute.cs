using System;

namespace Thingie.WPF.Attributes
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class NameAttributeBase : Attribute
    {
        public virtual string Name { get; protected set; }
    }

    public sealed class NameAttribute : NameAttributeBase
    {
        public NameAttribute(string name)
        {
            Name = name;
        }
    }

    public sealed class NameLocalizedAttribute : NameAttributeBase
    {
        public NameLocalizedAttribute(Type resxClass, string key)
        {
            Name = ResxHelperClass.GetKeyFromResx(resxClass, key);
        }
    }
}
