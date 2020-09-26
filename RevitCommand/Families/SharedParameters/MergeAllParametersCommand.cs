using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DataSource.Model.FileSystem;
using RevitAction.Revit;
using RevitCommand.Reports;
using System.IO;

namespace RevitCommand.Families.SharedParameters
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class MergeAllParametersCommand : ARevitActionCommand<MergeAllParametersAction>
    {
        public MergeAllParametersCommand() : base() { }

        protected override Result ExecuteRevitCommand(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var filePath = Action.SharedFile.Value;
            if (File.Exists(filePath) == false)
            {
#if DEBUG
                Action.SharedFile.Value = @"C:\Users\rasc\OneDrive - Amstein + Walthert AG\workspace\amwa\Shared Parameter\AWH_Shared_Parameter.txt";
#else
                errorMessage = $"Shared Parameter file does NOT exist: {filePath}";
                return Result.Failed;
#endif
            }
            var reportManager = new RevitFamilyManagerReport(new RevitFamilyParameterManager(Document));
            var sharedManager = new SharedParameterManager(Application, filePath);

            var root = PathFactory.Instance.CreateRoot(Action.SharedFile.Value);
            var reportFile = PathFactory.Instance.Create<RevitFamilyFile>(Document.PathName);
            reportFile.AddSuffixes("merge, shared", "file");

            var report = new Report(reportFile);
            foreach (var definition in sharedManager.GetSharedParameters())
            {
                var reportLine = reportManager.MergeSharedParameter(definition);
                report.AddLine(reportLine);
            }

            if (report.IsEmpty == false)
            {
                report.Write();
            }
            return Result.Succeeded;
        }
    }
}
