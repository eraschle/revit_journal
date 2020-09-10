using DataSource.Model.FileSystem;
using RevitAction.Action;
using System;

namespace RevitCommand.Families.Metadata
{
    public class CreateMetadataAction : ATaskActionCommand
    {
        public ActionParameter Library { get; private set; }

        public CreateMetadataAction() : base("Create metadata")
        {
            Library = ParameterBuilder.CreateJournal("Library", "Library", ParameterKind.Hidden);
            Parameters.Add(Library);
        }

        public override Guid Id
        {
            get { return new Guid("7d3a1639-4384-4488-b34c-08f29aebac2f"); }
        }

        public override string TaskNamespace
        {
            get { return GetType().Namespace; }
        }

        protected override string ExternalCommandName
        {
            get { return nameof(CreateMetadataRevitCommand); }
        }

        public override void PreTask(RevitFamily family)
        {
            if (family is null) { return; }

            family.WriteMetaData();
            Library.Value = family.LibraryPath;
        }
    }
}
