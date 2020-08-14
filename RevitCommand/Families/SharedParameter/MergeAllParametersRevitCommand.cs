using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DataSource.Model.FileSystem;
using RevitCommand.Reports;

namespace RevitCommand.Families.SharedParameter
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class MergeAllParametersRevitCommand : AParameterRevitCommand<MergeAllParametersAction>
    {
        public MergeAllParametersRevitCommand() : base() { }

        protected override Result ManageSharedParameter(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var filePath = commandData.JournalData[SharedFileJournalKey];
            var reportManager = new RevitFamilyManagerReport(new RevitFamilyParameterManager(Document));
            var sharedManager = new SharedParameterManager(Application, filePath);
            var report = new Report(AFile.Create<RevitFamilyFile>(Document.PathName));
            foreach (var definition in sharedManager.GetSharedParameters())
            {
                var reportLine = reportManager.MergeSharedParameter(definition);
                report.AddLine(reportLine);
            }

            if (report.IsEmpty == false)
            {
                report.Write(new string[] { "merge, shared", "file" });
            }
            return Result.Succeeded;
        }
    }
}
