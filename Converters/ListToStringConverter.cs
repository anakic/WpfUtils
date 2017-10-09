using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace Thingie.WPF.Converters
{
    public class ListToStringConverter : MarkupExtension, IValueConverter
    {
        public string Separator { get; set; } = ", ";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string separator = GetEscapedSeparator();

            StringBuilder sb = new StringBuilder();
            if (value is IEnumerable ie)
            {
                foreach (object x in ie)
                    sb.Append($"{x}{separator}");
            }

            if (sb.Length > 0)
                sb.Remove(sb.Length - separator.Length, separator.Length);

            return sb.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string separator = GetEscapedSeparator();
            string str = value as string;

            var itemType = targetType.GetGenericArguments().Single();

            IList list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(itemType));

            foreach (string itemStr in str.Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries))
            {
                var item = System.Convert.ChangeType(itemStr, itemType);
                list.Add(item);
            }

            return list;
        }

        private string GetEscapedSeparator()
        {
            return Separator.Replace("\\r", "\r").Replace("\\n", "\n").Replace("\\t", "\t");
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
