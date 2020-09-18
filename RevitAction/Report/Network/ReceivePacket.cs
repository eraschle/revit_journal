using RevitAction.Report.Message;
using System;
using System.Net.Sockets;
using System.Text;
using Utilities;

namespace RevitAction.Report.Network
{
    public class ReceivePacket
    {

        private readonly Socket _socket;

        public Action<ReportMessage> ReportAction { get; set; }

        private byte[] _buffer;

        public ReceivePacket(Socket socket)
        {
            _socket = socket;
        }

        public void StartReceiving()
        {
            if (_socket.Connected == false) { return; }
            try
            {
                _buffer = new byte[4];
                _socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, ReceiveCallback, null);
            }
            catch (Exception exception)
            {
                DebugUtils.DebugException<ReceivePacket>(exception);
            }
        }

        public string GetReceivedResponse()
        {
            if (_socket.Connected == false) { return string.Empty; }
            try
            {
                _buffer = new byte[4];
                _socket.Receive(_buffer, _buffer.Length, SocketFlags.None);
                _buffer = new byte[BitConverter.ToInt32(_buffer, 0)];
                _socket.Receive(_buffer, _buffer.Length, SocketFlags.None);
                return Encoding.Default.GetString(_buffer);
            }
            catch (Exception exception)
            {
                DebugUtils.DebugException<ReceivePacket>(exception);
                throw;
            }
        }

        public string GetTaskActionsResponse()
        {
            if (_socket.Connected == false) { return null; }
            try
            {
                _buffer = new byte[4];
                _socket.Receive(_buffer, _buffer.Length, SocketFlags.None);
                _buffer = new byte[BitConverter.ToInt32(_buffer, 0)];
                _socket.Receive(_buffer, _buffer.Length, SocketFlags.None);
                return Encoding.Default.GetString(_buffer);
            }
            catch (Exception exception)
            {
                DebugUtils.DebugException<ReceivePacket>(exception);
                throw;
            }
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            if (_socket.Connected == false) { return; }

            try
            {
                // if bytes are less than 1 takes place when a client disconnect from the server.
                if (_socket.EndReceive(result) < 1)
                {
                    StopReceiving();
                    return;
                }
                // Convert the first 4 bytes (int 32) that we received 
                _buffer = new byte[BitConverter.ToInt32(_buffer, 0)];
                _socket.Receive(_buffer, _buffer.Length, SocketFlags.None);

                // Convert the bytes to object
                string data = Encoding.Default.GetString(_buffer);
                var report = MessageUtils.ReadReport(data);

                ReportAction.Invoke(report);
            }
            catch (Exception exception)
            {
                DebugUtils.DebugException<ReceivePacket>(exception);
            }
            finally
            {
                if (_socket.Connected)
                {
                    StartReceiving();
                }
                else
                {
                    StopReceiving();
                }
            }
        }

        public void StopReceiving()
        {
            if (_socket.Connected == false) { return; }
            try
            {
                _socket.Disconnect(true);
            }
            catch (Exception exception)
            {
                DebugUtils.DebugException<ReceivePacket>(exception);
            }
        }
    }
}