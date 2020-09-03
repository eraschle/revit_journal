using RevitAction.Report;
using System;
using System.Net.Sockets;

namespace RevitJournal.Tasks.Report
{
    public class ReportClient
    {
        private readonly ReceivePacket _receive;

        public ReportClient(Socket socket, Func<string, IReportReceiver> func)
        {
            _receive = new ReceivePacket(socket)
            {
                FindFunc = func
            };
        }

        public string TaskId
        {
            get
            {
                try
                {
                    return _receive.Report.TaskId;
                }
                catch (NullReferenceException)
                {
                    return null;
                }
            }
        }

        public bool HasTaskId
        {
            get { return string.IsNullOrEmpty(TaskId) == false; }
        }

        public void StartReceiving()
        {
            _receive.StartReceiving();
        }

        public void StopReceiving()
        {
            _receive.StopReceiving();
        }
    }
}