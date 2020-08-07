using Autodesk.Revit.DB;
using System;

namespace RevitCommand.Repositories
{
    public class CategoryRepo
    {
        public static bool HasSubCategory(Category category, string name, out Category subCategory)
        {
            subCategory = null;
            if (category is null || HasSubCategory(category) == false) { return false; }

            var subCategories = category.SubCategories;
            if (subCategories.Contains(name))
            {
                subCategory = subCategories.get_Item(name);
            }
            return subCategory != null;
        }

        public static bool HasSubCategory(Category category)
        {
            if (category is null) { return false; }

            var subCategories = category.SubCategories;
            return subCategories != null && subCategories.IsEmpty == false;
        }

        public static Category ById(Document document, ElementId elementId)
        {
            return Category.GetCategory(document, elementId);
        }

        private readonly Document Document;
        private readonly Categories Categories;

        public CategoryRepo(Document document)
        {
            if (document is null)
            {
                throw new ArgumentNullException(nameof(document));
            }
            Document = document;
            Categories = Document.Settings.Categories;
        }

        public bool HasById(ElementId elementId)
        {
            return ById(elementId) != null;
        }

        public Category ById(ElementId elementId)
        {
            return ById(Document, elementId);
        }

        public bool HasByName(string name, out Category category)
        {
            category = null;
            if (HasByName(name))
            {
                category = Categories.get_Item(name);
            }
            return category != null;
        }

        public bool HasByName(string name)
        {
            return Categories.Contains(name);
        }
    }
}
