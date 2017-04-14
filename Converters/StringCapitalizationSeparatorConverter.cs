using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace Thingie.WPF.Converters
{
    class StringCapitalizationSeparatorConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string str = (string)value;
            //just add a space if there are several capital letters in a row (likely an acronym)
            str = Regex.Replace(str, "([a-z])([A-Z]{2,})", m => { return string.Format("{0} {1}", m.Groups[1].Value, m.Groups[2].Value); });
            //add a space and tolower the next word
            str = Regex.Replace(str, "([a-z])([A-Z])", m => { return string.Format("{0} {1}", m.Groups[1].Value, m.Groups[2].Value.ToLower()); });
            return str;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
