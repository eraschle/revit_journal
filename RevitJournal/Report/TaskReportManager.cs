using System.Collections.Generic;
using DataSource.Model.FileSystem;
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
        private readonly TaskUnitOfWork UnitOfWork;

        private ReportOptions Options
        {
            get { return UnitOfWork.Options.Report; }
        }

        public IDictionary<ITaskAction, TaskActionReport> ActionReports { get; }
            = new Dictionary<ITaskAction, TaskActionReport>();

        public ITaskAction ErrorAction { get; private set; } = null;

        public string ErrorMessage { get; private set; } = null;

        public bool HasErrorAction
        {
            get { return ErrorAction != null; }
        }

        public TaskReportManager(TaskUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        public bool HasErrorReport()
        {
            return Options.LogError
                && ErrorAction is object;
        }

        public void CreateErrorReport(RecordJournalFile copyRecord, params string[] suffixes)
        {
            if (HasErrorReport() == false) { return; }

            var errorReport = new ReportError(UnitOfWork)
            {
                ErrorReport = ErrorAction,
                ErrorMessage = ErrorMessage
            };
            errorReport.WarningReport.AddRange(GetWarnings());
            CreateReport(errorReport, copyRecord, suffixes);
        }

        public bool HasSuccessReport()
        {
            return (Options.LogSuccess || Options.LogResults);
        }

        public void CreateSuccessReport(RecordJournalFile copyRecord, params string[] suffixes)
        {
            if (HasSuccessReport() == false) { return; }

            var successReport = new ReportSuccess(UnitOfWork);
            successReport.SuccessReport.AddRange(GetSuccess());
            successReport.WarningReport.AddRange(GetWarnings());
            CreateReport(successReport, copyRecord, suffixes);
        }

        private void CreateReport<TReport>(TReport report, RecordJournalFile copyRecord, params string[] suffixes) where TReport : ATaskReport
        {
            report.CopiedRecordJournal = UnitOfWork.GetRenamedJournalFile();
            var jsonFile = UnitOfWork.Task.SourceFile.ChangeExtension<JsonFile>();
            jsonFile.AddSuffixes(suffixes);
            var dataSource = new JsonDataSource<TReport>();
            dataSource.SetFile(jsonFile);
            dataSource.Write(report);
        }

        private IList<string> GetWarnings()
        {
            var warnings = new List<string>();
            foreach (var action in UnitOfWork.Task.Actions)
            {
                if (ActionReports.ContainsKey(action) == false) { continue; }

                if (ActionReports[action].HasWarningReports())
                {
                    warnings.AddRange(GetWarnings(action));
                }
            }
            return warnings;
        }

        private IList<string> GetSuccess()
        {
            var success = new List<string>();
            foreach (var action in UnitOfWork.Task.Actions)
            {
                if (ActionReports.ContainsKey(action) == false) { continue; }

                if (ActionReports[action].HasStatusReports())
                {
                    success.AddRange(GetStatus(action));
                }
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

        public void AddReport(ReportMessage report)
        {
            if (report is null) { return; }

            switch (report.Kind)
            {
                case ReportKind.Message:
                case ReportKind.Warning:
                    if (HasActionReport(report.ActionId, out var actionReport))
                    {
                        actionReport.Add(report);
                    }
                    break;
                case ReportKind.Error:
                    if (HasErrorAction == false
                        && UnitOfWork.Task.HasActionById(report.ActionId, out var errorAction))
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

        private bool HasActionReport(Guid actionId, out TaskActionReport actionReport)
        {
            actionReport = null;
            if (UnitOfWork.Task.HasActionById(actionId, out var action))
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
