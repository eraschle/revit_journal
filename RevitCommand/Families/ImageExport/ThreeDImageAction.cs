using RevitAction.Action;
using System;

namespace RevitCommand.Families.ImageExport
{
    public class ThreeDImageAction : ATaskActionCommand<ThreeDImageAction, ThreeDImageRevitCommand>
    {
        public ActionParameter Background { get; set; }

        public ThreeDImageAction() : base("3D Image Export", new Guid("d62ba092-cf93-4906-90d8-1948d2dd67c5"))
        {
            Background = ActionParameter.Create("Background Image", "Background", ParameterKind.ImageFile);
            Parameters.Add(Background);
        }
    }
}
