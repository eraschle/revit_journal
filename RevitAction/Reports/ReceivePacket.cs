using RevitAction.Reports.Messages;
using System;
using System.Net.Sockets;
using Utilities.System;

namespace RevitAction.Reports
{
    public class ReceivePacket
    {
        private readonly Socket _socket;

        public Func<string, IReportReceiver> FindReportFunc { get; set; }

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
            finally { }
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                // if bytes are less than 1 takes place when a client disconnect from the server.
                // So we run the Disconnect function on the current client
                if (_socket.EndReceive(result) > 1)
                {
                    // Convert the first 4 bytes (int 32) that we received 
                    // and convert it to an Int32 (this is the size for the coming data).
                    _buffer = new byte[BitConverter.ToInt32(_buffer, 0)];
                    _socket.Receive(_buffer, _buffer.Length, SocketFlags.None);

                    // Convert the bytes to object
                    var data = ByteUtils.ToObject<ReportData>(_buffer);
                    var report = data.Message;
                    var kind = report.Kind;
                    if (Report is null)
                    {
                        if (data.Message.Kind != ReportKind.Open)
                        {
                            throw new ArgumentException("First Report must be an Open Report");
                        }
                        Report = FindReportFunc.Invoke(data.FilePath);
                    }
                    Report.MakeReport(data);
                }
                else
                {
                    Disconnect();
                }
            }
            finally
            {
                // if exeption is throw check if socket is connected 
                // because than you can startreive again else Dissconect
                if (_socket.Connected)
                {
                    StartReceiving();
                }
            }
        }

        public void StopReceiving()
        {
            Disconnect();
        }

        private void Disconnect()
        {

            try { _socket.Disconnect(true); }
            finally { }
        }
    }
}