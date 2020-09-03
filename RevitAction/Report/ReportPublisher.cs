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
        private readonly Socket _socket;

        private readonly SendPacket _sendPacket;

        public ReportPublisher()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _sendPacket = new SendPacket(_socket);
        }

        public void SendReport(ReportMessage report)
        {
            _sendPacket.Send(report);
        }

        public void Connect(IPAddress address, short port)
        {
            var endPoint = new IPEndPoint(address, port);
            while (_socket.Connected == false)
            {
                try
                {
                    _socket.Connect(endPoint);
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
            _socket.Disconnect(true);
        }
    }
}