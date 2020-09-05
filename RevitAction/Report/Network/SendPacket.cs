using RevitAction.Report.Message;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace RevitAction.Report.Network
{
    public class SendPacket
    {
        private readonly Socket _socket;

        public SendPacket(Socket socket)
        {
            _socket = socket;
        }

        public void Send(ReportMessage report)
        {
            var reportData = MessageUtils.Write(report);
            Send(reportData);
        }

        public void Send(string message)
        {
            if (string.IsNullOrEmpty(message)) { return; }
            try
            {
                var fullPacket = new List<byte>();
                fullPacket.AddRange(BitConverter.GetBytes(message.Length));
                fullPacket.AddRange(Encoding.Default.GetBytes(message));
                _socket.Send(fullPacket.ToArray());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}