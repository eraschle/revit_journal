using System;
using System.Net.Sockets;
using Utilities.System;

namespace RevitAction.Reports
{
    public class SendPacket
    {
        private readonly Socket _socket;

        public SendPacket(Socket socket)
        {
            _socket = socket;
        }

        public void Send(ReportData report)
        {
            try
            {
                var data = ByteUtils.ToArray(report);
                _socket.Send(data);
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }
    }
}