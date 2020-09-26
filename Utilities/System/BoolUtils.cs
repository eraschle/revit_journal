using System.Globalization;

namespace Utilities.System
{
    public static class BoolUtils
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
