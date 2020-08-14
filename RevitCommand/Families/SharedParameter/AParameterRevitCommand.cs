using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.IO;

namespace RevitCommand.Families.SharedParameter
{
    public abstract class AParameterRevitCommand<TAction> : AFamilyRevitCommand<TAction> where TAction : AParametersAction, new()
    {
        protected string SharedFileJournalKey
        {
            get { return Action.SharedFile.JournalKey; }
        }

        protected override Result InternalExecute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            if (HasJournal(commandData, out var journalData) == false)
            {
                message = $"Journal data are null";
                return Result.Failed;
            }

            if (JournalKeyExist(commandData, SharedFileJournalKey, out var journalValue) == false)
            {
#if DEBUG
                journalData.Add(SharedFileJournalKey, @"C:\Users\rasc\OneDrive - Amstein + Walthert AG\workspace\amwa\Shared Parameter\AWH_Shared_Parameter.txt");
#else
                message = $"Journal Key NOT found: {SharedJournalKey}";
                return Result.Failed;
#endif
            }

            var filePath = journalData[SharedFileJournalKey];
            if (File.Exists(filePath) == false)
            {
                message = $"Shared Parameter file does NOT exist: {filePath}";
                return Result.Failed;
            }

            return ManageSharedParameter(commandData, ref message, elements);
        }


        protected abstract Result ManageSharedParameter(ExternalCommandData commandData, ref string message, ElementSet elements);
    }
}
