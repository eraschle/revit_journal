using RevitJournal.Journal.Model;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using RevitJournal.Journal.Execution;
using DataSource.Model.FileSystem;

namespace RevitJournal.Journal
{
    public static class ResultStatus
    {
        public const int Initial = 0;
        public const int Waiting = 2;
        public const int Started = 4;
        public const int Running = 8;
        public const int Finish = 16;
        public const int Timeout = 32;
        public const int Cancel = 64;
        public const int Error = 128;

        private static IList<int> allStatus;
        public static IList<int> All
        {
            get
            {
                if (allStatus is null)
                {
                    allStatus = new List<int> { Initial, Waiting, Started, Running, Finish, Timeout, Cancel, Error };
                }
                return allStatus;
            }
        }

        public static bool IsStatus(int status)
        {
            return All.Contains(status);
        }
    }

    public class JournalResult
    {
        public static void Write(JournalResult result)
        {
            var content = JsonConvert.SerializeObject(result, Formatting.Indented);
            var resultFile = result.Original
                .ChangeExtension<JournalResultFile>(JournalResultFile.JournalResultExtension)
                .ChangeFileName<JournalResultFile>(result.Original.Name + "_Result");
            File.WriteAllText(resultFile.FullPath, content);
        }

        [JsonIgnore]
        public RevitTask JournalTask { get; private set; }

        public RevitFamilyFile Original { get { return JournalTask.Family.RevitFile; } }

        public JournalProcessFile JournalProcess { get { return JournalTask.JournalProcess; } }

        public JournalRevitFile JournalRevit { get; set; }

        public RevitFamilyFile Result { get; internal set; }

        public RevitFamilyFile Backup { get; internal set; }

        [JsonIgnore]
        public TimeSpan ProcessTimeout { get; private set; }

        [JsonIgnore]
        public int Status { get; private set; } = ResultStatus.Initial;

        public JournalResult(RevitTask journalTask)
        {
            JournalTask = journalTask;
            ProcessTimeout = journalTask.TaskOption.TaskTimeout;
            Result = journalTask.RevitFile;
        }

        private bool HasJournals { get { return HasJournalRevit && HasJournalProcess; } }

        [JsonIgnore]
        public bool HasJournalRevit { get { return JournalRevit != null; } }

        [JsonIgnore]
        public bool HasJournalProcess { get { return JournalProcess != null; } }

        [JsonIgnore]
        public bool Executed
        {
            get
            {
                return (Status & ResultStatus.Finish) == ResultStatus.Finish ||
                       (Status & ResultStatus.Timeout) == ResultStatus.Timeout ||
                       (Status & ResultStatus.Error) == ResultStatus.Error ||
                       (Status & ResultStatus.Cancel) == ResultStatus.Cancel;
            }
        }

        public void SetStatus(int status)
        {
            if (ResultStatus.All.Contains(status) == false)
            {
                throw new ArgumentException("[" + status + "] is not a valid Status");
            }
            if (status == ResultStatus.Initial)
            {
                Status = status;
            }
            else
            {
                Status |= status;
            }
        }

        public bool IsWaiting { get { return IsStatus(ResultStatus.Waiting) && IsStarted == false && Executed == false; } }

        public bool IsStarted { get { return IsStatus(ResultStatus.Started) && Executed == false; } }

        public bool IsFinished { get { return IsStatus(ResultStatus.Finish); } }

        public bool IsTimeout { get { return IsStatus(ResultStatus.Timeout); } }

        public bool IsCancel { get { return IsStatus(ResultStatus.Cancel); } }

        public bool IsError { get { return IsStatus(ResultStatus.Error); } }

        private bool IsStatus(int status) { return (Status & status) == status; }

        public bool HasError()
        {
            return IsTimeout || (HasJournals && JournalRevit.HasError(JournalProcess));
        }

        public bool HasError(out JournalError taskError)
        {
            taskError = null;
            if (HasError())
            {
                taskError = new JournalError(this);
            }
            return taskError != null;
        }

        public override bool Equals(object obj)
        {
            return obj is JournalResult result &&
                   EqualityComparer<RevitTask>.Default.Equals(JournalTask, result.JournalTask);
        }

        public override int GetHashCode()
        {
            return -174109371 + EqualityComparer<RevitTask>.Default.GetHashCode(JournalTask);
        }
    }
}
