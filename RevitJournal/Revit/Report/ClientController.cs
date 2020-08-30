using RevitJournal.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace RevitJournal.Revit.Report
{
    public static class ClientController
    {
        private static readonly List<ReportClient> Clients = new List<ReportClient>();

        private static readonly object lockObject = new object();

        public static ReportClient Add(Socket socket, TaskManager manager)
        {
            if (manager is null) { throw new ArgumentNullException(nameof(manager)); }

            var client = new ReportClient(socket, manager.ByFamilyPath);
            lock (lockObject)
            {
                Clients.Add(client);
            }
            return client;
        }

        public static void RemoveClient(string filePath)
        {
            var client = Clients.FirstOrDefault(clt => clt.IsId(filePath));
            if (client is null) { return; }
            lock (lockObject)
            {
                Clients.Remove(client);
            }
            client.StopReceiving();
        }
    }
}