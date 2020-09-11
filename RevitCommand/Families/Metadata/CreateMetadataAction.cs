using RevitAction.Action;
using System;

namespace RevitCommand.Families.Metadata
{
    public class CreateMetadataAction : ATaskActionCommand<CreateMetadataAction, CreateMetadataCommand>
    {
        public ActionParameter Library { get; set; }

        public CreateMetadataAction() : base("Create metadata", new Guid("7d3a1639-4384-4488-b34c-08f29aebac2f"))
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
