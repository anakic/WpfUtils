using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Thingie.WPF.Attributes
{
    public static class ResxHelperClass
    {
        public static string GetKeyFromResx(Type resxClass, string key)
        {
            if (string.IsNullOrEmpty(key))
                return null;

            string val;

            PropertyInfo[] properties = resxClass.GetProperties(BindingFlags.Static | BindingFlags.Public);

            PropertyInfo nameLocProperty = properties.Where(pi => pi.Name == key).SingleOrDefault();
            if (nameLocProperty == null)
            {
                val = "#" + key;//ako nije pronađeno: '#' je oznaka da nije dobro lokalizirano
            }
            else
            {
                val = (string)nameLocProperty.GetValue(null, null);
            }

            return val;
        }
    }
}
