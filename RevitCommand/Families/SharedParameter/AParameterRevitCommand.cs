using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitAction.Action;
using System.IO;

namespace RevitCommand.Families.SharedParameter
{
    public abstract class AParameterRevitCommand : AFamilyRevitCommand
    {
        private readonly ActionParameter sharedFile;

        protected AParameterRevitCommand(ActionParameter sharedParameterFile)
        {
            sharedFile = sharedParameterFile;
        }

        protected override Result InternalExecute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            if (HasJournal(commandData, out var journalData) == false)
            {
                message = $"Journal data are null";
                return Result.Failed;
            }

            if (JournalKeyExist(commandData, sharedFile.JournalKey, out var journalValue) == false)
            {
#if DEBUG
                journalData.Add(sharedFile.JournalKey, @"C:\Users\rasc\OneDrive - Amstein + Walthert AG\workspace\amwa\Shared Parameter\AWH_Shared_Parameter.txt");
#else
                message = $"Journal Key NOT found: {SharedJournalKey}";
                return Result.Failed;
#endif
            }

            var filePath = journalData[sharedFile.JournalKey];
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
