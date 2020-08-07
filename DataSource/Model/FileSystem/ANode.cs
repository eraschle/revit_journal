namespace DataSource.Model.FileSystem
{
    public abstract class ANode
    {
        public string Name { get; protected set; } = string.Empty;

        public virtual string FullPath { get; set; }

        public abstract bool Exist { get; }
    }
}
