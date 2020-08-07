using Autodesk.Revit.DB;
using RevitCommand.Repositories;
using System;

namespace RevitCommand.RevitUtils.Builder.Family
{
    public class SketchPlaneBuilder : AModelElementBuilder<SketchPlane>
    {
        public SketchPlaneBuilder(Document document, Document source) : base(document, source) { }

        protected override SketchPlane CreateElement(SketchPlane source)
        {
            if (source is null) { return null; }

            var plane = source.GetPlane();
            return Create(plane.Normal, plane.Origin);
        }

        protected override void MergeElement(SketchPlane source, ref SketchPlane created) { }

        protected override void AddCreatedElements(SketchPlane source, SketchPlane created)
        {
            BuildManager.Add(source, created);
            var sourceRef = ReferenceRepo.Get(source);
            var createdRef = ReferenceRepo.Get(created);
            BuildManager.Add(sourceRef.ElementId, createdRef.ElementId);
        }

        public SketchPlane Create(XYZ normal, XYZ origin)
        {
            using (Plane geometryPlane = Plane.CreateByNormalAndOrigin(normal, origin))
            {
                if (geometryPlane is null)
                {
                    throw new Exception($"Not created > {nameof(geometryPlane)} ");
                }
                var plane = SketchPlane.Create(Document, geometryPlane);
                if (plane is null)
                {
                    throw new Exception($"Not created > {nameof(plane)} ");
                }
                return plane;
            }
        }

        public SketchPlane Create(Sketch sketch)
        {
            if (sketch is null) { return null; }

            return Create(sketch.SketchPlane);
        }
    }
}
