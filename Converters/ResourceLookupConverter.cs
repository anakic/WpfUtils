using System;
using System.Windows.Markup;
using System.Resources;
using System.Windows.Data;

namespace Thingie.WPF.Converters
{
	public class ResourceLookupConverter : MarkupExtension, IValueConverter
    {
        ResourceManager _resMan;

        Type _dictionaryType;
        public Type DictionaryType
        {
            get { return _dictionaryType; }
            set 
            { 
                _dictionaryType = value;
                _resMan = (ResourceManager)DictionaryType.GetProperty("ResourceManager").GetValue(null, null);
            }
        }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return _resMan.GetObject(value.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
