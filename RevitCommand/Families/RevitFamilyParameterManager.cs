using Autodesk.Revit.DB;
using System;

namespace RevitCommand.Families
{
    public class RevitFamilyParameterManager
    {
        private const string TmpParameterPrefix = "tmp";

        private readonly Document Document;
        private readonly FamilyManager FamilyManager;

        public RevitFamilyParameterManager(Document document)
        {
            Document = document;
            FamilyManager = Document.FamilyManager;
        }

        public bool CanMergeSharedParameter(ExternalDefinition definition, out FamilyParameter parameter)
        {
            parameter = null;
            return definition != null
                && HasParameterByName(definition.Name, out parameter)
                && parameter.IsShared
                && SameDefintion(parameter, definition)
                && parameter.GUID.Equals(definition.GUID) == false;
        }

        public FamilyParameter MergeSharedParameter(ExternalDefinition definition)
        {
            if (CanMergeSharedParameter(definition, out var parameter) == false)
            {
                return null;
            }

            var tmpParameterName = string.Concat(TmpParameterPrefix, definition.Name);
            using (var group = new TransactionGroup(Document, "Merge Shared Parameter"))
            {
                try
                {
                    group.Start();
                    var changed = ReplaceSharedWithFamilyParameter(parameter, tmpParameterName);
                    var merged = ReplaceFamilyWithSharedParameter(changed, definition);
                    group.Commit();
                    return merged;
                }
                catch (Exception)
                {
                    group.RollBack();
                    throw;
                }
            }
        }

        public FamilyParameter AddSharedInstance(ExternalDefinition definition, BuiltInParameterGroup group)
        {
            return AddShared(definition, group, true);
        }

        public FamilyParameter AddSharedType(ExternalDefinition definition, BuiltInParameterGroup group)
        {
            return AddShared(definition, group, false);
        }

        private FamilyParameter AddShared(ExternalDefinition definition, BuiltInParameterGroup group, bool isInstance)
        {
            using (var transaction = new Transaction(Document, "Add Shared Parameter"))
            {
                try
                {
                    transaction.Start();
                    var addedParameter = FamilyManager.AddParameter(definition, group, isInstance);
                    transaction.Commit();
                    return addedParameter;
                }
                catch (Exception)
                {
                    transaction.RollBack();
                    throw;
                }
            }
        }

        public static bool IsBuiltInParameter(FamilyParameter parameter)
        {
            if (!(parameter.Definition is InternalDefinition definition)) { return false; }

            return definition.BuiltInParameter != BuiltInParameter.INVALID;
        }

        private bool SameDefintion(FamilyParameter parameter, ExternalDefinition definition)
        {
            var paramDefinition = parameter.Definition;
            return paramDefinition.ParameterType == definition.ParameterType
                && paramDefinition.UnitType == definition.UnitType;
        }

        public FamilyParameter RenameParameter(FamilyParameter parameter, string newName)
        {
            using (var transaction = new Transaction(Document, "Rename Family parameter"))
            {
                try
                {
                    transaction.Start();
                    FamilyManager.RenameParameter(parameter, newName);
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.RollBack();
                }
            }
            return GetParameterByName(newName);
        }

        public FamilyParameter ReplaceSharedWithFamilyParameter(FamilyParameter parameter, string newName)
        {
            FamilyParameter replaced = null;
            using (var transaction = new Transaction(Document, "Shared to Family parameter"))
            {
                try
                {
                    transaction.Start();
                    var isInstance = parameter.IsInstance;
                    var paramGroup = parameter.Definition.ParameterGroup;
                    replaced = FamilyManager.ReplaceParameter(parameter, newName, paramGroup, isInstance);
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.RollBack();
                    throw;
                }
            }
            return replaced;
        }

        public FamilyParameter ReplaceFamilyWithSharedParameter(FamilyParameter parameter, ExternalDefinition definition)
        {
            FamilyParameter replaced = null;
            using (var transaction = new Transaction(Document, "Family to Shared parameter"))
            {
                transaction.Start();
                try
                {
                    var isInstance = parameter.IsInstance;
                    var paramGroup = parameter.Definition.ParameterGroup;
                    replaced = FamilyManager.ReplaceParameter(parameter, definition, paramGroup, isInstance);
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.RollBack();
                    throw;
                }
            }
            return replaced;
        }

        public bool HasParameterByName(string name)
        {
            return GetParameterByName(name) != null;
        }

        public bool HasParameterByName(string name, out FamilyParameter parameter)
        {
            parameter = GetParameterByName(name);
            return parameter != null;
        }

        public FamilyParameter GetParameterByName(string name)
        {
            return FamilyManager.get_Parameter(name);
        }
    }
}
