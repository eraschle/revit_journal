using System;
using System.Collections.Generic;

namespace RevitCommand
{
    public static class ParameterListConverter
    {
        public const string Delimeter = ";";
        private static readonly string[] delimeterSplit = new string[] { Delimeter };

        private static string[] GetDelimeterSplit()
        {
            return delimeterSplit;
        }

        public static IList<string> GetList(string content)
        {
            return content.Split(GetDelimeterSplit(), StringSplitOptions.RemoveEmptyEntries);
        }

        public static string GetLine(IList<string> values)
        {
            if(values is null ||values.Count == 0) { return string.Empty; }

            return string.Join(Delimeter, values);
        }
    }
}
