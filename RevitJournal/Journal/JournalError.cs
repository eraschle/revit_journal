using Newtonsoft.Json;
using RevitJournal.Journal.Model;
using System;
using System.IO;

namespace RevitJournal.Journal
{
    public class JournalError
    {
        public static void Write(JournalError taskError)
        {
            var content = JsonConvert.SerializeObject(taskError, Formatting.Indented);
            File.WriteAllText(taskError.ErrorFilePath, content);
        }

        public JournalResult Result { get; internal set; }

        [JsonIgnore]
        public string ErrorFilePath
        {
            get
            {
                return Result.Original
                    .ChangeExtension<JournalResultFile>(JournalResultFile.JournalResultExtension)
                    .ChangeFileName<JournalResultFile>(Result.Original.Name + "_Error")
                    .FullPath;
            }
        }

        public string ErrorMessage { get; private set; } = string.Empty;

        internal JournalError(JournalResult result)
        {
            Result = result;
            Init();
        }

        private void Init()
        {
            if (Result.HasJournalRevit)
            {
                if (Result.JournalRevit.HasBeenStarted() == false)
                {
                    ErrorMessage = "Revit process has NOT started" + Environment.NewLine;
                }
                else if (Result.JournalRevit.AllCommandExecuted(Result.JournalProcess) == false)
                {
                    var lastLine = Result.JournalRevit.LastCommandLineNumber;
                    var command = Result.JournalProcess.ByLineNumber(lastLine);
                    if (command != null)
                    {
                        ErrorMessage = "Command at line: " + lastLine + Environment.NewLine;
                        ErrorMessage += "with Error: " + command;
                    }
                }
                else if (Result.JournalRevit.HasBeenFinished() == false)
                {
                    ErrorMessage = "Journal Protocol has not been finished" + Environment.NewLine;
                }
            }
            else
            {
                ErrorMessage = "No Journal of Revit process found" + Environment.NewLine;
            }
        }
    }
}
