using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Utilities.System;

namespace RevitAction.Report.Network
{
    public class SendPacket
    {
        private readonly Socket _socket;

        public SendPacket(Socket socket)
        {
            _socket = socket;
        }

        public void Send(string message)
        {
            if (string.IsNullOrEmpty(message)) { return; }

            var fullPacket = new List<byte>();
            fullPacket.AddRange(BitConverter.GetBytes(message.Length));
            fullPacket.AddRange(Encoding.Default.GetBytes(message));
            try
            {
                _socket.Send(fullPacket.ToArray());
            }
            catch (Exception exception)
            {
                DebugUtils.DebugException<SendPacket>(exception);
            }
        }
    }
}