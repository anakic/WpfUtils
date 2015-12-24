using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows.Data;
using System.Text.RegularExpressions;
using System.ComponentModel;

namespace Thingie.WPF.Converters
{
    public class MapConverter : MarkupExtension, IValueConverter
    {
        public string DefaultString { get; set; }

        public string NullString { get; set; }

        public MapConverter(string expression):this()
        {
            MapExpression = expression;
        }

        public MapConverter()
        {
            DefaultString = "_default";
            NullString = "_null";
        }

        Dictionary<string, string> _propValMap = new Dictionary<string, string>();
        private string _mapExpression;
        [ConstructorArgument("expression")]
        public string MapExpression
        {
            get { return _mapExpression; }
            set 
            { 
                _mapExpression = value;
                FillMap();
            }
        }

        private void FillMap()
        {
            string mapItemsPattern = @"((?<item>([^;]|((?<=\\);))+)(;|$))+";
            string partsPattern = @"^(?<left>([^:]|(?<=\\))*):(?<right>.*)$";

            Match itemsMatch = Regex.Match(MapExpression, mapItemsPattern);

            foreach (Capture item in itemsMatch.Groups["item"].Captures)
            {
                Match partsMatch = Regex.Match(item.Value, partsPattern);
                _propValMap.Add(partsMatch.Groups["left"].Value.ToLower().Trim(), partsMatch.Groups["right"].Value.Trim());
            }
        }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string valAsLowerCaseStr = (value == null ? NullString : value.ToString().ToLower());
            
            string retValAsString;
            if (_propValMap.ContainsKey(valAsLowerCaseStr))
                retValAsString = _propValMap[valAsLowerCaseStr];
            else if (_propValMap.ContainsKey(DefaultString))
                retValAsString = _propValMap[DefaultString];
            else
                return value;

            if (retValAsString == NullString)
                return null;
            else
            {
                if (targetType == typeof(object))
                    return retValAsString;
                else
                    return TypeDescriptor.GetConverter(targetType).ConvertFromString(retValAsString);
            }
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
