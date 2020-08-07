using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace RevitCommand.RevitUtils.Builder.Family
{
    /// <summary>
    /// More extamples in SDK Samples
    /// </summary>
    public abstract class GenericFormBuilder<TElement> : AModelElementBuilder<TElement> where TElement : GenericForm
    {
        private readonly MaterialBuilder MaterialBuilder;

        protected SketchPlaneBuilder SketchBuilder { get; private set; }

        protected GenericFormBuilder(Document document, Document source) : base(document, source)
        {
            MaterialBuilder = new MaterialBuilder(Document, source);
            SketchBuilder = new SketchPlaneBuilder(Document, source);
        }

        protected override void MergeElement(TElement source, ref TElement created)
        {
            if (source is null || created is null) { return; }

            created.SetVisibility(source.GetVisibility());
            created.SetVisibility(source.GetVisibility());
            var subCategory = GetCategory(source.Subcategory);
            if (subCategory != null)
            {
                created.Subcategory = subCategory;
            }

            CopyParameter(source, created, BipToCopy);
        }

        private static IEnumerable<BuiltInParameter> BipToCopy =
            new List<BuiltInParameter> { BuiltInParameter.MATERIAL_ID_PARAM };

        private Category GetCategory(Category oSubCategory)
        {
            if (oSubCategory is null) { return null; }

            var subCategory = CategoryBuilder.Create(oSubCategory);
            if (subCategory is null) { return null; }

            var newMaterial = MaterialBuilder.Create(oSubCategory.Material);
            if (newMaterial != null)
            {
                subCategory.Material = newMaterial;
            }
            return subCategory;
        }
    }
}
