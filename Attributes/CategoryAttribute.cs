using System;

namespace Thingie.WPF.Attributes
{
	[AttributeUsage(AttributeTargets.Property|AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
    public class CategoryAttributeBase : Attribute
    {
        public virtual string Category { get; protected set; }
    }

    public sealed class CategoryAttribute : CategoryAttributeBase
    {
        public CategoryAttribute(string category)
        {
            Category = category;
        }
    }

    public sealed class CategoryLocalizedAttribute  : CategoryAttributeBase
    {
        public CategoryLocalizedAttribute(Type resxClass, string key)
        {
            Category = ResxHelperClass.GetKeyFromResx(resxClass, key);
        }
    }
}
