using RevitAction.Action;
using System;

namespace RevitCommand.Families.Metadata
{
    public class EditMetadataAction : ATaskActionCommand<EditMetadataAction, EditMetadataCommand>
    {
        public ActionParameter Library { get; set; }

        public EditMetadataAction() : base("Change Metadata", new Guid("318cfd92-27ee-47a8-bb0f-840a9ff0b081"))
        {
            Library = ActionParameter.Text("Library Path", "Library");
            Parameters.Add(Library);
        }

        public override void SetLibraryRoot(string libraryRoot)
        {
            Library.Value = libraryRoot;
        }
    }
}
