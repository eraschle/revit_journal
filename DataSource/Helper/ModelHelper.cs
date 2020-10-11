using DataSource.Models.FileSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Utilities.System;
using DataSource.Model.Metadata;

namespace DataSource.Helper
{
    public static class ModelHelper
    {
        private const string Tag = "Beschriftung";
        private const int AmWaShortCategoryLength = 3;

        private static readonly Dictionary<string, string> Special = new Dictionary<string, string>
        {
            { "ae", "ä" }, { "oe", "ö" }, { "ue", "ü" },
            { "Ae", "Ä" }, { "Oe", "Ö" }, { "Ue", "Ü" }
        };

        public static string ReplaceVowel(string value)
        {
            foreach (var special in Special.Keys)
            {
                if (value.Contains(special) == false) { continue; }

                value = value.Replace(special, Special[special]);
            }
            return value;
        }

        private static readonly ISet<string> AmWaShort = new HashSet<string> { "AW", "AWH", "AWZH", "AWBE" };

        private static readonly ISet<string> NotDisplayName = new HashSet<string> { "1", "50", "100" };

        public static string[] SplitedDisplay(string value)
        {
            return Path.GetFileNameWithoutExtension(value)
                .Replace(Constant.Space, Constant.Underline)
                .Replace(Constant.Minus, Constant.Underline)
                .Split(Constant.UnderlineChar);
        }

        public static bool ConatinNotDisplay(string value)
        {
            return NotDisplayName.Any(name => name.Equals(value, StringComparison.CurrentCulture))
                || AmWaShort.Any(name => name.Equals(value, StringComparison.CurrentCulture));
        }

        public static string FamilyDisplayName(Family family)
        {
            if (family is null) { return string.Empty; }
            if (string.IsNullOrWhiteSpace(family.DisplayName) == false) { return family.DisplayName; }

            var displayName = new StringBuilder();
            foreach (var split in SplitedDisplay(family.Name))
            {
                if (IsAmWaShort(split) && displayName.Length == 0) { continue; }
                if (ConatinNotDisplay(split)) { continue; }

                if (displayName.Length != 0)
                {
                    displayName.Append(Constant.Space);
                }

                var splitDisplay = ReplaceVowel(split);
                if (splitDisplay.Length < AmWaShortCategoryLength)
                {
                    splitDisplay = string.Empty;
                }
                if (family.HasCategory(out var category) && category.Name.Contains(Tag))
                {
                    displayName.Insert(0, Tag);
                }
                displayName.Append(splitDisplay);
            }
            return displayName.ToString();
        }

        private static bool IsAmWaShort(string value)
        {
            return string.IsNullOrWhiteSpace(value) == false 
                && value.ToCharArray().All(character => char.IsUpper(character)) 
                && value.Length == AmWaShortCategoryLength;
        }
    }
}
