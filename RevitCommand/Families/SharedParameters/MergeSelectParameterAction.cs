using DataSource;
using RevitAction.Action;
using RevitAction.Action.Parameter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RevitCommand.Families.SharedParameters
{
    public class MergeSelectParameterAction : ATaskActionCommand<MergeSelectParameterAction, MergeSelectParameterCommand>
    {
        public SharedFileActionParameter SharedFile { get; set; }
      
        public SharedParameterActionParameter SharedParameters { get; set; }

        public ActionParameter AddIfNotExists { get; set; }

        public ActionParameter IsInstance { get; set; }

        public ActionParameterSelect ParameterGroups { get; set; }

        public ActionParameter ParameterGroup { get; set; }

        public MergeSelectParameterAction() : base("Merge Shared [Selectable]", new Guid("2c0ddb9f-ec48-4c34-bc96-1105eb3a1637"))
        {
            SharedFile = new SharedFileActionParameter("Shared Parameter File");
            Parameters.Add(SharedFile);

            SharedParameters = new SharedParameterActionParameter("Shared Parameters", SharedFile.GetSharedParameters);
            Parameters.Add(SharedParameters);

            AddIfNotExists = ActionParameter.Bool("Add if not exists", "AddIfNot", true);
            Parameters.Add(AddIfNotExists);

            IsInstance = ActionParameter.Bool("Is instance", "IsInstance", true);
            Parameters.Add(IsInstance);

            ParameterGroups = new ActionParameterSelect("Parameter Group", "ParameterGroup", GetGroupNames());
            Parameters.Add(ParameterGroups);

            MakeChanges = true;
        }

        private IList<string> GetGroupNames()
        {
            return ProductDataManager.Get().ParameterGroups()
                                     .Select(grp => grp.Name)
                                     .ToList();
        }
    }
}
