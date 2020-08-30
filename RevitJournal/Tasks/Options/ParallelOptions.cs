using System;

namespace RevitJournal.Tasks.Options
{
    public class ParallelOptions
    {
        public ParallelOptions()
        {
            ParallelProcesses = MaxProcesses / 2;
        }

        public static int MinProcesses { get; } = 1;

        public static int MaxProcesses { get; } = Environment.ProcessorCount;

        public int ParallelProcesses { get; set; }
    }
}
