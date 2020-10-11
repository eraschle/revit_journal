﻿using DataSource;
using DataSource.Models.FileSystem;
using RevitAction;
using RevitAction.Action;
using RevitAction.Action.Parameter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RevitCommand.Families.SharedParameters
{
    public class MergeSelectParameterAction : ATaskAction, ITaskActionCommand
    {
        public SharedFileActionParameter SharedFile { get; set; }
      
        public SharedParameterActionParameter SharedParameters { get; set; }

        public ActionParameter AddIfNotExists { get; set; }

        public ActionParameter IsInstance { get; set; }

        public ActionParameterSelect ParameterGroups { get; set; }

        public ActionParameter ParameterGroup { get; set; }
     
        public ActionParameter RootDirectory { get; set; }
       
        public ITaskInfo TaskInfo { get; }

        public MergeSelectParameterAction() : base("Merge Shared [Selectable]", new Guid("2c0ddb9f-ec48-4c34-bc96-1105eb3a1637"))
        {
            TaskInfo = new TaskActionInfo<MergeSelectParameterAction>(ActionId, nameof(MergeSelectParameterCommand));
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

            RootDirectory = ActionParameter.Create("Root Directory", "Root", ParameterKind.ImageFile);
            Parameters.Add(RootDirectory);

            MakeChanges = true;
        }

        private IList<string> GetGroupNames()
        {
            return ProductDataManager.Get().ParameterGroups()
                                     .Select(grp => grp.Name)
                                     .ToList();
        }

        public override void PreTask(RevitFamilyFile family) { }


        public override void SetLibraryRoot(string libraryRoot)
        {
            RootDirectory.Value = libraryRoot;
        }
    }
}
