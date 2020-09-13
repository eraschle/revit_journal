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

        public Func<Socket, ReportClient<TResult>> AddClient{ get; set; }

        public void StartListening(TaskOptions options)
        {
            if (options is null || ServerSocket != null) { return; }

            try
            {
                ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                var endPoint = new IPEndPoint(IPAddress.Loopback, Port);
                ServerSocket.Bind(endPoint);
                ServerSocket.Listen(options.Parallel.ParallelProcesses);
                ServerSocket.BeginAccept(AcceptCallback, ServerSocket);
            }
            catch (Exception ex)
            {
                throw new Exception("start listening error" + ex);
            }
        }

        public void AcceptCallback(IAsyncResult result)
        {
            if(ServerSocket is null) { return; }
            try
            {
                Debug.WriteLine($"Accept CallBack port:{Port} protocol type: {ProtocolType.Tcp}");
                var socket = ServerSocket.EndAccept(result);
                var client = AddClient.Invoke(socket);
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
                ServerSocket.Close();
                ServerSocket = null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("stop listening error: " + ex.Message);
            }
        }
    }
}