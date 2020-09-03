namespace RevitCommand.Families.SharedParameters
{
    public abstract class AParametersAction : ATaskActionCommand
    {
        public SharedFileActionParameter SharedFile { get; private set; }

        public AParametersAction(string name) : base(name)
        {
            SharedFile = new SharedFileActionParameter("File");
            Parameters.Add(SharedFile);
            MakeChanges = true;
        }
    }
}
