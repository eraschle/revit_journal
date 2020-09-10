using DataSource.Model.FileSystem;
using RevitAction.Action;
using System;

namespace RevitCommand.Families.Metadata
{
    public class EditMetadataAction : ATaskActionCommand
    {
        public ActionParameter Library { get; private set; }

        public EditMetadataAction() : base("Change Metadata")
        {
            Library = ParameterBuilder.CreateJournal("Library", "Library", ParameterKind.Hidden);
            Parameters.Add(Library);
        }

        public override Guid Id
        {
            get { return new Guid("318cfd92-27ee-47a8-bb0f-840a9ff0b081"); }
        }

        public override string TaskNamespace
        {
            get { return GetType().Namespace; }
        }

        protected override string ExternalCommandName
        {
            get { return nameof(EditMetadataRevitCommand); }
        }

        public override void PreTask(RevitFamily family)
        {
            if (family is null) { return; }

            Library.Value = family.LibraryPath;
        }
    }
}
