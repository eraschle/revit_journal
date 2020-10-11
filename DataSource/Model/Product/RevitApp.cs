using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using Utilities.System;

namespace DataSource.Models.Product
{
    public class RevitApp : IModel, IEquatable<RevitApp>
    {
        public const string Metadata = "of File";

        public static RevitApp DefaultApp { get; } = new RevitApp(RevitAppFile.RevitFileName, 1);

        [JsonIgnore]
        public RevitAppFile AppFile { get; }

        public string Name { get; set; }

        public int Version { get; set; }

        [JsonIgnore]
        public string Language { get; private set; } = "DEU";

        [JsonIgnore]
        public bool ShowSlash { get; private set; } = false;

        [JsonIgnore]
        public bool StartMinimized { get; private set; } = true;

        public RevitApp() { }

        public RevitApp(RevitAppFile appFile)
        {
            AppFile = appFile ?? throw new ArgumentNullException(nameof(appFile));
            Name = AppFile.NameWithoutExtension;
            Version = AppFile.Version;
        }

        public RevitApp(string name, int version)
        {
            Name = name;
            Version = version;
        }

        [JsonIgnore]
        public string ProductName
        {
            get
            {
                var suffix = UseMetadata ? Metadata : Version.ToString(CultureInfo.CurrentCulture);
                return string.Concat(Name, Constant.Space, suffix);
            }
        }

        [JsonIgnore]
        public bool UseMetadata { get { return Equals(DefaultApp); } }

        [JsonIgnore]
        public bool Executable
        {
            get { return AppFile != null && AppFile.Exists(); }
        }

        public override string ToString()
        {
            return ProductName;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as RevitApp);
        }

        public bool Equals(RevitApp other)
        {
            return other != null &&
                   Name == other.Name &&
                   Version == other.Version;
        }

        public override int GetHashCode()
        {
            var hashCode = 2112831277;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + Version.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(RevitApp left, RevitApp right)
        {
            return EqualityComparer<RevitApp>.Default.Equals(left, right);
        }

        public static bool operator !=(RevitApp left, RevitApp right)
        {
            return !(left == right);
        }
    }
}
