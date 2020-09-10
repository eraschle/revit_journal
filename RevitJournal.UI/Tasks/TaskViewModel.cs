﻿using RevitAction;
using RevitJournal.Tasks;
using RevitJournalUI.Helper;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
using Utilities;

namespace RevitJournalUI.Tasks
{
    public class TaskViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private TimeSpan timerInterval;

        private TimeSpan executionTime = TimeSpan.Zero;

        internal TaskUnitOfWork TaskUoW { get; set; }

        #region Task

        public string TaskName
        {
            get { return TaskUoW.Task.Name; }
        }

        public TaskAppStatus TaskStatus
        {
            get { return TaskUoW.Status; }
            set { OnPropertyChanged(nameof(TaskStatus)); }
        }

        private string journalTask = string.Empty;
        public string JournalTask
        {
            get { return journalTask; }
            set
            {
                if (StringUtils.Equals(journalTask, value)) { return; }

                journalTask = value;
                OnPropertyChanged(nameof(JournalTask));
            }
        }

        private string journalRecorde = string.Empty;
        public string JournalRecorde
        {
            get { return journalRecorde; }
            set
            {
                if (StringUtils.Equals(journalRecorde, value)) { return; }

                journalRecorde = value;
                OnPropertyChanged(nameof(JournalRecorde));
            }
        }

        internal void AddTimer(DispatcherTimer timer)
        {
            timerInterval = timer.Interval;
            timer.Tick += DispatcherTimer_Tick;
        }

        internal void RemoveTimer(DispatcherTimer timer)
        {
            timer.Tick -= DispatcherTimer_Tick;
        }

        internal void AddProgessEvent(Progress<TaskUnitOfWork> progress)
        {
            if (progress is null) { return; }

            progress.ProgressChanged += Progress_ProgressChanged;
        }

        internal void RemoveProgessEvent(Progress<TaskUnitOfWork> progress)
        {
            if (progress is null) { return; }

            progress.ProgressChanged -= Progress_ProgressChanged;
        }

        private void Progress_ProgressChanged(object sender, TaskUnitOfWork task)
        {
            if (task is null || task != TaskUoW) { return; }

            TaskStatus = TaskUoW.Status;

            JournalTask = TaskUoW.TaskJournal.Name;
            JournalRecorde = TaskUoW.RecordeJournal.Name;

            CurrentAction = TaskUoW.CurrentAction.Name;
            ExecutedActions = TaskUoW.ExecutedActions;
            ExecutedActionsText = $"{executedActions} / {ActionsCount}";
            if (TaskUoW.Status.IsError)
            {
                ErrorText = TaskUoW.ErrorAction.Name;
                ErrorTextToolTip = TaskUoW.ErrorMessage;
            }
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (TaskUoW.Status.IsRunning == false) { return; }

            executionTime += timerInterval;
            TaskTime = $"{TimeSpanHelper.GetMinuteAndSeconds(executionTime)} / {TimeSpanHelper.GetMinuteAndSeconds(TaskUoW.Options.Arguments.Timeout)}";
        }

        #endregion

        #region Execute

        private double executedActions = 0;
        public double ExecutedActions
        {
            get { return executedActions; }
            set
            {
                if (executedActions == value) { return; }

                executedActions = value;
                OnPropertyChanged(nameof(ExecutedActions));
            }
        }

        private string executedActionsText = string.Empty;
        public string ExecutedActionsText
        {
            get { return executedActionsText; }
            set
            {
                if (StringUtils.Equals(executedActionsText, value)) { return; }

                executedActionsText = value;
                OnPropertyChanged(nameof(ExecutedActionsText));
            }
        }

        public double ActionsCount
        {
            get { return TaskUoW.Task.Actions.Count; }
        }

        private string taskTime = string.Empty;
        public string TaskTime
        {
            get { return taskTime; }
            set
            {
                if (StringUtils.Equals(taskTime, value)) { return; }

                taskTime = value;
                OnPropertyChanged(nameof(TaskTime));
            }
        }

        private string currentAction = string.Empty;
        public string CurrentAction
        {
            get { return currentAction; }
            set
            {
                if (StringUtils.Equals(currentAction, value)) { return; }

                currentAction = value;
                OnPropertyChanged(nameof(CurrentAction));
            }
        }

        #endregion

        #region Result

        private string errorText = string.Empty;
        public string ErrorText
        {
            get { return errorText; }
            set
            {
                if (StringUtils.Equals(errorText, value)) { return; }

                errorText = value;
                OnPropertyChanged(nameof(ErrorText));
            }
        }

        private string errorTextToolTip = string.Empty;
        public string ErrorTextToolTip
        {
            get { return errorTextToolTip; }
            set
            {
                if (StringUtils.Equals(errorTextToolTip, value)) { return; }

                errorTextToolTip = value;
                OnPropertyChanged(nameof(ErrorTextToolTip));
            }
        }

        #endregion

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}