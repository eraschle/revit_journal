using DataSource.Models;
using DataSource.Models.Catalog;
using DataSource.Models.Product;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataSource.Model.Metadata
{
    public enum Source { Invalid, File, Revit }

    public class Family : IModel, IEquatable<Family>, IComparable<Family>
    {
        public const string WorkplaneBased = "Arbeitsebenenbasiert";
        public const string RoomCalculation = "Raumberechnungspunkt";
        public const string SharedFamily = "Gemeinsam genutzt";
        public const string AlwaysVertical = "Immer vertikal";
        public const string BasicComponent = "Basisbauteil";
        public const string Cut = "Beim Laden mit Abzugskörper schneiden";
        public const string OmniClassNumber = "OmniClass-Nummer";
        public const string OmniClassText = "OmniClass-Titel";

        [JsonIgnore]
        public Source Source { get; set; } = Source.Invalid;

        [JsonProperty("Source")]
        public string SourceAsString
        {
            get { return GetSource(); }
            set { SetSource(value); }
        }

        public DateTime SourceUpdated { get; set; } = DateTime.Now;

        private string GetSource() { return Source.ToString(); }

        private void SetSource(string sourceValue)
        {
            if (string.IsNullOrWhiteSpace(sourceValue)) { Source = Source.Invalid; }

            foreach (Source source in Enum.GetValues(typeof(Source)))
            {
                var sourceName = source.ToString();
                if (sourceName.Equals(sourceValue, StringComparison.CurrentCulture) == false) { continue; }

                Source = source;
            }
        }

        public string Name { get; set; } = string.Empty;

        public string DisplayName { get; set; } = string.Empty;

        public string LibraryPath { get; set; } = string.Empty;

        public Category Category { get; set; } = null;

        public bool HasCategory() { return Category != null; }

        public bool HasCategory(out Category category)
        {
            category = Category;
            return HasCategory();
        }

        public OmniClass OmniClass { get; set; }

        public bool HasOmniClass(out OmniClass omniClass)
        {
            omniClass = OmniClass;
            return omniClass != null;
        }

        public void AddCatalog(ACatalog catalog)
        {
            if (catalog is null || Catalogs.Contains(catalog)) { return; }

            Catalogs.Add(catalog);
        }

        public IList<ACatalog> Catalogs { get; set; } = new List<ACatalog>();

        public bool HasCatalog<TCatalog>(out TCatalog catalog) where TCatalog : class
        {
            catalog = Catalogs.OfType<TCatalog>().FirstOrDefault();
            return catalog != null;
        }

        public bool HasCatalog<TCatalog>() where TCatalog : class
        {
            return Catalogs.OfType<TCatalog>().Any();
        }

        public DateTime Updated { get; set; } = DateTime.MinValue;

        public RevitApp Product { get; set; } = null;

        public bool HasProduct() { return Product != null; }

        public bool HasProduct(out RevitApp product)
        {
            product = Product;
            return HasProduct();
        }

        public string UniqueId { get; set; } = null;

        private readonly List<FamilyType> _FamilyTypes = new List<FamilyType>();
        public IList<FamilyType> FamilyTypes { get { return _FamilyTypes; } }

        public bool HasByName(string typeName, out FamilyType familyType)
        {
            familyType = null;
            if (HasFamilyTypes())
            {
                familyType = FamilyTypes
                    .FirstOrDefault(type => IsFamilyTypeName(type, typeName));
            }
            return familyType != null;
        }

        private bool IsFamilyTypeName(FamilyType familyType, string name)
        {
            return familyType != null
                && familyType.Name.Equals(name, StringComparison.CurrentCulture);
        }

        public bool HasFamilyTypes() { return FamilyTypes.Count > 0; }

        private readonly List<Parameter> _Parameters = new List<Parameter>();
        public IList<Parameter> Parameters { get { return _Parameters; } }

        public bool HasByName(string name, out Parameter parameter)
        {
            parameter = ByName(name);
            return parameter != null;
        }

        public Parameter ByName(string name)
        {
            return Parameters.FirstOrDefault(par => par.IsParameterName(name));
        }

        public void AddParameter(Parameter parameter)
        {
            if (parameter is null || Parameters.Contains(parameter)) { return; }

            _Parameters.Add(parameter);
            _Parameters.Sort();
        }

        public void AddParameters(ICollection<Parameter> parameters)
        {
            if (parameters is null || parameters.Count == 0) { return; }

            foreach (var parameter in parameters)
            {
                AddParameter(parameter);
            }
        }

        public void AddFamilyType(FamilyType familyType)
        {
            if (familyType is null || FamilyTypes.Contains(familyType)) { return; }

            _FamilyTypes.Add(familyType);
            _FamilyTypes.Sort();
        }

        public void AddFamilyTypes(ICollection<FamilyType> familyTypes)
        {
            if (familyTypes is null || familyTypes.Count == 0) { return; }

            foreach (var familyType in familyTypes)
            {
                AddFamilyType(familyType);
            }
        }

        public override string ToString()
        {
            return string.Format("Name: {0} [{1}] Path: {2}", Name, Product.Version, LibraryPath);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Family);
        }

        public bool Equals(Family other)
        {
            return other != null
                && Name == other.Name
                && LibraryPath == other.LibraryPath
                && UniqueId == other.UniqueId
                && FamilyTypes.SequenceEqual(other.FamilyTypes)
                && Parameters.SequenceEqual(other.Parameters);
        }

        public override int GetHashCode()
        {
            var hashCode = 985813280;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(LibraryPath);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(UniqueId);
            hashCode = hashCode * -1521134295 + EqualityComparer<IList<FamilyType>>.Default.GetHashCode(FamilyTypes);
            hashCode = hashCode * -1521134295 + EqualityComparer<IList<Parameter>>.Default.GetHashCode(Parameters);
            return hashCode;
        }

        public int CompareTo(Family other)
        {
            var compare = 0;
            if (HasCategory(out var category)
                 && other.HasCategory(out var otherCategory))
            {
                compare = category.Name.CompareTo(otherCategory.Name);
            }
            if (compare == 0)
            {
                compare = Name.CompareTo(other.Name);
            }
            return compare;
        }

        public static bool operator ==(Family left, Family right)
        {
            return EqualityComparer<Family>.Default.Equals(left, right);
        }

        public static bool operator !=(Family left, Family right)
        {
            return !(left == right);
        }

        public static bool operator <(Family left, Family right)
        {
            return left is null ? right is object : left.CompareTo(right) < 0;
        }

        public static bool operator <=(Family left, Family right)
        {
            return left is null || left.CompareTo(right) <= 0;
        }

        public static bool operator >(Family left, Family right)
        {
            return left is object && left.CompareTo(right) > 0;
        }

        public static bool operator >=(Family left, Family right)
        {
            return left is null ? right is null : left.CompareTo(right) >= 0;
        }
    }
}
