using DataSource.Model.FileSystem;
using RevitAction;
using RevitAction.Action;
using System;
using System.Collections.Generic;

namespace RevitCommand.Families.SharedParameters
{
    public class MergeAllParametersAction : ATaskAction, ITaskActionCommand
    {
        public SharedFileActionParameter SharedFile { get; set; }

        public ActionParameter RootDirectory { get; set; }
     
        public ITaskInfo TaskInfo { get; }
      
        public MergeAllParametersAction() : base("Merge Shared [ALL]", new Guid("af072261-088e-42d3-bf5e-39fc99ea5736"))
        {
            TaskInfo = new TaskActionInfo<MergeAllParametersAction>(ActionId, nameof(MergeAllParametersCommand));
            SharedFile = new SharedFileActionParameter("Shared Parameter File");
            Parameters.Add(SharedFile); 
            RootDirectory = ActionParameter.Create("Root Directory", "Root", ParameterKind.ImageFile);
            Parameters.Add(RootDirectory);
            MakeChanges = true;
        }


        public override void PreTask(RevitFamily family) { }

        public override void SetLibraryRoot(string libraryRoot)
        {
            RootDirectory.Value = libraryRoot;
        }
    }
}
