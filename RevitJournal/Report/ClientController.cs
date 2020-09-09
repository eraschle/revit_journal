using RevitAction.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace RevitJournal.Report
{
    public class ClientController<TResult> where TResult : class, IReportReceiver
    {
        private readonly List<ReportClient<TResult>> Clients = new List<ReportClient<TResult>>();
        private readonly object lockObject = new object();

        public Func<string, TResult> FindReport { get; set; }

        public IProgress<TResult> Progress { get; set; }

        public ReportClient<TResult> Add(Socket socket)
        {
            var client = new ReportClient<TResult>(socket)
            {
                FindReport = FindReport,
                Progress = Progress,
            };

            lock (lockObject)
            {
                Clients.Add(client);
            }
            return client;
        }

        private void Remove(ReportClient<TResult> client)
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

            ReportClient<TResult> client = null;
            lock (lockObject)
            {
                client = Clients.FirstOrDefault(clt => clt.ClientId == taskId);
            }
            Remove(client);
        }

        public void RemoveClients()
        {
            foreach (var clientId in new List<ReportClient<TResult>>(Clients))
            {
                Remove(clientId);
            }
        }
    }
}