using RevitAction.Report;
using RevitJournal.Tasks.Options;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;

namespace RevitJournal.Report.Network
{
    public class ReportServer
    {
        public const int Port = 8888;

        private Socket ServerSocket { get; set; }

        public Func<Socket, ReportClient> AddClient { get; set; }

        public void StartListening(TaskOptions options)
        {
            if (options is null || ServerSocket != null) { return; }

            try
            {
                ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                var endPoint = new IPEndPoint(IPAddress.Loopback, Port);
                ServerSocket.Bind(endPoint);
                ServerSocket.Listen(options.ParallelProcesses.Value);
                ServerSocket.BeginAccept(AcceptCallback, ServerSocket);
            }
            catch (Exception exception)
            {
                throw new Exception("start listening error" + exception);
            }
        }

        [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public void AcceptCallback(IAsyncResult result)
        {
            if (ServerSocket is null) { return; }
            try
            {
                Debug.WriteLine($"Accept CallBack port:{Port} protocol type: {ProtocolType.Tcp}");
                var socket = ServerSocket.EndAccept(result);
                var client = AddClient.Invoke(socket);
                client.StartReceiving();

                ServerSocket.BeginAccept(AcceptCallback, ServerSocket);
            }
            catch (Exception exception)
            {
                DebugMessage(nameof(AcceptCallback), exception);
            }
        }

        [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        public void StopListening()
        {
            try
            {
                ServerSocket.Close();
                ServerSocket = null;
            }
            catch (Exception exception)
            {
                DebugMessage(nameof(StopListening), exception);
            }
        }

        private void DebugMessage(string methodName, Exception exception)
        {
            Debug.WriteLine($"{nameof(ReportServer)} {methodName}: {exception.Message}");
        }
    }
}