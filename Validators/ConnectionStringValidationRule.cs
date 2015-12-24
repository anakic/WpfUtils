using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Globalization;
using System.Data.SqlClient;

namespace Thingie.WPF.Validators
{
    public class ConnectionStringValidationRule : ValidationRule
    {
        DateTime _lastValidationTime;
        string _lastValidationValue;
        ValidationResult _lastResult;

        int _refreshInterval = 5;

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (_lastValidationTime.AddSeconds(_refreshInterval) < DateTime.Now)
            {
                _lastValidationTime = DateTime.Now;
                _lastValidationValue = value as string;

                SqlConnection conn = new SqlConnection();
                try
                {
                    conn.ConnectionString = value as string;
                    conn.Open();
                }
                catch (Exception ex)
                {
                    _lastResult = new ValidationResult(false, ex.Message);
                }
                conn.Close();
                _lastResult = ValidationResult.ValidResult;
                return _lastResult;
            }
            else
                return _lastResult;
        }
    }
}
