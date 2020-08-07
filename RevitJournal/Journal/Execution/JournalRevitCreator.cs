using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace RevitJournal.Journal.Execution
{
    public delegate void JournalRevitCreatorHandler(object source, JournalRevitCreatedEventArgs e);

    public class JournalRevitCreatedEventArgs : EventArgs
    {
        public JournalRevitFile JournalRevit { get; set; }

        public string JournalProcessPath { get { return JournalRevit.JournalProcessPath; } }
    }

    public class JournalRevitCreator
    {
        private const string SearchPattern = "journal.*.txt";

        public event JournalRevitCreatorHandler JournalCreated;

        private readonly ISet<string> ExistingJournals;

        private readonly object FinishedJournalsLock = new object();
        internal readonly ISet<string> FinishedJournals;

        internal string JournalDirectory { get; private set; }

        internal bool WatchingJournals = false;

        public JournalRevitCreator(string journalDirectory)
        {
            JournalDirectory = journalDirectory;
            FinishedJournals = new HashSet<string>();
            ExistingJournals = GetJournalFiles();
        }

        private ISet<string> GetJournalFiles()
        {
            return new HashSet<string>(Directory.GetFiles(JournalDirectory, SearchPattern, SearchOption.AllDirectories));
        }

        public Task CreatorTask(TimeSpan waitingTime)
        {
            return Task.Run(() =>
            {
                WatchingJournals = true;
                while (WatchingJournals)
                {
                    Task.Delay(waitingTime).Wait();
                    if (HasNewJournals(out var newJournals) == false) { continue; }

                    foreach (var journalFile in newJournals)
                    {
                        var journalContent = JournalRevitReader.Read(journalFile);
                        var journalRevit = JournalRevitReader.Create(journalFile, journalContent);

                        if (journalRevit.HasJournalProcessPath == false) { continue; }

                        OnJournalCreated(journalRevit);
                    }
                }
            });
        }

        private bool HasNewJournals(out ISet<string> newJournals)
        {
            newJournals = GetJournalFiles();
            newJournals.ExceptWith(ExistingJournals);
            lock (FinishedJournalsLock)
            {
                newJournals.ExceptWith(FinishedJournals);
            }
            return newJournals.Count > 0;
        }

        internal void AddFinishedTask(JournalRevitFile journalRevit)
        {
            if (journalRevit is null) { return; }

            lock (FinishedJournalsLock)
            {
                FinishedJournals.Add(journalRevit.FullPath);
            }
        }

        protected void OnJournalCreated(JournalRevitFile journalRevit)
        {
            var args = new JournalRevitCreatedEventArgs
            {
                JournalRevit = journalRevit
            };
            JournalCreated?.Invoke(this, args);
        }
    }
}
