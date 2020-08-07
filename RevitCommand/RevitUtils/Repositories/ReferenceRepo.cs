using Autodesk.Revit.DB;

namespace RevitCommand.Repositories
{
    public static class ReferenceRepo
    {
        public static Reference Get<TElement>(TElement element) where TElement : Element
        {
            if (element is Extrusion extrusion)
            {
                return Get(extrusion);
            }
            if (element is Blend blend)
            {
                return Get(blend);
            }
            if (element is Revolution revolution)
            {
                return Get(revolution);
            }
            if (element is ReferencePlane referencePlane)
            {
                return Get(referencePlane);
            }
            if (element is SketchPlane sketchPlane)
            {
                return Get(sketchPlane);
            }
            if (element is Sketch sketch)
            {
                return Get(sketch);
            }
            if (element is ModelLine modelLine)
            {
                return Get(modelLine);
            }
            if (element is Level level)
            {
                return Get(level);
            }
            if (element is FamilyInstance instance)
            {
                return Get(instance);
            }
            return null;
        }

        public static Reference Get(Extrusion extrusion)
        {
            return Get(extrusion?.Sketch);
        }

        public static Reference Get(Blend blend)
        {
            return Get(blend?.BottomSketch);
        }

        public static Reference Get(Revolution revolution)
        {
            return Get(revolution?.Sketch);
        }

        public static Reference Get(ReferencePlane element)
        {
            return element?.GetReference();
        }

        public static Reference Get(SketchPlane element)
        {
            return element?.GetPlaneReference();
        }

        public static Reference Get(Sketch element)
        {
            return Get(element?.SketchPlane);
        }

        public static Reference Get(ModelLine modelLine)
        {
            return modelLine?.GeometryCurve.Reference;
        }

        public static Reference Get(FamilyInstance source)
        {
            if (source is null) { return null; }

            var hostFace = source.HostFace;
            if (hostFace != null)
            {
                return hostFace;
            }

            var host = GetHost(source);
            if (host is null)
            {
                host = GetLevel(source);
            }
            return Get(host);
        }

        public static Reference Get(Level level)
        {
            return level?.GetPlaneReference();
        }

        private static Level GetLevel(FamilyInstance source)
        {
            var bip = BuiltInParameter.SCHEDULE_LEVEL_PARAM;
            var elementId = GetElementId(source, bip);
            return ElementRepo.ById<Level>(source.Document, elementId);
        }

        private static Element GetHost(FamilyInstance source)
        {
            var host = source.Host;
            if (host is null)
            {
                var bip = BuiltInParameter.HOST_ID_PARAM;
                var elementId = GetElementId(source, bip);
                host = ElementRepo.ById<Element>(source.Document, elementId);
            }
            return host;
        }

        private static ElementId GetElementId(FamilyInstance source, BuiltInParameter bip)
        {
            var parameter = source.get_Parameter(bip);
            if (parameter is null) { return null; }

            return parameter.AsElementId();
        }
    }
}
