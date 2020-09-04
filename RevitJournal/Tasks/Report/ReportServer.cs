using RevitAction.Report;
using RevitAction.Report.Network;
using RevitJournal.Tasks.Options;
using System;
using System.Net;
using System.Net.Sockets;

namespace RevitJournal.Tasks.Report
{
    public class ReportServer
    {
        public const int Port = 8888;

        public Socket Socket { get; private set; }

        public Func<string, IReportReceiver> FindFunc { get; set; }

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
                Console.WriteLine($"Accept CallBack port:{Port} protocol type: {ProtocolType.Tcp}");
                var socket = Socket.EndAccept(result);
                var client = ClientController.Add(socket, FindFunc);
                client.StartReceiving();

                Socket.BeginAccept(AcceptCallback, Socket);
            }
            catch (Exception ex)
            {
                throw new Exception("Base Accept error" + ex);
            }
        }

        public void StopListening()
        {
            try
            {
                ClientController.RemoveClients();
                Socket.Disconnect(true);
            }
            catch (Exception ex)
            {
                throw new Exception("stop listening error" + ex);
            }
            finally
            {
                FindFunc = null;
            }
        }
    }
}