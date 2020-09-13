using RevitAction.Report;
using RevitJournal.Tasks.Options;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace RevitJournal.Report.Network
{
    public class ReportServer<TResult> where TResult : class, IReportReceiver
    {
        public const int Port = 8888;

        private Socket ServerSocket { get; set; }

        public ClientController<TResult> Clients { get; set; }

        public void StartListening(TaskOptions options)
        {
            if (options is null || ServerSocket != null) { return; }

            try
            {
                ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                var endPoint = new IPEndPoint(IPAddress.Loopback, Port);
                ServerSocket.Bind(endPoint);
                //ServerSocket.Listen(options.Parallel.ParallelProcesses);
                ServerSocket.Listen(100);
                ServerSocket.BeginAccept(AcceptCallback, ServerSocket);
            }
            catch (Exception ex)
            {
                throw new Exception("start listening error" + ex);
            }
        }

        public void AcceptCallback(IAsyncResult result)
        {
            try
            {
                Debug.WriteLine($"Accept CallBack port:{Port} protocol type: {ProtocolType.Tcp}");
                var socket = ServerSocket.EndAccept(result);
                var client = Clients.AddClient(socket);
                client.StartReceiving();

                ServerSocket.BeginAccept(AcceptCallback, ServerSocket);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Base Accept error" + ex);
            }
        }

        public void StopListening()
        {
            try
            {
                if (ServerSocket.Connected == false) { return; }

                ServerSocket.Disconnect(true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("stop listening error: " + ex.Message);
            }
        }
    }
}