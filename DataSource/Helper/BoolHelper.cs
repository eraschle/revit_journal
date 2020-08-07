using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSource.Helper
{
    public static class BoolHelper
    {
        public static string Get(bool value, CultureInfo culture = null)
        {
            if (culture is null)
            {
                culture = CultureInfo.CurrentCulture;
            }

            switch (culture.Name)
            {
                case "en-US":
                    return value.ToString();
                case "de-DE":
                case "de-CH":
                    if (value)
                    {
                        return "Ja";
                    }
                    return "Nein";
                default:
                    return value.ToString();
            }
        }

        public static string Get(string boolValue, CultureInfo culture = null)
        {
            if (bool.TryParse(boolValue, out var value) == false) { return null; }

            return Get(value, culture);
        }
    }
}
