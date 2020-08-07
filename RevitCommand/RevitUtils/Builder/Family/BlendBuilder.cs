using Autodesk.Revit.DB;

namespace RevitCommand.RevitUtils.Builder.Family
{
    public class BlendBuilder : GenericFormBuilder<Blend>
    {
        public BlendBuilder(Document document, Document source) : base(document, source) { }

        protected override Blend CreateElement(Blend source)
        {
            if (source is null) { return null; }

            var sketch = source.BottomSketch;
            using (var sketchPlane = SketchBuilder.Create(sketch))
            {
                var top = source.TopProfile.get_Item(0);
                var bottom = source.BottomProfile.get_Item(0);
                return Factory.NewBlend(source.IsSolid, top, bottom, sketchPlane);
            }
        }

        protected override void MergeElement(Blend source, ref Blend created)
        {
            base.MergeElement(source, ref created);
            if (source is null || created is null) { return; }

            created.TopOffset = source.TopOffset;
            created.BottomOffset = source.BottomOffset;
        }

        protected override void AddCreatedElements(Blend source, Blend created)
        {
            if (source is null || created is null) { return; }

            BuildManager.Add(source, created);
            BuildManager.Add(source.TopProfile, created.TopProfile);
            BuildManager.Add(source.BottomProfile, created.BottomProfile);
        }
    }
}
