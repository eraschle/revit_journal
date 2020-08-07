using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace RevitCommand.RevitUtils.Builder
{
    public static class BuildManager
    {
        private static readonly Dictionary<ElementId, ElementId> CreatedElementIds
            = new Dictionary<ElementId, ElementId>();

        public static bool IsCreated(ElementId original, out ElementId created)
        {
            created = null;
            if (CreatedElementIds.ContainsKey(original))
            {
                created = CreatedElementIds[original];
            }
            return created != null;
        }

        public static void Add(ElementId original, ElementId created)
        {
            if (original is null || created is null
                || CreatedElementIds.ContainsKey(original)) { return; }

            CreatedElementIds.Add(original, created);
        }

        public static void Add(Element original, Element created)
        {
            if (original is null || created is null) { return; }

            Add(original.Id, created.Id);
        }

        public static void Add(CurveArrArray original, CurveArrArray created)
        {
            if (original is null || created is null) { return; }
          
            for (int idx = 0; idx < original.Size; idx++)
            {
                var sourceArray = original.get_Item(idx);
                for (int idy = 0; idy < sourceArray.Size; idy++)
                {
                    Add(GetId(original, idx, idy), GetId(created, idx, idy));
                }
            }
        }

        private static ElementId GetId(CurveArrArray curveArray, int first, int second)
        {
            var array = curveArray.get_Item(first);
            var curve = array.get_Item(second);
            return curve.Reference.ElementId;
        }
    }
}
