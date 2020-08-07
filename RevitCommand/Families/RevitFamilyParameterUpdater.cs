using Autodesk.Revit.DB;
using Fam = DataSource.Model.Family;
using Cat = DataSource.Model.Catalog;
using System;
using System.Collections.Generic;
using RevitCommand.Families.Metadata;
using DataSource.Helper;

namespace RevitCommand.Families
{
    public class RevitFamilyParameterUpdater
    {


        private readonly Document Document;
        private readonly Family Family;
        private readonly FamilyManager Manager;

        private readonly MetadataFamilyComparer FamilyComparer;
        private readonly MetadataParameterComparer ParameterComparer;

        public RevitFamilyParameterUpdater(Document document)
        {
            Document = document;
            Family = document.OwnerFamily;
            Manager = Document.FamilyManager;
            FamilyComparer = new MetadataFamilyComparer();
            ParameterComparer = new MetadataParameterComparer();
        }

        public void UpdateMetadata(Fam.Family original, Fam.Family other)
        {
            UpdateFamilyParameters(original, other);
            UpdateFamilyTypeParameters(original, other);
            if (FamilyComparer.Equals(original, other) == false) { return; }

            if (FamilyComparer.HasChangedCategory(original, other, out var metaCategory)
                && HasCategory(metaCategory, out var category))
            {
                var transactionName = $"Set Category: {metaCategory.Name}";

                using (var transaction = new Transaction(Document, transactionName))
                {
                    transaction.Start();
                    try
                    {
                        Family.FamilyCategory = category;
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.RollBack();
                    }
                }
            }

            if (FamilyComparer.HasChangedOmniClass(original, other, out var omniClass))
            {
                var transactionName = $"Set OmniClass: {omniClass.NumberAndName}";
                var bip = BuiltInParameter.OMNICLASS_CODE;
                var parameter = Family.get_Parameter(bip);
                SetParameter(parameter, omniClass.Id, transactionName);
            }
        }

        private bool HasCategory(Cat.Category metaCategory, out Category category)
        {
            category = null;
            var categoryName = metaCategory.Name;
            foreach (Category rvtCategory in Document.Settings.Categories)
            {
                if (rvtCategory.Name.Equals(categoryName, StringComparison.CurrentCulture) == false) { continue; }

                category = rvtCategory;
                break;
            }
            return category != null;
        }

        private void UpdateFamilyParameters(Fam.Family original, Fam.Family other)
        {
            var changedParams = ChangedParameters(original.Parameters, other.Parameters);
            UpdateFamilyParameters(changedParams);
        }

        private void UpdateFamilyTypeParameters(Fam.Family original, Fam.Family other)
        {
            foreach (FamilyType familyType in Manager.Types)
            {
                var typeName = familyType.Name;
                if (original.HasByName(typeName, out Fam.FamilyType metaType) == false
                    || other.HasByName(typeName, out Fam.FamilyType otherType) == false)
                {
                    continue;
                }

                SetCurrentType(familyType);
                var changedParams = ChangedParameters(metaType.Parameters, otherType.Parameters);
                UpdateFamilyTypeParameters(changedParams);
            }
        }

        private void SetCurrentType(FamilyType familyType)
        {
            using (var transaction = new Transaction(Document, "Cahnge Family Type"))
            {
                transaction.Start();
                try
                {
                    Manager.CurrentType = familyType;
                    transaction.Commit();
                }
                catch
                {
                    transaction.RollBack();
                }

            }
        }

        private void UpdateFamilyParameters(ICollection<Fam.Parameter> metaParameters)
        {
            if (metaParameters.Count == 0) { return; }

            foreach (var metaParameter in metaParameters)
            {
                var parameterName = metaParameter.Name;
                var revitParameter = Family.LookupParameter(parameterName);
                SetParameter(revitParameter, metaParameter);
            }
        }


        private void UpdateFamilyTypeParameters(ICollection<Fam.Parameter> metaParameters)
        {
            if (metaParameters.Count == 0) { return; }

            foreach (var metaParameter in metaParameters)
            {
                var parameterName = metaParameter.Name;
                var revitParameter = Manager.get_Parameter(parameterName);
                SetParameter(revitParameter, metaParameter);
            }
        }

        private void SetParameter<TParameter>(TParameter revitParameter, Fam.Parameter metaParameter) where TParameter : class
        {
            var parameterValue = metaParameter.Value;
            if (revitParameter is null || metaParameter is null
                || string.IsNullOrWhiteSpace(parameterValue)) { return; }

            var transactionName = $"Set Parameter: {metaParameter.Name} [{parameterValue}]";
            SetParameter(revitParameter, parameterValue, transactionName);
        }

        private void SetParameter<TParameter>(TParameter revitParameter, string parameterValue, string transactionName)
        {
            using (var transaction = new Transaction(Document, transactionName))
            {
                transaction.Start();
                try
                {
                    if (revitParameter is FamilyParameter familyParameter)
                    {
                        if (familyParameter.StorageType == StorageType.String)
                        {
                            Manager.Set(familyParameter, parameterValue);
                        }
                        else if (familyParameter.Definition.ParameterType == ParameterType.YesNo)
                        {
                            var boolValue = GetBooleanIntValue(parameterValue);
                            Manager.Set(familyParameter, boolValue);
                        }
                        else
                        {
                            Manager.SetValueString(familyParameter, parameterValue);
                        }
                    }
                    else if (revitParameter is Parameter parameter)
                    {
                        if (parameter.StorageType == StorageType.String)
                        {
                            parameter.Set(parameterValue);
                        }
                        else if (parameter.Definition.ParameterType == ParameterType.YesNo)
                        {
                            var boolValue = GetBooleanIntValue(parameterValue);
                            parameter.Set(parameterValue);
                        }
                        else
                        {
                            parameter.SetValueString(parameterValue);
                        }
                    }
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.RollBack();
                }
            }
        }

        private int GetBooleanIntValue(string parameterValue)
        {
            if (bool.TryParse(parameterValue, out var boolValue))
            {
                return boolValue ? 1 : 0;
            }
            return -1;
        }

        private ICollection<Fam.Parameter> ChangedParameters(IList<Fam.Parameter> original, IList<Fam.Parameter> other)
        {
            var changed = new List<Fam.Parameter>();
            foreach (var parameter in original)
            {
                if (other.Contains(parameter) == false)
                {
                    continue;
                }

                var idx = other.IndexOf(parameter);
                var changedParameter = other[idx];
                if (ParameterComparer.Equals(parameter, changedParameter)) { continue; }

                changed.Add(parameter);
            }
            return changed;
        }
    }
}
