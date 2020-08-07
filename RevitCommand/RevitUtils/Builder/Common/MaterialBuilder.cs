using Autodesk.Revit.DB;

namespace RevitCommand.RevitUtils.Builder
{
    public class MaterialBuilder : AModelElementBuilder<Material>
    {
        public MaterialBuilder(Document document, Document source) : base(document, source) { }

        protected override Material CreateElement(Material source)
        {
            if (source is null) { return null; }

            if (Material.IsNameUnique(Document, source.Name))
            {
                ElementId materialId = Material.Create(Document, source.Name);
                return ElementRepos.ById<Material>(materialId);
            }
            return ElementRepos.ByName<Material>(source.Name);
        }

        protected override void MergeElement(Material source, ref Material created)
        {
            if (source is null || created is null) { return; }

            created.MaterialCategory = source.MaterialCategory;
            created.MaterialClass = source.MaterialClass;
            //TODO May other function have to be called
        }

        protected override void AddCreatedElements(Material source, Material created) { }
    }
}
