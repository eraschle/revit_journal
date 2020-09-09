using RevitAction.Report.Message;
using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

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
            catch (Exception ex)
            {
                Debug.WriteLine($"StartReceiving: {ex.Message}");
            }
        }

        public string ReceivedResponse()
        {
            if (_socket.Connected == false) { return null; }
            try
            {
                _buffer = new byte[4];
                _socket.Receive(_buffer, _buffer.Length, SocketFlags.None);
                _buffer = new byte[BitConverter.ToInt32(_buffer, 0)];
                _socket.Receive(_buffer, _buffer.Length, SocketFlags.None);
                var response = Encoding.Default.GetString(_buffer);
                return response;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ReceivedResponse: {ex.Message}");
                return null;
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
                var report = MessageUtils.Read(data);

                ReportAction.Invoke(report);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ReceiveCallback: {ex.Message}");
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
            catch (Exception ex)
            {
                Debug.WriteLine($"StopReceiving: {ex.Message}");
            }
        }
    }
}