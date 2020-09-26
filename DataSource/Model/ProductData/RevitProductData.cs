using DataSource.Helper;
using DataSource.Model.Catalog;
using DataSource.Model.Product;
using System.Collections.Generic;

namespace DataSource.Model.ProductData
{
    public class RevitProductData
    {
        public const string NoData = "No data available";
        public const string FileNameData = "RevitProductData";

        public static string GetFileName(int version)
        {
            return string.Join(Constant.Underline, FileNameData, version);
        }

        public static OmniClass DefaultOmniClass { get; } = new OmniClass { IdArray = new int[] { 0 }, Name = NoData };

        public static Category DefaultCategory { get; } = new Category { Id = "-1", Name = NoData };

        public static RevitParameterGroup DefaultParameterGroup { get; } = new RevitParameterGroup { Id = "-1", Name = NoData };

        public static RevitEnum DefaultRevitEnum { get; } = new RevitEnum { Id = "-1", Name = NoData };

        public string Name { get; set; }

        public int Version { get; set; }

        public IList<OmniClass> OmniClasses { get; set; } = new List<OmniClass>();

        public IList<Category> Categories { get; } = new List<Category>();

        public IList<RevitEnum> PlacementTypes { get; } = new List<RevitEnum>();

        public IList<RevitEnum> BuiltInParameters { get; } = new List<RevitEnum>();

        public IList<RevitParameterGroup> ParameterGroups { get; } = new List<RevitParameterGroup>();

        public IList<RevitEnum> ParameterTypes { get; } = new List<RevitEnum>();

        public IList<RevitEnum> DisplayUnitTypes { get; } = new List<RevitEnum>();

        public IList<RevitEnum> UnitSymbolTypes { get; } = new List<RevitEnum>();

        public IList<RevitEnum> UnitTypes { get; } = new List<RevitEnum>();

        internal void SetDefaultData(RevitApp app)
        {
            if (OmniClasses.Count == 0)
            {
                OmniClasses = OmniClassManager.GetOmniClasses(app);
            }
            Check(OmniClasses, DefaultOmniClass);
            Check(Categories, DefaultCategory);
            Check(PlacementTypes, DefaultRevitEnum);
            Check(BuiltInParameters, DefaultRevitEnum);
            Check(ParameterGroups, DefaultParameterGroup);
            Check(ParameterTypes, DefaultRevitEnum);
            Check(DisplayUnitTypes, DefaultRevitEnum);
            Check(UnitSymbolTypes, DefaultRevitEnum);
            Check(UnitTypes, DefaultRevitEnum);
        }

        private static void Check<TModel>(ICollection<TModel> models, TModel defaultModel)
        {
            if (models.Count == 0)
            {
                models.Add(defaultModel);
            }
        }
    }
}
