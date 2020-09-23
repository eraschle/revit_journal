using DataSource.Helper;
using DataSource.Model.FileSystem;
using System.Globalization;

namespace DataSource.Model.Product
{
    public class RevitAppFile : AFileNode
    {
        internal const string RevitAppExtension = "exe";
        public const string RevitFileName = "Revit";


        private int version = 0;
        public int Version
        {
            get
            {
                if (version == 0)
                {
                    version = GetRevitVersion();
                }
                return version;
            }
        }

        private int GetRevitVersion()
        {
            var versionIdx = Parent.Name.LastIndexOf(Constant.SpaceChar);
            return int.Parse(Parent.Name.Remove(0, versionIdx), CultureInfo.CurrentCulture);
        }

        public override string FileExtension { get; } = RevitAppExtension;
    }
}
