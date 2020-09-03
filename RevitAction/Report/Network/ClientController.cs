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

        public static void Remove(string filePath)
        {
            var client = Clients.FirstOrDefault(clt => clt.HasTaskId && clt.TaskId == filePath);
            if (client is null) { return; }

            lock (lockObject)
            {
                Clients.Remove(client);
            }
            client.StopReceiving();
        }

        public static void Remove(IReportReceiver report)
        {
            if(report is null) { return; }

            Remove(report.TaskId);
        }

        public static void RemoveClients()
        {
            var clientIds = Clients.Where(client => client.HasTaskId)
                                   .Select(client => client.TaskId);
            foreach (var clientId in clientIds)
            {
                Remove(clientId);
            }
        }
    }
}