using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace RevitCommand.Families.ImageExport
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class ThreeDImageRevitCommand : AFamilyImageExportCommand<ThreeDImageAction>
    {
        private const string ImageSuffix = "3D";

        private readonly ElementFilter Not3dCategoriesFilter;

        public ThreeDImageRevitCommand() : base()
        {
            var categories = new List<BuiltInCategory>
            {
                BuiltInCategory.OST_IOSRoomCalculationPoint,
                BuiltInCategory.OST_ConnectorElem,
                BuiltInCategory.OST_LightingFixtureSource,
                BuiltInCategory.OST_ReferenceLines,
                BuiltInCategory.OST_ReferencePoints,
                BuiltInCategory.OST_Dimensions,
                BuiltInCategory.OST_IOSSketchGrid
            };
            Not3dCategoriesFilter = new ElementMulticategoryFilter(categories, true);
        }

        private View3D View { get { return Document.ActiveView as View3D; } }

        protected override Result ExportImage(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            if (SetCorrectView(IsCorrectView) == false)
            {
                return Result.Succeeded;
            }

            var imageFilePath = Action.Background.Value;
            var collector = GetViewCollector().WherePasses(Not3dCategoriesFilter);
            var excludingIds = new List<ElementId>();
            if (HasFamilyInstances(out var instanceCollector))
            {
                excludingIds.AddRange(instanceCollector.ToElementIds());
            }
            if (HasDetailElements(out var detailCollector))
            {
                excludingIds.AddRange(detailCollector.ToElementIds());
            }
            if (HasModelText(out var modelTextCollector)
                && HaveModelTextSameLocation(modelTextCollector))
            {
                var firstId = modelTextCollector.FirstElementId();
                var elementIds = modelTextCollector.ToElementIds();
                elementIds.Remove(firstId);
                excludingIds.AddRange(elementIds);
            }
            if (HasGroups(out var groups))
            {
                foreach (var group in groups)
                {
                    excludingIds.AddRange(GetGroupElementIds(group));
                }
            }
            if (excludingIds.Count > 0)
            {
                collector.Excluding(excludingIds);
            }
            HideOtherElements(collector);

            if (instanceCollector != null
                && (AreAllElementsHidden(out var visibleCollector)
                || visibleCollector.GetElementCount() == 1))
            {
                UnhideElements(instanceCollector);
            }

            if (HasTemplateExtrusion(collector, out var element))
            {
                HideElement(element);
            }

            if (HasFamilyTypeParameter(out var familySymbol))
            {
                if (HasFamilyInstancesOfSymbol(familySymbol, false, out var instances))
                {
                    UnhideElements(instances);
                }
            }

            if (HasLinesNotAroundExtrusion(out var linesCollector))
            {
                HideElements(linesCollector);
            }

            if (AreAllElementsHidden(out visibleCollector)
                || (OnlyOneFamilyInstanceWithSameCategory(visibleCollector) == false
                    && ContainsGenericForm(visibleCollector) == false))
            {
                return Result.Succeeded;
            }

            if (ContainsOneExtrusionAndIntrusion(visibleCollector, out var intrusion))
            {
                HideElement(intrusion);
            }

            SetOrientation();
            using (var transaction = new Transaction(Document, "SetupView"))
            {
                transaction.Start();
                View.Scale = 1;
                View.DisplayStyle = DisplayStyle.RealisticWithEdges;
                View.DetailLevel = ViewDetailLevel.Fine;
                View.CropBoxActive = false;

                View.SunlightIntensity = 40;
                View.ShadowIntensity = 60;

                var renderOptions = View.GetRenderingSettings();
                renderOptions.LightingSource = LightingSource.ExteriorSunAndArtificial;

                if (HasImageBackground(imageFilePath, out var background))
                {
                    View.SetBackground(background);
                }

                SetLineColorAndWeight(new Color(0, 0, 0), 4);
                SetMaterial();

                transaction.Commit();
            }

            var options = Manager.Get3dOptions(ImageSuffix);
            Manager.ExportImage(options);
            return Result.Succeeded;
        }

        private bool ContainsOneExtrusionAndIntrusion(FilteredElementCollector visibleCollector, out Extrusion intrusion)
        {
            intrusion = null;
            if (visibleCollector.GetElementCount() != 2) { return false; }

            Extrusion extrusion = null;
            foreach (var element in visibleCollector)
            {
                if (!(element is Extrusion geometry)) { continue; }

                if (IsIntrusion(geometry))
                {
                    intrusion = geometry;
                }
                else
                {
                    extrusion = geometry;
                }
            }
            if (extrusion is null || intrusion is null) { return false; }

            var extBb = extrusion.get_BoundingBox(View);
            var intBb = intrusion.get_BoundingBox(View);
            return extBb.Min.IsAlmostEqualTo(intBb.Min) && extBb.Max.IsAlmostEqualTo(intBb.Max);
        }

        private bool HasLinesNotAroundExtrusion(out FilteredElementCollector lineCollector)
        {
            var lines = new List<ElementId>();
            foreach (var intrusion in GetExtrusuion())
            {
                var filter = GetBoundingBoxFilter(intrusion);
                var extLineCollector = GetLineCollector().WherePasses(filter);
                foreach (var element in extLineCollector.ToElements())
                {
                    if (lines.Contains(element.Id)) { continue; }

                    lines.Add(element.Id);
                }
            }
            Unhide(lines);
            lineCollector = GetLineCollector();
            if (lines.Count > 0)
            {
                lineCollector.Excluding(lines);
            }
            return lineCollector.GetElementCount() > 0;
        }

        private FilteredElementCollector GetLineCollector()
        {
            return new FilteredElementCollector(Document)
                .OfCategory(BuiltInCategory.OST_Lines);
        }

        private ElementFilter GetBoundingBoxFilter(Extrusion intrusion)
        {
            var boundingBox = intrusion.get_BoundingBox(View);
            var outline = new Outline(boundingBox.Min, boundingBox.Max);
            var tolerence = UnitUtils.ConvertToInternalUnits(1, DisplayUnitType.DUT_CENTIMETERS);
            return new LogicalOrFilter(new BoundingBoxIsInsideFilter(outline, tolerence),
                                       new BoundingBoxIntersectsFilter(outline, tolerence));
        }

        private IList<Extrusion> GetExtrusuion()
        {
            var intrusion = new List<Extrusion>();
            var detailCollector = GetViewCollector().OfClass(typeof(GenericForm));
            foreach (var element in detailCollector.ToElements())
            {
                if (!(element is Extrusion extrusion)
                    || IsIntrusion(extrusion) == false) { continue; }

                intrusion.Add(extrusion);
            }
            return intrusion;
        }

        private bool IsIntrusion(Extrusion extrusion)
        {
            var bip = BuiltInParameter.ELEMENT_IS_CUTTING;
            var parameter = extrusion.get_Parameter(bip);
            return parameter != null && parameter.AsInteger() == 1;
        }

        private Material CreateMaterial()
        {
            var materialId = Material.Create(Document, "ImageExport");
            var material = Document.GetElement(materialId) as Material;
            material.SurfacePatternColor = new Color(220, 220, 220);
            //var fillPattern = FillPatternElement.GetFillPatternElementByName(Document, FillPatternTarget.Drafting, "Füllung");
            //material.SurfacePatternId = fillPattern.Id;
            //material.Transparency = 0;
            //material.Smoothness = 120;
            //material.Shininess = 64;
            material.UseRenderAppearanceForShading = false;
            return material;
        }

        [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private void SetMaterial()
        {
            if (AreAllElementsHidden(out var visibleCollector)
                || Document.IsFamilyDocument == false) { return; }

            var bip = BuiltInParameter.MATERIAL_ID_PARAM;
            var material = CreateMaterial();
            Document.OwnerFamily.FamilyCategory.Material = material;
            SetFamilyParameterMaterial(material);
            foreach (var element in visibleCollector.ToElements())
            {
                try
                {
                    SetCategoryMaterial(element.Category, material);

                    var parameter = element.get_Parameter(bip);
                    if (parameter is null || parameter.IsReadOnly) { continue; }

                    parameter.Set(material.Id);
                }
                catch (Exception)
                {
                    continue;
                }
            }
        }

        private void SetCategoryMaterial(Category category, Material material)
        {
            if (category != null && category.CategoryType != CategoryType.Internal)
            {
                category.Material = material;
            }
            if (category.CanAddSubcategory)
            {
                foreach (Category subCategory in category.SubCategories)
                {
                    SetCategoryMaterial(subCategory, material);
                }
            }
        }

        private void SetFamilyParameterMaterial(Material material)
        {
            var manager = Document.FamilyManager;
            foreach (FamilyParameter parameter in manager.Parameters)
            {
                if (parameter.Definition.ParameterType != ParameterType.Material) { continue; }

                manager.Set(parameter, material.Id);
            }
        }


        private bool OnlyOneFamilyInstanceWithSameCategory(FilteredElementCollector visibleCollector)
        {
            if (visibleCollector.GetElementCount() != 1) { return false; }

            var element = visibleCollector.FirstElement();
            if (element is null) { return false; }

            var category = element.Category;
            return category != null && category.Id.Equals(Document.OwnerFamily.FamilyCategoryId);
        }


        private bool HasFamilyTypeParameter(out FamilySymbol familySymbol)
        {
            familySymbol = null;
            var familyManager = Document.FamilyManager;
            var currentType = familyManager.CurrentType;
            if (currentType is null) { return false; }

            var familyCategory = Document.OwnerFamily.FamilyCategory;
            foreach (FamilyParameter parameter in familyManager.Parameters)
            {
                if (parameter.Definition.ParameterType != ParameterType.FamilyType) { continue; }

                var elementId = currentType.AsElementId(parameter);
                var element = Document.GetElement(elementId);

                if (!(element is FamilySymbol symbol)
                    || symbol.Category.Id != familyCategory.Id) { continue; }

                familySymbol = symbol;
            }
            return familySymbol != null;
        }

        private bool HasTemplateExtrusion(FilteredElementCollector collector, out Element element)
        {
            element = null;
            var family = Document.OwnerFamily;
            var host = family.get_Parameter(BuiltInParameter.FAMILY_HOSTING_BEHAVIOR);
            if (host != null && host.AsInteger() > 0)
            {
                element = FirstCreatedExtrusion(collector);
            }
            return element != null;
        }

        private Extrusion FirstCreatedExtrusion(FilteredElementCollector collector)
        {
            Extrusion firstCreated = null;
            foreach (var element in collector)
            {
                if (element is Extrusion extrusion
                    && (firstCreated is null || firstCreated.Id > extrusion.Id))
                {
                    firstCreated = extrusion;
                }
            }
            return firstCreated;
        }

        private bool ContainsGenericForm(FilteredElementCollector elements)
        {
            foreach (var element in elements)
            {
                if ((element is GenericForm) == false) { continue; }

                return true;
            }
            return false;
        }

        private bool HasImageBackground(string filePath, out ViewDisplayBackground background)
        {
            background = null;
            if (ImageExportManager.LogoExist(filePath) == false) { return false; }

            var flag = ViewDisplayBackgroundImageFlags.None;
            background = ViewDisplayBackground.CreateImage(filePath, flag, UV.Zero, new UV(1, 1));
            return background != null;
        }

        private void SetOrientation()
        {
            var upDirection = new XYZ(0.408248290463863, 0.408248290463863, 0.816496580927726);
            var forwardDirection = new XYZ(0.577350269189626, 0.577350269189625, -0.577350269189626);
            View.SetOrientation(new ViewOrientation3D(View.Origin, upDirection, forwardDirection));
        }

        public static bool IsCorrectView(View view)
        {
            return view.ViewType == ViewType.ThreeD
                && view.IsTemplate == false;
        }
    }
}
