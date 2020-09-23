using DataSource.Helper;
using System.Collections.Generic;
using System.Linq;

namespace DataSource.Model.FileSystem
{
    public class RevitFamilyFile : AFileNode
    {
        public const string FamilyExtension = "rfa";

        public override string FileExtension { get; } = FamilyExtension;

        public bool IsBackup()
        {
            if (NameWithoutExtension.Contains(Constant.Point) == false) { return false; }

            var nameSplit = NameWithoutExtension.Split(Constant.PointChar);
            var backup = nameSplit.LastOrDefault();
            return string.IsNullOrWhiteSpace(backup) == false
                && backup.Length == 4
                && int.TryParse(backup, out _);
        }

        public IEnumerable<RevitFamilyFile> GetRevitBackups()
        {
            var pattern = string.Concat(NameWithoutExtension, Constant.Point, Constant.Star);
            return Parent.GetFiles<RevitFamilyFile>(false, GetSearchPattern(pattern))
                         .Where(rvt => rvt.IsBackup());
        }

        public void DeleteBackups()
        {
            foreach (var file in GetRevitBackups())
            {
                file.Delete();
            }
        }
    }
}
