using DataSource;
using RevitAction.Action;
using RevitAction.Action.Parameter;
using RevitJournal.Revit.Commands.Parameter;
using System;
using System.Linq;

namespace RevitCommand.Families.SharedParameter
{
    public class MergeSelectParameterAction : AParametersAction
    {
        public SharedParameterActionParameter SharedParameters { get; private set; }

        public ActionParameter AddIfNotExists { get; private set; }

        public ActionParameter IsInstance { get; private set; }

        public ActionParameterSelect ParameterGroups { get; private set; }

        public MergeSelectParameterAction() : base("Merge Shared [Selectable]")
        {
            SharedParameters = new SharedParameterActionParameter("Shared Parameters", SharedFile.GetSharedParameters);
            Parameters.Add(SharedParameters);

            AddIfNotExists = ParameterBuilder.BoolJournal("Add if not exist", "AddIfNot", true);
            Parameters.Add(AddIfNotExists);
            
            IsInstance = ParameterBuilder.BoolJournal("Is Instance", "IsInstance", true);
            Parameters.Add(IsInstance);

            var parameterGroups = ProductDataManager.Get().ParameterGroups().Select(grp => grp.Name).ToList();
            ParameterGroups = new ActionParameterSelect("Parameter Group", parameterGroups);
            Parameters.Add(ParameterGroups);
            
            MakeChanges = true;
        }

        public override Guid Id
        {
            get { return new Guid("2c0ddb9f-ec48-4c34-bc96-1105eb3a1637"); }
        }

        public override string Namespace
        {
            get { return GetType().Namespace; }
        }

        protected override string ExternalCommandName
        {
            get { return nameof(MergeSelectParametersRevitCommand); }
        }
    }
}
