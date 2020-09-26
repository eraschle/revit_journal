using Autodesk.Revit.DB;
using Rvt = Autodesk.Revit.DB;
using System.Collections.Generic;
using Cat = DataSource.Model.Catalog;
using System;
using DataSource.Helper;
using System.IO;
using Fam = DataSource.Model.Family;
using DataSource;
using System.Linq;
using System.Diagnostics;
using DataSource.Model.Product;
using DataSource.Model.Family;
using DataSource.Model.Catalog;
using Utilities.System;

namespace RevitCommand.Families.Metadata
{
    public class RevitMetadataManager
    {

        private readonly Document Document;
        private readonly Rvt.Family Family;
        private readonly FamilyManager Manager;
        private readonly RevitEnumCreator Creator;

        public RevitMetadataManager(Document document)
        {
            Document = document;
            Family = document.OwnerFamily;
            Manager = document.FamilyManager;
            Creator = new RevitEnumCreator();
        }

        public Fam.Family CreateFamily()
        {
            var family = MergeInformation(new Fam.Family());
            family.Category = GetCategory();
            family.OmniClass = GetOmniClass();
            family.AddCatalog(GetPlacement());
            family.AddFamilyTypes(CreateFamilyTypes());
            family.AddParameters(CreateFamilyParmeters());
            return family;
        }

        public Fam.Family MergeFamily(Fam.Family family)
        {
            family = MergeInformation(family);
            family.Category = GetCategory();
            family.OmniClass = GetOmniClass();
            family.AddCatalog(GetPlacement());
            family = MergeFamilyParameters(family);
            family = MergeFamilyTypes(family);
            return family;
        }

        public Fam.Family MergeInformation(Fam.Family metadata)
        {
            metadata.Source = Source.Revit;
            metadata.SourceUpdated = DateTime.Now;
            if (string.IsNullOrWhiteSpace(metadata.Name))
            {
                metadata.Name = GetMetatdateName();
            }

            if (string.IsNullOrWhiteSpace(metadata.DisplayName))
            {
                metadata.DisplayName = ModelHelper.FamilyDisplayName(metadata);
                var category = Family.FamilyCategory;
                if (category.IsTagCategory)
                {
                    var tagValue = category.Name.Split(Constant.SpaceChar).LastOrDefault();
                    if (string.IsNullOrWhiteSpace(tagValue) == false
                        && metadata.DisplayName.Contains(tagValue) == false)
                    {
                        metadata.DisplayName = string.Concat(tagValue, metadata.DisplayName);
                    }
                }
            }

            if (metadata.HasProduct() == false)
            {
                metadata.Product = GetProduct();
            }

            if (string.IsNullOrWhiteSpace(metadata.UniqueId))
            {
                metadata.UniqueId = GetUniqueId();
            }

            if (metadata.Updated == DateTime.MinValue)
            {
                metadata.Updated = DateTime.Now;
            }
            return metadata;
        }

        public Fam.Family MergeFamilyParameters(Fam.Family metaFamily)
        {
            foreach (Rvt.Parameter parameter in Family.Parameters)
            {
                var definition = parameter.Definition;
                if (IsOmniClassOrCategory(definition)) { continue; }

                if (metaFamily.HasByName(definition.Name, out Fam.Parameter metaParameter))
                {
                    CreateRevitParameter(parameter, metaParameter);
                }
                else
                {
                    metaParameter = CreateRevitParameter(parameter);
                    metaFamily.AddParameter(metaParameter);
                }
            }
            return metaFamily;
        }

        public ICollection<Fam.FamilyType> CreateFamilyTypes()
        {
            var familyTypes = new List<Fam.FamilyType>();
            foreach (Rvt.FamilyType familyType in Manager.Types)
            {
                SetCurrentType(familyType);
                var metaFamilyType = CreateFamilyType();
                familyTypes.Add(metaFamilyType);
            }
            return familyTypes;
        }

        public Fam.FamilyType CreateFamilyType()
        {
            var familyType = new Fam.FamilyType
            {
                Name = Manager.CurrentType.Name
            };
            foreach (var created in CreateFamilyTypeParmeters())
            {
                familyType.AddParameter(created);
            }
            return familyType;
        }

        public Fam.Family MergeFamilyTypes(Fam.Family metaFamily)
        {
            if (Manager.Types.Size <= 1)
            {
                metaFamily = MergeFamilyType(metaFamily);
            }
            else
            {
                foreach (Rvt.FamilyType familyType in Manager.Types)
                {
                    using (var transaction = new Transaction(Document, "Change Family Type"))
                    {
                        transaction.Start();
                        try
                        {
                            SetCurrentType(familyType);
                            metaFamily = MergeFamilyType(metaFamily);
                        }
                        catch { }
                        finally
                        {
                            transaction.RollBack();
                        }
                    }
                }
            }

            return metaFamily;
        }

        private void SetCurrentType(Rvt.FamilyType familyType)
        {
            Manager.CurrentType = familyType;
        }

        public Fam.Family MergeFamilyType(Fam.Family metaFamily)
        {
            var manager = Document.FamilyManager;
            var revitType = manager.CurrentType;
            if (metaFamily.HasByName(revitType.Name, out Fam.FamilyType metaType) == false)
            {
                metaType = CreateFamilyType();
                metaFamily.AddFamilyType(metaType);
            }
            foreach (FamilyParameter parameter in manager.Parameters)
            {
                if (metaType.HasByName(parameter.Definition.Name, out var metaParameter))
                {
                    CreateRevitParameter(parameter, metaParameter);
                }
                else
                {
                    metaParameter = CreateRevitParameter(parameter);
                    metaType.AddParameter(metaParameter);
                }
            }
            return metaFamily;
        }

        public IEnumerable<Fam.Parameter> CreateFamilyTypeParmeters()
        {
            var manager = Document.FamilyManager;
            var parameters = new List<Fam.Parameter>();
            foreach (FamilyParameter revit in manager.Parameters)
            {
                var metadata = CreateRevitParameter(revit);
                parameters.Add(metadata);
            }
            return parameters;
        }

        public ICollection<Fam.Parameter> CreateFamilyParmeters()
        {
            var parameters = new List<Fam.Parameter>();
            foreach (Rvt.Parameter revit in Family.Parameters)
            {
                if (IsOmniClassOrCategory(revit.Definition)) { continue; }

                var metadata = CreateRevitParameter(revit);
                parameters.Add(metadata);
            }
            return parameters;
        }

        public Fam.Parameter CreateRevitParameter(Rvt.Parameter revit, Fam.Parameter metadata = null)
        {
            if (metadata is null)
            {
                metadata = new Fam.Parameter();
            }
            metadata = SetupRevitParameterValues(metadata, revit.Definition);
            metadata.Id = GetParameterId(revit);
            metadata.ParameterType = GetParameterType(revit);
            metadata.IsReadOnly = revit.IsReadOnly;
            metadata.Unit = GetDisplayUnit(revit);
            metadata.Value = GetParameterValue(revit);
            return metadata;
        }

        public string GetParameterValue(Rvt.Parameter parameter)
        {
            string value;
            if (parameter.StorageType == StorageType.String)
            {
                value = parameter.AsString();
            }
            else if (parameter.StorageType == StorageType.ElementId)
            {
                var elementId = parameter.AsElementId();
                value = GetElementIdValue(elementId);
            }
            else if (parameter.Definition.ParameterType == ParameterType.YesNo)
            {
                var yesNo = parameter.AsInteger();
                value = yesNo == 0 ? bool.FalseString : bool.TrueString;
            }
            else
            {
                value = parameter.AsValueString();
            }
            if (value is null) { value = string.Empty; }

            return value;
        }

        public Fam.Parameter CreateRevitParameter(FamilyParameter revit, Fam.Parameter metadata = null)
        {
            if (metadata is null)
            {
                metadata = new Fam.Parameter();
            }
            metadata = SetupRevitParameterValues(metadata, revit.Definition);
            metadata.Id = GetParameterId(revit);
            metadata.ParameterType = GetParameterType(revit);
            metadata.IsInstance = revit.IsInstance;
            metadata.IsReadOnly = revit.IsReadOnly || revit.IsReporting;
            metadata.Formula = revit.Formula;
            metadata.Unit = GetDisplayUnit(revit);
            metadata.Value = GetParameterValue(revit);
            return metadata;
        }

        public string GetParameterValue(FamilyParameter parameter)
        {
            string value;
            var familyType = Document.FamilyManager.CurrentType;
            if (parameter.StorageType == StorageType.String)
            {
                value = familyType.AsString(parameter);
            }
            else if (parameter.StorageType == StorageType.ElementId)
            {
                var elementId = familyType.AsElementId(parameter);
                value = GetElementIdValue(elementId);
            }
            else if (parameter.Definition.ParameterType == ParameterType.YesNo)
            {
                var yesNo = familyType.AsInteger(parameter);
                value = yesNo == 0 ? bool.FalseString : bool.TrueString;
            }
            else
            {
                value = familyType.AsValueString(parameter);
            }
            if (value is null) { value = string.Empty; }

            return value;
        }

        private string GetElementIdValue(ElementId elementId)
        {
            if (elementId == ElementId.InvalidElementId) { return null; }

            var element = Document.GetElement(elementId);
            if (!(element is FamilySymbol symbol)) { return null; }

            return $"{symbol.Name} [{elementId}]";
        }

        private Fam.Parameter SetupRevitParameterValues(Fam.Parameter metadata, Definition definition)
        {
            metadata.Name = definition.Name;
            metadata.ValueType = GetValueType(definition);
            metadata.BuiltIn = GetBuiltIn(definition);
            return metadata;
        }

        private string GetValueType(Definition definition)
        {
            var parameterType = definition.ParameterType;
            if (parameterType == ParameterType.Invalid) { return null; }
            if (parameterType == ParameterType.FamilyType) { return Fam.Parameter.FamilyTypeValueType; }

            return LabelUtils.GetLabelFor(parameterType);
        }

        public string GetMetatdateName()
        {
            return Path.GetFileName(Document.PathName);
        }

        public RevitApp GetProduct()
        {
            var rvtApp = Document.Application;
            var productVersion = rvtApp.VersionNumber;
            if (int.TryParse(productVersion, out var version) == false)
            {
                throw new ArgumentException("Can not parse Revit Product Version to int");
            }
            return ProductManager.GetVersion(version, true);
        }

        public string GetUniqueId()
        {
            return Family.UniqueId;
        }

        public Cat.Category GetCategory()
        {
            var category = Family.FamilyCategory;

            return Creator.CreateCategory(category);
        }

        public OmniClass GetOmniClass()
        {
            var number = Family.get_Parameter(BuiltInParameter.OMNICLASS_CODE);
            var name = Family.get_Parameter(BuiltInParameter.OMNICLASS_DESCRIPTION);
            if (name is null || number is null) { return null; }

            var numberValue = number.AsString();
            var nameValue = name.AsString();
            if (string.IsNullOrWhiteSpace(numberValue)
                || string.IsNullOrWhiteSpace(nameValue)) { return null; }

            return new OmniClass
            {
                IdArray = OmniClass.GetId(numberValue),
                Name = nameValue
            };
        }

        public RevitEnum GetPlacement()
        {
            var placement = Family.FamilyPlacementType;
            return Creator.CreateFamilyPlacementType(placement);
        }

        private static bool IsOmniClassOrCategory(Definition definition)
        {
            return IsBip(definition, out var bip)
                && (bip == BuiltInParameter.OMNICLASS_CODE
                    || bip == BuiltInParameter.OMNICLASS_DESCRIPTION
                    || bip == BuiltInParameter.ELEM_CATEGORY_PARAM
                    || bip == BuiltInParameter.ELEM_CATEGORY_PARAM_MT
                    || bip == BuiltInParameter.FAMILY_CATEGORY_PSEUDO_PARAM);
        }

        private static string GetDisplayUnit(FamilyParameter parameter)
        {
            try
            {
                return GetDisplayUnitType(parameter.DisplayUnitType);
            }
            catch (Exception)
            {
                Debug.WriteLine("ERROR FamilyParameter DisplayUnitType: " + parameter.Definition.Name);
                return null;
            }
        }

        private static string GetDisplayUnit(Rvt.Parameter parameter)
        {
            try
            {
                return GetDisplayUnitType(parameter.DisplayUnitType);
            }
            catch (Exception)
            {
                Debug.WriteLine("ERROR Parameter DisplayUnitType: " + parameter.Definition.Name);
                return null;
            }
        }

        private static string GetDisplayUnitType(DisplayUnitType unitType)
        {
            if (unitType == DisplayUnitType.DUT_UNDEFINED) { return null; }

            return LabelUtils.GetLabelFor(unitType);
        }

        private static string GetParameterId(Rvt.Parameter parameter)
        {
            if (parameter.IsShared)
            {
                return parameter.GUID.ToString();
            }
            return parameter.Id.ToString();
        }

        private static string GetParameterId(FamilyParameter parameter)
        {
            if (parameter.IsShared)
            {
                return parameter.GUID.ToString();
            }
            return parameter.Id.ToString();
        }

        private static string GetBuiltIn(Definition definition)
        {
            if (IsBip(definition, out var bip))
            {
                return RevitEnumCreator.GetBuiltInEnum(bip);
            }
            return null;
        }

        private static bool IsBip(Definition definition, out BuiltInParameter builtInParameter)
        {
            builtInParameter = BuiltInParameter.INVALID;
            if (definition is InternalDefinition internalDefinition)
            {
                builtInParameter = internalDefinition.BuiltInParameter;
            }
            return builtInParameter != BuiltInParameter.INVALID;
        }

        private static bool IsBic(ElementId elementId, out BuiltInCategory builtInCategory)
        {
            builtInCategory = BuiltInCategory.INVALID;
            if (elementId == ElementId.InvalidElementId) { return false; }

            var bicId = elementId.IntegerValue;
            foreach (BuiltInCategory category in Enum.GetValues(typeof(BuiltInCategory)))
            {
                if ((int)category != bicId) { continue; }

                builtInCategory = category;
                break;
            }
            return builtInCategory != BuiltInCategory.INVALID;
        }

        private static string GetParameterType(FamilyParameter parameter)
        {
            if (parameter.IsShared)
            {
                return Fam.Parameter.SharedParameterType;
            }
            if (IsRevitParameter(parameter.Definition))
            {
                return Fam.Parameter.SystemParameterType;
            }
            return Fam.Parameter.CostumParameterType;
        }

        private static string GetParameterType(Rvt.Parameter parameter)
        {
            if (parameter.IsShared)
            {
                return Fam.Parameter.SharedParameterType;
            }
            if (IsRevitParameter(parameter.Definition))
            {
                return Fam.Parameter.SystemParameterType;
            }
            return Fam.Parameter.CostumParameterType;
        }

        private static bool IsRevitParameter(Definition definition)
        {
            return definition is InternalDefinition internalDefinition
                && internalDefinition.BuiltInParameter != BuiltInParameter.INVALID;
        }
    }
}
