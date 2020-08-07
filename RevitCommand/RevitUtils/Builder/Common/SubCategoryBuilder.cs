using Autodesk.Revit.DB;
using RevitCommand.Repositories;
using System;
using System.Collections.Generic;

namespace RevitCommand.RevitUtils.Builder
{
    public class SubCategoryBuilder : AModelBuilder<Category>
    {
        private readonly CategoryRepo Repo;

        private readonly Document SourceDoc;

        public SubCategoryBuilder(Document document, Document source) : base(document)
        {
            Repo = new CategoryRepo(Document);
            SourceDoc = source;
        }

        protected override Category CreateElement(Category source)
        {
            if (source is null || source.Parent is null
                || CategoryRepo.HasSubCategory(source.Parent) == false) { return null; }

            var parent = Repo.ById(source.Parent.Id);
            if (CategoryRepo.HasSubCategory(parent, source.Name, out var subCategory) == false)
            {
                var categories = Document.Settings.Categories;
                subCategory = categories.NewSubcategory(parent, source.Name);
            }
            return subCategory;
        }

        protected override void MergeElement(Category source, ref Category created)
        {
            if (created is null || source is null) { return; }

            SetLineColor(source, created);
            foreach (GraphicsStyleType style in Enum.GetValues(typeof(GraphicsStyleType)))
            {
                SetLineWeight(source, created, style);
                SetLinePattern(source, created, style);
            }
        }

        private void SetLineColor(Category source, Category created)
        {
            if (source.LineColor is null) { return; }

            created.LineColor = source.LineColor;
        }

        private void SetLineWeight(Category source, Category created, GraphicsStyleType styleType)
        {
            var weight = source.GetLineWeight(styleType);
            if (weight is null) { return; }

            created.SetLineWeight((int)weight, styleType);
        }

        private void SetLinePattern(Category source, Category created, GraphicsStyleType styleType)
        {
            var patternId = source.GetLinePatternId(styleType);
            var linePattern = ElementRepo.ById<LinePatternElement>(SourceDoc, patternId);
            if (linePattern != null && HasPatternElement(linePattern, out var patternElement))
            {
                created.SetLinePatternId(patternElement.Id, styleType);
            }
        }

        private bool HasPatternElement(LinePatternElement linePattern, out LinePatternElement patternElement)
        {
            var patternName = linePattern.Name;
            if (ElementRepos.HasByName(patternName, out patternElement) == false)
            {
                using (var pattern = new LinePattern(patternName))
                {
                    patternElement = LinePatternElement.Create(Document, pattern);
                }
            }
            return patternElement != null;
        }

        protected override ElementId GetId(Category source)
        {
            if (source is null) { return null; }

            return source.Id;
        }

        protected override Category GetElement(ElementId elementId)
        {
            return Category.GetCategory(Document, elementId);
        }

        public override bool Equals(Category element, Category other)
        {
            return element != null && other != null
                && element.Id == other.Id;
        }

        public override int GetHashCode(Category obj)
        {
            int hashCode = -905324395;
            if (obj != null)
            {
                hashCode = hashCode * -1521134295 + EqualityComparer<ElementId>.Default.GetHashCode(obj.Id);
            }
            return hashCode;
        }

        protected override void AddCreatedElements(Category source, Category created) { }
    }
}
