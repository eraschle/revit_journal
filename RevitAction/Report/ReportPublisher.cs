using RevitAction.Report.Message;
using RevitAction.Report.Network;
using System;
using System.Net;
using System.Net.Sockets;
using Utilities.System;

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
                DebugUtils.DebugException<ReportPublisher>(exception);
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
                DebugUtils.DebugException<ReportPublisher>(exception);
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
                DebugUtils.DebugException<ReportPublisher>(exception);
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
                DebugUtils.DebugException<ReportPublisher>(exception);
                throw;
            }
        }
    }
}