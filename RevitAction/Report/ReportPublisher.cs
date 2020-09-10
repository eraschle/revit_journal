using RevitAction.Report.Message;
using RevitAction.Report.Network;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace RevitAction.Report
{
    public class ReportPublisher : IReportPublisher
    {
        private readonly Socket socket;

        private readonly SendPacket sendPacket;
        private readonly ReceivePacket receivePacket;

        public ReportPublisher()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sendPacket = new SendPacket(socket);
            receivePacket = new ReceivePacket(socket);
        }

        public void SendReport(ReportMessage report)
        {
            var message = MessageUtils.Write(report);
            sendPacket.Send(message);
        }

        public Guid GetResponsed()
        {
            var received = receivePacket.ReceivedResponse();
            if (Guid.TryParse(received, out var newActionId))
            {
                return newActionId;
            }
            return Guid.Empty;
        }

        public void Connect(IPAddress address, short port)
        {
            var endPoint = new IPEndPoint(address, port);
            socket.Connect(endPoint);
        }

        public void Disconnect()
        {
            try
            {
                receivePacket.StopReceiving();
                socket.Disconnect(true);
            }
            catch (Exception) { }
        }
    }
}