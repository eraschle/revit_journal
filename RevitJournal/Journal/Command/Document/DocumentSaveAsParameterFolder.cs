using DataSource.Helper;

namespace RevitJournal.Journal.Command.Document
{
    public class DocumentSaveAsParameterFolder : CommandParameter
    {
        private const string DefaultParameterName = "Save As Folder";

        public DocumentSaveAsParameterFolder(string parameterName = DefaultParameterName)
            : base(parameterName, JournalParameterType.String) { }

        private string _Value = string.Empty;
        public override string Value
        {
            get { return _Value; }
            set
            {
                _Value = value;
                if (_Value.Contains(Constant.Space))
                {
                    _Value = _Value.Replace(Constant.Space, Constant.Underline);
                }
            }
        }
    }
}
