using System;
using System.Collections.Generic;
using System.Linq;

namespace RevitJournal.Journal.Command
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
            return content.Split(GetDelimeterSplit(), 
                                 StringSplitOptions.RemoveEmptyEntries);
        }

        public static string GetLine(IEnumerable<string> values)
        {
            if(values is null || values.Any() == false) { return string.Empty; }

            return string.Join(Delimeter, values);
        }
    }
}
