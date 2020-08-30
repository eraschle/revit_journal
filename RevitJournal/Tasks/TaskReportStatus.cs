using System;
using System.Collections.Generic;

namespace RevitJournal.Tasks
{
    public class TaskReportStatus
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

        public int Status { get; private set; } = Initial;

        public bool Executed
        {
            get
            {
                return (Status & Finish) == Finish ||
                       (Status & Timeout) == Timeout ||
                       (Status & Error) == Error ||
                       (Status & Cancel) == Cancel;
            }
        }

        public void SetStatus(int status)
        {
            if (All.Contains(status) == false)
            {
                throw new ArgumentException("[" + status + "] is not a valid Status");
            }
            if (status == Initial) { return; }

            Status |= status;
        }

        public bool IsWaiting
        {
            get
            {
                return IsReportStatus(Waiting)
                  && IsStarted == false
                  && Executed == false;
            }
        }

        public bool IsStarted
        {
            get
            {
                return IsReportStatus(Started)
                  && Executed == false;
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
