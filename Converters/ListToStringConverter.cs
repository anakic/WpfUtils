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
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			StringBuilder sb = new StringBuilder();
			if (value is IEnumerable ie)
			{
				foreach (object x in ie)
					sb.Append($"{x}, ");
			}

			if (sb.Length > 0)
				sb.Remove(sb.Length - 2, 2);

			return sb.ToString();
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
