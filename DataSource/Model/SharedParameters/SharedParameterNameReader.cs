using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DataSource.Models.SharedParameters
{
    public static class SharedParameterReader
    {
        private const string GroupTitle = "*GROUP";
        private const string Group = "GROUP";
        private const string StartParameters = "*PARAM";
        private const string Comment = "#";
        private const char Tabolator = '\t';

        private static IList<SharedGroup> Groups = new List<SharedGroup>();

        private static IList<SharedParameter> Parameters = new List<SharedParameter>();

        public static IList<SharedParameter> GetParameters(string filepath)
        {
            if (File.Exists(filepath) == false) { return null; }

            Groups.Clear();
            Parameters.Clear();
            var content = File.ReadAllText(filepath);
            var foundParams = false;
            foreach (var line in GetLines(content))
            {
                if (line.StartsWith(Comment)) { continue; }

                if (foundParams == false)
                {
                    if (line.StartsWith(Group))
                    {
                        var colums = GetColumns(line);
                        if (int.TryParse(colums[1], out var groupId))
                        {
                            Groups.Add(new SharedGroup { Id = groupId, Name = colums[2] });
                        }
                    }
                    foundParams |= line.StartsWith(StartParameters);
                    continue;
                }

                var columns = GetColumns(line);
                if (columns is null || columns.Length != 9) { continue; }

                var parameter = new SharedParameter
                {
                    Guid = columns[1],
                    Name = columns[2],
                    Datatype = columns[3],
                    DataCategory = columns[4],
                    Group = GetGroup(columns[5]),
                    Visible = GetBool(columns[6]),
                    Description = columns[7],
                    UserModifiable = GetBool(columns[8])
                };
                if (Parameters.Contains(parameter)) { continue; }

                Parameters.Add(parameter);
            }
            return Parameters;
        }

        private static SharedGroup GetGroup(string column)
        {
            if (int.TryParse(column, out var intValue))
            {
                return Groups.FirstOrDefault(group => group.Id == intValue);
            }
            return null;
        }

        private static bool GetBool(string column)
        {
            if (int.TryParse(column, out var intValue))
            {
                return intValue == 1;
            }
            return false;
        }

        private static string[] GetColumns(string line)
        {
            return line.Split(Tabolator);
        }

        private static IList<string> GetLines(string content)
        {
            return content.Split(
                new string[] { Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
