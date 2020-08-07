using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitCommand.Repositories;

namespace RevitCommand.RevitUtils.Builder
{
    public class DatumPlaneBuilder : AModelElementBuilder<ReferencePlane>
    {
        public DatumPlaneBuilder(Document document, Document source) : base(document, source) { }

        protected override ReferencePlane CreateElement(ReferencePlane source)
        {
            if (source is null) { return null; }

            var activeSrcId = source.Document.ActiveView.Id;
            var view = GetView(activeSrcId);
            var created = Factory.NewReferencePlane(source.BubbleEnd, source.FreeEnd, XYZ.BasisZ, view);

            return created;
        }

        protected override void MergeElement(ReferencePlane source, ref ReferencePlane created)
        {
            if (source is null || created is null) { return; }

            CopyParameter(source, created, BipToCopy);
            if (HasSubCategory(source, out var subCategory) == false
                || HasParameter(created, out var parameter) == false) { return; }

            var newSubCategory = CategoryBuilder.Create(subCategory);
            if (newSubCategory is null) { return; }

            parameter.Set(newSubCategory.Id);
        }

        protected override void AddCreatedElements(ReferencePlane source, ReferencePlane created)
        {
            BuildManager.Add(source, created);
            BuildManager.Add(GetId(source), GetId(created));
        }

        private ElementId GetId(ReferencePlane referencePlane)
        {
            var reference = ReferenceRepo.Get(referencePlane);
            return reference.ElementId;
        }

        private static IEnumerable<BuiltInParameter> BipToCopy =
            new List<BuiltInParameter>
            {
                BuiltInParameter.DATUM_VOLUME_OF_INTEREST,
                BuiltInParameter.ELEM_REFERENCE_NAME,
                BuiltInParameter.DATUM_TEXT,
                BuiltInParameter.DATUM_PLANE_DEFINES_WALL_CLOSURE,
            };

        private bool HasSubCategory(ReferencePlane plane, out Category category)
        {
            category = null;
            if (HasParameter(plane, out var parameter) == false) { return false; }

            var elementId = parameter.AsElementId();
            var planeCatId = plane.Category.Id;
            if (elementId is null
                || elementId == ElementId.InvalidElementId
                || elementId == planeCatId) { return false; }

            category = CategoryRepo.ById(plane.Document, elementId);
            return category != null;
        }

        private bool HasParameter(ReferencePlane plane, out Parameter parameter)
        {
            var bip = BuiltInParameter.CLINE_SUBCATEGORY;
            parameter = plane.get_Parameter(bip);
            return parameter != null;
        }
    }
}
