using System;
using System.Windows.Markup;

namespace Thingie.WPF.MarkupExtensions
{
	public class EnumItemsExtension : MarkupExtension
    {
        Type _enumType;
        public Type EnumType
        {
            get { return _enumType; }
            set { _enumType = value; }
        }

        public EnumItemsExtension()
        {
        }

        public EnumItemsExtension(Type enumType)
        {
            _enumType = enumType;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Enum.GetValues(_enumType);
        }
    }
}
