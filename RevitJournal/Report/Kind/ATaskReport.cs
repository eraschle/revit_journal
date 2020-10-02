using System;
using System.Collections.Generic;
using DataSource.Model.FileSystem;
using RevitJournal.Revit.Journal;
using RevitJournal.Tasks;

namespace RevitJournal.Report
{
    public abstract class ATaskReport : IEquatable<ATaskReport>
    {
        public RevitFamilyFile SourceFile { get; private set; }

        public RevitFamilyFile ResultFile { get; private set; }

        public RevitFamilyFile BackupFile { get; private set; }

        public TaskJournalFile TaskJournal { get; private set; }

        public RecordJournalFile OriginalRecordJournal { get; set; } = null;

        public RecordJournalFile CopiedRecordJournal { get; set; } = null;

        public List<string> WarningReport { get; } = new List<string>();

        protected ATaskReport(RevitTask task)
        {
            if(task is null) { throw new ArgumentNullException(nameof(task)); }
            SourceFile = task.SourceFile;
            ResultFile = task.ResultFile;
            BackupFile = task.BackupFile;
            TaskJournal = task.TaskJournal;
            OriginalRecordJournal = task.RecordeJournal;
        }


        public override bool Equals(object obj)
        {
            return Equals(obj as ATaskReport);
        }

        public bool Equals(ATaskReport other)
        {
            return other != null &&
                   EqualityComparer<RevitFamilyFile>.Default.Equals(SourceFile, other.SourceFile);
        }

        public override int GetHashCode()
        {
            return 1355363486 + EqualityComparer<RevitFamilyFile>.Default.GetHashCode(SourceFile);
        }

        public static bool operator ==(ATaskReport left, ATaskReport right)
        {
            return EqualityComparer<ATaskReport>.Default.Equals(left, right);
        }

        public static bool operator !=(ATaskReport left, ATaskReport right)
        {
            return !(left == right);
        }
    }
}
