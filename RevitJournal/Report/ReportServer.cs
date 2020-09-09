using RevitAction.Report;
using RevitJournal.Tasks.Options;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace RevitJournal.Report
{
    public class ReportServer<TResult> where TResult : class, IReportReceiver
    {
        public const int Port = 8888;

        public ClientController<TResult> ClientController { get; set; } = new ClientController<TResult>();

        public Socket Socket { get; private set; }

        public void SetFindReport(Func<string, TResult> findReport)
        {
            ClientController.FindReport = findReport;
        }

        public void SetProgress(IProgress<TResult> progress)
        {
            ClientController.Progress = progress;
        }

        public void DisconnectClient(string taskId)
        {
            ClientController.Remove(taskId);
        }

        public void StartListening(TaskOptions options)
        {
            if (options is null || Socket != null) { return; }

            try
            {
                Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                var endPoint = new IPEndPoint(IPAddress.Loopback, Port);
                Socket.Bind(endPoint);
                Socket.Listen(options.Parallel.ParallelProcesses);
                Socket.BeginAccept(AcceptCallback, Socket);
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
                var socket = Socket.EndAccept(result);
                var client = ClientController.Add(socket);
                client.StartReceiving();

                Socket.BeginAccept(AcceptCallback, Socket);
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
                ClientController.RemoveClients();
                if(Socket.Connected == false) { return; }

                Socket.Disconnect(true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("stop listening error: " + ex.Message);
            }
        }
    }
}