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
        public const int Open = 8;
        public const int Run = 16;
        public const int Cancel = 32;
        public const int Error = 64;
        public const int Timeout = 128;
        public const int Finish = 256;
        public const int CleanUp = 512;

        private static IList<int> allStatus;
        public static IList<int> All
        {
            get
            {
                if (allStatus is null)
                {
                    allStatus = new List<int> { Unknown, Initial, Waiting, Started, Open, Run, Cancel, Error, Timeout, Finish, CleanUp };
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

        public bool IsRunning
        {
            get { return (IsStarted || IsOpen || IsRun) && Executed == false; }
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

        public bool IsInitial
        {
            get
            {
                return IsReportStatus(Initial)
                  && IsRunning == false
                  && Executed == false
                  && IsCleanUp == false;
            }
        }

        public bool IsWaiting
        {
            get
            {
                return IsReportStatus(Waiting)
                  && IsRunning == false
                  && Executed == false
                  && IsCleanUp == false;
            }
        }

        public bool IsStarted
        {
            get
            {
                return IsReportStatus(Started)
                  && IsOpen == false
                  && IsCleanUp == false;
            }
        }

        public bool IsOpen
        {
            get
            {
                return IsReportStatus(Open)
                    && IsRun == false
                  && IsCleanUp == false;
            }
        }

        public bool IsRun
        {
            get
            {
                return IsReportStatus(Run)
                    && Executed == false
                    && IsCleanUp == false;
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

        public bool IsCleanUp
        {
            get { return IsReportStatus(CleanUp); }
        }

        private bool IsReportStatus(int status)
        {
            return (Status & status) == status;
        }
    }
}
