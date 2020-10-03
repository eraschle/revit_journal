using DataSource;
using DataSource.Model.FileSystem;
using DataSource.Model.Product;
using RevitAction.Action;
using RevitJournal.Revit;
using RevitJournal.Revit.Journal;
using RevitJournal.Tasks.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Utilities.System;

namespace RevitJournal.Tasks
{
    public class RevitTask : IEquatable<RevitTask>
    {
        private static readonly string[] timeFormat = new string[] 
        { 
            DateUtils.Hour, 
            DateUtils.Minute, 
            DateUtils.Seconds, 
            DateUtils.Milliseconds 
        };

        private static readonly RecordJournalNullFile nullRecorde = new RecordJournalNullFile();
        private static readonly TaskJournalNullFile nullTask = new TaskJournalNullFile();

        public RevitFamily Family { get; private set; }

        public RevitFamilyFile SourceFile
        {
            get { return Family.RevitFile; }
        }

        public RevitFamilyFile ResultFile { get; set; } = null;

        public bool HasResultFile
        {
            get { return ResultFile != null; }
        }

        public RevitFamilyFile BackupFile { get; set; } = null;

        public bool HasBackupFile
        {
            get { return BackupFile != null; }
        }

        public TaskJournalFile TaskJournal { get; set; } = nullTask;

        public RecordJournalFile RecordeJournal { get; set; } = nullRecorde;

        public List<ITaskAction> Actions { get; } = new List<ITaskAction>();

        public RevitTask(RevitFamily family)
        {
            if (family is null) { throw new ArgumentNullException(nameof(family)); }

            Family = family;
        }

        public string Name
        {
            get { return Family.RevitFile.NameWithoutExtension; }
        }

        public void ClearActions()
        {
            Actions.Clear();
        }

        public void AddActions(IEnumerable<ITaskAction> actions)
        {
            Actions.Clear();
            Actions.AddRange(actions);
        }

        public bool HasActionById(Guid actionId, out ITaskAction action)
        {
            action = Actions.FirstOrDefault(act => act.ActionId == actionId);
            return action != null;
        }

        public int GetExecutedActions(ITaskAction current)
        {
            var count = 0;
            foreach (var action in Actions)
            {
                count += 1;
                if (action.Equals(current)) { break; }
            }
            return count;
        }

        public bool HasCommands(out ICollection<ITaskActionCommand> actionCommands)
        {
            actionCommands = new List<ITaskActionCommand>();
            foreach (var command in Actions)
            {
                if (!(command is ITaskActionCommand actionCommand)) { continue; }

                actionCommands.Add(actionCommand);
            }
            return actionCommands.Count > 0;
        }

        public void PreExecution(TaskOptions options)
        {
            if (options is null) { throw new ArgumentNullException(nameof(options)); }

            if (options.CreateBackup)
            {
                var backupFile = options.CreateBackupFile(SourceFile);
                BackupFile = SourceFile.CopyTo(backupFile, true);
            }

            foreach (var command in Actions)
            {
                command.PreTask(Family);
            }
        }

        public void DeleteBackups(TaskOptions options)
        {
            if (options is null || options.DeleteRevitBackup == false) { return; }

            SourceFile.DeleteBackups();
            if (HasResultFile)
            {
                ResultFile.DeleteBackups();
            }

            if (HasBackupFile)
            {
                BackupFile.DeleteBackups();
            }
        }

        public void CreateTaskJournal(TaskOptions options)
        {
            if (options is null) { throw new ArgumentNullException(nameof(options)); }

            var workingDirectory = options.GetJournalWorking();
            TaskJournal = SourceFile.ChangeDirectory<TaskJournalFile>(workingDirectory);
            TaskJournal.SetFileName(SourceFile);
            var fileSuffix = DateUtils.GetPathDate(format: timeFormat);
            TaskJournal.AddSuffixes(fileSuffix);
        }

        public void SetDefaultTaskJournal()
        {
            TaskJournal = nullTask;
        }

        public bool HasRecordJournal()
        {
            return RecordeJournal is object && RecordeJournal.Exists();
        }

        public bool DoesRecordCopyExists()
        {
            var renamed = GetRenamedJournalFile();
            return renamed is object && renamed.Exists();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public async void CopyRecordJournal()
        {
            await Task.Run(() =>
            {
                while (HasRecordJournal() && DoesRecordCopyExists() == false)
                {
                    var renamed = GetRenamedJournalFile();
                    try
                    {
                        RecordeJournal.CopyTo(renamed, overrideFile: true);
                        DebugUtils.Line<TaskUnitOfWork>($"Copy Name: {renamed.Name} [{renamed.FullPath}]");
                    }
                    catch (Exception ex)
                    {
                        DebugUtils.Exception<TaskUnitOfWork>(ex, $"Wait: {renamed.Name}");
                        Task.Delay(TimeSpan.FromSeconds(1));
                    }
                }
            }).ConfigureAwait(false);
        }

        public RecordJournalFile GetRenamedJournalFile()
        {
            if (HasRecordJournal() == false
                || SourceFile is null
                || SourceFile.HasParent(out var directory) == false) { return null; }

            var newFileName = $"{Name}_{RecordeJournal.NameWithoutExtension}";
            return RecordeJournal.ChangeFileName<RecordJournalFile>(newFileName)
                                 .ChangeDirectory<RecordJournalFile>(directory);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as RevitTask);
        }

        public bool Equals(RevitTask other)
        {
            return other != null &&
                   EqualityComparer<RevitFamily>.Default.Equals(Family, other.Family);
        }

        public override int GetHashCode()
        {
            return 548286385 + EqualityComparer<RevitFamily>.Default.GetHashCode(Family);
        }

        public static bool operator ==(RevitTask left, RevitTask right)
        {
            return EqualityComparer<RevitTask>.Default.Equals(left, right);
        }

        public static bool operator !=(RevitTask left, RevitTask right)
        {
            return !(left == right);
        }
    }
}
