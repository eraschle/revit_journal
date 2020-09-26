using DataSource.Model.FileSystem;
using System;

namespace DataSource.DataSource
{
    public abstract class AFileDataSource<TModel, TFile> where TModel : class where TFile : AFileNode
    {
        public TFile FileNode { get; private set; }

        public abstract TModel Read();

        public abstract void Write(TModel model);

        public virtual void SetFile(TFile fileNode)
        {
            FileNode = fileNode ?? throw new ArgumentNullException(nameof(fileNode));
        }
    }
}
