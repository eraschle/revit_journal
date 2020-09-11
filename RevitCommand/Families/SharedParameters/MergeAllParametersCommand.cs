using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DataSource.Model.FileSystem;
using RevitCommand.Reports;
using System.IO;

namespace RevitCommand.Families.SharedParameters
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    [Journaling(JournalingMode.UsingCommandData)]
    public class MergeAllParametersCommand : AFamilyRevitCommand<MergeAllParametersAction>
    {
        public MergeAllParametersCommand() : base() { }

        protected override Result InternalExecute(ref string message, ref string errorMessage)
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
