using RevitAction.Action;
using System;

namespace RevitCommand.Families.ImageExport
{
    public class ThreeDImageAction : ATaskActionCommand
    {
        public ActionParameter Background { get; private set; }

        public ThreeDImageAction() : base("3D Image Export")
        {
            Background = ParameterBuilder.CreateJournal("Background Image", "Background", ParameterKind.ImageFile);
            Parameters.Add(Background);
        }

        public override Guid Id
        {
            get { return new Guid("d62ba092-cf93-4906-90d8-1948d2dd67c5"); }
        }

        public override string Namespace
        {
            get { return GetType().Namespace; }
        }

        protected override string ExternalCommandName
        {
            get { return nameof(ThreeDImageRevitCommand); }
        }
    }
}
