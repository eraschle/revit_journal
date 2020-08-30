using RevitJournal.Tasks;
using System;
using System.Net;
using System.Net.Sockets;

namespace RevitJournal.Revit.Report
{
    public class ReportServer
    {
        public const short Port = 8888;

        public Socket Socket { get; private set; }

        public TaskManager Manager { get; set; }

        public ReportServer()
        {
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void StartListening(TaskOptions options)
        {
            if (options is null) { return; }

            try
            {
                if (Socket is null)
                {
                    Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                }
                Socket.Bind(new IPEndPoint(IPAddress.Any, Port));
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
                var client = ClientController.Add(socket, Manager);
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
                Socket.Disconnect(true);
            }
            catch (Exception ex)
            {
                throw new Exception("stop listening error" + ex);
            }
            finally
            {
                Manager = null;
            }
        }
    }
}