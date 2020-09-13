using RevitAction.Report.Message;
using RevitAction.Report.Network;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace RevitAction.Report
{
    public class ReportPublisher : IReportPublisher
    {
        private readonly Socket _socket;

        private readonly SendPacket _sendPacket;
        private readonly ReceivePacket _receivePacket;

        public ReportPublisher()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _sendPacket = new SendPacket(_socket);
            _receivePacket = new ReceivePacket(_socket);
        }

        public void SendReport(ReportMessage report)
        {
            var message = MessageUtils.Write(report);
            _sendPacket.Send(message);
        }

        public Guid GetActionIdResponse()
        {
            try
            {
                var received = _receivePacket.GetReceivedResponse();
                return Guid.TryParse(received, out var actionId) ? actionId : Guid.Empty;
            }
            catch (Exception exception)
            {
                DebugMessage(nameof(GetActionIdResponse), exception);
                return Guid.Empty;
            }
        }

        public ActionManager GetActionManagerResponse()
        {
            try
            {
                var taskActions = _receivePacket.GetTaskActionsResponse();
                return MessageUtils.ReadTasks(taskActions);
            }
            catch (Exception exception)
            {
                DebugMessage(nameof(GetActionManagerResponse), exception);
                return null;
            }
        }

        public void Connect(IPAddress address, short port)
        {
            try
            {
                var endPoint = new IPEndPoint(address, port);
                _socket.Connect(endPoint);
            }
            catch (Exception exception)
            {
                DebugMessage(nameof(Connect), exception);
            }
        }

        public void Disconnect()
        {
            try
            {
                _receivePacket.StopReceiving();
                _socket.Disconnect(true);
            }
            catch (Exception exception)
            {
                DebugMessage(nameof(Disconnect), exception);
                throw;
            }
        }

        private void DebugMessage(string methodName, Exception exception)
        {
            Debug.WriteLine($"{nameof(ReportPublisher)} {methodName}: {exception.Message}");
        }
    }
}