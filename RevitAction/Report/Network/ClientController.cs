using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace RevitAction.Report.Network
{
    public static class ClientController
    {
        private static readonly List<ReportClient> Clients = new List<ReportClient>();

        private static readonly object lockObject = new object();

        public static ReportClient Add(Socket socket, Func<string, IReportReceiver> func)
        {
            var client = new ReportClient(socket, func);
            lock (lockObject)
            {
                Clients.Add(client);
            }
            return client;
        }

        private static void Remove(ReportClient client)
        {
            if (client is null) { return; }
            lock (lockObject)
            {
                Clients.Remove(client);
                client.Disconnect();
            }
        }

        public static void Remove(IReportReceiver report)
        {
            if (report is null) { return; }

            var client = Clients.FirstOrDefault(clt => clt.HasTaskId && clt.TaskId == report.TaskId);
            lock (lockObject)
            {
                Remove(client);
            }
        }

        public static void RemoveClients()
        {
            foreach (var clientId in new List<ReportClient>(Clients))
            {
                Remove(clientId);
            }
        }
    }
}