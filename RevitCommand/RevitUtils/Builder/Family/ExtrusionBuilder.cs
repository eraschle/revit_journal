using Autodesk.Revit.DB;

namespace RevitCommand.RevitUtils.Builder.Family
{
    public class ExtrusionBuilder : GenericFormBuilder<Extrusion>
    {
        public ExtrusionBuilder(Document document, Document source) : base(document, source) { }

        protected override Extrusion CreateElement(Extrusion source)
        {
            if (source is null) { return null; }

            var sketch = source.Sketch;
            using (var newSketchPlane = SketchBuilder.Create(sketch))
            {
                return Factory.NewExtrusion(source.IsSolid, sketch.Profile, newSketchPlane, 10);
            }
        }

        protected override void MergeElement(Extrusion source, ref Extrusion created)
        {
            base.MergeElement(source, ref created);
            if (source is null || created is null) { return; }

            created.StartOffset = source.StartOffset;
            created.EndOffset = source.EndOffset;
        }

        protected override void AddCreatedElements(Extrusion source, Extrusion created)
        {
            if (source is null || created is null) { return; }
      
            BuildManager.Add(source, created);
            BuildManager.Add(source.Sketch.Profile, created.Sketch.Profile);
        }
    }
}
