using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using RevitCommand.Repositories;

namespace RevitCommand.RevitUtils.Builder
{
    public class DimensionBuilder : AModelElementBuilder<Dimension>
    {
        private readonly DatumPlaneBuilder PlaneBuilder;

        public DimensionBuilder(Document document, Document source) : base(document, source)
        {
            PlaneBuilder = new DatumPlaneBuilder(Document, source);
        }

        protected override Dimension CreateElement(Dimension source)
        {
            if (source is null || source.AreReferencesAvailable == false) { return null; }

            using (var array = GetReferenceArray(source))
            {
                if (array.Size < 2) { return null; }

                using (var line = CreateLine(source))
                {
                    var viewId = source.Document.ActiveView.Id;
                    if (source.ViewSpecific)
                    {
                        viewId = source.View.Id;
                    }
                    var view = GetView(viewId);
                    var type = GetDimensionType(source);
                    var dimensionShape = source.DimensionShape;
                    Dimension created = null;
                    try
                    {
                        if (type is null)
                        {
                            created = Factory.NewLinearDimension(view, line, array);
                        }
                        else
                        {
                            created = Factory.NewLinearDimension(view, line, array, type);
                        }
                    }
                    catch (Exception ex)
                    {
                        var msg = ex.Message;
                        //TODO remove when all model elements are implemented
                        created = null;
                    }
                    return created;
                }
            }
        }

        private Line CreateLine(Dimension source)
        {
            if (!(source.Curve is Line line)) { return null; }
            if (line.IsBound)
            {
                return Line.CreateBound(line.GetEndPoint(0), line.GetEndPoint(1));
            }
            return Line.CreateUnbound(line.Origin, line.Direction);
        }

        private DimensionType GetDimensionType(Dimension dimension)
        {
            var sourceStyleType = dimension.DimensionType.StyleType;
            foreach (var type in ElementRepos.GetElements<DimensionType>())
            {
                if (type.StyleType != sourceStyleType) { continue; }

                return type;
            }
            return null;
        }

        protected override void MergeElement(Dimension source, ref Dimension created)
        {
            CopyParameter(source, created, BipToCopy);
        }

        private static IEnumerable<BuiltInParameter> BipToCopy =
            new List<BuiltInParameter>
            {
                BuiltInParameter.DIM_LEADER,
                BuiltInParameter.DIM_LABEL_IS_INSTANCE,
                BuiltInParameter.DIM_DISPLAY_EQ
            };


        protected override void AddCreatedElements(Dimension source, Dimension created) { }

        public override IEnumerable<Dimension> SourceElements(Document document)
        {
            var elements = base.SourceElements(document);
            var elementIds = ElementRepo.GetListIds(elements);
            using (var collector = new FilteredElementCollector(document, elementIds))
            {
                using (var filter = new ElementCategoryFilter(BuiltInCategory.OST_Constraints, true))
                {
                    elements = collector.OfClass(typeof(Dimension))
                                        .WherePasses(filter)
                                        .ToElements()
                                        .OfType<Dimension>();
                }
            }
            return elements;
        }

        private ReferenceArray GetReferenceArray(Dimension source)
        {
            var array = new ReferenceArray();
            var srcArray = source.References;
            for (int idx = 0; idx < srcArray.Size; idx++)
            {
                var sourceId = srcArray.get_Item(idx).ElementId;
                var sourceElement = ElementRepo.ById<Element>(source.Document, sourceId);
                if (ElementRepos.IsCreated<Element>(sourceId, out var element) == false)
                {
                    var curve = ElementRepos.ById<Element>(sourceId);
                    continue;
                }
                var reference = ReferenceRepo.Get(element);
                if (reference is null) { continue; }

                array.Append(reference);
            }
            return array;
        }

    }
}
