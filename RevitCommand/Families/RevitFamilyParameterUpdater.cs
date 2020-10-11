using Revit = Autodesk.Revit.DB;
using Model = DataSource.Model.Metadata;
using Cat = DataSource.Models.Catalog;
using System;
using System.Collections.Generic;
using RevitCommand.Families.Metadata;
using System.Diagnostics.CodeAnalysis;
using DataSource.Model.Metadata;

namespace RevitCommand.Families
{
    public class RevitFamilyParameterUpdater
    {


        private readonly Revit.Document Document;
        private readonly Revit.Family Family;
        private readonly Revit.FamilyManager Manager;

        private readonly MetadataFamilyComparer FamilyComparer;
        private readonly MetadataParameterComparer ParameterComparer;

        public RevitFamilyParameterUpdater(Revit.Document document)
        {
            Document = document;
            Family = document.OwnerFamily;
            Manager = Document.FamilyManager;
            FamilyComparer = new MetadataFamilyComparer();
            ParameterComparer = new MetadataParameterComparer();
        }

        [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public void UpdateMetadata(Family original, Family other)
        {
            UpdateFamilyParameters(original, other);
            UpdateFamilyTypeParameters(original, other);
            if (FamilyComparer.Equals(original, other) == false) { return; }

            if (FamilyComparer.HasChangedCategory(original, other, out var metaCategory)
                && HasCategory(metaCategory, out var category))
            {
                var transactionName = $"Set Category: {metaCategory.Name}";

                using (var transaction = new Revit.Transaction(Document, transactionName))
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
                var bip = Revit.BuiltInParameter.OMNICLASS_CODE;
                var parameter = Family.get_Parameter(bip);
                SetParameter(parameter, omniClass.Id, transactionName);
            }
        }

        private bool HasCategory(Cat.Category metaCategory, out Revit.Category category)
        {
            category = null;
            var categoryName = metaCategory.Name;
            foreach (Revit.Category rvtCategory in Document.Settings.Categories)
            {
                if (rvtCategory.Name.Equals(categoryName, StringComparison.CurrentCulture) == false) { continue; }

                category = rvtCategory;
                break;
            }
            return category != null;
        }

        private void UpdateFamilyParameters(Family original, Family other)
        {
            var changedParams = ChangedParameters(original.Parameters, other.Parameters);
            UpdateFamilyParameters(changedParams);
        }

        private void UpdateFamilyTypeParameters(Family original, Family other)
        {
            foreach (Revit.FamilyType familyType in Manager.Types)
            {
                var typeName = familyType.Name;
                if (original.HasByName(typeName, out FamilyType metaType) == false
                    || other.HasByName(typeName, out FamilyType otherType) == false)
                {
                    continue;
                }

                SetCurrentType(familyType);
                var changedParams = ChangedParameters(metaType.Parameters, otherType.Parameters);
                UpdateFamilyTypeParameters(changedParams);
            }
        }

        [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private void SetCurrentType(Revit.FamilyType familyType)
        {
            using (var transaction = new Revit.Transaction(Document, "Cahnge Family Type"))
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

        private void UpdateFamilyParameters(ICollection<Parameter> metaParameters)
        {
            if (metaParameters.Count == 0) { return; }

            foreach (var metaParameter in metaParameters)
            {
                var parameterName = metaParameter.Name;
                var revitParameter = Family.LookupParameter(parameterName);
                SetParameter(revitParameter, metaParameter);
            }
        }


        private void UpdateFamilyTypeParameters(ICollection<Parameter> metaParameters)
        {
            if (metaParameters.Count == 0) { return; }

            foreach (var metaParameter in metaParameters)
            {
                var parameterName = metaParameter.Name;
                var revitParameter = Manager.get_Parameter(parameterName);
                SetParameter(revitParameter, metaParameter);
            }
        }

        private void SetParameter<TParameter>(TParameter revitParameter, Parameter metaParameter) where TParameter : class
        {
            var parameterValue = metaParameter.Value;
            if (revitParameter is null || metaParameter is null
                || string.IsNullOrWhiteSpace(parameterValue)) { return; }

            var transactionName = $"Set Parameter: {metaParameter.Name} [{parameterValue}]";
            SetParameter(revitParameter, parameterValue, transactionName);
        }

        [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private void SetParameter<TParameter>(TParameter revitParameter, string parameterValue, string transactionName)
        {
            using (var transaction = new Revit.Transaction(Document, transactionName))
            {
                transaction.Start();
                try
                {
                    if (revitParameter is Revit.FamilyParameter familyParameter)
                    {
                        if (familyParameter.StorageType == Revit.StorageType.String)
                        {
                            Manager.Set(familyParameter, parameterValue);
                        }
                        else if (familyParameter.Definition.ParameterType == Revit.ParameterType.YesNo)
                        {
                            var boolValue = GetBooleanIntValue(parameterValue);
                            Manager.Set(familyParameter, boolValue);
                        }
                        else
                        {
                            Manager.SetValueString(familyParameter, parameterValue);
                        }
                    }
                    else if (revitParameter is Revit.Parameter parameter)
                    {
                        if (parameter.StorageType == Revit.StorageType.String)
                        {
                            parameter.Set(parameterValue);
                        }
                        else if (parameter.Definition.ParameterType == Revit.ParameterType.YesNo)
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

        private ICollection<Parameter> ChangedParameters(IList<Parameter> original, IList<Parameter> other)
        {
            var changed = new List<Parameter>();
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
