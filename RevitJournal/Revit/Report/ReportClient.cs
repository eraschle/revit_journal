using RevitAction.Reports;
using RevitJournal.Tasks;
using System;
using System.Net.Sockets;

namespace RevitJournal.Revit.Report
{
    public class ReportClient
    {
        private readonly ReceivePacket _receive;

        public ReportClient(Socket socket, Func<string, IReportReceiver> func)
        {
            _receive = new ReceivePacket(socket)
            {
                FindReportFunc = func
            };
        }

        public bool IsId(string id)
        {
            return _receive.Report.GetId() == id;
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