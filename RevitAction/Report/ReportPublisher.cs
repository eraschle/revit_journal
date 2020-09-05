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
            sendPacket.Send(report);
        }

        public bool HasResponsed(ReportMessage report)
        {
            return receivePacket.ReceivedResponse(report);
        }

        public void Connect(IPAddress address, short port)
        {
            var endPoint = new IPEndPoint(address, port);
            while (socket.Connected == false)
            {
                try
                {
                    socket.Connect(endPoint);
                    Console.WriteLine("Try to connect");
                }
                catch
                {
                    Thread.Sleep(1000);
                    Console.WriteLine("Wait a second to try again connect to the server");
                }
            }
            Console.WriteLine("Connected");
        }

        public void Disconnect()
        {
            receivePacket.StopReceiving();
            try { socket.Disconnect(true); }
            catch (Exception) { }
        }
    }
}