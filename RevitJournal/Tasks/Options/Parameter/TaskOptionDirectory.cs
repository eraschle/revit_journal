using DataSource.Models.FileSystem;
using System;

namespace RevitJournal.Tasks.Options.Parameter
{
    public class TaskOptionDirectory : TaskOption<string>, ITaskOptionDirectory
    {
        private readonly IPathBuilder builder;
        public TaskOptionDirectory(string defaultValue, IPathBuilder pathBuilder) : base(defaultValue)
        {
            builder = pathBuilder ?? throw new ArgumentNullException(nameof(pathBuilder));
        }

        public DirectoryRootNode GetRootNode<TFile>() where TFile : AFileNode, new()
        {
            var value = Value;
            if (string.IsNullOrWhiteSpace(value))
            {
                value = DefaultValue;
            }
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"Not valid path {value}");
            }
            var rootNode = builder.CreateRoot(value);
            builder.CreateFiles<TFile>(rootNode);
            return rootNode;
        }

    }
}
