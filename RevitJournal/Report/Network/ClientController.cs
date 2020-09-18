using RevitAction.Report;
using RevitAction.Report.Message;
using RevitJournal.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace RevitJournal.Report.Network
{
    public class ClientController
    {
        private readonly List<ReportClient> Clients = new List<ReportClient>();
        private readonly object lockObject = new object();

        public TaskManager TaskManager { get; set; }

        public ReportClient AddClient(Socket socket)
        {
            var client = new ReportClient(socket)
            {
                TaskManager = TaskManager,
                ActionManager = TaskManager.GetActionManager()
            };

            lock (lockObject)
            {
                Clients.Add(client);
            }
            return client;
        }

        private void Remove(ReportClient client)
        {
            if (client is null) { return; }

            lock (lockObject)
            {
                Clients.Remove(client);
                client.StopReceiving();
            }
        }

        public void Remove(string taskId)
        {
            if (string.IsNullOrWhiteSpace(taskId)) { return; }

            ReportClient client = null;
            lock (lockObject)
            {
                client = Clients.FirstOrDefault(clt => clt.ClientId == taskId);
            }
            Remove(client);
        }

        public void RemoveClients()
        {
            foreach (var clientId in new List<ReportClient>(Clients))
            {
                Remove(clientId);
            }
        }
    }
}