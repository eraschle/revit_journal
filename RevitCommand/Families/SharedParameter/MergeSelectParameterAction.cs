using DataSource;
using DataSource.Model.FileSystem;
using RevitAction.Action;
using RevitAction.Action.Parameter;
using RevitCommand;
using RevitCommand.Families.SharedParameter;
using System;
using System.Linq;

namespace RevitJournal.Revit.Commands.Parameter
{
    public class MergeSelectParameterAction : ATaskActionCommand
    {
        public MergeSelectParameterAction() : base("Merge Shared [Selectable]")
        {
            var fileCommand = new SharedFileActionParameter("Shared File");
            var parameterCommand = new SharedParameterActionParameter("Shared Parameters",
                                                                       fileCommand.GetSharedParameters);
            var parameterGroups = ProductDataManager.Get().ParameterGroups().Select(grp => grp.Name).ToList();

            Parameters.Add(fileCommand);
            Parameters.Add(parameterCommand);
            Parameters.Add(ParameterBuilder.BoolJournal("Add if not exist", "AddIfNot", true));
            Parameters.Add(ParameterBuilder.BoolJournal("Is Instance", "IsInstance", true));
            Parameters.Add(new ActionParameterSelect("Parameter Group", parameterGroups));
            MakeChanges = true;
        }

        public override Guid AddinId
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
