using RevitAction.Report.Message;
using System;
using System.Net.Sockets;
using System.Text;

namespace RevitAction.Report.Network
{
    public class ReceivePacket
    {
        private readonly Socket _socket;

        public Func<string, IReportReceiver> FindReport { get; set; }

        public IReportReceiver Report { get; private set; }

        private byte[] _buffer;

        public ReceivePacket(Socket socket)
        {
            _socket = socket;
        }

        public void StartReceiving()
        {
            try
            {
                _buffer = new byte[4];
                _socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, ReceiveCallback, null);
            }
            finally
            {
            }
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            if (_socket.Connected == false) { return; }

            var receivedBytes = 0;
            try
            {
                // if bytes are less than 1 takes place when a client disconnect from the server.
                // So we run the Disconnect function on the current client
                receivedBytes = _socket.EndReceive(result);
                if (receivedBytes > 1)
                {
                    // Convert the first 4 bytes (int 32) that we received 
                    // and convert it to an Int32 (this is the size for the coming data).
                    _buffer = new byte[BitConverter.ToInt32(_buffer, 0)];
                    _socket.Receive(_buffer, _buffer.Length, SocketFlags.None);

                    // Convert the bytes to object
                    string data = Encoding.Default.GetString(_buffer);
                    var report = MessageUtils.Read(data);
                    if (Report is null)
                    {
                        Report = FindReport.Invoke(report.FilePath);
                    }
                    if (Report != null)
                    {
                        Report.MakeReport(report);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Receiving data: {ex.Message}");
            }
            finally
            {
                // if exeption is throw check if socket is connected 
                // because than you can startreive again else Dissconect
                if (_socket.Connected)
                {
                    StartReceiving();
                }
                else
                {
                    Disconnect();
                }
            }
        }

        public void StopReceiving()
        {
            Disconnect();
        }

        private void Disconnect()
        {

            try
            {
                ClientController.Remove(Report);
                _socket?.Disconnect(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}