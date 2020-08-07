using DataSource.Helper;
using System;

namespace RevitJournal.Journal.Command.Document
{
    public class DocumentSaveAsParameterFile : CommandParameter
    {
        private const string DefaultParameterName = "File suffix";

        public DocumentSaveAsParameterFile(string parameterName = DefaultParameterName)
            : base(parameterName, JournalParameterType.String, true) { }

        private string _Value = string.Empty;
        public override string Value
        {
            get { return _Value; }
            set
            {
                _Value = value;
                if (string.IsNullOrWhiteSpace(_Value) == false
                    && _Value.StartsWith(Constant.Underline, StringComparison.CurrentCulture) == false)
                {
                    _Value = string.Concat(Constant.Underline, _Value);
                }
            }
        }
    }
}
