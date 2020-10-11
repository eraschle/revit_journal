using System.Collections.Generic;
using DataSource.Models.FileSystem;
using RevitAction.Report;
using RevitJournal.Revit.Journal;
using RevitAction.Action;
using System;
using RevitAction.Report.Message;
using RevitJournal.Tasks;
using RevitJournal.Tasks.Options;
using DataSource.DataSource.Json;
using System.Management;
using Newtonsoft.Json.Schema;
using System.Linq;
using System.Diagnostics;

namespace RevitJournal.Report
{
    public class TaskReportManager
    {
        public IDictionary<ITaskAction, TaskActionReport> ActionReports { get; }
            = new Dictionary<ITaskAction, TaskActionReport>();

        public ITaskAction ErrorAction { get; private set; } = null;

        public string ErrorMessage { get; private set; } = null;

        public bool HasErrorAction
        {
            get { return ErrorAction != null; }
        }

        public bool HasErrorReport(TaskOptions options)
        {
            return options is object && ErrorAction is object
                && options.LogResults.Value && options.LogError.Value;
        }

        public void CreateErrorReport(RevitTask task, TaskOptions options, params string[] suffixes)
        {
            if (task is null || HasErrorReport(options) == false) { return; }

            var errorReport = new ReportError(task)
            {
                ErrorReport = ErrorAction,
                ErrorMessage = ErrorMessage
            };
            errorReport.WarningReport.AddRange(GetWarnings(task));
            CreateReport(errorReport, task, suffixes);
        }

        public bool HasSuccessReport(RevitTask task, TaskOptions options)
        {
            return options is object && task is object
                && options.LogResults.Value && options.LogSuccess.Value
                && HasSuccessReports(task);
        }

        public void CreateSuccessReport(RevitTask task, TaskOptions options, params string[] suffixes)
        {
            if (HasSuccessReport(task, options) == false) { return; }

            var successReport = new ReportSuccess(task);
            successReport.SuccessReport.AddRange(GetSuccess(task));
            successReport.WarningReport.AddRange(GetWarnings(task));
            CreateReport(successReport, task, suffixes);
        }

        private void CreateReport<TReport>(TReport report, RevitTask task, params string[] suffixes) where TReport : ATaskReport
        {
            if (task is null) { throw new ArgumentNullException(nameof(task)); }

            report.CopiedRecordJournal = task.GetRenamedJournalFile();
            var jsonFile = report.SourceFile.ChangeExtension<JsonFile>();
            jsonFile.AddSuffixes(suffixes);
            var dataSource = new JsonDataSource<TReport>();
            dataSource.SetFile(jsonFile);
            dataSource.Write(report);
        }

        private IList<string> GetWarnings(RevitTask task)
        {
            if (task is null) { throw new ArgumentNullException(nameof(task)); }

            var warnings = new List<string>();
            foreach (var action in task.Actions)
            {
                if (ActionReports.ContainsKey(action) == false) { continue; }

                if (ActionReports[action].HasWarningReports())
                {
                    warnings.AddRange(GetWarnings(action));
                }
            }
            return warnings;
        }

        private IList<string> GetSuccess(RevitTask task)
        {
            if (task is null) { throw new ArgumentNullException(nameof(task)); }

            var success = new List<string>();
            foreach (var action in task.Actions)
            {
                if (ActionReports.ContainsKey(action) == false) { continue; }

                if (ActionReports[action].HasStatusReports())
                {
                    success.AddRange(GetStatus(action));
                }
            }
            return success;
        }

        private bool HasSuccessReports(RevitTask task)
        {
            if (task is null) { throw new ArgumentNullException(nameof(task)); }

            var success = true;
            var it = task.Actions.GetEnumerator();
            while (it.MoveNext() && success)
            {
                var action = it.Current;
                if (ActionReports.ContainsKey(action) == false) { continue; }

                success &= ActionReports[action].HasStatusReports();
            }
            return success;
        }

        private IEnumerable<string> GetWarnings(ITaskAction action)
        {
            return GetMessages(action, ActionReports[action].WarningReports(), "WARNING");
        }

        private IEnumerable<string> GetStatus(ITaskAction action)
        {
            return GetMessages(action, ActionReports[action].StatusReports(), "STATUS");
        }

        public void AddReport(RevitTask task, ReportMessage report)
        {
            if (task is null || report is null) { return; }

            switch (report.Kind)
            {
                case ReportKind.Message:
                case ReportKind.Warning:
                    if (HasActionReport(task, report.ActionId, out var actionReport))
                    {
                        actionReport.Add(report);
                    }
                    break;
                case ReportKind.Error:
                    if (HasErrorAction == false
                        && task.HasActionById(report.ActionId, out var errorAction))
                    {
                        ErrorAction = errorAction;
                        var message = report.Message;
                        if (report.Exception is object)
                        {
                            message += Environment.NewLine + "Exception Message: " + report.Exception.Message;
                            message += Environment.NewLine + "Exception StackTrace: " + report.Exception.StackTrace;
                        }
                        ErrorMessage = message;
                    }
                    break;
                default:
                    break;
            }
        }

        private bool HasActionReport(RevitTask task, Guid actionId, out TaskActionReport actionReport)
        {
            actionReport = null;
            if (task is object && task.HasActionById(actionId, out var action))
            {
                if (ActionReports.ContainsKey(action) == false)
                {
                    ActionReports.Add(action, new TaskActionReport { TaskAction = action });
                }
                actionReport = ActionReports[action];
            }
            return actionReport != null;
        }

        private IEnumerable<string> GetMessages(ITaskAction action, IEnumerable<string> messages, string type)
        {
            var builder = new List<string>();
            foreach (var message in messages)
            {
                var line = $"{action.Name}: {type}: {message}";
                builder.Add(line);
            }
            return builder;
        }
    }
}
