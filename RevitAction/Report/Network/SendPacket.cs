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
            try
            {
                var reportData = MessageUtils.Write(report);
                var fullPacket = new List<byte>();
                fullPacket.AddRange(BitConverter.GetBytes(reportData.Length));
                fullPacket.AddRange(Encoding.Default.GetBytes(reportData));
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