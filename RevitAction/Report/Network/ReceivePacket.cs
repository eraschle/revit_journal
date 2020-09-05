using RevitAction.Report.Message;
using System;
using System.Net.Sockets;
using System.Text;

namespace RevitAction.Report.Network
{
    public class ReceivePacket
    {
        private readonly Socket _socket;

        private readonly SendPacket sendPacket;

        public Func<string, IReportReceiver> FindReport { get; set; }

        public IReportReceiver Report { get; private set; }

        private byte[] _buffer;

        public ReceivePacket(Socket socket)
        {
            _socket = socket;
            sendPacket = new SendPacket(socket);
        }

        public void StartReceiving()
        {
            if(_socket.Connected == false) { return; }
            try
            {
                _buffer = new byte[4];
                _socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, ReceiveCallback, null);
            }
            catch
            {
            }
        }

        public bool ReceivedResponse(ReportMessage report)
        {
            if(_socket.Connected == false) { return false; }
            try
            {
                _buffer = new byte[4];
                _socket.Receive(_buffer, _buffer.Length, SocketFlags.None);
                _buffer = new byte[BitConverter.ToInt32(_buffer, 0)];
                _socket.Receive(_buffer, _buffer.Length, SocketFlags.None);
                var response = Encoding.Default.GetString(_buffer);
                return string.IsNullOrEmpty(response) == false
                    && response.Equals(report.Message);
            }
            catch
            {
                return false;
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

                    if (report.Kind == ReportKind.Open)
                    {
                        Report = FindReport.Invoke(report.Message);
                        sendPacket.Send(Report?.TaskId);
                    }
                    if (Report != null)
                    {
                        Report.MakeReport(report);
                    }
                    if (report.Kind == ReportKind.Close)
                    {
                        sendPacket.Send(report.Message);
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
            if(_socket.Connected == false) { return; }
            try
            {
                _socket?.Disconnect(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}