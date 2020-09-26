using DataSource.DataSource;
using System;
using System.Collections.Generic;

namespace DataSource.Model.FileSystem
{
    public class RevitFamily : AMetadataContainer, IEquatable<RevitFamily>
    {
        public RevitFamilyFile RevitFile { get; private set; }

        public RevitFamily(RevitFamilyFile revitFile)
        {
            RevitFile = revitFile ?? throw new ArgumentNullException(nameof(revitFile));
            SetRevitDataSource();
        }

        protected override RevitFamilyFile GetFamilyFile()
        {
            return RevitFile;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as RevitFamily);
        }

        public bool Equals(RevitFamily other)
        {
            return other != null &&
                   EqualityComparer<RevitFamilyFile>.Default.Equals(RevitFile, other.RevitFile);
        }

        public override int GetHashCode()
        {
            return 1472110217 + EqualityComparer<RevitFamilyFile>.Default.GetHashCode(RevitFile);
        }

        public static bool operator ==(RevitFamily left, RevitFamily right)
        {
            return EqualityComparer<RevitFamily>.Default.Equals(left, right);
        }

        public static bool operator !=(RevitFamily left, RevitFamily right)
        {
            return !(left == right);
        }
    }
}
