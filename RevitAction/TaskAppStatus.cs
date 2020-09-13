using System;
using System.Collections.Generic;

namespace RevitAction
{
    public class TaskAppStatus
    {
        public const int Unknown = 0;
        public const int Initial = 1;
        public const int Waiting = 2;
        public const int Started = 4;
        public const int Running = 8;
        public const int Cancel = 16;
        public const int Error = 32;
        public const int Timeout = 64;
        public const int Finish = 128;

        private static IList<int> allStatus;
        public static IList<int> All
        {
            get
            {
                if (allStatus is null)
                {
                    allStatus = new List<int> { Unknown, Initial, Waiting, Started, Running, Cancel, Error, Timeout, Finish };
                }
                return allStatus;
            }
        }

        public static bool IsStatus(int status)
        {
            return All.Contains(status);
        }

        public int Status { get; private set; } = Initial;

        public void SetStatus(int status)
        {
            if (All.Contains(status) == false)
            {
                throw new ArgumentException("[" + status + "] is not a valid Status");
            }
            if (status == Initial) { return; }

            Status |= status;
        }

        public bool IsExecuted
        {
            get { return IsFinished || IsError || IsCancel || IsTimeout; }
        }


        public bool IsInitial
        {
            get
            {
                return IsReportStatus(Initial)
                    && Status < Waiting;
            }
        }

        public bool IsWaiting
        {
            get
            {
                return IsReportStatus(Waiting)
                    && Status < Started;
            }
        }

        public bool IsStarted
        {
            get
            {
                return IsReportStatus(Started)
                    && Status < Running;
            }
        }

        public bool IsRunning
        {
            get
            {
                return IsReportStatus(Running)
                    && IsExecuted == false;
            }
        }

        public bool IsFinished
        {
            get { return IsReportStatus(Finish); }
        }

        public bool IsTimeout
        {
            get { return IsReportStatus(Timeout); }
        }

        public bool IsCancel
        {
            get { return IsReportStatus(Cancel); }
        }

        public bool IsError
        {
            get { return IsReportStatus(Error); }
        }

        private bool IsReportStatus(int status)
        {
            return (Status & status) == status;
        }
    }
}
