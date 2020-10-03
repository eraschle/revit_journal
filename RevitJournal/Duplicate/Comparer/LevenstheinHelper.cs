using System.Globalization;
using Utilities.Helper;

namespace RevitJournal.Duplicate.Comparer
{
    public static class LevenstheinHelper
    {
        public static string LevenstheinAsString(int distance)
        {
            return distance == 0 ? string.Empty : distance.ToString(CultureInfo.CurrentCulture);
        }

        public static int ComputeLevensthein(string original, string other)
        {
            return Levensthein.GetDistance(original, other);
        }
    }
}
