﻿using DataSource.Model.Family;

namespace RevitJournal.Duplicate.Comparer.FamilyComparer
{
    public class FamilyCategoryDublicateComparer : ADuplicateComparer<Family>
    {
        private const Family Model = null;
        private const string ModelPropertyName = nameof(Model.Category);

        public FamilyCategoryDublicateComparer(bool dublicateComparer, string displayName = ModelPropertyName)
            : base(dublicateComparer, ModelPropertyName, displayName) { }

        public override string GetProperty(Family model)
        {
            if (model.HasCategory(out var category) == false) { return string.Empty; }

            return category.Name;
        }
    }
}
