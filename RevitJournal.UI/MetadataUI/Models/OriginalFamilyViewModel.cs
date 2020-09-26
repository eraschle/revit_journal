using DataSource.Model.Family;
using RevitJournal.Duplicate.Comparer;
using System;
using System.Collections.Generic;
using System.Linq;
using Utilities.System;
using Utilities.UI;

namespace RevitJournalUI.MetadataUI.Models
{
    public class OriginalFamilyViewModel : ANotifyPropertyChangedModel
    {
        public Family Family { get; private set; }

        public OriginalFamilyViewModel(Family family, HashSet<Family> dublicates, FamilyDuplicateComparer comparer)
        {
            if (family is null) { throw new ArgumentNullException(nameof(family)); }
            if (dublicates is null) { throw new ArgumentNullException(nameof(dublicates)); }

            Family = family;
            DuplicateModels = dublicates
                .OrderBy(item => comparer.Compare(family, item))
                .Select(item => new DuplicateFamilyViewModel(item, family, comparer));
            DublicateCount = dublicates.Count;
        }

        public string ModelName { get { return Family.Name; } }

        public int DublicateCount { get; private set; }

        public IEnumerable<DuplicateFamilyViewModel> DuplicateModels { get; private set; }

        public override bool Equals(object obj)
        {
            return obj is OriginalFamilyViewModel model
                && StringUtils.Equals(ModelName, model.ModelName);
        }

        public override int GetHashCode()
        {
            return -1698245239 + ModelName.GetHashCode();
        }
    }
}
