﻿using DataSource.Model.Family;
using RevitJournal.Duplicate.Comparer;
using System;
using Utilities.UI;

namespace RevitJournalUI.MetadataUI.Models
{
    public class DuplicateFamilyViewModel : ANotifyPropertyChangedModel
    {
        public Family Family { get; private set; }

        public DuplicateFamilyViewModel(Family family, Family original, FamilyDuplicateComparer familyComparer)
        {
            if(family is null) { throw new ArgumentNullException(nameof(family)); }
            if(original is null) { throw new ArgumentNullException(nameof(original)); }
            if(familyComparer is null) { throw new ArgumentNullException(nameof(familyComparer)); }

            Family = family;
            DisplayeNameDistance = familyComparer.ByName(nameof(Family.Name)).LevenstheinDistanceAsString(original, Family);
        }

        public string DisplayName { get { return Family.Name; } }

        public string DisplayeNameDistance { get; private set; }
    }
}
