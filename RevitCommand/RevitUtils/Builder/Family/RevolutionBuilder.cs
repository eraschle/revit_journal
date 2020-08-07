using Autodesk.Revit.DB;

namespace RevitCommand.RevitUtils.Builder.Family
{
    public class RevolutionBuilder : GenericFormBuilder<Revolution>
    {
        public RevolutionBuilder(Document document, Document source) : base(document, source) { }

        protected override void AddCreatedElements(Revolution source, Revolution created) { }

        protected override Revolution CreateElement(Revolution source)
        {
            if (source is null) { return null; }

            var sketch = source.Sketch;
            using (var sketchPlane = SketchBuilder.Create(sketch))
            {
                var curve = source.Axis.GeometryCurve;
                using (var line = Line.CreateBound(curve.GetEndPoint(0), curve.GetEndPoint(1)))
                {
                    var startAngle = source.StartAngle;
                    var endAngle = source.StartAngle;
                    var created = Factory.NewRevolution(source.IsSolid, sketch.Profile, sketchPlane, line, startAngle, endAngle);
                    return created;
                }
            }
        }
    }
}
